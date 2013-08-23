package fileTransfer.document;

import java.util.ArrayList;

import android.app.Activity;
import android.content.Context;
import android.os.Handler;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsoluteLayout;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.example.meetingapp.R;

import function.touch.MyTouchListener;

public class Document_grid_Adapter extends BaseAdapter {
	private ArrayList<Xxx> arrlist;
	private Context context;
	private LayoutInflater mInflater;
	private int type;

	
	public Document_grid_Adapter(Context context,ArrayList<Xxx> arrlist,int type){
		this.arrlist=arrlist;
		this.context=context;
		this.type=type;
		mInflater=(LayoutInflater)context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
	}
	@Override
	public int getCount() {
		// TODO Auto-generated method stub
		Log.d("test5",""+arrlist.size());
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
		return 0;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		if(convertView==null){
			convertView=mInflater.inflate(R.layout.document_grid_item, parent,false);
		}
		
		ImageView imageview=(ImageView)convertView.findViewById(R.id.imageView_gridimage);
		TextView textview=(TextView)convertView.findViewById(R.id.textView_griditem);
		
		textview.setText(arrlist.get(position).getName());

		
		return convertView;
	}

}
