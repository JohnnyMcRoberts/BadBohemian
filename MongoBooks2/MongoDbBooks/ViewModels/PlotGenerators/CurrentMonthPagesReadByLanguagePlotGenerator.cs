namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using OxyPlot;
    using System.Collections.Generic;
    using System.Linq;
    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;

    public class CurrentMonthPagesReadByLanguagePlotGenerator : IPlotGenerator
    {
        public PlotModel SetupPlot(MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupCurrentPagesReadByCountryPlot();
        }

        private Models.MainBooksModel _mainModel;
        
        private PlotModel SetupCurrentPagesReadByCountryPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Current Month Pages Read By Language" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Current Month Pages Read By Language");

            Dictionary<string, int> pagesPerLanguage = new Dictionary<string, int>();

            foreach(var book in _mainModel.SelectedMonthTally.BooksRead)
            {
                if (pagesPerLanguage.ContainsKey(book.OriginalLanguage))
                    pagesPerLanguage[book.OriginalLanguage] += book.Pages;
                else
                    pagesPerLanguage.Add(book.OriginalLanguage, book.Pages);

            }           

            var sortedCountryTotals = pagesPerLanguage.OrderByDescending(x => x.Value).ToList();

            return OxyPlotUtilities.CreatePieSeriesModelForResultsSet(
                sortedCountryTotals, "Current Month Pages Read By Language", 128);
        }
    }
}
