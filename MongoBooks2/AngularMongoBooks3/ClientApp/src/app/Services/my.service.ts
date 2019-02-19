import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { Book } from '../Models/Book';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root',
})
export class MyService
{
  constructor(
    private http: HttpClient)
  {
    this.requestUrl = '/api/BooksData/';
  }

  //public http: HttpClient;
  public requestUrl: string;

  public getServiceName(): string
  {
    if (this.http != null)
      return "This is me the service with: requestURL" + this.requestUrl +" & http" + this.http.toString();

    return "This is me the service";
  }


  // Get the promised data
  public books: Book[];
  fetchAllBooksData() {
    return this.http.get<Book[]>(this.requestUrl + "GetAllBooks")
      .toPromise().then(result => {
          this.books = result as Book[];
        },
        error => console.error(error));
  }


}
