// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentagePagesReadByCountryLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The percentage pages read by country with time line chart view model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
{
    using System;
    using LiveCharts;

    /// <summary>
    /// The percentage pages read by country with time line chart view model class.
    /// </summary>
    public sealed class PercentagePagesReadByCountryLineChartViewModel : BasePercentageLineChartViewModel
    {
        /// <summary>
        /// Sets up the line chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            SetupSeries(isBooks: false, isCountries: true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentagePagesReadByCountryLineChartViewModel"/> class.
        /// </summary>
        public PercentagePagesReadByCountryLineChartViewModel()
        {
            Title = "Percentage Pages Read by Country With Time";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Top;
            SetupSeries();
        }
    }
}
