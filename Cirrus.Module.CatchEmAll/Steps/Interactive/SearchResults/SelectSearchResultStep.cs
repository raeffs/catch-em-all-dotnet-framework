using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.DAL.Entities;
using Cirrus.Module.Core;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchResults
{
    [Step]
    public interface ISelectSearchResultStep : ISelectStep
    {
        EdgeDefinition UpdateFromSource { get; }

        EdgeDefinition ShowArchive { get; }

        EdgeDefinition EditQuery { get; }

        [Input]
        long QueryId { get; set; }
    }

    internal class SelectSearchResultStep : SelectStep<Models.SearchResultSummary, DAL.Entities.SearchResult, ICatchEmAllEntityContext>, ISelectSearchResultStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition UpdateFromSource { get; } = new EdgeDefinition();

        public EdgeDefinition ShowArchive { get; } = new EdgeDefinition();

        public EdgeDefinition EditQuery { get; } = new EdgeDefinition();

        public long QueryId { get; set; }

        protected override Expression<Func<DAL.Entities.SearchResult, Models.SearchResultSummary>> Selector { get; } = e => new Models.SearchResultSummary
        {
            Id = e.Id,
            Marker = e.New ? "marker:new" : e.Favorite ? "marker:favorite" : "marker:none",
            Name = e.Name,
            BidPrice = e.BidPrice,
            Ends = e.Ends,
            PurchasePrice = e.PurchasePrice,
            LastUpdated = e.Executions.Where(x => x.Successful).OrderByDescending(x => x.End).Select(x => x.End).FirstOrDefault()
        };

        public SelectSearchResultStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
            : base(dataAccess, viewModelEngine, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override Task<Expression<Func<SearchResult, bool>>> PredicateAsync()
        {
            return Task.FromResult<Expression<Func<DAL.Entities.SearchResult, bool>>>(e => e.QueryId == this.QueryId && !e.Hidden && !e.Closed);
        }

        protected override async Task<List<ViewModelAction>> CreateActionsAsync()
        {
            var actions = await base.CreateActionsAsync();

            var indexOfCreate = actions.IndexOf(actions.Single(a => a.Name == DisplayActions.Create.Key));
            actions[indexOfCreate] = new ViewModelAction(DisplayActions.CatchEmAll_UpdateFromSource) { Color = ViewModelActionColor.Primary };

            actions.Add(new ViewModelAction(DisplayActions.CatchEmAll_EditQuery));

            actions.Add(new ViewModelAction(DisplayActions.CatchEmAll_ShowArchive));

            return actions;
        }

        protected override async Task<EdgeDefinition> ProcessSuccessfulResponseAsync(ViewModelResult response)
        {
            if (response.IsAction(DisplayActions.CatchEmAll_UpdateFromSource))
                return this.UpdateFromSource;

            if (response.IsAction(DisplayActions.CatchEmAll_ShowArchive))
                return this.ShowArchive;

            if (response.IsAction(DisplayActions.CatchEmAll_EditQuery))
                return this.EditQuery;

            return await base.ProcessSuccessfulResponseAsync(response);
        }

        protected override Task<Dictionary<string, List<PossibleValue>>> PossibleValuesAsync()
        {
            var values = new Dictionary<string, List<PossibleValue>>
            {
                {
                    nameof(Models.SearchResultSummary.Marker), new List<PossibleValue>
                    {
                        new PossibleValue { Id = "marker:new", Name = "new_releases", Color = "orange" },
                        new PossibleValue { Id = "marker:favorite", Name = "favorite", Color = "lightskyblue" },
                        new PossibleValue { Id = "marker:none" },
                    }
                }
            };

            return Task.FromResult(values);
        }

        protected override Filter CreateDefaultFilter()
        {
            return new Filter
            {
                OrderByProperties = nameof(Models.SearchResultSummary.Ends)
            };
        }
    }
}