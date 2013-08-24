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
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using TouchFramework.ControlHandlers.Class;
using System.IO;
using System.Drawing;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Ppt_Viewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PptViewer : UserControl
    {
        ObjectInfo objInfo;
        int slideNum = 0;
        
        public Window window;
        public Canvas main;
        public string imgPath = "";

        FrameworkControl framework = null;
        public MTSmoothContainer thisCont = null;
        public PointF thisPosition;
        public double thisAngle;

        public Presentation PPT;

        Microsoft.Office.Interop.PowerPoint.Application oPPT;
        Microsoft.Office.Interop.PowerPoint.Presentations objPresSet;
        Microsoft.Office.Interop.PowerPoint.Presentation objPres;
        Microsoft.Office.Interop.PowerPoint.SlideShowView oSlideShowView;
        System.Timers.Timer slidetest;

        public PptViewer()
        {
            InitializeComponent();

            //PlayPresentation("C:/Users/tjssm/Documents/Visual Studio 2012/Projects/WpfApplication1/WpfApplication1/ppt.pptx");
            //Loaded += new RoutedEventHandler(Ppt_Viewer_loaded);


            //OpenPPT();

            //rect1.MouseDown += rect1_MouseDown;
        }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, MTSmoothContainer cont, double angle, ObjectInfo _objInfo)
        {
            main = _main;
            window = target;
            framework = fw;
            thisCont = cont;
            thisAngle = angle;

            objInfo = _objInfo;

            ReadPPTfile(objInfo.FilePath);
            MakePPTIamge(objInfo.DirPath);
            ViewPPTImage(objInfo.DirPath, 0);
        }

        #region 뷰어 컨트롤

        public void pptViewerCon_Tap(PointF p)
        {
            if (btGrid.Visibility == Visibility.Hidden)
                btGrid.Visibility = Visibility.Visible;
            else
                btGrid.Visibility = Visibility.Hidden;
        }

        public void bt1_Click(PointF p)
        {
            ViewPPTImage(objInfo.DirPath, 0);
        }

        public void bt2_Click(PointF p)
        {
            if (slideNum > 0)
            {
                ViewPPTImage(objInfo.DirPath, --slideNum);
            }
            else
            {
                slideNum = 0;
                ViewPPTImage(objInfo.DirPath, slideNum);
            }
        }

        public void bt3_Click(PointF p)
        {
            if (slideNum < PPT.Slides.Count - 1)
            {
                ViewPPTImage(objInfo.DirPath, ++slideNum);
            }
            else
            {
                ViewPPTImage(objInfo.DirPath, slideNum);
            }
        }

        public void bt4_Click(PointF p)
        {
        }

        public void bt5_Click(PointF p)
        {
            main.Children.Remove(this);
            pptImg.Source = null;

            framework.UnregisterElement(thisCont.Id);
            thisCont.isRemoved = true;
        }

        #endregion

        public void sendFile(string ip)
        {
            if (ip[0] == 'f') //파일 박스로 전송
            {

            }
            else if (ip[0] == 'm') // 모바일 폰으로 전송
            {

            }
        }

        void rect1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            oSlideShowView.Next();
        }
// 
//         private void OpenPPT()
//         {
//             //Create an instance of PowerPoint.
//             oPPT = new Microsoft.Office.Interop.PowerPoint.Application();
//             // Show PowerPoint to the user.
//             oPPT.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
//             objPresSet = oPPT.Presentations;
// 
//             //open the presentation
//             objPres = objPresSet.Open(Path, MsoTriState.msoFalse,
//             MsoTriState.msoTrue, MsoTriState.msoTrue);
// 
//             objPres.SlideShowSettings.ShowPresenterView = MsoTriState.msoFalse;
//             System.Diagnostics.Debug.WriteLine(objPres.SlideShowSettings.ShowWithAnimation);
//             objPres.SlideShowSettings.Run();
// 
//             oSlideShowView = objPres.SlideShowWindow.View;
//         }

        private void ReadPPTfile(string path)
        {
            Microsoft.Office.Interop.PowerPoint.Application app = new Microsoft.Office.Interop.PowerPoint.Application();
            PPT = app.Presentations.Open(path, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
            app.Quit();
        }
        private void MakePPTIamge(string path)
        {
            path += "\\ppt_" + objInfo.Name;
            DirectoryInfo drinfo = new DirectoryInfo(path);
            if (drinfo.Exists == false)
            {
                drinfo.Create();

                for (int i = 0; i < PPT.Slides.Count; ++i)
                {
                    PPT.Slides[i + 1].Export(string.Format("{0}\\ppt_" + objInfo.Name + "_" + "{1}.jpg", path, i), "JPG",
                         (int)PPT.Slides[i + 1].Master.Width, (int)PPT.Slides[i + 1].Master.Height);
                }
            }

        //    PPT.CreateVideo("C:\\Users\\tjssm\\Documents\\Visual Studio 2012\\Projects\\WpfApplication1\\WpfApplication1\\ppt2.wmv");
        //    PPT.SaveCopyAs(myPicturesPath, PpSaveAsFileType.ppSaveAsWMV, MsoTriState.msoCTrue);
        }

        private void ViewPPTImage(string imgPath, int cnt)
        {
            imgPath += "\\ppt_" + objInfo.Name + "\\ppt_" + objInfo.Name + "_" + cnt + ".jpg";
            
            //string imgpath = "C:\\Users\\Administrator\\Documents\\Visual Studio 2012\\Projects\\Tjssm Multi Touch Table\\Tjssm Multi Touch Table\\ppt\\temp0.jpg";

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.None;
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.UriSource = new Uri(imgPath, UriKind.Absolute);
            bi.EndInit();

            pptImg.Source = bi;

            //pptImg.Source = new BitmapImage(new Uri(imgPath, UriKind.Absolute));
        }

        private void DeletePptDir(string path)
        {
            path += "\\ppt_" + objInfo.Name;
            DirectoryInfo drinfo = new DirectoryInfo(path);

            if (drinfo.Exists == true)
            {
                drinfo.Delete(true);
            }
        }
    }
}