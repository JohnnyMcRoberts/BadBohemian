import { Component, Input } from '@angular/core';

import { Book } from './../../../Models/book';

@Component({
    selector: 'app-selected-book-detail',
    templateUrl: './selected-book-detail.component.html',
    styleUrls: ['./selected-book-detail.component.scss']
})
/** SelectedBookDetail component*/
export class SelectedBookDetailComponent
{
    //#region Input Data

    @Input() bookSelected: Book | any;

    //#endregion


    //#region Public Getters

    get selectedBookName(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.title;
        }
        else
        {
            return "nor set";
        }
    }

    get selectedBookAuthor(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.author;
        }
        else
        {
            return "nor set";
        }
    }

    get selectedBookImageUrl(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.imageUrl;
        }
        else
        {
            return "";
        }
    }

    get selectedBookNote(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.note;
        }
        else
        {
            return "";
        }
    }

    get selectedBookDate(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.dateString;
        }
        else
        {
            return "";
        }
    }


    get selectedBookPages(): number
    {
        if (this.bookSelected)
        {
            return this.bookSelected.pages;
        }
        else
        {
            return 0;
        }
    }
    
    get selectedBookNationality(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.nationality;
        }
        else
        {
            return "";
        }
    }
    
    get selectedBookOriginalLanguage(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.originalLanguage;
        }
        else
        {
            return "";
        }
    }

    get selectedBookFormat(): string
    {
        if (this.bookSelected)
        {
            return this.bookSelected.format;
        }
        else
        {
            return "";
        }
    }

    get selectedBookTags(): string
    {
        if (this.bookSelected)
        {
            if (this.bookSelected.tags && this.bookSelected.tags.length > 0)
            {
                return this.bookSelected.tags.join(', '); 
            }
        }

        return "";
    }

    //#endregion

    /** SelectedBookDetail ctor */
    constructor() {

    }
}
