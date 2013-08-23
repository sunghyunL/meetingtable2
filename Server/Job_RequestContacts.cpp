#include "Job_RequestContacts.h"
#include "Tokenizer.h"
#include "WPFReceiver.h"
#include <iostream>

using std::cout;
using std::endl;
Job_RequestContacts::Job_RequestContacts(TaskInfo info){
	this->info=info;
}

void Job_RequestContacts::job_start(){
	info.ipAdd=info.vec.at(1); //ip�������ְ�
	makeSocketforWPF(); //���� �������ְ�

	sendStringProtocol("REQUESTCONTACTS&",stringbuf);
	vector<string> filevec;
	receiveStringProtocolforTCP(stringbuf);
	filevec.clear();
	Tokenizer::Tokenize(stringbuf,filevec);
	int filesize=atoi(filevec.at(0).c_str());

	string forderName2="c:/Touchtable/"+info.ipAdd;
	::CreateDirectory(forderName2.c_str(),NULL);

	string fileName="c:/Touchtable/"+info.ipAdd+"/contacts.txt";

	FILE* file; 
	fopen_s(&file,+fileName.c_str(), "wb");

	int totalCount=0,readCount=0;
	while(true){ //
		//	cout<<"���ϻ�����:"<<filesize<<"totalCount:"<<totalCount<<"Count:"<<temp++<<"test:"<<cd->fileTransbuf[1023]<<endl;
		//����̽��� ���� �������� ����
		if(totalCount>=filesize){	
			//cout<<"���ϻ�����:"<<filesize<<"����������:"<<totalCount<<endl;
			break;
		}else if(totalCount+BUFSIZE<=filesize){			
			readCount=recv(info.socketNum,buf,BUFSIZE,0);
			fwrite(buf,sizeof(char),readCount,file);	
			totalCount+=readCount;
		}else if(totalCount+BUFSIZE>filesize){
			int remainder=filesize-totalCount;
			readCount=recv(info.socketNum,buf,remainder,0);
			fwrite(buf,sizeof(char),readCount,file);
			totalCount+=readCount;
		}
	}
	int flag=fclose(file);
	if(flag != 0){ // ���� �ݱ⸦ �������� ��� 0�� �����Ѵ�.
		cout<<"<���� �ݱ� ����>"<<endl;
	}
	string str="REQUESTCONTACTS&"+info.ipAdd+"&\n";
	WPFReceiver::getInstance()->sendStringProtocol(str);
}