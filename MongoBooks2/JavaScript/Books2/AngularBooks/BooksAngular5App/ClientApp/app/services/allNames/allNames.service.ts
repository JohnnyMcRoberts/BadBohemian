import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';

export interface Author {
  name: string;
}

@Injectable()
export class AuthorService {
  constructor(private http: HttpClient) { }

  private uri: string = 'http://localhost:3000/authors';

  getAllAuthors(): Observable<Author[]> {
    console.log('-> getting authors from : ' + this.uri);
    let result = this.http.get<Author[]>(this.uri, { responseType: 'json' });
    console.log('-> done getting authors from : ' + this.uri);
    return result;
  }
}