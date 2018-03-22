// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageDaysPerBookPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The average days per book oxy-plot generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using System.Collections.Generic;
    using BooksCore.Books;
    using BooksCore.Utilities;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    /// <summary>
    /// The average days per book plot generator.
    /// </summary>
    public class AverageDaysPerBookPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Average Days Per Book Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Average Days Per Book Plot");
            SetupDaysPerBookVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries overallSeries;
            LineSeries lastTenSeries;
            LineSeries overallTrendlineSeries;
            LineSeries lastTenTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out overallSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Overall", 1);
            OxyPlotUtilities.CreateLineSeries(out overallTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Overall Trendline", 4);
            OxyPlotUtilities.CreateLineSeries(out lastTenSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Last 10", 0);
            OxyPlotUtilities.CreateLineSeries(out lastTenTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Last 10 Trendline", 3);

            ICurveFitter lastTenCurveFitter;
            ICurveFitter overallCurveFitter;
            GetAverageDaysPerBookCurveFitters(out lastTenCurveFitter, out overallCurveFitter);

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                double trendOverallDaysPerBook =
                    overallCurveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);
                double trendLastTenDaysPerBook =
                    lastTenCurveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);

                overallSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.DaysPerBook));
                lastTenSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.LastTenTally.DaysPerBook));

                overallTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendOverallDaysPerBook));
                lastTenTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendLastTenDaysPerBook));
            }

            OxyPlotUtilities.AddLineSeriesToModel(
                newPlot,
                new[] { overallSeries, lastTenSeries, overallTrendlineSeries, lastTenTrendlineSeries });

            // finally update the model with the new plot
            return newPlot;
        }

        /// <summary>
        /// Gets the curve fitters for the two trend lines.
        /// </summary>
        /// <param name="lastTenCurveFitter">The last ten curve fitter.</param>
        /// <param name="overallCurveFitter">The overall curve fitter.</param>
        private void GetAverageDaysPerBookCurveFitters(out ICurveFitter lastTenCurveFitter, out ICurveFitter overallCurveFitter)
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
        /// Sets up the axes for the plot.
        /// </summary>
        /// <param name="newPlot">The plot to set up the axes for.</param>
        private void SetupDaysPerBookVsTimeAxes(PlotModel newPlot)
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
                Title = "Days per Book",
                Key = ChartAxisKeys.DaysPerBookKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Minimum = 0
            };

            newPlot.Axes.Add(lhsAxis);
        }
    }
}
