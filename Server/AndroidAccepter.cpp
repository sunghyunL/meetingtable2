#include "AndroidAccepter.h"
#include <process.h>
#include <winsock2.h>
#include <stdio.h>
#include "Tokenizer.h"
#include "ThreadPool.h"
#include <iostream>


using std::cout;
using std::endl;
#pragma comment (lib,"ws2_32.lib")

AndroidAccepter* AndroidAccepter::accepter=0;

AndroidAccepter* AndroidAccepter::getInstance(){
	if(!accepter){
		accepter=new AndroidAccepter();
	}
	return accepter;
}

void AndroidAccepter::accepterStart(){
	DWORD dwThreadID;
	CreateThread( NULL, 0, (LPTHREAD_START_ROUTINE)acceptAndroid, (void *)this, 0 ,&dwThreadID);
}

//�ȵ���̵��� ���� ��⸦ �Ѵ�
void acceptAndroid(void* arg){
	AndroidAccepter* aa=(AndroidAccepter*)arg;
		//���� �ʱ�ȭ
	WSADATA wsd;
	int s, ns, addrsize;
	sockaddr_in client, server;
	vector<string> vec;
	string opcode;
	if(WSAStartup(MAKEWORD(2,2), &wsd) != 0)
	{
		printf("winsock ����\n");
		return;
	}

	/************server bind**************/
	server.sin_family = AF_INET;
	server.sin_addr.s_addr = INADDR_ANY;		// ���� ����(localhost)�� ��Ÿ��. ȥ�� ���̿�.
	server.sin_port = htons(PORT);

	// bind() - ���� ���� Ŭ���̾�Ʈ�� ���� welcome ����
	s = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
	bind(s, (sockaddr *)&server, sizeof(server));
		
	// listen ��Ʈ�� ���� �ξ� ������ ��ٸ��� ��
	listen(s,2);
	/***************************************/
	addrsize = sizeof(client);

	while(true){
		cout<<"Ŭ���̾�Ʈ ���"<<endl;
		ns = accept(s, (sockaddr*)&client, &addrsize);
		if(ns == INVALID_SOCKET)
		{
			printf("accept() ����\n");
			WSACleanup();
			return;
		}
		printf("%s:%d�� ����\n", inet_ntoa(client.sin_addr),ntohs(client.sin_port));
		aa->receiveStringProtocol(ns,aa->stringbuf);
		
		vec.clear();
		Tokenizer::Tokenize(aa->stringbuf,vec);
		opcode=vec.at(0);

		ThreadPool::getInstance()->pushTask(opcode,vec,ns,inet_ntoa(client.sin_addr));
	}
}

bool AndroidAccepter::receiveStringProtocol(int socketNum,char* buf){
	//char tempBuf[100];
	//int totalCount=0,readCount=0,remainder=0;
	//while(totalCount!=100){
		//remainder=100-totalCount;
		//if(remainder>0){
		memset(buf,0,STRINGBUFSIZE);
		int result=recv(socketNum,buf,STRINGBUFSIZE,0);
		if(result==SOCKET_ERROR){
				return false;
		}
		return true;
			//int j=0;
			//for(int i=totalCount;i<totalCount+readCount;i++){
				//buf[i]=tempBuf[j++];
			//}
		//}
		//totalCount+=readCount;
	//}
}