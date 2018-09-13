/*
 * allBooksByMonthMongo.ts
 * GET all books by month from mongo.
 */
import express = require('express');
import mongoose = require('mongoose');

import Book from './book'
import { IBook } from "./book";
import { MonthlyBooksReports } from './MonthlyBooksReports'


const router = express.Router();

var hostname: string;
var listBook: IBook;
var listBookItems: IBook[];

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

      let i: number = 0;
      for (let book in books) {
        if (books.hasOwnProperty(book))
        {
          listBook = books[i] as IBook;
          i++;

          listBookItems.push(listBook);
        }
      }

      let monthlyBooksReports: MonthlyBooksReports = new MonthlyBooksReports(listBookItems);

      let bookMonths = monthlyBooksReports.monthlyBooksRead;

      let monthIndex = randomIntFromInterval(0, bookMonths.length);

      let randomMonth = bookMonths[monthIndex];

      const totalDays: number = getDifferenceInDays(listBookItems[0].date, listBookItems[listBookItems.length - 1].date);
      
      res.render(
        'allBooksByMonthMongo',
        {
          title: 'Express got a subset of books from DB for ' + randomMonth.monthDate + ' it has ' + randomMonth.monthDate.daysInMonth() + ' days. Overall ' + totalDays + ' Days' ,
          monthDate: randomMonth.monthDate,
          monthTotals: randomMonth.monthTotals,
          monthlyBreakdownTotals: randomMonth.breakdownTotals,
          overallTotals: monthlyBooksReports.overallTotals,
          books: randomMonth.listBooksRead,
          availableMonthsByYear: monthlyBooksReports.monthlyReportYears
        });
    }
  });

});

export default router;