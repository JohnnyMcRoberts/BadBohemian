import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { Book } from '../Models/Book';

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

}
