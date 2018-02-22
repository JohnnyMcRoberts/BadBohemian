// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookLocation.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The book location class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    public class BookLocation
    {
        public BookRead Book { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
