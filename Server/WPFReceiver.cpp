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
		printf("WinSock �ʱ�ȭ�κп��� ���� �߻�.n");
		WSACleanup();
	}
	memset( &(rc->ServerInfo), 0, sizeof(rc->ServerInfo) );
	memset( &(rc->FromClient), 0, sizeof(rc->FromClient) );
	
	rc->ServerInfo.sin_family  = AF_INET;
	rc->ServerInfo.sin_addr.s_addr = inet_addr( "127.0.0.1" );
	rc->ServerInfo.sin_port   = htons( UDPPORT ); // ��Ʈ��ȣ
	//################################### create #########################################
	rc->ServerSocket = socket( AF_INET, SOCK_DGRAM, 0 ); // udp�� ���� ����

	if( rc->ServerSocket == INVALID_SOCKET )
	{
		printf("������ �����Ҽ� �����ϴ�.");
		closesocket( rc->ServerSocket );
		WSACleanup();
		exit(0);
	}
	//##################################################################################

	//################################### bind #########################################
	if( bind( rc->ServerSocket, (struct sockaddr*)&(rc->ServerInfo), //���ε� ���Ͽ� �������� �ο�
		sizeof(rc->ServerInfo) ) == SOCKET_ERROR )
	{
		printf("���ε带 �� �� �����ϴ�.");
		closesocket( rc->ServerSocket );
		WSACleanup();
		exit(0);
	}

	rc->FromClient_Size = sizeof(rc->FromClient );
	int Recv_Size;
	cout<<"Ŭ���̾�Ʈ ���Ӵ�� ��"<<endl;
	Recv_Size=recvfrom( rc->ServerSocket, rc->stringbuf, STRINGBUFSIZE, 0,
         (struct sockaddr*) &(rc->FromClient), &(rc->FromClient_Size) );
	cout<<"WPFŬ���̾�Ʈ ���� �Ϸ�"<<endl;
	if( Recv_Size<0 ){ printf("recefrom() error! \n"); exit(0); }

	vector<string> vec;

	while(true){
		memset( rc->stringbuf, 0, STRINGBUFSIZE );
		vec.clear();
		
		Recv_Size=recvfrom( rc->ServerSocket, rc->stringbuf, STRINGBUFSIZE, 0,
			(struct sockaddr*) &(rc->FromClient), &(rc->FromClient_Size) );
		cout<<"WPF �޼��� ����"<<endl;
		if( Recv_Size<0 ){ 
			printf("recefrom() error! \n"); exit(0);
		}
		Tokenizer::Tokenize(rc->stringbuf,vec);
		string opcode=vec.at(0);

		if(opcode=="FILESENDTOAND"){
			string ip=vec.at(2);
			if(!Job_FileSendToAnd::reserveTask(ip,vec)){//��ϵǾ� ���� ������
				ThreadPool::getInstance()->pushTask(opcode,vec,-1,ip);
			}
		}else if(opcode=="CAPTUREDISPLAY"){
			Job_CaptureDisplay::isCaptureTable[vec.at(1)]=true; //map�� true�� set
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