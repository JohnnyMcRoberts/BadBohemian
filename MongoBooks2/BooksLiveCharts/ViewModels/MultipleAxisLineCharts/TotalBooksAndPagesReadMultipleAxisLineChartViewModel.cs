// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalBooksAndPagesReadMultipleAxisLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The total books and pages read multiple axis line chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.MultipleAxisLineCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksCore.Books;
    using BooksCore.Utilities;
    using BooksOxyCharts.Utilities;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Definitions.Series;

    /// <summary>
    /// The total books and pages read multiple axis line chart view model class.
    /// </summary>
    public sealed class TotalBooksAndPagesReadMultipleAxisLineChartViewModel : BaseMultipleAxisLineChartViewModel
    {
        /// <summary>
        /// Sets up the line chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            // If no books return the default.
            if (BooksReadProvider == null)
            {
                base.SetupSeries();
                return;
            }

            // Set up the axis names and formatters.
            XAxisTitle = "Date";
            LeftHandSideYAxisTitle = "Books Read";
            RightHandSideYAxisTitle = "Pages Read";

            // Setup the curve fitters.
            ICurveFitter booksReadWithTimeCurveFitter;
            ICurveFitter pagesReadWithTimeCurveFitter;
            GetBooksReadWithTimeCurveFitter(out booksReadWithTimeCurveFitter);
            GetPagesReadWithTimeCurveFitter(out pagesReadWithTimeCurveFitter);

            // Set up the series for the overall and the trendline.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<DateTime> dates = new List<DateTime>();
            List<double> booksReadSeriesValues = new List<double>();
            List<double> booksReadTrendlineSeriesValues = new List<double>();
            List<double> pagesReadSeriesValues = new List<double>();
            List<double> pagesReadTrendlineSeriesValues = new List<double>();

            // Get the values.
            DateTime start = BooksReadProvider.BookPerYearDeltas[0].Date;
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                dates.Add(delta.Date);

                int daysSinceStart = (delta.Date - start).Days;

                double trendBooks = booksReadWithTimeCurveFitter.EvaluateYValueAtPoint(daysSinceStart);
                double trendPages = pagesReadWithTimeCurveFitter.EvaluateYValueAtPoint(daysSinceStart);

                booksReadSeriesValues.Add(delta.OverallTally.TotalBooks);
                booksReadTrendlineSeriesValues.Add(trendBooks);

                pagesReadSeriesValues.Add(delta.OverallTally.TotalPages);
                pagesReadTrendlineSeriesValues.Add(trendPages);
            }

            // Create series from them.
            seriesViews.Add(
                CreateLineSeries("Books", dates, booksReadSeriesValues, Colors.Blue, 0d, true));
            seriesViews.Add(
                CreateLineSeries("Books trendline", dates, booksReadTrendlineSeriesValues, ColorUtilities.GetFaintColor(Colors.Blue), 0d, true));
            seriesViews.Add(
                CreateLineSeries("Pages", dates, pagesReadSeriesValues, Colors.Red, 0d, false));
            seriesViews.Add(
                CreateLineSeries("Pages trendline", dates, pagesReadTrendlineSeriesValues, ColorUtilities.GetFaintColor(Colors.Red), 0d, false));

            Series.AddRange(seriesViews);
            SeriesCollection = Series;

            List<double> allValues = booksReadSeriesValues.Concat(booksReadTrendlineSeriesValues).ToList();
            MinLeftHandSideY = Math.Floor(allValues.Min());
            MaxLeftHandSideY = Math.Ceiling(allValues.Max());

            allValues = pagesReadSeriesValues.Concat(pagesReadTrendlineSeriesValues).ToList();
            MinRightHandSideY = Math.Floor(allValues.Min());
            MaxRightHandSideY = Math.Ceiling(allValues.Max());
        }

        private void GetBooksReadWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            DateTime start = BooksReadProvider.BookPerYearDeltas[0].Date;
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                int daysSinceStart = (delta.Date - start).Days;
                xVals.Add(daysSinceStart);
                yVals.Add(delta.OverallTally.TotalBooks);
            }

            curveFitter = new QuadraticCurveFitter(xVals, yVals);
        }

        private void GetPagesReadWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            DateTime start = BooksReadProvider.BookPerYearDeltas[0].Date;
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                int daysSinceStart = (delta.Date - start).Days;
                xVals.Add(daysSinceStart);
                yVals.Add(delta.OverallTally.TotalPages);
            }

            curveFitter = new QuadraticCurveFitter(xVals, yVals);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalBooksAndPagesReadMultipleAxisLineChartViewModel"/> class.
        /// </summary>
        public TotalBooksAndPagesReadMultipleAxisLineChartViewModel()
        {
            Title = "Total books and pages read with time";
            PointLabel = chartPoint =>
            {
                string yAxisName = chartPoint.SeriesView.ScalesYAt == 0
                    ? LeftHandSideYAxisTitle
                    : RightHandSideYAxisTitle;

                return $"({XAxisTitle} {new DateTime((long)chartPoint.X):d} , {yAxisName} {chartPoint.Y:0.##})";
            };

            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
