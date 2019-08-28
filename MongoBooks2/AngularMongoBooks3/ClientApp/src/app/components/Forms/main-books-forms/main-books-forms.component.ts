import { Component } from '@angular/core';
import { CurrentLoginService } from './../../../Services/current-login.service';

@Component({
    selector: 'app-main-books-forms',
    templateUrl: './main-books-forms.component.html',
    styleUrls: ['./main-books-forms.component.scss']
})
/** MainBooksForms component*/
export class MainBooksFormsComponent
{
    /** MainBooksForms ctor */
    constructor(private currentLoginService: CurrentLoginService)
    {

    }
}
