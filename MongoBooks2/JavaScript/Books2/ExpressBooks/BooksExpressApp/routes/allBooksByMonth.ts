/*
 * allBooksByMonth.ts
 * GET all books by month for reports.
 */
import express = require('express');
import mongoose = require('mongoose');

import Book from './book'
import { IBook } from "./book";
import { MonthlyBooksReports } from './MonthlyBooksReports'

const router = express.Router();

router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get allBooksByMonth / ');

  let books = Book.find((err: any, books: any) => {
    if (err)
    {
      console.log('error in books find - ', err);
      res.send(
        {
          title: 'Express all books from DB error!',
          books:
          [
            { author: "A bad thing", title: "but worse" },
            { author: "is going", title: "To follow" }
          ]
        });
    } else
    {
      let i: number = 0;
      const listBookItems: IBook[] = new Array<IBook>();
      for (let book in books) {
        if (books.hasOwnProperty(book))
        {
          listBookItems.push(books[i] as IBook);
          i++;
        }
      }

      let monthlyBooksReports: MonthlyBooksReports = new MonthlyBooksReports(listBookItems);
      res.send(
        {
          title: 'Express got all monthly books reports from DB',
          monthlyBooksReports: monthlyBooksReports
        });
    }
  });
});

export default router;