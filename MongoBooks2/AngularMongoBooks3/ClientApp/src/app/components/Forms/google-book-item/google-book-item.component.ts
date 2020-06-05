import { Component, OnInit, Input } from "@angular/core";

import { GoogleBook } from './../../../Models/google-api.interface';

@Component({
    selector: 'app-google-book-item',
    templateUrl: './google-book-item.component.html',
    styleUrls: ['./google-book-item.component.scss']
})
/** GoogleBookItem component*/
export class GoogleBookItemComponent implements OnInit
{
    /** GoogleBookItem ctor */
    constructor()
    {

    }

    @Input() book: GoogleBook;

    get id(): string
    {
        return this.book.id;
    }

    get thumbnail(): string {
        return this.book.volumeInfo.imageLinks
            ? this.book.volumeInfo.imageLinks.thumbnail.replace("http:", "")
            : "";
    }


    get fullPath(): string {
        return this.book.volumeInfo.imageLinks
            ? this.book.volumeInfo.imageLinks.thumbnail
            : "";
    }

    ngOnInit(): void { }

}
