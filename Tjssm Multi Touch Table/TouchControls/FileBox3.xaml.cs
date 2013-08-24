using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using TouchFramework.ControlHandlers.Class;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// FileBox3.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileBox3 : UserControl
    {
        public string userIP = "";

        public Canvas main;
        public Window window;
        public MTSmoothContainer thisCont = null;

        public string mode = "MyBox";
        public string transMode = "Mobile";
        public bool filter_img = false;
        public bool filter_video = false;
        public bool filter_ppt = false;
        public bool filter_doc = false;
        public bool filter_all = false;

        public bool IsSelecting = false;
        public bool IsMoving = false;
        public bool IsRotating = false;
        public bool firstAng = false;

        string ResourcePath = SingleTonePath.getInstance().resourcesPath;
        string path = SingleTonePath.getInstance().fileBoxPath;
        string currentPath = SingleTonePath.getInstance().fileBoxPath;
        string thisPath = "";
            
        public PointF FirstPoint, SecondPoint, CenterPoint;
        public PointF thisPosition;
        public double thisAngle;

        public RotateTransform rotateTrans = new RotateTransform();

        FileBoxObject fileBoxObject;
        public List<ObjectInfo> objects = new List<ObjectInfo>();
        public List<FileBoxItem> itemList = new List<FileBoxItem>();

        private int _count = 0;

        FrameworkControl framework = null;
        
        public FileBox3()
        {
            InitializeComponent();

            img_img.Visibility = Visibility.Hidden;
            img_video.Visibility = Visibility.Hidden;
            img_ppt.Visibility = Visibility.Hidden;
            img_doc.Visibility = Visibility.Hidden;
        }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, MTSmoothContainer cont, double angle, string _ip)
        {
            main = _main;
            window = target;
            framework = fw;
            thisCont = cont;
            thisAngle = angle;
            userIP = _ip;

            thisPath = path + userIP + "\\";
            currentPath += userIP + "\\MyBox\\";

            makeDir();
            setListBox(path + "MyBox\\");
        }

        public void makeDir()
        {            
            path += userIP + "\\";
            DirectoryInfo drinfo = new DirectoryInfo(path);
            DirectoryInfo drinfo_myBox = new DirectoryInfo(path + "MyBox\\");
            DirectoryInfo drinfo_Mobile = new DirectoryInfo(path + "Mobile\\");
            DirectoryInfo drinfo_PPT = new DirectoryInfo(path + "PPT\\");

            if (drinfo.Exists == false)
            {
                drinfo.Create();
                drinfo_myBox.Create();
                drinfo_Mobile.Create();
                drinfo_PPT.Create();
            }
        }

        public void setListBox(string _path)
        {
            itemListBox.ItemsSource = null;
            itemList.Clear();
            //getDirectory(_path);
            getFileInDirectoryE(_path);
            itemListBox.ItemsSource = itemList;
            itemListBox.InvalidateVisual();
        }

        public void updateListBox()
        {
            setListBox(currentPath);
        }

        #region 버튼 클릭 이벤트


        private void mode_bt_left_Click(object sender, RoutedEventArgs e)
        {
            if (mode != "USB")
            {
                img_mode.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_USB.png", UriKind.Absolute));
                mode = "USB";
            }
        }

        private void mode_bt_right_Click(object sender, RoutedEventArgs e)
        {
            if (mode != "MyBox")
            {
                img_mode.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_MyBox.png", UriKind.Absolute));
                mode = "MyBox";
                currentPath = path + "\\MyBox\\";
                setListBox(currentPath);
            }
            Console.WriteLine("mode_bt_right_Click");
        }

        private void mode_bt_up_Click(object sender, RoutedEventArgs e)
        {
            if (mode != "Mobile")
            {
                img_mode.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_Mobile.png", UriKind.Absolute));
                mode = "Mobile";
                currentPath = path + "\\Mobile\\";
                setListBox(currentPath);
            }
            Console.WriteLine("mode_bt_up_Click");
        }

        private void mode_bt_center_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("mode_bt_center_Click");
        }

        private void bt_trans_Click(object sender, RoutedEventArgs e)
        {
            if (transMode == "Mobile")
            {
                img_trans.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_icon2.png", UriKind.Absolute));
                transMode = "MyBox";
            }
            else if (transMode == "MyBox")
            {
                img_trans.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_icon3.png", UriKind.Absolute));
                transMode = "USB";
            }
            else if (transMode == "USB")
            {
                img_trans.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_icon1.png", UriKind.Absolute));
                transMode = "Mobile";
            }
        }

        private void mode_bt1_img_Click(object sender, RoutedEventArgs e)
        {
            if (filter_img == true)
            {
                img_img.Visibility = Visibility.Hidden;
                filter_img = false;
            }
            else
            {
                img_img.Visibility = Visibility.Visible;
                filter_img = true;
            }

            setListBox(currentPath);
        }

        private void mode_bt2_video_Click(object sender, RoutedEventArgs e)
        {
            if (filter_video == true)
            {
                img_video.Visibility = Visibility.Hidden;
                filter_video = false;
            }
            else
            {
                img_video.Visibility = Visibility.Visible;
                filter_video = true;
            }

            setListBox(currentPath);
        }

        private void mode_bt3_ppt_Click(object sender, RoutedEventArgs e)
        {
            if (filter_ppt == true)
            {
                img_ppt.Visibility = Visibility.Hidden;
                filter_ppt = false;
            }
            else
            {
                img_ppt.Visibility = Visibility.Visible;
                filter_ppt = true;
            }

            setListBox(currentPath);
        }

        private void mode_bt4_doc_Click(object sender, RoutedEventArgs e)
        {
            if (filter_doc == true)
            {
                img_doc.Visibility = Visibility.Hidden;
                filter_doc = false;
            }
            else
            {
                img_doc.Visibility = Visibility.Visible;
                filter_doc = true;
            }

            setListBox(currentPath);
        }

        private void mode_bt_close_Click(object sender, RoutedEventArgs e)
        {
            main.Children.Remove(this);
            framework.UnregisterElement(thisCont.Id);
            thisCont.isRemoved = true;
        }

        #endregion

        public void Move_TouchDown(PointF p)
        {
            IsMoving = true;
        }
        
        public void Rotate_TouchDown(PointF p)
        {
            IsRotating = true;
        }

        public void Item_TouchDown(PointF p, ObjectInfo objInfo)
        {            
            //절대 좌표로 변경
            PointF globalPt = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);

            IsSelecting = true;

            fileBoxObject = new FileBoxObject(window, main, objInfo);
            main.Children.Add(fileBoxObject);
            
            fileBoxObject.setPosition(globalPt, thisAngle + thisCont.RotateFilter.Target);
        }

        public void Item_TouchMove(PointF p, int state)
        {
            //절대 좌표로 변경
            PointF globalPt = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);

            if (IsSelecting == true)
            {
                if (state == 1)
                {
                    fileBoxObject.img_send.Visibility = Visibility.Visible;
                }
                else
                {
                    fileBoxObject.img_send.Visibility = Visibility.Hidden;
                }
                fileBoxObject.setPosition(globalPt, thisAngle + thisCont.RotateFilter.Target);
                
            }
            else if (IsMoving == true)
            {
                thisCont.SetPosition(globalPt.X - ((float)fileBoxCon.Width / 2) + (float)bt_move.Width,
                    globalPt.Y - ((float)fileBoxCon.Height / 2) + (float)bt_move.Height,
                    thisAngle + thisCont.RotateFilter.Target, 1.0);

            }
            else if (IsRotating == true)
            {
//                 System.Windows.Point tmpt = bt_rotate.TransformToAncestor(window).Transform(new System.Windows.Point(0, 0));
//                 CenterPoint.X = (float)tmpt.X + (float)(bt_rotate.Width / 2);
//                 CenterPoint.Y = (float)tmpt.Y + (float)(bt_rotate.Height / 2);
// 
//                 PointF DistanceVector = new PointF((globalPt.X - CenterPoint.X), (CenterPoint.Y - globalPt.Y));
// 
//                 double CurrentAngle = Math.Atan2(DistanceVector.Y, DistanceVector.X) * (180 / Math.PI);
// 
//                 if (!firstAng)
//                 {
//                     // 처음 각도
//                     thisAngle = CurrentAngle + rotateTrans.Angle;
//                     firstAng = true;
//                 }
//                 // 현재 각도
//                 double AngleDiff = (thisAngle - CurrentAngle + 360) % 360;
//                 rotateTrans.Angle = AngleDiff;
// 
//                 //fileBoxCon.RenderTransform = rotateTrans;
//                 thisCont.SetPosition(CenterPoint.X, CenterPoint.Y,
//                     AngleDiff, 1.0);
//                 //thisCont.RotateFileBox(AngleDiff, globalPt, CenterPoint);
            }
        }

        public void Item_TouchUp(PointF p)
        {
            //절대 좌표로 변경
            PointF globalPt = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);

            if (IsSelecting == true)
            {
                IsSelecting = false;
                main.Children.Remove(fileBoxObject);

                if (fileBoxObject.fileType == "image")
                {
                    ImageView imgView = new ImageView();
                    main.Children.Add(imgView);

                    ElementProperties imgViewProp = new ElementProperties();
                    imgViewProp.ElementSupport.AddSupportForAll();

                    MTSmoothContainer imgViewCont = new MTSmoothContainer(imgView, main, imgViewProp);
                    imgViewCont.SetPosition(globalPt.X, globalPt.Y,
                        thisAngle + thisCont.RotateFilter.Target, 1.0);

                    framework.RegisterElement(imgViewCont);

                    imgView.setInit(main, window, framework, imgViewCont, thisAngle + thisCont.RotateFilter.Target, fileBoxObject.objInfo);
                }
                else if (fileBoxObject.fileType == "ppt")
                {
                    PptViewer pptViewer = new PptViewer();
                    main.Children.Add(pptViewer);

                    ElementProperties pptViewerProp = new ElementProperties();
                    pptViewerProp.ElementSupport.AddSupportForAll();

                    MTSmoothContainer pptViewerCont = new MTSmoothContainer(pptViewer, main, pptViewerProp);
                    pptViewerCont.SetPosition(globalPt.X, globalPt.Y,
                        thisAngle + thisCont.RotateFilter.Target, 1.0);

                    framework.RegisterElement(pptViewerCont);

                    pptViewer.setInit(main, window, framework, pptViewerCont, thisAngle + thisCont.RotateFilter.Target, fileBoxObject.objInfo);                
                }
                else if (fileBoxObject.fileType == "video")
                {
                    VideoControl videoCon = new VideoControl();
                    System.Windows.Shapes.Rectangle i = videoCon.SetVideo(fileBoxObject.objInfo.FilePath);

                    ElementProperties vprop = new ElementProperties();
                    vprop.ElementSupport.AddSupportForAll();

                    MTSmoothContainer cont = new MTSmoothContainer(videoCon, main, vprop);
                    cont.SetPosition(globalPt.X, globalPt.Y,
                        thisAngle + thisCont.RotateFilter.Target, 1.0);

                    framework.RegisterElement(cont);

                    main.Children.Add(videoCon);
// 
//                     cont.MaxX = (int)(this.screen_width);
//                     cont.MaxY = (int)(this.screen_height);
//                     cont.MinX = (int)(this.screen_width / 10);
//                     cont.MinY = (int)(this.screen_height / 10);
                }
                else if (fileBoxObject.fileType == "doc")
                {
                    DocViewer dv = new DocViewer();
                    
                    ElementProperties vprop = new ElementProperties();
                    vprop.ElementSupport.AddSupportForAll();

                    MTSmoothContainer cont = new MTSmoothContainer(dv, main, vprop);
                    cont.SetPosition(globalPt.X, globalPt.Y,
                        thisAngle + thisCont.RotateFilter.Target, 1.0);
                    framework.RegisterElement(cont);

                    dv.setInit(main, window, framework, cont, thisAngle + thisCont.RotateFilter.Target, fileBoxObject.objInfo);                
                
                    main.Children.Add(dv);
                    // 
                    //                     cont.MaxX = (int)(this.screen_width);
                    //                     cont.MaxY = (int)(this.screen_height);
                    //                     cont.MinX = (int)(this.screen_width / 10);
                    //                     cont.MinY = (int)(this.screen_height / 10);
                }
            }
            if (IsMoving == true)
            {
                IsMoving = false;
            }
            if (IsRotating == true)
            {
                IsRotating = false;
                firstAng = false;
            }
        }

        public void main_TouchUp(PointF p)
        {
            if (IsSelecting == true)
            {
                IsSelecting = false;
                main.Children.Remove(fileBoxObject);
            }

            if (IsMoving == true)
            {
                IsMoving = false;
            }
            if (IsRotating == true)
            {
                IsRotating = false;
                firstAng = false;
            }
        }

        public void trans_TouchUp(PointF p)
        {
            if (IsSelecting == true)
            {
                IsSelecting = false;
                main.Children.Remove(fileBoxObject);

                if (transMode == "Mobile")
                {

                }
                else if (transMode == "MyBox")
                {
                    FileInfo fileinfo = new FileInfo(fileBoxObject.objInfo.FilePath);
                    FileInfo fileinfo2 = new FileInfo(thisPath + "MyBox\\" + fileBoxObject.objInfo.FileName);

                    if (fileinfo2.Exists == false)
                    {
                        fileinfo.CopyTo(thisPath + "MyBox\\" + fileBoxObject.objInfo.FileName, false);
                        updateListBox();
                    }                    
                }
                else if (transMode == "USB")
                {

                }
            }

            if (IsMoving == true)
            {
                IsMoving = false;
            }
            if (IsRotating == true)
            {
                IsRotating = false;
                firstAng = false;
            }
        }

        public void removeChild(FrameworkElement fe)
        {
            main.Children.Remove(fe);
            fe = new FrameworkElement();
        }


        void setItem(ObjectInfo objInfo)
        {
            FileBoxItem fbi = new FileBoxItem(objInfo);
            itemList.Add(fbi);
        }

        void getDirectory(string path)
        {
            DirectoryInfo Info = new DirectoryInfo(path);            

            if (Info.Exists)
            {
                DirectoryInfo[] CInfo = Info.GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly);

                foreach (DirectoryInfo info in CInfo)
                {
                    string str = info.FullName;// + Environment.NewLine;

                    ObjectInfo id = new ObjectInfo()
                    {
                        UserName = userIP,
                        Path = ResourcePath + "Icon_folder.png",
                        FileName = info.Name,
                        Extension = "Folder",
                        Cnt = _count++
                    };

                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.UriSource = new Uri(id.Path, UriKind.Absolute);
                    img.EndInit();
                    id.Width = img.PixelWidth;
                    id.Height = img.PixelHeight;

                    // I couldn't find file size in BitmapImage
                    id.Size = 0;
                    //Console.WriteLine("File Name : " + id.FileName);
                    objects.Add(id);

                    getFileInDirectoryE(str);
                    getDirectory(str);
                }
            }
        }

        void getFileInDirectoryE(string dirPath)
        {
            string[] supportedExtensions = new[] { ".bmp", ".jpeg", ".jpg", ".png", ".tiff", ".pptx", ".ppt", ".doc" , ".docx", ".avi"};
            var files = Directory.GetFiles(dirPath, "*.*").Where(s => supportedExtensions.Contains(System.IO.Path.GetExtension(s).ToLower()));
            filter_all = false;

            if (filter_img == false && filter_ppt == false && filter_doc == false && filter_video == false)
            {
                filter_all = true;
            }
            //Console.WriteLine("DIR Path : " + dirPath);

            foreach (var file in files)
            {
                string filePath = "";
                string imgPath = "";
                string fileType = "";
                bool nextFile = false;

                if (System.IO.Path.GetExtension(file) == ".bmp" || System.IO.Path.GetExtension(file) == ".jpeg" || System.IO.Path.GetExtension(file) == ".jpg" || System.IO.Path.GetExtension(file) == ".png" || System.IO.Path.GetExtension(file) == ".tiff")
                {
                    if (filter_all == true || filter_img == true)
                    {
                        imgPath = file;
                        filePath = file;
                        fileType = "image";
                    }
                    else
                    {
                        nextFile = true;
                    }
                }
                else if (System.IO.Path.GetExtension(file) == ".ppt" || System.IO.Path.GetExtension(file) == ".pptx")
                {
                    if (filter_all == true || filter_ppt == true)
                    {
                        imgPath = ResourcePath + "Icon_ppt.png";
                        filePath = file;
                        fileType = "ppt";
                    }
                    else
                    {
                        nextFile = true;
                    }
                }
                else if (System.IO.Path.GetExtension(file) == ".doc" || System.IO.Path.GetExtension(file) == ".docx")
                {
                    if (filter_all == true || filter_doc == true)
                    {
                        imgPath = ResourcePath + "Icon_doc.png";
                        filePath = file;
                        fileType = "doc";
                    }
                    else
                    {
                        nextFile = true;
                    }
                }
                else if (System.IO.Path.GetExtension(file) == ".avi")
                {
                    if (filter_all == true || filter_video == true)
                    {
                        imgPath = ResourcePath + "Icon_video.png";
                        filePath = file;
                        fileType = "video";
                    }
                    else
                    {
                        nextFile = true;
                    }
                }
                else
                {
                    if (filter_all == true)
                    {
                        imgPath = ResourcePath + "Icon_file.png";
                        filePath = file;
                        fileType = "file";
                    }
                }

                if (nextFile == false)
                {
                    ObjectInfo id = new ObjectInfo()
                    {
                        UserName = userIP,
                        Name = System.IO.Path.GetFileNameWithoutExtension(file),
                        DirPath = dirPath,
                        Path = imgPath,
                        Type = fileType,
                        FilePath = filePath,
                        FileName = System.IO.Path.GetFileName(file),
                        Extension = System.IO.Path.GetExtension(file),
                        Cnt = _count++
                    };

                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.UriSource = new Uri(imgPath, UriKind.Absolute);
                    img.EndInit();
                    id.Width = img.PixelWidth;
                    id.Height = img.PixelHeight;

                    // I couldn't find file size in BitmapImage
                    FileInfo fi = new FileInfo(file);
                    id.Size = fi.Length;
                    //Console.WriteLine("File Name : " + id.FileName);
                    //objects.Add(id);
                    setItem(id);
                    //Consdlwldole.WriteLine("File Name : " + id.Extension);
                }
            }
        }

        void getFileInDirectory(string path)
        {
            DirectoryInfo drinfo = new DirectoryInfo(path);

            foreach (FileInfo fiin in drinfo.GetFiles())
            {
                string str = fiin.Name;
                //MessageBox.Show(str);
                //listView1.Items.Add("  " + str);
            }
        }
    }
}
