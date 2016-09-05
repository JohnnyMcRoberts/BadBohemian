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
    public class CurrentPagesReadByCountryPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupCurrentPagesReadByCountryPlot();
        }

        private Models.MainBooksModel _mainModel;

        private PlotModel SetupCurrentPagesReadByCountryPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Current Pages Read by Country" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Pages Read by Country");

            var currentResults = _mainModel.BookDeltas.Last();

            List<KeyValuePair<string, int>> countryTotals = new List<KeyValuePair<string, int>>();

            // Country, ttl books, ttl books %, ttl pages, ttl pages%
            //Tuple<string, UInt32, double, UInt32, double>

            foreach (var country in currentResults.OverallTally.CountryTotals)
            {
                string countryName = country.Item1;
                int ttlPages = (int)country.Item4;
                countryTotals.Add(new KeyValuePair<string, int>(countryName, ttlPages));
            }

            var sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();
            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Pages Read by Country", 128);
        }

    }
}

