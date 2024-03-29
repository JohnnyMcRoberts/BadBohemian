import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';


//#region Import Components

import { MainBooksGridsComponent } from './components/Grids/main-books-grids/main-books-grids.component';
import { BooksReadTableComponent } from './components/Grids/books-read-table/books-read-table.component';
import { AuthorsReadTableComponent } from './components/Grids/authors-read-table/authors-read-table.component';
import { LanguageAuthorsTableComponent } from './components/Grids/language-authors-table/language-authors-table.component';
import { CountryAuthorTableComponent } from './components/Grids/country-author-table/country-author-table.component';
import { BookTalliesTableComponent } from './components/Grids/book-tallies-table/book-tallies-table.component';
import { BooksTagsTableComponent } from './components/Grids/books-tags-table/books-tags-table.component';
import { MonthlyTalliesTableComponent } from './components/Grids/monthly-tallies-table/monthly-tallies-table.component';

import { BookDetailComponent } from './components/Grids/book-detail/book-detail.component';


import { MainBooksChartsComponent } from './components/Charts/main-books-charts/main-books-charts.component';
import { BookAndPageRatesComponent } from './components/Charts/book-and-page-rates/book-and-page-rates.component';
import { BooksAndPagesByTimeChartsComponent } from './components/Charts/books-and-pages-by-time-charts/books-and-pages-by-time-charts.component';
import { ByLanguageChartsComponent } from './components/Charts/by-language-charts/by-language-charts.component';
import { ByCountryChartsComponent } from './components/Charts/by-country-charts/by-country-charts.component';
import { CountryMapsComponent } from './components/Charts/country-maps/country-maps.component';


import { MainBooksFormsComponent } from './components/Forms/main-books-forms/main-books-forms.component';
import { AddNewBookComponent } from './components/Forms/add-new-book/add-new-book.component';
import { EditExistingBookComponent } from './components/Forms/edit-existing-book/edit-existing-book.component';
import { GoogleBookItemComponent } from './components/Forms/google-book-item/google-book-item.component';
import { FindBookComponent } from './components/Forms/find-book/find-book.component';


import { MainImportExportComponent } from './components/ImportExport/main-import-export/main-import-export.component';
import { TextFileExportComponent } from './components/ImportExport/text-file-export/text-file-export.component';
import { ExportToEmailComponent } from './components/ImportExport/export-to-email/export-to-email.component';


import { UserLoginComponent } from './components/Forms/user-login/user-login.component';


import { LayoutComponent } from './Layout/layout/layout.component';
import { SideNavigationListComponent } from './Layout/side-navigation-list/side-navigation-list.component';
import { NavigationHeaderComponent } from './Layout/navigation-header/navigation-header.component';

//#endregion

//#region Import Services

import { UserLoginService } from './Services/user-login.service';
import { CurrentLoginService } from './Services/current-login.service';
import { BooksDataService } from './Services/books-data.service';

import { GoogleBookService } from './Services/google-book.service';

//#endregion

//#region Import Pipes and Modules

import { NoSanitizePipe } from './pipes/no-sanitize.pipe';

import { AngularMaterialModule } from './angular-material';

import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AgmCoreModule } from '@agm/core';
import { ReactiveFormsModule } from '@angular/forms';

import { LayoutModule } from '@angular/cdk/layout';
import { AppRoutingModule } from './app.routing.module';

import { CommonModule } from '@angular/common';

import { PlotlyViaCDNModule } from 'angular-plotly.js';

// choose the full version of the latest release
PlotlyViaCDNModule.plotlyVersion = 'latest';
PlotlyViaCDNModule.plotlyBundle = null;

//#endregion

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,

        NoSanitizePipe,


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

        BookDetailComponent,

        MainBooksChartsComponent,
        BookAndPageRatesComponent,
        BooksAndPagesByTimeChartsComponent,
        ByLanguageChartsComponent,
        ByCountryChartsComponent,
        CountryMapsComponent,

        MainBooksFormsComponent,
        AddNewBookComponent,
        EditExistingBookComponent,
        FindBookComponent,
        GoogleBookItemComponent,

        MainImportExportComponent,
        TextFileExportComponent,
        ExportToEmailComponent,

        UserLoginComponent

    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        
        ReactiveFormsModule,
        
        AngularMaterialModule,

        OwlDateTimeModule,
        OwlNativeDateTimeModule,

        AgmCoreModule,
        BrowserAnimationsModule,
        AppRoutingModule,
        LayoutModule,
        
        CommonModule,
        PlotlyViaCDNModule,

        
        RouterModule.forRoot([
            { path: '', component: UserLoginComponent, pathMatch: 'full' },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },

            { path: 'main-books-grids', component: MainBooksGridsComponent },
            { path: 'main-books-charts', component: MainBooksChartsComponent },
            { path: 'main-books-forms', component: MainBooksFormsComponent },
            { path: 'user-login', component: UserLoginComponent },
            { path: 'import-export', component: MainImportExportComponent }

        ])
    ],
    providers: [
        BooksDataService,
        UserLoginService,
        CurrentLoginService,
        GoogleBookService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
