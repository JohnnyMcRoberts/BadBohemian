import { Component, OnInit, AfterViewInit } from '@angular/core';

import * as FileSaver from 'file-saver';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { Book } from './../../../Models/book';
import { NationGeography } from './../../../Models/nation-geography';

import { ExportText } from './../../../Models/export-text';

@Component({
  selector: 'app-text-file-import',
  templateUrl: './text-file-import.component.html',
  styleUrls: ['./text-file-import.component.css']
})
export class TextFileImportComponent implements OnInit, AfterViewInit
{
    /** TextFileExport ctor */
    constructor(
        private booksDataService: BooksDataService,
        public currentLoginService: CurrentLoginService
    )
    {
        this.componentTitle = "Loading books charts from database...";
    }

    //#region Data Setup

    ngOnInit()
    {
        this.booksDataService.fetchAllBooksData().then(() => {
            this.books = this.booksDataService.books;
        });

        this.booksDataService.fetchAllNationsData().then(() => {
            this.nations = this.booksDataService.nations;
        });
    }

    ngAfterViewInit()
    {
    }

    public books: Book[] | any;
    public nations: NationGeography[] | any;

    public componentTitle: string;
    public componentStatus: string = '';

    public get loadingChartData(): boolean {

        return (!this.books || !this.books);
    }

    public async uploadFile(files: any | null)
    {
        if (files)
        {
            const fileToUpload = <File>files[0];
            const uploadData = new FormData();
            uploadData.append('file', fileToUpload, fileToUpload.name);

            this.componentStatus = "Should at this point upload file: " + fileToUpload.name;

            console.log(this.componentStatus);

            await this.booksDataService.importBooksCsvAsync(uploadData);

            this.componentStatus = "addAsyncBookRead has returned ";

            console.log(this.componentStatus);
            //console.log("addAsyncBookRead has returned");


            //this.http
            //    .post(
            //        `${environment.apiProtocol}://${environment.apiBase}/api/Insight/submissions/upload-report?reportSubmissionId=${this.submissionId}`,
            //        uploadData,
            //        { reportProgress: true, observe: 'events' }
            //    )
            //    .pipe(
            //        tap((event) =>
            //            event.type === HttpEventType.UploadProgress
            //                ? (this.uploading = true)
            //                : (this.uploading = false)
            //        ),
            //        takeUntil(this.destroyed$)
            //    )
            //    .subscribe();
        }
    };

}
