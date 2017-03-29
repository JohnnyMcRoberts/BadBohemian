// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookReadFromGmail.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   Defines the BookReadFromGmail type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.Models.Mailbox
{
    using System;

    /// <summary>
    /// The book read from gmail.
    /// </summary>
    public class BookReadFromGmail : IBookRead
    {
        #region Private data

        /// <summary>
        /// Whether item read was a book.
        /// </summary>
        private bool _isBook;

        /// <summary>
        /// Whether item read was a comic.
        /// </summary>
        private bool _isComic;

        /// <summary>
        /// Whether item read was an audiobook.
        /// </summary>
        private bool _isAudio;

        /// <summary>
        /// The format of the read book.
        /// </summary>
        private BookFormat _bookFormat;

        #endregion

        #region Implementation of IBookRead

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
        public ushort Pages { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        public BookFormat Format
        {
            get
            {
                return _bookFormat;
            }

            set
            {
                _bookFormat = value;
                if (_bookFormat == BookFormat.Book)
                {
                    _isBook = true;
                    _isComic = false;
                    _isAudio = false;
                }
                else if (_bookFormat == BookFormat.Audio)
                {
                    _isAudio = true;
                    _isBook = false;
                    _isComic = false;
                }
                else if (_bookFormat == BookFormat.Comic)
                {
                    _isComic = true;
                    _isBook = false;
                    _isAudio = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the total read.
        /// </summary>
        public ushort TotalRead { get; set; }

        /// <summary>
        /// Gets or sets the total books.
        /// </summary>
        public ushort TotalBooks { get; set; }

        /// <summary>
        /// Gets or sets the total comics.
        /// </summary>
        public ushort TotalComics { get; set; }

        /// <summary>
        /// Gets or sets the total audio.
        /// </summary>
        public ushort TotalAudio { get; set; }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        public ulong TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the previous total read.
        /// </summary>
        public ushort PreviousTotalRead { get; set; }

        /// <summary>
        /// Gets or sets the previous total books.
        /// </summary>
        public ushort PreviousTotalBooks { get; set; }

        /// <summary>
        /// Gets or sets the previous total comics.
        /// </summary>
        public ushort PreviousTotalComics { get; set; }

        /// <summary>
        /// Gets or sets the previous total audio.
        /// </summary>
        public ushort PreviousTotalAudio { get; set; }

        /// <summary>
        /// Gets or sets the previous total pages.
        /// </summary>
        public ulong PreviousTotalPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is book.
        /// </summary>
        public bool IsBook
        {
            get
            {
                return _isBook;
            }

            set
            {
                _isBook = value;
                if (_isBook)
                {
                    IsComic = false;
                    IsAudio = false;
                    Format = BookFormat.Book;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is comic.
        /// </summary>
        public bool IsComic
        {
            get
            {
                return _isComic;
            }

            set
            {
                _isComic = value;
                if (_isComic)
                {
                    IsBook = false;
                    IsAudio = false;
                    Format = BookFormat.Comic;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is audio.
        /// </summary>
        public bool IsAudio
        {
            get
            {
                return _isAudio;
            }

            set
            {
                _isAudio = value;
                if (_isAudio)
                {
                    IsComic = false;
                    IsBook = false;
                    Format = BookFormat.Audio;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookReadFromGmail"/> class.
        /// </summary>
        public BookReadFromGmail()
        {
            Date = DateTime.Today;
            Author = string.Empty;
            Title = string.Empty;

            IsBook = true;
            Pages = 0;

            TotalRead = 0;
            TotalBooks = 0;
            TotalComics = 0;
            TotalAudio = 0;
            TotalPages = 0;

            PreviousTotalRead = 0;
            PreviousTotalBooks = 0;
            PreviousTotalComics = 0;
            PreviousTotalAudio = 0;
            PreviousTotalPages = 0;
        }

        /// <summary>
        /// Sets up the previous book properties.
        /// </summary>
        /// <param name="previousBook">
        /// The previous book.
        /// </param>
        public void SetupPreviousBook(BookReadFromGmail previousBook)
        {
            PreviousTotalRead = previousBook.TotalRead;
            PreviousTotalBooks = previousBook.TotalBooks;
            PreviousTotalComics = previousBook.TotalComics;
            PreviousTotalAudio = previousBook.TotalAudio;
            PreviousTotalPages = previousBook.TotalPages;

            if (TotalComics - PreviousTotalComics == 1)
            {
                IsComic = true;
            }
            else if (TotalAudio - PreviousTotalAudio == 1)
            {
                IsComic = true;
            }
            else
            {
                IsBook = true;
            }
        }
    }
}
