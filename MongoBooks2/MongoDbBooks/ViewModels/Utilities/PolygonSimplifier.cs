namespace MongoDbBooks.ViewModels.Utilities
{
    using System.Collections.Generic;
    using Poly2Tri;
    using Poly2Tri.Triangulation.Polygon;


    public class PolygonSimplifier
    {
        public static List<List<OxyPlot.DataPoint>> TriangulatePolygon(List<OxyPlot.DataPoint> xyPoints)
        {
            int nVertices = xyPoints.Count;
            if (nVertices <= 0)
                return new List<List<OxyPlot.DataPoint>>();

            var points = new List<PolygonPoint>();
            foreach (var xy in xyPoints)
                points.Add(new PolygonPoint(xy.X, xy.Y));

            Polygon poly = new Polygon(points);
            P2T.Triangulate(poly);

            List<List<OxyPlot.DataPoint>> simpleShapes = new List<List<OxyPlot.DataPoint>>();

            foreach (var triangle in poly.Triangles)
            {
                List<OxyPlot.DataPoint> simple = new System.Collections.Generic.List<OxyPlot.DataPoint>();

                foreach (var pt in triangle.Points)
                {
                    simple.Add(new OxyPlot.DataPoint(pt.X, pt.Y));
                }

                simpleShapes.Add(simple);
            }

            return simpleShapes;
        }
    }
}
