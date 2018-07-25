import IBook = books.IBook;

export class Book implements IBook {
  public _id: string;
  public dateString: string;
  public date: Date;
  public author: string;
  public title: string;
  public pages: number;
  public note: string;
  public nationality: string;
  public originalLanguage: string;
  public image_url: string;
  public tags: Array<string>;
  public format: number;
  public prettyDate: string;
  public prettyFormat: string;
  
  constructor(
    _id: string,
    dateString: string,
    date: Date,
    author: string,
    title: string,
    pages: number,
    note: string,
    nationality: string,
    originalLanguage: string,
    image_url: string,
    tags: Array<string>,
    format: number) {
    this._id = _id;
    this.dateString = dateString;
    this.date = date;
    this.author = author;
    this.title = title;
    this.pages = pages;
    this.note = note;
    this.nationality = nationality;
    this.originalLanguage = originalLanguage;
    this.image_url = image_url;
    this.tags = tags;
    this.format = format;

    this.prettyDate = this.displayDate();
    this.prettyFormat = this.displayFormat();
  }

  private displayFormat(): string {
    const formats = ["undefined", "Book", "Comic", "Audio"];
    return formats[this.format % formats.length];
  }

  private displayDate(): string {
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
