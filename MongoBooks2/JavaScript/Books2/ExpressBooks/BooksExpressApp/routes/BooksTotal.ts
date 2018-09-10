import { IBookRead, Format } from "./book";

export class BooksTotal {
  public totalDays: number;
  public totalBooks: number;
  public totalPages: number;

  public totalBookFormat: number;
  public totalComicFormat: number;
  public totalAudioFormat: number;

  public percentageInEnglish: number;
  public percentageInTranslation: number;

  public pageRate: number;
  public daysPerBook: number;
  public pagesPerBook: number;
  public booksPerYear: number;

  constructor(days: number, books: IBookRead[]) {
    this.totalDays = days;
    this.totalBooks = books.length;

    // Default the totals to zero.
    let totalInEnglish: number = 0;
    let totalInTranslation: number = 0;
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

      if (book.format === Format.Book) {
        this.totalBookFormat++;
      }
      else if (book.format === Format.Audio) {
        this.totalAudioFormat++;
      }
      else if (book.format === Format.Comic) {
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