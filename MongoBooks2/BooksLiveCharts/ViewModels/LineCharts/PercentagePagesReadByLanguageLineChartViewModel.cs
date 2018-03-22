// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentagePagesReadByLanguageLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The percentage pages read by language with time line chart view model class.
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
    /// The percentage pages read by language with time line chart view model class.
    /// </summary>
    public sealed class PercentagePagesReadByLanguageLineChartViewModel : BaseLineChartViewModel
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
            YAxisTitle = "% of Pages Read";

            // Get the data series for each language.
            List<DateTime> dates;
            List<Tuple<string, List<double>>> languageNamesAndValues;
            GetLanguageSeries(out dates, out languageNamesAndValues);

            // Set up the colours for the series.
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            // Add a series per language.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<double> allValues = new List<double>();
            for (int i = 0; i < languageNamesAndValues.Count && i < MaximumSeries; i++)
            {
                Color color = colors[i % colors.Count];
                Tuple<string, List<double>> languageValues = languageNamesAndValues[i];
                seriesViews.Add(CreateLineSeries(languageValues.Item1, dates, languageValues.Item2, color, 0d));
                allValues.Add(languageValues.Item2.Max());
                allValues.Add(languageValues.Item2.Min());
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
        /// <param name="languageNamesAndValues">The countries and the associated percentages.</param>
        private void GetLanguageSeries(out List<DateTime> dates, out List<Tuple<string, List<double>>> languageNamesAndValues)
        {
            // Get the countries (in order)
            BooksDelta.DeltaTally latestTally = BooksReadProvider.BookDeltas.Last().OverallTally;
            List<string> languages = (from item in latestTally.LanguageTotals
                                      orderby item.Item4 descending
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
            languageNamesAndValues = new List<Tuple<string, List<double>>>();
            foreach (string language in languages)
            {
                List<double> countryValues = new List<double>();
                foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
                {
                    if (!dates.Contains(delta.Date))
                        continue;

                    double percentage = 0;
                    foreach (Tuple<string, uint, double, uint, double> countryTotal in delta.OverallTally.LanguageTotals)
                    {
                        if (countryTotal.Item1 == language)
                        {
                            percentage = countryTotal.Item5;
                        }
                    }

                    countryValues.Add(percentage);
                }

                languageNamesAndValues.Add(new Tuple<string, List<double>>(language, countryValues));
            }

            // If more series than can be displayed group them 'Other'
            if (MaximumSeries < languages.Count)
            {
                List<double> otherTotals = new List<double>();

                for (int dateIndex = 0; dateIndex < dates.Count; dateIndex++)
                {
                    double total = 0d;
                    for (int languageIndex = MaximumSeries - 1; languageIndex < languages.Count; languageIndex++)
                    {
                        total += languageNamesAndValues[languageIndex].Item2[dateIndex];
                    }

                    otherTotals.Add(total);
                }

                // Remove the othe languages and replace with the other total
                languageNamesAndValues.RemoveRange(MaximumSeries - 1, languageNamesAndValues.Count - (MaximumSeries - 1));
                languageNamesAndValues.Add(new Tuple<string, List<double>>("Other", otherTotals));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentagePagesReadByLanguageLineChartViewModel"/> class.
        /// </summary>
        public PercentagePagesReadByLanguageLineChartViewModel()
        {
            Title = "Percentage Pages Read by Language With Time";
            PointLabel = chartPoint => $"({XAxisTitle} {new DateTime((long)chartPoint.X):d}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Top;
            SetupSeries();
        }
    }
}