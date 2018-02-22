// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountryGeography.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The base MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Geography
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using BooksCore.Interfaces;

    public class CountryGeography : IGeographicEntity
    {
        public string Name { get; set; }

        public string Description { get; set; } // eg "ISO_A2=SE : ISO_N3=752.0"

        public string ISO_A2
        {
            get
            {
                if (!Description.Contains("ISO_A2=")) return string.Empty;
                string[] elements = Description.Substring(7).Split(' ');
                return elements[0];
            }
        }

        public string ISO_N3
        {
            get
            {
                if (!Description.Contains("ISO_N3=")) return string.Empty;
                string[] elements = Description.Split('=');
                return elements[elements.Length - 1];
            }
        }

        public double CentralLongitude { get; set; }

        public double CentralLatitude { get; set; }

        public double MinLongitude { get; private set; }

        public double MinLatitude { get; private set; }

        public double MaxLongitude { get; private set; }

        public double MaxLatitude { get; private set; }

        public double CentroidLongitude { get; private set; }

        public double CentroidLatitude { get; private set; }

        public double TotalArea
        {
            get
            {
                double ttl = 0;
                foreach (PolygonBoundary block in LandBlocks)
                {
                    ttl += block.TotalArea;
                }

                return ttl;
            }
        }

        public List<PolygonBoundary> LandBlocks { get; set; }

        public XmlElement XmlElement { get; set; }

        public CountryGeography()
        {
            LandBlocks = new List<PolygonBoundary>();
        }

        public static CountryGeography Create(XmlElement element)
        {
            CountryGeography country = new CountryGeography
            {
                Name = element.SelectSingleNode("name")?.InnerText,
                Description = element.SelectSingleNode("description")?.InnerText
            };

            XmlNode lookatNode = element.SelectSingleNode("LookAt");

            if (lookatNode != null)
            {
                string latText = lookatNode.SelectSingleNode("latitude")?.InnerText;
                if (latText != null)
                {
                    country.CentralLatitude = double.Parse(latText);
                }

                string longText = lookatNode.SelectSingleNode("longitude")?.InnerText;
                if (longText != null)
                {
                    country.CentralLongitude = double.Parse(longText);
                }
            }

            XmlNodeList boundaryRingCoordinates =
                element.SelectNodes("MultiGeometry/Polygon/outerBoundaryIs/LinearRing");

            if (boundaryRingCoordinates != null)
            {
                foreach (object boundary in boundaryRingCoordinates)
                {
                    XmlElement boundaryElement = boundary as XmlElement;
                    if (boundaryElement != null)
                    {
                        string coordinateText = boundaryElement.SelectSingleNode("coordinates")?.InnerText;
                        PolygonBoundary landBlock = new PolygonBoundary(coordinateText);
                        country.LandBlocks.Add(landBlock);
                    }
                }
            }

            country.UpdateLatLongs();

            country.XmlElement = element;

            return country;
        }

        public static CountryGeography Create(string geographyXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(geographyXml);
            return Create(doc.DocumentElement);
        }

        public void UpdateLatLongs()
        {
            MinLongitude = MinLatitude = double.MaxValue;
            MaxLongitude = MaxLatitude = double.MinValue;

            CentroidLongitude = 0;
            CentroidLatitude = 0;

            double totalArea = 0;

            foreach (var landBlock in LandBlocks)
            {
                MinLongitude = Math.Min(landBlock.MinLongitude, MinLongitude);
                MaxLongitude = Math.Max(landBlock.MaxLongitude, MaxLongitude);

                MinLatitude = Math.Min(landBlock.MinLatitude, MinLatitude);
                MaxLatitude = Math.Max(landBlock.MaxLatitude, MaxLatitude);

                CentroidLongitude += landBlock.CentroidLongitude * landBlock.TotalArea;
                CentroidLatitude += landBlock.CentroidLatitude * landBlock.TotalArea;

                totalArea += landBlock.TotalArea;
            }

            CentroidLongitude /= totalArea;
            CentroidLatitude /= totalArea;
        }
    }
}
