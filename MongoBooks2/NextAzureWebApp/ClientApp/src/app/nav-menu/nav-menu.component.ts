import { Component } from '@angular/core';
import { CurrentLoginService } from './../Services/current-login.service';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent
{
    isExpanded = false;

    collapse()
    {
        this.isExpanded = false;
    }

    toggle()
    {
        this.isExpanded = !this.isExpanded;
    }


    public get isLoggedIn(): boolean {

        if (this.currentLoginService) {

            return this.currentLoginService.isLoggedIn;
        }

        return false;
    }

    constructor(public currentLoginService: CurrentLoginService) {

    }
}
