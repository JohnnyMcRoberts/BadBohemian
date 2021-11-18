import { Author } from './author'

export interface ILanguageAuthors
{
  name: string;
  totalBooksReadInLanguage: number;
  totalPagesReadInLanguage: number;
  totalBooksWorldWide: number;
  totalPagesWorldWide: number;
  percentageOfBooksRead: number;
  percentageOfPagesRead: number;
  authors: Author[];
};

export class LanguageAuthors implements ILanguageAuthors
{
  constructor(
    public name: string = "",
    public totalBooksReadInLanguage: number = 0,
    public totalPagesReadInLanguage: number = 0,
    public totalBooksWorldWide: number = 0,
    public totalPagesWorldWide: number = 0,
    public percentageOfBooksRead: number = 0,
    public percentageOfPagesRead: number = 0,
    public authors: Author[] = new Array<Author>())
  {
  }

  public authorsNames: string;

  public static getAuthorsAsHtmlList(item: LanguageAuthors): string
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
