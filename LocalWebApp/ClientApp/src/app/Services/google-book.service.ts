import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

import {
    GoogleBooksApiInterface,
    GoogleBookInterface,
    GoogleBookDetailInterface
} from './../Models/google-books-api-interface';

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

    constructor(private http: HttpClient) {
        this.googleBookDetail = undefined;
    }

    findBook(title: string): Observable<GoogleBookInterface[]>
    {
        return this.http
            .get<GoogleBooksApiInterface>(`${this.API_URL}?q=${title}`)
            .pipe(map((data: GoogleBooksApiInterface) => data.items));
    }

    public googleBookDetail: GoogleBookDetailInterface | undefined;

    fetchBookDetail(volumeId: string)
    {
        return this.http
            .get<GoogleBookDetailInterface>(`${this.API_URL}/${volumeId}`)
            .toPromise().then(result =>
                {
                this.googleBookDetail = result as GoogleBookDetailInterface;
                },
                error => console.error(error));
    }

}
