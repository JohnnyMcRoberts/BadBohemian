using System;
using System.Collections.Generic;
using System.Linq;


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbBooks.Models
{

    [BsonIgnoreExtraElements]
    public class WorldCountry : BaseMongoEntity
    {
        [BsonElement("country")]
        public string Country { get; set; }

        [BsonElement("capital")]
        public string Capital { get; set; }

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }

        [BsonElement("flag_url")]
        public string FlagUrl { get; set; }

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
    }
}
