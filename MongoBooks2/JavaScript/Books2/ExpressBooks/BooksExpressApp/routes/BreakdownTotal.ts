import { IBookRead } from "./book";
import * as Collections from 'typescript-collections';

export class BreakdownElement {
  public title: string;
  public total: number;

  constructor(title: string, total: number) {
    this.title = title;
    this.total = total;
  }
}

export class BreakdownTotal {
  public title: string;
  public isLanguage: boolean;
  public isPages: boolean;
  public source: string;
  public division: string;
  public elements: BreakdownElement[];

  constructor(isLanguage: boolean, isPages: boolean, books: IBookRead[]) {
    this.isLanguage = isLanguage;
    this.isPages = isPages;
    this.elements = new Array<BreakdownElement>();

    if (isPages)
      this.source = "Pages";
    else
      this.source = "Books";

    if (isLanguage)
      this.division = "Language";
    else
      this.division = "Nation";

    this.title = this.source + " by " + this.division;

    const dict = new Collections.Dictionary<string, BreakdownElement>();

    // Get the elements.
    let title: string;
    let total: number;
    for (let book of books) {
      if (isPages)
        total = book.pages;
      else
        total = 1;

      if (isLanguage)
        title = book.originalLanguage;
      else
        title = book.nationality;

      if (dict.containsKey(title)) {
        const breakdownElement = dict.getValue(title);
        breakdownElement.total += total;
        dict.setValue(title, breakdownElement);
      }
      else {
        const breakdownElement = new BreakdownElement(title, total);
        dict.setValue(title, breakdownElement);
      }
    }

    // Get the elements sorted highest to lowest.
    const elements = dict.values();
    elements.sort((a, b) => {
      if (a.total > b.total)
        return -1;
      if (a.total < b.total)
        return 1;
      return 0;
    });

    this.elements = elements;
  }
}