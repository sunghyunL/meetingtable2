#include "Job_FileSendToAnd.h"
#include <iostream>

using std::endl;
using std::cout;
using std::make_pair;

map<string,Job*>* Job_FileSendToAnd::andSendTable=0;

map<string,Job*>* Job_FileSendToAnd::getJobTable(){
	if(!andSendTable){
		andSendTable=new map<string,Job*>();
	}
	return andSendTable;
}

Job_FileSendToAnd::Job_FileSendToAnd(TaskInfo taskinfo){
	this->info=taskinfo;
}

void Job_FileSendToAnd::job_start(){
	info.ipAdd=info.vec.at(2); //ip지정해주고
	makeSocketforWPF(); //소켓 생성해주고

	while(true){
		if(TaskQueue.empty()){
			job_finish();
			break;
		}
		vector<string> vec=TaskQueue.front();
		TaskQueue.pop();

		
		string ipAddress=vec.at(2);

		string fileName1="c:/Touchtable/"+vec.at(2)+"/other/"+vec.at(1);
		string fileName2="c:/Touchtable/"+vec.at(2)+"/mobile/"+vec.at(1);
		
		string sendStr="FILESENDTOAND&";


		FILE* file;// fopen(fileName.c_str(), "r");
		errno_t err;
		if( (err =fopen_s(&file,fileName1.c_str(), "rb"))!=0){//파일이 없을 경우.
			fopen_s(&file,fileName2.c_str(), "rb");
		}
		fseek( file, 0, SEEK_END );
		int filesize = ftell( file );
		fseek( file, 0, 0 );
		char temp[10];
		_itoa_s(filesize,temp,10);
		sendStr.append(vec.at(1));
		sendStr.append("&");
		sendStr.append(temp);
		sendStr.append("&");
		//스트링 프로토콜 보내기
		sendStringProtocol(sendStr,stringBuf);
		//파일 보내기 시작
		int totalCount=0;
		int readCount=0;
		//int count=0;

		while(true){
		//	cout<<"fileSize:"<<filesize<<"total:"<<totalCount<<"count:"<<count++<<"flag:"<<fileBuf[0]<<endl;	
			if(totalCount>=filesize){
				break;
			}else if(totalCount+BUFSIZE<=filesize){
				readCount=fread(fileBuf,sizeof(char),BUFSIZE,file);
				//read(file,fileTransbuf,
				send(info.socketNum,fileBuf,readCount,0);
				totalCount+=readCount;
			}else if(totalCount+BUFSIZE>filesize){
				int remainder=filesize-totalCount;
				readCount=fread(fileBuf,sizeof(char),remainder,file);
				
				send(info.socketNum,fileBuf,readCount,0);
				totalCount+=readCount;
			}
		}
		fclose(file);
		cout<<"파일보내기 완료"<<endl;
	}
}

void Job_FileSendToAnd::addJob(vector<string> vec){
	TaskQueue.push(vec);
}

void Job_FileSendToAnd::job_finish(){
	sendStringProtocol("STOP&",stringBuf);
	getJobTable()->erase(info.ipAdd);
}

//리시버에서 받자마자 호출해주는 함수
bool Job_FileSendToAnd ::reserveTask(string ip,vector<string> vec){
		map<string,Job*>::iterator it =getJobTable()->find(ip);
		
		if(it==getJobTable()->end()){ //기존에 테이블에 존재하지 않는다면 ip만 등록
			Job* job=NULL;
			getJobTable()->insert(make_pair(ip,job));//ThreadPool의 Thread가 job을 만드는 중에 새로운 명령이 들어올경우
													//테이블에 이중등록되는 것을 방지하기 위해 NULL아이템을 삽입해줌
			return false;//작업 등록을 못했을시
		}else{
			Job* job=it->second;
			Job_FileSendToAnd* jf=(Job_FileSendToAnd*)job;
			cout<<"작업등록시도"<<endl;
			if(jf!=NULL){ //jf까지 등록이 된 시점부터 addJob만 수행한다.
				cout<<"작업등록"<<endl;
				jf->addJob(vec);
			}
			return true;//작업등록 완료
		}
}

void Job_FileSendToAnd::registAndTable_instance(string ip,Job* jf){
	cout<<"테이블 등록"<<endl;
	(*getJobTable())[ip]=jf;
}