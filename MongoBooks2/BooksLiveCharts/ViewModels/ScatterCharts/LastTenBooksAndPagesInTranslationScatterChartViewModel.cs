// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentBooksReadByCountryPieChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base pie-chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.ScatterCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksCore.Books;
    using BooksCore.Utilities;
    using BooksLiveCharts.ViewModels.PieCharts;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;

    /// <summary>
    /// The books and pages scatter chart view model class.
    /// </summary>
    public sealed class LastTenBooksAndPagesInTranslationScatterChartViewModel : BaseScatterChartViewModel
    {
        /// <summary>
        /// The rounding error epsilon for the rates.
        /// </summary>
        private const double RateEpsilon = 1e-5;

        /// <summary>
        /// The number of colors in range.
        /// </summary>
        private const int NumberOfColors = 11;

        /// <summary>
        /// Gets the scatter point color for a given translated percentage in a range.
        /// </summary>
        /// <param name="translated">The translated percentage.</param>
        /// <param name="colors">The colors set.</param>
        /// <returns>The interpolated color for the point.</returns>
        private static Color GetPointColorForTranslatedRate(double translated, List<Color> colors)
        {
            int colorIndex = (int)Math.Round(translated / 10.0d);
            if (Math.Abs(translated) < RateEpsilon)
            {
                colorIndex = 0;
            }
            else if (Math.Abs(translated - 100) < RateEpsilon)
            {
                colorIndex = NumberOfColors - 1;
            }

            return colors[colorIndex];
        }

        /// <summary>
        /// Sets up the scatter chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            // If no books return the default.
            if (BooksReadProvider == null)
            {
                base.SetupSeries();
                return;
            }

            // Set up the empty series set and the color range.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<Color> colors = ColorUtilities.Jet(NumberOfColors);

            //       End date, daysTaken, pagesRead, translated
            List<Tuple<DateTime, double, double, double>> points = new List<Tuple<DateTime, double, double, double>>();

            // Set up the list of deltas with to add points for.
            List<BooksDelta> deltasSet = new List<BooksDelta>();
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                deltasSet.Add(delta);
                if (deltasSet.Count < 10)
                {
                    continue;
                }

                BooksDelta end = deltasSet.Last();

                double daysTaken = end.LastTenTally.DaysInTally;
                double pagesRead = end.LastTenTally.TotalPages;

                double translated = end.LastTenTally.PercentageInTranslation;

                points.Add(new Tuple<DateTime, double, double, double>(end.Date, daysTaken, pagesRead, translated));

                deltasSet.RemoveAt(0);
            }

            // Add a series per point.
            foreach (Tuple<DateTime, double, double, double> point in points)
            {
                Color color = GetPointColorForTranslatedRate(point.Item4, colors);
                ScatterSeries pointSeries =
                    CreateScatterSeries(
                        point.Item1.ToString("ddd d MMM yyy") + " % Translated = " + point.Item4.ToString("G3"),
                        point.Item2,
                        point.Item3,
                        color,
                        9);
                seriesViews.Add(pointSeries);
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentBooksReadByCountryPieChartViewModel"/> class.
        /// </summary>
        public LastTenBooksAndPagesInTranslationScatterChartViewModel()
        {
            Title = "Last 10 Books in Translation: Time Taken vs Pages";

            XAxisTitle = "Days Taken";
            YAxisTitle = "Pages Read";
            MinX = MinY = 0;
            MaxX = MaxY = 1;
            PointLabel = chartPoint => $"({XAxisTitle} {chartPoint.X:G} , {YAxisTitle} {chartPoint.Y:G})";
            Formatter = value => value.ToString("G3");

            LegendLocation = LegendLocation.None;
            SetupSeries();
        }
    }
}
