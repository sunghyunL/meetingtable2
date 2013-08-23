#pragma once
#include "Job.h"
#include <vector>
#include <list>
#include <queue>
#include <map>

using std::map;
using std::queue;
using std::list;
using std::vector;

class Job_FileSendToAnd 
	:public Job{
private:
	queue<vector<string>, list<vector<string>>> TaskQueue; //해당 리스트에서 작업을 쌓아놓는 큐
	char fileBuf[BUFSIZE];
	char stringBuf[STRINGBUFSIZE];
public:
	static map<string,Job*>* andSendTable; //ip에 따른 작업 인스턴스를 저장해놓는 용도
	static bool reserveTask(string ip,vector<string> vec);
	static void registAndTable_instance(string ip,Job* jf);
	static map<string,Job*>* getJobTable();

	Job_FileSendToAnd(TaskInfo taskinfo);
	void addJob(vector<string> vec);
	
	void job_start();
	void job_finish();


};