import { Component, Input, OnChanges, SimpleChange } from '@angular/core';
import { DatePipe } from '@angular/common';

import { Book } from './../../../Models/Book';

@Component({
    selector: 'app-book-detail',
    templateUrl: './book-detail.component.html',
    styleUrls: ['./book-detail.component.scss']
})
/** BookDetail component*/
export class BookDetailComponent implements OnChanges {

    //#region Private Data
    
    private updates: number = 0;

    //#endregion

    //#region Public Data
    
    public numberLoaded: string = "None";

    public selectedBook: Book = null;
    public displayTags: string = "";

    //#endregion

    //#region Input Data

    @Input() bookSelected: Book;

    //#endregion


    /** BookDetail ctor */
    constructor(private datePipe: DatePipe) {

    }

    //#region Public methods

    public setBookSelected(sheet: Book): void
    {
        this.selectedBook = sheet;
        this.displayTags = "";
        let tagCount: number = 0;
        for (let tag of this.selectedBook.tags)
        {
            if (tagCount !== 0) {
                this.displayTags += ", ";
            }

            this.displayTags += tag;
            tagCount++;
        }
    }

    public getDisplayDate(dateRead: Date): string
    {
        // get the update time
        const displayDate: Date = new Date(dateRead);

        const formattedDate: string =
            this.datePipe.transform(displayDate, 'longDate');

        return formattedDate;
    }
    
    ngOnChanges(changes: { [propKey: string]: SimpleChange; })
    {
        console.log(" BookDetailComponent ngOnChanges - update #" + this.updates);

        for (let propName in changes)
        {
            if (changes.hasOwnProperty(propName))
            {
                let changedProp = changes[propName];
                const currentValue = changedProp.currentValue;

                this.updates++;
                if (currentValue != null && propName === "bookSelected")
                {
                    this.setBookSelected((currentValue as Book));
                }
            }
        }
    }

    //#endregion
}
