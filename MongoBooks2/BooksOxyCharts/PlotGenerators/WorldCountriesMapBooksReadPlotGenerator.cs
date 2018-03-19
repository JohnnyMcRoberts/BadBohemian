// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldCountriesMapBooksReadPlotGenerator.cs" company="N/A">
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
    using BooksCore.Books;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using BooksCore.Geography;

    public class WorldCountriesMapBooksReadPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Countries of the World and Books Read" };
            SetupLatitudeAndLongitudeAxes(newPlot);

            // make up a lit of the countries with books read
            int maxBooksRead = -1;
            Dictionary<string, int> countryToReadLookUp = new Dictionary<string, int>();
            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries)
            {
                maxBooksRead = Math.Max(authorCountry.TotalBooksReadFromCountry, maxBooksRead);
                countryToReadLookUp.Add(authorCountry.Country, authorCountry.TotalBooksReadFromCountry);
            }
            List<OxyColor> colors;
            OxyPalette faintPalette;
            maxBooksRead = 
                OxyPlotUtilities.SetupFaintPaletteForRange(maxBooksRead, out colors, out faintPalette, 128);
            
            foreach (Nation nation in GeographyProvider.Nations)
            {
                CountryGeography country = nation.Geography;
                if (country != null)
                {
                    AddCountryGeographyToPlot(newPlot, countryToReadLookUp, colors, country);
                }
            }

            newPlot.Axes.Add(new LinearColorAxis
            { Position = AxisPosition.Right, Palette = faintPalette, Title = "Books Read", Maximum = maxBooksRead, Minimum = 0 });

            // finally update the model with the new plot
            return newPlot;
        }


        private static void AddCountryGeographyToPlot(
            PlotModel newPlot, 
            Dictionary<string, int> countryToReadLookUp, 
            List<OxyColor> colors, 
            CountryGeography country)
        {
            OxyColor color = OxyColors.LightGray;
            string tagString = "";

            if (countryToReadLookUp.ContainsKey(country.Name))
            {
                color = colors[countryToReadLookUp[country.Name]];
                tagString = "\nBooks Read = " + countryToReadLookUp[country.Name].ToString();
            }

            string trackerFormat = "{0}\nLat/Long ( {4:0.###} ,{2:0.###} )" + tagString;
            OxyPlotUtilities.AddCountryGeographyAreaSeriesToPlot(newPlot, country, color, country.Name, tagString, trackerFormat);
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
