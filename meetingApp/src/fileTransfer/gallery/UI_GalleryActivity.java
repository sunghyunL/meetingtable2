package fileTransfer.gallery;

import network.InternalTask.FileTrans;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.LoaderManager;
import android.content.CursorLoader;
import android.content.Intent;
import android.content.Loader;
import android.database.Cursor;
import android.graphics.drawable.AnimationDrawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.provider.MediaStore;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.widget.AbsoluteLayout;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.GridView;
import android.widget.ImageView;

import com.example.meetingapp.R;

import function.touch.MyTouchListener;

@SuppressLint("NewApi")
public class UI_GalleryActivity extends Activity implements
		LoaderManager.LoaderCallbacks<Cursor> {
	GridView myGrid;
	GalleryAdapter myAdapter;
	GalleryCacheMap cacheMap;
	ImageView sendImage;

	private static final String[] PROJECTION = new String[] {
			MediaStore.Images.Media._ID, MediaStore.Images.Media.DATA };

	private static final int LOADER_ID = 1;

	LoaderManager lm;
	AbsoluteLayout scrollLayout;
	private MyTouchListener mlistener;
	/*************for arrow animation***********/
	private ImageView arrowImage;
	private AnimationDrawable anim;
	/*******************************************/
	
	@SuppressLint("NewApi")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.gallery_main);
		myGrid = (GridView) findViewById(R.id.photosender_grid); // �̹� �������ִ�
		// �׸��带 �޾Ƽ�
		sendImage=(ImageView)findViewById(R.id.scroll_image);
		scrollLayout=(AbsoluteLayout)findViewById(R.id.scroll_image_layout);
		
		//����ó ������ ����
		//mDetector = new GestureDetector(this, mGestureListener);
		
		//���Ϸδ� �ʱ�ȭ
		lm = getLoaderManager();
		lm.initLoader(LOADER_ID, null, this);
		
		/**************make arrow animation**************/
		arrowImage=(ImageView)findViewById(R.id.imageView_grid_main_arrow);
		anim=(AnimationDrawable)arrowImage.getBackground();
		anim.start();
		/************************************************/
		
		mlistener=new MyTouchListener(scrollLayout, handler, sendImage, arrowImage ,this);
		
		//Ŭ�� ������ ����
		myGrid.setOnItemClickListener(new OnItemClickListener() {
			public void onItemClick(AdapterView parent, View v, int position,
					long id) {
				// TODO Auto-generated method stub
				Intent intent = new Intent(getApplicationContext(),
						UI_GalleryDetailView.class);
				intent.putExtra("filename", cacheMap.getTargetFileName(position));
				startActivity(intent);
			}
		});
		
		//��Ŭ�� ������ ����
		myGrid.setOnItemLongClickListener(mlistener);
		
		//��ġ ������ ����
		myGrid.setOnTouchListener(mlistener);

	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	/********** LoaderCallbacks ******************/
	@Override
	public Loader<Cursor> onCreateLoader(int id, Bundle arg1) {
		// TODO Auto-generated method stub
		return new CursorLoader(UI_GalleryActivity.this,
				MediaStore.Images.Media.EXTERNAL_CONTENT_URI, PROJECTION, null,
				null, null);
	}

	
	//���Ϸδ� �Ϸ� ���� �� ȣ��Ǵ� �Լ�
	@Override
	public void onLoadFinished(Loader<Cursor> loader, Cursor cursor) {
		switch (loader.getId()) {
		case LOADER_ID:
			//���۷�ƾ ĳ�ø� ����, ť������ ����, ����� ����, ������ ����
			cacheMap=new GalleryCacheMap(handler,cursor,this);
			
			myAdapter = new GalleryAdapter(UI_GalleryActivity.this, cacheMap);
			myGrid.setAdapter(myAdapter);
			
			mlistener.setCacheMap(cacheMap);
			
			break;
		}

	}

	@Override
	public void onLoaderReset(Loader<Cursor> arg0) {
		// TODO Auto-generated method stub

	}
	/*******************************************/

	Handler handler = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			switch(msg.what){
			case 0:
				Log.d("mtest","mposition start");
				myAdapter.notifyDataSetChanged();
				break;
			case 1:
				if(sendImage.getVisibility()==View.VISIBLE){
					int position=msg.getData().getInt("position");
					String pathname=cacheMap.getTargetFileName(position);
					String pathnameList[]=pathname.split("/");
					String filename=pathnameList[pathnameList.length-1];
					FileTrans.makeFileSendMessage(0,pathname,filename); //ť�� �۾��� �־��ְ� �۾��� �����Ѵ�.
					FileTrans.demand_job_Start();
					sendImage.setImageBitmap(null);
					sendImage.setVisibility(View.INVISIBLE);
				}
				break;
			}
		}
	};
}
