// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailboxLoaderViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The mailbox loader view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.Views;

    /// <summary>
    /// The mailbox loader view model.
    /// </summary>
    public class MailboxLoaderViewModel : BaseViewModel
    {
        #region Private Data

        /// <summary>
        /// The _main window.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// The _log.
        /// </summary>
        private log4net.ILog _log;

        /// <summary>
        /// The _main model.
        /// </summary>
        private MainBooksModel _mainModel;

        /// <summary>
        /// The _parent.
        /// </summary>
        private MainViewModel _parent;

        /// <summary>
        /// The _user name.
        /// </summary>
        private string _userName;

        /// <summary>
        /// The _data loaded.
        /// </summary>
        private bool _dataLoaded;

        /// <summary>
        /// The _connected to database successfully.
        /// </summary>
        private bool _connectedToDatabaseSuccessfully;

        /// <summary>
        /// The _email address.
        /// </summary>
        private string _emailAddress;

        /// <summary>
        /// The _password.
        /// </summary>
        private string _password;

        /// <summary>
        /// The _is valid to connect.
        /// </summary>
        private bool _isValidToConnect;

        /// <summary>
        /// True if currently reading e-mails from mailbox, false otherwise.
        /// </summary>
        private bool _readingEmails;

        /// <summary>
        /// The _mail items text.
        /// </summary>
        private string _mailItemsText;

        /// <summary>
        /// The books read from email.
        /// </summary>
        private ObservableCollection<IBookRead> _booksReadFromEmail;

        /// <summary>
        /// The error message from the mailbox reader.
        /// </summary>
        private string _mailboxErrorMessage;

        /// <summary>
        /// The books read from email.
        /// </summary>
        private bool _readBooksFromMailOk;

        /// <summary>
        /// The selected book read from email.
        /// </summary>
        private IBookRead _selectedBook;

        /// <summary>
        /// The selected book read from email ready to be added new.
        /// </summary>
        private BookRead _newBook;

        /// <summary>
        /// The connect to mailbox command.
        /// </summary>
        private ICommand _connectToMailboxCommand;

        /// <summary>
        /// The read email command.
        /// </summary>
        private ICommand _readEmailCommand;

        /// <summary>
        /// The set default user command.
        /// </summary>
        private ICommand _setDefaultUserCommand;

        /// <summary>
        /// The add new book from e-mail command.
        /// </summary>
        private ICommand _addSelectedBookCommand;

        /// <summary>
        /// The new book data input control has lost focus command.
        /// </summary>
        private ICommand _lostFocusCommand;

        /// <summary>
        /// The select image for nation command.
        /// </summary>
        private ICommand _selectImageForBookCommand;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether is data loaded.
        /// </summary>
        public bool IsDataLoaded
        {
            get
            {
                return _dataLoaded;
            }

            private set
            {
                _dataLoaded = value;
                OnPropertyChanged(() => IsDataLoaded);
            }
        }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }

            private set
            {
                _userName = value;
                OnPropertyChanged(() => UserName);
            }
        }

        /// <summary>
        /// Gets or sets the email adress.
        /// </summary>
        public string EmailAdress
        {
            get
            {
                return _emailAddress;
            }

            set
            {
                _emailAddress = value;
                ValidateCredentials();
                OnPropertyChanged(() => EmailAdress);
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                ValidateCredentials();
                OnPropertyChanged(() => Password);
            }
        }

        /// <summary>
        /// Gets or sets if reading e-mails.
        /// </summary>
        public bool ReadingEmails
        {
            get
            {
                return _readingEmails;
            }

            set
            {
                _readingEmails = value;
                OnPropertyChanged(() => ReadingEmails);
                OnPropertyChanged(() => CanSetEmailParameters);
            }

        }

        /// <summary>
        /// Gets if can set the e-mail parameters ie not reading e-mails.
        /// </summary>
        public bool CanSetEmailParameters => !_readingEmails;

        /// <summary>
        /// Gets or sets the mail items text.
        /// </summary>
        public string MailItemsText
        {
            get
            {
                return _mailItemsText;
            }

            set
            {
                _mailItemsText = value;
                OnPropertyChanged(() => MailItemsText);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is valid to connect.
        /// </summary>
        public bool IsValidToConnect
        {
            get
            {
                return _isValidToConnect;
            }

            private set
            {
                _isValidToConnect = value;
                OnPropertyChanged(() => IsValidToConnect);
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are books that have been read from e-mail to display & process.
        /// </summary>
        public bool BooksReadFromEmailToDisplay => BooksReadFromEmail != null && BooksReadFromEmail.Count > 0;

        /// <summary>
        /// Gets a value indicating whether there is a default user email.
        /// </summary>
        public bool IsDefaultUser => !string.IsNullOrEmpty(_userName);

        /// <summary>
        /// Gets the text for the set default e-mail button.
        /// </summary>
        public string SetDefaultUserText
        {
            get
            {
                if (!string.IsNullOrEmpty(_userName))
                {
                    return "Set e-mail to default? (" + _userName + ")";
                }

                return "No default available";
            }
        }

        /// <summary>
        /// Gets the text for the set default e-mail button.
        /// </summary>
        public IList<IBookRead> BooksReadFromEmail
        {
            get
            {
                return _booksReadFromEmail;
            }
        }

        public IBookRead SelectedEmailedBook
        {
            get { return _selectedBook; }
            set { _selectedBook = value; UpdateSelectedBook(); OnPropertyChanged(() => CanAddSelectedBook); }
        }

        public BookRead NewBook { get { return _newBook; } set { _newBook = value; UpdateNewBook(); } }

        public DateTime NewBookDate
        {
            get { if (_newBook != null) return _newBook.Date; return DateTime.Now; }
            set { if (_newBook != null) _newBook.Date = value; OnPropertyChanged(() => NewBookDateText); }
        }

        public string NewBookAuthor
        {
            get { if (_newBook != null) return _newBook.Author; return null; }
            set
            {
                if (value != null && _newBook != null)
                    _newBook.Author = value; OnPropertyChanged(() => NewBookPages); OnPropertyChanged(() => CanAddSelectedBook);
            }
        }

        public string NewBookTitle
        {
            get { if (_newBook != null) return _newBook.Title; return null; }
            set {
                if (value != null && _newBook != null)
                    _newBook.Title = value; OnPropertyChanged(() => NewBookTitle); OnPropertyChanged(() => CanAddSelectedBook); }
        }

        public UInt16 NewBookPages
        {
            get { if (_newBook != null) return _newBook.Pages; return 0; }
            set { if (_newBook != null) _newBook.Pages = value; OnPropertyChanged(() => NewBookPages); OnPropertyChanged(() => CanAddSelectedBook); }
        }

        public string NewBookNote
        {
            get { if (_newBook != null) return _newBook.Note; return null; }
            set { if (_newBook != null) _newBook.Note = value; }
        }

        public string NewBookNationality
        {
            get { if (_newBook != null) return _newBook.Nationality; return null; }
            set { if (_newBook != null) _newBook.Nationality = value; }
        }

        public string NewBookOriginalLanguage
        {
            get { if (_newBook != null) return _newBook.OriginalLanguage; return null; }
            set { if (_newBook != null) _newBook.OriginalLanguage = value; }
        }

        public BookFormat NewBookFormat
        {
            get { if (_newBook != null) return _newBook.Format; return BookFormat.Book; }
            set { if (_newBook != null) _newBook.Format = value; OnPropertyChanged(() => NewBookFormat); }
        }

        public string NewBookDateText
        { get { if (_newBook == null) return ""; _newBook.DateString = GetBookDateText(false); return _newBook.DateString; } }

        public string NewBookAuthorText { get; set; }

        public string NewBookNationalityText { get; set; }

        public string NewBookOriginalLanguageText { get; set; }

        public bool NewBookFormatIsAudio
        {
            get
            {
                return _newBook?.Format == BookFormat.Audio;
            }
            set
            {
                if (_newBook == null)
                    return;
                _newBook.Audio = value ? "x" : "";
                OnPropertyChanged(() => NewBookFormatIsAudio);
            }
        }

        public bool NewBookFormatIsComic
        {
            get
            {
                return _newBook?.Format == BookFormat.Comic;
            }
            set
            {
                if (_newBook == null)
                    return;
                _newBook.Comic = value ? "x" : "";
                OnPropertyChanged(() => NewBookFormatIsComic);
            }
        }

        public bool NewBookFormatIsBook
        {
            get
            {
                if (_newBook == null)
                    return true;
                return _newBook.Format == BookFormat.Book;
            }
            set
            {
                if (_newBook == null) return;
                _newBook.Comic = value ? "x" : "";
                OnPropertyChanged(() => NewBookFormatIsBook);
            }
        }

        public ObservableCollection<BookRead> BooksRead { get { return _mainModel.BooksRead; } }

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

        public bool CanAddSelectedBook => AreAllRequiredBookFieldsComplete();

        public Uri NewBookImageSource => _newBook != null ? _newBook?.DisplayImage : new Uri("pack://application:,,,/Images/camera_image_cancel-32.png");

        #endregion

        #region Commands

        /// <summary>
        /// Gets the read e-mail command.
        /// </summary>
        public ICommand ReadEmailCommand => _readEmailCommand ??
                                            (_readEmailCommand =
                                             new CommandHandler(ReadEmailCommandAction, true));

        /// <summary>
        /// Gets the connect to mailbox command.
        /// </summary>
        public ICommand SetDefaultUserCommand => _setDefaultUserCommand ??
                                                 (_setDefaultUserCommand =
                                                  new CommandHandler(SetDefaultUserCommandAction, true));

        /// <summary>
        /// Gets the connect to mailbox command.
        /// </summary>
        public ICommand ConnectToMailboxCommand => _connectToMailboxCommand ??
                                                   (_connectToMailboxCommand =
                                                    new CommandHandler(ConnectToMailboxCommandAction, true));

        /// <summary>
        /// Gets the add new book from e-mail command.
        /// </summary>
        public ICommand AddSelectedBookCommand => _addSelectedBookCommand ??
                                                   (_addSelectedBookCommand =
                                                    new CommandHandler(AddSelectedBookCommandAction, true));

        /// <summary>
        /// Gets the add new book from e-mail command.
        /// </summary>
        public ICommand LostFocusCommand => _lostFocusCommand ??
                                                   (_lostFocusCommand =
                                                    new CommandHandler(LostFocusCommandAction, true));

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand SelectImageForBookCommand => _selectImageForBookCommand ??
                                                     (_selectImageForBookCommand =
                                                         new RelayCommandHandler(SelectImageForBookCommandAction) { IsEnabled = true });

        #endregion

        #region Utility Functions

        /// <summary>
        /// Check if all the required fields of new book completed.
        /// </summary>
        private bool AreAllRequiredBookFieldsComplete()
        {
            if (_selectedBook != null 
                && !string.IsNullOrEmpty(_newBook?.Author) 
                && !string.IsNullOrEmpty(_newBook.Title) 
                && !string.IsNullOrEmpty(_newBook.Nationality) 
                && !string.IsNullOrEmpty(_newBook.OriginalLanguage))
            {
                return _newBook.Format != BookFormat.Book || _newBook.Pages >= 1;
            }

            return false;
        }

        /// <summary>
        /// The validate credentials.
        /// </summary>
        private void ValidateCredentials()
        {
            IsValidToConnect = false;

            if (_password != null && _password.Length > 1 &&
                _emailAddress != null && _emailAddress.Length > 3 &&
                    _emailAddress.Contains("@"))
            {
                if (_mainModel.MailReader.ValidateEmailAndPassword(_emailAddress, _password))
                {
                    IsValidToConnect = true;
                }
            }
        }

        /// <summary>
        /// The validate credentials.
        /// </summary>
        private string GetMessageTextForEmailedBooksRead(ObservableCollection<IBookRead> books)
        {
            if (books == null || books.Count == 0)
                return "No books read from E-mails\n";

            string messageText = $"The following {books.Count} books have been read from E-mail:\n\n";

            return books.Select(book => $"{book.Title} by {book.Author} on {book.DateString} \n")
                .Aggregate(messageText, (current, bookLine) => current + bookLine);
        }

        private void UpdateSelectedBook()
        {
            if (_selectedBook == null)
                return;

            // Create a new book for the db populated with the info rom the selection
            BookRead newBook = new BookRead
            {
                DateString = _selectedBook.DateString,
                Date = _selectedBook.Date,
                Author = _selectedBook.Author,
                Title = _selectedBook.Title,
                Format = _selectedBook.Format,
                Pages = _selectedBook.Pages,
            };
            NewBook = newBook;
            NewBookAuthorText = _selectedBook.Author;
            OnPropertyChanged(() => NewBookAuthorText);
            NewBookFormat = _selectedBook.Format;
            _newBook.Audio = _selectedBook.Format == BookFormat.Audio ? "x" : "";
            _newBook.Book = _selectedBook.Format == BookFormat.Book ? "x" : "";
            _newBook.Comic = _selectedBook.Format == BookFormat.Comic ? "x" : "";
            OnPropertyChanged(() => NewBookFormat);
            OnPropertyChanged(() => NewBookFormatIsAudio);
            OnPropertyChanged(() => NewBookFormatIsBook);
            OnPropertyChanged(() => NewBookFormatIsComic);
            OnPropertyChanged(() => NewBookImageSource);
        }

        private void UpdateNewBook()
        {
            OnPropertyChanged(() => NewBook);
            OnPropertyChanged(() => NewBookDate);
            OnPropertyChanged(() => NewBookDate);
            OnPropertyChanged(() => NewBookAuthor);
            OnPropertyChanged(() => NewBookTitle);
            OnPropertyChanged(() => NewBookPages);
            OnPropertyChanged(() => NewBookNote);
            OnPropertyChanged(() => NewBookNationality);
            OnPropertyChanged(() => NewBookOriginalLanguage);
            OnPropertyChanged(() => NewBookFormat);
            OnPropertyChanged(() => NewBookDateText);
            OnPropertyChanged(() => NewBookAuthorText);
            OnPropertyChanged(() => NewBookNationalityText);
            OnPropertyChanged(() => NewBookOriginalLanguageText);
        }

        private string GetBookDateText(bool isNew = true)
        {
            DateTime date = NewBookDate;
            if (!isNew)
            {
                date = NewBookDate;
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

        private void ReadMailWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!_readBooksFromMailOk)
            {
                ReadingEmails = false;
                MessageBox.Show(_mailboxErrorMessage, "Could Not Read Mail");
            }
            else
            {
                ReadingEmails = false;
                OnPropertyChanged(() => BooksReadFromEmail);
                OnPropertyChanged(() => BooksReadFromEmailToDisplay);

                MailItemsText = GetMessageTextForEmailedBooksRead(_booksReadFromEmail);

                RemoveBooksAlreadyInDatabaseFromEmailBooks(_booksReadFromEmail);

                _mainModel.DefaultUserName = _emailAddress;
            }
        }

        private void RemoveBooksAlreadyInDatabaseFromEmailBooks(ObservableCollection<IBookRead> booksReadFromEmail)
        {
            ObservableCollection<IBookRead> duplicateBooks = 
                new ObservableCollection<IBookRead>(
                    booksReadFromEmail.Where(emailBook => _mainModel.BooksRead.Any(existingBook => (
                    emailBook.Title == existingBook.Title && emailBook.Author == existingBook.Author) || 
                    (emailBook.Title == existingBook.Title && emailBook.Date.ToShortDateString() == existingBook.Date.ToShortDateString()) || 
                    (emailBook.Author == existingBook.Author && emailBook.Date.ToShortDateString() == existingBook.Date.ToShortDateString()))).ToList()
                    );

            foreach (IBookRead duplicate in duplicateBooks)
                booksReadFromEmail.Remove(duplicate);
        }

        private void DoReadMailWork(object sender, DoWorkEventArgs e)
        {
            _readBooksFromMailOk =
                _mainModel.MailReader.ReadBooksFromMail(_emailAddress, _password, out _mailboxErrorMessage, out _booksReadFromEmail);
        }

        #endregion

        #region Command Handlers

        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void AddSelectedBookCommandAction()
        {
            string errorMsg;
            // add to the list....
            if (!_mainModel.AddNewBook(_newBook, out errorMsg))
            {
                MessageBox.Show(errorMsg);
            }
            else
            {
                // remove the selected item from the list
                BooksReadFromEmail.Remove(_selectedBook);
                
                // reset the new & selected
                NewBook = null;
                SelectedEmailedBook = null;

                // update the main grid ets
                _parent.UpdateData();
                OnPropertyChanged("");

                OnPropertyChanged(() => BooksReadFromEmail);
                OnPropertyChanged(() => CanAddSelectedBook);
            }
        }

        /// <summary>
        /// The command action to set to the default e-mail.
        /// </summary>
        public void SetDefaultUserCommandAction()
        {
            EmailAdress = _userName;
        }

        /// <summary>
        /// The command action to read the books read from  e-mail server.
        /// </summary>
        public void ReadEmailCommandAction()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += DoReadMailWork;
            bw.RunWorkerCompleted += ReadMailWorkerCompleted;

            ReadingEmails = true;
            bw.RunWorkerAsync();
        }
        
        /// <summary>
        /// The connect to mailbox command action.
        /// </summary>
        public void ConnectToMailboxCommandAction()
        {
            BooksFromMailboxWindow mailDialog = new BooksFromMailboxWindow { DataContext = this };
            mailDialog.ShowDialog();
        }

        /// <summary>
        /// The new book data input control has lost focus command action.
        /// </summary>
        public void LostFocusCommandAction()
        {
            OnPropertyChanged(() => CanAddSelectedBook);
        }

        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void SelectImageForBookCommandAction(object parameter)
        {
            BookRead book = _newBook;
            if (book != null)
            {
                _log.Debug("Getting Book information for " + book.Title);

                // https://books.google.co.uk/
                string searchTerm = DataUpdaterViewModel.GetImageSearchTerm(book);
                ImageSelectionViewModel selectionViewModel = new ImageSelectionViewModel(_log, book.Title, searchTerm);

                ImageSelectionWindow imageSelectDialog = new ImageSelectionWindow { DataContext = selectionViewModel };
                var success = imageSelectDialog.ShowDialog();
                if (success.HasValue && success.Value)
                {
                    _log.Debug("Success Getting image information for " + book.Title +
                               "\n   Img = " + selectionViewModel.SelectedImageAddress);

                    book.ImageUrl = selectionViewModel.SelectedImageAddress;

                    OnPropertyChanged(() => NewBookImageSource);
                }
                else
                {
                    _log.Debug("Failed Getting Book information for " + book.Title);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MailboxLoaderViewModel"/> class.
        /// </summary>
        /// <param name="mainWindow">
        /// The main window.
        /// </param>
        /// <param name="log">
        /// The log.
        /// </param>
        /// <param name="mainModel">
        /// The main model.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public MailboxLoaderViewModel(
            MainWindow mainWindow, log4net.ILog log, MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;
            _userName = _mainModel.DefaultUserName;

            IsDataLoaded = mainModel.BooksRead.Count > 0;
            ReadingEmails = false;

            _booksReadFromEmail = new ObservableCollection<IBookRead>();
        }

        #endregion
    }
}
