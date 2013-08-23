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

//TODO : 그리드 이미지를 관리해준다. 보여지는 이미지는 어댑터에 추가되거나 삭제됨으로써 보여진다.
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
		this.cacheMap = cacheMap; //캐시맵 생성
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
		//TODO : 각각의 이미지를 만들어 준다.
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
