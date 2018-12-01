import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient, HttpHeaders } from '@angular/common/http';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

export interface Author {
  name: string;
}

@Injectable()
export class AuthorService {
  constructor(private http: HttpClient) { }

  private heroesUrl: string = 'http://localhost:3000/authors';

  getAllAuthors(): Observable<Author[]> {
    console.log('-> getting authors from : ' + this.heroesUrl);
    return this.http.get<Author[]>(this.heroesUrl);
  }
}