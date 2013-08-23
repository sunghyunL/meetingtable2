package fileTransfer.gallery;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.Queue;


import android.annotation.SuppressLint;
import android.content.Context;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.os.Handler;
import android.support.v4.util.LruCache;
import android.util.Log;

@SuppressLint("NewApi")
public class GalleryCacheMap {

Queue<Integer> oderQueue=new LinkedList<Integer>();
private HashMap<Integer, Bitmap> map;
private int MAXSIZE = 15;
private CacheMapThread thread;

	public GalleryCacheMap(Handler handler,Cursor cursor,Context context) {
		map = new HashMap<Integer, Bitmap>();
		
		this.thread=new CacheMapThread(handler, this, cursor);
		thread.setPriority(Thread.MAX_PRIORITY);
		thread.start();
	}

	public Bitmap getItem(int position) {
		Integer tempint = new Integer(position);
		//if (map.containsKey(tempint)) {
		if (map.containsKey(tempint)) {
		//if (map.get(tempint)==null) {
			return map.get(tempint);
		} else { //캐시 맵에 없다면 큐에 넣고 만들어 주기를 기다림
			thread.gridEnQueue(position);
			return null;
		}
	}

	public void setItem(int key, Bitmap value) {
		Log.d("setItem","setItemBefore:"+key);
		if(map.containsKey(key)){
		//if (map.get(key)==null) {
			map.put(key, value);
			Log.d("setItem","setItem:"+key);
		}
	}
	
	public int getCount(){
		return thread.getFileNameList().size();
	}
	
	public void pushDumyItem(int key){
		if (map.size() > MAXSIZE) { //캐시맵의 개수는 제한되어 있음 넘으면 bitmap을 삭제
			Integer tempint = oderQueue.poll();
			Log.d("mtest","poll:"+tempint);
			/*Bitmap bit=map.get(tempint);
			Bitmap temp = null;
			if(bit!=null){
				temp=Bitmap.createBitmap(bit);
			}
			if(bit!=temp){
				Log.d("mtest2","recycle:"+tempint);
				bit.recycle();			
			}*/
			map.remove(tempint); // 맵에서 엔트리를 삭제한다
		}
		
		oderQueue.add(key);
		Log.d("mtest","Dumypush:"+key);
		map.put(key, null);
	}
	
	public boolean contains(int position){
		return map.containsValue(position);
	}
	
	public String getTargetFileName(int position){
		return thread.getFileNameList().get(position);
	}
	
	public void removeItem(int key){
		map.remove(key);
	}
}
