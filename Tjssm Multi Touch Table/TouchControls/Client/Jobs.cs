﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchFramework.ControlHandlers;
using System.Drawing;
using TouchFramework.ControlHandlers.Class;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TouchFramework.Events;
using TouchFramework;
using System.Windows.Threading;

namespace TouchFramework.ControlHandlers.Client
{
    public class ContactInfo
    {
        private String name;
        private String number;
        
        public ContactInfo(String name, String number)
        {
            this.name = name;
            this.number = number;
        }
        public String getName()
        {
            return name;
        }
        public String getNumber()
        {
            return number;
        }
    }

    public class Jobs
    {
        private static Jobs _instance;
        private ArrayList arrList = new ArrayList();
        private Canvas main = new Canvas();
        public Window window;
        FrameworkControl framework;
        private SmartArea _sArea;
        private bool requestComplete = false;

        //싱글톤
        public static Jobs Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Jobs();

                return _instance;
            }
        }

        private Jobs() { }

        public void setInit(Canvas _main, Window target, FrameworkControl fw, SmartArea s)
        {
            main = _main;
            window = target;
            framework = fw;
            _sArea = s;
        }

        private delegate void createSmartAreaDelegate(string _ip, PointF pt, double angle);
        private delegate void removeSmartAreaDelegate(string _ip);
        private delegate void MTContDelegate(string _ip, float offsetX, float offsetY, double angle);

        private void BeginInvokeCreate(SmartArea s, string _ip, PointF pt, double angle)
        {
            if (s.Dispatcher.CheckAccess())
                createSmartArea(_ip, pt, angle);
            else
                s.Dispatcher.BeginInvoke(new createSmartAreaDelegate(createSmartArea), _ip, pt, angle);

        }

        private void BeginInvokeRemove(SmartArea s, string _ip)
        {
            if (s.Dispatcher.CheckAccess())
                removeSmartArea(_ip);
            else
                s.Dispatcher.BeginInvoke(new removeSmartAreaDelegate(removeSmartArea), _ip);

        }

        private void BeginInvokeCont(SmartArea s, string _ip, float offsetX, float offsetY, double angle)
        {
            if (s.Dispatcher.CheckAccess())
                findContainer(_ip, offsetX, offsetY, angle);
            else
                s.Dispatcher.BeginInvoke(new MTContDelegate(findContainer), _ip, offsetX, offsetY, angle);
        }

        public void createSmartArea(string _ip, PointF pt, double angle)
        {
            //Init SmartArea Control
            SmartArea smartArea = new SmartArea();

            ElementProperties prop = new ElementProperties();
            prop.ElementSupport.AddSupport(TouchFramework.TouchAction.Drag);
            prop.ElementSupport.AddSupport(TouchFramework.TouchAction.Tap);

            MTSmoothContainer smartAreaCont = new MTSmoothContainer(smartArea, main, prop);
            framework.RegisterElement(smartAreaCont);

            smartArea.Tag = _ip;
            smartAreaCont.userIP = _ip;

            main.Children.Add(smartArea);
            smartArea.setInit(main, window, framework, smartAreaCont, _ip, angle);

            smartAreaCont.SetPosition(pt.X, pt.Y, angle, 1.0);

            SingleToneTrans.getInstance().addToArea(smartArea);
            SingleToneTrans.getInstance().addToCont(smartAreaCont);
        }

        public void removeSmartArea(string _ip)
        {
            //Init SmartArea Control
            SmartArea smartArea = SingleToneTrans.getInstance().getArea(_ip);
            MTSmoothContainer cont = SingleToneTrans.getInstance().getCont(_ip);
            
            SingleToneTrans.getInstance().removeArea(smartArea);
            SingleToneTrans.getInstance().removeCont(cont);

            main.Children.Remove(smartArea);
            framework.UnregisterElement(cont.Id);
            cont.isRemoved = true;
        }

        public void findContainer(string ipAddress, float offsetX, float offsetY, double angle)
        {
            foreach (MTSmoothContainer s in SingleToneTrans.getInstance().contList)
            {
                if (s.userIP == ipAddress)
                {
                    s.SetPosition(offsetX, offsetY, angle, 1.0);
                }
            }
        }

        public void listen_adjust_Device_Coordinate(String ipAddress,int angle,int x,int y, int state)
        {/*TO DO: 핸드폰 좌표 수정 한다
          * 스레드 분기 없이 실행해주세요
          */
            if (state == 0)
            {
                BeginInvokeCreate(_sArea, ipAddress, new PointF(x, y), angle);

                //createSmartArea(ipAddress, new PointF(x, y), angle);
            }
            else if (state == 1)
            {
                BeginInvokeCont(_sArea, ipAddress, x, y, angle);

//                 foreach (MTSmoothContainer s in SingleToneTrans.getInstance().contList)
//                 {
//                     if (s.userIP == ipAddress)
//                     {
//                         //s.SetPosition(x, y, angle, 1.0);
//                     }
//                 }
            }
            else if (state == 2)
            {
                BeginInvokeRemove(_sArea, ipAddress);
                //removeSmartArea(ipAddress);
            }
        }

        public void listen_capture_image(String ipAddress, int angle, int x, int y, int touchX, int touchY)
        { /*TO DO: 이미지 캡쳐해서 파일로 저장하기 c:/Touchtable/capture_ip(ip주소) 로 캡쳐 이미지를 저장
           *스레드 분기 없이 실행해 주세요
           *x,y는 device flash 좌표(절대좌표)
           *touchX,touchY는 device내의 좌표 (상대좌표)
           */
           
        }

        public void listen_fileReceive_start(String pathName, String ipAddress)
        {/*TO DO:파일전송 시작 시점에 호출됨*/

        }

        public void listen_fileReceive_finish(String pathName, String ipAddress)
        {/*TO DO:파일전송 종료 시점에 호출됨*/

        }

        /****************연락처 요청 및 받기*******************/


        public void listen_processContacts(ArrayList list)
        {/*TO DO:요청후에 callback형식으로 이 함수가 호출 된다*/
            arrList = list;
            requestComplete = true;
            foreach (object o in list)
            {
                ContactInfo info = (ContactInfo)o;
                
                System.Console.WriteLine(info.getName() + "/" + info.getNumber());
            }
        }

        public ArrayList getList()
        {
            while (requestComplete == false)
            {

            }
            return arrList;
        }

        public void call_requestContacts(String ipAddress)
        {/*TO DO:연락처 리스트를 요청한다*/
            PipeControl.Instance.sendStringProtocolToServer_pipe("REQUESTCONTACTS&" + ipAddress);
        }

        public void call_sendContacts(String ipAddress, String name, String number)
        {/*TO DO:해당 ip로 연락처를 보낸다*/
            String str="SAVECONTACT&"+ipAddress+"&"+name+"&"+number+"&";
            PipeControl.Instance.sendStringProtocolToServer_pipe(str);
        }
        /********************************************************/

        public void call_sendFileToDevice(String ipAdd, String filename)
        { /*해당 ip에 파일 전송하기*/
            PipeControl.Instance.sendFileToServer_pipe(ipAdd, filename);
        }
    }
}
