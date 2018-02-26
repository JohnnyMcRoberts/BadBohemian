// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyChartsTester.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Provider;
    using BooksDatabase.Implementations;
    using BooksOxyCharts.Utilities;
    using BooksOxyCharts.ViewModels;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The viewvmodel for a books helix chart test application.
    /// </summary>
    public class MainViewModel : BaseViewModel
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
        /// The pages by country diagram view model.
        /// </summary>
        private OxyPlotViewModel _oxyPlotChart;

        /// <summary>
        /// The selected plot type.
        /// </summary>
        private PlotTypes _selectedPlot;

        /// <summary>
        /// The get books command.
        /// </summary>
        private ICommand _getBooksCommand;

        /// <summary>
        /// The get nations command.
        /// </summary>
        private ICommand _getNationsCommand;

        /// <summary>
        /// The update the selected oxy plot chart command.
        /// </summary>
        private ICommand _updateOxyPlotChartCommand;

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
        /// Gets the pages by country diagram view model.
        /// </summary>
        public OxyPlotViewModel OxyPlotChart => _oxyPlotChart;

        /// <summary>
        /// Gets the plot types and titles.
        /// </summary>
        public Dictionary<PlotTypes, string> PlotTypesByTitle { get; private set; }

        public PlotTypes SelectedPlot
        {
            get { return _selectedPlot; }
            set
            {
                if (value != _selectedPlot)
                {
                    _selectedPlot = value;
                    _oxyPlotChart = new OxyPlotViewModel(_selectedPlot);
                    UpdateOxyPlotChartCommandAction();
                }
            }
        }

        /// <summary>
        /// Gets the get books from database command.
        /// </summary>
        public ICommand GetBooksCommand => _getBooksCommand ?? (_getBooksCommand = new CommandHandler(GetBooksCommandAction, true));

        /// <summary>
        /// Gets the get nations from database command.
        /// </summary>
        public ICommand GetNationsCommand => _getNationsCommand ?? (_getNationsCommand = new CommandHandler(GetNationsCommandAction, true));

        /// <summary>
        /// Gets the update pages by country diagram command.
        /// </summary>
        public ICommand UpdateOxyPlotChartCommand => _updateOxyPlotChartCommand ?? (_updateOxyPlotChartCommand = new CommandHandler(UpdateOxyPlotChartCommandAction, true));

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
        private void SetupPlotTypesByTitle()
        {
            PlotTypesByTitle = new Dictionary<PlotTypes, string>();
            foreach (PlotTypes selection in Enum.GetValues(typeof(PlotTypes)))
            {
                string title = selection.GetTitle();
                PlotTypesByTitle.Add(selection, title);
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
        /// The update average days per book plot command action.
        /// </summary>
        private void UpdateOxyPlotChartCommandAction()
        {
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;

            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                _oxyPlotChart.Update(geographyProvider, booksReadProvider);

                OnPropertyChanged(() => OxyPlotChart);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class. 
        /// </summary>
        public MainViewModel()
        {
            _booksReadFromDatabase = new ObservableCollection<BookRead>();
            _nationsReadFromDatabase = new ObservableCollection<Nation>();

            _booksReadDatabase = new BooksReadDatabase(DatabaseConnectionString);
            _nationsReadDatabase = new NationDatabase(DatabaseConnectionString);

            _selectedPlot = PlotTypes.AverageDaysPerBook;

            _oxyPlotChart = new OxyPlotViewModel(_selectedPlot);

            SetupPlotTypesByTitle();
        }

        #endregion
    }
}
