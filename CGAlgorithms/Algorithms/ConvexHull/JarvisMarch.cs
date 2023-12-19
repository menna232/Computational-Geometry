using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int numbeerOfPoints = points.Count;
            if (numbeerOfPoints <= 3)
            {
                outPoints = points;
                return;
            }

            Point smallestPoint =getSmallestYPoint(ref points);
            Point startPoint = (Point)smallestPoint.Clone();
            Point nextPoint = (Point)points[0].Clone();


            do
            {
                outPoints.Add(startPoint);

                for (int i = 0; i < numbeerOfPoints; i++)
                {
                    Point VectorCurrent = new Point(startPoint.X - nextPoint.X, startPoint.Y - nextPoint.Y);
                    Point VectorCurrentNext = new Point(startPoint.X - points[i].X, startPoint.Y - points[i].Y);

                    if (HelperMethods.CheckTurn(VectorCurrent, VectorCurrentNext) == Enums.TurnType.Left || (HelperMethods.CheckTurn(VectorCurrent, VectorCurrentNext) == Enums.TurnType.Colinear && !HelperMethods.PointOnSegment(points[i], startPoint, nextPoint)))
                    {
                        nextPoint = points[i];
                    }
                }
                startPoint = nextPoint;

            } while (!nextPoint.Equals(smallestPoint));
        }


        public static Point getSmallestYPoint(ref List<Point> points)
        {
            Point smallestPoint = null;
            double smallY = double.MaxValue;

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < smallY)
                {
                    smallY = points[i].Y;
                    smallestPoint = points[i];
                }
                else if (points[i].Y == smallY)
                {
                    if (points[i].X < smallestPoint.X)
                    {
                        smallestPoint = points[i];
                    }

                }
            }
            return (Point)smallestPoint.Clone();
        }
        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
