package fileTransfer.gallery;

import network.InternalTask.FileTrans;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.AnimationDrawable;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MotionEvent;
import android.widget.ImageView;
import android.widget.LinearLayout;

import com.example.meetingapp.R;

import function.touch.MyTouchListenerDetailView;

public class UI_GalleryDetailView extends Activity {
	private String pathname;
	private ImageView image;
	private BitmapFactory.Options bfo;
	private LinearLayout scrollLayout;
	private MyTouchListenerDetailView mlistener;
	/*************for arrow animation***********/
	private ImageView arrowImage;
	private AnimationDrawable anim;
	/*******************************************/
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		// TODO Auto-generated method stub
		super.onCreate(savedInstanceState);
		System.gc();
		setContentView(R.layout.gallery_detail_view);
		//스크롤 애니메이션을 적용하기 위한 리니어레이아웃
		scrollLayout=(LinearLayout)findViewById(R.id.linearlayout_gallery_detail_view);
		//애니메이션을 위한 터치리스너 생성
		mlistener=new MyTouchListenerDetailView(scrollLayout, handler);
		Intent i = getIntent();
		Bundle extras = i.getExtras();
		BitmapFactory.Options bfo = new BitmapFactory.Options();
		bfo.inSampleSize = 2;
		pathname = extras.getString("filename");
		image = (ImageView) findViewById(R.id.big_image);

		Bitmap bit = BitmapFactory.decodeFile(pathname, bfo);

		image.setImageBitmap(bit);

		/**************make arrow animation**************/
		arrowImage=(ImageView)findViewById(R.id.imageView_gallery_detail_arrow);
		anim=(AnimationDrawable)arrowImage.getBackground();
		anim.start();
		/************************************************/
	}

	@Override
	protected void onDestroy() {
		// TODO Auto-generated method stub
		super.onDestroy();

		Drawable d = image.getDrawable();
		if (d instanceof BitmapDrawable) {
			Bitmap b = ((BitmapDrawable) d).getBitmap();
			b.recycle();
		}
	}

	@Override
	public boolean onTouchEvent(MotionEvent event) {
		mlistener.onTouch(null, event);
		return false;
	}
	
	Handler handler=new Handler(){
		@Override
		public void handleMessage(Message msg) {
			String pathnameList[]=pathname.split("/");
			String filename=pathnameList[pathnameList.length-1];
			FileTrans.makeFileSendMessage(0,pathname,filename); //큐에 작업을 넣어주고 작업을 시작한다.
			FileTrans.demand_job_Start();
			finish();
			overridePendingTransition(0, R.anim.animation_scoll_exit);
		}
	};
}
