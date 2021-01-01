using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.SearchQueries
{
    [Step]
    public interface ISelectSearchQueryStep : ISelectStep
    {
        [Input(Optional = true)]
        long? CategoryId { get; set; }
    }

    internal class SelectSearchQueryStep : SelectStep<Models.SearchQuerySummary, DAL.Entities.SearchQuery, ICatchEmAllEntityContext>, ISelectSearchQueryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public long? CategoryId { get; set; }

        protected override Expression<Func<DAL.Entities.SearchQuery, Models.SearchQuerySummary>> Selector { get; } = e => new Models.SearchQuerySummary
        {
            Id = e.Id,
            Name = e.Name,
            Category = e.Category.Name,
            NumberOfResults = e.Results.Count(r => !r.Hidden),
            NumberOfOpenResults = e.Results.Count(r => !r.Hidden && !r.Closed),
            AverageFinalPrice = e.Results.Where(r => !r.Hidden && r.Sold).Average(r => r.FinalPrice),
            LastUpdated = e.Executions.Where(x => x.Successful).OrderByDescending(x => x.End).Select(x => x.End).FirstOrDefault(),
            EnableNotifications = e.EnableNotifications,
            Tags = e.TagValues
        };

        public SelectSearchQueryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
            : base(dataAccess, viewModelEngine, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override Task<Expression<Func<DAL.Entities.SearchQuery, bool>>> PredicateAsync()
        {
            var userId = Context.CurrentUserInfo.Id;
            return Task.FromResult<Expression<Func<DAL.Entities.SearchQuery, bool>>>(e => e.Category.User.UserId == userId && (!this.CategoryId.HasValue || e.CategoryId == this.CategoryId.Value) && !e.Hidden);
        }

        protected override Filter CreateDefaultFilter()
        {
            return new Filter
            {
                OrderByProperties = nameof(Models.SearchQuery.Name),
                OrderByAscending = true
            };
        }
    }
}
