namespace BooksControllerUtilities.RequestsResponses
{
    using System;
    using System.Collections.Generic;

    public class BookReadAddRequest
    {
        public DateTime Date { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public ushort Pages { get; set; }

        public string Note { get; set; }

        public string Nationality { get; set; }

        public string OriginalLanguage { get; set; }

        public string ImageUrl { get; set; }

        public List<string> Tags { get; set; }

        public string UserId { get; set; }

        public string Format { get; set; }

        public BookReadAddRequest()
        {

        }
    }
}
