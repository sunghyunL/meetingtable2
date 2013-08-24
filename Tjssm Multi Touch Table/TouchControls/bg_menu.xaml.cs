using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// bg_menu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class bg_menu : UserControl
    {
        public string userIP = "center";

        public Canvas main;
        public Window window;
        public MTSmoothContainer thisCont = null;

        public PointF thisPosition;
        public double thisAngle;
        public FileBox3 fBox;
        bool isCreated = false;

        FrameworkControl framework = null;
        MTSmoothContainer fBoxCont;
        ElementProperties fBoxProp = new ElementProperties();

        private Storyboard myStoryboard; // 전역변수로 선언한다.

        public bg_menu()
        {
            InitializeComponent();

            animation();
        }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, MTSmoothContainer cont, double angle)
        {
            main = _main;
            window = target;
            framework = fw;
            thisCont = cont;
            thisAngle = angle;
        }

        public void bt_filebox_Click(object sender, RoutedEventArgs e)
        {
            if (main.Children.Contains(fBox) == false)
            {
                fBox = new FileBox3();
                main.Children.Add(fBox);

                fBoxProp.ElementSupport.AddSupport(TouchAction.Tap);
                fBoxProp.ElementSupport.AddSupport(TouchAction.Drag);
                fBoxProp.ElementSupport.AddSupport(TouchAction.ScrollY);

                fBoxCont = new MTSmoothContainer(fBox, main, fBoxProp);
                fBoxCont.SetPosition(thisPosition.X, thisPosition.Y,
                    thisAngle + thisCont.RotateFilter.Target, 1.0);

                framework.RegisterElement(fBoxCont);
                fBox.setInit(main, window, framework, fBoxCont, thisAngle, userIP);

                isCreated = true;
            }
            else
            {
                main.Children.Remove(fBox);
                framework.UnregisterElement(fBoxCont.Id);
                fBox = new FileBox3();
                isCreated = false;
            }
        }

        public void bt_web_Click(object sender, RoutedEventArgs e)
        {
            if (main.Children.Contains(fBox) == false)
            {
                fBox = new FileBox3();
                main.Children.Add(fBox);

                fBoxProp.ElementSupport.AddSupport(TouchAction.Tap);
                fBoxProp.ElementSupport.AddSupport(TouchAction.Drag);
                fBoxProp.ElementSupport.AddSupport(TouchAction.ScrollY);

                fBoxCont = new MTSmoothContainer(fBox, main, fBoxProp);
                fBoxCont.SetPosition(thisPosition.X, thisPosition.Y,
                    thisAngle + thisCont.RotateFilter.Target, 1.0);

                framework.RegisterElement(fBoxCont);
                fBox.setInit(main, window, framework, fBoxCont, thisAngle, userIP);

                isCreated = true;
            }
            else
            {
                main.Children.Remove(fBox);
                framework.UnregisterElement(fBoxCont.Id);
                fBox = new FileBox3();
                isCreated = false;
            }
        }

        void animation()
        {            
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;  // 시작
            myDoubleAnimation.To = 0.0;    // 끝
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3)); // 실행할 에니메이션 시간
            myDoubleAnimation.AutoReverse = true; // 반복여부
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever; // 반복횟수

            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            myDoubleAnimation1.To = 0;
            myDoubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(2));
            myDoubleAnimation1.AutoReverse = true;
            myDoubleAnimation1.RepeatBehavior = RepeatBehavior.Forever;
            
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation); // 스토리보드에 등록
            //myStoryboard.Children.Add(myDoubleAnimation1); // 스토리보드에 등록
            
            Storyboard.SetTargetName(myDoubleAnimation, img_bg_effect.Name); // 실행할 에니메이션의 객체 설정

            //Storyboard.SetTargetName(myDoubleAnimation1, "grid1"); // 실행할 에니메이션의 객체 설정

            // 에니메이션을 적용 할 프로퍼트 설정
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(System.Windows.Controls.Image.OpacityProperty));
            //Storyboard.SetTargetProperty(myDoubleAnimation1, new PropertyPath(System.Windows.Controls.Grid.HeightProperty));

            myStoryboard.Begin(this); // 스토리보드를 실행 시킵니다.
        }
    }
}
