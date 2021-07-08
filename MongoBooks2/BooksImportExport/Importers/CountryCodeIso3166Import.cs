// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountryCodeIso3166Import.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The country codes from csv file importer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Importers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using CsvHelper;

    using BooksCore.Base;
    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksImportExport.Interfaces;

    public class CountryCodeIso3166Import : IBooksFileImport
    {
        /// <summary>
        /// Gets the import method name.
        /// </summary>
        public string Name => "ISO-3166 Country codes from CSV";

        /// <summary>
        /// Gets the import file type extension.
        /// </summary>
        public string Extension => "csv";

        /// <summary>
        /// Gets the import file type filter.
        /// </summary>
        public string Filter => @"All files (*.*)|*.*|CSV files (*.csv)|*.csv";

        /// <summary>
        /// Gets the type of the items that are being imported from the file.
        /// </summary>
        public Type ImportType => typeof(CountryCodeIso3166);

        /// <summary>
        /// Gets the imported items.
        /// </summary>
        public List<BaseMongoEntity> ImportedItems { get; }

        /// <summary>
        /// Reads the data for this import from the file specified.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        /// <param name="errorMessage">The error message if unsuccessful.</param>
        /// <returns>True if read successfully, false otherwise.</returns>
        public bool ReadFromFile(string filename, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check the file exists.
            if (!File.Exists(filename))
            {
                errorMessage = $"File {filename} does not exist";
                return false;
            }

            // Try to deserialize the books file.
            try
            {
                using (StreamReader sr = new StreamReader(filename, Encoding.Default))
                {
                    CsvReader csv = new CsvReader(sr);

                    ImportedItems.Clear();

                    // name,alpha-2,alpha-3,country-code,iso_3166-2	region,sub-region,intermediate-region,region-code,sub-region-code,intermediate-region-code
                    while (csv.Read())
                    {
                        string stringName = csv.GetField<string>(0);
                        string stringAlpha_2 = csv.GetField<string>(1);
                        string stringAlpha_3 = csv.GetField<string>(2);
                        string stringCountryCode = csv.GetField<string>(3);
                        string stringIso_3166_2 = csv.GetField<string>(4);
                        string stringRegion = csv.GetField<string>(5);
                        string stringSubRegion = csv.GetField<string>(6);
                        string stringIntermediateRegion = csv.GetField<string>(7);
                        string stringRegionCode = csv.GetField<string>(8);
                        string stringSubRegionCode = csv.GetField<string>(9);
                        string stringIntermediateRegionCode = csv.GetField<string>(10);

                        ushort intCountryCode = 0;
                        ushort.TryParse(stringCountryCode, out intCountryCode);
                        ushort intRegionCode = 0;
                        ushort.TryParse(stringRegionCode, out intRegionCode);
                        ushort intSubRegionCode = 0;
                        ushort.TryParse(stringSubRegionCode, out intSubRegionCode);
                        ushort intIntermediateRegionCode = 0;
                        ushort.TryParse(stringIntermediateRegionCode, out intIntermediateRegionCode);

                        CountryCodeIso3166 countryCode = new CountryCodeIso3166
                        {
                            Name = stringName,
                            Alpha_2 = stringAlpha_2,
                            Alpha_3 = stringAlpha_3,
                            CountryCode = intCountryCode,
                            Iso_3166_2 = stringIso_3166_2,
                            Region = stringRegion,
                            SubRegion = stringSubRegion,
                            IntermediateRegion = stringIntermediateRegion,
                            RegionCode = intRegionCode,
                            SubRegionCode = intSubRegionCode,
                            IntermediateRegionCode = intIntermediateRegionCode
                        };

                        if (intCountryCode > 0 & intCountryCode < 1000)
                        {
                            ImportedItems.Add(countryCode);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryCodeIso3166Import"/> class.
        /// </summary>
        public CountryCodeIso3166Import()
        {
            ImportedItems = new List<BaseMongoEntity>();
        }
    }
}
