namespace BooksOxyCharts.Utilities
{
    public enum PlotTypes
    {
        [PlotType(Title = "Average Days Per Book Plot", CanHover = false)]
        AverageDaysPerBook,

        [PlotType(Title = "Last 10 Books Time vs Pages Plot", CanHover = false)]
        BooksAndPagesLastTen,

        [PlotType(Title = "Last 10 Books Time vs Pages in Translation Plot", CanHover = false)]
        BooksAndPagesLastTenTranslation,

        [PlotType(Title = "Books and Pages per Year Plot", CanHover = false)]
        BooksAndPagesThisYear,

        [PlotType(Title = "% Books In Translation Plot", CanHover = false)]
        BooksInTranslation
    }
}
