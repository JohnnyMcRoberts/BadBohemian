import { IBook, Format } from "./book";

export class BookRead
{
  _id: string;
  dateString: string;
  date: Date;
  title: string;
  author: string;
  pages: number;
  note: string;
  nationality: string;
  originalLanguage: string;
  image_url: string;
  tags: Array<string>;
  format: Format;

  constructor(theBook: IBook)
  {
    this._id = theBook._id;
    this.dateString = theBook.dateString;
    this.date = theBook.date;
    this.title = theBook.title;
    this.author = theBook.author;
    this.pages = theBook.pages;
    this.note = theBook.note;
    this.nationality = theBook.nationality;
    this.originalLanguage = theBook.originalLanguage;
    this.image_url = theBook.image_url;
    this.tags = theBook.tags;
    this.format = theBook.format;
  }

  public displayFormat(): string {
    const formats = ["undefined", "Book", "Comic", "Audio"];
    return formats[this.format % formats.length];
  }

  public displayDate(): string {
    const months = ["January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"];
    const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    const weekday = days[this.date.getDay()];
    const displayDay = this.nthDay(this.date.getDate());
    const month = months[this.date.getMonth()];
    return weekday + " " + displayDay + " " + month + " " + this.date.getFullYear();
  }

  private nthDay(day: number): string {
    switch (day % 10) {
    case 1:
      return day + "st";
    case 2:
      return day + "nd";
    case 3:
      return day + "rd";
    default:
      return day + "th";
    }
  }
  
}