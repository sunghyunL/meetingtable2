package network.InternalTask;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import captureImage.UI_CaptureImage_main;

public class CaptureDisplay extends InternalJob {
	UI_CaptureImage_main activity;
	Handler handler;
	int CAPTUREBUFSIZE =1400;
	boolean loopFlag;
	byte stringBuf[]=new byte[STRINGBUFSIZE];
	byte fileBuf[]=new byte[CAPTUREBUFSIZE];
	
	public CaptureDisplay(UI_CaptureImage_main activity,Handler handler){
		this.activity=activity;
		this.handler=handler;
		loopFlag=true;
	}

	@Override
	public void job_Start() {
		// TODO Auto-generated method stub
		Thread thread=new Thread(){

			@Override
			public void run() {
				makeSocket(); //서버에 접속
				sendStringProtocol("CAPTUREDISPLAY&"); //서버 Thread시작
				ByteArrayOutputStream bos=new ByteArrayOutputStream(1024*100);//defaultsize: 100kb
				ByteArrayOutputStream capturebos=new ByteArrayOutputStream(1024*300);
				try {
					int totalCount = 0, readCount = 0;
					while (loopFlag) {
						String sendStr = activity.getRotate() + "&"
								+ activity.getIsMenu() + "&" +activity.getIsTouch()+"&"+
								activity.getTouchX()+"&"+activity.getTouchY()+"&";
						sendStringProtocol(sendStr);
						if(activity.getIsTouch()){
							activity.setIsTouch(false);
						}
						/******** 이미지 받기 시작 **********/
						receiveStringProtocol(dis, stringBuf); //파일사이즈와 캡쳐파일 여부 받기
						String receiveStr[] = new String(stringBuf).split("&");

						int fileSize = Integer.valueOf(receiveStr[0]);
						boolean isCapture = Boolean.valueOf(receiveStr[1]);
						Log.d("ver2.test","isCapture:"+receiveStr[1]);
						if (isCapture) { // 캡쳐 파일이 있을 경우

							int captureFileSize = Integer
									.valueOf(receiveStr[2]);

							capturebos.reset();

							totalCount = 0;
							readCount = 0;// temp = 0;
							while (true) {
								/*
								 * Log.d("ver2.test", "filesize:" + fileSize +
								 * "total:" + totalCount + "count:" + temp++ +
								 * "flag:" + fileBuf[0]);
								 */
								if (totalCount >= captureFileSize) {
									break;
								} else if (totalCount + CAPTUREBUFSIZE <= captureFileSize) {
									readCount = dis.read(fileBuf, 0,
											CAPTUREBUFSIZE);
									capturebos.write(fileBuf, 0, readCount);

									totalCount += readCount;
								} else if (totalCount + CAPTUREBUFSIZE > captureFileSize) {
									int remainder = captureFileSize - totalCount;
									readCount = dis.read(fileBuf, 0, remainder);
									capturebos.write(fileBuf, 0, readCount);

									totalCount += readCount;
								}
							}
							byte imageByte[] = capturebos.toByteArray();
							Bundle bundle = new Bundle();
							bundle.putByteArray("captureimage", imageByte);
							Message message = new Message();
							message.what = 3;
							message.setData(bundle);
							handler.sendMessage(message);
						}

						bos.reset();
						
						totalCount = 0; readCount = 0;// temp = 0;
						while (true) {
						/*	Log.d("ver2.test", "filesize:" + fileSize
									+ "total:" + totalCount + "count:" + temp++
									+ "flag:" + fileBuf[0]);*/
							if (totalCount >= fileSize) {
								break;
							} else if (totalCount + CAPTUREBUFSIZE <= fileSize) {
								readCount = dis.read(fileBuf, 0, CAPTUREBUFSIZE);
								bos.write(fileBuf, 0, readCount);

								totalCount += readCount;
							} else if (totalCount + CAPTUREBUFSIZE > fileSize) {
								int remainder = fileSize - totalCount;
								readCount = dis.read(fileBuf, 0, remainder);
								bos.write(fileBuf, 0, readCount);

								totalCount += readCount;
							}
						}
						byte imageByte[] = bos.toByteArray();
						Bundle bundle = new Bundle();
						bundle.putByteArray("image", imageByte);
						Message message = new Message();
						message.what=0;
						message.setData(bundle);
						handler.sendMessage(message);
						/******************************/
					}
					bos.close();
				} catch (Exception e) {
					e.printStackTrace();
					activity.finish();
				}
			}
		};
		thread.start();
	}
	
	public void finish(){
		sendStringProtocol("1&STOP&STOP");
		//try {
			//Thread.sleep(1000);
		//} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
		//}
		loopFlag=false;
		closeSocket();
	}
}
