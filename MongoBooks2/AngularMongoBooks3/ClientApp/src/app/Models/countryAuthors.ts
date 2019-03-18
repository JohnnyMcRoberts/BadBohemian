import { Author } from './author'

export interface ICountryAuthors
{
  name: string;
  totalBooksReadInLanguage: number;
  totalPagesReadInLanguage: number;
  totalBooksWorldWide: number;
  totalPagesWorldWide: number;
  percentageOfBooksRead: number;
  percentageOfPagesRead: number;
  flagUrl: string;
  authors: Author[];
};

export class CountryAuthors implements ICountryAuthors
{
  constructor(
    public name: string = "",
    public totalBooksReadInLanguage: number = 0,
    public totalPagesReadInLanguage: number = 0,
    public totalBooksWorldWide: number = 0,
    public totalPagesWorldWide: number = 0,
    public percentageOfBooksRead: number = 0,
    public percentageOfPagesRead: number = 0,
    public flagUrl: string = "",
    public authors: Author[] = new Array<Author>())
  {
  }

  public authorsNames: string;

  public static getAuthorsAsHtmlList(item: CountryAuthors): string
  {
    var htmlList: string = "<ul style=\"list-style-type:none;\">";

    if (item.authors != null && item.authors.length > 0) {
      for (let author of item.authors) {
        htmlList += "<li>" + author.name + "</li>";
      }
    }

    htmlList += "</ul>";

    return htmlList;
  }
}
