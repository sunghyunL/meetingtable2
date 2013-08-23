#pragma once
#include <vector>
#include "ThreadPool.h"
#define _WINSOCKAPI_

#define STRINGBUFSIZE 150
#define BUFSIZE 1024
#define ANDPORT 9997

using std::vector;

class Job{
protected:
	TaskInfo info;
public:
	virtual void job_start()=0;
	virtual ~Job(){};
	void McloseSocket();
protected:
	bool receiveStringProtocolforTCP(char* buf);
	void makeSocketforWPF();
	void sendStringProtocol(string str,char *buf);
};