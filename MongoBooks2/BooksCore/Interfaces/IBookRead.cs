// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBookRead.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The Book Read interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksCore.Interfaces
{
    using System;
    using BooksCore.Books;

    /// <summary>
    /// The Book Read interface.
    /// </summary>
    public interface IBookRead
    {
        /// <summary>
        /// Gets or sets the date string.
        /// </summary>
        string DateString { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        ushort Pages { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        BookFormat Format { get; set; }
    }
}
