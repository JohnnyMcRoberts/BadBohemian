// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePercentageLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base class for the percentage books or pages read by country or language with time line chart view model class.
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
    public abstract class BasePercentageLineChartViewModel : BaseLineChartViewModel
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
        /// Groups the smaller series into a single 'Other' series.
        /// </summary>
        /// <param name="dates">The dates the individual values are for.</param>
        /// <param name="countryOrLanguageNamesAndValues">The country or language names with their series values.</param>
        /// <param name="countriesOrLanguages">The names of the countries or languages.</param>
        private static void GroupOtherTotals(
            List<DateTime> dates, 
            List<Tuple<string, List<double>>> countryOrLanguageNamesAndValues, 
            List<string> countriesOrLanguages)
        {
            List<double> otherTotals = new List<double>();

            for (int dateIndex = 0; dateIndex < dates.Count; dateIndex++)
            {
                double total = 0d;
                for
                (
                    int countriesOrLanguageIndex = MaximumSeries - 1;
                    countriesOrLanguageIndex < countriesOrLanguages.Count;
                    countriesOrLanguageIndex++
                )
                {
                    total += countryOrLanguageNamesAndValues[countriesOrLanguageIndex].Item2[dateIndex];
                }

                otherTotals.Add(total);
            }

            // Remove the othe countrie or languages totals and replace with the 'Other' total.
            countryOrLanguageNamesAndValues.RemoveRange(MaximumSeries - 1, countryOrLanguageNamesAndValues.Count - (MaximumSeries - 1));
            countryOrLanguageNamesAndValues.Add(new Tuple<string, List<double>>("Other", otherTotals));
        }

        /// <summary>
        /// Gets up the country or language names for line chart series.
        /// </summary>
        /// <param name="isBooks">True if the chart is for books, false for pages.</param>
        /// <param name="isCountries">True if the chart is for countries, false for languages.</param>
        private List<string> GetCountriesOrLanguagesNames(bool isBooks, bool isCountries)
        {
            BooksDelta.DeltaTally latestTally = BooksReadProvider.BookDeltas.Last().OverallTally;
            List<string> countriesOrLanguages;

            if (isCountries)
            {
                if (isBooks)
                {
                    countriesOrLanguages = (from item in latestTally.CountryTotals
                        orderby item.Item2 descending
                        select item.Item1).ToList();
                }
                else
                {
                    countriesOrLanguages = (from item in latestTally.CountryTotals
                        orderby item.Item5 descending
                        select item.Item1).ToList();
                }
            }
            else
            {
                if (isBooks)
                {
                    countriesOrLanguages = (from item in latestTally.LanguageTotals
                        orderby item.Item2 descending
                        select item.Item1).ToList();
                }
                else
                {
                    countriesOrLanguages = (from item in latestTally.LanguageTotals
                        orderby item.Item5 descending
                        select item.Item1).ToList();
                }
            }

            return countriesOrLanguages;
        }

        /// <summary>
        /// Sets up the line chart series.
        /// </summary>
        /// <param name="isBooks">True if the chart is for books, false for pages.</param>
        /// <param name="isCountries">True if the chart is for countries, false for languages.</param>
        protected virtual void SetupSeries(bool isBooks, bool isCountries)
        {
            // If no books return the default.
            if (BooksReadProvider == null)
            {
                base.SetupSeries();
                return;
            }

            // Set up the axis names.
            XAxisTitle = "Date";
            YAxisTitle = isBooks ? "% of Books Read" : "% of Pages Read";

            // Get the data series for each country or language.
            List<DateTime> dates;
            List<Tuple<string, List<double>>> countryOrLanguageNamesAndValues;
            GetCountrySeries(out dates, out countryOrLanguageNamesAndValues, isBooks, isCountries);

            // Set up the colours for the series.
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            // Add a series per country.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            List<double> allValues = new List<double>();
            for (int i = 0; i < countryOrLanguageNamesAndValues.Count && i < MaximumSeries; i++)
            {
                Color color = colors[i % colors.Count];
                Tuple<string, List<double>> countryOrLanguageValues = countryOrLanguageNamesAndValues[i];
                seriesViews.Add(CreateLineSeries(countryOrLanguageValues.Item1, dates, countryOrLanguageValues.Item2, color, 0d));
                allValues.Add(countryOrLanguageValues.Item2.Max());
                allValues.Add(countryOrLanguageValues.Item2.Min());
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
        /// <param name="countryOrLanguageNamesAndValues">The countries and the associated percentages.</param>
        /// <param name="isBooks">True if the chart is for books, false for pages.</param>
        /// <param name="isCountries">True if the chart is for countries, false for languages.</param>
        private void GetCountrySeries(
            out List<DateTime> dates,
            out List<Tuple<string, List<double>>> countryOrLanguageNamesAndValues,
            bool isBooks,
            bool isCountries)
        {
            // Get the countries (in order)
            List<string> countriesOrLanguages = GetCountriesOrLanguagesNames(isBooks, isCountries);

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
            countryOrLanguageNamesAndValues = new List<Tuple<string, List<double>>>();
            foreach (string countryOrLanguage in countriesOrLanguages)
            {
                List<double> countryOrLanguageValues = new List<double>();
                foreach (BooksDelta delta in BooksReadProvider.BookDeltas)
                {
                    if (!dates.Contains(delta.Date))
                        continue;

                    List<Tuple<string, uint, double, uint, double>> totals =
                        isCountries ? delta.OverallTally.CountryTotals : delta.OverallTally.LanguageTotals;
                    double percentage = 0;

                    foreach (Tuple<string, uint, double, uint, double> countryOrLanguageTotal in totals)
                    {
                        if (countryOrLanguageTotal.Item1 == countryOrLanguage)
                        {
                            percentage = isBooks ? countryOrLanguageTotal.Item3 : countryOrLanguageTotal.Item5;
                        }
                    }

                    countryOrLanguageValues.Add(percentage);
                }

                countryOrLanguageNamesAndValues.Add(new Tuple<string, List<double>>(countryOrLanguage, countryOrLanguageValues));
            }

            // If more series than can be displayed group them 'Other'
            if (MaximumSeries < countriesOrLanguages.Count)
            {
                GroupOtherTotals(dates, countryOrLanguageNamesAndValues, countriesOrLanguages);
            }
        }
    }
}