// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for the books editors tester application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditorsTester.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Provider;
    using BooksCore.Users;
    using BooksDatabase.Implementations;
    using BooksEditors.ViewModels.Grids;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The view model for the books editors tester application.
    /// </summary>
    public class TesterViewModel : BaseViewModel
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
        /// The books read grid.
        /// </summary>
        private BooksReadGridViewModel _booksReadGrid;

        /// <summary>
        /// The authors grid.
        /// </summary>
        private AuthorsGridViewModel _authorsGrid;

        /// <summary>
        /// The languages grid.
        /// </summary>
        private LanguagesGridViewModel _languagesGrid;

        /// <summary>
        /// The countries grid.
        /// </summary>
        private CountriesGridViewModel _countriesGrid;

        /// <summary>
        /// The get books command.
        /// </summary>
        private ICommand _getBooksCommand;

        /// <summary>
        /// The get nations command.
        /// </summary>
        private ICommand _getNationsCommand;

        /// <summary>
        /// The get users command.
        /// </summary>
        private ICommand _getUsersCommand;

        /// <summary>
        /// The get books read grid command.
        /// </summary>
        private ICommand _getBooksReadGridCommand;

        /// <summary>
        /// The get authors grid command.
        /// </summary>
        private ICommand _getAuthorsGridCommand;

        /// <summary>
        /// The get languages grid command.
        /// </summary>
        private ICommand _getLanguagesGridCommand;

        /// <summary>
        /// The get countries grid command.
        /// </summary>
        private ICommand _getCountriesGridCommand;

        #endregion

        #region Public data

        /// <summary>
        /// Gets the books.
        /// </summary>
        public ObservableCollection<BookRead> Books => _booksReadFromDatabase;

        /// <summary>
        /// Gets the books.
        /// </summary>
        public ObservableCollection<Nation> Nations => _nationsReadFromDatabase;

        /// <summary>
        /// Gets the books.
        /// </summary>
        public ObservableCollection<User> Users => _usersReadFromDatabase;

        /// <summary>
        /// Gets the books read grid.
        /// </summary>
        public BooksReadGridViewModel BooksReadGrid => _booksReadGrid;

        /// <summary>
        /// Gets the authors grid.
        /// </summary>
        public AuthorsGridViewModel AuthorsGrid => _authorsGrid;

        /// <summary>
        /// Gets the languages grid.
        /// </summary>
        public LanguagesGridViewModel LanguagesGrid => _languagesGrid;

        /// <summary>
        /// Gets the countries grid.
        /// </summary>
        public CountriesGridViewModel CountriesGrid => _countriesGrid;

        /// <summary>
        /// Gets the get books from database command.
        /// </summary>
        public ICommand GetBooksCommand => _getBooksCommand ?? (_getBooksCommand = new CommandHandler(GetBooksCommandAction, true));

        /// <summary>
        /// Gets the get nations from database command.
        /// </summary>
        public ICommand GetNationsCommand => _getNationsCommand ?? (_getNationsCommand = new CommandHandler(GetNationsCommandAction, true));

        /// <summary>
        /// Gets the get users from database command.
        /// </summary>
        public ICommand GetUsersCommand => _getUsersCommand ?? (_getUsersCommand = new CommandHandler(GetUsersCommandAction, true));

        /// <summary>
        /// Gets the get books read grid command.
        /// </summary>
        public ICommand GetBooksReadGridCommand =>
            _getBooksReadGridCommand ?? (_getBooksReadGridCommand = new CommandHandler(GetBooksReadGridCommandAction, true));

        /// <summary>
        /// Gets the get authors grid command.
        /// </summary>
        public ICommand GetAuthorsGridCommand =>
            _getAuthorsGridCommand ?? (_getAuthorsGridCommand = new CommandHandler(GetAuthorsGridCommandAction, true));

        /// <summary>
        /// Gets the get languages grid command.
        /// </summary>
        public ICommand GetLanguagesGridCommand =>
            _getLanguagesGridCommand ?? (_getLanguagesGridCommand = new CommandHandler(GetLanguagesGridCommandAction, true));

        /// <summary>
        /// Gets the get countries grid command.
        /// </summary>
        public ICommand GetCountriesGridCommand =>
            _getCountriesGridCommand ?? (_getCountriesGridCommand = new CommandHandler(GetCountriesGridCommandAction, true));

        #endregion

        #region Utility Functions

        /// <summary>
        /// Gets the providers for the books and geography data.
        /// </summary>
        /// <param name="geographyProvider">The geography data provider on exit.</param>
        /// <param name="booksReadProvider">The books data provider on exit.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool GetProviders(out GeographyProvider geographyProvider, out BooksReadProvider booksReadProvider)
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

        #region Command handlers

        /// <summary>
        /// The get books from database command action.
        /// </summary>
        public void GetBooksCommandAction()
        {
            _booksReadDatabase.ConnectToDatabase();
            if (_booksReadDatabase.ReadFromDatabase)
            {
                _booksReadFromDatabase = _booksReadDatabase.LoadedItems;
                OnPropertyChanged(() => Books);
            }
        }

        /// <summary>
        /// The get nations from database command action.
        /// </summary>
        public void GetNationsCommandAction()
        {
            _nationsReadDatabase.ConnectToDatabase();
            if (_nationsReadDatabase.ReadFromDatabase)
            {
                _nationsReadFromDatabase = _nationsReadDatabase.LoadedItems;
                OnPropertyChanged(() => Nations);
            }
        }

        /// <summary>
        /// The get users from database command action.
        /// </summary>
        public void GetUsersCommandAction()
        {
            _usersReadDatabase.ConnectToDatabase();
            if (_usersReadDatabase.ReadFromDatabase)
            {
                _usersReadFromDatabase = _usersReadDatabase.LoadedItems;
                OnPropertyChanged(() => Users);
            }
        }

        /// <summary>
        /// The get books read command action.
        /// </summary>
        public void GetBooksReadGridCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _booksReadGrid.SetupGrid(geographyProvider, booksReadProvider);
                OnPropertyChanged(() => BooksReadGrid);
            }
        }

        /// <summary>
        /// The get authors command action.
        /// </summary>
        public void GetAuthorsGridCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _authorsGrid.SetupGrid(geographyProvider, booksReadProvider);
                OnPropertyChanged(() => AuthorsGrid);
            }
        }

        /// <summary>
        /// The get languages command action.
        /// </summary>
        public void GetLanguagesGridCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _languagesGrid.SetupGrid(geographyProvider, booksReadProvider);
                OnPropertyChanged(() => LanguagesGrid);
            }
        }

        /// <summary>
        /// The get countries command action.
        /// </summary>
        public void GetCountriesGridCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _countriesGrid.SetupGrid(geographyProvider, booksReadProvider);
                OnPropertyChanged(() => CountriesGrid);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TesterViewModel"/> class. 
        /// </summary>
        public TesterViewModel()
        {
            _booksReadFromDatabase = new ObservableCollection<BookRead>();
            _nationsReadFromDatabase = new ObservableCollection<Nation>();
            _usersReadFromDatabase = new ObservableCollection<User>();

            _booksReadDatabase = new BooksReadDatabase(DatabaseConnectionString);
            _nationsReadDatabase = new NationDatabase(DatabaseConnectionString);
            _usersReadDatabase = new UserDatabase(DatabaseConnectionString);

            _booksReadGrid = new BooksReadGridViewModel();
            _authorsGrid = new AuthorsGridViewModel();
            _languagesGrid = new LanguagesGridViewModel();
            _countriesGrid = new CountriesGridViewModel();
        }

        #endregion
    }
}
