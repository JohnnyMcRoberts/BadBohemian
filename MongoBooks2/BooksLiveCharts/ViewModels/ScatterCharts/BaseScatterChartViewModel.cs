// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseScatterChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base scatter chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.ScatterCharts
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Defaults;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;

    /// <summary>
    /// The base scatter chart view model class.
    /// </summary>
    public class BaseScatterChartViewModel : BaseChartViewModel
    {
        /// <summary>
        /// The minimum X Axis Value.
        /// </summary>
        private double _minX;

        /// <summary>
        ///The maximum X Axis Value.
        /// </summary>
        private double _maxX;

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
        /// Gets or sets the minimum X Axis Value.
        /// </summary>
        public double MinX
        {
            get
            {
                return _minX;
            }

            protected set
            {
                _minX = value;
                OnPropertyChanged(() => MinX);
            }
        }

        /// <summary>
        /// Gets or sets the maximum X Axis Value.
        /// </summary>
        public double MaxX
        {
            get
            {
                return _maxX;
            }

            protected set
            {
                _maxX = value;
                OnPropertyChanged(() => MaxX);
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
        /// Initialises scatter chart title, series etc.
        /// </summary>
        private void Initialise()
        {
            XAxisTitle = "X";
            YAxisTitle = "Y";
            MinX = MinY = 0;
            MaxX = MaxY = 1;
            PointLabel = chartPoint => $"({XAxisTitle} {chartPoint.X:G} , {YAxisTitle} {chartPoint.Y:G})";
            Formatter = value => value.ToString("G3");

            LegendLocation = LegendLocation.None;
            SetupSeries();
        }

        /// <summary>
        /// Sets up the scatter chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Random random = new Random((int) DateTime.Now.Ticks);
            List<Color> colors = ColorUtilities.SetupStandardColourSet();
            Series = new SeriesCollection();

            List<ISeriesView> seriesViews = new List<ISeriesView>();
            for (int i = 0; i < 10; i++)
            {
                Color color = colors[i % colors.Count];
                seriesViews.Add(CreateScatterSeries($"Test {1+i}", random.NextDouble() * 10.0, random.NextDouble() * 10.0, color));
            }
            
            Series.AddRange(seriesViews);
            SeriesCollection = Series;
        }

        /// <summary>
        /// Creates a new scatter series point with a given name and value.
        /// </summary>
        /// <param name="title">The scatter series title.</param>
        /// <param name="xValue">The x value for the series.</param>
        /// <param name="yValue">The y value for the series.</param>
        /// <param name="color">The color for the series.</param>
        /// <param name="pointSize">The size of the points in pixels.</param>
        /// <returns>The newly created scatter series.</returns>
        public ScatterSeries CreateScatterSeries(string title, double xValue, double yValue, Color color, double pointSize = 15d)
        {
            MinX = Math.Min(MinX, xValue);
            MinY = Math.Min(MinY, yValue);
            MaxX = Math.Max(MaxX, xValue);
            MaxY = Math.Max(MaxY, yValue);
            ScatterSeries scatterSeries = new ScatterSeries
            {
                Title = title,
                Values = new ChartValues<ObservablePoint>
                {
                    new ObservablePoint(xValue, yValue)
                },
                DataLabels = false,
                LabelPoint = PointLabel,
                MaxPointShapeDiameter = pointSize,
                MinPointShapeDiameter = pointSize,
                Fill = new SolidColorBrush(color)
            };

            return scatterSeries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseScatterChartViewModel"/> class.
        /// </summary>
        public BaseScatterChartViewModel()
        {
            Initialise();
        }
    }
}
