package function.touch;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.content.Context;
import android.os.Build;
import android.os.Handler;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.view.animation.AccelerateInterpolator;
import android.view.animation.Animation;
import android.view.animation.AnimationSet;
import android.view.animation.TranslateAnimation;
import android.widget.LinearLayout;

@SuppressLint("NewApi")
public class MyTouchListenerDetailView implements OnTouchListener{
	
	private float startPointX,startPointY,prevMovePoint=0.0f;
	private boolean isBeforeDown;
	private int deltaArrCount=0,scrollDeltaSampleCount=3; //��ũ�� ���ӵ� ���� delta���� � ��� �� ������
	private float scrollDelta[]=new float[scrollDeltaSampleCount]; //��ũ���� ���ӵ��� ���ϱ� ���� �迭
	private AnimationSet aniSet;
	private Handler handler;

	private LinearLayout scrollLayout;
	
	public MyTouchListenerDetailView(LinearLayout scrollLayout,Handler handler){
		this.scrollLayout=scrollLayout;
		this.handler=handler;
	
		
		makeAnimationSet();
	}
	
	public boolean onTouch(View v, MotionEvent event) {
		
		int action = event.getAction();
		int pureaction = action & MotionEvent.ACTION_MASK;
		//Log.d("test9","onTouch:"+pureaction);
		/*
		final static String[] arAction = {
			"DOWN", "UP", "MOVE", "CANCEL", "OUTSIDE",
			"PDOWN", "PUP"
		}; //��ġ ������ �̺�Ʈ �ε���
		*/
		
		Log.d("test9","onTouch:"+pureaction);
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
		return false;
	}
	
	private void makeAnimationSet(){
		aniSet=new AnimationSet(true);
	}
	
	private void gridOnTouchUp(MotionEvent event){
		
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
			
			trans.setDuration(600);
			
			aniSet.addAnimation(trans);
			scrollLayout.startAnimation(aniSet);
			handler.sendEmptyMessageDelayed(1, 450);
		}
	}
	
	private void gridOnTouchDown( MotionEvent event) {
		startPointX=event.getX();
		startPointY=event.getY();
		prevMovePoint=startPointY;
		isBeforeDown=true;
	}
	
	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	@SuppressLint("NewApi")
	private void gridOnTouchMove( MotionEvent event) {
		isBeforeDown=false;
		//��ũ���� ��Ÿ���� ��������� ����
		
		scrollDelta[deltaArrCount]=prevMovePoint-event.getY(0);//y�࿡���� ��Ÿ�� ����
		deltaArrCount=(++deltaArrCount)%scrollDeltaSampleCount;
		
		prevMovePoint=event.getY(0);
	}

}
