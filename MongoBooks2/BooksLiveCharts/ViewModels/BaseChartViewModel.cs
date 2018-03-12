// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePieChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels
{
    using System;
    using System.Windows.Media;
    using BooksCore.Interfaces;
    using BooksUtilities.ViewModels;
    using LiveCharts;

    /// <summary>
    /// The base chart view model class.
    /// </summary>
    public abstract class BaseChartViewModel : BaseViewModel
    {
        /// <summary>
        /// The location for the legend shown in the chart.
        /// </summary>
        private LegendLocation _legendLocation;


        /// <summary>
        /// The location for the legend shown in the chart.
        /// </summary>
        private Brush _background;

        /// <summary>
        /// Gets the geography data for the plots.
        /// </summary>
        public IGeographyProvider GeographyProvider { get; private set; }

        /// <summary>
        /// Gets the books read data for the plots.
        /// </summary>
        public IBooksReadProvider BooksReadProvider { get; private set; }

        /// <summary>
        /// Gets the chart title.
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Gets the series shown in the chart.
        /// </summary>
        public SeriesCollection Series { get; protected set; }

        /// <summary>
        /// Gets or sets the point label function.
        /// </summary>
        public Func<ChartPoint, string> PointLabel { get; protected set; }

        /// <summary>
        /// Gets or sets the location of the legend or if off.
        /// </summary>
        public LegendLocation LegendLocation
        {
            get
            {
                return _legendLocation;
            }

            protected set
            {
                if (_legendLocation != value)
                {
                    _legendLocation = value;
                    OnPropertyChanged(() => LegendLocation);
                }
            }
        }

        public Brush Background
        {
            get
            {
                return _background;
            }

            protected set
            {
                _background = value;
                OnPropertyChanged(() => Background);
            }
        }

        /// <summary>
        /// Gets or sets the series data formatter.
        /// </summary>
        public Func<double, string> Formatter { get; protected set; }

        /// <summary>
        /// Gets or sets the collection of series for the chart.
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get
            {
                return Series;
            }

            protected set
            {
                Series = value;
                OnPropertyChanged(() => SeriesCollection);
            }
        }

        /// <summary>
        /// Sets up the pie chart series.
        /// </summary>
        protected abstract void SetupSeries();

        /// <summary>
        /// Sets up the providers then gets plot model to be displayed.
        /// </summary>
        /// <param name="geographyProvider">The geography data source.</param>
        /// <param name="booksReadProvider">The books data source.</param>
        /// <returns>The user view item model.</returns>
        public void SetupPlot(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            GeographyProvider = geographyProvider;
            BooksReadProvider = booksReadProvider;
            SetupSeries();
        }
    }
}
