//#region Import Modules

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from './angular-material/angular-material.module';
import { AppRoutingModule } from './app-routing/app-routing.module';

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

//#endregion



@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    AngularMaterialModule,
    AppRoutingModule
  ],
  providers:
    [
      DatePipe,
      NoSanitizePipe,

      BooksDataService,
      UserLoginService,
      CurrentLoginService,
      GoogleBookService
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
