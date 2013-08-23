package captureImage;

import java.io.File;
import java.io.FileOutputStream;
import java.util.Calendar;

import network.InternalTask.CaptureDisplay;
import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.hardware.Camera;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.Animation.AnimationListener;
import android.view.animation.AnimationSet;
import android.view.animation.LinearInterpolator;
import android.view.animation.ScaleAnimation;
import android.view.animation.TranslateAnimation;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.SeekBar;
import android.widget.SeekBar.OnSeekBarChangeListener;

import com.example.meetingapp.R;

public class UI_CaptureImage_main extends Activity {
	private ImageView captureImage;
	private ImageView captureImageFront;
	private Button menu;
	private SeekBar seekbar;
	private CaptureDisplay capturedisplay;
	private boolean isMenu=false;
	private boolean isTouch=false;
	private int touchX=0,touchY=0;
	private Camera camera;
	private Camera.Parameters params;
	private boolean flashFlage=false;
	private boolean isStart=false;
	private SensorManager mSm;
	private float azimuth=10.0f,pitch=10.0f,roll=10.0f;
	private static int adAngle=0;
	private AnimationSet ani1;
	private AnimationSet ani2;
	private SoundPool sp;
	private int soundid;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.captureimagemain);
		captureImage=(ImageView)findViewById(R.id.ImageView_captureImage);
		captureImageFront=(ImageView)findViewById(R.id.imageView_Frontcaptureimage);
		menu=(Button)findViewById(R.id.Button_captureImage);
		seekbar=(SeekBar)findViewById(R.id.seekbar_captureimage);
		
		seekbar.setProgress(adAngle);
		seekbar.setOnSeekBarChangeListener(new OnSeekBarChangeListener() {
			
			@Override
			public void onStopTrackingTouch(SeekBar seekBar) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void onStartTrackingTouch(SeekBar seekBar) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void onProgressChanged(SeekBar seekBar, int progress,
					boolean fromUser) {
				// TODO Auto-generated method stub
				adAngle=progress;
			}
		});
		
		menu.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				if(isMenu){
					isMenu=false;
				}else{
					isMenu=true;
				}
			}
		});
		mainLoop.start();
		
		mSm = (SensorManager)getSystemService(Context.SENSOR_SERVICE);
		makeAniSet();
		sp = new SoundPool(1, AudioManager.STREAM_MUSIC, 0); //soundpool 생성
        soundid = sp.load(this, R.raw.camera, 1); //typing id를 lo
        
	}
	
	@Override
	public boolean onTouchEvent(MotionEvent event) {
		
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
	
			break;
		case 1: //up
			gridOnTouchUp(event);
			break;
		case 2: //move
	
			break;
		
		}
		return true;
	}
	
	
	private void gridOnTouchUp(MotionEvent event){
		touchX=(int)event.getX(0);
		touchY=(int)event.getY(0);
		
		isTouch=true;
	}

	@Override
	protected void onResume() {
		super.onResume();
		int delay = SensorManager.SENSOR_DELAY_UI;
		mSm.registerListener(mSensorListener, 
				mSm.getDefaultSensor(Sensor.TYPE_ORIENTATION), delay);
	}
	
	
	@Override
	protected void onPause() {
		super.onPause();
		Log.d("ver2.test","onPause");
		mSm.unregisterListener(mSensorListener);
		turnOffFalsh();
	}
	
	 SensorEventListener mSensorListener = new SensorEventListener() {
			public void onAccuracyChanged(Sensor sensor, int accuracy) {
				// 특별히 처리할 필요없음
			}

			public void onSensorChanged(SensorEvent event) {
				// 신뢰성없는 값은 무시
				if (event.accuracy == SensorManager.SENSOR_STATUS_UNRELIABLE) {
					//return;
				}
				
				float[] v = event.values;
				switch (event.sensor.getType()) {
				case Sensor.TYPE_ORIENTATION:
					azimuth=v[0];
					pitch=v[1];
					roll=v[2];
					if(isStart){
						if(Math.abs(pitch)>3.0f&&Math.abs(roll)>3.0f){
							isStart=false;
							finish();
						}
					}
					break;
				}
			}
	 };

	Thread mainLoop=new Thread(){
		@Override
		public void run() {
			while(!isHorizontalDevice()); //수평이 아닐 경우 계속 대기  
			turnOnFlash();
			capturedisplay=new CaptureDisplay(UI_CaptureImage_main.this, handler);
			capturedisplay.job_Start();
		}
	};
	
	private boolean isHorizontalDevice(){ //구현 요망
/*		if(Math.abs(pitch)<3.0f&&Math.abs(roll)<3.0f){
			isStart=true;
			return true;
		}else 
			return false;*/
		return true;
	}
	
	private void turnOnFlash(){
		flashFlage=true;
		camera = Camera.open();
		params =  camera.getParameters();    
		params.setFlashMode(Camera.Parameters.FLASH_MODE_TORCH);
		camera.setParameters(params);
		handler.sendEmptyMessage(1);
	}
	
	private void turnOffFalsh(){
		flashFlage=false;
		handler.removeMessages(1);
		params.setFlashMode(Camera.Parameters.FLASH_MODE_OFF);
		camera.setParameters(params);
		camera.release();
	}
	
	public int getRotate(){
		return (int)azimuth+adAngle;
	}
	
	public boolean getIsMenu(){
		return isMenu;
	}
	
	public boolean getIsTouch(){
		return isTouch;
	}
	
	public void setIsTouch(boolean istouch){
		isTouch=istouch;
	}

	public int getTouchX(){
		return touchX;
	}
	
	public int getTouchY(){
		return touchY;
	}
	
	Handler handler=new Handler(){
		@Override
		public void handleMessage(Message msg) {
			if (msg.what == 0) {
				byte[] imageByte = msg.getData().getByteArray("image");
				Bitmap bitmap = BitmapFactory.decodeByteArray(imageByte, 0,
						imageByte.length);
				captureImage.setImageBitmap(bitmap);
			}else if(msg.what==1){
				camera.setParameters(params);
				if(flashFlage)
					handler.sendEmptyMessageDelayed(1, 2000);
			}else if(msg.what==3){//캡쳐이미지 뿌려주기

				byte[] imageByte = msg.getData().getByteArray("captureimage");
				Bitmap bitmap = BitmapFactory.decodeByteArray(imageByte, 0,
						imageByte.length);
				captureImageFront.setImageBitmap(bitmap);
				//captureImageFront.setVisibility(ImageView.VISIBLE);
				captureImageFront.startAnimation(ani1);
				
				//캡쳐 소리 재생
				 sp.play(soundid, 2, 2, 0, 0, 1);
				 
				//이미지 파일로 저장
				Calendar cal=Calendar.getInstance();
				int month=cal.get(Calendar.MONTH);
				month++;
				int day=cal.get(Calendar.DAY_OF_MONTH);
				int time=cal.get(Calendar.MILLISECOND);
				String forderName=""+month+"."+day;
				
				File folder1 = new File("/storage/sdcard0/TouchTable/"+forderName);
				if (!folder1.exists()) {
					folder1.mkdir();
				}
				
				File folder2 = new File("/storage/sdcard0/TouchTable/"+forderName+"/capture");
				if (!folder2.exists()) {
					folder2.mkdir();
				}

				File file = new File("/storage/sdcard0/TouchTable/"+forderName+"/capture/"
						+ time+".jpg");
				try {
					FileOutputStream fos = new FileOutputStream(file);
					fos.write(imageByte);
					fos.close();
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
		}
	};
	
	@Override
	public void finish() {
		super.finish();
		Log.d("ver2.test","onfinish");
		capturedisplay.finish();
	}
	
	private void makeAniSet(){
		ani1=new AnimationSet(true);
		ani1.setInterpolator(new LinearInterpolator());
		
		ScaleAnimation scale=new ScaleAnimation(1.0f, 0.8f, 1.0f,
				0.8f, ScaleAnimation.RELATIVE_TO_SELF,
				0.5f, ScaleAnimation.RELATIVE_TO_SELF, 0.5f);
		scale.setDuration(500);
		ani1.addAnimation(scale);
		AlphaAnimation alpha=new AlphaAnimation(0.0f, 1.0f);
		alpha.setDuration(500);
		ani1.addAnimation(alpha);
		
	
		ani2=new AnimationSet(true);
		ani2.setInterpolator(new LinearInterpolator());
		TranslateAnimation trans=new TranslateAnimation(TranslateAnimation.RELATIVE_TO_SELF, 
			0.0f, TranslateAnimation.RELATIVE_TO_SELF, 0.5f, 
		TranslateAnimation.RELATIVE_TO_SELF, 0.0f, 
		TranslateAnimation.RELATIVE_TO_SELF, 0.5f);
		//TranslateAnimation tran2s=new TranslateAnimation(fromXType, fromXValue,
		//		toXType, toXValue, fromYType, fromYValue, toYType, toYValue)

		trans.setDuration(500);
		ani2.addAnimation(trans);
		AlphaAnimation alpha2=new AlphaAnimation(1.0f, 0.0f);
		alpha2.setDuration(500);
		ani2.addAnimation(alpha2);
		ScaleAnimation scale2=new ScaleAnimation(1.0f, 0.3f, 1.0f, 0.3f);
		scale2.setDuration(500);
		ani2.addAnimation(scale2);
		
		ani1.setAnimationListener(new AnimationListener() {
			
			@Override
			public void onAnimationStart(Animation animation) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void onAnimationRepeat(Animation animation) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void onAnimationEnd(Animation animation) {
				// TODO Auto-generated method stub
				captureImageFront.startAnimation(ani2);
			}
		});
	}
}
