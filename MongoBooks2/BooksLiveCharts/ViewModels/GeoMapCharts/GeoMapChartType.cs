// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeoMapChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The geo map chart types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.GeoMapCharts
{
    using BooksLiveCharts.Utilities;

    public enum GeoMapChartType
    {
        [ChartType(Title = "Books Per Country",
            GeneratorClass = typeof(BooksPerCountryMapChartViewModel))]
        BooksPerCountry,

        [ChartType(Title = "Pages Per Country",
            GeneratorClass = typeof(PagesPerCountryMapChartViewModel))]
        PagesPerCountry,

        [ChartType(Title = "Authors Per Country",
            GeneratorClass = typeof(AuthorsPerCountryMapChartViewModel))]
        AuthorsPerCountry
    }
}
