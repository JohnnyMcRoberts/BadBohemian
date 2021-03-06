﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataUpdaterViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The data update view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.Windows.Input;
    using System.Windows.Forms;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.Views;

    public class DataUpdaterViewModel : BaseViewModel
    {
        #region Private Data

        private MainWindow _mainWindow;
        private log4net.ILog _log;
        private MainBooksModel _mainModel;
        private MainViewModel _parent;

        private ICommand _submitNewBookCommand;
        private ICommand _updateExistingBookCommand;
        private ICommand _updateAllBooksCommand;

        private ICommand _addExistingBookTagCommand;
        private ICommand _clearExistingBookTagsCommand;

        /// <summary>
        /// The select image for nation command.
        /// </summary>
        private ICommand _selectImageForBookCommand;

        private BookRead _newBook;
        private BookRead _existingBook;

        private readonly Dictionary<BookFormat, string> _bookFormats = new Dictionary<BookFormat, string>()
        {
            {BookFormat.Book, "Book"},
            {BookFormat.Comic, "Comic"},
            {BookFormat.Audio, "Audio"}
        };

        #endregion

        #region Constructor

        public DataUpdaterViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;

            InitialiseNewBook();
            InitialiseEditBook();
        }

        #endregion

        #region Public Data

        public ObservableCollection<BookRead> BooksRead { get { return _mainModel.BooksRead; } }

        public BookRead NewBook { get { return _newBook; } }

        public bool IsDataLoaded
        {
            get { return _parent.DataLoaderVM.IsDataLoaded; }
        }

        public DateTime NewBookDate
        { get { return _newBook.Date; } set { _newBook.Date = value; OnPropertyChanged(() => NewBookDateText); } }
        public string NewBookAuthor
        { get { return _newBook.Author; } set { _newBook.Author = value; } }
        public string NewBookTitle
        { get { return _newBook.Title; } set { _newBook.Title = value; } }
        public UInt16 NewBookPages
        { get { return _newBook.Pages; } set { _newBook.Pages = value; } }
        public string NewBookNote
        { get { return _newBook.Note; } set { _newBook.Note = value; } }
        public string NewBookNationality
        { get { return _newBook.Nationality; } set { _newBook.Nationality = value; } }
        public string NewBookOriginalLanguage
        { get { return _newBook.OriginalLanguage; } set { _newBook.OriginalLanguage = value; } }
        public BookFormat NewBookFormat
        { get { return _newBook.Format; } set { _newBook.Format = value; OnPropertyChanged(() => NewBookFormat); } }
        public string NewBookDateText
        { get { _newBook.DateString = GetBookDateText(); return _newBook.DateString; } }

        public string NewBookAuthorText { get; set; }
        public string NewBookNationalityText { get; set; }
        public string NewBookOriginalLanguageText { get; set; }

        public List<string> AuthorNames
        {
            get { return (from book in BooksRead orderby book.Author select book.Author).Distinct().ToList(); }
        }
        public List<string> AuthorNationalities
        {
            get { return (from country in _mainModel.WorldCountries orderby country.Country select country.Country).Distinct().ToList(); }
        }
        public List<string> OriginalLanguages
        {
            get { return (from book in BooksRead orderby book.OriginalLanguage select book.OriginalLanguage).Distinct().ToList(); }
        }



        public BookRead ExistingBook
        {
            get { return _existingBook; }
            set { _existingBook = value; UpdateExistingBook(); }
        }

        public DateTime ExistingBookDate
        {
            get { if (_existingBook != null) return _existingBook.Date; return DateTime.Now; }
            set { if (_existingBook != null) _existingBook.Date = value; OnPropertyChanged(() => ExistingBookDateText); }
        }
        public string ExistingBookAuthor
        {
            get { if (_existingBook != null) return _existingBook.Author; return null; }
            set { if (value != null) _existingBook.Author = value; }
        }
        public string ExistingBookTitle
        {
            get { if (_existingBook != null) return _existingBook.Title; return null; }
            set { if (_existingBook != null) _existingBook.Title = value; }
        }
        public UInt16 ExistingBookPages
        {
            get { if (_existingBook != null) return _existingBook.Pages; return 0; }
            set { if (_existingBook != null) _existingBook.Pages = value; }
        }
        public string ExistingBookNote
        {
            get { if (_existingBook != null) return _existingBook.Note; return null; }
            set { if (_existingBook != null) _existingBook.Note = value; }
        }
        public string ExistingBookNationality
        {
            get { if (_existingBook != null) return _existingBook.Nationality; return null; }
            set { _existingBook.Nationality = value; }
        }
        public string ExistingBookOriginalLanguage
        {
            get { if (_existingBook != null) return _existingBook.OriginalLanguage; return null; }
            set { if (_existingBook != null) _existingBook.OriginalLanguage = value; }
        }
        public BookFormat ExistingBookFormat
        {
            get { if (_existingBook != null) return _existingBook.Format; return BookFormat.Book; }
            set { if (_existingBook != null) _existingBook.Format = value; OnPropertyChanged(() => ExistingBookFormat); }
        }

        public string ExistingBookDateText
        { get { if (_existingBook == null) return ""; _existingBook.DateString = GetBookDateText(false); return _existingBook.DateString; } }

        public string ExistingBookAuthorText { get; set; }
        public string ExistingBookNationalityText { get; set; }
        public string ExistingBookOriginalLanguageText { get; set; }
        public Uri ExistingBookImageSource => _existingBook?.DisplayImage;
        public Dictionary<BookFormat, string> BookFormats => _bookFormats;

        private bool _isUpdating;

        public bool IsUpdating
        {
            get { return _isUpdating; }
            set
            {
                if (value != _isUpdating)
                {
                    _isUpdating = value;
                    OnPropertyChanged(() => IsUpdating);
                }
            }
        }

        public List<string> BookTags => _mainModel.BookTags.Select(tag => tag.Tag.Trim()).OrderBy(x => x).ToList();

        public string ExistingBookNewTag { get; set; }

        public string ExistingBookNewTagText { get; set; }

        public List<string> ExistingBookTags
        {
            get { if (_existingBook != null) return _existingBook.Tags; return null; }
            set { if (value != null) _existingBook.Tags = value; }
        }

        /// <summary>
        /// Gets the tags list ready to be displayed.
        /// </summary>
        public string ExistingBookDisplayTags
        {
            get
            {
                if (ExistingBookTags == null || ExistingBookTags.Count == 0)
                    return string.Empty;
                else if (ExistingBookTags.Count == 0)
                    return ExistingBookTags[0];
                else
                {
                    string listOfTags = ExistingBookTags[0];
                    for (int i = 1; i < ExistingBookTags.Count; i++)
                    {
                        listOfTags += ", " + ExistingBookTags[i];
                    }
                    return listOfTags;
                }
            }
        }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            OnPropertyChanged("");
        }

        public static string GetImageSearchTerm(BookRead book)
        {
            string nameAndTitle = book.Author + " " + book.Title + " amazon";
            string[] words = nameAndTitle.Split(' ');

            string term = "http://www.google.co.uk/search?q=" + words[0];

            for (int i = 1; i < words.Length; i++)
            {
                if (words[i] == "&")
                    continue;
                term += "+";
                term += words[i];
            }

            return term;
        }

        #endregion

        #region Commands

        public ICommand SubmitNewBookCommand
        {
            get
            {
                return _submitNewBookCommand ??
                    (_submitNewBookCommand =
                        new CommandHandler(() => SubmitNewBookCommandAction(), true));
            }
        }

        public ICommand UpdateExistingBookCommand
        {
            get
            {
                return _updateExistingBookCommand ??
                    (_updateExistingBookCommand =
                        new CommandHandler(() => UpdateExistingBookCommandAction(), true));
            }
        }

        public ICommand UpdateAllBooksCommand
        {
            get
            {
                return _updateAllBooksCommand ??
                    (_updateAllBooksCommand =
                        new CommandHandler(() => UpdateAllBooksCommandAction(), true));
            }
        }

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand SelectImageForBookCommand => _selectImageForBookCommand ??
                                                     (_selectImageForBookCommand =
                                                         new RelayCommandHandler(SelectImageForBookCommandAction) { IsEnabled = true });

        public ICommand AddExistingBookTagCommand
        {
            get
            {
                return _addExistingBookTagCommand ??
                    (_addExistingBookTagCommand =
                        new CommandHandler(() => AddExistingBookTagCommandAction(), true));
            }
        }

        public ICommand ClearExistingBookTagsCommand
        {
            get
            {
                return _clearExistingBookTagsCommand ??
                    (_clearExistingBookTagsCommand =
                        new CommandHandler(() => ClearExistingBookTagsCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void AddExistingBookTagCommandAction()
        {
            if (!string.IsNullOrEmpty(ExistingBookNewTagText))
            {
                if (!ExistingBookTags.Contains(ExistingBookNewTagText))
                {
                    ExistingBookTags.Add(ExistingBookNewTagText);
                    OnPropertyChanged(() => ExistingBookDisplayTags);
                    ExistingBookNewTagText = string.Empty;
                }
            }
        }

        public void ClearExistingBookTagsCommandAction()
        {
            ExistingBookTags.Clear();
            OnPropertyChanged(() => ExistingBookDisplayTags);
            ExistingBookNewTagText = string.Empty;
        }

        public void SubmitNewBookCommandAction()
        {
            // do basic validation stuff....
            UpdateNewBookWithNewText();

            string errorMsg;
            bool isValidBook = IsBookValid(_newBook, out errorMsg);

            if (!isValidBook)
            {
                MessageBox.Show(errorMsg);
                return;
            }

            // add to the list....
            if (!_mainModel.AddNewBook(_newBook, out errorMsg))
            {
                MessageBox.Show(errorMsg);
            }
            else
            {
                InitialiseNewBook();
                _parent.UpdateData();
                OnPropertyChanged("");
            }
        }

        public void UpdateExistingBookCommandAction()
        {
            // update the book with what has been typed in
            if (!string.IsNullOrEmpty(ExistingBookAuthorText) &&
                (_existingBook.Author == null || _existingBook.Author != ExistingBookAuthorText))
                ExistingBookAuthor = ExistingBookAuthorText;
            if (_existingBook.Nationality == null && !string.IsNullOrEmpty(ExistingBookNationalityText))
                ExistingBookNationality = ExistingBookNationalityText;
            if (_existingBook.OriginalLanguage == null && !string.IsNullOrEmpty(ExistingBookOriginalLanguageText))
                ExistingBookOriginalLanguage = ExistingBookOriginalLanguageText;
            if (_existingBook.DisplayTags != ExistingBookDisplayTags)
                _existingBook.Tags = ExistingBookTags;

            // Update in the DB.
            string errorMsg = string.Empty;
            bool isError = false;

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, ea) =>
            {
                if (!_mainModel.UpdateBook(_existingBook, out errorMsg, false))
                {
                    isError = true;
                }
                else
                {
                    InitialiseEditBook();
                }
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsUpdating = false;
                if (isError)
                {
                    MessageBox.Show(errorMsg);
                }
                _parent.UpdateData();
                _mainModel.UpdateCollections();
                OnPropertyChanged(string.Empty);
                OnPropertyChanged(() => BookTags);
            };

            IsUpdating = true;
            worker.RunWorkerAsync();
        }

        public void UpdateAllBooksCommandAction()
        {
            // update the book with what has been typed in
            if (!string.IsNullOrEmpty(ExistingBookAuthorText) &&
                (_existingBook.Author == null || _existingBook.Author != ExistingBookAuthorText))
                ExistingBookAuthor = ExistingBookAuthorText;
            if (_existingBook.Nationality == null && !string.IsNullOrEmpty(ExistingBookNationalityText))
                ExistingBookNationality = ExistingBookNationalityText;
            if (_existingBook.OriginalLanguage == null && !string.IsNullOrEmpty(ExistingBookOriginalLanguageText))
                ExistingBookOriginalLanguage = ExistingBookOriginalLanguageText;
            if (_existingBook.DisplayTags != ExistingBookDisplayTags)
                _existingBook.Tags = ExistingBookTags;

            // Update in the DB.
            string errorMsg = string.Empty;
            bool isError = false;

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, ea) =>
            {
                if (!_mainModel.UpdateAllBooks(out errorMsg, false))
                {
                    isError = true;
                }
                else
                {
                    InitialiseEditBook();
                }
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsUpdating = false;
                if (isError)
                {
                    MessageBox.Show(errorMsg);
                }
                _parent.UpdateData();
                _mainModel.UpdateCollections();
                OnPropertyChanged(string.Empty);
                OnPropertyChanged(() => BookTags);
            };

            IsUpdating = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void SelectImageForBookCommandAction(object parameter)
        {
            BookRead book = parameter as BookRead;
            if (book != null)
            {
                SelectImageForBook(book);
            }
        }

        #endregion

        #region Utility Functions

        private void UpdateExistingBook()
        {
            OnPropertyChanged(() => ExistingBook);
            OnPropertyChanged(() => ExistingBookDate);
            OnPropertyChanged(() => ExistingBookDate);
            OnPropertyChanged(() => ExistingBookAuthor);
            OnPropertyChanged(() => ExistingBookTitle);
            OnPropertyChanged(() => ExistingBookPages);
            OnPropertyChanged(() => ExistingBookNote);
            OnPropertyChanged(() => ExistingBookNationality);
            OnPropertyChanged(() => ExistingBookOriginalLanguage);
            OnPropertyChanged(() => ExistingBookFormat);
            OnPropertyChanged(() => ExistingBookDateText);
            OnPropertyChanged(() => ExistingBookAuthorText);
            OnPropertyChanged(() => ExistingBookNationalityText);
            OnPropertyChanged(() => ExistingBookOriginalLanguageText);
            OnPropertyChanged(() => ExistingBookImageSource);
            OnPropertyChanged(() => ExistingBookTags);
            OnPropertyChanged(() => ExistingBookDisplayTags);
            OnPropertyChanged(() => ExistingBookNewTag);
            OnPropertyChanged(() => ExistingBookNewTagText);
        }

        private string GetBookDateText(bool isNew = true)
        {
            DateTime date = NewBookDate;
            if (!isNew)
            {
                date = ExistingBookDate;
            }

            string datetext = date.ToString("y");
            datetext = datetext.Replace(", ", " ");
            int day = date.Day;
            string dayString = day.ToString() + "th ";
            switch (day)
            {
                case 1: dayString = "1st "; break;
                case 21: dayString = "21st "; break;
                case 31: dayString = "31st "; break;
                case 2: dayString = "2nd "; break;
                case 22: dayString = "22nd "; break;
                case 3: dayString = "3rd "; break;
                case 23: dayString = "23rd "; break;
            }

            datetext = dayString + datetext;

            return datetext;

        }

        private bool IsBookValid(BookRead newBook, out string errorMsg)
        {
            errorMsg = "";
            if (string.IsNullOrEmpty(newBook.Author))
            {
                errorMsg = "Must provide an Author";
                return false;
            }
            if (string.IsNullOrEmpty(newBook.Title))
            {
                errorMsg = "Must provide the Books's Title";
                return false;
            }
            if (string.IsNullOrEmpty(newBook.Nationality))
            {
                errorMsg = "Must provide the Author's Home Country";
                return false;
            }
            if (string.IsNullOrEmpty(newBook.OriginalLanguage))
            {
                errorMsg = "Must provide the Books's Original Language";
                return false;
            }
            if (_newBook.Pages < 1 && newBook.Format == BookFormat.Book)
            {
                errorMsg = "Must provide the number of pages for the books";
                return false;
            }

            return true;
        }

        private void InitialiseNewBook()
        {
            NewBookAuthorText = NewBookNationalityText = NewBookOriginalLanguageText = "";

            DateTime newDate =
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            _newBook = new BookRead()
            {
                Date = newDate,
                Format = BookFormat.Book,
                Book = "x",
                Comic = "",
                Audio = "",
                Note = ""
            };
        }

        private void UpdateNewBookWithNewText()
        {
            if (_newBook.Author == null && !string.IsNullOrEmpty(NewBookAuthorText))
                NewBookAuthor = NewBookAuthorText;
            if (_newBook.Author == null && !string.IsNullOrEmpty(NewBookNationalityText))
                NewBookNationality = NewBookNationalityText;
            if (_newBook.Author == null && !string.IsNullOrEmpty(NewBookOriginalLanguageText))
                NewBookOriginalLanguage = NewBookOriginalLanguageText;
        }

        private void InitialiseEditBook()
        {
            if (_mainModel.BooksRead.Count > 0)
                _existingBook = _mainModel.BooksRead.Last();
        }

        private void SelectImageForBook(BookRead book)
        {
            _log.Debug("Getting Book information for " + book.Title);

            // https://books.google.co.uk/
            string searchTerm = GetImageSearchTerm(book);
            ImageSelectionViewModel selectionViewModel = new ImageSelectionViewModel(_log, book.Title, searchTerm);

            ImageSelectionWindow imageSelectDialog = new ImageSelectionWindow { DataContext = selectionViewModel };
            var success = imageSelectDialog.ShowDialog();
            if (success.HasValue && success.Value)
            {
                _log.Debug("Success Getting image information for " + book.Title +
                           "\n   Img = " + selectionViewModel.SelectedImageAddress);

                book.ImageUrl = selectionViewModel.SelectedImageAddress;

                // Update in the DB.
                string errorMsg;
                if (!_mainModel.UpdateBook(book, out errorMsg))
                {
                    MessageBox.Show(errorMsg);
                    return;
                }

                OnPropertyChanged(() => ExistingBookImageSource);
            }
            else
            {
                _log.Debug("Failed Getting Book information for " + book.Title);
            }
        }

        #endregion
    }
}
