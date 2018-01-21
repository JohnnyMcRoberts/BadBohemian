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
    using MongoDbBooks.Models.Database;

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

        #region Update User

        /// <summary>
        /// The name of the new user to add.
        /// </summary>
        private string _updateUserName;

        /// <summary>
        /// The e-mail of the new user to add.
        /// </summary>
        private string _updateUserEmail;

        /// <summary>
        /// The password of the new user to add.
        /// </summary>
        private string _updateUserPassword;

        /// <summary>
        /// The description of the new user to add.
        /// </summary>
        private string _updateUserDescription;

        /// <summary>
        /// The image of the new user to add.
        /// </summary>
        private string _updateUserImage;

        /// <summary>
        /// The image of the new user to add.
        /// </summary>
        private Models.Database.User _updateUser;

        /// <summary>
        /// The add user command.
        /// </summary>
        private ICommand _updateUserCommand;

        /// <summary>
        /// The select image for add user command.
        /// </summary>
        private ICommand _updateUserSelectUserCommand;

        /// <summary>
        /// The update user name is set command.
        /// </summary>
        private ICommand _updateUserNameInputCommand;

        #endregion

        #region Current Users

        #endregion

        /// <summary>
        /// The _user name.
        /// </summary>
        private string _userName;
        
        /// <summary>
        /// The books read from email.
        /// </summary>
        private ObservableCollection<IBookRead> _booksReadFromEmail;
        
        /// <summary>
        /// The connect to mailbox command.
        /// </summary>
        private ICommand _manageUsersCommand;
        
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
        public Uri AddUserImageSource => !string.IsNullOrEmpty(_addUserImage) ? new Uri(_addUserImage) : new Uri("http://www.ucl.ac.uk/pals/research/linguistics/people/images/blank-300px");

        #endregion

        #region Update User

        /// <summary>
        /// Gets the user name.
        /// </summary>
        public string UpdateUserName
        {
            get
            {
                return _updateUserName;
            }

            private set
            {
                _updateUserName = value;
                OnPropertyChanged(() => UpdateUserName);
                OnPropertyChanged(() => CanUpdateUser);
                UpdateUserNameInputCommandAction();
            }
        }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        public string UpdateUserPassword
        {
            get
            {
                return _updateUserPassword;
            }

            private set
            {
                _updateUserPassword = value;
                OnPropertyChanged(() => UpdateUserPassword);
                OnPropertyChanged(() => CanUpdateUser);
            }
        }

        /// <summary>
        /// Gets the user email.
        /// </summary>
        public string UpdateUserEmail
        {
            get
            {
                return _updateUserEmail;
            }

            private set
            {
                _updateUserEmail = value;
                OnPropertyChanged(() => UpdateUserEmail);
                OnPropertyChanged(() => CanUpdateUser);
            }
        }

        /// <summary>
        /// Gets the user description.
        /// </summary>
        public string UpdateUserDescription
        {
            get
            {
                return _updateUserDescription;
            }

            private set
            {
                _updateUserDescription = value;
                OnPropertyChanged(() => UpdateUserDescription);
            }
        }

        /// <summary>
        /// Gets if can have enough data entered to update a new user.
        /// </summary>
        public bool UpdateUserIsSet => _updateUser != null;

        /// <summary>
        /// Gets if can have enough data entered to update a new user.
        /// </summary>
        public bool CanUpdateUser =>
            IsValidName(_updateUserName) &&
            IsValidPassword(_updateUserPassword) &&
            IsValidEmail(_updateUserEmail);

        /// <summary>
        /// Gets the add user image URI ready to be displayed.
        /// </summary>
        public Uri UpdateUserImageSource => !string.IsNullOrEmpty(_updateUserImage) ? new Uri(_updateUserImage) : new Uri("http://www.ucl.ac.uk/pals/research/linguistics/people/images/blank-300px");

        #endregion

        #region Current Users

        /// <summary>
        /// Gets the add user image URI ready to be displayed.
        /// </summary>
        public ObservableCollection<Models.Database.User> Users => _mainModel.Users;

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

        #region Update User

        public ICommand UpdateUserNameInputCommand => _updateUserNameInputCommand ??
                                              (_updateUserNameInputCommand =
                                                  new CommandHandler(UpdateUserNameInputCommandAction, true));


        /// <summary>
        /// Gets the add user select image command.
        /// </summary>
        public ICommand UpdateUserSelectImageCommand => _updateUserSelectUserCommand ??
                                              (_updateUserSelectUserCommand =
                                                  new CommandHandler(UpdateUserSelectImageCommandAction, true));

        /// <summary>
        /// Gets the add user command.
        /// </summary>
        public ICommand UpdateUserCommand => _updateUserCommand ??
                                              (_updateUserCommand =
                                                  new CommandHandler(UpdateUserCommandAction, true));
        #endregion

        #region Current Users

        #endregion

        #endregion

        #region Command Actions

        /// <summary>
        /// The manage users command action.
        /// </summary>
        public void ManageUsersCommandAction()
        {
            ResetParameters();
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

        #endregion

        #region Update User

        /// <summary>
        /// The update user name input command action.
        /// </summary>
        public void UpdateUserNameInputCommandAction()
        {
            _updateUser = null;
            foreach (var user in _mainModel.Users)
            {
                if (_updateUserName == user.Name)
                {
                    _updateUser = user;
                    break;
                }
            }

            SetUpdateUser(_updateUser);
        }

        private void SetUpdateUser(User updateUser)
        {
            UpdateUserEmail = updateUser == null ? string.Empty : updateUser.Email;
            UpdateUserDescription = updateUser == null ? string.Empty : updateUser.Description;
            _updateUserImage = updateUser == null ? string.Empty : updateUser.ImageUri;
            OnPropertyChanged(() => UpdateUserImageSource);
            OnPropertyChanged(() => UpdateUserIsSet);
        }

        /// <summary>
        /// The add user select image command action.
        /// </summary>
        public void UpdateUserSelectImageCommandAction()
        {
            string searchTerm = GetImageSearchTerm(_updateUserName);
            ImageSelectionViewModel selectionViewModel =
                new ImageSelectionViewModel(_log, _updateUserName, searchTerm);

            ImageSelectionWindow imageSelectDialog =
                new ImageSelectionWindow { DataContext = selectionViewModel };
            bool? success = imageSelectDialog.ShowDialog();
            if (success.HasValue && success.Value)
            {
                _updateUserImage = selectionViewModel.SelectedImageAddress;
                OnPropertyChanged(() => UpdateUserImageSource);
            }
        }

        /// <summary>
        /// The add user command action.
        /// </summary>
        public void UpdateUserCommandAction()
        {
            User updateUser = new User
            {
                Id = _updateUser.Id,
                Name = _updateUserName,
                Description = _updateUserDescription,
                Email = _updateUserEmail,
                ImageUri = _updateUserImage,
                DateAdded = _updateUser.DateAdded,
                PasswordHash =
                    string.IsNullOrEmpty(_addUserPassword) ? _updateUser.PasswordHash : MD5Hash(_addUserPassword)
            };

            _mainModel.UserDatabase.UpdateDatabaseItem(updateUser);
            _mainModel.UserDatabase.ConnectToDatabase();

            MessageBox.Show("User Updated ok ");            
        }

        #endregion

        #region Current Users

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


        private void ResetParameters()
        {
            AddUserName = string.Empty;
            AddUserEmail = string.Empty;
            AddUserPassword = string.Empty;
            AddUserDescription = string.Empty;
            _addUserPassword = string.Empty;

            UpdateUserName = string.Empty;
            UpdateUserDescription = string.Empty;
            UpdateUserPassword = string.Empty;
            UpdateUserEmail = string.Empty;
            _updateUserImage = string.Empty;

            _updateUser = null;
            OnPropertyChanged(() => CanAddUser);
            OnPropertyChanged(() => CanUpdateUser);
            OnPropertyChanged(() => Users);
            
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
        
    }
}
