"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const book_1 = require("./book");
class BooksTotal {
    constructor(days, books) {
        this.totalDays = days;
        this.totalBooks = books.length;
        // Default the totals to zero.
        let totalInEnglish = 0;
        let totalInTranslation = 0;
        this.totalPages = 0;
        this.totalBookFormat = 0;
        this.totalComicFormat = 0;
        this.totalAudioFormat = 0;
        // Get the totals from the books list.
        for (let book of books) {
            this.totalPages += book.pages;
            if (book.originalLanguage !== "English") {
                totalInTranslation++;
            }
            else {
                totalInEnglish++;
            }
            if (book.format === book_1.Format.Book) {
                this.totalBookFormat++;
            }
            else if (book.format === book_1.Format.Audio) {
                this.totalAudioFormat++;
            }
            else if (book.format === book_1.Format.Comic) {
                this.totalComicFormat++;
            }
        }
        // Calculate the percentages and rates.
        this.percentageInEnglish = (totalInEnglish * 100.0) / this.totalBooks;
        this.percentageInTranslation = (totalInTranslation * 100.0) / this.totalBooks;
        // Calculate the percentages and rates.
        this.pageRate = this.totalPages / this.totalDays;
        this.daysPerBook = this.totalDays / this.totalBooks;
        this.pagesPerBook = this.totalPages / this.totalBooks;
        this.booksPerYear = 365.25 / this.daysPerBook;
    }
}
exports.BooksTotal = BooksTotal;
//# sourceMappingURL=BooksTotal.js.map