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
    public class ImageViewHandler : ElementHandler
    {
        int posState = 0;
        string _ip = "";
        FrameworkElement fe = new FrameworkElement();

        public override void Drag(float x, float y)
        {
            //Console.WriteLine("Drag");
            ImageView s = Source as ImageView;
            if (s == null) return;
            
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            //FileBox 영역 탐색
            foreach (var f in SingleToneTrans.getInstance().fileBoxList)
            {
                if (ptIn.setPtinRect(s.window, f, pt, ang))
                {
                    fe = f;
                    posState = 1;
                    s.img_cloud.Visibility = Visibility.Visible;

                    break;
                }
                else
                {
                    s.img_cloud.Visibility = Visibility.Hidden;
                    posState = 0;
                }
            }
            //SmartArea 영역 탐색
            foreach (var f in SingleToneTrans.getInstance().smartAreaList)
            {
                if (ptIn.setPtinRect(s.window, f, pt, ang))
                {
                    fe = f;
                    posState = 2;
                    s.img_cloud.Visibility = Visibility.Visible;
                    
                    break;
                }
                else if (posState != 1)
                {
                    s.img_cloud.Visibility = Visibility.Hidden;
                    posState = 0;
                }
            }
            base.Drag(x, y);
        }

        public override void TouchUp(PointF p)
        {
            //Console.WriteLine("Drag");
            ImageView s = Source as ImageView;
            if (s == null) return;

            if (posState == 1)
            {
                s.sendFile(fe, "FileBox");
            }
            else if (posState == 2)
            {
                s.sendFile(fe, "SmartArea");
            }
            s.img_cloud.Visibility = Visibility.Hidden;
            posState = 0;

            base.TouchUp(p);
        }

        public override void Tap(PointF p)
        {
            //Console.WriteLine("Tab");
            ImageView s = Source as ImageView;
            if (s == null) return;

            PointIn ptIn = new PointIn();

            if (ptIn.setPtinRect(s, s.img_bt1, p, s.thisAngle))
            {
                s.img_bt1_Click(p);
            }
            else if (ptIn.setPtinRect(s, s.img_bt2, p, s.thisAngle))
            {
                s.img_bt2_Click(p);
            }
            else
            {
                s.imageViewCon_Tap(p);
            }

            base.Tap(p);
        }
    }
}
