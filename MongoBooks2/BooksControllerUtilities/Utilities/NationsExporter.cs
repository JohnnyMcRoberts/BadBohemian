namespace BooksControllerUtilities.Utilities
{
    using System;
    using System.Collections.Generic;

    using System.Text;

    using CsvHelper;


    using BooksControllerUtilities.DataClasses;
    using BooksCore.Geography;

    public static class NationsExporter
    {
        #region Constants

        public static readonly string[] ColumnHeaders =
        {
            "Date",
            "DD/MM/YYYY",
            "Author",
            "Title",
            "Pages",
            "Note",
            "Nationality",
            "Original Language",
            "Book",
            "Comic",
            "Audio",
            "Image",
            "Tags"
        };

        #endregion

        #region Private Utility functions

        public static string GetCsvHeader()
        {
            string headerString = string.Empty;

            headerString += ColumnHeaders[0];

            for (int i = 1; i < ColumnHeaders.Length; i++)
            {
                headerString += ",";
                headerString += ColumnHeaders[i];
            }

            headerString += "\n";

            return headerString;
        }

        private static string GetCsvNationRecord(Nation nation)
        {
            string recordString = string.Empty;

            int count = 0;
            foreach (NationRecordFields recordField in Enum.GetValues(typeof(NationRecordFields)))
            {
                if (count != 0)
                    recordString += ",";

                recordString += CsvNation.GetCsvNationRecordField(recordField, nation);
                count++;
            }

            recordString += "\n";

            return recordString;
        }


        #endregion

        #region Public Static methods

        public static void ExportToFile(List<Nation> nations, out string fileContent)
        {
            fileContent = string.Empty;

            fileContent += GetCsvHeader();

            foreach (Nation nation in nations)
            {
                fileContent += GetCsvNationRecord(nation);
            }
        }

        #endregion

        public static void ExportToCsvFile(List<Nation> nations, out string fileContent)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ExtendedStringWriter sw = new ExtendedStringWriter(stringBuilder, Encoding.UTF8);

            // write the header
            sw.WriteLine("Name,Capital,Latitude,Longitude,ImageUri,GeographyXml");

            // write the records
            CsvWriter csv = new CsvWriter(sw);
            foreach (Nation nation in nations)
            {
                csv.WriteField(nation.Name);
                csv.WriteField(nation.Capital);
                csv.WriteField(nation.Latitude);
                csv.WriteField(nation.Longitude);
                csv.WriteField(nation.ImageUri);
                csv.WriteField(nation.GeographyXml);
                csv.NextRecord();
            }

            // tidy up
            sw.Close();

            fileContent = sw.ToString();
        }
    }
}
