// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonthlyReportsTally.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The tally of the books read for a monthly tally.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System;

    /// <summary>
    /// The tally of the books read for a monthly tally.
    /// </summary>
    public class MonthlyReportsTally
    {
        /// <summary>
        /// Gets the title to display.
        /// </summary>
        public string DisplayTitle { get; set; }

        /// <summary>
        /// Gets or sets the number of days in the tallied period.
        /// </summary>
        public int TotalDays { get; set; }

        /// <summary>
        /// Gets the total books read in the month.
        /// </summary>
        public UInt32 TotalBooks { get; set; }

        /// <summary>
        /// Gets the total pages read in the month.
        /// </summary>
        public UInt32 TotalPagesRead { get; set; }

        /// <summary>
        /// Gets the total number of physical books for the month.
        /// </summary>
        public UInt32 TotalBookFormat { get; set; }

        /// <summary>
        /// Gets the total number of comics for the month.
        /// </summary>
        public UInt32 TotalComicFormat { get; set; }

        /// <summary>
        /// Gets the total number of audiobooks for the month.
        /// </summary>
        public UInt32 TotalAudioFormat { get; set; }

        /// <summary>
        /// Gets the percentage of books read in English for the month.
        /// </summary>
        public double PercentageInEnglish { get; set; }

        /// <summary>
        /// Gets the percentage of books read in translation for the month.
        /// </summary>
        public double PercentageInTranslation => 100.0 - PercentageInEnglish;

        /// <summary>
        /// Gets the page rate for the month.
        /// </summary>
        public double PageRate => TotalPagesRead / (double)TotalDays;

        /// <summary>
        /// Gets the average days per book for the month.
        /// </summary>
        public double DaysPerBook => TotalDays / (double)TotalBooks;

        /// <summary>
        /// Gets the average pages per book for the month.
        /// </summary>
        public double PagesPerBook => TotalPagesRead / (double)TotalBooks;

        /// <summary>
        /// Gets the expected books per year for the month.
        /// </summary>
        public double BooksPerYear => 365.25 / DaysPerBook;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyReportsTally"/> class.
        /// </summary>
        /// <param name="overallTally">The overall tally.</param>
        public MonthlyReportsTally(BooksDelta.DeltaTally overallTally)
        {
            DisplayTitle = "Overall";
            //TotalDays = talliedBook.;
            TotalDays = overallTally.DaysInTally;
            TotalBooks = overallTally.TotalBooks;
            TotalPagesRead = overallTally.TotalPages;
            TotalBookFormat = overallTally.TotalBookFormat;
            TotalComicFormat = overallTally.TotalComicFormat;
            TotalAudioFormat = overallTally.TotalAudioFormat;
            PercentageInEnglish = overallTally.PercentageInEnglish;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyReportsTally"/> class.
        /// </summary>
        /// <param name="selectedMonthTally">The selected month.</param>
        public MonthlyReportsTally(TalliedMonth selectedMonthTally)
        {
            DisplayTitle = selectedMonthTally.DisplayString;
            TotalDays = selectedMonthTally.DaysInTheMonth;
            TotalBooks = selectedMonthTally.TotalBooks;
            TotalPagesRead = selectedMonthTally.TotalPagesRead;
            TotalBookFormat = selectedMonthTally.TotalBookFormat;
            TotalComicFormat = selectedMonthTally.TotalComicFormat;
            TotalAudioFormat = selectedMonthTally.TotalAudioFormat;
            PercentageInEnglish = selectedMonthTally.PercentageInEnglish;
        }
    }
}
