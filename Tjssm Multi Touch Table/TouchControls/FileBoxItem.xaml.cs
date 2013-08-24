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
using TouchFramework.ControlHandlers.Class;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// FileBoxItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileBoxItem : UserControl
    {
        string DefaultPath = "C:\\Users\\Administrator\\Desktop\\Tjssm Multi Touch Table\\";
        string ResourcePath = "C:\\Users\\Administrator\\Desktop\\Tjssm Multi Touch Table\\TouchControls\\Resources\\";
        string filePath;

       public  ObjectInfo objectInfo = new ObjectInfo();

        public FileBoxItem(ObjectInfo objInfo)
        {
            InitializeComponent();

            objectInfo = objInfo;

            if (objectInfo.Type == "image")
            {
                img_pic.Source = new BitmapImage(new Uri(objInfo.Path, UriKind.Absolute));
                img_Item.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_item1.png", UriKind.Absolute));
            }
            else if (objectInfo.Type == "video")
            {
                img_Item.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_item2.png", UriKind.Absolute));
            }
            else if (objectInfo.Type == "ppt")
            {
                img_Item.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_item3.png", UriKind.Absolute));
            }
            else if (objectInfo.Type == "doc")
            {
                img_Item.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_item4.png", UriKind.Absolute));
            }
            else if (objectInfo.Type == "file")
            {
                img_Item.Source = new BitmapImage(new Uri(ResourcePath + "FileBox_item5.png", UriKind.Absolute));
            }

            filePath = objInfo.FilePath;

            text_fileName.Text = objInfo.FileName;
            this.Tag = "a";
        }
    }
}
