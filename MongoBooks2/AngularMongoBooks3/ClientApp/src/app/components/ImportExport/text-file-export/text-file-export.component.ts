import { Component, OnInit, AfterViewInit } from '@angular/core';

import * as FileSaver from 'file-saver';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { Book } from './../../../Models/Book';
import { NationGeography } from './../../../Models/NationGeography';

import { ExportText } from './../../../Models/ExportText';

export enum ExportDataSource
{
    Books = "Books",
    Geography = "Geography"
};

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
        public currentLoginService: CurrentLoginService
    )
    {
        this.componentTitle = "Loading books charts from database...";
    }

    //#region Data Setup

    ngOnInit()
    {
        this.booksDataService.fetchAllBooksData().then(() =>
        {
            this.books = this.booksDataService.books;
        });

        this.booksDataService.fetchAllNationsData().then(() =>
        {
            this.nations = this.booksDataService.nations;
        });
    }

    ngAfterViewInit()
    {
    }

    public books: Book[];
    public nations: NationGeography[];

    public componentTitle: string;

    public get loadingChartData(): boolean
    {

        return (!this.books || !this.books);
    }

    //#endregion

    //#region Export Options

    exportTypes: string[] =
        [
            ExportFileType.CSV,
            ExportFileType.JSON
        ];

    public selectedExportType: string;

    exportDataSources: string[] =
    [
        ExportDataSource.Books,
        ExportDataSource.Geography
    ];

    public selectedExportDataSource: string;

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

        this.setupExportDataAsText();
    }

    public get isGeography(): boolean
    {
        return (this.selectedExportDataSource && this.selectedExportDataSource === ExportDataSource.Geography);
    }

    private async setupExportDataAsText(setDisplayFlag: boolean = false)
    {
        if (this.isGeography)
        {
            this.booksDataService.fetchExportNationsCsvTextData(this.currentLoginService.userId).then(() =>
            {
                this.exportText = ExportText.fromData(this.booksDataService.exportCsvText);
                this.setDisplayText(this.exportText.formattedText);

                if (setDisplayFlag)
                {
                    this.exportDataToDisplay = true;
                }
            });
        }
        else
        {
            this.booksDataService.fetchExportCsvTextData(this.currentLoginService.userId).then(() =>
            {
                this.exportText = ExportText.fromData(this.booksDataService.exportCsvText);
                this.setDisplayText(this.exportText.formattedText);

                if (setDisplayFlag)
                {
                    this.exportDataToDisplay = true;
                }
            });
        }

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
        this.exportDataToDisplay = false;
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
                    if (this.isGeography)
                    {
                        this.displayText = JSON.stringify(this.nations, null, '\t');
                    }
                    else
                    {
                        this.displayText = JSON.stringify(this.books, null, '\t');
                    }

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
        // this is the workaround for special character in a csv as per
        // https://github.com/eligrey/FileSaver.js/issues/28
        const BOM = "\uFEFF";

        switch (this.selectedExportType)
        {
            case ExportFileType.JSON:
                {
                    const fileName: string =
                        this.isGeography ? "Nations.json" : "BooksRead.json";
                    this.displayText =
                        this.isGeography ? JSON.stringify(this.nations, null, '\t') : JSON.stringify(this.books, null, '\t');

                    this.exportDataToDisplay = true;
                    let blob = new Blob([this.displayText], { type: "application/json" });
                    FileSaver.saveAs(blob, fileName);
                }
                break;

            case ExportFileType.CSV:
                {
                    this.setupExportDataAsText(true);

                    if (!this.isGeography)
                    {
                        this.booksDataService.fetchExportCsvFileData(this.currentLoginService.userId).then(() =>
                        {
                            if (this.booksDataService.exportCsvTextFile !== null)
                            {
                                const csvData = BOM + this.booksDataService.exportCsvTextFile.formattedText;
                                let blob = new Blob([csvData], { type: "text/csv;charset=utf-8" });
                                FileSaver.saveAs(blob, "BooksRead.csv");
                            }
                        });
                    }
                    else
                    {
                        this.booksDataService.fetchExportNationsCsvFileData(this.currentLoginService.userId).then(() =>
                        {
                            if (this.booksDataService.exportCsvTextFile !== null)
                            {
                                const csvData = BOM + this.booksDataService.exportCsvTextFile.formattedText;
                                let blob = new Blob([csvData], { type: "text/csv;charset=utf-8" });
                                FileSaver.saveAs(blob, "Nations.csv");
                            }
                        });
                    }
                }
                break;
        }
    }

    //#endregion

    //#region File Save Options


    //#endregion

}
