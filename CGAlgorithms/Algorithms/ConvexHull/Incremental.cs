using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons, ref List<Point> convexHullPoints, ref List<Line> convexHullLines, ref List<Polygon> convexHullPolygons)
        {
            int Counter = inputPoints.Count;
            if (Counter < 3)
            {
                convexHullPoints = inputPoints.ToList();
                return;
            }
            inputPoints.Sort(twoPoint);
            int[] the_next = new int[Counter];
            int[] the_prev = new int[Counter];
            int index = 1;
            while (index < inputPoints.Count && Area(inputPoints[0], inputPoints[index]))
            {
                index++;
            }
            the_next[0] = index;
            the_prev[0] = index;

            int Indexx = index; //p3
            while (index < inputPoints.Count)
            {
                Point newPoint = inputPoints[index];
                if (Area(newPoint, inputPoints[Indexx]))
                {
                    //check shorter point
                    index++;
                    continue;
                }
                if (newPoint.Y < inputPoints[Indexx].Y)
                {
                    //new point y under p3
                    //to get the half
                    the_next[index] = Indexx;
                    the_prev[index] = the_prev[Indexx];
                }
                else
                {
                    //new point y top p3
                    //to get the half
                    the_next[index] = the_next[Indexx];
                    the_prev[index] = Indexx;
                }
                //update
                the_prev[the_next[index]] = index;
                the_next[the_prev[index]] = index;
                while (true)
                {
                    Point p = inputPoints[the_next[the_next[index]]];
                    Line s = new Line(newPoint, inputPoints[the_next[index]]);
                    Enums.TurnType turn = HelperMethods.CheckTurn(s, p);

                    if (turn == Enums.TurnType.Left)
                    { //clock wise
                        break;
                    }
                    else
                    {
                        the_prev[the_next[index]] = index;
                        the_next[index] = the_next[the_next[index]];
                    }
                }
                while (true)
                {
                    Point p = inputPoints[the_prev[the_prev[index]]];
                    Line s = new Line(newPoint, inputPoints[the_prev[index]]);
                    Enums.TurnType turn = HelperMethods.CheckTurn(s, p);

                    if (turn == Enums.TurnType.Right)
                    { //clock wise
                        break;
                    }
                    else
                    {
                        the_prev[index] = the_prev[the_prev[index]];
                        the_next[the_prev[index]] = index;
                    }
                }
                Indexx = index;
                index++;
            }
            int current = 0;
            while (true)
            {
                convexHullPoints.Add(inputPoints[current]);
                current = the_next[current];
                if (current == 0)
                {
                    break;
                }
            }
        }
        //get shorter point between two
        public bool Area(Point p1, Point p2)
        {
            double x = Math.Abs(p1.X - p2.X);
            double y = Math.Abs(p1.Y - p2.Y);
            bool X = x <= Constants.Epsilon;
            bool Y = y <= Constants.Epsilon;
            return X && Y;
        }
        //sort x then y
        public int twoPoint(Point point1, Point point2)
        {
            if (point1.X < point2.X || (point1.X == point2.X && point1.Y < point2.Y))
            {
                return -1; //first point get first X
            }
            return 1;
        }
        public override string ToString() => "Incremental Convex Hull Algorithm";
    }
}
