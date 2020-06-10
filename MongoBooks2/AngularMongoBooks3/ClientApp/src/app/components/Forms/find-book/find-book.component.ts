import { Component } from '@angular/core';
import { Observable } from 'rxjs';

import { GoogleBookService } from './../../../Services/google-book.service';
import { GoogleBookInterface } from './../../../Models/google-api.interface';

@Component({
    selector: 'app-find-book',
    templateUrl: './find-book.component.html',
    styleUrls: ['./find-book.component.scss']
})
/** FindBook component*/
export class FindBookComponent
{
    /** FindBook ctor */
    constructor(private googleBookApiService: GoogleBookService)
    {

    }

    books: Observable<GoogleBookInterface[]>;

    bookQuery(bookTitle)
    {
        if (bookTitle.length > 2)
        {
            this.books = this.googleBookApiService.findBook(bookTitle);
        }
    }
}
