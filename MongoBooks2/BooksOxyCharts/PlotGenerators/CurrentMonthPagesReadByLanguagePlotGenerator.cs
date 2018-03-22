// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentMonthPagesReadByLanguagePlotGenerator.cs" company="N/A">
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
    using System.Linq;
    using BooksCore.Books;

    public class CurrentMonthPagesReadByLanguagePlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Current Month Pages Read By Language" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Month Pages Read By Language");

            Dictionary<string, int> pagesPerLanguage = new Dictionary<string, int>();

            foreach (BookRead book in BooksReadProvider.SelectedMonthTally.BooksRead)
            {
                if (pagesPerLanguage.ContainsKey(book.OriginalLanguage))
                    pagesPerLanguage[book.OriginalLanguage] += book.Pages;
                else
                    pagesPerLanguage.Add(book.OriginalLanguage, book.Pages);

            }           

            List<KeyValuePair<string, int>> sortedCountryTotals = pagesPerLanguage.OrderByDescending(x => x.Value).ToList();

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Month Pages Read By Language", 128);
        }
    }
}
