// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDiagramViewModel.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The base helix 3D diagram view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksHelixCharts.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using BooksUtilities.ViewModels;
    using BooksCore.Interfaces;
    using BooksCore.Geography;
    using BooksHelixCharts.Utilities;

    public abstract class BaseDiagramViewModel : BaseViewModel
    {
        public IGeographyProvider GeographyProvider { get; }

        public IBooksReadProvider BooksReadProvider { get; }

        protected void AddGeographiesForCountriesWithoutBooksRead(Model3DGroup modelGroup)
        {
            // If no country geographies stop.
            if (GeographyProvider.CountryGeographies == null || GeographyProvider.CountryGeographies.Count <= 0)
            {
                return;
            }

            List<string> authorCountries = BooksReadProvider.AuthorCountries.Select(x => x.Country).ToList();
            foreach (CountryGeography geography in GeographyProvider.CountryGeographies)
            {
                if (geography == null)
                    continue;

                if (!authorCountries.Contains(geography.Name))
                {
                    GeometryModel3D geographyGeometry =
                        DiagramUtilities.GetGeographyPlaneGeometry(geography, Colors.LightGray);
                    modelGroup.Children.Add(geographyGeometry);
                }
            }
        }

        protected int AddCountryGeographyPlane(Model3DGroup modelGroup, List<Color> stdColors, int geographyIndex, string name)
        {
            CountryGeography geography = null;
            foreach (CountryGeography countryGeography in GeographyProvider.CountryGeographies)
            {
                if (countryGeography != null && countryGeography.Name == name)
                {
                    geography = countryGeography;
                    break;
                }

            }

            if (geography != null)
            {
                Color colour = stdColors[(geographyIndex % stdColors.Count)];
                GeometryModel3D geographyGeometry =
                    DiagramUtilities.GetGeographyPlaneGeometry(geography, colour, 0.4);
                modelGroup.Children.Add(geographyGeometry);
                geographyIndex++;
            }

            return geographyIndex;
        }

        protected void SetupCountyPagesLookups(out int maxBooksPages, out int maxBooksLogPages,
            out Dictionary<string, long> countryToReadLookUp, out Dictionary<string, uint> countryToPagesLookUp,
            out Dictionary<string, uint> countryToLogPagesLookUp)
        {
            maxBooksPages = 0;
            maxBooksLogPages = 0;
            countryToReadLookUp = new Dictionary<string, long>();
            countryToPagesLookUp = new Dictionary<string, uint>();
            countryToLogPagesLookUp = new Dictionary<string, uint>();
            foreach (var authorCountry in BooksReadProvider.AuthorCountries)
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

        public abstract void SetupModel();

        public BaseDiagramViewModel(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            GeographyProvider = geographyProvider;
            BooksReadProvider = booksReadProvider;
        }
    }
}
