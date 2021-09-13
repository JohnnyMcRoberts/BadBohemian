//#region Import Components

import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { MainComponentComponent } from './components/main-component/main-component.component';


import { MainUserLoginPageComponent } from './Components/UserLogin/main-user-login-page/main-user-login-page.component';


import { MainTablesPageComponent } from './Components/Tables/main-tables-page/main-tables-page.component';
import { BooksReadTableComponent } from './components/Tables/books-read-table/books-read-table.component';
import { AuthorsReadTableComponent } from './components/Tables/authors-read-table/authors-read-table.component';
import { LanguageAuthorsTableComponent } from './components/Tables/language-authors-table/language-authors-table.component';
import { CountryAuthorTableComponent } from './components/Tables/country-author-table/country-author-table.component';
import { BookTalliesTableComponent } from './components/Tables/book-tallies-table/book-tallies-table.component';
import { BooksTagsTableComponent } from './components/Tables/books-tags-table/books-tags-table.component';
import { MonthlyTalliesTableComponent } from './components/Tables/monthly-tallies-table/monthly-tallies-table.component';
import { BookDetailComponent } from './components/Tables/book-detail/book-detail.component';
import { SelectedBookDetailComponent } from './components/Tables/selected-book-detail/selected-book-detail.component';


import { MainChartsPageComponent } from './Components/Charts/main-charts-page/main-charts-page.component';
import { MainBooksChartsComponent } from './components/Charts/main-books-charts/main-books-charts.component';
import { BookAndPageRatesComponent } from './components/Charts/book-and-page-rates/book-and-page-rates.component';
import { BooksAndPagesByTimeChartsComponent } from './components/Charts/books-and-pages-by-time-charts/books-and-pages-by-time-charts.component';
import { ByLanguageChartsComponent } from './components/Charts/by-language-charts/by-language-charts.component';
import { ByCountryChartsComponent } from './components/Charts/by-country-charts/by-country-charts.component';
import { CountryMapsComponent } from './components/Charts/country-maps/country-maps.component';


import { MainFormsPageComponent } from './Components/Forms/main-forms-page/main-forms-page.component';
import { MainBooksFormsComponent } from './components/Forms/main-books-forms/main-books-forms.component';
import { AddNewBookComponent } from './components/Forms/add-new-book/add-new-book.component';
import { EditExistingBookComponent } from './components/Forms/edit-existing-book/edit-existing-book.component';
import { GoogleBookItemComponent } from './components/Forms/google-book-item/google-book-item.component';
import { FindBookComponent } from './components/Forms/find-book/find-book.component';


import { MainImportExportPageComponent } from './Components/ImportExport/main-import-export-page/main-import-export-page.component';
import { MainImportExportComponent } from './components/ImportExport/main-import-export/main-import-export.component';
import { TextFileExportComponent } from './components/ImportExport/text-file-export/text-file-export.component';
import { EmailExportComponent } from './components/ImportExport/email-export/email-export.component';


import { LayoutComponent } from './Components/Layout/layout/layout.component';
import { SideNavigationListComponent } from './Components/Layout/side-navigation-list/side-navigation-list.component';
import { NavigationHeaderComponent } from './Components/Layout/navigation-header/navigation-header.component';


import { ToDoListComponent } from './Components/ToDo/to-do-list/to-do-list.component';


//#endregion

//#region Import Services

import { UserLoginService } from './Services/user-login.service';
import { CurrentLoginService } from './Services/current-login.service';
import { BooksDataService } from './Services/books-data.service';

import { GoogleBookService } from './Services/google-book.service';

//#endregion

//#region Import Modules

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { AngularMaterialModule } from './angular-material';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatGridListModule } from '@angular/material/grid-list';

import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LayoutModule } from '@angular/cdk/layout';
import { AppRoutingModule } from './app-routing.module';
import { AgmCoreModule } from '@agm/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PlotlyViaCDNModule } from 'angular-plotly.js';
PlotlyViaCDNModule.plotlyVersion = 'latest';
PlotlyViaCDNModule.plotlyBundle = null;

import { NoSanitizePipe } from './pipes/no-sanitize.pipe';

//#endregion

@NgModule({
    declarations:
        [
            AppComponent,

            NoSanitizePipe,

            NavMenuComponent,
            HomeComponent,
            CounterComponent,
            FetchDataComponent,

            MainTablesPageComponent,
            BooksReadTableComponent,
            AuthorsReadTableComponent,
            LanguageAuthorsTableComponent,
            CountryAuthorTableComponent,
            BookTalliesTableComponent,
            BooksTagsTableComponent,
            MonthlyTalliesTableComponent,
            BookDetailComponent,
            SelectedBookDetailComponent,

            MainChartsPageComponent,
            MainBooksChartsComponent,
            BookAndPageRatesComponent,
            BooksAndPagesByTimeChartsComponent,
            ByLanguageChartsComponent,
            ByCountryChartsComponent,
            CountryMapsComponent,

            MainFormsPageComponent,
            MainBooksFormsComponent,
            AddNewBookComponent,
            EditExistingBookComponent,
            GoogleBookItemComponent,
            FindBookComponent,

            MainImportExportPageComponent,
            MainImportExportComponent,
            TextFileExportComponent,
            EmailExportComponent,

            LayoutComponent,
            SideNavigationListComponent,
            NavigationHeaderComponent,

            ToDoListComponent,

            MainUserLoginPageComponent
        ],
    entryComponents: [
        MainComponentComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,

        AngularMaterialModule,
        MatToolbarModule,
        MatGridListModule,

        OwlDateTimeModule,
        OwlNativeDateTimeModule,

        AgmCoreModule,
        CommonModule,
        PlotlyViaCDNModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        LayoutModule
    ],
    providers:
        [
            DatePipe,
            BooksDataService,
            UserLoginService,
            CurrentLoginService,
            GoogleBookService
        ],
    bootstrap: [AppComponent]
})
export class AppModule { }
