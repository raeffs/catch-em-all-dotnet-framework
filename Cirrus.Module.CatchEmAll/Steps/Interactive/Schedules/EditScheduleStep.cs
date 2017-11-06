using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
    public interface IEditScheduleStep : IEditStep
    {
    }

    internal class EditScheduleStep : EditStep<Models.Schedule, DAL.Entities.Schedule, ICatchEmAllEntityContext>, IEditScheduleStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;
        private readonly Func<IOwned<IBatchScheduler>> schedulerFactory;
        private readonly IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine;

        internal EditScheduleStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.Schedule> viewmodelExtensions, IDashboard dashboard, Func<IOwned<IBatchScheduler>> schedulerFactory)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
            this.schedulerFactory = schedulerFactory;
            this.viewModelEngine = viewModelEngine;
        }

        protected override async Task<Models.Schedule> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                return await context.NoTracking<DAL.Entities.Schedule>()
                    .Select(e => new Models.Schedule
                    {
                        Id = e.Id,
                        CronExpression = e.CronExpression,
                        IsDefault = e.IsDefault,
                        IsEnabled = e.IsEnabled,
                        Type = e.Type,
                        UpdateIntervalInMinutes = e.UpdateIntervalInMinutes
                    })
                    .FirstOrDefaultAsync(e => e.Id == this.Id);
            }
        }

        protected override async Task SaveAsync(Models.Schedule dto)
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entity = await context.Tracking<DAL.Entities.Schedule>().FirstOrDefaultAsync(e => e.Id == this.Id);

                entity.CronExpression = dto.CronExpression;
                entity.IsDefault = dto.IsDefault;
                entity.IsEnabled = dto.IsEnabled;
                entity.Type = dto.Type;
                entity.UpdateIntervalInMinutes = dto.UpdateIntervalInMinutes;

                using (var scheduler = this.schedulerFactory())
                {
                    if (!entity.IsEnabled)
                    {
                        await scheduler.Value.DeleteScheduleAsync(entity.ScheduleId);
                    }
                    else
                    {
                        if (!await scheduler.Value.HasUptodateCronScheduleAsync(entity.ScheduleId, dto.CronExpression))
                        {
                            await scheduler.Value.DeleteScheduleAsync(entity.ScheduleId);
                            entity.ScheduleId = await scheduler.Value.CreateCronScheduleAsync(
                                dto.CronExpression,
                                WorkflowIds.ExecuteScheduleWorkflow,
                                new Dictionary<string, object> { { ExecuteScheduleWorkflow.InputName, this.Id } });

                            entity.IsEnabled = entity.ScheduleId.HasValue;

                            if (!entity.IsEnabled)
                            {
                                this.viewModelEngine.ShowNotification("Could not be enabled!");
                            }
                        }
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
