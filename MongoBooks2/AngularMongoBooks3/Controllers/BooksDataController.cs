namespace AngularMongoBooks3.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
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

    [Route("api/[controller]")]
    public class BooksDataController : Controller
    {
        #region Constants

        public const string DefaultDatabaseConnectionString = "mongodb://localhost:27017";

        /// <summary>
        /// The connection string for the database.
        /// </summary>
        public readonly string DatabaseConnectionString;

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

        #endregion

        #region Constructor

        public BooksDataController(IOptions<MongoDbSettings> config)
        {
            MongoDbSettings dbSettings = config.Value;

            DatabaseConnectionString = dbSettings.DatabaseConnectionString;

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
