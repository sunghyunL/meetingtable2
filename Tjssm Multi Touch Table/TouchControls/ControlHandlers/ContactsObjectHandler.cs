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
using TouchFramework.ControlHandlers.Contacts;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Custom logic for handling multi-touch button interation
    /// </summary>
    public class ContactsObjectHandler : ElementHandler
    {
        int posState = 0;
        string ipAddress = "";

        public override void Drag(float x, float y)
        {
            //Console.WriteLine("Drag");
            ContactsObject s = Source as ContactsObject;
            if (s == null) return;
            
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            
            ipAddress = "";
            posState = 0;

            //SmartArea 영역 탐색
            foreach (SmartArea f in SingleToneTrans.getInstance().smartAreaList)
            {
                if (ptIn.setPtinRect(s.window, f, pt, f.objRotateTrans.Angle))
                {
                    ipAddress = f.userIP;
                    posState = 1;
                    s.img_send.Visibility = Visibility.Visible;
                    
                    break;
                }
                else if (posState != 1)
                {
                    s.img_send.Visibility = Visibility.Hidden;
                    posState = 0;
                }
            }
            base.Drag(x, y);
        }

        public override void TouchUp(PointF p)
        {
            //Console.WriteLine("Drag");
            ContactsObject s = Source as ContactsObject;
            if (s == null) return;

            if (posState == 1)
            {
                s.sendContact(ipAddress);
            }

            s.img_send.Visibility = Visibility.Hidden;
            posState = 0;

            base.TouchUp(p);
        }

        public override void Tap(PointF p)
        {
            //Console.WriteLine("Tab");
            ContactsObject s = Source as ContactsObject;
            if (s == null) return;

            PointIn ptIn = new PointIn();

            if (ptIn.setPtinRect(s, s.bt_close, p, s.thisCont.RotateFilter.Target))
            {
                s.bt_close_Click();
            }
            base.Tap(p);
        }
    }
}
