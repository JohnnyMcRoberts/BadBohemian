// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBooksFileImport.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The books file import interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksImportExport.Interfaces
{
    using System;
    using System.Collections.Generic;

    using BooksCore.Base;

    /// <summary>
    /// The books file import interface.
    /// </summary>
    public interface IBooksFileImport
    {
        /// <summary>
        /// Gets the import method name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the import file type extension.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the import file type filter.
        /// </summary>
        string Filter { get; }

        /// <summary>
        /// Gets the type of the items that are being imported from the file.
        /// </summary>
        Type ImportType { get; }

        /// <summary>
        /// Gets the list of imported items.
        /// </summary>
        List<BaseMongoEntity> ImportedItems { get; }

        /// <summary>
        /// Reads the data for this import from the file specified.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        /// <param name="errorMessage">The error message if unsuccessful.</param>
        /// <returns>True if read successfully, false otherwise.</returns>
        bool ReadFromFile(
            string filename,
            out string errorMessage);
    }
}
