
namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    using MongoDbBooks.Models.Geography;
    using MongoDbBooks.ViewModels.Utilities;

    public class WorldCountriesMapLastTenLatLongPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupWorldCountriesMapPlot();
        }
        private Models.MainBooksModel _mainModel;

        private PlotModel SetupWorldCountriesMapPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Countries of the World" };

            SetupLatitudeAndLongitudeAxes(newPlot);

            AddCountriesForMap(newPlot);

            AddLastTenLatLongWithTimeScatterSeries(newPlot);

            // finally update the model with the new plot
            return newPlot;
        }

        private void AddLastTenLatLongWithTimeScatterSeries(PlotModel newPlot)
        {
            ScatterSeries pointsSeries;

            OxyPlotUtilities.CreateScatterPointSeries(out pointsSeries,
                ChartAxisKeys.LongitudeKey, ChartAxisKeys.LatitudeKey, "Last Ten Books With Time");


            LineSeries overallSeries;
            OxyPlotUtilities.CreateLineSeries(out overallSeries, ChartAxisKeys.LongitudeKey, ChartAxisKeys.LatitudeKey, "Overall", 0);
            var faintColorBlue = OxyColor.FromArgb(80, OxyColors.Blue.R, OxyColors.Blue.G, OxyColors.Blue.B);
            overallSeries.Color = faintColorBlue;
            overallSeries.StrokeThickness = 2;

            LineSeries lastTenSeries;
            OxyPlotUtilities.CreateLineSeries(out lastTenSeries, ChartAxisKeys.LongitudeKey, ChartAxisKeys.LatitudeKey, "Last 10", 0);            
            var faintColorRed = OxyColor.FromArgb(80, OxyColors.Red.R, OxyColors.Red.G, OxyColors.Red.B);
            lastTenSeries.Color = faintColorRed;
            lastTenSeries.StrokeThickness = 2;

            foreach (var delta in _mainModel.BookLocationDeltas)
            {
                var pointSize = 5;

                PolygonPoint latLong =
                    new PolygonPoint() { Latitude = delta.AverageLatitudeLastTen, Longitude = delta.AverageLongitudeLastTen };
                double x, y;
                latLong.GetCoordinates(out x, out y);

                lastTenSeries.Points.Add( new DataPoint(x, y) );

                ScatterPoint point =
                    new ScatterPoint(x, y, pointSize, delta.DaysSinceStart) { Tag = delta.Date.ToString("ddd d MMM yyy") };
                pointsSeries.Points.Add(point);

                latLong =
                    new PolygonPoint() { Latitude = delta.AverageLatitude, Longitude = delta.AverageLongitude };
                latLong.GetCoordinates(out x, out y);

                overallSeries.Points.Add(new DataPoint(x, y));

            }

            // don't draw these as renders the pic unusable
            //newPlot.Series.Add(lastTenSeries);
            //newPlot.Series.Add(overallSeries);

            pointsSeries.RenderInLegend = false;
            pointsSeries.TrackerFormatString = "{Tag}\nLat/Long ( {4:0.###} ,{2:0.###} )";
            newPlot.Series.Add(pointsSeries);

            List<OxyColor> colors = new List<OxyColor>();
            foreach (var color in OxyPalettes.Jet(200).Colors)
            {
                var faintColor = OxyColor.FromArgb(80, color.R, color.G, color.B);
                colors.Add(faintColor);
            }

            OxyPalette faintPalette = new OxyPalette(colors);

            newPlot.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = faintPalette, Title = "Days Since Start" });
        }

        private void AddCountriesForMap(PlotModel newPlot)
        {

            foreach (var country in _mainModel.CountryGeographies)
            {

                int i = 0;
                var landBlocks = country.LandBlocks.OrderByDescending(b => b.TotalArea);

                foreach (var boundary in landBlocks)
                {
                    var areaSeries = new AreaSeries
                    {
                        Color = OxyColors.LightGreen,
                        Title = country.Name,
                        RenderInLegend = false
                    };
                    var points = boundary.Points;
                    if (points.Count > PolygonReducer.MaxPolygonPoints)
                        points = PolygonReducer.AdaptativePolygonReduce(points, PolygonReducer.MaxPolygonPoints);

                    foreach (var point in points)
                    {
                        double ptX = 0;
                        double ptY = 0;
                        point.GetCoordinates(out ptX, out ptY);
                        DataPoint dataPoint = new DataPoint(ptX, ptY);

                        areaSeries.Points.Add(dataPoint);
                    }

                    newPlot.Series.Add(areaSeries);

                    // just do the 10 biggest bits per country (looks to be enough)
                    i++;
                    if (i > 10)
                        break;
                }
            }
        }

        private void SetupLatitudeAndLongitudeAxes(PlotModel newPlot)
        {
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Longitude",
                Key = ChartAxisKeys.LongitudeKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Maximum = 200,
                Minimum = -200
            };
            newPlot.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Latitude",
                Key = ChartAxisKeys.LatitudeKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Maximum = 100,
                Minimum = -100
            };
            newPlot.Axes.Add(yAxis);
        }

    }
}