import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { CurrentLoginService } from './../../../Services/current-login.service';

@Component({
    selector: 'app-main-tables-page',
    templateUrl: './main-tables-page.component.html',
    styleUrls: ['./main-tables-page.component.scss']
})
/** MainTablesPage component*/
export class MainTablesPageComponent
{
    /** MainTablesPage ctor */
    constructor(
        private router: Router,
        public currentLoginService: CurrentLoginService)
    {

    }

    public goToLogin(): Promise<void>
    {
        // to refresh the page, navigate to the application root
        return this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
        {
            // then navigate back to the login item
            this.router.navigate(['/user-login']);
        });
    }
}
