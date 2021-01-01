using Cirrus.Engine.Scheduler;
using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.Steps.NonInteractive.Notifications;
using Cirrus.Module.CatchEmAll.Steps.NonInteractive.Schedules;
using Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchQueries;
using Cirrus.Module.CatchEmAll.Steps.NonInteractive.SearchResults;

namespace Cirrus.Module.CatchEmAll.Workflows.NonInteractive
{
    internal static class ExecuteScheduleWorkflow
    {
        public static readonly WorkflowEdgeDefinition EdgeDone = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeCanceled = new WorkflowEdgeDefinition();

        public static readonly WorkflowEdgeDefinition EdgeFailed = new WorkflowEdgeDefinition();

        public static readonly string InputName = "Batch";

        public static Workflow Build()
        {
            var startSchedule = new StepBuilder<IStartScheduleStep>();
            var loadQuery = new StepBuilder<ILoadQueryStep>();
            var updateQuery = new StepBuilder<IUpdateQueryFromSourceStep>();
            var loadResult = new StepBuilder<ILoadResultStep>();
            var updateResult = new StepBuilder<IUpdateResultFromSourceStep>();
            var loadOutdatedResult = new StepBuilder<ILoadOutdatedResultStep>();
            var updateOutdatedResult = new StepBuilder<IUpdateResultFromSourceStep>("UpdateOutdatedResult");
            var loadResults = new StepBuilder<ILoadResultsStep>();
            var filterResults = new StepBuilder<IFilterResultsStep>();
            var sendEmail = new StepBuilder<ISendEmailStep>();
            var triggerWebhook = new StepBuilder<ITriggerIftttWebhookStep>();
            var saveResults = new StepBuilder<ISaveResultsStep>();

            startSchedule
                .Edge(x => x.GetNewResults).MapsTo(loadQuery)
                .Edge(x => x.UpdateOpenResults).MapsTo(loadResult)
                .Edge(x => x.UpdateOutdatedResults).MapsTo(loadOutdatedResult)
                .Edge(x => x.ScheduleNotFound).MapsTo(EdgeFailed)
                .Input(x => x.Batch).FromWorkflow(InputName);

            loadQuery
                .Edge(x => x.Next).MapsTo(updateQuery)
                .Edge(x => x.NoWorkToDo).MapsTo(EdgeDone)
                .Input(x => x.Options).From(startSchedule).Output(x => x.Options);

            updateQuery
                .Edge(x => x.Next).MapsTo(loadResults)
                .Edge(x => x.QueryNotFound).MapsTo(EdgeFailed)
                .Input(x => x.Id).From(loadQuery).Output(x => x.QueryId)
                .Input(x => x.IsUserInitiated).Constant(false);

            loadResult
                .Edge(x => x.Next).MapsTo(updateResult)
                .Edge(x => x.NoWorkToDo).MapsTo(EdgeDone)
                .Input(x => x.Options).From(startSchedule).Output(x => x.Options);

            updateResult
                .Edge(x => x.Next).MapsTo(EdgeDone)
                .Edge(x => x.ResultNotFound).MapsTo(EdgeFailed)
                .Input(x => x.Id).From(loadResult).Output(x => x.ResultId)
                .Input(x => x.IsUserInitiated).Constant(false);

            loadOutdatedResult
                .Edge(x => x.Next).MapsTo(updateOutdatedResult)
                .Edge(x => x.NoWorkToDo).MapsTo(EdgeDone)
                .Input(x => x.Options).From(startSchedule).Output(x => x.Options);

            updateOutdatedResult
                .Edge(x => x.Next).MapsTo(EdgeDone)
                .Edge(x => x.ResultNotFound).MapsTo(EdgeFailed)
                .Input(x => x.Id).From(loadOutdatedResult).Output(x => x.ResultId)
                .Input(x => x.IsUserInitiated).Constant(false);

            loadResults
                .Edge(x => x.Next).MapsTo(filterResults)
                .Edge(x => x.NoWorkToDo).MapsTo(EdgeDone)
                .Input(x => x.QueryId).From(loadQuery).Output(x => x.QueryId);

            filterResults
                .Edge(x => x.Next).MapsTo(sendEmail)
                .Input(x => x.QueryId).From(loadQuery).Output(x => x.QueryId)
                .Input(x => x.ResultIds).From(loadResults).Output(x => x.ResultIds);

            sendEmail
                .Edge(x => x.Next).MapsTo(triggerWebhook)
                .Input(x => x.ResultIds).From(filterResults).Output(x => x.FilteredResultIds);

            triggerWebhook
                .Edge(x => x.Next).MapsTo(saveResults)
                .Input(x => x.ResultIds).From(filterResults).Output(x => x.FilteredResultIds);

            saveResults
                .Edge(x => x.Next).MapsTo(EdgeDone)
                .Input(x => x.ResultIds).From(loadResults).Output(x => x.ResultIds);

            return WorkflowBuilder.Create()
                .WithId(WorkflowIds.ExecuteScheduleWorkflow)
                .WithDefinition(nameof(WorkflowIds.ExecuteScheduleWorkflow))
                .WithEdges(EdgeDone, EdgeCanceled, EdgeFailed)
                .AddInput<BatchedParameter>(InputName)
                .StartWith(startSchedule);
        }
    }
}
