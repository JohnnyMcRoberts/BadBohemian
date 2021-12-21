import { Component, OnInit, AfterViewInit, ViewChild, TemplateRef, ElementRef } from '@angular/core';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Clipboard } from '@angular/cdk/clipboard';

import { MatDialog } from '@angular/material/dialog';
import { MatAutocompleteSelectedEvent, MatAutocomplete } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { COMMA, ENTER } from '@angular/cdk/keycodes';

import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { EditorDetails } from './../../../Models/editor-details';
import { Book, BookReadAddResponse, BookReadAddRequest } from './../../../Models/book';

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
        private clipboard: Clipboard,
        private router: Router,
        private dialog: MatDialog,
        private formBuilder: FormBuilder,
        private booksDataService: BooksDataService,
        public currentLoginService: CurrentLoginService)
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

        this.filteredTags = this.tagsControl.valueChanges.pipe(
            startWith(null),
            map((tag: string | null) => tag ? this.filterTags(tag) : this.unselectedTags.slice()));
    }

    @ViewChild('addedDialog') addedDialog: TemplateRef<any> | any;
    @ViewChild('failedDialog') failedDialog: TemplateRef<any> | any;

    @ViewChild('tagsInput') tagsInput: ElementRef<HTMLInputElement> | any;
    @ViewChild('autocompleteTags') matAutocomplete: MatAutocomplete | any;

    public componentTitle: string;
    public newBook: Book = new Book();
    public editorDetails: EditorDetails | any;

    public selectedBookToDisplay: boolean = false;
    public updateInProgress: boolean = false;

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

        this.booksDataService.fetchEditorDetails().then(() =>
        {
            this.editorDetails = this.booksDataService.editorDetails;
            this.setupSelectionOptions();
        });
    }

    ngAfterViewInit() {
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
                this.allCurrentTags = this.editorDetails.tags;
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

    private hardRefresh(): Promise<void>
    {
        // to refresh the page fist navigate to the application root
        return this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
        {

            // then navigate back to the current page item
            this.router.navigate(['/forms']);
        });
    }

    //#region Main Form

    public addNewBookForm: FormGroup;

    public async onSubmit()
    {
        this.setupNewBook();

        const addRequest: BookReadAddRequest =
            BookReadAddRequest.fromBook(this.newBook, this.currentLoginService.userId);

        this.updateInProgress = true;
        await this.booksDataService.addAsyncBookRead(addRequest);
        console.log("addAsyncBookRead has returned");

        const resp = BookReadAddResponse.fromData(this.booksDataService.addBookReadResponse);
        this.updateInProgress = false;

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
                    console.log('The dialog was closed:' + result.toString());
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

        const authorCountry: string = this.addNewBookForm.value.authorCountry as string;
        const countrySelection = this.countryLookup.get(authorCountry) as SelectionItem;
        const bookFormat: string = this.addNewBookForm.value.bookFormat as string;
        const formatSelection = this.formatLookup.get(bookFormat) as NumericSelectionItem;

        const format: string = formatSelection.viewValue as string;
        const country: string = countrySelection.viewValue;

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

    public selectedBook: Book | any;

    public onNewBookDisplay()
    {
        this.setupNewBook();
        this.selectedBook = this.newBook;
        console.log('Books Row clicked: ', this.selectedBook.date);
        this.selectedBookToDisplay = true;
    }

    public setupNewBook(): void
    {
        const authorCountry: string = this.addNewBookForm.value.authorCountry as string;
        const countrySelection = this.countryLookup.get(authorCountry) as SelectionItem;
        const bookFormat: string = this.addNewBookForm.value.bookFormat as string;
        const formatSelection = this.formatLookup.get(bookFormat) as NumericSelectionItem;

        const title = this.addNewBookForm.value.bookTitle;
        const author = this.addNewBookForm.value.bookAuthor;
        this.inputDateRead = this.addNewBookForm.value.dateBookRead;
        const pages = this.addNewBookForm.value.bookPages;
        const country = countrySelection.viewValue;
        const language = this.addNewBookForm.value.originalLanguage;
        const format = formatSelection.viewValue;
        const imageUrl: string = this.addNewBookForm.value.imageUrl;
        const theTags: string[] = this.selectedTags;
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
        this.newBook.format = formatSelection.viewValue.toString();
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
    filteredAuthors: Observable<string[]> | any;
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
    public countryLookup: Map<string, SelectionItem> | any = null;
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

    public newSelectedCountry(value : any )
    {
        console.log("newSelectedCountry : " + value.toString());
    }

    //#endregion

    //#region Languages

    optionForLanguages: string[] = ['Swahili'];
    filteredLanguages: Observable<string[]> | any;
    originalLanguage: FormControl;

    private filterLanguage(value: string): string[]
    {
        const filterValue = value.toLowerCase();

        return this.optionForLanguages.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
    }

    //#endregion

    //#region Format Options

    public formatOptions: NumericSelectionItem[] = new Array<NumericSelectionItem>();
    public formatLookup: Map<string, NumericSelectionItem> | any = null;
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

    public newSelectedFormat(value: any)
    {
        console.log("newSelectedFormat : " + value.toString());
        //this.setupOutliersScatterPlots();
    }

    //#endregion

    //#region Tag Options

    public tagOptions: string[] = [''];
    public displayTags: string = "";

    public tagsSelectionChange(value: any)
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

    public async pasteImageUrlFromClipboard() {

        console.log("pasteImageUrlFromClipboard");

        navigator
            .clipboard
            .readText()
            .then((clipboardText) => {

                // copy the clipboard contents to the image url
                console.log('clipboard contents', clipboardText);

                this.addNewBookForm.patchValue(
                    {
                        imageUrl: clipboardText
                    });
            });
    }

    //#endregion

    //#region Tag Chip list Options

    visible = true;
    selectable = true;
    removable = true;
    separatorKeysCodes: number[] = [ENTER, COMMA];
    tagsControl = new FormControl();
    filteredTags: Observable<string[]>;
    selectedTags: string[] = [];
    allCurrentTags: string[] = [];

    get unselectedTags(): string[]
    {
        const selectedTagMap: Map<string, string> = new Map<string, string>();
        const unusedTags: string[] = [];

        for (let i = 0; i < this.selectedTags.length; i++)
        {
            const tag: string = this.selectedTags[i];
            selectedTagMap.set(tag, tag);
        }

        for (let i = 0; i < this.allCurrentTags.length; i++)
        {
            const tag: string = this.allCurrentTags[i];
            if (!selectedTagMap.has(tag))
            {
                unusedTags.push(tag);
            }
        }

        return unusedTags;
    }

    add(event: MatChipInputEvent): void
    {
        // get the data from the event
        const input = event.input;
        const value = event.value;

        // Add our tag
        if ((value || '').trim())
        {
            this.selectedTags.push(value.trim());
        }

        // Reset the input value
        if (input)
        {
            input.value = '';
        }

        this.tagsControl.setValue(null);

        // update the display list
        this.setupDisplayTags();
    }

    remove(fruit: string): void
    {
        // see where the item to remove is in the list
        const index = this.selectedTags.indexOf(fruit);

        if (index >= 0)
        {
            // if it is in the list remove it
            this.selectedTags.splice(index, 1);
        }

        // update the display list
        this.setupDisplayTags();
    }

    selected(event: MatAutocompleteSelectedEvent): void
    {

        // add the selected tag
        this.selectedTags.push(event.option.viewValue);

        // clear the control
        this.tagsInput.nativeElement.value = '';
        this.tagsControl.setValue(null);

        // update the display list
        this.setupDisplayTags();
    }

    setupDisplayTags(): void
    {
        this.displayTags = "";
        for (let i = 0; i < this.selectedTags.length; i++)
        {
            if (i === 0) {
                this.displayTags = this.selectedTags[0];
            }
            else
            {
                this.displayTags += ", ";
                this.displayTags += this.selectedTags[i];
            }
        }
    }

    filterTags(value: string): string[]
    {
        const filterValue = value.toLowerCase();

        return this.unselectedTags.filter(tag => tag.toLowerCase().indexOf(filterValue) === 0);
    }

    //#endregion
}
