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
    using BooksUtilities.ViewModels;
    using LiveCharts;

    /// <summary>
    /// The base chart view model class.
    /// </summary>
    public abstract class BaseChartViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the location for the legend shown in the chart.
        /// </summary>
        private LegendLocation _legendLocation;

        /// <summary>
        /// Gets the series shown in the chart.
        /// </summary>
        public SeriesCollection Series { get; protected set; }

        /// <summary>
        /// Gets or sets the point label function.
        /// </summary>
        public Func<ChartPoint, string> PointLabel { get; protected set; }

        /// <summary>
        /// Gets or sets the collection of series shown in the chart.
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
    }
}
