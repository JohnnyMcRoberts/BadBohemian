import { Component, OnInit } from '@angular/core';
import { Book } from './Book';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.css']
})
export class BooksComponent implements OnInit {
  book1: Book = {
    _id: "5a6b69dc6488960e6808bad0",
    dateString: "24th February 2012",
    date: new Date("2012-02-24T00:00:00.000Z"),
    author: "Gabriel Garcia Marquez",
    title: "No One Writes to the Colonel",
    pages: 68,
    note: "",
    nationality: "Colombia",
    originalLanguage: "Spanish",
    image_url: "https://images-eu.ssl-images-amazon.com/images/I/51XOjayPg3L._SY291_BO1,204,203,200_QL40_.jpg",
    tags: ["Magic Realism"],
    format: 1
  };

  constructor() { }
  author = 'Joseph Roth';
  ngOnInit() {
  }

}
