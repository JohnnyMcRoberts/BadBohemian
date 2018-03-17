// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base line chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.LineCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Defaults;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;

    /// <summary>
    /// The base line chart view model class.
    /// </summary>
    public class BaseLineChartViewModel : BaseChartViewModel
    {
        /// <summary>
        /// The minimum X Axis Value.
        /// </summary>
        private double _minTickX;

        /// <summary>
        ///The maximum X Axis Value.
        /// </summary>
        private double _maxTickX;

        /// <summary>
        /// The minimum X Axis Value.
        /// </summary>
        private DateTime _minX;

        /// <summary>
        ///The maximum X Axis Value.
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

        private LineSeries GetBasicLineSeries(string title, Color color, double pointSize)
        {
            // Set up the gradient brush.
            LinearGradientBrush gradientBrush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1)
            };

            gradientBrush.GradientStops.Add(pointSize > 0.1 ? new GradientStop(color, 0) : new GradientStop(Colors.Transparent, 1));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

            LineSeries lineSeries = new LineSeries
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
        /// Sets up the line chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            DateTime startTime = DateTime.Today.Subtract(new TimeSpan(300, 0, 0,0));
            MinX = MaxX = startTime;

            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            for (int i = 0; i < 3; i++)
            {
                Color color = colors[i % colors.Count];

                DateTime date = startTime;
                List<DateTime> dates = new List<DateTime>();
                List<double> yValues = new List<double>();
                for (int j = 0; j < 10; j++)
                {
                    int timeIncrement = random.Next(1, 10);
                    date = date.Add(new TimeSpan(timeIncrement, 0, 0, 0));
                    dates.Add(date);
                    yValues.Add(random.NextDouble() * 100);
                }

                UpdateMaxMinValues(dates, yValues);

                seriesViews.Add(CreateLineSeries($"Test {1 + i}", dates, yValues, color));
            }

            Series.AddRange(seriesViews);
            SeriesCollection = Series;
        }

        /// <summary>
        /// Creates a new line series point with a given name and value.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="xValues">The x values for the series.</param>
        /// <param name="yValues">The y values for the series.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created line series.</returns>
        public LineSeries CreateLineSeries(string title, List<double> xValues, List<double> yValues, Color color, double pointSize = 15d)
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
            LineSeries lineSeries = GetBasicLineSeries(title, color, pointSize);
            lineSeries.Values = values;

            return lineSeries;
        }

        /// <summary>
        /// Creates a new line series point with a given name and value.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="xValues">The x date values for the series.</param>
        /// <param name="yValues">The y values for the series.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created line series.</returns>
        public LineSeries CreateLineSeries(string title, List<DateTime> xValues, List<double> yValues, Color color, double pointSize = 15d)
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
            LineSeries lineSeries = GetBasicLineSeries(title, color, pointSize);
            lineSeries.Values = values;

            return lineSeries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseLineChartViewModel"/> class.
        /// </summary>
        public BaseLineChartViewModel()
        {
            Initialise();
        }
    }
}
