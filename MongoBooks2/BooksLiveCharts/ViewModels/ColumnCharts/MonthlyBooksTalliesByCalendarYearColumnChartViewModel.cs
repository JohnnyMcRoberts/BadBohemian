// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonthlyBooksTalliesByCalendarYearColumnChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The monthly books tallies by calendar year chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.ColumnCharts
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
    /// The monthly books tallies by calendar year chart view model class.
    /// </summary>
    public sealed class MonthlyBooksTalliesByCalendarYearColumnChartViewModel : BaseColumnChartViewModel
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

            // Set the categories.
            Categories = BookTotalsUtilities.MonthNames.Skip(1).Take(12).ToArray();

            // Set up the axis names and formatters.
            XAxisTitle = "Month of the year";
            YAxisTitle = "Books Read";

            // get the books & pages read for each calendar year
            Dictionary<int, List<MonthOfYearTally>> bookListsByMonthOfYear =
                BookTotalsUtilities.GetBookListsByMonthOfYear(BooksReadProvider);

            // Set up the series collection and initialise the min and max.
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
                for (int i = BookTotalsUtilities.FirstMonth; i <= BookTotalsUtilities.LastMonth; i++)
                {
                    // Get the tally for this month if set, otherwise set to zero
                    MonthOfYearTally tally = bookListsByMonthOfYear[year].FirstOrDefault(x => x.MonthOfYear == i);
                    booksReadSeriesValues.Add(tally?.BooksReadThisMonth ?? 0);
                }

                // Create the series for the year.
                Color color = stdColors[colourIndex % stdColors.Count];
                seriesViews.Add(CreateColumnSeries(year.ToString(), booksReadSeriesValues, color, 0d));
                colourIndex++;

                // Update the Y-range.
                MinY = Math.Floor(Math.Min(booksReadSeriesValues.Min(), MinY));
                MaxY = Math.Ceiling(Math.Max(booksReadSeriesValues.Max(), MaxY));
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyBooksTalliesByCalendarYearColumnChartViewModel"/> class.
        /// </summary>
        public MonthlyBooksTalliesByCalendarYearColumnChartViewModel()
        {
            Title = "Monthly Books By Calendar Year";
            PointLabel = chartPoint => $"({Categories[chartPoint.Key]}, {YAxisTitle} {chartPoint.Y:G5})";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
