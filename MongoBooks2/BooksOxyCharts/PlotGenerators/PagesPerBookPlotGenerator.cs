// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagesPerBookPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The pages per book plot generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using System.Collections.Generic;
    using BooksCore.Books;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    /// <summary>
    /// The pages per book plot generator.
    /// </summary>
    public class PagesPerBookPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Pages Per Book Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Pages Per Book Plot");
            SetupPagesPerBookVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries overallSeries;
            LineSeries lastTenSeries;
            LineSeries overallTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out overallSeries, ChartAxisKeys.DateKey, ChartAxisKeys.PagesPerBookKey, "Overall", 1);
            OxyPlotUtilities.CreateLineSeries(out lastTenSeries, ChartAxisKeys.DateKey, ChartAxisKeys.PagesPerBookKey, "Last 10", 0);
            OxyPlotUtilities.CreateLineSeries(out overallTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.PagesPerBookKey, "Overall Trendline", 4);

            double yintercept;
            double slope;
            GetPagesPerBookLinearTrendlineParameters(out yintercept, out slope);

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                double trendPageRate = yintercept + (slope * delta.DaysSinceStart);

                overallSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.PagesPerBook));
                lastTenSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.LastTenTally.PagesPerBook));
                overallTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendPageRate));
            }

            OxyPlotUtilities.AddLineSeriesToModel(newPlot, new[] { overallSeries, lastTenSeries, overallTrendlineSeries } );

            // finally update the model with the new plot
            return newPlot;
        }

        private void GetPagesPerBookLinearTrendlineParameters(out double yintercept, out double slope)
        {
            double rsquared;

            List<double> overallDays = new List<double>();
            List<double> overallPageRate = new List<double>();

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallPageRate.Add(delta.OverallTally.PagesPerBook);
            }

            OxyPlotUtilities.LinearRegression(overallDays, overallPageRate, out  rsquared, out  yintercept, out  slope);
        }

        /// <summary>
        /// Sets up the axes for the plot.
        /// </summary>
        /// <param name="newPlot">The plot to set up the axes for.</param>
        private void SetupPagesPerBookVsTimeAxes(PlotModel newPlot)
        {
            DateTimeAxis xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = ChartAxisKeys.DateKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                StringFormat = "yyyy-MM-dd"
            };

            newPlot.Axes.Add(xAxis);

            LinearAxis lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Pages Per Book",
                Key = ChartAxisKeys.PagesPerBookKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Minimum = 0
            };

            newPlot.Axes.Add(lhsAxis);
        }

    }
}
