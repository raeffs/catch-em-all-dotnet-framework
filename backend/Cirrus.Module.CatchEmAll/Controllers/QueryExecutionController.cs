using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cirrus.Module.CatchEmAll.Helper;

namespace Cirrus.Module.CatchEmAll.Controllers
{
    [RoutePrefix("api/cea/queryexecutions")]
    public class QueryExecutionController : BaseController
    {
        [Route("statistics/weekly")]
        [HttpGet]
        public async Task<IHttpActionResult> WeeklyStatistics()
        {
            using (var context = this.DataAccess.GetContext())
            {
                var offset = DateTime.Now.AddDays(-7);
                var start = new DateTime(offset.Year, offset.Month, offset.Day, 0, 0, 0);
                var result = await context.NoTracking<DAL.Entities.QueryExecution>()
                    .Where(e => e.Start >= start)
                    .GroupBy(e => DbFunctions.DiffDays(start, e.Start))
                    .Select(g => new
                    {
                        Days = g.Key,
                        Total = g.Count(),
                        Successful = g.Count(e => e.Successful),
                        Failed = g.Count(e => !e.Successful)
                    })
                    .Select(x => new
                    {
                        Day = DbFunctions.AddDays(start, x.Days),
                        Total = x.Total,
                        Successful = x.Successful,
                        Failed = x.Failed
                    })
                    .OrderBy(x => x.Day)
                    .ToListAsync();

                return this.Ok(result);
            }
        }

        [Route("statistics/daily")]
        [HttpGet]
        public async Task<IHttpActionResult> DailyStatistics()
        {
            using (var context = this.DataAccess.GetContext())
            {
                var offset = DateTime.Now.AddHours(-24);
                var start = new DateTime(offset.Year, offset.Month, offset.Day, offset.Hour, 0, 0);
                var result = await context.NoTracking<DAL.Entities.QueryExecution>()
                    .Where(e => e.Start >= start)
                    .GroupBy(e => DbFunctions.DiffHours(start, e.Start))
                    .Select(g => new
                    {
                        Hours = g.Key,
                        Total = g.Count(),
                        Successful = g.Count(e => e.Successful),
                        Failed = g.Count(e => !e.Successful)
                    })
                    .Select(x => new
                    {
                        Hour = DbFunctions.AddHours(start, x.Hours),
                        Total = x.Total,
                        Successful = x.Successful,
                        Failed = x.Failed
                    })
                    .OrderBy(x => x.Hour)
                    .ToListAsync();

                return this.Ok(result);
            }
        }

        [Route("statistics/hourly")]
        [HttpGet]
        public async Task<IHttpActionResult> HourlyStatistics()
        {
            using (var context = this.DataAccess.GetContext())
            {
                var offset = DateTime.Now.AddMinutes(-60);
                var start = new DateTime(offset.Year, offset.Month, offset.Day, offset.Hour, offset.Minute, 0);
                var result = await context.NoTracking<DAL.Entities.QueryExecution>()
                    .Where(e => e.Start >= start)
                    .GroupBy(e => DbFunctions.DiffMinutes(start, e.Start))
                    .Select(g => new
                    {
                        Minutes = g.Key,
                        Total = g.Count(),
                        Successful = g.Count(e => e.Successful),
                        Failed = g.Count(e => !e.Successful)
                    })
                    .Select(x => new Models.ExecutionStatistic
                    {
                        Minute = DbFunctions.AddMinutes(start, x.Minutes),
                        Total = x.Total,
                        Successful = x.Successful,
                        Failed = x.Failed
                    })
                    .OrderBy(x => x.Minute)
                    .ToListAsync();

                var allDates = Enumerable.Range(0, 60).Select(i => start.AddMinutes(i));

                result = result.DefaultForMissingDate(
                    allDates,
                    r => r.Minute.Value,
                    m => new Models.ExecutionStatistic
                    {
                        Minute = m,
                        Total = 0,
                        Successful = 0,
                        Failed = 0
                    })
                    .ToList();

                return this.Ok(result);
            }
        }
    }
}
