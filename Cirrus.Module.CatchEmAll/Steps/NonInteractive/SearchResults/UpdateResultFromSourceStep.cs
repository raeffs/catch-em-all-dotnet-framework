using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using HtmlAgilityPack;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchResults
{
    [Step]
    public interface IUpdateResultFromSourceStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        EdgeDefinition ResultNotFound { get; }

        [Input]
        long Id { get; set; }

        [Input]
        bool IsUserInitiated { get; set; }
    }

    internal class UpdateResultFromSourceStep : IUpdateResultFromSourceStep
    {
        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public EdgeDefinition ResultNotFound { get; } = new EdgeDefinition();

        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public long Id { get; set; }

        public bool IsUserInitiated { get; set; }

        internal UpdateResultFromSourceStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            Context.GetCurrentLogger().Debug("Updating result with id {ResultId}.", this.Id);

            try
            {
                var executionId = await this.StartExecutionAsync();

                if (!executionId.HasValue)
                {
                    return this.ResultNotFound;
                }

                try
                {
                    await this.ExecuteAsync();
                    await this.EndExecutionAsync(executionId.Value);
                }
                catch (Exception e)
                {
                    Context.GetCurrentLogger().Error(CLMTIDs.CouldNotUpdateResult, e, "Could not update result with id {ResultId}!", this.Id);
                    await this.EndExecutionAsync(executionId.Value, e);
                }
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Fatal(CLMTIDs.UnexpectedErrorWhileUpdatingResult, e, "Unexpected error while updating result with id {ResultId}!", this.Id);
            }

            try
            {
                await this.DeleteOldExecutionsAsync();
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Warning(CLMTIDs.CouldNotDeleteOldResultExecutions, e, "Could not delete old query executions!");
            }

            return this.Next;
        }

        private async Task<long?> StartExecutionAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var resultFound = await context.NoTracking<DAL.Entities.SearchResult>()
                    .AnyAsync(e => e.Id == this.Id);

                if (!resultFound)
                {
                    return null;
                }

                var execution = await context.AddAsync(new DAL.Entities.ResultExecution
                {
                    IsUserInitiated = this.IsUserInitiated,
                    ResultId = this.Id,
                    Start = DateTime.Now
                });

                await context.SaveChangesAsync();

                return execution.Id;
            }
        }

        private async Task ExecuteAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var result = await context.Tracking<DAL.Entities.SearchResult>()
                    .SingleAsync(e => e.Id == this.Id);

                var page = await this.LoadItemPageAsync(result);

                result.Ends = page.Ends;
                result.Closed = page.Closed;
                result.Sold = page.Sold;
                result.BidPrice = page.BidPrice ?? result.BidPrice;
                result.PurchasePrice = page.PurchasePrice ?? result.PurchasePrice;
                result.FinalPrice = page.FinalPrice ?? page.FinalPrice;

                await context.SaveChangesAsync();
            }
        }

        private async Task EndExecutionAsync(long executionId, Exception exception = null)
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

        private async Task<Models.SearchResultPage> LoadItemPageAsync(DAL.Entities.SearchResult entity)
        {
            var url = Helper.SearchResultTransformations.EntityToUrl(entity);
            var response = await WebRequest.Create(url).GetResponseAsync();
            var document = new HtmlDocument();
            document.Load(response.GetResponseStream());
            return new Models.SearchResultPage(document.DocumentNode);
        }

        private async Task DeleteOldExecutionsAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var deleteFrom = DateTime.Now.AddDays(-8);

                var ids = await context.NoTracking<DAL.Entities.ResultExecution>()
                    .Where(e => e.Start <= deleteFrom)
                    .Select(e => e.Id)
                    .Take(100)
                    .ToListAsync();

                foreach (var id in ids)
                {
                    await context.DeleteAsync<DAL.Entities.ResultExecution>(id);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
