﻿/*
TouchFramework connects touch tracking from a tracking engine to WPF controls 
allow scaling, rotation, movement and other multi-touch behaviours.

Copyright 2009 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of TouchFramework.

TouchFramework is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

TouchFramework is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with TouchFramework.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

using System;
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.ComponentModel;

using System.Xml;
using System.Xml.XPath;

using System.Configuration;
using System.IO;

using TouchFramework;
using TouchFramework.Tracking;
using TouchFramework.Events;
using TouchFramework.ControlHandlers;
using System.Windows.Media.Animation;
using TouchFramework.ControlHandlers.Class;
using TouchFramework.ControlHandlers.Client;

namespace Tjssm_Multi_Touch_Table
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        //--------------------------------------------*
        //시작 : 변수 선언
        //--------------------------------------------*
        private System.Windows.Point firstPoint, secondPoint;
        Window window;
        Window1 winClass;
        Canvas MainElement = new Canvas();
        Start startThread = new Start();

        //--------------------------------------------*
        //TouchFrameWork 변수 선언
        //--------------------------------------------*
        MTSmoothContainer CenterMenuCont;
        MTSmoothContainer smartPhoneCont;
        MTSmoothContainer smartAreaCont;

        //--------------------------------------------*
        //종료 : 변수 선언
        //--------------------------------------------*

        //--------------------------------------------*
        //시작 : 사용자 정의 컨트롤 선언
        //--------------------------------------------*
        bg_menu center_menu = new bg_menu();
        SmartArea _smartArea;
        //--------------------------------------------*
        //종료 : 사용자 정의 컨트롤 선언
        //--------------------------------------------*

        #region TouchFrameWork Setting

        double screen_width = SystemParameters.PrimaryScreenWidth;
        double screen_height = SystemParameters.PrimaryScreenHeight;
        double window_width = 640;
        double window_height = 480;
        double window_left = 0;
        double window_top = 0;
        int numItems = 0;
        const int MAX_ITEMS = 40;

        /// <summary>
        /// This sets the tracking mode between all available modes from TouchFrameworkTracking.
        /// NOTE: If you use Traal (Mindstorm's tracking system) you need to copy ALL the dependencies
        /// from the Dependencies folder into the Bin\Debug or Bin\Release folder.  These are the DLLs used for the
        /// Lightning tracking system.  
        /// 
        /// Traal/Mindstorm lightning is only available with Mindstorm products.
        /// </summary>
        TrackingHelper.TrackingType currentTrackingType = TrackingHelper.TrackingType.TUIO;

        bool fullscreen = false;
        static System.Random randomGen = new System.Random();
        Dictionary<int, UIElement> points = new Dictionary<int, UIElement>();
        FrameworkControl framework = null;

        #endregion

        
        public Window1()
        {
            InitializeComponent();
                        
        }

        /// <summary>
        /// Inits everything and starts the tracking engine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            framework = TrackingHelper.GetTracking(canvas1, currentTrackingType);
            framework.OnProcessUpdates += new FrameworkControl.ProcessUpdatesDelegate(this.DisplayPoints);
            framework.Start();

            window = this;  //윈도우 등록
            MainElement = canvas1;

            startThread.start();

            Jobs.Instance.setInit(canvas1, this, framework, new SmartArea());
            
            

            //if (AppConfig.StartFullscreen) toggleFullscreen();
// 
//             this.canvas1.Width = this.Width;
//             this.canvas1.Height = this.Height;
            
            takeBackground();

            addControl();
            //canvas1.Children.Add(p);
// 
//             LoadMyPictures();
//             LoadMyVideos();
//             LoadAllFeeds();
//             
            //PosAll();
            //DelayedRotate(4000);
        }

        #region UserControl 추가
        void addControl()
        {
            //Center Menu
            ElementProperties prop_c = new ElementProperties();
            prop_c.ElementSupport.AddSupport(TouchFramework.TouchAction.Tap);

            CenterMenuCont = new MTSmoothContainer(center_menu, MainElement, prop_c);
            CenterMenuCont.SetPosition((float)MainElement.ActualWidth / 2, (float)MainElement.ActualHeight / 2, 0, 1.0);
            framework.RegisterElement(CenterMenuCont);

            center_menu.setInit(MainElement, window, framework, CenterMenuCont, 0);                
                
            MainElement.Children.Add(center_menu);

            //SmartPhoneCont 
            ElementProperties prop2 = new ElementProperties();
            prop2.ElementSupport.AddSupportForAll();

            smartPhoneCont = new MTSmoothContainer(smartPhone_img, MainElement, prop2);
            framework.RegisterElement(smartPhoneCont);

            //createSmartArea();
        }

        #endregion

        public void createSmartArea(string _ip, PointF pt, double angle)
        {
            //Init SmartArea Control
            SmartArea smartArea = new SmartArea();

            ElementProperties prop = new ElementProperties();
            prop.ElementSupport.AddSupport(TouchFramework.TouchAction.Drag);
            prop.ElementSupport.AddSupport(TouchFramework.TouchAction.Tap);

            smartAreaCont = new MTSmoothContainer(smartArea, MainElement, prop);
            framework.RegisterElement(smartAreaCont);

            smartArea.Tag = _ip;
            smartAreaCont.userIP = _ip;

            MainElement.Children.Add(smartArea);
            smartArea.setInit(MainElement, window, framework, smartAreaCont, _ip, angle);

            smartAreaCont.SetPosition(pt.X, pt.Y, angle, 1.0);

            SingleToneTrans.getInstance().addToArea(smartArea);
            SingleToneTrans.getInstance().addToCont(smartAreaCont);
            _smartArea = smartArea;
        }

        public void removeSmartArea(string _ip)
        {
            //Init SmartArea Control
            SmartArea smartArea = SingleToneTrans.getInstance().getArea(_ip);
                        

            SingleToneTrans.getInstance().addToArea(smartArea);
            int _index = SingleToneTrans.getInstance().getAreaIndex(smartArea);
            MTSmoothContainer cont = SingleToneTrans.getInstance().getIndexAtCont(_index);

            MainElement.Children.Remove(smartArea);
            framework.UnregisterElement(cont.Id);
            cont.isRemoved = true;
        }

        private void smartPhone_img_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(smartPhoneCont.ObjectTouches.MoveCenter.X + " , " + smartPhoneCont.ObjectTouches.MoveCenter.Y);
            Console.WriteLine(smartPhoneCont.CenterFilter.Target.X + " , " + smartPhoneCont.CenterFilter.Target.Y);
            
            //smartPhoneCont.updatePosition();

            PointF smartPos = new PointF();
            double smartAngle = new double();

            smartPos.X = smartPhoneCont.CenterFilter.Target.X;
            smartPos.Y = smartPhoneCont.CenterFilter.Target.Y;
            smartAngle = smartPhoneCont.RotateFilter.Target;

            _smartArea.setPosition(smartPos, smartAngle);
            smartAreaCont.SetPosition(smartPos.X, smartPos.Y, smartAngle, 1.0);

            if (_smartArea.Opacity == 0)
            {
                DoubleAnimation widthAnimation = new DoubleAnimation();
                DoubleAnimation sizeAni = new DoubleAnimation();

                sizeAni.From = 0;
                sizeAni.To = 1.0;
                sizeAni.Duration = TimeSpan.FromSeconds(0.2);

                widthAnimation.From = 0;
                widthAnimation.To = 1;
                widthAnimation.Duration = TimeSpan.FromSeconds(0.2);

                ScaleTransform oTransform = new ScaleTransform();
                smartAreaCont.transforms = new TransformGroup();
                smartAreaCont.transforms.Children.Add(oTransform);
                smartAreaCont.WorkingObject.RenderTransform = smartAreaCont.transforms;

                _smartArea.BeginAnimation(System.Windows.Controls.Image.OpacityProperty, widthAnimation);
                oTransform.BeginAnimation(ScaleTransform.ScaleXProperty, sizeAni);
                oTransform.BeginAnimation(ScaleTransform.ScaleYProperty, sizeAni);
            }
            else
            {
                DoubleAnimation widthAnimation = new DoubleAnimation();
                DoubleAnimation sizeAni = new DoubleAnimation();

                sizeAni.From = 1.0;
                sizeAni.To = 0;
                sizeAni.Duration = TimeSpan.FromSeconds(0.2);

                widthAnimation.From = 1.0;
                widthAnimation.To = 0;
                widthAnimation.Duration = TimeSpan.FromSeconds(0.2);

                ScaleTransform oTransform = new ScaleTransform();
                smartAreaCont.transforms = new TransformGroup();
                smartAreaCont.transforms.Children.Add(oTransform);
                smartAreaCont.WorkingObject.RenderTransform = smartAreaCont.transforms;

                _smartArea.BeginAnimation(System.Windows.Controls.Image.OpacityProperty, widthAnimation);
                oTransform.BeginAnimation(ScaleTransform.ScaleXProperty, sizeAni);
                oTransform.BeginAnimation(ScaleTransform.ScaleYProperty, sizeAni);
            }
        }

        #region TouchFrameWork 기본 소스

        /// <summary>
        /// Displays all points from the collection of points on the screen as elipses.
        /// </summary>
        void DisplayPoints()
        {
            foreach (int i in points.Keys)
            {
                if (!framework.AllTouches.Keys.Contains(i)) canvas1.Children.Remove(points[i]);
            }
            foreach (TouchFramework.Touch te in framework.AllTouches.Values)
            {
                DisplayPoint(te.TouchId, te.TouchPoint);
            }
        }
        /// <summary>
        /// Goes through and removes all points from the screen.  I.e. all elipses created to represent touch points.
        /// </summary>
        void RemovePoints()
        {
            foreach (UIElement e in points.Values)
            {
                canvas1.Children.Remove(e);
            }
            points = new Dictionary<int, UIElement>();
        }
        /// <summary>
        /// Displays a point on the screen in the specified location, with the specified colour.
        /// </summary>
        /// <param name="id">Id of the point.</param>
        /// <param name="p">Position of the point in screen coordinates.</param>
        void DisplayPoint(int id, PointF p)
        {
            DisplayPoint(id, p, Colors.White);
        }
        /// <summary>
        /// Displays a point on the screen in the specified location, with the specified colour.
        /// </summary>
        /// <param name="id">Id of the point.</param>
        /// <param name="p">Position of the point in screen coordinates.</param>
        /// <param name="brushColor">The brush to use for the elipse.</param>
        void DisplayPoint(int id, PointF p, System.Windows.Media.Color brushColor)
        {
            Ellipse e = null;
            if (points.ContainsKey(id))
            {
                e = points[id] as Ellipse;
                e.RenderTransform = new TranslateTransform(p.X - 13, p.Y - 13);
            }

            if (e == null)
            {
                e = new Ellipse();

                RadialGradientBrush radialGradient = new RadialGradientBrush();
                radialGradient.GradientOrigin = new System.Windows.Point(0.5, 0.5);
                radialGradient.Center = new System.Windows.Point(0.5, 0.5);
                radialGradient.RadiusX = 0.5;
                radialGradient.RadiusY = 0.5;

                System.Windows.Media.Color shadow = Colors.Black;
                shadow.A = 30;
                radialGradient.GradientStops.Add(new GradientStop(shadow, 0.9));
                brushColor.A = 60;
                radialGradient.GradientStops.Add(new GradientStop(brushColor, 0.8));
                brushColor.A = 150;
                radialGradient.GradientStops.Add(new GradientStop(brushColor, 0.1));

                radialGradient.Freeze();

                e.Height = 26.0;
                e.Width = 26.0;
                e.Fill = radialGradient;

                int eZ = this.framework.MaxZIndex + 100;
                e.IsHitTestVisible = false;
                e.RenderTransform = new TranslateTransform(p.X - 13, p.Y - 13);
                canvas1.Children.Add(e);
                Panel.SetZIndex(e, eZ);
                points.Add(id, e);
            }
        }

        /// <summary>
        /// Loads all images within My Pictures special folder.
        /// </summary>
        void LoadMyPictures()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            LoadAllImages(path);
        }

        void LoadAllFeeds()
        {
            //AddFeed(@"http://twitter.com/statuses/user_timeline/72821989.rss");
            //AddFeed(@"http://twitter.com/statuses/user_timeline/816653.rss");
            //AddFeed(@"http://www.microsoft.com/switzerland/xml/socialmedia/community_twitter.xml");
        }

        /// <summary>
        /// Loads all images within a specified folder.
        /// </summary>
        /// <param name="folderName">Folder to load all images from.</param>
        void LoadAllImages(string folderName)
        {
            string[] fileNames = Directory.GetFiles(folderName);

            foreach (string fileName in fileNames)
            {
                if (IsImageExt(System.IO.Path.GetExtension(fileName)))
                {
                    if (numItems > MAX_ITEMS) break;
                    AddPhoto(fileName);
                    numItems++;
                }
            }
        }

        /// <summary>
        /// Unregisters all containers and clears the whole canvas then recreates the touch points. 
        /// </summary>
        void ClearAll()
        {
            this.framework.UnregisterAllElements();
            this.RemovePoints();
            canvas1.Children.Clear();
            DisplayPoints();
            numItems = 0;
        }

        /// <summary>
        /// Loads all videos within My Pictures special folder.
        /// </summary>
        void LoadMyVideos()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            LoadAllVideos(path);
        }

        /// <summary>
        /// Loads all images within a specified folder.
        /// </summary>
        /// <param name="folderName">Folder to load all images from.</param>
        void LoadAllVideos(string folderName)
        {
            string[] fileNames = Directory.GetFiles(folderName);
            foreach (string fileName in fileNames)
            {
                if (IsVideoExt(System.IO.Path.GetExtension(fileName)))
                {
                    if (numItems > MAX_ITEMS) break;
                    AddVideo(fileName);
                    numItems++;
                }
            }
        }

        /// <summary>
        /// Checks if a file extension is a valid image file extension
        /// </summary>
        /// <param name="ext">Extension to check if it's valid</param>
        /// <returns>True if valid false if not.</returns>
        bool IsImageExt(string ext)
        {
            string[] exts = { ".jpg", ".png", ".gif", ".tiff", ".bmp", ".jpeg" };
            return exts.Contains(ext.ToLower());
        }

        /// <summary>
        /// Checks if a file extension is a valid video file extension
        /// </summary>
        /// <param name="ext">Extension to check if it's valid</param>
        /// <returns>True if valid false if not.</returns>
        bool IsVideoExt(string ext)
        {
            string[] exts = { ".wmv", ".mpeg", ".mpg", ".avi" };
            return exts.Contains(ext.ToLower());
        }

        /// <summary>
        /// Checks if a file has the specified extension
        /// </summary>
        /// <param name="filename">Name or path to the file</param>
        /// <param name="ext">Extension to compare for</param>
        /// <returns>Whether or not the filename has the extension</returns>
        bool CheckExtension(string filename, string ext)
        {
            return (System.IO.Path.GetExtension(filename).ToLower() == ext);
        }

        /// <summary>
        /// Creates a new photo and adds it as a touch managed object to the MTElementDictionary withing the framework.
        /// Randomly positions and rotates the photo within the screen area.
        /// </summary>
        /// <param name="filePath">Full path to the image.</param>
        void AddPhoto(string filePath)
        {
            BitmapImage bi = new BitmapImage(new Uri(filePath));
            Photo p = new Photo();
            System.Windows.Controls.Image i = p.SetPicture(filePath);

            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.HighQuality);

            ElementProperties prop = new ElementProperties();
            prop.ElementSupport.AddSupportForAll();

            MTContainer cont = new MTSmoothContainer(p, canvas1, prop);
            framework.RegisterElement(cont);

            canvas1.Children.Add(p);

            cont.MaxX = (int)(this.screen_width);
            cont.MaxY = (int)(this.screen_height);
            cont.MinX = (int)(this.screen_width / 10);
            cont.MinY = (int)(this.screen_height / 10);
        }

        void AddFeed(string url)
        {
            ElementProperties prop = new ElementProperties();
            prop.ElementSupport.AddSupportForAll();

            RssList r = new RssList();
            r.Read(url);

            MTContainer cont = new MTSmoothContainer(r, canvas1, prop);
            framework.RegisterElement(cont);

            canvas1.Children.Add(r);

            cont.MaxX = (int)(this.screen_width);
            cont.MaxY = (int)(this.screen_height);
            cont.MinX = (int)(this.screen_width / 10);
            cont.MinY = (int)(this.screen_height / 10);
        }

        /// <summary>
        /// Creates a non-blocking delay using a callback to delay the rotation until the element has been positioned.
        /// Required as the center point is calculated using the built-in render location.  If you've moved the element
        /// the you need to wait for it to be rendered before WPF knows the location of the object.
        /// </summary>
        void DelayedRotate(int delayMs)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(NonBlockingDelay);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DelayedRotateCallback);
            bw.RunWorkerAsync(delayMs);
        }

        void NonBlockingDelay(object sender, DoWorkEventArgs e)
        {
            int delay = (int)e.Argument;
            Thread.Sleep(delay);
        }

        void DelayedRotateCallback(object sender, RunWorkerCompletedEventArgs e)
        {
            RotateAll();
        }

        /// <summary>
        /// Positions all the elements to random locations within the window.
        /// Uses SetTop and SetLeft and provides an example of how to position or re-position an element
        /// to an arbitary location without messing up the center point calculations.
        /// </summary>
        void PosAll()
        {
            foreach (var cont in this.framework.Assigner.Elements.Values)
            {
                // Just incase canvas is too small
                int difX = canvas1.ActualWidth > 200 ? 200 : 0;
                int difY = canvas1.ActualWidth > 200 ? 200 : 0;

                // Get random position and rotation
                int x = randomGen.Next(0, (int)canvas1.ActualWidth - difX);
                int y = randomGen.Next(0, (int)canvas1.ActualHeight - difY);

                Canvas.SetTop(cont.WorkingObject, y);
                Canvas.SetLeft(cont.WorkingObject, x);
                cont.Reset();
                cont.StartX = x;
                cont.StartY = y;
            }
        }

        /// <summary>
        /// Positions all elements registered with the framework randomly within the window area.
        /// Uses SetTop and SetLeft of the Canvas for positioning.  The timer behaviour in the MTSmoothContainer
        /// means that Move is not suitable to set initial positioning.
        /// </summary>
        void MoveAll()
        {
            foreach (var cont in this.framework.Assigner.Elements.Values)
            {
                // Just incase canvas is too small
                int difX = canvas1.ActualWidth > 200 ? 200 : 0;
                int difY = canvas1.ActualWidth > 200 ? 200 : 0;

                // Get random position and rotation
                int x = randomGen.Next(0, (int)canvas1.ActualWidth - difX);
                int y = randomGen.Next(0, (int)canvas1.ActualHeight - difY);
                Rect curPos = cont.GetElementBounds();
                x -= (int)curPos.Left;
                y -= (int)curPos.Top;

                cont.Move(x, y);
            }
        }

        /// <summary>
        /// Rotates all elements registered in the framework by a random angle between -90 and 90 degrees
        /// </summary>
        void RotateAll()
        {
            foreach (var cont in this.framework.Assigner.Elements.Values)
            {
                int a = randomGen.Next(-90, 90);
                PointF p = cont.GetElementCenter();
                cont.Rotate(a, p);
            }
        }

        /// <summary>
        /// Rotates all elements registered in the framework by the specified angle
        /// </summary>
        void RotateAll(int angle)
        {
            foreach (var cont in this.framework.Assigner.Elements.Values)
            {
                PointF p = cont.GetElementCenter();
                cont.Rotate(angle, p);
            }
        }


        /// <summary>
        /// Creates a new video and adds it as a touch managed object to the MTElementDictionary withing the framework.
        /// Randomly positions and rotates the photo within the screen area.
        /// </summary>
        /// <param name="filePath">Full path to the image.</param>
        void AddVideo(string filePath)
        {
            VideoControl p = new VideoControl();
            System.Windows.Shapes.Rectangle i = p.SetVideo(filePath);

            ElementProperties prop = new ElementProperties();
            prop.ElementSupport.AddSupportForAll();

            MTContainer cont = new MTSmoothContainer(p, canvas1, prop);
            framework.RegisterElement(cont);

            canvas1.Children.Add(p);

            cont.MaxX = (int)(this.screen_width);
            cont.MaxY = (int)(this.screen_height);
            cont.MinX = (int)(this.screen_width / 10);
            cont.MinY = (int)(this.screen_height / 10);
        }


        /// <summary>
        /// Handles key presses to clear the background etc...
        /// B = clear background
        /// Return = Full screen toggle
        /// R = Reload everything
        /// Space = Reposition elements
        /// S = Spin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.B)
            {
                takeBackground();
            }
            else if (e.Key == Key.R)
            {
                ClearAll();
                LoadMyPictures();
                LoadMyVideos();

                PosAll();
                //DelayedRotate(3000);
            }
            else if (e.Key == Key.Space)
            {
                //RotateAll();
                //MoveAll();
                PosAll();
            }
            else if (e.Key == Key.S)
            {
                RotateAll(30);
            }
            else if (e.Key == Key.W)
            {
                this.canvas1.Width = this.Width;
                this.canvas1.Height = this.Height;
            }
            else if (e.Key == Key.Return)
            {
                toggleFullscreen();
            }
        }

        void takeBackground()
        {
            framework.ForceRefresh();
        }

        void toggleFullscreen()
        {
            if (!fullscreen) switchFullScreen(); else switchWindowed();
        }

        void switchWindowed()
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.Left = window_left;
            this.Top = window_top;
            this.Width = window_width;
            this.Height = window_height;

            canvas1.Width = window_width;
            canvas1.Height = window_height;

            fullscreen = false;
        }

        void switchFullScreen()
        {
            window_left = this.Left;
            window_top = this.Top;

            window_width = this.Width;
            window_height = this.Height;

            this.Left = 0;
            this.Top = 0;
            this.Width = screen_width;
            this.Height = screen_height;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            //this.Topmost = true;

            canvas1.Width = screen_width;
            canvas1.Height = screen_height;

            fullscreen = true;
        }


        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            this.ClearAll();
            framework.Stop();
            startThread.stop();
        }
    }
}
