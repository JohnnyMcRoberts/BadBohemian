import { BookRead } from './BookRead'
import { MonthDate } from './MonthDate'
import { BooksTotal } from './BooksTotal'
import { BreakdownTotal } from "./BreakdownTotal";

export class MonthlyBooksRead
{
  public monthDate: MonthDate;
  public listBooksRead: BookRead[];
  public monthTotals: BooksTotal;
  public breakdownTotals: BreakdownTotal[];

  constructor(theDate: Date)
  {
    this.monthDate = new MonthDate(theDate);
    this.listBooksRead = new Array<BookRead>();
  }

  public updateTotals(): void
  {
    this.monthTotals = new BooksTotal(this.monthDate.daysInMonth(), this.listBooksRead);

    this.breakdownTotals = new Array<BreakdownTotal>();
    this.breakdownTotals.push(new BreakdownTotal(true, true, this.listBooksRead));
    this.breakdownTotals.push(new BreakdownTotal(true, false, this.listBooksRead));
    this.breakdownTotals.push(new BreakdownTotal(false, true, this.listBooksRead));
    this.breakdownTotals.push(new BreakdownTotal(false, false, this.listBooksRead));
  }
}