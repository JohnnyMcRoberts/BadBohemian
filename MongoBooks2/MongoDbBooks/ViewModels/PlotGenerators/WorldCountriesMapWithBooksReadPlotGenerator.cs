namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    using MongoDbBooks.Models.Geography;
    using MongoDbBooks.ViewModels.Utilities;

    public class WorldCountriesMapWithBooksReadPlotGenerator : IPlotGenerator
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

            AddBooksAndPagesScatterSeries(newPlot);

            // finally update the model with the new plot
            return newPlot;
        }

        private void AddBooksAndPagesScatterSeries(PlotModel newPlot)
        {
            ScatterSeries pointsSeries;

            OxyPlotUtilities.CreateScatterPointSeries(out pointsSeries,
                ChartAxisKeys.LongitudeKey, ChartAxisKeys.LatitudeKey, "Countries");

            foreach (var authorCountry in _mainModel.AuthorCountries)
            {
                var name = authorCountry.Country;
                var country = _mainModel.WorldCountries.Where(w => w.Country == name).FirstOrDefault();
                if (country != null)
                {
                    var pointSize = authorCountry.TotalBooksReadFromCountry;
                    if (pointSize < 5) pointSize = 5;

                    PolygonPoint latLong = new PolygonPoint() { Latitude = country.Latitude, Longitude = country.Longitude };
                    double x, y;
                    latLong.GetCoordinates(out x, out y);

                    ScatterPoint point =
                        new ScatterPoint(x, y, pointSize,
                        authorCountry.TotalPagesReadFromCountry) { Tag = name };
                    pointsSeries.Points.Add(point);
                }
            }
            pointsSeries.RenderInLegend = false;
            pointsSeries.TrackerFormatString = "{Tag}\nLat/Long ( {4:0.###} ,{2:0.###} ) \nTotalPages {6}";
            newPlot.Series.Add(pointsSeries);

            List<OxyColor> colors = new List<OxyColor>();
            foreach (var color in OxyPalettes.Jet(200).Colors)
            {
                var faintColor = OxyColor.FromArgb(128, color.R, color.G, color.B);
                colors.Add(faintColor);
            }

            OxyPalette faintPalette = new OxyPalette(colors);

            newPlot.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = faintPalette, Title = "Total Pages" });
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
