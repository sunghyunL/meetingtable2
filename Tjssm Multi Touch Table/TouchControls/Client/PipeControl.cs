using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace TouchFramework.ControlHandlers.Client
{
    class PipeControl
    {
        static int BUF_SIZE = 150;
        byte[] StringBuf = new byte[BUF_SIZE];
        short ServerPortNumber = 9993;
        Socket udpSocket;
        EndPoint localEP;
        EndPoint remoteEP;
        
        private static PipeControl _instance;

        public Queue<String> stringMessageQueue;
       
        private byte[] buf = new byte[BUF_SIZE];
        //싱글톤
        public static PipeControl Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PipeControl();

                return _instance;
            }
        }

        //싱글톤 생성자
        private PipeControl()
        {
            udpSocket = new Socket

         (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localEP = new IPEndPoint(IPAddress.Any, 5433);
            remoteEP = new IPEndPoint(IPAddress.Loopback, ServerPortNumber);
            udpSocket.Bind(localEP);

        }

        //pipeConnect및 stream생성 queue생성
        public void connectPipe()
        {
            udpSocket.SendTo(buf, remoteEP);

            stringMessageQueue = new Queue<string>();
        }

        //send 추상화 함수
        public void sendStringProtocolToServer_pipe(String str)
        {
            byte []getString = Encoding.Default.GetBytes(str);
            int count=(getString.Length<BUF_SIZE)?getString.Length:BUF_SIZE;
            for (int i = 0; i < BUF_SIZE; i++) //memset
            {
                StringBuf[i] = 0;
            }
            for (int i = 0; i < count; i++) //memcpy
            {
                StringBuf[i] = getString[i];
            }
            //udpSocket.SendTo(StringBuf, remoteEP);
            udpSocket.SendTo(StringBuf,BUF_SIZE,System.Net.Sockets.SocketFlags.None,remoteEP);
        }

        //안드로이드로 파일 보내기
        public void sendFileToServer_pipe(String IPaddress,String fileName)
        {
            String str = "FILESENDTOAND&" + fileName + "&" + IPaddress + "&";
            sendStringProtocolToServer_pipe(str); 
        }

        public void pipeReceiveThreadStart()
        {
            Thread t1 = new Thread(new ThreadStart(stringMessageReceiveThread));
            t1.Start();
        }

        //string message receive thread
        private void stringMessageReceiveThread(){
            System.Console.WriteLine("<Thread Start>stringMessageReceiveThread");
            while (true)
            {

                udpSocket.ReceiveFrom(buf, ref remoteEP);

                int isString = BitConverter.ToInt32(buf, 0);


                //String receiveStirng = Encoding.Default.GetString(buf);
                int strleng = 0;
                while (buf[strleng++] != 10) ;

                String receiveStirng = Encoding.Default.GetString(buf, 0, strleng - 1);

                stringMessageQueue.Enqueue(receiveStirng);

            }
        }
    }
}
