import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';

import { Book } from './../../../Models/Book';
import { Author } from './../../../Models/Author';
import { LanguageAuthors } from './../../../Models/LanguageAuthors';
import { BooksDataService } from './../../../Services/books-data.service';
import { BaseTableComponent } from './../base-table.component'

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

    public selectedItem: LanguageAuthors;
    public selectedAuthors: Author[];

    ngOnInit() {
      this.booksDataService.fetchAllLanguageAuthorsData().then(() =>
      {
        this.items = new Array<LanguageAuthors>();

        for (let item of this.booksDataService.languageAuthors) {
          var languageAuthor: LanguageAuthors = item;
          languageAuthor.authorsNames = LanguageAuthors.getAuthorsAsHtmlList(languageAuthor);
          this.items.push(languageAuthor);
        }

        this.itemsDataSource = new MatTableDataSource(this.items);
        this.setupItemsPagingAndSorting();
      });
    }

  ngAfterViewInit()
  {
      this.setupItemsPagingAndSorting();
    }

    public getItemsDisplayedColumns(): string[] {
      var columns =
      [
        'name',
        'totalBooksReadInLanguage',
        'totalPagesReadInLanguage',
        'percentageOfBooksRead',
        'percentageOfPagesRead',
        //'flag',

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
