// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseLineChartViewModel.cs" company="N/A">
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
    using LiveCharts.Defaults;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Collections;
    using System.Text;

    /// <summary>
    /// The base geo map chart view model class.
    /// </summary>
    public class BaseGeoMapChartViewModel : BaseChartViewModel
    {
        private string _mapPath;

        public string MapPath
        {
            get { return _mapPath; }
            protected set { _mapPath = value; OnPropertyChanged(() => MapPath); }
        }

        public Dictionary<string, double> Values { get; set; }

        /// <summary>
        /// Initialises geo map chart title, series etc.
        /// </summary>
        private void Initialise()
        {
            LegendLocation = LegendLocation.None;
            SetupWorldMapFile();
            SetupSeries();
        }

        public static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }

        protected void SetupWorldMapFile()
        {
            Stream worldStream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.World ?? ""));
            string tempFilePath = GetTempFilePathWithExtension(".xml");

            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                worldStream.CopyTo(fileStream);
            }

            _mapPath = tempFilePath;
        }

        /// <summary>
        /// Sets up the geo map chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            List<Color> colors = ColorUtilities.SetupStandardColourSet();

            Values = new Dictionary<string, double>();

            Values["MX"] = r.Next(0, 100);
            Values["CA"] = r.Next(0, 100);
            Values["US"] = r.Next(0, 100);
            Values["IN"] = r.Next(0, 100);
            Values["CN"] = r.Next(0, 100);
            Values["JP"] = r.Next(0, 100);
            Values["BR"] = r.Next(0, 100);
            Values["DE"] = r.Next(0, 100);
            Values["FR"] = r.Next(0, 100);
            Values["GB"] = r.Next(0, 100);
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
