import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class BooksReadDataService {
  baseUrl: string = "http://localhost:9000";

  constructor(private httpClient: HttpClient) { }

  get_authors() {
    return this.httpClient.get(this.baseUrl + '/authors');
  }
}
