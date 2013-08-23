#pragma once
#include "Job.h"
#include <vector>
#include <string>

using std::string;
using std::vector;

class Job_FileSendToWPF
	:public Job{
public:
	Job_FileSendToWPF(TaskInfo info);
	void job_start();
private:
	char stringbuf[STRINGBUFSIZE];
	char buf[BUFSIZE];
};