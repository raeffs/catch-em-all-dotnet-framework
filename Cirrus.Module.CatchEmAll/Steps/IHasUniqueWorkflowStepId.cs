using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;

namespace Cirrus.Module.CatchEmAll.Steps
{
    public interface IHasUniqueWorkflowStepId : IWorkflowStep
    {
        [Input]
        string WorkflowStepId { get; set; }
    }
}
