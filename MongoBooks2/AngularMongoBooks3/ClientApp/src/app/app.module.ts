import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import {
  MatDialogModule, MatTabsModule, MatProgressBarModule, MatTableModule, MatButtonModule, MatCheckboxModule, MatGridListModule, MatCardModule, MatMenuModule, MatIconModule, MatToolbarModule, MatSidenavModule, MatListModule, MatFormFieldModule, MatInputModule,
  MatSortModule, MatPaginatorModule, } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LayoutModule } from '@angular/cdk/layout';
import { AppRoutingModule } from './app-routing.module';
import { AgmCoreModule } from '@agm/core';
import { FormsModule } from '@angular/forms';
import { MaincomponentComponent } from './components/maincomponent/maincomponent.component';

import { HomeComponent } from './components/home-component/home.component';

import { MainBooksGridsComponent } from './components/Grids/main-books-grids/main-books-grids.component';
import { BooksReadTableComponent } from './components/Grids/books-read-table/books-read-table.component';


import { MainBooksChartsComponent } from './components/Charts/main-books-charts/main-books-charts.component';


import { MainBooksFormsComponent } from './components/Forms/main-books-forms/main-books-forms.component';


@NgModule({
  declarations: [
    AppComponent,
    MaincomponentComponent,



    HomeComponent,

    MainBooksGridsComponent,
    BooksReadTableComponent,

    MainBooksChartsComponent,

    MainBooksFormsComponent,
  ],
  entryComponents: [],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    MatDialogModule, MatTabsModule, MatProgressBarModule, MatTableModule, MatInputModule, MatFormFieldModule, MatButtonModule, MatCheckboxModule, BrowserAnimationsModule, MatGridListModule, MatCardModule, MatMenuModule, MatIconModule, LayoutModule, MatToolbarModule, MatSidenavModule, MatListModule, AppRoutingModule,
    AppRoutingModule,

    MatSortModule, MatPaginatorModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
