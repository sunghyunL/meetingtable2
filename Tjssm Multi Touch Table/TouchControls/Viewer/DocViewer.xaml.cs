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

using System.IO;
using System.Windows;
using System.Windows.Xps.Packaging;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using Word = Microsoft.Office.Interop.Word;
using System.Drawing;
using TouchFramework.ControlHandlers.Class;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// DocViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DocViewer : UserControl
    {
        public System.Windows.Window window;
        public Canvas main;
        public string imgPath = "";

        FrameworkControl framework = null;
        public MTSmoothContainer thisCont = null;
        public PointF thisPosition;
        public double thisAngle;
        
        public DocViewer()
        {
            InitializeComponent();            
        }

        public void setInit(Canvas _main, System.Windows.Window target, FrameworkControl fw, MTSmoothContainer cont, double angle, ObjectInfo _objInfo)
        {
            main = _main;
            window = target;
            framework = fw;
            thisCont = cont;
            thisAngle = angle;

            string wordDocument = _objInfo.FilePath;

            string convertedXpsDoc = string.Concat(System.IO.Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".xps");
            XpsDocument xpsDocument = ConvertWordToXps(wordDocument, convertedXpsDoc);
            if (xpsDocument == null)
            {
                return;
            }

            documentviewWord.Document = xpsDocument.GetFixedDocumentSequence();
//             System.Windows.Controls.ContentControl cc = documentviewWord.Template.FindName("PART_FindToolBarHost", documentviewWord) as System.Windows.Controls.ContentControl;
//             cc.Visibility = Visibility.Collapsed;
            
        }

        public void bt1_Click(PointF p)
        {
            documentviewWord.IncreaseZoom();
        }

        public void bt2_Click(PointF p)
        {
            documentviewWord.DecreaseZoom();
        }

        public void bt_close_Click(PointF p)
        {
            main.Children.Remove(this);
            framework.UnregisterElement(thisCont.Id);
            thisCont.isRemoved = true;
        }

        public void sendFile(string ip)
        {
            if (ip[0] == 'f') //파일 박스로 전송
            {

            }
            else if (ip[0] == 'm') // 모바일 폰으로 전송
            {

            }
        }

        /// <summary> 
        ///  Convert the word document to xps document 
        /// </summary> 
        /// <param name="wordFilename">Word document Path</param> 
        /// <param name="xpsFilename">Xps document Path</param> 
        /// <returns></returns> 
        private XpsDocument ConvertWordToXps(string wordFilename, string xpsFilename)
        {
            // Create a WordApplication and host word document 
            Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            try
            {
                wordApp.Documents.Open(wordFilename);

                // To Invisible the word document 
                wordApp.Application.Visible = false;


                // Minimize the opened word document 
                wordApp.WindowState = WdWindowState.wdWindowStateMinimize;


                Document doc = wordApp.ActiveDocument;


                doc.SaveAs(xpsFilename, WdSaveFormat.wdFormatXPS);


                XpsDocument xpsDocument = new XpsDocument(xpsFilename, FileAccess.Read);
                return xpsDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurs, The error message is  " + ex.ToString());
                return null;
            }
            finally
            {
                wordApp.Documents.Close();
                ((_Application)wordApp).Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
        }

        // 
        //         /// <summary> 
        //         ///  Select Word file  
        //         /// </summary> 
        //         /// <param name="sender"></param> 
        //         /// <param name="e"></param> 
        //         private void btnSelectWord_Click(object sender, RoutedEventArgs e)
        //         {
        //             // Initialize an OpenFileDialog 
        //             OpenFileDialog openFileDialog = new OpenFileDialog();
        // 
        // 
        //             // Set filter and RestoreDirectory 
        //             openFileDialog.RestoreDirectory = true;
        //             openFileDialog.Filter = "Word documents(*.doc;*.docx)|*.doc;*.docx";
        // 
        // 
        //             bool? result = openFileDialog.ShowDialog();
        //             if (result == true)
        //             {
        //                 if (openFileDialog.FileName.Length > 0)
        //                 {
        //                     txbSelectedWordFile.Text = openFileDialog.FileName;
        //                 }
        //             }
        //         }

// 
//         /// <summary> 
//         ///  View Word Document in WPF DocumentView Control 
//         /// </summary> 
//         /// <param name="sender"></param> 
//         /// <param name="e"></param> 
//         private void btnViewDoc_Click(object sender, RoutedEventArgs e)
//         {
//             string wordDocument = txbSelectedWordFile.Text;
//             if (string.IsNullOrEmpty(wordDocument) || !File.Exists(wordDocument))
//             {
//                 MessageBox.Show("The file is invalid. Please select an existing file again.");
//             }
//             else
//             {
//                 string convertedXpsDoc = string.Concat(System.IO.Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".xps");
//                 XpsDocument xpsDocument = ConvertWordToXps(wordDocument, convertedXpsDoc);
//                 if (xpsDocument == null)
//                 {
//                     return;
//                 }
// 
//                 documentviewWord.Document = xpsDocument.GetFixedDocumentSequence();
//             }
//         } 

        /*
        <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="70"></RowDefinition>
            <RowDefinition></RowDefinition>
        </Grid.RowDefinitions>
        <Label Name="lable1" Margin="3,6,0,0" Content="Word Document :" VerticalAlignment="Top" HorizontalAlignment="Left" />
        <TextBox  Name="txbSelectedWordFile" VerticalAlignment="Top"  HorizontalAlignment="Stretch" Margin="110,10,300,0" HorizontalContentAlignment="Left" />
        <Button HorizontalAlignment="Right" VerticalAlignment="Top" Width="150" Content="Select Word File" Name="btnSelectWord" Margin="0,10,130,0" Click="btnSelectWord_Click" />
        <Button HorizontalAlignment="Left" Margin="3,40,0,0" VerticalAlignment="Top" Content="View Word Doc" Width="100" Name="btnViewDoc" Click="btnViewDoc_Click" />
        <DocumentViewer Grid.Row="1" Name="documentviewWord" VerticalAlignment="Top" HorizontalAlignment="Left"/>
    </Grid>

          */
    } 

}
