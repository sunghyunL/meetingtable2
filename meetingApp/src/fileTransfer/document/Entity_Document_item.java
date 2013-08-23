package fileTransfer.document;

import java.util.ArrayList;
import java.util.HashMap;

import android.widget.BaseAdapter;

public class Entity_Document_item {
	private String path;
	private int size;
	private ArrayList<ArrayList<Xxx>> fileNames; //��ü ������ �����ϴ� arrlist
	private BaseAdapter adapter=null;
	private int RECOGNITION_NUM = 4; //�������� Ȯ���� ����
	
	public Entity_Document_item(String path){
		this.fileNames=new ArrayList<ArrayList<Xxx>>();
		
		//Ȯ���� ���� ���� �����ϱ� ���� arrlist����
		for(int i=0;i<RECOGNITION_NUM;i++){
			fileNames.add(new ArrayList<Xxx>());
		}
		
		this.path=path;
		size=0;
	}
	//TODO ��ü ���� ����Ʈ ������(Ȯ���� ���� �Ϸ�)
	public ArrayList<Xxx> getFileNames() { 
		ArrayList<Xxx> arrlist=new ArrayList<Xxx>();
		
		for(int i=0;i<RECOGNITION_NUM;i++){
			arrlist.addAll(fileNames.get(i));
		}
		return arrlist;
	}
	
	//TODO Ȯ���ں� ������ ���ؼ� (Ȯ���� ���� �����ؼ� ���� arrlist�� ����)
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
