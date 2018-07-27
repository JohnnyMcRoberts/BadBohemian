import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { BooksComponent } from './books/books.component';
import { PieChartComponent } from './pie-chart/pie-chart.component';
import { AppRoutingModule } from './app-routing.module';
 

@NgModule({
  declarations: [
    AppComponent,
    BooksComponent,
    PieChartComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
