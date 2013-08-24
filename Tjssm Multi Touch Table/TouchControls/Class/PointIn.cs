using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace TouchFramework.ControlHandlers
{
    class PointIn
    {
        public bool pointInRect(Rectangle rect, PointF p, double angle)
        {
            bool result = false;
            Rectangle r = rect;
            float X = rect.X;
            float Y = rect.Y;

            PointF pt = new PointF();
            PointF pr = new PointF();
            
            pt.X = p.X - X;
            pt.Y = p.Y - Y;
            
            float rt = 0.01745328f; // 3.141592 / 180
            float radian = (float)angle * -1 * rt;

            float s = (float)Math.Sin(radian);
            float c = (float)Math.Cos(radian);
            
            pr.X = (pt.X * c) - (pt.Y * s) + X;
            pr.Y = (pt.X * s) + (pt.Y * c) + Y;

            result = r.Contains((int)pr.X, (int)pr.Y);

            return result;
        }

        public bool pointInPolygon(float[] polyX, float[] polyY, int polySides, PointF p)
        {
            int i, j = polySides - 1;
            bool oddNodes = false;

            for (i = 0; i < polySides; i++)
            {
                if (polyY[i] < p.Y && polyY[j] >= p.Y || polyY[j] < p.Y && polyY[i] >= p.Y)
                {
                    if (polyX[i] + (p.Y - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]) < p.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }
            return oddNodes;
        }

        public bool checkPoint(System.Windows.Point[] points, PointF p)
        {
            int polySides = 4;
            int i, j = polySides - 1;
            bool oddNodes = false;

            for (i = 0; i < polySides; i++)
            {
                if (points[i].Y < p.Y && points[j].Y >= p.Y || points[j].Y < p.Y && points[i].Y >= p.Y)
                {
                    if (points[i].X + (p.Y - points[i].Y) / (points[j].Y - points[i].Y) * (points[j].X - points[i].X) < p.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }
            return oddNodes;
        }

        public System.Windows.Point[] getCorners(FrameworkElement parent, FrameworkElement child)
        {
            var t = child.TransformToVisual(parent);
            System.Windows.Point topLeft = t.Transform(new System.Windows.Point(0, 0));
            System.Windows.Point topRight = t.Transform(new System.Windows.Point(child.ActualWidth, 0));
            System.Windows.Point bottomLeft = t.Transform(new System.Windows.Point(0, child.ActualHeight));
            System.Windows.Point bottomRight = t.Transform(new System.Windows.Point(child.ActualWidth, child.ActualHeight));

            return new System.Windows.Point[] { topLeft, topRight, bottomLeft, bottomRight };
        }
         

        public Rectangle setRect(FrameworkElement parent, FrameworkElement child)
        {
            System.Windows.Point coordinate = child.TransformToAncestor(parent).Transform(new System.Windows.Point(0, 0));
            Rectangle rect = new Rectangle((int)coordinate.X, (int)coordinate.Y, (int)child.Width, (int)child.Height);

            return rect;
        }

        public bool setPtinRect(FrameworkElement parent, FrameworkElement child, PointF p, double angle)
        {
            Rectangle rect = setRect(parent, child);
            bool result = pointInRect(rect, p, angle);
            return result;

//             var points = getCorners(parent, child);
//             Console.WriteLine("p:" + p);
//             foreach (var ptt in points)
//             {
//                 Console.WriteLine(ptt);
//             }
//             Console.WriteLine(checkPoint(points, p));
//             return checkPoint(points, p);

        }
    }
}
