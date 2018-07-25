import ICountryTotal = books.ICountryTotal;

export class CountryTotal implements ICountryTotal
{
  public nationality: string;
  public total: number;

  public constructor(aNationality: string, aTotal: number)
  {
    this.nationality = aNationality;
    this.total = aTotal;
  }
}
