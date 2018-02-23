// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooksReadByCountryViewModel.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The books read by country diagram view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksHelixCharts.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    using BooksCore.Interfaces;
    using BooksUtilities.Colors;
    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksHelixCharts.Utilities;

    public class BooksReadByCountryViewModel : BaseDiagramViewModel
    {
        private Model3D _booksReadByCountryModel;

        public Model3D BooksReadByCountryModel
        {
            get
            {
                return _booksReadByCountryModel;
            }

            set
            {
                _booksReadByCountryModel = value;
                OnPropertyChanged(() => BooksReadByCountryModel);
            }
        }

        public override void SetupModel()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            // get the range of colours for the for the countries
            int range = BooksReadProvider.AuthorCountries.Count > 0 ?
                BooksReadProvider.AuthorCountries.Select(s => s.TotalBooksReadFromCountry).Max() : 5;
            List<Color> colors;
            ColorUtilities.SetupFaintPaletteForRange(range, out colors, 128);
            List<Color> stdColors = ColorUtilities.SetupStandardColourSet();

            int geographyIndex = 0;

            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries.OrderByDescending(x => x.TotalBooksReadFromCountry))
            {
                string name = authorCountry.Country;
                WorldCountry country = GeographyProvider.WorldCountries.FirstOrDefault(w => w.Country == name);
                if (country != null)
                {
                    DiagramUtilities.AddCountryBooksEllipsoid(modelGroup, colors, authorCountry, name, country);
                }

                if (GeographyProvider.CountryGeographies != null && GeographyProvider.CountryGeographies.Count > 0)
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

        private TubeVisual3D GetPathForMeanReadingLocation(double maxHeight)
        {
            int totalDeltas = BooksReadProvider.BookLocationDeltas.Count;
            double increment = maxHeight / (0.5 * (1 + totalDeltas));

            List<Point3D> averagePosition = new List<Point3D>();
            int counter = 0;
            foreach (BookLocationDelta delta in BooksReadProvider.BookLocationDeltas)
            {
                PolygonPoint latLong = new PolygonPoint { Latitude = delta.AverageLatitude, Longitude = delta.AverageLongitude };
                double x, y;
                latLong.GetCoordinates(out x, out y);

                double height = maxHeight - (counter * increment);

                averagePosition.Add(new Point3D(x, y, height));
                counter++;
            }

            TubeVisual3D path = new TubeVisual3D
            {
                Path = new Point3DCollection(averagePosition),
                Diameter = 0.5,
                ThetaDiv = 20,
                IsPathClosed = false,
                Fill = Brushes.Green
            };

            return path;
        }

        public BooksReadByCountryViewModel(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
            : base(geographyProvider, booksReadProvider)
        {
        }
    }
}
