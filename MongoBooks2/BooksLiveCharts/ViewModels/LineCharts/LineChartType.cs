﻿// --------------------------------------------------------------------------------------------------------------------
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

    /// <summary>
    /// Type of line chart.
    /// </summary>
    public enum LineChartType
    {
        [ChartType(Title = "Pages per Day with time",
            GeneratorClass = typeof(PagesPerDayWithTimeLineChartViewModel))]
        PagesPerDayWithTime,

        [ChartType(Title = "Average Days per Book",
            GeneratorClass = typeof(AverageDaysPerBookLineChartViewModel))]
        AverageDaysPerBook,

        [ChartType(Title = "% Books in Translation",
            GeneratorClass = typeof(BooksInTranslationLineChartViewModel))]
        BooksInTranslation,

        [ChartType(Title = "Days per book with time",
            GeneratorClass = typeof(DaysPerBookLineChartViewModel))]
        DaysPerBook,

        [ChartType(Title = "Page rate with time",
            GeneratorClass = typeof(PageRateLineChartViewModel))]
        PageRate,

        [ChartType(Title = "Percentage Books Read by Country With Time",
            GeneratorClass = typeof(PercentageBooksReadByCountryLineChartViewModel))]
        PercentageBooksReadByCountry,

        [ChartType(Title = "Percentage Books Read by Language With Time",
            GeneratorClass = typeof(PercentageBooksReadByLanguageLineChartViewModel))]
        PercentageBooksReadByLanguage,

        [ChartType(Title = "Percentage Pages Read by Country With Time",
            GeneratorClass = typeof(PercentagePagesReadByCountryLineChartViewModel))]
        PercentagePagesReadByCountry,

        [ChartType(Title = "Percentage Pages Read by Language With Time",
            GeneratorClass = typeof(PercentagePagesReadByLanguageLineChartViewModel))]
        PercentagePagesReadByLanguage,

        [ChartType(Title = "Monthly Pages Read Tallies by Year",
            GeneratorClass = typeof(MonthlyPageTalliesPerYearLineChartViewModel))]
        MonthlyPageTalliesPerYear,

        [ChartType(Title = "Monthly Books Read Tallies by Year",
            GeneratorClass = typeof(MonthlyBookTalliesPerYearLineChartViewModel))]
        MonthlyBookTalliesPerYear,

        [ChartType(Title = "Monthly Pages Read Totals by Year",
            GeneratorClass = typeof(MonthlyPageTotalsPerYearLineChartViewModel))]
        MonthlyPageTotalsPerYear,

        [ChartType(Title = "Monthly Books Read Totals by Year",
            GeneratorClass = typeof(MonthlyBookTotalsPerYearLineChartViewModel))]
        MonthlyBookTotalsPerYear
    }
}
