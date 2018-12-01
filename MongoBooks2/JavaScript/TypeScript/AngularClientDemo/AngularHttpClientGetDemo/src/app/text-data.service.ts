import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class TextDataService {
  baseUrl: string = "http://localhost:1337";

  constructor(private httpClient: HttpClient) { }

  get_text() {
    return this.httpClient.get(this.baseUrl + '/products');
  }
}
