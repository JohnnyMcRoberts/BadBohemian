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
    using System.Windows.Input;
    
    using BooksCore.Provider;
    using BooksEditors.ViewModels.Grids;
    using BooksTesterUtilities.ViewModels;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The view model for the books editors tester application.
    /// </summary>
    public class TesterViewModel : BaseTesterViewModel
    {
        #region Private data

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

        #region Command handlers

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
            _booksReadGrid = new BooksReadGridViewModel();
            _authorsGrid = new AuthorsGridViewModel();
            _languagesGrid = new LanguagesGridViewModel();
            _countriesGrid = new CountriesGridViewModel();
        }

        #endregion
    }
}
