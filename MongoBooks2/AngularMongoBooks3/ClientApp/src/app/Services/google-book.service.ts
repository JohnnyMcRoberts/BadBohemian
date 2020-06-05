import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

import { UserAddRequest, UserAddResponse, UserLoginRequest, UserLoginResponse } from './../Models/User';

import { GoogleBook, GoogleBooksApiInterface } from './../Models/google-api.interface';

const httpOptions =
{
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
    providedIn: "root",
})
export class GoogleBookService
{
    private API_URL = "https://www.googleapis.com/books/v1/volumes";

    constructor(private http: HttpClient) { }

    findBook(title: string): Observable<GoogleBook[]> {
        return this.http
            .get<GoogleBooksApiInterface>(`${this.API_URL}?q=${title}`)
            .pipe(map((data: GoogleBooksApiInterface) => data.items));
    }
}
