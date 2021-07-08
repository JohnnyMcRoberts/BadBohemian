
namespace BooksImportExport.Utilities
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    using BooksImportExport.Importers;
    using BooksCore.Geography;

    public static class AllCountryCodes
    {
        #region File Constants

        /// <summary>
        /// The All Country Codes Csv file.
        /// </summary>
        public const string AllCountryCodesCsv = "AllCountryCodes.csv";

        #endregion

        #region Local Data

        private static CountryCodeIso3166Import _countryCodeImport = null;

        private static string _errorMessage = null;

        private static List<CountryCodeIso3166> _countryCodes = null;

        #endregion

        #region Public Properties

        public static string ErrorMessage => _errorMessage;

        public static CountryCodeIso3166Import CountryCodeImport
        {
            get
            {
                if (_countryCodeImport == null)
                {
                    string path = GetPathToTopLevelFile(AllCountryCodesCsv);
                    _countryCodeImport = new CountryCodeIso3166Import();
                    _countryCodeImport.ReadFromFile(path, out _errorMessage);
                }

                return _countryCodeImport;
            }
        }

        /// <summary>
        /// Gets the imported items.
        /// </summary>
        public static List<CountryCodeIso3166> CountryCodes
        {
            get
            {
                if (_countryCodes != null)
                {
                    return _countryCodes;
                }

                List<CountryCodeIso3166> codes = new List<CountryCodeIso3166>();
                foreach (var readItem in CountryCodeImport.ImportedItems)
                {
                    if (readItem is CountryCodeIso3166)
                    {
                        codes.Add(readItem as CountryCodeIso3166);
                    }

                }

                return codes;
            }

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the full path to a file in the top directory.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        /// <returns>The full path for the local file.</returns>
        public static string GetPathToTopLevelFile(string filename)
        {
            try
            {
                string projectPath = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(projectPath, filename);
            }
            catch (Exception ex)
            {
                return ("Directory for file not found: " + ex.Message);
            }
        }

        #endregion
    }
}
