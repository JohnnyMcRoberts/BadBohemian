/*
 * allBooks.ts
 * GET all books.
 */
import express = require('express');
import mongoose = require('mongoose');
import Book from './book'

const router = express.Router();

var hostname: string;
export interface Author {
  name: string;
}
export class BaseAuthor {
  name: string;
  constructor(name: string) {
    this.name = name;
  }
}

router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get authors / ');
  hostname = req.hostname;
  //res.send([{ name: 'Joyce' }, { name: 'Becket' }]);
  let items = new Array<Author>();
  items.push(new BaseAuthor('Joyce'));
  items.push(new BaseAuthor('Wolff'));
  res.json(items);//.send([{ name: 'Joyce' }, { name: 'Becket' }]);
});

export default router;