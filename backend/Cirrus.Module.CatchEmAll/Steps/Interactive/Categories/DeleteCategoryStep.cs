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
    public interface IDeleteCategoryStep : IDeleteStep
    {
    }

    internal class DeleteCategoryStep : DeleteStep, IDeleteCategoryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;
        private readonly IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine;

        public DeleteCategoryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine)
        {
            this.dataAccess = dataAccess;
            this.viewModelEngine = viewModelEngine;
        }

        protected override async Task DeleteAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                if (await context.NoTracking<DAL.Entities.Category>().AnyAsync(e => this.Ids.Contains(e.Id) && e.Queries.Count > 0))
                {
                    this.viewModelEngine.ShowNotification(AlertMessages.CatchEmAll_CategoryHasAssignedQueries.Translate(), true);
                    return;
                }

                foreach (var id in this.Ids)
                {
                    await context.DeleteAsync<DAL.Entities.Category>(id);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
