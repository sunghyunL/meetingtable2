package scan;


import network.InternalTask.Scan;
import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ProgressBar;

import com.example.meetingapp.R;


public class Scan_main extends Activity {
	 ImageView scanimage;
	 Button scanbutton;
	 ProgressBar progress;
	 
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.scan_main);
		scanimage=(ImageView)findViewById(R.id.ImageView_scan);
		scanbutton=(Button)findViewById(R.id.Button_scan);
		progress=(ProgressBar)findViewById(R.id.progress_scan);
		
		scanbutton.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				progress.setVisibility(View.VISIBLE);
				new Scan(handler);
			}
		});
	}
	
	Handler handler= new Handler(){
		@Override
		public void handleMessage(Message msg) {
			switch(msg.what){
			case 0:
				progress.setVisibility(View.INVISIBLE);
				Bundle bundle=msg.getData();
				byte imageByte[]=bundle.getByteArray("image");
				Bitmap bitmap = BitmapFactory.decodeByteArray(imageByte, 0,
						imageByte.length);
				scanimage.setImageBitmap(bitmap);
				
				break;
			}
		}
	};
}
