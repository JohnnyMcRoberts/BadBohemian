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
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Mail;
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
        /// The default destination e-mail address.
        /// </summary>
        private string _defaultDestinationEmail;
        
        /// <summary>
        /// The home e-mail address.
        /// </summary>
        private string _homeEmailAddress;

        /// <summary>
        /// The destination e-mail address.
        /// </summary>
        private string _destinationEmailAddress;

        /// <summary>
        /// The password.
        /// </summary>
        private string _password;

        /// <summary>
        /// The text to include in the export e-mail body.
        /// </summary>
        private string _emailMessageText;

        /// <summary>
        /// Flag indicating whether it is valid to connect to the mailbox.
        /// </summary>
        private bool _isValidToConnect;

        /// <summary>
        /// Flag indicating whether to send the books read file in the export.
        /// </summary>
        private bool _sendBooksReadFile;

        /// <summary>
        /// Flag indicating whether to send the locations file in the export.
        /// </summary>
        private bool _sendLocationsFile;

        /// <summary>
        /// Flag indicating whether it is valid to send export e-mail.
        /// </summary>
        private bool _isValidToSendExportEmail;

        /// <summary>
        /// True if currently reading e-mails from mailbox, false otherwise.
        /// </summary>
        private bool _connectingToMailbox;

        /// <summary>
        /// True if currently sending the e-mails from mailbox, false otherwise.
        /// </summary>
        private bool _sendingExportEmail;

        /// <summary>
        /// True if connected successfully to the mailbox, false otherwise.
        /// </summary>
        private bool _connectedToMailbox;

        /// <summary>
        /// True if sent the e-mail successfully, false otherwise.
        /// </summary>
        private bool _sentEmail;

        /// <summary>
        /// The error message from the mailbox reader.
        /// </summary>
        private string _mailboxErrorMessage;

        /// <summary>
        /// The connect to mailbox command.
        /// </summary>
        private ICommand _connectToMailboxCommand;

        /// <summary>
        /// The set default user command.
        /// </summary>
        private ICommand _setDefaultUserCommand;

        /// <summary>
        /// The export via email command.
        /// </summary>
        private ICommand _exportViaEmailCommand;

        /// <summary>
        /// The set default destination e-mail command.
        /// </summary>
        private ICommand _setDefaultDestinationEmailCommand;

        /// <summary>
        /// The send export email command.
        /// </summary>
        private ICommand _sendExportEmailCommand;

        #endregion

        #region Public Properties

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
        /// Gets or sets the home e-mail adress.
        /// </summary>
        public string HomeEmailAdress
        {
            get
            {
                return _homeEmailAddress;
            }

            set
            {
                _homeEmailAddress = value;
                ValidateMailboxCredentials();
                OnPropertyChanged(() => HomeEmailAdress);
            }
        }

        /// <summary>
        /// Gets or sets the destination e-mail adress.
        /// </summary>
        public string DestinationEmailAddress
        {
            get
            {
                return _destinationEmailAddress;
            }

            set
            {
                _destinationEmailAddress = value;
                ValidateExportCredentials();
                OnPropertyChanged(() => DestinationEmailAddress);
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
                ValidateMailboxCredentials();
                OnPropertyChanged(() => Password);
            }
        }

        /// <summary>
        /// Gets if can set the e-mail parameters ie not reading e-mails.
        /// </summary>
        public bool CanSetEmailParameters => !_connectingToMailbox;

        /// <summary>
        /// Gets or sets if connecting to the mailbox.
        /// </summary>
        public bool ConnectingToMailbox
        {
            get
            {
                return _connectingToMailbox;
            }

            set
            {
                _connectingToMailbox = value;
                OnPropertyChanged(() => ConnectingToMailbox);
                OnPropertyChanged(() => CanSetEmailParameters);
            }
        }

        /// <summary>
        /// Gets or sets if sending the export e-mail.
        /// </summary>
        public bool SendingExportEmail
        {
            get
            {
                return _sendingExportEmail;
            }

            set
            {
                _sendingExportEmail = value;
                OnPropertyChanged(() => SendingExportEmail);
                OnPropertyChanged(() => CanSetEmailParameters);
            }
        }

        /// <summary>
        /// Gets or sets if connected to the mailbox.
        /// </summary>
        public bool ConnectedToMailbox
        {
            get
            {
                return _connectedToMailbox;
            }

            set
            {
                _connectedToMailbox = value;
                OnPropertyChanged(() => ConnectedToMailbox);
                OnPropertyChanged(() => CanSetEmailParameters);
            }
        }

        /// <summary>
        /// Gets or sets if sent the export e-mail.
        /// </summary>
        public bool SentEmail
        {
            get
            {
                return _sentEmail;
            }

            set
            {
                _sentEmail = value;
                OnPropertyChanged(() => SentEmail);
                OnPropertyChanged(() => CanSetEmailParameters);
            }
        }

        /// <summary>
        /// Gets or sets if to include the books read file in the export e-mail.
        /// </summary>
        public bool SendBooksReadFile
        {
            get
            {
                return _sendBooksReadFile;
            }

            set
            {
                _sendBooksReadFile = value;
                OnPropertyChanged(() => SendBooksReadFile);
                ValidateExportCredentials();
            }
        }

        /// <summary>
        /// Gets or sets if to include the locations file in the export e-mail.
        /// </summary>
        public bool SendLocationsFile
        {
            get
            {
                return _sendLocationsFile;
            }

            set
            {
                _sendLocationsFile = value;
                OnPropertyChanged(() => SendLocationsFile);
                ValidateExportCredentials();
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
        /// Gets a value indicating whether is valid to export.
        /// </summary>
        public bool IsValidToSendExportEmail
        {
            get
            {
                return _isValidToSendExportEmail;
            }

            private set
            {
                _isValidToSendExportEmail = value;
                OnPropertyChanged(() => IsValidToSendExportEmail);
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is a default user email.
        /// </summary>
        public bool IsDefaultUser => !string.IsNullOrEmpty(_userName);

        /// <summary>
        /// Gets a value indicating whether there is a default destination e-mail.
        /// </summary>
        public bool IsDefaultDestinationEmail => !string.IsNullOrEmpty(_defaultDestinationEmail);       

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
        public string SetDefaultDestinationEmailText
        {
            get
            {
                if (!string.IsNullOrEmpty(_defaultDestinationEmail))
                {
                    return "Set e-mail to default? (" + _defaultDestinationEmail + ")";
                }

                return "No default available";
            }
        }


        public string EmailMessageText
        {
            get
            {
                return _emailMessageText;
            }

            set
            {
                _emailMessageText = value;
                ValidateExportCredentials();
                OnPropertyChanged(() => EmailMessageText);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the export via email command.
        /// </summary>
        public ICommand ExportViaEmailCommand => _exportViaEmailCommand ??
                                            (_exportViaEmailCommand =
                                             new CommandHandler(ExportViaEmailCommandAction, true));

        /// <summary>
        /// Gets the set defualt user command.
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
        /// Gets the send export e-mail command.
        /// </summary>
        public ICommand SendExportEmailCommand => _sendExportEmailCommand ??
                                                 (_sendExportEmailCommand =
                                                  new CommandHandler(SendExportEmailCommandAction, true));

        /// <summary>
        /// Gets the set default destination e-mail command.
        /// </summary>
        public ICommand SetDefaultDestinationEmailCommand => _setDefaultDestinationEmailCommand ??
                                                   (_setDefaultDestinationEmailCommand =
                                                    new CommandHandler(SetDefaultDestinationEmailCommandAction, true));

        #endregion

        #region Utility Functions

        /// <summary>
        /// Validates the home mailbox credentials.
        /// </summary>
        private void ValidateMailboxCredentials()
        {
            IsValidToConnect = false;

            if (_password != null && _password.Length > 1 &&
                _homeEmailAddress != null && _homeEmailAddress.Length > 3 &&
                    _homeEmailAddress.Contains("@"))
            {
                if (_mainModel.MailReader.ValidateEmailAndPassword(_homeEmailAddress, _password))
                {
                    IsValidToConnect = true;
                }
            }
        }

        /// <summary>
        /// Validates the export credentials.
        /// </summary>
        private void ValidateExportCredentials()
        {
            IsValidToSendExportEmail = false;

            if (_password != null && _password.Length > 1 &&
                _homeEmailAddress != null && _homeEmailAddress.Length > 3 &&
                    _homeEmailAddress.Contains("@"))
            {
                if (_mainModel.MailReader.ValidateEmailAndPassword(_homeEmailAddress, _password))
                {
                    IsValidToSendExportEmail = true;
                }
            }
        }

        /// <summary>
        /// Handles the result of a connection to a gmail inbox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArguments">The completed event arguments.</param>
        private void ConnectToMailboxWorkerCompleted(object sender, RunWorkerCompletedEventArgs eventArguments)
        {
            if (!ConnectedToMailbox)
            {
                MessageBox.Show(_mailboxErrorMessage, "Could not connect to E-mail");
            }
            ConnectingToMailbox = false;
        }

        /// <summary>
        /// Tries to connect to a gmail inbox on a separate thread.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArguments">The event arguments.</param>
        private void DoConnectToMailboxWork(object sender, DoWorkEventArgs eventArguments)
        {
            ConnectingToMailbox = true;
            ConnectedToMailbox =
                _mainModel.MailReader.ConnectToMailbox(HomeEmailAdress, _password, out _mailboxErrorMessage);
        }

        /// <summary>
        /// Handles the result of an attempt to send an e-mail.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArguments">The completed event arguments.</param>
        private void SendExportEmailWorkerCompleted(object sender, RunWorkerCompletedEventArgs eventArguments)
        {
            if (!SentEmail)
            {
                MessageBox.Show(_mailboxErrorMessage, "Could not connect to E-mail");
            }
            SendingExportEmail = false;
        }

        /// <summary>
        /// Tries to send the export e-mail on a separate thread.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArguments">The event arguments.</param>
        private void DoSendExportEmailWork(object sender, DoWorkEventArgs eventArguments)
        {
            SentEmail = true;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(HomeEmailAdress);
                    mail.To.Add(DestinationEmailAddress);
                    mail.Subject = "Export Books";
                    mail.Body = "<h1>Export Books Notes</h1>" + "<h2>"+EmailMessageText+"</h2>";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(HomeEmailAdress, Password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        Console.WriteLine(@"Sent OK 1!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" error 1 = " + ex.Message);
                Console.WriteLine(" ex 1 = " + ex.ToString());
                SentEmail = false;
            }
        }

        #endregion

        #region Command Actions

        /// <summary>
        /// The command action to set to the default e-mail.
        /// </summary>
        public void SetDefaultUserCommandAction()
        {
            HomeEmailAdress = _userName;
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
            bw.DoWork += DoConnectToMailboxWork;
            bw.RunWorkerCompleted += ConnectToMailboxWorkerCompleted;

            ConnectingToMailbox = true;
            ConnectedToMailbox = false;
            bw.RunWorkerAsync();
        }

        /// <summary>
        /// The command action to set to the default destination e-mail.
        /// </summary>
        public void SetDefaultDestinationEmailCommandAction()
        {
            HomeEmailAdress = _userName;
        }

        /// <summary>
        /// The command action to send the export e-mail.
        /// </summary>
        public void SendExportEmailCommandAction()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += DoSendExportEmailWork;
            bw.RunWorkerCompleted += SendExportEmailWorkerCompleted;

            SendingExportEmail = true;
            bw.RunWorkerAsync();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MailboxLoaderViewModel"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="log">The log.</param>
        /// <param name="mainModel">The main model.</param>
        /// <param name="parent">The parent.</param>
        public ExportersViewModel(
            MainWindow mainWindow, log4net.ILog log, MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;
            _userName = _mainModel.DefaultUserName;
            _defaultDestinationEmail = _mainModel.DefaultRecipientName;

            ConnectingToMailbox = false;
            ConnectedToMailbox = false;
            SendingExportEmail = false;

        }

        #endregion
    }
}
