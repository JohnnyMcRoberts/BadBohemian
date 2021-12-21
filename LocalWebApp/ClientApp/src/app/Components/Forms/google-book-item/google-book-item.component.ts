import { Component, OnInit, Input } from "@angular/core";
import { Clipboard } from '@angular/cdk/clipboard';

import { GoogleBookInterface, GoogleBookDetailInterface, GoogleBookDetail } from './../../../Models/google-books-api-interface';

import { GoogleBookService } from './../../../Services/google-book.service';

@Component({
    selector: 'app-google-book-item',
    templateUrl: './google-book-item.component.html',
    styleUrls: ['./google-book-item.component.scss']
})
/** GoogleBookItem component*/
export class GoogleBookItemComponent implements OnInit
{
    /** GoogleBookItem ctor */
    constructor(
        private clipboard: Clipboard,
        private googleBookApiService: GoogleBookService
    )
    {

    }

    @Input() book: GoogleBookInterface | any;

    get id(): string
    {
        return this.book.id;
    }

    get thumbnail(): string {
        return this.book.volumeInfo.imageLinks
            ? this.book.volumeInfo.imageLinks.thumbnail.replace("http:", "")
            : "";
    }

    get fullPath(): string {
        return this.book.volumeInfo.imageLinks
            ? this.book.volumeInfo.imageLinks.thumbnail
            : "";
    }

    ngOnInit(): void { }

    onGetDetails()
    {
        console.log("onGetDetails: title = " + this.book.volumeInfo.title + " volume id ->" + this.book.id);

        this.googleBookApiService.fetchBookDetail(this.book.id).then(() =>
        {
            if (this.googleBookApiService.googleBookDetail !== undefined &&
                this.googleBookApiService.googleBookDetail !== null)
            {
                this.bookDetailsString = JSON.stringify(this.googleBookApiService.googleBookDetail, null, 4);

                this.bookDetails = GoogleBookDetail.fromData(this.googleBookApiService.googleBookDetail);


                console.log("\n bookDetails \n as JSON \n\n\n" + JSON.stringify(this.bookDetails, null, 4));
            }
        });
    }

    copyToClipboard(imageLinkThumbnail: string) {

        console.log(" copyToClipboard: " + imageLinkThumbnail);
        this.clipboard.copy(imageLinkThumbnail);

    }

    public bookDetailsString: string | any;
    public bookDetails: GoogleBookDetailInterface | any;
}
