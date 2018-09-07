// book.ts
import * as mongoose from 'mongoose';

//const uri: string = 'mongodb://127.0.0.1:27017/local';
const mongoIP: string = '127.0.0.1';
const mongoPort: string = '27017';
const databaseName: string = 'books_read';
const uri: string = 'mongodb://' + mongoIP + ':' + mongoPort + '/' + databaseName;
//const uri: string = 'mongodb://127.0.0.1:27017/books_read';

mongoose.connect(uri, (err: any) => {
  if (err)
  {
    console.log(err.message);
  }
  else
  {
    console.log("Succesfully Connected to " + uri + "!");
  }
});

export enum Format {
  Book = 1,
  Comic,
  Audio
};

export interface IBookRead {
  _id: string;
  dateString: string;
  date: Date;
  title: string;
  author: string;
  pages: number;
  note: string;
  nationality: string;
  originalLanguage: string;
  image_url: string;
  tags: Array<string>;
  format: Format;
};

export interface IBook extends mongoose.Document, IBookRead {
  _id: string;
  dateString: string;
  date: Date;
  title: string;
  author: string;
  pages: number;
  note: string;
  nationality: string;
  originalLanguage: string;
  image_url: string;
  tags: Array<string>;
  format: Format;
};

export const BookSchema = new mongoose.Schema({
  _id: { type: String, required: true },
  dateString: { type: String, required: true },
  date: { type: Date, required: true },
  title: { type: String, required: true },
  author: { type: String, required: true },
  pages: { type: Number, required: true },
  note: { type: String, required: false },
  nationality: { type: String, required: true },
  originalLanguage: { type: String, required: true },
  image_url: { type: String, required: true },
  tags: [{
    type: String
  }],
  format: { type: Number, required: true }
});

//const Book = mongoose.model('Book', BookSchema);
const Book = mongoose.model('books', BookSchema);
export default Book;