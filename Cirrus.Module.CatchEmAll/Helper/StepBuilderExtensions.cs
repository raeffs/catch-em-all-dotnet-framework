using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Steps;

namespace Cirrus.Module.CatchEmAll.Helper
{
    public static class StepBuilderExtensions
    {
        public static StepBuilder<T> SetUniqueWorkflowStepId<T>(this StepBuilder<T> builder, string workflowId)
            where T : IHasUniqueWorkflowStepId
        {
            var workflowStepId = $"{workflowId}.{builder.ConfiguredStep.Name}";

            builder
                .Input(x => x.WorkflowStepId)
                .Constant(workflowStepId);

            return builder;
        }
    }
}
