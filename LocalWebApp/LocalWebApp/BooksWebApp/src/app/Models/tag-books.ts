import { Book } from './book'

export interface ITagBooks {
    name: string;
    totalPages: number;
    totalBooks: number;
    books: Book[];
};

export class TagBooks implements ITagBooks {
    constructor(
        public name: string = "",
        public totalPages: number = 0,
        public totalBooks: number = 0,
        public books: Book[] = new Array<Book>()) {
    }
}
