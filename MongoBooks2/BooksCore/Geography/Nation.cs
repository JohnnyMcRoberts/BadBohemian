// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Nation.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The nation MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksCore.Geography
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    using BooksCore.Base;

    /// <summary>
    /// The MongoDb entity for a nation.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Nation : WorldCountry
    {
        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

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

        /// <summary>
        /// Gets the name to use for equivalence checks.
        /// </summary>
        public override string EquivalenceName => Name;

        private CountryGeography _countryGeography;
    }
}
