using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cirrus.Engine.Interface;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Controllers
{
    [RoutePrefix("api/cea/searchqueries")]
    public class SearchQueryController : BaseController
    {
        [Route("count")]
        [HttpGet]
        [ResponseType(typeof(long))]
        public async Task<IHttpActionResult> Count()
        {
            using (var context = this.DataAccess.GetContext())
            {
                var count = await context.NoTracking<DAL.Entities.SearchQuery>()
                    .BelongingToCurrentUser()
                    .Where(e => !e.Hidden)
                    .CountAsync();
                return this.Ok(count);
            }
        }

        [Route("count/{categoryId}")]
        [HttpGet]
        [ResponseType(typeof(long))]
        public async Task<IHttpActionResult> CountByCategory(long categoryId)
        {
            using (var context = this.DataAccess.GetContext())
            {
                var count = await context.NoTracking<DAL.Entities.SearchQuery>()
                    .BelongingToCurrentUser()
                    .Where(e => !e.Hidden)
                    .Where(e => e.CategoryId == categoryId)
                    .CountAsync();
                return this.Ok(count);
            }
        }

        [Route("countResults/{queryId}")]
        [HttpGet]
        [ResponseType(typeof(long))]
        public async Task<IHttpActionResult> CountResults(long queryId)
        {
            using (var context = this.DataAccess.GetContext())
            {
                var count = await context.NoTracking<DAL.Entities.SearchResult>()
                    .Where(e => e.QueryId == queryId && !e.Hidden && !e.Closed)
                    .CountAsync();
                return this.Ok(count);
            }
        }
    }
}
