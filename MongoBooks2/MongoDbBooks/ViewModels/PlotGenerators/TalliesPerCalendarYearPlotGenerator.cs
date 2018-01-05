namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;

    public class TalliesPerCalendarYearPlotGenerator : IPlotGenerator
    {
        #region IPlotGenerator implementation

        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupTalliesPerCalendarYearPlot();
        }
        
        #endregion

        #region Private data
        
        private Models.MainBooksModel _mainModel;

        #endregion

        #region Nested classes

        private class DayOfYearTally
        {
            public int Year { get; set; }
            public int DayOfYear { get; set; }
            public int BooksReadOnThisDay { get; set; }
            public int PagesReadOnThisDay { get; set; }
            public int BooksReadThisYearOnThisDay { get; set; }
            public int PagesReadThisYearOnThisDay { get; set; }
        }

        #endregion

        #region Local helper functions

        private PlotModel SetupTalliesPerCalendarYearPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Tallies Per Calendar Year Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Tallies Per Calendar Year Plot");
            SetupBookAndPagesVsDayOfYearAxes(newPlot);

            // get the books & pages read for each calendar year
            Dictionary<int, List<DayOfYearTally>> bookListsByDayandYear = GetBookListsByDayAndYear();

            // add a series for each year (in order)
            int colourIndex = 1;
            var colours = OxyPlotUtilities.SetupStandardColourSet();
            foreach (var year in bookListsByDayandYear.Keys.ToList().OrderBy(x => x))
            {
                LineSeries booksReadSeries;
                LineSeries pagesReadSeries;
                GetBooksAndPagesReadLineSeries(colourIndex, colours, year, out booksReadSeries, out pagesReadSeries);

                // add the points for the days of this year
                foreach (var tally in bookListsByDayandYear[year])
                {
                    booksReadSeries.Points.Add(
                        new DataPoint(tally.DayOfYear, tally.BooksReadThisYearOnThisDay));
                    pagesReadSeries.Points.Add(
                        new DataPoint(tally.DayOfYear, tally.PagesReadThisYearOnThisDay)); 
                }

                // then add them to the model
                OxyPlotUtilities.AddLineSeriesToModel(newPlot, new 
                    LineSeries[] { booksReadSeries, pagesReadSeries });
                colourIndex++;
            }
                        
            // finally update the model with the new plot
            return newPlot;
        }

        private static void GetBooksAndPagesReadLineSeries(int colourIndex, List<OxyColor> colours, int year, 
            out LineSeries booksReadSeries, out LineSeries pagesReadSeries)
        {
            var colour = colours[colourIndex];

            OxyPlotUtilities.CreateLineSeries(out booksReadSeries, ChartAxisKeys.DayOfYearKey,
                ChartAxisKeys.BooksReadKey, "Total Books Read in " + year.ToString(), colourIndex);
            booksReadSeries.StrokeThickness++;
            booksReadSeries.Color = colour;

            OxyPlotUtilities.CreateLineSeries(out pagesReadSeries, ChartAxisKeys.DayOfYearKey,
                ChartAxisKeys.PagesReadKey, "Total Pages Read in " + year.ToString(), colourIndex);
            pagesReadSeries.LineStyle = LineStyle.Dash;
            pagesReadSeries.Color = colour;
        }

        private Dictionary<int, List<DayOfYearTally>> GetBookListsByDayAndYear()
        {
            // get the books foreach year
            Dictionary<int, List<BookRead>> bookListsByYear = new Dictionary<int, List<BookRead>>();
            foreach (var book in _mainModel.BooksRead)
            {
                int bookReadYear = book.Date.Year;
                if (bookListsByYear.ContainsKey(bookReadYear))
                    bookListsByYear[bookReadYear].Add(book);
                else
                    bookListsByYear.Add(bookReadYear, new List<BookRead>() { book });
            }

            // for each year get the lists of books read each day
            Dictionary<int, Dictionary<int, List<BookRead>>> bookTotalsByDayandYear =
                new Dictionary<int, Dictionary<int, List<BookRead>>>();
            foreach (var year in bookListsByYear.Keys)
            {
                Dictionary<int, List<BookRead>> booksByDayOfYear = new Dictionary<int, List<BookRead>>();
                foreach (var book in bookListsByYear[year])
                {
                    int bookReadDayOfYear = book.Date.DayOfYear;
                    if (booksByDayOfYear.ContainsKey(bookReadDayOfYear))
                        booksByDayOfYear[bookReadDayOfYear].Add(book);
                    else
                        booksByDayOfYear.Add(bookReadDayOfYear, new List<BookRead>() { book });
                }
                bookTotalsByDayandYear.Add(year, booksByDayOfYear);
            }

            // from these get the daily tallies for each year
            Dictionary<int, List<DayOfYearTally>> bookListsByDayandYear = new Dictionary<int, List<DayOfYearTally>>();
            foreach (var year in bookTotalsByDayandYear.Keys)
            {
                Dictionary<int, List<BookRead>> booksByDayOfYear = bookTotalsByDayandYear[year];
                List<DayOfYearTally> dayOfYearTallies = new List<DayOfYearTally>();
                int ttlBooksRead = 0, ttlPagesRead = 0;

                foreach (var dayOfYear in booksByDayOfYear.Keys.ToList().OrderBy(x => x))
                {
                    List<BookRead> booksforDay = booksByDayOfYear[dayOfYear];
                    DayOfYearTally tally = new DayOfYearTally() 
                    { 
                        Year = year, DayOfYear = dayOfYear,
                        BooksReadOnThisDay = booksforDay.Count(), 
                        PagesReadOnThisDay = booksforDay.Sum(x => x.Pages) 
                    };

                    ttlBooksRead += tally.BooksReadOnThisDay;
                    ttlPagesRead += tally.PagesReadOnThisDay;
                    tally.BooksReadThisYearOnThisDay = ttlBooksRead;
                    tally.PagesReadThisYearOnThisDay = ttlPagesRead;

                    dayOfYearTallies.Add(tally);
                }
                bookListsByDayandYear.Add(year, dayOfYearTallies);
            }

            return bookListsByDayandYear;
        }

        private void SetupBookAndPagesVsDayOfYearAxes(PlotModel newPlot)
        {
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Day of the year",
                Key = ChartAxisKeys.DayOfYearKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(xAxis);

            var lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Books Read",
                Key = ChartAxisKeys.BooksReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Minimum = 0
            };
            newPlot.Axes.Add(lhsAxis);

            var rhsAxis = new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Pages Read",
                Key = ChartAxisKeys.PagesReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Minimum = 0
            };
            newPlot.Axes.Add(rhsAxis);
        }

        #endregion
    }
}
