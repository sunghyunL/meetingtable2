#include "ThreadPool.h"
#include "Tokenizer.h"

#include "Job.h"
#include "Job_CaptureDisplay.h"
#include "Job_ContactSendToAnd.h"
#include "Job_RequestContacts.h"
#include "Job_FileSendToAnd.h"
#include "Job_FileSendToWPF.h"
#include "Job_SavePresentation.h"

#include <iostream>
#include <process.h>
#include <vector>

using std::vector;
using std::cout;
using std::endl;

ThreadPool* ThreadPool::threadpool=0;
ThreadInfo info[THREADMAXCOUNT]; //쓰레드풀 변수 배열

ThreadPool::ThreadPool(){
	threadCount=0;
	currentThreadIndex=0;
	InitializeCriticalSection(&crit);
	InitializeCriticalSection(&crit2);
}

ThreadPool* ThreadPool::getInstance(){
	if(!threadpool){
		threadpool=new ThreadPool();
	}
	return threadpool;
}

//commandQueue에 넣는 쓰레드가 호출하거나, 작업이 끝난 Task가 호출한다.
TaskInfo ThreadPool::popTask(){ 
	TaskInfo *ptaskinfo;
	TaskInfo taskinfo;
	EnterCriticalSection(&crit); //크리티컬 섹션
	if(commandQueue.empty()){
		taskinfo.opcode=""; //Task가 존재하지 않음 구분
		LeaveCriticalSection(&crit);
		return taskinfo; //Task종료
	}
	ptaskinfo=commandQueue.front();
	taskinfo=*ptaskinfo; //리턴할 taskinfo복사
	commandQueue.pop();
	delete ptaskinfo;
	LeaveCriticalSection(&crit);
	return taskinfo;
}

void ThreadPool::makeThreadPool(){
	DWORD dwThreadID;
	
	for(int i=0;i<THREADMAXCOUNT;i++){
		info[i].threadpool=this;
		info[i].ThreadNumber=i;
		poolArr[i]= CreateThread( NULL, 0,
			(LPTHREAD_START_ROUTINE)threadPoolTask, 
			(void *)&info[i], 0 ,&dwThreadID);
	}
}


void threadPoolTask(void* arg){
	ThreadInfo* info=(ThreadInfo*)arg;
	int threadNumber=info->ThreadNumber;
	ThreadPool *tp=info->threadpool;

	TaskInfo taskinfo;
	Job *job=NULL;

	bool isLooping=false; //현재 작업을 하고 루프를 도는 것인지.처음작업인지
	while(true){
		if(!isLooping){
			cout<<threadNumber<<"Thread Sleep"<<endl;
			SuspendThread(tp->poolArr[threadNumber]);
		}
		cout<<threadNumber<<"Thread DO"<<endl;

		taskinfo=tp->popTask();//popQueue

		if(taskinfo.opcode=="") { //큐에 Task가 존재하지 않음
			//closesocket(taskinfo.socketNum);
			tp->decreaseThreadCount();
			isLooping=false;
			continue;
		}
		/******TaskRoutine*****/
		if(taskinfo.opcode=="CAPTUREDISPLAY"){
			job=new Job_CaptureDisplay(taskinfo);
		}else if(taskinfo.opcode=="CONTACTSENDTOAND"){
			job=new Job_ContactSendToAnd(taskinfo);
		}else if(taskinfo.opcode=="REQUESTCONTACTS"){
			job=new Job_RequestContacts(taskinfo);
		}else if(taskinfo.opcode=="FILESENDTOAND"){
			job=new Job_FileSendToAnd(taskinfo);
			Job_FileSendToAnd::registAndTable_instance(taskinfo.ipAdd,job);
			Job_FileSendToAnd::reserveTask(taskinfo.ipAdd,taskinfo.vec);
		}else if(taskinfo.opcode=="FILESENDTOWPF"){
			job=new Job_FileSendToWPF(taskinfo);
		}else if(taskinfo.opcode=="SAVEPRESENTATION"){
			job=new Job_SavePresentation(taskinfo);
		}else if(taskinfo.opcode=="SAVECONTACT"){
			job=new Job_ContactSendToAnd(taskinfo);
		}

		job->job_start();
		job->McloseSocket();
		
		/**********************/

		delete job;
		isLooping=true;
	}
}

void ThreadPool::pushTask(string opcode,vector<string> vec,int socketNum,string ipAdd){
	EnterCriticalSection(&crit2);
	TaskInfo* taskinfo=new TaskInfo();
	taskinfo->opcode=opcode;
	taskinfo->socketNum=socketNum;
	taskinfo->vec=vec;
	taskinfo->ipAdd=ipAdd;
	commandQueue.push(taskinfo);
	LeaveCriticalSection(&crit2);

	if(threadCount!=THREADMAXCOUNT){ //쓰레드 풀의 모든 쓰레드가 동작이 아니라면
		increaseThreadCount();
		ResumeThread(poolArr[currentThreadIndex]);
		currentThreadIndex=(++currentThreadIndex)%THREADMAXCOUNT;
	}
}

void ThreadPool::increaseThreadCount(){
	EnterCriticalSection(&crit2);
	threadCount++;
	LeaveCriticalSection(&crit2);
}

void ThreadPool::decreaseThreadCount(){
	EnterCriticalSection(&crit2);
	threadCount--;
	LeaveCriticalSection(&crit2);
}

