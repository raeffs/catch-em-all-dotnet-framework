using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Scheduler;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Models;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.Schedules
{
    [Step]
    public interface IStartScheduleStep : IWorkflowStep
    {
        EdgeDefinition GetNewResults { get; }

        EdgeDefinition UpdateOpenResults { get; }

        EdgeDefinition UpdateOutdatedResults { get; }

        EdgeDefinition ScheduleNotFound { get; }

        [Input]
        BatchedParameter Batch { get; set; }

        [Output]
        ScheduleOptions Options { get; set; }
    }

    internal class StartScheduleStep : IStartScheduleStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition GetNewResults { get; } = new EdgeDefinition();

        public EdgeDefinition UpdateOpenResults { get; } = new EdgeDefinition();

        public EdgeDefinition UpdateOutdatedResults { get; } = new EdgeDefinition();

        public EdgeDefinition ScheduleNotFound { get; } = new EdgeDefinition();

        public BatchedParameter Batch { get; set; }

        public ScheduleOptions Options { get; set; }

        public StartScheduleStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            var scheduleId = (long)this.Batch.Items.First().Parameters["InputScheduleId"];

            using (var context = this.dataAccess.GetContext())
            {
                this.Options = await context.NoTracking<DAL.Entities.Schedule>()
                    .Where(s => s.Id == scheduleId)
                    .Select(s => new ScheduleOptions
                    {
                        IsDefault = s.IsDefault,
                        ScheduleId = s.Id,
                        Type = s.Type,
                        UpdateIntervalInMinutes = s.UpdateIntervalInMinutes
                    })
                    .SingleOrDefaultAsync();

                switch (this.Options?.Type)
                {
                    case DAL.Entities.ExecutionType.GetNewResults:
                        return this.GetNewResults;
                    case DAL.Entities.ExecutionType.UpdateOpenResults:
                        return this.UpdateOpenResults;
                    case DAL.Entities.ExecutionType.UpdateOutdatedResults:
                        return this.UpdateOutdatedResults;
                    default:
                        return this.ScheduleNotFound;
                }
            }
        }
    }
}
