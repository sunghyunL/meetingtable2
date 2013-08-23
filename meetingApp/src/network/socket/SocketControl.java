package network.socket;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.Queue;

import android.app.Activity;
import android.content.Context;
import android.util.Log;

public class SocketControl {
	public static String SOCKET_ADD="192.168.0.12";
	public Queue<DcoordItem> dcoordQueue=new LinkedList<DcoordItem>();
	public Queue<String> stringQueue=new LinkedList<String>();
	
	private static int BUFSIZE = 1024;
	private static int STRINGBUFSIZE = 100;
	private byte stringReceiveBuf[]=new byte[STRINGBUFSIZE];
	private byte intBuf[]=new byte[4];
	private byte fileReadBuf[]=new byte[BUFSIZE];
	private byte stringSendBuf[]=new byte[STRINGBUFSIZE];
	
	private DataOutputStream dos;
	private DataInputStream dis;
	private Activity context;
	private Thread sendThread;
	private Socket socket;
	
	public SocketControl(Activity context){
		this.context=context;
		//connectSocket(); //receivePacketStart에서 해줌
		receivePacketStart();
		sendPacketStart();
	};

	public void connectSocket() {
		try {
			socket = new Socket(SOCKET_ADD, 4889);
			dos = new DataOutputStream(new BufferedOutputStream(
					socket.getOutputStream()));
			dis = new DataInputStream(new BufferedInputStream(
					socket.getInputStream()));
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			Log.d("error", "connectSocket");
		}
	}

	public void receivePacketStart() {
		Thread thread = new Thread() {
			@Override
			public void run() {
				connectSocket();
				byte tempBuf[]=new byte[100];
				
				int totalCount=0,readCount=0,remainder=0,j=0;
				try {
					
					while (true) {
						totalCount=0;readCount=0;remainder=0;j=0;
						Arrays.fill(stringReceiveBuf, (byte) 0); //memset
						
						//사이즈 100만큼을 읽어서 다음작업을 진행하기 위해//무조건 패킷을 100만큼 읽는다.
						while(totalCount!=STRINGBUFSIZE){
							remainder=100-totalCount;
							if(remainder>0){
							//	readCount=dis.read(stringReceiveBuf);
								Log.d("test11","<서버패킷 수신 대기>");
								readCount=dis.read(tempBuf, 0, remainder);
								//readCount=dis.read(stringReceiveBuf);
								Log.d("test11","<서버패킷 수신 완료>"+readCount);
								for(int i=totalCount;i<totalCount+readCount;i++){
									stringReceiveBuf[i]=tempBuf[j++];
								}
								totalCount+=readCount;
							}
							
						}
						//Log.d("test11","<서버패킷 수신>");
						ByteArrayInputStream bis = new ByteArrayInputStream(
								stringReceiveBuf);
						
						bis.read(intBuf, 0, 4);
						int byteProcotolDumy = byteArrayToInt(intBuf);
						if (byteProcotolDumy == -1) {// 바이트 프로토콜
							bis.read(intBuf, 0, 4);
							int opcode = byteArrayToInt(intBuf);
							if (opcode == 1) { // Dcoord
								DcoordItem di = new DcoordItem();

								bis.read(intBuf, 0, 4);
								di.setState(byteArrayToInt(intBuf));

								bis.read(intBuf, 0, 4);
								di.setStateInfo(byteArrayToInt(intBuf));

								bis.read(intBuf, 0, 4);
								di.setLx(byteArrayToInt(intBuf));

								bis.read(intBuf, 0, 4);
								di.setLy(byteArrayToInt(intBuf));

								bis.read(intBuf, 0, 4);
								di.setRx(byteArrayToInt(intBuf));

								bis.read(intBuf, 0, 4);
								di.setRy(byteArrayToInt(intBuf));
								// dcoord패킷을 받아서 큐에 삽입
								dcoordQueue.add(di);
							}
						} else { // 스트링 프로토콜
							processStringProtocol(new String(stringReceiveBuf));
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
	
	public void sendPacketStart(){
		sendThread = new Thread() {
			@Override
			public void run() {
				while (true) {
					if (!stringQueue.isEmpty()) {
						String[] strList = stringQueue.poll().split("&");

						if (strList[0].equals("FILETRANS")) {
							sendFileToServer(strList[1],strList[2]);
						} else if (strList[0].equals("DCOORDSTART")) {
							sendRequestDcoord();
						} else if (strList[0].equals("DCOORDEND")) {
							sendEndDcoord();
						}
					} else {
						try {
							sleep(100000);
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
					}
				
				}

			}
		};
		sendThread.start();
	}
	
	private void processStringProtocol(String str){
		String []strList=str.split("&");
		if(strList[0].equals("FILETRANS")){
			//파일 저장하기
			String fileName=strList[1];
			int fileSize=Integer.valueOf(strList[2]);
			try {
				
				File folder=new File("/storage/sdcard1/TouchTable");
				if(!folder.exists()){
					folder.mkdir();
				}
				
				File file=new File("/storage/sdcard1/TouchTable/"+fileName);
				FileOutputStream fos = new FileOutputStream(file);

				int totalCount=0;
				int readCount=0;
				Log.d("test11","<FILERECEIVE>파일받기 시작:"+fileName);
				//파일 받기 시작
				int temp=0;
				while(true){
					//Log.d("test11","filesize:"+fileSize+"total:"+totalCount+"count:"+temp++ +"flag:"+fileReadBuf[0]);
					if(totalCount>=fileSize){
						break;
					}else if(totalCount+BUFSIZE<=fileSize){
						readCount=dis.read(fileReadBuf,0,BUFSIZE);
						fos.write(fileReadBuf, 0 , readCount);
						
						totalCount+=readCount;
					}else if(totalCount+BUFSIZE>fileSize){
						int remainder=fileSize-totalCount;
						readCount=dis.read(fileReadBuf,0,remainder);						
						fos.write(fileReadBuf, 0, readCount);
				
						totalCount+=readCount;
					}
				}
				
				fos.close();
				Log.d("test11","<FILERECEIVE>파일받기 종료:"+fileName);
				//파일 받기 종료
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
	
	private int byteArrayToInt(byte[] bytes) {
		int newValue = 0;
		switch (bytes.length) {
		case 1:
			newValue |= (bytes[0]) & 0xFF;
			break;
		case 2:
			newValue |= ((bytes[1]) << 8) & 0xFF00;
			newValue |= (bytes[0]) & 0xFF;
			break;
		case 3:
			newValue |= ((bytes[2]) << 16) & 0xFF0000;
			newValue |= ((bytes[1]) << 8) & 0xFF00;
			newValue |= (bytes[0]) & 0xFF;
			break;
		case 4:
			newValue |= ((bytes[3]) << 24) & 0xFF000000;
			newValue |= ((bytes[2]) << 16) & 0xFF0000;
			newValue |= ((bytes[1]) << 8) & 0xFF00;
			newValue |= (bytes[0]) & 0xFF;
		default:
			break;
		}
		return newValue;
	}
	
	public void sendFileToServer(String fullName,String fileName){
		File f=new File(fullName);
		int fileSize=(int)f.length();
		String sendStr="FILETRANS&"+fileName+"&"+fileSize+"&";

		try {
			sendStringProtocol(sendStr);
			
			Log.d("test","mark2:"+fileName);
			FileInputStream fis = new FileInputStream(new File(fullName));
			
			int totalCount=0;
			int readCount=0;
			int temp=0;
			//파일 보내기 시작
			while(true){
				//Log.d("fileTrans","fileSize:"+fileSize+"totalCount:"+totalCount+"Count:"+temp++ +"test:"+fileReadBuf[1023]);
				if(totalCount>=fileSize){
					break;
				}else if(totalCount+BUFSIZE<=fileSize){
					readCount=fis.read(fileReadBuf);
					
					dos.write(fileReadBuf,0,readCount);
					totalCount+=readCount;
				}else if(totalCount+BUFSIZE>fileSize){
					int remainder=fileSize-totalCount;
					readCount=fis.read(fileReadBuf,0,remainder);
					
					dos.write(fileReadBuf,0,readCount);
					
					totalCount+=readCount;
					Log.d("test","lastpacket");
				}
			}
			dos.flush();
			fis.close();
			
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void sendRequestDcoord(){
		sendStringProtocol("DCOORDSTART&");
	}
	
	public void sendEndDcoord(){
		sendStringProtocol("DCOORDEND&");
	}
	
	public void makeFileSendMessage(String fullName,String fileName){
		stringQueue.add("FILETRANS&"+fullName+"&"+fileName);
		sendThread.interrupt();
	}
	
	public void makeRequestDcoordMessage(){
		stringQueue.add("DCOORDSTART&");
		sendThread.interrupt();
	}
	
	public void makeEndDcoordMessage(){
		stringQueue.add("DCOORDEND&");
		sendThread.interrupt();
	}
	
	//send 추상화 함수
	private void sendStringProtocol(String str){
		try {
			for(int i=0;i<STRINGBUFSIZE;i++){ //버퍼초기화
				stringSendBuf[i]=0;
			}
			Log.d("test",str);
			str.getBytes(0, str.length(), stringSendBuf, 0);
			dos.write(stringSendBuf);
			dos.flush();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}
