namespace AngularMongoBooks3.Controllers.DataClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BooksCore.Books;

    public class YearlyTally
    {
        #region Constants

        public const string EnglishLanguage = "English";

        public const int DaysPerYear = 365;

        #endregion

        #region Tally Dates

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the day of year.
        /// </summary>
        public int DayOfYear { get; set; }

        /// <summary>
        /// Gets or sets the days since the start.
        /// </summary>
        public int DaysSinceStart { get; set; }

        #endregion

        #region Totals Since Start

        /// <summary>
        /// Gets or sets the total pages read since the start.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total books read since the start.
        /// </summary>
        public int TotalBooks { get; set; }

        /// <summary>
        /// Gets or sets the total books read in English since the start.
        /// </summary>
        public int TotalBooksInEnglish { get; set; }

        /// <summary>
        /// Gets or sets the total in book format since the start.
        /// </summary>
        public int TotalBookFormat { get; set; }

        /// <summary>
        /// Gets or sets the total in comic format since the start.
        /// </summary>
        public int TotalComicFormat { get; set; }

        /// <summary>
        /// Gets or sets the total in audio format since the start.
        /// </summary>
        public int TotalAudioFormat { get; set; }

        #endregion

        #region Since Start Rates

        /// <summary>
        /// Gets or sets the total page rate.
        /// </summary>
        public float TotalPageRate { get; set; }

        /// <summary>
        /// Gets or sets the total days per book.
        /// </summary>
        public float TotalDaysPerBook { get; set; }

        /// <summary>
        /// Gets or sets the total pages per book.
        /// </summary>
        public float TotalPagesPerBook { get; set; }

        /// <summary>
        /// Gets or sets the total percentage of books read in English.
        /// </summary>
        public float TotalBooksPercentageInEnglish { get; set; }

        #endregion

        #region Year Delta Totals

        /// <summary>
        /// Gets or sets the days in the year delta.
        /// </summary>
        public int DaysInYearDelta { get; set; }

        /// <summary>
        /// Gets or sets the total pages read in the year delta.
        /// </summary>
        public int YearDeltaPages { get; set; }

        /// <summary>
        /// Gets or sets the total books read in the year delta.
        /// </summary>
        public int YearDeltaBooks { get; set; }

        /// <summary>
        /// Gets or sets the total books read in English in the year delta.
        /// </summary>
        public int YearDeltaBooksInEnglish { get; set; }

        /// <summary>
        /// Gets or sets the total in book format in the year delta.
        /// </summary>
        public int YearDeltaBookFormat { get; set; }

        /// <summary>
        /// Gets or sets the total in comic format in the year delta.
        /// </summary>
        public int YearDeltaComicFormat { get; set; }

        /// <summary>
        /// Gets or sets the total in audio format in the year delta.
        /// </summary>
        public int YearDeltaAudioFormat { get; set; }

        #endregion

        #region Year Delta Rates

        /// <summary>
        /// Gets or sets the page rate in the year delta.
        /// </summary>
        public float YearDeltaPageRate { get; set; }

        /// <summary>
        /// Gets or sets the days per book in the year delta.
        /// </summary>
        public float YearDeltaDaysPerBook { get; set; }

        /// <summary>
        /// Gets or sets the pages per book in the year delta.
        /// </summary>
        public float YearDeltaPagesPerBook { get; set; }

        /// <summary>
        /// Gets or sets the percentage of books read in English in the year delta.
        /// </summary>
        public float YearDeltaPercentageInEnglish { get; set; }

        #endregion

        #region Local Utility Functions

        private void UpdateTotalsSinceStart(YearlyTally currentTally)
        {
            TotalPages = currentTally.TotalPages;
            TotalBooks = currentTally.TotalBooks;
            TotalBooksInEnglish = currentTally.TotalBooksInEnglish;
            TotalBookFormat = currentTally.TotalBookFormat;
            TotalComicFormat = currentTally.TotalComicFormat;
            TotalAudioFormat = currentTally.TotalAudioFormat;

            if (DaysSinceStart >= 1)
            {
                TotalPageRate = TotalPages / (float)DaysSinceStart;
            }

            if (TotalBooks >= 1)
            {
                TotalDaysPerBook = DaysSinceStart / (float)TotalBooks;
                TotalPagesPerBook = TotalPages / (float)TotalBooks;
                TotalBooksPercentageInEnglish = TotalBooksInEnglish / (float)TotalBooks;
            }
        }

        private void UpdateTotalsYearDelta(YearlyTally previousYearTally)
        {
            DaysInYearDelta = DaysSinceStart - previousYearTally.DaysSinceStart;
            YearDeltaPages = TotalPages - previousYearTally.TotalPages;
            YearDeltaBooks = TotalBooks - previousYearTally.TotalBooks;
            YearDeltaBooksInEnglish = TotalBooksInEnglish - previousYearTally.TotalBooksInEnglish;
            YearDeltaBookFormat = TotalBookFormat - previousYearTally.TotalBookFormat;
            YearDeltaComicFormat = TotalComicFormat - previousYearTally.TotalComicFormat;
            YearDeltaAudioFormat = TotalAudioFormat - previousYearTally.TotalAudioFormat;

            YearDeltaPageRate = YearDeltaPages / (float)DaysInYearDelta;

            if (YearDeltaBooks >= 1)
            {
                TotalDaysPerBook = DaysInYearDelta / (float)YearDeltaBooks;
                TotalPagesPerBook = YearDeltaPages / (float)YearDeltaBooks;
                TotalBooksPercentageInEnglish = YearDeltaBooksInEnglish / (float)YearDeltaBooks;
            }
        }

        #endregion

        #region Public Static Methods

        public static List<YearlyTally> GetYearlyTalliesForBooks(List<BookRead> allBooks)
        {
            List<BookRead> sortedBooks = allBooks.OrderBy(x => x.Date).ToList();
            DateTime firstDay = sortedBooks.First().Date;
            int totalDays = 1 + (int)(sortedBooks.Last().Date - firstDay).TotalDays;

            YearlyTally[] yearlyTallies = new YearlyTally[totalDays];

            for (int i = 0; i < totalDays; i++)
            {
                yearlyTallies[i] = new YearlyTally(firstDay, i);
            }

            YearlyTally currentTally = new YearlyTally();

            // fill in the days with books
            Dictionary<int, int> updatedDays = new Dictionary<int, int>();
            foreach (BookRead book in allBooks)
            {
                int daysSinceStart = (int)(book.Date - firstDay).TotalDays;

                currentTally.TotalPages += book.Pages;
                currentTally.TotalBooks++;

                if (book.OriginalLanguage == EnglishLanguage)
                    currentTally.TotalBooksInEnglish++;

                if (book.Format == BookFormat.Book)
                    currentTally.TotalBookFormat++;
                if (book.Format == BookFormat.Comic)
                    currentTally.TotalComicFormat++;
                if (book.Format == BookFormat.Audio)
                    currentTally.TotalAudioFormat++;

                yearlyTallies[daysSinceStart].UpdateTotalsSinceStart(currentTally);

                if (!updatedDays.ContainsKey(daysSinceStart))
                    updatedDays.Add(daysSinceStart, daysSinceStart);
            }

            // fill in the missing days with the totals for the day before
            for (int i = 1; i < totalDays; i++)
            {
                if (updatedDays.ContainsKey(i))
                    continue;

                yearlyTallies[i].UpdateTotalsSinceStart(yearlyTallies[i-1]);
            }


            for (int i = DaysPerYear; i < totalDays; i++)
            {
                var previousYearTally = yearlyTallies[i - DaysPerYear];
                yearlyTallies[i].UpdateTotalsYearDelta(previousYearTally);
            }

            return yearlyTallies.ToList();
        }

        #endregion

        #region Constructors

        public YearlyTally()
        {
            Date = DateTime.Now;
            Year = Date.Year;
            DayOfYear = Date.DayOfYear;

            DaysSinceStart = 0;
            TotalPages = 0;
            TotalBooks = 0;
            TotalBooksInEnglish = 0;
            TotalBookFormat = 0;
            TotalComicFormat = 0;
            TotalAudioFormat = 0;

            TotalPageRate = 0f;
            TotalDaysPerBook = 0f;
            TotalPagesPerBook = 0f;
            TotalBooksPercentageInEnglish = 0f;

            DaysInYearDelta = 0;
            YearDeltaPages = 0;
            YearDeltaBooks = 0;
            YearDeltaBooksInEnglish = 0;
            YearDeltaBookFormat = 0;
            YearDeltaComicFormat = 0;
            YearDeltaAudioFormat = 0;

            YearDeltaPageRate = 0f;
            YearDeltaDaysPerBook = 0f;
            YearDeltaPagesPerBook = 0f;
            YearDeltaPercentageInEnglish = 0f;
        }

        public YearlyTally(DateTime firstDay, int daysSinceStart)
        {
            DaysSinceStart = daysSinceStart;
            Date = firstDay.AddDays(daysSinceStart);
            Year = Date.Year;
            DayOfYear = Date.DayOfYear;

            TotalPages = 0;
            TotalBooks = 0;
            TotalBooksInEnglish = 0;
            TotalBookFormat = 0;
            TotalComicFormat = 0;
            TotalAudioFormat = 0;

            TotalPageRate = 0f;
            TotalDaysPerBook = 0f;
            TotalPagesPerBook = 0f;
            TotalBooksPercentageInEnglish = 0f;

            DaysInYearDelta = 0;
            YearDeltaPages = 0;
            YearDeltaBooks = 0;
            YearDeltaBooksInEnglish = 0;
            YearDeltaBookFormat = 0;
            YearDeltaComicFormat = 0;
            YearDeltaAudioFormat = 0;

            YearDeltaPageRate = 0f;
            YearDeltaDaysPerBook = 0f;
            YearDeltaPagesPerBook = 0f;
            YearDeltaPercentageInEnglish = 0f;
        }

        #endregion
    }
}
