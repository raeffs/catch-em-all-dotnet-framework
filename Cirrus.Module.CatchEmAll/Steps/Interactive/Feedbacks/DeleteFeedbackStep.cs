using System.Data.Entity;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Categories
{
    [Step]
    public interface IDeleteFeedbackStep : IDeleteStep
    {
    }

    internal class DeleteFeedbackStep : DeleteStep, IDeleteFeedbackStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;
        private readonly IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine;

        public DeleteFeedbackStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine)
        {
            this.dataAccess = dataAccess;
            this.viewModelEngine = viewModelEngine;
        }

        protected override async Task DeleteAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                if (await context.NoTracking<DAL.Entities.Feedback>().AnyAsync(e => e.Status != DAL.Entities.FeedbackStatus.New))
                {
                    this.viewModelEngine.ShowNotification(AlertMessages.CatchEmAll_CannotDeleteFeedbackInGivenState.Translate(), true);
                    return;
                }

                foreach (var id in this.Ids)
                {
                    await context.DeleteAsync<DAL.Entities.Feedback>(id);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
