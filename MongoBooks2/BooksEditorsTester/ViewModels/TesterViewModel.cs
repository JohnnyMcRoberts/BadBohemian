// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TesterViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for the books editors tester application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditorsTester.ViewModels
{
    using System;
    using System.IO;
    using System.Windows.Input;
    using System.Xml.Serialization;
    using BooksCore.Books;
    using BooksCore.Provider;
    using BooksEditors.ViewModels.Grids;
    using BooksEditors.ViewModels.Editors;
    using BooksEditors.Views.Editors;
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
        /// The book editor.
        /// </summary>
        private BookEditorViewModel _bookEditor;

        /// <summary>
        /// The image selection window.
        /// </summary>
        private ImageSelectionWindowViewModel _imageSelectionWindow;

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

        /// <summary>
        /// The set editor to add command.
        /// </summary>
        private ICommand _setEditorToAddCommand;

        /// <summary>
        /// The set editor to update command.
        /// </summary>
        private ICommand _setEditorToUpdateCommand;

        /// <summary>
        /// The select image command.
        /// </summary>
        private ICommand _selectImageCommand;

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
        /// Gets the book editor.
        /// </summary>
        public BookEditorViewModel BookEditor => _bookEditor;

        /// <summary>
        /// Gets the image selection window.
        /// </summary>
        public ImageSelectionWindowViewModel ImageSelectionWindow => _imageSelectionWindow;

        /// <summary>
        /// Gets or sets the initial search terms for the window.
        /// </summary>
        public string InitialImageSearchTerm { get; set; }

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

        /// <summary>
        /// Gets the set editor to add command.
        /// </summary>
        public ICommand SetEditorToAddCommand =>
            _setEditorToAddCommand ?? (_setEditorToAddCommand = new CommandHandler(SetEditorToAddCommandAction, true));

        /// <summary>
        /// Gets the set editor to update command.
        /// </summary>
        public ICommand SetEditorToUpdateCommand =>
            _setEditorToUpdateCommand ?? (_setEditorToUpdateCommand = new CommandHandler(SetEditorToUpdateCommandAction, true));

        /// <summary>
        /// Gets the set editor to update command.
        /// </summary>
        public ICommand SelectImageCommand =>
            _selectImageCommand ?? (_selectImageCommand = new CommandHandler(SelectImageCommandAction, true));

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
                _bookEditor.SetupEditor(geographyProvider, booksReadProvider);
                OnPropertyChanged(() => BooksReadGrid);
                OnPropertyChanged(() => BookEditor);
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

        /// <summary>
        /// The set editor to add command action.
        /// </summary>
        public void SetEditorToAddCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _bookEditor.SetupEditor(geographyProvider, booksReadProvider);
                _bookEditor.UpdateBookCommandText = "Add new book";
                _bookEditor.SelectedBook = new BookRead { Date = DateTime.Now.AddDays(-2) };
                _bookEditor.UpdateBookAction = book =>
                {
                    StringWriter stringwriter = new StringWriter();
                    XmlSerializer serializer = new XmlSerializer(typeof(BookRead));
                    serializer.Serialize(stringwriter, book);
                    string text = $"Add new book ({stringwriter.ToString()})" ;
                    Console.WriteLine(text);
                };

                OnPropertyChanged(() => BookEditor);
            }
        }

        /// <summary>
        /// The set editor to update command action.
        /// </summary>
        public void SetEditorToUpdateCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _bookEditor.SetupEditor(geographyProvider, booksReadProvider);
                _bookEditor.UpdateBookCommandText = "Update existing book";
                Random rand = new Random((int) DateTime.Now.Ticks);
                int randIndex = rand.Next(_bookEditor.BooksRead.Count);
                _bookEditor.SelectedBook = _bookEditor.BooksRead[randIndex];
                _bookEditor.UpdateBookAction = book =>
                {
                    StringWriter stringwriter = new StringWriter();
                    XmlSerializer serializer = new XmlSerializer(typeof(BookRead));
                    serializer.Serialize(stringwriter, book);
                    string text = $"Edit existing book ({stringwriter.ToString()})";
                    Console.WriteLine(text);
                };

                OnPropertyChanged(() => BookEditor);
            }
        }

        /// <summary>
        /// The set editor to update command action.
        /// </summary>
        public void SelectImageCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _imageSelectionWindow = new ImageSelectionWindowViewModel {SearchTerm = InitialImageSearchTerm};

                ImageSelectionWindowView imageSelectDialog = new ImageSelectionWindowView(_imageSelectionWindow);
                imageSelectDialog.ShowDialog();
                if (_imageSelectionWindow.DialogResult.HasValue && _imageSelectionWindow.DialogResult.Value)
                {
                    // update the selected image....
                }


                OnPropertyChanged(() => BookEditor);
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
            _bookEditor = new BookEditorViewModel();
            InitialImageSearchTerm = "Amazon Ann Quin Unmapped Country";
        }

        #endregion
    }
}
