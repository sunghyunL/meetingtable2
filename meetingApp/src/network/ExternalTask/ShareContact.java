package network.ExternalTask;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.util.Arrays;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.ContactsContract;
import android.util.Log;

public class ShareContact extends ExternalJob {
	Context context;
	StringBuilder builder;

	public ShareContact(Context context,Socket socket,DataInputStream dis,DataOutputStream dos,String strList[]){
		super(socket, dis, dos, strList);
		this.context = context;
		builder=new StringBuilder();
	}

	@Override
	public void job_Start() {
		
		Thread thread = new Thread() {
			@Override
			public void run() {
				Uri uri = ContactsContract.Contacts.CONTENT_URI;

				// 2. 요청할 필드를 지정
				String[] requestColumn = { ContactsContract.Contacts._ID,
						ContactsContract.Contacts.DISPLAY_NAME,
						ContactsContract.Contacts.HAS_PHONE_NUMBER };

				// 3. 쿼리를 보내 커서를 얻는다. 이름순 정렬
				Cursor cursor = context.getContentResolver().query(uri,
						requestColumn, null, null,
						ContactsContract.Contacts.DISPLAY_NAME + " ASC");

				while (cursor.moveToNext()) {
					long id = cursor.getLong(0);
					String name = cursor.getString(1);
					int hasnumber = Integer.valueOf(cursor.getString(2));
					String phonenumber = "";
					if (hasnumber > 0) {
						Cursor phones = context
								.getContentResolver()
								.query(ContactsContract.CommonDataKinds.Phone.CONTENT_URI,
										null,
										ContactsContract.CommonDataKinds.Phone.CONTACT_ID
												+ " = " + id, null, null);
						if (phones.moveToNext()) {
							phonenumber = phones
									.getString(phones
											.getColumnIndex(ContactsContract.CommonDataKinds.Phone.NUMBER));
							
						} else {
							continue;
						}
						phones.close();
					}
					
					builder.append(name+"&"+phonenumber+"&");
					Log.d("ver2.test", "name:" + name + "number:" + phonenumber+"length:"+builder.length());
					
				
				}
				cursor.close();
				String sendStr=builder.toString().toString();

				try {
					byte sendbyte[]=sendStr.getBytes("EUC-KR");
					sendStringProtocol(sendbyte.length+"&");
					dos.write(sendbyte);
					dos.flush();

				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				closeSocket();
			}
		};
		thread.start();
	}
	
}
