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
    using System.Collections.Generic;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using System.Linq;
    using BooksCore.Books;
    using BooksCore.Utilities;

    public class CurrentPagesReadByCountryPlotGenerator : BasePlotGenerator
    {
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Current Pages Read by Country" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Pages Read by Country");
#if ddfff
            BooksDelta currentResults = BooksReadProvider.BookDeltas.Last();

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

            List<KeyValuePair<string, int>> sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();

            if (ttlOtherPercentage > 1.0)
                sortedCountryTotals.Add(new KeyValuePair<string, int>("Other", ttlOtherPages));
#endif


            List<KeyValuePair<string, int>> sortedCountryTotals =
                BookTotalsUtilities.SortedSortedPagesReadByCountryTotals(BooksReadProvider);

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Pages Read by Country", 128);
        }

    }
}

