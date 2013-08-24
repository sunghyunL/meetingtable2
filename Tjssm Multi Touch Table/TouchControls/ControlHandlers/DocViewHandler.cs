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
    public class DocViewHandler : ElementHandler
    {
        int posState = 0;
        string _ip = "";

        public override void Drag(float x, float y)
        {
            //Console.WriteLine("Drag");
            DocViewer s = Source as DocViewer;
            if (s == null) return;

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;
            
            base.Drag(x, y);
        }

        public override void TouchUp(PointF p)
        {
            //Console.WriteLine("Drag");
            DocViewer s = Source as DocViewer;
            if (s == null) return;

            base.TouchUp(p);
        }

        public override void Scroll(float x, float y)
        {
            DocViewer s = Source as DocViewer;
            if (s.thisCont.IsScrollView == false) return;
           
            base.Scroll(x, y);
        }

        public override void TouchDown(PointF p)
        {
            //Console.WriteLine("touchDown");
            DocViewer s = Source as DocViewer;

            if (s == null) return;

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            if (ptIn.setPtinRect(s.window, s.documentviewWord, pt, ang))
            {
                isDocViewer = true;
                s.thisCont.IsScrollView = true;
            }
            else
            {
                s.thisCont.IsScrollView = false;
            }
        }

        public override void Tap(PointF p)
        {
            //Console.WriteLine("Tab");
            DocViewer s = Source as DocViewer;
            if (s == null) return;

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;
            
            if (ptIn.setPtinRect(s.window, s.bt1, pt, ang))
            {
                Console.WriteLine("bt1");
                s.bt1_Click(p);
            }
            if (ptIn.setPtinRect(s.window, s.bt2, pt, ang))
            {
                Console.WriteLine("bt2");
                s.bt2_Click(p);
            }
            if (ptIn.setPtinRect(s.window, s.bt_close, pt, ang))
            {
                Console.WriteLine("bt3");
                s.bt_close_Click(p);
            }
            else
            {
                //s.pptViewerCon_Tap(p);
            }

            base.Tap(p);
        }
    }
}
