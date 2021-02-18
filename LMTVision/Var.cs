using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.IO.Ports;
using System.Drawing;
using HalconDotNet;


namespace LMTVision
{
    public class PLC
    {
        //
        public static bool IsConnected = false;
        public static Color pColor = Color.Red;
        public static System.Net.Sockets.Socket socket;
        public static int Port = 12289;
        public static string startIp = "192.168.0.1";
        public static System.Net.IPAddress Ip = System.Net.IPAddress.Parse(startIp);
        public static string PLCVisions = "20201020";//PLC版本
        public static string HMIVisions = "20201020";//人機版本

        public static bool[] BTrigger = new bool[11];
        public static int Blocation = 0;
        public static int Qlocation = 0;
        public static bool[] allstatus = new bool[16];
        public static bool[] loopstatus = new bool[16];
        public static bool[] ccdTrigger = new bool[24];
        public static bool[] Barstatus = new bool[16];
        public static bool LogTrigger = false;
        public static bool Tray1end = false;
        public static string MachineStatus;
        public static string MachineStatusL;
        public static string PlateCount = "0";
        public static string PlcStatus = "0000";
        public static string PlcStatusP = "0000";

        public static bool bPlateEnd_PZ0 = false;
        public static bool bPlateEnd_AZ0 = false;
        //AZ0最大盤型X
        public static int AZ0TrayMax_X = 20;
        //AZ0最大盤型Y
        public static int AZ0TrayMax_Y = 20;
        //PZ0最大盤型X
        public static int PZ0TrayMax_X = 20;
        //PZ0最大盤型Y
        public static int PZ0TrayMax_Y = 20;
    }
    public class Sys
    {
        public static string Factory = "";
        public static string MachineId = "";
        public static string IniPath = Application.StartupPath + "\\Ini";
        public static string AlarmPath = "D:\\GSLMT Report" + "\\Alarm";
        public static string ReportLog = "D:\\GSLMT Report" + "\\Log";
        public static string ReportLog1 = "D:\\MB Mounter Report" + "\\Log";
        public static string ReportImage = "D:\\GSLMT Report" + "\\Image";
        public static bool CloseNow = false;
        public static List<string> Productions = new List<string>();
        public static string CurrentProduction = "";
        public static string CurrentProID = "";
        public static string CurrentBarID = "";
        public static bool CurProduceIDCheck = false;
        public static string CurErrMessage = "";
        public static string CurErrNum = "";
        public static string Codes = "M";  //cbCodes索引（0-1）；分别记名为M、F
        public static int TrigMode = 1;//CCD 1:硬触发;2:软触发
        public static bool NoAutoMatic = false;
        public static bool ErrRead = false;
        public static bool AssTest = false;
        public static string AssLocation = "";
        public static string AssLocation2 = "";
        public static string AssX = "", AssY = "";
        public static string[] AssHX = new string[5];
        public static string[] AssHY = new string[5];
        public static string[] AssLX = new string[5];
        public static string[] AssLY = new string[5];
        public static string AssHXA11t = "", AssHYA11t = "", AssLXA11t = "", AssLYA11t = "";
        public static string AssHXA21t = "", AssHYA21t = "", AssLXA21t = "", AssLYA21t = "";
        public static string AssHXP1t = "", AssHYP1t = "", AssLXP1t = "", AssLYP1t = "";
        public static string AssDisX = "", AssDisY = "", AssDis = "";
        public static int AssTestNum = 0;
        public static bool autoTest = false;
        public static bool autoTestL = false;
        public static string autoTestTime = "";
        public static int autoTestNoA11 = 0, autoTestNoA21 = 0;
        public static int A11autoS = 0, A21autoS = 0;
        public static bool P1autoS = false;
        public static bool P1DisMode2 = false;
        public static string P1Result = "";        
        public static bool bCalView = false;
        public static bool CalViewPross = false;
        public static int calviewNum = 0;
        public static int calviewNumL = 0;
        public static double[,] pXY = new double[,] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };//
        public static  List<Label> LabelsTray = new List<Label>();
        public static double rmin = 0.0;
        public static double Orirmin = 0.0;
        public static double diam_min = 0.0;
        public static string CoatResult = "";
        public static string setStation = "";

        public static double NumBase = 0.0;
        public static double NumAdd = 0.0;
        public static double NumBase2 = 0.0;
        public static double NumAdd2 = 0.0;
        public static bool TimerGlue = false;
        public static bool TimerOtherGlue = false;
        public static bool TimerOEE = false;
        public static bool FCTShow = false;
        public static string Mode5Distance = "";     //Barcodeq缺口半径
        public static string Mode5Distance1 = "";    //对称缺口长度逆时针
        public static string Mode5Distance2 = "";    //对称缺口长度顺时针
        public static string Mode5result = "";
        public static double[] LogMsgSign = new double[8]; //0：PQC、PCCD1取像最终放入TRAY盘标记  1：A1CCD2盘/A1CCD1取像坐标标记 2：A2CCD2/A2CCD1盘取像坐标标记 3;PCCD2盘取像坐标标记 6;A1CCD2平台1当前坐标标记   7:A2CCD2平台2当前坐标标记
        public static string ECCIP = "";
    }
    public class User
    {
        public static Dictionary<string, string> Total = new Dictionary<string, string>();
        public static string CurrentUser = "";
    }
    public class WriteToPlc
    {
        //Vision
        public static bool[] CMDsend = new bool[9];
        public static string[] CMDOKNG = new string[9];
        public static string[] CMDresult = new string[9];
        public static string CMDresult4 = "";
        public static string CMDRead = "";
        public static bool CMDRsend = false;
        public static double DoubleRead = 0.0;
        public static bool CMDchecksend = false;
        public static string CMDcheckR = "";
        public static bool bTimeCelebrate = false;
        public static string sTimeCelebrate = "";
        //L_Trigger
        public static bool bBarcodeTrigger = false;
        public static string sBarcodeTrigger = "";
        public static bool bBRsend = false;
        public static string sBRsend = "";
        //R_Trigger
        public static bool bRBarcodeTrigger = false;
        public static string sRBarcodeTrigger = "";
        public static bool bRBRsend = false;
        public static string sRBRsend = "";
        //height
        public static bool[] bCmdGT2 = new bool[5];
        public static string[] strCmdGT2 = new string[5] { "", "", "", "", "" };
    }
    public class Protocol
    {
        public static byte[] GetViewA1CCD1 = Encoding.ASCII.GetBytes("01WRDD00002,01\r\n");
        public static byte[] GetViewA1CCD2 = Encoding.ASCII.GetBytes("01WRDD00010,01\r\n");
        public static byte[] GetViewA2CCD1 = Encoding.ASCII.GetBytes("01WRDD00024,01\r\n");
        public static byte[] GetViewA2CCD2 = Encoding.ASCII.GetBytes("01WRDD00032,01\r\n");
        public static byte[] GetViewPCCD1 = Encoding.ASCII.GetBytes("01WRDD00046,01\r\n");
        public static byte[] GetViewPCCD2 = Encoding.ASCII.GetBytes("01WRDD00054,01\r\n");
        public static byte[] GetViewGCCD1 = Encoding.ASCII.GetBytes("01WRDD00076,01\r\n");
        public static byte[] GetViewGCCD2 = Encoding.ASCII.GetBytes("01WRDD00084,01\r\n");
        public static byte[] GetViewQCCD = Encoding.ASCII.GetBytes("01WRDD00068,01\r\n");
        public static byte[] GetReader = Encoding.ASCII.GetBytes("01WRDD00124,11\r\n");

        public static byte[] GetLogTri = Encoding.ASCII.GetBytes("01WRDD00301,01\r\n");
        public static byte[] GetLogMes = Encoding.ASCII.GetBytes("01WRDD00302,23\r\n");
        public static byte[] GetLogMesSign = Encoding.ASCII.GetBytes("01WRDD00220,08\r\n");
        public static byte[] GetErrTri = Encoding.ASCII.GetBytes("01WRDD00350,01\r\n");
        public static byte[] GetErrMes = Encoding.ASCII.GetBytes("01WRDD00352,15\r\n");
        public static string ErrMes = "";
        public static string strPCRead = "0000";
        public static bool strPCRead_PCReady = false;//PC準備 D1 0位
        public static bool strPCRead_GlueTimeOut = false;//膠水超時 D1 1位
        public static bool strPCRead_GlueUpdate = false;//膠水超時 D1 6位


        public static bool IsPCRead = false;
        public static byte[] GetloopTri = Encoding.ASCII.GetBytes("01WRDD00001,01\r\n");
        public static bool IsloopRead = false;
    }

    public class LIGHT1
    {
        public static bool IsConnected = false;
        public static Color lColor = Color.Red;
        public static byte[] ReadCh1 = new byte[] { 0x86, 0x55, 0xAA, 0x21, 0x00, 0xA6 };
        public static List<DateTime> OpTime = new List<DateTime>();
        public static string CurrentBarcode;
        public static DateTime CurrentOpTime;
        public static int SerialNumber = 0;
        public static SerialPort Com = new SerialPort("COM1", 19200, Parity.None, 8, StopBits.One);
    }
    public class LIGHT2
    {
        public static bool IsConnected = false;
        public static Color lColor = Color.Red;
        public static byte[] ReadCh1 = new byte[] { 0x86, 0x55, 0xAA, 0x21, 0x00, 0xA6 };
        public static List<DateTime> OpTime = new List<DateTime>();
        public static string CurrentBarcode;
        public static DateTime CurrentOpTime;
        public static int SerialNumber = 0;
        public static SerialPort Com = new SerialPort("COM2", 19200, Parity.None, 8, StopBits.One);
    }
    public class Reader //Tray2
    {
        public static bool IsChecked = false;
        public static bool IsConnected = false;
        public static string Barcode = "";
        public static string newBarcode = "";
        public static DateTime OpTime = new DateTime();
        public static string CurrentBarcode;
        public static DateTime CurrentOpTime;
        public static int SerialNumber = 0;
        public static SerialPort Com = new SerialPort("COM3", 19200, Parity.None, 8, StopBits.One);
        public static bool Trigger = false;
        public static bool WebChecked = false;
        public static string Row;
        public static string Col;
        public static string CurrentRow;
        public static string CurrentCol;
        public static string strTrRead = "0000";
        public static bool IsTrRead = false;
    }
    public class RiReader //Tray1
    {
        public static bool IsChecked = false;
        public static bool IsConnected = false;
        public static string Barcode = "";
        public static string newBarcode = "";
        public static DateTime OpTime = new DateTime();
        public static string CurrentBarcode;
        public static DateTime CurrentOpTime;
        public static int SerialNumber = 0;
        public static SerialPort Com = new SerialPort("COM4", 19200, Parity.None, 8, StopBits.One);
        public static bool Trigger = false;
        public static bool WebChecked = false;
        public static bool BPFWebChecked = false;
        public static bool Web_Tray_InOutStation = false;//管控PZ0進出站
        public static bool Web_Tray2InTray1Out_InOutStation = false;//管控PZ0進出站-前進後出
        public static bool Web_Tray_InOutStation_ErrorIgnore = false;//管控PZ0進出站
        public static bool Web_Tray_InOutStation_Complete = false;//進出站完成
        public static string Web_Tray_InOutStation_PlcResult = "0000";//進出站給Plc結果
        public static bool Web_Tray_InOutStation_Result = false;//進出站結果
        public static string Web_Tray_InOutStation_sMsg = "";//進出站錯誤信息
        public static string Barcode_Out = "";
        public static string Barcode_In = "";

        public static string Row;
        public static string Col;
        public static string CurrentRow;
        public static string CurrentCol;
        public static string strTrRead = "0000";
        public static bool IsTrRead = false;
    }
    public class GlueCU
    {
        public static int choice = 0;
        public static bool IsCheck = false;
        public static bool IsConnected = false;
        public static bool IsSendPara = false;
        public static bool IsSendPara808 = false;
        public static bool IsOk = false, IsOk1 = false, IsOk2 = false, IsOk3 = false;
        public static Color gColor = Color.Red;
        public static SerialPort Com = new SerialPort("COM5", 19200, Parity.None, 8, StopBits.One);
        public static NumericUpDown[] Time_array = new NumericUpDown[10];
        public static NumericUpDown[] Pressure_array1 = new NumericUpDown[10];
        public static NumericUpDown[] Pressure_array2 = new NumericUpDown[10];
        public static Label[] ShowTime_array = new Label[10];
        public static Label[] ShowPressure_array1 = new Label[10];
        public static Label[] ShowPressure_array2 = new Label[10];
        public static double[] arrayTime = new double[10];
        public static double[] arrayPressure = new double[20];
        public static string sendTime = null, sendPressure1 = null, sendPressure2 = null;
        public static string[] sendTime808 = new string[2];
        public static string[] sendPressure808 = new string[2];
        public static string TimeAddress = "7000"; //时间首地址
        public static string PressureAddress1 = "7221";   //正压力首地址
        public static string PressureAddress2 = "7231";   //负压力首地址
        public static string CutChAddress = "7021";   //通道切换地址
        public static string ChNumAddress = "7030";   //通道数
        public static byte[] send_Time_buff = MethodGlueCU.Read_plc(TimeAddress, 10); //读取时间命令
        public static byte[] send_Pressure_buff1 = MethodGlueCU.Read_plc(PressureAddress1, 20);//读取压力命令
        public static byte[] chNum = MethodGlueCU.Read_plc(ChNumAddress, 1);//读取压力命令
        public static int ChanleNumber = 10;
        public static int LChanleNumber = 10;
        public static int sendChanleNumber = 10;
    }
    public class LIGHT3
    {
        public static bool IsConnected = false;
        public static bool IsChecked = false;
        public static Color lColor = Color.Red;
        public static byte[] ReadCh1 = new byte[] { 0x86, 0x55, 0xAA, 0x21, 0x00, 0xA6 };
        public static List<DateTime> OpTime = new List<DateTime>();
        public static string CurrentBarcode;
        public static DateTime CurrentOpTime;
        public static int SerialNumber = 0;
        public static SerialPort Com = new SerialPort("COM6", 19200, Parity.None, 8, StopBits.One);
    }

    public class Barcode1
    {
        public static bool QCCDisChecked = false;
        public static bool qBarcodeTrig = false;
        public static bool IsChecked = false;
        public static bool IsConnected = false;
        public static Color bColor = Color.Red;
        public static bool bOpen = false;
        public static IPAddress ip;
        public static string Gain = "0";
        public static string ExposureTime = "35000";

        public static bool bContinuousShot = false;
        public static bool bBarcodeRangeSearch = false;
        //解码范围
        public static HTuple HandleRow1 = 0;
        public static HTuple HandleCol1 = 0;
        public static HTuple HandleRow2 = 576;
        public static HTuple HandleCol2 = 720;
        //解码结果
        public static string sResultBarcode = "";
        //解码角度结果
        public static double LensBarcodeAngle = 0;
        //图片
        public static HObject theBarImage = new HObject();
        public static HObject ResultImage;
        public static bool OkSave = false;//OK图片保存
        public static bool NgSave = false;//NG图片保存

        public static string Result_ReadOK = "0";
        public static bool bReadOK = false;//Barcode 读取完成，通知PLC
        public static bool Mirrored = false;
        public static int BarcodeAngleSet = 0;
        public static int AllowableOffsetAngle = 0;
        public static int Production = 0;
    }
    public class Barcode2
    {
        public static bool IsChecked = false;
        public static bool IsConnected = false;
        //读取结果
        public static Dictionary<int, string> BarCodes = new Dictionary<int, string>();
        public static Dictionary<int, string> TriggerTime = new Dictionary<int, string>();
        public static Dictionary<int, string> DecodeTime = new Dictionary<int, string>();
        public static Dictionary<int, string> Grade = new Dictionary<int, string>();
        public static Dictionary<int, string> symbolContrast = new Dictionary<int, string>();
        public static Dictionary<int, string> printGrowth = new Dictionary<int, string>();
        public static Dictionary<int, string> UEC = new Dictionary<int, string>();
        public static Dictionary<int, string> modulation = new Dictionary<int, string>();
        public static Dictionary<int, string> fixedPatternDamage = new Dictionary<int, string>();
        public static Dictionary<int, string> gridNonUniformity = new Dictionary<int, string>();
        //图片
        public static Dictionary<int, Image> Images = new Dictionary<int, Image>();
        public static bool OkSave = false;//OK图片保存
        public static bool NgSave = false;//NG图片保存

        public static string Result_ReadOK = "0";
        public static bool bReadOK = false;//Barcode 读取完成，通知PLC
    }
    public class Barcode3
    {
        public static bool IsChecked = false;
        public static bool IsConnected = false;
        //读取结果
        public static Dictionary<int, string> BarCodes = new Dictionary<int, string>();
        public static Dictionary<int, string> TriggerTime = new Dictionary<int, string>();
        public static Dictionary<int, string> DecodeTime = new Dictionary<int, string>();
        public static Dictionary<int, string> Grade = new Dictionary<int, string>();
        public static Dictionary<int, string> symbolContrast = new Dictionary<int, string>();
        public static Dictionary<int, string> printGrowth = new Dictionary<int, string>();
        public static Dictionary<int, string> UEC = new Dictionary<int, string>();
        public static Dictionary<int, string> modulation = new Dictionary<int, string>();
        public static Dictionary<int, string> fixedPatternDamage = new Dictionary<int, string>();
        public static Dictionary<int, string> gridNonUniformity = new Dictionary<int, string>();
        //图片
        public static Dictionary<int, Image> Images = new Dictionary<int, Image>();
        public static bool OkSave = false;//OK图片保存
        public static bool NgSave = false;//NG图片保存

        public static string Result_ReadOK = "0";
        public static bool bReadOK = false;//Barcode 读取完成，通知PLC
    }
    public class Transmission
    {
        public static bool Trigger = false;

        public static string BarcodeResult = "";//讀碼結果
        public static Image BarcodeImage;//Barcode圖片
        public static string TriggerTime = "";
        public static string DecodeTime = "";
        public static string code = "";
        public static bool ChangeUI = false;//是否刷新UI
    }

    public class Glue
    {
        public static int Count_Max = 0;//從PLC獲取點膠最大數量
        public static int Count_Now = 0;//從PLC獲取點膠當前數量
        public static bool Glue_Follow = false;
        public static bool Glue_Circle_2 = false;
        public static int Glue_Circle_OuterRadius_2 = 1;
        public static int Glue_Circle_InnerRadius_2 = 1;
        public static int Glue_Circle_StartAngle_2 = 1;
        public static int Glue_Circle_EndAngle_2 = 1;
        public static int Glue_Circle_Gray_2 = 0;

        public static double GlueAngleRatio = 0;
        public static double GlueAngleRatio_2 = 0;

        public static double ResultGlueArea_CCD1 = 0;
        public static double ResultGlueArea_CCD2 = 0;
        public static double ResultGlueAngle_CCD1 = 0;
        public static double ResultGlueAngle_CCD2 = 0;
        public static double ResultGlueAngle_2_CCD1 = 0;
        public static double ResultGlueAngle_2_CCD2 = 0;

        //膠水檢測內外徑增益
        public static double Offset_OuterRadius = 0;
        public static double Offset_InnerRadius = 0;

        public static bool IsChecked = false;
        public static bool IsKeyEnter = false;
        public static bool WebChecked = false;
        public static bool WeightWebChecked = false;
        public static string Hour = "24";
        public static string Minute = "0";
        public static int glueTime = 0;
        public static string Barcode = "";
        public static double GlueOutArea = 0.0;
        public static double GlueOutArea2 = 0.0;
        public static bool Cir2AVIchecked = false;
        public static double GlueOutAreaMax = 0.0;
        public static string GlueOutResult = "";
        public static double Gxpm = 0.0;
        public static bool WidthIsChecked = false;
        public static bool InOutIsChecked = false;
        public static double WidthMaxSet = 0.0;
        public static double InRMin = 0.0, OutRMax = 0.0;
        public static double ID = 0.0, OD = 0.0, GlueD = 0.0, GlueWidth = 0.0;
        public static string GlueDisR = "", P1GlueDisR = "", P2GlueDisR = "";
        public static string GlueDis1 = "null", GlueDis2 = "null";
        public static string P1GlueDis1 = "null", P1GlueDis2 = "null";
        public static string P2GlueDis1 = "null", P2GlueDis2 = "null";
        public static int GlueUseMins = 0;
        public static bool IsGlueCan = false;
        public static bool IsGlueUntie = false;

        public static  List<string> AllProductPair = new List<string>();
        public static List<string> AllProductPairlist = new List<string>();
        public static string CurProductPair = "";
    }  
    public class Monitor
    {
        public static bool UVModeCheck = false;
        public static bool PCCD2TrayAVI = false;
        public static bool PCCD2Sampling = false;
        public static byte[] GetRead = Encoding.ASCII.GetBytes("01WRDD00200,01\r\n");
        public static bool IsRead = false;
        public static bool[] allstatus = new bool[16];
    }

    public class A1CCD1
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "01WWRD00002,08,0000";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;
        public static string x = "";
        public static string y = "";
    }
    public class A1CCD2
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int IntSingle = 0;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;
        public static bool HCoatCh1 = false;
        public static string x = "";
        public static string y = "";
    }
    public class A2CCD1
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "01WWRD00024,08,0000";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;
        public static string x = "";
        public static string y = "";
    }
    public class A2CCD2
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int IntSingle = 0;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;
        public static bool HCoatCh2 = false;
        public static string x = "";
        public static string y = "";
    }
    public class PCCD1
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "01WWRD00046,08,0000";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;
    }
    public class PCCD2
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int IntSingle = 0;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double ppxm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;
        public static bool isPUAVI = false;
    }
    public class GCCD1
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "01WWRD00076,08,0000";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;

        public struct NeedleTipTest
        {
            public static int ContrastSet;
            public static int NeedleChoice;

            public static HTuple RegionRow;
            public static HTuple RegionColumn;
            public static HTuple RegionPhi;
            public static HTuple RegionLength1;
            public static HTuple RegionLength2;
            public static int Gray;

            public static int Radius;
            public static string Measure_Transition;
            public static string Measure_Select;
            public static int Num_Measures;
            public static int Measure_Length1;
            public static int Measure_Length2;
            public static int Measure_Threshold;

            public static int X_UpperSet;
            public static int X_LowerSet;
            public static int Y_UpperSet;
            public static int Y_LowerSet;
            public static double X_UpperValue;
            public static double X_LowerValue;
            public static double Y_UpperValue;
            public static double Y_LowerValue;

            public static HTuple ResultRow;
            public static HTuple ResultColumn;
        }
    }
    public class GCCD2
    {
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "";
        public static string ExposureTime = "";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int IntSingle = 0;
        public static int IntPosition = 0;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "01WWRD00084,08,0000";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;
        public static bool Deg4Checked = false;
        public static string x = "";
        public static string y = "";
    }
    public class QCCD
    {
        public static int CompareSetPF = 0;

        public static string QTime = "";
        public static int CCDBrand = 0;
        public static bool IsConnected = false;
        public static Color color = Color.Red;
        public static bool IsOpen = false;
        public static bool IsCheck = false;
        public static bool AVI1IsCheck = false;
        public static bool AVI2IsCheck = false;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static string Gain = "0";
        public static string ExposureTime = "35000";
        public static bool Grabimage = false;
        public static bool GrabSignal = true;
        public static int signalNum = 0;
        public static bool SaveOf = false;
        public static bool SaveRf = false;
        public static double angleC = 0;
        public static string sendaddress = "01WWRD00068,08,0000";
        public static double Pxpm = 0.00441, Pypm = 0.00441;
        public static double xpm = 0.00441, ypm = 0.00441;

        //固定环外圍
        public static int dOutRange = 1;
        //固定环內圍
        public static int dInRange = 1;
        //固定環膠水檢測黑/白
        public static bool Detection_Black = true;
        public static bool Detection_White = true;

        public static int dGraythresholdBlack = 1;
        public static int dGraythresholdWhite = 1;
        public static int dUnderSizeArea = 1;

        //固定环膠水角度
        public static int iGlueAngleSet = 1;
        //固定环膠水閥值
        public static int iGlueRatioSet = 1;
        //固定环胶水总角度
        public static int dAngleSet = 1;

        //小台階外圍
        public static int dOutRangePF = 1;
        //小台階內圍
        public static int dInRangePF = 1;
        //小平台方法
        public static bool DetectionPF_Dark2 = true;
        public static bool DetectionPF_Light2 = true;
        public static bool DetectionPF_Black = true;
        public static bool DetectionPF_White = true;

        public static bool ClosingPF2 = false;
        public static bool OpeningPF2 = false;

        public static int dGraythresholdBlackPF = 1;
        public static int dGraythresholdWhitePF = 1;
        public static int dUnderSizeAreaPF = 1;
        
        //小台階方法二差異閥值
        public static int iDynthresholdDarkPF2 = 1;
        public static int iDynthresholdLightPF2 = 1;
        //小台階方法二灰度閥值
        public static int iGraythresholdBlackPF2 = 1;
        public static int iGraythresholdWhitePF2 = 1;
        //小台階方法二連接/斷開/過濾小面積
        public static int iUnderSizeAreaPF2 = 1;
        public static int iCloseWidthPF2 = 1;
        public static int iCloseHeightPF2 = 1;
        public static int iOpenWidthPF2 = 1;
        public static int iOpenHeightPF2 = 1;
        //小台阶膠水角度
        public static int iGlueAngleSetPF = 1;
        //小台阶膠水閥值
        public static int iGlueRatioSetPF = 1;
        //小台阶胶水总角度
        public static int dAngleSetPF = 1;
    }

    public class halcon
    {
        public static HWindow[] HWindowID = new HWindow[9];
        public static HObject[] Image = new HObject[9];
        public static HObject[] ImageOri = new HObject[9];

        public static bool IsPreview = false;
        public static bool AIsChecked = false;
        public static bool IsCrossDraw = false;

        public static HTuple centerRow = new HTuple();
        public static HTuple centerCol = new HTuple();
        public static HTuple centerDeg = new HTuple();

        public static HTuple hv_FirstRow = new HTuple();
        public static HTuple hv_FirstColumn = new HTuple();
        public static HTuple hv_FirstRadius = new HTuple();

        public static HTuple hv_ResultRow = new HTuple();
        public static HTuple hv_ResultColumn = new HTuple();
        public static HTuple hv_ResultRadius = new HTuple();

        public static int CircleRadius = 1;
        public static string CircleMeasureSelect = "last";
        public static string CircleMeasureTransition = "negative";
        public static int CircleMeasureThreshold = 1;
        public static int CircleLength = 1;


    }
    public class GT2
    {
        public static bool IsCheck = false;
        public static bool CH1IsCheck = false;
        public static bool CH2IsCheck = false;
        public static bool CH3IsCheck = false;
        public static bool CH4IsCheck = false;
        public static bool CH5IsCheck = false;
        public static bool IsConnected = false;
        public static bool IsOpen = false;
        public static Color gColor = Color.Red;
        public static Socket socket;
        public static IPAddress ip;
        public static int Port;
        public static bool clearCH = false;
        public static byte[] clearCHcmd;
        public static byte[] clearCH1cmd = Encoding.ASCII.GetBytes("SW,01,001,+000000001\r\n");
        public static byte[] clearCH2cmd = Encoding.ASCII.GetBytes("SW,02,001,+000000001\r\n");
        public static byte[] clearCH3cmd = Encoding.ASCII.GetBytes("SW,03,001,+000000001\r\n");
        public static byte[] clearCH4cmd = Encoding.ASCII.GetBytes("SW,04,001,+000000001\r\n");
        public static byte[] clearCH5cmd = Encoding.ASCII.GetBytes("SW,05,001,+000000001\r\n");
    }
#region WGB增加
    public class Mode5A1CCD2
    {       
        public static HTuple OutRangeRadius { get; set; }   //外圈半径
        public static HTuple InRangeRadius { get; set; }    //内圈半径
        public static HTuple Graythreshold { get; set; }    //灰度阈值
        public static HTuple MeasureThreshold { get; set; } //边缘阈值
        public static int DarkLightGray { get; set; }      //灰度值   0：黑找白，1：白找黑
        public static int DarkLightMeasure { get; set; }   //边缘阈值 0: 黑找白，1：白找黑
        public static HTuple MeasureLength1 { get; set; }  
        public static HTuple MeasureLength2 { get; set; }
        public static int FirstOrLast { get; set; }       //拟合位置 0：First,1:Last
        public static double LimitUp { get; set; }        //上限值
        public static double LimitDown { get; set; }      //下线值
        public static double Offset { get; set; }         //补偿值
        public static double GapOffset { get; set; }         //补偿值
        public static double LimitUpGap { get; set; }
        public static double LimitDownGap { get; set; }
        public static string BarcodeRadius { get; set; }         // 
        public static string GapD1 { get; set; }
        public static string GapD2 { get; set; }
        public static string Result { get; set; }
        public static bool   isMode5 { get; set; }              
        public static HTuple AreaLimitUp { get; set; }          //面积上限
        public static HTuple AreaLimitDown { get; set; }        //面积下限
        public static HTuple WidthLimitUp { get; set; }         //宽度上限
        public static HTuple WidthLimitDown { get; set; }       //宽度下限
        public static HTuple HeightLimitUp { get; set; }        //高度上限
        public static HTuple HeightLimitDown { get; set; }      //高度下限
     
    }
    public class Mode5A2CCD2
    {
        public static HTuple OutRangeRadius { get; set; }
        public static HTuple InRangeRadius { get; set; }
        public static HTuple Graythreshold { get; set; }
        public static HTuple MeasureThreshold { get; set; }
        public static int DarkLightGray { get; set; }      //灰度值   0：黑找白，1：白找黑
        public static int DarkLightMeasure { get; set; }   //边缘阈值 0: 黑找白，1：白找黑
        public static HTuple MeasureLength1 { get; set; }
        public static HTuple MeasureLength2 { get; set; }
        public static int FirstOrLast { get; set; }       //拟合位置 0：First,1:Last
        public static double LimitUp { get; set; }
        public static double LimitDown { get; set; }
        public static double Offset { get; set; }
        public static double GapOffset { get; set; }         //补偿值
        public static double LimitUpGap { get; set; }
        public static double LimitDownGap { get; set; }
        public static string BarcodeRadius { get; set; }         // 
        public static string GapD1 { get; set; }
        public static string GapD2 { get; set; }
        public static string Result { get; set; }
        public static bool  isMode5 { get; set; }
        public static HTuple AreaLimitUp { get; set; }  
        public static HTuple AreaLimitDown { get; set; }
        public static HTuple WidthLimitUp { get; set; }
        public static HTuple WidthLimitDown { get; set; }
        public static HTuple HeightLimitUp { get; set; }
        public static HTuple HeightLimitDown { get; set; } 
    }
    public class Sign
    {
        public static bool isMode5ParaChange { get; set; }
        public static bool isMode5 { get; set; }
    }
#endregion

    public class VisionPara
    {
        public struct _NeedleTip
        {
            //初始針尖檢測座標
            public HTuple RegionRow1;
            public HTuple RegionColumn1;
            public HTuple RegionRow2;
            public HTuple RegionColumn2;
            public int ContrastSet;
            public HTuple Gray;
            //public int TipChoice;
            //結果座標
            public HTuple hv_ResultRow;
            public HTuple hv_ResultColumn;
        }
        public _NeedleTip m_NeedleTip = new _NeedleTip();
    }
}