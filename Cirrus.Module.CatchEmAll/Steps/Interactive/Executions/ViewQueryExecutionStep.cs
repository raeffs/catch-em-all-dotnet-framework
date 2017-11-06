using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.DAL;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.ViewModel.Extended;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Helper;
using Cirrus.Module.Core;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive.Executions
{
    [Step]
    public interface IViewQueryExecutionStep : IEditStep
    {
    }

    internal class ViewQueryExecutionStep : EditStep<Models.QueryExecution, DAL.Entities.QueryExecution, ICatchEmAllEntityContext>, IViewQueryExecutionStep
    {
        private readonly IDataAccess<ICatchEmAllEntityContext> dataAccess;

        internal ViewQueryExecutionStep(IDataAccess<ICatchEmAllEntityContext> dataAccess, IViewModelEngine<ICatchEmAllEntityContext> viewModelEngine, IViewModelExtensionFactory<DAL.Entities.QueryExecution> viewmodelExtensions, IDashboard dashboard)
            : base(viewModelEngine, viewmodelExtensions, dashboard)
        {
            this.dataAccess = dataAccess;
        }

        protected override async Task<Models.QueryExecution> LoadAsync()
        {
            using (var context = this.dataAccess.GetContext())
            {
                return await context.NoTracking<DAL.Entities.QueryExecution>()
                    .Where(e => e.Id == this.Id)
                    .Select(e => new Models.QueryExecution
                    {
                        Id = e.Id,
                        Start = e.Start,
                        End = e.End,
                        Successful = e.Successful,
                        IsUserInitiated = e.IsUserInitiated,
                        Message = e.Message
                    })
                    .SingleAsync();
            }
        }

        protected override async Task<List<ViewModelAction>> CreateActionsAsync(Models.QueryExecution dto)
        {
            var actions = await base.CreateActionsAsync(dto);

            actions.RemoveAction(DisplayActions.Save);

            return actions;
        }

        protected override Task SaveAsync(Models.QueryExecution dto)
        {
            throw new NotImplementedException();
        }
    }
}
