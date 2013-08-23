package fileTransfer.document;

import java.util.ArrayList;

import android.content.Context;
import android.content.Intent;
import android.os.Handler;
import android.util.TypedValue;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsoluteLayout;
import android.widget.AbsoluteLayout.LayoutParams;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.FrameLayout;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.TextView;

import com.example.meetingapp.R;

import fileTransfer.gallery.UI_GalleryDetailView;
import function.touch.MyTouchListenerDoc;

public class Document_list_Adapter extends BaseAdapter {
	// private HashMap<String, Entity_Document_item> map;
	private ArrayList<Entity_Document_item> arrlist;
	private LayoutInflater mInflater;
	private Context context;
	private FrameLayout.LayoutParams params;
	private int oneOfHeightDP = 115; //그리드 아이템 1개당 dip높이
	private int oneOfHeightPX = 0; //그리드 아이템 1개당 px높이
	
	private int RECOGNITION_NUM = 4;
	private ArrayList<Xxx> arrlistXxx;
	private int type;
	
	public Document_list_Adapter(Context context,
			ArrayList<Entity_Document_item> arrlist,Handler handler,ImageView sendImage,AbsoluteLayout scrollLayout,int type) {
		this.arrlist = arrlist;
		this.context = context;
		this.type=type;
		params = new FrameLayout.LayoutParams(300, 300);
		mInflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		
		oneOfHeightPX=dip2px(oneOfHeightDP,context);//그리드 아이템 1개의 px높이 계산
	}

	@Override
	public int getCount() {
		// TODO Auto-generated method stub
		return arrlist.size();
	}

	@Override
	public Object getItem(int arg0) {
		// TODO Auto-generated method stub
		return arrlist.get(arg0);
	}

	@Override
	public long getItemId(int arg0) {
		// TODO Auto-generated method stub
		return arg0;
	}

	@Override
	public View getView(final int position, View convertView, ViewGroup parent) {
		if (convertView == null) {
			convertView = mInflater.inflate(R.layout.document_item, parent,
					false);
		}
		GridView gridview = (GridView) convertView
				.findViewById(R.id.gridview_document);

		TextView text = (TextView) convertView.findViewById(R.id.textview_item);
		if(type==0){
			text.setText(arrlist.get(position).getPath()); //path명 지정
		}else if(type==1){
			String strlist[]=arrlist.get(position).getPath().split("/");
			text.setText(strlist[strlist.length-2]);
		}

		/************ 어댑터가 만들어져 있지 않을 경우 만들어주기 ***************/
		Document_grid_Adapter adapter = (Document_grid_Adapter) arrlist.get(
				position).getAdapter();
		
		if (adapter == null) {
			adapter = new Document_grid_Adapter(context, arrlist.get(position).getFileNames(),type);
			arrlist.get(position).setAdapter(adapter);
		}
		/*******************************************************/
		
		params.height = calculateHeight(position); //아이템 개수에 따른 높이 계산
		params.width = LayoutParams.MATCH_PARENT; //가로 크기는 fill

		//listview호출 시마다 높이와 어댑터 연결을 수행
		gridview.setLayoutParams(params);
		gridview.setAdapter(adapter);
		
		//gridview.setOnItemLongClickListener(mlistener);
		//gridview.setOnTouchListener(mlistener);
		
		gridview.setOnItemClickListener(new OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView parent, View v, int position2,
					long id) {
				Intent intent = new Intent(context.getApplicationContext(),
						UI_Document_detail.class);
				intent.putExtra("pathname",arrlist.get(position).getPath());
				intent.putExtra("filename",arrlist.get(position).getFileNames().get(position2).getName());
				intent.putExtra("fileType",arrlist.get(position).getFileNames().get(position2).getXxx());
				context.startActivity(intent);	
			}
		});
		
		/**********************************************************/
		
		
		return convertView;
	}

	//TODO item개수 만큼 높이를 계산해서 반환해줌
	private int calculateHeight(int position) {
		int quotient = arrlist.get(position).getFileNames().size() / 4;
		int remainder = arrlist.get(position).getFileNames().size() % 4;
		
		//4로 나누어 떨어지지 않을 경우 높이를 1칸 더 추가
		if (remainder != 0) {
			quotient++;
		}
		return quotient * oneOfHeightPX;
	}

	//TODO dip를 px로 변환
	private int dip2px(int dip, Context context) {
		int px = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP,
				dip, context.getResources().getDisplayMetrics());
		return px;
	}
	//TODO px를 dip로 변환
	private int px2dip(int px, Context context) {
		int dip = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_PX,
				px, context.getResources().getDisplayMetrics());
		return dip;
	}
}
