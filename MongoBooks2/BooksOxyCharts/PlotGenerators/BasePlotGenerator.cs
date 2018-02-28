// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using BooksCore.Interfaces;
    using BooksOxyCharts.Interfaces;
    using OxyPlot;

    public abstract class BasePlotGenerator : IPlotGenerator
    {
        public IGeographyProvider GeographyProvider { get; private set; }

        public IBooksReadProvider BooksReadProvider { get; private set; }

        public PlotModel SetupPlot(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            GeographyProvider = geographyProvider;
            BooksReadProvider = booksReadProvider;
            return SetupPlot();
        }

        protected abstract PlotModel SetupPlot();
    }
}
