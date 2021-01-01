using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchQueries
{
    [Step]
    public interface IDeleteSearchQueryStep : IDeleteStep
    {
    }

    public class DeleteSearchQueryStep : DeleteStep, IDeleteSearchQueryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public DeleteSearchQueryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task DeleteAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var ids = this.Ids.ToList();
                var entities = await context.Tracking<DAL.Entities.SearchQuery>()
                    .BelongingToCurrentUser()
                    .Where(e => ids.Contains(e.Id))
                    .ToListAsync();

                foreach (var entity in entities)
                {
                    entity.Hidden = true;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
