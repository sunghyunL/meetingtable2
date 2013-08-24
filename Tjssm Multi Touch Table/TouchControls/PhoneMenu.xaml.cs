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

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// phoneMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class phoneMenu : UserControl
    {
        public string userIP = "";

        public Canvas main;
        public Window window;
        public MTSmoothContainer thisCont = null;
        
        public PointF FirstPoint, SecondPoint;
        public PointF thisPosition;
        public double thisAngle;
        public FileBox3 fBox;
        bool isCreated = false;

        FrameworkControl framework = null;
        MTSmoothContainer fBoxCont;
        ElementProperties fBoxProp = new ElementProperties();
        
        public phoneMenu()
        {
            InitializeComponent();

            bt1.Click += bt1_Click;
            bt2.Click += bt2_Click;
            bt3.Click += bt3_Click;
        }

        public void pMenuCon_TouchDown(PointF p)
        {
            FirstPoint = p;
        }

        public void pMenuCon_TouchUp(PointF p)
        {
            SecondPoint = p;

            //절대 좌표로 변경
            thisPosition = new PointF(thisCont.ObjectTouches.MoveCenter.X, thisCont.ObjectTouches.MoveCenter.Y);
        }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, MTSmoothContainer cont, double angle, string _ip)
        {
            main = _main;
            window = target;
            framework = fw;
            thisCont = cont;
            thisAngle = angle;
            userIP = _ip;
        }

        public void bt1_Click(object sender, RoutedEventArgs e)
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

                fBox.Tag = "f/" + userIP;
                SingleToneTrans.getInstance().addToFileBox(fBox);

                isCreated = true;
            }
            else
            {
                main.Children.Remove(fBox);
                framework.UnregisterElement(fBoxCont.Id);
                fBox = new FileBox3();

                SingleToneTrans.getInstance().removeFileBox(fBox);
                isCreated = false;
            }
        }

        public void bt2_Click(object sender, RoutedEventArgs e)
        {
//             Web web = new Web("http://www.naver.com", window, main, SecondPoint, objTransform.RotateTrans.Angle);
//             main.Children.Add(web);
        }

        public void bt3_Click(object sender, RoutedEventArgs e)
        {
//                 Web web = new Web("http://www.youtube.com", window, main, SecondPoint, objTransform.RotateTrans.Angle);
//                 main.Children.Add(web);

        }
    }
}
