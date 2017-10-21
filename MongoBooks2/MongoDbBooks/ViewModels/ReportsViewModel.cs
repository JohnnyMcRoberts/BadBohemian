namespace MongoDbBooks.ViewModels
{
    using System.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Data;

    using MongoDbBooks.Models;
    using MongoDbBooks.Models.Geography;
    using MongoDbBooks.Models.Database;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.Views;

    public class ReportsViewModel : INotifyPropertyChanged
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

        private MainWindow _mainWindow;
        private log4net.ILog _log;
        private readonly MainBooksModel _mainModel;
        private MainViewModel _parent;

        /// <summary>
        /// The select image for nation command.
        /// </summary>
        private ICommand _selectImageForNationCommand;

        #endregion


        #region Constants


        #endregion

        #region Public Data

        public string Title => "Reports";

        #endregion
        #region Constructor

        public ReportsViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;
        }

        #endregion
        #region Public Methods

        public void UpdateData()
        {
            OnPropertyChanged("");
        }

        #endregion

        #region Utility Functions

        #endregion

        #region Commands

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand SelectImageForNationCommand => _selectImageForNationCommand ??
                                            (_selectImageForNationCommand =
                                             new RelayCommandHandler(SelectImageForNationCommandAction) { IsEnabled = true });
        #endregion

        #region Command Handlers

        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void SelectImageForNationCommandAction(object parameter)
        {
            Nation nation = parameter as Nation;
        }

        #endregion

    }
}
