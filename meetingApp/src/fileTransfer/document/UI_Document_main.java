package fileTransfer.document;

import java.io.File;
import java.io.FilenameFilter;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.TreeMap;

import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.widget.AbsoluteLayout;
import android.widget.ImageView;
import android.widget.ListView;

import com.example.meetingapp.R;

//TODO ����Ȯ���� ������ �̸� �ɷ����� ���� ���� ����
class JavaFilter implements FilenameFilter {
	@Override
	public boolean accept(File dir, String filename) {
		if (!filename.contains(".")) { // ������ ���ɼ��� ����(.�̾��� ������ �� �� ����)
			return true;
		} else { // ������ �͵���
			return filename.endsWith("pptx") || filename.endsWith("docx")
					|| filename.endsWith("pdf") || filename.endsWith("txt")
					 ;
		}
	}
}

public class UI_Document_main extends Activity {
	private TreeMap<String, Entity_Document_item> map;
	private String fileName;
	private ListView listview;

	private AbsoluteLayout scrollLayout;
	private ImageView sendImage;
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.document_main);
		
		sendImage=(ImageView)findViewById(R.id.document_scroll_image_list);
		scrollLayout=(AbsoluteLayout)findViewById(R.id.document_scroll_image_layout_list);
		listview=(ListView)findViewById(R.id.listView_document_main);
		
		//key���� ������ ���ؼ� treemap�� ���
		map=new TreeMap<String, Entity_Document_item>();
		//String sdpath = Environment.getExternalStorageDirectory();
		
		//updateFileList("/storage/sdcard1/"); //���뼺 Ȯ�� �ʿ�
		updateFileList("/storage/sdcard0/"); //���뼺 Ȯ�� �ʿ�
	 
		//����� ���� �� ���
		Document_list_Adapter adapter=new Document_list_Adapter(this, mapToArrlist() , handler,
				sendImage, scrollLayout,0);
		listview.setAdapter(adapter);
	}

	//TODO map�� �̿��� ���ϸ���Ʈ�� �����
	public void updateFileList(String path) {
		File files = new File(path);
		File[] fileNames = files.listFiles(new JavaFilter());
	
		if (files.listFiles(new JavaFilter()).length > 0) {
			for (File file : fileNames) {
				fileName=file.getName();
				
				if (file.isDirectory()) { //���丮 �� ��� ��� ȣ��
					updateFileList(path + fileName + "/");
				} else { 
					if (fileName.contains(".")){ //�����߿��� 'Ȯ���ڰ� ����'�ϴ� ���� �� ���

						if(map.containsKey(path)){ //���� ��� �̶��
							Entity_Document_item item=map.get(path);
							item.addFileNames(fileName);
						}else{ //���丮�� �űԵ�� �̶��
							Entity_Document_item item=new Entity_Document_item(path);
							item.addFileNames(fileName);
							map.put(path, item);

						}
						
					}
				}
			}
		}
		map.remove("/storage/sdcard0/dumpstates/"); //�ý��� ���丮 ����(���뼺 Ȯ�� �ʿ�)
	}
	
	//TODO map->>arrlist
	private ArrayList<Entity_Document_item> mapToArrlist(){
		Iterator<String> itr=map.keySet().iterator();
		ArrayList<Entity_Document_item> arrlist=new ArrayList<Entity_Document_item>();
		while(itr.hasNext()){
			String key=(String)itr.next();
			
			Entity_Document_item item=((Entity_Document_item) map.get(key));
			
			arrlist.add(item);
		}
		return arrlist;
	}
	
	Handler handler = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			switch(msg.what){
			case 1:
				sendImage.setImageBitmap(null);
				break;
			}
		}
	};
}