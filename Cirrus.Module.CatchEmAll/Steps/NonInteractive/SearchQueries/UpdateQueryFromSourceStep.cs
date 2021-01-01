using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using HtmlAgilityPack;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchQueries
{
    [Step]
    public interface IUpdateQueryFromSourceStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        EdgeDefinition QueryNotFound { get; }

        [Input]
        long Id { get; set; }

        [Input]
        bool IsUserInitiated { get; set; }
    }

    internal class UpdateQueryFromSourceStep : IUpdateQueryFromSourceStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public EdgeDefinition QueryNotFound { get; } = new EdgeDefinition();

        public long Id { get; set; }

        public bool IsUserInitiated { get; set; }

        internal UpdateQueryFromSourceStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            Context.GetCurrentLogger().Debug("Updating query with id {QueryId}.", this.Id);

            try
            {
                var executionId = await this.StartExecutionAsync();

                if (!executionId.HasValue)
                {
                    return this.QueryNotFound;
                }

                try
                {
                    var exception = await this.ExecuteAsync();
                    await this.EndExecutionAsync(executionId.Value, exception);
                }
                catch (Exception e)
                {
                    Context.GetCurrentLogger().Error(CLMTIDs.CouldNotUpdateQuery, e, "Could not update query with id {QueryId}!", this.Id);
                    await this.EndExecutionAsync(executionId.Value, e);
                }
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Fatal(CLMTIDs.UnexpectedErrorWhileUpdatingQuery, e, "Unexpected error while updating query with id {QueryId}!", this.Id);
            }

            try
            {
                await this.DeleteOldExecutionsAsync();
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Warning(CLMTIDs.CouldNotDeleteOldQueryExecutions, e, "Could not delete old query executions!");
            }

            return this.Next;
        }

        private async Task<long?> StartExecutionAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var queryFound = await context.NoTracking<DAL.Entities.SearchQuery>()
                    .AnyAsync(e => e.Id == this.Id);

                if (!queryFound)
                {
                    return null;
                }

                var execution = await context.AddAsync(new DAL.Entities.QueryExecution
                {
                    IsUserInitiated = this.IsUserInitiated,
                    QueryId = this.Id,
                    Start = DateTime.Now
                });

                await context.SaveChangesAsync();

                return execution.Id;
            }
        }

        private async Task<Exception> ExecuteAsync()
        {
            var exceptions = new List<Exception>();
            var page = default(Models.SearchQueryPage);

            using (var context = this.dataAccess.GetContext())
            {
                var query = await context.NoTracking<DAL.Entities.SearchQuery>()
                    .SingleAsync(e => e.Id == this.Id);

                page = await this.LoadQueryPageAsync(query);
            }

            foreach (var item in page.Results)
            {
                var executionId = default(long);
                try
                {
                    var resultId = await this.EnsureResultExistsAsync(item.ExternalId);
                    executionId = await this.StartResultExecutionAsync(resultId);
                    try
                    {
                        await this.ExecuteResultAsync(resultId, item);
                        await this.EndResultExecutionAsync(executionId);
                    }
                    catch (Exception e)
                    {
                        Context.GetCurrentLogger().Error(CLMTIDs.CouldNotUpdateResultOfQuery, e, "Could not update result with id {ResultId} of query with id {QueryId}!", resultId, this.Id);
                        await this.EndResultExecutionAsync(executionId, e);
                        exceptions.Add(e);
                    }
                }
                catch (Exception e)
                {
                    Context.GetCurrentLogger().Fatal(CLMTIDs.UnexpectedErrorWhileUpdatingResultOfQuery, e, "Unexpected error while updating a result of query with id {QueryId}!", this.Id);
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 0)
            {
                return new AggregateException(exceptions);
            }

            return null;
        }

        private async Task<long> EnsureResultExistsAsync(long externalId)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var existing = await context.NoTracking<DAL.Entities.SearchResult>()
                    .Select(e => new
                    {
                        e.Id,
                        e.ExternalId,
                        e.QueryId
                    })
                    .SingleOrDefaultAsync(e => e.ExternalId == externalId && e.QueryId == this.Id);

                if (existing != null)
                {
                    return existing.Id;
                }

                var added = await context.AddAsync(new DAL.Entities.SearchResult
                {
                    QueryId = this.Id,
                    ExternalId = externalId,
                    Name = externalId.ToString(),
                    New = true
                });

                await context.SaveChangesAsync();

                return added.Id;
            }
        }

        private async Task<long> StartResultExecutionAsync(long resultId)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var execution = await context.AddAsync(new DAL.Entities.ResultExecution
                {
                    IsUserInitiated = this.IsUserInitiated,
                    ResultId = resultId,
                    Start = DateTime.Now
                });

                await context.SaveChangesAsync();

                return execution.Id;
            }
        }

        private async Task ExecuteResultAsync(long resultId, Models.SearchResultItem item)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var result = await context.Tracking<DAL.Entities.SearchResult>()
                    .Include(e => e.Query)
                    .SingleAsync(e => e.Id == resultId);

                result.Name = item.Name;
                result.Description = item.Description;
                result.Ends = item.Ends ?? result.Ends;
                result.BidPrice = item.BidPrice;
                result.PurchasePrice = item.PurchasePrice;
                result.Closed = false;
                result.Sold = false;
                result.FinalPrice = null;

                if (!result.Hidden && result.Query.AutoFilterDeletedDuplicates)
                {
                    var hasDeletedDuplicates = await context.NoTracking<DAL.Entities.SearchResult>()
                        .AnyAsync(e => e.QueryId == this.Id
                            && e.Hidden
                            && e.Name == result.Name
                            && e.Description == result.Description);

                    result.Hidden = result.Hidden || hasDeletedDuplicates;

                    if (hasDeletedDuplicates)
                    {
                        // todo lower level
                        Context.GetCurrentLogger().Information("Deleted duplicates found for result with id {ResultId}", resultId);
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        private async Task EndResultExecutionAsync(long executionId, Exception exception = null)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var execution = await context.Tracking<DAL.Entities.ResultExecution>()
                    .SingleAsync(e => e.Id == executionId);

                execution.End = DateTime.Now;
                execution.Successful = exception == null;
                execution.Message = exception != null ? exception.ToString() : null;

                await context.SaveChangesAsync();
            }
        }

        private async Task EndExecutionAsync(long executionId, Exception exception = null)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var execution = await context.Tracking<DAL.Entities.QueryExecution>()
                    .SingleAsync(e => e.Id == executionId);

                execution.End = DateTime.Now;
                execution.Successful = exception == null;
                execution.Message = exception != null ? exception.ToString() : null;

                await context.SaveChangesAsync();
            }
        }

        private async Task<Models.SearchQueryPage> LoadQueryPageAsync(DAL.Entities.SearchQuery entity)
        {
            var url = Helper.SearchQueryTransformations.EntityToUrl(entity);
            var response = await WebRequest.Create(url).GetResponseAsync();
            var document = new HtmlDocument();
            document.Load(response.GetResponseStream());
            return new Models.SearchQueryPage(document.DocumentNode);
        }

        private async Task DeleteOldExecutionsAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var deleteFrom = DateTime.Now.AddDays(-8);

                var ids = await context.NoTracking<DAL.Entities.QueryExecution>()
                    .Where(e => e.Start <= deleteFrom)
                    .Select(e => e.Id)
                    .Take(100)
                    .ToListAsync();

                foreach (var id in ids)
                {
                    await context.DeleteAsync<DAL.Entities.QueryExecution>(id);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
