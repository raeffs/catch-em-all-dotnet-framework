using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Settings;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class SettingsWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static Workflow Build(string requiredRole)
        {
            var edit = new StepBuilder<IEditSettingsStep>();

            edit.Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(edit)
                .Edge(x => x.Back).MapsTo(edit)
                .Input<object>(x => x.Id).Constant("1");

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.SettingsWorkflow)
                .WithDefinition(nameof(WorkflowIds.SettingsWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .WithRequiredRole(requiredRole)
                .StartWith(edit);
        }
    }
}
