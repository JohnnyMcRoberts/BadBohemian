// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TalliedBook.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The tallied book class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System;

    public class TalliedBook
    {
        public BookRead Book { get; }

        public string DateString => Book.DateString;

        public DateTime Date => Book.Date;

        public string Author => Book.Author; 

        public string Title => Book.Title; 

        public uint Pages => Book.Pages; 

        public uint TotalBooks { get; set; }

        public uint TotalBookFormat { get; set; }

        public uint TotalComicFormat { get; set; }

        public uint TotalAudioFormat { get; set; }

        public uint TotalPagesRead { get; set; }

        public TalliedBook(BookRead book)
        {
            Book = book;
        }
    }
}
