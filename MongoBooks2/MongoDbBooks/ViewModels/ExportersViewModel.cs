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
    using System.Linq.Expressions;
    using System.Windows;
    using System.Windows.Input;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.Views;

    /// <summary>
    /// The mailbox loader view model.
    /// </summary>
    public class ExportersViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="expression">
        /// The string from the function expression.
        /// </param>
        /// <typeparam name="T">The type that has changed</typeparam>
        protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(@"expression");
            }

            MemberExpression body = expression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }

            this.OnPropertyChanged(body.Member.Name);
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

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
        private bool _connectedToDatabaseSuccessfully = false;

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
        /// The new book data input control has lost focus command.
        /// </summary>
        private ICommand _lostFocusCommand;

        /// <summary>
        /// The export via email command.
        /// </summary>
        private ICommand _exportViaEmailCommand;

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
        /// Gets if can set the e-mail parameters ie not reading e-mails.
        /// </summary>
        public bool CanSetEmailParameters => !_readingEmails;

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
        #endregion // INotifyPropertyChanged Members


        #region Commands

        /// <summary>
        /// Gets the export via email command.
        /// </summary>
        public ICommand ExportViaEmailCommand => _exportViaEmailCommand ??
                                            (_exportViaEmailCommand =
                                             new CommandHandler(ExportViaEmailCommandAction, true));


        /// <summary>
        /// Gets the connect to mailbox command.
        /// </summary>
        public ICommand ConnectToMailboxCommand => _connectToMailboxCommand ??
                                                   (_connectToMailboxCommand =
                                                    new CommandHandler(ConnectToMailboxCommandAction, true));

        /// <summary>
        /// Gets the set defualt user command.
        /// </summary>
        public ICommand SetDefaultUserCommand => _setDefaultUserCommand ??
                                                 (_setDefaultUserCommand =
                                                  new CommandHandler(SetDefaultUserCommandAction, true));

        #endregion // INotifyPropertyChanged Members


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


        #region Command Actions

        /// <summary>
        /// The command action to set to the default e-mail.
        /// </summary>
        public void SetDefaultUserCommandAction()
        {
            EmailAdress = _userName;
        }

        /// <summary>
        /// The connect to mailbox command action.
        /// </summary>
        public void ExportViaEmailCommandAction()
        {
            ExportViaEmailWindow mailDialog = new ExportViaEmailWindow { DataContext = this };
            mailDialog.ShowDialog();
        }

        /// <summary>
        /// The command action to set to the default e-mail.
        /// </summary>
        public void ConnectToMailboxCommandAction()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += DoReadMailWork;
            bw.RunWorkerCompleted += ReadMailWorkerCompleted;

            ReadingEmails = true;
            bw.RunWorkerAsync();
        }

        private void ReadMailWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ReadingEmails = false;
            MessageBox.Show(_mailboxErrorMessage, "Could Not Read Mail");
        }

        private void DoReadMailWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
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
        public ExportersViewModel(
            MainWindow mainWindow, log4net.ILog log, MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;
            _userName = _mainModel.DefaultUserName;
            
            ReadingEmails = false;
            
        }

        #endregion

    }
}
