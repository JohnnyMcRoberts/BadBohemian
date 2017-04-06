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
    public class CurrentBooksReadByCountryPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupCurrentBooksReadByCountryPlot();
        }

        private Models.MainBooksModel _mainModel;

        private PlotModel SetupCurrentBooksReadByCountryPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Current Books Read by Country" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Books Read by Country");

            var currentResults = _mainModel.BookDeltas.Last();

            List<KeyValuePair<string, int>> countryTotals = new List<KeyValuePair<string, int>>();

            // Country, ttl books, ttl books %, ttl pages, ttl pages%
            //Tuple<string, UInt32, double, UInt32, double>

            foreach (var country in currentResults.OverallTally.CountryTotals)
            {
                string countryName = country.Item1;
                int ttlBooks = (int)country.Item2;
                countryTotals.Add(new KeyValuePair<string, int>(countryName, ttlBooks));
            }

            var sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();
            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Books Read by Country", 128);
        }

    }
}
