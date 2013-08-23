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

//TODO 파일확장자 명으로 미리 걸러내기 위한 파일 필터
class JavaFilter implements FilenameFilter {
	@Override
	public boolean accept(File dir, String filename) {
		if (!filename.contains(".")) { // 폴더일 가능성이 있음(.이없는 파일일 수 도 있음)
			return true;
		} else { // 파일인 것들임
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
		
		//key값의 순서를 위해서 treemap을 사용
		map=new TreeMap<String, Entity_Document_item>();
		//String sdpath = Environment.getExternalStorageDirectory();
		
		//updateFileList("/storage/sdcard1/"); //범용성 확인 필요
		updateFileList("/storage/sdcard0/"); //범용성 확인 필요
	 
		//어댑터 생성 및 등록
		Document_list_Adapter adapter=new Document_list_Adapter(this, mapToArrlist() , handler,
				sendImage, scrollLayout,0);
		listview.setAdapter(adapter);
	}

	//TODO map을 이용해 파일리스트를 만든다
	public void updateFileList(String path) {
		File files = new File(path);
		File[] fileNames = files.listFiles(new JavaFilter());
	
		if (files.listFiles(new JavaFilter()).length > 0) {
			for (File file : fileNames) {
				fileName=file.getName();
				
				if (file.isDirectory()) { //디렉토리 일 경우 재귀 호출
					updateFileList(path + fileName + "/");
				} else { 
					if (fileName.contains(".")){ //파일중에서 '확장자가 존재'하는 파일 일 경우

						if(map.containsKey(path)){ //기존 등록 이라면
							Entity_Document_item item=map.get(path);
							item.addFileNames(fileName);
						}else{ //디렉토리가 신규등록 이라면
							Entity_Document_item item=new Entity_Document_item(path);
							item.addFileNames(fileName);
							map.put(path, item);

						}
						
					}
				}
			}
		}
		map.remove("/storage/sdcard0/dumpstates/"); //시스템 디렉토리 삭제(범용성 확인 필요)
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