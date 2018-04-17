// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportType.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The export types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Exporters
{
    using BooksImportExport.Utilities;

    /// <summary>
    /// Type of export.
    /// </summary>
    public enum ExportType
    {
        [ImportExportType(Title = "Books to csv", GeneratorClass = typeof(BooksToCsvFileExport))]
        BooksToCsv,

        [ImportExportType(Title = "Nations to xml", GeneratorClass = typeof(NationsToXmlFileExport))]
        NationsToXml,

        [ImportExportType(Title = "Selected month to html", GeneratorClass = typeof(SelectedMonthToHtmlFileExporter))]
        SelectedMonthToHtml
    }
}
