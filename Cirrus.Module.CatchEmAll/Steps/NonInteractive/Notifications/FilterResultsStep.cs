using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.Notifications
{
    [Step]
    public interface IFilterResultsStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        [Input]
        long QueryId { get; set; }

        [Input]
        ICollection<long> ResultIds { get; set; }

        [Output]
        ICollection<long> FilteredResultIds { get; set; }
    }

    internal class FilterResultsStep : IFilterResultsStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public long QueryId { get; set; }

        public ICollection<long> ResultIds { get; set; } = new List<long>();

        public ICollection<long> FilteredResultIds { get; set; } = new List<long>();

        public FilterResultsStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            try
            {
                using (var context = this.dataAccess.GetContext())
                {
                    var unfiltered = this.ResultIds?.ToList() ?? new List<long>();

                    var querySettings = await context.NoTracking<DAL.Entities.SearchQuery>()
                        .Where(e => e.Id == this.QueryId)
                        .Select(e => new
                        {
                            e.NotificationMode,
                            e.DesiredPrice
                        })
                        .SingleOrDefaultAsync();

                    this.FilteredResultIds = this.ResultIds;

                    if (querySettings.NotificationMode == DAL.Entities.NotificationMode.All)
                    {
                        return this.Next;
                    }

                    if (querySettings.NotificationMode == DAL.Entities.NotificationMode.OnlyBelowDesired)
                    {
                        if (!querySettings.DesiredPrice.HasValue)
                        {
                            return this.Next;
                        }

                        this.FilteredResultIds = await context.NoTracking<DAL.Entities.SearchResult>()
                            .Where(e => unfiltered.Contains(e.Id)
                                && ((e.BidPrice.HasValue && e.BidPrice.Value <= querySettings.DesiredPrice.Value)
                                || (e.PurchasePrice.HasValue && e.PurchasePrice.Value <= querySettings.DesiredPrice.Value)))
                            .Select(e => e.Id)
                            .ToListAsync();

                        return this.Next;
                    }

                    if (querySettings.NotificationMode == DAL.Entities.NotificationMode.OnlyBelowAverage)
                    {
                        var averagePrice = await context.NoTracking<DAL.Entities.SearchResult>()
                            .Where(e => e.QueryId == this.QueryId && !e.Hidden && e.Sold)
                            .AverageAsync(e => e.FinalPrice);

                        if (!averagePrice.HasValue)
                        {
                            return this.Next;
                        }

                        this.FilteredResultIds = await context.NoTracking<DAL.Entities.SearchResult>()
                            .Where(e => unfiltered.Contains(e.Id)
                                && ((e.BidPrice.HasValue && e.BidPrice.Value <= averagePrice.Value)
                                || (e.PurchasePrice.HasValue && e.PurchasePrice.Value <= averagePrice.Value)))
                            .Select(e => e.Id)
                            .ToListAsync();

                        return this.Next;
                    }
                }

                return this.Next;
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Error(CLMTIDs.CouldNotLoadResults, e, "Could not filter result ids for query with id {QueryId}!", this.QueryId);
                return this.Next;
            }
        }
    }
}
