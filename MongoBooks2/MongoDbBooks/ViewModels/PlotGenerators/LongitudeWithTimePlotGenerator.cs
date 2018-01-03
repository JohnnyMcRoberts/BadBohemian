namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    using MongoDbBooks.ViewModels.Utilities;

    public class LongitudeWithTimePlotGenerator : IPlotGenerator
    {
        public PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupLongitudeWithTimePlot();
        }

        private Models.MainBooksModel _mainModel;

        private PlotModel SetupLongitudeWithTimePlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Longitude With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Longitude With Time Plot");
            SetupLongitudeVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries overallSeries;
            LineSeries lastTenSeries;
            LineSeries overallTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out overallSeries, ChartAxisKeys.DateKey, ChartAxisKeys.LongitudeKey, "Overall", 1);
            OxyPlotUtilities.CreateLineSeries(out lastTenSeries, ChartAxisKeys.DateKey, ChartAxisKeys.LongitudeKey, "Last 10", 0);
            OxyPlotUtilities.CreateLineSeries(out overallTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.LongitudeKey, "Overall Trendline", 4);

            double yintercept;
            double slope;
            GetLongitudeLinearTrendlineParameters(out yintercept, out slope);

            foreach (var delta in _mainModel.BookLocationDeltas)
            {
                double trendPageRate = yintercept + (slope * delta.DaysSinceStart);

                overallSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.AverageLongitude));
                lastTenSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.AverageLongitudeLastTen));
                overallTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendPageRate));
            }


            OxyPlotUtilities.AddLineSeriesToModel(newPlot, new[] { overallSeries, lastTenSeries, overallTrendlineSeries } );


            // finally update the model with the new plot
            return newPlot;
        }

        private void GetLongitudeLinearTrendlineParameters(out double yintercept, out double slope)
        {
            double rsquared;

            List<double> overallDays = new List<double>();
            List<double> overallLongitude = new List<double>();

            foreach (var delta in _mainModel.BookLocationDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallLongitude.Add(delta.AverageLongitudeLastTen);
            }
            OxyPlotUtilities.LinearRegression(overallDays, overallLongitude, out  rsquared, out  yintercept, out  slope);
        }

        private void SetupLongitudeVsTimeAxes(PlotModel newPlot)
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
                Title = "Longitude",
                Key = ChartAxisKeys.LongitudeKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }

    }
}
