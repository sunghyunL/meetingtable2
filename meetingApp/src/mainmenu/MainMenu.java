package mainmenu;

import java.io.File;

import receivefilebrowser.ReceiverFileBrowser_main;

import network.ExternalTask.acceptServer;
import network.socket.SocketControl;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.Toast;
import captureImage.UI_CaptureImage_main;

import com.example.meetingapp.R;

import fileTransfer.document.UI_Document_main;
import fileTransfer.gallery.UI_GalleryActivity;

public class MainMenu extends Activity {
	public static SocketControl socketcontrol;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.mainmenu);
		
		Button btn1=(Button)findViewById(R.id.button_mainmenu_gallery);
		btn1.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				Intent i=new Intent(getApplicationContext(), UI_GalleryActivity.class);
				startActivity(i);
			}
		});
		
		Button btn2=(Button)findViewById(R.id.button_mainmenu_document);
		btn2.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				Intent i=new Intent(getApplicationContext(), UI_Document_main.class);
				startActivity(i);
			}
		});
		
		Button btn3=(Button)findViewById(R.id.button_captureimage);
		btn3.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				Intent i=new Intent(getApplicationContext(), UI_CaptureImage_main.class);
				startActivity(i);
			}
		});
		
		Button btn4=(Button)findViewById(R.id.button_receivefilebrowser);
		btn4.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				Intent i=new Intent(getApplicationContext(), ReceiverFileBrowser_main.class);
				startActivity(i);
			}
		});
		
		//서버시작
		new acceptServer(this,handler).startServer();
		
		//루트 디렉토리 생성
		File folder = new File("/storage/sdcard0/TouchTable");
		if (!folder.exists()) {
			folder.mkdir();
		}
	}
	
	Handler handler=new Handler(){

		@Override
		public void handleMessage(Message msg) {
			if(msg.what==0){
				Toast.makeText(MainMenu.this, "연락처가 저장되었습니다.", Toast.LENGTH_SHORT).show();
			}else if(msg.what==1){
				String filename=msg.getData().getString("filename");
				Toast.makeText(MainMenu.this, filename+" 전송이 완료되었습니다.", Toast.LENGTH_SHORT).show();
			}
		}
	};
	
}
