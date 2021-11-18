import { Component } from '@angular/core';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { SideNavigationListComponent } from "./Components/Layout/side-navigation-list/side-navigation-list.component";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {

    // 
    public title = 'app';
    
    public selectedMenu: string = SideNavigationListComponent.defaultMenuItemText;

    onSelectedMenuItem(selection: string): void {
        this.selectedMenu = selection;
    }

}
