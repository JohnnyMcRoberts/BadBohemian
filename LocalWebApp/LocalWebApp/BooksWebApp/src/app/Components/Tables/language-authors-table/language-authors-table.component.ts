import { Component, OnInit, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

import { Author } from './../../../Models/author';
import { LanguageAuthors } from './../../../Models/language-authors';
import { BooksDataService } from './../../../Services/books-data.service';
import { BaseTableComponent } from './../base-table.component'

@Component({
    selector: 'app-language-authors-table',
    templateUrl: './language-authors-table.component.html',
    styleUrls: ['./language-authors-table.component.scss']
})
/** LanguageAuthorsTable component*/
export class LanguageAuthorsTableComponent
    extends BaseTableComponent
    implements OnInit, AfterViewInit
{
    /** LanguageAuthorsTable ctor */
    constructor(booksDataService: BooksDataService)
    {
        super();
        this.componentTitle = "Loading language authors from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;

    public selectedItem: LanguageAuthors | any;
    public selectedAuthors: Author[] | any;

    ngOnInit()
    {
        this.booksDataService.fetchAllLanguageAuthorsData().then(() =>
        {
            this.items = new Array<LanguageAuthors>();

            if (this.booksDataService.languageAuthors)
            {
                for (let item of this.booksDataService.languageAuthors)
                {
                    let languageAuthor: LanguageAuthors = item;
                    languageAuthor.authorsNames = LanguageAuthors.getAuthorsAsHtmlList(languageAuthor);
                    this.items.push(languageAuthor);
                }
            }

            this.itemsDataSource = new MatTableDataSource(this.items);
            this.setupItemsPagingAndSorting();
        });
    }

    ngAfterViewInit()
    {
        this.setupItemsPagingAndSorting();
    }

    public getItemsDisplayedColumns(): string[]
    {
        const columns =
            [
                'name',
                'totalBooksReadInLanguage',
                'totalPagesReadInLanguage',
                'percentageOfBooksRead',
                'percentageOfPagesRead',

                'authorsNames'
            ];

        return columns;
    }

    public onItemsRowClicked(row: any): void
    {
        this.selectedItem = row;
        console.log('Items Row clicked: ', this.selectedItem.name);

        this.selectedAuthors = this.selectedItem.authors;
    }
}
