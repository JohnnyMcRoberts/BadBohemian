// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OxyPlotViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The view model for for a particular oxyPlot.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.ViewModels
{
    using BooksCore.Interfaces;
    using BooksOxyCharts.Utilities;
    using BooksUtilities.ViewModels;
    using OxyPlot;
    using PlotGenerators;
    public class OxyPlotViewModel : BaseViewModel
    {
        private readonly OxyPlotPair _plotPair;

        public IPlotController ViewController => _plotPair.ViewController;

        public PlotModel Model => _plotPair.Model;

        public void Update(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            _plotPair.UpdateData(geographyProvider, booksReadProvider);
            OnPropertyChanged(() => ViewController);
            OnPropertyChanged(() => Model);
        }

        public OxyPlotViewModel(PlotTypes plotType)
        {
            // get the type.
            string title = plotType.GetTitle();
            bool? canHover = plotType.GetCanHover();

            BasePlotGenerator plotGenerator = null;
            switch (plotType)
            {
                case PlotTypes.AverageDaysPerBook:
                    plotGenerator = new AverageDaysPerBookPlotGenerator();
                    break;

                case PlotTypes.BooksAndPagesLastTen:
                    plotGenerator = new BooksAndPagesLastTenPlotGenerator();
                    break;

                case PlotTypes.BooksAndPagesLastTenTranslation:
                    plotGenerator = new BooksAndPagesLastTenTranslationPlotGenerator();
                    break;

                case PlotTypes.BooksAndPagesThisYear:
                    plotGenerator = new BooksAndPagesThisYearPlotGenerator();
                    break;

                case PlotTypes.BooksInTranslation:
                    plotGenerator = new BooksInTranslationPlotGenerator();
                    break;
            }

            _plotPair = new OxyPlotPair(plotGenerator, title, canHover.HasValue && canHover.Value);
        }
    }
}
