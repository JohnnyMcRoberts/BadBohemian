import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { DataService } from './data.service';
import { BooksReadDataService } from './books-read-data.service';
import { TextDataService } from './text-data.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [
    DataService,
    BooksReadDataService,
    TextDataService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
