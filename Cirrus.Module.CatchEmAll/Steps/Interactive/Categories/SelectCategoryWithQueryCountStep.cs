using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Categories
{
    [Step]
    public interface ISelectCategoryWithQueryCountStep : ISelectStep
    {
    }

    internal class SelectCategoryWithQueryCountStep : SelectStep<Models.CategoryWithQueryCount, DAL.Entities.Category, ICatchEmAllEntityContext>, ISelectCategoryWithQueryCountStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        protected override Expression<Func<DAL.Entities.Category, Models.CategoryWithQueryCount>> Selector { get; } = e => new Models.CategoryWithQueryCount
        {
            Id = e.Id,
            Number = e.Number,
            Name = e.Name,
            NumberOfQueries = e.Queries.Where(q => !q.Hidden).Count()
        };

        public SelectCategoryWithQueryCountStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
            : base(dataAccess, viewModelEngine, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override Task<Expression<Func<DAL.Entities.Category, bool>>> PredicateAsync()
        {
            var userId = Context.CurrentUserInfo.Id;
            return Task.FromResult<Expression<Func<DAL.Entities.Category, bool>>>(e => e.User.UserId == userId);
        }

        protected override async Task<List<ViewModelAction>> CreateActionsAsync()
        {
            var actions = await base.CreateActionsAsync();

            var create = actions.Single(a => a.Name == DisplayActions.Create.Key);
            actions.Remove(create);

            var delete = actions.Single(a => a.Name == DisplayActions.Delete.Key);
            actions.Remove(delete);

            return actions;
        }

        protected override Filter CreateDefaultFilter()
        {
            return new Filter
            {
                OrderByProperties = nameof(Models.Category.Name),
                OrderByAscending = true
            };
        }
    }
}
