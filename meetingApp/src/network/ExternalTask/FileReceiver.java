package network.ExternalTask;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.net.Socket;
import java.util.Calendar;

import network.InternalTask.InternalJob;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;

public class FileReceiver extends ExternalJob {
	private byte fileReadBuf[]=new byte[BUFSIZE];
	private byte stringBuf[]=new byte[STRINGBUFSIZE];
	private boolean isFirstLoop=true;
	private Handler handler;
	public FileReceiver(Handler handler,Socket socket,DataInputStream dis,DataOutputStream dos,String strList[]) {
		super(socket, dis, dos, strList);
		this.handler=handler;
	}

	@Override
	public void job_Start() {
		Thread thread = new Thread() {
			@Override
			public void run() {
				// 파일받기 시작
				try {
					while (true) {
						if (!isFirstLoop) {
							InternalJob.receiveStringProtocol(dis, stringBuf);
							strList = new String(stringBuf).split("&");
							if (strList[0].equals("STOP"))
								break;
						}

						// 파일 저장하기
						String fileName = strList[1];
						int fileSize = Integer.valueOf(strList[2]);
						
						Calendar cal=Calendar.getInstance();
						int month=cal.get(Calendar.MONTH);
						month++;
						int day=cal.get(Calendar.DAY_OF_MONTH);
						String forderName=""+month+"-"+day;
						
						File folder1 = new File("/storage/sdcard0/TouchTable/"+forderName);
						if (!folder1.exists()) {
							folder1.mkdir();
						}
						
						File folder2 = new File("/storage/sdcard0/TouchTable/"+forderName+"/file");
						if (!folder2.exists()) {
							folder2.mkdir();
						}

						File file = new File("/storage/sdcard0/TouchTable/"+forderName+"/file/"
								+ fileName);
						FileOutputStream fos = new FileOutputStream(file);

						int totalCount = 0;
						int readCount = 0;
						Log.d("ver2.test", "<FILERECEIVE>파일받기 시작:" + fileName);
						// 파일 받기 시작
						int temp = 0;
						while (true) {
							// Log.d("test11","filesize:"+fileSize+"total:"+totalCount+"count:"+temp++
							// +"flag:"+fileReadBuf[0]);
							if (totalCount >= fileSize) {
								break;
							} else if (totalCount + BUFSIZE <= fileSize) {
								readCount = dis.read(fileReadBuf, 0, BUFSIZE);
								fos.write(fileReadBuf, 0, readCount);

								totalCount += readCount;
							} else if (totalCount + BUFSIZE > fileSize) {
								int remainder = fileSize - totalCount;
								readCount = dis.read(fileReadBuf, 0, remainder);
								fos.write(fileReadBuf, 0, readCount);

								totalCount += readCount;
							}
						}
						fos.close();
						isFirstLoop = false;
						Message msg=new Message();
						Bundle bundle=new Bundle();
						bundle.putString("filename", fileName);
						msg.setData(bundle);
						msg.what=1;
						handler.sendMessage(msg);
						Log.d("ver2.test", "<FILERECEIVE>파일받기 종료:" + fileName);
						// 파일 받기 종료

					}
					closeSocket();
					Log.d("ver2.test", " FileReceiver Thread finish");
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
		};
		thread.start();
	}
}
