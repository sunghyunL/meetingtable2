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

//안드로이드의 소켓 대기를 한다
void acceptAndroid(void* arg){
	AndroidAccepter* aa=(AndroidAccepter*)arg;
		//윈속 초기화
	WSADATA wsd;
	int s, ns, addrsize;
	sockaddr_in client, server;
	vector<string> vec;
	string opcode;
	if(WSAStartup(MAKEWORD(2,2), &wsd) != 0)
	{
		printf("winsock 오류\n");
		return;
	}

	/************server bind**************/
	server.sin_family = AF_INET;
	server.sin_addr.s_addr = INADDR_ANY;		// 로컬 서버(localhost)를 나타냄. 혼자 놀이용.
	server.sin_port = htons(PORT);

	// bind() - 새로 오는 클라이언트를 받을 welcome 소켓
	s = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
	bind(s, (sockaddr *)&server, sizeof(server));
		
	// listen 포트를 열어 두어 떡밥을 기다리는 중
	listen(s,2);
	/***************************************/
	addrsize = sizeof(client);

	while(true){
		cout<<"클라이언트 대기"<<endl;
		ns = accept(s, (sockaddr*)&client, &addrsize);
		if(ns == INVALID_SOCKET)
		{
			printf("accept() 오류\n");
			WSACleanup();
			return;
		}
		printf("%s:%d로 접속\n", inet_ntoa(client.sin_addr),ntohs(client.sin_port));
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