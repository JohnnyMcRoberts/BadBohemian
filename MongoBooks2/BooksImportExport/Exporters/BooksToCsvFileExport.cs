// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooksToCsvFileExport.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The books to csv file exporter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Exporters
{
    using System;
    using System.IO;
    using System.Text;

    using CsvHelper;

    using BooksCore.Books;
    using BooksCore.Interfaces;
    using BooksImportExport.Interfaces;

    public class BooksToCsvFileExport : IBooksFileExport
    {
        /// <summary>
        /// Gets the export method name.
        /// </summary>
        public string Name => "Books To File";

        /// <summary>
        /// Gets the export file type extension.
        /// </summary>
        public string Extension => "csv";

        /// <summary>
        /// Gets the export file type filter.
        /// </summary>
        public string Filter => @"All files (*.*)|*.*|CSV files (*.csv)|*.csv";

        /// <summary>
        /// Writes the data for this export to the file specified.
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        /// <param name="geographyProvider">The geography data provider.</param>
        /// <param name="booksReadProvider">The books data provider.</param>
        /// <param name="errorMessage">The error message if unsuccessful.</param>
        /// <returns>True if written successfully, false otherwise.</returns>
        public bool WriteToFile(
            string filename,
            IGeographyProvider geographyProvider,
            IBooksReadProvider booksReadProvider,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                StreamWriter sw = new StreamWriter(filename, false, Encoding.Default); //overwrite original file

                // write the header
                sw.WriteLine(
                    "Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio,Image,Tags"
                );

                // write the records
                CsvWriter csv = new CsvWriter(sw);
                foreach (BookRead book in booksReadProvider.BooksRead)
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
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                return false;
            }

            return true;
        }
    }
}
