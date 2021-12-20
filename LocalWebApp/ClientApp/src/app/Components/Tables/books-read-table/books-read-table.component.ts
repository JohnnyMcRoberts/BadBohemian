import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

import { Book } from './../../../Models/book';
import { BooksDataService } from './../../../Services/books-data.service';

@Component({
    selector: 'app-books-read-table',
    templateUrl: './books-read-table.component.html',
    styleUrls: ['./books-read-table.component.scss']
})
/** BooksReadTable component*/
export class BooksReadTableComponent implements OnInit, AfterViewInit
{
    /** BooksReadTable ctor */
    constructor(booksDataService: BooksDataService)
    {
        // works you know
        this.componentTitle = "Loading books from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public books: Book[] | any;
    public selectedBook: Book | any;
    public booksDisplayedColumns: string[] =
        [
            'date',
            'author',
            'title',
            'pages',
            'nationality',
            'originalLanguage',
            'format',
            'cover',
            'note'
        ];

    @ViewChild('booksTablePaginator') booksTablePaginator: MatPaginator | any;
    @ViewChild('booksTableSort') public booksTableSort: MatSort | any;
    public booksDataSource: MatTableDataSource<Book> | any;

    ngOnInit()
    {
        this.booksDataService.fetchAllBooksData().then(() =>
        {
            this.books = this.booksDataService.books;
            this.booksDataSource = new MatTableDataSource(this.books);
            this.setupBooksPagingAndSorting();
        });
    }

    ngAfterViewInit()
    {
        this.setupBooksPagingAndSorting();
    }

    private setupBooksPagingAndSorting(): void
    {
        if (this.books != null)
        {
            setTimeout(() =>
            {
                this.booksDataSource.paginator = this.booksTablePaginator;
                this.booksDataSource.sort = this.booksTableSort;
                if (this.booksTableSort.sortChange)
                {
                    this.booksTableSort.sortChange.subscribe(() =>
                    {
                        this.booksTablePaginator.pageIndex = 0;
                        this.booksTablePaginator._changePageSize(this.booksTablePaginator.pageSize);
                    });
                }
            });
        }
    }

    applyBooksFilter(eventTarget: any)
    {
        const filterValue: string = (eventTarget as HTMLInputElement).value;
        this.booksDataSource.filter = filterValue.trim().toLowerCase();

        if (this.booksDataSource.paginator)
        {
            this.booksTablePaginator.pageIndex = 0;
            this.booksTablePaginator._changePageSize(this.booksTablePaginator.pageSize);
        }
    }

    public selectedBookToDisplay: boolean = false;

    onBooksRowClicked(row: any)
    {
        this.selectedBook = row;
        console.log('Books Row clicked: ', this.selectedBook.date);
        this.selectedBookToDisplay = true;
    }
}
