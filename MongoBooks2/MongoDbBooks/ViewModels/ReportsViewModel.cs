namespace MongoDbBooks.ViewModels
{
    using System.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Documents;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.ViewModels.PlotGenerators;

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
        //private TalliedMonth _selectedMonthTally;

        /// <summary>
        /// The print command.
        /// </summary>
        private ICommand _printCommand;

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
                    _mainModel.SelectedMonthTally = GetSelectedMonthTally();
                    OnPropertyChanged(() => SelectedMonth);
                    OnPropertyChanged(() => SelectedMonthTally);
                    OnPropertyChanged(() => SelectedMonthBooksRead);
                    PlotCurrentMonthPagesReadByLanguage.UpdateData(_mainModel);
                    PlotCurrentMonthPagesReadByCountry.UpdateData(_mainModel);
                    PlotCurrentMonthDocumentPagesReadByLanguage.UpdateData(_mainModel);
                    PlotCurrentMonthDocumentPagesReadByCountry.UpdateData(_mainModel);
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

        public TalliedMonth SelectedMonthTally => _mainModel.SelectedMonthTally;

        public List<BookRead> SelectedMonthBooksRead => _mainModel.SelectedMonthTally.BooksRead;

        public OxyPlotPair PlotCurrentMonthPagesReadByLanguage { get; private set; }

        public OxyPlotPair PlotCurrentMonthPagesReadByCountry { get; private set; }

        public OxyPlotPair PlotCurrentMonthDocumentPagesReadByLanguage { get; private set; }

        public OxyPlotPair PlotCurrentMonthDocumentPagesReadByCountry { get; private set; }

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
            _mainModel.SelectedMonthTally = GetSelectedMonthTally();

            PlotCurrentMonthPagesReadByLanguage =
                new OxyPlotPair(new CurrentMonthPagesReadByLanguagePlotGenerator(), "CurrentMonthPagesReadByLanguage");
            PlotCurrentMonthPagesReadByCountry =
                new OxyPlotPair(new CurrentMonthPagesReadByCountryPlotGenerator(), "CurrentMonthPagesReadByCountry");

            PlotCurrentMonthDocumentPagesReadByLanguage =
                new OxyPlotPair(new CurrentMonthPagesReadByLanguagePlotGenerator(), "CurrentMonthPagesReadByLanguage");
            PlotCurrentMonthDocumentPagesReadByCountry =
                new OxyPlotPair(new CurrentMonthPagesReadByCountryPlotGenerator(), "CurrentMonthPagesReadByCountry");
        }

        // should think about a flow doc and then html ....
        // https://msdn.microsoft.com/en-us/library/aa972129.aspx

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            // if no data don't waste time trying to create plots
            if (_mainModel == null || _mainModel.BooksRead == null || _mainModel.BooksRead.Count == 0)
                return;

            PlotCurrentMonthPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthPagesReadByCountry.UpdateData(_mainModel);

            PlotCurrentMonthDocumentPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthDocumentPagesReadByCountry.UpdateData(_mainModel);
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
        public ICommand PrintCommand => _printCommand ??
                                            (_printCommand =
                                             new RelayCommandHandler(PrintCommandAction) { IsEnabled = true });
        #endregion

        #region Command Handlers

        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void PrintCommandAction(object parameter)
        {
            string path = "test.xps";

            System.IO.Packaging.Package package = System.IO.Packaging.Package.Open(path, System.IO.FileMode.Create);
            System.Windows.Xps.Packaging.XpsDocument document = new System.Windows.Xps.Packaging.XpsDocument(package);
            System.Windows.Xps.XpsDocumentWriter writer = System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(document);
            FixedDocument doc = new FixedDocument();

            FixedPage page1 = new FixedPage();
            PageContent page1Content = new PageContent();
            ((System.Windows.Markup.IAddChild)page1Content).AddChild(page1);

            // add the content
            //System.Windows.Controls.Button button = new System.Windows.Controls.Button() { Content = };
            //page1.Children.Add();

            doc.Pages.Add(page1Content);
            writer.Write(doc);
            document.Close();
            package.Close();

            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Flow Document Print Job");
            }

        }

        #endregion

    }
}
