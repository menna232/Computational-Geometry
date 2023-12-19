using CGUtilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class SweepLine : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            var endpoints = GetSortedEndpoints(lines);

            //initialized to store the currently active line segments
            var activeSegments = new List<CGUtilities.Line>();

            foreach (var endpoint in endpoints)
            {
                if (endpoint.IsLeft)
                {
                    //if it represents the left endpoint of a line segment, the segment is added to the activeSegments
                    AddSegmentToActiveList(endpoint.Segment, activeSegments);
                    //the algorithm checks for intersections between the newly added segment and the other active segments
                    CheckAndAddIntersections(endpoint.Segment, activeSegments, ref outPoints, ref outLines);
                }
                else
                {
                    RemoveSegmentFromActiveList(endpoint.Segment, activeSegments);
                }
            }
        }
        //the input list of lines and returns a sorted list of endpoints
        //Each endpoint represents the start or end point of a line segment.
        private List<Endpoint> GetSortedEndpoints(List<CGUtilities.Line> lines)
        {
            var endpoints = lines.SelectMany(line => new[]
            {
                new Endpoint(line.Start, line, true),
                new Endpoint(line.End, line, false)
            }).OrderBy(endpoint => endpoint.Point.X).ToList();
            return endpoints;
        }

        private void AddSegmentToActiveList(CGUtilities.Line segment, List<CGUtilities.Line> activeSegments)
        {
            activeSegments.Add(segment);
        }

        private void RemoveSegmentFromActiveList(CGUtilities.Line segment, List<CGUtilities.Line> activeSegments)
        {
            activeSegments.Remove(segment);
        }

        private void CheckAndAddIntersections(CGUtilities.Line segment, List<CGUtilities.Line> activeSegments, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines)
        {
            foreach (var activeSegment in activeSegments)
            {
                //checks if the current segment intersects with each active segment.
                if (IfSegmentsIntersect(segment, activeSegment))
                {
                    var intersection = ComputeIntersection(segment, activeSegment);
                    AddUniqueIntersectionPoint(intersection, ref outPoints);
                    AddUniqueIntersectionSegments(segment, activeSegment, ref outLines);
                }
            }
        }

        private void AddUniqueIntersectionPoint(CGUtilities.Point intersection, ref List<CGUtilities.Point> outPoints)
        {
            //checks if an intersection point already exists in the outPoints list before adding it. 
            if (intersection != null && !outPoints.Any(p => p.Equals(intersection)))
            {
                outPoints.Add(intersection);
            }
        }

        private void AddUniqueIntersectionSegments(CGUtilities.Line segment1, CGUtilities.Line segment2, ref List<CGUtilities.Line> outLines)
        {
            outLines.Add(segment1);
            outLines.Add(segment2);
        }

        private bool IfSegmentsIntersect(Line segment1, Line segment2)
        {
            var dir1 = HelperMethods.CheckTurn(GetVector(segment1.End, segment1.Start), GetVector(segment2.Start, segment1.Start));
            var dir2 = HelperMethods.CheckTurn(GetVector(segment1.End, segment1.Start), GetVector(segment2.End, segment1.Start));

            return (dir1 == Enums.TurnType.Right && dir2 == Enums.TurnType.Left) ||
                   (dir1 == Enums.TurnType.Left && dir2 == Enums.TurnType.Right);
        }

        private Point GetVector(Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }


        private Point ComputeIntersection(Line segment1, Line segment2)
        {
            //the segments lie on the same straight line.
            if (HelperMethods.CheckTurn(segment1, segment2.Start) == Enums.TurnType.Colinear)
            {
                return null;
            }

            //direction vectors of the two line segments
            var dir1 = GetVector(segment1.End, segment1.Start);
            var dir2 = GetVector(segment2.End, segment2.Start);
            //represents the difference between the start points of the two line segments.
            var diff = GetVector(segment2.Start, segment1.Start);

            //These parameters are used to determine the intersection point.
            var t1 = HelperMethods.CrossProduct(diff, dir2) / HelperMethods.CrossProduct(dir1, dir2);
            var t2 = HelperMethods.CrossProduct(diff, dir1) / HelperMethods.CrossProduct(dir1, dir2);

            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                //if t1 and t2 are within valid ranges.
                return CalculateIntersectionPoint(segment1, t1);
            }

            return null;
        }

        private static Point CalculateIntersectionPoint(Line segment, double t)
        {
            var intersectionX = segment.Start.X + t * (segment.End.X - segment.Start.X);
            var intersectionY = segment.Start.Y + t * (segment.End.Y - segment.Start.Y);
            return new Point(intersectionX, intersectionY);
        }




        public override string ToString()
        {
            return "Sweep Line";
        }
    }

    class Endpoint
    {
        public CGUtilities.Point Point { get; }
        public CGUtilities.Line Segment { get; }
        public bool IsLeft { get; }

        public Endpoint(CGUtilities.Point point, CGUtilities.Line segment, bool isLeft)
        {
            Point = point;
            Segment = segment;
            IsLeft = isLeft;
        }
    }
}
