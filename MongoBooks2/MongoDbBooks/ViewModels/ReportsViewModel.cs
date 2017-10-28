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
        private DateTime _selectedMonth;
        private DateTime _firstMonth;
        private DateTime _lastMonth;
        private TalliedMonth _selectedMonthTally;

        /// <summary>
        /// The select image for nation command.
        /// </summary>
        private ICommand _selectImageForNationCommand;

        #endregion

        #region Constants


        #endregion

        #region Public Data

        public string Title => "Reports";

        public ObservableCollection<TalliedMonth> TalliedMonths { get; set; }

        public DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                if (_selectedMonth.Year != value.Year || _selectedMonth.Month != value.Month)
                {
                    _selectedMonth = value;
                    _selectedMonthTally = GetSelectedMonthTally();
                    OnPropertyChanged(() => SelectedMonth);
                    OnPropertyChanged(() => SelectedMonthTally);
                    OnPropertyChanged(() => SelectedMonthBooksRead);
                }
            }
        }

        public DateTime FirstMonth
        {
            get { return _firstMonth; }
            set
            {
                _firstMonth = value;
                OnPropertyChanged(() => FirstMonth);
            }
        }

        public DateTime LastMonth
        {
            get { return _lastMonth; }
            set
            {
                _lastMonth = value;
                OnPropertyChanged(() => LastMonth);
            }
        }

        public TalliedMonth SelectedMonthTally => _selectedMonthTally;

        public List<BookRead> SelectedMonthBooksRead => _selectedMonthTally.BooksRead;

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

            FirstMonth = _mainModel.TalliedMonths.First().MonthDate;
            var lastMonth = _mainModel.TalliedMonths.Last().MonthDate;
            lastMonth = lastMonth.AddMonths(1);
            lastMonth = lastMonth.AddDays(-1);
            LastMonth = lastMonth;
            _selectedMonth = lastMonth.AddMonths(-1);
            TalliedMonths = new ObservableCollection<TalliedMonth>();
            _selectedMonthTally = GetSelectedMonthTally();
        }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            OnPropertyChanged("");
        }

        #endregion

        #region Utility Functions

        private TalliedMonth GetSelectedMonthTally()
        {
            var selected = _mainModel.TalliedMonths.First();
            foreach(var month in _mainModel.TalliedMonths)
            {
                if (month.MonthDate.Year == _selectedMonth.Year && month.MonthDate.Month == _selectedMonth.Month)
                {
                    selected = month;
                    break;
                }
            }
            TalliedMonths.Clear();
            TalliedMonths.Add(selected);
            OnPropertyChanged(() => TalliedMonths);
            return selected;
        }

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
