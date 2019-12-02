namespace AngularMongoBooks3.Controllers.DataClasses
{
    using System;

    using BooksCore.Books;

    public class CategoryTotal
    {
        public string Name { get; set; }
        public int TotalPages { get; set; }
        public int TotalBooks { get; set; }
        public float PercentagePages { get; set; }
        public float PercentageBooks { get; set; }

        public CategoryTotal()
        {
            Name = string.Empty;
            TotalPages = 0;
            TotalBooks = 0;
            PercentagePages = 0f;
            PercentageBooks = 0f;
        }

        public CategoryTotal(CategoryTotal src)
        {
            Name = src.Name;
            TotalPages = src.TotalPages;
            TotalBooks = src.TotalBooks;
            PercentagePages = src.PercentagePages;
            PercentageBooks = src.PercentageBooks;
        }
    }

    public class DeltaBooks
    {
        public DateTime Date { get; set; }

        public DateTime StartDate { get; set; }

        public int DaysSinceStart { get; set; }

        public TallyDelta OverallTally { get; set; }

        public TallyDelta LastTenTally { get; set; }

        public CategoryTotal[] LanguageTotals { get; set; }

        public CategoryTotal[] CountryTotals { get; set; }

        public DeltaBooks()
        {
            Date = DateTime.Now;
            StartDate = DateTime.Now;
            DaysSinceStart = 0;
            OverallTally = new TallyDelta();
            LastTenTally = new TallyDelta();
            LanguageTotals = new CategoryTotal[] { };
            CountryTotals = new CategoryTotal[] { };
        }

        public DeltaBooks(BooksDelta booksDelta)
        {
            Date = booksDelta.Date;
            StartDate = booksDelta.StartDate;
            DaysSinceStart = booksDelta.DaysSinceStart;
            OverallTally = new TallyDelta(booksDelta.OverallTally);
            LastTenTally = new TallyDelta(booksDelta.LastTenTally);

            int languageTotalsCount = booksDelta.OverallTally.LanguageTotals.Count;
            LanguageTotals = new CategoryTotal[languageTotalsCount];
            for (int i = 0; i < languageTotalsCount; i++)
            {
                var languageTotal = booksDelta.OverallTally.LanguageTotals[i];
                LanguageTotals[i] =
                    new CategoryTotal
                    {
                        Name = languageTotal.Item1,
                        TotalBooks = (int)languageTotal.Item2,
                        PercentageBooks = (float)languageTotal.Item3,
                        TotalPages = (int)languageTotal.Item4,
                        PercentagePages = (float)languageTotal.Item5
                    };
            }


            int countryTotalsCount = booksDelta.OverallTally.CountryTotals.Count;
            CountryTotals = new CategoryTotal[countryTotalsCount];
            for (int i = 0; i < countryTotalsCount; i++)
            {
                var countryTotal = booksDelta.OverallTally.CountryTotals[i];
                CountryTotals[i] =
                    new CategoryTotal
                    {
                        Name = countryTotal.Item1,
                        TotalBooks = (int)countryTotal.Item2,
                        PercentageBooks = (float)countryTotal.Item3,
                        TotalPages = (int)countryTotal.Item4,
                        PercentagePages = (float)countryTotal.Item5
                    };
            }
        }
    }
}
