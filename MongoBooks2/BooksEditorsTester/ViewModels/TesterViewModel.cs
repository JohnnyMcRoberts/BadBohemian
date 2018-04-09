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
    using System;
    using System.Collections.Generic;
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
        /// The users read from database.
        /// </summary>
        private AuthorsGridViewModel _authorsGrid;

        /// <summary>
        /// The get authors grid command.
        /// </summary>
        private ICommand _getAuthorsGridCommand;

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
        /// Gets the authors grid.
        /// </summary>
        public AuthorsGridViewModel AuthorsGrid => _authorsGrid;

        /// <summary>
        /// Gets the get authors grid command.
        /// </summary>
        public ICommand GetAuthorsGridCommand => _getAuthorsGridCommand ?? (_getAuthorsGridCommand = new CommandHandler(GetAuthorsGridCommandAction, true));


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
        /// The get users from database command action.
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

            _authorsGrid = new AuthorsGridViewModel();
        }

        #endregion
    }
}
