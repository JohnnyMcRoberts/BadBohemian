namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;

    using MongoDbBooks.ViewModels.Utilities;

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

            int ttlOtherBooks = 0;
            double ttlOtherPercentage = 0;
            foreach (var country in currentResults.OverallTally.CountryTotals)
            {
                string countryName = country.Item1;
                int ttlBooks = (int)country.Item2;
                double countryPercentage = country.Item3;
                if (countryPercentage > 1.0)
                {
                    countryTotals.Add(new KeyValuePair<string, int>(countryName, ttlBooks));
                }
                else
                {
                    ttlOtherPercentage += countryPercentage;
                    ttlOtherBooks += ttlBooks;
                }
            }

            var sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();

            if (ttlOtherPercentage > 1.0)
                sortedCountryTotals.Add(new KeyValuePair<string, int>("Other", ttlOtherBooks));

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Books Read by Country", 128);
        }

    }
}
