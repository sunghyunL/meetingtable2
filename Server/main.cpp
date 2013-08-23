#include<iostream>
#include <process.h>
#include "ThreadPool.h"
#include "AndroidAccepter.h"
#include "WPFReceiver.h"
#include "CvControl.h"
#include <tchar.h>
using std::cout;
using std::endl;
#pragma comment(linker, "/entry:WinMainCRTStartup /subsystem:console")

int DeleteAllFiles(LPCSTR szDir, int recur) ;

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpszCmdParam, int nCmdShow){
	cout<<"서버 시작"<<endl;

	ThreadPool::getInstance()->makeThreadPool(); //쓰레드 풀 생성
	WPFReceiver::getInstance()->receiverStart();
	
	AndroidAccepter::getInstance()->accepterStart();
	
	//fortest
	CvControl::getInstance()->sendCoordinate(0,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	CvControl::getInstance()->sendCoordinate(1,0,0,0);
	//루트 폴더 만들기
	//DeleteAllFiles("c:/Touchtable",0);

	string forderName1="c:/Touchtable";
	::CreateDirectory(forderName1.c_str(),NULL);

	system("PAUSE");
	return 0;
}

int DeleteAllFiles(LPCSTR szDir, int recur) 
{ 
 HANDLE hSrch;
 WIN32_FIND_DATA wfd;
 int res = 1;

 TCHAR DelPath[MAX_PATH];
 TCHAR FullPath[MAX_PATH];
 TCHAR TempPath[MAX_PATH];
 
 lstrcpy(DelPath, szDir);
 lstrcpy(TempPath, szDir);
 if (lstrcmp(DelPath+lstrlen(DelPath)-4,_T("\\*.*")) != 0)
  lstrcat(DelPath, _T("\\*.*"));

 hSrch = FindFirstFile(DelPath, &wfd);
 if (hSrch == INVALID_HANDLE_VALUE)
 { 
  if (recur > 0) 
   RemoveDirectory(TempPath);

  return -1;
 }
 while(res)
 {
  wsprintf(FullPath, _T("%s\\%s"), TempPath, wfd.cFileName);

  if (wfd.dwFileAttributes & FILE_ATTRIBUTE_READONLY)
  {
   SetFileAttributes(FullPath, FILE_ATTRIBUTE_NORMAL);
  }

  if(wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
  {
   if(lstrcmp(wfd.cFileName, _T(".")) && lstrcmp(wfd.cFileName, _T("..")))
   {
    recur++;
    DeleteAllFiles(FullPath, recur);
    recur--;
   }
  }
  else
  {
   DeleteFile(FullPath);
  }
  res = FindNextFile(hSrch, &wfd);
 }
 FindClose(hSrch);
 
 if (recur > 0) 
  RemoveDirectory(TempPath);

 return 0;
}