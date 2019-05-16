import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { Book, BookReadAddResponse, BookReadAddRequest } from '../Models/Book';
import { Author } from '../Models/Author';
import { LanguageAuthors } from '../Models/LanguageAuthors';
import { CountryAuthors } from '../Models/CountryAuthors';
import { BookTally } from '../Models/BookTally';
import { MonthlyTally } from '../Models/MonthlyTally';
import { TagBooks } from '../Models/TagBooks';
import { DeltaBooks } from '../Models/DeltaBooks';
import { EditorDetails } from '../Models/EditorDetails';

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


  public authors: Author[];
  fetchAllAuthorsData()
  {
    return this.http.get<Author[]>(this.requestUrl + "GetAllAuthors")
      .toPromise().then(result =>
        {
          this.authors = result as Author[];
        },
        error => console.error(error));
  }

  public languageAuthors: LanguageAuthors[];
  fetchAllLanguageAuthorsData()
  {
    return this.http.get<LanguageAuthors[]>(this.requestUrl + "GetAllLanguageAuthors")
      .toPromise().then(result =>
        {
          this.languageAuthors = result as LanguageAuthors[];
        },
        error => console.error(error));
  }

  public countryAuthors: CountryAuthors[];
  fetchAllCountryAuthorsData()
  {
    return this.http.get<CountryAuthors[]>(this.requestUrl + "GetAllCountryAuthors")
      .toPromise().then(result =>
        {
          this.countryAuthors = result as CountryAuthors[];
        },
        error => console.error(error));
  }

  public bookTallies: BookTally[];
  fetchAllBookTalliesData() {
    return this.http.get<BookTally[]>(this.requestUrl + "GetAllBookTallies")
      .toPromise().then(result =>
        {
          this.bookTallies = result as BookTally[];
        },
        error => console.error(error));
  }

  public monthlyTallies: MonthlyTally[];
  fetchAllMonthlyTalliesData()
  {
    return this.http.get<MonthlyTally[]>(this.requestUrl + "GetAllMonthlyTallies")
      .toPromise().then(result =>
        {
          this.monthlyTallies = result as MonthlyTally[];
        },
        error => console.error(error));
  }

  public tagBooks: TagBooks[];
  fetchAllTagBooksData()
  {
    return this.http.get<TagBooks[]>(this.requestUrl + "GetAllTagBooks")
      .toPromise().then(result =>
        {
          this.tagBooks = result as TagBooks[];
        },
        error => console.error(error));
  }

  public deltaBooks: DeltaBooks[];
  fetchAllDeltaBooksData()
  {
    return this.http.get<DeltaBooks[]>(this.requestUrl + "GetAllBooksDeltas")
      .toPromise().then(result =>
        {
          this.deltaBooks = result as DeltaBooks[];
        },
        error => console.error(error));
  }

  public editorDetails: EditorDetails;
  fetchEditorDetails()
  {
    return this.http.get<EditorDetails>(this.requestUrl + "GetEditorDetails")
      .toPromise().then(result =>
        {
          this.editorDetails = result as EditorDetails;
        },
        error => console.error(error));
  }


  public addBookReadResponse: any;
  async addAsyncBookRead(request: BookReadAddRequest)
  {
    this.addBookReadResponse =
      await this.http.post<BookReadAddResponse>(
        this.requestUrl, request, httpOptions
      ).toPromise();

    console.log('No issues, waiting until promise is resolved...');
  }

}
