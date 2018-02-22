// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookRead.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The book format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookRead.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The book read MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System;
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    using BooksCore.Base;
    using BooksCore.Interfaces;

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
            Format = BookFormat.Book;
            Tags = new List<string>();
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

        [BsonElement("image_url")]
        public string ImageUrl { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the book.
        /// </summary>
        public string Book
        {
            get
            {
                return _isBook;
            }

            set
            {
                _isBook = value;
                if (!string.IsNullOrEmpty(_isBook))
                {
                    Format = BookFormat.Book;
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
                return _isComic;
            }

            set
            {
                _isComic = value;
                if (!string.IsNullOrEmpty(_isComic))
                {
                    Format = BookFormat.Comic;
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
                return _isAudio;
            }

            set
            {
                _isAudio = value;
                if (!string.IsNullOrEmpty(_isAudio))
                {
                    Format = BookFormat.Audio;
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

        [BsonElement("user")]
        public string User { get; set; }

        /// <summary>
        /// Gets the image address ready to be displayed.
        /// </summary>
        public string DisplayImageAddress
        {
            get
            {
                if (ImageUrl == null)
                {
                    return "N/A";
                }

                return ImageUrl.Substring(0, Math.Min(ImageUrl.Length, 50)) + " ...";
            }
        }

        /// <summary>
        /// Gets the image URI ready to be displayed.
        /// </summary>
        public Uri DisplayImage => string.IsNullOrEmpty(ImageUrl) ? new Uri("pack://application:,,,/Images/camera_image_cancel-32.png") : new Uri(ImageUrl);

        /// <summary>
        /// Gets the tags list ready to be displayed.
        /// </summary>
        public string DisplayTags
        {
            get
            {
                if (Tags == null || Tags.Count == 0)
                {
                    return string.Empty;
                }

                if (Tags.Count == 0)
                {
                    return Tags[0];
                }

                string listOfTags = Tags[0];
                for (int i = 1; i < Tags.Count; i++)
                {
                    listOfTags += ", " + Tags[i];
                }

                return listOfTags;
            }
        }

        #endregion
    }
}
