extern "C" void PASCAL EXPORT Area_Copy1(long hl,long left,long top,long w,long h);
extern "C" long PASCAL EXPORT Area_to_bmp1(long hl,long left,long top,long w,long h,char *name);
extern "C" long PASCAL EXPORT Area_to_jpeg1(long hl,long left,long top,long w,long h,char * name,long qa);
extern "C" void PASCAL EXPORT init_jpeg1();
extern "C" long PASCAL EXPORT readjpg1(char *name,long hd,long l,long t,long *width,long *height);
extern "C" long PASCAL EXPORT to_clip1(long hl,long t);
extern "C" long PASCAL EXPORT showjpg1(char *name,long hd,long l,long t);
extern "C" long PASCAL EXPORT show_bmp1(char *name,long hl,long left,long top);
extern "C" long PASCAL EXPORT jpg_to_bmp1(char *jpg,char *bmp);
extern "C" long PASCAL EXPORT bmp_to_jpg1(char *bmp,char * jpg,long qa);
extern "C" long PASCAL EXPORT copy_to_bmp1(long hl,char * name,long t);
extern "C" long PASCAL EXPORT copy_to_jpeg1(long hl,char * name,long t,long qa);
extern "C" long PASCAL EXPORT clip_to_bmp1(char *name,long hl);
extern "C" long PASCAL EXPORT clip_to_jpeg1(char *name,long hl,long qa);
extern "C" long PASCAL EXPORT findwindow1(char *title);


