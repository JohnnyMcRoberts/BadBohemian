import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators, ValidatorFn, ValidationErrors } from '@angular/forms';

import * as FileSaver from 'file-saver';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { Book } from './../../../Models/Book';
import { NationGeography } from './../../../Models/NationGeography';

import { ExportText } from './../../../Models/ExportText';
import { ExportDataToEmailRequest, ExportDataToEmailResponse } from './../../../Models/ExportDataToEmail';

export enum ExportDataSource {
    Books = "Books",
    Geography = "Geography"
};

export enum ExportFileType {
    CSV = "CSV",
    JSON = "JSON"
};

export interface IHash {
    [details: string]: boolean;
}

@Component({
    selector: 'app-export-to-email',
    templateUrl: './export-to-email.component.html',
    styleUrls: ['./export-to-email.component.scss']
})
/** ExportToEmail component*/
export class ExportToEmailComponent {

    /** ExportToEmail ctor */
    constructor(
        private formBuilder: FormBuilder,
        private booksDataService: BooksDataService,
        public currentLoginService: CurrentLoginService
    ) {
        this.componentTitle = "Loading books charts from database...";
    }

    //#region Public Data

    public exportTypes: string[] =
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

    public books: Book[];
    public nations: NationGeography[];

    public componentTitle: string;

    public exportText: ExportText;

    public destinationUserFormGroup: FormGroup;

    //#endregion

    //#region Public Properties

    public get loadingChartData(): boolean
    {
        return (!this.books || !this.books);
    }

    public get isGeography(): boolean
    {
        return this.selectedExportDataSource &&
            this.selectedExportDataSource === ExportDataSource.Geography;
    }

    public get newUserEmail() { return this.destinationUserFormGroup.get('newUserEmail'); }
    
    get sourceOptionSelected()
    {
        return this.selectedExportDataSource != undefined
            && this.selectedExportDataSource != null
            && this.selectedExportDataSource !== '';
    }

    public get exportOptionSelected()
    {
        return this.selectedExportType != undefined
            && this.selectedExportType != null
            && this.selectedExportType !== '';
    }

    //#endregion

    //#region Standard Interface Implementations

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

        this.destinationUserFormGroup =
            this.formBuilder.group(
                {
                    newUserEmail: ['', [Validators.required, Validators.email]]
                });
    }

    ngAfterViewInit()
    {
    }

    //#endregion

    //#region Public Methods

    public getExportDataAsCsv(): string
    {
        const replacer = (key, value) => value === null ? '' : value; // specify how you want to handle null values here
        const header = Object.keys(this.books[0]);
        const csv = this.books.map(row => header.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','));
        csv.unshift(header.join(','));
        const csvArray = csv.join('\r\n');

        return csvArray;
    }

    public async getExportDataAsText()
    {
        this.displayText = "Formatting....";

        this.setupExportDataAsText();
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

    public onNewUserEmailInput()
    {
        // just display for now
        console.log(" onNewUserEmailInput : " + this.destinationUserFormGroup.value.newUserEmail);
    }

    public async onSendEmail()
    {
        // just display for now
        console.log(" onSendEmail -> ");
        console.log("  -> selectedExportDataSource : " + this.selectedExportDataSource);
        console.log("  -> selectedExportType : " + this.selectedExportType );
        console.log("  -> newUserEmail : " + this.destinationUserFormGroup.value.newUserEmail);

        const request: ExportDataToEmailRequest =
            new ExportDataToEmailRequest(
                this.destinationUserFormGroup.value.newUserEmail,
                this.selectedExportDataSource,
                this.selectedExportType);

        await this.booksDataService.exportDataToEmailAsync(request);

        const resp = this.booksDataService.exportDataToEmailResponse;

        if (resp == undefined)
        {
            console.log("Error in response");
        } else
        {
            console.log("Response OK : " + JSON.stringify(resp, null, 4));
        }


        //console.log('onNewUserSubmitted -> newUserName : ', this.newUserFormGroup.value.newUserName);
        //this.newUserLoginValidating = false;

        //const addUserReq: UserAddRequest =
        //    new UserAddRequest(this.newUserFormGroup.value.newUserName,
        //        this.newUserFormGroup.value.password,
        //        this.newUserFormGroup.value.newUserDescription,
        //        this.newUserFormGroup.value.newUserEmail);

        //await this.userLoginService.getAsyncUserAdd(addUserReq);

        //const resp = this.userLoginService.addUserLoginResponse;

        //if (resp == undefined) {
        //    console.log("Error in response");
        //} else {
        //    console.log("Response OK");

        //}
    }

    //#endregion
}
