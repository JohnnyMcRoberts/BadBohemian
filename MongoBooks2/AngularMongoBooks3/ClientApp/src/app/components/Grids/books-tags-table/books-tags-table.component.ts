import { Component, OnInit, AfterViewInit } from '@angular/core';

import { MatTableDataSource } from '@angular/material/table';

import { Book } from './../../../Models/Book';
import { CountryAuthors } from './../../../Models/CountryAuthors';
import { TagBooks } from './../../../Models/TagBooks';
import { BooksDataService } from './../../../Services/books-data.service';
import { BaseTableComponent } from './../base-table.component';

@Component({
    selector: 'app-books-tags-table',
    templateUrl: './books-tags-table.component.html',
    styleUrls: ['./books-tags-table.component.scss']
})
/** BooksTagsTable component*/
export class BooksTagsTableComponent
  extends BaseTableComponent
  implements OnInit, AfterViewInit
{
    /** BooksTagsTable ctor */
    constructor(booksDataService: BooksDataService)
    {
      super();
      this.componentTitle = "Loading book tags from database...";
      this.booksDataService = booksDataService;
    }

    private booksDataService: BooksDataService;
    public componentTitle: string;

    public selectedItem: TagBooks;
    public selectedBooks: Book[];

    ngOnInit()
    {
      this.booksDataService.fetchAllTagBooksData().then(() =>
      {
        this.items = new Array<TagBooks>();

        for (let item of this.booksDataService.tagBooks)
        {
          var monthlyTally: TagBooks = item;
          this.items.push(monthlyTally);
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
        'totalBooks',
        'totalPages'
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
