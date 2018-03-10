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
