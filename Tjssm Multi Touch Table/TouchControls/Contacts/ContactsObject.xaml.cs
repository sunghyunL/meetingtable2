﻿using System;
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
    /// ContactsObject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContactsObject : UserControl
    {
        public RotateTransform objRotateTrans = new RotateTransform();
        public TranslateTransform objTranslateTrans = new TranslateTransform();
        public TransformGroup objTransGroup = new TransformGroup();

        public Window window;
        public Canvas main;
        public ContactInfo contacts;

        FrameworkControl framework = null;
        public MTSmoothContainer thisCont = null;

        public ContactsObject(Window target, Canvas _main, ContactInfo _contacts)
        {
            contacts = _contacts;
            window = target;
            main = _main;

            InitializeComponent();

            TextName.Text = contacts.getName();
            TextNumber.Text = contacts.getNumber();

            objTransGroup.Children.Add(objRotateTrans);
            objTransGroup.Children.Add(objTranslateTrans);
        }

        public void setInit(FrameworkControl fw, MTSmoothContainer cont)
        {
            framework = fw;
            thisCont = cont;
            bt_close.Visibility = Visibility.Visible;
        }

        public void setPosition(PointF _pos, double _rotate)
        {
            // 위치 조정

            objRotateTrans.Angle = _rotate;
            objTranslateTrans.X = _pos.X - (objCon.Width / 2);
            objTranslateTrans.Y = _pos.Y - (objCon.Height / 2);

            objCon.RenderTransform = objTransGroup;
        }

        public void bt_close_Click()
        {
            main.Children.Remove(this);
            framework.UnregisterElement(thisCont.Id);
            thisCont.isRemoved = true;
        }

        public void sendContact(string ipAddress)
        {
            Console.WriteLine("sendContact : " + ipAddress + "  Name : " + contacts.getName());
        }
    }
}
