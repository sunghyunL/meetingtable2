package network.ExternalTask;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.Socket;

import android.content.ContentValues;
import android.content.Context;
import android.net.Uri;
import android.os.Handler;
import android.provider.Contacts.People;

public class SaveContact extends ExternalJob {
	private Context context;
	private Handler handler;
	public SaveContact(Context context,Handler handler,Socket socket,DataInputStream dis,DataOutputStream dos,String strList[]) {
		super(socket, dis, dos, strList);
		// TODO Auto-generated constructor stub
		this.context=context;
		this.handler=handler;
	}

	@Override
	public void job_Start() {
		// TODO Auto-generated method stub
		Thread thread=new Thread(){
			@Override
			public void run() {
				ContentValues values=new ContentValues();
				values.put(People.NAME, strList[1]);
				Uri uri =context.getContentResolver().insert(People.CONTENT_URI, values);
				
				Uri phoneUri = Uri.withAppendedPath(uri, People.Phones.CONTENT_DIRECTORY);
				 
				values.clear();
				values.put(People.Phones.TYPE, People.Phones.TYPE_MOBILE);
				values.put(People.Phones.NUMBER,strList[2]);
				context.getContentResolver().insert(phoneUri, values);
				handler.sendEmptyMessage(0);
				closeSocket();
			}
		};
		thread.start();
	}

}
