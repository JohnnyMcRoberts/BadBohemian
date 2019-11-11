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
import { ExportText } from '../Models/ExportText';

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
        private http: HttpClient)
    {
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
    fetchAllBookTalliesData()
    {
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

    public exportCsvText: ExportText;
    fetchExportCsvTextData(userId: string)
    {
        return this.http.get<ExportText>(this.requestUrl + "GetExportCsvText/" + userId)
            .toPromise().then(result =>
                {
                    this.exportCsvText = result as ExportText;
                },
                error => console.error(error));
    }

    public booksWithDefaultUser: Book[];
    getAsDefaultUser(userId: string)
    {
        return this.http.get<Book[]>(this.requestUrl + "GetAsDefaultUser/" + userId)
            .toPromise().then(result =>
              {
                  this.booksWithDefaultUser = result as Book[];
              },
              error => console.error(error));
    }

    public addBookReadResponse: any;
    async addAsyncBookRead(request: BookReadAddRequest)
    {
        this.addBookReadResponse =
            await this.http.post<BookReadAddResponse>( this.requestUrl, request, httpOptions ).toPromise();

        console.log('No issues, waiting until promise is resolved...');
    }

    public updateBookResponse: any;
    async  updateAsyncBook(book: Book)
    {
        this.updateBookResponse =
          await this.http.put<BookReadAddResponse>( this.requestUrl, book, httpOptions ).toPromise();

        console.log('No issues, waiting until promise is resolved...');
    }

    public deleteBookResponse: any;
    async  deleteAsyncBook(book: Book)
    {
        this.deleteBookResponse =
            await this.http.delete<BookReadAddResponse>(
            this.requestUrl + "/" + book.id, httpOptions
            ).toPromise();

        console.log('No issues, waiting until promise is resolved...');
    }

    public readonly MaxErrorLength: number  = 255;
    public exportCsvFile: any;
    public exportCsvFileBlob: Blob;
    public exportCsvTextFile: ExportText;
    fetchExportCsvFileData(userId: string)
    {
        var requestUrl = this.requestUrl + "GetExportCsvFile/" + userId;

        const headers = new HttpHeaders().set('Content-Type', 'text/csv; charset=utf-8');

        return this.http.get(requestUrl, { headers, responseType: 'text' }).pipe(
            tap((result: any) =>
            {
                var resultString = result.toString();

                this.exportCsvTextFile = new ExportText('text/csv', resultString);

                if (resultString.length > this.MaxErrorLength)
                    resultString = resultString.substring(0, this.MaxErrorLength) + "...";

                this.log("fetched ExportCsvFileData " + resultString);
                this.exportCsvFile = result;
                this.exportCsvFileBlob = result as Blob;
            }
            ),
            catchError(this.handleError<any>('fetchExportCsvFileData'))
        ).toPromise();

    }

    // utility methods

    private handleError<T>(operation = 'operation', result?: T)
    {
      return (error: any): Observable<T> => {
        // TODO: send the error to remote logging infrastructure
        console.error(error); // log to console instead

        // TODO: better job of transforming error for user consumption
        this.log(`${operation} failed: ${error.message}`);

        // Let the app keep running by returning an empty result.
        return new Observable<T>();
      };
    }

    /** Log a QuoteService message with the MessageService */
    private log(message: string)
    {
        console.log(`BooksDataService: ${message}`);
    }
}
