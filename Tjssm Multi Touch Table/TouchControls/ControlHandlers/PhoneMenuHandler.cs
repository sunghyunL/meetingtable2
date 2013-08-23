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

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Custom logic for handling multi-touch button interation
    /// </summary>
    public class PhoneMenuHandler : ElementHandler
    {
        public PointF FirstPoint, SecondPoint;
        
        public override void TouchDown(PointF p)
        {
            //Console.WriteLine("touchDown");
            phoneMenu s = Source as phoneMenu;
            
            if (s == null) return;

            s.pMenuCon_TouchDown(p);

            base.TouchDown(p);
        }

//         public override void Drag(float x, float y)
//         {
//             //Console.WriteLine("Drag");
//             phoneMenu s = Source as phoneMenu;
//             if (s == null) return;
//             
// 
//             base.Drag(x, y);
//         }

        public override void TouchUp(PointF p)
        {
            //Console.WriteLine("touchUp");
            phoneMenu s = Source as phoneMenu;
            if (s == null) return;

            s.pMenuCon_TouchUp(p);

            base.TouchUp(p);
        }

        public override void Tap(PointF p)
        {
            //Console.WriteLine("Tab");
            phoneMenu s = Source as phoneMenu;
            if (s == null) return;

            RoutedEventArgs e = new RoutedEventArgs();
            e.RoutedEvent = Button.ClickEvent;

            System.Windows.Point coordinates1 = s.bt1.TransformToAncestor(s).Transform(new System.Windows.Point(0, 0));
            System.Windows.Point coordinates2 = s.bt2.TransformToAncestor(s).Transform(new System.Windows.Point(0, 0));
            System.Windows.Point coordinates3 = s.bt3.TransformToAncestor(s).Transform(new System.Windows.Point(0, 0));
            
            Rectangle r1 = new Rectangle((int)coordinates1.X, (int)coordinates1.Y, (int)s.bt1.Width, (int)s.bt1.Height);
            Rectangle r2 = new Rectangle((int)coordinates2.X, (int)coordinates2.Y, (int)s.bt2.Width, (int)s.bt2.Height);
            Rectangle r3 = new Rectangle((int)coordinates3.X, (int)coordinates3.Y, (int)s.bt3.Width, (int)s.bt3.Height);

            PointIn ptIn = new PointIn();

//             if (ptIn.pointInRect(r1, p, s.thisAngle + s.thisCont.RotateFilter.Target))
//             {
//                 s.bt1.RaiseEvent(e);
//             }
//             else if (ptIn.pointInRect(r2, p, s.thisAngle + s.thisCont.RotateFilter.Target))
//             {
//                 s.bt2.RaiseEvent(e);
//             }
//             else if (ptIn.pointInRect(r3, p, s.thisAngle + s.thisCont.RotateFilter.Target))
//             {
//                 s.bt3.RaiseEvent(e);
//             }

            if (ptIn.setPtinRect(s.window, s.bt1, s.thisCont.ObjectTouches.MoveCenter, s.thisAngle + s.thisCont.RotateFilter.Target))
            {
                Console.WriteLine("bt1:");
                s.bt1.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.bt2, s.thisCont.ObjectTouches.MoveCenter, s.thisAngle + s.thisCont.RotateFilter.Target))
            {
                Console.WriteLine("bt2:");
                s.bt2.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.bt3, s.thisCont.ObjectTouches.MoveCenter, s.thisAngle + s.thisCont.RotateFilter.Target))
            {
                Console.WriteLine("bt3:");
                s.bt3.RaiseEvent(e);
            }

            base.Tap(p);
        }
    }
}
