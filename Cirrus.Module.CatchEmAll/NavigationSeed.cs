using System.Collections.Generic;
using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll
{
    internal class NavigationSeed : INavigationSeed
    {
        public List<INavigationGroup> Seed()
        {
            return new NavigationBuilder()
                .CreateGroup(Navigation.CatchEmAll_CatchEmAll, "gavel")
                    .WithWorkflow(Navigation.CatchEmAll_Categories, WorkflowIds.CategoryWorkflow)
                    .WithWorkflow(Navigation.CatchEmAll_SearchQueries, WorkflowIds.SearchQueryWorkflow)
                    .WithWorkflow(Navigation.CatchEmAll_SearchQueriesByCategory, WorkflowIds.SearchQueryByCategoryWorkflow)
                    .WithWorkflow(Navigation.CatchEmAll_Settings, WorkflowIds.SettingsWorkflow)
                    .WithWorkflow(Navigation.CatchEmAll_Feedback, WorkflowIds.FeedbackWorkflow)
                    .Add()
                .CreateGroup(Navigation.CatchEmAll_CatchEmAllAdmin, "verified_user")
                    .WithWorkflow(Navigation.CatchEmAll_Schedules, WorkflowIds.ScheduleWorkflow)
                    .WithWorkflow(Navigation.CatchEmAll_QueryExecution, WorkflowIds.QueryExecutionWorkflow)
                    .Add()
                .Build();
        }
    }
}
