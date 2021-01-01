using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Categories;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Feedbacks;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class FeedbackWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static Workflow Build(string requiredRole)
        {
            var select = new StepBuilder<ISelectFeedbackStep>();
            var create = new StepBuilder<ICreateFeedbackStep>();
            var edit = new StepBuilder<IEditFeedbackStep>();
            var redirect = new StepBuilder<IRedirectSelectOrCreated>();
            var delete = new StepBuilder<IDeleteFeedbackStep>();

            select
                .Edge(x => x.Back).MapsTo(EdgeDone)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(create)
                .Edge(x => x.Edit).MapsTo(redirect)
                .Edge(x => x.Delete).MapsTo(delete)
                .Input(x => x.CanGoBack).Constant(false)
                .SetUniqueWorkflowStepId(WorkflowIds.FeedbackWorkflow);

            create
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(redirect)
                .Edge(x => x.Back).MapsTo(select);

            redirect
                .Edge(x => x.Next).MapsTo(edit)
                .Edge(x => x.NotSet).MapsTo(select)
                .Input(x => x.CreatedId).From(create).Output(x => x.Id)
                .Input(x => x.SelectedId).From(select).Output(x => x.SingleSelectId);

            edit
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(edit)
                .Edge(x => x.Back).MapsTo(select)
                .Input(x => x.Id).From(redirect).Output(x => x.RedirectId);

            delete
                .Edge(x => x.Deleted).MapsTo(select)
                .Input(x => x.Ids).From(select).Output(x => x.MultiSelectIds);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.FeedbackWorkflow)
                .WithDefinition(nameof(WorkflowIds.FeedbackWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .WithRequiredRole(requiredRole)
                .StartWith(select);
        }
    }
}
