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
ThreadInfo info[THREADMAXCOUNT]; //������Ǯ ���� �迭

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

//commandQueue�� �ִ� �����尡 ȣ���ϰų�, �۾��� ���� Task�� ȣ���Ѵ�.
TaskInfo ThreadPool::popTask(){ 
	TaskInfo *ptaskinfo;
	TaskInfo taskinfo;
	EnterCriticalSection(&crit); //ũ��Ƽ�� ����
	if(commandQueue.empty()){
		taskinfo.opcode=""; //Task�� �������� ���� ����
		LeaveCriticalSection(&crit);
		return taskinfo; //Task����
	}
	ptaskinfo=commandQueue.front();
	taskinfo=*ptaskinfo; //������ taskinfo����
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

	bool isLooping=false; //���� �۾��� �ϰ� ������ ���� ������.ó���۾�����
	while(true){
		if(!isLooping){
			cout<<threadNumber<<"Thread Sleep"<<endl;
			SuspendThread(tp->poolArr[threadNumber]);
		}
		cout<<threadNumber<<"Thread DO"<<endl;

		taskinfo=tp->popTask();//popQueue

		if(taskinfo.opcode=="") { //ť�� Task�� �������� ����
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

	if(threadCount!=THREADMAXCOUNT){ //������ Ǯ�� ��� �����尡 ������ �ƴ϶��
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

