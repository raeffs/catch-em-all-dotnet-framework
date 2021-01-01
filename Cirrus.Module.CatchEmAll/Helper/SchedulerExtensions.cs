using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Engine.Scheduler;

namespace Cirrus.Module.CatchEmAll.Helper
{
    internal static class SchedulerExtensions
    {
        public static async Task<Guid?> CreateCronScheduleAsync(this IBatchScheduler scheduler, string cronExpression, string workflowId, Dictionary<string, object> parameters)
        {
            var scheduleId = Guid.NewGuid();
            var item = new BatchedItem
            {
                SiteId = Context.CurrentConfiguration.SiteId.Value,
                Parameters = parameters
            };
            var batch = new BatchedParameter
            {
                Tenant = Context.CurrentConfiguration.Tenant,
                ScheduleId = scheduleId,
                WorkflowId = workflowId,
                CronExpression = cronExpression,
                CronTimeZone = "UTC",
                Items = new List<BatchedItem> { item }
            };
            var result = await scheduler.ScheduleBatchedExecutionAsync(batch);
            return result ? (Guid?)scheduleId : null;
        }

        public static async Task<bool> HasScheduleAsync(this IBatchScheduler scheduler, Guid? scheduleId)
        {
            try
            {
                return scheduleId.HasValue && (await scheduler.GetAsync(scheduleId.Value)) != null;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> HasUptodateCronScheduleAsync(this IBatchScheduler scheduler, Guid? scheduleId, string cronExpression)
        {
            try
            {
                return scheduleId.HasValue
                    && !string.IsNullOrWhiteSpace(cronExpression)
                    && (await scheduler.GetAsync(scheduleId.Value))?.CronExpression == cronExpression;
            }
            catch
            {
                return false;
            }
        }

        public static async Task DeleteScheduleAsync(this IBatchScheduler scheduler, Guid? scheduleId)
        {
            if (!scheduleId.HasValue)
            {
                return;
            }

            try
            {
                await scheduler.DeleteAsync(scheduleId.Value);
            }
            catch
            {
                // todo
            }
        }
    }
}
