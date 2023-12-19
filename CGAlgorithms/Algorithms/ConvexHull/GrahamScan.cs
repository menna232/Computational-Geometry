using CGUtilities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {

        // method finds the index of the point in the given list of points with the minimum Y-coordinate.
        // hena bageb el extreme 
        private int GetPointWithMinY(List<Point> points)
        {
            int Index = 0;
            int i = 0;
            for (; i < points.Count;)
            {
                if (points[i].Y < points[Index].Y)
                    Index = i;
                i++;
            }
            return Index;
        }

        public void baseCheck(List<Point> points, List<Point> outPoints)
        {

            if (points.Count == 1)
            {
                outPoints = points;
                return;
            }

        }
        //method swaps two points and returns a new list containing the swapped points.
        public List<Point> SwapTwoPoints(Point firstPoint, Point secondPoint)
        {
            Point temp = firstPoint;
            firstPoint = secondPoint;
            secondPoint = temp;
            List<Point> NewPoints = new List<Point>();


            bool isSwap = true;
            if (isSwap)
            {
                NewPoints.Add(firstPoint);
                NewPoints.Add(secondPoint);
            }
            if (true)
            {
                return NewPoints;
            }
        }



        //        The point with the minimum Y-coordinate is swapped to the first position in the points list
        //        to ensure it is part of the convex hull.
        //        The points list is sorted based on the Y-coordinate of each point.

        //The points list is then sorted based on the angle each point makes with the first point in counterclockwise order.
        //This sorting step arranges the points in the points list in a counterclockwise order around the first point.

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            baseCheck(points, outPoints);


            int PointWithMinY = GetPointWithMinY(points);

            // Swap the minimum Y-coordinate point to the first position
            //this part of the code organizes the points in a counterclockwise order
            //around the point with the minimum Y-coordinate.
            List<Point> swapedPoints = SwapTwoPoints(points[0], points[PointWithMinY]);

            points[0] = swapedPoints[0];
            points[PointWithMinY] = swapedPoints[1];

            points = points.OrderBy(point => point.Y).ToList();

            points = points.OrderBy(point => Math.Atan2(point.Y - points[0].Y, point.X - points[0].X)).ToList();



            List<Point> finalResult = new List<Point>();


            int x = 0;
            while (x < points.Count)
            {
                //This loop iterates through each point in the sorted order and adds it to the finalResult list.
                finalResult.Add(points[x]);

                if (x > 2)
                {
                    //After adding a new point to finalResult,
                    //this part of the code checks if the last three points in the result make a right turn.
                    //If they do, it removes the middle point to maintain the convex hull property.
                    //This ensures that only the left turns(counterclockwise direction) are preserved, forming the convex hull.
                    int lastIndex = finalResult.Count - 1;
                    while (lastIndex >= 2 && HelperMethods.CheckTurn(new Line(finalResult[lastIndex - 2], finalResult[lastIndex - 1]), finalResult[lastIndex]) != Enums.TurnType.Left)
                    {
                        finalResult.RemoveAt(lastIndex - 1);
                        lastIndex--;
                    }
                }
                x++;
            }



            Point p1 = finalResult[0];
            int i = 1;
            for (; i < finalResult.Count - 1;)
            {
                //This loop iterates through each point in finalResult starting from the second point.
                Point p2 = finalResult[i];

                //removes any collinear points and points that lie on the same segment.

                //Checks if the current point p2 lies on the line segment formed by points p1 and finalResult[i + 1].
                bool onsegment = HelperMethods.PointOnSegment(p1, p2, finalResult[i + 1]);
                Enums.TurnType t = HelperMethods.CheckTurn(p2, finalResult[i + 1]);

                if (t == Enums.TurnType.Colinear || onsegment)
                {
                    //If the points are collinear or lie on the same line segment,
                    //it removes the middle point(finalResult[i - 1]) from the convex hull.
                    finalResult.Remove(finalResult[i - 1]);
                }
                ++i;
            }


            //the output points are in ascending order.
            finalResult = finalResult.OrderBy(point => point.Y).ToList();
            outPoints = finalResult;




        }



        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
