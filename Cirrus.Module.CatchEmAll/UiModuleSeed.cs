using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll
{
    internal class UiModuleSeed : IUiModuleSeed
    {
        public UiModule Seed()
        {
            return new UiModuleBuilder("@cirrus/module.catch-em-all.ui", "CatchEmAllModule", "cea")
                .AddComponent(
                    "ExternalResultDataComponent",
                    "CORE_WORKFLOW_CHILD_COMPONENT",
                    UiComponents.CatchEmAll_ExternalResultDataComponentName,
                    UiComponents.CatchEmAll_ExternalResultDataComponentDescription)
                .AddComponent(
                    "QueryExecutionStatisticsComponent",
                    "CORE_DASHBOARD_COMPONENT",
                    UiComponents.CatchEmAll_QueryExecutionStatisticsComponentName,
                    UiComponents.CatchEmAll_QueryExecutionStatisticsComponentDescription)
                .AddComponent(
                    "ScheduleStatisticsComponent",
                    "CORE_DASHBOARD_COMPONENT",
                    UiComponents.CatchEmAll_ScheduleStatisticsComponentName,
                    UiComponents.CatchEmAll_ScheduleStatisticsComponentDescription)
                .Build();
        }
    }
}
