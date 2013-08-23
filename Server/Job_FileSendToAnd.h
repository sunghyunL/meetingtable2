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
	queue<vector<string>, list<vector<string>>> TaskQueue; //�ش� ����Ʈ���� �۾��� �׾Ƴ��� ť
	char fileBuf[BUFSIZE];
	char stringBuf[STRINGBUFSIZE];
public:
	static map<string,Job*>* andSendTable; //ip�� ���� �۾� �ν��Ͻ��� �����س��� �뵵
	static bool reserveTask(string ip,vector<string> vec);
	static void registAndTable_instance(string ip,Job* jf);
	static map<string,Job*>* getJobTable();

	Job_FileSendToAnd(TaskInfo taskinfo);
	void addJob(vector<string> vec);
	
	void job_start();
	void job_finish();


};