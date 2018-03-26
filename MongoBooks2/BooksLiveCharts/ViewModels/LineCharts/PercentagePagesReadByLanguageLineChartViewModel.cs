// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentagePagesReadByLanguageLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The percentage pages read by language with time line chart view model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
{
    using System;
    using LiveCharts;

    /// <summary>
    /// The percentage pages read by language with time line chart view model class.
    /// </summary>
    public sealed class PercentagePagesReadByLanguageLineChartViewModel : BasePercentageLineChartViewModel
    {
        /// <summary>
        /// Sets up the line chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            SetupSeries(isBooks: false, isCountries: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentagePagesReadByLanguageLineChartViewModel"/> class.
        /// </summary>
        public PercentagePagesReadByLanguageLineChartViewModel()
        {
            Title = "Percentage Pages Read by Language With Time";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Top;
            SetupSeries();
        }
    }
}