import { Component, Input, Output, EventEmitter, ViewChild, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/observable/interval';
import moment from 'moment';

import { Api, ChartComponent, formatColor, COLORS_KELLY_MAX_CONTRAST, Color, I18n, I18nDatetimeFormat } from '@cirrus/core.webui';
import { DashboardConfig } from '@cirrus/module.core.ui.dashboard';

interface ComponentConfiguration {
    readonly Type?: string;
}

interface Statistics {
    readonly Minute?: string;
    readonly Hour?: string;
    readonly Day?: string;
    readonly Total: number;
    readonly Successful: number;
    readonly Failed: number;
}

enum StatisticsType {
    Hourly,
    Daily,
    Weekly
}

const HOURLY_STATISTICS_URL = 'api/cea/queryexecutions/statistics/hourly';
const DAILY_STATISTICS_URL = 'api/cea/queryexecutions/statistics/daily';
const WEEKLY_STATISTICS_URL = 'api/cea/queryexecutions/statistics/weekly';

// tslint:disable:no-magic-numbers
@Component({
    moduleId: __moduleName,
    templateUrl: 'query-executions-statistics.component.html',
    styleUrls: ['query-executions-statistics.component.css']
})
export class QueryExecutionStatisticsComponent implements OnDestroy {

    private readonly refreshSubscription: Subscription;
    private readonly componentConfiguration: ComponentConfiguration;

    @ViewChild(ChartComponent)
    private chart: ChartComponent;

    public chartData: any;

    public get titleKey(): string {
        switch (StatisticsType[this.componentConfiguration.Type || StatisticsType.Hourly]) {
            case StatisticsType.Weekly:
                return 'CatchEmAll_QueryExecutionsPastWeek';
            case StatisticsType.Daily:
                return 'CatchEmAll_QueryExecutionsPastDay';
            case StatisticsType.Hourly:
            default:
                return 'CatchEmAll_QueryExecutionsPastHour';
        }
    }

    constructor(
        private api: Api,
        private i18n: I18n,
        private dashboardConfiguration: DashboardConfig
    ) {
        this.componentConfiguration = JSON.parse(dashboardConfiguration.config);
        this.refreshSubscription = Observable.interval(60 * 1000).subscribe(() => this.refresh());
        this.refresh();
    }

    public ngOnDestroy(): void {
        this.refreshSubscription.unsubscribe();
    }

    private refresh(): void {
        const url = this.getUrl();
        this.api.get(url)
            .toJson<Statistics[]>()
            .subscribe(data => this.setData(data));
    }

    private setData(data: Statistics[]): void {
        let chartData = this.chartData;

        if (!chartData) {
            chartData = this.createInitialChartData();
        }

        switch (StatisticsType[this.componentConfiguration.Type || StatisticsType.Hourly]) {
            case StatisticsType.Weekly:
                chartData.data.labels = data.map(x => x.Day);
                break;
            case StatisticsType.Daily:
                chartData.data.labels = data.map(x => x.Hour);
                break;
            case StatisticsType.Hourly:
            default:
                chartData.data.labels = data.map(x => x.Minute);
                break;
        }

        chartData.data.datasets[0].data = data.map(x => x.Successful);
        chartData.data.datasets[1].data = data.map(x => x.Failed);

        this.chartData = chartData;
        this.chart.update();
    }

    private getUrl(): string {
        switch (StatisticsType[this.componentConfiguration.Type || StatisticsType.Hourly]) {
            case StatisticsType.Weekly:
                return WEEKLY_STATISTICS_URL;
            case StatisticsType.Daily:
                return DAILY_STATISTICS_URL;
            case StatisticsType.Hourly:
            default:
                return HOURLY_STATISTICS_URL;
        }
    }

    private formatXAxesLabel(value: string, index: number): string {
        switch (StatisticsType[this.componentConfiguration.Type || StatisticsType.Hourly]) {
            case StatisticsType.Weekly:
                return index === 1 || index === 6 ? this.i18n.formatDatetime(value, I18nDatetimeFormat.Date) : '';
            case StatisticsType.Daily:
                const hours = moment(value).hours();
                return hours % 6 === 0 ? this.i18n.formatDatetime(value, I18nDatetimeFormat.Time) : '';
            case StatisticsType.Hourly:
            default:
                const minutes = moment(value).minutes();
                return minutes % 15 === 0 ? this.i18n.formatDatetime(value, I18nDatetimeFormat.Time) : '';
        }
    }

    private formatTooltipTitle(item: any, chart: any): string {
        switch (StatisticsType[this.componentConfiguration.Type || StatisticsType.Hourly]) {
            case StatisticsType.Weekly:
                return this.i18n.formatDatetime(chart.labels[item[0].index], I18nDatetimeFormat.Date);
            case StatisticsType.Daily:
            case StatisticsType.Hourly:
            default:
                return this.i18n.formatDatetime(chart.labels[item[0].index], I18nDatetimeFormat.Time);
        }
    }

    private createInitialChartData(): any {
        return {
            type: 'line',
            options: {
                legend: {
                    display: false
                },
                scales: {
                    xAxes: [
                        {
                            ticks: {
                                fontColor: 'grey',
                                maxRotation: 0,
                                autoSkip: false,
                                callback: (value, index, values) => this.formatXAxesLabel(value, index)
                            },
                            gridLines: {
                                display: false
                            }
                        }
                    ],
                    yAxes: [
                        {
                            ticks: {
                                fontColor: 'grey',
                                beginAtZero: true,
                                mirror: false,
                                callback: (value, index, values) => `${value}   `
                            },
                            gridLines: {
                                color: 'grey',
                                zeroLineColor: 'grey',
                                drawBorder: false,
                                drawTicks: false,
                                borderDash: [2, 2],
                                zeroLineBorderDash: [2, 2]
                            }
                        }
                    ]
                },
                tooltips: {
                    mode: 'index',
                    position: 'nearest',
                    intersect: false,
                    cornerRadius: 0,
                    callbacks: {
                        title: (item, chart) => this.formatTooltipTitle(item, chart)
                    }
                }
            },
            data: {
                labels: [],
                datasets: [
                    createEmptyDataset('Successful', COLORS_KELLY_MAX_CONTRAST[9]),
                    createEmptyDataset('Failed', COLORS_KELLY_MAX_CONTRAST[4])
                ]
            }
        };
    }

}

function createEmptyDataset(label: string, color: Color): any {
    return {
        label,
        data: [],
        fill: false,
        borderColor: formatColor(color),
        pointBackgroundColor: formatColor(color),
        pointBorderColor: 'white',
        pointBorderWidth: 1,
        pointRadius: 3,
        borderWidth: 2,
        lineTension: 0.1,
    };
}
// tslint:enable:no-magic-numbers