import { Component, OnInit, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

import { Author } from './../../../Models/author';
import { CountryAuthors } from './../../../Models/country-authors';
import { BooksDataService } from './../../../Services/books-data.service';
import { BaseTableComponent } from './../base-table.component';

@Component({
    selector: 'app-country-author-table',
    templateUrl: './country-author-table.component.html',
    styleUrls: ['./country-author-table.component.scss']
})
/** CountryAuthorTable component*/
export class CountryAuthorTableComponent
    extends BaseTableComponent
    implements OnInit, AfterViewInit
{
    /** CountryAuthorTable ctor */
    constructor(booksDataService: BooksDataService)
    {
        super();
        this.componentTitle = "Loading country authors from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public selectedItem: CountryAuthors | any;
    public selectedAuthors: Author[] | any;

    ngOnInit()
    {
        this.booksDataService.fetchAllCountryAuthorsData().then(() =>
        {
            this.items = new Array<CountryAuthors>();

            if (this.booksDataService && this.booksDataService.countryAuthors)
            {

                for (let item of this.booksDataService.countryAuthors)
                {
                    let countryAuthor: CountryAuthors = item;
                    countryAuthor.authorsNames = CountryAuthors.getAuthorsAsHtmlList(countryAuthor);
                    this.items.push(countryAuthor);
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
        var columns =
            [
                'name',
                'totalBooksReadInLanguage',
                'totalPagesReadInLanguage',
                'percentageOfBooksRead',
                'percentageOfPagesRead',
                'flag',

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
