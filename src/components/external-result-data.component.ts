import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { WorkflowChildComponentBase } from '@cirrus/module.core.ui';

@Component({
    moduleId: __moduleName,
    templateUrl: 'external-result-data.component.html',
    styleUrls: ['external-result-data.component.css']
})
export class ExternalResultDataComponent extends WorkflowChildComponentBase {

    private externalUrl: string;

    public openExternalUrl(): void {
        window.open(this.externalUrl);
    }

    protected onChildViewModelChanged(): void {
        // there is only one row, so we can extract the external url from the first item in the table
        const externalUrlColumn = this.childViewModel.Table.Columns.filter(c => c.PropertyName === 'ExternalUrl')[0];
        const externalUrlColumnIndex = this.childViewModel.Table.Columns.indexOf(externalUrlColumn);
        this.externalUrl = this.childViewModel.Table.Rows[0].Values[externalUrlColumnIndex];
    }

}