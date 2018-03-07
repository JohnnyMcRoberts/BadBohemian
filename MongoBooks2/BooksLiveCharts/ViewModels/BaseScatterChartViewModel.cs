// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePieChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base pie-chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels
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
        private double _minX;

        private double _maxX;

        private double _minY;

        private double _maxY;

        private string _xAxisString;

        private string _yAxisString;


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
        /// Sets up the pie chart series.
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
        /// <returns>The newly created scatter series.</returns>
        public ScatterSeries CreateScatterSeries(string title, double xValue, double yValue, Color color)
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
                MaxPointShapeDiameter = 15,
                MinPointShapeDiameter = 15,
                Fill = new SolidColorBrush(color)
            };

            return scatterSeries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePieChartViewModel"/> class.
        /// </summary>
        public BaseScatterChartViewModel()
        {
            XAxisTitle = "X";
            YAxisTitle = "Y";
            MinX = MinY = 0;
            MaxX = MaxY = 1;
            PointLabel = chartPoint => $"({XAxisTitle} {chartPoint.X:G} , {YAxisTitle} {chartPoint.Y:G})";

            LegendLocation = LegendLocation.None;
            SetupSeries();
        }
    }
}
