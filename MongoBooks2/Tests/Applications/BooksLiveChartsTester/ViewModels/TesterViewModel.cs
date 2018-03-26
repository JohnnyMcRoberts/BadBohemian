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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Provider;
    using BooksDatabase.Implementations;
    using BooksUtilities.ViewModels;
    using BooksLiveCharts.Utilities;
    using BooksLiveCharts.ViewModels.PieCharts;
    using BooksLiveCharts.ViewModels.LineCharts;
    using BooksLiveCharts.ViewModels.ScatterCharts;
    using BooksLiveCharts.ViewModels.GeoMapCharts;
    using BooksLiveCharts.ViewModels.MultipleAxisLineCharts;

    /// <summary>
    /// The view model for a books live chart test application.
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
        /// The selected pie chart type.
        /// </summary>
        private PieChartType _selectedPieChart;

        /// <summary>
        /// The pie chart view model.
        /// </summary>
        private BasePieChartViewModel _basePieChart;

        /// <summary>
        /// The selected scatter chart type.
        /// </summary>
        private ScatterChartType _selectedScatterChart;

        /// <summary>
        /// The scatter chart view model.
        /// </summary>
        private BaseScatterChartViewModel _baseScatterChart;

        /// <summary>
        /// The selected line chart type.
        /// </summary>
        private LineChartType _selectedLineChart;

        /// <summary>
        /// The line chart view model.
        /// </summary>
        private BaseLineChartViewModel _baseLineChart;

        /// <summary>
        /// The selected geo map chart type.
        /// </summary>
        private GeoMapChartType _selectedGeoMapChart;

        /// <summary>
        /// The geo map chart view model.
        /// </summary>
        private BaseGeoMapChartViewModel _baseGeoMapChart;

        /// <summary>
        /// The multiple axis line chart view model.
        /// </summary>
        private BaseMultipleAxisLineChartViewModel _baseMultipleAxisLineChart;

        /// <summary>
        /// The selected multiple axis line chart type.
        /// </summary>
        private MultipleAxisLineChartType _selectedMultipleAxisLineChart;

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

        /// <summary>
        /// The update the selected line chart command.
        /// </summary>
        private ICommand _updateLineChartCommand;

        /// <summary>
        /// The update the selected geo map chart command.
        /// </summary>
        private ICommand _updateGeoMapChartCommand;

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
        /// Gets or sets the selected pie chart type.
        /// </summary>
        public PieChartType SelectedPieChartType
        {
            get
            {
                return _selectedPieChart;
            }

            set
            {
                if (value != _selectedPieChart)
                {
                    _selectedPieChart = value;
                    UpdatePieChartCommandAction();
                }
            }
        }

        /// <summary>
        /// Gets the pie chart types and titles.
        /// </summary>
        public Dictionary<PieChartType, string> PieChartTypesByTitle { get; private set; }

        /// <summary>
        /// Gets the pie chart.
        /// </summary>
        public BasePieChartViewModel BasePieChart => _basePieChart;

        /// <summary>
        /// Gets or sets the selected scatter chart type.
        /// </summary>
        public ScatterChartType SelectedScatterChartType
        {
            get
            {
                return _selectedScatterChart;
            }

            set
            {
                if (value != _selectedScatterChart)
                {
                    _selectedScatterChart = value;
                    UpdateScatterChartCommandAction();
                }
            }
        }

        /// <summary>
        /// Gets the scatter chart types and titles.
        /// </summary>
        public Dictionary<ScatterChartType, string> ScatterChartTypesByTitle { get; private set; }

        /// <summary>
        /// Gets the scatter chart.
        /// </summary>
        public BaseScatterChartViewModel BaseScatterChart => _baseScatterChart;

        /// <summary>
        /// Gets or sets the selected scatter chart type.
        /// </summary>
        public MultipleAxisLineChartType SelectedMultipleAxisLineChartType
        {
            get
            {
                return _selectedMultipleAxisLineChart;
            }

            set
            {
                if (value != _selectedMultipleAxisLineChart)
                {
                    _selectedMultipleAxisLineChart = value;
                    UpdateMultipleAxisLineChartCommandAction();
                }
            }
        }

        /// <summary>
        /// Gets the line chart types and titles.
        /// </summary>
        public Dictionary<LineChartType, string> MultipleAxisLineChartTypesByTitle { get; private set; }

        /// <summary>
        /// Gets the line chart.
        /// </summary>
        public BaseMultipleAxisLineChartViewModel BaseMultipleAxisLineChart => _baseMultipleAxisLineChart;

        /// <summary>
        /// Gets or sets the selected scatter chart type.
        /// </summary>
        public GeoMapChartType SelectedGeoMapChartType
        {
            get
            {
                return _selectedGeoMapChart;
            }

            set
            {
                if (value != _selectedGeoMapChart)
                {
                    _selectedGeoMapChart = value;
                    UpdateGeoMapChartCommandAction();
                }
            }
        }

        /// <summary>
        /// Gets the geo map chart types and titles.
        /// </summary>
        public Dictionary<GeoMapChartType, string> GeoMapChartTypesByTitle { get; private set; }

        /// <summary>
        /// Gets the geo map chart.
        /// </summary>
        public BaseGeoMapChartViewModel BaseGeoMapChart => _baseGeoMapChart;

        /// <summary>
        /// Gets or sets the selected scatter chart type.
        /// </summary>
        public LineChartType SelectedLineChartType
        {
            get
            {
                return _selectedLineChart;
            }

            set
            {
                if (value != _selectedLineChart)
                {
                    _selectedLineChart = value;
                    UpdateLineChartCommandAction();
                }
            }
        }

        /// <summary>
        /// Gets the line chart types and titles.
        /// </summary>
        public Dictionary<LineChartType, string> LineChartTypesByTitle { get; private set; }

        /// <summary>
        /// Gets the line chart.
        /// </summary>
        public BaseLineChartViewModel BaseLineChart => _baseLineChart;

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

        /// <summary>
        /// Gets the update line chart command.
        /// </summary>
        public ICommand UpdateLineChartCommand => _updateLineChartCommand ?? (_updateLineChartCommand = new CommandHandler(UpdateLineChartCommandAction, true));

        /// <summary>
        /// Gets the update geo map chart command.
        /// </summary>
        public ICommand UpdateGeoMapChartCommand => _updateGeoMapChartCommand ?? (_updateGeoMapChartCommand = new CommandHandler(UpdateGeoMapChartCommandAction, true));

        #endregion

        #region Utility Functions

        /// <summary>
        /// Gets the providers for the books and geography data.
        /// </summary>
        /// <param name="geographyProvider">The geography data provider on exit.</param>
        /// <param name="booksReadProvider">The books data provider on exit.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool GetProviders(out GeographyProvider geographyProvider, out BooksReadProvider booksReadProvider)
        {
            geographyProvider = null;
            booksReadProvider = null;

            if (!_booksReadDatabase.ReadFromDatabase)
            {
                _booksReadDatabase.ConnectToDatabase();
            }

            if (!_nationsReadDatabase.ReadFromDatabase)
            {
                _nationsReadDatabase.ConnectToDatabase();
            }

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
        /// Sets up the Pie chart selection types.
        /// </summary>
        private void SetupPieChartTypesByTitle()
        {
            PieChartTypesByTitle = new Dictionary<PieChartType, string>();
            foreach (PieChartType selection in Enum.GetValues(typeof(PieChartType)))
            {
                string title = selection.GetTitle();
                PieChartTypesByTitle.Add(selection, title);
            }
        }

        /// <summary>
        /// Sets up the Scatter chart selection types.
        /// </summary>
        private void SetupScatterChartTypesByTitle()
        {
            ScatterChartTypesByTitle = new Dictionary<ScatterChartType, string>();
            foreach (ScatterChartType selection in Enum.GetValues(typeof(ScatterChartType)))
            {
                string title = selection.GetTitle();
                ScatterChartTypesByTitle.Add(selection, title);
            }
        }

        /// <summary>
        /// Sets up the Line chart selection types.
        /// </summary>
        private void SetupLineChartTypesByTitle()
        {
            LineChartTypesByTitle = new Dictionary<LineChartType, string>();
            foreach (LineChartType selection in Enum.GetValues(typeof(LineChartType)))
            {
                string title = selection.GetTitle();
                LineChartTypesByTitle.Add(selection, title);
            }
        }

        /// <summary>
        /// Sets up the Geo Map chart selection types.
        /// </summary>
        private void SetupGeoMapChartTypesByTitle()
        {
            GeoMapChartTypesByTitle = new Dictionary<GeoMapChartType, string>();
            foreach (GeoMapChartType selection in Enum.GetValues(typeof(GeoMapChartType)))
            {
                string title = selection.GetTitle();
                GeoMapChartTypesByTitle.Add(selection, title);
            }
        }

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
        /// The update the pie chart command action.
        /// </summary>
        private void UpdatePieChartCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                Type pieChartType = _selectedPieChart.GetGeneratorClass();
                object instance = Activator.CreateInstance(pieChartType);
                _basePieChart = (BasePieChartViewModel)instance;                
                _basePieChart.SetupPlot(geographyProvider, booksReadProvider);
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
                Type scatterChartType = _selectedScatterChart.GetGeneratorClass();
                object instance = Activator.CreateInstance(scatterChartType);
                _baseScatterChart = (BaseScatterChartViewModel)instance;
                _baseScatterChart.SetupPlot(geographyProvider, booksReadProvider);

                OnPropertyChanged(() => BaseScatterChart);
            }
        }

        /// <summary>
        /// The update average days per book plot command action.
        /// </summary>
        private void UpdateLineChartCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                Type lineChartType = _selectedLineChart.GetGeneratorClass();
                object instance = Activator.CreateInstance(lineChartType);
                _baseLineChart = (BaseLineChartViewModel)instance;
                _baseLineChart.SetupPlot(geographyProvider, booksReadProvider);

                OnPropertyChanged(() => BaseLineChart);
            }
        }
        
        /// <summary>
        /// The update the geo map chart command action.
        /// </summary>
        private void UpdateGeoMapChartCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                Type geoMapChartType = _selectedGeoMapChart.GetGeneratorClass();
                object instance = Activator.CreateInstance(geoMapChartType);
                _baseGeoMapChart = (BaseGeoMapChartViewModel)instance;
                _baseGeoMapChart.SetupPlot(geographyProvider, booksReadProvider);

                OnPropertyChanged(() => BaseGeoMapChart);
            }
        }

        /// <summary>
        /// The update average days per book plot command action.
        /// </summary>
        private void UpdateMultipleAxisLineChartCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                Type lineChartType = _selectedLineChart.GetGeneratorClass();
                object instance = Activator.CreateInstance(lineChartType);
                _baseMultipleAxisLineChart = (BaseMultipleAxisLineChartViewModel)instance;
                _baseMultipleAxisLineChart.SetupPlot(geographyProvider, booksReadProvider);

                OnPropertyChanged(() => BaseMultipleAxisLineChart);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TesterViewModel"/> class. 
        /// </summary>
        public TesterViewModel()
        {
            _booksReadFromDatabase = new ObservableCollection<BookRead>();
            _nationsReadFromDatabase = new ObservableCollection<Nation>();

            _booksReadDatabase = new BooksReadDatabase(DatabaseConnectionString);
            _nationsReadDatabase = new NationDatabase(DatabaseConnectionString);
            
            _basePieChart = new BasePieChartViewModel();
            SetupPieChartTypesByTitle();

            _baseScatterChart = new BaseScatterChartViewModel();
            SetupScatterChartTypesByTitle();

            _baseLineChart = new BaseLineChartViewModel();
            SetupLineChartTypesByTitle();

            _baseGeoMapChart = new BaseGeoMapChartViewModel();
            SetupGeoMapChartTypesByTitle();

            _baseMultipleAxisLineChart = new BaseMultipleAxisLineChartViewModel();
        }

        #endregion
    }
}
