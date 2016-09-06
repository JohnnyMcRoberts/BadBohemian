using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

using MongoDbBooks.Models;
using MongoDbBooks.ViewModels.Utilities;

namespace MongoDbBooks.ViewModels.PlotGenerators
{
    public class LatitudeWithTimePlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupLatitudeWithTimePlot();
        }

        private Models.MainBooksModel _mainModel;

        private PlotModel SetupLatitudeWithTimePlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Latitude With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Latitude With Time Plot");
            SetupLatitudeVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries overallSeries;
            LineSeries lastTenSeries;
            LineSeries overallTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out overallSeries, ChartAxisKeys.DateKey, ChartAxisKeys.LatitudeKey, "Overall", 1);
            OxyPlotUtilities.CreateLineSeries(out lastTenSeries, ChartAxisKeys.DateKey, ChartAxisKeys.LatitudeKey, "Last 10", 0);
            OxyPlotUtilities.CreateLineSeries(out overallTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.LatitudeKey, "Overall Trendline", 4);

            double yintercept;
            double slope;
            GetLatitudeLinearTrendlineParameters(out yintercept, out slope);

            foreach (var delta in _mainModel.BookLocationDeltas)
            {
                double trendPageRate = yintercept + (slope * delta.DaysSinceStart);

                overallSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.AverageLatitude));
                lastTenSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.AverageLatitudeLastTen));
                overallTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendPageRate));
            }


            OxyPlotUtilities.AddLineSeriesToModel(newPlot,
                new LineSeries[] { overallSeries, lastTenSeries, overallTrendlineSeries }
                );


            // finally update the model with the new plot
            return newPlot;
        }

        private void GetLatitudeLinearTrendlineParameters(out double yintercept, out double slope)
        {
            double rsquared;

            List<double> overallDays = new List<double>();
            List<double> overallLatitude = new List<double>();

            foreach (var delta in _mainModel.BookLocationDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallLatitude.Add(delta.AverageLatitudeLastTen);
            }
            OxyPlotUtilities.LinearRegression(overallDays, overallLatitude, out  rsquared, out  yintercept, out  slope);
        }

        private void SetupLatitudeVsTimeAxes(PlotModel newPlot)
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
                Title = "Latitude",
                Key = ChartAxisKeys.LatitudeKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }

    }
}
