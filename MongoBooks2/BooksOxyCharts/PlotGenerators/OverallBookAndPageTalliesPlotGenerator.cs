// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageDaysPerBookPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using System.Collections.Generic;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    public class OverallBookAndPageTalliesPlotGenerator : BasePlotGenerator
    {
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Overall Book And Pages Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Overall Book And Pages Plot");
            SetupOverallBookAndPagesVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries booksReadSeries;
            LineSeries booksReadTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out booksReadSeries, ChartAxisKeys.DateKey, ChartAxisKeys.BooksReadKey, "Total Books Read", 1);
            OxyPlotUtilities.CreateLineSeries(out booksReadTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.BooksReadKey, "Books Read Trendline", 4);
            double yinterceptBooks;
            double slopeBooks;
            GetBooksLinearTrendlineParameters(out yinterceptBooks, out slopeBooks);


            LineSeries pagesReadSeries;
            LineSeries pagesReadTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out pagesReadSeries, ChartAxisKeys.DateKey, ChartAxisKeys.PagesReadKey, "Total Pages Read", 0);
            OxyPlotUtilities.CreateLineSeries(out pagesReadTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.PagesReadKey, "Pages Read Trendline", 3
                );
            double yinterceptPages;
            double slopePages;
            GetPagesLinearTrendlineParameters(out yinterceptPages, out slopePages);


            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                double trendBooks = yinterceptBooks + (slopeBooks * delta.DaysSinceStart);
                double trendPages = yinterceptPages + (slopePages * delta.DaysSinceStart);

                booksReadSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.TotalBooks));
                booksReadTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendBooks));

                pagesReadSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.TotalPages));
                pagesReadTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendPages));

            }


            OxyPlotUtilities.AddLineSeriesToModel(newPlot,
                new LineSeries[] { booksReadSeries, booksReadTrendlineSeries, 
                    pagesReadSeries, pagesReadTrendlineSeries }
                );


            // finally update the model with the new plot
            return  newPlot;
        }

        private void GetBooksLinearTrendlineParameters(out double yintercept, out double slope)
        {
            double rsquared;

            List<double> overallDays = new List<double>();
            List<double> overallDaysPerBook = new List<double>();

            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallDaysPerBook.Add(delta.OverallTally.TotalBooks);
            }
            OxyPlotUtilities.LinearRegression(overallDays, overallDaysPerBook, out  rsquared, out  yintercept, out  slope);
        }

        private void GetPagesLinearTrendlineParameters(out double yintercept, out double slope)
        {
            double rsquared;

            List<double> overallDays = new List<double>();
            List<double> overallDaysPerBook = new List<double>();

            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallDaysPerBook.Add(delta.OverallTally.TotalPages);
            }
            OxyPlotUtilities.LinearRegression(overallDays, overallDaysPerBook, out  rsquared, out  yintercept, out  slope);
        }

        private void SetupOverallBookAndPagesVsTimeAxes(PlotModel newPlot)
        {
            var xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = ChartAxisKeys.DateKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                StringFormat = "yyyy-MM-dd"
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

    }
}
