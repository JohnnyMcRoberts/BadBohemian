import { Injectable } from '@angular/core';

import { Observable, of } from 'rxjs';

import { BookData } from './../Models/BookData';
import { Book } from './../Models/Book';
import { CountryTotal } from './../Models/CountryTotal';
import { MockBooksSet } from './../Services/MockBooks';
import IBook = books.IBook;
import ICountryTotal = books.ICountryTotal;

@Injectable({
  providedIn: 'root'
})
export class LibraryService {

  constructor() { }

  booksSet: BookData[] = MockBooksSet;

  public getHeroes(): Observable<IBook[]> {

    var allBooks = new Array<IBook>();
    for (let bookData of this.booksSet) {
      allBooks.push(new Book(
        bookData._id,
        bookData.dateString,
        bookData.date,
        bookData.author,
        bookData.title,
        bookData.pages,
        bookData.note,
        bookData.nationality,
        bookData.originalLanguage,
        bookData.image_url,
        bookData.tags,
        bookData.format
      ));
    }
    // TODO: send the message _after_ fetching the heroes
    //this.messageService.add('HeroService: fetched heroes');
    return of(allBooks);
  }
  
  public GetCountryTotals(calculatePages: boolean): Observable<ICountryTotal[]> {
    let result: ICountryTotal[];
    if (calculatePages) {
      result = [];

      for (let bookData of this.booksSet) {
        let existingItem: ICountryTotal = null;

        for (let total of result) {
          if (total.nationality === bookData.nationality) {
            existingItem = total;
            existingItem.total += bookData.pages;
            break;
          }
        }

        if (existingItem === null) {
          result.push(new CountryTotal(bookData.nationality, bookData.pages));
        }
      }
    }
    else {
      result = [];

      for (let bookData of this.booksSet) {
        let existingItem: ICountryTotal = null;

        for (let total of result) {
          if (total.nationality === bookData.nationality) {
            existingItem = total;
            existingItem.total += 1;
            break;
          }
        }

        if (existingItem === null) {
          result.push(new CountryTotal(bookData.nationality, 1));
        }
      }
    }

    // sort then items largest to smallest
    result.sort(function (a, b) { return b.total - a.total });

    return of(result);
  }


}
