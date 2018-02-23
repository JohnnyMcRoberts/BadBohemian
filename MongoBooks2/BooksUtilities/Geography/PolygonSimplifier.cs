// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolygonSimplifier.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The Polygon simplifier utility which reduces a complex polygon to a set of triangles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksUtilities.Geography
{
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;
    using Poly2Tri;
    using Poly2Tri.Triangulation.Polygon;


    public class PolygonSimplifier
    {
        public static List<List<Point3D>> TriangulatePolygon(List<Point3D> xyPoints)
        {
            int nVertices = xyPoints.Count;
            if (nVertices <= 0)
                return new List<List<Point3D>>();

            var points = new List<PolygonPoint>();
            foreach (var xy in xyPoints)
                points.Add(new PolygonPoint(xy.X, xy.Y));

            Polygon poly = new Polygon(points);
            P2T.Triangulate(poly);

            List<List<Point3D>> simpleShapes = new List<List<Point3D>>();

            foreach (var triangle in poly.Triangles)
            {
                List<Point3D> simple = new System.Collections.Generic.List<Point3D>();

                foreach (var pt in triangle.Points)
                {
                    simple.Add(new Point3D(pt.X, pt.Y, 0));
                }

                simpleShapes.Add(simple);
            }

            return simpleShapes;
        }
    }
}
