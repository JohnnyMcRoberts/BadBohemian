export interface IBook
{
  id: string;
  dateString: string;
  date: Date;
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
  static fromData(data: IBook)
  {
    var tags: string[] = new Array<string>();

    for (let tag of data.tags)
      tags.push(tag);

    return new this(
      data.id,
      data.dateString,
      data.date,
      data.author,
      data.title,
      data.pages,
      data.note,
      data.nationality,
      data.originalLanguage,
      data.imageUrl,
      tags,
      data.user,
      data.format);
  }

  constructor(
    public id: string = "",
    public dateString: string = "",
    public date: Date = new Date(),
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

export interface IBookReadAddRequest
{
  date: Date;
  author: string;
  title: string;
  pages: number;
  note: string;
  nationality: string;
  originalLanguage: string;
  imageUrl: string;
  tags: string[];
  userId: string;
  format: string;
};

export class BookReadAddRequest implements IBookReadAddRequest
{
  static fromData(data: IBookReadAddRequest)
  {
    var tags: string[] = new Array<string>();

    for (let tag of data.tags)
      tags.push(tag);

    return new this(
      data.date,
      data.author,
      data.title,
      data.pages,
      data.note,
      data.nationality,
      data.originalLanguage,
      data.imageUrl,
      tags,
      data.userId,
      data.format);
  }

  static fromBook(data: IBook, userId: string)
  {
    var tags: string[] = new Array<string>();

    for (let tag of data.tags)
      tags.push(tag);

    var dateRead: Date = data.date;
    dateRead.setHours(12, 0, 0);

    return new this(
      dateRead,
      data.author,
      data.title,
      data.pages,
      data.note,
      data.nationality,
      data.originalLanguage,
      data.imageUrl,
      tags,
      userId,
      data.format);
  }

  constructor(
    public date: Date = new Date(),
    public author: string = "",
    public title: string = "",
    public pages: number = 0,
    public note: string = "",
    public nationality: string = "",
    public originalLanguage: string = "",
    public imageUrl: string = "",
    public tags: string[] = new Array<string>(),
    public userId: string = "",
    public format: string = "")
  {
  }
}

export interface IBookReadAddResponse
{
  newItem: IBook;
  errorCode: number;
  failReason: string;
  userId: string;
};

export class BookReadAddResponse implements IBookReadAddResponse
{
  static fromData(data: IBookReadAddResponse) {
    var book: Book;
    if (data.newItem == undefined || data.newItem == null)
      book = new Book();
    else
      book = Book.fromData(data.newItem);

    return new this(
      book,
      data.errorCode,
      data.failReason,
      data.userId);
  }

  constructor(
    public newItem: IBook = new Book(),
    public errorCode: number = -1,
    public failReason: string = "",
    public userId: string = "")
  {
  }
};
