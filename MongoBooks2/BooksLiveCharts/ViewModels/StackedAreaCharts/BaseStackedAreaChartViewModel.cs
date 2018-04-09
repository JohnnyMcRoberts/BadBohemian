// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base stacked area chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.StackedAreaCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksCore.Books;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Defaults;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;

    /// <summary>
    /// The base stacked area chart view model class.
    /// </summary>
    public class BaseStackedAreaChartViewModel : BaseChartViewModel
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
        /// The minimum X Axis Value.
        /// </summary>
        private double _minTickX;

        /// <summary>
        /// The maximum X Axis Value.
        /// </summary>
        private double _maxTickX;

        /// <summary>
        /// The minimum X Axis Value.
        /// </summary>
        private DateTime _minX;

        /// <summary>
        /// The maximum X Axis Value.
        /// </summary>
        private DateTime _maxX;

        /// <summary>
        /// The minimum Y Axis Value.
        /// </summary>
        private double _minY;

        /// <summary>
        /// The maximum Y Axis Value.
        /// </summary>
        private double _maxY;

        /// <summary>
        /// Gets or sets X Axis Title.
        /// </summary>
        private string _xAxisString;

        /// <summary>
        /// Gets or sets Y Axis Title.
        /// </summary>
        private string _yAxisString;

        /// <summary>
        /// Gets the minimum X Axis tick value.
        /// </summary>
        public double MinTick
        {
            get
            {
                return _minTickX;
            }

            protected set
            {
                _minTickX = value;
                OnPropertyChanged(() => MinTick);
            }
        }

        /// <summary>
        /// Gets the maximum X Axis tick value.
        /// </summary>
        public double MaxTick
        {
            get
            {
                return _maxTickX;
            }

            protected set
            {
                _maxTickX = value;
                OnPropertyChanged(() => MaxTick);
            }
        }

        /// <summary>
        /// Gets or sets the minimum X Axis Value.
        /// </summary>
        public DateTime MinX
        {
            get
            {
                return _minX;
            }

            protected set
            {
                _minX = value;
                OnPropertyChanged(() => MinX);
                OnPropertyChanged(() => MinTick);
            }
        }

        /// <summary>
        /// Gets or sets the maximum X Axis Value.
        /// </summary>
        public DateTime MaxX
        {
            get
            {
                return _maxX;
            }

            protected set
            {
                _maxX = value;
                OnPropertyChanged(() => MaxX);
                OnPropertyChanged(() => MaxTick);
            }
        }

        /// <summary>
        /// Gets or sets the minimum Y Axis Value.
        /// </summary>
        public double MinY
        {
            get
            {
                return _minY;
            }

            protected set
            {
                _minY = value;
                OnPropertyChanged(() => MinY);
            }
        }

        /// <summary>
        /// Gets or sets the maximum Y Axis Value.
        /// </summary>
        public double MaxY
        {
            get
            {
                return _maxY;
            }

            protected set
            {
                _maxY = value;
                OnPropertyChanged(() => MaxY);
            }
        }

        /// <summary>
        /// Gets or sets X Axis Title.
        /// </summary>
        public string XAxisTitle
        {
            get
            {
                return _xAxisString;
            }

            protected set
            {
                if (value != _xAxisString)
                {
                    _xAxisString = value;
                    OnPropertyChanged(() => XAxisTitle);
                }
            }
        }

        /// <summary>
        /// Gets or sets Y Axis Title.
        /// </summary>
        public string YAxisTitle
        {
            get
            {
                return _yAxisString;
            }

            protected set
            {
                if (value != _yAxisString)
                {
                    _yAxisString = value;
                    OnPropertyChanged(() => YAxisTitle);
                }
            }
        }

        /// <summary>
        /// Gets or sets the series date values formatter.
        /// </summary>
        public Func<double, string> XValueFormatter { get; set; }

        /// <summary>
        /// Gets or sets the series numeric values formatter.
        /// </summary>
        public Func<double, string> YValueFormatter { get; set; }

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
        /// Initialises line chart title, series etc.
        /// </summary>
        private void Initialise()
        {
            XAxisTitle = "X dates";
            YAxisTitle = "Y values";

            MinX = DateTime.MaxValue;
            MaxX = DateTime.MinValue;

            MinY = 0;
            MaxY = 1;

            PointLabel = chartPoint => $"Pt ({XAxisTitle} {new DateTime((long)chartPoint.X):d} , {YAxisTitle} {chartPoint.Y:G})";

            XValueFormatter = value => $"{new DateTime((long)value):d}";
            YValueFormatter = value => $"{value:G}";

            LegendLocation = LegendLocation.None;
            SetupSeries();
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
        /// Gets a new stacked area series for a given point size, color and title.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created line series.</returns>
        private StackedAreaSeries GetBasicStackedAreaSeries(string title, Color color, double pointSize)
        {
            // Set up the gradient brush.
            LinearGradientBrush gradientBrush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1)
            };

            gradientBrush.GradientStops.Add(pointSize > 0.1 ? new GradientStop(color, 0) : new GradientStop(Colors.Transparent, 1));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

            StackedAreaSeries lineSeries = new StackedAreaSeries
            {
                Title = title,
                DataLabels = false,
                LabelPoint = PointLabel,
                LineSmoothness = 0.2,
                PointGeometrySize = pointSize,
                Foreground = new SolidColorBrush(color),
                PointForeground = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(color),
                Fill = gradientBrush
            };

            return lineSeries;
        }

        /// <summary>
        /// Gets the dates and the line series values for the countries.
        /// </summary>
        /// <param name="dates">The dates for the values.</param>
        /// <param name="countryOrLanguageNamesAndValues">The countries and the associated percentages.</param>
        /// <param name="isBooks">True if the chart is for books, false for pages.</param>
        /// <param name="isCountries">True if the chart is for countries, false for languages.</param>
        private void GetCountryOrLanguageSeries(
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
                {
                    continue;
                }

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
                    {
                        continue;
                    }

                    List<Tuple<string, uint, double, uint, double>> totals =
                        isCountries ? delta.OverallTally.CountryTotals : delta.OverallTally.LanguageTotals;
                    double percentage = 0;

                    foreach (Tuple<string, uint, double, uint, double> countryOrLanguageTotal in totals)
                    {
                        if (countryOrLanguageTotal.Item1 == countryOrLanguage)
                        {
                            percentage = isBooks ? countryOrLanguageTotal.Item2 : countryOrLanguageTotal.Item4;
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
                return;
            }

            // Set up the axis names.
            XAxisTitle = "Date";
            YAxisTitle = isBooks ? "Total Books Read" : "Total Pages Read";

            // Get the data series for each country or language.
            List<DateTime> dates;
            List<Tuple<string, List<double>>> countryOrLanguageNamesAndValues;
            GetCountryOrLanguageSeries(out dates, out countryOrLanguageNamesAndValues, isBooks, isCountries);

            // Set up the colours for the series.
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            // Add a series per country.
            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();

            List<double> totals = new List<double>();
            for (int i = 0; i < countryOrLanguageNamesAndValues[0].Item2.Count; i++)
            {
                totals.Add(0);
            }

            for (int i = 0; i < countryOrLanguageNamesAndValues.Count && i < MaximumSeries; i++)
            {
                Color color = colors[i % colors.Count];
                Tuple<string, List<double>> countryOrLanguageValues = countryOrLanguageNamesAndValues[i];
                seriesViews.Add(CreateStackedAreaSeries(countryOrLanguageValues.Item1, dates, countryOrLanguageValues.Item2, color, 1d));

                for (int j = 0; j < countryOrLanguageValues.Item2.Count; j++)
                {
                    totals[j] += countryOrLanguageValues.Item2[j];
                }
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;

            MinY = Math.Floor(Math.Min(totals.Min(), 0));
            MaxY = Math.Ceiling(totals.Max());
        }

        /// <summary>
        /// Update the maximum and minimum values for the series.
        /// </summary>
        protected void UpdateMaxMinValues(List<DateTime> xValues, List<double> yValues)
        {
            MinX = MinX.Ticks < xValues.Min().Ticks ? MinX : xValues.Min();
            MaxX = MaxX.Ticks > xValues.Max().Ticks ? MaxX : xValues.Max();

            MinTick = MinX.Ticks;
            MaxTick = MaxX.Ticks;

            MinY = Math.Min(yValues.Min(), MinY);
            MaxY = Math.Max(yValues.Max(), MaxY);
        }

        /// <summary>
        /// Update the maximum and minimum values for the series.
        /// </summary>
        protected void UpdateMaxMinValues(List<double> xValues, List<double> yValues)
        {
            MinTick = MinTick < xValues.Min() ? MinTick : xValues.Min();
            MaxTick = MaxTick > xValues.Max() ? MaxTick : xValues.Max();
            MinY = Math.Min(yValues.Min(), MinY);
            MaxY = Math.Max(yValues.Max(), MaxY);
        }

        /// <summary>
        /// Sets up the stacked area chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            DateTime startTime = DateTime.Today.Subtract(new TimeSpan(300, 0, 0, 0));
            MinX = MaxX = startTime;

            Series = new SeriesCollection();
            List<DateTime> dates = new List<DateTime>();
            List<double> totals = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                totals.Add(0);
            }

            List<ISeriesView> seriesViews = new List<ISeriesView>();
            for (int i = 0; i < 3; i++)
            {
                Color color = colors[i % colors.Count];

                DateTime date = startTime;
                List<double> yValues = new List<double>();
                for (int j = 0; j < 10; j++)
                {
                    if (dates.Count < 10)
                    {
                        int timeIncrement = random.Next(1, 10);
                        date = date.Add(new TimeSpan(timeIncrement, 0, 0, 0));
                        dates.Add(date);
                    }
                    yValues.Add(random.NextDouble() * 100);
                    totals[j] += yValues[j];
                }

                UpdateMaxMinValues(dates, totals);

                seriesViews.Add(CreateStackedAreaSeries($"Test {1 + i}", dates, yValues, color));
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;
        }

        /// <summary>
        /// Creates a new stacked area series point with a given name and value.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="xValues">The x values for the series.</param>
        /// <param name="yValues">The y values for the series.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created line series.</returns>
        public StackedAreaSeries CreateStackedAreaSeries(string title, List<double> xValues, List<double> yValues, Color color, double pointSize = 15d)
        {
            // Update the maxima and minima.
            UpdateMaxMinValues(xValues, yValues);

            // Get the date time point values.
            ChartValues<ObservablePoint> values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
            {
                values.Add(new ObservablePoint(xValues[i], yValues[i]));
            }

            // Finally create the line series and set the values.
            StackedAreaSeries lineSeries = GetBasicStackedAreaSeries(title, color, pointSize);
            lineSeries.Values = values;

            return lineSeries;
        }

        /// <summary>
        /// Creates a new stacked area series point with a given name and value.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="xValues">The x date values for the series.</param>
        /// <param name="yValues">The y values for the series.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created line series.</returns>
        public StackedAreaSeries CreateStackedAreaSeries(string title, List<DateTime> xValues, List<double> yValues, Color color, double pointSize = 15d)
        {
            // Update the maxima and minima.
            UpdateMaxMinValues(xValues, yValues);

            // Get the date time point values.
            ChartValues<DateTimePoint> values = new ChartValues<DateTimePoint>();
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
            {
                values.Add(new DateTimePoint(xValues[i], yValues[i]));
            }

            // Finally create the line series and set the values.
            StackedAreaSeries lineSeries = GetBasicStackedAreaSeries(title, color, pointSize);
            lineSeries.Values = values;

            return lineSeries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStackedAreaChartViewModel"/> class.
        /// </summary>
        public BaseStackedAreaChartViewModel()
        {
            Initialise();
        }
    }
}
