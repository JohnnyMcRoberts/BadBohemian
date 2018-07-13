import { Component, OnInit } from '@angular/core';
import { Book } from './Book';
import IBook = books.IBook;
import { BookData } from './BookData';
import { MockBooksSet } from './MockBooks';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.css']
})

export class BooksComponent implements OnInit {

  book1: IBook = new Book("5a6b69dc6488960e6808bad0",
     "24th February 2012",
   new Date("2012-02-24T00:00:00.000Z"),
   "Gabriel Garcia Marquez",
   "No One Writes to the Colonel",
   68,
   "",
   "Colombia",
   "Spanish",
   "https://images-eu.ssl-images-amazon.com/images/I/51XOjayPg3L._SY291_BO1,204,203,200_QL40_.jpg",
   ["Magic Realism"],
    1);

  constructor() { }
  author = 'Joseph Roth';

  BooksSet = MockBooksSet;
  ngOnInit() {
  }

}
