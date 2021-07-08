import { Component, Input, OnChanges, SimpleChange } from '@angular/core';

import { Book } from './../../../Models/Book';

@Component({
    selector: 'app-book-detail',
    templateUrl: './book-detail.component.html',
    styleUrls: ['./book-detail.component.scss']
})
/** BookDetail component*/
export class BookDetailComponent implements OnChanges
{
    /** BookDetail ctor */
  constructor()
  {

  }

  //#region OnChanges implementation

  @Input() bookSelected: Book;

  public numberLoaded: string = "None";
  private updates: number = 0;

  ngOnChanges(changes: { [propKey: string]: SimpleChange })
  {
    for (let propName in changes) {
      if (changes.hasOwnProperty(propName))
      {
        let changedProp = changes[propName];
        var currentValue = changedProp.currentValue;

        this.updates++;
        if (currentValue != null && propName === "bookSelected") {
          this.setBookSelected((currentValue as Book));
        }
      }
    }
  }

  //#endregion

  //#region Local data population

  public selectedBook: Book = null;
  public displayTags: string = "";

  public setBookSelected(sheet: Book): void
  {
    this.selectedBook = sheet;
    this.displayTags = "";
    var tagCount: number = 0;
    for (let tag of this.selectedBook.tags)
    {
      if (tagCount !== 0)
      {
        this.displayTags += ", ";
      }

      this.displayTags += tag;
      tagCount++;
    }
  }

  //#endregion
}
