#pragma once
#include "Job.h"
#include <vector>
#include <queue>
#include <list>
#include <string>
#include <map>
#include <windows.h>
#include <stdio.h>
#include <gdiplus.h>
#include <tchar.h>

#define CAPTUREBUFSIZE 1400
using std::make_pair;
using std::map;
using std::string;
using std::queue;
using std::list;
using std::vector;
using namespace Gdiplus;

typedef struct ImageItem{
	char *buf;
	int length;
}ImageItem;

class Job_CaptureDisplay 
	:public Job{
private:
	static queue<int, list<int>> idQueue;
	static map<int,Job_CaptureDisplay*> id_instance_table;
	
	
	bool loopFlag;
	char filebuf[CAPTUREBUFSIZE];
	char stringbuf[STRINGBUFSIZE];
	int displayWidth;
	int displayHeight;
	ImageItem returnitem;
	bool isPrevMenu;
	int LabelID;

	int devicePointX;
	int devicePointY;
	int padding;

	//gdiPush member
	CLSID clsid;
	HBITMAP hBitmap;
	int ScreenWidth;
	int ScreenHeight;
	IStream *pStream;
	HDC hScrDC, hMemDC,hDC;
	EncoderParameters encoderParameters;  
	Bitmap *pBitmap;
	Bitmap *groundBitmap;

	void initGdiPlus();
	BOOL Job_CaptureDisplay::GetEncCLSID(WCHAR *mime, CLSID *pClsid);
	void Job_CaptureDisplay::screenCapture(int angle);

public:
	static map<string,bool> isCaptureTable;
	static void request_renew_XY(int id,int x,int y);
	static void request_delete_id(int id);
	void finish(int id);
	void setXYCoordinate(int x, int y);
	static void pushID(int id);
	Job_CaptureDisplay(TaskInfo info);
	void job_start();
	void delete_captureImage();

};