// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonthlyBookTotalsPerYearLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The  monthly page totals for each calendar year line chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksCore.Utilities;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Definitions.Series;

    /// <summary>
    /// The monthly book totals for each calendar year line chart view model class.
    /// </summary>
    public sealed class MonthlyBookTotalsPerYearLineChartViewModel : BaseLineChartViewModel
    {
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

            // Set up the axis names and formatters.
            XAxisTitle = "Month of the year";
            YAxisTitle = "Books Read";

            // get the books & pages read for each calendar year
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthOfYear =
                BookTotalsUtilities.GetBookListsByMonthOfYear(BooksReadProvider);

            // Set up the series for the overall and the trendline.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            MinY = 0;
            MaxY = 1;

            // Add a series for each year.
            List<Color> stdColors = ColorUtilities.SetupStandardColourSet();
            int colourIndex = 0;
            foreach (int year in bookListsByMonthOfYear.Keys.ToList().OrderBy(x => x))
            {
                // Get the totals for the months.
                List<double> booksReadSeriesValues = new List<double>();
                List<double> months = new List<double>();
                double total = 0;
                for (int i = BookTotalsUtilities.FirstMonth; i <= BookTotalsUtilities.LastMonth; i++)
                {
                    // Get the tally for this month if set and add it to the total
                    MonthOfYearTally tally = bookListsByMonthOfYear[year].FirstOrDefault(x => x.MonthOfYear == i);
                    months.Add(i);
                    total += tally?.BooksReadThisMonth ?? 0;
                    booksReadSeriesValues.Add(total);
                }

                // Create the series for the year.
                Color color = stdColors[colourIndex % stdColors.Count];
                seriesViews.Add(CreateLineSeries(year.ToString(), months, booksReadSeriesValues, color, 0d));
                colourIndex++;

                // Update the Y-range.
                MinY = Math.Floor(Math.Min(booksReadSeriesValues.Min(), MinY));
                MaxY = Math.Ceiling(Math.Max(booksReadSeriesValues.Max(), MaxY));
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;

            // Update the X-range.
            MinTick = BookTotalsUtilities.FirstMonth - 1;
            MaxTick = BookTotalsUtilities.LastMonth + 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyBookTotalsPerYearLineChartViewModel"/> class.
        /// </summary>
        public MonthlyBookTotalsPerYearLineChartViewModel()
        {
            Title = "Monthly Books Read Totals by Year";
            PointLabel = chartPoint => $"{BookTotalsUtilities.MonthNames[(int)Math.Round(chartPoint.X)]} {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Bottom;

            XValueFormatter = value => $"{value:G}";
            YValueFormatter = value => $"{value:G}";
            SetupSeries();
        }
    }
}
