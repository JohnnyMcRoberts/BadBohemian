import { Book } from './book'

export interface IMonthlyTally {
    monthDate: Date;
    name: string;
    daysInTheMonth: number;
    totalBooks: number;
    totalPagesRead: number;
    totalBookFormat: number;
    totalComicFormat: number;
    totalAudioFormat: number;
    percentageInEnglish: number;
    percentageInTranslation: number;
    pageRate: number;
    daysPerBook: number;
    pagesPerBook: number;
    booksPerYear: number;
    books: Book[];
};

export class MonthlyTally implements IMonthlyTally {
    constructor(
        public monthDate: Date = new Date(),
        public name: string = "",
        public daysInTheMonth: number = 0,
        public totalBooks: number = 0,
        public totalPagesRead: number = 0,
        public totalBookFormat: number = 0,
        public totalComicFormat: number = 0,
        public totalAudioFormat: number = 0,
        public percentageInEnglish: number = 0,
        public percentageInTranslation: number = 0,
        public pageRate: number = 0,
        public daysPerBook: number = 0,
        public pagesPerBook: number = 0,
        public booksPerYear: number = 0,
        public books: Book[] = new Array<Book>()) {
    }
}
