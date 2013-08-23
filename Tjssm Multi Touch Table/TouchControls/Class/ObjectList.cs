using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace TouchFramework.ControlHandlers.Class
{
    //===============================================

    // BaseSingleton

    //===============================================

    abstract class BaseSingleton<T> where T : class, new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }

    class ObjectList : BaseSingleton<ObjectList>
    {
        List<ObjectInfo> objList = new List<ObjectInfo>();
                
        private int sampleValue;
        
        public ObjectInfo getIndexAtObj(int _index)
        {
            return objList[_index];
        }

        public void addElement(UIElement _elt)
        {

        }

        public void addObj(ObjectInfo _obj)
        {
            objList.Add(_obj);
        }
    }
}