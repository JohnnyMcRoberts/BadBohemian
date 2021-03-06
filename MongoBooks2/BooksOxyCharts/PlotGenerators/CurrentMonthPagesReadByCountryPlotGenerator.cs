﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentMonthPagesReadByCountryPlotGenerator.cs" company="N/A">
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

    public class CurrentMonthPagesReadByCountryPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Current Month Pages Read by Author Nationality", TitlePadding = 15 };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Month Pages Read by Author Nationality");

            Dictionary<string, int> pagesPerCountry = new Dictionary<string, int>();

            foreach (var book in BooksReadProvider.SelectedMonthTally.BooksRead)
            {
                if (pagesPerCountry.ContainsKey(book.Nationality))
                    pagesPerCountry[book.Nationality] += book.Pages;
                else
                    pagesPerCountry.Add(book.Nationality, book.Pages);

            }

            List<KeyValuePair<string, int>> sortedCountryTotals = pagesPerCountry.OrderByDescending(x => x.Value).ToList();

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Month Pages Read by Author Nationality", 128);
        }
    }
}
