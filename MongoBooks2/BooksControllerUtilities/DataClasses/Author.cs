namespace BooksControllerUtilities.DataClasses
{
    using BooksCore.Books;

    public class Author
    {
        public string Name { get; set; }

        public string Nationality { get; set; }

        public string Language { get; set; }

        public int TotalPages { get; set; }

        public int TotalBooksReadBy { get; set; }
        
        public Book[] Books { get; set; }

        public Author()
        {

        }

        public Author(BookAuthor author)
        {
            Name = author.Author;
            Nationality = author.Nationality;
            Language = author.Language;
            TotalPages = author.TotalPages;
            TotalBooksReadBy = author.TotalBooksReadBy;
            Books = new Book[author.BooksReadBy.Count];
            for (int i = 0; i < author.BooksReadBy.Count; i++)
            {
                Books[i] = new Book( author.BooksReadBy[i] );
            }
        }
    }
}
