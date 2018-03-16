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
    using LiveCharts;
    using LiveCharts.Definitions.Series;

    /// <summary>
    /// The pages per day with time line chart view model class.
    /// </summary>
    public sealed class BooksInTranslationLineChartViewModel : BaseLineChartViewModel
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
            YAxisTitle = "% Books In Translation";

            // Set up the series for the overall and the trendline.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();

            // Setup the curve fitter.
            ICurveFitter overallCurveFitter;
            GetBooksInTranslationLinearCurveFitter(out overallCurveFitter);

            // Get the data points for the series.
            List<double> overallSeries;
            List<double> lastTenSeries;
            List<double> overallTrendlineSeries;
            List<DateTime> dates =
                GetDataForSeries(overallCurveFitter, out overallSeries, out lastTenSeries, out overallTrendlineSeries);

            // Add the series for the values.
            seriesViews.Add(CreateLineSeries("Overall", dates, overallSeries, Colors.Blue, 5d));
            seriesViews.Add(CreateLineSeries("Overall trendline", dates, overallTrendlineSeries, Colors.Green, 0d));
            seriesViews.Add(CreateLineSeries("Last 10", dates, lastTenSeries, Colors.Red, 5d));

            Series.AddRange(seriesViews);
            SeriesCollection = Series;

            // Update the Y-axis range.
            List<double> allValues = overallSeries.Concat(lastTenSeries).ToList();
            MinY = Math.Floor(allValues.Min());
            MaxY = Math.Ceiling(allValues.Max());
        }

        private List<DateTime> GetDataForSeries(
            ICurveFitter overallCurveFitter,
            out List<double> overallSeries,
            out List<double> lastTenSeries,
            out List<double> overallTrendlineSeries)
        {
            List<DateTime> dates = new List<DateTime>();

            overallSeries = new List<double>();
            lastTenSeries = new List<double>();
            overallTrendlineSeries = new List<double>();

            // Get the values.
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                double trendOverallDaysPerBook = overallCurveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);
                dates.Add(delta.Date);

                overallSeries.Add(delta.OverallTally.PercentageInTranslation);
                lastTenSeries.Add(delta.LastTenTally.PercentageInTranslation);
                overallTrendlineSeries.Add(trendOverallDaysPerBook);
            }

            return dates;
        }

        private void GetBooksInTranslationLinearCurveFitter(out ICurveFitter overallCurveFitter)
        {
            List<double> overallDays = new List<double>();
            List<double> overallDaysPerBook = new List<double>();

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallDaysPerBook.Add(delta.OverallTally.PercentageInTranslation);
            }

            overallCurveFitter = new LinearCurveFitter(overallDays, overallDaysPerBook);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagesPerDayWithTimeLineChartViewModel"/> class.
        /// </summary>
        public BooksInTranslationLineChartViewModel()
        {
            Title = "% Books In Translation";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
