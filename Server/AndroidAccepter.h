//ΩÃ±€≈Ê
#pragma once
#define PORT 4888
#define _WINSOCKAPI_
#define STRINGBUFSIZE 150

class AndroidAccepter{
public:
	static AndroidAccepter* getInstance();
	void accepterStart();
	friend void acceptAndroid(void* arg);
	static bool receiveStringProtocol(int socketNum,char* buf);
private:
	static AndroidAccepter* accepter;
	char stringbuf[STRINGBUFSIZE];
	
};