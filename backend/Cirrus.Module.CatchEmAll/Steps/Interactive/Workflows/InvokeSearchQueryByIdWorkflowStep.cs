using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.Workflows.Interactive;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Workflows
{
    [Step]
    public interface IInvokeSearchQueryByIdWorkflowStep : IWorkflowStep
    {
        EdgeDefinition Done { get; }

        EdgeDefinition Canceled { get; }

        EdgeDefinition Failed { get; }

        [Input]
        long QueryId { get; set; }
    }

    internal class InvokeSearchQueryByIdWorkflowStep : IInvokeSearchQueryByIdWorkflowStep
    {
        private readonly IWorkflowManager workflowManager;
        public EdgeDefinition Done { get; } = new EdgeDefinition();

        public EdgeDefinition Canceled { get; } = new EdgeDefinition();

        public EdgeDefinition Failed { get; } = new EdgeDefinition();

        public long QueryId { get; set; }

        public InvokeSearchQueryByIdWorkflowStep(IWorkflowManager workflowManager)
        {
            this.workflowManager = workflowManager;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            var parameters = new Dictionary<string, object>
            {
                { SearchQueryByIdWorkflow.InputQueryId, this.QueryId },
                { SearchQueryByIdWorkflow.InputCanGoBack, true }
            };

            var runInstance = await this.workflowManager.GetRunInstanceAsync(WorkflowIds.SearchQueryByIdWorkflow, parameters);

            var result = await runInstance.RunWorkflowAsync();

            switch (result.OutgoingEdge)
            {
                case nameof(SearchQueryByIdWorkflow.EdgeDone):
                    return this.Done;
                case nameof(SearchQueryByIdWorkflow.EdgeCanceled):
                    return this.Canceled;
                case nameof(SearchQueryByIdWorkflow.EdgeFailed):
                    return this.Failed;
            }

            return this.Failed;
        }
    }
}
