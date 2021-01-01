using System.Threading.Tasks;
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
    public interface ICreateCategoryStep : ICreateStep
    {
    }

    internal class CreateCategoryStep : CreateStep<Models.Category, DAL.Entities.Category, ICatchEmAllEntityContext>, ICreateCategoryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public CreateCategoryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.Category> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override Task<Models.Category> LoadAsync() => Task.FromResult(new Models.Category());

        protected override async Task SaveAsync(Models.Category dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.AddAsync(new DAL.Entities.Category
                {
                    Number = dto.Number,
                    Name = dto.Name,
                    User = await context.GetOrCreateUserReferenceAsync()
                });

                await context.SaveChangesAsync();

                this.Id = entity.Id;
            }
        }
    }
}
