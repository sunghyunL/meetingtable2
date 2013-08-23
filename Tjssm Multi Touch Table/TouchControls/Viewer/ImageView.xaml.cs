using System;
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
using TouchFramework.ControlHandlers.Class;
using System.IO;
using System.Windows.Media.Animation;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// ImageView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageView : UserControl
    {
        public Window window;
        public Canvas main;
        public string imgPath = "";
        public string fboxPath = SingleTonePath.getInstance().fileBoxPath;
        ObjectInfo objInfo;

        FrameworkControl framework = null;
        public MTSmoothContainer thisCont = null;
        public PointF thisPosition;
        public double thisAngle;

        private Storyboard myStoryboard;

        public ImageView()
        {
            InitializeComponent();
        }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, MTSmoothContainer cont, double angle, ObjectInfo _objInfo)
        {
            objInfo = _objInfo;
            imgPath = objInfo.Path;
            img.Source = new BitmapImage(new Uri(imgPath, UriKind.Absolute));

            main = _main;
            window = target;
            framework = fw;
            thisCont = cont;
            thisAngle = angle;
        }

        public void img_bt1_Click(PointF p)
        {
            if (btGrid.Visibility == Visibility.Visible)
            {
            }
        }

        public void img_bt2_Click(PointF p)
        {
            animation(this);
            if (btGrid.Visibility == Visibility.Visible)
            {
                main.Children.Remove(this);
                framework.UnregisterElement(thisCont.Id);
                thisCont.isRemoved = true;
            }
        }

        public void imageViewCon_Tap(PointF p)
        {
            if (btGrid.Visibility == Visibility.Hidden)
                btGrid.Visibility = Visibility.Visible;
            else
                btGrid.Visibility = Visibility.Hidden;
        }

        public void sendFile(FrameworkElement f, string state)
        {
            if (state == "FileBox")  //파일 박스로 전송
            {
                FileBox3 fb = f as FileBox3;
                string fpath = fboxPath + fb.userIP + "\\";
                fpath += "MyBox\\" + objInfo.FileName;

                FileInfo fileinfo = new FileInfo(objInfo.FilePath);
                FileInfo fileinfo2 = new FileInfo(fpath);

                if (fileinfo2.Exists == false)
                {
                    fileinfo.CopyTo(fpath, false);
                    fb.updateListBox();
                }
            }
            else if (state == "SmartArea")
            {

            }
            img_bt2_Click(thisPosition);
        }

        void animation(FrameworkElement target)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;  // 시작
            myDoubleAnimation.To = 0.0;    // 끝
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3)); // 실행할 에니메이션 시간
            myDoubleAnimation.AutoReverse = false; // 반복여부

            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation); // 스토리보드에 등록
            //myStoryboard.Children.Add(myDoubleAnimation1); // 스토리보드에 등록

            Storyboard.SetTargetName(myDoubleAnimation, target.Name); // 실행할 에니메이션의 객체 설정

            // 에니메이션을 적용 할 프로퍼트 설정
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(System.Windows.Controls.Image.OpacityProperty));
            //Storyboard.SetTargetProperty(myDoubleAnimation1, new PropertyPath(System.Windows.Controls.Grid.HeightProperty));

            myStoryboard.Begin(this); // 스토리보드를 실행 시킵니다.
        }
    }
}
