#include "WPFReceiver.h"
#include "Tokenizer.h"
#include "ThreadPool.h"
#include "Job_FileSendToAnd.h"
#include "Job_CaptureDisplay.h"
#include "Job_RequestContacts.h"
#include <process.h>

#include <stdio.h>
#include <vector>
#include <iostream>

using std::cout;
using std::endl;
using std::vector;

WPFReceiver* WPFReceiver::receiver=0;

WPFReceiver::WPFReceiver(){
}

WPFReceiver* WPFReceiver::getInstance(){
	if(!receiver){
		receiver=new WPFReceiver();
	}
	return receiver;
}

void WPFReceiver::receiverStart(){
		DWORD dwThreadID;
	CreateThread( NULL, 0, (LPTHREAD_START_ROUTINE)receiverLoop, (void *)this, 0 ,&dwThreadID);
}

void receiverLoop(void* arg){
	WPFReceiver* rc=(WPFReceiver*)arg;
	WSADATA   wsaData;

	if( WSAStartup(0x202,&wsaData) == SOCKET_ERROR )
	{
		printf("WinSock 초기화부분에서 문제 발생.n");
		WSACleanup();
	}
	memset( &(rc->ServerInfo), 0, sizeof(rc->ServerInfo) );
	memset( &(rc->FromClient), 0, sizeof(rc->FromClient) );
	
	rc->ServerInfo.sin_family  = AF_INET;
	rc->ServerInfo.sin_addr.s_addr = inet_addr( "127.0.0.1" );
	rc->ServerInfo.sin_port   = htons( UDPPORT ); // 포트번호
	//################################### create #########################################
	rc->ServerSocket = socket( AF_INET, SOCK_DGRAM, 0 ); // udp용 소켓 생성

	if( rc->ServerSocket == INVALID_SOCKET )
	{
		printf("소켓을 생성할수 없습니다.");
		closesocket( rc->ServerSocket );
		WSACleanup();
		exit(0);
	}
	//##################################################################################

	//################################### bind #########################################
	if( bind( rc->ServerSocket, (struct sockaddr*)&(rc->ServerInfo), //바인드 소켓에 서버정보 부여
		sizeof(rc->ServerInfo) ) == SOCKET_ERROR )
	{
		printf("바인드를 할 수 없습니다.");
		closesocket( rc->ServerSocket );
		WSACleanup();
		exit(0);
	}

	rc->FromClient_Size = sizeof(rc->FromClient );
	int Recv_Size;
	cout<<"클라이언트 접속대기 중"<<endl;
	Recv_Size=recvfrom( rc->ServerSocket, rc->stringbuf, STRINGBUFSIZE, 0,
         (struct sockaddr*) &(rc->FromClient), &(rc->FromClient_Size) );
	cout<<"WPF클라이언트 접속 완료"<<endl;
	if( Recv_Size<0 ){ printf("recefrom() error! \n"); exit(0); }

	vector<string> vec;

	while(true){
		memset( rc->stringbuf, 0, STRINGBUFSIZE );
		vec.clear();
		
		Recv_Size=recvfrom( rc->ServerSocket, rc->stringbuf, STRINGBUFSIZE, 0,
			(struct sockaddr*) &(rc->FromClient), &(rc->FromClient_Size) );
		cout<<"WPF 메세지 수신"<<endl;
		if( Recv_Size<0 ){ 
			printf("recefrom() error! \n"); exit(0);
		}
		Tokenizer::Tokenize(rc->stringbuf,vec);
		string opcode=vec.at(0);

		if(opcode=="FILESENDTOAND"){
			string ip=vec.at(2);
			if(!Job_FileSendToAnd::reserveTask(ip,vec)){//등록되어 있지 않으면
				ThreadPool::getInstance()->pushTask(opcode,vec,-1,ip);
			}
		}else if(opcode=="CAPTUREDISPLAY"){
			Job_CaptureDisplay::isCaptureTable[vec.at(1)]=true; //map을 true로 set
		}else if(opcode=="REQUESTCONTACTS"){
			string ip=vec.at(1);
			ThreadPool::getInstance()->pushTask(opcode,vec,-1,ip);
		}else if(opcode=="SAVECONTACT"){
			string ip=vec.at(1);
			ThreadPool::getInstance()->pushTask(opcode,vec,-1,ip);
		}
	}
}

void WPFReceiver::sendFileDisplayStartMessage(string ipAdd,string pathName){
	string str="FILEDISPLAYSTART&";
	str.append(pathName);
	str.append("&");
	str.append(ipAdd);
	str.append("\n");
	sendStringProtocol(str);
}

void WPFReceiver::sendFileDisplayCompleteMessage(string ipAdd,string pathName){
	string str="FILEDISPLAYCOMPLETE&";
	str.append(pathName);
	str.append("&");
	str.append(ipAdd);
	str.append("\n");
	sendStringProtocol(str);
}

void WPFReceiver::sendStringProtocol(string str){
	sendto( ServerSocket, str.data(), str.length() , 0,
       (struct sockaddr*) &FromClient, sizeof( FromClient ) );
}

void WPFReceiver::sendDeviceCoordinate(string ip,int angle,int x,int y,int state,string isTouch,int touchX,int touchY){//state0:add 1:set 2:del
	memset(temp1,0,10);
	memset(temp2,0,10);
	memset(temp3,0,10);
	memset(temp4,0,10);
	memset(temp5,0,10);
	memset(temp6,0,10);
	string sendStr="CAPTUREDISPLAY&"+ip+"&";
	_itoa_s(angle,temp1,10);
	_itoa_s(x,temp2,10);
	_itoa_s(y,temp3,10);
	_itoa_s(state,temp4,10);
	_itoa_s(touchX,temp5,10);
	_itoa_s(touchY,temp6,10);
	sendStr.append(temp1);
	sendStr.append("&");
	sendStr.append(temp2);
	sendStr.append("&");
	sendStr.append(temp3);
	sendStr.append("&");
	sendStr.append(temp4);
	sendStr.append("&");
	sendStr.append(isTouch);
	sendStr.append("&");
	sendStr.append(temp5);
	sendStr.append("&");
	sendStr.append(temp6);
	sendStr.append("&\n");
	sendStringProtocol(sendStr);
}