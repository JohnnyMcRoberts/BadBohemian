// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalPagesReadByCountryPlotGenerator.cs" company="N/A">
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

    public class TotalPagesReadByCountryPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Total Pages Read by Country With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Total Pages Read by Country With Time Plot");
            SetupTotalPagesReadKeyVsTimeAxes(newPlot);

            // get the languages (in order)
            BooksDelta.DeltaTally latestTally = BooksReadProvider.BookDeltas.Last().OverallTally;
            List<string> countries = (from item in latestTally.CountryTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // create the series for the languages
            List<KeyValuePair<string, LineSeries>> countriesLineSeries =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < countries.Count; i++)
            {
                LineSeries lineSeries;
                OxyPlotUtilities.CreateLongLineSeries(out lineSeries,
                    ChartAxisKeys.DateKey, ChartAxisKeys.TotalPagesReadKey, countries[i], i, 128);
                countriesLineSeries.Add(
                    new KeyValuePair<string, LineSeries>(countries[i], lineSeries));
            }

            // loop through the deltas adding points for each of the items to the lines
            foreach (var delta in BooksReadProvider.BookDeltas)
            {
                var date = DateTimeAxis.ToDouble(delta.Date);
                foreach (var countryLine in countriesLineSeries)
                {
                    double ttl = 0.0;
                    foreach (var langTotal in delta.OverallTally.CountryTotals)
                        if (langTotal.Item1 == countryLine.Key)
                            ttl = langTotal.Item4;
                    countryLine.Value.Points.Add(new DataPoint(date, ttl));
                }
            }

            IList<LineSeries> lineSeriesSet =
                (from item in countriesLineSeries select item.Value).ToList();
            var stackSeries = OxyPlotUtilities.StackLineSeries(lineSeriesSet);

            // add them to the plot
            foreach (var languageItems in stackSeries)
                newPlot.Series.Add(languageItems);

            // finally update the model with the new plot
            return newPlot;
        }

        /// <summary>
        /// Sets up the axes for the plot.
        /// </summary>
        /// <param name="newPlot">The plot to set up the axes for.</param>
        private void SetupTotalPagesReadKeyVsTimeAxes(PlotModel newPlot)
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
                Title = "Total Pages Read",
                Key = ChartAxisKeys.TotalPagesReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }
    }
}
