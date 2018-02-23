using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksCore.Provider
{
    using System.Collections.ObjectModel;
    using BooksCore.Geography;
    using BooksCore.Interfaces;

    public class GeographyProvider : IGeographyProvider
    {
        public ObservableCollection<Nation> Nations { get; private set; }

        public ObservableCollection<WorldCountry> WorldCountries { get; }

        public ObservableCollection<CountryGeography> CountryGeographies { get; }

        public void Setup(ObservableCollection<Nation> nations)
        {
            Nations = nations;
            WorldCountries.Clear();
            CountryGeographies.Clear();
            foreach (Nation nation in Nations)
            {
                if (string.IsNullOrEmpty(nation.Country))
                    nation.Country = nation.Name;

                WorldCountries.Add(nation);
                CountryGeographies.Add(nation.Geography);
            }
        }

        public GeographyProvider()
        {
            WorldCountries = new ObservableCollection<WorldCountry>();
            Nations = new ObservableCollection<Nation>();
            CountryGeographies = new ObservableCollection<CountryGeography>();
        }
    }
}
