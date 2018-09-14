import { BookRead } from './BookRead'
import { MonthDate } from './MonthDate'
import { BooksTotal } from './BooksTotal'
import { MonthlyBooksRead } from "./MonthlyBooksRead";
import { MonthlyReportAvailableYear } from "./MonthlyReportAvailableYear";
import { IBook } from "./book";
import * as Collections from 'typescript-collections';

export class MonthlyBooksReports
{
  public monthlyBooksRead: MonthlyBooksRead[];
  public monthlyReportYears: MonthlyReportAvailableYear[];
  public overallTotals: BooksTotal;

  constructor(listBooksRead: IBook[])
  {
    this.monthlyBooksRead = this.getMonthlyBooksRead(listBooksRead);
    for (let monthlyReport of this.monthlyBooksRead)
    {
      monthlyReport.updateTotals();
    }

    this.monthlyReportYears = this.getMonthlyReportYears();
    const totalDays: number = this.getDifferenceInDays(listBooksRead[0].date, listBooksRead[listBooksRead.length - 1].date);
    this.overallTotals = new BooksTotal(totalDays, listBooksRead);
  }

  private getMonthlyReportYears(): MonthlyReportAvailableYear[]
  {
    const monthlyReportAvailableYears: MonthlyReportAvailableYear[] = new Array<MonthlyReportAvailableYear>();
    const dict = new Collections.Dictionary<number, number>();
    for (let month of this.monthlyBooksRead)
    {
      if (!dict.containsKey(month.monthDate.year))
      {
        dict.setValue(month.monthDate.year, month.monthDate.year);
      }
    }

    const years: number[] = dict.values().sort((a, b) => {
      if (a < b)
        return -1;
      if (a > b)
        return 1;
      return 0;
    });

    for (let year of years)
    {
      monthlyReportAvailableYears.push(new MonthlyReportAvailableYear(year, this.monthlyBooksRead));
    }

    return monthlyReportAvailableYears;
  }

  private getMonthlyBooksRead(books: IBook[]): MonthlyBooksRead[]
  {
    const dict = new Collections.Dictionary<MonthDate, MonthlyBooksRead>();
    for (let book of books) {
      const month = new MonthDate(book.date);

      if (dict.containsKey(month)) {
        const monthSet = dict.getValue(month);
        monthSet.listBooksRead.push(new BookRead(book));
        dict.setValue(month, monthSet);
      }
      else {
        const monthOfBooks = new MonthlyBooksRead(book.date);
        monthOfBooks.listBooksRead.push(new BookRead(book));
        dict.setValue(month, monthOfBooks);
      }
    }

    return dict.values();
  }

  private getDifferenceInDays(start: Date, end: Date): number {
    let diff: number = Math.abs(start.getTime() - end.getTime());
    let diffDays: number = Math.ceil(diff / (1000 * 3600 * 24));
    return diffDays;
  }
}