import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { CurrentLoginService } from './../../../Services/current-login.service';

@Component({
    selector: 'app-main-import-export',
    templateUrl: './main-import-export.component.html',
    styleUrls: ['./main-import-export.component.scss']
})
/** MainImportExport component*/
export class MainImportExportComponent
{
    /** MainImportExport ctor */
    constructor(
        private router: Router,
        public currentLoginService: CurrentLoginService)
    {

    }

    public goToLogin(): Promise<void>
    {
        // to refresh the page fist navigate to the application root
        return this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
        {
            // then navigate back to the login item
            this.router.navigate(['/user-login']);
        });
    }
}
