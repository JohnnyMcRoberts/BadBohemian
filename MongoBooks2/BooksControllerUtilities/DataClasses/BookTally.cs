namespace BooksControllerUtilities.DataClasses
{
    using System;

    using BooksCore.Books;

    public class BookTally
    {
        /// <summary>
        /// Gets or sets the date string.
        /// </summary>
        public Book Book { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public int TotalBooks { get; set; }

        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        public int TotalBookFormat { get; set; }

        /// <summary>
        /// Gets or sets the original language.
        /// </summary>
        public int TotalComicFormat { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        public int TotalAudioFormat { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public int TotalPagesRead { get; set; }

        public BookTally()
        {

        }

        public BookTally(TalliedBook talliedBook)
        {
            Book = new Book(talliedBook.Book);
            Date = talliedBook.Date;
            Author = talliedBook.Author;
            Title = talliedBook.Title;
            Pages = (int)talliedBook.Pages;
            TotalBooks = (int)talliedBook.TotalBooks;
            TotalBookFormat = (int)talliedBook.TotalBookFormat;
            TotalComicFormat = (int)talliedBook.TotalComicFormat;
            TotalAudioFormat = (int)talliedBook.TotalAudioFormat;
            TotalPagesRead = (int)talliedBook.TotalPagesRead;
        }

    }
}
