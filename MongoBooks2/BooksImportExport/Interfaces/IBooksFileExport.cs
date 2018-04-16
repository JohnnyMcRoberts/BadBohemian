// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBooksFileExport.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The Books export to file interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksImportExport.Interfaces
{
    using BooksCore.Provider;

    /// <summary>
    /// The books file export interface.
    /// </summary>
    public interface IBooksFileExport
    {
        /// <summary>
        /// Gets the export method name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the export file type extension.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the export file type filter.
        /// </summary>
        string Filter { get; }

        /// <summary>
        /// Writes the data for this export to the file specified.
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        /// <param name="geographyProvider">The geography data provider.</param>
        /// <param name="booksReadProvider">The books data provider.</param>
        /// <param name="errorMessage">The error message if unsuccessful.</param>
        /// <returns>True if written successfully, false otherwise.</returns>
        bool WriteToFile(
            string filename,
            GeographyProvider geographyProvider,
            BooksReadProvider booksReadProvider,
            out string errorMessage);
    }
}
