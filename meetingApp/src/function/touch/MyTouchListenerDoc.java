package function.touch;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Context;
import android.os.Build;
import android.os.Handler;
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
import android.widget.LinearLayout;

import com.example.meetingapp.R;

import fileTransfer.gallery.GalleryCacheMap;

@SuppressLint("NewApi")
public class MyTouchListenerDoc implements OnTouchListener,OnItemLongClickListener{
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
	
	private AbsoluteLayout scrollLayoutAdded;
	private ImageView sendImageAdded;

	public void setLayoutAdded(AbsoluteLayout scrollLayoutAdded){
		this.scrollLayoutAdded=scrollLayoutAdded;
	}
	
	public void setsendImageAdded(ImageView sendImageAdded){
		this.sendImageAdded=sendImageAdded;
	}
	
	public void setCacheMap(GalleryCacheMap cacheMap){
		this.cacheMap=cacheMap;
	}

	public MyTouchListenerDoc(AbsoluteLayout scrollLayout,ImageView sendImage,Context context,Handler handler){
		this.scrollLayout=scrollLayout;
		this.handler=handler;
		this.sendImage=sendImage;
		this.context=context;
		
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
			//gridOnTouchMove(event);
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
			handler.sendEmptyMessageDelayed(1, 800);
		}else{
			handler.sendEmptyMessage(1);
		}
		Log.d("test9","gridOnTouchUp");
	}
	
	private void gridOnTouchDown( MotionEvent event) {
		startPointX=event.getXPrecision();
		startPointY=event.getYPrecision();
		prevMovePoint=startPointY;
		isBeforeDown=true;
		Log.d("test9","gridOnTouchDown"+"x:"+startPointX+"y:"+startPointY);
	}
	
	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	@SuppressLint("NewApi")
	private void gridOnTouchMove( MotionEvent event) {
		//�׸��� ���� �̹����� �ٷ� ����� ����Ʈ ���� �̹����� ���̰� �Ѵ�.
		sendImageAdded.setVisibility(ImageView.INVISIBLE);
		sendImage.setVisibility(ImageView.VISIBLE);
		
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
	public boolean onItemLongClick(AdapterView<?> parent, View view,
			int position, long arg3) {
		
		/************��Ŭ���� ��ũ�� �̹��� ����****************/
		//0:pptx,1:docx,2:pdf,3:txt
		/*switch(arrlist.get(position).getXxx()){			
			case 0:
				sendImageAdded.setImageResource(R.drawable.pptx);
				sendImage.setImageResource(R.drawable.pptx);
				break;
			case 1:
				sendImageAdded.setImageResource(R.drawable.docx);
				sendImage.setImageResource(R.drawable.docx);
				break;
			case 2:
				sendImageAdded.setImageResource(R.drawable.pdf);
				sendImage.setImageResource(R.drawable.pdf);
				break;
			case 3:
				sendImageAdded.setImageResource(R.drawable.txt);
				sendImage.setImageResource(R.drawable.txt);
				break;
		}
		
		//�׸��忡 �پ��ִ� ���̾ƿ��� �����ϰ�, �׸��忡 �پ��ִ� �̹����� ���̰� �Ѵ�.
		scrollLayoutAdded.setX(startPointX-SUMNAIL_SIZE);
		scrollLayoutAdded.setY(startPointY-SUMNAIL_SIZE);
		
		sendImageAdded.setVisibility(ImageView.VISIBLE);*/
		
		ImageView image=new ImageView(context);
		image.setImageResource(R.drawable.pptx);
		
		image.setX(startPointX-SUMNAIL_SIZE);
		image.setY(startPointY-SUMNAIL_SIZE);
		
		LinearLayout.LayoutParams pram=new LinearLayout.LayoutParams(500	, 500);
		
	
		
		/********************************************/
		
		Log.d("test9","onItemLongClick");
		return true;
	}
}
