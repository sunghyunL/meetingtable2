package network.InternalTask;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.lang.reflect.Array;
import java.net.Socket;
import java.net.UnknownHostException;
import java.nio.charset.Charset;
import java.util.Arrays;

import android.util.Log;

//내부에서 나가는 작업을 정의
public abstract class InternalJob {
	public static String IPaddress ="192.168.0.11";
	public static int Port=4888;
	public static int STRINGBUFSIZE =150;
	public static int BUFSIZE = 1024;
	
	private Socket socket;
	protected DataOutputStream dos;
	protected DataInputStream dis;
	
	public InternalJob(){
		
	}
	
	protected void makeSocket(){
		try {
			socket=new Socket(IPaddress, Port);
			dos=new DataOutputStream(new BufferedOutputStream(socket.getOutputStream()));
			dis=new DataInputStream(new BufferedInputStream(socket.getInputStream()));
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
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
	
	abstract public void job_Start();
	
	static public void receiveStringProtocol(DataInputStream dis,byte stringBuf[])throws Exception{
		int totalCount=0,remainder=0,readCount=0,j=0;
		byte tempBuf[]=new byte[STRINGBUFSIZE];
		
		Arrays.fill(stringBuf, (byte) 0); //받으려는 바이트배열 초기화
		
		while(totalCount!=STRINGBUFSIZE){
			remainder=STRINGBUFSIZE-totalCount;
			if(remainder>0){
		
			
					readCount=dis.read(tempBuf, 0, remainder);
				
				for(int i=totalCount;i<totalCount+readCount;i++){
					stringBuf[i]=tempBuf[j++];
				}
				totalCount+=readCount;
			}
			
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
