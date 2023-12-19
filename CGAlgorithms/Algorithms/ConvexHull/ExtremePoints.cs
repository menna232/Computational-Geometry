using CGUtilities;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            List<Point> Check = new List<Point>(points);
            int p1 = 0;
            while (p1 < Check.Count)
            {
                bool foundPoint = false;
                int t1 = 0;
                while (t1 < Check.Count)
                {
                    int t2 = 0;
                    while (t2 < Check.Count)
                    {
                        int t3 = 0;
                        while (t3 < Check.Count)
                        {
                            if (p1 != t1 && p1 != t2 && p1 != t3)
                            {
                                Point pointI = Check[p1];
                                Point pointJ = Check[t1];
                                Point pointK = Check[t2];
                                Point pointL = Check[t3];
                                Enums.PointInPolygon checkPoint = HelperMethods.PointInTriangle(pointI, pointJ, pointK, pointL);
                                if (checkPoint == Enums.PointInPolygon.Inside || checkPoint == Enums.PointInPolygon.OnEdge)
                                {
                                    Check.RemoveAt(p1);
                                    p1--;
                                    foundPoint = true;
                                    break;
                                }
                            }
                            t3++;
                        }
                        if (foundPoint)
                            break;
                        t2++;
                    }
                    if (foundPoint)
                        break;
                    t1++;
                }
                p1++;
            }

            outPoints = Check;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
