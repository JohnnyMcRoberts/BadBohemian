// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The users view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Input;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.Views;

    /// <summary>
    /// The users view model.
    /// </summary>
    public class UsersViewModel : BaseViewModel
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
        private ICommand _manageUsersCommand;

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
        private ICommand _addNewUserCommand;

        /// <summary>
        /// The new book data input control has lost focus command.
        /// </summary>
        private ICommand _lostFocusCommand;

        /// <summary>
        /// The select image for nation command.
        /// </summary>
        private ICommand _selectImageForBookCommand;

        #endregion

        #region Public data

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

        #endregion

        #region Commands

        /// <summary>
        /// Gets the connect to mailbox command.
        /// </summary>
        public ICommand ManageUsersCommand => _manageUsersCommand ??
                                                   (_manageUsersCommand =
                                                       new CommandHandler(ManageUsersCommandAction, true));

        /// <summary>
        /// Gets the connect to mailbox command.
        /// </summary>
        public ICommand AddNewUserCommand => _addNewUserCommand ??
                                              (_addNewUserCommand =
                                                  new CommandHandler(AddNewUserCommandAction, true));

        #endregion

        #region Command Actions

        /// <summary>
        /// The manage users command action.
        /// </summary>
        public void ManageUsersCommandAction()
        {
            UsersWindow usersDialog = new UsersWindow { DataContext = this };
            usersDialog.ShowDialog();
        }

        /// <summary>
        /// The manage users command action.
        /// </summary>
        public void AddNewUserCommandAction()
        {
            UserName = "Hash Now " + DateTime.Now.ToLongTimeString();
            UserName += " = " + MD5Hash(UserName);
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Sets the fill color.
        /// </summary>
        /// <param name="input">The input text.</param>
        /// <returns>The hash for string.</returns>
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5Provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            foreach (byte b in bytes)
            {
                hash.Append(b.ToString("x2"));
            }

            return hash.ToString();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersViewModel"/> class.
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
        public UsersViewModel(
            MainWindow mainWindow, log4net.ILog log, MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;
            _userName = _mainModel.DefaultUserName;

            _booksReadFromEmail = new ObservableCollection<IBookRead>();
            UserName = "N/A";
        }

        #endregion

    }
}
