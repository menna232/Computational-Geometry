using CGUtilities;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
                return;
            }
            points.Sort(SortByX);
            bool[] visitedPoints = new bool[points.Count];
            int p1 = 0;
            while (p1 < points.Count)
            {
                if (p1 > 0 && points[p1].Equals(points[p1 - 1]))
                {
                    p1++;
                    continue;
                }
                int p2 = p1 + 1;
                while (p2 < points.Count)
                {
                    if (points[p1].Equals(points[p2]))
                    {
                        p2++;
                        continue;
                    }
                    Line line = new Line(points[p1], points[p2]);
                    int RIGHT = 0, left = 0;

                    int k = 0;
                    while (k < points.Count)
                    {
                        var point = points[k];
                        if (point != points[p1] && point != points[p2])
                        {
                            Enums.TurnType checkTurn = HelperMethods.CheckTurn(line, point);

                            switch (checkTurn)
                            {
                                case Enums.TurnType.Right:
                                    RIGHT++;
                                    break;
                                case Enums.TurnType.Left:
                                    left++;
                                    break;
                                case Enums.TurnType.Colinear:
                                    if (!HelperMethods.PointOnSegment(point, points[p1], points[p2]))
                                    {
                                        RIGHT++;
                                        left++;
                                        break;
                                    }
                                    break;
                            }
                        }
                        k++;
                    }

                    if (left == 0 || RIGHT == 0)
                    {
                        if (!visitedPoints[p1])
                        {
                            visitedPoints[p1] = true;
                            outPoints.Add(points[p1]);
                        }

                        if (!visitedPoints[p2])
                        {
                            visitedPoints[p2] = true;
                            outPoints.Add(points[p2]);
                        }
                    }

                    p2++;
                }

                p1++;
            }
        }

        public int SortByX(Point a, Point b)
        {
            if (a.X == b.X)
            {
                return a.Y.CompareTo(b.Y);
            }
            return a.X.CompareTo(b.X);
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
