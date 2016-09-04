using System;
using System.Collections.Generic;
using System.Linq;


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbBooks.Models
{
    public class WorldCountry : MongoEntity
    {
        // Country,Capital,Latitude,Longitude

        public string Country { get; set; }
        public string Capital { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

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
