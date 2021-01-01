using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Steps.NonInteractive.Categories
{
    [Step]
    public interface ICategoryImportExistingStep : IWorkflowStep
    {
        EdgeDefinition Next { get; }
    }

    internal class CategoryImportExistingStep : ICategoryImportExistingStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public CategoryImportExistingStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public async Task<EdgeDefinition> RunAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                var existing = await context.NoTracking<DAL.Entities.Category>()
                    .Select(e => new
                    {
                        e.Number,
                        e.Name
                    })
                    .Distinct()
                    .ToListAsync();

                var user = await context.GetOrCreateUserReferenceAsync();

                existing.ForEach(e =>
                {
                    context.AddAsync(new DAL.Entities.Category
                    {
                        Number = e.Number,
                        Name = e.Name,
                        User = user
                    });
                });

                await context.SaveChangesAsync();
            }

            return this.Next;
        }
    }
}
