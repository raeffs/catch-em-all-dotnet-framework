using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchResults
{
    [Step]
    public interface ILoadOutdatedResultStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }

        EdgeDefinition NoWorkToDo { get; }

        [Input]
        Models.ScheduleOptions Options { get; set; }

        [Output]
        long ResultId { get; set; }
    }

    internal class LoadOutdatedResultStep : ILoadOutdatedResultStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public EdgeDefinition NoWorkToDo { get; } = new EdgeDefinition();

        public Models.ScheduleOptions Options { get; set; }

        public long ResultId { get; set; }

        public LoadOutdatedResultStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var predicate = context.NoTracking<DAL.Entities.SearchResult>()
                    .Where(r => !r.Hidden && !r.Closed)
                    .Where(r => !r.Executions.Any(e => e.End == null));

                if (!this.Options.IsDefault)
                {
                    predicate = predicate.Where(r => r.Query.Schedules.Any(s => s.Id == this.Options.ScheduleId));
                }

                var successor = await predicate
                    .Select(r => new
                    {
                        Id = r.Id,
                        Ends = r.Ends,
                        Updated = r.Executions.Where(e => e.Successful).OrderByDescending(x => x.End).Select(x => x.End).FirstOrDefault()
                    })
                    .Where(r => r.Ends == null || r.Ends <= DateTime.Now)
                    .OrderBy(r => r.Updated)
                    .FirstOrDefaultAsync();

                if (successor == null)
                {
                    return this.NoWorkToDo;
                }

                this.ResultId = successor.Id;

                return this.Next;
            }
        }
    }
}
