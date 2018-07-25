import { Component, OnInit } from '@angular/core';
import { Book } from './../Models/Book';
import IBook = books.IBook;
import { BookDataService } from './../Services/BookDataService';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.css']
})

export class BooksComponent implements OnInit {

  bookSelected: IBook = new Book("5a6b69dc6488960e6808bad0",
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
  
  ngOnInit() {
    this.booksSet = this.vBookDataService.GetAllBookData();
    this.selection = "no selection from " + this.booksSet.length;
  }
  selectedBook: IBook;
  booksSet: IBook[];
  vBookDataService: BookDataService = new BookDataService();
  selection: string;
  author: string = 'Joseph Roth';

  onSelect(book: IBook): void {
    this.selectedBook = book;
    this.selection = this.selectedBook.title +
      " by " +
      this.selectedBook.author;
    //this.bookSelected = book;
    this.bookSelected = new Book(book._id,
      book.dateString,
      book.date,
      book.author,
      book.title,
      book.pages,
      book.note,
      book.nationality,
      book.originalLanguage,
      book.image_url,
      book.tags,
      book.format);
  }

}
