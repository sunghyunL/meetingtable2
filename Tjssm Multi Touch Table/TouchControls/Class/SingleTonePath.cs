using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TouchFramework.ControlHandlers.Class
{
    public class SingleTonePath
    {
        private static SingleTonePath Instance;
        private SingleTonePath() { }

        public string rootPath = "C:\\Users\\Administrator\\Desktop\\Tjssm Multi Touch Table\\";
        public string mainPath = "C:\\Users\\Administrator\\Desktop\\Tjssm Multi Touch Table\\Tjssm Multi Touch Table\\";
        public string controlsPath = "C:\\Users\\Administrator\\Desktop\\Tjssm Multi Touch Table\\TouchControls\\";
        public string resourcesPath = "C:\\Users\\Administrator\\Desktop\\Tjssm Multi Touch Table\\TouchControls\\Resources\\";
        public string fileBoxPath = "C:\\Users\\Administrator\\Desktop\\Tjssm Multi Touch Table\\FileBox\\";

        public static SingleTonePath getInstance()
        {
            if (Instance == null) Instance = new SingleTonePath();
            return Instance;
        }
    }
}
