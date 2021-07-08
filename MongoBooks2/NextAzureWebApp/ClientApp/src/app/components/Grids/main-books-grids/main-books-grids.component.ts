import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { CurrentLoginService } from './../../../Services/current-login.service';

@Component({
    selector: 'app-main-books-grids',
    templateUrl: './main-books-grids.component.html',
    styleUrls: ['./main-books-grids.component.scss']
})
/** MainBooksGrids component*/
export class MainBooksGridsComponent
{
    /** MainBooksGrids ctor */
    constructor(
        private router: Router,
        public currentLoginService: CurrentLoginService)
    {

    }

    public goToLogin(): Promise<void>
    {
        // to refresh the page fist navigate to the application root
        return this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {

            // then navigate back to the login item
            this.router.navigate(['/user-login']);
        });
    }
}
