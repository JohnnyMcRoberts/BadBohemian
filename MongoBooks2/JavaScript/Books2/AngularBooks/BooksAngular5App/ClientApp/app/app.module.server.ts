import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { AppModuleShared } from './app.module.shared';
import { AppComponent } from './components/app/app.component';
import { IBookRead } from './interfaces/IBookRead';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
    bootstrap: [ AppComponent ],
  imports: [
    ServerModule,
    HttpClientModule,
    AppModuleShared
    ]
})
export class AppModule {
}
