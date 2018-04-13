// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBooksReadProvider.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The Books Read provider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Interfaces
{
    using System.Collections.ObjectModel;

    using BooksCore.Books;

    /// <summary>
    /// The Book Read provider interface.
    /// </summary>
    public interface IBooksReadProvider
    {
        /// <summary>
        /// Gets the books.
        /// </summary>
        ObservableCollection<BookRead> BooksRead { get; }

        /// <summary>
        /// Gets the book deltas.
        /// </summary>
        ObservableCollection<BooksDelta> BookDeltas { get; }

        /// <summary>
        /// Gets the changes per year between books.
        /// </summary>
        ObservableCollection<BooksDelta> BookPerYearDeltas { get; }

        /// <summary>
        /// Gets the book location deltas.
        /// </summary>
        ObservableCollection<BookLocationDelta> BookLocationDeltas { get; }

        /// <summary>
        /// Gets the authors.
        /// </summary>
        ObservableCollection<BookAuthor> AuthorsRead { get; }

        /// <summary>
        /// Gets the author countries.
        /// </summary>
        ObservableCollection<AuthorCountry> AuthorCountries { get; }

        /// <summary>
        /// Gets the author languages.
        /// </summary>
        ObservableCollection<AuthorLanguage> AuthorLanguages { get; }

        /// <summary>
        /// Gets the tallied months.
        /// </summary>
        ObservableCollection<TalliedMonth> TalliedMonths { get;}

        /// <summary>
        /// Gets or sets the selected month tally.
        /// </summary>
        TalliedMonth SelectedMonthTally { get; set; }
    }
}
