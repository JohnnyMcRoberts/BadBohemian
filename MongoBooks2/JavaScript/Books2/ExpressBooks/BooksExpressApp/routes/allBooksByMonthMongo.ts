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
import { MonthDate } from './MonthDate'
import { BooksTotal } from './BooksTotal'
import { MonthlyBooksRead } from './MonthlyBooksRead'


const router = express.Router();

var hostname: string;
var listBook: IBook;
var listBookItems: IBook[];


function getMonthlyBooksRead(books: IBook[]): MonthlyBooksRead[]
{
  const dict = new Collections.Dictionary<MonthDate, MonthlyBooksRead>();
  for (let book of books) {
    const month = new MonthDate(book.date);

    if (dict.containsKey(month))
    {
      const monthSet = dict.getValue(month);
      monthSet.listBooksRead.push(new BookRead(book));
      dict.setValue(month, monthSet);
    }
    else
    {
      const monthOfBooks = new MonthlyBooksRead(book.date);
      monthOfBooks.listBooksRead.push(new BookRead(book));
      dict.setValue(month, monthOfBooks);
    }
  }

  return dict.values();
}

function randomIntFromInterval(min: number, max: number): number {
  return Math.floor(Math.random() * (max - min + 1) + min);
}

function getDifferenceInDays(start: Date, end: Date): number
{
  let diff: number = Math.abs(start.getTime() - end.getTime());
  let diffDays: number = Math.ceil(diff / (1000 * 3600 * 24));
  return diffDays;
}

router.get('/', (req: express.Request, res: express.Response) => {

  hostname = req.hostname;
  console.log('router.get allBooksByMonthMongo for host ' + hostname + ' / ');

  listBookItems = new Array<IBook>();

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
      console.log('successfully got the books');
      //console.log('successfully got the books -> ', books.toString().slice(0, 50));

      let i: number = 0;
      for (let book in books) {
        if (books.hasOwnProperty(book))
        {
          listBook = books[i] as IBook;
          i++;

          listBookItems.push(listBook);
        }
      }

      let bookMonths = getMonthlyBooksRead(listBookItems);

      let monthIndex = randomIntFromInterval(0, bookMonths.length);

      let randomMonth = bookMonths[monthIndex];
      randomMonth.updateTotals();

      const totalDays: number = getDifferenceInDays(listBookItems[0].date, listBookItems[listBookItems.length - 1].date);

      const overallTotals: BooksTotal = new BooksTotal(totalDays, listBookItems);
      
      res.render(
        'allBooksByMonthMongo',
        {
          title: 'Express got a subset of books from DB for ' + randomMonth.monthDate + ' it has ' + randomMonth.monthDate.daysInMonth() + ' days. Overall ' + totalDays + ' Days' ,
          monthDate: randomMonth.monthDate,
          monthTotals: randomMonth.monthTotals,
          monthlyBreakdownTotals: randomMonth.breakdownTotals,
          overallTotals: overallTotals,
          books: randomMonth.listBooksRead
        });
    }
  });

});

export default router;