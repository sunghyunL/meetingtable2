#include "Job_SavePresentation.h"

Job_SavePresentation::Job_SavePresentation(TaskInfo taskinfo){
	this->info=taskinfo;
}

void Job_SavePresentation::job_start(){
	makeSocketforWPF(); //소켓 서버에 접속




}