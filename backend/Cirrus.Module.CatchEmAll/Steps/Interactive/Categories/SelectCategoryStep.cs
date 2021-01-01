using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Categories
{
    [Step]
    public interface ISelectCategoryStep : ISelectStep
    {
        EdgeDefinition ImportExisting { get; }
    }

    internal class SelectCategoryStep : SelectStep<Models.Category, DAL.Entities.Category, ICatchEmAllEntityContext>, ISelectCategoryStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        public EdgeDefinition ImportExisting { get; } = new EdgeDefinition();

        protected override Expression<Func<DAL.Entities.Category, Models.Category>> Selector { get; } = e => new Models.Category
        {
            Id = e.Id,
            Number = e.Number,
            Name = e.Name
        };

        public SelectCategoryStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
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

            using (var context = this.dataAccess.GetContext())
            {
                if ((await context.NoTracking<DAL.Entities.Category>().BelongingToCurrentUser().CountAsync()) == 0)
                {
                    var create = actions.Single(a => a.Name == nameof(DisplayActions.Create));
                    var indexOfCreate = actions.IndexOf(create);
                    actions[indexOfCreate] = new ViewModelAction(DisplayActions.CatchEmAll_ImportExisting) { Color = ViewModelActionColor.Primary };
                    create.Color = ViewModelActionColor.Secondary;
                    actions.Add(create);
                }
            }

            return actions;
        }

        protected override async Task<EdgeDefinition> ProcessSuccessfulResponseAsync(ViewModelResult response)
        {
            if (response.IsAction(DisplayActions.CatchEmAll_ImportExisting))
                return this.ImportExisting;

            return await base.ProcessSuccessfulResponseAsync(response);
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
