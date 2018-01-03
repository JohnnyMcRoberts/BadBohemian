// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartSelectionViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The chart selection view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.PlotGenerators;

    public class ChartSelectionViewModel : BaseViewModel
    {
        public class ChartPair
        {
            public string Name { get; set; }
            public OxyPlotPair Plot { get; set; }
        }
        
        #region Private Data

        private MainWindow _mainWindow;
        private log4net.ILog _log;
        private MainBooksModel _mainModel;
        private MainViewModel _parent;
        private ChartPair _selected;

        #endregion

        #region Public properties

        public List<ChartPair> AvailableCharts { get; private set; }

        public ChartPair SelectedChart
        {
            get { return _selected; }
            set { _selected = value; SelectedPlot = value.Plot; OnPropertyChanged(() => SelectedChart); OnPropertyChanged(() => SelectedPlot); }
        }

        public OxyPlotPair SelectedPlot { get; private set; }


        public OxyPlotPair PlotOverallBookAndPageTallies { get; private set; }

        public OxyPlotPair PlotDaysPerBook { get; private set; }

        public OxyPlotPair PlotPageRate { get; private set; }

        public OxyPlotPair PlotPagesPerBook { get; private set; }

#if Works

        public OxyPlotPair PlotBooksInTranslation { get; private set; }

        public OxyPlotPair PlotDaysPerBookWithTime { get; private set; }

        public OxyPlotPair PlotPagesPerDayWithTime { get; private set; }

        public OxyPlotPair PlotAverageDaysPerBook { get; private set; }

        public OxyPlotPair PlotPercentageBooksReadByLanguage { get; private set; }

        public OxyPlotPair PlotTotalBooksReadByLanguage { get; private set; }

        public OxyPlotPair PlotPercentagePagesReadByLanguage { get; private set; }

        public OxyPlotPair PlotTotalPagesReadByLanguage { get; private set; }

        public OxyPlotPair PlotPercentageBooksReadByCountry { get; private set; }

        public OxyPlotPair PlotTotalBooksReadByCountry { get; private set; }

        public OxyPlotPair PlotPercentagePagesReadByCountry { get; private set; }

        public OxyPlotPair PlotTotalPagesReadByCountry { get; private set; }

        public OxyPlotPair PlotBooksAndPagesThisYear { get; private set; }

        public OxyPlotPair PlotCurrentPagesReadByCountry { get; private set; }

        public OxyPlotPair PlotCurrentBooksReadByCountry { get; private set; }

        public OxyPlotPair PlotBooksAndPagesLastTen { get; private set; }

        public OxyPlotPair PlotBooksAndPagesLastTenTranslation { get; private set; }

        public OxyPlotPair PlotCountryLocationsBooksAndPages { get; private set; }

        public OxyPlotPair PlotCountryLocationsBooksRead { get; private set; }

        public OxyPlotPair PlotWorldCountriesMap { get; private set; }

        public OxyPlotPair PlotWorldCountriesMapBooksRead { get; private set; }

        public OxyPlotPair PlotWorldCountriesMapPagesRead { get; private set; }

        public OxyPlotPair PlotWorldCountriesMapWithBooksRead { get; private set; }

        public OxyPlotPair PlotLatitudeWithTime { get; private set; }

        public OxyPlotPair PlotLongitudeWithTime { get; private set; }

        public OxyPlotPair PlotWorldCountriesMapLastTenLatLong { get; private set; }

        public OxyPlotPair PlotTalliesPerCalendarYear { get; private set; }

        public OxyPlotPair PlotMonthlyBookTalliesByCalendarYear { get; private set; }

        public OxyPlotPair PlotMonthlyPageTalliesByCalendarYear { get; private set; }

        public OxyPlotPair PlotMonthlyBookAndPageTalliesByCalendarYear { get; private set; }

    #endif

    #endregion

        #region Constructor

        public ChartSelectionViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;

            PlotOverallBookAndPageTallies =
                new OxyPlotPair(new OverallBookAndPageTalliesPlotGenerator(), "OverallBookAndPage");
            PlotDaysPerBook =
                new OxyPlotPair(new DaysPerBookPlotGenerator(), "DaysPerBook");
            PlotPageRate =
                new OxyPlotPair(new PageRatePlotGenerator(), "PageRate");
            PlotPagesPerBook =
                new OxyPlotPair(new PagesPerBookPlotGenerator(), "PagesPerBook");

            AvailableCharts = new List<ChartPair>()
            {
                new ChartPair {Name = "Overall Books and Pages", Plot = PlotOverallBookAndPageTallies },
                new ChartPair {Name = "Days Per Book", Plot = PlotDaysPerBook },
                new ChartPair {Name = "Page Rate", Plot = PlotPageRate },
                new ChartPair {Name = "Pages per Book", Plot = PlotPagesPerBook },
            };

            SelectedChart = AvailableCharts.First();

#if Works

            PlotBooksInTranslation =
                new OxyPlotPair(new BooksInTranslationPlotGenerator(), "BooksInTranslation");
            PlotDaysPerBookWithTime =
                new OxyPlotPair(new DaysPerBookWithTimePlotGenerator(), "DaysPerBook");
            PlotPagesPerDayWithTime =
                new OxyPlotPair(new PagesPerDayWithTimePlotGenerator(), "PagesPerDayWithTime");
            PlotAverageDaysPerBook =
                new OxyPlotPair(new AverageDaysPerBookPlotGenerator(), "AverageDaysPerBook");
            PlotPercentageBooksReadByLanguage =
                new OxyPlotPair(new PercentageBooksReadByLanguagePlotGenerator(), "PercentageBooksReadByLanguage");
            PlotTotalBooksReadByLanguage =
                new OxyPlotPair(new TotalBooksReadByLanguagePlotGenerator(), "TotalBooksReadByLanguage");
            PlotPercentagePagesReadByLanguage =
                new OxyPlotPair(new PercentagePagesReadByLanguagePlotGenerator(), "PercentagePagesReadByLanguage");
            PlotTotalPagesReadByLanguage =
                new OxyPlotPair(new TotalPagesReadByLanguagePlotGenerator(), "TotalPagesReadByLanguage");
            PlotPercentageBooksReadByCountry =
                new OxyPlotPair(new PercentageBooksReadByCountryPlotGenerator(), "PercentageBooksReadByCountry");
            PlotTotalBooksReadByCountry =
                new OxyPlotPair(new TotalBooksReadByCountryPlotGenerator(), "TotalBooksReadByCountry");
            PlotPercentagePagesReadByCountry =
                new OxyPlotPair(new PercentagePagesReadByCountryPlotGenerator(), "PercentagePagesReadByCountry");
            PlotTotalPagesReadByCountry =
                new OxyPlotPair(new TotalPagesReadByCountryPlotGenerator(), "TotalPagesReadByCountry");
            PlotTotalPagesReadByLanguage =
                new OxyPlotPair(new TotalPagesReadByLanguagePlotGenerator(), "TotalPagesReadByLanguage");
            PlotBooksAndPagesThisYear =
                new OxyPlotPair(new BooksAndPagesThisYearPlotGenerator(), "BooksAndPagesThisYear");
            PlotCurrentPagesReadByCountry =
                new OxyPlotPair(new CurrentPagesReadByCountryPlotGenerator(), "CurrentPagesReadByCountry");
            PlotCurrentBooksReadByCountry =
                new OxyPlotPair(new CurrentBooksReadByCountryPlotGenerator(), "CurrentBooksReadByCountry");
            PlotBooksAndPagesLastTen =
                new OxyPlotPair(new BooksAndPagesLastTenPlotGenerator(), "BooksAndPagesLastTen", true);
            PlotBooksAndPagesLastTenTranslation =
                new OxyPlotPair(new BooksAndPagesLastTenTranslationPlotGenerator(), "BooksAndPagesLastTenTranslation", true);
            PlotCountryLocationsBooksAndPages =
                new OxyPlotPair(new CountryLocationsBooksAndPagesPlotGenerator(), "CountryLocationsBooksAndPages", true);
            PlotCountryLocationsBooksRead =
                new OxyPlotPair(new CountryLocationsBooksReadPlotGenerator(), "CountryLocationsBooksRead", true);
            PlotLatitudeWithTime =
                new OxyPlotPair(new LatitudeWithTimePlotGenerator(), "LatitudeWithTime");
            PlotLongitudeWithTime =
                new OxyPlotPair(new LongitudeWithTimePlotGenerator(), "LongitudeWithTime");
            PlotTalliesPerCalendarYear =
                new OxyPlotPair(new TalliesPerCalendarYearPlotGenerator(), "TalliesPerCalendar");
            PlotMonthlyBookTalliesByCalendarYear =
                new OxyPlotPair(new MonthlyBookTalliesByCalendarYearPlotGenerator(
                    MonthlyBookTalliesByCalendarYearPlotGenerator.ChartType.BooksAsColumns),
                    "MonthlyBookTalliesByCalendarYear");
            PlotMonthlyPageTalliesByCalendarYear =
                new OxyPlotPair(new MonthlyBookTalliesByCalendarYearPlotGenerator(
                    MonthlyBookTalliesByCalendarYearPlotGenerator.ChartType.PagesAsColumns),
                    "MonthlyPageTalliesByCalendarYear");
            PlotMonthlyBookAndPageTalliesByCalendarYear =
                new OxyPlotPair(new MonthlyBookTalliesByCalendarYearPlotGenerator(
                    MonthlyBookTalliesByCalendarYearPlotGenerator.ChartType.BothAsLines),
                    "MonthlyBookAndPageTalliesByCalendarYear");

            PlotWorldCountriesMap =
                new OxyPlotPair(new WorldCountriesMapPlotGenerator(), "WorldCountriesMap", true);
            PlotWorldCountriesMapBooksRead =
                new OxyPlotPair(new WorldCountriesMapBooksReadPlotGenerator(), "WorldCountriesMapBooksRead", true);
            PlotWorldCountriesMapPagesRead =
                new OxyPlotPair(new WorldCountriesMapPagesReadPlotGenerator(), "WorldCountriesMapPagesRead", true);
            PlotWorldCountriesMapWithBooksRead =
                new OxyPlotPair(new WorldCountriesMapWithBooksReadPlotGenerator(), "WorldCountriesMapWithBooksRead", true);
            PlotWorldCountriesMapLastTenLatLong =
                new OxyPlotPair(new WorldCountriesMapLastTenLatLongPlotGenerator(), "WorldCountriesMapLastTenLatLong", true);
#endif
        }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            // if no data don't waste time trying to create plots
            if (_mainModel == null || _mainModel.BooksRead == null || _mainModel.BooksRead.Count == 0)
                return;

            PlotOverallBookAndPageTallies.UpdateData(_mainModel);
            PlotDaysPerBook.UpdateData(_mainModel);
            PlotPageRate.UpdateData(_mainModel);
            PlotPagesPerBook.UpdateData(_mainModel);

            #if Works

            PlotBooksInTranslation.UpdateData(_mainModel);
            PlotDaysPerBookWithTime.UpdateData(_mainModel);
            PlotPagesPerDayWithTime.UpdateData(_mainModel);
            PlotAverageDaysPerBook.UpdateData(_mainModel);
            PlotPercentageBooksReadByLanguage.UpdateData(_mainModel);
            PlotTotalBooksReadByLanguage.UpdateData(_mainModel);
            PlotPercentagePagesReadByLanguage.UpdateData(_mainModel);
            PlotTotalPagesReadByLanguage.UpdateData(_mainModel);
            PlotPercentageBooksReadByCountry.UpdateData(_mainModel);
            PlotTotalBooksReadByCountry.UpdateData(_mainModel);
            PlotPercentagePagesReadByCountry.UpdateData(_mainModel);
            PlotTotalPagesReadByCountry.UpdateData(_mainModel);
            PlotBooksAndPagesThisYear.UpdateData(_mainModel);
            PlotCurrentPagesReadByCountry.UpdateData(_mainModel);
            PlotCurrentBooksReadByCountry.UpdateData(_mainModel);
            PlotBooksAndPagesLastTen.UpdateData(_mainModel);
            PlotBooksAndPagesLastTenTranslation.UpdateData(_mainModel);
            PlotCountryLocationsBooksAndPages.UpdateData(_mainModel);
            PlotCountryLocationsBooksRead.UpdateData(_mainModel);
            PlotLatitudeWithTime.UpdateData(_mainModel);
            PlotLongitudeWithTime.UpdateData(_mainModel);
            PlotTalliesPerCalendarYear.UpdateData(_mainModel);
            PlotMonthlyBookTalliesByCalendarYear.UpdateData(_mainModel);
            PlotMonthlyPageTalliesByCalendarYear.UpdateData(_mainModel);
            PlotMonthlyBookAndPageTalliesByCalendarYear.UpdateData(_mainModel);

            if (_mainModel.CountryGeographies != null)
            {
                PlotWorldCountriesMap.UpdateData(_mainModel);
                PlotWorldCountriesMapBooksRead.UpdateData(_mainModel);
                PlotWorldCountriesMapPagesRead.UpdateData(_mainModel);
                PlotWorldCountriesMapWithBooksRead.UpdateData(_mainModel);
                PlotWorldCountriesMapLastTenLatLong.UpdateData(_mainModel);
            }

            #endif

            OnPropertyChanged("");
        }

        #endregion

    }
}
