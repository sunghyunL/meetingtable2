using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TouchFramework.ControlHandlers.Class
{
    public class SingleToneTrans
    {
        private static SingleToneTrans Instance;

        private SingleToneTrans() { }
        public List<FileBox3> fileBoxList = new List<FileBox3>();
        public List<SmartArea> smartAreaList = new List<SmartArea>();
        public List<MTSmoothContainer> contList = new List<MTSmoothContainer>();

        public static SingleToneTrans getInstance()
        {
            if (Instance == null) Instance = new SingleToneTrans();
            return Instance;
        }

        public void addToFileBox(FileBox3 e)
        {
            fileBoxList.Add(e);
        }

        public void removeFileBox(FileBox3 e)
        {
            fileBoxList.Remove(e);
        }

        public void addToArea(SmartArea e)
        {
            smartAreaList.Add(e);
        }

        public void removeArea(SmartArea e)
        {
            smartAreaList.Remove(e);
        }

        public SmartArea getArea(string ip)
        {
            foreach (SmartArea s in smartAreaList)
            {
                if (ip == s.userIP)
                {
                    return s;
                }
            }
            return null;
        }

        public int getAreaIndex(SmartArea e)
        {
            int index = 0;
            foreach (SmartArea s in smartAreaList)
            {
                if (e.userIP == s.userIP)
                    break;
                index++;
            }
            return index;
        }

        public void addToCont(MTSmoothContainer e)
        {
            contList.Add(e);
        }

        public void removeCont(MTSmoothContainer e)
        {
            contList.Remove(e);
        }

        public MTSmoothContainer getCont(string ip)
        {
            foreach (MTSmoothContainer s in contList)
            {
                if (ip == s.userIP)
                {
                    return s;
                }
            }
            return null;
        }

        public MTSmoothContainer getIndexAtCont(int index)
        {
            return contList.ElementAt(index);
        }
// 
//         public void containsObj(FrameworkElement e)
//         {
//             foreach (var f in ObjList)
//             {
//                 if (ptIn.setPtinRect())
//                 {
//                 }
//             }
//         }
    }
}
