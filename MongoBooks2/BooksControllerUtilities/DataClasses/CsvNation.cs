namespace BooksControllerUtilities.DataClasses
{
    using BooksControllerUtilities.Utilities;
    using BooksCore.Geography;

    public enum NationRecordFields
    {
        Name = 0,
        Capital,
        Latitude,
        Longitude,
        ImageUri,
        GeographyXml
    }

    public class CsvNation
    {
        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the capital city.
        /// </summary>
        public string Capital { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the capital city.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the capital city.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the URI string for a .jpg/.png image for the nation.
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// Gets or sets the XML string for the nation's geography.
        /// </summary>
        public string GeographyXml { get; set; }

        public static string GetCsvNationRecordField(NationRecordFields recordField, Nation nation)
        {
            string fieldString = string.Empty;
            switch (recordField)
            {
                case NationRecordFields.Name:
                    fieldString += ExtendedStringWriter.GetSafeString(nation.Name);
                    break;

                case NationRecordFields.Capital:
                    fieldString += ExtendedStringWriter.GetSafeString(nation.Capital);
                    break;

                case NationRecordFields.Latitude:
                    fieldString += nation.Longitude;
                    break;

                case NationRecordFields.Longitude:
                    fieldString += nation.Longitude;
                    break;

                case NationRecordFields.ImageUri:
                    fieldString += ExtendedStringWriter.GetSafeString(nation.ImageUri);
                    break;

                case NationRecordFields.GeographyXml:
                    fieldString += ExtendedStringWriter.GetSafeString(nation.GeographyXml);
                    break;
            }

            return fieldString;
        }
    }
}
