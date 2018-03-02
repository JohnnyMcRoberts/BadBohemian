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

    public class CurrentMonthPagesReadByLanguagePlotGenerator : BasePlotGenerator
    {
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
