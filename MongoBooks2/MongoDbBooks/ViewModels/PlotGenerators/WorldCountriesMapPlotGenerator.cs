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
            //OxyPlotUtilities.SetupPlotLegend(newPlot, "Total Pages Read by Language With Time Plot");
            SetupLatitudeAndLongitudeAxes(newPlot);

            foreach(var country in _mainModel.CountryGeographies)
            {

                int i = 0;
                // just do the 5 biggest bits per country (looks enough)
                var landBlocks = country.LandBlocks.OrderByDescending(b => b.TotalArea);

                foreach (var boundary in landBlocks)
                {
                    var areaSeries = new AreaSeries
                    {
                        Color = OxyColors.LightGreen,
                        Title = country.Name,
                        RenderInLegend = false
                    };
                    foreach (var point in boundary.Points)
                    {

                        double ptX = 0;
                        double ptY = 0;
                        point.GetCoordinates(out ptX, out ptY);
                        DataPoint dataPoint = new DataPoint(ptX, ptY);

                        areaSeries.Points.Add(dataPoint);

                    }

                    newPlot.Series.Add(areaSeries);

                    i++;
                    if (i > 5)
                        break;
                }
            }

            // finally update the model with the new plot
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
