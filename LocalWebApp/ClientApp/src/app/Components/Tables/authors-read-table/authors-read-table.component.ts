import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';

import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

import { Book } from './../../../Models/book';
import { Author } from './../../../Models/author';
import { BooksDataService } from './../../../Services/books-data.service';


@Component({
    selector: 'app-authors-read-table',
    templateUrl: './authors-read-table.component.html',
    styleUrls: ['./authors-read-table.component.scss']
})
/** AuthorsReadTable component*/
export class AuthorsReadTableComponent implements OnInit, AfterViewInit {
    /** AuthorsReadTable ctor */
    constructor(booksDataService: BooksDataService) {
        // works you know
        this.componentTitle = "Loading authors from database...";
        this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public authors: Author[] | any;
    public selectedAuthor: Author | any;
    public selectedBooks: Book[] | any;
    public authorsDisplayedColumns: string[] =
        [
            'name',
            'nationality',
            'language',
            'totalBooksReadBy',
            'totalPages'
        ];

    @ViewChild('authorsTablePaginator') authorsTablePaginator: MatPaginator | any;
    @ViewChild('authorsTableSort') authorsTableSort: MatSort | any;
    public authorsDataSource: MatTableDataSource<Author> | any;

    ngOnInit() {
        this.booksDataService.fetchAllAuthorsData().then(() => {
            this.authors = this.booksDataService.authors;
            this.authorsDataSource = new MatTableDataSource(this.authors);
            this.setupAuthorsPagingAndSorting();
        });
    }

    ngAfterViewInit()
    {
        this.setupAuthorsPagingAndSorting();
    }

    private setupAuthorsPagingAndSorting(): void
    {
        if (this.authors != null)
        {
            setTimeout(() =>
            {
                this.authorsDataSource.paginator = this.authorsTablePaginator;
                this.authorsDataSource.sort = this.authorsTableSort;
                this.authorsTableSort.sortChange.subscribe(() => {
                    this.authorsTablePaginator.pageIndex = 0;
                    this.authorsTablePaginator._changePageSize(this.authorsTablePaginator.pageSize);
                });
            });
        }
    }

    applyAuthorsFilter(eventTarget: any)
    {
        let filterValue: string = (eventTarget as HTMLInputElement).value;

        this.authorsDataSource.filter = filterValue.trim().toLowerCase();

        if (this.authorsDataSource.paginator) {
            this.authorsTablePaginator.pageIndex = 0;
            this.authorsTablePaginator._changePageSize(this.authorsTablePaginator.pageSize);
        }
    }

    onAuthorsRowClicked(row: any)
    {
        this.selectedAuthor = row;
        console.log('Authors Row clicked: ', this.selectedAuthor.name);

        this.selectedBooks = this.selectedAuthor.books;
    }
}
