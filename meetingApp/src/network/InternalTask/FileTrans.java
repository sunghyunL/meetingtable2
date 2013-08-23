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
						do_fileSend(0, command[1], command[2]); // ���� ����
					} else if (command[0].equals("IMAGESEND")) {						
						do_fileSend(1, command[1], command[2]);
					}
				}
			}
		};
		thread.start();
	}
	
	public static void makeFileSendMessage(int type,String filePath,String fileName){ //type 0:���� 1:�̹���
		if(type==0){ //�����ϰ��� �ϴ� ���� ������ ���
			fileReserveQueue.add("FILESEND&"+filePath+"&"+fileName);
		}else if(type==1){ //�̹��� �ΰ��
			fileReserveQueue.add("IMAGESEND&"+filePath+"&"+fileName);
		}
	}
	
	public static void demand_job_Start(){
		if(isRunning) return; //�̹� �������� �����尡 ���� ��� �������� �����忡�� �۾��� �ñ��.
		else{
			new FileTrans().job_Start(); //�������� �����尡 ���� ��� ������ ��Ų��.
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
		if(type==0){//���� ���� �� ���
			sendStr="FILESENDTOWPF&"+fileName+"&"+fileSize+"&"; //�������� ������ �޼��� ����
		}else if(type==1){//�̹��� ���� �� ���
			sendStr="IMAGESENDTOWPF&"+fileName+"&"+fileSize+"&"; //�������� ������ �޼��� ����
		}

		try {
			if(isFirstLoop){
				sendStringProtocol(sendStr); //ó�� Task�� �����ϱ� ����
				isFirstLoop=false;
			}
			receiveStringProtocol(dis,stringBuf); //���μ��� ���� ����ȭ�� ���� ��Ŷ �ޱ�
			sendStringProtocol(sendStr); //������ Ǯ���� �۾��ϱ� ����
			Log.d("test","mark2:"+fileName);
			FileInputStream fis = new FileInputStream(new File(filePath));
			
			int totalCount=0;
			int readCount=0;
			int temp=0;
			//���� ������ ����
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
