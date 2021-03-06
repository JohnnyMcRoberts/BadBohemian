﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DaysPerBookPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The percentage books read by country with time plot generator.
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

    public class DaysPerBookPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Days Per Book Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Days Per Book Plot");
            SetupDaysPerBookVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries overallSeries;
            LineSeries lastTenSeries;
            LineSeries overallTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out overallSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Overall", 1);
            OxyPlotUtilities.CreateLineSeries(out lastTenSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Last 10", 0);
            OxyPlotUtilities.CreateLineSeries(out overallTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Overall Trendline", 4);
            double yintercept;
            double slope;
            GetDaysPerBookLinearTrendlineParameters(out yintercept, out slope);

            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                double trendDaysPerBook = yintercept + (slope * delta.DaysSinceStart);

                overallSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.DaysPerBook));
                lastTenSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.LastTenTally.DaysPerBook));
                overallTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendDaysPerBook));
            }

            OxyPlotUtilities.AddLineSeriesToModel(newPlot, new[] { overallSeries, lastTenSeries, overallTrendlineSeries } );


            return newPlot;
        }

        private void GetDaysPerBookLinearTrendlineParameters(out double yintercept, out double slope)
        {
            double rsquared;

            List<double> overallDays = new List<double>();
            List<double> overallDaysPerBook = new List<double>();

            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallDaysPerBook.Add(delta.OverallTally.DaysPerBook);
            }
            OxyPlotUtilities.LinearRegression(overallDays, overallDaysPerBook, out rsquared, out yintercept, out slope);
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
