﻿using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.Drawing;

using TouchFramework;
using TouchFramework.Tracking;
using TouchFramework.Events;
using TouchFramework.ControlHandlers;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// SmartArea.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmartArea : UserControl
    {
        public string userIP = "";

        public RotateTransform gridRotateTrans = new RotateTransform();
        public RotateTransform objRotateTrans = new RotateTransform();
        public TranslateTransform objTranslateTrans = new TranslateTransform();
        public ScaleTransform objScaleTrans = new ScaleTransform();
        public TransformGroup objTransGroup = new TransformGroup();

        public Button button1 = new Button();
        public Button button2 = new Button();
        public Button button3 = new Button();
        public Button button4 = new Button();

        private PointF FirstPoint, SecondPoint, pos, realPos, translate;
        private double FirstAngle;
        private bool IsRotating = false;
        private bool isCreated = false;

        //Touch FrameWork
        FrameworkControl framework = null;
        phoneMenu pMenu = new phoneMenu();
        MTSmoothContainer pMenuCont;
        ElementProperties pMenuProp = new ElementProperties();

        public Window window;
        public Canvas main;

        public SmartArea()
        {
            InitializeComponent();

            initPosition();

            objTransGroup.Children.Add(objRotateTrans);
            objTransGroup.Children.Add(objScaleTrans);
            objTransGroup.Children.Add(objTranslateTrans);

            button1 = bt1;
            button2 = bt2;
            button3 = bt3;
            button4 = bt4;

            bt1.Click += bt1_Click;
            bt2.Click += bt2_Click;
            bt3.Click += bt3_Click;
            bt4.Click += bt4_Click;
        }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, string _ip)
        {
            main = _main;
            window = target;
            framework = fw;
            userIP = _ip;
        }

        void bt1_Click(object sender, RoutedEventArgs e)
        {
            if (isCreated == false)
            {
                main.Children.Add(pMenu);

                pMenuProp.ElementSupport.AddSupportForAll();

                pMenuCont = new MTSmoothContainer(pMenu, main, pMenuProp);

                pMenuCont.SetPosition(translate.X + (float)objTranslateTrans.X, translate.Y + (float)objTranslateTrans.Y, gridRotateTrans.Angle, 1.0);
                framework.RegisterElement(pMenuCont);

                pMenu.setInit(main, window, framework, pMenuCont, gridRotateTrans.Angle, userIP);
                isCreated = true;
            }
            else
            {
                main.Children.Remove(pMenu);
                framework.UnregisterElement(pMenuCont.Id);
                pMenuCont.isRemoved = true;
                isCreated = false;
            }
        }

        void bt2_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("bt2");
        }

        void bt3_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("bt3");
                //Web web = new Web("http://www.naver.com", window, main, translate, gridRotateTrans.Angle);
                //main.Children.Add(web);
        }

        void bt4_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("bt4");
        }

        public void grid1_TouchDown(PointF p)
        {
            pos = p;
            realPos = p;
        }

        public void grid1_TouchMove(float x, float y)
        {
            SecondPoint.X = x;
            SecondPoint.Y = y;
            PointF DistanceVector = new PointF((SecondPoint.X - FirstPoint.X), (FirstPoint.Y - SecondPoint.Y));

            double CurrentAngle = Math.Atan2(DistanceVector.Y, DistanceVector.X) * (180 / Math.PI);

            if (!IsRotating)
            {
                // 처음 각도
                FirstAngle = CurrentAngle + gridRotateTrans.Angle;
                IsRotating = true;
            }
            // 현재 각도
            double AngleDiff = (FirstAngle - CurrentAngle + 360) % 360;
            gridRotateTrans.Angle = AngleDiff;
            
            grid1.RenderTransform = gridRotateTrans;
        }

        public void grid1_TouchUp(PointF p)
        {
            IsRotating = false;
            translate = p;
        }

        public void initPosition()
        {
            smartCon.RenderTransform = objTranslateTrans;

            FirstPoint.X = (float)(grid1.Width / 2);
            FirstPoint.Y = (float)(grid1.Height / 2);
        }

        public void setPosition(PointF _pos, double _rotate)
        {
            objRotateTrans.Angle = _rotate;
            objTranslateTrans.X = _pos.X - (smartCon.Width / 2);
            objTranslateTrans.Y = _pos.Y - (smartCon.Height / 2);
            smartCon.RenderTransform = objTransGroup;
        }
    }
}
