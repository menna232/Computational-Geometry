using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            for (int i = 0; i < polygons.Count; i++)
            {
                //take the start point of each vertex in the polygon in a list to work on 
                List<Point> polygonPoints = new List<Point>();
                for (int j = 0; j < polygons[i].lines.Count; j++)
                {
                    polygonPoints.Add(polygons[i].lines[j].Start);
                }

                //sort the points counter clock wise 
                SortCounterClockWise(ref polygonPoints);

                //find the diagonals the the polygon
                FindIntersectingDiagonals(polygonPoints, ref outLines);

            
            }
      
        
        }


      
        private double calcDistanceBetweenPoints(Point a, Point b)
        {
            // Calculate the squared Euclidean distance using the formula: (x2 - x1)^2 + (y2 - y1)^2
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }
        private void SortCounterClockWise(ref List<Point> points)
        {
            int idx = 0;
            int N = points.Count;

            // Find the leftmost point as the starting point
            for (int i = 1; i < N; i++)
            {
                if (points[i].X < points[idx].X || (points[i].X == points[idx].X && points[i].Y < points[idx].Y))
                {
                    idx = i;
                }
            }
            // Check if the turn formed by the line segments is counter-clockwise
            if (HelperMethods.CheckTurn(new Line(points[(idx - 1 + N) % N], points[(idx + 1) % N]), points[idx]) == Enums.TurnType.Right) return;
            points.Reverse();
        }

        private bool IsConvex(Point current, Point previous, Point next)
        {
            // Check if the current vertex is convex by calculating the cross product of the two adjacent edges
            double crossProduct = (previous.X - current.X) * (next.Y - current.Y) - (previous.Y - current.Y) * (next.X - current.X);
            return crossProduct < 0; // Convex if the cross product is negative
        }

        private void FindIntersectingDiagonals(List<Point> points, ref List<Line> outLines)
        {
            //check if the point forms a polygon
            if (points.Count <= 3)
            {

                return;  //invalid polygon , must be more than 3 points 
            }


            int j = 0, idx_Diagonal_Start = 0;

            int N = points.Count;
            List<Point> p1 = new List<Point>();
            List<Point> p2 = new List<Point>();
            
            Point nearestPoint = null;
            double nearestDist = double.MaxValue;

            // Find a convex point in the polygon
            for (j = 0; j < N; j++)
            {
                if (IsConvex(points[j], points[(j - 1 + N) % N], points[(j + 1) % N]))
                {
                    // Check for points inside the triangle formed by the convex point
                    for (int k = 0; k < N; k++)
                    {
                        int previousIndex = (j + N - 1) % N;
                        int currentIndex = j;
                        int nextIndex = (j + 1) % N;

                        Point previousPoint = points[previousIndex];
                        Point currentPoint = points[currentIndex];
                        Point nextPoint = points[nextIndex];

                        if (k == currentIndex || k == nextIndex || k == previousIndex) continue;
                        if (HelperMethods.PointInTriangle(points[k], previousPoint, currentPoint, nextPoint) == Enums.PointInPolygon.Inside)
                        {
                            double curDist = calcDistanceBetweenPoints(points[j], points[k]);
                            if (curDist < nearestDist)
                            {
                                // Find the nearest point inside the triangle
                                nearestDist = curDist;
                                nearestPoint = points[k];
                            }
                        }
                    }
                    break;
                }
            }

            Line diagonal = null;
            if (nearestPoint == null)
            {
                // If no point is inside the triangle, create a diagonal between two adjacent vertices (previous and next)
                diagonal = new Line(points[(j - 1 + N) % N], points[(j + 1) % N]);
                //move to the prev point
                idx_Diagonal_Start = (j - 1 + N) % N;
            }
            else
            {
                // If a point is inside the triangle, create a diagonal between the convex point and the nearest point
                diagonal = new Line(points[j], nearestPoint);
               //move to the current point
                idx_Diagonal_Start = j;
            }
            // Add the diagonal to the list of intersecting diagonals
            outLines.Add(diagonal);

            Point start = points[idx_Diagonal_Start];

            // Extract points on one side of the diagonal
            while (start != diagonal.End)
            {
                p1.Add(start);
                idx_Diagonal_Start = (idx_Diagonal_Start + 1) % N;
                start = points[idx_Diagonal_Start];
            }
            p1.Add(diagonal.End);

            // Extract points on the other side of the diagonal
            while (start != diagonal.Start)
            {
                p2.Add(start);
                idx_Diagonal_Start = (idx_Diagonal_Start + 1 + N) % N;
                start = points[idx_Diagonal_Start];
            }
            p2.Add(diagonal.Start);

            // Sort the points in counterclockwise order
            SortCounterClockWise(ref p1);
            SortCounterClockWise(ref p2);

            // Recursively find intersecting diagonals for each side
            FindIntersectingDiagonals(p1, ref outLines);
            FindIntersectingDiagonals(p2, ref outLines);
        }

        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}
