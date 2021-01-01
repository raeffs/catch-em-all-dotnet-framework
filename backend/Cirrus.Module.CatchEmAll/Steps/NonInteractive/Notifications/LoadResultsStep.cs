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
    public interface ILoadResultsStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        EdgeDefinition NoWorkToDo { get; }

        [Input]
        long QueryId { get; set; }

        [Output]
        ICollection<long> ResultIds { get; set; }
    }

    internal class LoadResultsStep : ILoadResultsStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public EdgeDefinition NoWorkToDo { get; } = new EdgeDefinition();

        public long QueryId { get; set; }

        public ICollection<long> ResultIds { get; set; } = new List<long>();

        public LoadResultsStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            try
            {
                using (var context = this.dataAccess.GetContext())
                {
                    this.ResultIds = await context.NoTracking<DAL.Entities.SearchResult>()
                        .Where(e => e.QueryId == this.QueryId && !e.Notified && !e.Hidden)
                        .Select(e => e.Id)
                        .ToListAsync();
                }

                if (this.ResultIds.Count == 0)
                {
                    return this.NoWorkToDo;
                }

                return this.Next;
            }
            catch (Exception e)
            {
                Context.GetCurrentLogger().Error(CLMTIDs.CouldNotLoadResults, e, "Could not load result ids for query with id {QueryId}!", this.QueryId);
                return this.NoWorkToDo;
            }
        }
    }
}
