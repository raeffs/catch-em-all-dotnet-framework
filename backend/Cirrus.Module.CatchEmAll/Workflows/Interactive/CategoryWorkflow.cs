using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Steps.Interactive.Categories;
using Cirrus.Module.CatchEmAll.Steps.NonInteractive.Categories;
using Cirrus.Module.Core.Extensions;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class CategoryWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static Workflow Build(string requiredRole)
        {
            var select = new StepBuilder<ISelectCategoryStep>();
            var create = new StepBuilder<ICreateCategoryStep>();
            var edit = new StepBuilder<IEditCategoryStep>();
            var redirect = new StepBuilder<IRedirectSelectOrCreated>();
            var delete = new StepBuilder<IDeleteCategoryStep>();
            var importExisting = new StepBuilder<ICategoryImportExistingStep>();

            select
                .Edge(x => x.Back).MapsTo(EdgeDone)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(create)
                .Edge(x => x.Edit).MapsTo(redirect)
                .Edge(x => x.Delete).MapsTo(delete)
                .Edge(x => x.ImportExisting).MapsTo(importExisting)
                .Input(x => x.CanGoBack).Constant(false)
                .SetUniqueWorkflowStepId(WorkflowIds.CategoryWorkflow)
                .UseGenericDashboardComponentConfiguration(WorkflowIds.CategoryWorkflow, "api/cea/categories/count", DAL.WebApplication.CatchEmAll_Categories.Key);

            create
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(redirect)
                .Edge(x => x.Back).MapsTo(select)
                .UseDashboardComponentConfigurationFrom(select);

            redirect
                .Edge(x => x.Next).MapsTo(edit)
                .Edge(x => x.NotSet).MapsTo(select)
                .Input(x => x.CreatedId).From(create).Output(x => x.Id)
                .Input(x => x.SelectedId).From(select).Output(x => x.SingleSelectId);

            edit
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Saved).MapsTo(edit)
                .Edge(x => x.Back).MapsTo(select)
                .Input(x => x.Id).From(redirect).Output(x => x.RedirectId)
                .UseDashboardComponentConfigurationFrom(select);

            delete
                .Edge(x => x.Deleted).MapsTo(select)
                .Input(x => x.Ids).From(select).Output(x => x.MultiSelectIds);

            importExisting
                .Edge(x => x.Next).MapsTo(select);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.CategoryWorkflow)
                .WithDefinition(nameof(WorkflowIds.CategoryWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .WithRequiredRole(requiredRole)
                .StartWith(select);
        }
    }
}
