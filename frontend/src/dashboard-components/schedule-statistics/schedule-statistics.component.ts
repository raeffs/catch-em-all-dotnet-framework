import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Api, formatColor, COLORS_KELLY_MAX_CONTRAST } from '@cirrus/core.webui';

interface Statistics {
    readonly Total: number;
    readonly Enabled: number;
    readonly Disabled: number;
    readonly MissingSchedule: number;
    readonly OutdatedSchedule: number;
}

// tslint:disable:no-magic-numbers
@Component({
    moduleId: __moduleName,
    templateUrl: 'schedule-statistics.component.html',
    styleUrls: ['schedule-statistics.component.css']
})
export class ScheduleStatisticsComponent {

    public chartData: any;

    constructor(
        private api: Api
    ) {
        this.refresh();
    }

    private refresh(): void {
        this.api.get('api/cea/schedules/statistics')
            .toJson<Statistics>()
            .subscribe(data => this.setData(data));
    }

    private setData(data: Statistics): void {
        let chartData = this.chartData;

        if (!chartData) {
            chartData = this.createInitialChartData();
        }

        chartData.data.datasets[0].data = [
            data.Enabled,
            data.Disabled,
            data.MissingSchedule,
            data.OutdatedSchedule
        ];

        this.chartData = chartData;
    }

    private createInitialChartData(): any {
        return {
            type: 'pie',
            options: {
                legend: {
                    display: false
                }
            },
            data: {
                labels: ['Enabled', 'Disabled', 'Missing Schedule', 'Outdated Schedule'],
                datasets: [{
                    backgroundColor: [
                        formatColor(COLORS_KELLY_MAX_CONTRAST[9]),
                        formatColor(COLORS_KELLY_MAX_CONTRAST[6]),
                        formatColor(COLORS_KELLY_MAX_CONTRAST[4]),
                        formatColor(COLORS_KELLY_MAX_CONTRAST[2]),
                    ]
                }]
            }
        };
    }

}
// tslint:enable:no-magic-numbers