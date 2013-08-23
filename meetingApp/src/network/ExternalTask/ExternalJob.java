package network.ExternalTask;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;

import android.util.Log;

//외부에서 들어오는 작업을 정의
public abstract class ExternalJob {
	protected DataOutputStream dos;
	protected DataInputStream dis;
	private Socket socket;
	protected String strList[];
	public static int BUFSIZE = 1024;
	public static int STRINGBUFSIZE = 150;
	
	public ExternalJob(Socket socket,DataInputStream dis,DataOutputStream dos,String strList[]){
		this.socket=socket;
		this.dos=dos;
		this.dis=dis;
		this.strList=strList;
	}
	
	abstract public void job_Start();
	
	protected void closeSocket(){
		try {
			dos.close();
			dis.close();
			socket.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	protected void sendStringProtocol(String str){
		try {
			//Arrays.fill(stringBuf, (byte)0);
			Log.d("test",str);
			//str.getBytes(0, str.length(), stringBuf, 0);
			
			byte[]Buf=str.getBytes("EUC-KR");
			dos.write(Buf);
			dos.flush();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}
