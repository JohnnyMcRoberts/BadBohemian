using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Annotations;

using MongoDbBooks.Models;
using MongoDbBooks.ViewModels.Utilities;


namespace MongoDbBooks.ViewModels.PlotGenerators
{
    public class CountryLocationsBooksReadPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupCountryLocationsBooksReadPlot();
        }
        private Models.MainBooksModel _mainModel;

        private PlotModel SetupCountryLocationsBooksReadPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Countries in Location with Books Read Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Countries in Location with Books Read Plot");
            SetupLatitudeAndLongitudeAxes(newPlot);

            // create series and add them to the plot
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

                    ScatterPoint point =
                        new ScatterPoint(country.Longitude, country.Latitude, pointSize,
                        authorCountry.TotalBooksReadFromCountry)
                        { Tag = name };
                    pointsSeries.Points.Add(point);

                }

            }

            pointsSeries.TrackerFormatString = "{Tag}\nLat/Long ( {4:0.###} ,{2:0.###} ) \nTotal Books {6}";
            newPlot.Series.Add(pointsSeries);

            List<OxyColor> colors = new List<OxyColor>();
            foreach (var color in OxyPalettes.Jet(200).Colors)
            {
                var faintColor = OxyColor.FromArgb(128, color.R, color.G, color.B);
                colors.Add(faintColor);
            }

            OxyPalette faintPalette = new OxyPalette(colors);

            newPlot.Axes.Add(new LinearColorAxis
            { Position = AxisPosition.Right, Palette = faintPalette, Title = "Books Read" });

            return newPlot;
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
