import { Component, OnInit, Output, EventEmitter, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';

import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatAutocomplete } from '@angular/material/autocomplete';
import { MAT_DATE_LOCALE } from '@angular/material/core';

import { map, startWith } from 'rxjs/operators';

import { BooksDataService } from './../../../Services/books-data.service';
import { CurrentLoginService } from './../../../Services/current-login.service';

import { Book, IBook, BookReadAddResponse } from './../../../Models/book';

import { BaseEditBookComponent, SelectionItem, NumericSelectionItem } from './../base-book-edit.component';

@Component({
    selector: 'app-edit-existing-book',
    templateUrl: './edit-existing-book.component.html',
    styleUrls: ['./edit-existing-book.component.scss'],
    providers: [{ provide: MAT_DATE_LOCALE, useValue: 'en-GB' }]
})
/** EditExistingBook component*/
export class EditExistingBookComponent extends BaseEditBookComponent implements OnInit, AfterViewInit
{
    /** EditExistingBook ctor */
    constructor(
        public formBuilder: FormBuilder,
        public booksDataService: BooksDataService,
        public currentLoginService: CurrentLoginService)
    {
        super(formBuilder, booksDataService, currentLoginService);
    }

    //#region BaseAlbumEdit Implementation

    @Output() change = new EventEmitter();

    public ngOnInitAddition() : void
    {
        this.booksDataService.fetchAllBooksData().then(() =>
        {
            this.books = this.booksDataService.books as Book[];
            this.booksDataSource = new MatTableDataSource(this.books);
            this.setupBooksPagingAndSorting();
        });
    };

    public ngAfterViewInitAddition(): void
    {
        this.setupBooksPagingAndSorting();
    };

    //#endregion

    //#region Local data handlers

    public selectedBook: Book = new Book();

    public displayOnResp(resp: BookReadAddResponse): void
    {
        if (resp == undefined)
        {
            console.log("Error in response");
        }
        else
        {
            console.log("Response OK");

            if (resp.errorCode === 0)
            {
                console.log("Successfully added a book");
            }
            else
            {
                console.log("Add book failed - reason:" + resp.failReason);
            }
        }
    }

    //#endregion

    //#region Accordion State implementation

    public showAllBooksPanelOpenState = true;
    public showEditBookPanelOpenState = true;
    public editBook: Book | any = null;
    public bookToEdit: boolean = false;
    public updateInProgress: boolean = false;

    //#endregion

    //#region Populate Books Table

    public books: Book[] | any;
    public booksDisplayedColumns: string[] =
        [
            'date',
            'author',
            'title',
            'pages',
            'nationality',
            'originalLanguage',
            'format',
            'cover',
            'note'
        ];

    @ViewChild('booksTablePaginator') booksTablePaginator: MatPaginator | any;
    @ViewChild('booksTableSort') public booksTableSort: MatSort | any;
    public booksDataSource: MatTableDataSource<Book> | any;

    private setupBooksPagingAndSorting(): void
    {
        if (this.books != null)
        {
            setTimeout(() =>
            {
                this.booksDataSource.paginator = this.booksTablePaginator;
                this.booksDataSource.sort = this.booksTableSort;
                this.booksTableSort.sortChange.subscribe(() =>
                {
                    this.booksTablePaginator.pageIndex = 0;
                    this.booksTablePaginator._changePageSize(this.booksTablePaginator.pageSize);
                });
            });
        }
    }

    applyBooksFilter(eventTarget: any)
    {
        let filterValue: string = (eventTarget as HTMLInputElement).value;
        this.booksDataSource.filter = filterValue.trim().toLowerCase();

        if (this.booksDataSource.paginator)
        {
            this.booksTablePaginator.pageIndex = 0;
            this.booksTablePaginator._changePageSize(this.booksTablePaginator.pageSize);
        }
    }

    public selectedBookToDisplay: boolean = false;
    public selectedBookToEdit: boolean = false;
    public editBookRead: Book | any = null;

    onBooksRowClicked(row : IBook)
    {
        const bookRead = Book.fromData(row);
        this.selectedBook = bookRead;
        console.log('Books Row clicked: ', this.selectedBook.date);
        this.selectedBookToDisplay = false;

        this.selectedBookToEdit = true;
        this.showEditBookPanelOpenState = true;
        this.editBookRead = bookRead;

        this.setCurrentValues();
    }

    //#endregion

    //#region Tag Chip list Options

    @ViewChild('tagsInput') tagsInput: ElementRef<HTMLInputElement> | any;
    @ViewChild('autocompleteTags') matAutocomplete: MatAutocomplete | any;

    //#endregion

    //#region Update Details

    public setCurrentValues()
    {
        let dateSelected: Date = new Date(this.selectedBook.date);
        this.selectedBookReadTime.setValue(dateSelected);

        this.imageUrl = "";
        if (this.selectedBook.imageUrl !== undefined &&
            this.selectedBook.imageUrl !== null &&
            this.selectedBook.imageUrl !== '')
        {
            this.imageUrl = this.selectedBook.imageUrl;
        }

        let authorCountry: string = this.selectedBook.nationality;
        for (let i = 0; i < this.countryOptions.length; i++)
        {
            let item: SelectionItem = this.countryOptions[i];

            if (item.viewValue === this.selectedBook.nationality)
            {
                authorCountry = item.value;
                break;
            }
        }

        let format: string = this.selectedBook.format;
        for (let i = 0; i < this.formatOptions.length; i++)
        {
            let item: NumericSelectionItem = this.formatOptions[i];

            if (item.viewValue === this.selectedBook.format)
            {
                format = item.value;
                break;
            }
        }

        this.editBookFormGroup.setValue(
            {
                dateBookRead: this.selectedBook.date,
                bookAuthor: this.selectedBook.author,
                bookTitle: this.selectedBook.title,
                bookPages: this.selectedBook.pages,
                authorCountry: authorCountry,
                originalLanguage: this.selectedBook.originalLanguage,
                bookFormat: format,
                bookNotes: this.selectedBook.note,
                imageUrl: this.imageUrl,
                bookTags: this.selectedBook.tags
            });

        this.tagsSelectionChange(this.selectedBook.tags);
        this.selectedTags = this.selectedBook.tags;
    }

    public selectedBookReadTime = new FormControl(new Date());

    public inputDateRead = new Date();

    public currentDate = new Date();

    public setNewDateRead() {
        console.log(this.inputDateRead.toString());
    }

    public selectedMoment = new Date();

    public updatedBookRead: Book | any = null;

    public getUpdatedValues()
    {
        const updatedBookRead = Book.fromData(this.editBookRead);

        const inputDateRead = new Date(this.inputDateRead);
        const title = this.editBookFormGroup.value.bookTitle;
        const author = this.editBookFormGroup.value.bookAuthor;
        const pages = this.editBookFormGroup.value.bookPages;
        const country = this.countryLookup.get(this.editBookFormGroup.value.authorCountry).viewValue;
        const language = this.editBookFormGroup.value.originalLanguage;
        const format = this.formatLookup.get(this.editBookFormGroup.value.bookFormat).viewValue;
        const imageUrl: string = this.editBookFormGroup.value.imageUrl;
        const theTags: string[] = this.selectedTags;// = this.selectedBook.tags;
        const notes: string = this.editBookFormGroup.value.bookNotes;

        console.warn("setupNewBook ==== >>>> ");
        console.warn("Input Date Read: " + inputDateRead.toString());
        console.warn("Title: " + title);
        console.warn("Author: " + author);
        console.warn("Pages: " + pages);
        console.warn("Country: " + country);
        console.warn("Language: " + language);
        console.warn("Format: " + format);
        console.warn("Notes: " + notes);
        console.warn("Tags: " + theTags.toString());

        updatedBookRead.dateString = this.formatDate(inputDateRead);
        updatedBookRead.date = inputDateRead;
        updatedBookRead.author = author;
        updatedBookRead.title = title;
        updatedBookRead.pages = pages as number;
        updatedBookRead.nationality = country;
        updatedBookRead.originalLanguage = language;
        updatedBookRead.tags = theTags;
        updatedBookRead.format = this.formatLookup.get(this.editBookFormGroup.value.bookFormat).viewValue.toString();
        updatedBookRead.imageUrl = imageUrl;
        updatedBookRead.note = notes;

        this.updatedBookRead = updatedBookRead;
    }

    //#endregion

    //#region Page Button handlers

    public onDisplayUpdated()
    {
        console.log("onDisplayUpdated");

        this.selectedBookToDisplay = true;
        this.inputDateRead = this.selectedBookReadTime.value;
        this.getUpdatedValues();
        this.selectedBook = this.updatedBookRead;
    }

    public async onUpdateAlbum()
    {
        console.log("onUpdateAlbum");

        this.selectedBookToDisplay = true;
        this.inputDateRead = this.selectedBookReadTime.value;
        this.getUpdatedValues();
        this.selectedBook = this.updatedBookRead;
        this.selectedBook.user = this.currentLoginService.userId;

        this.updateInProgress = true;
        await this.booksDataService.updateAsyncBook(this.selectedBook);
        this.updateInProgress = false;

        const resp = BookReadAddResponse.fromData(this.booksDataService.updateBookResponse);

        this.displayOnResp(resp);
    }

    public async onDeleteBook()
    {
        console.log("onDeleteBook");

        this.selectedBookToDisplay = true;
        this.inputDateRead = this.selectedBookReadTime.value;
        this.getUpdatedValues();
        this.selectedBook = this.updatedBookRead;
        this.selectedBook.user = this.currentLoginService.userId;

        this.updateInProgress = true;
        await this.booksDataService.deleteAsyncBook(this.selectedBook);
        this.updateInProgress = false;

        const resp = BookReadAddResponse.fromData(this.booksDataService.deleteBookResponse);

        this.displayOnResp(resp);
    }

    //#endregion

}
