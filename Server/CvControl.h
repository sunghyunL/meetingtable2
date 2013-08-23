//ΩÃ±€≈Ê
#pragma once

class CvControl{
public:
	static CvControl* getInstance();
	void sendCoordinate(int id,int x,int y,int state); //state 0:add 1:set 2:delete
private:
	CvControl();
	static CvControl* cvcontrol;
};