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
    using System.Linq;
    using BooksCore.Books;

    public class TotalBooksReadByCountryPlotGenerator : BasePlotGenerator
    {
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Total Books Read by Language With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Total Books Read by Language With Time Plot");
            SetupTotalBooksReadKeyVsTimeAxes(newPlot);

            // get the languages (in order)
            BooksDelta.DeltaTally latestTally = BooksReadProvider.BookDeltas.Last().OverallTally;
            List<string> countries = (from item in latestTally.CountryTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // create the series for the languages
            List<KeyValuePair<string, LineSeries>> lineSeriesSet =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < countries.Count; i++)
            {
                LineSeries series;
                OxyPlotUtilities.CreateLongLineSeries(out series,
                    ChartAxisKeys.DateKey, ChartAxisKeys.TotalBooksReadKey, countries[i], i, 128);
                lineSeriesSet.Add(
                    new KeyValuePair<string, LineSeries>(countries[i], series));
            }

            // loop through the deltas adding points for each of the items to the lines
            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                var date = DateTimeAxis.ToDouble(delta.Date);
                foreach (var line in lineSeriesSet)
                {
                    double ttl = 0.0;
                    foreach (var langTotal in delta.OverallTally.CountryTotals)
                        if (langTotal.Item1 == line.Key)
                            ttl = langTotal.Item2;
                    line.Value.Points.Add(new DataPoint(date, ttl));
                }
            }

            IList<LineSeries> lineSeries =
                (from item in lineSeriesSet select item.Value).ToList();
            var stackSeries = OxyPlotUtilities.StackLineSeries(lineSeries);

            // add them to the plot
            foreach (var areaSeries in stackSeries)
                newPlot.Series.Add(areaSeries);

            // finally update the model with the new plot
            return newPlot;
        }

        private void SetupTotalBooksReadKeyVsTimeAxes(PlotModel newPlot)
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
                Title = "Total Books Read",
                Key = ChartAxisKeys.TotalBooksReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }
    }
}
