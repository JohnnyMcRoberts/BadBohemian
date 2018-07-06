// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooksFromCsvFileImport.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The books from csv file importer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Importers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using CsvHelper;

    using BooksCore.Base;
    using BooksCore.Books;
    using BooksImportExport.Interfaces;

    /// <summary>
    /// The books from csv file importer class.
    /// </summary>
    public class BooksFromCsvFileImport : IBooksFileImport
    {
        /// <summary>
        /// Gets the import method name.
        /// </summary>
        public string Name => "Books from CSV";

        /// <summary>
        /// Gets the import file type extension.
        /// </summary>
        public string Extension => "csv";

        /// <summary>
        /// Gets the import file type filter.
        /// </summary>
        public string Filter => @"All files (*.*)|*.*|CSV files (*.csv)|*.csv";

        /// <summary>
        /// Gets the type of the items that are being imported from the file.
        /// </summary>
        public Type ImportType => typeof(BookRead);

        /// <summary>
        /// Gets the imported items.
        /// </summary>
        public List<BaseMongoEntity> ImportedItems { get; }

        /// <summary>
        /// Reads the data for this import from the file specified.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        /// <param name="errorMessage">The error message if unsuccessful.</param>
        /// <returns>True if read successfully, false otherwise.</returns>
        public bool ReadFromFile(string filename, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check the file exists.
            if (!File.Exists(filename))
            {
                errorMessage = $"File {filename} does not exist";
                return false;
            }

            // Try to deserialize the books file.
            try
            {
                using (StreamReader sr = new StreamReader(filename, Encoding.Default))
                {
                    CsvReader csv = new CsvReader(sr);

                    ImportedItems.Clear();

                    // Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio,Image,Tags
                    while (csv.Read())
                    {
                        string stringFieldDate = csv.GetField<string>(0);
                        string stringFieldDdmmyyyy = csv.GetField<string>(1);
                        string stringFieldAuthor = csv.GetField<string>(2);
                        string stringFieldTitle = csv.GetField<string>(3);
                        string stringFieldPages = csv.GetField<string>(4);
                        string stringFieldNote = csv.GetField<string>(5);
                        string stringFieldNationality = csv.GetField<string>(6);
                        string stringFieldOriginalLanguage = csv.GetField<string>(7);
                        string stringFieldBook = csv.GetField<string>(8);
                        string stringFieldComic = csv.GetField<string>(9);
                        string stringFieldAudio = csv.GetField<string>(10);
                        string stringFieldImage = csv.GetField<string>(11);
                        string stringFieldTags = csv.GetField<string>(12);

                        DateTime dateForBook;
                        if (DateTime.TryParseExact(stringFieldDdmmyyyy, "d/M/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out dateForBook))
                        {
                            ushort pages;
                            ushort.TryParse(stringFieldPages, out pages);
                            List<string> tags = new List<string>();
                            if (stringFieldTags.Length > 0)
                            {
                                tags = stringFieldTags.Split(',').ToList();
                            }

                            BookRead book = new BookRead
                            {
                                DateString = stringFieldDate,
                                Date = dateForBook,
                                Author = stringFieldAuthor,
                                Title = stringFieldTitle,
                                Pages = pages,
                                Note = stringFieldNote,
                                Nationality = stringFieldNationality,
                                OriginalLanguage = stringFieldOriginalLanguage,
                                Audio = stringFieldAudio,
                                Book = stringFieldBook,
                                Comic = stringFieldComic,
                                ImageUrl = stringFieldImage,
                                Tags = tags
                            };

                            ImportedItems.Add(book);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksFromCsvFileImport"/> class.
        /// </summary>
        public BooksFromCsvFileImport()
        {
            ImportedItems = new List<BaseMongoEntity>();
        }
    }
}
