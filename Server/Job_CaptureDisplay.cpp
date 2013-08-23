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


//����Ʈ �����ڷ� static ��ü ����

queue<int, list<int>> Job_CaptureDisplay::idQueue;
map<int,Job_CaptureDisplay*> Job_CaptureDisplay::id_instance_table;
map<string,bool> Job_CaptureDisplay::isCaptureTable;

Job_CaptureDisplay::Job_CaptureDisplay(TaskInfo info){
		/********GDI Plus �ʱ�ȭ***********/	
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
	/*********ID���************/
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
		if(!receiveStringProtocolforTCP(stringbuf)){ //�������� ��ƾ
			finish(LabelID);
			break;
		}
		//receiveStringProtocolforTCP(stringbuf);
		Tokenizer::Tokenize(stringbuf,vec);
		int angle=atoi(vec.at(0).c_str());
		isMenu=vec.at(1);
		isTouch=vec.at(2);
		
		if(isMenu=="STOP"){ //����ó��
			cout<<"STOP!"<<endl;
			finish(LabelID);
			break;
		}
		
		//map<int,Job_CaptureDisplay*>::iterator it =id_instance_table.find(id);
		//ĸ�ĿϷῩ�� üũ
		map<string,bool>::iterator it=isCaptureTable.find(info.ipAdd);
		if(!(it==isCaptureTable.end())){ 
			isCapture=it->second;
			if(isCapture==true){//ĸ�ĵ� ���� �ִٸ�
				isCaptureTable[info.ipAdd]=false; //�ٽ� false�� ����� ����
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
			if(isPrevMenu==false&&isMenu=="true"){//WPF�� ��ǥ �����ֱ�
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

		//�̹��� �����
		screenCapture(angle);

		//�̹��� ������
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
	if(it!=id_instance_table.end()) //��Ұ� ���� ���
		id_instance_table.erase(id);
}

void Job_CaptureDisplay::setXYCoordinate(int x,int y){
	devicePointX=x;
	devicePointY=y;
}

void Job_CaptureDisplay::initGdiPlus(){

	ScreenWidth = GetSystemMetrics(SM_CXSCREEN);
	ScreenHeight = GetSystemMetrics(SM_CYSCREEN);
	/*********�ڵ��� ��ũ�� ������ ����********/
	displayWidth=ScreenWidth/9;
	displayHeight=ScreenHeight/3.5;
	/***************ȭ�� �𼭸� �߸����� �߻��� ����********************/
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
	
/********************************����Ŭ*******************************************/

	hBitmap = CreateCompatibleBitmap(hScrDC, (displayHeight+padding)*2, (displayHeight+padding)*2); 
	SelectObject(hMemDC, hBitmap);

	int leftX=(devicePointX-displayHeight-padding);
	int	leftY=(devicePointY-displayHeight-padding);
	BitBlt(hMemDC, -leftX, -leftY, 
		(displayHeight+padding)*2+leftX, (displayHeight+padding)*2+leftY,
		hScrDC, 0, 0, SRCCOPY); //ȭ��DC�� �޸�DC�� ����   
	
	pBitmap = Bitmap::FromHBITMAP(hBitmap, NULL);
	
	groundBitmap=new Bitmap(displayWidth, displayHeight, PixelFormat24bppRGB);                                      // ȸ���� ���� �� �̹���
    Graphics gx(groundBitmap);
	
	/**********�ڵ��� �߽���ǥ �ű��*********/
	gx.TranslateTransform(210,100); //�߽��� �ȱ��
	/*****************************************/

	gx.RotateTransform(-angle);   

	
	gx.DrawImage(pBitmap,-displayHeight-padding,-displayHeight-padding,pBitmap->GetWidth(),pBitmap->GetHeight());      // �׸���
	                         
	//for test
	//pBitmap->Save(L"test1.jpg", &clsid, &encoderParameters);

	if ( ::CreateStreamOnHGlobal( NULL, TRUE, &pStream ) == S_OK ) //�۷ι� �޸� Istream����� �ֱ�
	{
		if ( groundBitmap->Save(pStream, &clsid, &encoderParameters) == Ok ) //Istream�� jpeg byteStream�����ϱ�
		{
			HGLOBAL hGlobal = NULL;
			::GetHGlobalFromStream( pStream, &hGlobal );

			LONG imgsz = GlobalSize( hGlobal ); //������ ������
			char* lpbuffer =(char*)GlobalLock( hGlobal ); //stream���� ������

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
	if(it==id_instance_table.end()){ //��ü�� ���� ��ϵ��� ���� ��쿡�� �Լ� ����
		return;
	}
	Job_CaptureDisplay* cd=it->second;
	cd->setXYCoordinate(x,y);
}

void Job_CaptureDisplay::request_delete_id(int id){
	map<int,Job_CaptureDisplay*>::iterator it =id_instance_table.find(id);
	if(it==id_instance_table.end()){ //��ü�� ���� ��ϵ��� ���� ��쿡�� �Լ� ����
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
