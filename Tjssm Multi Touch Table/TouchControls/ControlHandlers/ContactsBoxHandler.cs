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
using TouchFramework.ControlHandlers.Contacts;
using TouchFramework.ControlHandlers.Class;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Custom logic for handling multi-touch button interation
    /// </summary>
    public class ContactsBoxHandler : ElementHandler
    {
        public int index = -1;
        public bool IsSelecting = false;
        public string sendIP = "";
        int state = 0;

        public override void Scroll(float x, float y)
        {
            if (IsSelecting != true)
            {
                base.Scroll(x, y);
            }
        }

        public override void TouchDown(PointF p)
        {
            //Console.WriteLine("touchDown");
            ContactsBox s = Source as ContactsBox;
            
            if (s == null) return;

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            if (ptIn.setPtinRect(s.window, s.ContactslistBox, pt, ang))
            {
                isListBox = true;
                s.thisCont.IsScrollView = true;
               
                index = findSelectedItem(s, new PointF(p.X, p.Y));
                if (index != -1)
                {
                    if (ptIn.setPtinRect(s.window, s.contactsObjList.ElementAt(index).bt_img, pt, ang))
                    {
                        IsSelecting = true;
                        s.Item_TouchDown(p, s.contactsObjList.ElementAt(index));
                    }
                    else
                    {
                        IsSelecting = false;
                    }
                }
            }
            else
            {
                s.thisCont.IsScrollView = false;
                isListBox = false;
                IsSelecting = false;
            }

            base.TouchDown(p);
        }

        public override void Drag(float x, float y)
        {
            //Console.WriteLine("Drag");
            ContactsBox s = Source as ContactsBox;
            if (s == null) return;

            state = 0;
            sendIP = "";

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            foreach (SmartArea a in SingleToneTrans.getInstance().smartAreaList)
            {
                if (ptIn.setPtinRect(s.window, a, pt, ang))
                {
                    state = 1;
                    sendIP = a.userIP;
                    break;
                }
                else
                {
                    sendIP = "";
                    state = 0;
                }
            }
            s.Item_TouchMove(pt, state);

            base.Drag(x, y);
        }

        public override void TouchUp(PointF p)
        {
            //Console.WriteLine("touchUp");
            ContactsBox s = Source as ContactsBox;
            if (s == null) return;
            
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            s.Item_TouchUp(p, sendIP);

            base.TouchUp(p);
        }

        public override void Tap(PointF p)
        {
            //Console.WriteLine("Tab");
            ContactsBox s = Source as ContactsBox;
            if (s == null) return;

            RoutedEventArgs e = new RoutedEventArgs();
            e.RoutedEvent = Button.ClickEvent;
  
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            if (ptIn.setPtinRect(s.window, s.bt_close, pt, ang))
            {
                s.bt_close_Click();
            }
            base.Tap(p);
        }

        int findSelectedItem(ContactsBox s, PointF p)
        {
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            ScrollViewer scroll = s.ContactslistBox.FindChild<ScrollViewer>() as ScrollViewer;

            for (int i = 0; i < s.contactsObjList.Count; i++)
            {
                if (ptIn.setPtinRect(s.window, s.contactsObjList.ElementAt(i), pt, ang))
                {
                    return i;
                }
            }
            return -1;

        }
    }
}
