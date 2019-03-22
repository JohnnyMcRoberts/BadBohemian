import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import
{

  MatAutocompleteModule,
  MatBadgeModule,
  MatBottomSheetModule,
  MatButtonModule,
  MatButtonToggleModule,
  MatCardModule,
  MatCheckboxModule,
  MatChipsModule,
  MatDatepickerModule,
  MatDialogModule,
  MatDividerModule,
  MatExpansionModule,
  MatGridListModule,
  MatIconModule,
  MatInputModule,
  MatListModule,
  MatMenuModule,
  MatNativeDateModule,
  MatPaginatorModule,
  MatProgressBarModule,
  MatProgressSpinnerModule,
  MatRadioModule,
  MatRippleModule,
  MatSelectModule,
  MatSidenavModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSnackBarModule,
  MatSortModule,
  MatStepperModule,
  MatTableModule,
  MatTabsModule,
  MatToolbarModule,
  MatTooltipModule,
  MatTreeModule,

  MatFormFieldModule

} from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LayoutModule } from '@angular/cdk/layout';
import { AppRoutingModule } from './app-routing.module';
import { AgmCoreModule } from '@agm/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { PlotlyModule } from 'angular-plotly.js';

import { NoSanitizePipe } from './pipes/no-sanitize.pipe';


import { MaincomponentComponent } from './components/maincomponent/maincomponent.component';

import { HomeComponent } from './components/home-component/home.component';

import { MainBooksGridsComponent } from './components/Grids/main-books-grids/main-books-grids.component';
import { BooksReadTableComponent } from './components/Grids/books-read-table/books-read-table.component';
import { AuthorsReadTableComponent } from './components/Grids/authors-read-table/authors-read-table.component';
import { LanguageAuthorsTableComponent } from './components/Grids/language-authors-table/language-authors-table.component';
import { CountryAuthorTableComponent } from './components/Grids/country-author-table/country-author-table.component';
import { BookTalliesTableComponent } from './components/Grids/book-tallies-table/book-tallies-table.component';
import { BooksTagsTableComponent } from './components/Grids/books-tags-table/books-tags-table.component';
import { MonthlyTalliesTableComponent } from './components/Grids/monthly-tallies-table/monthly-tallies-table.component';


import { MainBooksChartsComponent } from './components/Charts/main-books-charts/main-books-charts.component';
import { BookAndPageRatesComponent } from './components/Charts/book-and-page-rates/book-and-page-rates.component';


import { MainBooksFormsComponent } from './components/Forms/main-books-forms/main-books-forms.component';
import { AddNewBookComponent } from './components/Forms/add-new-book/add-new-book.component';


import { LayoutComponent } from './Layout/layout/layout.component';
import { SideNavigationListComponent } from './Layout/side-navigation-list/side-navigation-list.component';
import { NavigationHeaderComponent } from './Layout/navigation-header/navigation-header.component';



@NgModule({
  declarations:
  [
    AppComponent,

    NoSanitizePipe,

    MaincomponentComponent,


    LayoutComponent,
    SideNavigationListComponent,
    NavigationHeaderComponent,

    MainBooksGridsComponent,
    BooksReadTableComponent,
    AuthorsReadTableComponent,
    LanguageAuthorsTableComponent,
    CountryAuthorTableComponent,
    BookTalliesTableComponent,
    MonthlyTalliesTableComponent,
    BooksTagsTableComponent,

    MainBooksChartsComponent,
    BookAndPageRatesComponent,

    MainBooksFormsComponent,
    AddNewBookComponent,


    HomeComponent
  ],
  entryComponents: [],
  imports:
  [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,

    MatAutocompleteModule,
    MatBadgeModule,
    MatBottomSheetModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatStepperModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatTreeModule,

    MatFormFieldModule,


    PlotlyModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
