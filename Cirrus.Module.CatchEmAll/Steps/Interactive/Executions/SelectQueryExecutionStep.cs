using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Executions
{
    public enum ExecutionDisplayMode
    {
        ShowAll,
        ShowFailed,
        ShowRunning
    }

    [Step]
    public interface ISelectQueryExecutionStep : ISelectStep
    {
        [Output(ResumeOldValues = true)]
        ExecutionDisplayMode Mode { get; set; }
    }

    internal class SelectQueryExecutionStep : SelectStep<Models.QueryExecutionSummary, DAL.Entities.QueryExecution, ICatchEmAllEntityContext>, ISelectQueryExecutionStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public ExecutionDisplayMode Mode { get; set; } = ExecutionDisplayMode.ShowAll;

        protected override Expression<Func<DAL.Entities.QueryExecution, Models.QueryExecutionSummary>> Selector { get; } = e => new Models.QueryExecutionSummary
        {
            Id = e.Id,
            Start = e.Start,
            End = e.End,
            Successful = e.Successful,
            IsUserInitiated = e.IsUserInitiated,
            Query = e.Query.Name,
            Category = e.Query.Category.Name,
            Username = e.Query.Category.User.Username
        };

        public SelectQueryExecutionStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
            : base(dataAccess, viewModelEngine, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<List<ViewModelAction>> CreateActionsAsync()
        {
            var actions = await base.CreateActionsAsync();

            actions.RemoveAction(DisplayActions.Create);
            actions.RemoveAction(DisplayActions.Delete);

            actions.AddRange(new[]
            {
                new ViewModelAction(DisplayActions.CatchEmAll_ShowAllExecutions),
                new ViewModelAction(DisplayActions.CatchEmAll_ShowFailedExecutions),
                new ViewModelAction(DisplayActions.CatchEmAll_ShowRunningExecutions)
            });

            return actions;
        }

        protected override async Task<EdgeDefinition> ProcessSuccessfulResponseAsync(ViewModelResult response)
        {
            if (response.IsAction(DisplayActions.CatchEmAll_ShowAllExecutions))
                this.Mode = ExecutionDisplayMode.ShowAll;

            if (response.IsAction(DisplayActions.CatchEmAll_ShowFailedExecutions))
                this.Mode = ExecutionDisplayMode.ShowFailed;

            if (response.IsAction(DisplayActions.CatchEmAll_ShowRunningExecutions))
                this.Mode = ExecutionDisplayMode.ShowRunning;

            return await base.ProcessSuccessfulResponseAsync(response);
        }

        protected override Task<Expression<Func<DAL.Entities.QueryExecution, bool>>> PredicateAsync()
        {
            switch (this.Mode)
            {
                default:
                case ExecutionDisplayMode.ShowAll:
                    return base.PredicateAsync();
                case ExecutionDisplayMode.ShowFailed:
                    return Task.FromResult<Expression<Func<DAL.Entities.QueryExecution, bool>>>(e => !e.Successful);
                case ExecutionDisplayMode.ShowRunning:
                    return Task.FromResult<Expression<Func<DAL.Entities.QueryExecution, bool>>>(e => e.End == null);
            }
        }

        protected override Filter CreateDefaultFilter()
        {
            return new Filter
            {
                OrderByProperties = nameof(Models.QueryExecutionSummary.Start),
                OrderByAscending = false
            };
        }
    }
}
