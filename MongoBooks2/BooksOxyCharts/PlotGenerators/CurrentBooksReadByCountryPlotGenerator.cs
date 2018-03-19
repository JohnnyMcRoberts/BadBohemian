// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentBooksReadByCountryPlotGenerator.cs" company="N/A">
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
    using BooksCore.Utilities;

    public class CurrentBooksReadByCountryPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Current Books Read by Country" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Books Read by Country");

            List<KeyValuePair<string, int>> sortedCountryTotals =
                BookTotalsUtilities.SortedSortedBooksReadByCountryTotals(BooksReadProvider);

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Books Read by Country", 128);
        }
    }
}
