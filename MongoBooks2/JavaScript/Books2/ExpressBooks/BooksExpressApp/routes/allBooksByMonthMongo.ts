/*
 * allBooksByMonthMongo.ts
 * GET all books by month from mongo.
 */
import express = require('express');
import mongoose = require('mongoose');
import Collections = require('typescript-collections');

import Book from './book'
import { IBook } from "./book";
import { BookRead } from './BookRead'
import { MonthDate, MonthlyBooksRead } from './MonthlyBooksRead'


const router = express.Router();

var hostname: string;
var listBook: IBook;
var listBookItems: IBook[];
var listBooksRead: BookRead[];


function getMonthlyBooksRead(books: IBook[]): MonthlyBooksRead[]
{
  var dict = new Collections.Dictionary<string, MonthlyBooksRead>();
  for (let book of books) {
    const month = new MonthDate(book.date);
    var monthName = month.year.toString() + '/' + month.month.toString();

    if (dict.containsKey(monthName)) {
      const monthSet = dict.getValue(monthName);
      monthSet.listBooksRead.push(new BookRead(book));
      dict.setValue(monthName, monthSet);

      console.log('adding book ' + book.title + ' to existing month ' + month); // "4", "5", "6"
    }
    else
    {
      const monthOfBooks = new MonthlyBooksRead(book.date);
      monthOfBooks.listBooksRead.push(new BookRead(book));
      dict.setValue(monthName, monthOfBooks);
      console.log('adding book ' + book.title + ' to new month ' + month); // "4", "5", "6"
    }

  }

  return dict.values();
}


router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get allBooksByMonthMongo / ');
  hostname = req.hostname;

  listBookItems = new Array<IBook>();
  listBooksRead = new Array<BookRead>();

  let books = Book.find((err: any, books: mongoose.Document[]) => {
    if (err)
    {
      console.log('error in books find - ', err);
      res.render(
        'allBooksMongo',
        {
          title: 'Express all books from DB error!',
          books:
          [
            { author: "A bad thing", title: "but worse" },
            { author: "is going", title: "To follow" }
          ]
        });
      return;
    }
    else
    {
      console.log('successfully got the books -> ', books.toString().slice(0, 50));

      let i: number = 0;
      for (let book in books) {
        if (books.hasOwnProperty(book))
        {
          listBook = books[i] as IBook;
          //console.log('successfully got the book ' + i + ' -> ', listBook);
          i++;

          var bookRead = new BookRead(listBook);
          listBookItems.push(listBook);

          if (i > 5)
            continue;

          listBooksRead.push(bookRead);
        }
      }

      var bookMonths = getMonthlyBooksRead(listBookItems);
      var fourthMonth = bookMonths[4];
      
      res.render(
        'allBooksByMonthMongo',
        {
          title: 'Express got a subset of books from DB',
          books: fourthMonth.listBooksRead
        });
    }
  });


});

export default router;