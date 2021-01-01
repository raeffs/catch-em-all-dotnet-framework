using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Categories;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Workflows;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class SearchQueryByCategoryWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static Workflow Build(string requiredRole)
        {
            var selectCategory = new StepBuilder<ISelectCategoryWithQueryCountStep>();
            var invokeWorkflow = new StepBuilder<IInvokeSearchQueryByCategoryIdWorkflowStep>();

            selectCategory
                .Edge(x => x.Back).MapsTo(EdgeDone)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(selectCategory)
                .Edge(x => x.Edit).MapsTo(invokeWorkflow)
                .Edge(x => x.Delete).MapsTo(selectCategory)
                .Input(x => x.CanGoBack).Constant(false)
                .SetUniqueWorkflowStepId(WorkflowIds.SearchQueryByCategoryWorkflow);

            invokeWorkflow
                .Edge(x => x.Done).MapsTo(selectCategory)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Failed).MapsTo(EdgeFailed)
                .Input(x => x.CategoryId).From(selectCategory).Output(x => x.SingleSelectId);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.SearchQueryByCategoryWorkflow)
                .WithDefinition(nameof(WorkflowIds.SearchQueryByCategoryWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .WithRequiredRole(requiredRole)
                .StartWith(selectCategory);
        }
    }
}
