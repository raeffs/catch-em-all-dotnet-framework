using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using Cirrus.Engine.Interface;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Controllers
{
    [RoutePrefix("api/cea/categories")]
    public class CategoryController : BaseController
    {
        [Route("count")]
        [HttpGet]
        [ResponseType(typeof(long))]
        public async Task<IHttpActionResult> Count()
        {
            using (var context = this.DataAccess.GetContext())
            {
                var count = await context.NoTracking<DAL.Entities.Category>()
                    .BelongingToCurrentUser()
                    .CountAsync();
                return this.Ok(count);
            }
        }
    }
}
