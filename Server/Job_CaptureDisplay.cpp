#include "Job_CaptureDisplay.h"
#include "Tokenizer.h"
#include "WPFReceiver.h"
#include <vector>
#include <tchar.h>
#include <strstream>
#include <iostream>

using std::cout;
using std::endl;
using std::vector;
using std::istrstream;


//디폴트 생성자로 static 객체 생성

queue<int, list<int>> Job_CaptureDisplay::idQueue;
map<int,Job_CaptureDisplay*> Job_CaptureDisplay::id_instance_table;
map<string,bool> Job_CaptureDisplay::isCaptureTable;

Job_CaptureDisplay::Job_CaptureDisplay(TaskInfo info){
		/********GDI Plus 초기화***********/	
	GdiplusStartupInput         gdiplusStartupInput;
	ULONG_PTR                   gdiplusToken;
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);
	/*********************************/
	this->info=info;
	loopFlag=true;

	isPrevMenu=false;
	devicePointX=800;
	devicePointY=500;
}

void Job_CaptureDisplay::job_start(){
	initGdiPlus();
	int count=0;
	while(idQueue.empty()){ //busyWaiting
		Sleep(1);
		count++;
		if(count==5000){
			finish(LabelID);
			return;
		}
	}
	/*********ID등록************/
	int id=idQueue.front();
	LabelID=id;
	idQueue.pop(); //delete
	id_instance_table.insert(make_pair(id,this)); //id,instance
	/***************************/
	vector<string> vec;
	char temp1[10];
	int totalCount=0,readCount=0;

	/*capture member*/
	string isTouch;
	string isMenu;
	int touchX;
	int touchY;
	bool isCapture=false;
	FILE* capturefile;
	
	int capturefilesize;
	char capturetemp[10];
	/****************/
	while(loopFlag){
		vec.clear();
		if(!receiveStringProtocolforTCP(stringbuf)){ //정상종료 루틴
			finish(LabelID);
			break;
		}
		//receiveStringProtocolforTCP(stringbuf);
		Tokenizer::Tokenize(stringbuf,vec);
		int angle=atoi(vec.at(0).c_str());
		isMenu=vec.at(1);
		isTouch=vec.at(2);
		
		if(isMenu=="STOP"){ //예외처리
			cout<<"STOP!"<<endl;
			finish(LabelID);
			break;
		}
		
		//map<int,Job_CaptureDisplay*>::iterator it =id_instance_table.find(id);
		//캡쳐완료여부 체크
		map<string,bool>::iterator it=isCaptureTable.find(info.ipAdd);
		if(!(it==isCaptureTable.end())){ 
			isCapture=it->second;
			if(isCapture==true){//캡쳐된 것이 있다면
				isCaptureTable[info.ipAdd]=false; //다시 false로 만들어 놓고
				string fileName;
				fileName.append("c:/Touchtable/capture_");
				fileName.append(info.ipAdd);
				fileName.append(".jpg");
				fopen_s(&capturefile,fileName.c_str(), "rb");
				fseek( capturefile, 0, SEEK_END );
				capturefilesize = ftell( capturefile );
				fseek( capturefile, 0, 0 );
				memset(capturetemp,0,10);
				_itoa_s(capturefilesize,capturetemp,10);
			}
		}
		
		//opcode&ip&angle&x&y&state
		if(!(isPrevMenu==false&&isMenu=="false")){
			
			touchX=atoi(vec.at(3).c_str());
			touchY=atoi(vec.at(4).c_str());
			
			int state;
			if(isPrevMenu==false&&isMenu=="true"){//WPF로 좌표 보내주기
				isPrevMenu=true;
				state=0;//add
			}else if(isPrevMenu==true&&isMenu=="false"){
				isPrevMenu=false;
				state=2;//del
			}else if(isPrevMenu==true&&isMenu=="true"){
				state=1;//set
			}
			WPFReceiver::getInstance()->sendDeviceCoordinate(info.ipAdd,angle
					,devicePointX,devicePointY,state,isTouch,touchX,touchY);
		}else{
			if(isTouch=="true"){
				touchX=atoi(vec.at(3).c_str());
				touchY=atoi(vec.at(4).c_str());
				WPFReceiver::getInstance()->sendDeviceCoordinate(info.ipAdd,angle
					,devicePointX,devicePointY,-1,isTouch,touchX,touchY);
			}
		}

		//이미지 만들기
		screenCapture(angle);

		//이미지 보내기
		memset(temp1,0,10);
		_itoa_s(returnitem.length,temp1,10);
		string sendStr;
		sendStr.append(temp1);
		sendStr.append("&");
		if(isCapture){
			sendStr.append("true");
		}else{
			sendStr.append("false");
		}
		sendStr.append("&");
		sendStr.append(capturetemp);
		sendStr.append("&");

		
		sendStringProtocol(sendStr,stringbuf);
		
		/***********send Capture Image************/
		if(isCapture){
			isCapture=false;
			totalCount=0,readCount=0;
			while(true){
				//	cout<<"fileSize:"<<filesize<<"total:"<<totalCount<<"count:"<<count++<<"flag:"<<fileBuf[0]<<endl;	
				if(totalCount>=capturefilesize){
					break;
				}else if(totalCount+CAPTUREBUFSIZE<=capturefilesize){
					readCount=fread(filebuf,sizeof(char),CAPTUREBUFSIZE,capturefile);
					//read(file,fileTransbuf,
					send(info.socketNum,filebuf,readCount,0);
					totalCount+=readCount;
				}else if(totalCount+CAPTUREBUFSIZE>capturefilesize){
					int remainder=capturefilesize-totalCount;
					readCount=fread(filebuf,sizeof(char),remainder,capturefile);

					send(info.socketNum,filebuf,readCount,0);
					totalCount+=readCount;
				}
			}
			fclose(capturefile);
		}
		/*****************************************/
		totalCount=0,readCount=0;
		int filesize=returnitem.length;
		istrstream arrStream(returnitem.buf, filesize);
		
		//int count=0;
		while(true){
			//cout<<"fileSize:"<<filesize<<"total:"<<totalCount<<"count:"<<count++<<"flag:"<<filebuf[0]<<endl;	
			if(totalCount>=filesize){
				break;
			}else if(totalCount+CAPTUREBUFSIZE<=filesize){
				arrStream.read(filebuf,CAPTUREBUFSIZE);
				
				//read(file,fileTransbuf,
				send(info.socketNum,filebuf,CAPTUREBUFSIZE,0);
				totalCount+=CAPTUREBUFSIZE;
			}else if(totalCount+CAPTUREBUFSIZE>filesize){
				int remainder=filesize-totalCount;
				arrStream.read(filebuf,remainder);
				send(info.socketNum,filebuf,remainder,0);
				totalCount+=remainder;
			}
		}

		delete_captureImage();

		//loopFlag=false;
	}
}

void Job_CaptureDisplay::pushID(int id){
	idQueue.push(id);
}

void Job_CaptureDisplay::finish(int id){
	loopFlag=false;
	DeleteDC(hMemDC);
	DeleteDC(hScrDC);
	map<int,Job_CaptureDisplay*>::iterator it =id_instance_table.find(id);
	if(it!=id_instance_table.end()) //요소가 있을 경우
		id_instance_table.erase(id);
}

void Job_CaptureDisplay::setXYCoordinate(int x,int y){
	devicePointX=x;
	devicePointY=y;
}

void Job_CaptureDisplay::initGdiPlus(){

	ScreenWidth = GetSystemMetrics(SM_CXSCREEN);
	ScreenHeight = GetSystemMetrics(SM_CYSCREEN);
	/*********핸드폰 스크린 사이즈 조정********/
	displayWidth=ScreenWidth/9;
	displayHeight=ScreenHeight/3.5;
	/***************화면 모서리 잘림현상 발생시 조정********************/
	//padding=displayHeight/7;
	padding=0;
	/*****************************************************************/
	
	hScrDC=CreateDC("DISPLAY",NULL,NULL,NULL);
	hMemDC = CreateCompatibleDC(hScrDC);

	GetEncCLSID(L"image/jpeg", &clsid); // bmp : L"image/bmp", png : L"image/png", ...
	
	encoderParameters.Count = 1;  
	encoderParameters.Parameter[0].Guid = EncoderQuality;  
	encoderParameters.Parameter[0].Type = EncoderParameterValueTypeLong ;  
	encoderParameters.Parameter[0].NumberOfValues = 1;  
	ULONG quality = 100;
	encoderParameters.Parameter[0].Value = &quality;  

}

BOOL Job_CaptureDisplay::GetEncCLSID(WCHAR *mime, CLSID *pClsid)
{
	UINT num,size,i;
	ImageCodecInfo *arCod;
	BOOL bFound=FALSE;
	GetImageEncodersSize(&num,&size);
	arCod=(ImageCodecInfo *)malloc(size);
	GetImageEncoders(num,size,arCod);
	for (i=0;i<num;i++) {
		if(wcscmp(arCod[i].MimeType,mime)==0) {
			*pClsid=arCod[i].Clsid;
			bFound=TRUE;
			break;
		}    
	}
	free(arCod);
	return bFound;
}

void Job_CaptureDisplay::screenCapture(int angle){

	GetEncCLSID(L"image/jpeg", &clsid); // bmp : L"image/bmp", png : L"image/png", ...
	EncoderParameters encoderParameters;  
	encoderParameters.Count = 1;  
	encoderParameters.Parameter[0].Guid = EncoderQuality;  
	encoderParameters.Parameter[0].Type = EncoderParameterValueTypeLong ;  
	encoderParameters.Parameter[0].NumberOfValues = 1;  
	ULONG quality = 100;
	encoderParameters.Parameter[0].Value = &quality;  
	
/********************************사이클*******************************************/

	hBitmap = CreateCompatibleBitmap(hScrDC, (displayHeight+padding)*2, (displayHeight+padding)*2); 
	SelectObject(hMemDC, hBitmap);

	int leftX=(devicePointX-displayHeight-padding);
	int	leftY=(devicePointY-displayHeight-padding);
	BitBlt(hMemDC, -leftX, -leftY, 
		(displayHeight+padding)*2+leftX, (displayHeight+padding)*2+leftY,
		hScrDC, 0, 0, SRCCOPY); //화면DC를 메모리DC로 복사   
	
	pBitmap = Bitmap::FromHBITMAP(hBitmap, NULL);
	
	groundBitmap=new Bitmap(displayWidth, displayHeight, PixelFormat24bppRGB);                                      // 회전에 쓰일 빈 이미지
    Graphics gx(groundBitmap);
	
	/**********핸드폰 중심좌표 옮기기*********/
	gx.TranslateTransform(210,100); //중심점 옴기기
	/*****************************************/

	gx.RotateTransform(-angle);   

	
	gx.DrawImage(pBitmap,-displayHeight-padding,-displayHeight-padding,pBitmap->GetWidth(),pBitmap->GetHeight());      // 그리기
	                         
	//for test
	//pBitmap->Save(L"test1.jpg", &clsid, &encoderParameters);

	if ( ::CreateStreamOnHGlobal( NULL, TRUE, &pStream ) == S_OK ) //글로벌 메모리 Istream만들어 주기
	{
		if ( groundBitmap->Save(pStream, &clsid, &encoderParameters) == Ok ) //Istream에 jpeg byteStream저장하기
		{
			HGLOBAL hGlobal = NULL;
			::GetHGlobalFromStream( pStream, &hGlobal );

			LONG imgsz = GlobalSize( hGlobal ); //사이즈 얻어오기
			char* lpbuffer =(char*)GlobalLock( hGlobal ); //stream버퍼 얻어오기

			if ( lpbuffer )
			{
				returnitem.buf=lpbuffer;
				returnitem.length=imgsz;
			}
		}
	
	}    
	
}

void Job_CaptureDisplay::request_renew_XY(int id,int x,int y){
	map<int,Job_CaptureDisplay*>::iterator it =id_instance_table.find(id);
	if(it==id_instance_table.end()){ //객체가 아직 등록되지 않은 경우에는 함수 종료
		return;
	}
	Job_CaptureDisplay* cd=it->second;
	cd->setXYCoordinate(x,y);
}

void Job_CaptureDisplay::request_delete_id(int id){
	map<int,Job_CaptureDisplay*>::iterator it =id_instance_table.find(id);
	if(it==id_instance_table.end()){ //객체가 아직 등록되지 않은 경우에는 함수 종료
		return;
	}
	Job_CaptureDisplay* cd=it->second;
	cd->finish(id);
}

void Job_CaptureDisplay::delete_captureImage(){
	DeleteObject(hBitmap);
	delete pBitmap;

	delete groundBitmap;
	pStream->Release();
}
