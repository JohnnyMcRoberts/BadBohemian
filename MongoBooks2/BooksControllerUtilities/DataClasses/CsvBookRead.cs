namespace BooksControllerUtilities.DataClasses
{
    public enum RecordFields
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

        public static string GetCsvBookRecordField(RecordFields recordField, Book book)
        {
            string fieldString = string.Empty;
            switch (recordField)
            {
                case RecordFields.DateString:
                    fieldString += book.DateString;
                    break;

                case RecordFields.Date:
                    fieldString +=
                       book.Date.Day + "/" + book.Date.Month + "/" + book.Date.Year;
                    break;

                case RecordFields.Author:
                    fieldString += GetSafeString(book.Author);
                    break;

                case RecordFields.Title:
                    fieldString += GetSafeString(book.Title);
                    break;

                case RecordFields.Pages:
                    if (!(book.Pages == 0 && (book.Format == "Audio" || book.Format == "Comic")))
                        fieldString += book.Pages;
                    break;

                case RecordFields.Note:
                    fieldString += GetSafeString(book.Note);
                    break;

                case RecordFields.Nationality:
                    fieldString += GetSafeString(book.Nationality);
                    break;

                case RecordFields.OriginalLanguage:
                    fieldString += GetSafeString(book.OriginalLanguage);
                    break;

                case RecordFields.Book:
                    if (book.Format == "Book")
                        fieldString += "x";
                    break;

                case RecordFields.Comic:
                    if (book.Format == "Comic")
                        fieldString += "x";
                    break;
                case RecordFields.Audio:
                    if (book.Format == "Audio")
                        fieldString += "x";
                    break;

                case RecordFields.Image:
                    fieldString += GetSafeString(book.ImageUrl);
                    break;

                case RecordFields.Tags:
                    {
                        string tags = string.Empty;
                        for (int i = 0; i < book.Tags.Length; i++)
                        {
                            if (i != 0)
                                tags += ", ";

                            tags += book.Tags[i];
                        }

                        fieldString += GetSafeString(tags);

                    }
                    break;
            }

            return fieldString;
        }

        public static string GetSafeString(string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
                return string.Empty;

            return fieldValue;
        }

        #endregion

        #region Constructors

        public CsvBookRead(Book book)
        {
            DateString = GetCsvBookRecordField(RecordFields.DateString, book);
            Date = GetCsvBookRecordField(RecordFields.Date, book);
            Author = GetCsvBookRecordField(RecordFields.Author, book);
            Title = GetCsvBookRecordField(RecordFields.Title, book);
            Pages = GetCsvBookRecordField(RecordFields.Pages, book);
            Note = GetCsvBookRecordField(RecordFields.Note, book);
            Nationality = GetCsvBookRecordField(RecordFields.Nationality, book);
            OriginalLanguage = GetCsvBookRecordField(RecordFields.OriginalLanguage, book);
            Book = GetCsvBookRecordField(RecordFields.Book, book);
            Comic = GetCsvBookRecordField(RecordFields.Comic, book);
            Audio = GetCsvBookRecordField(RecordFields.Audio, book);
            Image = GetCsvBookRecordField(RecordFields.Image, book);
            Tags = GetCsvBookRecordField(RecordFields.Tags, book);
        }

        #endregion
    }
}
