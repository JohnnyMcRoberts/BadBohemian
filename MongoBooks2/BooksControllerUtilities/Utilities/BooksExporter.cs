namespace BooksControllerUtilities.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using CsvHelper;

    using BooksCore.Books;

    using BooksControllerUtilities.DataClasses;

    public static class BooksExporter
    {
        #region Constants

        public static readonly string[] ColumnHeaders =
        {
            "Date",
            "DD/MM/YYYY",
            "Author",
            "Title",
            "Pages",
            "Note",
            "Nationality",
            "Original Language",
            "Book",
            "Comic",
            "Audio",
            "Image",
            "Tags"
        };

        #endregion

        #region Private Utility functions

        public static string GetCsvHeader()
        {
            string headerString = string.Empty;

            headerString += ColumnHeaders[0];

            for (int i = 1; i < ColumnHeaders.Length; i++)
            {
                headerString += ",";
                headerString += ColumnHeaders[i];
            }

            headerString += "\n";

            return headerString;
        }

        private static string GetCsvBookRecord(Book book)
        {
            string recordString = string.Empty;

            int count = 0;
            foreach (RecordFields recordField in Enum.GetValues(typeof(RecordFields)))
            {
                if (count != 0)
                    recordString += ",";

                recordString += CsvBookRead.GetCsvBookRecordField(recordField, book);
                count++;
            }

            recordString += "\n";

            return recordString;
        }

        #endregion

        #region Public Static methods

        public static void ExportToFile(List<Book> books, out string fileContent)
        {
            fileContent = string.Empty;

            fileContent += GetCsvHeader();

            foreach(var book in books)
                fileContent += GetCsvBookRecord(book);
        }

        #endregion

        #region Nested classes

        public sealed class ExtentedStringWriter : StringWriter
        {
            private readonly Encoding stringWriterEncoding;
            public ExtentedStringWriter(StringBuilder builder, Encoding desiredEncoding)
                : base(builder)
            {
                this.stringWriterEncoding = desiredEncoding;
            }

            public override Encoding Encoding
            {
                get
                {
                    return this.stringWriterEncoding;
                }
            }
        }

        #endregion

        public static void ExportToCsvFile(List<BookRead> books, out string fileContent)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ExtentedStringWriter sw = new ExtentedStringWriter(stringBuilder, Encoding.UTF8);

            // write the header
            sw.WriteLine(
                "Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio,Image,Tags"
            );

            // write the records
            var csv = new CsvWriter(sw);
            foreach (var book in books)
            {
                csv.WriteField(book.DateString);
                csv.WriteField(book.Date.ToString("d/M/yyyy"));
                csv.WriteField(book.Author);
                csv.WriteField(book.Title);
                csv.WriteField(book.Pages > 0 ? book.Pages.ToString() : "");
                csv.WriteField(book.Note);
                csv.WriteField(book.Nationality);
                csv.WriteField(book.OriginalLanguage);
                csv.WriteField(book.Format == BookFormat.Book ? "x" : "");
                csv.WriteField(book.Format == BookFormat.Comic ? "x" : "");
                csv.WriteField(book.Format == BookFormat.Audio ? "x" : "");
                csv.WriteField(book.ImageUrl);
                csv.WriteField(book.DisplayTags);
                csv.NextRecord();
            }

            // tidy up
            sw.Close();

            fileContent = sw.ToString();

        }
    }
}
