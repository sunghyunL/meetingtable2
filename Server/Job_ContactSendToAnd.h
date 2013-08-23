#pragma once
#include "Job.h"
#include <vector>

using std::vector;

class Job_ContactSendToAnd
	:public Job{
public:
	Job_ContactSendToAnd(TaskInfo taskinfo);
	void job_start();
private:
	char stringbuf[STRINGBUFSIZE];
};