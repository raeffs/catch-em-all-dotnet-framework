using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchResults
{
    [Step]
    public interface IDeleteSearchResultStep : IDeleteStep
    {
    }

    internal class DeleteSearchResultStep : DeleteStep, IDeleteSearchResultStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public DeleteSearchResultStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task DeleteAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var entities = await context.Tracking<DAL.Entities.SearchResult>()
                    .Where(e => this.Ids.Contains(e.Id))
                    .ToListAsync();

                entities.ForEach(e => e.Hidden = true);

                await context.SaveChangesAsync();
            }
        }
    }
}
