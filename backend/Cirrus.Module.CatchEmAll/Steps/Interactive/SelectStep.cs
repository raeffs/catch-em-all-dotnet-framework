using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Localisation;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.Core;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive
{
    /// <summary>
    /// Step to select a <typeparamref name="TModel" />.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TIEntityContext">The type of the i entity context.</typeparam>
    [Step]
    public abstract class SelectStep<TModel, TEntity, TIEntityContext> : IWorkflowStep, ISelectStep
        where TIEntityContext : class, DAL.ICatchEmAllEntityContext
        where TModel : class, IHasIndex
        where TEntity : class, IHasIndex
    {
        private readonly IViewModelEngine<TIEntityContext> viewModelEngine;
        private readonly IDataAccess<DAL.ICatchEmAllEntityContext> dataAccess;
        private readonly IDashboard dashboard;

        /// <inheritdoc />
        public EdgeDefinition Back { get; } = new EdgeDefinition();

        /// <inheritdoc />
        public EdgeDefinition Canceled { get; } = new EdgeDefinition();

        /// <inheritdoc />
        public EdgeDefinition Create { get; } = new EdgeDefinition();

        /// <inheritdoc />
        public EdgeDefinition Edit { get; } = new EdgeDefinition();

        /// <inheritdoc />
        public EdgeDefinition Delete { get; } = new EdgeDefinition();

        /// <inheritdoc />
        public bool CanGoBack { get; set; } = true;

        /// <inheritdoc />
        public string HelpId { get; set; }

        /// <inheritdoc />
        public long SingleSelectId { get; set; }

        /// <inheritdoc />
        public ICollection<long> MultiSelectIds { get; set; }

        /// <inheritdoc />
        public Filter Filter { get; set; }

        /// <inheritdoc />
        public string DashboardComponentName { get; set; }

        /// <inheritdoc />
        public string DashboardComponentConfiguration { get; set; }

        /// <inheritdoc />
        public AddToDashboardOptions DashboardComponentOptions { get; set; }

        /// <inheritdoc />
        public string WorkflowStepId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectStep{TModel, TEntity, TIEntityContext}" /> class.
        /// </summary>
        /// <param name="dataAccess">The data access.</param>
        /// <param name="viewModelEngine">The view model engine.</param>
        /// <param name="dashboard">The dashboard.</param>
        public SelectStep(IDataAccess<DAL.ICatchEmAllEntityContext> dataAccess, IViewModelEngine<TIEntityContext> viewModelEngine, IDashboard dashboard)
        {
            this.dataAccess = dataAccess;
            this.viewModelEngine = viewModelEngine;
            this.dashboard = dashboard;
        }

        /// <inheritdoc />
        public async Task<EdgeDefinition> RunAsync()
        {
            this.Filter = await FilterHelper.LoadFilterOrCreateDefaultAsync(this.dataAccess, this.WorkflowStepId, () => this.CreateDefaultFilter());

            while (true)
            {
                var predicate = await this.PredicateAsync();

                var response = await this.viewModelEngine.SelectAsync(this.Selector, new SelectArguments<TEntity>
                {
                    Actions = await this.CreateActionsAsync(),
                    Predicate = predicate,
                    PossibleValues = await this.PossibleValuesAsync(),
                    PreSelected = this.MultiSelectIds,
                    Filter = this.Filter,
                    HelpId = !string.IsNullOrWhiteSpace(this.HelpId) ? new Guid(this.HelpId) : (Guid?)null,
                });

                this.Filter = response.UsedFilter;
                await FilterHelper.SaveFilterAsync(this.dataAccess, this.WorkflowStepId, this.Filter);

                if (response.Result == DisplayResult.Canceled)
                    return this.Canceled;

                if (response.Result == DisplayResult.Success)
                {
                    var next = await this.ProcessSuccessfulResponseAsync(response);
                    if (next == null)
                        continue;
                    else
                        return next;
                }

                throw new Exception($"Selecting of '{typeof(TModel).Name}' not successful ({response.Result}, {response.Action})");
            }
        }

        /// <summary>
        /// Gets the possibles the values asynchronously.
        /// </summary>
        /// <returns>Possible values.</returns>
        protected virtual Task<Dictionary<string, List<PossibleValue>>> PossibleValuesAsync()
            => Task.FromResult(new Dictionary<string, List<PossibleValue>>());

        protected virtual Filter CreateDefaultFilter()
            => new Filter { OrderByProperties = nameof(IHasIndex.Id) };

        /// <summary>
        /// Processes the successful response asynchronously.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task<EdgeDefinition> ProcessSuccessfulResponseAsync(ViewModelResult response)
        {
            if (response.IsAction(DisplayActions.Create))
                return this.Create;

            if (response.IsAction(DisplayActions.Back))
                return this.Back;

            if (response.IsAction(DisplayActions.Delete))
            {
                if (response.Ids != null && response.Ids.Count > 0)
                {
                    this.MultiSelectIds = response.Ids;
                    return this.Delete;
                }
            }

            if (response.IsAction(DisplayActions.Next))
            {
                var singleId = response.Ids?.FirstOrDefault() ?? 0;
                if (singleId > 0)
                {
                    this.SingleSelectId = singleId;
                    return this.Edit;
                }
            }

            if (response.IsAction(DisplayActions.AddToDashboard))
            {
                await this.dashboard.AddComponentAsync(this.DashboardComponentName, this.DashboardComponentConfiguration, this.DashboardComponentOptions);
                return null;
            }

            return null;
        }

        /// <summary>
        /// Creates the actions asynchronously every time a new request is sent to the client.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation and contains the actions.</returns>
        protected virtual Task<List<ViewModelAction>> CreateActionsAsync()
        {
            var actions = new[]
            {
                    new ViewModelAction(DisplayActions.Create) { Color = ViewModelActionColor.Primary },
                    new ViewModelAction(DisplayActions.Next) { Type = ViewModelActionType.SingleSelect }, // Edit
                    new ViewModelAction(DisplayActions.Delete) { Type = ViewModelActionType.Delete, Color = ViewModelActionColor.Warn },
            }.ToList();

            if (this.CanGoBack)
                actions.Add(new ViewModelAction(DisplayActions.Back) { Type = ViewModelActionType.Back });

            if (this.dashboard.GetType().Name != "NoDashboard" && !string.IsNullOrWhiteSpace(this.DashboardComponentName))
                actions.Add(new ViewModelAction(DisplayActions.AddToDashboard) { Type = ViewModelActionType.Toolbar, Icon = "favorite" });

            return Task.FromResult(actions);
        }

        /// <summary>
        /// Gets the selector expression for the model conversion.
        /// </summary>
        /// <returns>The selector expression.</returns>
        protected virtual Task<Expression<Func<TEntity, bool>>> PredicateAsync()
        {
            Expression<Func<TEntity, bool>> exp = (c) => c.Id != 0;
            return Task.FromResult(exp);
        }

        /// <summary>
        /// Gets the selector expression for the model conversion.
        /// </summary>
        protected abstract Expression<Func<TEntity, TModel>> Selector { get; }
    }
}
