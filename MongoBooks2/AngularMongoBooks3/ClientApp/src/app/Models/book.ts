export interface IBook
{
  dateString: string;
  dateTime: Date;
  author: string;
  title: string;
  pages: number;
  note: string;
  nationality: string;
  originalLanguage: string;
  imageUrl: string;
  tags: string[];
  user: string;
  format: string;
};

export class Book implements IBook
{
  constructor(
    public dateString: string = "",
    public dateTime: Date = new Date(),
    public author: string = "",
    public title: string = "",
    public pages: number = 0,
    public note: string = "",
    public nationality: string = "",
    public originalLanguage: string = "",
    public imageUrl: string = "",
    public tags: string[] = new Array<string>(),
    public user: string = "",
    public format: string = "")
  {
  }
}
