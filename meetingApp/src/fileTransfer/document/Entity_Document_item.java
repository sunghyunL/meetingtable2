package fileTransfer.document;

import java.util.ArrayList;
import java.util.HashMap;

import android.widget.BaseAdapter;

public class Entity_Document_item {
	private String path;
	private int size;
	private ArrayList<ArrayList<Xxx>> fileNames; //전체 파일을 관리하는 arrlist
	private BaseAdapter adapter=null;
	private int RECOGNITION_NUM = 4; //지원가능 확장자 개수
	
	public Entity_Document_item(String path){
		this.fileNames=new ArrayList<ArrayList<Xxx>>();
		
		//확장자 별로 따로 관리하기 위한 arrlist생성
		for(int i=0;i<RECOGNITION_NUM;i++){
			fileNames.add(new ArrayList<Xxx>());
		}
		
		this.path=path;
		size=0;
	}
	//TODO 전체 파일 리스트 얻어오기(확장자 정렬 완료)
	public ArrayList<Xxx> getFileNames() { 
		ArrayList<Xxx> arrlist=new ArrayList<Xxx>();
		
		for(int i=0;i<RECOGNITION_NUM;i++){
			arrlist.addAll(fileNames.get(i));
		}
		return arrlist;
	}
	
	//TODO 확장자별 정렬을 위해서 (확장자 별로 구분해서 개별 arrlist에 삽입)
	public void addFileNames(String name){
		//Arrlsit Factory
		
		//0:pptx,1:docx,2:pdf,3:txt
		if(name.endsWith("pptx")){
			fileNames.get(0).add(new Xxx(0, name));
		}else if(name.endsWith("docx")){
			fileNames.get(1).add(new Xxx(1,name));
		}else if(name.endsWith("pdf")){
			fileNames.get(2).add(new Xxx(2,name));
		}else if(name.endsWith("txt")){
			fileNames.get(3).add(new Xxx(3,name));
		}
	}
	
	public BaseAdapter getAdapter() {
		return adapter;
	}
	public void setAdapter(BaseAdapter adapter) {
		this.adapter = adapter;
	}
	public String getPath() {
		return path;
	}
	public void setPath(String path) {
		this.path = path;
	}
}
