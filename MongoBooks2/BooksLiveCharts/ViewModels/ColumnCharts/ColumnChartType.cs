// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The column chart types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.ColumnCharts
{
    using BooksLiveCharts.Utilities;

    /// <summary>
    /// Type of column chart.
    /// </summary>
    public enum ColumnChartType
    {
        [ChartType(Title = "Monthly Pages By Calendar Year",
            GeneratorClass = typeof(MonthlyPagesTalliesByCalendarYearColumnChartViewModel))]
        MonthlyPagesTalliesByCalendarYear,

        [ChartType(Title = "Monthly Books By Calendar Year",
            GeneratorClass = typeof(MonthlyBooksTalliesByCalendarYearColumnChartViewModel))]
        MonthlyBooksTalliesByCalendarYear
    }
}
