// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NationsFromXmlFileImport.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The nations from xml file importer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Importers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using BooksCore.Base;
    using BooksCore.Geography;
    using BooksImportExport.Interfaces;
    using BooksImportExport.Utilities;

    /// <summary>
    /// The nations from xml file importer class.
    /// </summary>
    public class NationsFromXmlFileImport : IBooksFileImport
    {
        /// <summary>
        /// Gets the import method name.
        /// </summary>
        public string Name => "Nations To Xml";

        /// <summary>
        /// Gets the import file type extension.
        /// </summary>
        public string Extension => "xml";

        /// <summary>
        /// Gets the import file type filter.
        /// </summary>
        public string Filter => @"All files (*.*)|*.*|XML Files (*.xml)|*.xml";

        /// <summary>
        /// Gets the type of the items that are being imported from the file.
        /// </summary>
        public Type ImportType => typeof(Nation);

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

            // Try to deserialize the nations file.
            try
            {
                using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    // Create the serializer.
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(NationsFile));

                    // Try to read the nations file.
                    NationsFile nationsFile = xmlSerializer.Deserialize(file) as NationsFile;

                    if (nationsFile == null)
                    {
                        errorMessage = $"File {filename} could not be deserialized";
                        return false;
                    }

                    ImportedItems.Clear();

                    // Add the items to the list.
                    foreach (Nation nation in nationsFile.Nations)
                    {
                        ImportedItems.Add(nation);
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
        /// Initializes a new instance of the <see cref="NationsFromXmlFileImport"/> class.
        /// </summary>
        public NationsFromXmlFileImport()
        {
            ImportedItems = new List<BaseMongoEntity>();
        }
    }
}
