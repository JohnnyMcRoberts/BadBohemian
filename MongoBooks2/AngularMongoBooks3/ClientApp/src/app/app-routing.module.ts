import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
 
import { MaincomponentComponent } from './components/maincomponent/maincomponent.component';
import { HomeComponent } from './components/home-component/home.component';
import { MainBooksGridsComponent } from './components/Grids/main-books-grids/main-books-grids.component';
import { MainBooksChartsComponent } from './components/Charts/main-books-charts/main-books-charts.component';
import { MainBooksFormsComponent } from './components/Forms/main-books-forms/main-books-forms.component';

const routes: Routes =
[
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'dashboard', component: MaincomponentComponent },
  { path: 'to-do-list', component: HomeComponent },
  { path: 'main-books-grids', component: MainBooksGridsComponent },
  { path: 'main-books-charts', component: MainBooksChartsComponent },
  { path: 'main-books-forms', component: MainBooksFormsComponent },
];
 
@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}
