package network.ExternalTask;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.ServerSocket;
import java.net.Socket;

import network.InternalTask.InternalJob;
import android.content.Context;
import android.os.Handler;
import android.util.Log;

public class acceptServer {
	public static int PORT = 9997;
	private Context context;
	private Handler handler;
	public acceptServer(Context context,Handler handler) {
		this.context=context;
		this.handler=handler;
	}
	byte stringBuf[]=new byte[ExternalJob.STRINGBUFSIZE];
	public void startServer(){
		Thread thread=new Thread(){

			@Override
			public void run() {
				try {
					ServerSocket server=new ServerSocket(PORT);
					Log.d("ver2.test","<SERVER>serverStart");
					while(true){
						Socket socket= server.accept();
						Log.d("ver2.test","<SERVER>client access ");
						DataOutputStream dos=new DataOutputStream(new BufferedOutputStream(socket.getOutputStream()));
						DataInputStream dis=new DataInputStream(new BufferedInputStream(socket.getInputStream()));
						InternalJob.receiveStringProtocol(dis, stringBuf);
						String strList[]=new String(stringBuf).split("&");
						String opcode=strList[0];
						if(opcode.equals("FILESENDTOAND")){
							new FileReceiver(handler,socket, dis, dos, strList).job_Start();
						}else if(opcode.equals("REQUESTCONTACTS")){
							new ShareContact(context,socket, dis, dos, strList).job_Start();
						}else if(opcode.equals("SAVECONTACT")){
							new SaveContact(context,handler,socket, dis, dos, strList).job_Start();
						}
					}
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			
			}
			
		};
		
		thread.start();
	}
}
