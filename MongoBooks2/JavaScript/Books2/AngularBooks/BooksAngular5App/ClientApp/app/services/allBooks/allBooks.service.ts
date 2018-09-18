import { Observable } from 'rxjs';
import { Http } from '@angular/http';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { IBookRead } from './../../interfaces/IBookRead';
import { Injectable } from '@angular/core';

export interface AllBooksResponse
{
  message: string;
  books: IBookRead[]; 
}

@Injectable()
export class AllBooksService {
  private httpUrl: string = "http://localhost:3000/allBooks";
  private http: Http;
  private numBooks: number;
  constructor(http: Http) {
    this.http = http;
    console.trace("construct AllBooksService");
    this.numBooks = 55;
  }

  getBooksRead(): IBookRead[] {
    // TODO: send the message _after_ fetching the heroes
    //this.messageService.add('HeroService: fetched heroes');
    return new Array<IBookRead>();
  }

  async getTotalNumber(): Promise<number> {
    const response = await this.http.get(this.httpUrl).toPromise();
    let res = response.json() as AllBooksResponse;
    return res.books.length;
  }

  getNumberBooks(): number {
    // TODO: send the message _after_ fetching the heroes
    //this.messageService.add('HeroService: fetched heroes');
    let response = this.http.get(this.httpUrl).subscribe(
      result => {
        let res = result.json() as AllBooksResponse;
        this.numBooks = res.books.length;
      },
      error => console.error(error));

    console.trace("getNumberBooks");
    console.trace(response);
    return this.numBooks;
  }

}