import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';

import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

import { Book } from './../../../Models/Book';
import { Author } from './../../../Models/Author';
import { BooksDataService } from './../../../Services/books-data.service';


@Component({
    selector: 'app-authors-read-table',
    templateUrl: './authors-read-table.component.html',
    styleUrls: ['./authors-read-table.component.scss']
})
/** AuthorsReadTable component*/
export class AuthorsReadTableComponent implements OnInit, AfterViewInit
{
    /** AuthorsReadTable ctor */
  constructor(booksDataService: BooksDataService)
  {
    // works you know
    this.componentTitle = "Loading authors from database...";
    this.booksDataService = booksDataService;
  }

  private booksDataService: BooksDataService;
  public componentTitle: string;

  public authors: Author[];
  public selectedAuthor: Author;
  public selectedBooks: Book[];
  public authorsDisplayedColumns: string[] =
  [
    'name',
    'nationality',
    'language',
    'totalBooksReadBy',
    'totalPages'
  ];

  @ViewChild('authorsTablePaginator') authorsTablePaginator: MatPaginator;
  @ViewChild('authorsTableSort') authorsTableSort: MatSort;
  public authorsDataSource: MatTableDataSource<Author>;

  ngOnInit()
  {
    this.booksDataService.fetchAllAuthorsData().then(() =>
    {
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
        this.authorsTableSort.sortChange.subscribe(() =>
        {
          this.authorsTablePaginator.pageIndex = 0;
          this.authorsTablePaginator._changePageSize(this.authorsTablePaginator.pageSize);
        });
      });
    }
  }

  applyAuthorsFilter(filterValue: string)
  {
    this.authorsDataSource.filter = filterValue.trim().toLowerCase();

    if (this.authorsDataSource.paginator)
    {
      this.authorsTablePaginator.pageIndex = 0;
      this.authorsTablePaginator._changePageSize(this.authorsTablePaginator.pageSize);
    }
  }

  onAuthorsRowClicked(row)
  {
    this.selectedAuthor = row;
    console.log('Authors Row clicked: ', this.selectedAuthor.name);

    this.selectedBooks = this.selectedAuthor.books;
  }
}
