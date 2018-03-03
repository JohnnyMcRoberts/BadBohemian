namespace BooksOxyCharts.Utilities
{
    using BooksOxyCharts.PlotGenerators;

    public enum PlotType
    {
        [PlotType(Title = "Average Days Per Book", 
            CanHover = false, 
            GeneratorClass = typeof(AverageDaysPerBookPlotGenerator))]
        AverageDaysPerBook,

        [PlotType(Title = "Last 10 Books Time vs Pages", 
            CanHover = false, 
            GeneratorClass = typeof(BooksAndPagesLastTenPlotGenerator))]
        BooksAndPagesLastTen,

        [PlotType(Title = "Last 10 Books Time vs Pages in Translation",
            CanHover = false, 
            GeneratorClass = typeof(BooksAndPagesLastTenTranslationPlotGenerator))]
        BooksAndPagesLastTenTranslation,

        [PlotType(Title = "Books and Pages per Year", 
            CanHover = false,
            GeneratorClass = typeof(BooksAndPagesThisYearPlotGenerator))]
        BooksAndPagesThisYear,

        [PlotType(Title = "% Books In Translation", 
            CanHover = false,
            GeneratorClass = typeof(BooksInTranslationPlotGenerator))]
        BooksInTranslation,

        [PlotType(Title = "Countries in Location with Books and Pages",
            CanHover = true,
            GeneratorClass = typeof(CountryLocationsBooksAndPagesPlotGenerator))]
        CountryLocationsBooksAndPages,

        [PlotType(Title = "Current Books Read by Country",
            CanHover = false,
            GeneratorClass = typeof(CurrentBooksReadByCountryPlotGenerator))]
        CurrentBooksReadByCountry,

        [PlotType(Title = "Current Pages Read by Country",
            CanHover = false,
            GeneratorClass = typeof(CurrentPagesReadByCountryPlotGenerator))]
        CurrentPagesReadByCountry,

        [PlotType(Title = "Days Per Book",
            CanHover = false,
            GeneratorClass = typeof(DaysPerBookPlotGenerator))]
        DaysPerBook,

        [PlotType(Title = "Days Per Book With Time",
            CanHover = false,
            GeneratorClass = typeof(DaysPerBookWithTimePlotGenerator))]
        DaysPerBookWithTime,

        [PlotType(Title = "Latitude With Time",
            CanHover = false,
            GeneratorClass = typeof(LatitudeWithTimePlotGenerator))]
        LatitudeWithTime,

        [PlotType(Title = "Longitude With Time",
            CanHover = false,
            GeneratorClass = typeof(LongitudeWithTimePlotGenerator))]
        LongitudeWithTime,

        [PlotType(Title = "Overall Book And Page Tallies",
            CanHover = false,
            GeneratorClass = typeof(OverallBookAndPageTalliesPlotGenerator))]
        OverallBookAndPageTallies,

        [PlotType(Title = "Monthly Pages As Columns Tallies By Calendar Year",
            CanHover = false,
            GeneratorClass = typeof(MonthlyPagesAsColumnsTalliesByCalendarYearPlotGenerator))]
        MonthlyPagesAsColumnsTalliesByCalendarYear,

        [PlotType(Title = "Monthly Books As Columns Tallies By Calendar Year",
            CanHover = false,
            GeneratorClass = typeof(MonthlyBooksAsColumnsTalliesByCalendarYearPlotGenerator))]
        MonthlyBooksAsColumnsTalliesByCalendarYear,

        [PlotType(Title = "Monthly Books And Pages Lines Tallies By Calendar Year",
            CanHover = false,
            GeneratorClass = typeof(MonthlyBooksAndPagesLinesTalliesByCalendarYearPlotGenerator))]
        MonthlyBooksAndPagesLinesTalliesByCalendarYearPlotGenerator
    }
}
