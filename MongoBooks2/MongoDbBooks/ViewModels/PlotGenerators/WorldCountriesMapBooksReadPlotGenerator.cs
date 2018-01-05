
namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System;
    using System.Collections.Generic;

    using OxyPlot;
    using OxyPlot.Axes;

    using MongoDbBooks.ViewModels.Utilities;

    public class WorldCountriesMapBooksReadPlotGenerator : IPlotGenerator
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
            var newPlot = new PlotModel { Title = "Countries of the World and Books Read" };
            SetupLatitudeAndLongitudeAxes(newPlot);

            // make up a lit of the countries with books read
            int maxBooksRead = -1;
            Dictionary<string, int> countryToReadLookUp = new Dictionary<string, int>();
            foreach(var authorCountry in _mainModel.AuthorCountries)
            {
                maxBooksRead = Math.Max(authorCountry.TotalBooksReadFromCountry, maxBooksRead);
                countryToReadLookUp.Add(authorCountry.Country, authorCountry.TotalBooksReadFromCountry);
            }
            List<OxyColor> colors;
            OxyPalette faintPalette;
            maxBooksRead = 
                OxyPlotUtilities.SetupFaintPaletteForRange(maxBooksRead, out colors, out faintPalette, 128);
            
            foreach (Models.Database.Nation nation in _mainModel.Nations)
            {
                Models.Geography.CountryGeography country = nation.Geography;
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
            Models.Geography.CountryGeography country)
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
