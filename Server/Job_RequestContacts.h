#pragma once
#include "Job.h"
#include <vector>

using std::vector;

class Job_RequestContacts
	:public Job{
private:

public:
	Job_RequestContacts(TaskInfo info);
	void job_start();
	char buf[BUFSIZE];
	char stringbuf[STRINGBUFSIZE];
};