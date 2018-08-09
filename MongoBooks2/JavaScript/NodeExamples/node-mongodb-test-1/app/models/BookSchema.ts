/*   app/models/BookSchema.ts */
import * as mongoose from 'mongoose';
import { IBook } from "../interfaces";

const Schema = mongoose.Schema;

export const BookSchema = new Schema({
  dateString: {
    type: String,
    required: 'Enter a date'
  },
  date: {
    type: Date,
    default: Date.now
  },
  author: {
    type: String,
    required: 'Enter an author'
  },
  title: {
    type: String,
    required: 'Enter a title'
  },
  pages: {
    type: Number,
    default: 0
  },
  note: {
    type: String
  },
  nationality: {
    type: String
  },
  original_language: {
    type: String
  },
  image_url: {
    type: String,
  },
  tags: {
    type: [String]
  },
  format: {
    type: String,
    enum: ['Book', 'Comic','Audio'],
    default: 'Book'
  }
});