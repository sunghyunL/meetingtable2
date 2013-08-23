#include "Job_FileSendToWPF.h"
#include <winsock2.h>
#include <iostream>
#include "Tokenizer.h"
#include "WPFReceiver.h"
#include <tchar.h>

using std::cout;
using std::endl;

Job_FileSendToWPF::Job_FileSendToWPF(TaskInfo info){
	this->info=info;
}

void Job_FileSendToWPF::job_start(){
	//FILESENDTOWPF&fileName&filesize&
	vector<string> filevec;
	int totalCount=0;
	int readCount=0;
	
	while(true){ //������ ���� ������ ���� 2�� ����
		send(info.socketNum,stringbuf,STRINGBUFSIZE,0); //��ƾ ����ȭ�� ���� send
		totalCount=0;
		readCount=0;
		/********�迭��� �ʱ�ȭ*********/
		memset(stringbuf,0,sizeof(stringbuf));
		filevec.clear();
		/*******************************/
		receiveStringProtocolforTCP(stringbuf);
		
		Tokenizer::Tokenize(stringbuf,filevec);
		string opcode=filevec.at(0);
		
		if(opcode=="STOP"){ //opcode�� ���� �ߴ� ��ȣ�� ������ �������� �� 
			break;
		}

		//���� ����
		string forderName="c:/Touchtable/"+info.ipAdd;
		::CreateDirectory(forderName.c_str(),NULL);

		string forderName2="c:/Touchtable/"+info.ipAdd+"/"+"Mobile";
		::CreateDirectory(forderName2.c_str(),NULL);

		int filesize=atoi(filevec.at(2).c_str());
		string fileName=forderName2+"/"+filevec.at(1);

		FILE* file; 
		fopen_s(&file,+fileName.c_str(), "wb");
		

		//���� ���� ���� �Ǿ����� wpf�� �˸�
		WPFReceiver::getInstance()->sendFileDisplayStartMessage(info.ipAdd,fileName);

		while(true){ //
			//	cout<<"���ϻ�����:"<<filesize<<"totalCount:"<<totalCount<<"Count:"<<temp++<<"test:"<<cd->fileTransbuf[1023]<<endl;
			//����̽��� ���� �������� ����
			if(totalCount>=filesize){	
				cout<<"���ϻ�����:"<<filesize<<"����������:"<<totalCount<<endl;
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

		cout<<"<���Ϲޱ� �Ϸ�>"<<endl;
		//���� �ޱ� �Ϸ�
		int flag=fclose(file);
		if(flag == 0){ // ���� �ݱ⸦ �������� ��� 0�� �����Ѵ�.
			cout<<"<���� �ݱ� ����>"<<endl;
		}else{
			cout<<"<���� �ݱ� ����>"<<endl;
		}

		//WPF�� ���� ���� �Ϸ� ���� �˷��ֱ�
		WPFReceiver::getInstance()->sendFileDisplayCompleteMessage(info.ipAdd,fileName);
	}
}

