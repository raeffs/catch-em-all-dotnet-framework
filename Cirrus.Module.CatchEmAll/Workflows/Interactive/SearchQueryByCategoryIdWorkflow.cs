using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Steps.Interactive.SearchQueries;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Workflows;
using Cirrus.Module.Core.Extensions;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class SearchQueryByCategoryIdWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static readonly string InputCategoryId = nameof(InputCategoryId);

        public static readonly string InputCanGoBack = nameof(InputCanGoBack);

        public static Workflow Build(string requiredRole)
        {
            var createConfig = new StepBuilder<Steps.Interactive.Categories.ICreateDashboardComponentConfigurationStep>();
            var selectQuery = new StepBuilder<ISelectSearchQueryStep>();
            var createQuery = new StepBuilder<ICreateSearchQueryStep>();
            var deleteQueries = new StepBuilder<IDeleteSearchQueryStep>();
            var invokeWorkflow = new StepBuilder<IInvokeSearchQueryByIdWorkflowStep>();

            createConfig
                .Edge(x => x.Next).MapsTo(selectQuery)
                .Input(x => x.CategoryId).FromWorkflow(InputCategoryId);

            selectQuery
                .Edge(x => x.Back).MapsTo(EdgeDone)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(createQuery)
                .Edge(x => x.Edit).MapsTo(invokeWorkflow)
                .Edge(x => x.Delete).MapsTo(deleteQueries)
                .Input(x => x.CanGoBack).FromWorkflow(InputCanGoBack)
                .Input(x => x.CategoryId).FromWorkflow(InputCategoryId)
                .SetUniqueWorkflowStepId(WorkflowIds.SearchQueryByCategoryIdWorkflow)
                .UseDashboardComponentConfigurationFrom(createConfig);

            createQuery
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(selectQuery)
                .Edge(x => x.Back).MapsTo(selectQuery)
                .Input(x => x.CategoryId).FromWorkflow(InputCategoryId)
                .UseDashboardComponentConfigurationFrom(createConfig);

            deleteQueries
                .Edge(x => x.Deleted).MapsTo(selectQuery)
                .Input(x => x.Ids).From(selectQuery).Output(x => x.MultiSelectIds);

            invokeWorkflow
                .Edge(x => x.Done).MapsTo(selectQuery)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Failed).MapsTo(EdgeFailed)
                .Input(x => x.QueryId).From(selectQuery).Output(x => x.SingleSelectId);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.SearchQueryByCategoryIdWorkflow)
                .WithDefinition(nameof(WorkflowIds.SearchQueryByCategoryIdWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .AddInput<long>(InputCategoryId)
                .AddInput<bool>(InputCanGoBack)
                .WithRequiredRole(requiredRole)
                .StartWith(createConfig);
        }
    }
}
