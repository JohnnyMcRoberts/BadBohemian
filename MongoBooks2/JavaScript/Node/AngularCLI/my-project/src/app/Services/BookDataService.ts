import { BookData } from './../Models/BookData';
import { Book } from './../Models/Book';
import { MockBooksSet } from './../Services/MockBooks';
import IBook = books.IBook;

export class BookDataService {
  booksSet = MockBooksSet;

  constructor() {
  }

  public GetAllBookData(): IBook[]
  {
    var allBooks = new Array<IBook>();

    for (let item of this.booksSet)
    {
      if (item instanceof BookData)
      {
        var bookData = <BookData>item;
        var book = new Book(
          bookData._id,
          bookData.dateString,
          bookData.date,
          bookData.author,
          bookData.title,
          bookData.pages,
          bookData.note,
          bookData.nationality,
          bookData.originalLanguage,
          bookData.image_url,
          bookData.tags,
          bookData.format
        );

        allBooks.push(book);
      }

    }

    return allBooks;
  }

}
