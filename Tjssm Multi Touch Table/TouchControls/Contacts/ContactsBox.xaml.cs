using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
        public ArrayList list = new ArrayList();
        public List<ContactsObject> contactsObjList = new List<ContactsObject>();
        public ContactsObject contactsObj;

        public string userIP = "";

        public Canvas main;
        public Window window;
        public MTSmoothContainer thisCont = null;

        FrameworkControl framework = null;
        public double thisAngle;

        public bool IsSelecting = false;
        public int itemState = 0;

        Thread t;

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

            t = new Thread(new ThreadStart(thread_recv));
            t.Start();
        }

        public void setList()
        {
            foreach (ContactInfo c in list)
            {
                ContactsObject con = new ContactsObject(window, main, c);
                contactsObjList.Add(con);
            }
            ContactslistBox.ItemsSource = contactsObjList;
        }

        public void thread_recv()
        {
            Jobs.Instance.call_requestContacts(userIP);
            list = Jobs.Instance.getList();
            setList();

            t.Abort();
        }

        public void bt_close_Click()
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
            Canvas.SetZIndex(contactsObj, 100000);
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

        public void Item_TouchUp(PointF p, string ipAddress)
        {
            //절대 좌표로 변경
            PointF globalPt = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);

            if (IsSelecting == true)
            {
                IsSelecting = false;
                main.Children.Remove(contactsObj);

                if (itemState == 1) //연락처 전송
                {
                    contactsObj.sendContact(ipAddress);
                }
                else if (itemState == 2)
                {
                }
                else
                {
                    ContactsObject cObj = new ContactsObject(window, main, contactsObj.contacts);
                    
                    ElementProperties cObjProp = new ElementProperties();
                    cObjProp.ElementSupport.AddSupportForAll();

                    MTSmoothContainer cObjCont = new MTSmoothContainer(cObj, main, cObjProp);
                    framework.RegisterElement(cObjCont);

                    cObj.setInit(framework, cObjCont);
                    
                    main.Children.Add(cObj);

                    cObjCont.SetPosition(globalPt.X, globalPt.Y, thisAngle + thisCont.RotateFilter.Target, 1.0);
                }
            }
        }
    }
}
