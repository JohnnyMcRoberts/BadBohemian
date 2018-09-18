/*
 * allBooks.ts
 * GET all books.
 */
import express = require('express');
import mongoose = require('mongoose');
import Book from './book'

const router = express.Router();

var hostname: string;

router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get authors / ');
  hostname = req.hostname;
  res.send({
    authors: [{ name: 'Joyce' }, { name: 'Becket' }]
  });
});

export default router;