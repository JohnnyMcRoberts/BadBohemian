// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagesByCountryDiagramViewModel.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The pages read by country diagram view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksHelixCharts.ViewModels
{
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

    public class PagesByCountryDiagramViewModel : BaseDiagramViewModel
    {
        private Dictionary<string, uint> _countryToLogPagesLookUp;

        private Model3D _pagesByCountryModel;

        public Model3D PagesByCountryModel
        {
            get
            {
                return _pagesByCountryModel;
            }

            set
            {
                _pagesByCountryModel = value;
                OnPropertyChanged(() => PagesByCountryModel);
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

            // set up lookups of the countries with numbers read
            int maxBooksPages;
            int maxBooksLogPages;
            Dictionary<string, long> countryToReadLookUp;
            Dictionary<string, uint> countryToPagesLookUp;
            Dictionary<string, uint> countryToLogPagesLookUp;
            SetupCountyPagesLookups(out maxBooksPages, out maxBooksLogPages,
                out countryToReadLookUp, out countryToPagesLookUp, out countryToLogPagesLookUp);
            _countryToLogPagesLookUp = countryToLogPagesLookUp;
            

            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries.OrderByDescending(x => x.TotalBooksReadFromCountry))
            {
                string name = authorCountry.Country;
                WorldCountry country = GeographyProvider.WorldCountries.FirstOrDefault(w => w.Country == name);
                if (country != null)
                {
                    AddCountryPagesSpiral(modelGroup, colors, authorCountry, name, country);
                }

                if (GeographyProvider.CountryGeographies != null && GeographyProvider.CountryGeographies.Count > 0)
                {
                    geographyIndex = AddCountryGeographyPlane(modelGroup, stdColors, geographyIndex, name);
                }
            }

            AddGeographiesForCountriesWithoutBooksRead(modelGroup);

            PagesByCountryModel = modelGroup;
        }

        private void AddCountryPagesSpiral(Model3DGroup modelGroup, List<Color> colors, AuthorCountry authorCountry,
            string name, WorldCountry country)
        {
            int pagesCount = (int)authorCountry.TotalPagesReadFromCountry;

            int pagesLookup = (int)_countryToLogPagesLookUp[authorCountry.Country];
            uint maxPages = _countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            double height = (12.0 * pagesLookup) / maxPages;
            if (height < 1.0) height = 1.0;

            GeometryModel3D countryGeometry =
                DiagramUtilities.GetCountryHelixArrowGeometry(country.Latitude, country.Longitude, height, colors[pagesLookup]);
            modelGroup.Children.Add(countryGeometry);

            string label =
                $"{name}\nLat/Long ( {country.Latitude:0.###} ,{country.Longitude:0.###} ) \nTotal Pages {pagesCount}";

            TextVisual3D countryText =
                DiagramUtilities.GetCountryText(country.Latitude, country.Longitude, pagesCount, label, height);

            modelGroup.Children.Add(countryText.Content);
        }

        public PagesByCountryDiagramViewModel(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        : base(geographyProvider, booksReadProvider)
        {
        }
    }
}
