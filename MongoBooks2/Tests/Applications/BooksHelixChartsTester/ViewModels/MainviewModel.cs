// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksHelixChartsTester.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Provider;
    using BooksDatabase.Implementations;
    using BooksHelixCharts.ViewModels;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The viewvmodel for a books helix chart test application.
    /// </summary>
    public class MainViewModel : BaseViewModel
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
        /// The books read from database.
        /// </summary>
        private ObservableCollection<BookRead> _booksReadFromDatabase;

        /// <summary>
        /// The nations read from database.
        /// </summary>
        private ObservableCollection<Nation> _nationsReadFromDatabase;

        /// <summary>
        /// The pages by country diagram view model.
        /// </summary>
        private PagesByCountryDiagramViewModel _pagesByCountryDiagramViewModel;

        /// <summary>
        /// The pages read by country diagram view model.
        /// </summary>
        private PagesReadByCountryDiagramViewModel _pagesReadByCountryDiagramViewModel;

        /// <summary>
        /// The books read by country diagram view model.
        /// </summary>
        private BooksReadByCountryViewModel _booksReadByCountryDiagramViewModel;

        /// <summary>
        /// The get books command.
        /// </summary>
        private ICommand _getBooksCommand;

        /// <summary>
        /// The get nations command.
        /// </summary>
        private ICommand _getNationsCommand;

        /// <summary>
        /// The update pages by country diagram command.
        /// </summary>
        private ICommand _updatePagesByCountryCommand;

        /// <summary>
        /// The update pages read by country diagram command.
        /// </summary>
        private ICommand _updatePagesReadByCountryCommand;

        /// <summary>
        /// The update books read by country diagram command.
        /// </summary>
        private ICommand _updateBooksReadByCountryCommand;

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
        /// Gets the pages by country diagram view model.
        /// </summary>
        public PagesByCountryDiagramViewModel PagesByCountry => _pagesByCountryDiagramViewModel;

        /// <summary>
        /// Gets the pages by country diagram view model.
        /// </summary>
        public PagesReadByCountryDiagramViewModel PagesReadByCountry => _pagesReadByCountryDiagramViewModel;

        /// <summary>
        /// Gets the books read by country diagram view model.
        /// </summary>
        public BooksReadByCountryViewModel BooksReadByCountry => _booksReadByCountryDiagramViewModel;

        /// <summary>
        /// Gets the get books from database command.
        /// </summary>
        public ICommand GetBooksCommand => _getBooksCommand ?? (_getBooksCommand = new CommandHandler(GetBooksCommandAction, true));

        /// <summary>
        /// Gets the get nations from database command.
        /// </summary>
        public ICommand GetNationsCommand => _getNationsCommand ?? (_getNationsCommand = new CommandHandler(GetNationsCommandAction, true));

        /// <summary>
        /// Gets the update pages by country diagram command.
        /// </summary>
        public ICommand UpdateBooksReadByCountryCommand => _updateBooksReadByCountryCommand ?? (_updateBooksReadByCountryCommand = new CommandHandler(UpdateBooksReadByCountryCommandAction, true));

        /// <summary>
        /// Gets the update pages by country diagram command.
        /// </summary>
        public ICommand UpdatePagesByCountryCommand => _updatePagesByCountryCommand ?? (_updatePagesByCountryCommand = new CommandHandler(UpdatePagesByCountryCommandAction, true));

        /// <summary>
        /// Gets the update pages read by country diagram command.
        /// </summary>
        public ICommand UpdatePagesReadByCountryCommand => _updatePagesReadByCountryCommand ?? (_updatePagesReadByCountryCommand = new CommandHandler(UpdatePagesReadByCountryCommandAction, true));

        #endregion

        #region Command handlers

        /// <summary>
        /// The get books from database command action.
        /// </summary>
        private void GetBooksCommandAction()
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
        private void GetNationsCommandAction()
        {
            _nationsReadDatabase.ConnectToDatabase();
            if (_nationsReadDatabase.ReadFromDatabase)
            {
                _nationsReadFromDatabase = _nationsReadDatabase.LoadedItems;
                OnPropertyChanged(() => Nations);
            }
        }

        /// <summary>
        /// The update pages by country diagram command action.
        /// </summary>
        private void UpdatePagesByCountryCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {

                _pagesByCountryDiagramViewModel = new PagesByCountryDiagramViewModel(geographyProvider, booksReadProvider);
                _pagesByCountryDiagramViewModel.SetupModel();

                OnPropertyChanged(() => PagesByCountry);
            }
        }

        /// <summary>
        /// The update pages by country diagram command action.
        /// </summary>
        private void UpdatePagesReadByCountryCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {

                _pagesReadByCountryDiagramViewModel = new PagesReadByCountryDiagramViewModel(geographyProvider, booksReadProvider);
                _pagesReadByCountryDiagramViewModel.SetupModel();

                OnPropertyChanged(() => PagesReadByCountry);
            }
        }
        private bool GetProviders(out GeographyProvider geographyProvider, out BooksReadProvider booksReadProvider)
        {
            geographyProvider = null;
            booksReadProvider = null;

            if (!_booksReadDatabase.ReadFromDatabase)
                _booksReadDatabase.ConnectToDatabase();

            if (!_nationsReadDatabase.ReadFromDatabase)
                _nationsReadDatabase.ConnectToDatabase();

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
        /// The update books read by country diagram command action.
        /// </summary>
        private void UpdateBooksReadByCountryCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _booksReadByCountryDiagramViewModel = new BooksReadByCountryViewModel(geographyProvider, booksReadProvider);
                _booksReadByCountryDiagramViewModel.SetupModel();

                OnPropertyChanged(() => BooksReadByCountry);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class. 
        /// </summary>
        public MainViewModel()
        {
            _booksReadFromDatabase = new ObservableCollection<BookRead>();
            _nationsReadFromDatabase = new ObservableCollection<Nation>();

            _booksReadDatabase = new BooksReadDatabase(DatabaseConnectionString);
            _nationsReadDatabase = new NationDatabase(DatabaseConnectionString);
        }

        #endregion
    }
}
