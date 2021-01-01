using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Attributes;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.ViewModel.Extended;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Categories
{
    [Step]
    public interface IEditCategoryStep : IEditStep
    {
    }

    internal class EditCategoryStep : EditStep<Models.Category, DAL.Entities.Category, ICatchEmAllEntityContext>, IEditCategoryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        internal EditCategoryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.Category> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<Models.Category> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                return await context.NoTracking<DAL.Entities.Category>()
                    .Select(e => new Models.Category
                    {
                        Id = e.Id,
                        Number = e.Number,
                        Name = e.Name
                    })
                    .FirstOrDefaultAsync(e => e.Id == this.Id);
            }
        }

        protected override async Task SaveAsync(Models.Category dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.Tracking<DAL.Entities.Category>().FirstOrDefaultAsync(e => e.Id == this.Id);

                entity.Number = dto.Number;
                entity.Name = dto.Name;

                await context.SaveChangesAsync();
            }
        }

        protected override EditViewModel InterceptViewModel(EditViewModel viewModel)
        {
            viewModel.As<Models.Category>().Attributes(m => m.Number).Add(new ReadonlyAttribute());

            return base.InterceptViewModel(viewModel);
        }
    }
}
