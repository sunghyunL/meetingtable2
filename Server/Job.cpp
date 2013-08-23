#include "Job.h"
#include <winsock2.h>
#include <windows.h>
#include <stdio.h>
#include "AndroidAccepter.h"
bool Job::receiveStringProtocolforTCP(char* buf){
	//char tempBuf[100];
	//int totalCount=0,readCount=0,remainder=0;
	//while(totalCount!=100){
	//	remainder=100-totalCount;
	//	if(remainder>0){
	//		readCount=recv(info.socketNum,tempBuf,remainder,0);
	//		if(readCount==SOCKET_ERROR){
	//			return false;
	//		}
	//		int j=0;
	//		for(int i=totalCount;i<totalCount+readCount;i++){
	//			buf[i]=tempBuf[j++];
	//		}
	//	}
	//	totalCount+=readCount;
	//}
	
	return AndroidAccepter::receiveStringProtocol(info.socketNum,buf);
}

void Job::sendStringProtocol(string str,char *buf){
	memset(buf,0,STRINGBUFSIZE);
	sprintf_s(buf,STRINGBUFSIZE,str.c_str());
	
	send(info.socketNum,buf, STRINGBUFSIZE,0);
}

void Job::makeSocketforWPF(){
	int s, ns, addrsize;
	sockaddr_in client;
	client.sin_family = AF_INET;
	client.sin_addr.s_addr = inet_addr(info.ipAdd.c_str());		// ���� ����(localhost)�� ��Ÿ��. ȥ�� ���̿�.
	client.sin_port = htons(ANDPORT);
	memset(&(client.sin_zero), 0, 8); 

	// bind() - ���� ���� Ŭ���̾�Ʈ�� ���� welcome ����
	s = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
	info.socketNum=s; //���Ϲ�ȣ ����

	connect(s,(struct sockaddr*)&client,sizeof(struct sockaddr));
}

void Job::McloseSocket(){
	closesocket(info.socketNum);
}