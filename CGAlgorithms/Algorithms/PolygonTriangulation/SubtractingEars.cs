using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            // steps
            // 1 - check if the point is convex by checking the crosss product of the 2 adjacent vertcies is negative == convex 
            //  if == convex  , check if it is an ear
            // if == ear crop it  by removing it from the polygon and update n  , next , current  and previous points 
            // add a line between the previous and the next point 
            //  if not breake 
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

                

                // Find the ears in the current polygon
                List<int> ears = FindEars(polygonPoints);
              
                // Continue the triangulation process as long as there are ears and the polygon has more than 3 points
                while (ears.Count != 0 && polygonPoints.Count > 3)
                {
                    // Extract the index of the first ear in the list
                    // Remove the ear and add the corresponding diagonal line to the output
                    outLines.Add(SubtractEar(ears[0], ref ears, ref polygonPoints));
                }
            }
        }

        private  void SortCounterClockWise(ref List<Point> points)
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



        private bool IsPointEar(int n, List<Point> points)
        {
            int NumOfPoints = points.Count;
            // Check if the current vertex is an ear 
            // An ear is a convex vertex that does not contain any other points inside its triangle

            // Check if the current vertex is convex
            if (!IsConvex(points[n], points[(n - 1 + NumOfPoints) % NumOfPoints], points[(n + 1) % NumOfPoints]))
                return false;

            for (int i = 0; i < NumOfPoints; i++)
            {
                int previousIndex = (n + NumOfPoints - 1) % NumOfPoints;
                int currentIndex = n;
                int nextIndex = (n + 1) % NumOfPoints;

                if (i == currentIndex || i == previousIndex || i == nextIndex) continue;
                if (HelperMethods.PointInTriangle(points[i], points[(n - 1 + NumOfPoints) % NumOfPoints], points[n], points[(n + 1) % NumOfPoints]) != Enums.PointInPolygon.Outside)

                    return false; // Found a point inside the triangle, so it is not an ear 
            }
            return true; // No point inside the triangle, so it is an ear 
        }

        private List<int> FindEars(List<Point> points)
        {
            //total number of points in the polygon
            int N = points.Count;
            // list to store the indices of ears in the polygon
            List<int> ears = new List<int>();

            // Iterate through each vertex in the polygon and Check if the current vertex forms an ear

            for (int i = 0; i < N; i++)
            {
                if (IsPointEar(i, points))
                {
                    // Add the index of the ear to the list
                    ears.Add(i);
                }
            }
            return ears;
        }

        private Line SubtractEar(int i, ref List<int> ears, ref List<Point> points)
        {
            // Calculate indices for the previous, current, and next points in the polygon

            int NumOfPoints = points.Count;
            int previousIndex = (i + NumOfPoints - 1) % NumOfPoints;
            int currentIndex = i;
            int nextIndex = (i + 1) % NumOfPoints;

            // Get the actual points for the points

            Point previousPoint = points[previousIndex];
            Point currentPoint = points[currentIndex];
            Point nextPoint = points[nextIndex];
           
            // Create a diagonal line connecting the previous and next points
            Line diagonal = new Line(previousPoint, nextPoint);

            // Update the number of points in the polygon
            NumOfPoints--;

            // Remove the current vertex (ear) from the polygon
            points.RemoveAt(i);
            ears.Remove(i);
            
            // Adjust ear indices greater than the removed index
            for (int j = 0; j < ears.Count; j++)
            { 
                if (ears[j] > i)
                {
                    ears[j]--;
                }
            }

            // Check and update the ear status of the point before the removed ear
            if (IsPointEar(i % NumOfPoints, points) && !ears.Contains(i % NumOfPoints))
            {
                ears.Add(i % NumOfPoints);
            }
            else if (ears.Contains(i % NumOfPoints) && !IsPointEar(i % NumOfPoints, points))
            { 
                ears.Remove(i % NumOfPoints); 
            }

            // Check and update the ear status of the point after the removed ear

            if (IsPointEar((i - 1 + NumOfPoints) % NumOfPoints, points) && !ears.Contains((i - 1 + NumOfPoints) % NumOfPoints))
            {
                ears.Add((i - 1 + NumOfPoints) % NumOfPoints);
            }
            else if (!IsPointEar((i - 1 + NumOfPoints) % NumOfPoints, points) && ears.Contains((i - 1 + NumOfPoints) % NumOfPoints))
            { 
                ears.Remove((i - 1 + NumOfPoints) % NumOfPoints); 
            }

            // Return the diagonal line
            return diagonal;
        }

        public override string ToString()
        {
            return "Subtracting Ears";
        }

    }
}


