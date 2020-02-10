using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace ATAPY.Document.Data.Core
{
    internal class XpsGeometryAnalyzer
    {
        private const double MAX_LINE_THINKESS = 3.2;
        private const double LINE_LENGTH_TO_WIDTH_RATIO = 0.95;

        public static void ParsePaths(List<System.Windows.Shapes.Path> Paths, List<Rect> Rectangles, List<Rect> VerticalLines, List<Rect> HorizontallLines, double Scale)
        {
            Rectangles.Clear();
            VerticalLines.Clear();
            HorizontallLines.Clear();

            foreach (var item in Paths) {
                PathGeometry PathGeo;
                if (item.Data is StreamGeometry) {
                    PathGeo = ((StreamGeometry)item.Data).GetOutlinedPathGeometry();
                } else {
                    continue;
                }
                var RenderGeo = item.RenderedGeometry;
                if (item.RenderTransform != Transform.Identity) {
                    PathGeo.Transform = item.RenderTransform;
                    RenderGeo.Transform = item.RenderTransform;
                }

                if (PathGeo.Figures.Count == 1 && PathGeo.Figures[0].Segments.Count == 1
                        && PathGeo.Figures[0].Segments[0] is PolyLineSegment && ((PolyLineSegment)PathGeo.Figures[0].Segments[0]).Points.Count == 4) {
                    Rect AddRect = ScaleRect(PathGeo.Bounds, Scale);
                    if (!Rectangles.Contains(AddRect)) {
                        Rectangles.Add(AddRect);
                    }
                    continue;
                }
                //shall be separated to another function
                if (PathGeo.Figures.Count < 1 && item.StrokeThickness > 0.0 && item.Stroke != null) {
                    if (RenderGeo.Bounds.Height == 0.0 && RenderGeo.Bounds.Width == 0.0) {
                        continue;
                    }
                    if (RenderGeo.Bounds.Height == 0.0) {
                        Rect hLine = RenderGeo.Bounds;
                        hLine.Inflate(0.0, item.StrokeThickness / 2.0);
                        Rect AddRect = ScaleRect(hLine, Scale);
                        if (!Rectangles.Contains(AddRect)) {
                            Rectangles.Add(AddRect);
                        }
                    }
                    if (RenderGeo.Bounds.Width == 0.0) {
                        Rect vLine = RenderGeo.Bounds;
                        vLine.Inflate(item.StrokeThickness / 2.0, 0.0);
                        Rect AddRect = ScaleRect(vLine, Scale);
                        if (!Rectangles.Contains(AddRect)) {
                            Rectangles.Add(AddRect);
                        }
                    }
                }
                for (int i = 0; i < PathGeo.Figures.Count; i++) {
                    var Figure = PathGeo.Figures[i];
                    if (Figure.Segments.Count == 1) {
                        var Segment = Figure.Segments[0];
                        if (Segment is PolyLineSegment) {
                            var PoliLine = (PolyLineSegment)Segment;
                            if (PoliLine.Points.Count == 4) {
                                Trace.Assert(item.Stroke == null);
                                Rect AddRect = GetRect(PoliLine.Points, Scale);
                                if (!Rectangles.Contains(AddRect)) {
                                    Rectangles.Add(AddRect);
                                }
                            } else {
                                continue;
                            }
                        } else {
                            continue;
                        }
                    } else {
                        continue;
                    }
                }
            }
            double MaxLineThinkess = MAX_LINE_THINKESS * Scale;
            VerticalLines.AddRange(JoinVerticalLines(Rectangles, MaxLineThinkess));
            HorizontallLines.AddRange(JoinHorizontalLines(Rectangles, MaxLineThinkess));
        }
        private static List<Rect> JoinVerticalLines(List<Rect> Rectangles, double MaxLineThinkess)
        {
            List<Rect> Ret = new List<Rect>();
            var VLines = (from Rect in Rectangles
                          where Rect.Width < MaxLineThinkess && Rect.Height / Rect.Width > LINE_LENGTH_TO_WIDTH_RATIO
                          orderby Rect.Left ascending
                          select Rect).ToList();

            bool[] Used = new bool[VLines.Count];
            //search for vertical lines union
            for (int i = 0; i < VLines.Count; i++) {
                if (Used[i]) {
                    continue;
                }
                var Vertical = (from rect in VLines
                                where !Used[VLines.IndexOf(rect)] && rect.Left == VLines[i].Left// && rect.Right == VLines[i].Right
                                orderby rect.Left ascending
                                select rect).ToList();
                Trace.Assert(Vertical.Count > 0);
                Used[i] = true;
                if (Vertical.Count == 1) {
                    Ret.Add(Vertical[0]);
                    continue;
                }
                Rect JoinedRect = Vertical[0];
                for (int j = 1; j < Vertical.Count; j++) {
                    Rect Temp = Vertical[j];
                    Temp.Inflate(0.0, JoinedRect.Width * 2 + 0.1);
                    if (JoinedRect.IntersectsWith(Temp)) {
                        JoinedRect.Union(Temp);
                    } else {
                        Ret.Add(JoinedRect);
                        JoinedRect = Vertical[j];
                    }
                    Used[VLines.IndexOf(Vertical[j])] = true;
                }
                Ret.Add(JoinedRect);
            }
            Ret.Sort((x, y) => x.Left.CompareTo(y.Left));
            return Ret;
        }
        private static List<Rect> JoinHorizontalLines(List<Rect> Rectangles, double MaxLineThinkess)
        {
            List<Rect> Ret = new List<Rect>();
            var HLines = (from Rect in Rectangles
                          where Rect.Height < MaxLineThinkess && Rect.Width / Rect.Height > LINE_LENGTH_TO_WIDTH_RATIO
                          orderby Rect.Top ascending
                          select Rect).ToList(); 
            bool[] Used = new bool[HLines.Count];
            //search for vertical lines union
            for (int i = 0; i < HLines.Count; i++) {
                if (Used[i]) {
                    continue;
                }
                var Horizontal = (from rect in HLines
                                  where !Used[HLines.IndexOf(rect)] && rect.Top == HLines[i].Top //&& rect.Bottom == HLines[i].Bottom
                                  orderby rect.Top ascending
                                  select rect).ToList();
                Trace.Assert(Horizontal.Count > 0);
                Used[i] = true;
                if (Horizontal.Count == 1) {
                    Ret.Add(Horizontal[0]);
                    continue;
                }
                Rect JoinedRect = Horizontal[0];
                for (int j = 1; j < Horizontal.Count; j++) {
                    Rect Temp = Horizontal[j];
                    Temp.Inflate(Temp.Height + 0.1, 0.0);
                    if (JoinedRect.IntersectsWith(Horizontal[j])) {
                        JoinedRect.Union(Horizontal[j]);
                    } else if (JoinedRect.IntersectsWith(Temp)) {
                        JoinedRect.Union(Temp);
                    } else {
                        Ret.Add(JoinedRect);
                        JoinedRect = Horizontal[j];
                    }
                    Used[HLines.IndexOf(Horizontal[j])] = true;
                }
                JoinedRect.Inflate(JoinedRect.Height + 0.1, 0.0);
                Ret.Add(JoinedRect);
            }
            Ret.Sort((x, y) => x.Top.CompareTo(y.Top));
            return Ret;
        }        
        private static Rect ScaleRect(Rect Source, double Scale)
        {
            Rect Ret = new Rect(Source.Left * Scale, Source.Top * Scale, Source.Width * Scale, Source.Height * Scale);
            return Ret;
        }
        private static Rect GetRect(PointCollection Points, double Scale)
        {
            Point LeftTop = Points[0];
            Point RightBottom = Points[0];
            for (int i = 1; i < Points.Count; i++) {
                LeftTop.X = Math.Min(LeftTop.X, Points[i].X);
                LeftTop.Y = Math.Min(LeftTop.Y, Points[i].Y);
                RightBottom.X = Math.Max(RightBottom.X, Points[i].X);
                RightBottom.Y = Math.Max(RightBottom.Y, Points[i].Y);
            }
            //apply scale
            LeftTop.X *= Scale;
            LeftTop.Y *= Scale;
            RightBottom.X *= Scale;
            RightBottom.Y *= Scale;
            return new Rect(LeftTop, RightBottom);
        }
        /*private static Rect GetRect(string StartPoint, string[] RestPoints, double Scale)
        {
            Point[] Points = new Point[4];
            Points[0] = GetPointFromString(StartPoint);
            Point LeftTop = Points[0];
            Point RightBottom = Points[0];
            //do not check the negative values
            Trace.Assert(Points[0].X >= 0 && Points[0].Y >= 0);
            for (int i = 0; i < RestPoints.Length; i++) {
                Points[i + 1] = GetPointFromString(RestPoints[i]);
                Trace.Assert(Points[i].X >= 0 && Points[i].Y >= 0);
                LeftTop.X = Math.Min(LeftTop.X, Points[i + 1].X);
                LeftTop.Y = Math.Min(LeftTop.Y, Points[i + 1].Y);
                RightBottom.X = Math.Max(RightBottom.X, Points[i + 1].X);
                RightBottom.Y = Math.Max(RightBottom.Y, Points[i + 1].Y);
            }
            //apply scale
            LeftTop.X *= Scale;
            LeftTop.Y *= Scale;
            RightBottom.X *= Scale;
            RightBottom.Y *= Scale;
            return new Rect(LeftTop, RightBottom);
        }
        private static Point GetPointFromString(string StringPoint)
        {
            string[] StrPoint = StringPoint.Split(new char[] { ';' });
            Trace.Assert(StrPoint.Length == 2);
            return new Point(double.Parse(StrPoint[0]), double.Parse(StrPoint[1]));
        }*/
    }
}
