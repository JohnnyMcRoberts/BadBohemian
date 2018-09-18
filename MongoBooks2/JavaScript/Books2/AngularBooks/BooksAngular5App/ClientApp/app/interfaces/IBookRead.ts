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
