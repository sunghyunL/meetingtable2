package network.InternalTask;

import java.io.File;
import java.io.FileInputStream;
import java.util.LinkedList;
import java.util.Queue;

import android.content.ReceiverCallNotAllowedException;
import android.util.Log;

import network.socket.DcoordItem;

public class FileTrans extends InternalJob {
	public static Queue<String> fileReserveQueue=new LinkedList<String>();
	private static boolean isRunning=false;
	private boolean isFirstLoop=true;
	private byte fileBuf[]=new byte[BUFSIZE];
	private byte stringBuf[]=new byte[STRINGBUFSIZE];
	@Override
	public void job_Start() {
		// TODO Auto-generated method stub
		isRunning=true;
		Thread thread=new Thread(){
			@Override
			public void run() {
				makeSocket();
				while (true) {
					if(fileReserveQueue.isEmpty()){
						job_finish();
						break;
					}
					String[] command = fileReserveQueue.poll().split("&");
					
					if (command[0].equals("FILESEND")) {
						do_fileSend(0, command[1], command[2]); // 파일 전송
					} else if (command[0].equals("IMAGESEND")) {						
						do_fileSend(1, command[1], command[2]);
					}
				}
			}
		};
		thread.start();
	}
	
	public static void makeFileSendMessage(int type,String filePath,String fileName){ //type 0:파일 1:이미지
		if(type==0){ //전송하고자 하는 것이 파일일 경우
			fileReserveQueue.add("FILESEND&"+filePath+"&"+fileName);
		}else if(type==1){ //이미지 인경우
			fileReserveQueue.add("IMAGESEND&"+filePath+"&"+fileName);
		}
	}
	
	public static void demand_job_Start(){
		if(isRunning) return; //이미 동작중인 쓰레드가 있을 경우 동작중인 쓰레드에게 작업을 맡긴다.
		else{
			new FileTrans().job_Start(); //동작중인 쓰레드가 없을 경우 동작을 시킨다.
		}
	}
	
	private void job_finish(){
		sendStringProtocol("STOP&");
		closeSocket();
		isRunning=false;
	}
	
	public void do_fileSend(int type,String filePath,String fileName){
	
		File f=new File(filePath);
		int fileSize=(int)f.length();
		String sendStr = null;
		if(type==0){//파일 전송 일 경우
			sendStr="FILESENDTOWPF&"+fileName+"&"+fileSize+"&"; //서버에게 전송할 메세지 내용
		}else if(type==1){//이미지 전송 일 경우
			sendStr="IMAGESENDTOWPF&"+fileName+"&"+fileSize+"&"; //서버에게 전송할 메세지 내용
		}

		try {
			if(isFirstLoop){
				sendStringProtocol(sendStr); //처음 Task를 생성하기 위한
				isFirstLoop=false;
			}
			receiveStringProtocol(dis,stringBuf); //프로세스 시작 동기화를 위한 패킷 받기
			sendStringProtocol(sendStr); //쓰레드 풀에서 작업하기 위한
			Log.d("test","mark2:"+fileName);
			FileInputStream fis = new FileInputStream(new File(filePath));
			
			int totalCount=0;
			int readCount=0;
			int temp=0;
			//파일 보내기 시작
			while(true){
				//Log.d("fileTrans","fileSize:"+fileSize+"totalCount:"+totalCount+"Count:"+temp++ +"test:"+fileReadBuf[1023]);
				if(totalCount>=fileSize){
					break;
				}else if(totalCount+BUFSIZE<=fileSize){
					readCount=fis.read(fileBuf);
					
					dos.write(fileBuf,0,readCount);
					totalCount+=readCount;
				}else if(totalCount+BUFSIZE>fileSize){
					int remainder=fileSize-totalCount;
					readCount=fis.read(fileBuf,0,remainder);
					
					dos.write(fileBuf,0,readCount);
					
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
}
