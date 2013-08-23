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
	
	while(true){ //파일의 연속 전송을 위한 2중 루프
		send(info.socketNum,stringbuf,STRINGBUFSIZE,0); //루틴 동기화를 위한 send
		totalCount=0;
		readCount=0;
		/********배열요소 초기화*********/
		memset(stringbuf,0,sizeof(stringbuf));
		filevec.clear();
		/*******************************/
		receiveStringProtocolforTCP(stringbuf);
		
		Tokenizer::Tokenize(stringbuf,filevec);
		string opcode=filevec.at(0);
		
		if(opcode=="STOP"){ //opcode에 파일 중단 신호가 들어오면 파일전송 끝 
			break;
		}

		//폴더 생성
		string forderName="c:/Touchtable/"+info.ipAdd;
		::CreateDirectory(forderName.c_str(),NULL);

		string forderName2="c:/Touchtable/"+info.ipAdd+"/"+"Mobile";
		::CreateDirectory(forderName2.c_str(),NULL);

		int filesize=atoi(filevec.at(2).c_str());
		string fileName=forderName2+"/"+filevec.at(1);

		FILE* file; 
		fopen_s(&file,+fileName.c_str(), "wb");
		

		//파일 전송 시작 되었음을 wpf에 알림
		WPFReceiver::getInstance()->sendFileDisplayStartMessage(info.ipAdd,fileName);

		while(true){ //
			//	cout<<"파일사이즈:"<<filesize<<"totalCount:"<<totalCount<<"Count:"<<temp++<<"test:"<<cd->fileTransbuf[1023]<<endl;
			//디바이스로 부터 파일전송 받음
			if(totalCount>=filesize){	
				cout<<"파일사이즈:"<<filesize<<"받은사이즈:"<<totalCount<<endl;
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

		cout<<"<파일받기 완료>"<<endl;
		//파일 받기 완료
		int flag=fclose(file);
		if(flag == 0){ // 파일 닫기를 성공했을 경우 0을 리턴한다.
			cout<<"<파일 닫기 성공>"<<endl;
		}else{
			cout<<"<파일 닫기 실패>"<<endl;
		}

		//WPF에 파일 전송 완료 여부 알려주기
		WPFReceiver::getInstance()->sendFileDisplayCompleteMessage(info.ipAdd,fileName);
	}
}

