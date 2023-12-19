using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public List<Point> globalinput;
        public List<Point> globaloutput;
        public static void getSmallestPointInEachDirection(ref List<Point> points, ref Point north, ref Point south, ref Point east, ref Point west)
        {
            double minY = double.MaxValue;
            double minX = double.MaxValue;
            double maxY = double.MinValue;
            double maxX = double.MinValue;
            for (int index = 0; index < points.Count; index++)
            {
                if (points[index].Y < minY)
                {
                    south = (Point)points[index].Clone();
                    minY = points[index].Y;
                }
                if (points[index].X < minX)
                {
                    west = (Point)points[index].Clone();
                    minX = points[index].X;
                }
                if (points[index].X > maxX)
                {
                    east = (Point)points[index].Clone();
                    maxX = points[index].X;

                }
                if (points[index].Y > maxY)
                {
                    north = (Point)points[index].Clone();
                    maxY = points[index].Y;
                }
            }



        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {


            int numbeerOfPoints = points.Count;
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            globalinput = points;
            globaloutput = outPoints;
            Point north = null;
            Point east = null;
            Point south = null;
            Point west = null;


            getSmallestPointInEachDirection(ref points, ref north, ref south, ref east, ref west);
            outPoints.Add(north); points.Remove(north);
            outPoints.Add(south); points.Remove(south);
            outPoints.Add(east); points.Remove(east);
            outPoints.Add(west); points.Remove(west);





            //concat the 4 exxtreme points to form a hull
            int c;
            List<Line> Hull = new List<Line>();
            Hull.Add(new Line(south, east));
            Hull.Add(new Line(east, north));
            Hull.Add(new Line(north, west));
            Hull.Add(new Line(west, south));

            for (int i = 0; i < points.Count; i++)
            {
                c = 0;

                for (int m = 0; m < 4; m++)
                {
                    //check if all the lines go o the point in the same direction 
                    if (HelperMethods.CheckTurn(Hull[m], points[i]) == Enums.TurnType.Left)
                    {
                        c++;
                    }

                    //if yes [the 4 lines of the hull then we will remove the point because it is inside the hull ]
                    if (c == 4)
                    {
                        points.RemoveAt(i);
                        i--;
                    }
                }


            }

            //south east north west
            DrawHull(south, east);
            DrawHull(east, north);
            DrawHull(north, west);
            DrawHull(west, south);
            removeDuplicatePoints(ref globaloutput);
        }
        public void DrawHull(Point a, Point b)
        {
            //the base case of the function where the function should stop when the points list is empty
            if (globalinput.Count == 0)
            {
                return;
            }
           
            Line line1 = new Line(a, b);
            Point nextPoint = null;
            double disatance;
            double maxDistance = double.MinValue;

            for (int i = 0; i < globalinput.Count; i++)
            {
                //loop over the points and find the point with the maximum distance and this will be our next point  
                if (HelperMethods.CheckTurn(line1, globalinput[i]) == Enums.TurnType.Right)
                {
                    disatance = calcEuclideanDistance(ref a, globalinput[i]) +calcEuclideanDistance(ref b, globalinput[i]);

                    if (disatance > maxDistance)
                    {

                        maxDistance = disatance;
                        nextPoint = globalinput[i];
                    }
                }
            }

            if (nextPoint != null)
            {
                globaloutput.Add(nextPoint);
                globalinput.Remove(nextPoint);


                for (int n = 0; n < globalinput.Count; n++)
                {
                    if (HelperMethods.PointInTriangle(globalinput[n], a, b, nextPoint) != Enums.PointInPolygon.Outside)
                    {
                        globalinput.Remove(globalinput[n]);
                        n--;
                    }
                }
                DrawHull(a, nextPoint);
                DrawHull(nextPoint, b);
            }


        }
        public static void removeDuplicatePoints(ref List<Point> inputPoints)
        {
            for (int pointA = 0; pointA < inputPoints.Count; pointA++)
            {
                for (int pointB = pointA + 1; pointB < inputPoints.Count; pointB++)
                {
                    if (inputPoints[pointA].Equals(inputPoints[pointB]))
                    {
                        inputPoints.RemoveAt(pointB);
                        pointB--;
                        break;
                    }
                }
            }
        }

        public static double calcEuclideanDistance(ref Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.X - a.X, 2));
        }
        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
