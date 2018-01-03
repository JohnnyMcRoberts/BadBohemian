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
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using Geography;

    /// <summary>
    /// The MongoDb entity for a nation.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Nation : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the capital city.
        /// </summary>
        [BsonElement("capital")]
        public string Capital { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the capital city.
        /// </summary>
        [BsonElement("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the capital city.
        /// </summary>
        [BsonElement("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the URI string for a .jpg/.png imafe for the nation.
        /// </summary>
        [BsonElement("image")]
        public double Image { get; set; }

        /// <summary>
        /// Gets or sets the XML string for the nation's geography.
        /// </summary>
        [BsonElement("geography_xml")]
        public string GeographyXml { get; set; }

        /// <summary>
        /// Gets or sets the URI string for a .jpg/.png image for the nation.
        /// </summary>
        [BsonElement("image_uri")]
        public string ImageUri { get; set; }

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
                uint seconds = (uint)((inDegrees - degrees) * 60.0);

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
                uint seconds = (uint)((inDegrees - degrees) * 60.0);

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

        /// <summary>
        /// Gets the image address ready to be displayed.
        /// </summary>
        public string DisplayImageAddress
        {
            get
            {
                if (ImageUri == null)
                    return "N/A";
                else
                    return ImageUri.Substring(0, Math.Min(ImageUri.Length, 50)) + " ...";
            }
        }

        /// <summary>
        /// Gets the image URI ready to be displayed.
        /// </summary>
        public Uri DisplayImage
        {
            get
            {
                return string.IsNullOrEmpty(ImageUri) ? new Uri("pack://application:,,,/Images/camera_image_cancel-32.png") : new Uri(ImageUri);
            }
        }

        /// <summary>
        /// Gets the image URI ready to be displayed.
        /// </summary>
        public CountryGeography Geography
        {
            get
            {
                if (string.IsNullOrEmpty(GeographyXml))
                    return null;

                if (_countryGeography == null)
                    _countryGeography = CountryGeography.Create(GeographyXml);

                return _countryGeography;
            }
        }

        private CountryGeography _countryGeography;
    }
}
