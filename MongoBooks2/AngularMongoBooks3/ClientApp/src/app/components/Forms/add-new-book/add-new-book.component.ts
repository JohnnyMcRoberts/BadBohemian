import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { FormControl } from '@angular/forms';

import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { BooksDataService } from './../../../Services/books-data.service';

import { EditorDetails } from './../../../Models/EditorDetails';
import { Book } from './../../../Models/Book';

export class SelectionItem
{
  constructor(
    public value: string = "",
    public viewValue: string = ""
  )
  { }
}

export class NumericSelectionItem
{
  constructor(
    public value: string = "",
    public viewValue: string = "",
    public numericValue: number = 0
  )
  { }
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
      private formBuilder: FormBuilder,
      private booksDataService: BooksDataService)
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
          bookTags: ['']
          });
    }

    public componentTitle: string;
    public newBook: Book = new Book();
    public editorDetails: EditorDetails;

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

    ngAfterViewInit()
    {
      this.setupSelectionOptions();
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
            map(value => this.filterAuthor(value))
            );
        }

        if (this.editorDetails.countryNames != null)
        {
          this.setupCountrySelection();
        }

        if (this.editorDetails.tags != null)
        {
          this.tagOptions = this.editorDetails.tags;
        }

        if (this.editorDetails.languages != null)
        {
          this.optionForLanguages = this.editorDetails.languages;
          this.filteredLanguages = this.originalLanguage.valueChanges.pipe(
            startWith(''),
            map(value => this.filterLanguage(value))
          );
        }
      }
    }

    //#region Main Form

    public addNewBookForm: FormGroup;

    public onSubmit()
    {
      // TODO: Use EventEmitter with form value
      var title = this.addNewBookForm.value.bookTitle;
      var author = this.addNewBookForm.value.bookAuthor;
      this.inputDateRead = this.addNewBookForm.value.dateBookRead;
      var pages = this.addNewBookForm.value.bookPages;
      var country = this.countryLookup.get(this.addNewBookForm.value.authorCountry).viewValue;
      var language = this.addNewBookForm.value.originalLanguage;
      var format = this.formatLookup.get(this.addNewBookForm.value.bookFormat).viewValue;
      var theTags: string[] = this.addNewBookForm.value.bookTags;
      var notes: string = this.addNewBookForm.value.bookNotes;

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
      this.newBook.dateTime = this.inputDateRead;
      this.newBook.author = author;
      this.newBook.title = title;
      this.newBook.pages = pages as number;
      this.newBook.nationality = country;
      this.newBook.originalLanguage = language;
      this.newBook.tags = theTags;
      this.newBook.format = this.formatLookup.get(this.addNewBookForm.value.bookFormat).viewValue.toString();
      this.newBook.note = notes;
    }

    //#endregion

    //#region Date Read

    public formatDate(date: Date) : string
    {
      const month = date.toLocaleString('en-us', { month: 'long' });
      const day = date.getDate();
      const year = date.getFullYear();

      var ordinal = "th";
      if (day === 1 || day === 21 || day === 31)
        ordinal = "st";
      if (day === 2 || day === 22)
        ordinal = "nd";
      if (day === 3 || day === 23)
        ordinal = "rd";

      //var d = new Date(date),
      //  month = '' + (d.getMonth() + 1),
      //  day = '' + d.getDate(),
      //  year = d.getFullYear();

      //if (month.length < 2) month = '0' + month;
      //if (day.length < 2) day = '0' + day;

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

      for (var i = 0; i < this.editorDetails.countryNames.length; i++)
      {
        var value = 'country_' + (1 + i);
        var viewValue = this.editorDetails.countryNames[i];
        var clusterOption = new SelectionItem(value, viewValue);
        this.countryLookup.set(value, clusterOption);
        this.countryOptions.push(clusterOption);
      }
    }

    public newSelectedCountry(value)
    {
      console.log("newSelectedCountry : " + value);
      //this.setupOutliersScatterPlots();
    }

    //#endregion

    //#region Languages

    optionForLanguages: string[] = ['Swahili'];
    filteredLanguages: Observable<string[]>;
    originalLanguage: FormControl;

    private filterLanguage(value: string): string[]
    {
      const filterValue = value.toLowerCase();

      return this.optionForLanguages.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
    }

  //#endregion

    //#region Format Options

    public formatOptions: NumericSelectionItem[] = new Array<NumericSelectionItem>();
    public formatLookup: Map<string, NumericSelectionItem> = null;
    public selectedFormat = 'format_1';
    public formats:string[] = ["Book", "Comic", "Audio"];

    private setupFormatSelection(): void
    {
      this.formatOptions = new Array<NumericSelectionItem>();
      this.formatLookup = new Map<string, NumericSelectionItem>();

      for (var i = 0; i < this.formats.length; i++)
      {
        var viewValue = this.formats[i];
        var numericValue = (1 + i);
        var value = 'format_' + numericValue.toString();
        var formatOption = new NumericSelectionItem(value, viewValue, numericValue);
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
      var tagStrings: string[] = value;
      this.displayTags = "";
      for (var i = 0; i < tagStrings.length; i++)
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

  public imageUrl: string = "https://images-na.ssl-images-amazon.com/images/I/61TGJyLMu6L._SY606_.jpg";

    //#endregion
}
