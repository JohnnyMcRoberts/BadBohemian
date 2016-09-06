﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

using OxyPlot;

using MongoDbBooks.Models;
using MongoDbBooks.Models.Geography;
using MongoDbBooks.ViewModels.Utilities;
using MongoDbBooks.ViewModels.PlotGenerators;

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
        }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            SetupBooksReadByCountryModel();
        }

        #endregion

        #region Utility Function

        private void SetupBooksReadByCountryModel()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            // get the range of colours for the for the countries
            int range = _mainModel.AuthorCountries.Select(s => s.TotalBooksReadFromCountry).Max();
            OxyPalette faintPalette;
            List<OxyColor> colors;
            OxyPlotUtilities.SetupFaintPaletteForRange( range, out colors, out faintPalette, 128);

            foreach (var authorCountry in _mainModel.AuthorCountries)
            {
                var name = authorCountry.Country;
                var country = _mainModel.WorldCountries.Where(w => w.Country == name).FirstOrDefault();
                if (country != null)
                {
                    var booksCount = authorCountry.TotalBooksReadFromCountry;

                    GeometryModel3D countryGeometry =
                        GetCountryGeometry(country.Latitude, country.Longitude, booksCount, colors[booksCount]);
                    modelGroup.Children.Add(countryGeometry);

                    string label = 
                        string.Format("{0}\nLat/Long ( {1:0.###} ,{2:0.###} ) \nTotal Books {3}", 
                            name, country.Latitude, country.Longitude, booksCount);

                    TextVisual3D countryText =
                        GetCountryText(country.Latitude, country.Longitude, booksCount, label);

                    modelGroup.Children.Add(countryText.Content);
                }

            }

            double maxHeight = Math.Log(range);
            TubeVisual3D path = GetPathForMeanReadingLocation(maxHeight);

            modelGroup.Children.Add(path.Content);

            BooksReadByCountryModel = modelGroup;
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

        private GeometryModel3D GetCountryGeometry(
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

        #endregion

    }
}