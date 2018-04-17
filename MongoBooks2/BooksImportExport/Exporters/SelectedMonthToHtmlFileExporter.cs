// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooksImportExport.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The selected month to html file exporter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Exporters
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using BooksCore.Geography;
    using BooksCore.Books;
    using BooksCore.Provider;
    using BooksImportExport.Interfaces;
    using BooksImportExport.Utilities;

    public class SelectedMonthToHtmlFileExporter : IBooksFileExport
    {
        /// <summary>
        /// Gets the export method name.
        /// </summary>
        public string Name => "Selected month to HTML";

        /// <summary>
        /// Gets the export file type extension.
        /// </summary>
        public string Extension => "html";

        /// <summary>
        /// Gets the export file type filter.
        /// </summary>
        public string Filter => @"All files (*.*)|*.*|HTML Files (*.html)|*.html";

        /// <summary>
        /// Writes the data for this export to the file specified.
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        /// <param name="geographyProvider">The geography data provider.</param>
        /// <param name="booksReadProvider">The books data provider.</param>
        /// <param name="errorMessage">The error message if unsuccessful.</param>
        /// <returns>True if written successfully, false otherwise.</returns>
        public bool WriteToFile(
            string filename,
            GeographyProvider geographyProvider,
            BooksReadProvider booksReadProvider,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Set up the nations file.
                NationsFile nationsFile = new NationsFile();
                foreach (Nation nation in geographyProvider.Nations.OrderBy(x => x.Name))
                {
                    nationsFile.Nations.Add(nation);
                }

                // Serialize the XML
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(NationsFile));
                TextWriter textWriter = new StreamWriter(filename, false, Encoding.Default); //overwrite original file
                xmlSerializer.Serialize(textWriter, nationsFile);

                // Tidy up
                textWriter.Close();
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                return false;
            }

            return true;
        }
    }
}