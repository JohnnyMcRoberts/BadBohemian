namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;

    public class CurrentMonthPagesReadByCountryPlotGenerator : IPlotGenerator
    {
        public PlotModel SetupPlot(MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupCurrentPagesReadByCountryPlot();
        }

        private MainBooksModel _mainModel;

        private PlotModel SetupCurrentPagesReadByCountryPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Current Month Pages Read by Author Nationality", TitlePadding = 15 };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Month Pages Read by Author Nationality");

            Dictionary<string, int> pagesPerCountry = new Dictionary<string, int>();

            foreach (var book in _mainModel.SelectedMonthTally.BooksRead)
            {
                if (pagesPerCountry.ContainsKey(book.Nationality))
                    pagesPerCountry[book.Nationality] += book.Pages;
                else
                    pagesPerCountry.Add(book.Nationality, book.Pages);

            }

            var sortedCountryTotals = pagesPerCountry.OrderByDescending(x => x.Value).ToList();

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Month Pages Read by Author Nationality", 128);
        }
    }
}
