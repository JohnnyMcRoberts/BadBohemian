export class MonthDate {
  public year: number;
  public month: number;

  constructor(theDate: Date) {
    this.year = theDate.getFullYear();
    this.month = theDate.getMonth();
  }

  public toString(): string {
    return this.year.toString() + '/' + (1 + this.month).toString();
  }

  public daysInMonth(): number {
    return new Date(this.year, this.month + 1, 0).getDate();
  }

  public displayDate(): string {
    const months = ["January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"];

    const month = months[this.month];
    return month + " " + this.year;
  }
}