namespace BooksControllerUtilities.DataClasses
{
    using System;
    using BooksCore.Books;

    public class LanguageAuthors
    {
        public string Name { get; set; }

        public int TotalBooksReadInLanguage { get; set; }

        public int TotalPagesReadInLanguage { get; set; }

        public int TotalBooksWorldWide { get; set; }

        public int TotalPagesWorldWide { get; set; }

        public float PercentageOfBooksRead { get; set; }

        public float PercentageOfPagesRead { get; set; }

        public Author[] Authors { get; set; }

        public LanguageAuthors()
        {

        }

        public LanguageAuthors(AuthorLanguage authorLanguage)
        {
            Name = authorLanguage.Language;
            TotalBooksReadInLanguage = authorLanguage.TotalBooksReadInLanguage;
            TotalPagesReadInLanguage = (int)authorLanguage.TotalPagesReadInLanguage;
            TotalBooksWorldWide = authorLanguage.TotalBooksWorldWide;
            TotalPagesWorldWide = (int)authorLanguage.TotalPagesWorldWide;
            PercentageOfBooksRead = (float)Math.Round(authorLanguage.PercentageOfBooksRead, 2);
            PercentageOfPagesRead = (float)Math.Round(authorLanguage.PercentageOfPagesRead, 2);

            Authors = new Author[authorLanguage.AuthorsInLanguage.Count];
            for (int i = 0; i < authorLanguage.AuthorsInLanguage.Count; i++)
            {
                Authors[i] = new Author(authorLanguage.AuthorsInLanguage[i]);
            }
        }
    }
}
