namespace BooksControllerUtilities.DataClasses
{
    using BooksCore.Books;

    public class TallyDelta
    {
        public int DaysInTally { get; set; }

        public int TotalPages { get; set; }

        public int TotalBooks { get; set; }

        public int TotalBookFormat { get; set; }

        public int TotalComicFormat { get; set; }

        public int TotalAudioFormat { get; set; }

        public float PercentageInEnglish { get; set; }

        public float PercentageInTranslation { get; set; }

        public float PageRate { get; set; }

        public float DaysPerBook { get; set; }

        public float PagesPerBook { get; set; }

        public float BooksPerYear { get; set; }

        public TallyDelta()
        {

        }

        public TallyDelta(BooksDelta.DeltaTally talliedBook)
        {
            DaysInTally = (int)talliedBook.DaysInTally;
            TotalPages = (int)talliedBook.TotalPages;
            TotalBooks = (int)talliedBook.TotalBooks;
            TotalBookFormat = (int)talliedBook.TotalBookFormat;
            TotalComicFormat = (int)talliedBook.TotalComicFormat;
            TotalAudioFormat = (int)talliedBook.TotalAudioFormat;

            PercentageInEnglish = (float)talliedBook.PercentageInEnglish;
            PercentageInTranslation = (float)talliedBook.PercentageInTranslation;
            PageRate = (float)talliedBook.PageRate;
            DaysPerBook = (float)talliedBook.DaysPerBook;
            PagesPerBook = (float)talliedBook.PagesPerBook;
            BooksPerYear = (float)talliedBook.BooksPerYear;
        }

    }
}
