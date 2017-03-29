using System;
using System.Collections.Generic;
using System.Linq;


namespace MongoDbBooks.Models
{
    public class AuthorCountry
    {
        public string Country { get; set; }

        public UInt32 TotalPagesReadFromCountry
        {
            get
            {
                return AuthorsFromCountry.Aggregate<BookAuthor, uint>(0, (current, author) => current + author.TotalPages);
            }
        }

        public int TotalBooksReadFromCountry
        {
            get
            {
                return AuthorsFromCountry.Sum(author => author.TotalBooksReadBy);
            }
        }

        public List<BookAuthor> AuthorsFromCountry { get; set; }

        public UInt32 TotalPagesWorldWide { get; set; }
        public int TotalBooksWorldWide { get; set; }

        public double PercentageOfBooksRead
        {
            get
            {
                if (TotalBooksWorldWide < 1) return 0.0;
                return 100.0 * ((double)TotalBooksReadFromCountry / (double)TotalBooksWorldWide);
            }
        }

        public double PercentageOfPagesRead
        {
            get
            {
                if (TotalPagesWorldWide < 1) return 0.0;
                return 100.0 * ((double)TotalPagesReadFromCountry / (double)TotalPagesWorldWide);
            }
        }

        public AuthorCountry()
        {
            Country = "";
            AuthorsFromCountry = new List<BookAuthor>();
            TotalPagesWorldWide = 1;
            TotalBooksWorldWide = 1;
        }
    }
}
