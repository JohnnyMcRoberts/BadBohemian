/*
 * allBooksMongo.ts
 * GET all books mongo.
 */
import express = require('express');
import mongoose = require('mongoose');
import Book from './book'

const router = express.Router();

var hostname: string;

router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get allBooksMongo / ');
  hostname = req.hostname;

  let books = Book.find((err: any, books: any) => {
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
    } else
    {
      res.render(
        'allBooksMongo',
        {
          title: 'Express got all books from DB',
          books: books
        });
    }
  });
});

export default router;