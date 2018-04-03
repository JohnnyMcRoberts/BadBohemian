// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StackedAreaChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The stacked area chart types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.StackedAreaCharts
{
    using BooksLiveCharts.Utilities;

    /// <summary>
    /// Type of stacked area line chart.
    /// </summary>
    public enum StackedAreaChartType
    {
        [ChartType(Title = "Total Books Read By Country",
            GeneratorClass = typeof(TotalBooksReadByCountryStackedAreaChartViewModel))]
        TotalBooksReadByCountry,

        [ChartType(Title = "Total Books Read By Language",
            GeneratorClass = typeof(TotalBooksReadByLanguageStackedAreaChartViewModel))]
        TotalBooksReadByLanguage,

        [ChartType(Title = "Total Pages Read By Country",
            GeneratorClass = typeof(TotalPagesReadByCountryStackedAreaChartViewModel))]
        TotalPagesReadByCountry,

        [ChartType(Title = "Total Pages Read By Language",
            GeneratorClass = typeof(TotalPagesReadByLanguageStackedAreaChartViewModel))]
        TotalPagesReadByLanguage
    }
}
