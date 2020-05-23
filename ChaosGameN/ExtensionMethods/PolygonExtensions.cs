using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosGameN.ExtensionMethods
{
    public static class PolygonExtensions
    {
        public static bool IsPointInPolygon(this List<PointF> polygon, PointF point)
        {
            bool result = false;
            PointF a = polygon.Last();
            foreach (PointF b in polygon)
            {
                if ((b.X == point.X) && (b.Y == point.Y))
                {
                    return true;
                }

                if ((b.Y == a.Y) && (point.Y == a.Y) && (a.X <= point.X) && (point.X <= b.X))
                {
                    return true;
                }

                if ((b.Y < point.Y) && (a.Y >= point.Y) || (a.Y < point.Y) && (b.Y >= point.Y))
                {
                    if (b.X + (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X) <= point.X)
                    {
                        result = !result;
                    }
                }
                a = b;
            }

            return result;
        }
    }
}
