// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldCountry.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The world country MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Geography
{
    using BooksCore.Base;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class WorldCountry : BaseMongoEntity
    {
        [BsonElement("country")]
        public string Country { get; set; }

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

        [BsonElement("flag_url")]
        public string FlagUrl { get; set; }

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
        /// Gets the name to use for equivalence checks.
        /// </summary>
        public override string EquivalenceName => Country;
    }
}
