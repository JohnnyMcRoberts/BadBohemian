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
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// The users view model.
    /// </summary>
    public class UsersViewModel : BaseViewModel
    {
        #region Constants

        /// <summary>
        /// The minimum allowable length for a user name.
        /// </summary>
        private const int MinimumUserNameLength = 8;

        #endregion

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


        #region Add User

        /// <summary>
        /// The name of the new user to add.
        /// </summary>
        private string _addUserName;

        /// <summary>
        /// The e-mail of the new user to add.
        /// </summary>
        private string _addUserEmail;

        /// <summary>
        /// The password of the new user to add.
        /// </summary>
        private string _addUserPassword;

        /// <summary>
        /// The description of the new user to add.
        /// </summary>
        private string _addUserDescription;

        /// <summary>
        /// The image of the new user to add.
        /// </summary>
        private string _addUserImage;

        /// <summary>
        /// The add user command.
        /// </summary>
        private ICommand _addUserCommand;

        /// <summary>
        /// The select image for add user command.
        /// </summary>
        private ICommand _addUserSelectUserCommand;

        #endregion

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
        /// The new book data input control has lost focus command.
        /// </summary>
        private ICommand _lostFocusCommand;

        /// <summary>
        /// The select image for nation command.
        /// </summary>
        private ICommand _selectImageForBookCommand;

        #endregion

        #region Public data

        #region Add User

        /// <summary>
        /// Gets the user name.
        /// </summary>
        public string AddUserName
        {
            get
            {
                return _addUserName;
            }

            private set
            {
                _addUserName = value;
                OnPropertyChanged(() => AddUserName);
                OnPropertyChanged(() => CanAddUser);
            }
        }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        public string AddUserPassword
        {
            get
            {
                return _addUserPassword;
            }

            private set
            {
                _addUserPassword = value;
                OnPropertyChanged(() => AddUserPassword);
                OnPropertyChanged(() => CanAddUser);
            }
        }

        /// <summary>
        /// Gets the user email.
        /// </summary>
        public string AddUserEmail
        {
            get
            {
                return _addUserEmail;
            }

            private set
            {
                _addUserEmail = value;
                OnPropertyChanged(() => AddUserEmail);
                OnPropertyChanged(() => CanAddUser);
            }
        }

        /// <summary>
        /// Gets the user description.
        /// </summary>
        public string AddUserDescription
        {
            get
            {
                return _addUserDescription;
            }

            private set
            {
                _addUserDescription = value;
                OnPropertyChanged(() => AddUserDescription);
            }
        }

        /// <summary>
        /// Gets if can have enough data entered to add a new user.
        /// </summary>
        public bool CanAddUser =>
            IsValidName(_addUserName) &&
            IsValidPassword(_addUserPassword) &&
            IsValidEmail(_addUserEmail);

        /// <summary>
        /// Gets the add user image URI ready to be displayed.
        /// </summary>
        //"http://www.ucl.ac.uk/pals/research/linguistics/people/images/blank-300px"
        //public Uri AddUserImageSource => !string.IsNullOrEmpty(_addUserImage) ? new Uri(_addUserImage) : new Uri("pack://application:,,,/Images/camera_image_cancel-32.png");
        public Uri AddUserImageSource => !string.IsNullOrEmpty(_addUserImage) ? new Uri(_addUserImage) : new Uri("http://www.ucl.ac.uk/pals/research/linguistics/people/images/blank-300px");

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Gets the connect to mailbox command.
        /// </summary>
        public ICommand ManageUsersCommand => _manageUsersCommand ??
                                                   (_manageUsersCommand =
                                                       new CommandHandler(ManageUsersCommandAction, true));

        #region Add User

        /// <summary>
        /// Gets the add user select image command.
        /// </summary>
        public ICommand AddUserSelectImageCommand => _addUserSelectUserCommand ??
                                              (_addUserSelectUserCommand =
                                                  new CommandHandler(AddUserSelectImageCommandAction, true));

        /// <summary>
        /// Gets the add user command.
        /// </summary>
        public ICommand AddUserCommand => _addUserCommand ??
                                              (_addUserCommand =
                                                  new CommandHandler(AddUserCommandAction, true));

        #endregion


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

        #region Add User

        /// <summary>
        /// The add user select image command action.
        /// </summary>
        public void AddUserSelectImageCommandAction()
        {
            string searchTerm = GetImageSearchTerm(_addUserName);
            ImageSelectionViewModel selectionViewModel = 
                new ImageSelectionViewModel(_log, _addUserName, searchTerm);

            ImageSelectionWindow imageSelectDialog = 
                new ImageSelectionWindow { DataContext = selectionViewModel };
            var success = imageSelectDialog.ShowDialog();
            if (success.HasValue && success.Value)
            {
                _addUserImage = selectionViewModel.SelectedImageAddress;
                OnPropertyChanged(() => AddUserImageSource);
            }
        }

        /// <summary>
        /// The add user command action.
        /// </summary>
        public void AddUserCommandAction()
        {
            //UserName = "Hash Now " + DateTime.Now.ToLongTimeString();
            //UserName += " = " + MD5Hash(UserName);

            Models.Database.User newUser = new Models.Database.User()
            {
                Name = _addUserName,
                Description = _addUserDescription,
                Email = _addUserEmail,
                ImageUri = _addUserImage,
                DateAdded = DateTime.Now,
                PasswordHash = MD5Hash(_addUserPassword)
            };

            _mainModel.UserDatabase.AddNewItemToDatabase(newUser);
            _mainModel.UserDatabase.ConnectToDatabase();

            MessageBox.Show("User added ok ");

            ClearAddUser();
        }

        private void ClearAddUser()
        {
            AddUserName = string.Empty;
            AddUserEmail = string.Empty;
            AddUserPassword = string.Empty;
            AddUserDescription = string.Empty;
            _addUserPassword = string.Empty;            
            OnPropertyChanged(() => AddUserImageSource);
            OnPropertyChanged(() => CanAddUser);
        }

        #endregion

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

        /// <summary>
        /// Checks if a valid name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            // check long enough
            if (name.Length < MinimumUserNameLength)
                return false;

            // check not already there
            foreach (var user in _mainModel.Users)
                if (user.Name.ToUpper().Equals(name.ToUpper()))
                    return false;

            return true;
        }

        /// <summary>
        /// Checks if a valid password.
        /// </summary>
        /// <param name="password">The string to check.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public bool IsValidPassword(string password)
        {
            // check something
            if (string.IsNullOrEmpty(password))
                return false;

            // check long enough
            if (password.Length < MinimumUserNameLength)
                return false;

            // check contains at least a number
            if (!password.Any(char.IsDigit) || !password.Any(char.IsLower) || !password.Any(char.IsUpper))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a valid email.
        /// </summary>
        /// <param name="email">The string to check.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public static bool IsValidEmail(string email)
        {
            // check something
            if (string.IsNullOrEmpty(email))
                return false;

            // check long enough
            if (email.Length < MinimumUserNameLength)
                return false;

            // check contains at least a number
            if (!email.Contains("@") || !email.Contains("."))
                return false;

            return true;
        }

        public static string GetImageSearchTerm(string name)
        {
            string[] words = name.Split(' ');

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

        }

        #endregion


#if not_this
        
        /// <summary>
        /// Gets the add user select image command.
        /// </summary>
        public ICommand AddUserSelectImageCommand => _addUserSelectUserCommand ??
                                              (_addUserSelectUserCommand =
                                                  new CommandHandler(AddUserSelectImageCommandAction, true));

        /// <summary>
        /// Gets the add user command.
        /// </summary>
        public ICommand AddUserCommand => _addUserCommand ??
                                              (_addUserCommand =
                                                  new CommandHandler(AddUserCommandAction, true));
        
                    <Label Grid.Row="0" Grid.Column="0" Content="Name"/>
                    <TextBox Grid.Row="0" Grid.Column="1" Text="{Binding Path=AddUserName}" 
                             IsReadOnly="True" Height="22"/>
                    <Image Grid.Row="0" Grid.Column="2" Grid.RowSpan="4" 
                             MinHeight="100" MinWidth="100"
                             HorizontalAlignment="Left" VerticalAlignment="Center" 
                             Source="{Binding ChangeDetailsImageSource}" />

                    <Label Grid.Row="1" Grid.Column="0" Content="E-Mail"/>
                    <TextBox Grid.Row="1" Grid.Column="1" Text="{Binding Path=AddUserEmail}" Height="22"/>

                    <Label Grid.Row="2" Grid.Column="0" Content="Password"/>
                    <PasswordBox PasswordChar="*" Grid.Row="2" Grid.Column="1"  Height="22"
                            MinWidth="150"
                            vmu:PasswordHelper.Attach="True" 
                            vmu:PasswordHelper.Password="{Binding Path=AddUserPassword, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                            HorizontalAlignment="Stretch"  VerticalAlignment="Center">
                    </PasswordBox>

                    <Label Grid.Row="3" Grid.Column="0" Content="Note"/>
                    <TextBox Grid.Row="3" Grid.Column="1" Text="{Binding Path=AddUserNote}" Height="22"/>

                    <Button Grid.Row="4" Grid.Column="2" Height="22" Margin="2"
                            Content="Select Image" Command="{Binding Path=AddUserImageCommand}" />

                    <Button Grid.Row="6" Grid.Column="0" Height="22" Margin="2"
                            Content="Add User" Command="{Binding Path=AddUserCommand}" />
#endif
    }
}
