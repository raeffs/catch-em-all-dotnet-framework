using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Schedules
{
    [Step]
    public interface IDeleteScheduleStep : IDeleteStep
    {
    }

    internal class DeleteScheduleStep : DeleteStep, IDeleteScheduleStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public DeleteScheduleStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task DeleteAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                foreach (var id in this.Ids)
                {
                    await context.DeleteAsync<DAL.Entities.Schedule>(id);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
