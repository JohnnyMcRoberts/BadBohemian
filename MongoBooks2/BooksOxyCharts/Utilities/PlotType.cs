namespace BooksOxyCharts.Utilities
{
    using BooksOxyCharts.PlotGenerators;

    public enum PlotType
    {
        [PlotType(Title = "Average Days Per Book Plot", 
            CanHover = false, 
            GeneratorClass = typeof(AverageDaysPerBookPlotGenerator))]
        AverageDaysPerBook,

        [PlotType(Title = "Last 10 Books Time vs Pages Plot", 
            CanHover = false, 
            GeneratorClass = typeof(BooksAndPagesLastTenPlotGenerator))]
        BooksAndPagesLastTen,

        [PlotType(Title = "Last 10 Books Time vs Pages in Translation Plot",
            CanHover = false, 
            GeneratorClass = typeof(BooksAndPagesLastTenTranslationPlotGenerator))]
        BooksAndPagesLastTenTranslation,

        [PlotType(Title = "Books and Pages per Year Plot", 
            CanHover = false,
            GeneratorClass = typeof(BooksAndPagesThisYearPlotGenerator))]
        BooksAndPagesThisYear,

        [PlotType(Title = "% Books In Translation Plot", 
            CanHover = false,
            GeneratorClass = typeof(BooksInTranslationPlotGenerator))]
        BooksInTranslation,

        [PlotType(Title = "Countries in Location with Books and Pages Plot",
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

        [PlotType(Title = "Days Per Book Plot",
            CanHover = false,
            GeneratorClass = typeof(DaysPerBookPlotGenerator))]
        DaysPerBook
    }
}
