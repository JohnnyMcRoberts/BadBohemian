// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.Interfaces
{
    using BooksCore.Interfaces;
    using OxyPlot;

    public interface IPlotGenerator
    {
        PlotModel SetupPlot(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider);
    }
}
