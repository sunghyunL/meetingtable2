package function.touch;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.content.Context;
import android.graphics.Bitmap;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.view.animation.Animation;
import android.view.animation.AnimationSet;
import android.view.animation.TranslateAnimation;
import android.widget.AbsoluteLayout;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemLongClickListener;
import android.widget.ImageView;
import fileTransfer.gallery.GalleryCacheMap;

@SuppressLint("NewApi")
public class MyTouchListener implements OnTouchListener,OnItemLongClickListener{
	private int SUMNAIL_SIZE=128;
	private float startPointX,startPointY,lastPointX,lastPointY,prevMovePoint=0.0f;
	private AbsoluteLayout scrollLayout;
	private boolean isBeforeDown;
	private int deltaArrCount=0,scrollDeltaSampleCount=3; //��ũ�� ���ӵ� ���� delta���� � ��� �� ������
	private float scrollDelta[]=new float[scrollDeltaSampleCount]; //��ũ���� ���ӵ��� ���ϱ� ���� �迭
	private AnimationSet aniSet;
	private Handler handler;
	private ImageView sendImage;
	private GalleryCacheMap cacheMap;
	private Context context;
	private ImageView arrowImage;
	private int imagePosition;
	public void setCacheMap(GalleryCacheMap cacheMap){
		this.cacheMap=cacheMap;
	}
	
	public MyTouchListener(AbsoluteLayout scrollLayout,Handler handler,ImageView sendImage,ImageView arrowImage,Context context){
		this.scrollLayout=scrollLayout;
		this.handler=handler;
		this.sendImage=sendImage;
		this.context=context;
		this.arrowImage=arrowImage;
		makeAnimationSet();
	}
	
	public boolean onTouch(View v, MotionEvent event) {
		
		int action = event.getAction();
		int pureaction = action & MotionEvent.ACTION_MASK;
		/*
		final static String[] arAction = {
			"DOWN", "UP", "MOVE", "CANCEL", "OUTSIDE",
			"PDOWN", "PUP"
		}; //��ġ ������ �̺�Ʈ �ε���
		*/
		switch(pureaction){
		case 0: //down
			gridOnTouchDown(event);
			break;
		case 1: //up
			gridOnTouchUp(event);
			break;
		case 2: //move
			gridOnTouchMove(event);
			break;
		
		}
	
		//return mDetector.onTouchEvent(event); //������ ���� ȣ�����ش� down�� ���ؼ�
		return false;
	}
	
	private void makeAnimationSet(){
		aniSet=new AnimationSet(true);
	}
	
	private void gridOnTouchUp(MotionEvent event){
		lastPointX=event.getX(0);
		lastPointY=event.getY(0);
		arrowImage.setVisibility(ImageView.INVISIBLE);
		/***********�ֱ� 3���� move��ǥ�� ���� delta�� ����� ����************/
		float total=0;
		for(float i:scrollDelta){
			total+=i;
		}
		float average=total/scrollDeltaSampleCount;
		/******************************************************/
		
		//���ӵ� ���� 
		if(average>60.0f&&!isBeforeDown){ //���ӵ� ������ �����Ǹ� lastPoint�� ���� �̹����� Y������ �����δ�.
			TranslateAnimation trans=new TranslateAnimation(
					Animation.RELATIVE_TO_PARENT,0,
					Animation.RELATIVE_TO_PARENT,0,
					Animation.RELATIVE_TO_PARENT,0,
					Animation.RELATIVE_TO_PARENT,-1
					);
			trans.setDuration(800);
			aniSet.addAnimation(trans);
			
			scrollLayout.startAnimation(aniSet);
			Bundle bundle=new Bundle();
			bundle.putInt("position", imagePosition);
			Message message=new Message();
			message.setData(bundle);
			message.what=1;
			
			handler.sendMessageDelayed(message, 800);
		}else{
			sendImage.setVisibility(View.INVISIBLE);
			handler.sendEmptyMessage(1);
		}
		Log.d("test9","gridOnTouchUp");
	}
	
	private void gridOnTouchDown( MotionEvent event) {
		startPointX=event.getX();
		startPointY=event.getY();
		prevMovePoint=startPointY;
		isBeforeDown=true;
		Log.d("test9","gridOnTouchDown"+"x:"+startPointX+"y:"+startPointY);
	}
	
	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	@SuppressLint("NewApi")
	private void gridOnTouchMove( MotionEvent event) {
		isBeforeDown=false;
		
		scrollLayout.setX(event.getX(0)-SUMNAIL_SIZE);
		scrollLayout.setY(event.getY(0)-SUMNAIL_SIZE);
		
		//��ũ���� ��Ÿ���� ��������� ����
		
		scrollDelta[deltaArrCount]=prevMovePoint-event.getY(0);//y�࿡���� ��Ÿ�� ����
		deltaArrCount=(++deltaArrCount)%scrollDeltaSampleCount;
		
		prevMovePoint=event.getY(0);
		Log.d("test9","gridOnTouchMove"+"x:"+event.getX(0)+"y:"+event.getY(0));
	}

	@SuppressLint("NewApi")
	@Override
	public boolean onItemLongClick(AdapterView<?> arg0, View arg1,
			int position, long arg3) {
		imagePosition=position;
		arrowImage.setVisibility(ImageView.VISIBLE);
		/************��Ŭ���� ��ũ�� �̹��� ����****************/
		Bitmap bitmap=cacheMap.getItem(position);
		sendImage.setImageBitmap(bitmap);
		sendImage.setVisibility(View.VISIBLE);
		
		scrollLayout.setX(startPointX-SUMNAIL_SIZE);
		scrollLayout.setY(startPointY-SUMNAIL_SIZE);
		/********************************************/
		Log.d("test9","onItemLongClick");
		return true;
	}

}
