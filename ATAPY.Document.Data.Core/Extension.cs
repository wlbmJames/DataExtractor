using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATAPY.Document.Data.Core
{
    internal static class Extension
    {
        public static double IntersectRatio(this Rect Rect1, Rect Rect2)
        {
            if (!Rect1.IntersectsWith(Rect2)) {
                return 0.0;
            }
            double Square1 = Rect1.GetSquare();
            double Square2 = Rect2.GetSquare();
            Rect Test = Rect1;
            Test.Intersect(Rect2);
            double IntersectSquare = Test.GetSquare();
            double Ratio = Math.Max(IntersectSquare / Square1, IntersectSquare / Square2);
            return Ratio;

        }
        public static Point GetCenterPoint(this Rect Rect)
        {
            return new Point(Rect.Left + Rect.Width / 2, Rect.Top + Rect.Height / 2);
        }
        public static List<Rect> GetIntersecions(this Rect Rect, List<Rect> RectList)
        {
            List<Rect> Ret = new List<Rect>();
            foreach (var item in RectList) {
                if (Rect.IntersectsWith(item)) {
                    Ret.Add(item);
                }
            }
            return Ret;
        }
        private static double GetSquare(this Rect Rectangle)
        {
            return Rectangle.Width * Rectangle.Height;
        }
    }
}
