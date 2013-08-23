package fileTransfer.gallery;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.Queue;

import android.annotation.SuppressLint;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Handler;
import android.provider.MediaStore;
import android.util.Log;



@SuppressLint("NewApi")
public class CacheMapThread extends Thread {
	private Queue<Integer> queue=new LinkedList<Integer>();
	
	private Handler handler;
	private ArrayList<String> fileNameList;
	private BitmapFactory.Options bfo;
	private GalleryCacheMap cacheMap;
	private String filename;
	private Cursor cursor;
	private int image_data_colunm;
	
	private int MAXSIZE = 15;
	
	public CacheMapThread(Handler handler,GalleryCacheMap cacheMap, Cursor cursor){
		this.handler=handler;
		this.cacheMap=cacheMap;
		fileNameList=new ArrayList<String>();
		this.cursor=cursor;
		loadImageFileNames();
		
		//bfo setting
		bfo = new BitmapFactory.Options();
		bfo.inSampleSize = 8;
		//bfo.outHeight = 64;
		//bfo.outWidth = 64;
		bfo.inPreferQualityOverSpeed=false;
	}
	
	@Override
	public void run() {
		while(true){
			if(queue.isEmpty()){
				try {
					Log.d("test","wait");
					sleep(10000);
					
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				
			}else{
				// 쿠에있는 포지션을 파악해서 비트맵 만들어서 캐시맵에 넣어주기
				int gridPosition; 
				synchronized (this) {
					gridPosition = queue.poll();
				}
				String targetFileName = fileNameList.get(gridPosition);
				Bitmap bit = BitmapFactory.decodeFile(targetFileName, bfo);
				if(bit==null){
					continue;
				}
				int width=bit.getWidth();
				int height=bit.getHeight();
				int stand=(width>=height)?height:width;
				//Bitmap nbit=Bitmap.createScaledBitmap(bit, 64, 64, false);
				Bitmap nbit=Bitmap.createBitmap(bit, (width/2)-(stand/2), (height/2)-(stand/2), stand, stand);
				cacheMap.setItem(gridPosition, nbit);
				handler.sendEmptyMessage(0);
				// 어댑터에 바뀌었다는 것을 알려주기 위해서 핸들러 호출
				
			}
		}
	}
	
	public void gridEnQueue(int position){
		if(!cacheMap.contains(position)){ //큐에 중복된 수를 넣지 않기 위함
			if (queue.size() > MAXSIZE){
				int temp;
				synchronized (this) {
					temp=queue.poll();
				}
				cacheMap.removeItem(temp);
			}
			cacheMap.pushDumyItem(position);
			queue.add(position);
			this.interrupt();
		}
	}
	
	public int gridPollQueue(){
		return queue.poll();
	}
	
	private void loadImageFileNames(){ //이미지 파일이름 로드하기
		image_data_colunm = cursor
				.getColumnIndexOrThrow(MediaStore.Images.Media.DATA); //파일 이름의 컬럼 인덱스

		cursor.moveToFirst();
		while (cursor.moveToNext()) { 
			String filename = cursor.getString(image_data_colunm);
			fileNameList.add(filename);
		}
	}	
	
	public ArrayList<String> getFileNameList() {
		return fileNameList;
	}
	
}
