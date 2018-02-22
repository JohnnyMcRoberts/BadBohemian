namespace BooksHelixCharts.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using BooksUtilities.ViewModels;
    using BooksCore.Interfaces;
    using BooksUtilities.Colors;
    using BooksCore.Books;
    using BooksCore.Geography;

    public class PagesByCountryDiagramViewModel : BaseViewModel
    {
        private IGeographyProvider _geographyProvider;

        private IBooksReadProvider _booksReadProvider;

        public Model3D PagesByCountryModel { get; set; }

        public void SetupModel()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            // get the range of colours for the for the countries
            int range = _booksReadProvider.AuthorCountries.Count > 0 ?
                _booksReadProvider.AuthorCountries.Select(s => s.TotalBooksReadFromCountry).Max() : 5;
            List<Color> colors;
            ColorUtilities.SetupFaintPaletteForRange(range, out colors, 128);

            List<Color> stdColors = ColorUtilities.SetupStandardColourSet();
            int geographyIndex = 0;

            foreach (var authorCountry in _booksReadProvider.AuthorCountries.OrderByDescending(x => x.TotalBooksReadFromCountry))
            {
                var name = authorCountry.Country;
                var country = _geographyProvider.WorldCountries.Where(w => w.Country == name).FirstOrDefault();
                if (country != null)
                {
                    AddCountryBooksEllipsoid(modelGroup, colors, authorCountry, name, country);
                }

                if (_geographyProvider.CountryGeographies != null && _geographyProvider.CountryGeographies.Count > 0)
                {
                    geographyIndex = AddCountryGeographyPlane(modelGroup, stdColors, geographyIndex, name);
                }
            }

            AddGeographiesForCountriesWithoutBooksRead(modelGroup);

            double maxHeight = Math.Log(range);
            TubeVisual3D path = GetPathForMeanReadingLocation(maxHeight);

            modelGroup.Children.Add(path.Content);

            PagesByCountryModel = modelGroup;
        }

        private TubeVisual3D GetPathForMeanReadingLocation(double maxHeight)
        {
            throw new NotImplementedException();
        }

        private void AddGeographiesForCountriesWithoutBooksRead(Model3DGroup modelGroup)
        {
            throw new NotImplementedException();
        }

        private int AddCountryGeographyPlane(Model3DGroup modelGroup, List<Color> stdColors, int geographyIndex, string name)
        {
            throw new NotImplementedException();
        }

        private void AddCountryBooksEllipsoid(Model3DGroup modelGroup, List<Color> colors, AuthorCountry authorCountry, string name, WorldCountry country)
        {
            throw new NotImplementedException();
        }

        public PagesByCountryDiagramViewModel()
        {

        }

    }
}
