/*

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TouchFramework.Events;
using TouchFramework.ControlHandlers.Class;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Custom logic for handling multi-touch button interation
    /// </summary>
    public class PptViewHandler : ElementHandler
    {
        int posState = 0;
        string _ip = "";

        public override void Drag(float x, float y)
        {
            //Console.WriteLine("Drag");
            PptViewer s = Source as PptViewer;
            if (s == null) return;

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;


            base.Drag(x, y);
        }

        public override void TouchUp(PointF p)
        {
            //Console.WriteLine("Drag");
            PptViewer s = Source as PptViewer;


            base.TouchUp(p);
        }

        public override void Tap(PointF p)
        {
            //Console.WriteLine("Tab");
            PptViewer s = Source as PptViewer;
            if (s == null) return;

            PointIn ptIn = new PointIn();

            if (ptIn.setPtinRect(s, s.bt1, p, s.thisAngle))
            {
                s.bt1_Click(p);
            }
            else if (ptIn.setPtinRect(s, s.bt2, p, s.thisAngle))
            {
                s.bt2_Click(p);
            }
            else if (ptIn.setPtinRect(s, s.bt3, p, s.thisAngle))
            {
                s.bt3_Click(p);
            }
            else if (ptIn.setPtinRect(s, s.bt4, p, s.thisAngle))
            {
                s.bt4_Click(p);
            }
            else if (ptIn.setPtinRect(s, s.bt5, p, s.thisAngle))
            {
                s.bt5_Click(p);
            }
            else
            {
                //s.pptViewerCon_Tap(p);
            }

            base.Tap(p);
        }
    }
}
