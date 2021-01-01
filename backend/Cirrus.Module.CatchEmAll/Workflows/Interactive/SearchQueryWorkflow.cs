using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Steps.Interactive.SearchQueries;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Workflows;
using Cirrus.Module.Core.Extensions;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class SearchQueryWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static Workflow Build(string requiredRole)
        {
            var selectQuery = new StepBuilder<ISelectSearchQueryStep>();
            var createQuery = new StepBuilder<ICreateSearchQueryStep>();
            var deleteQueries = new StepBuilder<IDeleteSearchQueryStep>();
            var invokeWorkflow = new StepBuilder<IInvokeSearchQueryByIdWorkflowStep>();

            selectQuery
                .Edge(x => x.Back).MapsTo(EdgeDone)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(createQuery)
                .Edge(x => x.Edit).MapsTo(invokeWorkflow)
                .Edge(x => x.Delete).MapsTo(deleteQueries)
                .Input(x => x.CanGoBack).Constant(false)
                .SetUniqueWorkflowStepId(WorkflowIds.SearchQueryWorkflow)
                .UseGenericDashboardComponentConfiguration(WorkflowIds.SearchQueryWorkflow, "api/cea/searchqueries/count", DAL.WebApplication.CatchEmAll_SearchQueries.Key);

            createQuery
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(selectQuery)
                .Edge(x => x.Back).MapsTo(selectQuery)
                .UseDashboardComponentConfigurationFrom(selectQuery);

            deleteQueries
                .Edge(x => x.Deleted).MapsTo(selectQuery)
                .Input(x => x.Ids).From(selectQuery).Output(x => x.MultiSelectIds);

            invokeWorkflow
                .Edge(x => x.Done).MapsTo(selectQuery)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Failed).MapsTo(EdgeFailed)
                .Input(x => x.QueryId).From(selectQuery).Output(x => x.SingleSelectId);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.SearchQueryWorkflow)
                .WithDefinition(nameof(WorkflowIds.SearchQueryWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .WithRequiredRole(requiredRole)
                .StartWith(selectQuery);
        }
    }
}
