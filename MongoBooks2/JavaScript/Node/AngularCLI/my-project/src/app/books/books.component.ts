import { Component, OnInit } from '@angular/core';
import { Book } from './Book';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.css']
})
export class BooksComponent implements OnInit {
  book1: Book = {
    id: 1,
    author: 'Joseph Roth',
    title: 'Weights and Measures',
    pages: 123
  };

  constructor() { }
  author = 'Joseph Roth';
  ngOnInit() {
  }

}
