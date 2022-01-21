import { Component } from '@angular/core';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';

import * as FileSaver from 'file-saver';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { Book } from './../../../Models/book';
import { NationGeography } from './../../../Models/nation-geography';

import { ExportText } from './../../../Models/export-text';
import { ExportDataToEmailRequest } from './../../../Models/export-data-to-email';

export enum ExportDataSource {
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
    selector: 'app-email-export',
    templateUrl: './email-export.component.html',
    styleUrls: ['./email-export.component.scss']
})
/** EmailExport component*/
export class EmailExportComponent
{
    /** EmailExport ctor */
    constructor(
        private formBuilder: FormBuilder,
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

        this.sendEmailFormGroup =
            this.formBuilder.group({
                destinationEmail: ['',
                    [Validators.required,
                        Validators.email,
                        Validators.pattern('^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$')]],
                notes: ['']
            });

    }

    ngAfterViewInit()
    {
    }

    public books: Book[] | any;
    public nations: NationGeography[] | any;

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

    public selectedExportType: string | any;

    exportDataSources: string[] =
        [
            ExportDataSource.Books,
            ExportDataSource.Geography
        ];

    public selectedExportDataSource: string | any;

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
        const replacer = (key : any, value: any) => value === null ? '' : value; // specify how you want to handle null values here
        const header = Object.keys(this.books[0]);

        let csv =
            this.books.map(
                (row : any) => header.map(
                    fieldName => JSON.stringify(
                        row[fieldName], replacer)
                ).join(','));
        csv.unshift(header.join(','));
        let csvArray = csv.join('\r\n');

        return csvArray;
    }

    public exportText: ExportText | any;

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
                this.exportText = ExportText.fromData(this.booksDataService.exportCsvText as ExportText);
                this.setDisplayText(this.exportText.formattedText);

                if (setDisplayFlag) {
                    this.exportDataToDisplay = true;
                }
            });
        }
        else
        {
            this.booksDataService.fetchExportCsvTextData(this.currentLoginService.userId).then(() =>
            {
                this.exportText = ExportText.fromData(this.booksDataService.exportCsvText as ExportText);
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
                    //saveAs(blob, fileName);
                }
                break;

            case ExportFileType.CSV:
                {
                    this.setupExportDataAsText(true);

                    if (!this.isGeography)
                    {
                        this.booksDataService.fetchExportCsvFileData(this.currentLoginService.userId).then(() =>
                        {
                            if (this.booksDataService.exportCsvTextFile !== null) {
                                const csvData = BOM +
                                    (this.booksDataService.exportCsvTextFile as ExportText).formattedText;
                                let blob = new Blob([csvData], { type: "text/csv;charset=utf-8" });
                                FileSaver.saveAs(blob, "BooksRead.csv");
                                //saveAs(blob, "BooksRead.csv");
                            }
                        });
                    }
                    else
                    {
                        this.booksDataService.fetchExportNationsCsvFileData(this.currentLoginService.userId).then(() =>
                        {
                            if (this.booksDataService.exportCsvTextFile !== null) {
                                const csvData = BOM +
                                    (this.booksDataService.exportCsvTextFile as ExportText).formattedText;
                                let blob = new Blob([csvData], { type: "text/csv;charset=utf-8" });
                                FileSaver.saveAs(blob, "Nations.csv");
                                //saveAs(blob, "Nations.csv");
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

    //#region Send Email Form

    public sendEmailFormGroup: FormGroup | any;

    public async onExportDataToEmail() {

        console.log("onExportDataToEmail");

        // this is the workaround for special character in a csv as per
        // https://github.com/eligrey/FileSaver.js/issues/28
        const BOM = "\uFEFF";

        let blob: Blob | any = null;

        switch (this.selectedExportType) {
            case ExportFileType.JSON:
                {
                    const fileName: string =
                        this.isGeography ? "Nations.json" : "BooksRead.json";
                    this.displayText =
                        this.isGeography ? JSON.stringify(this.nations, null, '\t') : JSON.stringify(this.books, null, '\t');

                    this.exportDataToDisplay = true;
                    blob = new Blob([this.displayText], { type: "application/json" });
                    //FileSaver.saveAs(blob, fileName);
                    //saveAs(blob, fileName);
                }
                break;

            case ExportFileType.CSV:
                {
                    this.setupExportDataAsText(true);

                    if (!this.isGeography) {
                        this.booksDataService.fetchExportCsvFileData(this.currentLoginService.userId).then(() => {
                            if (this.booksDataService.exportCsvTextFile !== null) {
                                const csvData = BOM +
                                    (this.booksDataService.exportCsvTextFile as ExportText).formattedText;
                                blob = new Blob([csvData], { type: "text/csv;charset=utf-8" });
                                //FileSaver.saveAs(blob, "BooksRead.csv");
                                //saveAs(blob, "BooksRead.csv");
                            }
                        });
                    }
                    else {
                        this.booksDataService.fetchExportNationsCsvFileData(this.currentLoginService.userId).then(() => {
                            if (this.booksDataService.exportCsvTextFile !== null) {
                                const csvData = BOM +
                                    (this.booksDataService.exportCsvTextFile as ExportText).formattedText;
                                blob = new Blob([csvData], { type: "text/csv;charset=utf-8" });
                                //FileSaver.saveAs(blob, "Nations.csv");
                                //saveAs(blob, "Nations.csv");
                            }
                        });
                    }
                }
                break;
        }

        //sourceEmail: ['',
        //    [Validators.required, Validators.email, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]],
        //    sourcePassword: ['',
        //        Validators.required],
        //        destinationEmail: ['',
        //            [Validators.required, Validators.email, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]],
        //            notes: ['']

        const destinationEmail: string =
            this.sendEmailFormGroup.value.destinationEmail as string;
        const notes: string =
            this.sendEmailFormGroup.value.notes as string;

        console.log("the parameters are : ---");
        console.log("   --- destinationEmail : " + destinationEmail);
        console.log("   --- notes : " + notes);
        console.log("   --- selectedExportType : " + this.selectedExportType);
        console.log("   --- isGeography : " + this.isGeography);

        let request: ExportDataToEmailRequest =
            new ExportDataToEmailRequest(destinationEmail, this.selectedExportType, this.isGeography.toString());

        this.booksDataService.exportEmail(request).then(() => {

            if (this.booksDataService.exportEmailResponse !== null)
            {

                console.log("Response = " + JSON.stringify(this.booksDataService.exportEmailResponse, null, 4));

                //const csvData = BOM +
                //    (this.booksDataService.exportCsvTextFile as ExportText).formattedText;
                //blob = new Blob([csvData], { type: "text/csv;charset=utf-8" });
                //FileSaver.saveAs(blob, "Nations.csv");
                //saveAs(blob, "Nations.csv");
            }
        });
    }


    //#endregion
}
