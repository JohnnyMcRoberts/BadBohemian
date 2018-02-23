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
    using BooksCore.Books;
    using BooksCore.Interfaces;
    using BooksUtilities.Colors;
    using BooksCore.Geography;
    using BooksHelixCharts.Utilities;

    public class PagesReadByCountryDiagramViewModel : BaseDiagramViewModel
    {
        private Dictionary<string, uint> _countryToLogPagesLookUp;

        private Model3D _pagesReadByCountryModel;

        public Model3D PagesReadByCountryModel
        {
            get
            {
                return _pagesReadByCountryModel;
            }

            set
            {
                _pagesReadByCountryModel = value;
                OnPropertyChanged(() => PagesReadByCountryModel);
            }
        }

        public override void SetupModel()
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
            uint numColours = 1 + countryToLogPagesLookUp.Values.OrderByDescending(x => x).FirstOrDefault();
            List<Color> colors;
            ColorUtilities.SetupFaintPaletteForRange((int)numColours, out colors, 128);
            List<Color> stdColors = ColorUtilities.SetupStandardColourSet();

            int geographyIndex = 0;

            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries.OrderByDescending(x => x.TotalBooksReadFromCountry))
            {
                var name = authorCountry.Country;
                var country = GeographyProvider.Nations.FirstOrDefault(w => w.Name == name);
                if (country != null)
                {
                    DiagramUtilities.AddNationPagesPins(modelGroup, colors, authorCountry, name, country, _countryToLogPagesLookUp);
                }

                if (GeographyProvider.Nations != null && GeographyProvider.Nations.Count > 0)
                {
                    geographyIndex = AddNationsCountryGeographyPlane(modelGroup, stdColors, geographyIndex, name);
                }
            }

            AddGeographiesForNationsWithoutBooksRead(modelGroup);

            PagesReadByCountryModel = modelGroup;
        }

        private void AddGeographiesForNationsWithoutBooksRead(Model3DGroup modelGroup)
        {
            if (GeographyProvider.Nations != null && GeographyProvider.Nations.Count > 0)
            {
                List<string> authorCountries = BooksReadProvider.AuthorCountries.Select(x => x.Country).ToList();
                foreach (Nation nation in GeographyProvider.Nations)
                {
                    CountryGeography geography = nation.Geography;
                    if (geography != null && !authorCountries.Contains(geography.Name))
                    {
                        GeometryModel3D geographyGeometry =
                            DiagramUtilities.GetGeographyPlaneGeometry(geography, Colors.LightGray);
                        modelGroup.Children.Add(geographyGeometry);
                    }
                }
            }
        }

        private int AddNationsCountryGeographyPlane(Model3DGroup modelGroup, List<Color> stdColors, int geographyIndex, string name)
        {
            var nation = GeographyProvider.Nations.FirstOrDefault(g => g.Name == name);
            CountryGeography geography = nation?.Geography;
            if (geography != null)
            {
                var colour = stdColors[(geographyIndex % stdColors.Count)];
                GeometryModel3D geographyGeometry =
                    DiagramUtilities.GetGeographyPlaneGeometry(geography, colour, 0.4);
                modelGroup.Children.Add(geographyGeometry);
                geographyIndex++;
            }

            return geographyIndex;
        }


        public PagesReadByCountryDiagramViewModel(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        : base(geographyProvider, booksReadProvider)
        {
        }
    }
}