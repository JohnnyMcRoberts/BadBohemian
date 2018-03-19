// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountryLocationsBooksReadPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The percentage books read by country with time plot generator.
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

    public class CountryLocationsBooksReadPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Countries in Location with Books Read Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Countries in Location with Books Read Plot");
            SetupLatitudeAndLongitudeAxes(newPlot);

            // create series and add them to the plot
            ScatterSeries pointsSeries;

            OxyPlotUtilities.CreateScatterPointSeries(out pointsSeries,
                ChartAxisKeys.LongitudeKey, ChartAxisKeys.LatitudeKey, "Countries");

            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries)
            {
                string name = authorCountry.Country;
                WorldCountry country = GeographyProvider.WorldCountries.FirstOrDefault(w => w.Country == name);
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

            List<OxyColor> colors = 
                OxyPalettes.Jet(200).Colors.Select(color => OxyColor.FromArgb(128, color.R, color.G, color.B)).ToList();

            OxyPalette faintPalette = new OxyPalette(colors);

            newPlot.Axes.Add(new LinearColorAxis
            { Position = AxisPosition.Right, Palette = faintPalette, Title = "Books Read" });

            return newPlot;
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
