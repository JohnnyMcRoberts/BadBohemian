// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageDaysPerBookPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using System.Collections.Generic;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    public class DaysPerBookWithTimePlotGenerator : BasePlotGenerator
    {
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Days Per Book With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Days Per Book With Time Plot");
            SetupDaysPerBookWithTimeVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries overallSeries;
            LineSeries overallTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(
                out overallSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysKey, "Overall", 1);
            OxyPlotUtilities.CreateLineSeries(
                out overallTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysKey, "Overall Trendline", 0);

            ICurveFitter curveFitter;
            GetDaysPerBookWithTimeCurveFitter(out curveFitter);

            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                double trendPageRate = curveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);

                overallSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.DaysPerBook));
                overallTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendPageRate));
            }

            OxyPlotUtilities.AddLineSeriesToModel(newPlot, new[] { overallSeries, overallTrendlineSeries } );

            // finally update the model with the new plot
            return newPlot;
        }

        private void GetDaysPerBookWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                xVals.Add(delta.DaysSinceStart);
                yVals.Add(delta.OverallTally.DaysPerBook);
            }

            curveFitter = new QuadraticCurveFitter(xVals, yVals);
        }

        private void SetupDaysPerBookWithTimeVsTimeAxes(PlotModel newPlot)
        {
            var xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = ChartAxisKeys.DateKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                StringFormat = "yyyy-MM-dd"
            };
            newPlot.Axes.Add(xAxis);

            var lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Days Per Book",
                Key = ChartAxisKeys.DaysKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }

    }
}
