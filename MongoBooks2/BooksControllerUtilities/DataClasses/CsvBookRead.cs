namespace BooksControllerUtilities.DataClasses
{
    using BooksControllerUtilities.Utilities;

    public enum BookRecordFields
    {
        DateString = 0,
        Date,
        Author,
        Title,
        Pages,
        Note,
        Nationality,
        OriginalLanguage,
        Book,
        Comic,
        Audio,
        Image,
        Tags
    }

    public class CsvBookRead
    {
        #region Public Properties

        public string DateString { get; set; }

        public string Date { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Pages { get; set; }

        public string Note { get; set; }

        public string Nationality { get; set; }

        public string OriginalLanguage { get; set; }

        public string Book { get; set; }

        public string Comic { get; set; }

        public string Audio { get; set; }

        public string Image { get; set; }

        public string Tags { get; set; }

        #endregion

        #region Public Utility functions

        public static string GetCsvBookRecordField(BookRecordFields recordField, Book book)
        {
            string fieldString = string.Empty;
            switch (recordField)
            {
                case BookRecordFields.DateString:
                    fieldString += book.DateString;
                    break;

                case BookRecordFields.Date:
                    fieldString +=
                       book.Date.Day + "/" + book.Date.Month + "/" + book.Date.Year;
                    break;

                case BookRecordFields.Author:
                    fieldString += ExtendedStringWriter.GetSafeString(book.Author);
                    break;

                case BookRecordFields.Title:
                    fieldString += ExtendedStringWriter.GetSafeString(book.Title);
                    break;

                case BookRecordFields.Pages:
                    if (!(book.Pages == 0 && (book.Format == "Audio" || book.Format == "Comic")))
                        fieldString += book.Pages;
                    break;

                case BookRecordFields.Note:
                    fieldString += ExtendedStringWriter.GetSafeString(book.Note);
                    break;

                case BookRecordFields.Nationality:
                    fieldString += ExtendedStringWriter.GetSafeString(book.Nationality);
                    break;

                case BookRecordFields.OriginalLanguage:
                    fieldString += ExtendedStringWriter.GetSafeString(book.OriginalLanguage);
                    break;

                case BookRecordFields.Book:
                    if (book.Format == "Book")
                        fieldString += "x";
                    break;

                case BookRecordFields.Comic:
                    if (book.Format == "Comic")
                        fieldString += "x";
                    break;
                case BookRecordFields.Audio:
                    if (book.Format == "Audio")
                        fieldString += "x";
                    break;

                case BookRecordFields.Image:
                    fieldString += ExtendedStringWriter.GetSafeString(book.ImageUrl);
                    break;

                case BookRecordFields.Tags:
                    {
                        string tags = string.Empty;
                        for (int i = 0; i < book.Tags.Length; i++)
                        {
                            if (i != 0)
                                tags += ", ";

                            tags += book.Tags[i];
                        }

                        fieldString += ExtendedStringWriter.GetSafeString(tags);

                    }
                    break;
            }

            return fieldString;
        }

        #endregion

        #region Constructors

        public CsvBookRead(Book book)
        {
            DateString = GetCsvBookRecordField(BookRecordFields.DateString, book);
            Date = GetCsvBookRecordField(BookRecordFields.Date, book);
            Author = GetCsvBookRecordField(BookRecordFields.Author, book);
            Title = GetCsvBookRecordField(BookRecordFields.Title, book);
            Pages = GetCsvBookRecordField(BookRecordFields.Pages, book);
            Note = GetCsvBookRecordField(BookRecordFields.Note, book);
            Nationality = GetCsvBookRecordField(BookRecordFields.Nationality, book);
            OriginalLanguage = GetCsvBookRecordField(BookRecordFields.OriginalLanguage, book);
            Book = GetCsvBookRecordField(BookRecordFields.Book, book);
            Comic = GetCsvBookRecordField(BookRecordFields.Comic, book);
            Audio = GetCsvBookRecordField(BookRecordFields.Audio, book);
            Image = GetCsvBookRecordField(BookRecordFields.Image, book);
            Tags = GetCsvBookRecordField(BookRecordFields.Tags, book);
        }

        #endregion
    }
}
