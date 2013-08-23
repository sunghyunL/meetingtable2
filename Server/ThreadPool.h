//싱글톤
#pragma once
#define _WINSOCKAPI_
#include <winsock2.h>
#include <queue>
#include <list>
#include <string>
#include <vector>

using std::vector;
using std::string;

#define THREADMAXCOUNT 10
#define BUFSIZE 1024

using std::queue;
using std::list;
class ThreadPool;

typedef struct TaskInfo{
	string opcode;
	vector<string> vec;
	int socketNum;
	string ipAdd;
}TaskInfo;



typedef struct ThreadInfo{
	ThreadPool *threadpool;
	int ThreadNumber;
}ThreadInfo;


class ThreadPool{
private:
	ThreadPool();
	static ThreadPool* threadpool;
	queue<TaskInfo*,list<TaskInfo*>> commandQueue;
	int threadCount;
	int currentThreadIndex;

	HANDLE poolArr[THREADMAXCOUNT]; //쓰레드풀 핸들 배열
	
	CRITICAL_SECTION crit;
	CRITICAL_SECTION crit2;
public:
	static ThreadPool* getInstance();
	TaskInfo popTask();
	void makeThreadPool();
	void pushTask(string opcode,vector<string> vec,int socketNum,string ipAdd);
	void increaseThreadCount();
	void decreaseThreadCount();

	friend void threadPoolTask(void* arg);
};