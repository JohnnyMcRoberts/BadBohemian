/*
 * allAuthorsMongo.ts
 * GET all authors mongo.
 */
import express = require('express');
import mongoose = require('mongoose');

import Book from './book'
import { IBook } from "./book";
import { AuthorTotals } from './AuthorTotals'

const router = express.Router();

var hostname: string;

router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get allAuthorsMongo / ');
  hostname = req.hostname;

  let books = Book.find((err: any, books: any) => {
    if (err)
    {
      console.log('error in books find - ', err);
      res.render(
        'allAuthorsMongo',
        {
          title: 'Express all books from DB error!',
          books:
          [
            { author: "A bad thing", title: "but worse" },
            { author: "is going", title: "To follow" }
          ]
        });
    } else {
      const authorTotals: AuthorTotals = new AuthorTotals(books);
      res.render(
        'allAuthorsMongo',
        {
          title: 'Express got all books and authors from DB',
          authorTotals: authorTotals
        });
    }
  });
});

export default router;