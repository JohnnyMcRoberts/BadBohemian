import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { Book } from '../Models/Book';
import { Author } from '../Models/Author';
import { LanguageAuthors } from '../Models/LanguageAuthors';

const httpOptions =
{
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root',
})
export class BooksDataService
{
  constructor(
    private http: HttpClient) {
    this.requestUrl = '/api/BooksData/';
  }

  public requestUrl: string;

  // Get the promised data
  public books: Book[];
  fetchAllBooksData()
  {
    return this.http.get<Book[]>(this.requestUrl + "GetAllBooks")
      .toPromise().then(result =>
        {
          this.books = result as Book[];
        },
        error => console.error(error));
  }


  public authors: Author[];
  fetchAllAuthorsData()
  {
    return this.http.get<Author[]>(this.requestUrl + "GetAllAuthors")
      .toPromise().then(result =>
        {
          this.authors = result as Author[];
        },
        error => console.error(error));
  }


  public languageAuthors: LanguageAuthors[];
  fetchAllLanguageAuthorsData()
  {
    return this.http.get<LanguageAuthors[]>(this.requestUrl + "GetAllLanguageAuthors")
      .toPromise().then(result =>
        {
          this.languageAuthors = result as LanguageAuthors[];
        },
        error => console.error(error));
  }

}
