import { Book } from './book'

export interface IAuthor
{
  name: string;
  nationality: string;
  language: string;
  totalPages: number;
  totalBooksReadBy: number;
  books: Book[];
};

export class Author implements IAuthor
{
  constructor(
    public name: string = "",
    public nationality: string = "",
    public language: string = "",
    public totalPages: number = 0,
    public totalBooksReadBy: number = 0,
    public books: Book[] = new Array<Book>())
  {
  }
}
