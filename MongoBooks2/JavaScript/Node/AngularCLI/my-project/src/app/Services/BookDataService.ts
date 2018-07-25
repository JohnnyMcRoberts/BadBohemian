
import { OnInit } from '@angular/core';

import { BookData } from './../Models/BookData';
import { Book } from './../Models/Book';
import { CountryTotal } from './../Models/CountryTotal';
import { MockBooksSet } from './../Services/MockBooks';
//import { IBook } from './../Interfaces/IBook';
import IBook = books.IBook;
import ICountryTotal = books.ICountryTotal;


export class BookDataService implements OnInit  {

  constructor() {
    this.booksSet = MockBooksSet;
    this.status = "ZZZZ";
  }

    ngOnInit() {
    }
  
    booksSet: BookData[];
    public status: string;

    public GetAllBookData(): IBook[] {
      var allBooks = new Array<IBook>();
      for (let bookData of this.booksSet)
      {
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

      return allBooks;
    }

    public GetCountryTotals(calculatePages: boolean): ICountryTotal[]
    {
      var result: ICountryTotal[];
      if (calculatePages) {
        result =
        [
          new CountryTotal( "USA", 1363), // {nationality: "USA", total: 1363},
          new CountryTotal("Scotland", 518), // ["Scotland", 518],
          new CountryTotal("France", 351), // ["France", 351],
          new CountryTotal("Hungary", 312), // ["Hungary", 312],
          new CountryTotal("Belgium", 154), // ["Belgium", 154],
          new CountryTotal("England", 0) // ["England", 0]
        ];
      }
      else
      {
        result =
        [
          new CountryTotal("USA", 6), // ["USA", 6],
          new CountryTotal("England", 2), // ["England", 2],
          new CountryTotal("Belgium", 1), // ["Belgium", 1],
          new CountryTotal("Hungary", 1), // ["Hungary", 1],
          new CountryTotal("France", 1), // ["France", 1],
          new CountryTotal("Scotland", 1) // ["Scotland", 1]
        ];
      }

      return result;
    }


  }