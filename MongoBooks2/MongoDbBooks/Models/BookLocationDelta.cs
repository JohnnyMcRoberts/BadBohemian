namespace MongoDbBooks.Models
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
                return (totalLatitude / (double)(BooksLocationsToDate.Count));
            }
        }
        public double AverageLongitude
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count == 0) return 0.0;
                double totalLongitude = BooksLocationsToDate.Select(l => l.Longitude).Sum();
                return (totalLongitude / (double)(BooksLocationsToDate.Count));
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

                return (totalLatitude / (double)(totalPages));
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

                return (totalLongitude / (double)(totalPages));
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

                var lastTen = BooksLocationsToDate.OrderByDescending(c => c.Book.Date).Take(10);

                double totalLatitude =
                    lastTen.Select(l => (l.Latitude * l.Book.Pages)).Sum();
                var totalPages =
                    lastTen.Select(l => (long)l.Book.Pages).Sum();

                return (totalLatitude / (double)(totalPages));
            }
        }
        public double WeightedLongitudeLastTen
        {
            get
            {
                if (BooksLocationsToDate == null || BooksLocationsToDate.Count < 10) return WeightedLongitude;

                var lastTen = BooksLocationsToDate.OrderByDescending(c => c.Book.Date).Take(10);

                double totalLongitude =
                    lastTen.Select(l => (l.Longitude * l.Book.Pages)).Sum();
                var totalPages =
                    lastTen.Select(l => (long)l.Book.Pages).Sum();

                return (totalLongitude / (double)(totalPages));
            }
        }
    }
}
