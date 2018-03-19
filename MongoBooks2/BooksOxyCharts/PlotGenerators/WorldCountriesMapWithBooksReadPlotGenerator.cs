// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldCountriesMapWithBooksReadPlotGenerator.cs" company="N/A">
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
    using System.Linq;
    using BooksCore.Books;
    using BooksCore.Geography;

    public class WorldCountriesMapWithBooksReadPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Countries of the World" };

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

            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries)
            {
                string name = authorCountry.Country;
                WorldCountry country = GeographyProvider.WorldCountries.FirstOrDefault(w => w.Country == name);
                if (country != null)
                {
                    int pointSize = authorCountry.TotalBooksReadFromCountry;
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
            foreach (OxyColor color in OxyPalettes.Jet(200).Colors)
            {
                OxyColor faintColor = OxyColor.FromArgb(128, color.R, color.G, color.B);
                colors.Add(faintColor);
            }

            OxyPalette faintPalette = new OxyPalette(colors);

            newPlot.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = faintPalette, Title = "Total Pages" });
        }

        private void AddCountriesForMap(PlotModel newPlot)
        {
            foreach (CountryGeography country in GeographyProvider.CountryGeographies)
            {
                if (country?.LandBlocks == null || !country.LandBlocks.Any())
                    continue;

                int i = 0;
                IOrderedEnumerable<PolygonBoundary> landBlocks = country.LandBlocks.OrderByDescending(b => b.TotalArea);

                foreach (PolygonBoundary boundary in landBlocks)
                {
                    AreaSeries areaSeries = new AreaSeries
                    {
                        Color = OxyColors.LightGreen,
                        Title = country.Name,
                        RenderInLegend = false
                    };
                    List<PolygonPoint> points = boundary.Points;
                    if (points.Count > PolygonReducer.MaxPolygonPoints)
                        points = PolygonReducer.AdaptativePolygonReduce(points, PolygonReducer.MaxPolygonPoints);

                    foreach (PolygonPoint point in points)
                    {
                        double ptX;
                        double ptY;
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

        /// <summary>
        /// Sets up the axes for the plot.
        /// </summary>
        /// <param name="newPlot">The plot to set up the axes for.</param>
        private void SetupLatitudeAndLongitudeAxes(PlotModel newPlot)
        {
            LinearAxis xAxis = new LinearAxis
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

            LinearAxis yAxis = new LinearAxis
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
