// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageDaysPerBookPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using System;
    using System.Collections.Generic;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System.Linq;
    using BooksCore.Books;
    using BooksCore.Geography;

    public class WorldCountriesMapPagesReadPlotGenerator : BasePlotGenerator
    {
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Countries of the World with Pages read" };
            //OxyPlotUtilities.SetupPlotLegend(newPlot, "Total Pages Read by Language With Time Plot");
            SetupLatitudeAndLongitudeAxes(newPlot);

            // make up a lit of the countries with books read
            int maxBooksPages = 0;
            int maxBooksLogPages = 0;
            Dictionary<string, long> countryToReadLookUp = new Dictionary<string, long>();
            Dictionary<string, uint> countryToPagesLookUp = new Dictionary<string, uint>();
            Dictionary<string, uint> countryToLogPagesLookUp = new Dictionary<string, uint>();
            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries)
            {
                int totalPagesInThousands = (int)((long)authorCountry.TotalPagesReadFromCountry / 1000);
                if (totalPagesInThousands < 1)
                    totalPagesInThousands = 1;

                double ttl = 
                    (authorCountry.TotalPagesReadFromCountry > 1 ) 
                    ? authorCountry.TotalPagesReadFromCountry : 10;
                uint logPages =  (uint)(10.0 * Math.Log10(ttl));

                maxBooksPages = Math.Max(totalPagesInThousands, maxBooksPages);
                maxBooksLogPages = Math.Max((int)logPages, maxBooksLogPages);
                countryToReadLookUp.Add(authorCountry.Country, totalPagesInThousands);
                countryToPagesLookUp.Add(authorCountry.Country, authorCountry.TotalPagesReadFromCountry);
                countryToLogPagesLookUp.Add(authorCountry.Country, logPages - 10);
            }

            List<OxyColor> colors;
            OxyPalette faintPalette;
            maxBooksPages =
                OxyPlotUtilities.SetupFaintPaletteForRange(maxBooksLogPages, out colors, out faintPalette, 128);

            foreach (CountryGeography country in GeographyProvider.CountryGeographies)
            {
                if (country?.Name == null)
                    continue;

                OxyColor color = OxyColors.LightGray;
                string tagString = "";

                if (countryToReadLookUp.ContainsKey(country.Name))
                {
                    color = colors[(int)countryToLogPagesLookUp[country.Name]];
                    tagString = "\nPages Read = " + countryToPagesLookUp[country.Name].ToString();
                }

                int i = 0;
                // just do the 5 biggest bits per country (looks enough)
                //IOrderedEnumerable<PolygonBoundary> landBlocks = country.LandBlocks.OrderByDescending(b => b.TotalArea);

                foreach (PolygonBoundary boundary in country.LandBlocks.OrderByDescending(b => b.TotalArea))
                {
                    AreaSeries areaSeries = new AreaSeries
                    {
                        Color = color,
                        Title = country.Name,
                        RenderInLegend = false,
                        Tag = tagString
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

                    areaSeries.TrackerFormatString = "{0}\nLat/Long ( {4:0.###} ,{2:0.###} )" + tagString;
                    newPlot.Series.Add(areaSeries);

                    i++;
                    if (i > 10)
                        break;
                }
            }

            newPlot.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = faintPalette, Title = "Thousand Pages Read", 
                Maximum = maxBooksPages, Minimum = 0 });

            // finally update the model with the new plot
            return newPlot;
        }

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
