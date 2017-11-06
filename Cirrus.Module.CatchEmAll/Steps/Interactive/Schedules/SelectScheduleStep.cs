using System;
using System.Linq.Expressions;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Schedules
{
    [Step]
    public interface ISelectScheduleStep : ISelectStep
    {
    }

    internal class SelectScheduleStep : SelectStep<Models.Schedule, DAL.Entities.Schedule, ICatchEmAllEntityContext>, ISelectScheduleStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        protected override Expression<Func<DAL.Entities.Schedule, Models.Schedule>> Selector { get; } = e => new Models.Schedule
        {
            Id = e.Id,
            CronExpression = e.CronExpression,
            IsDefault = e.IsDefault,
            IsEnabled = e.IsEnabled,
            Type = e.Type,
            UpdateIntervalInMinutes = e.UpdateIntervalInMinutes
        };

        public SelectScheduleStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IDashboard dashboard)
            : base(dataAccess, viewModelEngine, dashboard)
        {
            this.dataAccess = dataAccess;
        }
    }
}
