/*
 * AuthorTotal.ts
 * the author total record
 */
import { IBookRead } from "./book";

export class AuthorTotal {
  public name: string;
  public nation: string;
  public language: string;
  public totalBooks: number;
  public totalPages: number;

  books: string[];

  constructor(book: IBookRead) {
    this.name = book.author;
    this.nation = book.nationality;
    this.language = book.originalLanguage;
    this.totalBooks = 1;
    this.totalPages = book.pages;

    this.books = new Array<string>();
    this.books.push(book.title);
  }

  public addBook(book: IBookRead): void
  {
    this.totalBooks++;
    this.totalPages += book.pages;
    this.books.push(book.title);
  }
}