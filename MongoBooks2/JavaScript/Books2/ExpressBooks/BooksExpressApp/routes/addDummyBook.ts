/*
 * PUT add dummy book.
 */
import express = require('express');
const router = express.Router();
import Book from './book'

var hostname: string;

router.post('/', (req: express.Request, res: express.Response) =>
{

  console.log('router.put addDummyBook / ');
  hostname = req.hostname;
  //var book = new Book(req.body);
  //var displayDate = new Date().toLocaleDateString();
  var displayTime = new Date().toISOString();
  var book = new Book({ title: "Title created on " + displayTime, author: "Dummy" });

  //book.save((err: any) => {
  //  if (err) {
  //    res.send(err);
  //  } else {
  //    res.send(book);
  //  }
  //});
});

export default router;