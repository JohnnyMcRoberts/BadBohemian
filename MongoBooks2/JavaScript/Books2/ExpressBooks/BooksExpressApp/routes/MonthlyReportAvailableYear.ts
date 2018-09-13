import { MonthDate } from './MonthDate'
import { MonthlyBooksRead } from "./MonthlyBooksRead";

export class AvailableReportMonth {
  public name: string;
  public index: number;

  constructor(name: string, index: number) {
    this.name = name;
    this.index = index;
  }
}

export class MonthlyReportAvailableYear
{
  public year: number;
  public availableMonths: AvailableReportMonth[];

  constructor(year: number, listBooksRead: MonthlyBooksRead[])
  {
    this.year = year;

    const months = ["January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"];

    this.availableMonths = new Array<AvailableReportMonth>();
    for (let monthIndex in listBooksRead)
    {
      if (listBooksRead.hasOwnProperty(monthIndex))
      {
        const monthDate: MonthDate = listBooksRead[monthIndex].monthDate;
        if (monthDate.year === year)
        {
          this.availableMonths.push(new AvailableReportMonth(months[monthDate.month], +monthIndex));
        }
      }
    }
  }
}