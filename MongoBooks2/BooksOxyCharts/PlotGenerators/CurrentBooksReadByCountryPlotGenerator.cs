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
    using System;
    using System.Collections.Generic;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using BooksCore.Utilities;

    public class CurrentBooksReadByCountryPlotGenerator : BasePlotGenerator
    {
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
