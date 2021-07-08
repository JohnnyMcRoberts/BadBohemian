namespace BooksControllerUtilities.DataClasses
{
    using System;

    using BooksCore.Books;

    public class MonthlyTally
    {
        /// <summary>
        /// Gets or sets the date time for the month.
        /// </summary>
        public DateTime MonthDate { get; set; }

        /// <summary>
        /// Gets the month as a string to display.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of days in the month.
        /// </summary>
        public int DaysInTheMonth { get; set; }

        /// <summary>
        /// Gets the total books read in the month.
        /// </summary>
        public int TotalBooks { get; set; }

        /// <summary>
        /// Gets the total pages read in the month.
        /// </summary>
        public int TotalPagesRead { get; set; }

        /// <summary>
        /// Gets the total number of physical books for the month.
        /// </summary>
        public int TotalBookFormat { get; set; }

        /// <summary>
        /// Gets the total number of comics for the month.
        /// </summary>
        public int TotalComicFormat { get; set; }

        /// <summary>
        /// Gets the total number of audiobooks for the month.
        /// </summary>
        public int TotalAudioFormat { get; set; }

        /// <summary>
        /// Gets the percentage of books read in English for the month.
        /// </summary>
        public float PercentageInEnglish { get; set; }

        /// <summary>
        /// Gets the percentage of books read in translation for the month.
        /// </summary>
        public float PercentageInTranslation { get; set; }

        /// <summary>
        /// Gets the page rate for the month.
        /// </summary>
        public float PageRate { get; set; }

        /// <summary>
        /// Gets the average days per book for the month.
        /// </summary>
        public float DaysPerBook { get; set; }

        /// <summary>
        /// Gets the average pages per book for the month.
        /// </summary>
        public float PagesPerBook { get; set; }

        /// <summary>
        /// Gets the expected books per year for the month.
        /// </summary>
        public float BooksPerYear { get; set; }

        /// <summary>
        /// Gets or sets the set of books read in this month.
        /// </summary>
        public Book[] Books { get; set; }

        public MonthlyTally()
        {

        }

        public MonthlyTally(TalliedMonth talliedBook)
        {
            MonthDate = talliedBook.MonthDate;
            Name = talliedBook.DisplayString;
            DaysInTheMonth = talliedBook.DaysInTheMonth;
            TotalBooks = (int)talliedBook.TotalBooks;
            TotalPagesRead = (int)talliedBook.TotalPagesRead;
            TotalBookFormat = (int)talliedBook.TotalBookFormat;
            TotalComicFormat = (int)talliedBook.TotalComicFormat;
            TotalAudioFormat = (int)talliedBook.TotalAudioFormat;
            PercentageInEnglish = (float)talliedBook.PercentageInEnglish;
            PercentageInTranslation = (float)talliedBook.PercentageInTranslation;
            PageRate = (float)talliedBook.PageRate;
            DaysPerBook = (float)talliedBook.DaysPerBook;
            PagesPerBook = (float)talliedBook.PagesPerBook;
            BooksPerYear = (float)talliedBook.BooksPerYear;

            Books = new Book[talliedBook.BooksRead.Count];
            for (int i = 0; i < talliedBook.BooksRead.Count; i++)
            {
                Books[i] = new Book(talliedBook.BooksRead[i]);
            }
        }

    }
}
