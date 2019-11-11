import { Component, OnInit, AfterViewInit } from '@angular/core';

import * as FileSaver from 'file-saver';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { Book } from './../../../Models/Book';

import { ExportText } from './../../../Models/ExportText';

export enum ExportFileType
{
  CSV = "CSV",
  JSON = "JSON"
};

export interface IHash
{
  [details: string]: boolean;
}

@Component({
    selector: 'app-text-file-export',
    templateUrl: './text-file-export.component.html',
    styleUrls: ['./text-file-export.component.scss']
})
/** TextFileExport component*/
export class TextFileExportComponent implements OnInit, AfterViewInit
{
    /** TextFileExport ctor */
    constructor(
        private booksDataService: BooksDataService,
        private currentLoginService: CurrentLoginService
    )
    {

    }

    //#region Data Setup

    ngOnInit()
    {
        this.booksDataService.fetchAllBooksData().then(() =>
        {
            this.books = this.booksDataService.books;
        });
    }

    ngAfterViewInit()
    {
    }

    public books: Book[];

    //#endregion

    //#region Export Options

    exportTypes: string[] =
        [
            ExportFileType.CSV,
            ExportFileType.JSON
        ];

    public selectedExportType: string;
    public exportDataToDisplay: boolean = false;
    public displayText: string = '';

    get optionSelected()
    {
      return this.selectedExportType != undefined
        && this.selectedExportType != null
        && this.selectedExportType !== '';
    }

    public getExportDataAsCsv(): string
    {
        const replacer = (key, value) => value === null ? '' : value; // specify how you want to handle null values here
        const header = Object.keys(this.books[0]);
        let csv = this.books.map(row => header.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','));
        csv.unshift(header.join(','));
        let csvArray = csv.join('\r\n');

        return csvArray;
    }

    public exportText: ExportText;

    public async getExportDataAsText()
    {
      this.displayText = "Formatting....";

      this.booksDataService.fetchExportCsvTextData(this.currentLoginService.userId).then(() =>
      {
          this.exportText = ExportText.fromData(this.booksDataService.exportCsvText);
          this.setDisplayText(this.exportText.formattedText);
      });
    }

    public setDisplayText(exportText: any): void
    {
        this.exportDataToDisplay = false;
        this.displayText = exportText.toString();
        this.exportDataToDisplay = true;
    }

    //#endregion

  //#region Page Control Handlers

  public onFileReset(): void
  {
    //this.fileInput.nativeElement.value = '';
    //this.songsFilesDetailsResponse = null;
    //this.songsFilesDetailsResponseRxed = false;
    //this.fileIsSelected = false;
    this.exportDataToDisplay =  false;
  }

  public async onFileSetForUser()
  {
    //await this.setFileSongsForUser(this.fileInfoLatest, this.currentLoginService.userId);
  }

  public async onDisplayExportData()
  {
    switch (this.selectedExportType)
    {
        case ExportFileType.JSON:
        {
            this.displayText = JSON.stringify(this.books, null, '\t');
            this.exportDataToDisplay = true;
        }
        break;

        case ExportFileType.CSV:
        {
            await this.getExportDataAsText();
            this.exportDataToDisplay = true;
        }
        break;
    }

  }

  public async onExportDataToFile()
  {
      switch (this.selectedExportType)
      {
        case ExportFileType.JSON:
        {
            this.displayText = JSON.stringify(this.books, null, '\t');
            this.exportDataToDisplay = true;
            let blob = new Blob([this.displayText], { type: "application/json" });
            FileSaver.saveAs(blob, "BooksRead.json");
        }
        break;

        case ExportFileType.CSV:
        {
            this.booksDataService.fetchExportCsvTextData(this.currentLoginService.userId).then(() =>
            {
                this.exportText = ExportText.fromData(this.booksDataService.exportCsvText);
                this.setDisplayText(this.exportText.formattedText);
                this.exportDataToDisplay = true;
            });

            this.booksDataService.fetchExportCsvFileData(this.currentLoginService.userId).then(() =>
            {
                if (this.booksDataService.exportCsvTextFile !== null)
                {
                    // this is the workaround for special character in a csv as per
                    // https://github.com/eligrey/FileSaver.js/issues/28
                    var BOM = "\uFEFF";
                    var csvData = BOM + this.booksDataService.exportCsvTextFile.formattedText;
                    let blob = new Blob([csvData], { type: "text/csv;charset=utf-8" }); ;
                    FileSaver.saveAs(blob, "BooksRead.csv");
                }
            });
        }
        break;
      }
  }

    //#endregion

}
