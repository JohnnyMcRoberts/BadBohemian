import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { Counter2Component } from './components/counter2/counter2.component';
import { Counter3Component } from './components/counter3/counter3.component';
import { AuthorService, Author } from './services/allNames/allNames.service';
import { AllNames2Service } from './services/allNames2/allNames2.service';

@NgModule({
    declarations: [
        AppComponent,
      NavMenuComponent,
      CounterComponent,
      Counter2Component,
      Counter3Component,
      FetchDataComponent,
        HomeComponent
  ],
  providers: [
    AuthorService,
    AllNames2Service
    ],
    imports: [
        CommonModule,
      HttpModule,
      HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
          { path: 'home', component: HomeComponent },
          { path: 'counter', component: CounterComponent },
          { path: 'counter2', component: Counter2Component },
          { path: 'counter3', component: Counter3Component },
          { path: 'fetch-data', component: FetchDataComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
