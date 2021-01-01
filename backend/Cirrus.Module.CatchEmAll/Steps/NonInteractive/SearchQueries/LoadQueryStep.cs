using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchQueries
{
    [Step]
    public interface ILoadQueryStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        EdgeDefinition NoWorkToDo { get; }

        [Input]
        Models.ScheduleOptions Options { get; set; }

        [Output]
        long QueryId { get; set; }
    }

    internal class LoadQueryStep : ILoadQueryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public EdgeDefinition NoWorkToDo { get; } = new EdgeDefinition();

        public Models.ScheduleOptions Options { get; set; }

        public long QueryId { get; set; }

        public LoadQueryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var lastUpdatedBefore = DateTime.Now.Add(TimeSpan.FromMinutes(this.Options.UpdateIntervalInMinutes * -1));

                var predicate = context.NoTracking<DAL.Entities.SearchQuery>()
                    .Where(q => !q.Hidden)
                    .Where(q => !q.Executions.Any(e => e.End == null));

                if (!this.Options.IsDefault)
                {
                    predicate = predicate.Where(q => q.Schedules.Any(s => s.Id == this.Options.ScheduleId));
                }

                var successor = await predicate
                    .Select(q => new
                    {
                        Id = q.Id,
                        Updated = q.Executions.Where(e => e.Successful).OrderByDescending(x => x.End).Select(x => x.End).FirstOrDefault()
                    })
                    .Where(q => q.Updated == null || q.Updated <= lastUpdatedBefore)
                    .OrderBy(q => q.Updated)
                    .FirstOrDefaultAsync();

                if (successor == null)
                {
                    return this.NoWorkToDo;
                }

                this.QueryId = successor.Id;

                return this.Next;
            }
        }
    }
}
