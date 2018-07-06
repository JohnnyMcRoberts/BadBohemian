// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportType.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The import types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Exporters
{
    using BooksImportExport.Importers;
    using BooksImportExport.Utilities;

    /// <summary>
    /// Type of import.
    /// </summary>
    public enum ImportType
    {
        [ImportExportType(Title = "Books From csv", GeneratorClass = typeof(BooksFromCsvFileImport))]
        BooksFromCsv,

        [ImportExportType(Title = "Nations From xml", GeneratorClass = typeof(NationsFromXmlFileImport))]
        NationsFromXml
    }
}
