/*   app/interfaces/IBook.ts */
import * as mongoose from 'mongoose';

export interface IBook extends mongoose.Document {
  dateString: string;
  date: Date;
  author: string;
  title: string;
  pages: number;
  note: string;
  nationality: string;
  original_language: string;
  image_url: string;
  tags: string[];
  format: string;
};