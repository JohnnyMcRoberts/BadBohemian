/*
 * allBooks.ts
 * GET all books.
 */
import express = require('express');
import mongoose = require('mongoose');
import Book from './book'

const router = express.Router();

var hostname: string;
//export interface Author {
//  name: string;
//}
export class BaseAuthor {
  name: string;
  constructor(name: string) {
    this.name = name;
  }
}

export class Author {
  name: string;
  constructor() { }
} 


router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get authors / ');
  hostname = req.hostname;
  //res.send([{ name: 'Joyce' }, { name: 'Becket' }]);
  let items = new Array<Author>();
  let a1 = new Author();
  a1.name = "Elliot";
  let a2 = new Author();
  a2.name = "Pound";
  items.push(a1);
  items.push(a2);
  //res.json(items);//.send([{ name: 'Joyce' }, { name: 'Becket' }]);
  //res.status(200).send(items);
  var data = ([
    {
      "name": "Mann"
    },
    {
      "name": "Joyce"
    },
    {
      "name": "Wolff"
    }
  ]);
  res.setHeader("Access-Control-Allow-Credentials", "true");
  res.setHeader("Cache-Control", "no-cache");
  res.setHeader("Pragma", "no-cache");
  res.setHeader("Vary", "Origin, Accept-Encoding");
  res.setHeader("Expires", "-1");
  res.setHeader("X-Content-Type-Options", "nosniff");
  res.status(200).send(data);
});

export default router;