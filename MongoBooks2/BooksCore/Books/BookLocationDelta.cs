// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookLocationDelta.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The difference between books locations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BookLocationDelta
    {
        public BookLocationDelta(DateTime date, DateTime startDate)
        {
            Date = date;
            StartDate = startDate;
            TimeSpan ts = date - startDate;
            DaysSinceStart = ts.Days;
            BooksLocationsToDate = new List<BookLocation>();
        }

        public DateTime Date { get; private set; }

        public DateTime StartDate { get; private set; }

        public List<BookLocation> BooksLocationsToDate { get; set; }

        public int DaysSinceStart { get; private set; }

        public double AverageLatitude
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count == 0) return 0.0;
                double totalLatitude = BooksLocationsToDate.Select(l => l.Latitude).Sum();
                return (totalLatitude / BooksLocationsToDate.Count);
            }
        }

        public double AverageLongitude
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count == 0) return 0.0;
                double totalLongitude = BooksLocationsToDate.Select(l => l.Longitude).Sum();
                return (totalLongitude / BooksLocationsToDate.Count);
            }
        }

        public double WeightedLatitude
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count == 0) return 0.0;
                double totalLatitude = BooksLocationsToDate.Select(l => (l.Latitude * l.Book.Pages)).Sum();
                var totalPages =
                    BooksLocationsToDate.Select(l => (long)l.Book.Pages).Sum();

                return (totalLatitude / totalPages);
            }
        }

        public double WeightedLongitude
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count == 0) return 0.0;
                double totalLongitude = BooksLocationsToDate.Select(l => (l.Longitude * l.Book.Pages)).Sum();
                var totalPages =
                    BooksLocationsToDate.Select(l => (long)l.Book.Pages).Sum();

                return (totalLongitude / totalPages);
            }
        }

        public double AverageLatitudeLastTen
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count < 10) return AverageLatitude;
                double totalLatitude = 
                    BooksLocationsToDate.OrderByDescending(c => c.Book.Date).Take(10).Select(l => l.Latitude).Sum();
                return (totalLatitude / 10.0);
            }
        }

        public double AverageLongitudeLastTen
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count < 10) return AverageLongitude;
                double totalLongitude = 
                    BooksLocationsToDate.OrderByDescending(c => c.Book.Date).Take(10).Select(l => l.Longitude).Sum();
                return (totalLongitude / 10.0);
            }
        }

        public double WeightedLatitudeLastTen
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count < 10) return WeightedLatitude;

                IEnumerable<BookLocation> lastTen = BooksLocationsToDate.OrderByDescending(c => c.Book.Date).Take(10);
                IEnumerable<BookLocation> bookLocations = lastTen as BookLocation[] ?? lastTen.ToArray();

                double totalLatitude =
                    bookLocations.Select(l => (l.Latitude * l.Book.Pages)).Sum();
                long totalPages =
                    bookLocations.Select(l => (long)l.Book.Pages).Sum();

                return (totalLatitude / totalPages);
            }
        }

        public double WeightedLongitudeLastTen
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count < 10) return WeightedLongitude;

                IEnumerable<BookLocation> lastTen = BooksLocationsToDate.OrderByDescending(c => c.Book.Date).Take(10);

                IEnumerable<BookLocation> bookLocations = lastTen as BookLocation[] ?? lastTen.ToArray();
                double totalLongitude = bookLocations.Select(l => (l.Longitude * l.Book.Pages)).Sum();
                long totalPages = bookLocations.Select(l => (long)l.Book.Pages).Sum();

                return (totalLongitude / totalPages);
            }
        }
    }
}
