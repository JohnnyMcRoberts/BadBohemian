// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Nation.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The nation MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MongoDbBooks.Models.Database
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    [BsonIgnoreExtraElements]
    /// <summary>
    /// The MongoDb entity for a nation.
    /// </summary>
    public class Nation : BaseEntity
    {
        [BsonElement("capital")]
        /// <summary>
        /// Gets or sets the name of the capital city.
        /// </summary>
        public string Capital { get; set; }

        [BsonElement("latitude")]
        /// <summary>
        /// Gets or sets the latitude of the capital city.
        /// </summary>
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        /// <summary>
        /// Gets or sets the longitude of the capital city.
        /// </summary>
        public double Longitude { get; set; }

        [BsonElement("image")]
        /// <summary>
        /// Gets or sets the URI string for a .jpg/.png imafe for the nation.
        /// </summary>
        public double Image { get; set; }

        [BsonElement("geography_xml")]
        /// <summary>
        /// Gets or sets the XML string for the nation's geography.
        /// </summary>
        public string GeographyXml { get; set; }

        /// <summary>
        /// Gets the latitude in a degree, minutes and seconds.
        /// </summary>
        public string LatitudeText
        {
            get
            {
                double inDegrees = Latitude;
                string northSouth = "N";
                if (Latitude < 0)
                {
                    northSouth = "S";
                    inDegrees *= -1.0;
                }
                uint degrees = (uint)inDegrees;
                uint seconds = (uint)((inDegrees - (double)degrees) * 60.0);

                return degrees.ToString() + "\u00b0 " + seconds.ToString() + "' " + northSouth;
            }
        }

        /// <summary>
        /// Gets the latitude in a degree, minutes and seconds.
        /// </summary>
        public string LongitudeText
        {
            get
            {
                double inDegrees = Longitude;
                string eastWest = "E";
                if (Longitude < 0)
                {
                    eastWest = "W";
                    inDegrees *= -1.0;
                }
                uint degrees = (uint)inDegrees;
                uint seconds = (uint)((inDegrees - (double)degrees) * 60.0);

                return degrees.ToString() + "\u00b0 " + seconds.ToString() + "' " + eastWest;
            }
        }
        
        /// <summary>
        /// Gets the XML ready to be displayed.
        /// </summary>
        public string DisplayXML
        {
            get
            {
                if (GeographyXml == null)
                    return "N/A";
                else
                    return GeographyXml.Substring(0, Math.Min(GeographyXml.Length, 50)) + " ...";
            }
        }
    }
}
