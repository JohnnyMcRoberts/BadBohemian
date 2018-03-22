// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageRateLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The page rate line chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
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
    /// The page rate line chart view model class.
    /// </summary>
    public sealed class PageRateLineChartViewModel : BaseLineChartViewModel
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
            YAxisTitle = "Page Rate";

            // Setup the curve fitter.
            ICurveFitter curveFitter;
            GetPageRateWithTimeCurveFitter(out curveFitter);

            // Set up the series for the overall and the trendline.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<DateTime> dates = new List<DateTime>();
            List<double> overallSeriesValues = new List<double>();
            List<double> lastTenSeriesValues = new List<double>();
            List<double> overallTrendlineValues = new List<double>();

            // Get the values.
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                dates.Add(delta.Date);

                overallSeriesValues.Add(delta.OverallTally.PageRate);
                lastTenSeriesValues.Add(delta.LastTenTally.PageRate);
                overallTrendlineValues.Add(curveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart));
            }

            seriesViews.Add(CreateLineSeries("Overall", dates, overallSeriesValues, Colors.Blue, 3d));
            seriesViews.Add(CreateLineSeries("Last 10", dates, lastTenSeriesValues, Colors.Red, 3d));
            seriesViews.Add(CreateLineSeries("Overall trendline", dates, overallTrendlineValues, ColorUtilities.GetFaintColor(Colors.Blue), 0d));

            Series.AddRange(seriesViews);
            SeriesCollection = Series;

            List<double> allValues = overallSeriesValues.Concat(lastTenSeriesValues).Concat(overallTrendlineValues).ToList();
            MinY = Math.Floor(allValues.Min());
            MaxY = Math.Ceiling(allValues.Max());
        }

        /// <summary>
        /// Gets the curve fitter for the trend line.
        /// </summary>
        /// <param name="curveFitter">The overall page rate curve fitter.</param>
        private void GetPageRateWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                xVals.Add(delta.DaysSinceStart);
                yVals.Add(delta.OverallTally.PageRate);
            }

            curveFitter = new LinearCurveFitter(xVals, yVals);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageRateLineChartViewModel"/> class.
        /// </summary>
        public PageRateLineChartViewModel()
        {
            Title = "Pages rate with time";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
