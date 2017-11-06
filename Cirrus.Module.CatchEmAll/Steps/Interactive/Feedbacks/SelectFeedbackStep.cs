using System;
using System.Linq.Expressions;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Feedbacks
{
    [Step]
    public interface ISelectFeedbackStep : ISelectStep
    {
    }

    internal class SelectFeedbackStep : SelectStep<Models.FeedbackSummary, DAL.Entities.Feedback, ICatchEmAllEntityContext>, ISelectFeedbackStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        protected override Expression<Func<DAL.Entities.Feedback, Models.FeedbackSummary>> Selector { get; } = e => new Models.FeedbackSummary
        {
            Id = e.Id,
            Name = e.Name,
            Status = e.Status,
            Created = e.Created,
            Username = e.User.Username
        };

        public SelectFeedbackStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
            : base(dataAccess, viewModelEngine, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override Filter CreateDefaultFilter()
        {
            return new Filter
            {
                OrderByProperties = nameof(Models.FeedbackSummary.Created),
                OrderByAscending = false
            };
        }
    }
}
