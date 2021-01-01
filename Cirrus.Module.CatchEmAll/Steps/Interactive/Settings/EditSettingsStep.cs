using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.ViewModel.Extended;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Settings
{
    [Step]
    public interface IEditSettingsStep : IEditStep
    {
        [Input(Optional = true)]
        bool CanGoBack { get; set; }
    }

    internal class EditSettingsStep : EditStep<Models.Settings, DAL.Entities.Settings, ICatchEmAllEntityContext>, IEditSettingsStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public bool CanGoBack { get; set; }

        internal EditSettingsStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.Settings> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<Models.Settings> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.Tracking<DAL.Entities.Settings>().BelongingToCurrentUser().SingleOrDefaultAsync();

                if (entity == null)
                {
                    entity = await context.AddAsync(new DAL.Entities.Settings
                    {
                        User = await context.GetOrCreateUserReferenceAsync(),
                        EnableEmailNotification = true,
                        Email = Context.CurrentUserInfo.Email,
                        EnableNotificationsDefault = true
                    });

                    await context.SaveChangesAsync();
                }

                return new Models.Settings
                {
                    EnableEmailNotification = entity.EnableEmailNotification,
                    EnableIftttNotification = entity.EnableIftttNotification,
                    Email = entity.Email,
                    AlternativeEmail = entity.AlternativeEmail,
                    IftttMakerKey = entity.IftttMakerKey,
                    IftttMakerEventName = entity.IftttMakerEventName,
                    EnableNotificationsDefault = entity.EnableNotificationsDefault,
                    AutoFilterDeletedDuplicatesDefault = entity.AutoFilterDeletedDuplicatesDefault
                };
            }
        }

        protected override async Task SaveAsync(Models.Settings dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.Tracking<DAL.Entities.Settings>().BelongingToCurrentUser().SingleAsync();

                entity.EnableEmailNotification = dto.EnableEmailNotification;
                entity.EnableIftttNotification = dto.EnableIftttNotification;
                entity.Email = dto.Email;
                entity.AlternativeEmail = dto.AlternativeEmail;
                entity.IftttMakerKey = dto.IftttMakerKey;
                entity.IftttMakerEventName = dto.IftttMakerEventName;
                entity.EnableNotificationsDefault = dto.EnableNotificationsDefault;
                entity.AutoFilterDeletedDuplicatesDefault = dto.AutoFilterDeletedDuplicatesDefault;

                await context.SaveChangesAsync();
            }
        }

        protected override async Task<List<ViewModelAction>> CreateActionsAsync(Models.Settings dto)
        {
            var actions = await base.CreateActionsAsync(dto);

            if (!this.CanGoBack)
            {
                var backAction = actions.Single(a => a.Type == ViewModelActionType.Back);
                actions.Remove(backAction);
            }

            return actions;
        }

        protected override EditViewModel InterceptViewModel(EditViewModel viewModel, Models.Settings dto)
        {
            if (!dto.EnableEmailNotification)
            {
                viewModel.As<Models.Settings>().Hide(x => x.Email);
                viewModel.As<Models.Settings>().Hide(x => x.AlternativeEmail);
            }

            if (!dto.EnableIftttNotification)
            {
                viewModel.As<Models.Settings>().Hide(x => x.IftttMakerKey);
                viewModel.As<Models.Settings>().Hide(x => x.IftttMakerEventName);
            }

            return base.InterceptViewModel(viewModel, dto);
        }
    }
}
