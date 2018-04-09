// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseScatterChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base scatter chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.ColumnCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using LiveCharts;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;

    /// <summary>
    /// The base column chart view model class.
    /// </summary>
    public class BaseColumnChartViewModel : BaseChartViewModel
    {
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
        /// Gets or sets the labels for the X Axis.
        /// </summary>
        public string[] Categories { get; protected set; }

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
        /// Initialises column chart title, series etc.
        /// </summary>
        private void Initialise()
        {
            XAxisTitle = "X";
            YAxisTitle = "Y";
            PointLabel = chartPoint => $"({XAxisTitle} {chartPoint.X} , {YAxisTitle} {chartPoint.Y:G5})";

            Formatter = value => value.ToString("N");

            LegendLocation = LegendLocation.None;
            SetupSeries();
        }


        /// <summary>
        /// Gets a new column series for a given point size, color and title.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created column series.</returns>
        private ColumnSeries GetBasicColumnSeries(string title, Color color, double pointSize)
        {
            // Set up the gradient brush.
            LinearGradientBrush gradientBrush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1)
            };

            gradientBrush.GradientStops.Add(pointSize > 0.1 ? new GradientStop(color, 0) : new GradientStop(Colors.Transparent, 1));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
            SolidColorBrush solidColor = new SolidColorBrush(color);

            ColumnSeries columnSeries = new ColumnSeries
            {
                Title = title,
                DataLabels = false,
                LabelPoint = PointLabel,
                Foreground = solidColor,
                MinWidth = 40d,
                ColumnPadding = 5d,
                MaxColumnWidth = 80,
                Stroke = solidColor
            };

            columnSeries.Fill = pointSize > 0 ? (Brush) gradientBrush : solidColor;

            return columnSeries;
        }

        /// <summary>
        /// Sets up the column chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Categories = new[] { "Maria", "Susan", "Charles", "Frida" };
            Series = new SeriesCollection();

            MinY = 0;
            MaxY = 1;

            List<ISeriesView> seriesViews = new List<ISeriesView>();

            seriesViews.Add(CreateColumnSeries("2015", new List<double> { 10, 50, 39, 50 }, Colors.Red, 5d));
            seriesViews.Add(CreateColumnSeries("2015", new List<double> { 11, 56, 42, 48 }, Colors.Blue, 5d));
            seriesViews.Add(CreateColumnSeries("2015", new List<double> { 52, 18, 64, 62 }, Colors.Green, 5d));

            Series.AddRange(seriesViews);
            SeriesCollection = Series;
        }

        /// <summary>
        /// Creates a new column series point with a given name and value.
        /// </summary>
        /// <param name="title">The line series title.</param>
        /// <param name="yValues">The y values for the series.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created line series.</returns>
        public ColumnSeries CreateColumnSeries(string title, List<double> yValues, Color color, double pointSize = 15d)
        {
            // Update the maxima and minima.
            MaxY = Math.Max(MaxY, yValues.Max());
            MinY = Math.Min(MinY, yValues.Min());

            // Get the column values.
            ChartValues<double> values = new ChartValues<double>();
            for (int i = 0; i < yValues.Count && i < yValues.Count; i++)
            {
                values.Add(yValues[i]);
            }

            // Finally create the line series and set the values.
            ColumnSeries columnSeries = GetBasicColumnSeries(title, color, pointSize);
            columnSeries.Values = values;

            return columnSeries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseColumnChartViewModel"/> class.
        /// </summary>
        public BaseColumnChartViewModel()
        {
            Initialise();
        }
    }
}
