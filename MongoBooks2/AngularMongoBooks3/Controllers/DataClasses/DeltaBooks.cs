namespace AngularMongoBooks3.Controllers.DataClasses
{
    using System;

    using BooksCore.Books;

    public class DeltaBooks
    {
        public DateTime Date { get; set; }

        public DateTime StartDate { get; set; }

        public int DaysSinceStart { get; set; }

        public TallyDelta OverallTally { get; set; }

        public TallyDelta LastTenTally { get; set; }

        public DeltaBooks()
        {

        }

        public DeltaBooks(BooksDelta booksDelta)
        {
            Date = booksDelta.Date;
            StartDate = booksDelta.StartDate;
            DaysSinceStart = booksDelta.DaysSinceStart;
            OverallTally = new TallyDelta(booksDelta.OverallTally);
            LastTenTally = new TallyDelta(booksDelta.LastTenTally);
        }
    }
}
