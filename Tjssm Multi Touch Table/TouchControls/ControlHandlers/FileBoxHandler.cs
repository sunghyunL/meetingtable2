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

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Custom logic for handling multi-touch button interation
    /// </summary>
    public class FileBoxHandler : ElementHandler
    {
        public PointF FirstPoint, SecondPoint;
        public int index = -1;
        public bool IsSelecting = false;

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
            FileBox3 s = Source as FileBox3;
            
            if (s == null) return;

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            if (ptIn.setPtinRect(s.window, s.bt_listBox, pt, ang))
            {
                isListBox = true;
               
                index = findSelectedItem(s, new PointF(p.X, p.Y));
                if (index != -1)
                {
                    if (ptIn.setPtinRect(s.window, s.itemList.ElementAt(index).img_pic, pt, ang))
                    {
                        IsSelecting = true;
                        s.Item_TouchDown(p, s.itemList.ElementAt(index).objectInfo);
                    }
                    else
                    {
                        IsSelecting = false;
                    }

                }
            }
            else if (ptIn.setPtinRect(s.window, s.bt_rotate, pt, ang))
            {
                isListBox = false;
                IsSelecting = false;

                if (ptIn.setPtinRect(s.window, s.bt_inMain, pt, ang))
                {
                }
                else
                {
                    if (ptIn.setPtinRect(s.window, s.bt_move, pt, ang))
                    {
                        s.Move_TouchDown(p);
                    }
                    else
                    {
                        s.Rotate_TouchDown(p);
                    }
                }
            }  
            else
            {
                isListBox = false;
                IsSelecting = false;
            }

            base.TouchDown(p);
        }

        public override void Drag(float x, float y)
        {
            //Console.WriteLine("Drag");
            FileBox3 s = Source as FileBox3;
            if (s == null) return;

            int state = 0;

            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            if (ptIn.setPtinRect(s.window, s.bt_trans, pt, ang))
            {
                state = 1;
            }
            else if (ptIn.setPtinRect(s.window, s.bt_main, pt, ang))
            {
                state = 2;
            }
            else
            {
                state = 0;
            }

            s.Item_TouchMove(pt, state);

            base.Drag(x, y);
        }

        public override void TouchUp(PointF p)
        {
            //Console.WriteLine("touchUp");
            FileBox3 s = Source as FileBox3;
            if (s == null) return;
            
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            if (ptIn.setPtinRect(s.window, s.bt_trans, pt, ang))
            {
                s.trans_TouchUp(p);
            }
            else if (ptIn.setPtinRect(s.window, s.bt_main, pt, ang))
            {
                s.main_TouchUp(p);
            }
            else
            {
                s.Item_TouchUp(p);
            }

            base.TouchUp(p);
        }

        public override void Tap(PointF p)
        {
            //Console.WriteLine("Tab");
            FileBox3 s = Source as FileBox3;
            if (s == null) return;

            RoutedEventArgs e = new RoutedEventArgs();
            e.RoutedEvent = Button.ClickEvent;
  
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            if (ptIn.setPtinRect(s.window, s.mode_bt_center, pt, ang))
            {
                s.mode_bt_center.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt_up, pt, ang))
            {
                s.mode_bt_up.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt_left, pt, ang))
            {
                s.mode_bt_left.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt_right, pt, ang))
            {
                s.mode_bt_right.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt1_img, pt, ang))
            {
                s.mode_bt1_img.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt2_video, pt, ang))
            {
                s.mode_bt2_video.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt3_ppt, pt, ang))
            {
                s.mode_bt3_ppt.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt4_doc, pt, ang))
            {
                s.mode_bt4_doc.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.mode_bt_close, pt, ang))
            {
                s.mode_bt_close.RaiseEvent(e);
            }
            else if (ptIn.setPtinRect(s.window, s.bt_trans, pt, ang))
            {
                s.bt_trans.RaiseEvent(e);
            }          
            base.Tap(p);
        }

        void selectItem(ListBox c, FileBoxItem item)
        {


        }

        int findSelectedItem(FileBox3 s, PointF p)
        {
            PointIn ptIn = new PointIn();
            PointF pt = s.thisCont.ObjectTouches.MoveCenter;
            double ang = s.thisAngle + s.thisCont.RotateFilter.Target;

            ScrollViewer scroll = s.itemListBox.FindChild<ScrollViewer>() as ScrollViewer;
            
            for (int i = 0; i < s.itemList.Count; i++)
            {
                if (ptIn.setPtinRect(s.window, s.itemList.ElementAt(i), pt, ang))
                {
                    return i;
                }
            }
            return -1;

        }
    }
}
