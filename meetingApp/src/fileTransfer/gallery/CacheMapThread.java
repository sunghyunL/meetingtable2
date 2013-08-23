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
				// ���ִ� �������� �ľ��ؼ� ��Ʈ�� ���� ĳ�øʿ� �־��ֱ�
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
				// ����Ϳ� �ٲ���ٴ� ���� �˷��ֱ� ���ؼ� �ڵ鷯 ȣ��
				
			}
		}
	}
	
	public void gridEnQueue(int position){
		if(!cacheMap.contains(position)){ //ť�� �ߺ��� ���� ���� �ʱ� ����
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
	
	private void loadImageFileNames(){ //�̹��� �����̸� �ε��ϱ�
		image_data_colunm = cursor
				.getColumnIndexOrThrow(MediaStore.Images.Media.DATA); //���� �̸��� �÷� �ε���

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
