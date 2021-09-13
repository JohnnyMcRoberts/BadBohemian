import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainComponentComponent } from './components/main-component/main-component.component';
import { HomeComponent } from './home/home.component';
import { ToDoListComponent } from './Components/ToDo/to-do-list/to-do-list.component';
import { MainTablesPageComponent } from './components/Tables/main-tables-page/main-tables-page.component';
import { MainBooksChartsComponent } from './components/Charts/main-books-charts/main-books-charts.component';
import { MainBooksFormsComponent } from './components/Forms/main-books-forms/main-books-forms.component';

import { MainUserLoginPageComponent } from './components/UserLogin/main-user-login-page/main-user-login-page.component';

import { MainImportExportComponent } from './components/ImportExport/main-import-export/main-import-export.component';

const routes: Routes =
    [
        { path: '', component: HomeComponent, pathMatch: 'full' },
        { path: 'home', component: HomeComponent },

        { path: 'charts', component: MainBooksChartsComponent },
        { path: 'forms', component: MainBooksFormsComponent },
        { path: 'import-export', component: MainImportExportComponent },
        { path: 'tables', component: MainTablesPageComponent },
        { path: 'to-do', component: ToDoListComponent },
        { path: 'user-login', component: MainUserLoginPageComponent },

        { path: 'dashboard', component: MainComponentComponent }
    ];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }

