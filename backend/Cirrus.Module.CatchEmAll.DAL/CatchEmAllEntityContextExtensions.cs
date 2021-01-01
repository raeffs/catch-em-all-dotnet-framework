using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Module.CatchEmAll.DAL.Entities;

namespace Cirrus.Module.CatchEmAll.DAL
{
    internal static class CatchEmAllEntityContextExtensions
    {
        public static async Task<UserReference> GetOrCreateUserReferenceAsync(this IDatabaseContext<ICatchEmAllEntityContext> context)
        {
            var id = Context.CurrentUserInfo.Id;
            return (await context.Tracking<UserReference>().Where(u => u.UserId == id).SingleOrDefaultAsync())
                ?? new UserReference { UserId = id, Username = Context.CurrentUserInfo.Username };
        }

        public static IQueryable<TEntity> BelongingToCurrentUser<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class, IForUser
        {
            var id = Context.CurrentUserInfo.Id;
            return source.Where(e => e.User.UserId == id);
        }

        public static IQueryable<SearchQuery> BelongingToCurrentUser(this IQueryable<SearchQuery> source)
        {
            var id = Context.CurrentUserInfo.Id;
            return source.Where(e => e.Category.User.UserId == id);
        }
    }
}
