﻿namespace AngularMongoBooks3.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Users;
    using BooksDatabase.Implementations;
    using BooksCore.Provider;

    using AngularMongoBooks3.Controllers.DataClasses;
    using AngularMongoBooks3.Controllers.RequestsResponses;
    using AngularMongoBooks3.Controllers.Settings;
    using AngularMongoBooks3.Controllers.Utilities;
    using CsvHelper;

    [Route("api/[controller]")]
    public class BooksDataController : Controller
    {
        #region Constants

        public const string DefaultDatabaseConnectionString = "mongodb://localhost:27017";

        /// <summary>
        /// The connection string for the database.
        /// </summary>
        public readonly string DatabaseConnectionString;

        /// <summary>
        /// The export directory to put temporary files into.
        /// </summary>
        public readonly string ExportDirectory;

        public readonly DateTime EarliestDate = DateTime.Now.AddYears(-20);

        public readonly string[] SuffixedDaysOfMonths =
        {
            "0th",  "1st",  "2nd",  "3rd",  "4th",  "5th",  "6th",  "7th",  "8th",  "9th",
            "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th",
            "20th", "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th",
            "30th", "31st"
        };

        #endregion

        #region Private data

        /// <summary>
        /// The books read database.
        /// </summary>
        private readonly BooksReadDatabase _booksReadDatabase;

        /// <summary>
        /// The nations read database.
        /// </summary>
        private readonly NationDatabase _nationsReadDatabase;

        /// <summary>
        /// The users read database.
        /// </summary>
        private readonly UserDatabase _userDatabase;

        /// <summary>
        /// The books read from database.
        /// </summary>
        private ObservableCollection<BookRead> _booksReadFromDatabase;

        /// <summary>
        /// The nations read from database.
        /// </summary>
        private ObservableCollection<Nation> _nationsReadFromDatabase;

        /// <summary>
        /// The users read from database.
        /// </summary>
        private ObservableCollection<User> _usersReadFromDatabase;

        /// <summary>
        /// The books read from database.
        /// </summary>
        private ObservableCollection<Book> _books;

        /// <summary>
        /// The authors read from database.
        /// </summary>
        private ObservableCollection<Author> _authors;

        #endregion

        #region Utility Functions

        /// <summary>
        /// Gets the providers for the books and geography data.
        /// </summary>
        /// <param name="geographyProvider">The geography data provider on exit.</param>
        /// <param name="booksReadProvider">The books data provider on exit.</param>
        /// <returns>True if successful, false otherwise.</returns>
        protected bool GetProviders(out GeographyProvider geographyProvider, out BooksReadProvider booksReadProvider)
        {
            geographyProvider = null;
            booksReadProvider = null;

            if (!_booksReadDatabase.ReadFromDatabase)
            {
                _booksReadDatabase.ConnectToDatabase();
            }

            if (!_nationsReadDatabase.ReadFromDatabase)
            {
                _nationsReadDatabase.ConnectToDatabase();
            }

            if (_booksReadDatabase.ReadFromDatabase && _nationsReadDatabase.ReadFromDatabase)
            {
                // Setup the providers.
                geographyProvider = new GeographyProvider();
                geographyProvider.Setup(_nationsReadDatabase.LoadedItems);

                booksReadProvider = new BooksReadProvider();
                booksReadProvider.Setup(_booksReadDatabase.LoadedItems, geographyProvider);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets if there is a valid new book read based on the request.
        /// </summary>
        /// <param name="addRequest">The request to add a book read.</param>
        /// <param name="newBook">The new book  to add on exit.</param>
        /// <returns>True if a valid new book, false otherwise.</returns>
        private bool GetBookRead(Book addRequest, out BookRead newBook)
        {
            newBook = new BookRead
            {
                Author = addRequest.Author,
                Title = addRequest.Title,
                Pages = (ushort)addRequest.Pages,
                Note = addRequest.Note,
                Nationality = addRequest.Nationality,
                OriginalLanguage = addRequest.OriginalLanguage,
                ImageUrl = addRequest.ImageUrl,
                Tags = addRequest.Tags.ToList()
            };

            // Check the required strings are ok.
            if (string.IsNullOrWhiteSpace(newBook.Author)
                || string.IsNullOrWhiteSpace(newBook.Title)
                || string.IsNullOrWhiteSpace(newBook.Nationality)
                || string.IsNullOrWhiteSpace(newBook.OriginalLanguage))
            {
                return false;
            }

            // Check the format is valid
            switch (addRequest.Format)
            {
                case "Book":
                    newBook.Format = BookFormat.Book;
                    break;
                case "Comic":
                    newBook.Format = BookFormat.Comic;
                    break;
                case "Audio":
                    newBook.Format = BookFormat.Audio;
                    break;
                default:
                    return false;
            }

            // Check the date
            if (DateTime.Now < addRequest.Date || addRequest.Date < EarliestDate)
            {
                return false;
            }

            // Set the date string 
            newBook.Date = addRequest.Date;
            newBook.DateString =
                SuffixedDaysOfMonths[newBook.Date.Day] + newBook.Date.ToString(" MMMM yyyy");

            return true;
        }

        /// <summary>
        /// Gets if there is a valid new book read based on the request.
        /// </summary>
        /// <param name="addRequest">The request to add a book read.</param>
        /// <param name="newBook">The new book  to add on exit.</param>
        /// <returns>True if a valid new book, false otherwise.</returns>
        private bool GetBookRead(BookReadAddRequest addRequest, out BookRead newBook)
        {
            newBook = new BookRead
            {
                Author = addRequest.Author,
                Title = addRequest.Title,
                Pages = addRequest.Pages,
                Note = addRequest.Note,
                Nationality = addRequest.Nationality,
                OriginalLanguage = addRequest.OriginalLanguage,
                ImageUrl = addRequest.ImageUrl,
                Tags = addRequest.Tags.ToList()
            };

            // Check the required strings are ok.
            if (string.IsNullOrWhiteSpace(newBook.Author)
                || string.IsNullOrWhiteSpace(newBook.Title)
                || string.IsNullOrWhiteSpace(newBook.Nationality)
                || string.IsNullOrWhiteSpace(newBook.OriginalLanguage))
            {
                return false;
            }

            // Check the format is valid
            switch (addRequest.Format)
            {
                case "Book":
                    newBook.Format = BookFormat.Book;
                    break;
                case "Comic":
                    newBook.Format = BookFormat.Comic;
                    break;
                case "Audio":
                    newBook.Format = BookFormat.Audio;
                    break;
                default:
                    return false;
            }

            // Check the date
            if (DateTime.Now < addRequest.Date || addRequest.Date < EarliestDate)
            {
                return false;
            }

            // Set the date string 
            newBook.Date = addRequest.Date;
            newBook.DateString = 
                SuffixedDaysOfMonths[newBook.Date.Day] + newBook.Date.ToString(" MMMM yyyy");

            return true;
        }

        private string GetExportFileName(string exportDirectory, string fileName)
        {
            DateTime time = DateTime.Now;
            fileName += " ";
            fileName += time.ToString("yyyy MMMM dd H-mm-ss");
            fileName += ".csv";
            return Path.Combine(exportDirectory, fileName);
        }

        public void WriteBooksToFile(string filename, List<BookRead> booksRead)
        {
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                // write the header
                sw.WriteLine(
                    "Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio,Image,Tags"
                );

                // write the records
                var csv = new CsvWriter(sw);
                foreach (var book in booksRead)
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
        }

        public void WriteBooksToFile(string filename, List<BookRead> booksRead, out string formattedText)
        {
            BooksExporter.ExportToCsvFile(booksRead, out formattedText);

            WriteBooksToFile(filename, booksRead);
        }

        #endregion

        #region HTTP Handlers

        [HttpGet("[action]")]
        public IEnumerable<Book> GetAllBooks()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            _books = new ObservableCollection<Book>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (var bookRead in booksReadProvider.BooksRead)
                {
                    _books.Add(new Book(bookRead));
                }
            }

            return _books;
        }

        [HttpGet("[action]")]
        public IEnumerable<Author> GetAllAuthors()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            _authors = new ObservableCollection<Author>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (var authorRead in booksReadProvider.AuthorsRead)
                {
                    _authors.Add(new Author(authorRead));
                }
            }

            return _authors;
        }

        [HttpGet("[action]")]
        public IEnumerable<LanguageAuthors> GetAllLanguageAuthors()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            var languageAuthors = new ObservableCollection<LanguageAuthors>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (AuthorLanguage authorLanguage in booksReadProvider.AuthorLanguages)
                {
                    languageAuthors.Add(new LanguageAuthors(authorLanguage));
                }
            }

            return languageAuthors;
        }

        [HttpGet("[action]")]
        public IEnumerable<CountryAuthors> GetAllCountryAuthors()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            var countryAuthors = new ObservableCollection<CountryAuthors>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (AuthorCountry authorCountry in booksReadProvider.AuthorCountries)
                {
                    countryAuthors.Add(new CountryAuthors(authorCountry));
                }
            }

            return countryAuthors;
        }

        [HttpGet("[action]")]
        public IEnumerable<BookTally> GetAllBookTallies()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            var bookTallies = new ObservableCollection<BookTally>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (TalliedBook talliedBook in booksReadProvider.TalliedBooks)
                {
                    bookTallies.Add(new BookTally(talliedBook));
                }
            }

            return bookTallies;
        }

        [HttpGet("[action]")]
        public IEnumerable<MonthlyTally> GetAllMonthlyTallies()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            var monthlyTallies = new ObservableCollection<MonthlyTally>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (TalliedMonth talliedMonth in booksReadProvider.TalliedMonths)
                {
                    monthlyTallies.Add(new MonthlyTally(talliedMonth));
                }
            }

            return monthlyTallies;
        }

        [HttpGet("[action]")]
        public IEnumerable<TagBooks> GetAllTagBooks()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            var tagBooks = new ObservableCollection<TagBooks>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (BookTag bookTag in booksReadProvider.BookTags)
                {
                    tagBooks.Add(new TagBooks(bookTag));
                }
            }

            return tagBooks;
        }

        [HttpGet("[action]")]
        public IEnumerable<DeltaBooks> GetAllBooksDeltas()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            var booksDeltas = new ObservableCollection<DeltaBooks>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (BooksDelta bookDeltas in booksReadProvider.BookDeltas)
                {
                    booksDeltas.Add(new DeltaBooks(bookDeltas));
                }
            }

            return booksDeltas;
        }

        [HttpGet("[action]")]
        public IEnumerable<YearlyTally> GetAllYearlyTallies()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            List<YearlyTally> yearlyTallies = new List<YearlyTally>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                yearlyTallies = 
                    YearlyTally.GetYearlyTalliesForBooks(booksReadProvider.BooksRead.ToList());
            }

            return yearlyTallies;
        }

        [HttpGet("[action]/{userId}")]
        [ProducesResponseType(201, Type = typeof(ExportText))]
        public ExportText GetExportCsvText(string userId)
        {
            ExportText exportText = new ExportText { Format = "text/plain" };

            User foundUser = _userDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == userId);
            if (foundUser == null)
            {
                return exportText;
            }

            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            _books = new ObservableCollection<Book>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                if (booksReadProvider.BooksRead != null && booksReadProvider.BooksRead.Any())
                {
                    List<BookRead> books =
                        booksReadProvider.BooksRead.Where(x => x.User == foundUser.Name).OrderBy(x => x.Date).ToList();

                    // Get the file export string 
                    string formattedText;

                    BooksExporter.ExportToCsvFile(books, out formattedText);


                    // Return the formatted text
                    exportText.FormattedText = formattedText;
                }
            }

            return exportText;
        }

        [HttpGet("[action]/{userId}")]
        [ProducesResponseType(201, Type = typeof(ExportText))]
        [ProducesResponseType(404)]
        public IActionResult GetExportCsvFile(string userId)
        {
            User foundUser = _userDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == userId);
            if (foundUser == null)
            {
                return NotFound();
            }

            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            _books = new ObservableCollection<Book>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                if (booksReadProvider.BooksRead != null && booksReadProvider.BooksRead.Any())
                {
                    List<BookRead> books =
                        booksReadProvider.BooksRead.Where(x => x.User == foundUser.Name).OrderBy(x => x.Date).ToList();

                    // Get the file export string 
                    string formattedText;
                    string fileName = GetExportFileName(ExportDirectory, "Books");
                    WriteBooksToFile(fileName, books, out formattedText);


                    // Return the formatted text
                    if (System.IO.File.Exists(fileName))
                        return new FileStreamResult(new FileStream(fileName, FileMode.Open), "text/csv");
                }
            }

            return NotFound();
        }

        [HttpGet("[action]")]
        [ProducesResponseType(200, Type = typeof(EditorDetails))]
        [ProducesResponseType(404)]
        public ActionResult<EditorDetails> GetEditorDetails()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            EditorDetails editorDetails = new EditorDetails();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                editorDetails.AuthorNames =
                    booksReadProvider.AuthorsRead.Select(x => x.Author).OrderBy(x => x).ToArray();

                editorDetails.CountryNames =
                    geographyProvider.Nations.Select(x => x.Name).OrderBy(x => x).ToArray();

                editorDetails.Languages =
                    booksReadProvider.AuthorLanguages.Select(x => x.Language).OrderBy(x => x).ToArray();

                editorDetails.Tags =
                    booksReadProvider.BookTags.Select(x => x.Tag).OrderBy(x => x).ToArray();
            }

            return editorDetails;
        }

        [HttpGet("[action]/{userId}")]
        public IEnumerable<Book> GetAsDefaultUser(string userId)
        {
            var blankBooks = new List<Book>();

            User foundUser = _userDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == userId);
            if (foundUser == null)
            {
                return blankBooks;
            }

            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            List<BookRead> blankUserBooks = new List<BookRead>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                foreach (var bookRead in booksReadProvider.BooksRead)
                {
                    if (bookRead != null && string.IsNullOrEmpty(bookRead.User))
                    {
                        blankUserBooks.Add(bookRead);
                    }
                }
            }


            if (blankUserBooks.Any())
            {
                foreach (var itemToUpdate in blankUserBooks)
                {
                    itemToUpdate.User = foundUser.Name;
                    _booksReadDatabase.UpdateDatabaseItem(itemToUpdate);
                    blankBooks.Add(new Book(itemToUpdate));
                }
            }

            return blankBooks;
        }

        /// <summary>
        /// Adds a new user book read.
        /// </summary>
        /// <param name="bookReadAddRequest">The new book read to try to add.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public IActionResult Post([FromBody] BookReadAddRequest bookReadAddRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookReadAddResponse response = 
                new BookReadAddResponse
                {
                    ErrorCode = (int)BookReadAddResponseCode.Success,
                    FailReason = "",
                    UserId = bookReadAddRequest.UserId
                };

            // First check that the user exists
            User userLogin = 
                _userDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == bookReadAddRequest.UserId);

            if (userLogin == null)
            {
                response.ErrorCode = (int) BookReadAddResponseCode.UnknownUser;
                response.FailReason = "Could not find this user.";
                return Ok(response);
            }

            // Check the book data is valid
            BookRead newBook;
            if (!GetBookRead(bookReadAddRequest, out newBook))
            {
                response.ErrorCode = (int)BookReadAddResponseCode.InvalidItem;
                response.FailReason = "Invalid book data please try again.";
                return Ok(response);
            }

            // Check if this is duplicate
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            _books = new ObservableCollection<Book>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                BookRead match = booksReadProvider.BooksRead.FirstOrDefault(
                    x => x.Author == newBook.Author &&
                         x.Title == newBook.Title &&
                         x.DateString == newBook.DateString);

                if (match != null)
                {
                    response.ErrorCode = (int)BookReadAddResponseCode.Duplicate;
                    response.FailReason = "This book has already been added.";
                    return Ok(response);
                }
            }

            newBook.User = userLogin.Name;
            _booksReadDatabase.AddNewItemToDatabase(newBook);
            response.NewItem = new Book(newBook);

            return Ok(response);
        }

        [HttpPut]
        public IActionResult UpdateAlbum([FromBody] Book existingBook)
        {
            // set up the successful response
            BookReadAddResponse response = new BookReadAddResponse
            {
                ErrorCode = (int)UserResponseCode.Success,
                NewItem = new Book(existingBook),
                FailReason = "",
                UserId = ""
            };

            // First check that the user exists
            User userLogin =
                _userDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == existingBook.User);

            if (userLogin == null)
            {
                response.ErrorCode = (int)BookReadAddResponseCode.UnknownUser;
                response.FailReason = "Could not find this user.";
                return Ok(response);
            }

            // Check the book data is valid
            BookRead newBook;
            if (!GetBookRead(existingBook, out newBook))
            {
                response.ErrorCode = (int)BookReadAddResponseCode.InvalidItem;
                response.FailReason = "Invalid book data please try again.";
                return Ok(response);
            }

            // Find the item
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            _books = new ObservableCollection<Book>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                var itemToUpdate =
                    _booksReadDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == existingBook.Id);

                if (itemToUpdate == null)
                {
                    response.ErrorCode = (int)BookReadAddResponseCode.UnknownItem;
                    response.FailReason = "Could not find item";
                }
                else
                {
                    itemToUpdate.DateString = newBook.DateString;
                    itemToUpdate.Date = newBook.Date;
                    itemToUpdate.Author = newBook.Author;
                    itemToUpdate.Title = newBook.Title;
                    itemToUpdate.Pages = newBook.Pages;
                    itemToUpdate.Note = newBook.Note;
                    itemToUpdate.Nationality = newBook.Nationality;
                    itemToUpdate.OriginalLanguage = newBook.OriginalLanguage;
                    itemToUpdate.ImageUrl = newBook.ImageUrl;
                    itemToUpdate.Tags = newBook.Tags.ToList();
                    itemToUpdate.Format = newBook.Format;

                    _booksReadDatabase.UpdateDatabaseItem(itemToUpdate);
                }
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            // set up the successful response
            BookReadAddResponse response = new BookReadAddResponse
            {
                ErrorCode = (int)BookReadAddResponseCode.Success,
                NewItem = new Book(),
                FailReason = "",
                UserId = ""
            };

            // Find the item
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            _books = new ObservableCollection<Book>();

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                var itemToDelete =
                    _booksReadDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == id);

                if (itemToDelete == null)
                {
                    response.ErrorCode = (int)BookReadAddResponseCode.UnknownItem;
                    response.FailReason = "Could not find item";
                }
                else
                {
                    _booksReadDatabase.RemoveItemFromDatabase(itemToDelete);
                }
            }

            return Ok(response);
        }

        #endregion

        #region Constructor

        public BooksDataController(IOptions<MongoDbSettings> config)
        {
            MongoDbSettings dbSettings = config.Value;

            DatabaseConnectionString = dbSettings.DatabaseConnectionString;
            ExportDirectory = dbSettings.ExportDirectory;

            _books = new ObservableCollection<Book>();
            
            _booksReadFromDatabase = new ObservableCollection<BookRead>();
            _nationsReadFromDatabase = new ObservableCollection<Nation>();
            _usersReadFromDatabase = new ObservableCollection<User>();

            _booksReadDatabase = new BooksReadDatabase(DatabaseConnectionString);
            _nationsReadDatabase = new NationDatabase(DatabaseConnectionString);
            _userDatabase = new UserDatabase(DatabaseConnectionString);
        }

        #endregion
    }
}
