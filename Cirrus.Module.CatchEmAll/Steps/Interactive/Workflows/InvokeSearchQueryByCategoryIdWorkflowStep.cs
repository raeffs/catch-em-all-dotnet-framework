using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.Workflows.Interactive;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Workflows
{
    [Step]
    public interface IInvokeSearchQueryByCategoryIdWorkflowStep : IWorkflowStep
    {
        EdgeDefinition Done { get; }

        EdgeDefinition Canceled { get; }

        EdgeDefinition Failed { get; }

        [Input]
        long CategoryId { get; set; }
    }

    internal class InvokeSearchQueryByCategoryIdWorkflowStep : IInvokeSearchQueryByCategoryIdWorkflowStep
    {
        private readonly IWorkflowManager workflowManager;
        public EdgeDefinition Done { get; } = new EdgeDefinition();

        public EdgeDefinition Canceled { get; } = new EdgeDefinition();

        public EdgeDefinition Failed { get; } = new EdgeDefinition();

        public long CategoryId { get; set; }

        public InvokeSearchQueryByCategoryIdWorkflowStep(IWorkflowManager workflowManager)
        {
            this.workflowManager = workflowManager;
        }

        public async Task<EdgeDefinition> RunAsync()
        {
            var parameters = new Dictionary<string, object>
            {
                { SearchQueryByCategoryIdWorkflow.InputCategoryId, this.CategoryId },
                { SearchQueryByCategoryIdWorkflow.InputCanGoBack, true }
            };

            var runInstance = await this.workflowManager.GetRunInstanceAsync(WorkflowIds.SearchQueryByCategoryIdWorkflow, parameters);

            var result = await runInstance.RunWorkflowAsync();

            switch (result.OutgoingEdge)
            {
                case nameof(SearchQueryByCategoryIdWorkflow.EdgeDone):
                    return this.Done;
                case nameof(SearchQueryByCategoryIdWorkflow.EdgeCanceled):
                    return this.Canceled;
                case nameof(SearchQueryByCategoryIdWorkflow.EdgeFailed):
                    return this.Failed;
            }

            return this.Failed;
        }
    }
}
