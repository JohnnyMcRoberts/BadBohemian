namespace AngularMongoBooks3.Controllers.DataClasses
{
    using System;


    using BooksCore.Books;

    public class Book
    {
        /// <summary>
        /// Gets or sets the date string.
        /// </summary>
        public string DateString { get; set; }

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
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the original language.
        /// </summary>
        public string OriginalLanguage { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the book format.
        /// </summary>
        public string Format { get; set; }

        public Book()
        {

        }

        public Book(BookRead book)
        {
            DateString = book.DateString;
            Date = book.Date;
            Author = book.Author;
            Title = book.Title;
            Pages = book.Pages;
            Note = book.Note;
            Nationality = book.Nationality;
            OriginalLanguage = book.OriginalLanguage;
            ImageUrl = book.ImageUrl;
            User = book.User;
            Format = book.Format.ToString();
            Tags = new string[book.Tags.Count];
            for (int i = 0; i < book.Tags.Count; i++)
            {
                Tags[i] = book.Tags[i];
            }
        }

        //public GetBookRead()

    }
}
