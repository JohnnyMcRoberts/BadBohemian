using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Linq.Expressions;

using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

using OxyPlot;

using MongoDbBooks.Models;
using MongoDbBooks.Models.Geography;
using MongoDbBooks.ViewModels.Utilities;
using MongoDbBooks.Models.Database;

namespace MongoDbBooks.ViewModels
{
    public class DiagramsViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException("sExpression");

            MemberExpression body = sExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        #region Private Data

        private MainWindow _mainWindow;
        private log4net.ILog _log;
        private MainBooksModel _mainModel;
        private MainViewModel _parent;

        #endregion

        #region Public Data

        public Model3D BooksReadByCountryModel { get; set; }

        public Model3D PagesReadByCountryModel { get; set; }

        public Model3D PagesByCountryModel { get; set; }

        #endregion

        #region Constructors

        public DiagramsViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;

            SetupBooksReadByCountryModel();
            SetupPagesReadByCountryModel();
            SetupPagesByCountryModel();
        }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            SetupBooksReadByCountryModel();
            SetupPagesReadByCountryModel();
            SetupPagesByCountryModel();
            OnPropertyChanged("");
        }

        #endregion

        #region Utility Function

        private void SetupBooksReadByCountryModel()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            // get the range of colours for the for the countries
            int range = _mainModel.AuthorCountries.Count > 0 ?
                _mainModel.AuthorCountries.Select(s => s.TotalBooksReadFromCountry).Max() : 5;
            OxyPalette faintPalette;
            List<OxyColor> colors;
            OxyPlotUtilities.SetupFaintPaletteForRange( range, out colors, out faintPalette, 128);

            List<OxyColor> stdColors = OxyPlotUtilities.SetupStandardColourSet();
            int geographyIndex = 0;

            foreach (var authorCountry in _mainModel.AuthorCountries.OrderByDescending(x => x.TotalBooksReadFromCountry))
            {
                var name = authorCountry.Country;
                var country = _mainModel.WorldCountries.Where(w => w.Country == name).FirstOrDefault();
                if (country != null)
                {
                    AddCountryBooksEllipsoid(modelGroup, colors, authorCountry, name, country);
                }

                if (_mainModel.CountryGeographies != null && _mainModel.CountryGeographies.Count > 0)
                {
                    geographyIndex = AddCountryGeographyPlane(modelGroup, stdColors, geographyIndex, name);
                }
            }

            AddGeographiesForCountriesWithoutBooksRead(modelGroup);

            double maxHeight = Math.Log(range);
            TubeVisual3D path = GetPathForMeanReadingLocation(maxHeight);

            modelGroup.Children.Add(path.Content);

            BooksReadByCountryModel = modelGroup;
        }

        private int AddCountryGeographyPlane(Model3DGroup modelGroup, List<OxyColor> stdColors, 
            int geographyIndex, string name)
        {
            var geography = _mainModel.CountryGeographies.Where(g => g.Name == name).FirstOrDefault();
            if (geography != null)
            {
                var colour = stdColors[(geographyIndex % stdColors.Count)];
                GeometryModel3D geographyGeometry =
                    GetGeographyPlaneGeometry(geography, colour, 0.4);
                modelGroup.Children.Add(geographyGeometry);
                geographyIndex++;
            }
            return geographyIndex;
        }

        private void AddCountryBooksEllipsoid(Model3DGroup modelGroup, List<OxyColor> colors, 
            AuthorCountry authorCountry, string name, WorldCountry country)
        {
            var booksCount = authorCountry.TotalBooksReadFromCountry;

            GeometryModel3D countryGeometry =
                GetCountryEllipsoidArrowGeometry(country.Latitude, country.Longitude, booksCount, colors[booksCount]);
            modelGroup.Children.Add(countryGeometry);

            string label =
                string.Format("{0}\nLat/Long ( {1:0.###} ,{2:0.###} ) \nTotal Books {3}",
                    name, country.Latitude, country.Longitude, booksCount);

            TextVisual3D countryText =
                GetCountryText(country.Latitude, country.Longitude, booksCount, label);

            modelGroup.Children.Add(countryText.Content);
        }

        private GeometryModel3D GetGeographyPlaneGeometry(CountryGeography geography, OxyColor color, double opacity=0.25)
        {
            int i = 0;
            var landBlocks = geography.LandBlocks.OrderByDescending(b => b.TotalArea);

            GeometryModel3D countryGeometry = new GeometryModel3D();

            var mapColor = new Color() { A= color.A, R=color.R, G=color.G, B=color.B };
            var material = MaterialHelper.CreateMaterial(mapColor, opacity);
            countryGeometry.Material = material;
            countryGeometry.BackMaterial = material;

            var geographyBuilder = new MeshBuilder(false, false);

            foreach (var boundary in landBlocks)
            {
                List<List<DataPoint>> simpleAreaSet = SimplifyBoundary(boundary);

                foreach (var simpleArea in simpleAreaSet)
                {
                    var areaPts = new List<Point3D>() { };

                    foreach(var vertex in simpleArea)
                        areaPts.Add(new Point3D(vertex.X, vertex.Y, 0));

                    Point3DCollection area = new Point3DCollection(areaPts);
                    Polygon3D poly3D = new Polygon3D(area);
                    geographyBuilder.AddPolygon(area);
                }

                // just do the 10 biggest bits per country (looks to be enough)
                i++;
                if (i > 10)
                    break;
            }
            countryGeometry.Geometry = geographyBuilder.ToMesh();
            return countryGeometry;
        }

        private static List<List<DataPoint>> SimplifyBoundary(PolygonBoundary boundary)
        {
            var points = boundary.Points;

            List<DataPoint> xyPoints = new List<DataPoint>();
            foreach (var point in points)
            {
                double ptX = 0;
                double ptY = 0;
                point.GetCoordinates(out ptX, out ptY);
                DataPoint dataPoint = new DataPoint(ptX, ptY);
                xyPoints.Add(dataPoint);
            }

            // need to turn these into a set of triangles
            List<List<DataPoint>> simpleAreaSet =
                PolygonSimplifier.TriangulatePolygon(xyPoints);
            return simpleAreaSet;
        }

        private TubeVisual3D GetPathForMeanReadingLocation(double maxHeight)
        {
            int totalDeltas = _mainModel.BookLocationDeltas.Count;
            double increment = maxHeight / (0.5 * (1 + totalDeltas));

            var averagePosition = new List<Point3D>();
            int counter = 0;
            foreach (var delta in _mainModel.BookLocationDeltas)
            {
                PolygonPoint latLong =
                    new PolygonPoint() { Latitude = delta.AverageLatitude, Longitude = delta.AverageLongitude };
                double x, y;
                latLong.GetCoordinates(out x, out y);

                double height = maxHeight - (counter * increment);

                averagePosition.Add(new Point3D(x, y, height));
                counter++;
            }

            TubeVisual3D path = new TubeVisual3D()
            {
                Path = new Point3DCollection(averagePosition),
                Diameter = 0.5,
                ThetaDiv = 20,
                IsPathClosed = false,
                Fill = Brushes.Green
            };
            return path;
        }

        private TextVisual3D GetCountryText(
            double latitude, double longitude, int booksCount, string label)
        {
            PolygonPoint latLong = new PolygonPoint() { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out  x, out  y);

            double height = 1+Math.Log(booksCount);

            var labelPoint = new Point3D(x, y+1, height+1);

            TextVisual3D text3D = new TextVisual3D()
            {
                Foreground = Brushes.Black,
                Background = Brushes.LightYellow,
                BorderBrush = Brushes.DarkBlue,
                Height = 2,
                FontWeight = System.Windows.FontWeights.Normal,
                IsDoubleSided = true,
                Position = labelPoint,
                UpDirection = new Vector3D(0, 0, 1),
                TextDirection = new Vector3D(1, 0, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                Text = label
            };

            return text3D;


        }

        private GeometryModel3D GetCountryEllipsoidArrowGeometry(
            double latitude, double longitude, int booksCount, OxyColor color)
        {
            PolygonPoint latLong = new PolygonPoint() {Latitude = latitude, Longitude = longitude};
            double x,  y;
            latLong.GetCoordinates(out  x, out  y);
            double height = 1 + Math.Log(booksCount);

            GeometryModel3D countryGeometry = new GeometryModel3D();
            var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

            countryGeometry.Material = MaterialHelper.CreateMaterial(brush, ambient: 177);
            var meshBuilder = new MeshBuilder(false, false);

            var countryPoint = new Point3D(x, y, 0);

            meshBuilder.AddEllipsoid(countryPoint, 1.0, 1.0, height);

            var top = new Point3D(x, y, height);
            var textStart = new Point3D(x, y+1, height+1);

            meshBuilder.AddArrow(textStart, top, 0.1);

            countryGeometry.Geometry = meshBuilder.ToMesh();
            return countryGeometry;
        }

        private Dictionary<string, uint> _countryToLogPagesLookUp;

        private void SetupPagesReadByCountryModel()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            // get the range of colours for the for the countries

            // set up lookups of the countries with numbers read
            int maxBooksPages;
            int maxBooksLogPages;
            Dictionary<string, long> countryToReadLookUp;
            Dictionary<string, uint> countryToPagesLookUp;
            Dictionary<string, uint> countryToLogPagesLookUp;
            SetupCountyPagesLookups(out maxBooksPages, out maxBooksLogPages,
                out countryToReadLookUp, out countryToPagesLookUp, out countryToLogPagesLookUp);
            _countryToLogPagesLookUp = countryToLogPagesLookUp;

            // set up a palette based on this
            List<OxyColor> colors;
            OxyPalette faintPalette;
            uint numColours = 1+ countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            OxyPlotUtilities.SetupFaintPaletteForRange((int)numColours, out colors, out faintPalette, 128);

            List<OxyColor> stdColors = OxyPlotUtilities.SetupStandardColourSet();
            int geographyIndex = 0;

            foreach (var authorCountry in _mainModel.AuthorCountries.OrderByDescending(x => x.TotalBooksReadFromCountry))
            {
                var name = authorCountry.Country;
                var country = _mainModel.WorldCountries.Where(w => w.Country == name).FirstOrDefault();
                if (country != null)
                {
                    AddCountryPagesSpiral(modelGroup, colors, authorCountry, name, country);
                }

                if (_mainModel.CountryGeographies != null && _mainModel.CountryGeographies.Count > 0)
                {
                    geographyIndex = AddCountryGeographyPlane(modelGroup, stdColors, geographyIndex, name);
                }
            }

            AddGeographiesForCountriesWithoutBooksRead(modelGroup);

            PagesReadByCountryModel = modelGroup;
        }

        private void AddGeographiesForCountriesWithoutBooksRead(Model3DGroup modelGroup)
        {

            if (_mainModel.CountryGeographies != null && _mainModel.CountryGeographies.Count > 0)
            {
                List<string> authorCountries = _mainModel.AuthorCountries.Select(x => x.Country).ToList();
                foreach (var geography in _mainModel.CountryGeographies)
                {

                    if (!authorCountries.Contains(geography.Name))
                    {
                        GeometryModel3D geographyGeometry =
                            GetGeographyPlaneGeometry(geography, OxyColors.LightGray);
                        modelGroup.Children.Add(geographyGeometry);
                    }
                }
            }
        }

        private void AddCountryPagesSpiral(Model3DGroup modelGroup, List<OxyColor> colors, AuthorCountry authorCountry, 
            string name, WorldCountry country)
        {
            int pagesCount = (int)authorCountry.TotalPagesReadFromCountry;

            var pagesLookup = (int)_countryToLogPagesLookUp[authorCountry.Country];
            var maxPages = _countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            double height = (12.0 * (double)pagesLookup) / ((double)maxPages);
            if (height < 1.0) height = 1.0;

            GeometryModel3D countryGeometry =
                GetCountryHelixArrowGeometry(country.Latitude, country.Longitude, height, colors[pagesLookup]);
            modelGroup.Children.Add(countryGeometry);

            string label =
                string.Format("{0}\nLat/Long ( {1:0.###} ,{2:0.###} ) \nTotal Pages {3}",
                    name, country.Latitude, country.Longitude, pagesCount);

            TextVisual3D countryText =
                GetCountryText(country.Latitude, country.Longitude, pagesCount, label, height);

            modelGroup.Children.Add(countryText.Content);
        }

        private GeometryModel3D GetCountryHelixArrowGeometry(
            double latitude, double longitude, double height, OxyColor color)
        {
            PolygonPoint latLong = new PolygonPoint() { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out  x, out  y);

            GeometryModel3D countryGeometry = new GeometryModel3D();
            var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

            countryGeometry.Material = MaterialHelper.CreateMaterial(brush, ambient: 177);
            var meshBuilder = new MeshBuilder(false, false);
            
            var top = new Point3D(x, y, height);
            var countryPoint = new Point3D(x, y, 0);

            meshBuilder.AddCone(top, countryPoint, 1.0, true, 16);

            var textStart = new Point3D(x, y + 1, height + 1);

            meshBuilder.AddArrow(textStart, top, 0.1);

            countryGeometry.Geometry = meshBuilder.ToMesh();
            return countryGeometry;
        }

        private void SetupCountyPagesLookups(out int maxBooksPages, out int maxBooksLogPages,
            out Dictionary<string, long> countryToReadLookUp, out Dictionary<string, uint> countryToPagesLookUp,
            out Dictionary<string, uint> countryToLogPagesLookUp)
        {
            maxBooksPages = 0;
            maxBooksLogPages = 0;
            countryToReadLookUp = new Dictionary<string, long>();
            countryToPagesLookUp = new Dictionary<string, uint>();
            countryToLogPagesLookUp = new Dictionary<string, uint>();
            foreach (var authorCountry in _mainModel.AuthorCountries)
            {
                int totalPagesInThousands = (int)((long)authorCountry.TotalPagesReadFromCountry / 1000);
                if (totalPagesInThousands < 1)
                    totalPagesInThousands = 1;

                double ttl =
                    (authorCountry.TotalPagesReadFromCountry > 1)
                    ? authorCountry.TotalPagesReadFromCountry : 10;
                var logPages = (uint)(10.0 * Math.Log10(ttl));

                maxBooksPages = Math.Max(totalPagesInThousands, maxBooksPages);
                maxBooksLogPages = Math.Max((int)logPages, maxBooksLogPages);
                countryToReadLookUp.Add(authorCountry.Country, totalPagesInThousands);
                countryToPagesLookUp.Add(authorCountry.Country, authorCountry.TotalPagesReadFromCountry);
                countryToLogPagesLookUp.Add(authorCountry.Country, logPages - 10);
            }
        }

        private TextVisual3D GetCountryText(
            double latitude, double longitude, int booksCount, string label, double height)
        {
            PolygonPoint latLong = new PolygonPoint() { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out  x, out  y);

            var labelPoint = new Point3D(x, y + 1, height + 1);

            TextVisual3D text3D = new TextVisual3D()
            {
                Foreground = Brushes.Black,
                Background = Brushes.LightYellow,
                BorderBrush = Brushes.DarkBlue,
                Height = 2,
                FontWeight = System.Windows.FontWeights.Normal,
                IsDoubleSided = true,
                Position = labelPoint,
                UpDirection = new Vector3D(0, 0.7, 1),
                TextDirection = new Vector3D(1, 0, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                Text = label
            };

            return text3D;


        }

        private void SetupPagesByCountryModel()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            // get the range of colours for the for the countries

            // set up lookups of the countries with numbers read
            int maxBooksPages;
            int maxBooksLogPages;
            Dictionary<string, long> countryToReadLookUp;
            Dictionary<string, uint> countryToPagesLookUp;
            Dictionary<string, uint> countryToLogPagesLookUp;
            SetupCountyPagesLookups(out maxBooksPages, out maxBooksLogPages,
                out countryToReadLookUp, out countryToPagesLookUp, out countryToLogPagesLookUp);
            _countryToLogPagesLookUp = countryToLogPagesLookUp;

            // set up a palette based on this
            List<OxyColor> colors;
            OxyPalette faintPalette;
            uint numColours = 1 + countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            OxyPlotUtilities.SetupFaintPaletteForRange((int)numColours, out colors, out faintPalette, 128);

            List<OxyColor> stdColors = OxyPlotUtilities.SetupStandardColourSet();
            int geographyIndex = 0;

            foreach (var authorCountry in _mainModel.AuthorCountries.OrderByDescending(x => x.TotalBooksReadFromCountry))
            {
                var name = authorCountry.Country;
                var country = _mainModel.Nations.Where(w => w.Name == name).FirstOrDefault();
                if (country != null)
                {
                    AddNationPagesPins(modelGroup, colors, authorCountry, name, country);
                }

                if (_mainModel.Nations != null && _mainModel.Nations.Count > 0)
                {
                    geographyIndex = AddNationsCountryGeographyPlane(modelGroup, stdColors, geographyIndex, name);
                }
            }

            AddGeographiesForNationsWithoutBooksRead(modelGroup);

            PagesByCountryModel = modelGroup;
        }

        private int AddNationsCountryGeographyPlane(Model3DGroup modelGroup, List<OxyColor> stdColors, int geographyIndex, string name)
        {
            var nation = _mainModel.Nations.Where(g => g.Name == name).FirstOrDefault();
            if (nation != null)
            {
                CountryGeography geography = nation.Geography;
                if (geography != null)
                {
                    var colour = stdColors[(geographyIndex % stdColors.Count)];
                    GeometryModel3D geographyGeometry =
                        GetGeographyPlaneGeometry(geography, colour, 0.4);
                    modelGroup.Children.Add(geographyGeometry);
                    geographyIndex++;
                }
            }
            
            return geographyIndex;
        }

        private void AddGeographiesForNationsWithoutBooksRead(Model3DGroup modelGroup)
        {
            if (_mainModel.Nations != null && _mainModel.Nations.Count > 0)
            {
                List<string> authorCountries = _mainModel.AuthorCountries.Select(x => x.Country).ToList();
                foreach (Nation nation in _mainModel.Nations)
                {
                    CountryGeography geography = nation.Geography;
                    if (geography != null && !authorCountries.Contains(geography.Name))
                    {
                        GeometryModel3D geographyGeometry =
                            GetGeographyPlaneGeometry(geography, OxyColors.LightGray);
                        modelGroup.Children.Add(geographyGeometry);
                    }
                }
            }
        }

        private void AddNationPagesPins(Model3DGroup modelGroup, List<OxyColor> colors, AuthorCountry authorCountry, string name, Nation country)
        {
            int pagesCount = (int)authorCountry.TotalPagesReadFromCountry;
            int booksCount = (int)authorCountry.TotalBooksReadFromCountry;

            var pagesLookup = (int)_countryToLogPagesLookUp[authorCountry.Country];
            var maxPages = _countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            double height = (12.0 * (double)pagesLookup) / ((double)maxPages);
            if (height < 1.0) height = 1.0;

            GeometryModel3D countryGeometry =
                GetCountryOctahedronGeometry(country.Latitude, country.Longitude,
                    height, colors[pagesLookup], booksCount, country.DisplayImage);
            modelGroup.Children.Add(countryGeometry);

            string label =
                string.Format("{0}\nLat/Long ( {1:0.###} ,{2:0.###} ) \nTotal Pages {3}",
                    name, country.Latitude, country.Longitude, pagesCount);

            TextVisual3D countryText =
                GetNationText(country, pagesCount, label, height);

            modelGroup.Children.Add(countryText.Content);
        }

        private TextVisual3D GetNationText(Nation country, int count, string label, double height)
        {
            double latitude = country.Latitude;
            double longitude = country.Longitude;
            PolygonPoint latLong = new PolygonPoint() { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out x, out y);

            var labelPoint = new Point3D(x, y + 1, height + 1);

            Brush background = Brushes.LightYellow;
            if (country.ImageURI != null)
            {
                System.Windows.Media.Imaging.BitmapImage im =
                    new System.Windows.Media.Imaging.BitmapImage(country.DisplayImage);

                background = new ImageBrush(im);
                background.Opacity = 0.5;
            }

            TextVisual3D text3D = new TextVisual3D()
            {
                Foreground = Brushes.DarkBlue,
                Background = background,
                BorderBrush = Brushes.PapayaWhip,
                Height = 2,
                FontWeight = System.Windows.FontWeights.Bold,
                IsDoubleSided = true,
                Position = labelPoint,
                UpDirection = new Vector3D(0, 0.7, 1),
                TextDirection = new Vector3D(1, 0, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                Text = label

            };

            return text3D;

        }

        private void AddCountryPagesPins(Model3DGroup modelGroup, List<OxyColor> colors, AuthorCountry authorCountry,
            string name, WorldCountry country)
        {
            int pagesCount = (int)authorCountry.TotalPagesReadFromCountry;
            int booksCount = (int)authorCountry.TotalBooksReadFromCountry;

            var pagesLookup = (int)_countryToLogPagesLookUp[authorCountry.Country];
            var maxPages = _countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            double height = (12.0 * (double)pagesLookup) / ((double)maxPages);
            if (height < 1.0) height = 1.0;

            GeometryModel3D countryGeometry =
                GetCountryOctahedronGeometry(country.Latitude, country.Longitude, 
                    height, colors[pagesLookup], booksCount);
            modelGroup.Children.Add(countryGeometry);

            string label =
                string.Format("{0}\nLat/Long ( {1:0.###} ,{2:0.###} ) \nTotal Pages {3}",
                    name, country.Latitude, country.Longitude, pagesCount);

            TextVisual3D countryText =
                GetCountryText(country.Latitude, country.Longitude, pagesCount, label, height);

            modelGroup.Children.Add(countryText.Content);
        }

        private GeometryModel3D GetCountryOctahedronGeometry(
            double latitude, double longitude, double height, OxyColor color, int books, Uri image = null)
        {
            PolygonPoint latLong = new PolygonPoint() { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out x, out y);

            GeometryModel3D countryGeometry = new GeometryModel3D();
            var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

            countryGeometry.Material = MaterialHelper.CreateMaterial(brush, ambient: 177);

            var meshBuilder = new MeshBuilder(false, false);

            var top = new Point3D(x, y, height);
            var countryPoint = new Point3D(x, y, 0);

            Vector3D normalVector = new Vector3D(1, 1, 0);
            Vector3D upVector = new Vector3D(0, 0, 1);

            meshBuilder.AddCone(top, countryPoint, 1.0, true, 16);

            double side = (1 + Math.Log(books)) / Math.Log(3);
            meshBuilder.AddOctahedron(countryPoint, normalVector, upVector, side, height);

            var textStart = new Point3D(x, y + 1, height + 1);

            meshBuilder.AddArrow(textStart, top, 0.1);

            countryGeometry.Geometry = meshBuilder.ToMesh();
            return countryGeometry;
        }

        #endregion

    }
}
