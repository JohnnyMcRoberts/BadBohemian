// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageDaysPerBookLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The average days per book with time line chart view model.
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
    /// The average days per book with time line chart view model class.
    /// </summary>
    public sealed class AverageDaysPerBookLineChartViewModel : BaseLineChartViewModel
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

            // Set up the series for the overall and the trendline.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();

            // Setup the curve fitter.
            ICurveFitter lastTenCurveFitter;
            ICurveFitter overallCurveFitter;
            GetAverageDaysPerBookCurveFitters(out lastTenCurveFitter, out overallCurveFitter);

            // Get the data points for the series.
            List<double> overallSeries;
            List<double> lastTenSeries;
            List<double> overallTrendlineSeries;
            List<double> lastTenTrendlineSeries;
            List<DateTime> dates =
                GetDataForSeries(overallCurveFitter, lastTenCurveFitter, out overallSeries, out lastTenSeries, out overallTrendlineSeries, out lastTenTrendlineSeries);

            // Add the series for the values.
            seriesViews.Add(CreateLineSeries("Overall", dates, overallSeries, Colors.Blue, 5d));
            seriesViews.Add(CreateLineSeries("Overall trendline", dates, overallTrendlineSeries, ColorUtilities.GetFaintColor(Colors.Blue), 0d));
            seriesViews.Add(CreateLineSeries("Last 10", dates, lastTenSeries, Colors.Red, 5d));
            seriesViews.Add(CreateLineSeries("Last 10 trendline", dates, lastTenTrendlineSeries, ColorUtilities.GetFaintColor(Colors.Red), 0d));

            Series.AddRange(seriesViews);
            SeriesCollection = Series;

            // Update the Y-axis range.
            List<double> allValues = overallSeries.Concat(lastTenSeries).ToList();
            MinY = Math.Floor(allValues.Min());
            MaxY = Math.Ceiling(allValues.Max());
        }

        /// <summary>
        /// Gets data for the line chart series.
        /// </summary>
        /// <param name="lastTenCurveFitter">The last ten curve fitter.</param>
        /// <param name="overallCurveFitter">The overall curve fitter.</param>
        /// <param name="overallSeries">The overall series y-values on completion.</param>
        /// <param name="lastTenSeries">The last ten series y-values on completion.</param>
        /// <param name="overallTrendlineSeries">The overall trend-line series y-values on completion.</param>
        /// <param name="lastTenTrendlineSeries">The last ten trend-line series y-values on completion.</param>
        /// <returns>The dates associated with the series values.</returns>
        private List<DateTime> GetDataForSeries(
            ICurveFitter overallCurveFitter,
            ICurveFitter lastTenCurveFitter,
            out List<double> overallSeries,
            out List<double> lastTenSeries,
            out List<double> overallTrendlineSeries,
            out List<double> lastTenTrendlineSeries)
        {
            List<DateTime> dates = new List<DateTime>();

            overallSeries = new List<double>();
            lastTenSeries = new List<double>();
            overallTrendlineSeries = new List<double>();
            lastTenTrendlineSeries = new List<double>();

            // Get the values.
            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                double trendOverallDaysPerBook =
                    overallCurveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);
                double trendLastTenDaysPerBook =
                    lastTenCurveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);

                dates.Add(delta.Date);

                overallSeries.Add(delta.OverallTally.DaysPerBook);
                lastTenSeries.Add(delta.LastTenTally.DaysPerBook);

                overallTrendlineSeries.Add(trendOverallDaysPerBook);
                lastTenTrendlineSeries.Add(trendLastTenDaysPerBook);
            }

            return dates;
        }

        /// <summary>
        /// Gets the curve fitters for the two trend lines.
        /// </summary>
        /// <param name="lastTenCurveFitter">The last ten curve fitter.</param>
        /// <param name="overallCurveFitter">The overall curve fitter.</param>
        private void GetAverageDaysPerBookCurveFitters(
            out ICurveFitter lastTenCurveFitter,
            out ICurveFitter overallCurveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yValsLastTen = new List<double>();
            List<double> yValsOverall = new List<double>();

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                xVals.Add(delta.DaysSinceStart);
                yValsLastTen.Add(delta.LastTenTally.DaysPerBook);
                yValsOverall.Add(delta.OverallTally.DaysPerBook);
            }
            
            lastTenCurveFitter = new QuadraticCurveFitter(xVals, yValsLastTen);
            overallCurveFitter = new QuadraticCurveFitter(xVals, yValsOverall);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageDaysPerBookLineChartViewModel"/> class.
        /// </summary>
        public AverageDaysPerBookLineChartViewModel()
        {
            Title = "Average Days Per Book";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
