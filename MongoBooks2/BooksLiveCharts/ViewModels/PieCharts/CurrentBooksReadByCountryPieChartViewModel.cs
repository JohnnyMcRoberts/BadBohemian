// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentBooksReadByCountryPieChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base pie-chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.PieCharts
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using BooksCore.Utilities;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Definitions.Series;

    /// <summary>
    /// The books by country pie chart view model class.
    /// </summary>
    public sealed class CurrentBooksReadByCountryPieChartViewModel : BasePieChartViewModel
    {
        /// <summary>
        /// Sets up the pie chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            // If no books return the default.
            if (BooksReadProvider == null)
            {
                base.SetupSeries();
                return;
            }

            // Get the sorted books.
            List<KeyValuePair<string, int>> sortedCountryTotals =
                BookTotalsUtilities.SortedSortedBooksReadByCountryTotals(BooksReadProvider);

            // Set up the series per county.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<Color> colors = ColorUtilities.SetupStandardColourSet();
            for (int i = 0; i < sortedCountryTotals.Count; i++)
            {
                KeyValuePair<string, int> countryTotal = sortedCountryTotals[i];
                Color color = colors[i % colors.Count];
                seriesViews.Add(CreatePieSeries(countryTotal.Key, countryTotal.Value, color));
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentBooksReadByCountryPieChartViewModel"/> class.
        /// </summary>
        public CurrentBooksReadByCountryPieChartViewModel()
        {
            Title = "Current Books Read by Country";
            PointLabel = chartPoint => $"{chartPoint.Y:G3}";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
