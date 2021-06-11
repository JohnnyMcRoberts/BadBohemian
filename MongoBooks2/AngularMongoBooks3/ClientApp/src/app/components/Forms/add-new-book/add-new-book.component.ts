import { Component, OnInit, AfterViewInit, ViewChild, TemplateRef } from '@angular/core';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';

import { MatDialog } from '@angular/material/dialog';

import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { EditorDetails } from './../../../Models/EditorDetails';
import { Book, BookReadAddResponse, BookReadAddRequest } from './../../../Models/Book';

export class SelectionItem
{
    constructor(
        public value: string = "",
        public viewValue: string = ""
    ) { }
}

export class NumericSelectionItem
{
    constructor(
        public value: string = "",
        public viewValue: string = "",
        public numericValue: number = 0
    ) { }
}

@Component({
    selector: 'app-add-new-book',
    templateUrl: './add-new-book.component.html',
    styleUrls: ['./add-new-book.component.scss']
})
/** AddNewBook component*/
export class AddNewBookComponent implements OnInit, AfterViewInit
{
    /** AddNewBook ctor */
    constructor(
        private router: Router,
        private dialog: MatDialog,
        private formBuilder: FormBuilder,
        private booksDataService: BooksDataService,
        private currentLoginService: CurrentLoginService)
    {
        this.componentTitle = "Loading books data...";
        this.booksDataService = booksDataService;

        this.bookAuthor = new FormControl('', Validators.required);
        this.originalLanguage = new FormControl('');

        this.addNewBookForm =
            this.formBuilder.group({
                dateBookRead: ['', Validators.required],
                bookAuthor: this.bookAuthor,
                bookTitle: ['', Validators.required],
                bookPages: ['', Validators.required],
                authorCountry: ['', Validators.required],
                originalLanguage: this.originalLanguage,
                bookFormat: ['', Validators.required],
                bookNotes: [''],
                imageUrl: [''],
                bookTags: ['']
            });
    }

    @ViewChild('addedDialog') addedDialog: TemplateRef<any>;
    @ViewChild('failedDialog') failedDialog: TemplateRef<any>;

    public componentTitle: string;
    public newBook: Book = new Book();
    public editorDetails: EditorDetails;

    public selectedBookToDisplay: boolean = false;

    ngOnInit()
    {
        this.filteredAuthors = this.bookAuthor.valueChanges.pipe(
            startWith(''),
            map(value => this.filterAuthor(value))
        );

        this.filteredLanguages = this.originalLanguage.valueChanges.pipe(
            startWith(''),
            map(value => this.filterLanguage(value))
        );

        this.booksDataService.fetchEditorDetails().then(() => {
            this.editorDetails = this.booksDataService.editorDetails;
            this.setupSelectionOptions();
        });
    }

    ngAfterViewInit()
    {
        this.setupSelectionOptions();
    }

    public setupSelectionOptions()
    {
        this.setupFormatSelection();

        if (this.editorDetails !== undefined && this.editorDetails !== null)
        {
            if (this.editorDetails.authorNames !== null)
            {
                this.optionForAuthors = this.editorDetails.authorNames;
                this.filteredAuthors = this.bookAuthor.valueChanges.pipe(
                    startWith(''),
                    map(value => this.filterAuthor(value))
                );
            }

            if (this.editorDetails.countryNames !== null)
            {
                this.setupCountrySelection();
            }

            if (this.editorDetails.tags !== null)
            {
                this.tagOptions = this.editorDetails.tags;
            }

            if (this.editorDetails.languages !== null)
            {
                this.optionForLanguages = this.editorDetails.languages;
                this.filteredLanguages = this.originalLanguage.valueChanges.pipe(
                    startWith(''),
                    map(value => this.filterLanguage(value))
                );
            }
        }
    }

    private hardRefresh(): Promise<void> {

        // to refresh the page fist navigate to the application root
        return this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {

            // then navigate back to the current page item
            this.router.navigate(['/main-books-forms']);
        });
    }

    //#region Main Form

    public addNewBookForm: FormGroup;

    public async onSubmit()
    {
        this.setupNewBook();

        const addRequest: BookReadAddRequest =
            BookReadAddRequest.fromBook(this.newBook, this.currentLoginService.userId);

        await this.booksDataService.addAsyncBookRead(addRequest);
        console.log("addAsyncBookRead has returned");

        const resp = BookReadAddResponse.fromData(this.booksDataService.addBookReadResponse);

        if (resp === undefined || resp === null)
        {
            console.log("Error in response");
            this.dialog.open(this.failedDialog);
        }
        else
        {
            console.log("Response OK");

            if (resp.errorCode === 0)
            {
                console.log("Successfully added a book");
                const dialogRef = this.dialog.open(this.addedDialog);

                dialogRef.afterClosed().subscribe(result =>
                {
                    console.log('The dialog was closed');
                    this.onNewBookReset();
                });
            }
            else
            {
                console.log("Add book failed - reason:" + resp.failReason);
                this.dialog.open(this.failedDialog);
            }
        }
    }

    public async onNewBookReset()
    {
        this.newBook = new Book();
        this.selectedBookToDisplay = false;
        this.imageUrl = this.defaultImageUrl;

        const format = this.formatLookup.get(this.addNewBookForm.value.bookFormat).viewValue;
        const country = this.countryLookup.get(this.addNewBookForm.value.authorCountry).viewValue;

        this.addNewBookForm.setValue(
            {
                dateBookRead: this.newBook.date,
                bookAuthor: this.newBook.author,
                bookTitle: this.newBook.title,
                bookPages: this.newBook.pages,
                authorCountry: country,
                originalLanguage: this.newBook.originalLanguage,
                bookFormat: format,
                bookNotes: this.newBook.note,
                imageUrl: this.imageUrl,
                bookTags: this.newBook.tags
            });

        this.tagsSelectionChange(this.selectedBook.tags);
        this.displayTags = "";
        this.addNewBookForm.reset();
        this.addNewBookForm.markAsPristine();
        this.addNewBookForm.markAsUntouched();
        this.addNewBookForm.updateValueAndValidity();

        await this.hardRefresh();
    }

    public selectedBook: Book;

    public onNewBookDisplay()
    {
        this.setupNewBook();
        this.selectedBook = this.newBook;
        console.log('Books Row clicked: ', this.selectedBook.date);
        this.selectedBookToDisplay = true;
    }

    public setupNewBook(): void
{
        const title = this.addNewBookForm.value.bookTitle;
        const author = this.addNewBookForm.value.bookAuthor;
        this.inputDateRead = this.addNewBookForm.value.dateBookRead;
        const pages = this.addNewBookForm.value.bookPages;
        const country = this.countryLookup.get(this.addNewBookForm.value.authorCountry).viewValue;
        const language = this.addNewBookForm.value.originalLanguage;
        const format = this.formatLookup.get(this.addNewBookForm.value.bookFormat).viewValue;
        const imageUrl: string = this.addNewBookForm.value.imageUrl;
        const theTags: string[] = this.addNewBookForm.value.bookTags;
        const notes: string = this.addNewBookForm.value.bookNotes;

        console.warn("setupNewBook ==== >>>> ");
        console.warn("Input Date Read: " + this.inputDateRead.toString());
        console.warn("Title: " + title);
        console.warn("Author: " + author);
        console.warn("Pages: " + pages);
        console.warn("Country: " + country);
        console.warn("Language: " + language);
        console.warn("Format: " + format);
        console.warn("Notes: " + notes);
        console.warn("Tags: " + theTags.toString());

        this.newBook.dateString = this.formatDate(this.inputDateRead);
        this.newBook.date = this.inputDateRead;
        this.newBook.author = author;
        this.newBook.title = title;
        this.newBook.pages = pages as number;
        this.newBook.nationality = country;
        this.newBook.originalLanguage = language;
        this.newBook.tags = theTags;
        this.newBook.format = this.formatLookup.get(this.addNewBookForm.value.bookFormat).viewValue.toString();
        this.newBook.imageUrl = imageUrl;
        this.newBook.note = notes;

        if (this.newBook.imageUrl !== '')
        {
            this.imageUrl = imageUrl;
        }
    }

    //#endregion

    //#region Date Read

    public formatDate(date: Date): string
    {
        const month = date.toLocaleString('en-us', { month: 'long' });
        const day = date.getDate();
        const year = date.getFullYear();

        let ordinal = "th";
        if (day === 1 || day === 21 || day === 31)
            ordinal = "st";
        if (day === 2 || day === 22)
            ordinal = "nd";
        if (day === 3 || day === 23)
            ordinal = "rd";


        return day.toString() + ordinal + " " + month + " " + year.toString();
    }

    public inputDateRead = new Date();

    public currentDate = new Date();

    public setNewDateRead()
    {
        console.log(this.inputDateRead.toString());
    }

    //#endregion

    //#region Authors

    optionForAuthors: string[] = ['A. N. Other'];
    filteredAuthors: Observable<string[]>;
    bookAuthor: FormControl;

    private filterAuthor(value: string): string[]
    {
        const filterValue = value.toLowerCase();

        var authors = this.optionForAuthors.filter(option => option.toLowerCase().indexOf(filterValue) === 0);

        return authors;
    }

    //#endregion

    //#region Country Options

    public countryOptions: SelectionItem[] = new Array<SelectionItem>();
    public countryLookup: Map<string, SelectionItem> = null;
    public selectedCountry = 'country_1';

    private setupCountrySelection(): void
    {
        this.countryOptions = new Array<SelectionItem>();
        this.countryLookup = new Map<string, SelectionItem>();

        for (let i = 0; i < this.editorDetails.countryNames.length; i++)
        {
            const value = 'country_' + (1 + i);
            const viewValue = this.editorDetails.countryNames[i];
            const clusterOption = new SelectionItem(value, viewValue);
            this.countryLookup.set(value, clusterOption);
            this.countryOptions.push(clusterOption);
        }
    }

    public newSelectedCountry(value)
    {
        console.log("newSelectedCountry : " + value);
    }

    //#endregion

    //#region Languages

    optionForLanguages: string[] = ['Swahili'];
    filteredLanguages: Observable<string[]>;
    originalLanguage: FormControl;

    private filterLanguage(value: string): string[] {
        const filterValue = value.toLowerCase();

        return this.optionForLanguages.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
    }

    //#endregion

    //#region Format Options

    public formatOptions: NumericSelectionItem[] = new Array<NumericSelectionItem>();
    public formatLookup: Map<string, NumericSelectionItem> = null;
    public selectedFormat = 'format_1';
    public formats: string[] = ["Book", "Comic", "Audio"];

    private setupFormatSelection(): void
    {
        this.formatOptions = new Array<NumericSelectionItem>();
        this.formatLookup = new Map<string, NumericSelectionItem>();

        for (let i = 0; i < this.formats.length; i++)
        {
            const viewValue = this.formats[i];
            const numericValue = (1 + i);
            const value = 'format_' + numericValue.toString();
            const formatOption = new NumericSelectionItem(value, viewValue, numericValue);
            this.formatLookup.set(value, formatOption);
            this.formatOptions.push(formatOption);
        }
    }

    public newSelectedFormat(value)
    {
        console.log("newSelectedFormat : " + value);
        //this.setupOutliersScatterPlots();
    }

    //#endregion

    //#region Tag Options

    public tagOptions: string[] = [''];
    public displayTags: string = "";

    public tagsSelectionChange(value)
    {
        console.log("tagsSelectionChange : " + value.toString());
        const tagStrings: string[] = value;
        this.displayTags = "";
        for (let i = 0; i < tagStrings.length; i++)
        {
            if (i === 0)
            {
                this.displayTags = tagStrings[0];
            }
            else
            {
                this.displayTags += ", ";
                this.displayTags += tagStrings[i];
            }
        }
    }

    //#endregion

    //#region Images

    public readonly defaultImageUrl = "https://images-na.ssl-images-amazon.com/images/I/61TGJyLMu6L._SY606_.jpg";

    public imageUrl: string = this.defaultImageUrl;

    //#endregion
}
