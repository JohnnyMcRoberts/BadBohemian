import { Component } from '@angular/core';

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
    constructor(private currentLoginService: CurrentLoginService)
    {

    }
}
