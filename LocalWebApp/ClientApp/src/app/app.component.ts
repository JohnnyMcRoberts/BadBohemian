import { Component } from '@angular/core';
import { SideNavigationListComponent } from "./Components/Layout/side-navigation-list/side-navigation-list.component";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
    //title = 'app';
    // 
    public title = 'app';

    public selectedMenu: string = SideNavigationListComponent.defaultMenuItemText;

    onSelectedMenuItem(selection: string): void {
        this.selectedMenu = selection;
    }
}
