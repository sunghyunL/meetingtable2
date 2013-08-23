//ΩÃ±€≈Ê
#pragma once
#define STRINGBUFSIZE 150
#define UDPPORT 9993
#include <string>
#include <winsock2.h>
using std::string;

class WPFReceiver{
private:
	static WPFReceiver* receiver;
	WPFReceiver();
	char stringbuf[STRINGBUFSIZE];
	SOCKET   ServerSocket;  SOCKADDR_IN  ServerInfo;
    SOCKADDR_IN  FromClient;   int   FromClient_Size;
	char temp1[10];	
	char temp2[10];
	char temp3[10];
	char temp4[10];
	char temp5[10];
	char temp6[10];
public:
	static WPFReceiver* getInstance();
	void receiverStart();
	friend void receiverLoop(void* arg);
	void sendFileDisplayStartMessage(string ipAdd,string pathName);
	void sendFileDisplayCompleteMessage(string ipAdd,string pathName);
	void sendStringProtocol(string str);
	void sendDeviceCoordinate(string ip,int angle,int x,int y,int state,string isTouch,int touchX,int touchY);
};
