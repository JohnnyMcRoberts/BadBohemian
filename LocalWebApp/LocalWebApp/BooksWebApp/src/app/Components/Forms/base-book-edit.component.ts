import { OnInit, EventEmitter, AfterViewInit, ElementRef, Injectable } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { FormControl } from '@angular/forms';

import { MatAutocompleteSelectedEvent, MatAutocomplete } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { COMMA, ENTER } from '@angular/cdk/keycodes';

import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { BooksDataService } from './../../Services/books-data.service';
import { CurrentLoginService } from './../../Services/current-login.service';

import { EditorDetails } from './../../Models/editor-details';
import { Book } from './../../Models/book';

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

/** BaseEditBook component*/
@Injectable()
export abstract class BaseEditBookComponent implements OnInit, AfterViewInit
{
    /** BaseEditBook ctor */
    constructor(
        public formBuilder: FormBuilder,
        public booksDataService: BooksDataService,
        public currentLoginService: CurrentLoginService)
    {
        this.componentTitle = "Loading books data...";
        this.booksDataService = booksDataService;

        this.setupFormGroup();
    }

    public componentTitle: string;
    public newBook: Book = new Book();
    public editorDetails: EditorDetails | any;

    ngOnInit()
    {
        this.filteredAuthors = this.bookAuthor.valueChanges.pipe(
            startWith(''),
            map(value => this.filterAuthor(value as string))
        );

        this.filteredLanguages = this.originalLanguage.valueChanges.pipe(
            startWith(''),
            map(value => this.filterLanguage(value as string))
        );

        this.booksDataService.fetchEditorDetails().then(() =>
        {
            this.editorDetails = this.booksDataService.editorDetails;
            this.setupSelectionOptions();
        });

        this.filteredTags = this.tagsControl.valueChanges.pipe(
            startWith(null),
            map((tag: string | null) => tag ? this.filterTags(tag) : this.unselectedTags.slice()));

        this.ngOnInitAddition();
    }

    ngAfterViewInit()
    {
        this.setupSelectionOptions();
        this.ngAfterViewInitAddition();
    }

    public setupSelectionOptions()
    {
        this.setupFormatSelection();

        if (this.editorDetails != undefined && this.editorDetails != null)
        {
            if (this.editorDetails.authorNames != null)
            {
                this.optionForAuthors = this.editorDetails.authorNames;
                this.filteredAuthors = this.bookAuthor.valueChanges.pipe(
                    startWith(''),
                    map(value => this.filterAuthor(value as string))
                );
            }

            if (this.editorDetails.countryNames != null)
            {
                this.setupCountrySelection();
            }

            if (this.editorDetails.tags != null)
            {
                this.tagOptions = this.editorDetails.tags;
                this.allCurrentTags = this.editorDetails.tags;
            }

            if (this.editorDetails.languages != null)
            {
                this.optionForLanguages = this.editorDetails.languages;
                this.filteredLanguages = this.originalLanguage.valueChanges.pipe(
                    map(value => this.filterLanguage(value as string))
                );
            }
        }
    }

    //#region Main Form

    public editBookFormGroup: FormGroup | any;

    public setupFormGroup(): void
    {
        this.bookAuthor = new FormControl('', Validators.required);
        this.originalLanguage = new FormControl('');

        this.editBookFormGroup =
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

    public selectedBookToDisplay: boolean = false;

    public selectedBook: Book | any;

    public setupNewBook(): void
    {
        const title = this.editBookFormGroup.value.bookTitle;
        const author = this.editBookFormGroup.value.bookAuthor;
        this.inputDateRead = this.editBookFormGroup.value.dateBookRead;
        const pages = this.editBookFormGroup.value.bookPages;
        const country = this.countryLookup.get(this.editBookFormGroup.value.authorCountry).viewValue;
        const language = this.editBookFormGroup.value.originalLanguage;
        const format = this.formatLookup.get(this.editBookFormGroup.value.bookFormat).viewValue;
        const imageUrl: string = this.editBookFormGroup.value.imageUrl;
        const theTags: string[] = this.editBookFormGroup.value.bookTags;
        const notes: string = this.editBookFormGroup.value.bookNotes;

        console.warn("setupNewBook ==== >>>> ");
        console.warn("Input Date Read: " + this.inputDateRead.toString());
        console.warn("Title: " + title);
        console.warn("Author: " + author);
        console.warn("Pages: " + pages);
        console.warn("Country: " + country);
        console.warn("Language: " + language);
        console.warn("Format: " + format);
        console.warn("Notes: " + notes);
        console.warn("Tags: " + theTags.join(", "));

        this.newBook.dateString = this.formatDate(this.inputDateRead);
        this.newBook.date = this.inputDateRead;
        this.newBook.author = author;
        this.newBook.title = title;
        this.newBook.pages = pages as number;
        this.newBook.nationality = country;
        this.newBook.originalLanguage = language;
        this.newBook.tags = theTags;
        this.newBook.format =
            this.formatLookup.get(this.editBookFormGroup.value.bookFormat).viewValue.toString();
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
    bookAuthor: FormControl | any;

    private filterAuthor(value: string): string[]
    {
        const filterValue = value.toLowerCase();

        const authors =
            this.optionForAuthors.filter(option => option.toLowerCase().indexOf(filterValue) === 0);

        return authors;
    }

    //#endregion

    //#region Country Options

    public countryOptions: SelectionItem[] = new Array<SelectionItem>();
    public countryLookup: Map<string, SelectionItem> | any = null;
    public selectedCountry = 'country_1';

    public setupCountrySelection(): void
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

    public newSelectedCountry(value: any) {
        console.log("newSelectedCountry : " + value.ToString());
        //this.setupOutliersScatterPlots();
    }

    //#endregion

    //#region Languages

    optionForLanguages: string[] = ['Swahili'];
    filteredLanguages: Observable<string[]> | any;
    originalLanguage: FormControl | any;

    public filterLanguage(value: string): string[]
    {
        const filterValue = value.toLowerCase();

        return this.optionForLanguages.filter(
            option => option.toLowerCase().indexOf(filterValue) === 0);
    }

    //#endregion

    //#region Format Options

    public formatOptions: NumericSelectionItem[] = new Array<NumericSelectionItem>();
    public formatLookup: Map<string, NumericSelectionItem> | any = null;
    public selectedFormat = 'format_1';
    public formats: string[] = ["Book", "Comic", "Audio"];

    public setupFormatSelection(): void
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

    public newSelectedFormat(value : any)
    {
        console.log("newSelectedFormat : " + value.toString());
        //this.setupOutliersScatterPlots();
    }

    //#endregion

    //#region Tag Options

    public tagOptions: string[] = [''];
    public displayTags: string = "";

    public tagsSelectionChange(value : any)
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

    public readonly defaultImageUrl =
        "https://images-na.ssl-images-amazon.com/images/I/61TGJyLMu6L._SY606_.jpg";

    public imageUrl: string = this.defaultImageUrl;

    //#endregion

    //#region Tag Chip list Options

    visible = true;
    selectable = true;
    removable = true;
    separatorKeysCodes: number[] = [ENTER, COMMA];
    tagsControl = new FormControl();
    filteredTags: Observable<string[]> | any;
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
            if (i === 0)
            {
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

    //#region Abstract Elements

    //@Output() change = new EventEmitter();	
    public abstract change: EventEmitter<{}>;

    public abstract ngOnInitAddition() : void;

    public abstract ngAfterViewInitAddition() : void;

    //@ViewChild('tagsInput')
    public abstract tagsInput: ElementRef<HTMLInputElement>;
    //@ViewChild('autocompleteTags')
    public abstract matAutocomplete: MatAutocomplete;

    //#endregion
}
