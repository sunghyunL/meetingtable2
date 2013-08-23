#include "CvControl.h"
#include "Job_CaptureDisplay.h"

CvControl* CvControl::cvcontrol=0;

CvControl::CvControl(){

}

CvControl* CvControl::getInstance(){
	if(!cvcontrol){
		cvcontrol=new CvControl();
	}
	return cvcontrol;
}

void CvControl::sendCoordinate(int id,int x,int y,int state){
	if(state==0){//add
		Job_CaptureDisplay::pushID(id);
	}else if(state==1){//set
		Job_CaptureDisplay::request_renew_XY(id,x,y);
	}else if(state==2){//del
		Job_CaptureDisplay::request_delete_id(id);
	}
}