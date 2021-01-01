using System;
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
    public interface ICreateFeedbackStep : ICreateStep
    {
    }

    internal class CreateFeedbackStep : CreateStep<Models.Feedback, DAL.Entities.Feedback, ICatchEmAllEntityContext>, ICreateFeedbackStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public CreateFeedbackStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.Feedback> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override Task<Models.Feedback> LoadAsync() => Task.FromResult(new Models.Feedback());

        protected override async Task SaveAsync(Models.Feedback dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.AddAsync(new DAL.Entities.Feedback
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Created = DateTime.Now,
                    Status = DAL.Entities.FeedbackStatus.New,
                    User = await context.GetOrCreateUserReferenceAsync()
                });

                await context.SaveChangesAsync();

                this.Id = entity.Id;
            }
        }

        protected override EditViewModel InterceptViewModel(EditViewModel viewModel)
        {
            viewModel.As<Models.Feedback>().Hide(x => x.Status);

            return base.InterceptViewModel(viewModel);
        }
    }
}
