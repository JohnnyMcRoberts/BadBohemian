// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TesterViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The view model for books live chart tester application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveChartsTester.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Provider;
    using BooksDatabase.Implementations;
    using BooksLiveCharts.ViewModels;
    using BooksLiveCharts.ViewModels.PieCharts;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The viewvmodel for a books helix chart test application.
    /// </summary>
    public class TesterViewModel : BaseViewModel
    {
        #region Constants

        /// <summary>
        /// The connection string for the database.
        /// </summary>
        public const string DatabaseConnectionString = "mongodb://localhost:27017";

        #endregion

        #region Private data

        /// <summary>
        /// The books read database.
        /// </summary>
        private readonly BooksReadDatabase _booksReadDatabase;

        /// <summary>
        /// The nations read database.
        /// </summary>
        private readonly NationDatabase _nationsReadDatabase;

        /// <summary>
        /// The books read from database.
        /// </summary>
        private ObservableCollection<BookRead> _booksReadFromDatabase;

        /// <summary>
        /// The nations read from database.
        /// </summary>
        private ObservableCollection<Nation> _nationsReadFromDatabase;

        /// <summary>
        /// The pie chart view model.
        /// </summary>
        private BasePieChartViewModel _basePieChart;

        /// <summary>
        /// The scatter chart view model.
        /// </summary>
        private BaseScatterChartViewModel _baseScatterChart;

        /// <summary>
        /// The selected plot type.
        /// </summary>
        //private PlotType _selectedPlot;

        /// <summary>
        /// The get books command.
        /// </summary>
        private ICommand _getBooksCommand;

        /// <summary>
        /// The get nations command.
        /// </summary>
        private ICommand _getNationsCommand;

        /// <summary>
        /// The update the selected pie chart command.
        /// </summary>
        private ICommand _updatePieChartCommand;

        /// <summary>
        /// The update the selected scatter chart command.
        /// </summary>
        private ICommand _updateScatterChartCommand;

        #endregion

        #region Public data

        /// <summary>
        /// Gets the books.
        /// </summary>
        public ObservableCollection<BookRead> Books => _booksReadFromDatabase;

        /// <summary>
        /// Gets the books.
        /// </summary>
        public ObservableCollection<Nation> Nations => _nationsReadFromDatabase;

        /// <summary>
        /// Gets the pie chart.
        /// </summary>
        public BasePieChartViewModel BasePieChart =>_basePieChart;

        /// <summary>
        /// Gets the scatter chart.
        /// </summary>
        public BaseScatterChartViewModel BaseScatterChart => _baseScatterChart;

        /// <summary>
        /// Gets the plot types and titles.
        /// </summary>
        //public Dictionary<PlotType, string> PlotTypesByTitle { get; private set; }

        //public PlotType SelectedPlot
        //{
        //    get { return _selectedPlot; }
        //    set
        //    {
        //        if (value != _selectedPlot)
        //        {
        //            _selectedPlot = value;
        //            _oxyPlotChart = new OxyPlotViewModel(_selectedPlot);
        //            UpdateOxyPlotChartCommandAction();
        //        }
        //    }
        //}

        /// <summary>
        /// Gets the get books from database command.
        /// </summary>
        public ICommand GetBooksCommand => _getBooksCommand ?? (_getBooksCommand = new CommandHandler(GetBooksCommandAction, true));

        /// <summary>
        /// Gets the get nations from database command.
        /// </summary>
        public ICommand GetNationsCommand => _getNationsCommand ?? (_getNationsCommand = new CommandHandler(GetNationsCommandAction, true));

        /// <summary>
        /// Gets the update pie chart command.
        /// </summary>
        public ICommand UpdatePieChartCommand => _updatePieChartCommand ?? (_updatePieChartCommand = new CommandHandler(UpdatePieChartCommandAction, true));

        /// <summary>
        /// Gets the update scatter chart command.
        /// </summary>
        public ICommand UpdateScatterChartCommand => _updateScatterChartCommand ?? (_updateScatterChartCommand = new CommandHandler(UpdateScatterChartCommandAction, true));

        #endregion

        #region Utility Functions

        private bool GetProviders(out GeographyProvider geographyProvider, out BooksReadProvider booksReadProvider)
        {
            geographyProvider = null;
            booksReadProvider = null;

            if (!_booksReadDatabase.ReadFromDatabase)
                _booksReadDatabase.ConnectToDatabase();

            if (!_nationsReadDatabase.ReadFromDatabase)
                _nationsReadDatabase.ConnectToDatabase();

            if (_booksReadDatabase.ReadFromDatabase && _nationsReadDatabase.ReadFromDatabase)
            {
                // Setup the providers.
                geographyProvider = new GeographyProvider();
                geographyProvider.Setup(_nationsReadDatabase.LoadedItems);

                booksReadProvider = new BooksReadProvider();
                booksReadProvider.Setup(_booksReadDatabase.LoadedItems, geographyProvider);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets up the chart selection types.
        /// </summary>
        //private void SetupPlotTypesByTitle()
        //{
        //    PlotTypesByTitle = new Dictionary<PlotType, string>();
        //    foreach (PlotType selection in Enum.GetValues(typeof(PlotType)))
        //    {
        //        string title = selection.GetTitle();
        //        PlotTypesByTitle.Add(selection, title);
        //    }
        //}


        #endregion

        #region Command handlers

        /// <summary>
        /// The get books from database command action.
        /// </summary>
        private void GetBooksCommandAction()
        {
            _booksReadDatabase.ConnectToDatabase();
            if (_booksReadDatabase.ReadFromDatabase)
            {
                _booksReadFromDatabase = _booksReadDatabase.LoadedItems;
                OnPropertyChanged(() => Books);
            }
        }

        /// <summary>
        /// The get nations from database command action.
        /// </summary>
        private void GetNationsCommandAction()
        {
            _nationsReadDatabase.ConnectToDatabase();
            if (_nationsReadDatabase.ReadFromDatabase)
            {
                _nationsReadFromDatabase = _nationsReadDatabase.LoadedItems;
                OnPropertyChanged(() => Nations);
            }
        }

        /// <summary>
        /// The update average days per book plot command action.
        /// </summary>
        private void UpdatePieChartCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                //_oxyPlotChart.Update(geographyProvider, booksReadProvider);
                CurrentBooksReadByCountryPieChartViewModel countries = new CurrentBooksReadByCountryPieChartViewModel();
                countries.SetupPlot(geographyProvider, booksReadProvider);
                _basePieChart = countries;

                OnPropertyChanged(() => BasePieChart);
            }
        }

        /// <summary>
        /// The update average days per book plot command action.
        /// </summary>
        private void UpdateScatterChartCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                //_oxyPlotChart.Update(geographyProvider, booksReadProvider);

                OnPropertyChanged(() => BaseScatterChart);
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class. 
        /// </summary>
        public TesterViewModel()
        {
            _booksReadFromDatabase = new ObservableCollection<BookRead>();
            _nationsReadFromDatabase = new ObservableCollection<Nation>();

            _booksReadDatabase = new BooksReadDatabase(DatabaseConnectionString);
            _nationsReadDatabase = new NationDatabase(DatabaseConnectionString);

            //_selectedPlot = PlotType.AverageDaysPerBook;
            _basePieChart = new BasePieChartViewModel();
            _baseScatterChart = new BaseScatterChartViewModel();

            //_oxyPlotChart = new OxyPlotViewModel(_selectedPlot);

            //SetupPlotTypesByTitle();
        }

        #endregion
    }
}
