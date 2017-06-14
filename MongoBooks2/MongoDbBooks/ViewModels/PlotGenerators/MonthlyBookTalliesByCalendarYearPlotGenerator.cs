
namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;

    public class MonthlyBookTalliesByCalendarYearPlotGenerator : IPlotGenerator
    {
        #region Public Types

        public enum ChartType
        {
            BothAsLines,
            BooksAsColumns,
            PagesAsColumns,
        }

        #endregion

        #region Constructor

        public MonthlyBookTalliesByCalendarYearPlotGenerator(ChartType type)
        {
            _chartType = type;
        }

        #endregion

        #region IPlotGenerator implementation

        public PlotModel SetupPlot(MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupTalliesPerCalendarYearPlot();
        }

        #endregion

        #region Private data

        private MainBooksModel _mainModel;

        private readonly ChartType _chartType;

        private readonly string[] _monthNames = new string[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
        };


        #endregion

        #region Nested classes

        private class MonthOfYearTally
        {
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
            if (_chartType == ChartType.BothAsLines)    
                SetupBookAndPagesVsMonthOfYearAxes(newPlot);
            else if (_chartType == ChartType.PagesAsColumns)
                SetupPagesVsMonthOfYearCalegoryAxes(newPlot);
            else                
                SetupBookVsMonthOfYearCalegoryAxes(newPlot);

            // get the books & pages read for each calendar year
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthOfYear = GetBookListsByMonthOfYear();

            // add a series for each year (in order)
            if (_chartType == ChartType.BothAsLines)
                AddLineSeriesForMonthlyTallies(newPlot, bookListsByMonthOfYear);
            else
                AddColumnSeriesForMonthlyTallies(newPlot, bookListsByMonthOfYear);

            // finally update the model with the new plot
            return newPlot;
        }

        private void AddColumnSeriesForMonthlyTallies(PlotModel newPlot, 
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthOfYear)
        {
            int colourIndex = 1;
            var colours = OxyPlotUtilities.SetupStandardColourSet();
            foreach (var year in bookListsByMonthOfYear.Keys.ToList().OrderBy(x => x))
            {
                int index = colourIndex % colours.Count;
                var colour = colours[index];
                string title =  (_chartType == ChartType.PagesAsColumns) 
                    ? "Pages read in " + year.ToString()
                    : "Books read in " + year.ToString();

                var yKey =  (_chartType == ChartType.PagesAsColumns)
                    ? ChartAxisKeys.PagesReadKey
                    : ChartAxisKeys.BooksReadKey;

                ColumnSeries booksReadSeries = new ColumnSeries
                {
                    Title = title,
                    XAxisKey = ChartAxisKeys.MonthOfYearKey,
                    YAxisKey = yKey, 
                    FillColor = colour, 
                    ColumnWidth = 13, 
                };

                var monthTallies = bookListsByMonthOfYear[year];
                foreach (var monthTally in monthTallies)
                {
                    booksReadSeries.Items.Add(
                        new ColumnItem() 
                        { 
                            CategoryIndex = monthTally.MonthOfYear - 1,
                            Value = (_chartType == ChartType.PagesAsColumns)
                                ? monthTally.PagesReadThisMonth
                                : monthTally.BooksReadThisMonth 
                        });
                }

                newPlot.Series.Add(booksReadSeries);
                colourIndex++;
            }
        }

        private static void AddLineSeriesForMonthlyTallies(PlotModel newPlot, 
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthOfYear)
        {
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
                OxyPlotUtilities.AddLineSeriesToModel(newPlot, new[] { booksReadSeries, pagesReadSeries });
                colourIndex++;
            }
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
        
        private void SetupBookVsMonthOfYearCalegoryAxes(PlotModel newPlot)
        {
            var xAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Month of the year",
                Key = ChartAxisKeys.MonthOfYearKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                ItemsSource = _monthNames
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
        }

        private void SetupPagesVsMonthOfYearCalegoryAxes(PlotModel newPlot)
        {
            var xAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Month of the year",
                Key = ChartAxisKeys.MonthOfYearKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                ItemsSource = _monthNames
            };
            newPlot.Axes.Add(xAxis);

            var lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Pages Read",
                Key = ChartAxisKeys.PagesReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Minimum = 0
            };
            newPlot.Axes.Add(lhsAxis);
        }

        
        #endregion
    }
}
