using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

using MongoDbBooks.Models;
using MongoDbBooks.ViewModels.Utilities;

namespace MongoDbBooks.ViewModels.PlotGenerators
{
    public class MonthlyBookTalliesByCalendarYearPlotGenerator : IPlotGenerator
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

        private class MonthOfYearTally
        {
            public int Year { get; set; }
            public int MonthOfYear { get; set; }
            public int BooksReadThisMonth { get; set; }
            public int PagesReadThisMonth { get; set; }
        }

        #endregion

        #region Local helper functions

        private PlotModel SetupTalliesPerCalendarYearPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Tallies Per Month for each Calendar Year" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Tallies Per Month for each Calendar Year");
            SetupBookAndPagesVsMonthOfYearAxes(newPlot);

            // get the books & pages read for each calendar year
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthOfYear = GetBookListsByMonthOfYear();

            // add a series for each year (in order)
            int colourIndex = 1;
            var colours = OxyPlotUtilities.SetupStandardColourSet();
            foreach (var year in bookListsByMonthOfYear.Keys.ToList().OrderBy(x => x))
            {
                LineSeries booksReadSeries;
                LineSeries pagesReadSeries;
                GetBooksAndPagesReadLineSeries(colourIndex, colours, year, out booksReadSeries, out pagesReadSeries);

                // add the points for the days of this year
                foreach (var tally in bookListsByMonthOfYear[year])
                {
                    booksReadSeries.Points.Add(
                        new DataPoint(tally.MonthOfYear, tally.BooksReadThisMonth));
                    pagesReadSeries.Points.Add(
                        new DataPoint(tally.MonthOfYear, tally.PagesReadThisMonth));
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

            OxyPlotUtilities.CreateLineSeries(out booksReadSeries, ChartAxisKeys.MonthOfYearKey,
                ChartAxisKeys.BooksReadKey, "Total Books Read in " + year.ToString(), colourIndex);
            booksReadSeries.StrokeThickness++;
            booksReadSeries.Color = colour;

            OxyPlotUtilities.CreateLineSeries(out pagesReadSeries, ChartAxisKeys.MonthOfYearKey,
                ChartAxisKeys.PagesReadKey, "Total Pages Read in " + year.ToString(), colourIndex);
            pagesReadSeries.LineStyle = LineStyle.Dash;
            pagesReadSeries.Color = colour;
        }

        private Dictionary<int, List<MonthOfYearTally>> GetBookListsByMonthOfYear()
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
            Dictionary<int, Dictionary<int, List<BookRead>>> bookTotalsByMonthAndYear =
                new Dictionary<int, Dictionary<int, List<BookRead>>>();
            foreach (var year in bookListsByYear.Keys)
            {
                Dictionary<int, List<BookRead>> booksByMonthOfYear = new Dictionary<int, List<BookRead>>();
                foreach (var book in bookListsByYear[year])
                {
                    int bookReadMonthOfYear = book.Date.Month;
                    if (booksByMonthOfYear.ContainsKey(bookReadMonthOfYear))
                        booksByMonthOfYear[bookReadMonthOfYear].Add(book);
                    else
                        booksByMonthOfYear.Add(bookReadMonthOfYear, new List<BookRead>() { book });
                }
                bookTotalsByMonthAndYear.Add(year, booksByMonthOfYear);
            }

            // from these get the daily tallies for each year
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthAndYear = 
                new Dictionary<int, List<MonthOfYearTally>>();
            foreach (var year in bookTotalsByMonthAndYear.Keys)
            {
                Dictionary<int, List<BookRead>> booksByMonthOfYear = bookTotalsByMonthAndYear[year];
                List<MonthOfYearTally> monthOfYearTallies = new List<MonthOfYearTally>();

                foreach (var monthOfYear in booksByMonthOfYear.Keys.ToList().OrderBy(x => x))
                {
                    List<BookRead> booksforMonth = booksByMonthOfYear[monthOfYear];
                    MonthOfYearTally tally = new MonthOfYearTally()
                    {
                        Year = year,
                        MonthOfYear = monthOfYear,
                        BooksReadThisMonth = booksforMonth.Count(),
                        PagesReadThisMonth = booksforMonth.Sum(x => x.Pages)
                    };


                    monthOfYearTallies.Add(tally);
                }
                bookListsByMonthAndYear.Add(year, monthOfYearTallies);
            }

            return bookListsByMonthAndYear;
        }

        private void SetupBookAndPagesVsMonthOfYearAxes(PlotModel newPlot)
        {
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Month of the year",
                Key = ChartAxisKeys.MonthOfYearKey,
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
