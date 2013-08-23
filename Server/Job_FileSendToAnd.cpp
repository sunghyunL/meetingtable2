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
	info.ipAdd=info.vec.at(2); //ip�������ְ�
	makeSocketforWPF(); //���� �������ְ�

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
		if( (err =fopen_s(&file,fileName1.c_str(), "rb"))!=0){//������ ���� ���.
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
		//��Ʈ�� �������� ������
		sendStringProtocol(sendStr,stringBuf);
		//���� ������ ����
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
		cout<<"���Ϻ����� �Ϸ�"<<endl;
	}
}

void Job_FileSendToAnd::addJob(vector<string> vec){
	TaskQueue.push(vec);
}

void Job_FileSendToAnd::job_finish(){
	sendStringProtocol("STOP&",stringBuf);
	getJobTable()->erase(info.ipAdd);
}

//���ù����� ���ڸ��� ȣ�����ִ� �Լ�
bool Job_FileSendToAnd ::reserveTask(string ip,vector<string> vec){
		map<string,Job*>::iterator it =getJobTable()->find(ip);
		
		if(it==getJobTable()->end()){ //������ ���̺� �������� �ʴ´ٸ� ip�� ���
			Job* job=NULL;
			getJobTable()->insert(make_pair(ip,job));//ThreadPool�� Thread�� job�� ����� �߿� ���ο� ����� ���ð��
													//���̺� ���ߵ�ϵǴ� ���� �����ϱ� ���� NULL�������� ��������
			return false;//�۾� ����� ��������
		}else{
			Job* job=it->second;
			Job_FileSendToAnd* jf=(Job_FileSendToAnd*)job;
			cout<<"�۾���Ͻõ�"<<endl;
			if(jf!=NULL){ //jf���� ����� �� �������� addJob�� �����Ѵ�.
				cout<<"�۾����"<<endl;
				jf->addJob(vec);
			}
			return true;//�۾���� �Ϸ�
		}
}

void Job_FileSendToAnd::registAndTable_instance(string ip,Job* jf){
	cout<<"���̺� ���"<<endl;
	(*getJobTable())[ip]=jf;
}