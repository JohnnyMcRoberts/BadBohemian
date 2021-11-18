import { Book } from './book'

export interface IBookTally
{
  book: Book;
  dateTime: Date;
  author: string;
  title: string;
  pages: number;

  totalBooks: number;
  totalBookFormat: number;
  totalComicFormat: number;
  totalAudioFormat: number;
  totalPagesRead: number;
};

export class BookTally implements IBookTally
{
  constructor(
    public book: Book = new Book(),
    public dateTime: Date = new Date(),
    public author: string = "",
    public title: string = "",
    public pages: number = 0,
    public totalBooks: number = 0,
    public totalBookFormat: number = 0,
    public totalComicFormat: number = 0,
    public totalAudioFormat: number = 0,
    public totalPagesRead: number = 0)
  {
  }
}
