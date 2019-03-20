import { Component, OnInit, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material';

import { Author } from './../../../Models/Author';
import { BookTally } from './../../../Models/BookTally';
import { BooksDataService } from './../../../Services/books-data.service';
import { BaseTableComponent } from './../base-table.component'

@Component({
  selector: 'app-book-tallies-table',
  templateUrl: './book-tallies-table.component.html',
  styleUrls: ['./book-tallies-table.component.scss']
})
/** BookTalliesTable component*/
export class BookTalliesTableComponent
  extends BaseTableComponent
  implements OnInit, AfterViewInit
{
  /** BookTalliesTable ctor */
  constructor(booksDataService: BooksDataService)
  {
    super();
    this.componentTitle = "Loading book tallies from database...";
    this.booksDataService = booksDataService;
  }

  private booksDataService: BooksDataService;
  public componentTitle: string;

  public selectedItem: BookTally;
  public selectedAuthors: Author[];

  ngOnInit()
  {
    this.booksDataService.fetchAllBookTalliesData().then(() =>
    {
      this.items = new Array<BookTally>();

      for (let item of this.booksDataService.bookTallies) {
        var bookTally: BookTally = item;
        this.items.push(bookTally);
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
      'date',
      'author',
      'title',
      'pages',
      'totalBooks',
      'totalBookFormat',
      'totalComicFormat',
      'totalAudioFormat',
      'totalPagesRead',
    ];

    return columns;
  }

  public onItemsRowClicked(row: any): void
  {
    this.selectedItem = row;
    console.log('Items Row clicked: ', this.selectedItem.title);
  }

}
