using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

namespace MongoDbBooks.Models.Geography
{
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
            var nameNodes = doc.SelectNodes("//Document/Placemark/name");

            var placemarkNodes = doc.SelectNodes("//Document/Placemark");

            if (placemarkNodes == null) return;

            foreach (var node in placemarkNodes)
            {
                XmlElement element = (XmlElement)node;
                CountryGeography country = CountryGeography.Create(element);
                _countries.Add(country.Name, country);
            }
        }
        
        #endregion

        #region Public Data

        public List<string> CountryNames { get { return _countries.Keys.ToList(); } }
        public List<CountryGeography> Countries { get { return _countries.Values.OrderBy(x => x.Name).ToList(); } }
        
        #endregion
    }
}
