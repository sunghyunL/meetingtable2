using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TouchFramework.ControlHandlers.Client
{
    public class Start
    {
        Thread t;
        public void start()
        {         
            t=new Thread(new ThreadStart(mmain));
            t.Start();        
        }

        public void stop()
        {
            t.Abort();
        }

        public void mmain()
        {
            PipeControl.Instance.connectPipe();
            PipeControl.Instance.pipeReceiveThreadStart();

            QueueObserver observer = new QueueObserver();
            observer.observerStart();

            //PipeControl.Instance.sendFileToServer_pipe("192.168.0.9", "a.pdf"); 파일 전송 테스트
            //Jobs.Instance.call_sendContacts("192.168.0.9", "test", "01012341234");연락처 보내기 테스트
            //Jobs.Instance.call_requestContacts("192.168.0.9"); 연락처 요청 테스트

        }
    }

}
