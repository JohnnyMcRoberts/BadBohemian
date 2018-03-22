// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagesPerDayWithTimeLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The days per book with time with time line chart view model.
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
    /// The days per book with time line chart view model class.
    /// </summary>
    public sealed class DaysPerBookLineChartViewModel : BaseLineChartViewModel
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
            YAxisTitle = "Days per Book";

            // Setup the curve fitter.
            ICurveFitter curveFitter;
            GetDaysPerBookWithTimeCurveFitter(out curveFitter);

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

                overallSeriesValues.Add(delta.OverallTally.DaysPerBook);
                lastTenSeriesValues.Add(delta.LastTenTally.DaysPerBook);
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
        /// <param name="curveFitter">The overall days per book curve fitter.</param>
        private void GetDaysPerBookWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                xVals.Add(delta.DaysSinceStart);
                yVals.Add(delta.OverallTally.DaysPerBook);
            }

            curveFitter = new LinearCurveFitter(xVals, yVals);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DaysPerBookLineChartViewModel"/> class.
        /// </summary>
        public DaysPerBookLineChartViewModel()
        {
            Title = "Days per book with time";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
