// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseGeoMapChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base line chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.GeoMapCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksUtilities.Colors;
    using LiveCharts;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The base geo map chart view model class.
    /// </summary>
    public class BaseGeoMapChartViewModel : BaseChartViewModel
    {
        /// <summary>
        /// The minimum map block value.
        /// </summary>
        private double _minValue;

        /// <summary>
        /// The maximum map block value.
        /// </summary>
        private double _maxValue;

        /// <summary>
        /// The full path to the map file.
        /// </summary>
        private string _mapPath;

        /// <summary>
        /// The set of colors and value elements that make up the colour gradient.
        /// </summary>
        private GradientStopCollection _colorGradientElements;

        /// <summary>
        /// Gets the full path to the map file.
        /// </summary>
        public string MapPath
        {
            get
            {
                return _mapPath;
            }

            protected set
            {
                _mapPath = value;
                OnPropertyChanged(() => MapPath);
            }
        }

        /// <summary>
        /// Gets the set of colors and value elements that make up the colour gradient.
        /// </summary>
        public GradientStopCollection ColorGradientElements
        {
            get
            {
                return _colorGradientElements;
            }

            protected set
            {
                _colorGradientElements = value;
                OnPropertyChanged(() => ColorGradientElements);
            }
        }

        /// <summary>
        /// Gets the values for each of the county codes.
        /// </summary>
        public Dictionary<string, double> Values { get; protected set; }

        /// <summary>
        /// Gets the largest value of the county codes.
        /// </summary>
        public double MaxValue
        {
            get
            {
                return _maxValue;
            }

            protected set
            {
                _maxValue = value;
                OnPropertyChanged(() => MaxValue);
            }
        }

        /// <summary>
        /// Gets the smallest value of the county codes.
        /// </summary>
        public double MinValue
        {
            get
            {
                return _minValue;
            }

            protected set
            {
                _minValue = value;
                OnPropertyChanged(() => MinValue);
            }
        }

        /// <summary>
        /// Initialises geo map chart title, series etc.
        /// </summary>
        private void Initialise()
        {
            LegendLocation = LegendLocation.None;
            SetupWorldMapFile();
            SetupColorGradient();
            SetupSeries();
        }

        /// <summary>
        /// Gets the full path for a temporary file with a given extension.
        /// </summary>
        public static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Sets up the temporary file for the world map to be bound to.
        /// </summary>
        protected void SetupWorldMapFile()
        {
            Stream worldStream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.World ?? string.Empty));
            string tempFilePath = GetTempFilePathWithExtension(".xml");

            using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                worldStream.CopyTo(fileStream);
            }

            _mapPath = tempFilePath;
        }

        /// <summary>
        /// Sets up the geo map color gradient.
        /// </summary>
        protected void SetupColorGradient()
        {
            List<Color> colors = ColorUtilities.Jet(100);

            ColorGradientElements = new GradientStopCollection();
            for (int i = 0; i < colors.Count - 1; i++)
            {
                ColorGradientElements.Add(new GradientStop(colors[i], (double)i / (double)colors.Count));
            }

            ColorGradientElements.Add(new GradientStop(colors.Last(), 1.0));
        }

        /// <summary>
        /// Sets up the geo map chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Random r = new Random((int)DateTime.Now.Ticks);

            Values = new Dictionary<string, double>
            {
                ["MX"] = r.Next(0, 100),
                ["CA"] = r.Next(0, 100),
                ["US"] = r.Next(0, 100),
                ["IN"] = r.Next(0, 100),
                ["CN"] = r.Next(0, 100),
                ["JP"] = r.Next(0, 100),
                ["BR"] = r.Next(0, 100),
                ["DE"] = r.Next(0, 100),
                ["FR"] = r.Next(0, 100),
                ["GB"] = r.Next(0, 100)
            };

            MinValue = Values.Values.Min();
            MaxValue = Values.Values.Max();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGeoMapChartViewModel"/> class.
        /// </summary>
        public BaseGeoMapChartViewModel()
        {
            Initialise();
        }
    }
}
