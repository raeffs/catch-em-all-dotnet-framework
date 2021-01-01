using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.DAL.Entities;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchResults
{
    [Step]
    public interface ISelectArchivedSearchResultStep : ISelectStep
    {
        [Input]
        long QueryId { get; set; }
    }

    internal class SelectArchivedSearchResultStep : SelectStep<Models.ArchivedSearchResultSummary, DAL.Entities.SearchResult, ICatchEmAllEntityContext>, ISelectArchivedSearchResultStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public long QueryId { get; set; }

        protected override Expression<Func<DAL.Entities.SearchResult, Models.ArchivedSearchResultSummary>> Selector { get; } = e => new Models.ArchivedSearchResultSummary
        {
            Id = e.Id,
            Name = e.Name,
            BidPrice = e.BidPrice,
            FinalPrice = e.FinalPrice,
            PurchasePrice = e.PurchasePrice,
            Sold = e.Sold,
            Ended = e.Ends,
            LastUpdated = e.Executions.Where(x => x.Successful).OrderByDescending(x => x.End).Select(x => x.End).FirstOrDefault()
        };

        public SelectArchivedSearchResultStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
            : base(dataAccess, viewModelEngine, dashboard)
        {
            this.dataAccess = dataAccess;
            this.Filter = this.CreateFilter();
        }

        protected override Task<Expression<Func<SearchResult, bool>>> PredicateAsync()
        {
            return Task.FromResult<Expression<Func<DAL.Entities.SearchResult, bool>>>(e => e.QueryId == this.QueryId && !e.Hidden && e.Closed);
        }

        protected override async Task<List<ViewModelAction>> CreateActionsAsync()
        {
            var actions = await base.CreateActionsAsync();

            actions.RemoveAction(DisplayActions.Create);

            return actions;
        }

        private Filter CreateFilter()
        {
            return new Filter
            {
                OrderByProperties = nameof(Models.ArchivedSearchResultSummary.Ended)
            };
        }
    }
}
