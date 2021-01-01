using System.Collections.Generic;
using Cirrus.Engine.Workflow;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.Workflows.Interactive;
using Cirrus.Module.CatchEmAll.Workflows.NonInteractive;

namespace Cirrus.Module.CatchEmAll
{
    public class WorkflowSeed : WorkflowSeed<ICatchEmAllEntityContext>
    {
        public override List<Workflow> Seed()
        {
            return new List<Workflow>
            {
                CategoryWorkflow.Build(BusinessRoles.UserRole),
                SearchQueryWorkflow.Build(BusinessRoles.UserRole),
                SearchQueryByCategoryWorkflow.Build(BusinessRoles.UserRole),
                SearchQueryByCategoryIdWorkflow.Build(BusinessRoles.UserRole),
                SearchQueryByIdWorkflow.Build(BusinessRoles.UserRole),
                SettingsWorkflow.Build(BusinessRoles.UserRole),
                FeedbackWorkflow.Build(BusinessRoles.UserRole),
                ScheduleWorkflow.Build(BusinessRoles.AdminRole),
                QueryExecutionWorkflow.Build(BusinessRoles.AdminRole),
                ExecuteScheduleWorkflow.Build()
            };
        }
    }
}
