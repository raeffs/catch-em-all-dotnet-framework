using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Executions;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class QueryExecutionWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static Workflow Build(string requiredRole)
        {
            var select = new StepBuilder<ISelectQueryExecutionStep>();
            var view = new StepBuilder<IViewQueryExecutionStep>();

            select
                .Edge(x => x.Back).MapsTo(EdgeDone)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(EdgeFailed)
                .Edge(x => x.Delete).MapsTo(EdgeFailed)
                .Edge(x => x.Edit).MapsTo(view)
                .Input(x => x.CanGoBack).Constant(false)
                .SetUniqueWorkflowStepId(WorkflowIds.QueryExecutionWorkflow);

            view
                .Edge(x => x.Back).MapsTo(select)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(EdgeFailed)
                .Input(x => x.Id).From(select).Output(x => x.SingleSelectId);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.QueryExecutionWorkflow)
                .WithDefinition(nameof(WorkflowIds.QueryExecutionWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .WithRequiredRole(requiredRole)
                .StartWith(select);
        }
    }
}
