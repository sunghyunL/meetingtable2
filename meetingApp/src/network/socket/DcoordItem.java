package network.socket;
/*
typedef struct dcoord{
	int byteProcotolDumy; //�ȵ���̵� �ܿ��� �޾��� �� ��Ʈ���� �ƴ϶�°��� 
					//�˷��ֱ� ����
	int opcode; 
	//int ID; 
	int state;  
	int stateInfo; 
	int Lx; 
	int Ly; 
	int Rx; 
	int Ry; 
	char dumy[18];
}dcoord; //:50
*/

public class DcoordItem {
	private int state;
	private int stateInfo;
	private int Lx;
	private int Ly;
	private int Rx;
	private int Ry;
	public int getState() {
		return state;
	}
	public void setState(int state) {
		this.state = state;
	}
	public int getStateInfo() {
		return stateInfo;
	}
	public void setStateInfo(int stateInfo) {
		this.stateInfo = stateInfo;
	}
	public int getLx() {
		return Lx;
	}
	public void setLx(int lx) {
		Lx = lx;
	}
	public int getLy() {
		return Ly;
	}
	public void setLy(int ly) {
		Ly = ly;
	}
	public int getRx() {
		return Rx;
	}
	public void setRx(int rx) {
		Rx = rx;
	}
	public int getRy() {
		return Ry;
	}
	public void setRy(int ry) {
		Ry = ry;
	}
	
}
