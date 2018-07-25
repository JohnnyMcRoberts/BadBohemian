
import { OnInit } from '@angular/core';

import { BookData } from './../Models/BookData';
import { Book } from './../Models/Book';
import { CountryTotal } from './../Models/CountryTotal';
import { MockBooksSet } from './../Services/MockBooks';
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
      if (calculatePages)
      {
        result = [ ];

        for (let bookData of this.booksSet)
        {
          let existingItem: ICountryTotal = null;

          for (let total of result)
          {
            if (total.nationality === bookData.nationality)
            {
              existingItem = total;
              existingItem.total += bookData.pages;
              break;
            }
          }

          if (existingItem === null)
          {
            result.push(new CountryTotal(bookData.nationality, bookData.pages));
          }
        }
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