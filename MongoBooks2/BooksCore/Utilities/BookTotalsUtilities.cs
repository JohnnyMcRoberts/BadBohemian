// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookTotalsUtilities.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The books totals utilities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BooksCore.Books;
    using BooksCore.Interfaces;

    public static class BookTotalsUtilities
    {
        public const int FirstMonth = 1;
        public const int LastMonth = 12;

        public static readonly string[] MonthNames =
        {
            "",
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
        };

        public static Dictionary<int, List<MonthOfYearTally>> GetBookListsByMonthOfYear(IBooksReadProvider booksReadProvider)
        {
            // get the books foreach year
            Dictionary<int, List<BookRead>> bookListsByYear = new Dictionary<int, List<BookRead>>();

            foreach (BookRead book in booksReadProvider.BooksRead)
            {
                int bookReadYear = book.Date.Year;
                if (bookListsByYear.ContainsKey(bookReadYear))
                {
                    bookListsByYear[bookReadYear].Add(book);
                }
                else
                {
                    bookListsByYear.Add(bookReadYear, new List<BookRead> { book });
                }
            }

            // for each year get the lists of books read each day
            Dictionary<int, Dictionary<int, List<BookRead>>> bookTotalsByMonthAndYear = new Dictionary<int, Dictionary<int, List<BookRead>>>();

            foreach (int year in bookListsByYear.Keys)
            {
                Dictionary<int, List<BookRead>> booksByMonthOfYear = new Dictionary<int, List<BookRead>>();
                foreach (BookRead book in bookListsByYear[year])
                {
                    int bookReadMonthOfYear = book.Date.Month;
                    if (booksByMonthOfYear.ContainsKey(bookReadMonthOfYear))
                    {
                        booksByMonthOfYear[bookReadMonthOfYear].Add(book);
                    }
                    else
                    {
                        booksByMonthOfYear.Add(bookReadMonthOfYear, new List<BookRead> { book });
                    }
                }

                bookTotalsByMonthAndYear.Add(year, booksByMonthOfYear);
            }

            // from these get the daily tallies for each year
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthAndYear = new Dictionary<int, List<MonthOfYearTally>>();
            foreach (int year in bookTotalsByMonthAndYear.Keys)
            {
                Dictionary<int, List<BookRead>> booksByMonthOfYear = bookTotalsByMonthAndYear[year];
                List<MonthOfYearTally> monthOfYearTallies = new List<MonthOfYearTally>();

                foreach (int monthOfYear in booksByMonthOfYear.Keys.ToList().OrderBy(x => x))
                {
                    List<BookRead> booksforMonth = booksByMonthOfYear[monthOfYear];
                    MonthOfYearTally tally = new MonthOfYearTally
                    {
                        MonthOfYear = monthOfYear,
                        BooksReadThisMonth = booksforMonth.Count,
                        PagesReadThisMonth = booksforMonth.Sum(x => x.Pages)
                    };

                    monthOfYearTallies.Add(tally);
                }

                bookListsByMonthAndYear.Add(year, monthOfYearTallies);
            }

            return bookListsByMonthAndYear;
        }

        public static List<KeyValuePair<string, int>> SortedSortedPagesReadByCountryTotals(IBooksReadProvider booksReadProvider)
        {
            BooksDelta currentResults = booksReadProvider.BookDeltas.Last();

            List<KeyValuePair<string, int>> countryTotals = new List<KeyValuePair<string, int>>();

            // Country, ttl books, ttl books %, ttl pages, ttl pages%
            //Tuple<string, UInt32, double, UInt32, double>

            int ttlOtherPages = 0;
            double ttlOtherPercentage = 0;
            foreach (var country in currentResults.OverallTally.CountryTotals)
            {
                string countryName = country.Item1;
                int ttlPages = (int)country.Item4;
                double countryPercentage = country.Item5;
                if (countryPercentage > 1.0)
                {
                    countryTotals.Add(new KeyValuePair<string, int>(countryName, ttlPages));
                }
                else
                {
                    ttlOtherPercentage += countryPercentage;
                    ttlOtherPages += ttlPages;
                }
            }

            List<KeyValuePair<string, int>> sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();

            if (ttlOtherPercentage > 1.0)
                sortedCountryTotals.Add(new KeyValuePair<string, int>("Other", ttlOtherPages));

            return sortedCountryTotals;
        }

        public static List<KeyValuePair<string, int>> SortedSortedBooksReadByCountryTotals(IBooksReadProvider booksReadProvider)
        {
            BooksDelta currentResults = booksReadProvider.BookDeltas.Last();

            List<KeyValuePair<string, int>> countryTotals = new List<KeyValuePair<string, int>>();

            // Country, ttl books, ttl books %, ttl pages, ttl pages%
            //Tuple<string, UInt32, double, UInt32, double>
            int ttlOtherBooks = 0;
            double ttlOtherPercentage = 0;
            foreach (Tuple<string, uint, double, uint, double> country in currentResults.OverallTally.CountryTotals)
            {
                string countryName = country.Item1;
                int ttlBooks = (int)country.Item2;
                double countryPercentage = country.Item3;
                if (countryPercentage > 1.0)
                {
                    countryTotals.Add(new KeyValuePair<string, int>(countryName, ttlBooks));
                }
                else
                {
                    ttlOtherPercentage += countryPercentage;
                    ttlOtherBooks += ttlBooks;
                }
            }

            List<KeyValuePair<string, int>> sortedCountryTotals = countryTotals.OrderByDescending(x => x.Value).ToList();

            if (ttlOtherPercentage > 1.0)
                sortedCountryTotals.Add(new KeyValuePair<string, int>("Other", ttlOtherBooks));
            return sortedCountryTotals;
        }
    }
}
