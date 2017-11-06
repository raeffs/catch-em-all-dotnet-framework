import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

/**
 * Routing module that provides routes for CatchEmAllModule.
 */
@NgModule({
    imports: [RouterModule.forChild([
        /** Add routes to your components here. Note that the routes will be prefixed with 'cea/' */
        // { path: 'example', component: ExampleComponent }
    ])],
    exports: [RouterModule]
})
export class CatchEmAllRoutingModule { }