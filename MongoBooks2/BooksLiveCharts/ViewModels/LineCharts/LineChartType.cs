// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The line chart types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
{
    using BooksLiveCharts.Utilities;

    public enum LineChartType
    {
        [ChartType(Title = "Pages per Day With Time",
            GeneratorClass = typeof(PagesPerDayWithTimeLineChartViewModel))]
        PagesPerDayWithTime,

        [ChartType(Title = "Average Days Per Book",
            GeneratorClass = typeof(AverageDaysPerBookLineChartViewModel))]
        AverageDaysPerBook,

        [ChartType(Title = "% Books In Translation",
            GeneratorClass = typeof(BooksInTranslationLineChartViewModel))]
        BooksInTranslation
    }
}
