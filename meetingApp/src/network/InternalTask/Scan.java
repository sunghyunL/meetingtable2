package network.InternalTask;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;

public class Scan extends InternalJob {
	private Handler handler;
	private int CAPTUREBUFSIZE =1400;
	private byte stringBuf[]=new byte[STRINGBUFSIZE];
	private byte fileBuf[]=new byte[CAPTUREBUFSIZE];
	
	public Scan(Handler handler){
		this.handler=handler;
	}

	@Override
	public void job_Start() {

		Thread thread = new Thread() {
			@Override
			public void run() {
				makeSocket(); // 서버에 접속
				String sendstr="SCAN&";
				sendStringProtocol(sendstr);
				
				receiveStringProtocol(dis, stringBuf); //파일사이즈와 캡쳐파일 여부 받기
				String receiveStr[] = new String(stringBuf).split("&");
				
				int fileSize = Integer.valueOf(receiveStr[0]);
				String filename = receiveStr[1];
		

				int totalCount = 0;
				int 	readCount = 0;// temp = 0;
					while (true) {
						/*
						 * Log.d("ver2.test", "filesize:" + fileSize +
						 * "total:" + totalCount + "count:" + temp++ +
						 * "flag:" + fileBuf[0]);
						 */
						if (totalCount >= fileSize) {
							break;
						} else if (totalCount + CAPTUREBUFSIZE <= fileSize) {
							readCount = dis.read(fileBuf, 0,
									CAPTUREBUFSIZE);
							capturebos.write(fileBuf, 0, readCount);

							totalCount += readCount;
						} else if (totalCount + CAPTUREBUFSIZE > fileSize) {
							int remainder = fileSize - totalCount;
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
				
				closeSocket();
			}
		};
		thread.start();
	}

}
