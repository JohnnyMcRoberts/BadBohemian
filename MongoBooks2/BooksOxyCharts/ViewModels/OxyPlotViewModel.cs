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
    using System;

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

        public OxyPlotViewModel(Utilities.PlotType plotType)
        {
            // Get the plot generator type etc and create theplot pair.
            string title = plotType.GetTitle();
            bool? canHover = plotType.GetCanHover();
            Type plotGeneratorType = plotType.GetGeneratorClass();
            var instance = Activator.CreateInstance(plotGeneratorType);
            BasePlotGenerator plotGenerator =  (BasePlotGenerator)instance;
            _plotPair = new OxyPlotPair(plotGenerator, title, canHover.HasValue && canHover.Value);
        }
    }
}
