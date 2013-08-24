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
    /// FileBoxObject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileBoxObject : UserControl
    {
        public RotateTransform objRotateTrans = new RotateTransform();
        public TranslateTransform objTranslateTrans = new TranslateTransform();
        public TransformGroup objTransGroup = new TransformGroup();

        public Window window;
        public Canvas main;
        public ObjectInfo objInfo;

        public string path, fileName, fileType, filePath;

        public FileBoxObject(Window target, Canvas _main, ObjectInfo _objInfo)
        {
            objInfo = _objInfo;
            path = objInfo.Path;
            fileName = objInfo.FileName;
            fileType = objInfo.Type;
            filePath = objInfo.FilePath;
            
            InitializeComponent();
            iconImg.Source = new BitmapImage(new Uri(objInfo.Path, UriKind.Absolute));
            textName.Text = fileName;

            objTransGroup.Children.Add(objRotateTrans);
            objTransGroup.Children.Add(objTranslateTrans);
        }

        public void setPosition(PointF _pos, double _rotate)
        {
            // 위치 조정

            objRotateTrans.Angle = _rotate;
            objTranslateTrans.X = _pos.X - (fileObjectCon.Width / 2);
            objTranslateTrans.Y = _pos.Y - (fileObjectCon.Height / 2);

            fileObjectCon.RenderTransform = objTransGroup;
        }
    }
}
