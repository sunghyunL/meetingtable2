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
	private int oneOfHeightDP = 115; //�׸��� ������ 1���� dip����
	private int oneOfHeightPX = 0; //�׸��� ������ 1���� px����
	
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
		
		oneOfHeightPX=dip2px(oneOfHeightDP,context);//�׸��� ������ 1���� px���� ���
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
			text.setText(arrlist.get(position).getPath()); //path�� ����
		}else if(type==1){
			String strlist[]=arrlist.get(position).getPath().split("/");
			text.setText(strlist[strlist.length-2]);
		}

		/************ ����Ͱ� ������� ���� ���� ��� ������ֱ� ***************/
		Document_grid_Adapter adapter = (Document_grid_Adapter) arrlist.get(
				position).getAdapter();
		
		if (adapter == null) {
			adapter = new Document_grid_Adapter(context, arrlist.get(position).getFileNames(),type);
			arrlist.get(position).setAdapter(adapter);
		}
		/*******************************************************/
		
		params.height = calculateHeight(position); //������ ������ ���� ���� ���
		params.width = LayoutParams.MATCH_PARENT; //���� ũ��� fill

		//listviewȣ�� �ø��� ���̿� ����� ������ ����
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

	//TODO item���� ��ŭ ���̸� ����ؼ� ��ȯ����
	private int calculateHeight(int position) {
		int quotient = arrlist.get(position).getFileNames().size() / 4;
		int remainder = arrlist.get(position).getFileNames().size() % 4;
		
		//4�� ������ �������� ���� ��� ���̸� 1ĭ �� �߰�
		if (remainder != 0) {
			quotient++;
		}
		return quotient * oneOfHeightPX;
	}

	//TODO dip�� px�� ��ȯ
	private int dip2px(int dip, Context context) {
		int px = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP,
				dip, context.getResources().getDisplayMetrics());
		return px;
	}
	//TODO px�� dip�� ��ȯ
	private int px2dip(int px, Context context) {
		int dip = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_PX,
				px, context.getResources().getDisplayMetrics());
		return dip;
	}
}
