// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountriesData.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The countries data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Geography
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public class CountriesData
    {
        #region Constructor
        public CountriesData(string countriesXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(countriesXml));
            ParseCountries(doc);
        }
        #endregion

        #region Private data

        private readonly Dictionary<string, CountryGeography> _countries = 
            new Dictionary<string, CountryGeography>();
        
        #endregion

        #region Local Methods

        private void ParseCountries(XmlDocument doc)
        {
            doc.SelectNodes("//Document/Placemark/name");

            XmlNodeList placemarkNodes = doc.SelectNodes("//Document/Placemark");

            if (placemarkNodes == null) return;

            foreach (object node in placemarkNodes)
            {
                XmlElement element = (XmlElement)node;
                CountryGeography country = CountryGeography.Create(element);
                _countries.Add(country.Name, country);
            }
        }
        
        #endregion

        #region Public Data

        public List<string> CountryNames => _countries.Keys.ToList();

        public List<CountryGeography> Countries => _countries.Values.OrderBy(x => x.Name).ToList();
        
        #endregion
    }
}
