// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookAuthor.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The book authour class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System.Collections.Generic;

    public class BookAuthor
    {
        public string Author { get; set; }

        public string Nationality { get; set; }

        public string Language { get; set; }

        public ushort TotalPages
        {
            get
            {
                ushort total = 0;
                foreach (BookRead book in BooksReadBy)
                {
                    total += book.Pages;
                }

                return total;
            }
        }

        public int TotalBooksReadBy => BooksReadBy.Count;

        public List<BookRead> BooksReadBy { get; set; }

        public BookAuthor()
        {
            Author = string.Empty;
            Nationality = string.Empty;
            Language = string.Empty;
            BooksReadBy = new List<BookRead>();
        }
    }
}
