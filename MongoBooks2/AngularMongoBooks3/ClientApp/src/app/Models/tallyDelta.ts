import { Book } from './book'

export interface ITallyDelta
{
  daysInTally: number;
  totalPages: number;
  totalBooks: number;
  totalBookFormat: number;
  totalComicFormat: number;
  totalAudioFormat: number;
  percentageInEnglish: number;
  percentageInTranslation: number;
  pageRate: number;
  daysPerBook: number;
  pagesPerBook: number;
  booksPerYear: number;
};

export class TallyDelta implements ITallyDelta
{
  constructor(
    public daysInTally: number = 0,
    public totalPages: number = 0,
    public totalBooks: number = 0,
    public totalBookFormat: number = 0,
    public totalComicFormat: number = 0,
    public totalAudioFormat: number = 0,
    public percentageInEnglish: number = 0,
    public percentageInTranslation: number = 0,
    public pageRate: number = 0,
    public daysPerBook: number = 0,
    public pagesPerBook: number = 0,
    public booksPerYear: number = 0)
  {
  }
}
