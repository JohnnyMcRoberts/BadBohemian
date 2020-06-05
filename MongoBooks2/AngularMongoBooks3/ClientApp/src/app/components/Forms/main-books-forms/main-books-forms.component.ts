import { Component } from '@angular/core';
import { Observable } from 'rxjs';

import { CurrentLoginService } from './../../../Services/current-login.service';
import { GoogleBookService } from './../../../Services/google-book.service';
import { GoogleBook } from './../../../Models/google-api.interface';

@Component({
    selector: 'app-main-books-forms',
    templateUrl: './main-books-forms.component.html',
    styleUrls: ['./main-books-forms.component.scss']
})
/** MainBooksForms component*/
export class MainBooksFormsComponent
{
    /** MainBooksForms ctor */
    constructor(
        private currentLoginService: CurrentLoginService,
        private googleBookApiService: GoogleBookService)
    {

    }

    books: Observable<GoogleBook[]>;

    bookQuery(bookTitle)
    {
        if (bookTitle.length > 2)
        {
            this.books = this.googleBookApiService.findBook(bookTitle);
        }

    }
}
