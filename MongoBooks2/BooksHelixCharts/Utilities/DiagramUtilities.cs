// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBooksReadProvider.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The Books Read provider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksHelixCharts.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using BooksCore.Books;
    using HelixToolkit.Wpf;
    using BooksCore.Geography;
    using BooksUtilities.Geography;
    using BooksUtilities.Logging;

    public class DiagramUtilities
    {
        public static GeometryModel3D GetGeographyPlaneGeometry(CountryGeography geography, Color color, double opacity = 0.25)
        {
            int i = 0;
            IOrderedEnumerable<PolygonBoundary> landBlocks = geography.LandBlocks.OrderByDescending(b => b.TotalArea);

            GeometryModel3D countryGeometry = new GeometryModel3D();

            Color mapColor = new Color { A = color.A, R = color.R, G = color.G, B = color.B };
            Material material = MaterialHelper.CreateMaterial(mapColor, opacity);
            countryGeometry.Material = material;
            countryGeometry.BackMaterial = material;

            MeshBuilder geographyBuilder = new MeshBuilder(false, false);

            foreach (PolygonBoundary boundary in landBlocks)
            {
                List<List<Point3D>> simpleAreaSet = SimplifyBoundary(boundary);

                foreach (var simpleArea in simpleAreaSet)
                {
                    List<Point3D> areaPts = new List<Point3D>();

                    foreach (Point3D vertex in simpleArea)
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

        public static List<List<Point3D>> SimplifyBoundary(PolygonBoundary boundary)
        {
            List<PolygonPoint> points = boundary.Points;

            List<Point3D> xyPoints = new List<Point3D>();
            foreach (PolygonPoint point in points)
            {
                double ptX;
                double ptY;
                point.GetCoordinates(out ptX, out ptY);
                Point3D dataPoint = new Point3D(ptX, ptY, 0);
                xyPoints.Add(dataPoint);
            }

            // need to turn these into a set of triangles
            List<List<Point3D>> simpleAreaSet =
                PolygonSimplifier.TriangulatePolygon(xyPoints);
            return simpleAreaSet;
        }

        public static void AddCountryBooksEllipsoid(
            Model3DGroup modelGroup,
            List<Color> colors,
            AuthorCountry authorCountry,
            string name,
            WorldCountry country)
        {
            int booksCount = authorCountry.TotalBooksReadFromCountry;

            GeometryModel3D countryGeometry =
                GetCountryEllipsoidArrowGeometry(country.Latitude, country.Longitude, booksCount, colors[booksCount]);
            modelGroup.Children.Add(countryGeometry);

            string label =
                $"{name}\nLat/Long ( {country.Latitude:0.###} ,{country.Longitude:0.###} ) \nTotal Books {booksCount}";

            TextVisual3D countryText =
                GetCountryText(country.Latitude, country.Longitude, booksCount, label);

            modelGroup.Children.Add(countryText.Content);
        }

        public static TextVisual3D GetCountryText(
            double latitude,
            double longitude,
            int booksCount,
            string label)
        {
            PolygonPoint latLong = new PolygonPoint { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out x, out y);

            double height = 1 + Math.Log(booksCount);

            Point3D labelPoint = new Point3D(x, y + 1, height + 1);

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

        public static TextVisual3D GetCountryText(
            double latitude,
            double longitude,
            int booksCount,
            string label,
            double height)
        {
            PolygonPoint latLong = new PolygonPoint { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out x, out y);

            Point3D labelPoint = new Point3D(x, y + 1, height + 1);

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

        private static GeometryModel3D GetCountryEllipsoidArrowGeometry(
            double latitude,
            double longitude,
            int booksCount,
            Color color)
        {
            PolygonPoint latLong = new PolygonPoint { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out x, out y);
            double height = 1 + Math.Log(booksCount);

            GeometryModel3D countryGeometry = new GeometryModel3D();
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

            countryGeometry.Material = MaterialHelper.CreateMaterial(brush, ambient: 177);
            MeshBuilder meshBuilder = new MeshBuilder(false, false);

            Point3D countryPoint = new Point3D(x, y, 0);

            meshBuilder.AddEllipsoid(countryPoint, 1.0, 1.0, height);

            Point3D top = new Point3D(x, y, height);
            Point3D textStart = new Point3D(x, y + 1, height + 1);

            meshBuilder.AddArrow(textStart, top, 0.1);

            countryGeometry.Geometry = meshBuilder.ToMesh();
            return countryGeometry;
        }


        public static GeometryModel3D GetCountryHelixArrowGeometry(
            double latitude, double longitude, double height, Color color)
        {
            PolygonPoint latLong = new PolygonPoint { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out x, out y);

            GeometryModel3D countryGeometry = new GeometryModel3D();
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

            countryGeometry.Material = MaterialHelper.CreateMaterial(brush, ambient: 177);
            MeshBuilder meshBuilder = new MeshBuilder(false, false);

            Point3D top = new Point3D(x, y, height);
            Point3D countryPoint = new Point3D(x, y, 0);

            meshBuilder.AddCone(top, countryPoint, 1.0, true, 16);

            Point3D textStart = new Point3D(x, y + 1, height + 1);

            meshBuilder.AddArrow(textStart, top, 0.1);

            countryGeometry.Geometry = meshBuilder.ToMesh();
            return countryGeometry;
        }

        public static void AddNationPagesPins(
            Model3DGroup modelGroup, 
            List<Color> colors, 
            AuthorCountry authorCountry, 
            string name, 
            Nation country,
            Dictionary<string, uint> countryToLogPagesLookUp)
        {
            int pagesCount = (int)authorCountry.TotalPagesReadFromCountry;
            int booksCount = authorCountry.TotalBooksReadFromCountry;

            int pagesLookup = (int)countryToLogPagesLookUp[authorCountry.Country];
            uint maxPages = countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            double height = (12.0 * pagesLookup) / maxPages;
            if (height < 1.0) height = 1.0;

            GeometryModel3D countryGeometry =
                GetCountryOctahedronGeometry(country.Latitude, country.Longitude,
                    height, colors[pagesLookup], booksCount, country.DisplayImage);
            modelGroup.Children.Add(countryGeometry);

            string label = $"{name}\nLat/Long ( {country.Latitude:0.###} ,{country.Longitude:0.###} ) \nTotal Pages {pagesCount}";

            TextVisual3D countryText =
                GetNationText(country, pagesCount, label, height);

            modelGroup.Children.Add(countryText.Content);
        }

        public static TextVisual3D GetNationText(Nation country, int count, string label, double height)
        {
            double latitude = country.Latitude;
            double longitude = country.Longitude;
            PolygonPoint latLong = new PolygonPoint { Latitude = latitude, Longitude = longitude };
            double x, y;
            latLong.GetCoordinates(out x, out y);

            Point3D labelPoint = new Point3D(x, y + 1, height + 1);

            Brush background = Brushes.LightYellow;
            if (!string.IsNullOrEmpty(country.ImageUri))
            {
                try
                {
                    System.Windows.Media.Imaging.BitmapImage im =
                        new System.Windows.Media.Imaging.BitmapImage(country.DisplayImage);

                    background = new ImageBrush(im);
                    background.Opacity = 0.5;
                }
                catch (Exception e)
                {
                    Logger.Log.Debug(e);
                }
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


        public static GeometryModel3D GetCountryOctahedronGeometry(
            double latitude, double longitude, double height, Color color, int books, Uri image = null)
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
    }
}
