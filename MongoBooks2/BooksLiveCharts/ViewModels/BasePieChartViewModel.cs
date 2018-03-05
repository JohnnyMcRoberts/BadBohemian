// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePieChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base pie-chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels
{
    using System;
    using BooksUtilities.ViewModels;
    using LiveCharts;
    using LiveCharts.Defaults;
    using LiveCharts.Wpf;

    /// <summary>
    /// The base pie chart view model class.
    /// </summary>
    public class BasePieChartViewModel : BaseChartViewModel
    {
        /// <summary>
        /// Sets up the pie chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Series = new SeriesCollection
            {
                CreatePieSeries("Test 1", 123.4),
                CreatePieSeries("Test 2", 345.12),
                CreatePieSeries("Test 3", 765.43)
            };

            SeriesCollection = Series;
        }

        /// <summary>
        /// Creates a new pie series with a given name and value.
        /// </summary>
        /// <param name="title">The pie series title.</param>
        /// <param name="doubleValue">The value for the series.</param>
        /// <returns>The newly created pie series.</returns>
        public PieSeries CreatePieSeries(string title, double doubleValue)
        {
            return new PieSeries
            {
                Title = title,
                Values = new ChartValues<ObservableValue>
                {
                    new ObservableValue(doubleValue)
                },
                DataLabels = true,
                LabelPoint = PointLabel
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePieChartViewModel"/> class.
        /// </summary>
        public BasePieChartViewModel()
        {
            PointLabel = chartPoint => $"{chartPoint.Y:G3}";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
