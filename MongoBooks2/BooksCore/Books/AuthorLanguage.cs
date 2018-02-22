// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorLanguage.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The authour language class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System;
    using System.Collections.Generic;

    public class AuthorLanguage
    {
        public string Language { get; set; }

        public int TotalBooksReadInLanguage
        {
            get
            {
                int total = 0;
                foreach (BookAuthor author in AuthorsInLanguage)
                {
                    total += author.TotalBooksReadBy;
                }

                return total;
            }
        }

        public uint TotalPagesReadInLanguage
        {
            get
            {
                uint total = 0;
                foreach (var author in AuthorsInLanguage)
                {
                    total += author.TotalPages;
                }

                return total;
            }
        }

        public List<BookAuthor> AuthorsInLanguage { get; set; }

        public int TotalBooksWorldWide { get; set; }

        public uint TotalPagesWorldWide { get; set; }

        public double PercentageOfBooksRead
        {
            get
            {
                if (TotalBooksWorldWide < 1) return 0.0;
                return 100.0 * (TotalBooksReadInLanguage / (double)TotalBooksWorldWide);
            }
        }

        public double PercentageOfPagesRead
        {
            get
            {
                if (TotalPagesWorldWide < 1) return 0.0;
                return 100.0 * (TotalPagesReadInLanguage / (double)TotalPagesWorldWide);
            }
        }

        public AuthorLanguage()
        {
            Language = string.Empty;
            AuthorsInLanguage = new List<BookAuthor>();
            TotalPagesWorldWide = 1;
            TotalBooksWorldWide = 1;
        }
    }
}
