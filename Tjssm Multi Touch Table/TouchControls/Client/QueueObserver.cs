using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace TouchFramework.ControlHandlers.Client
{
 
    class QueueObserver
    {
        Thread t;
        Queue<String> mQueueString = PipeControl.Instance.stringMessageQueue;
        public void fcoordObserver()
        {
            System.Console.WriteLine("fcoordObserver");
            while (true)
            {
                if (mQueueString.Count != 0)
                {
                    String str = mQueueString.Dequeue();
                    System.Console.WriteLine(str);
                    //프로토콜 분석후 작업분기
                    String token="&";
                    String []strList=str.Split(token.ToCharArray());
                    
                    /*옵저버가 직접 실행하게 해도되나, 실행타임이 좀 걸리는 작업은
                    스레드로 분리해서 처리해주세요*/
                    if (strList[0].Equals("CAPTUREDISPLAY"))//핸드폰 좌표 받기
                    {
                        /*members*/
                        String ipAddress = strList[1];
                        int angle = Convert.ToInt32(strList[2]);
                        int x1 = Convert.ToInt32(strList[3]);
                        int y1 = Convert.ToInt32(strList[4]);
                        int state = Convert.ToInt32(strList[5]); //add:0 set:1 del:2
                        Jobs.Instance.listen_adjust_Device_Coordinate(ipAddress, angle, x1, y1, state);
                        if (strList[6].Equals("true"))//캡쳐 이미지 요청
                        {
                            //TO DO: c:/tablefile/capture_ip(ip주소) 로 캡쳐 이미지를 저장
                            int touchX = Convert.ToInt32(strList[7]);
                            int touchY = Convert.ToInt32(strList[8]);
                            Jobs.Instance.listen_capture_image(ipAddress, angle, x1, y1, touchX, touchY);
                            String sendStr = "CAPTUREDISPLAY&" + strList[1] + "&";
                            PipeControl.Instance.sendStringProtocolToServer_pipe(sendStr);

                        }
                    }
                    else if (strList[0].Equals("FILEDISPLAYSTART"))
                    {
                        String pathName = strList[1];
                        String ipAddress = strList[2];
                        Jobs.Instance.listen_fileReceive_start(pathName, ipAddress);

                    }
                    else if (strList[0].Equals("FILEDISPLAYCOMPLETE"))
                    {
                        String pathName = strList[1];
                        String ipAddress = strList[2];
                        Jobs.Instance.listen_fileReceive_finish(pathName, ipAddress);
                    }
                    else if (strList[0].Equals("REQUESTCONTACTS"))
                    {
                        String ipAddress = strList[1];
                        String filename = "c:/Touchtable/" + ipAddress + "/contacts.txt";
                        ArrayList list = new ArrayList();

                        FileStream fs2 = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);

                        StreamReader st = new StreamReader(fs2, System.Text.Encoding.Default);

                        st.BaseStream.Seek(0, SeekOrigin.Begin);
                        
                        
                        //while (st.Peek() > -1)
                        //{
                            // 한 줄씩 읽어와서 처리..
                        string contact = st.ReadToEnd();
                            
                        //}
                        int i=0;
                        String[] contactlist = contact.Split(token.ToCharArray());
                        st.Close();
                        fs2.Close();
                        
                        while (!contactlist[i].Equals(""))
                        {
                            ContactInfo info = new ContactInfo(contactlist[i], contactlist[i+1]);
                            i = i + 2;
                            list.Add(info);
                        }
                        Jobs.Instance.listen_processContacts(list);
                    }
                }
            }
        }

        public void observerStart()
        {
            t=new Thread(new ThreadStart(fcoordObserver));
            t.Start();
        }

        public void observerStop()
        {
            t.Abort();
        }
    }
}
