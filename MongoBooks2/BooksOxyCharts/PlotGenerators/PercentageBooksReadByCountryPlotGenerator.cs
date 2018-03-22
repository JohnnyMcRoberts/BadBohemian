// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentageBooksReadByCountryPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The percentage books read by country with time plot generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BooksCore.Books;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    /// <summary>
    /// The percentage books read by country with time plot generator.
    /// </summary>
    public class PercentageBooksReadByCountryPlotGenerator : BasePlotGenerator
    {
        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Percentage Books Read by Country With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Percentage Books Read by Country With Time Plot");
            SetupPercentageBooksReadKeyVsTimeAxes(newPlot);

            // get the countries (in order)
            BooksDelta.DeltaTally latestTally = BooksReadProvider.BookDeltas.Last().OverallTally;
            List<string> countries = (from item in latestTally.CountryTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // create the series for the languages
            List<KeyValuePair<string, LineSeries>> countriesSeries =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < countries.Count; i++)
            {
                LineSeries countrySeries;
                OxyPlotUtilities.CreateLongLineSeries(
                    out countrySeries,
                    ChartAxisKeys.DateKey,
                    ChartAxisKeys.PercentageBooksReadKey,
                    countries[i],
                    i);
                countriesSeries.Add(new KeyValuePair<string, LineSeries>(countries[i], countrySeries));
            }

            // loop through the deltas adding points for each of the items
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                double date = DateTimeAxis.ToDouble(delta.Date);
                foreach (KeyValuePair<string, LineSeries> countryLine in countriesSeries)
                {
                    double percentage = 0.0;
                    foreach (Tuple<string, uint, double, uint, double> countryTotal in delta.OverallTally.CountryTotals)
                    {
                        if (countryTotal.Item1 == countryLine.Key)
                        {
                            percentage = countryTotal.Item3;
                        }
                    }

                    countryLine.Value.Points.Add(new DataPoint(date, percentage));
                }
            }

            // add them to the plot
            foreach (KeyValuePair<string, LineSeries> countryItems in countriesSeries)
            {
                newPlot.Series.Add(countryItems.Value);
            }

            // finally update the model with the new plot
            return newPlot;
        }

        /// <summary>
        /// Sets up the axes for the plot.
        /// </summary>
        /// <param name="newPlot">The plot to set up the axes for.</param>
        private void SetupPercentageBooksReadKeyVsTimeAxes(PlotModel newPlot)
        {
            DateTimeAxis xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = ChartAxisKeys.DateKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                StringFormat = "yyyy-MM-dd"
            };

            newPlot.Axes.Add(xAxis);

            LinearAxis lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "% of Books Read",
                Key = ChartAxisKeys.PercentageBooksReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };

            newPlot.Axes.Add(lhsAxis);
        }
    }
}
