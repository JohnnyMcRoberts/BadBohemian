//#region Import Modules

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from './angular-material/angular-material.module';
import { AppRoutingModule } from './app-routing/app-routing.module';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatFormFieldModule } from '@angular/material/form-field';

//#endregion

//#region Import Services

import { UserLoginService } from './Services/user-login.service';
import { CurrentLoginService } from './Services/current-login.service';
import { BooksDataService } from './Services/books-data.service';
import { GoogleBookService } from './Services/google-book.service';

//#endregion

//#region Import Pipes

import { DatePipe } from '@angular/common';
import { NoSanitizePipe } from './Pipes/no-sanitize.pipe';

//#endregion

//#region Import Components

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

// Layout
import { LayoutComponent } from './Components/Layout/layout/layout.component';
import { SideNavigationListComponent } from './Components/Layout/side-navigation-list/side-navigation-list.component';
import { NavigationHeaderComponent } from './Components/Layout/navigation-header/navigation-header.component';

// UserLogin
import { MainUserLoginPageComponent } from './Components/UserLogin/main-user-login-page/main-user-login-page.component';

// Forms
import { MainFormsPageComponent } from './Components/Forms/main-forms-page/main-forms-page.component';
import { MainBooksFormsComponent } from './Components/Forms/main-books-forms/main-books-forms.component';
import { AddNewBookComponent } from './Components/Forms/add-new-book/add-new-book.component';
import { EditExistingBookComponent } from './Components/Forms/edit-existing-book/edit-existing-book.component';
import { GoogleBookItemComponent } from './Components/Forms/google-book-item/google-book-item.component';
import { FindBookComponent } from './Components/Forms/find-book/find-book.component';

// Tables
import { MainTablesPageComponent } from './Components/Tables/main-tables-page/main-tables-page.component';
import { BooksReadTableComponent } from './Components/Tables/books-read-table/books-read-table.component';
import { AuthorsReadTableComponent } from './Components/Tables/authors-read-table/authors-read-table.component';
import { LanguageAuthorsTableComponent } from './Components/Tables/language-authors-table/language-authors-table.component';
import { CountryAuthorTableComponent } from './Components/Tables/country-author-table/country-author-table.component';
import { BookTalliesTableComponent } from './Components/Tables/book-tallies-table/book-tallies-table.component';
import { BooksTagsTableComponent } from './Components/Tables/books-tags-table/books-tags-table.component';
import { MonthlyTalliesTableComponent } from './Components/Tables/monthly-tallies-table/monthly-tallies-table.component';
import { BookDetailComponent } from './Components/Tables/book-detail/book-detail.component';
import { SelectedBookDetailComponent } from './Components/Tables/selected-book-detail/selected-book-detail.component';

// Import/Export
import { MainImportExportPageComponent } from './Components/ImportExport/main-import-export-page/main-import-export-page.component';
import { MainImportExportComponent } from './Components/ImportExport/main-import-export/main-import-export.component';
import { TextFileExportComponent } from './Components/ImportExport/text-file-export/text-file-export.component';
import { EmailExportComponent } from './Components/ImportExport/email-export/email-export.component';

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

            LayoutComponent,
            SideNavigationListComponent,
            NavigationHeaderComponent,

            MainFormsPageComponent,
            MainBooksFormsComponent,
            AddNewBookComponent,
            EditExistingBookComponent,
            GoogleBookItemComponent,
            FindBookComponent,

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

            MainImportExportPageComponent,
            MainImportExportComponent,
            TextFileExportComponent,
            EmailExportComponent,

            MainUserLoginPageComponent
        ],
    imports:
        [
            BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
            FormsModule,
            ReactiveFormsModule,
            HttpClientModule,

            AngularMaterialModule,

            MatToolbarModule,
            MatGridListModule,
            MatFormFieldModule,

            AppRoutingModule,
            BrowserAnimationsModule
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
