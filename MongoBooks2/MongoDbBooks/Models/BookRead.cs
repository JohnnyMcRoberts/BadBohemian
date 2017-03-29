// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookRead.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The book format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.Models
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// The book read.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class BookRead : BaseMongoEntity, IBookRead
    {
        #region Private Data

        /// <summary>
        /// Whether is book.
        /// </summary>
        private string _isBook;

        /// <summary>
        /// Whether is comic.
        /// </summary>
        private string _isComic;

        /// <summary>
        /// Whether is audio.
        /// </summary>
        private string _isAudio;

        /// <summary>
        /// The format.
        /// </summary>
        private BookFormat _bookFormat;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRead"/> class.
        /// </summary>
        public BookRead()
        {
            this.Format = BookFormat.Book;
        }

        #endregion

        // Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio
        #region Public Data

        /// <summary>
        /// Gets or sets the date string.
        /// </summary>
        [BsonElement("dateString")]
        public string DateString { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [BsonElement("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        [BsonElement("author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [BsonElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        [BsonElement("pages")]
        public ushort Pages { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        [BsonElement("note")]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        [BsonElement("nationality")]
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the original language.
        /// </summary>
        [BsonElement("originalLanguage")]
        public string OriginalLanguage { get; set; }

        /// <summary>
        /// Gets or sets the book.
        /// </summary>
        public string Book
        {
            get
            {
                return this._isBook;
            }

            set
            {
                this._isBook = value;
                if (!string.IsNullOrEmpty(this._isBook))
                {
                    this.Format = BookFormat.Book;
                }
            }
        }

        /// <summary>
        /// Gets or sets the comic.
        /// </summary>
        public string Comic
        {
            get
            {
                return this._isComic;
            }

            set
            {
                this._isComic = value;
                if (!string.IsNullOrEmpty(this._isComic))
                {
                    this.Format = BookFormat.Comic;
                }
            }
        }

        /// <summary>
        /// Gets or sets the audio.
        /// </summary>
        public string Audio
        {
            get
            {
                return this._isAudio;
            }

            set
            {
                this._isAudio = value;
                if (!string.IsNullOrEmpty(this._isAudio))
                {
                    this.Format = BookFormat.Audio;
                }
            }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        [BsonElement("format")]
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
                    _isBook = "x";
                    _isComic = string.Empty;
                    _isAudio = string.Empty;
                }
                else if (_bookFormat == BookFormat.Audio)
                {
                    _isAudio = "x";
                    _isBook = string.Empty;
                    _isComic = string.Empty;
                }
                else if (_bookFormat == BookFormat.Comic)
                {
                    _isComic = "x";
                    _isBook = string.Empty;
                    _isAudio = string.Empty;
                }
            }
        }

        #endregion
    }
}
