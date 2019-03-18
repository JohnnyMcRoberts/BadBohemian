namespace AngularMongoBooks3.Controllers.DataClasses
{
    using BooksCore.Books;

    public class TagBooks
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the total number of pages this tag applies to.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets the total number of books this tag applies to.
        /// </summary>
        public int TotalBooks { get; set; }

        /// <summary>
        /// Gets or sets the set of books with this tag.
        /// </summary>
        public Book[] Books { get; set; }

        public TagBooks()
        {

        }

        public TagBooks(BookTag bookTag)
        {
            Name = bookTag.Tag;
            TotalPages = bookTag.TotalPages;
            TotalBooks = bookTag.TotalBooksReadBy;

            Books = new Book[bookTag.BooksWithTag.Count];
            for (int i = 0; i < bookTag.BooksWithTag.Count; i++)
            {
                Books[i] = new Book(bookTag.BooksWithTag[i]);
            }
        }
    }
}
