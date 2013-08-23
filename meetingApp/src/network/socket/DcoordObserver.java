package network.socket;

import java.util.Queue;

import android.util.Log;

public class DcoordObserver {
	private  Queue<DcoordItem> dcoordQueue;
	public DcoordObserver(SocketControl socketcontrol){
		dcoordQueue=socketcontrol.dcoordQueue;
	}

	public void observingStart(){
		Log.d("test12","observerStart");
		
		Thread thread=new Thread(){
			@Override
			public void run() {
				while(true){
					if(!dcoordQueue.isEmpty()){
						DcoordItem item=dcoordQueue.poll();
						
						Log.d("test12","Lx:"+item.getLx()+"Ly"+item.getLy()+"Rx:"+item.getRx()+"Ry:"+item.getRy()+"state:"+item.getState()+"stateInfo:"+item.getStateInfo());
						
					}
				}
			}
		};
		
		thread.start();
	}
}
