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
    public class WorldCountriesMapPlotGenerator : IPlotGenerator
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

            foreach (Models.Database.Nation nation in _mainModel.Nations)
            {
                Models.Geography.CountryGeography country = nation.Geography;
                if (country != null)
                {
                    OxyColor colour = OxyColors.LightGreen;
                    string title = country.Name;
                    string tag = "";
                    string trackerFormat = "{0}";
                    AddCountryGeographyAreaSeriesToPlot(newPlot, country, colour, title, tag, trackerFormat);
                }
            }

            // finally update the model with the new plot
            return newPlot;
        }

        private static void AddCountryGeographyAreaSeriesToPlot(
            PlotModel newPlot, Models.Geography.CountryGeography country, OxyColor colour, string title, string tag, string trackerFormat)
        {
            int i = 0;
            var landBlocks = country.LandBlocks.OrderByDescending(b => b.TotalArea);

            foreach (var boundary in landBlocks)
            {
                var areaSeries = new AreaSeries
                {
                    Color = colour,
                    Title = title,
                    RenderInLegend = false,
                    Tag = tag
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

                areaSeries.TrackerFormatString = trackerFormat;
                newPlot.Series.Add(areaSeries);

                // just do the 10 biggest bits per country (looks to be enough)
                i++;
                if (i > 10)
                    break;
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
