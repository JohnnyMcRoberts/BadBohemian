// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NationsFile.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The nations file for the xml importer/exporter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Utilities
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using BooksCore.Geography;

    [XmlRoot("NationsFile",  IsNullable = false)]
    [XmlInclude(typeof(Nation))]
    public class NationsFile
    {
        [XmlArray("Nations")]
        [XmlArrayItem("Nation")]
        public List<Nation> Nations;

        /// <summary>
        /// Initializes a new instance of the <see cref="NationsFile"/> class.
        /// </summary>
        public NationsFile()
        {
            Nations = new List<Nation>();
        }
    }
}
