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

    public class CurrentBooksReadByCountryPlotGenerator : BasePlotGenerator
    {
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Current Books Read by Country" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Books Read by Country");

            BooksDelta currentResults = BooksReadProvider.BookDeltas.Last();

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

            List<KeyValuePair<string, int>> sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();

            if (ttlOtherPercentage > 1.0)
                sortedCountryTotals.Add(new KeyValuePair<string, int>("Other", ttlOtherBooks));

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Books Read by Country", 128);
        }

    }
}
