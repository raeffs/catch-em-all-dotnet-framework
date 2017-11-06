using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cirrus.Engine.Interface;
using Cirrus.Module.CatchEmAll.Helper;

namespace Cirrus.Module.CatchEmAll.Controllers
{
    [RoutePrefix("api/cea/schedules")]
    public class ScheduleController : BaseController
    {
        [Route("statistics")]
        [HttpGet]
        [ResponseType(typeof(Models.ScheduleStatistic))]
        public async Task<IHttpActionResult> Count()
        {
            using (var context = this.DataAccess.GetContext())
            {
                var schedules = await context.NoTracking<DAL.Entities.Schedule>()
                    .Select(e => new
                    {
                        e.ScheduleId,
                        IsEnabled = e.IsEnabled,
                        e.CronExpression
                    })
                    .ToListAsync();

                var statistics = new Models.ScheduleStatistic
                {
                    Total = schedules.Count
                };

                foreach (var schedule in schedules)
                {
                    if (schedule.IsEnabled && await this.Scheduler.Value.HasUptodateCronScheduleAsync(schedule.ScheduleId, schedule.CronExpression))
                    {
                        statistics.Enabled++;
                    }
                    else if (schedule.IsEnabled && await this.Scheduler.Value.HasScheduleAsync(schedule.ScheduleId))
                    {
                        statistics.OutdatedSchedule++;
                    }
                    else if (schedule.IsEnabled)
                    {
                        statistics.MissingSchedule++;
                    }
                    else
                    {
                        statistics.Disabled++;
                    }
                }

                return this.Ok(statistics);
            }
        }
    }
}
