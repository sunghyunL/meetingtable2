using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tjssm_Multi_Touch_Table
{
    public class ObjectInfo
    {
        /// <summary>
        /// A name for the file, not the file name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// A name for the file, not the file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description for the File.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Full path such as c:\path\to\image.png
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// A name for the file, not the file name.
        /// </summary>
        public string DirPath { get; set; }

        /// <summary>
        /// Full path such as c:\path\to\image.png
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The image file name such as image.png
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The file name extension: bmp, gif, jpg, png, tiff, etc...
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The image height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The image width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The file size.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// The file Count.
        /// </summary>
        public int Cnt { get; set; }     
    }
}
