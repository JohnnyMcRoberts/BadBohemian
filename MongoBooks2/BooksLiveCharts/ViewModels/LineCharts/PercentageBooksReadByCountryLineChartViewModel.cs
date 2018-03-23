// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentageBooksReadByCountryLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The percentage books read by country with time line chart view model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksCore.Books;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Definitions.Series;

    /// <summary>
    /// The percentage books read by country with time line chart view model class.
    /// </summary>
    public sealed class PercentageBooksReadByCountryLineChartViewModel : BaseLineChartViewModel
    {
        /// <summary>
        /// The fewest days per delta date.
        /// </summary>
        public const int MinDaysPerDelta = 5;

        /// <summary>
        /// The fewest days per delta date.
        /// </summary>
        public const int MaximumSeries = 10;

        /// <summary>
        /// Sets up the line chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            // If no books return the default.
            if (BooksReadProvider == null)
            {
                base.SetupSeries();
                return;
            }

            // Set up the axis names.
            XAxisTitle = "Date";
            YAxisTitle = "% of Books Read";

            // Get the data series for each country.
            List<DateTime> dates;
            List<Tuple<string, List<double>>> countryNamesAndValues;
            GetCountrySeries(out dates, out countryNamesAndValues);

            // Set up the colours for the series.
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            // Add a series per country.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<double> allValues = new List<double>();
            for (int i = 0; i < countryNamesAndValues.Count && i < MaximumSeries; i++)
            {
                Color color = colors[i % colors.Count];
                Tuple<string, List<double>> countryValues = countryNamesAndValues[i];
                seriesViews.Add(CreateLineSeries(countryValues.Item1, dates, countryValues.Item2, color, 0d));
                allValues.Add(countryValues.Item2.Max());
                allValues.Add(countryValues.Item2.Min());
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;
            
            MinY = Math.Floor(allValues.Min());
            MaxY = Math.Ceiling(allValues.Max());
        }

        /// <summary>
        /// Gets the dates and the line series values for the countries.
        /// </summary>
        /// <param name="dates">The dates for the values.</param>
        /// <param name="countryNamesAndValues">The countries and the associated percentages.</param>
        private void GetCountrySeries(out List<DateTime> dates, out List<Tuple<string, List<double>>> countryNamesAndValues)
        {
            // Get the countries (in order)
            BooksDelta.DeltaTally latestTally = BooksReadProvider.BookDeltas.Last().OverallTally;
            List<string> countries = (from item in latestTally.CountryTotals
                orderby item.Item2 descending
                select item.Item1).ToList();

            // First get the dates.
            dates = new List<DateTime>();
            DateTime previousDateTime = BooksReadProvider.BookDeltas.First().Date;
            dates.Add(previousDateTime);
            foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
            {
                if ((delta.Date - previousDateTime).Days < MinDaysPerDelta)
                    continue;

                previousDateTime = delta.Date;
                dates.Add(previousDateTime);
            }

            // Loop through the deltas adding points for each of the items
            countryNamesAndValues = new List<Tuple<string, List<double>>>();
            foreach (string country in countries)
            {
                List<double> countryValues = new List<double>();
                foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
                {
                    if (!dates.Contains(delta.Date))
                        continue;

                    double percentage = 0;
                    foreach (Tuple<string, uint, double, uint, double> countryTotal in delta.OverallTally.CountryTotals)
                    {
                        if (countryTotal.Item1 == country)
                        {
                            percentage = countryTotal.Item3;
                        }
                    }

                    countryValues.Add(percentage);
                }

                countryNamesAndValues.Add(new Tuple<string, List<double>>(country, countryValues));
            }

            // If more series than can be displayed group them 'Other'
            if (MaximumSeries < countries.Count)
            {
                List<double> otherTotals = new List<double>();

                for (int dateIndex = 0; dateIndex < dates.Count; dateIndex++)
                {
                    double total = 0d;
                    for (int languageIndex = MaximumSeries - 1; languageIndex < countries.Count; languageIndex++)
                    {
                        total += countryNamesAndValues[languageIndex].Item2[dateIndex];
                    }

                    otherTotals.Add(total);
                }

                // Remove the othe countries and replace with the other total
                countryNamesAndValues.RemoveRange(MaximumSeries - 1, countryNamesAndValues.Count - (MaximumSeries - 1));
                countryNamesAndValues.Add(new Tuple<string, List<double>>("Other", otherTotals));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentageBooksReadByCountryLineChartViewModel"/> class.
        /// </summary>
        public PercentageBooksReadByCountryLineChartViewModel()
        {
            Title = "Percentage Books Read by Country With Time";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Top;
            SetupSeries();
        }
    }
}
