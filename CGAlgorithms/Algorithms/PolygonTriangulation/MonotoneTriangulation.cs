using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public static int point_sort(Point a, Point b)
        {
            if (a.Y != b.Y)
                return -a.Y.CompareTo(b.Y);
            else
                return -a.X.CompareTo(b.X);
        }
        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            List<Point> SortingTriangle = new List<Point>();
            Stack<Point> PushAndPop = new Stack<Point>();

            Polygon triangle = new Polygon(lines);

            int TriangleCount = triangle.lines.Count;
            int LenghQueue = 2;
            //add point then sorting 
            for (int i = 0; i < TriangleCount; i++)
            {
                points.Add(triangle.lines[i].Start);
                SortingTriangle.Add(triangle.lines[i].Start);
            }
            SortingTriangle.Sort(point_sort);

            //push first and second point
            for (int x = 0; x <= 1; x++)
            {
                PushAndPop.Push(SortingTriangle[x]);
            }

            while (LenghQueue != TriangleCount)
            {
                Point top = PushAndPop.Peek();
                Point End = SortingTriangle[LenghQueue];
                bool OneArea = false;
                if ((End.X < SortingTriangle[0].X && top.X < SortingTriangle[0].X) || (End.X > SortingTriangle[0].X && top.X > SortingTriangle[0].X))
                {
                    //thats mean all in same site
                    OneArea = true;
                }
                else if ((End.X < SortingTriangle[0].X && top.X > SortingTriangle[0].X) || (End.X > SortingTriangle[0].X && top.X < SortingTriangle[0].X))
                {
                    //thats mean all not same site
                    OneArea = false;
                }
                // P and Top on the same side 
                if (OneArea == true)
                {
                    PushAndPop.Pop();
                    Point top2 = PushAndPop.Peek();

                    //Check the top point is convex or not
                    if (DrawLine(triangle, points.IndexOf(top)) != true)
                    {
                        PushAndPop.Push(top);
                        PushAndPop.Push(End);
                        LenghQueue++;

                    }
                    else
                    {
                        outLines.Add(new Line(End, top2));
                        if (PushAndPop.Count == 1)
                        {
                            PushAndPop.Push(End);
                            LenghQueue++;
                        }
                    }
                }
                //P and Top on different side 
                else
                {
                    while (PushAndPop.Count != 1)
                    {
                        Point top2 = PushAndPop.Pop();
                        outLines.Add(new Line(End, top2));
                    }
                    PushAndPop.Pop();
                    PushAndPop.Push(top);
                    PushAndPop.Push(End);

                }
            }
        }
        //Check Convex point 
        public bool DrawLine(Polygon p, int POINT)
        {
            int last = ((POINT - 1) + p.lines.Count) % p.lines.Count;
            int next = (POINT + 1) % p.lines.Count;

            Point p1 = p.lines[last].Start;
            Point p2 = p.lines[POINT].Start;
            Point p3 = p.lines[next].Start;
            Line l = new Line(p1, p2);
            if (HelperMethods.CheckTurn(l, p3) == Enums.TurnType.Left)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}