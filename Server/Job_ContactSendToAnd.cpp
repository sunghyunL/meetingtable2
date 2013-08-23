#include "Job_ContactSendToAnd.h"

Job_ContactSendToAnd::Job_ContactSendToAnd(TaskInfo taskinfo){
	this->info=taskinfo;
}

void Job_ContactSendToAnd::job_start(){
	info.ipAdd=info.vec.at(1); //ip瘤沥秦林绊
	makeSocketforWPF(); //家南 积己秦林绊

	string sendstr=info.vec.at(0)+"&"+info.vec.at(2)+"&"+info.vec.at(3)+"&";
	sendStringProtocol(sendstr,stringbuf);
}