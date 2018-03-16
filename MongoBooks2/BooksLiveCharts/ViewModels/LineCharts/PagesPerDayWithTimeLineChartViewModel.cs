// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagesPerDayWithTimeLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The pages per day with time line chart view model.
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
    /// The pages per day with time line chart view model class.
    /// </summary>
    public sealed class PagesPerDayWithTimeLineChartViewModel : BaseLineChartViewModel
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
            YAxisTitle = "Pages per Day";

            // Setup the curve fitter.
            ICurveFitter curveFitter;
            GetPagesPerDayWithTimeCurveFitter(out curveFitter);

            // Set up the series for the overall and the trendline.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<DateTime> dates = new List<DateTime>();
            List<double> overallSeriesValues = new List<double>();
            List<double> overallTrendlineValues = new List<double>();

            // Get the values.
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                dates.Add(delta.Date);
                double trendPageRate = curveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);

                overallSeriesValues.Add(delta.OverallTally.PageRate);
                overallTrendlineValues.Add(trendPageRate);
            }

            seriesViews.Add(CreateLineSeries("Overall", dates, overallSeriesValues, Colors.Blue, 5d));
            seriesViews.Add(CreateLineSeries("Overall trendline", dates, overallTrendlineValues, ColorUtilities.GetFaintColor(Colors.Red), 0d));

            Series.AddRange(seriesViews);
            SeriesCollection = Series;

            MinY = Math.Floor(Math.Min(overallSeriesValues.Min(), overallTrendlineValues.Min()));
            MaxY = Math.Ceiling(Math.Max(overallSeriesValues.Max(), overallTrendlineValues.Max()));
        }

        private void GetPagesPerDayWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                xVals.Add(delta.DaysSinceStart);
                yVals.Add(delta.OverallTally.PageRate);
            }

            curveFitter = new QuadraticCurveFitter(xVals, yVals);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagesPerDayWithTimeLineChartViewModel"/> class.
        /// </summary>
        public PagesPerDayWithTimeLineChartViewModel()
        {
            Title = "Pages per day with time";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
