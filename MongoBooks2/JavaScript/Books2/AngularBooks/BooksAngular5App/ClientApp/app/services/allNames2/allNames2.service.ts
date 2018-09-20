import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Http } from '@angular/http';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

export interface Author {
  name: string;
}

@Injectable()
export class AllNames2Service {

  public allAuthors: Author[];

  private authorsUrl: string = 'http://localhost:3000/authors';

  constructor(http: Http) {

    http.get(this.authorsUrl).subscribe(
      result => {
        console.log(" allnames2 got result \n");
        this.allAuthors = result.json() as Author[];
        console.log(" allnames2 got data \n");
      },
      error => { console.log(" allnames2 error \n");
        console.error(error);
      });
  }
}