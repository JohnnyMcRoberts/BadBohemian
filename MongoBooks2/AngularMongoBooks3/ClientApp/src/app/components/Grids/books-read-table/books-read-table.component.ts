import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';

import { Book } from './../../../Models/Book';
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

  public books: Book[];
  public selectedBook: Book;
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

  @ViewChild('booksTablePaginator') booksTablePaginator: MatPaginator;
  @ViewChild('booksTableSort') public booksTableSort: MatSort;
  public booksDataSource: MatTableDataSource<Book>;

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
        this.booksTableSort.sortChange.subscribe(() =>
        {
          this.booksTablePaginator.pageIndex = 0;
          this.booksTablePaginator._changePageSize(this.booksTablePaginator.pageSize);
        });
      });
    }
  }

  applyBooksFilter(filterValue: string)
  {
    this.booksDataSource.filter = filterValue.trim().toLowerCase();

    if (this.booksDataSource.paginator)
    {
      this.booksTablePaginator.pageIndex = 0;
      this.booksTablePaginator._changePageSize(this.booksTablePaginator.pageSize);
    }
  }

  onBooksRowClicked(row)
  {
    this.selectedBook = row;
    console.log('Books Row clicked: ', this.selectedBook.dateTime);
  }
}
