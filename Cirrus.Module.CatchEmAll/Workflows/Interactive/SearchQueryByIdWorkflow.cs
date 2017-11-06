using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.CatchEmAll.Steps.Interactive.SearchQueries;
using Cirrus.Module.CatchEmAll.Steps.Interactive.SearchResults;
using Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchQueries;
using Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchResults;
using Cirrus.Module.Core.Extensions;

namespace Cirrus.Module.CatchEmAll.Workflows.Interactive
{
    internal static class SearchQueryByIdWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static readonly string InputQueryId = nameof(InputQueryId);

        public static readonly string InputCanGoBack = nameof(InputCanGoBack);

        public static Workflow Build(string requiredRole)
        {
            var createConfig = new StepBuilder<ICreateDashboardComponentConfigurationStep>();
            var selectResult = new StepBuilder<ISelectSearchResultStep>();
            var selectArchivedResult = new StepBuilder<ISelectArchivedSearchResultStep>();
            var updateQuery = new StepBuilder<IUpdateQueryFromSourceStep>();
            var deleteResults = new StepBuilder<IDeleteSearchResultStep>();
            var deleteArchivedResults = new StepBuilder<IDeleteSearchResultStep>("DeleteArchivedResults");
            var viewResult = new StepBuilder<IViewSearchResultStep>();
            var viewArchivedResult = new StepBuilder<IViewSearchResultStep>("ViewArchivedResult");
            var updateResult = new StepBuilder<IUpdateResultFromSourceStep>();
            var editQuery = new StepBuilder<IEditSearchQueryStep>();

            createConfig
                .Edge(x => x.Next).MapsTo(selectResult)
                .Input(x => x.QueryId).FromWorkflow(InputQueryId);

            selectResult
                .Edge(x => x.Back).MapsTo(EdgeDone)
                .Edge(x => x.UpdateFromSource).MapsTo(updateQuery)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(selectResult)
                .Edge(x => x.Edit).MapsTo(viewResult)
                .Edge(x => x.Delete).MapsTo(deleteResults)
                .Edge(x => x.ShowArchive).MapsTo(selectArchivedResult)
                .Edge(x => x.EditQuery).MapsTo(editQuery)
                .Input(x => x.CanGoBack).FromWorkflow(InputCanGoBack)
                .Input(x => x.QueryId).FromWorkflow(InputQueryId)
                .SetUniqueWorkflowStepId(WorkflowIds.SearchQueryByIdWorkflow)
                .UseDashboardComponentConfigurationFrom(createConfig);

            selectArchivedResult
                .Edge(x => x.Back).MapsTo(selectResult)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Create).MapsTo(EdgeFailed)
                .Edge(x => x.Edit).MapsTo(viewArchivedResult)
                .Edge(x => x.Delete).MapsTo(deleteArchivedResults)
                .Input(x => x.CanGoBack).Constant(true)
                .Input(x => x.QueryId).FromWorkflow(InputQueryId)
                .SetUniqueWorkflowStepId(WorkflowIds.SearchQueryByIdWorkflow)
                .UseDashboardComponentConfigurationFrom(createConfig);

            updateQuery
                .Edge(x => x.Next).MapsTo(selectResult)
                .Edge(x => x.QueryNotFound).MapsTo(EdgeFailed)
                .Input(x => x.Id).FromWorkflow(InputQueryId)
                .Input(x => x.IsUserInitiated).Constant(true);

            deleteResults
                .Edge(x => x.Deleted).MapsTo(selectResult)
                .Input(x => x.Ids).From(selectResult).Output(x => x.MultiSelectIds);

            deleteArchivedResults
                .Edge(x => x.Deleted).MapsTo(selectArchivedResult)
                .Input(x => x.Ids).From(selectArchivedResult).Output(x => x.MultiSelectIds);

            viewResult
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Back).MapsTo(selectResult)
                .Edge(x => x.Saved).MapsTo(viewResult)
                .Edge(x => x.UpdateFromSource).MapsTo(updateResult)
                .Input(x => x.Id).From(selectResult).Output(x => x.SingleSelectId);

            viewArchivedResult
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Edge(x => x.Back).MapsTo(selectArchivedResult)
                .Edge(x => x.Saved).MapsTo(viewArchivedResult)
                .Edge(x => x.UpdateFromSource).MapsTo(EdgeFailed)
                .Input(x => x.Id).From(selectArchivedResult).Output(x => x.SingleSelectId);

            updateResult
                .Edge(x => x.Next).MapsTo(viewResult)
                .Edge(x => x.ResultNotFound).MapsTo(EdgeFailed)
                .Input(x => x.Id).From(selectResult).Output(x => x.SingleSelectId)
                .Input(x => x.IsUserInitiated).Constant(true);

            editQuery
                .Edge(x => x.Saved).MapsTo(editQuery)
                .Edge(x => x.Back).MapsTo(selectResult)
                .Edge(x => x.Canceled).MapsTo(EdgeCanceled)
                .Input(x => x.Id).FromWorkflow(InputQueryId)
                .UseDashboardComponentConfigurationFrom(createConfig);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.SearchQueryByIdWorkflow)
                .WithDefinition(nameof(WorkflowIds.SearchQueryByIdWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .AddInput<long>(InputQueryId)
                .AddInput<bool>(InputCanGoBack)
                .WithRequiredRole(requiredRole)
                .StartWith(createConfig);
        }
    }
}
