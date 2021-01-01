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

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchQueries
{
    [Step]
    public interface ICreateDashboardComponentConfigurationStep : IHasDashboardComponentConfiguration
    {
        EdgeDefinition Next { get; }

        [Input]
        long QueryId { get; set; }
    }

    internal class CreateDashboardComponentConfigurationStep : ICreateDashboardComponentConfigurationStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition Next { get; } = new EdgeDefinition();

        public string DashboardComponentName { get; set; }

        public string DashboardComponentConfiguration { get; set; }

        public AddToDashboardOptions DashboardComponentOptions { get; set; }

        public long QueryId { get; set; }

        public CreateDashboardComponentConfigurationStep(IDataAccess<ICatchEmAllEntityContext> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            var workflowId = Helpers.WorkflowNameBuilder.ToString(WorkflowIds.SearchQueryByIdWorkflow, new Dictionary<string, object>
            {
                { SearchQueryByIdWorkflow.InputQueryId, this.QueryId },
                { SearchQueryByIdWorkflow.InputCanGoBack, false }
            });

            this.DashboardComponentName = "@cirrus/module.core.ui.dashboard#DisplayValueComponent";
            this.DashboardComponentConfiguration = JsonConvert.SerializeObject(new
            {
                LinkUrl = $"core/workflow/{workflowId}",
                QueryUrl = $"api/cea/searchqueries/countResults/{this.QueryId}",
                TranslationKey = WebApplication.CatchEmAll_SearchResults.Key,
                BackgroundColor = string.Empty
            });

            using (var context = this.dataAccess.GetContext())
            {
                var queryName = await context.NoTracking<DAL.Entities.SearchQuery>()
                    .Where(e => e.Id == this.QueryId)
                    .Select(e => e.Name)
                    .SingleAsync();

                this.DashboardComponentOptions = new AddToDashboardOptions
                {
                    Title = queryName
                };
            }

            return this.Next;
        }
    }
}
