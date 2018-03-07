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
        MonthlyBooksAndPagesLinesTalliesByCalendarYearPlotGenerator,

        [PlotType(Title = "Page Rate",
            CanHover = false,
            GeneratorClass = typeof(PageRatePlotGenerator))]
        PageRatePlotGenerator,

        [PlotType(Title = "Pages Per Book",
            CanHover = false,
            GeneratorClass = typeof(PagesPerBookPlotGenerator))]
        PagesPerBookPlotGenerator,

        [PlotType(Title = "Pages Per Day With Time",
            CanHover = false,
            GeneratorClass = typeof(PagesPerDayWithTimePlotGenerator))]
        PagesPerDayWithTime,

        [PlotType(Title = "Percentage Books Read By Country",
            CanHover = false,
            GeneratorClass = typeof(PercentageBooksReadByCountryPlotGenerator))]
        PercentageBooksReadByCountry,

        [PlotType(Title = "Percentage Books Read By Language",
            CanHover = false,
            GeneratorClass = typeof(PercentageBooksReadByLanguagePlotGenerator))]
        PercentageBooksReadByLanguage,

        [PlotType(Title = "Percentage Pages Read By Country",
            CanHover = false,
            GeneratorClass = typeof(PercentagePagesReadByCountryPlotGenerator))]
        PercentagePagesReadByCountry,

        [PlotType(Title = "Percentage Pages Read By Language",
            CanHover = false,
            GeneratorClass = typeof(PercentagePagesReadByLanguagePlotGenerator))]
        PercentagePagesReadByLanguage,

        [PlotType(Title = "Tallies Per Calendar Year",
            CanHover = false,
            GeneratorClass = typeof(TalliesPerCalendarYearPlotGenerator))]
        TalliesPerCalendarYear,

        [PlotType(Title = "Total Books Read By Country",
            CanHover = false,
            GeneratorClass = typeof(TotalBooksReadByCountryPlotGenerator))]
        TotalBooksReadByCountryPlotGenerator,

        [PlotType(Title = "Total Books Read By Language",
            CanHover = false,
            GeneratorClass = typeof(TotalBooksReadByLanguagePlotGenerator))]
        TotalBooksReadByLanguage,

        [PlotType(Title = "Total Pages Read By Country",
            CanHover = false,
            GeneratorClass = typeof(TotalPagesReadByCountryPlotGenerator))]
        TotalPagesReadByCountryPlotGenerator,

        [PlotType(Title = "Total Pages Read By Language",
            CanHover = false,
            GeneratorClass = typeof(TotalPagesReadByLanguagePlotGenerator))]
        TotalPagesReadByLanguage,

        [PlotType(Title = "World Countries Map Books Read",
            CanHover = false,
            GeneratorClass = typeof(WorldCountriesMapBooksReadPlotGenerator))]
        WorldCountriesMapBooksReadPlotGenerator,

        [PlotType(Title = "World Countries Map Last Ten Lat Long",
            CanHover = false,
            GeneratorClass = typeof(WorldCountriesMapLastTenLatLongPlotGenerator))]
        WorldCountriesMapLastTenLatLongPlotGenerator,

        [PlotType(Title = "World Countries Map Pages Read",
            CanHover = false,
            GeneratorClass = typeof(WorldCountriesMapPagesReadPlotGenerator))]
        WorldCountriesMapPagesReadPlotGenerator,

        [PlotType(Title = "World Countries Map",
            CanHover = false,
            GeneratorClass = typeof(WorldCountriesMapPlotGenerator))]
        WorldCountriesMapPlotGenerator,

        [PlotType(Title = "World Countries Map With Books",
            CanHover = false,
            GeneratorClass = typeof(WorldCountriesMapWithBooksReadPlotGenerator))]
        WorldCountriesMapWithBooksReadPlotGenerator
    }
}
