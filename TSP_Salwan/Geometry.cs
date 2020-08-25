using System;
using System.Collections.Generic;
using System.Drawing;

namespace TSP_Salwan
{
    class Geometry
    {

        public static Point[] g_MinMaxCorners;
        public static Rectangle g_MinMaxBox;
        public static Point[] g_NonCulledPoints;


        private static void GetMinMaxCorners(List<Point> points, ref Point ul, ref Point ur, ref Point ll, ref Point lr)
        {
  
            ul = points[0];
            ur = ul;
            ll = ul;
            lr = ul;


            foreach (Point pt in points)
            {
                if (-pt.X - pt.Y > -ul.X - ul.Y) ul = pt;
                if (pt.X - pt.Y > ur.X - ur.Y) ur = pt;
                if (-pt.X + pt.Y > -ll.X + ll.Y) ll = pt;
                if (pt.X + pt.Y > lr.X + lr.Y) lr = pt;
            }

            g_MinMaxCorners = new Point[] { ul, ur, lr, ll }; 
        }


        private static Rectangle GetMinMaxBox(List<Point> points)
        {
            // Find the MinMax quadrilateral.
            Point ul = new Point(0, 0), ur = ul, ll = ul, lr = ul;
            GetMinMaxCorners(points, ref ul, ref ur, ref ll, ref lr);


            int xmin, xmax, ymin, ymax;
            xmin = ul.X;
            ymin = ul.Y;

            xmax = ur.X;
            if (ymin < ur.Y) ymin = ur.Y;

            if (xmax > lr.X) xmax = lr.X;
            ymax = lr.Y;

            if (xmin < ll.X) xmin = ll.X;
            if (ymax > ll.Y) ymax = ll.Y;

            Rectangle result = new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
            g_MinMaxBox = result;   
            return result;
        }


        private static List<Point> HullCull(List<Point> points)
        {

            Rectangle culling_box = GetMinMaxBox(points);

            List<Point> results = new List<Point>();
            foreach (Point pt in points)
            {

                if (pt.X <= culling_box.Left ||
                    pt.X >= culling_box.Right ||
                    pt.Y <= culling_box.Top ||
                    pt.Y >= culling_box.Bottom)
                {
  
                    results.Add(pt);
                }
            }

            g_NonCulledPoints = new Point[results.Count];   
            results.CopyTo(g_NonCulledPoints);              
            return results;
        }


        public static List<Point> MakeConvexHull(List<Point> points)
        {

            points = HullCull(points);

            Point best_pt = points[0];
            foreach (Point pt in points)
            {
                if ((pt.Y < best_pt.Y) ||
                   ((pt.Y == best_pt.Y) && (pt.X < best_pt.X)))
                {
                    best_pt = pt;
                }
            }


            List<Point> hull = new List<Point>();
            hull.Add(best_pt);
            points.Remove(best_pt);


            float sweep_angle = 0;
            for (; ; )
            {

                int X = hull[hull.Count - 1].X;
                int Y = hull[hull.Count - 1].Y;
                best_pt = points[0];
                float best_angle = 3600;


                foreach (Point pt in points)
                {
                    float test_angle = AngleValue(X, Y, pt.X, pt.Y);
                    if ((test_angle >= sweep_angle) &&
                        (best_angle > test_angle))
                    {
                        best_angle = test_angle;
                        best_pt = pt;
                    }
                }

                float first_angle = AngleValue(X, Y, hull[0].X, hull[0].Y);
                if ((first_angle >= sweep_angle) &&
                    (best_angle >= first_angle))
                {

                    break;
                }


                hull.Add(best_pt);
                points.Remove(best_pt);

                sweep_angle = best_angle;

                if (points.Count == 0) break;
            }

            return hull;
        }

        private static float AngleValue(int x1, int y1, int x2, int y2)
        {
            float dx, dy, ax, ay, t;

            dx = x2 - x1;
            ax = Math.Abs(dx);
            dy = y2 - y1;
            ay = Math.Abs(dy);
            if (ax + ay == 0)
            {
                t = 360f / 9f;
            }
            else
            {
                t = dy / (ax + ay);
            }
            if (dx < 0)
            {
                t = 2 - t;
            }
            else if (dy < 0)
            {
                t = 4 + t;
            }
            return t * 90;
        }
    }
}
