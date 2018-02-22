// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TalliedMonth.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The tallied books read in a month.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The tallied books read in a month.
    /// </summary>
    public class TalliedMonth
    {
        /// <summary>
        /// Gets or sets the date time for the month.
        /// </summary>
        public DateTime MonthDate { get; private set; }

        /// <summary>
        /// Gets the month as a string to display.
        /// </summary>
        public string DisplayString => MonthDate.ToString("MMMM yyyy");

        /// <summary>
        /// Gets or sets the number of days in the month.
        /// </summary>
        public int DaysInTheMonth { get; }

        /// <summary>
        /// Gets the total books read in the month.
        /// </summary>
        public uint TotalBooks { get; }

        /// <summary>
        /// Gets the total pages read in the month.
        /// </summary>
        public uint TotalPagesRead { get; }

        /// <summary>
        /// Gets the total number of physical books for the month.
        /// </summary>
        public uint TotalBookFormat { get; }

        /// <summary>
        /// Gets the total number of comics for the month.
        /// </summary>
        public uint TotalComicFormat { get; }

        /// <summary>
        /// Gets the total number of audiobooks for the month.
        /// </summary>
        public uint TotalAudioFormat { get; }

        /// <summary>
        /// Gets the percentage of books read in English for the month.
        /// </summary>
        public double PercentageInEnglish { get; }

        /// <summary>
        /// Gets the percentage of books read in translation for the month.
        /// </summary>
        public double PercentageInTranslation => 100.0 - PercentageInEnglish;

        /// <summary>
        /// Gets the page rate for the month.
        /// </summary>
        public double PageRate => TotalPagesRead / (double)DaysInTheMonth;

        /// <summary>
        /// Gets the average days per book for the month.
        /// </summary>
        public double DaysPerBook => DaysInTheMonth / (double)TotalBooks;

        /// <summary>
        /// Gets the average pages per book for the month.
        /// </summary>
        public double PagesPerBook => TotalPagesRead / (double)TotalBooks;

        /// <summary>
        /// Gets the expected books per year for the month.
        /// </summary>
        public double BooksPerYear => 365.25 / DaysPerBook;

        /// <summary>
        /// Gets or sets the list of books read in this month.
        /// </summary>
        public List<BookRead> BooksRead { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TalliedMonth" /> class.
        /// This constructor captures the selection rectangle for printing a region.
        /// </summary>
        /// <param name="date">The owning document view model.</param>
        /// <param name="booksRead">The document name.</param>
        public TalliedMonth(DateTime date, List<BookRead> booksRead)
        {
            MonthDate = date;
            BooksRead = booksRead;
            DaysInTheMonth = DateTime.DaysInMonth(date.Year, date.Month);
            
            TotalBooks = 0;
            TotalPagesRead = 0;
            TotalBookFormat = 0;
            TotalComicFormat = 0;
            TotalAudioFormat = 0;
            uint totalInEnglish = 0;

            // Loop through the books updating the counts as we go.
            foreach (BookRead book in BooksRead)
            {
                TotalBooks++;
                TotalPagesRead += book.Pages;

                if (book.Format == BookFormat.Book)
                    TotalBookFormat++;

                if (book.Format == BookFormat.Comic)
                    TotalComicFormat++;

                if (book.Format == BookFormat.Audio)
                    TotalAudioFormat++;

                if (book.OriginalLanguage == "English")
                    totalInEnglish++;
            }

            // Finally get the percentage read in English.
            PercentageInEnglish = BooksDelta.GetAsPercentage(TotalBooks, totalInEnglish);
        }
    }
}
