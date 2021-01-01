using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Scheduler;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.ViewModel.Extended;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Workflows.NonInteractive;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Schedules
{
    [Step]
    public interface ICreateScheduleStep : ICreateStep
    {
    }

    internal class CreateScheduleStep : CreateStep<Models.Schedule, DAL.Entities.Schedule, ICatchEmAllEntityContext>, ICreateScheduleStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;
        private readonly Func<IOwned<IBatchScheduler>> schedulerFactory;
        private readonly IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine;

        public CreateScheduleStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.Schedule> viewmodelExtensions, IDashboard dashboard, Func<IOwned<IBatchScheduler>> schedulerFactory)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
            this.schedulerFactory = schedulerFactory;
            this.viewModelEngine = viewModelEngine;
        }

        protected override Task<Models.Schedule> LoadAsync() => Task.FromResult(new Models.Schedule());

        protected override async Task SaveAsync(Models.Schedule dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.AddAsync(new DAL.Entities.Schedule
                {
                    CronExpression = dto.CronExpression,
                    IsDefault = dto.IsDefault,
                    IsEnabled = dto.IsEnabled,
                    Type = dto.Type,
                    UpdateIntervalInMinutes = dto.UpdateIntervalInMinutes
                });

                await context.SaveChangesAsync();

                this.Id = entity.Id;

                if (entity.IsEnabled)
                {
                    using (var scheduler = this.schedulerFactory())
                    {
                        entity.ScheduleId = await scheduler.Value.CreateCronScheduleAsync(
                            dto.CronExpression,
                            WorkflowIds.ExecuteScheduleWorkflow,
                            new Dictionary<string, object> { { ExecuteScheduleWorkflow.InputName, this.Id } });
                    }

                    entity.IsEnabled = entity.ScheduleId.HasValue;

                    if (!entity.IsEnabled)
                    {
                        this.viewModelEngine.ShowNotification("Could not be enabled!");
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
