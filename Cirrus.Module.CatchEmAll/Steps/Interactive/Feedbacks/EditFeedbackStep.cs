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
    public interface IEditFeedbackStep : IEditStep
    {
    }

    internal class EditFeedbackStep : EditStep<Models.Feedback, DAL.Entities.Feedback, ICatchEmAllEntityContext>, IEditFeedbackStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        internal EditFeedbackStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.Feedback> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<Models.Feedback> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var userId = Context.CurrentUserInfo.Id;
                return await context.NoTracking<DAL.Entities.Feedback>()
                    .Select(e => new Models.Feedback
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        Status = e.Status,
                        BelongsToCurrentUser = e.User.UserId == userId
                    })
                    .FirstOrDefaultAsync(e => e.Id == this.Id);
            }
        }

        protected override async Task SaveAsync(Models.Feedback dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.Tracking<DAL.Entities.Feedback>().FirstOrDefaultAsync(e => e.Id == this.Id);

                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.Status = dto.Status;

                await context.SaveChangesAsync();
            }
        }

        protected override EditViewModel InterceptViewModel(EditViewModel viewModel, Models.Feedback dto)
        {
            if (!Context.CurrentUserInfo.BusinessRoles.Contains(BusinessRoles.AdminRole))
            {
                viewModel.As<Models.Feedback>().Attributes(x => x.Status).Add(new ReadonlyAttribute());
            }

            if (dto.Status != DAL.Entities.FeedbackStatus.New || !dto.BelongsToCurrentUser)
            {
                viewModel.As<Models.Feedback>().Attributes(x => x.Name).Add(new ReadonlyAttribute());
                viewModel.As<Models.Feedback>().Attributes(x => x.Description).Add(new ReadonlyAttribute());
            }

            return base.InterceptViewModel(viewModel, dto);
        }
    }
}
