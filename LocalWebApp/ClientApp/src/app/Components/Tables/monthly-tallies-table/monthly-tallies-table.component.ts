import { Component, OnInit, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';


import { Book } from './../../../Models/book';
import { MonthlyTally } from './../../../Models/monthly-tally';
import { BooksDataService } from './../../../Services/books-data.service';
import { BaseTableComponent } from './../base-table.component';


@Component({
    selector: 'app-monthly-tallies-table',
    templateUrl: './monthly-tallies-table.component.html',
    styleUrls: ['./monthly-tallies-table.component.scss']
})
/** MonthlyTalliesTable component*/
export class MonthlyTalliesTableComponent
    extends BaseTableComponent
    implements OnInit, AfterViewInit
{
    /** MonthlyTalliesTable ctor */
    constructor(booksDataService: BooksDataService)
    {
        super();
        this.componentTitle = "Loading monthly tallies from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public selectedItem: MonthlyTally | any;
    public selectedBooks: Book[] | any;

    ngOnInit()
    {
        this.booksDataService.fetchAllMonthlyTalliesData().then(() =>
        {
            this.items = new Array<MonthlyTally>();

            if (this.booksDataService.monthlyTallies)
            {
                for (let item of this.booksDataService.monthlyTallies)
                {
                    var monthlyTally: MonthlyTally = item;
                    this.items.push(monthlyTally);
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
                'daysInTheMonth',
                'totalBooks',
                'totalPagesRead',
                'totalBookFormat',
                'totalComicFormat',
                'totalAudioFormat',
                'percentageInTranslation',
                'pageRate',
                'daysPerBook',
                'pagesPerBook',
                'booksPerYear',
                'monthDate'
            ];

        return columns;
    }

    public onItemsRowClicked(row: any): void
    {
        this.selectedItem = row;
        console.log('Items Row clicked: ', this.selectedItem.name);

        this.selectedBooks = this.selectedItem.books;
    }
}
