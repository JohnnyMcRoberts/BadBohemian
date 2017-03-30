using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDbBooks.Models.Geography;

namespace MongoDbBooks.ViewModels.Utilities
{
    public class PolygonReducer
    {

        #region Constants

        public const double LatLongTolerance = 1e-12;
        public const double InitialPolygonTolerance = 1e-4;
        public const int MaxIterations = 20;
        public const int MaxPolygonPoints = 750;

        #endregion


        /// <span class="code-SummaryComment"><summary></span>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="Points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="Tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static List<PolygonPoint> DouglasPeuckerReduction (
            List<PolygonPoint> points, Double tolerance)
        {
            if (points == null || points.Count < 3)
                return points;

            Int32 firstPoint = 0;
            Int32 lastPoint = points.Count - 1;
            List<Int32> pointIndexsToKeep = new List<Int32>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(points, firstPoint, lastPoint,
            tolerance, ref pointIndexsToKeep);

            List<PolygonPoint> returnPoints = new List<PolygonPoint>();
            pointIndexsToKeep.Sort();
            foreach (Int32 index in pointIndexsToKeep)
            {
                returnPoints.Add(points[index]);
            }

            return returnPoints;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Douglases the peucker reduction.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="firstPoint">The first point.</param></span>
        /// <span class="code-SummaryComment"><param name="lastPoint">The last point.</param></span>
        /// <span class="code-SummaryComment"><param name="tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><param name="pointIndexsToKeep">The point index to keep.</param></span>
        private static void DouglasPeuckerReduction(List<PolygonPoint>
            points, Int32 firstPoint, Int32 lastPoint, Double tolerance,
            ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// The distance of a point from a line made from point1 and point2.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="pt1">The PT1.</param></span>
        /// <span class="code-SummaryComment"><param name="pt2">The PT2.</param></span>
        /// <span class="code-SummaryComment"><param name="p">The p.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static Double PerpendicularDistance(
            PolygonPoint point1, PolygonPoint point2, PolygonPoint point)
        {

            double Point1X, Point1Y;
            point1.GetCoordinates(out Point1X, out Point1Y);
            double Point2X, Point2Y;
            point2.GetCoordinates(out Point2X, out Point2Y);
            double PointX, PointY;
            point.GetCoordinates(out PointX, out PointY);

            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base


            Double area = Math.Abs(.5 * (Point1X * Point2Y + Point2X *
            PointY + PointX * Point1Y - Point2X * Point1Y - PointX *
            Point2Y - Point1X * PointY));
            Double bottom = Math.Sqrt(Math.Pow(Point1X - Point2X, 2) +
            Math.Pow(Point1Y - Point2Y, 2));
            Double height = area / bottom * 2;

            return height;


        }

        public static List<PolygonPoint> AdaptativePolygonReduce(
            List<PolygonPoint> points, Int32 maxSize)
        {
            int attempts = 0;

            if (points.Count < maxSize) return points;

            List<PolygonPoint> reducedPoints = new List<PolygonPoint>();
            reducedPoints.AddRange(points);

            // check that first and last points are different & if not remove the last point
            PolygonPoint first = points.First();
            PolygonPoint last = points.Last();
            bool lastRemoved = false;
            if (Math.Abs(first.Latitude - last.Latitude) < LatLongTolerance && 
                Math.Abs(first.Longitude - last.Longitude) < LatLongTolerance)
            {
                lastRemoved = true;
                reducedPoints.RemoveAt(points.Count - 1);
            }

            double tolerance = InitialPolygonTolerance;

            while (attempts < MaxIterations)
            {
                reducedPoints = DouglasPeuckerReduction(reducedPoints, tolerance);

                if (reducedPoints.Count <= maxSize)
                    break;
                else
                {
                    attempts++;
                    tolerance *= 2;
                }

            }

            if (lastRemoved)
                reducedPoints.Add(last);

            return reducedPoints;
        }

    }
}
