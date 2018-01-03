
namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;

    using MongoDbBooks.ViewModels.Utilities;

    public class CurrentPagesReadByCountryPlotGenerator : IPlotGenerator
    {
        public PlotModel SetupPlot(Models.MainBooksModel mainModel)
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

            int ttlOtherPages = 0;
            double ttlOtherPercentage = 0;
            foreach (var country in currentResults.OverallTally.CountryTotals)
            {
                string countryName = country.Item1;
                int ttlPages = (int)country.Item4;
                double countryPercentage = country.Item5;
                if (countryPercentage > 1.0)
                {
                    countryTotals.Add(new KeyValuePair<string, int>(countryName, ttlPages));
                }
                else
                {
                    ttlOtherPercentage += countryPercentage;
                    ttlOtherPages += ttlPages;
                }
            }

            var sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();

            if (ttlOtherPercentage > 1.0)
                sortedCountryTotals.Add(new KeyValuePair<string, int>("Other", ttlOtherPages));

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Pages Read by Country", 128);
        }

    }
}

