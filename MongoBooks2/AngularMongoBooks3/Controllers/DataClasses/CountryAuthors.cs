namespace AngularMongoBooks3.Controllers.DataClasses
{
    using System;
    using BooksCore.Books;

    public class CountryAuthors
    {
        public string Name { get; set; }

        public int TotalBooksReadInLanguage { get; set; }

        public int TotalPagesReadInLanguage { get; set; }

        public int TotalBooksWorldWide { get; set; }

        public int TotalPagesWorldWide { get; set; }

        public float PercentageOfBooksRead { get; set; }

        public float PercentageOfPagesRead { get; set; }

        public string FlagUrl { get; set; }

        public Author[] Authors { get; set; }

        public CountryAuthors()
        {

        }

        public CountryAuthors(AuthorCountry authorCountry)
        {
            Name = authorCountry.Country;
            TotalBooksReadInLanguage = authorCountry.TotalBooksWorldWide;
            TotalPagesReadInLanguage = (int)authorCountry.TotalPagesWorldWide;
            TotalBooksWorldWide = authorCountry.TotalBooksWorldWide;
            TotalPagesWorldWide = (int)authorCountry.TotalPagesWorldWide;
            PercentageOfBooksRead = (float)Math.Round(authorCountry.PercentageOfBooksRead, 2);
            PercentageOfPagesRead = (float)Math.Round(authorCountry.PercentageOfPagesRead, 2);
            FlagUrl = authorCountry.DisplayImage.ToString();

            Authors = new Author[authorCountry.AuthorsFromCountry.Count];
            for (int i = 0; i < authorCountry.AuthorsFromCountry.Count; i++)
            {
                Authors[i] = new Author(authorCountry.AuthorsFromCountry[i]);
            }
        }
    }
}
