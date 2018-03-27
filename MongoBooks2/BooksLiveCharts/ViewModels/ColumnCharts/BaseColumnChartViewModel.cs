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
    using System.Windows.Media;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Defaults;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;

    /// <summary>
    /// The base scatter chart view model class.
    /// </summary>
    public class BaseColumnChartViewModel : BaseChartViewModel
    {
        private static readonly List<KeyValuePair<double, double>> BritainCoastalPoints = 
            new List<KeyValuePair<double, double>>
            {
                new KeyValuePair<double, double>(1970,1034),
                new KeyValuePair<double, double>(1958,1056),
                new KeyValuePair<double, double>(1970,1054),
                new KeyValuePair<double, double>(1982,1054),
                new KeyValuePair<double, double>(1979,1070),
                new KeyValuePair<double, double>(1969,1089),
                new KeyValuePair<double, double>(1981,1090),
                new KeyValuePair<double, double>(1981,1092),
                new KeyValuePair<double, double>(1991,1115),
                new KeyValuePair<double, double>(1999,1118),
                new KeyValuePair<double, double>(2006,1140),
                new KeyValuePair<double, double>(2009,1147),
                new KeyValuePair<double, double>(2022,1150),
                new KeyValuePair<double, double>(2021,1162),
                new KeyValuePair<double, double>(2015,1167),
                new KeyValuePair<double, double>(2020,1177),
                new KeyValuePair<double, double>(2010,1186),
                new KeyValuePair<double, double>(1995,1186),
                new KeyValuePair<double, double>(1976,1191),
                new KeyValuePair<double, double>(1971,1187),
                new KeyValuePair<double, double>(1963,1195),
                new KeyValuePair<double, double>(1953,1193),
                new KeyValuePair<double, double>(1945,1200),
                new KeyValuePair<double, double>(1939,1196),
                new KeyValuePair<double, double>(1956,1178),
                new KeyValuePair<double, double>(1966,1174),
                new KeyValuePair<double, double>(1966,1174),
                new KeyValuePair<double, double>(1948,1171),
                new KeyValuePair<double, double>(1945,1164),
                new KeyValuePair<double, double>(1957,1158),
                new KeyValuePair<double, double>(1951,1149),
                new KeyValuePair<double, double>(1953,1136),
                new KeyValuePair<double, double>(1969,1138),
                new KeyValuePair<double, double>(1969,1138),
                new KeyValuePair<double, double>(1971,1127),
                new KeyValuePair<double, double>(1964,1115),
                new KeyValuePair<double, double>(1963,1115),
                new KeyValuePair<double, double>(1950,1112),
                new KeyValuePair<double, double>(1947,1106),
                new KeyValuePair<double, double>(1951,1098),
                new KeyValuePair<double, double>(1948,1092),
                new KeyValuePair<double, double>(1942,1102),
                new KeyValuePair<double, double>(1941,1083),
                new KeyValuePair<double, double>(1935,1072),
                new KeyValuePair<double, double>(1939,1051),
                new KeyValuePair<double, double>(1948,1034),
                new KeyValuePair<double, double>(1957,1035),
                new KeyValuePair<double, double>(1970,1034)
            };

        private static readonly List<KeyValuePair<double, double>> WalesCoastalPoints =
            new List<KeyValuePair<double, double>>
            {
                new KeyValuePair<double, double>(1969, 1138),
                new KeyValuePair<double, double>(1966, 1174),
                new KeyValuePair<double, double>(1948, 1171),
                new KeyValuePair<double, double>(1945, 1164),
                new KeyValuePair<double, double>(1957, 1158),
                new KeyValuePair<double, double>(1951, 1149),
                new KeyValuePair<double, double>(1953, 1136),
                new KeyValuePair<double, double>(1969, 1138)
            };

        private static readonly List<KeyValuePair<double, double>> ScotlandCoastalPoints =
            new List<KeyValuePair<double, double>>
            {
                new KeyValuePair<double, double>(1964,1115),
                new KeyValuePair<double, double>(1963,1115),
                new KeyValuePair<double, double>(1950,1112),
                new KeyValuePair<double, double>(1947,1106),
                new KeyValuePair<double, double>(1951,1098),
                new KeyValuePair<double, double>(1948,1092),
                new KeyValuePair<double, double>(1942,1102),
                new KeyValuePair<double, double>(1941,1083),
                new KeyValuePair<double, double>(1935,1072),
                new KeyValuePair<double, double>(1939,1051),
                new KeyValuePair<double, double>(1948,1034),
                new KeyValuePair<double, double>(1957,1035),
                new KeyValuePair<double, double>(1970,1034),
                new KeyValuePair<double, double>(1970,1034),
                new KeyValuePair<double, double>(1958,1056),
                new KeyValuePair<double, double>(1970,1054),
                new KeyValuePair<double, double>(1982,1054),
                new KeyValuePair<double, double>(1979,1070),
                new KeyValuePair<double, double>(1969,1089),
                new KeyValuePair<double, double>(1981,1090),
                new KeyValuePair<double, double>(1981,1092),
                new KeyValuePair<double, double>(1964,1115)
            };


        private static readonly List<KeyValuePair<double, double>> EnglandCoastalPoints =
            new List<KeyValuePair<double, double>>
            {
                new KeyValuePair<double, double>(1964,1115),
                new KeyValuePair<double, double>(1981,1092),
                new KeyValuePair<double, double>(1991,1115),
                new KeyValuePair<double, double>(1999,1118),
                new KeyValuePair<double, double>(2006,1140),
                new KeyValuePair<double, double>(2009,1147),
                new KeyValuePair<double, double>(2022,1150),
                new KeyValuePair<double, double>(2021,1162),
                new KeyValuePair<double, double>(2015,1167),
                new KeyValuePair<double, double>(2020,1177),
                new KeyValuePair<double, double>(2010,1186),
                new KeyValuePair<double, double>(1995,1186),
                new KeyValuePair<double, double>(1976,1191),
                new KeyValuePair<double, double>(1971,1187),
                new KeyValuePair<double, double>(1963,1195),
                new KeyValuePair<double, double>(1953,1193),
                new KeyValuePair<double, double>(1945,1200),
                new KeyValuePair<double, double>(1939,1196),
                new KeyValuePair<double, double>(1956,1178),
                new KeyValuePair<double, double>(1966,1174),
                new KeyValuePair<double, double>(1969,1138),
                new KeyValuePair<double, double>(1969,1138),
                new KeyValuePair<double, double>(1971,1127),
                new KeyValuePair<double, double>(1964,1115),
            };


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
            PointLabel = chartPoint => $"({XAxisTitle} {chartPoint.X:G5} , {YAxisTitle} {chartPoint.Y:G5})";
            Formatter = value => value.ToString("G5");

            LegendLocation = LegendLocation.None;
            SetupSeries();
        }

        /// <summary>
        /// Sets up the scatter chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            List<Color> colors = ColorUtilities.SetupStandardColourSet();
            Series = new SeriesCollection();

            List<ISeriesView> seriesViews = new List<ISeriesView>();

            MinX = MaxX = BritainCoastalPoints[0].Key;
            MinY = MaxY = BritainCoastalPoints[0].Value * -1;

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            for (int i = 0; i < BritainCoastalPoints.Count; i++)
            {
                seriesViews.Add(CreateScatterSeries($"Point {1 + i}", BritainCoastalPoints[i].Key, BritainCoastalPoints[i].Value * -1, Colors.Black, 5));
                xValues.Add(BritainCoastalPoints[i].Key);
                yValues.Add(BritainCoastalPoints[i].Value * -1);
            }


            LineSeries lineSeries = new LineSeries
            {
                Title = "outline",
                DataLabels = false,
                LabelPoint = PointLabel,
                LineSmoothness = 0.2,
                PointGeometrySize = 0,
                Foreground = new SolidColorBrush(Colors.Gold),
                PointForeground = new SolidColorBrush(Colors.Gold),
                Stroke = new SolidColorBrush(Colors.Gold)
            };

            ChartValues<ObservablePoint> values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
            {
                values.Add(new ObservablePoint(xValues[i], yValues[i]));
            }
            lineSeries.Values = values;
            seriesViews.Add(lineSeries);


            xValues = new List<double>();
            yValues = new List<double>();
            foreach (KeyValuePair<double, double> t in WalesCoastalPoints)
            {
                xValues.Add(t.Key);
                yValues.Add(t.Value * -1);
            }

            LineSeries walesLineSeries = new LineSeries
            {
                Title = "walesOutline",
                DataLabels = false,
                LabelPoint = PointLabel,
                LineSmoothness = 0.2,
                PointGeometrySize = 0,
                Foreground = new SolidColorBrush(Colors.Green),
                PointForeground = new SolidColorBrush(Colors.Green),
                Stroke = new SolidColorBrush(Colors.Green)
            };

            values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
            {
                values.Add(new ObservablePoint(xValues[i], yValues[i]));
            }
            walesLineSeries.Values = values;
            seriesViews.Add(walesLineSeries);


            xValues = new List<double>();
            yValues = new List<double>();
            foreach (KeyValuePair<double, double> t in ScotlandCoastalPoints)
            {
                xValues.Add(t.Key);
                yValues.Add(t.Value * -1);
            }

            LineSeries scotlandLineSeries = new LineSeries
            {
                Title = "scotlandOutline",
                DataLabels = false,
                LabelPoint = PointLabel,
                LineSmoothness = 0.2,
                PointGeometrySize = 0,
                Foreground = new SolidColorBrush(Colors.Navy),
                PointForeground = new SolidColorBrush(Colors.Navy),
                Stroke = new SolidColorBrush(Colors.Navy)
            };

            values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
            {
                values.Add(new ObservablePoint(xValues[i], yValues[i]));
            }
            scotlandLineSeries.Values = values;
            seriesViews.Add(scotlandLineSeries);







            xValues = new List<double>();
            yValues = new List<double>();
            foreach (KeyValuePair<double, double> t in EnglandCoastalPoints)
            {
                xValues.Add(t.Key);
                yValues.Add(t.Value * -1);
            }

            LineSeries englandLineSeries = new LineSeries
            {
                Title = "englandOutline",
                DataLabels = false,
                LabelPoint = PointLabel,
                LineSmoothness = 0.2,
                PointGeometrySize = 0,
                Foreground = new SolidColorBrush(Colors.Red),
                PointForeground = new SolidColorBrush(Colors.Red),
                Stroke = new SolidColorBrush(Colors.Red)
            };

            values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
            {
                values.Add(new ObservablePoint(xValues[i], yValues[i]));
            }
            englandLineSeries.Values = values;
            seriesViews.Add(englandLineSeries);



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
        /// Initializes a new instance of the <see cref="BaseColumnChartViewModel"/> class.
        /// </summary>
        public BaseColumnChartViewModel()
        {
            Initialise();
        }
    }
}
