﻿namespace MongoDbBooks.Models.Geography
{
    using System.Collections.Generic;

    public interface IGeographicEntity
    {
        string Name { get; }

        double CentroidLongitude { get; }

        double CentroidLatitude { get; }

        double MinLongitude { get; }

        double MinLatitude { get; }

        double MaxLongitude { get; }

        double MaxLatitude { get; }

        double TotalArea { get; }

        List<PolygonBoundary> LandBlocks { get; }
    }
}
