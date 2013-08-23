#pragma once
#include "Job.h"
#include <vector>

using std::vector;

class Job_SavePresentation
	:public Job{
public:
	Job_SavePresentation(TaskInfo taskinfo);
	void job_start();
};