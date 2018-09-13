/*
 * AuthorTotals.ts
 * the author totals
 */
import { AuthorTotal } from "./AuthorTotal";
import { IBook } from "./book";
import * as Collections from 'typescript-collections';

export class AuthorTotals
{
  public authors: AuthorTotal[];

  constructor(listBooksRead: IBook[])
  {
    const dict = new Collections.Dictionary<string, AuthorTotal>();
    for (let book of listBooksRead)
    {
      const author = book.author;

      if (dict.containsKey(author))
      {
        const authorTotal = dict.getValue(author);
        authorTotal.addBook(book);
        dict.setValue(author, authorTotal);
      }
      else
      {
        dict.setValue(author, new AuthorTotal(book));
      }
    }

    this.authors = dict.values();
  }
}