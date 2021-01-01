import { NgModule } from '@angular/core';
import { RouterModule, PreloadAllModules } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';

import { WebAppModule, WebAppComponent, CIRRUS_DEV_MODULE_ID_SOURCE, DevModuleId } from '@cirrus/core.webui';
import { CoreModule } from '@cirrus/module.core.ui';
import { CoreDashboardModule } from '@cirrus/module.core.ui.dashboard';

import { CatchEmAllModule } from './catch-em-all.module';

/**
 * Application module only used for development.
 * This module is bootstraped when the application is started in development mode.
 * It will not be exported.
 */
@NgModule({
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'core-dashboard/dashboard'
            },
            {
                path: 'cea',
                loadChildren: () => CatchEmAllModule
            },
            {
                path: 'core',
                loadChildren: () => CoreModule
            },
            {
                path: 'core-dashboard',
                loadChildren: () => CoreDashboardModule
            },
            {
                path: '**',
                redirectTo: 'demo/component-demo'
            }
        ], { preloadingStrategy: PreloadAllModules, initialNavigation: false }),
        WebAppModule.forBusinessDevRoot()
    ],
    bootstrap: [
        WebAppComponent
    ],
    providers: [
        { provide: CIRRUS_DEV_MODULE_ID_SOURCE, useValue: <DevModuleId>{ moduleName: '@cirrus/module.catch-em-all.ui', exportName: 'CatchEmAllModule', routePrefix: 'cea' } },
    ]
})
export class AppModule { }