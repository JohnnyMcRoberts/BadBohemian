using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using ActiveUp.Net.Mail;

namespace MailTestApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException("sExpression");

            MemberExpression body = sExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        #region Private Data

        private string _emailAddress;
        private string _password;
        private bool _isValidToConnect;
        private string _mailItemsText;

        private ICommand _readEmailCommand;

        #endregion

        #region Public Properties

        public string EmailAdress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; ValidateCredentials(); OnPropertyChanged(() => EmailAdress); }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; ValidateCredentials(); OnPropertyChanged(() => Password); }
        }

        public string MailItemsText
        {
            get { return _mailItemsText; }
            set { _mailItemsText = value; OnPropertyChanged(() => MailItemsText); }
        }

        public bool IsValidToConnect
        {
            get { return _isValidToConnect; }
            private set { _isValidToConnect = value; OnPropertyChanged(() => IsValidToConnect); }
        }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            _emailAddress = "";
            _password = "";
            _isValidToConnect = false;
        }

        #endregion

        #region Commands 
        public ICommand ReadEmailCommand
        {
            get
            {
                return _readEmailCommand ??
                    (_readEmailCommand =
                        new CommandHandler(() => ReadEmailCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers


        private const int _imapPort = 993;
        private const string _imapServerAddress = "imap.gmail.com";

        public void ReadEmailCommandAction()
        {
            string msg = "Getting Mail from '" + _emailAddress + "' with password '" + _password + "'";
            MessageBox.Show(msg);

            // login johnnymcbobatwork@gmail.com

            string _selectedMailBox = "INBOX";

            using (var _clientImap4 = new Imap4Client())
            {

                try
                {

                    _clientImap4.ConnectSsl(_imapServerAddress, _imapPort);
                    // another option is: _clientImap4.Connect(_mailServer.address, _mailServer.port);

                    _clientImap4.Login(_emailAddress, _password); // Make log in and load all MailBox.
                                                                  //_clientImap4.LoginFast(_imapLogin, _imapPassword); // Only make login.

                    var _mailBox = _clientImap4.SelectMailbox(_selectedMailBox);

                    string messageItemText = "";
                    foreach (var messageId in _mailBox.Search("ALL").AsEnumerable())
                    {
                        var message = _mailBox.Fetch.Message(messageId);
                        var _imapMessage = Parser.ParseMessage(message);
                        messageItemText +=
                        "From: " + _imapMessage.From.Name + ", Subject: " + _imapMessage.Subject + "\n";

                        if (_imapMessage.Subject.ToLower().Contains("tallies"))
                        {
                            messageItemText += "\n";

                            messageItemText += _imapMessage.BodyText.Text;

                            messageItemText += "\n";
                        }
                    }
                    MailItemsText = messageItemText;
                    _clientImap4.Disconnect();
                }
                catch (Exception e)
                {
                    string error = e.Message + " : " + e.InnerException;
                    MessageBox.Show(error);
                }

            }



        }

        #endregion

        #region Utility Functions

        private void ValidateCredentials()
        {
            IsValidToConnect = false;
            if (_password != null && _password.Length > 1 &&
                _emailAddress != null && _emailAddress.Length > 3 &&
                    _emailAddress.Contains("@"))
            {
                IsValidToConnect = true;
            }
        }

        #endregion
    }
}
