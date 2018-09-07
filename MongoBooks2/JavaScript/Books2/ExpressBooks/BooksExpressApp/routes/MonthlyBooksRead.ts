import { BookRead } from './BookRead'

export class MonthDate {
  year: number;
  month: number;
  constructor(theDate: Date) {
    this.year = theDate.getFullYear();
    this.month = theDate.getMonth();
  }
}

export class MonthlyBooksRead {
  monthDate: MonthDate;
  listBooksRead: BookRead[];
  constructor(theDate: Date)
  {
    this.monthDate = new MonthDate(theDate);
    this.listBooksRead = new Array<BookRead>();
  }
}