// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Nation.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The user MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The book tag class.
    /// </summary>
    public class BookTag
    {
        /// <summary>
        /// Gets or sets the tag text.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the list of books with this tag.
        /// </summary>
        public List<BookRead> BooksWithTag { get; set; }

        /// <summary>
        /// Gets the total number of pages this tag applies to.
        /// </summary>
        public ushort TotalPages => BooksWithTag.Aggregate<BookRead, ushort>(0, (current, book) => (ushort) (current + book.Pages));

        /// <summary>
        /// Gets the total number of books this tag applies to.
        /// </summary>
        public int TotalBooksReadBy => BooksWithTag.Count;


        /// <summary>
        /// Initializes a new instance of the <see cref="BookTag"/> class.
        /// </summary>
        public BookTag()
        {
            Tag = string.Empty;
            BooksWithTag = new List<BookRead>();
        }
    }
}
