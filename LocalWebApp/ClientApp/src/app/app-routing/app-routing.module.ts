import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';


import { HomeComponent } from './../home/home.component';
import { CounterComponent } from './../counter/counter.component';
import { FetchDataComponent } from './../fetch-data/fetch-data.component';

import { MainTablesPageComponent } from './../Components/Tables/main-tables-page/main-tables-page.component';
import { MainBooksFormsComponent } from './../Components/Forms/main-books-forms/main-books-forms.component';
import { MainUserLoginPageComponent } from './../Components/UserLogin/main-user-login-page/main-user-login-page.component';
import { MainImportExportComponent } from './../Components/ImportExport/main-import-export/main-import-export.component';

const routes: Routes =
  [
    { path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'counter', component: CounterComponent },
    { path: 'fetch-data', component: FetchDataComponent },

    //{ path: 'charts', component: MainBooksChartsComponent },
    { path: 'forms', component: MainBooksFormsComponent },
    { path: 'import-export', component: MainImportExportComponent },
    { path: 'tables', component: MainTablesPageComponent },
    //{ path: 'to-do', component: ToDoListComponent },
    { path: 'user-login', component: MainUserLoginPageComponent },

    //{ path: 'dashboard', component: MainComponentComponent }
  ];


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
