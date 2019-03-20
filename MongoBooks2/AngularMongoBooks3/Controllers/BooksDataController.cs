namespace AngularMongoBooks3.Controllers
{
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
    using AngularMongoBooks3.Controllers.Settings;

    [Route("api/[controller]")]
    public class BooksDataController : Controller
    {
        #region Constants

        /// <summary>
        /// The connection string for the database.
        /// </summary>
        public const string DatabaseConnectionString = "mongodb://localhost:27017";

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
        private readonly UserDatabase _usersReadDatabase;

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

        #endregion


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

        public BooksDataController(IOptions<MongoDbSettings> config)
        {
            MongoDbSettings dbSettings = config.Value;

            _books = new ObservableCollection<Book>();
            
            _booksReadFromDatabase = new ObservableCollection<BookRead>();
            _nationsReadFromDatabase = new ObservableCollection<Nation>();
            _usersReadFromDatabase = new ObservableCollection<User>();

            _booksReadDatabase = new BooksReadDatabase(DatabaseConnectionString);
            _nationsReadDatabase = new NationDatabase(DatabaseConnectionString);
            _usersReadDatabase = new UserDatabase(DatabaseConnectionString);
        }
    }
}
