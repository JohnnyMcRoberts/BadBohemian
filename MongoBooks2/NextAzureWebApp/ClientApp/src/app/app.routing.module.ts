import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainBooksGridsComponent } from './components/Grids/main-books-grids/main-books-grids.component';
import { MainBooksChartsComponent } from './components/Charts/main-books-charts/main-books-charts.component';
import { MainBooksFormsComponent } from './components/Forms/main-books-forms/main-books-forms.component';

import { UserLoginComponent } from './components/Forms/user-login/user-login.component';

import { MainImportExportComponent } from './components/ImportExport/main-import-export/main-import-export.component';

const routes: Routes =
    [
        { path: '', component: UserLoginComponent, pathMatch: 'full' },
        { path: 'main-books-grids', component: MainBooksGridsComponent },
        { path: 'main-books-charts', component: MainBooksChartsComponent },
        { path: 'main-books-forms', component: MainBooksFormsComponent },
        { path: 'user-login', component: UserLoginComponent },
        { path: 'import-export', component: MainImportExportComponent },
    ];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
