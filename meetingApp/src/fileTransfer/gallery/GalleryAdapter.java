package fileTransfer.gallery;

import android.content.Context;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.provider.MediaStore;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;

import com.example.meetingapp.R;

//TODO : �׸��� �̹����� �������ش�. �������� �̹����� ����Ϳ� �߰��ǰų� ���������ν� ��������.
public class GalleryAdapter extends BaseAdapter {
	
	private Context mContext;
	private LayoutInflater mInflater;

	private GalleryCacheMap cacheMap;

	private ImageView image;
	private Bitmap bit;
	
	public GalleryAdapter(Context context,GalleryCacheMap cacheMap) {
		mInflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		
		mContext = context;
		this.cacheMap = cacheMap; //ĳ�ø� ����
	}

	@Override
	public int getCount() {
		return cacheMap.getCount();
	}

	@Override
	public Object getItem(int arg0) {
		return null;
	}

	@Override
	public long getItemId(int arg0) {

		return 0;
	}

	@Override
	public int getItemViewType(int position) {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int getViewTypeCount() {
		// TODO Auto-generated method stub
		return 1;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		//TODO : ������ �̹����� ����� �ش�.
		//Log.d("mtest","mposition"+position);
	
		if (convertView == null) {
			convertView = mInflater.inflate(R.layout.gallery_item, parent, false);
		}
		
		if((bit=cacheMap.getItem(position))!=null){
			image = (ImageView) convertView
					.findViewById(R.id.imageView_gridimage);
			image.setImageBitmap(bit);
		}

		return convertView;
	}
}
