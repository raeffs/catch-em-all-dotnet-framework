using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Workflows.Interactive;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;
using Newtonsoft.Json;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Categories
{
    [Step]
    public interface ICreateDashboardComponentConfigurationStep : IHasDashboardComponentConfiguration
    {
        EdgeDefinition Next { get; }

        [Input]
        long CategoryId { get; set; }
    }

    internal class CreateDashboardComponentConfigurationStep : ICreateDashboardComponentConfigurationStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public string DashboardComponentName { get; set; }

        public string DashboardComponentConfiguration { get; set; }

        public AddToDashboardOptions DashboardComponentOptions { get; set; }

        public long CategoryId { get; set; }

        public CreateDashboardComponentConfigurationStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            var workflowId = Helpers.WorkflowNameBuilder.ToString(WorkflowIds.SearchQueryByCategoryIdWorkflow, new Dictionary<string, object>
            {
                { SearchQueryByCategoryIdWorkflow.InputCategoryId, this.CategoryId },
                { SearchQueryByCategoryIdWorkflow.InputCanGoBack, false }
            });

            this.DashboardComponentName = "@cirrus/module.core.ui.dashboard#DisplayValueComponent";
            this.DashboardComponentConfiguration = JsonConvert.SerializeObject(new
            {
                LinkUrl = $"core/workflow/{workflowId}",
                QueryUrl = $"api/cea/searchqueries/count/{this.CategoryId}",
                TranslationKey = WebApplication.CatchEmAll_SearchQueries.Key,
                BackgroundColor = string.Empty
            });

            using (var context = this.dataAccess.GetContext())
            {
                var categoryName = await context.NoTracking<DAL.Entities.Category>()
                    .Where(e => e.Id == this.CategoryId)
                    .Select(e => e.Name)
                    .SingleAsync();

                this.DashboardComponentOptions = new AddToDashboardOptions
                {
                    Title = categoryName
                };
            }

            return this.Next;
        }
    }
}
