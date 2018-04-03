// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseMultipleAxisLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base multiple-axis line chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.MultipleAxisLineCharts
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
    /// The base multiple-axis line chart view model class.
    /// </summary>
    public class BaseMultipleAxisLineChartViewModel : BaseChartViewModel
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
        /// The minimum LHS Y Axis Value.
        /// </summary>
        private double _minLeftHandSideY;

        /// <summary>
        /// The maximum LHS Y Axis Value.
        /// </summary>
        private double _maxLeftHandSideY;

        /// <summary>
        /// The minimum RHS Y Axis Value.
        /// </summary>
        private double _minRightHandSideY;

        /// <summary>
        /// The maximum RHS Y Axis Value.
        /// </summary>
        private double _maxRightHandSideY;

        /// <summary>
        /// Gets or sets X Axis Title.
        /// </summary>
        private string _xAxisString;

        /// <summary>
        /// Gets or sets LHS Y Axis Title.
        /// </summary>
        private string _leftHandSideYAxisString;

        /// <summary>
        /// Gets or sets RHS Y Axis Title.
        /// </summary>
        private string _rightHandSideYAxisString;

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
        /// Gets or sets the minimum Left Hand Side Y Axis Value.
        /// </summary>
        public double MinLeftHandSideY
        {
            get
            {
                return _minLeftHandSideY;
            }

            protected set
            {
                _minLeftHandSideY = value;
                OnPropertyChanged(() => MinLeftHandSideY);
            }
        }

        /// <summary>
        /// Gets or sets the maximum Left Hand Side Y Axis Value.
        /// </summary>
        public double MaxLeftHandSideY
        {
            get
            {
                return _maxLeftHandSideY;
            }

            protected set
            {
                _maxLeftHandSideY = value;
                OnPropertyChanged(() => MaxLeftHandSideY);
            }
        }

        /// <summary>
        /// Gets or sets the minimum Right Hand Side Y Axis Value.
        /// </summary>
        public double MinRightHandSideY
        {
            get
            {
                return _minRightHandSideY;
            }

            protected set
            {
                _minRightHandSideY = value;
                OnPropertyChanged(() => MinRightHandSideY);
            }
        }

        /// <summary>
        /// Gets or sets the maximum Right Hand Side Y Axis Value.
        /// </summary>
        public double MaxRightHandSideY
        {
            get
            {
                return _maxRightHandSideY;
            }

            protected set
            {
                _maxRightHandSideY = value;
                OnPropertyChanged(() => MaxRightHandSideY);
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
        public string LeftHandSideYAxisTitle
        {
            get
            {
                return _leftHandSideYAxisString;
            }

            protected set
            {
                if (value != _leftHandSideYAxisString)
                {
                    _leftHandSideYAxisString = value;
                    OnPropertyChanged(() => LeftHandSideYAxisTitle);
                }
            }
        }

        /// <summary>
        /// Gets or sets Y Axis Title.
        /// </summary>
        public string RightHandSideYAxisTitle
        {
            get
            {
                return _rightHandSideYAxisString;
            }

            protected set
            {
                if (value != _rightHandSideYAxisString)
                {
                    _rightHandSideYAxisString = value;
                    OnPropertyChanged(() => RightHandSideYAxisTitle);
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
        public Func<double, string> LeftHandSideYValueFormatter { get; set; }

        /// <summary>
        /// Gets or sets the series numeric values formatter.
        /// </summary>
        public Func<double, string> RightHandSideYValueFormatter { get; set; }

        /// <summary>
        /// Initialises line chart title, series etc.
        /// </summary>
        private void Initialise()
        {
            XAxisTitle = "X dates";
            LeftHandSideYAxisTitle = "Left Hand Side Y values";
            RightHandSideYAxisTitle = "Right Hand Side Y values";

            MinX = DateTime.MaxValue;
            MaxX = DateTime.MinValue;

            MinLeftHandSideY = 0;
            MaxLeftHandSideY = 1;

            MinRightHandSideY = 0;
            MaxRightHandSideY = 1;

            PointLabel = chartPoint =>
            {
                string yAxisName = chartPoint.SeriesView.ScalesYAt == 0
                    ? LeftHandSideYAxisTitle
                    : RightHandSideYAxisTitle;

                return $"Pt ({XAxisTitle} {new DateTime((long)chartPoint.X):d} , {yAxisName} {chartPoint.Y:G})";
            };

            XValueFormatter = value => $"{new DateTime((long)value):d}";
            LeftHandSideYValueFormatter = value => $"{value:G}";
            RightHandSideYValueFormatter = value => $"{value:G}";

            LegendLocation = LegendLocation.None;
            SetupSeries();
        }

        /// <summary>
        /// Gets a new line series for a given point size, color and title.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created line series.</returns>
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
        protected void UpdateMaxMinValues(List<DateTime> xValues, List<double> yValues, bool isLeftHandSide)
        {
            MinX = MinX.Ticks < xValues.Min().Ticks ? MinX : xValues.Min();
            MaxX = MaxX.Ticks > xValues.Max().Ticks ? MaxX : xValues.Max();

            MinTick = MinX.Ticks;
            MaxTick = MaxX.Ticks;

            if (isLeftHandSide)
            {
                MinLeftHandSideY = Math.Min(yValues.Min(), MinLeftHandSideY);
                MaxLeftHandSideY = Math.Max(yValues.Max(), MaxLeftHandSideY);
            }
            else
            {
                MinRightHandSideY = Math.Min(yValues.Min(), MinRightHandSideY);
                MaxRightHandSideY = Math.Max(yValues.Max(), MaxRightHandSideY);
            }
        }

        /// <summary>
        /// Update the maximum and minimum values for the series.
        /// </summary>
        protected void UpdateMaxMinValues(List<double> xValues, List<double> yValues, bool isLeftHandSide)
        {
            MinTick = MinTick < xValues.Min() ? MinTick : xValues.Min();
            MaxTick = MaxTick > xValues.Max() ? MaxTick : xValues.Max();
            if (isLeftHandSide)
            {
                MinLeftHandSideY = Math.Min(yValues.Min(), MinLeftHandSideY);
                MaxLeftHandSideY = Math.Max(yValues.Max(), MaxLeftHandSideY);
            }
            else
            {
                MinRightHandSideY = Math.Min(yValues.Min(), MinRightHandSideY);
                MaxRightHandSideY = Math.Max(yValues.Max(), MaxRightHandSideY);
            }
        }

        /// <summary>
        /// Sets up the line chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            DateTime startTime = DateTime.Today.Subtract(new TimeSpan(300, 0, 0, 0));
            MinX = MaxX = startTime;

            Series = new SeriesCollection();
            List<ISeriesView> seriesViews = new List<ISeriesView>();
            for (int i = 0; i < 4; i++)
            {
                Color color = colors[i % colors.Count];
                bool isLeftHandSide = (i % 2) == 0;

                DateTime date = startTime;
                List<DateTime> dates = new List<DateTime>();
                List<double> yValues = new List<double>();
                for (int j = 0; j < 10; j++)
                {
                    int timeIncrement = random.Next(1, 10);
                    date = date.Add(new TimeSpan(timeIncrement, 0, 0, 0));
                    dates.Add(date);
                    double yValue = random.NextDouble() * (isLeftHandSide ? 100 : 10);

                    yValues.Add(yValue);
                }

                UpdateMaxMinValues(dates, yValues, isLeftHandSide);

                LineSeries seriesView = CreateLineSeries($"Test {1 + i}", dates, yValues, color, leftHandSide: isLeftHandSide);
                seriesViews.Add(seriesView);
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
        /// <param name="leftHandSide">True if the axis is located on the left, false if it is on the right.</param>
        /// <returns>The newly created line series.</returns>
        public LineSeries CreateLineSeries(
            string title,
            List<double> xValues,
            List<double> yValues,
            Color color,
            double pointSize = 15d,
            bool leftHandSide = true)
        {
            // Update the maxima and minima.
            UpdateMaxMinValues(xValues, yValues, leftHandSide);

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
        /// <param name="leftHandSide">True if the axis is located on the left, false if it is on the right.</param>
        /// <returns>The newly created line series.</returns>
        public LineSeries CreateLineSeries(
            string title,
            List<DateTime> xValues,
            List<double> yValues,
            Color color,
            bool leftHandSide,
            double pointSize = 15d)
        {
            // Update the maxima and minima.
            UpdateMaxMinValues(xValues, yValues, leftHandSide);

            // Get the date time point values.
            ChartValues<DateTimePoint> values = new ChartValues<DateTimePoint>();
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
            {
                values.Add(new DateTimePoint(xValues[i], yValues[i]));
            }

            // Finally create the line series and set the values.
            LineSeries lineSeries = GetBasicLineSeries(title, color, pointSize);
            lineSeries.Values = values;
            lineSeries.ScalesYAt = leftHandSide ? 0 : 1;
            return lineSeries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMultipleAxisLineChartViewModel"/> class.
        /// </summary>
        public BaseMultipleAxisLineChartViewModel()
        {
            Initialise();
        }
    }
}
