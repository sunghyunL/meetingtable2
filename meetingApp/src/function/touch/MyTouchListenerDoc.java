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
	private int deltaArrCount=0,scrollDeltaSampleCount=3; //스크롤 가속도 이전 delta값을 몇개 평균 낼 것인지
	private float scrollDelta[]=new float[scrollDeltaSampleCount]; //스크롤의 가속도를 구하기 위한 배열
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
		}; //터치 리스너 이벤트 인덱스
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
	
		//return mDetector.onTouchEvent(event); //감지기 까지 호출해준다 down을 위해서
		return false;
	}
	
	private void makeAnimationSet(){
		aniSet=new AnimationSet(true);
	}
	
	private void gridOnTouchUp(MotionEvent event){
		lastPointX=event.getX(0);
		lastPointY=event.getY(0);
		
		/***********최근 3개의 move좌표에 대한 delta값 평균을 구함************/
		float total=0;
		for(float i:scrollDelta){
			total+=i;
		}
		float average=total/scrollDeltaSampleCount;
		/******************************************************/
		
		//가속도 조건 
		if(average>60.0f&&!isBeforeDown){ //가속도 조건이 만족되면 lastPoint로 부터 이미지를 Y축으로 움직인다.
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
		//그리드 뷰의 이미지는 바로 숨기고 리스트 뷰의 이미지를 보이게 한다.
		sendImageAdded.setVisibility(ImageView.INVISIBLE);
		sendImage.setVisibility(ImageView.VISIBLE);
		
		isBeforeDown=false;
		
		scrollLayout.setX(event.getX(0)-SUMNAIL_SIZE);
		scrollLayout.setY(event.getY(0)-SUMNAIL_SIZE);
		
		//스크롤의 델타값을 계속적으로 갱신
		
		scrollDelta[deltaArrCount]=prevMovePoint-event.getY(0);//y축에대한 델타값 측정
		deltaArrCount=(++deltaArrCount)%scrollDeltaSampleCount;
		
		prevMovePoint=event.getY(0);
		Log.d("test9","gridOnTouchMove"+"x:"+event.getX(0)+"y:"+event.getY(0));
	}

	@SuppressLint("NewApi")
	@Override
	public boolean onItemLongClick(AdapterView<?> parent, View view,
			int position, long arg3) {
		
		/************롱클릭시 스크롤 이미지 생성****************/
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
		
		//그리드에 붙어있는 레이아웃을 조정하고, 그리드에 붙어있는 이미지를 보이게 한다.
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
