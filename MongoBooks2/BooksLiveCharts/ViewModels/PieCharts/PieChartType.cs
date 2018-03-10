// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PieChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The pie-chart types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.PieCharts
{
    using BooksLiveCharts.Utilities;

    public enum PieChartType
    {
        [ChartType(Title = "Current Books Read By Country",
            GeneratorClass = typeof(CurrentBooksReadByCountryPieChartViewModel))]
        CurrentBooksReadByCountry,

        [ChartType(Title = "Current Pages Read By Country",
            GeneratorClass = typeof(CurrentPagesReadByCountryPieChartViewModel))]
        CurrentPagesReadByCountry,

    }
}
