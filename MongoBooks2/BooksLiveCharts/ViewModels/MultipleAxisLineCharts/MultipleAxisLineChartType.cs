// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The multiple axis line chart types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.MultipleAxisLineCharts
{
    using BooksLiveCharts.Utilities;

    /// <summary>
    /// Type of multiple axis line chart.
    /// </summary>
    public enum MultipleAxisLineChartType
    {
        [ChartType(Title = "Books and pages read with time",
            GeneratorClass = typeof(TotalBooksAndPagesReadMultipleAxisLineChartViewModel))]
        BooksAndPagesThisYear,
    }
}
