using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {


        //if there is only one point in the input.If so, it assigns that point to the outputPoints list and returns.
        public void BaseCheck(List<Point> inputPoints, List<Point> outputPoints)
        {
            if (inputPoints.Count == 1)
            {
                outputPoints = inputPoints;
                return;
            }
        }
        //return the leftmost and rightmost points from a given list of points.
        private Point GetMostLeftPoint(List<Point> points)
        {
            if (true)
            {
                return points.OrderByDescending(point => point.X).ThenByDescending(point => point.Y).FirstOrDefault();
            }

        }
        private Point GetMostRightPoint(List<Point> points)
        {
            if (true)
            {
                return points.OrderBy(point => point.X).ThenBy(point => point.Y).FirstOrDefault();
            }

        }
        //creates a line segment between two given points.
        private Line CreateLine(Point startPoint, Point endPoint)
        {
            if (true)
            {
                return new Line(startPoint, endPoint);
            }

        }
        //The Merge method takes two lists of points(left and right) and merges them to form a single list
        //representing the Convex Hull.It calculates the upper and lower supporting lines
        //by iteratively adjusting the supporting points until they cover all the points in the subsets.
        // ana 3andy 2 points kol wa7da fe polygon mo5tlf 
        private List<Point> Merge(List<Point> left, List<Point> right)
        {
            bool up = true;
            bool down = true;

            Point leftMostPoint = GetMostLeftPoint(left);
            Point rightMostPoint = GetMostRightPoint(right);


            // Up Supporting line
            Point UpSupportinglineOnLeft = leftMostPoint;
            Point UpSupportinglineOnRight = rightMostPoint;
            bool rightChange, leftChange;

            while (true)
            {
                rightChange = leftChange = false;
                //draw line between both and check turn type with the next left point if it the turn is right take the next point in the left 
                while (CGUtilities.HelperMethods.CheckTurn(CreateLine(UpSupportinglineOnRight, UpSupportinglineOnLeft), GetNextOrPrevPoint(UpSupportinglineOnLeft, left, true)) == Enums.TurnType.Right)
                {
                    if (up)
                    {
                        UpSupportinglineOnLeft = GetNextOrPrevPoint(UpSupportinglineOnLeft, left, true);
                    }
                    leftChange = true;
                }

                if (!leftChange && CGUtilities.HelperMethods.CheckTurn(CreateLine(UpSupportinglineOnRight, UpSupportinglineOnLeft), GetNextOrPrevPoint(UpSupportinglineOnLeft, left, true)) == Enums.TurnType.Colinear)
                    UpSupportinglineOnLeft = GetNextOrPrevPoint(UpSupportinglineOnLeft, left, true);

                while (CGUtilities.HelperMethods.CheckTurn(CreateLine(UpSupportinglineOnLeft, UpSupportinglineOnRight), GetNextOrPrevPoint(UpSupportinglineOnRight, right, false)) == Enums.TurnType.Left)
                {
                    if (up)
                    {
                        UpSupportinglineOnRight = GetNextOrPrevPoint(UpSupportinglineOnRight, right, false);
                    }

                    rightChange = true;
                }

                if (!rightChange && CGUtilities.HelperMethods.CheckTurn(CreateLine(UpSupportinglineOnLeft, UpSupportinglineOnRight), GetNextOrPrevPoint(UpSupportinglineOnRight, right, false)) == Enums.TurnType.Colinear)
                    UpSupportinglineOnRight = GetNextOrPrevPoint(UpSupportinglineOnRight, right, false);

                if (!rightChange && !leftChange)
                    break;
            }

            // Down Supporting Line
            Point DownSupportinglineOnLeft = leftMostPoint;
            Point DownSupportinglineOnRight = rightMostPoint;

            while (true)
            {
                rightChange = leftChange = false;

                while (CGUtilities.HelperMethods.CheckTurn(CreateLine(DownSupportinglineOnRight, DownSupportinglineOnLeft), GetPrevPoint(DownSupportinglineOnLeft, left)) == Enums.TurnType.Left)
                {
                    if (down)
                    {
                        DownSupportinglineOnLeft = GetNextOrPrevPoint(DownSupportinglineOnLeft, left, false);

                    }

                    leftChange = true;
                }

                if (!leftChange && CGUtilities.HelperMethods.CheckTurn(CreateLine(DownSupportinglineOnRight, DownSupportinglineOnLeft), GetPrevPoint(DownSupportinglineOnLeft, left)) == Enums.TurnType.Colinear)
                    DownSupportinglineOnLeft = GetNextOrPrevPoint(DownSupportinglineOnLeft, left, false);

                while (CGUtilities.HelperMethods.CheckTurn(CreateLine(DownSupportinglineOnLeft, DownSupportinglineOnRight), GetNextPoint(DownSupportinglineOnRight, right)) == Enums.TurnType.Right)
                {
                    if (down)
                    {
                        DownSupportinglineOnRight = GetNextPoint(DownSupportinglineOnRight, right);
                    }

                    rightChange = true;
                }

                if (!rightChange && CGUtilities.HelperMethods.CheckTurn(CreateLine(DownSupportinglineOnLeft, DownSupportinglineOnRight), GetNextPoint(DownSupportinglineOnRight, right)) == Enums.TurnType.Colinear)
                    DownSupportinglineOnRight = GetNextOrPrevPoint(DownSupportinglineOnRight, right, true);

                if (!rightChange && !leftChange)
                    break;
            }


            if (true)
            {
                List<Point> resultPoints = GenerateMergedPolygon(UpSupportinglineOnLeft, UpSupportinglineOnRight, DownSupportinglineOnLeft, DownSupportinglineOnRight, left, right);
                return resultPoints;
            }



        }




        private Point GetNextOrPrevPoint(Point currentPoint, List<Point> points, bool getNext)
        {
            int currentIndex = points.FindIndex(p => p.Equals(currentPoint));

            if (getNext)
            {
                currentIndex = (currentIndex + 1) % points.Count;
            }
            else
            {

                currentIndex = (currentIndex - 1 + points.Count) % points.Count;

                currentIndex = (currentIndex - 1 + points.Count) % points.Count;
            }

            return points[currentIndex];
        }

        private List<Point> DivideAndConquerAlgorithm(List<Point> points)
        {
            if (points.Count < 2)
            {
                return points;
            }

            int middleIndex = points.Count / 2;

            List<Point> left = DivideAndConquerAlgorithm(points.GetRange(0, middleIndex));
            List<Point> right = DivideAndConquerAlgorithm(points.GetRange(middleIndex, points.Count - middleIndex));

            return Merge(left, right);
        }
        //takes a list of points as input and recursively divides the points into smaller subsets
        //until the base case is reached(when there are only one or two points).
        //Then, it merges the smaller subsets to obtain the final Convex Hull.
        public override void Run(List<Point> inputPoints, List<Line> lines, List<Polygon> polygons, ref List<Point> outputPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            BaseCheck(inputPoints, outputPoints);
            inputPoints = inputPoints.OrderBy(point => point.Y).ToList();
            inputPoints = inputPoints.OrderBy(point => point.X).ToList();
            outputPoints = DivideAndConquerAlgorithm(inputPoints);
        }

        private Point GetNextPoint(Point current, List<Point> points)
        {
            int index = points.IndexOf(current);
            return points[(index + 1) % points.Count];
        }

        private Point GetPrevPoint(Point current, List<Point> points)
        {
            int index = points.IndexOf(current);
            if (index == 0)
            {
                return points[points.Count - 1];
            }
            else
            {
                return points[points.IndexOf(current) - 1];
            }
        }


        private List<Point> GenerateMergedPolygon(Point UpSupportinglineOnLeft, Point UpSupportinglineOnRight, Point DownSupportinglineOnLeft, Point DownSupportinglineOnRight, List<Point> left, List<Point> right)
        {
            List<Point> mergedPolygon = new List<Point>();

            Point vertex = UpSupportinglineOnLeft;
            mergedPolygon.Add(vertex);

            while (!vertex.Equals(DownSupportinglineOnLeft))
            {
                vertex = GetNextOrPrevPoint(vertex, left, true);
                mergedPolygon.Add(vertex);
            }


            vertex = DownSupportinglineOnRight;
            mergedPolygon.Add(vertex);



            while (!vertex.Equals(UpSupportinglineOnRight))
            {
                vertex = GetNextOrPrevPoint(vertex, right, true);
                mergedPolygon.Add(vertex);
            }

            return mergedPolygon;
        }


        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }
    }
}
