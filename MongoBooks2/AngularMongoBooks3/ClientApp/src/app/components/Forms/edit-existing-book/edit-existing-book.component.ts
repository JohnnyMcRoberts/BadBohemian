import { Component, OnInit, Output, EventEmitter, AfterViewInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, Validators, FormGroup } from '@angular/forms';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';

import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { EditorDetails } from './../../../Models/EditorDetails';
import { Book, BookReadAddResponse, BookReadAddRequest } from './../../../Models/Book';

@Component({
    selector: 'app-edit-existing-book',
    templateUrl: './edit-existing-book.component.html',
    styleUrls: ['./edit-existing-book.component.scss']
})
/** EditExistingBook component*/
export class EditExistingBookComponent
{
    /** EditExistingBook ctor */
  constructor(
    private formBuilder: FormBuilder,
    private booksDataService: BooksDataService,
    private currentLoginService: CurrentLoginService)
  {
    this.componentTitle = "Loading books data...";
  }

  public componentTitle: string;
  public selectedBook: Book = new Book();
  public editorDetails: EditorDetails;

  //#region Accordion State implementation

  public showAllBooksPanelOpenState = true;
  public editBook: Book = null;
  public bookToEdit: boolean = false;

  //#endregion

  //#region Populate Books Table

  public books: Book[];
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

  public selectedBookToDisplay: boolean = false;

  onBooksRowClicked(row)
  {
    this.selectedBook = row;
    console.log('Books Row clicked: ', this.selectedBook.date);
    this.selectedBookToDisplay = true;

    var dateSelected: Date = new Date(this.selectedBook.date);
    this.selectedBookReadTime.setValue(dateSelected);
  }

  //#endregion



  //#region Update Details


  public selectedBookReadTime = new FormControl(new Date());

  public selectedMoment = new Date();


  //#endregion

}
