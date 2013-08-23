package fileTransfer.document;

import java.io.File;
import java.util.Calendar;

import network.InternalTask.FileTrans;
import android.app.Activity;
import android.content.Intent;
import android.graphics.drawable.AnimationDrawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.MotionEvent;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.example.meetingapp.R;

import function.touch.MyTouchListenerDetailView;

public class UI_Document_detail extends Activity {
	private LinearLayout scrollLayout;
	private ImageView imageview;
	private TextView textview_name;
	private TextView textview_modify;
	private TextView textview_capacity;
	private File file;
	private MyTouchListenerDetailView mlistener;
	/*************for arrow animation***********/
	private ImageView arrowImage;
	private AnimationDrawable anim;
	/*******************************************/
	private String pathname;
	private String filename;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.document_detail);
		scrollLayout=(LinearLayout)findViewById(R.id.linearlayout_document_detail);
		imageview=(ImageView)findViewById(R.id.imageview_document_detail);
		
		textview_name=(TextView)findViewById(R.id.textview_document_detail_name);
		textview_capacity=(TextView)findViewById(R.id.textview_document_detail_capacity);
		textview_modify=(TextView)findViewById(R.id.textview_document_detail_modify);
		
		Intent i = getIntent();
		Bundle extras = i.getExtras();
		pathname=extras.getString("pathname");
		filename=extras.getString("filename"); 
		
		file = new File(pathname+filename);
		Log.d("test","mark1:"+pathname+filename);
		setFileInfo(filename);

		//scrollLayout.setOnTouchListener(new MyTouchListenerDetailView(scrollLayout, handler, this));
		mlistener=new MyTouchListenerDetailView(scrollLayout, handler);

		/**************make arrow animation**************/
		arrowImage=(ImageView)findViewById(R.id.imageView_document_detail_arrow);
		anim=(AnimationDrawable)arrowImage.getBackground();
		anim.start();
		/************************************************/
	}
	
	@Override
	public boolean onTouchEvent(MotionEvent event) {
		mlistener.onTouch(null, event);
		return false;
	}

	private void setFileInfo(String filename){
		//0:pptx,1:docx,2:pdf,3:txt
		
		Calendar cal=Calendar.getInstance();
		cal.setTimeInMillis(file.lastModified());
		
		if(filename.endsWith("pptx")){
			imageview.setImageResource(R.drawable.pptx);
		}else if(filename.endsWith("docx")){
			imageview.setImageResource(R.drawable.docx);
		}else if(filename.endsWith("pdf")){
			imageview.setImageResource(R.drawable.pdf);
		}else if(filename.endsWith("txt")){
			imageview.setImageResource(R.drawable.txt);
		}
		
		textview_name.setText(filename);
		textview_modify.setText(cal.getTime().toGMTString());
		textview_capacity.setText(""+file.length());	
	}
	
	Handler handler=new Handler(){
		@Override
		public void handleMessage(Message msg) {
			FileTrans.makeFileSendMessage(0,pathname+filename,filename); //큐에 작업을 넣어주고 작업을 시작한다.
			FileTrans.demand_job_Start();
			
			finish();
			overridePendingTransition(0, R.anim.animation_scoll_exit);
			//파일 전송 메시지를 큐에 넣어준다.
		}
	};
}
