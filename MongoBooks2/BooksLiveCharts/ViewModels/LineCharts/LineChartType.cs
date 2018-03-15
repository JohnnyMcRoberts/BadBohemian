// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PieChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The pie-chart types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
{
    using BooksLiveCharts.Utilities;

    public enum LineChartType
    {
        [ChartType(Title = "Pages per Day With Time",
            GeneratorClass = typeof(PagesPerDayWithTimeLineChartModel))]
        PagesPerDayWithTime,

        //[ChartType(Title = "Current Pages Read By Country",
        //    GeneratorClass = typeof(CurrentPagesReadByCountryPieChartViewModel))]
        //CurrentPagesReadByCountry
    }
}
