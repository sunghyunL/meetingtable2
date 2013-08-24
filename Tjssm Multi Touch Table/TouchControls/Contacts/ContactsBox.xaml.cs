using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TouchFramework.ControlHandlers.Client;

namespace TouchFramework.ControlHandlers.Contacts
{
    /// <summary>
    /// ContactsBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContactsBox : UserControl
    {
        ArrayList list = new ArrayList();
        List<ContactsObject> contactsObjList = new List<ContactsObject>();
        ContactsObject contactsObj;

        public string userIP = "";

        public Canvas main;
        public Window window;
        public MTSmoothContainer thisCont = null;

        FrameworkControl framework = null;
        public double thisAngle;

        public bool IsSelecting = false;
        int itemState = 0;

        public ContactsBox()
        {
            InitializeComponent();
        }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, MTSmoothContainer cont, double angle, string _ip)
        {
            main = _main;
            window = target;
            framework = fw;
            thisCont = cont;
            thisAngle = angle;
            userIP = _ip;

            Jobs.Instance.call_requestContacts(_ip);
            setList(Jobs.Instance.getList());
        }

        public void setList(ArrayList conList)
        {
            list = conList;

            foreach (ContactInfo c in list)
            {
                ContactsObject con = new ContactsObject(window, main, c);
                contactsObjList.Add(con);
            }
            ContactslistBox.ItemsSource = contactsObjList;
        }

        private void mode_bt_close_Click(object sender, RoutedEventArgs e)
        {
            main.Children.Remove(this);
            framework.UnregisterElement(thisCont.Id);
            thisCont.isRemoved = true;
        }

        public void Item_TouchDown(PointF p, ContactsObject obj)
        {
            //절대 좌표로 변경
            PointF globalPt = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);

            IsSelecting = true;

            contactsObj = new ContactsObject(window, main, obj.contacts);
            main.Children.Add(contactsObj);

            contactsObj.setPosition(globalPt, thisAngle + thisCont.RotateFilter.Target);
        }

        public void Item_TouchMove(PointF p, int state)
        {
            //절대 좌표로 변경
            PointF globalPt = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);

            itemState = state;

            if (IsSelecting == true)
            {
                if (itemState == 1)
                {
                    contactsObj.img_send.Visibility = Visibility.Visible;
                }
                else
                {
                    contactsObj.img_send.Visibility = Visibility.Hidden;
                }
                contactsObj.setPosition(globalPt, thisAngle + thisCont.RotateFilter.Target);
            }
        }

        public void Item_TouchUp(PointF p)
        {
            //절대 좌표로 변경
            PointF globalPt = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);

            if (IsSelecting == true)
            {
                IsSelecting = false;
                main.Children.Remove(contactsObj);

                if (itemState == 1)
                {
                    
                }
                else if (itemState == 2)
                {

                }
                else
                {
                    ContactsObject cObj = new ContactsObject(window, main, contactsObj.contacts);
                    main.Children.Add(cObj);
                }
            }
        }
    }
}
