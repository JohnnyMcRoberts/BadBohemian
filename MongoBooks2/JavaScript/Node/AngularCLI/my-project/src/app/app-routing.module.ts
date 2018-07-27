import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BooksComponent } from './books/books.component';
import { PieChartComponent } from './pie-chart/pie-chart.component';

const routes: Routes = [
  { path: 'books', component: BooksComponent },
  { path: 'pie-chart', component: PieChartComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
