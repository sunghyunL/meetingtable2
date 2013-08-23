package network.ExternalTask;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.Socket;

public class SaveLastPresentation extends ExternalJob {

	public SaveLastPresentation(Socket socket,DataInputStream dis,DataOutputStream dos,String strList[]) {
		super(socket, dis, dos, strList);
		// TODO Auto-generated constructor stub
	}

	@Override
	public void job_Start() {
		// TODO Auto-generated method stub

	}

}
