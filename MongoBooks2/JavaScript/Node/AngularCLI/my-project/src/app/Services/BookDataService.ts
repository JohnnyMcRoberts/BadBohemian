
import { BookData } from './../Models/BookData';
import { Book } from './../Models/Book';
import { Component, OnInit } from '@angular/core';
import { MockBooksSet } from './../Services/MockBooks';
//import { IBook } from './../Interfaces/IBook';
import IBook = books.IBook;


export class BookDataService implements OnInit  {

  constructor() {
    this.booksSet = MockBooksSet;
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

  }