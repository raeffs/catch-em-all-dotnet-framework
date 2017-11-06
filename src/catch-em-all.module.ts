import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { WebAppModule } from '@cirrus/core.webui';

import { CatchEmAllRoutingModule } from './catch-em-all-routing.module';
import { ExternalResultDataComponent } from './components/external-result-data.component';
import { QueryExecutionStatisticsComponent } from './dashboard-components/query-executions-statistics/query-executions-statistics.component';
import { ScheduleStatisticsComponent } from './dashboard-components/schedule-statistics/schedule-statistics.component';

@NgModule({
    imports: [
        CommonModule,
        WebAppModule,
        CatchEmAllRoutingModule,
    ],
    declarations: [
        ExternalResultDataComponent,
        QueryExecutionStatisticsComponent,
        ScheduleStatisticsComponent
    ],
    entryComponents: [
        ExternalResultDataComponent,
        QueryExecutionStatisticsComponent,
        ScheduleStatisticsComponent
    ]
})
export class CatchEmAllModule { }