export interface IBook {
  _id: string;
  dateString: string;
  date: Date;
  author: string;
  title: string;
  pages: number;
  note: string;
  nationality: string;
  originalLanguage: string;
  image_url: string;
  tags: Array<string>;
  format: number;
  prettyDate: string;
}
