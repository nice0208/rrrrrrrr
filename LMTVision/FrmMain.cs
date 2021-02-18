using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.IO.Ports;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using PylonC.NET;
using HalconDotNet;
using PylonC.NETSupportLibrary;
using System.Drawing.Imaging;
using System.Diagnostics;
using BarcodeReader;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace LMTVision
{
    public partial class FrmMain : Form
    {
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        FrmDisplay1 Run;
        FrmVisionSet VisionSet;
        FrmSetUp SetUp;
        FrmDisplay2 Run2;
        FrmLogin fb;
        FrmBarcode Fbarcode;
        FrmShowAVI AviShow = new FrmShowAVI();
        FrmChoseTable Fct;

        Web_Tray_InOut.Eqp_LMT m_Web_Tray_InOut = new Web_Tray_InOut.Eqp_LMT();
        public int Production = 0;
        #region //pylon變數     
        private NODE_HANDLE hnote1 = new NODE_HANDLE();
        public NODE_HANDLE hnoteGain1;
        public NODE_HANDLE hnoteExp1;
        private NODE_CALLBACK_HANDLE m_hCallbackHandle1 = new NODE_CALLBACK_HANDLE(); /* The handle of the node callback. */
        private NodeCallbackHandler m_nodeCallbackHandler1 = new NodeCallbackHandler(); /* The callback handler. */
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        public ImageProvider m_imageProvider1 = new ImageProvider(); /* Create one image provider. */
        public ImageProvider m_imageProvider2 = new ImageProvider();
        public ImageProvider m_imageProvider3 = new ImageProvider();
        public ImageProvider m_imageProvider4 = new ImageProvider();
        public ImageProvider m_imageProvider5 = new ImageProvider();
        public ImageProvider m_imageProvider6 = new ImageProvider();
        public ImageProvider m_imageProvider7 = new ImageProvider();
        public ImageProvider m_imageProvider8 = new ImageProvider();
        public ImageProvider m_imageProvider9 = new ImageProvider();
        public ImageProvider m_imageProvider10 = new ImageProvider();
        private Bitmap m_bitmap = null; /* The bitmap is used for displaying the image. */
        public static string[] theCamIp = new string[10] { "A1CCD1", "A1CCD2", "A2CCD1", "A2CCD2", "PCCD1", "PCCD2", "GCCD1", "GCCD2", "QCCD", "BarcodeReader" };
        public static string[] theImageIP = new string[33] { "A1CCD1-1","A1CCD1-2", "A1CCD2-PickUp", "A1CCD2-Platform", "A2CCD1-1","A2CCD1-2","A2CCD2-PickUp", "A2CCD2-Platform", 
                                                              "PCCD1", "PCCD1-Hold","PCCD1-Lens","PCCD2-PickUp", "PCCD2-Platform1","PCCD2-Platform2",  "GCCD1", "GCCD2-1","GCCD2-2", "GCCD2-3","GCCD2-4", "QCCD","A1CCD1-Hold",
                                                              "A1CCD1-Lens","A2CCD1-Hold","A2CCD1-Lens","A1CCD2-Hold","A1CCD2-Lens","A2CCD2-Hold","A2CCD2-Lens","BarcodeReader","GCCD2-1q","GCCD2-2q","GCCD2-3q","GCCD2-4q"};
        public static uint[] theCamIndex = new uint[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        #endregion
        public MyHikvision myHikvision = new MyHikvision();
        //增加代码：
        HDevelopExport HD = new HDevelopExport();
        public class FileTimeInfo
        {
            public string ChFileName;
            public DateTime ChFileCreateTime;
        }
        enum DataType
        {
            Input1 = 0,
            Input2 = 1,
            Input3 = 2,
            Input4 = 3,
            Input5 = 4,
            Input6 = 5,
            Input7 = 6,
            Input8 = 7,
            Input9 = 8,
            Output = 9,
            LogTrigger = 10,
            ErrTrigger = 11,
            linshi = 12,
            readLocation = 13,
            pianbai = 14,
            loop = 15,
            clince = 16,
            readcurLt = 17,
            PlateCount = 18,
            LogMsgSign =19,
            GlueCount = 20,
            PLCHMI_Versions = 21,
            TrayMaxXY = 22,
        }

        class ThreadInfo
        {
            public HObject image { get; set; }
            public HWindow Window { get; set; }
        }

        public FrmMain()
        {
            InitializeComponent();
            #region A1CD1
            m_imageProvider1.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback1);
            m_imageProvider1.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback1);
            m_imageProvider1.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback1);
            m_imageProvider1.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback1);
            m_imageProvider1.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback1);
            m_imageProvider1.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback1);
            m_imageProvider1.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback1);
            UpdateDeviceList1();
            #endregion
            #region A1CD2
            m_imageProvider2.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback2);
            m_imageProvider2.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback2);
            m_imageProvider2.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback2);
            m_imageProvider2.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback2);
            m_imageProvider2.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback2);
            m_imageProvider2.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback2);
            m_imageProvider2.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback2);
            UpdateDeviceList2();
            #endregion
            #region A2CD1
            m_imageProvider3.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback3);
            m_imageProvider3.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback3);
            m_imageProvider3.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback3);
            m_imageProvider3.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback3);
            m_imageProvider3.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback3);
            m_imageProvider3.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback3);
            m_imageProvider3.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback3);
            UpdateDeviceList3();
            #endregion
            #region A2CD2
            m_imageProvider4.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback4);
            m_imageProvider4.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback4);
            m_imageProvider4.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback4);
            m_imageProvider4.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback4);
            m_imageProvider4.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback4);
            m_imageProvider4.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback4);
            m_imageProvider4.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback4);
            UpdateDeviceList4();
            #endregion
            #region PCD1
            m_imageProvider5.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback5);
            m_imageProvider5.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback5);
            m_imageProvider5.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback5);
            m_imageProvider5.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback5);
            m_imageProvider5.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback5);
            m_imageProvider5.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback5);
            m_imageProvider5.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback5);
            UpdateDeviceList5();
            #endregion
            #region PCD2
            m_imageProvider6.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback6);
            m_imageProvider6.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback6);
            m_imageProvider6.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback6);
            m_imageProvider6.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback6);
            m_imageProvider6.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback6);
            m_imageProvider6.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback6);
            m_imageProvider6.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback6);
            UpdateDeviceList6();
            #endregion
            #region GCD1
            m_imageProvider7.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback7);
            m_imageProvider7.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback7);
            m_imageProvider7.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback7);
            m_imageProvider7.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback7);
            m_imageProvider7.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback7);
            m_imageProvider7.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback7);
            m_imageProvider7.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback7);
            UpdateDeviceList7();
            #endregion
            #region GCD2
            m_imageProvider8.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback8);
            m_imageProvider8.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback8);
            m_imageProvider8.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback8);
            m_imageProvider8.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback8);
            m_imageProvider8.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback8);
            m_imageProvider8.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback8);
            m_imageProvider8.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback8);
            UpdateDeviceList8();
            #endregion
            #region QCD
            //if (QCCD.CCDBrand == 0)
            //{
                m_imageProvider9.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback9);
                m_imageProvider9.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback9);
                m_imageProvider9.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback9);
                m_imageProvider9.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback9);
                m_imageProvider9.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback9);
                m_imageProvider9.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback9);
                m_imageProvider9.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback9);
                UpdateDeviceList9();
            //}
            //if (QCCD.CCDBrand == 1)
                myHikvision.InitializeSetting();
            #endregion
            #region BarcodeReader
            Barcode1.bOpen = true;
            m_imageProvider10.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback10);
            m_imageProvider10.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback10);
            m_imageProvider10.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback10);
            m_imageProvider10.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback10);
            m_imageProvider10.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback10);
            m_imageProvider10.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback10);
            m_imageProvider10.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback10);
            UpdateDeviceList10();
            #endregion
        }

        #region Var
        Int64 TimeRunning, oeeTimeRunning, oeeTimeRunning0;
        int Days, Hours, Minutes, Seconds;
        public static int Pnumber = 42;
        public static Int32 MacS = 10, MacSL = 10;
        public static string[] RealHight = new string[5] { "+0.000mm", "+0.000mm", "+0.000mm", "+0.000mm", "+0.000mm" };
        public bool quit = false;
        bool keepReading = true;
        bool Scaning = false, ScanStatus = false;
        public static bool GseoConn = false;
        public static bool sendBar = false;
        public static bool open = false;
        public static bool[] processing = new bool[9];
        public string[] ConStatus = new string[16] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" };
        public static string Barcresult = "";

        public string path = Sys.IniPath + "\\ComPortPara.ini";  //设备各参数路径
        public string setpath = Sys.IniPath + "\\SetParam.ini";  //设备各参数路径
        public string BarPath = Sys.IniPath + "\\BarcodePara.ini"; //扫码信息
        public string SysPath = Sys.IniPath + "\\System.ini";
        public static string propath;
        public static string mtime = ""; int ti = 0; bool cancelTrigger = false;
        string LogDate = "", LogTime = ""; string ImagePath = ""; //string LogSerial = ""; 
        string Pstation = "";//工位1,2
        string gb; //胶水的二维码
        public Log gluelog, weblog;
        public static string weblogfile = "";
        public static string gluelogfile = "";
                              //日期 +生成时间+电源支持天数+电源支持时间+自动运行时间+报警暂停时间+等待时间+报警计数+产品计数+周期时间+机台编号+机种名称+组装高度+  1 + PZ0Tray条码 +  R1盘坐标 + LENS二维码+ AZ0Tray盘二维码+
        string FileHeader = "Date\t" + "CreateTime\t" + "PowerSupplyTime-day\t" + "PowerSupplyTime-time\t" + "AutoOperationTime\t" +
                                "AlarmPauseTime\t" + "WaitTime\t" + "AlarmCounter\t" + "ProductionCounter\t" + "Station\t" + "CycleTime\t" +
                                "MachineId\t" + "ProductName\t" + "Height\t" + "Mode\t" + "R1TrayBarcode\t" + "R1TrayX-Y\t" + "LensBarcode\t" + "R3TrayBarcode\t" +
                                "GlueBarcode\t" + "AVIResult\t" + "SetValue\t" + "MeasuredValue\t" + "ID\t" + "OD\t" + "GlueD\t" + "GlueWidth\t" +
                                "DisResult\t" + "GlueDis1\t" + "GlueDis2\t" + "Result\t" + "DValue\t" + "DiamMin\t" +
                                "AssShiftResult\t" + "AssShiftX\t" + "AssShiftY\t" + "AssShift\t" +
                                "GlueCUTimeCH1(s)\t" + "GlueCUPressureCH1(+Kpa)\t" + "GlueCUPressureCH1(-Kpa)\t" +
                                "GlueCUTimeCH2(s)\t" + "GlueCUPressureCH2(+Kpa)\t" + "GlueCUPressureCH2(-Kpa)\t" +
                                " GlueArea " + "\t" + "GlueAngle_1" + "\t" + "GlueAngle_2" + "\t" + "DCutResult" + "\t" +
                                 "BarcodeGapRadius"+  "\t"+   "GapD1" +  "\t"+"GapD2"+"\t"+"LensBarcodeDecodedAngle"+
                                "\r\n";
        #endregion

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                gBWarm.SendToBack();
                this.SetVisibleCore(false);
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.SetVisibleCore(true);

                Run2 = new FrmDisplay2();
                LoadReportPara();

                ReadPeportPara();
                ReadQccdAVI();
                FactorySelected(Sys.Factory);//廠區設置webservice
                propath = Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini";
                weblogfile = DateTime.Now.ToString("yyyyMMdd") + "_" + Sys.MachineId + "_WebAlarm.txt";
                gluelogfile = DateTime.Now.ToString("yyyyMMdd") + "_" + Sys.MachineId + "_GlueAlarm.txt";
                weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                    weblog.log("Start:Newday");
                gluelog = new Log(Sys.AlarmPath + "\\" + gluelogfile);
                if (!File.Exists(Sys.AlarmPath + "\\" + gluelogfile))
                    gluelog.log("Start:Newday");

                Run = new FrmDisplay1(this);
                Run.Show();

                if (Screen.AllScreens.Count() != 1)
                {
                    Run2.Left = 0;// Screen.PrimaryScreen.Bounds.Width;
                    Run2.Top = Screen.PrimaryScreen.Bounds.Height;// 0;
                    Run2.StartPosition = FormStartPosition.Manual;
                    Run2.Size = new System.Drawing.Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
                }
                Run2.Show();
                timerRunningTime.Enabled = true;

                //commucation with PLC
                PLC.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                PLC.socket.LingerState = new LingerOption(false, 1);
                PLC.socket.ReceiveTimeout = 3000;
                PLC.socket.SendTimeout = 3000;
                Thread connectToPlc = new Thread(ConnectToPlc);
                connectToPlc.IsBackground = true;
                connectToPlc.Start();

                //影像连接
                Thread localconn = new Thread(LocalConnect);
                localconn.IsBackground = true;
                localconn.Start();

                #region 设置LightPort参数1
                LIGHT1.Com.ReceivedBytesThreshold = 1;
                //读取光源所有参数
                byte[] cmd = VarLighter.ReadAllPara();
                //当接收到18个字节才触发DataReceived事件，以防数据被分成两次接收
                LIGHT1.Com.ReceivedBytesThreshold = 18;
                if (LIGHT1.Com.IsOpen)
                    LIGHT1.Com.Write(cmd, 0, cmd.Length);
                //启动辅助线程， 判断是否正常接收到参数
                Thread t1 = new Thread(GetStatus);
                t1.IsBackground = true;
                t1.Start();
                LIGHT1.Com.DataReceived += new SerialDataReceivedEventHandler(LIGHT1Com_DataReceived);
                #endregion
                #region 设置LightPort参数2
                LIGHT2.Com.ReceivedBytesThreshold = 1;
                //读取光源所有参数
                cmd = VarLighter.ReadAllPara();
                //当接收到18个字节才触发DataReceived事件，以防数据被分成两次接收
                LIGHT2.Com.ReceivedBytesThreshold = 18;
                if (LIGHT2.Com.IsOpen)
                    LIGHT2.Com.Write(cmd, 0, cmd.Length);
                //启动辅助线程， 判断是否正常接收到参数
                Thread t2 = new Thread(GetStatus2);
                t2.IsBackground = true;
                t2.Start();
                LIGHT2.Com.DataReceived += new SerialDataReceivedEventHandler(LIGHT2Com_DataReceived);
                #endregion
                #region 
                if (LIGHT3.IsChecked)
                {
                    LIGHT3.Com.ReceivedBytesThreshold = 1;
                    //读取光源所有参数
                    cmd = VarLighter.ReadAllPara();
                    //当接收到18个字节才触发DataReceived事件，以防数据被分成两次接收
                    LIGHT3.Com.ReceivedBytesThreshold = 18;
                    if (LIGHT3.Com.IsOpen)
                        LIGHT3.Com.Write(cmd, 0, cmd.Length);
                    //启动辅助线程， 判断是否正常接收到参数
                    Thread t3 = new Thread(GetStatus);
                    t3.IsBackground = true;
                    t3.Start();
                    LIGHT3.Com.DataReceived += new SerialDataReceivedEventHandler(LIGHT3Com_DataReceived);
                }
                #endregion

                //DL-EN1(GT2)
                if (GT2.IsCheck)
                {
                    Thread connectToGT2 = new Thread(ConnectToGT2);
                    connectToGT2.IsBackground = true;
                    connectToGT2.Start();
                    DLEN1loop.RunWorkerAsync();
                }
                if (GlueCU.IsCheck & GlueCU.IsConnected)
                    bWGlueCU.RunWorkerAsync();

                if (Reader.IsChecked)
                    LTrayData.RunWorkerAsync();
                if (RiReader.IsChecked)
                    RTrayData.RunWorkerAsync();
                CommucationPlc.RunWorkerAsync();
                OfflineReConncet.RunWorkerAsync();
                if (Glue.IsChecked)
                    bWtimerGlue.RunWorkerAsync();
                //bWOEE.RunWorkerAsync();
                timerRefleshUI.Enabled = true;
                //Try Discover DataMan System
                ScanTrigger.RunWorkerAsync();
                TraversandChangeName();

                string lasttime = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location).ToString("yyyyMMdd");
                //VER.Text = "V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "_" +
                //           lasttime;

                if (gb != "" & Glue.IsChecked)
                {
                    #region jiaoshui 
                    Glue.Barcode = gb;
                    FrmDisplay1.replaceTime = Convert.ToDateTime(iniFile.Read("GlueBarcodePara", "GlueReplacementTime", path));
                    FrmDisplay1.GBtimeN = DateTime.Now;
                    System.TimeSpan t = FrmDisplay1.GBtimeN - FrmDisplay1.replaceTime;
                    if (t.TotalMinutes >= Glue.glueTime)
                    {
                        Protocol.strPCRead_GlueTimeOut = true;   //影像程式开启时判断胶水时间超时
                        Protocol.IsPCRead = true;
                        MessageBox.Show("胶水已超过规定使用时间，请更换胶水后点击确认！", "", MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Warning);
                        gluelog.log("Glue报错:TimeOut(Load)");
                    }
                    else
                    {
                        string gut = iniFile.Read("GlueBarcodePara", "GlueCanUseMins", path);
                        Glue.GlueUseMins = (gut != "" ? int.Parse(gut) : 0);
                        if (Glue.GlueUseMins != 0 & Glue.WebChecked)
                        {
                            Sys.TimerGlue = true;
                            Run.txtGluePiao.Enabled = false;
                        }
                        else
                        {
                            Sys.TimerOtherGlue = true;
                        }
                    }
                    #endregion
                }
                if (PLC.IsConnected)
                {
                    #region OEE
                    string onum = iniFile.Read("OEE", "ErrNum", SysPath);
                    string Ls = iniFile.Read("OEE", "LastStatus", SysPath);
                    if (Ls == "Idle")
                        IsIdle = true;
                    if (Ls == "Run")
                        Isrun = true;
                    if (Ls != "" & onum == "" & Ls != "Run")
                    {
                        DateTime LastT = Convert.ToDateTime(iniFile.Read("OEE", "LastTime", SysPath));
                        DateTime CurT = DateTime.Now;
                        TimeSpan ts = CurT - LastT;
                        if (ts.TotalMinutes >= 1)
                        {
                            Fct = new FrmChoseTable();
                            Fct.Left = 0;// Screen.PrimaryScreen.Bounds.Width;
                            Fct.Top =  0;
                            Fct.StartPosition = FormStartPosition.Manual;
                            Fct.Size = new System.Drawing.Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
                            DialogResult result = Fct.ShowDialog();
                        }
                        else
                        {
                            Sys.TimerOEE = true;
                        }
                    }
                    #endregion
                }
                bpfWebMessage.TableName = "arrLensBarcode";
                bpfWebMessage.Columns.Add("LensBarcode");
                bpfWebMessage.Columns.Add("X");
                bpfWebMessage.Columns.Add("Y");
                for (int i = 0; i < 30; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        strRlt[i, j] = iniFile.Read("DResult", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");
                        strD1[i, j] = iniFile.Read("DValue", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");
                        strDmin[i, j] = iniFile.Read("DminValue", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");
                        strresult[i, j] = iniFile.Read("DResult", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");
                        strDistance[i, j] = iniFile.Read("Ddistance", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");
                        strDistance1[i, j] = iniFile.Read("Ddistance1", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");
                        strDistance2[i, j] = iniFile.Read("Ddistance2", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");                 
                        string rdbar = iniFile.Read("DBarcode", i.ToString() + "_" + j.ToString(), Sys.IniPath + "\\Data.ini");
                        if (rdbar != "")
                            bpfWebMessage.Rows.Add(rdbar, i, j);
                    }
                }
                Sys.P1Result = iniFile.Read("AssShift", "Result", Sys.IniPath + "\\Data.ini");
                Sys.AssDisX = iniFile.Read("AssShift", "X", Sys.IniPath + "\\Data.ini");
                Sys.AssDisY = iniFile.Read("AssShift", "Y", Sys.IniPath + "\\Data.ini");
                Sys.AssDis = iniFile.Read("AssShift", "Dis", Sys.IniPath + "\\Data.ini");
                timerAlatm.Enabled = true;

            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
            lblDeleShow.SendToBack();
            lblLoading.SendToBack();
            
        }
        void LoadReportPara()
        {
            if (!Directory.Exists(Sys.IniPath)) //创建ini文档H
                Directory.CreateDirectory(Sys.IniPath);
            if (!Directory.Exists(Sys.AlarmPath)) //创建报警信息文档
                Directory.CreateDirectory(Sys.AlarmPath);
            if (!Directory.Exists(Sys.ReportLog)) //创建Log文档
                Directory.CreateDirectory(Sys.ReportLog);
            if (!Directory.Exists(Sys.ReportLog1)) //创建Log文档
                Directory.CreateDirectory(Sys.ReportLog1);
            if (!Directory.Exists(Sys.ReportImage)) //创建Image文档
                Directory.CreateDirectory(Sys.ReportImage);
        }
        void ReadPeportPara()
        {
            #region MachineID
            Sys.MachineId = iniFile.Read("System", "MachineId", path);
            Sys.MachineId = Dns.GetHostName();
            string fac = iniFile.Read("System", "FactoryChose", path);
            switch (fac)
            {
                case "0": Sys.Factory = "XM"; break;
                case "1": Sys.Factory = "JM"; break;
                case "2": Sys.Factory = "TD"; break;
            }
            //Productions
            Sys.CurrentProduction = iniFile.Read("CURRENTPRODUCENAME", "ProductName", Sys.IniPath + "\\Products.ini");
            propath = Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini";
            string pidname = iniFile.Read("CURRENTPRODUCEID", "ProductID", Sys.IniPath + "\\Products.ini");
            Sys.CurrentProID = pidname;
            string tnum = iniFile.Read("CURRENTTRAY1NUM", "Tray1Num", Sys.IniPath + "\\Products.ini");
            Pnumber = (IsNumber(tnum)) ? int.Parse(tnum) : 42;
            string[] MNamebar = new string[10] { "", "", "", "", "", "", "", "", "", "" };
            for (int i = 0; i < 10; i++)
            {
                MNamebar[i] = iniFile.Read("PRODUCEID", "List&Name" + (i + 1).ToString(), Sys.IniPath + "\\Products.ini");
                MNamebar[i] = (MNamebar[i].Length == 9) ? MNamebar[i].Substring(0, 6) : "";
                if (MNamebar[i] != "")
                {
                    if (MNamebar[i] == pidname)
                    {
                        string numBar = iniFile.Read("PRODUCEID", "List&Name" + (i + 1).ToString(), Sys.IniPath + "\\Products.ini");
                        Sys.CurrentBarID = (numBar.Length == 9) ? numBar.Substring(numBar.Length - 2, 2) : "";
                    }
                }
            }
            string checkpid = iniFile.Read("CURRENTPRODUCEID", "Checked", path);
            switch (checkpid)
            {
                case "YES": Sys.CurProduceIDCheck = true; break;
                case "NO": Sys.CurProduceIDCheck = false; Sys.CurrentProID = ""; break;
            }
            Int16 CodesIndex = Convert.ToInt16(IniFile.Read("Addition", "Codes", "0", path));
            switch (CodesIndex)
            {
                case 0: Sys.Codes = "M"; break;
                case 1: Sys.Codes = "F"; break;
            }
            #endregion
            #region Other
            string css = iniFile.Read("CROSSSHOWSET", "IsChecked", setpath);
            if (css != "" && css == "True")
                halcon.IsCrossDraw = true;
            #endregion
            #region LIGHT1
            string port1 = iniFile.Read("btnLig1", "Port", path);
            int Port1 = 0;
            if (Int32.TryParse(port1, out Port1))
                LIGHT1.Com.PortName = "COM" + Port1;
            string baudrate1 = iniFile.Read("btnLig1", "Baudrate", path);
            int Baudrate1 = 0;
            if (Int32.TryParse(baudrate1, out Baudrate1))
                LIGHT1.Com.BaudRate = Baudrate1;
            string dataBit1 = iniFile.Read("btnLig1", "DataBit", path);
            int DataBit1 = 0;
            if (Int32.TryParse(dataBit1, out DataBit1))
                LIGHT1.Com.DataBits = DataBit1;
            string parity1 = iniFile.Read("btnLig1", "Parity", path);
            switch (parity1)
            {
                case "Even": LIGHT1.Com.Parity = Parity.Even; break;
                case "Odd": LIGHT1.Com.Parity = Parity.Odd; break;
                case "None": LIGHT1.Com.Parity = Parity.None; break;
            }
            string stopBit1 = iniFile.Read("btnLig1", "StopBit", path);
            switch (stopBit1)
            {
                case "1": LIGHT1.Com.StopBits = StopBits.One; break;
                case "2": LIGHT1.Com.StopBits = StopBits.Two; break;
            }
            if (LIGHT1.Com.IsOpen)
            {
                MessageBox.Show("左侧测压串口被占用," + LIGHT1.Com.PortName);
            }
            else
            {
                try
                {
                    LIGHT1.Com.Open();
                    LIGHT1.Com.DiscardInBuffer();
                    LIGHT1.Com.DiscardOutBuffer();
                }
                catch (Exception ex)
                {
                    string exm = ex.ToString().Substring(0, ex.ToString().IndexOf("。"));
                    MessageBox.Show(exm + "。");
                }
            }
            #endregion
            #region LIGHT2
            string port2 = iniFile.Read("btnLig2", "Port", path);
            int Port2 = 0;
            if (Int32.TryParse(port2, out Port2))
                LIGHT2.Com.PortName = "COM" + Port2;
            string baudrate2 = iniFile.Read("btnLig2", "Baudrate", path);
            int Baudrate2 = 0;
            if (Int32.TryParse(baudrate2, out Baudrate2))
                LIGHT2.Com.BaudRate = Baudrate2;
            string dataBit2 = iniFile.Read("btnLig2", "DataBit", path);
            int DataBit2 = 0;
            if (Int32.TryParse(dataBit2, out DataBit2))
                LIGHT2.Com.DataBits = DataBit2;
            string parity2 = iniFile.Read("btnLig2", "Parity", path);
            switch (parity2)
            {
                case "Even": LIGHT2.Com.Parity = Parity.Even; break;
                case "Odd": LIGHT2.Com.Parity = Parity.Odd; break;
                case "None": LIGHT2.Com.Parity = Parity.None; break;
            }
            string stopBit2 = iniFile.Read("btnLig2", "StopBit", path);
            switch (stopBit2)
            {
                case "1": LIGHT2.Com.StopBits = StopBits.One; break;
                case "2": LIGHT2.Com.StopBits = StopBits.Two; break;
            }
            if (LIGHT2.Com.IsOpen)
            {
                MessageBox.Show("左侧测压串口被占用," + LIGHT2.Com.PortName);
            }
            else
            {
                try
                {
                    LIGHT2.Com.Open();
                    LIGHT2.Com.DiscardInBuffer();
                    LIGHT2.Com.DiscardOutBuffer();
                }
                catch (Exception ex)
                {
                    string exm = ex.ToString().Substring(0, ex.ToString().IndexOf("。"));
                    MessageBox.Show(exm + "。");
                }
            }
            #endregion
            #region Lift Reader
            string port3 = iniFile.Read("Reader2", "Port", path);
            int Port3 = 0;
            if (Int32.TryParse(port3, out Port3))
                Reader.Com.PortName = "COM" + port3;
            string baudrate3 = iniFile.Read("Reader2", "Baudrate", path);
            int Baudrate3 = 0;
            if (Int32.TryParse(baudrate3, out Baudrate3))
                Reader.Com.BaudRate = Baudrate3;
            string dataBit3 = iniFile.Read("Reader2", "DataBit", path);
            int DataBit3 = 0;
            if (Int32.TryParse(dataBit3, out DataBit3))
                Reader.Com.DataBits = DataBit3;
            string parity3 = iniFile.Read("Reader2", "Parity", path);
            switch (parity3)
            {
                case "Even": Reader.Com.Parity = Parity.Even; break;
                case "Odd": Reader.Com.Parity = Parity.Odd; break;
                case "None": Reader.Com.Parity = Parity.None; break;
            }
            string stopBit3 = iniFile.Read("Reader2", "StopBit", path);
            switch (stopBit3)
            {
                case "1": Reader.Com.StopBits = StopBits.One; break;
                case "2": Reader.Com.StopBits = StopBits.Two; break;
            }
            string option1 = iniFile.Read("Reader2", "Tray2Checked", path);
            Reader.IsChecked = (option1 == "True" ? true : false);
            if (Reader.IsChecked)
            {
                if (Reader.Com.IsOpen)
                {
                    Reader.IsConnected = false;
                    MessageBox.Show("Tray2Reader串口被占用," + Reader.Com.PortName);
                }
                else
                {
                    try
                    {
                        Reader.Com.Open();
                        Reader.Com.DiscardInBuffer();
                        Reader.Com.DiscardOutBuffer();
                        Reader.IsConnected = true;
                    }
                    catch (Exception ex)
                    {
                        Reader.IsConnected = false;
                        string exm = ex.ToString().Substring(0, ex.ToString().IndexOf("。"));
                        MessageBox.Show(exm + "。");
                    }
                }
            }
            RiReader.Barcode_In = Reader.Barcode = iniFile.Read("CurrentMessage", "Tray2Barcode", propath);
            if (RiReader.Web_Tray_InOutStation && RiReader.Web_Tray2InTray1Out_InOutStation)
            {
                RiReader.Barcode_In = Reader.Barcode;
            }
            if (!Reader.IsChecked)
                Reader.Barcode = "NA";
            #endregion
            #region Right Reader
            string port4 = iniFile.Read("Reader1", "Port", path);
            int Port4 = 0;
            if (Int32.TryParse(port4, out Port4))
                RiReader.Com.PortName = "COM" + port4;
            string baudrate4 = iniFile.Read("Reader1", "Baudrate", path);
            int Baudrate4 = 0;
            if (Int32.TryParse(baudrate4, out Baudrate4))
                RiReader.Com.BaudRate = Baudrate4;
            string dataBit4 = iniFile.Read("Reader1", "DataBit", path);
            int DataBit4 = 0;
            if (Int32.TryParse(dataBit4, out DataBit4))
                RiReader.Com.DataBits = DataBit4;
            string parity4 = iniFile.Read("Reader1", "Parity", path);
            switch (parity4)
            {
                case "Even": RiReader.Com.Parity = Parity.Even; break;
                case "Odd": RiReader.Com.Parity = Parity.Odd; break;
                case "None": RiReader.Com.Parity = Parity.None; break;
            }
            string stopBit4 = iniFile.Read("Reader1", "StopBit", path);
            switch (stopBit4)
            {
                case "1": RiReader.Com.StopBits = StopBits.One; break;
                case "2": RiReader.Com.StopBits = StopBits.Two; break;
            }
            string option2 = iniFile.Read("Reader1", "Tray1Checked", path);
            RiReader.IsChecked = (option2 == "True" ? true : false);
            option2 = iniFile.Read("WebServer", "Checked", path);
            RiReader.WebChecked = (option2 == "True" ? true : false);
            option2 = iniFile.Read("BPFWebServer", "Checked", path);
            RiReader.BPFWebChecked = (option2 == "True" ? true : false);
            RiReader.Web_Tray_InOutStation  = bool.Parse(IniFile.Read("Web_PZ0_InOutStation", "Checked","false", path));//膠水進出站
            RiReader.Web_Tray2InTray1Out_InOutStation = bool.Parse(IniFile.Read("Web_AZ0InPZ0Out_InOutStation", "Checked", "false", path));//膠水進出站-AZ0進PZ0出
            RiReader.Web_Tray_InOutStation_ErrorIgnore = bool.Parse(IniFile.Read("Web_PZ0_InOutStation_ErrorIgnore", "Checked", "false", path));//膠水進出站-NG不停機        
            if (RiReader.IsChecked)
            {
                if (RiReader.Com.IsOpen)
                {
                    RiReader.IsConnected = false;
                    MessageBox.Show("Tray1Reader串口被占用," + RiReader.Com.PortName);
                }
                else
                {
                    try
                    {
                        RiReader.Com.Open();
                        RiReader.Com.DiscardInBuffer();
                        RiReader.Com.DiscardOutBuffer();
                        RiReader.IsConnected = true;
                    }
                    catch (Exception ex)
                    {
                        RiReader.IsConnected = false;
                        string exm = ex.ToString().Substring(0, ex.ToString().IndexOf("。"));
                        MessageBox.Show(exm + "。");
                    }
                }
            }
            RiReader.Barcode = iniFile.Read("CurrentMessage", "Tray1Barcode", propath);
            if (RiReader.Web_Tray_InOutStation && !RiReader.Web_Tray2InTray1Out_InOutStation)
            {
                RiReader.Barcode_Out = RiReader.Barcode_In = RiReader.Barcode;
            }
            else if (RiReader.Web_Tray_InOutStation && RiReader.Web_Tray2InTray1Out_InOutStation)
            {
                RiReader.Barcode_Out = RiReader.Barcode; ;
            }
            #endregion
            #region GlueCU
            string glueCUch = iniFile.Read("GlueCU", "Choice", path);
            GlueCU.choice = int.Parse(glueCUch == "1" ? "1" : "0");
            string port5 = iniFile.Read("GlueCU", "Port", path);
            int Port5 = 0;
            if (Int32.TryParse(port5, out Port5))
                GlueCU.Com.PortName = "COM" + Port5;
            string baudrate5 = iniFile.Read("GlueCU", "Baudrate", path);
            int Baudrate5 = 0;
            if (Int32.TryParse(baudrate5, out Baudrate5))
                GlueCU.Com.BaudRate = Baudrate5;
            string dataBit5 = iniFile.Read("GlueCU", "DataBit", path);
            int DataBit5 = 0;
            if (Int32.TryParse(dataBit5, out DataBit5))
                GlueCU.Com.DataBits = DataBit5;
            string parity5 = iniFile.Read("GlueCU", "Parity", path);
            switch (parity5)
            {
                case "Even": GlueCU.Com.Parity = Parity.Even; break;
                case "Odd": GlueCU.Com.Parity = Parity.Odd; break;
                case "None": GlueCU.Com.Parity = Parity.None; break;
            }
            string stopBit5 = iniFile.Read("GlueCU", "StopBit", path);
            switch (stopBit5)
            {
                case "1": GlueCU.Com.StopBits = StopBits.One; break;
                case "2": GlueCU.Com.StopBits = StopBits.Two; break;
            }
            switch (iniFile.Read("GlueCU", "Checked", path))
            {
                case "YES": GlueCU.IsCheck = true; break;
                case "NO": GlueCU.IsCheck = false; break;
            }
            string chn = iniFile.Read("GlueCU", "ChanleNumber", path);
            GlueCU.ChanleNumber = (chn != "" ? int.Parse(chn) : 10);
            if (!GlueCU.IsCheck)
            {
                GlueCU.ChanleNumber = 0;
            }
            else
            {
                if (GlueCU.Com.IsOpen)
                {
                    GlueCU.IsConnected = false;
                    MessageBox.Show("点胶控制器串口被占用," + GlueCU.Com.PortName);
                }
                else
                {
                    try
                    {
                        GlueCU.Com.Open();
                        GlueCU.Com.DiscardInBuffer();
                        GlueCU.Com.DiscardOutBuffer();
                        GlueCU.IsConnected = true;
                    }
                    catch (Exception ex)
                    {
                        GlueCU.IsConnected = false;
                        string exm = ex.ToString().Substring(0, ex.ToString().IndexOf("。"));
                        MessageBox.Show(exm + "。");
                    }
                }
            }
            #endregion
            #region LIGHT3
            port2 = iniFile.Read("btnLig3", "Port", path);
            Port2 = 0;
            if (Int32.TryParse(port2, out Port2))
                LIGHT3.Com.PortName = "COM" + Port2;
            baudrate2 = iniFile.Read("btnLig3", "Baudrate", path);
            Baudrate2 = 0;
            if (Int32.TryParse(baudrate2, out Baudrate2))
                LIGHT3.Com.BaudRate = Baudrate2;
            dataBit2 = iniFile.Read("btnLig3", "DataBit", path);
            DataBit2 = 0;
            if (Int32.TryParse(dataBit2, out DataBit2))
                LIGHT3.Com.DataBits = DataBit2;
            switch (iniFile.Read("btnLig3", "Parity", path))
            {
                case "Even": LIGHT3.Com.Parity = Parity.Even; break;
                case "Odd": LIGHT3.Com.Parity = Parity.Odd; break;
                case "None": LIGHT3.Com.Parity = Parity.None; break;
            }
            switch (iniFile.Read("btnLig3", "StopBit", path))
            {
                case "1": LIGHT3.Com.StopBits = StopBits.One; break;
                case "2": LIGHT3.Com.StopBits = StopBits.Two; break;
            }
            switch (iniFile.Read("btnLig3", "Checked", path))
            {
                case "YES": LIGHT3.IsChecked = true; break;
                case "NO": LIGHT3.IsChecked = false; break;
            }
            if (LIGHT3.IsChecked)
            {
                if (LIGHT3.Com.IsOpen)
                {
                    MessageBox.Show("左侧测压串口被占用," + LIGHT3.Com.PortName);
                }
                else
                {
                    try
                    {
                        LIGHT3.Com.Open();
                        LIGHT3.Com.DiscardInBuffer();
                        LIGHT3.Com.DiscardOutBuffer();
                    }
                    catch (Exception ex)
                    {
                        string exm = ex.ToString().Substring(0, ex.ToString().IndexOf("。"));
                        MessageBox.Show(exm + "。");
                    }
                }
            }
            #endregion
            #region File
            LogDate = iniFile.Read("LOGMESSAGE", "Logdate", setpath);
            LogTime = iniFile.Read("LOGMESSAGE", "LogdTime", setpath);
            //LogSerial = iniFile.Read("LOGMESSAGE", "LogSerial", setpath);
            if (LogDate == "")
            {
                LogDate = DateTime.Now.ToString("yyyy-MM-dd");
                iniFile.Write("LOGMESSAGE", "Logdate", LogDate, setpath);
                LogTime = DateTime.Now.ToString("yyyyMMdd");
                iniFile.Write("LOGMESSAGE", "LogdTime", LogTime, setpath);
            }
            if (RiReader.IsChecked && RiReader.Barcode != "")
            {
                //if (LogSerial == "")
                //{
                //    LogSerial = DateTime.Now.ToString("HHmmss");
                //    iniFile.Write("LOGMESSAGE", "LogSerial", LogSerial, setpath);
                //}
                string logpath = Sys.ReportLog1 + "\\" + LogDate;
                if (!Directory.Exists(logpath))
                    Directory.CreateDirectory(logpath);
                string logfile = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
                if (!File.Exists(logpath + "\\" + logfile))
                {
                    File.WriteAllText(logpath + "\\" + logfile, FileHeader);
                }
                StreamReader sr = new StreamReader(logpath + "\\" + logfile, System.Text.Encoding.Default);
                int i_p = 0;
                while (sr.ReadLine() != null)
                {
                    i_p++;
                }
                sr.Close();
                ProductCount = i_p - 1;
            }
            #endregion
            #region Barcode
            IPAddress ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("BarcodeReader", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.1.1");
            Barcode1.ip = ip;
            string option3 = iniFile.Read("BarcodeReader", "IsChecked", path);
            Barcode1.IsChecked = (option3 == "True" ? true : false);
            option3 = iniFile.Read("BarcodeReader", "OkSave", path);
            Barcode1.OkSave = (option3 == "True" ? true : false);
            option3 = iniFile.Read("BarcodeReader", "NgSave", path);
            Barcode1.NgSave = (option3 == "True" ? true : false);
            option3 = iniFile.Read("BarcodeChoose", "QCCDchecked", BarPath);
            Barcode1.QCCDisChecked = (option3 == "True" ? true : false);
            string ccdName = (Barcode1.QCCDisChecked ? "QCCD" : "BarcodeReader");
            Barcode1.Gain = iniFile.Read(ccdName, "Gain", BarPath);
            if (Barcode1.Gain == "")
                Barcode1.Gain = "136";
            Barcode1.ExposureTime = iniFile.Read(ccdName, "ExposureTime", BarPath);
            if (Barcode1.ExposureTime == "")
                Barcode1.ExposureTime = "5000";
            #endregion
            #region  Glue
            option2 = iniFile.Read("GlueWebServer", "Checked", path);
            Glue.WebChecked = (option2 == "True" ? true : false);
            option2 = iniFile.Read("GlueWeightWebServer", "Checked", path);
            Glue.WeightWebChecked = (option2 == "True" ? true : false);
            string option6 = iniFile.Read("GlueBarcodePara", "IsChecked", path);
            Glue.IsChecked = (option6 == "True" ? true : false);
            string H = iniFile.Read("GlueBarcodePara", "GlueTimeH", path);
            if (H != "")
                Glue.Hour = H;
            string M = iniFile.Read("GlueBarcodePara", "GlueTimeM", path);
            if (M != "")
                Glue.Minute = M;
            Glue.glueTime = int.Parse(Glue.Hour) * 60 + int.Parse(Glue.Minute);
            gb = iniFile.Read("GlueBarcodePara", "GlueBarcode", path);
            if (gb != "")
                Glue.Barcode = gb;
            option6 = iniFile.Read("GlueBarcodePara", "IsKeyEnter", path);
            Glue.IsKeyEnter = (option6 == "True" ? true : false);
            string all = "";
            if (Sys.Factory == "XM")
            {
                all = (string)iniFile.Read("CodeNumber", "XMUntieTotal", Sys.IniPath + "\\SetParam.ini");
                if (all == "")
                {
                    all = "Q-3494,Y-7998,U-4532,YA-0046,Y-3117,R-2275,Y-6824,YA-1112,W-0616,U-2650,X-1713,C-0188,YA-1284,O-3464,Y-5662,Y-8021,Y-5232,Y-3853,W-1873,U-2615,R-1157";
                    iniFile.Write("CodeNumber", "XMUntieTotal", all, Sys.IniPath + "\\SetParam.ini");
                }
            }
            if (Sys.Factory == "JM" || Sys.Factory == "TD")
            {
                all = (string)iniFile.Read("CodeNumber", "JMUntieTotal", Sys.IniPath + "\\SetParam.ini");
                if (all == "")
                {
                    all = "Y-1619,R-1478,U-3483,YA-0711";
                    iniFile.Write("CodeNumber", "JMUntieTotal", all, Sys.IniPath + "\\SetParam.ini");
                }
            }
            string[] products = all.Split(',');
            for (int i = 0; i < products.Length; ++i)
            {
                string product = products[i].Trim();
                if (product != "")
                {
                    Glue.AllProductPair.Add(product);
                    Glue.AllProductPairlist.Add((i + 1).ToString() + "." + product);
                }
            }
            #endregion
            #region Monitor
            string option7 = iniFile.Read("Monitor", "UVModeChecked", path);
            Monitor.UVModeCheck = (option7 == "True" ? true : false);
            option7 = iniFile.Read("Monitor", "PCCD2TrayAVIChecked", path);
            Monitor.PCCD2TrayAVI = (option7 == "True" ? true : false);
            option7 = iniFile.Read("Monitor", "PCCD2Sampling", path);
            Monitor.PCCD2Sampling = (option7 == "True" ? true : false);
            #endregion

            #region A1CCD1
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("A1_CCD1", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.1");
            A1CCD1.ip = ip;
            int port = 0;
            if (!int.TryParse(iniFile.Read("A1_CCD1", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            A1CCD1.Port = port;
            string check1 = iniFile.Read("A1_CCD1", "Checked", path);
            switch (check1)
            {
                case "YES": A1CCD1.IsCheck = true; break;
                case "NO": A1CCD1.IsCheck = false; break;
            }
            string Sof = iniFile.Read("A1CCD1", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A1CCD1.SaveOf = ((Sof == "True") ? true : false);
            string Srf = iniFile.Read("A1CCD1", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A1CCD1.SaveRf = ((Srf == "True") ? true : false);
            string ta = iniFile.Read("A1CCD1", "defiexuion_angle", setpath);
            if (ta != "")
                A1CCD1.angleC = double.Parse(ta);
            string px = iniFile.Read("A1CCD1", "Pixel_X", setpath);
            if (px != "")
            {
                A1CCD1.xpm = double.Parse(px);
                A1CCD1.ypm = double.Parse(iniFile.Read("A1CCD1", "Pixel_Y", setpath));
            }
            //px = iniFile.Read("A1CCD1", "AssemblyPixel_X", setpath);
            //if (px != "")
            //{
            //    A1CCD1.xpm = double.Parse(px);
            //    A1CCD1.ypm = double.Parse(iniFile.Read("A1CCD1", "AssemblyPixel_Y", setpath));
            //}
            A1CCD1.Gain = iniFile.Read("A1CCD1", "Gain", setpath);
            if (A1CCD1.Gain == "")
                A1CCD1.Gain = "0";
            A1CCD1.ExposureTime = iniFile.Read("A1CCD1", "ExposureTime", setpath);
            if (A1CCD1.ExposureTime == "")
                A1CCD1.ExposureTime = "35000";
            #endregion
            #region A1CCD2
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("A1_CCD2", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.2");
            A1CCD2.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("A1_CCD2", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            A1CCD2.Port = port;
            check1 = iniFile.Read("A1_CCD2", "Checked", path);
            switch (check1)
            {
                case "YES": A1CCD2.IsCheck = true; break;
                case "NO": A1CCD2.IsCheck = false; break;
            }
            Sof = iniFile.Read("A1CCD2", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A1CCD2.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("A1CCD2", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A1CCD2.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("A1CCD2", "defiexuion_angle", setpath);
            if (ta != "")
                A1CCD2.angleC = double.Parse(ta);
            px = iniFile.Read("A1CCD2", "Pixel_X", setpath);
            if (px != "")
            {
                A1CCD2.xpm = double.Parse(px);
                A1CCD2.ypm = double.Parse(iniFile.Read("A1CCD2", "Pixel_Y", setpath));
            }
            //px = iniFile.Read("A1CCD2", "AssemblyPixel_X", setpath);
            //if (px != "")
            //{
            //    A1CCD2.xpm = double.Parse(px);
            //    A1CCD2.ypm = double.Parse(iniFile.Read("A1CCD2", "AssemblyPixel_Y", setpath));
            //}
            A1CCD2.Gain = iniFile.Read("A1CCD2", "Gain", setpath);
            if (A1CCD2.Gain == "")
                A1CCD2.Gain = "0";
            A1CCD2.ExposureTime = iniFile.Read("A1CCD2", "ExposureTime", setpath);
            if (A1CCD2.ExposureTime == "")
                A1CCD2.ExposureTime = "35000";
            #endregion
            #region A2CCD1
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("A2_CCD1", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.3");
            A2CCD1.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("A2_CCD1", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            A2CCD1.Port = port;
            check1 = iniFile.Read("A2_CCD1", "Checked", path);
            switch (check1)
            {
                case "YES": A2CCD1.IsCheck = true; break;
                case "NO": A2CCD1.IsCheck = false; break;
            }
            Sof = iniFile.Read("A2CCD1", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A2CCD1.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("A2CCD1", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A2CCD1.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("A2CCD1", "defiexuion_angle", setpath);
            if (ta != "")
                A2CCD1.angleC = double.Parse(ta);
            px = iniFile.Read("A2CCD1", "Pixel_X", setpath);
            if (px != "")
            {
                A2CCD1.xpm = double.Parse(px);
                A2CCD1.ypm = double.Parse(iniFile.Read("A2CCD1", "Pixel_Y", setpath));
            }
            //px = iniFile.Read("A2CCD1", "AssemblyPixel_X", setpath);
            //if (px != "")
            //{
            //    A2CCD1.xpm = double.Parse(px);
            //    A2CCD1.ypm = double.Parse(iniFile.Read("A2CCD1", "AssemblyPixel_Y", setpath));
            //}
            A2CCD1.Gain = iniFile.Read("A2CCD1", "Gain", setpath);
            if (A2CCD1.Gain == "")
                A2CCD1.Gain = "0";
            A2CCD1.ExposureTime = iniFile.Read("A2CCD1", "ExposureTime", setpath);
            if (A2CCD1.ExposureTime == "")
                A2CCD1.ExposureTime = "35000";
            #endregion
            #region A2CCD2
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("A2_CCD2", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.4");
            A2CCD2.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("A2_CCD2", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            A2CCD2.Port = port;
            check1 = iniFile.Read("A2_CCD2", "Checked", path);
            switch (check1)
            {
                case "YES": A2CCD2.IsCheck = true; break;
                case "NO": A2CCD2.IsCheck = false; break;
            }
            Sof = iniFile.Read("A2CCD2", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A2CCD2.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("A2CCD2", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            A2CCD2.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("A2CCD2", "defiexuion_angle", setpath);
            if (ta != "")
                A2CCD2.angleC = double.Parse(ta);
            px = iniFile.Read("A2CCD2", "Pixel_X", setpath);
            if (px != "")
            {
                A2CCD2.xpm = double.Parse(px);
                A2CCD2.ypm = double.Parse(iniFile.Read("A2CCD2", "Pixel_Y", setpath));
            }
            //px = iniFile.Read("A2CCD2", "AssemblyPixel_X", setpath);
            //if (px != "")
            //{
            //    A2CCD2.xpm = double.Parse(px);
            //    A2CCD2.ypm = double.Parse(iniFile.Read("A2CCD2", "AssemblyPixel_Y", setpath));
            //}
            A2CCD2.Gain = iniFile.Read("A2CCD2", "Gain", setpath);
            if (A2CCD2.Gain == "")
                A2CCD2.Gain = "0";
            A2CCD2.ExposureTime = iniFile.Read("A2CCD2", "ExposureTime", setpath);
            if (A2CCD2.ExposureTime == "")
                A2CCD2.ExposureTime = "35000";
            #endregion
            #region PCCD1
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("P_CCD1", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.5");
            PCCD1.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("P_CCD1", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            PCCD1.Port = port;
            check1 = iniFile.Read("P_CCD1", "Checked", path);
            switch (check1)
            {
                case "YES": PCCD1.IsCheck = true; break;
                case "NO": PCCD1.IsCheck = false; break;
            }
            Sof = iniFile.Read("PCCD1", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            PCCD1.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("PCCD1", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            PCCD1.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("PCCD1", "defiexuion_angle", setpath);
            if (ta != "")
                PCCD1.angleC = double.Parse(ta);
            px = iniFile.Read("PCCD1", "Pixel_X", setpath);
            if (px != "")
            {
                PCCD1.xpm = double.Parse(px);
                PCCD1.ypm = double.Parse(iniFile.Read("PCCD1", "Pixel_Y", setpath));
            }
            PCCD1.Gain = iniFile.Read("PCCD1", "Gain", setpath);
            if (PCCD1.Gain == "")
                PCCD1.Gain = "0";
            PCCD1.ExposureTime = iniFile.Read("PCCD1", "ExposureTime", setpath);
            if (PCCD1.ExposureTime == "")
                PCCD1.ExposureTime = "35000";
            #endregion
            #region PCCD2
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("P_CCD2", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.6");
            PCCD2.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("P_CCD2", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            PCCD2.Port = port;
            check1 = iniFile.Read("P_CCD2", "Checked", path);
            switch (check1)
            {
                case "YES": PCCD2.IsCheck = true; break;
                case "NO": PCCD2.IsCheck = false; break;
            }
            Sof = iniFile.Read("PCCD2", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            PCCD2.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("PCCD2", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            PCCD2.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("PCCD2", "defiexuion_angle", setpath);
            if (ta != "")
                PCCD2.angleC = double.Parse(ta);
            px = iniFile.Read("PCCD2", "Pixel_X", setpath);
            if (px != "")
            {
                PCCD2.xpm = double.Parse(px);
                PCCD2.ypm = double.Parse(iniFile.Read("PCCD2", "Pixel_Y", setpath));
            }
            PCCD2.Gain = iniFile.Read("PCCD2", "Gain", setpath);
            if (PCCD2.Gain == "")
                PCCD2.Gain = "0";
            PCCD2.ExposureTime = iniFile.Read("PCCD2", "ExposureTime", setpath);
            if (PCCD2.ExposureTime == "")
                PCCD2.ExposureTime = "35000";
            #endregion
            #region GCCD1
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("G_CCD1", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.7");
            GCCD1.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("G_CCD1", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            GCCD1.Port = port;
            check1 = iniFile.Read("G_CCD1", "Checked", path);
            switch (check1)
            {
                case "YES": GCCD1.IsCheck = true; break;
                case "NO": GCCD1.IsCheck = false; break;
            }
            Sof = iniFile.Read("GCCD1", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            GCCD1.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("GCCD1", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            GCCD1.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("GCCD1", "defiexuion_angle", setpath);
            if (ta != "")
                GCCD1.angleC = double.Parse(ta);
            px = iniFile.Read("GCCD1", "Pixel_X", setpath);
            if (px != "")
            {
                GCCD1.xpm = double.Parse(px);
                GCCD1.ypm = double.Parse(iniFile.Read("GCCD1", "Pixel_Y", setpath));
            }
            #region 針頭辨識
            GCCD1.NeedleTipTest.RegionRow = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.RegionRow", "0", propath));
            GCCD1.NeedleTipTest.RegionColumn = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.RegionColumn", "0", propath));
            GCCD1.NeedleTipTest.RegionPhi = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.RegionPhi", "0", propath));
            GCCD1.NeedleTipTest.RegionLength1 = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.RegionLength1", "0", propath));
            GCCD1.NeedleTipTest.RegionLength2 = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.RegionLength2", "0", propath));

            GCCD1.NeedleTipTest.ContrastSet = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.ContrastSet", "0", propath));
            GCCD1.NeedleTipTest.Gray = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Gray", "0", propath));
            GCCD1.NeedleTipTest.NeedleChoice = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.NeedleChoice", "0", propath));

            GCCD1.NeedleTipTest.Radius = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Radius", "100", propath));
            GCCD1.NeedleTipTest.Measure_Transition = IniFile.Read("GCCD1", "NeedleTipTest.Measure_Transition", "negative", propath);
            GCCD1.NeedleTipTest.Measure_Select = IniFile.Read("GCCD1", "NeedleTipTest.Measure_Select", "last", propath);
            GCCD1.NeedleTipTest.Num_Measures = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Num_Measures", "10", propath));
            GCCD1.NeedleTipTest.Measure_Length1 = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Measure_Length1", "30", propath));
            GCCD1.NeedleTipTest.Measure_Length2 = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Measure_Length2", "10", propath));
            GCCD1.NeedleTipTest.Measure_Threshold = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Measure_Threshold", "30", propath));

            GCCD1.NeedleTipTest.X_LowerSet = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.X_LowerSet", "0", propath));
            GCCD1.NeedleTipTest.X_UpperSet = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.X_UpperSet", "0", propath));
            GCCD1.NeedleTipTest.Y_LowerSet = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Y_LowerSet", "0", propath));
            GCCD1.NeedleTipTest.Y_UpperSet = int.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Y_UpperSet", "0", propath));

            GCCD1.NeedleTipTest.X_LowerValue = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.X_LowerValue", "0", propath));
            GCCD1.NeedleTipTest.X_UpperValue = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.X_UpperValue", "0", propath));
            GCCD1.NeedleTipTest.Y_LowerValue = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Y_LowerValue", "0", propath));
            GCCD1.NeedleTipTest.Y_UpperValue = double.Parse(IniFile.Read("GCCD1", "NeedleTipTest.Y_UpperValue", "0", propath));
            #endregion





            //px = iniFile.Read("GCCD1", "AssemblyPixel_X", setpath);
            //if (px != "")
            //{
            //    GCCD1.xpm = double.Parse(px);
            //    GCCD1.ypm = double.Parse(iniFile.Read("GCCD1", "AssemblyPixel_Y", setpath));
            //}
            GCCD1.Gain = iniFile.Read("GCCD1", "Gain", setpath);
            if (GCCD1.Gain == "")
                GCCD1.Gain = "0";
            GCCD1.ExposureTime = iniFile.Read("GCCD1", "ExposureTime", setpath);
            if (GCCD1.ExposureTime == "")
                GCCD1.ExposureTime = "35000";
            #endregion
            #region GCCD2
          
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("G_CCD2", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.8");
            GCCD2.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("G_CCD2", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            GCCD2.Port = port;
            check1 = iniFile.Read("G_CCD2", "Checked", path);
            switch (check1)
            {
                case "YES": GCCD2.IsCheck = true; break;
                case "NO": GCCD2.IsCheck = false; break;
            }
            Sof = iniFile.Read("GCCD2", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            GCCD2.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("GCCD2", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            GCCD2.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("GCCD2", "defiexuion_angle", setpath);
            if (ta != "")
                GCCD2.angleC = double.Parse(ta);
            px = iniFile.Read("GCCD2", "Pixel_X", setpath);
            if (px != "")
            {
                GCCD2.xpm = double.Parse(px);
                GCCD2.ypm = double.Parse(iniFile.Read("GCCD2", "Pixel_Y", setpath));
            }
           
            //px = iniFile.Read("GCCD2", "AssemblyPixel_X", setpath);
            //if (px != "")
            //{
            //    GCCD2.xpm = double.Parse(px);
            //    GCCD2.ypm = double.Parse(iniFile.Read("GCCD2", "AssemblyPixel_Y", setpath));
            //}
            GCCD2.Gain = iniFile.Read("GCCD2", "Gain", setpath);
            if (GCCD2.Gain == "")
                GCCD2.Gain = "0";
            GCCD2.ExposureTime = iniFile.Read("GCCD2", "ExposureTime", setpath);
            if (GCCD2.ExposureTime == "")
                GCCD2.ExposureTime = "35000";
            #endregion
            #region QCCD
            string ccdBrand = iniFile.Read("CCD", "CCDChoice", path);
            QCCD.CCDBrand = (ccdBrand != "" ? int.Parse(ccdBrand) : 0);
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("Q_CCD", "Ip", path), out ip))
                ip = IPAddress.Parse("192.168.0.9");
            QCCD.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("Q_CCD", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            QCCD.Port = port;
            string check3 = iniFile.Read("Q_CCD", "Checked", path);
            switch (check3)
            {
                case "YES": QCCD.IsCheck = true; break;
                case "NO": QCCD.IsCheck = false; break;
            }
            Sof = iniFile.Read("QCCD", "SaveOfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            QCCD.SaveOf = ((Sof == "True") ? true : false);
            Srf = iniFile.Read("QCCD", "SaveRfigure", Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini");
            QCCD.SaveRf = ((Srf == "True") ? true : false);
            ta = iniFile.Read("QCCD", "defiexuion_angle", setpath);
            if (ta != "")
                QCCD.angleC = double.Parse(ta);
            px = iniFile.Read("QCCD", "Pixel_X", setpath);
            if (px != "")
            {
                QCCD.xpm = double.Parse(px);
                QCCD.ypm = double.Parse(iniFile.Read("QCCD", "Pixel_Y", setpath));
            }
            if (!Barcode1.QCCDisChecked)
            {
                Run2.lblQbar.Hide();
                Run2.lblQbarResult.Hide();
                QCCD.Gain = iniFile.Read("QCCD", "Gain", setpath);
                if (QCCD.Gain == "")
                    QCCD.Gain = "0";
                QCCD.ExposureTime = iniFile.Read("QCCD", "ExposureTime", setpath);
                if (QCCD.ExposureTime == "")
                    QCCD.ExposureTime = "35000";
            }
            else
            {
                if (QCCD.CCDBrand == 1)
                    MessageBox.Show("QCCD扫码功能已打开，请选择Basler或关闭扫码功能！");
                Run2.lblQbar.Show();
                Run2.lblQbarResult.Show();
            }
            #endregion

            #region PLC
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("PLC", "Ip", path), out ip))
                ip = IPAddress.Parse("200.200.0.2");
            PLC.Ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("PLC", "Port", path), out port))
                port = 12289;
            port = (port <= 0 ? 12289 : port);
            PLC.Port = port;
            #endregion
            #region GT2
            ip = IPAddress.Any;
            if (!IPAddress.TryParse(iniFile.Read("GT2", "Ip", path), out ip))
                ip = IPAddress.Parse("200.200.0.1");
            GT2.ip = ip;
            port = 0;
            if (!int.TryParse(iniFile.Read("GT2", "Port", path), out port))
                port = 5000;
            port = (port <= 0 ? 5000 : port);
            GT2.Port = port;
            string check4 = iniFile.Read("GT2", "Checked", path);
            switch (check4)
            {
                case "YES": GT2.IsCheck = true; break;
                case "NO": GT2.IsCheck = false; break;
            }
            string chischecked = iniFile.Read("GT2", "CH1Checked", path);
            GT2.CH1IsCheck = (chischecked == "True" ? true : false);
            chischecked = iniFile.Read("GT2", "CH2Checked", path);
            GT2.CH2IsCheck = (chischecked == "True" ? true : false);
            chischecked = iniFile.Read("GT2", "CH3Checked", path);
            GT2.CH3IsCheck = (chischecked == "True" ? true : false);
            chischecked = iniFile.Read("GT2", "CH4Checked", path);
            GT2.CH4IsCheck = (chischecked == "True" ? true : false);
            chischecked = iniFile.Read("GT2", "CH5Checked", path);
            GT2.CH5IsCheck = (chischecked == "True" ? true : false);
            #endregion
        }
        void ReadQccdAVI()
        {
            //固定环
            string strReti = iniFile.Read("QCCD", "AVI1ischecked", propath);QCCD.AVI1IsCheck = (strReti == "True" ? true : false);
            strReti = iniFile.Read("QCCD", "InRange", propath);             QCCD.dInRange = (strReti != "" ? int.Parse(strReti) : 1);
            strReti = iniFile.Read("QCCD", "OutRange", propath);            QCCD.dOutRange = (strReti != "" ? int.Parse(strReti) : 200);
            strReti = iniFile.Read("QCCD", "Detection_Black", propath);     QCCD.Detection_Black = ((strReti == "True") ? true : false);
            strReti = iniFile.Read("QCCD", "Detection_White", propath);     QCCD.Detection_White = ((strReti == "True") ? true : false);
            strReti = iniFile.Read("QCCD", "GraythresholdBlack", propath);  QCCD.dGraythresholdBlack = (strReti != "" ? int.Parse(strReti) : 1);
            strReti = iniFile.Read("QCCD", "GraythresholdWhite", propath);  QCCD.dGraythresholdWhite = (strReti != "" ? int.Parse(strReti) : 1);
            strReti = iniFile.Read("QCCD", "UnderSizeArea", propath);       QCCD.dUnderSizeArea = (strReti != "" ? int.Parse(strReti) : 1);
            strReti = iniFile.Read("QCCD", "GlueAngleSet", propath);        QCCD.iGlueAngleSet = (strReti != "" ? int.Parse(strReti) : 1);
            strReti = iniFile.Read("QCCD", "GlueRatioSet", propath);        QCCD.iGlueRatioSet = (strReti != "" ? int.Parse(strReti) : 1);
            strReti = iniFile.Read("QCCD", "AngleSet", propath);            QCCD.dAngleSet = (strReti != "" ? int.Parse(strReti) : 1);
            //小台阶
            string strPF = iniFile.Read("QCCD", "AVI2ischecked", propath);  QCCD.AVI2IsCheck = ((strPF == "True") ? true : false);
            strPF = iniFile.Read("QCCD", "InRangePF", propath);             QCCD.dInRangePF = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "OutRangePF", propath);            QCCD.dOutRangePF = (strPF != "" ? int.Parse(strPF) : 200);
            strPF = iniFile.Read("QCCD", "DetectionPF_Dark2", propath);     QCCD.DetectionPF_Dark2 = ((strPF == "True") ? true : false);
            strPF = iniFile.Read("QCCD", "DetectionPF_Light2", propath);    QCCD.DetectionPF_Light2 = ((strPF == "True") ? true : false);
            strPF = iniFile.Read("QCCD", "DetectionPF_Black", propath);     QCCD.DetectionPF_Black = ((strPF == "True") ? true : false);
            strPF = iniFile.Read("QCCD", "DetectionPF_White", propath);     QCCD.DetectionPF_White = ((strPF == "True") ? true : false);
            strPF = iniFile.Read("QCCD", "ClosingPF2", propath);            QCCD.ClosingPF2 = ((strPF == "True") ? true : false);
            strPF = iniFile.Read("QCCD", "OpeningPF2", propath);            QCCD.OpeningPF2 = ((strPF == "True") ? true : false);
            strPF = iniFile.Read("QCCD", "GraythresholdBlackPF", propath);  QCCD.dGraythresholdBlackPF = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "GraythresholdWhitePF", propath);  QCCD.dGraythresholdWhitePF = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "UnderSizeAreaPF", propath);       QCCD.dUnderSizeAreaPF = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "DynthresholdDarkPF2", propath);   QCCD.iDynthresholdDarkPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "DynthresholdLightPF2", propath);  QCCD.iDynthresholdLightPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "GraythresholdBlackPF2", propath); QCCD.iGraythresholdBlackPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "GraythresholdWhitePF2", propath); QCCD.iGraythresholdWhitePF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "CloseWidthPF2", propath);         QCCD.iCloseWidthPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "CloseHeightPF2", propath);        QCCD.iCloseHeightPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "OpenWidthPF2", propath);          QCCD.iOpenWidthPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "OpenHeightPF2", propath);         QCCD.iOpenHeightPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "UnderSizeAreaPF2", propath);      QCCD.iUnderSizeAreaPF2 = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "GlueAngleSetPF", propath);        QCCD.iGlueAngleSetPF = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "GlueRatioSetPF", propath);        QCCD.iGlueRatioSetPF = (strPF != "" ? int.Parse(strPF) : 1);
            strPF = iniFile.Read("QCCD", "AngleSetPF", propath);            QCCD.dAngleSetPF = (strPF != "" ? int.Parse(strPF) : 1);
            QCCD.CompareSetPF = int.Parse(IniFile.Read("QCCD", "CompareSetPF", "0", propath));
            //QCCDBarcode
            string rmm = iniFile.Read("BarcodeReader", "SearchRangeRow1", BarPath);
            if (rmm != "")
            {
                Barcode1.HandleRow1 = double.Parse(rmm);
                Barcode1.HandleCol1 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeCol1", BarPath));
                Barcode1.HandleRow2 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeRow2", BarPath));
                Barcode1.HandleCol2 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeCol2", BarPath));
                Barcode1.BarcodeAngleSet = int.Parse(IniFile.Read("BarcodeReader", "BarcodeAngleSet","0", BarPath));
                Barcode1.AllowableOffsetAngle = int.Parse(IniFile.Read("BarcodeReader", "AllowableOffsetAngle","180", BarPath));
            }
        }
        private void FactorySelected(string index)
        {
            //m_Web_Tray_InOut.Url = "http://192.168.0.26/mesws/Assembly/Eqp_LMT.asmx";
            switch (index)
            {
                case "XM":
                    Sys.ECCIP = "192.168.21.6";
                    m_Web_Tray_InOut.Url = "http://192.168.21.6/MESWS/Assembly/Eqp_LMT.asmx";
                    break;
                case "JM":
                    Sys.ECCIP = "192.168.49.6";
                    m_Web_Tray_InOut.Url = "http://192.168.49.6/MESWS/Assembly/Eqp_LMT.asmx";
                    break;
                case "TD":
                    Sys.ECCIP = "10.50.10.201";
                    m_Web_Tray_InOut.Url = "http://10.50.10.201/MESWS/Assembly/Eqp_LMT.asmx";
                    break;
                default://測試
                    Sys.ECCIP = "192.168.21.6";
                    m_Web_Tray_InOut.Url = "http://192.168.0.26/MESWS/Coated/CoatedService.asmx";
                    break;
            }
        }

        private void timerRunningTime_Tick(object sender, EventArgs e)
        {
            TimeRunning += 1;
            Days = (int)(TimeRunning / 86400);
            int left = (int)(TimeRunning % 86400);
            Hours = left / 3600;
            left = left % 3600;
            Minutes = left / 60;
            Seconds = left % 60;
            lblRunTime.Text = string.Format("{0,2}天{1,2}时{2,2}分{3,2}秒", Days, Hours, Minutes, Seconds);
            mtime = DateTime.Now.ToString("yyyyMMdd");
            NowTime.Text = mtime;
            NowHour.Text = DateTime.Now.ToString("HH:mm:ss");
            NowDay.Text = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            
            #region 生成
            #region IamgePath
            ImagePath = Sys.ReportImage + "\\" + mtime + "\\" + Sys.CurrentProduction;
            if (Sys.CurrentProduction != "" & !Directory.Exists(ImagePath + "\\A1CCD1\\OriginalImage"))
            {
                for (int i = 0; i < theImageIP.Length; i++)
                {
                    DirectoryInfo di = new DirectoryInfo(ImagePath + "\\" + theImageIP[i] + "\\PASS\\OriginalImage");
                    DirectoryInfo dj = new DirectoryInfo(ImagePath + "\\" + theImageIP[i] + "\\PASS\\ResultImage");
                    DirectoryInfo dl = new DirectoryInfo(ImagePath + "\\" + theImageIP[i] + "\\NG\\OriginalImage");
                    DirectoryInfo dk = new DirectoryInfo(ImagePath + "\\" + theImageIP[i] + "\\NG\\ResultImage");
                    di.Create();
                    dj.Create();
                    dl.Create();
                    dk.Create();
                }
            }
            #endregion
            weblogfile = DateTime.Now.ToString("yyyyMMdd") + "_" + Sys.MachineId + "_WebAlarm.txt";
            gluelogfile = DateTime.Now.ToString("yyyyMMdd") + "_" + Sys.MachineId + "_GlueAlarm.txt";
            if (!File.Exists(Sys.AlarmPath + "\\" + gluelogfile))
            {
                gluelog = new Log(Sys.AlarmPath + "\\" + gluelogfile);
                gluelog.log("Start:Newday");
            }
            if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
            {
                weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                weblog.log("Start:Newday");
            }
            if (!RiReader.IsChecked)
            {
                #region  生成新的log文件夹及文件（防呆）
                string logfile = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
                string logpath = Sys.ReportLog1 + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(logpath))
                {
                    LogDate = DateTime.Now.ToString("yyyy-MM-dd");
                    LogTime = DateTime.Now.ToString("yyyyMMdd");
                    iniFile.Write("LOGMESSAGE", "LogdTime", LogTime, setpath);
                    iniFile.Write("LOGMESSAGE", "Logdate", LogDate, setpath);
                    Directory.CreateDirectory(logpath);
                }
                if (!File.Exists(logpath + "\\" + logfile))
                {
                    ProductCount = 0;
                    File.WriteAllText(logpath + "\\" + logfile, FileHeader);
                }
                #endregion
            }
            #endregion

            if (DateTime.Now.ToString("HH:mm:ss") == "00:00:00" ||
                DateTime.Now.ToString("HH:mm:ss") == "08:00:00")
            {
                Thread fileEdit = new Thread(FileEdit);
                fileEdit.IsBackground = true;
                fileEdit.Start();
            }

            string mnt = DateTime.Now.ToString("HH:mm:ss").Substring(4, 4);
            if (mnt == "0:00")
            {
                Thread traverse = new Thread(TraversandChangeName);
                traverse.IsBackground = true;
                traverse.Start();
            }

            if (TimeRunning >= 15)
                lblLoading.SendToBack();
            btnVisions.Text = "PLC:" + PLC.PLCVisions + "\r\t" + "HMI:" + PLC.HMIVisions;
        }

        void FileEdit()
        {
            cancelTrigger = true;
            DeleFiles();
            //btnZip.PerformClick();
        }
        void TraversandChangeName()
        {
            #region 更改文档名称
            try
            {
                string lpath = Sys.ReportLog1 + "\\" + LogDate;
                string mnt = DateTime.Now.ToString("HH:mm:ss").Substring(0, 4);
                if (DateTime.Now.ToString("HH:mm:ss") == "00:2")  //遍历前一晚文件生成结束标志
                {
                    string dy = DateTime.Now.Year.ToString(), dm = DateTime.Now.Month.ToString(), dd = DateTime.Now.AddDays(-1).Day.ToString();
                    if (DateTime.Now.Month < 10)
                        dm = "0" + dm;
                    if (DateTime.Now.Day < 10)
                        dd = "0" + dd;
                    lpath = Sys.ReportLog1 + "\\" + dy + "-" + dm + "-" + dd;
                }
                DirectoryInfo theFolder = new DirectoryInfo(lpath);
                FileInfo[] TfileInfo = theFolder.GetFiles();
                string logfile = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
                for (int i = 0; i < TfileInfo.Length; i++)
                {
                    FileStream fs = new FileStream(lpath + "\\" + TfileInfo[i].Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                    int i_p = 0;
                    while (sr.ReadLine() != null)
                    {
                        i_p++;
                    }
                    sr.Close();
                    fs.Close();
                    string txtl = TfileInfo[i].Name.Substring(TfileInfo[i].Name.Length - 5, 5);
                    if (TfileInfo[i].Name != logfile && TfileInfo[i].Name.Substring(0, 4) != "Done" && txtl != "_.txt" & i_p > 1)//
                    {
                        string lFileName = lpath + "\\" + TfileInfo[i].Name;
                        string lnewFileName = lpath + "\\" + "Done_" + TfileInfo[i].Name;
                        if (!ModifyFilename(lFileName, lnewFileName))
                        {
                            try
                            {
                                if (File.Exists(lFileName))
                                {
                                    ProductCount = 0;
                                    File.Move(lFileName, lnewFileName);
                                }
                            }
                            catch
                            {
                            }
                            if (RiReader.BPFWebChecked)
                            {
                                BPFOut();
                            }
                        }
                    }
                }
            }
            catch
            { }
            #endregion
        }
       
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定要离开程式吗?", "",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.OK)
            {
                quit = true;
                CloseAllChildren();
                CloseTheImageProvider1();
                CloseTheImageProvider2();
                CloseTheImageProvider3();
                CloseTheImageProvider4();
                CloseTheImageProvider5();
                CloseTheImageProvider6();
                CloseTheImageProvider7();
                CloseTheImageProvider8();
                CloseTheImageProvider9();
                CloseTheImageProvider10();
                Thread.Sleep(1000);
                try
                {
                    //Application.Exit();
                    //System.Environment.Exit(0);
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                catch
                {
                    MessageBox.Show("error");
                }
            }
        }
        private void btnMain_Click(object sender, EventArgs e)
        {
            if (halcon.IsPreview)
            {
                MessageBox.Show("请先停止预览！");
                return;
            }
            if (lblUser.Text != "Admin")
            {
                FrmLogin fb = new FrmLogin();
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Sys.NoAutoMatic = false;
                    lblUser.Text = User.CurrentUser;
                    Color1.BackColor = Color.Green;
                    Color2.BackColor = Color.Gray;
                    Color3.BackColor = Color.Gray;
                    halcon.AIsChecked = false;
                    if (this.ActiveMdiChild is FrmDisplay1)
                        return;
                    CloseAllChildren();
                    Run = new FrmDisplay1(this);
                    Run.Show();
                }
            }
            else
            {
                Sys.NoAutoMatic = false;
                Color1.BackColor = Color.Green;
                Color2.BackColor = Color.Gray;
                Color3.BackColor = Color.Gray;
                halcon.AIsChecked = false;
                if (this.ActiveMdiChild is FrmDisplay1)
                    return;
                CloseAllChildren();
                Run = new FrmDisplay1(this);
                Run.Show();
            }
        }
        private void btnViewSet_Click(object sender, EventArgs e)
        {
            if (lblUser.Text != "Admin")
            {
                fb = new FrmLogin();
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Sys.NoAutoMatic = true;
                    lblUser.Text = User.CurrentUser;
                    Color1.BackColor = Color.Gray;
                    Color2.BackColor = Color.Green;
                    Color3.BackColor = Color.Gray;
                    if (this.ActiveMdiChild is FrmVisionSet)
                        return;
                    CloseAllChildren();
                    VisionSet = new FrmVisionSet(this);
                    VisionSet.Show();
                }
            }
            else
            {
                Sys.NoAutoMatic = true;
                Color1.BackColor = Color.Gray;
                Color2.BackColor = Color.Green;
                Color3.BackColor = Color.Gray;
                if (this.ActiveMdiChild is FrmVisionSet)
                    return;
                CloseAllChildren();
                VisionSet = new FrmVisionSet(this);
                VisionSet.Show();
            }

        }
        private void btnSetUp_Click(object sender, EventArgs e)
        {
            if (lblUser.Text != "Admin")
            {
                fb = new FrmLogin();
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Sys.NoAutoMatic = true;
                    lblUser.Text = User.CurrentUser;
                    Color1.BackColor = Color.Gray;
                    Color2.BackColor = Color.Gray;
                    Color3.BackColor = Color.Green;
                    if (this.ActiveMdiChild is FrmSetUp)
                        return;
                    CloseAllChildren();
                    SetUp = new FrmSetUp(this);
                    SetUp.Show();
                    SetUp.timerRefleshUI.Enabled = true;
                }
            }
            else
            {
                Sys.NoAutoMatic = true;
                Color1.BackColor = Color.Gray;
                Color2.BackColor = Color.Gray;
                Color3.BackColor = Color.Green;
                if (this.ActiveMdiChild is FrmSetUp)
                    return;
                CloseAllChildren();
                SetUp = new FrmSetUp(this);
                SetUp.Show();
                SetUp.timerRefleshUI.Enabled = true;
            }
        }
        private void btnBarCode_Click(object sender, EventArgs e)
        {
            if (Barcode1.IsChecked)
            {
                if (lblUser.Text != "Admin")
                {
                    fb = new FrmLogin();
                    DialogResult result = fb.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        Sys.NoAutoMatic = true;
                        lblUser.Text = User.CurrentUser;
                        Color1.BackColor = Color.Gray;
                        Color2.BackColor = Color.Gray;
                        Color3.BackColor = Color.Gray;
                        if (this.ActiveMdiChild is FrmBarcode)
                            return;
                        CloseAllChildren();
                        Fbarcode = new FrmBarcode(this);
                        Fbarcode.Show();
                    }
                }
                else
                {
                    Sys.NoAutoMatic = true;
                    lblUser.Text = User.CurrentUser;
                    Color1.BackColor = Color.Gray;
                    Color2.BackColor = Color.Gray;
                    Color3.BackColor = Color.Gray;
                    if (this.ActiveMdiChild is FrmBarcode)
                        return;
                    CloseAllChildren();
                    Fbarcode = new FrmBarcode(this);
                    Fbarcode.Show();
                }
            }
            else
            {
                MessageBox.Show("BarCoder Reader未开启，请进入" + "*" + "其他设置" + "*" + "界面进行设置！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        int tis = 0;
        private void btnAviShow_Click(object sender, EventArgs e)
        {
            switch (tis)
            {
                case 0:
                    Run2.lblqccd.Text = "AVI";
                    Run2.pnlTray.BringToFront();
                    break;
                case 1:
                    Run2.lblqccd.Text = "Q_CCD";
                    Run2.pnlTray.SendToBack();
                    break;
            }
            tis++;
            if (tis == 2)
            {
                tis = 0;
            }
        }
        private void lblUser_Click(object sender, EventArgs e)
        {
            ti = ((lblUser.Text == "------") ? 0 : 1);
            switch (ti)
            {
                case 0:
                    FrmLogin fb = new FrmLogin();
                    DialogResult result = fb.ShowDialog();
                    if (result != DialogResult.OK)
                        lblUser.Text = "------";
                    else
                        lblUser.Text = User.CurrentUser;
                    break;
                case 1:
                    DialogResult dr = MessageBox.Show("确定要退出该用户吗?", "",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.OK)
                    {
                        lblUser.Text = "------";
                        Color1.BackColor = Color.Green;
                        Color2.BackColor = Color.Gray;
                        Color3.BackColor = Color.Gray;
                        if (this.ActiveMdiChild is FrmDisplay1)
                            return;
                        CloseAllChildren();
                        Run = new FrmDisplay1(this);
                        Run.Show();
                        SetUp.timerRefleshUI.Enabled = false;
                    }
                    break;
            }
            ti++;
            if (ti == 2)
                ti = 0;
        }
        private void btnConfirm_Click(object sender, EventArgs e)  // BPF物料出站防呆（手动物料出站后清空Barcode）
        {
            gBWarm.SendToBack();
            ChangeNameNewBarcode();  
        }
       
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            CommucationPlc.CancelAsync();
            DLEN1loop.CancelAsync();
            if (GT2.IsConnected)
                GT2.socket.Close();
            /* Close the image provider. */
            CloseTheImageProvider1();
            CloseTheImageProvider2();
            CloseTheImageProvider3();
            CloseTheImageProvider4();
            CloseTheImageProvider5();
            CloseTheImageProvider6();
            CloseTheImageProvider7();
            CloseTheImageProvider8();
            CloseTheImageProvider9();

            OfflineReConncet.CancelAsync();
            timerAlatm.Enabled = false;
            quit = true;
        }
        void CloseAllChildren()
        {
            Form[] children = this.MdiChildren;
            foreach (Form frm in children)
            {
                frm.Dispose();
            }
            GC.Collect();
        }
        public void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                FrmMain.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        #region WebService
        //超時跳出設置
        static void CallWithTimeout(Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }
        MBMatchLabel.MatchLabel MBWebCall = new MBMatchLabel.MatchLabel();
        JMBMatchLabel.MatchLabel JMBWebCall = new JMBMatchLabel.MatchLabel();
        glueWebReference.MatchLabel glueWeb = new glueWebReference.MatchLabel();
        string dtResult = string.Empty; string sErrory = string.Empty;
        void m_Web_Tray_In()
        {
            #region Webservice
            RiReader.Web_Tray_InOutStation_Result = false;
            RiReader.Web_Tray_InOutStation_sMsg = "";
            RiReader.Web_Tray_InOutStation_Result = m_Web_Tray_InOut.CheckIn(RiReader.Barcode_In, Sys.CurrentProduction, Sys.MachineId, out RiReader.Web_Tray_InOutStation_sMsg);
            #endregion
        }
        
        void m_Web_Tray_Out()
        {
            #region Webservice
            RiReader.Web_Tray_InOutStation_Result = false;
            string sGlue = Glue.Barcode;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
            RiReader.Web_Tray_InOutStation_sMsg = "";
            DataTable dtLensBarcode = new DataTable("LensBarcode");
            for (int i = 0; i < ProductCount; i++)
            {
                DataColumn dc = new DataColumn();
                dtLensBarcode.Columns.Add(dc);
            }
            //Ping容易P不通,先注釋掉
            //bool bEtherNetOK = PingIpOrDomainName(Sys.ECCIP) &&
            //                       (System.Diagnostics.Process.GetProcessesByName("ECClientAp").Length > 0);
            //if (bEtherNetOK)
            //{
            RiReader.Web_Tray_InOutStation_Result = m_Web_Tray_InOut.CheckOut_YF(RiReader.Barcode_In, RiReader.Barcode_Out, Sys.CurrentProduction, Sys.MachineId, sGlue, ProductCount, dtLensBarcode, out RiReader.Web_Tray_InOutStation_sMsg);
           
                //RiReader.Web_Tray_InOutStation_Result = m_Web_Tray_InOut.CheckOut(RiReader.Barcode, Sys.CurrentProduction, Sys.MachineId, sGlue, ProductCount, dtLensBarcode, out RiReader.Web_Tray_InOutStation_sMsg);
            //}
            //else
            //{
            //    RiReader.Web_Tray_InOutStation_Result = false;
            //    RiReader.Web_Tray_InOutStation_sMsg = "網線未連接";
            //}
            
            #endregion
        }
        void GSWeb()
        {
            #region WebService
            if (GseoConn)
            {
                try
                {
                    string produceName = "";
                    if (Sys.CurrentProduction == "8982LH")
                        produceName = (Sys.CurrentProID != "" ? "8982AA" : Sys.CurrentProID);
                    else
                        produceName = Sys.CurrentProduction;
                    if (Sys.Factory == "XM")
                    {
                        if (Reader.IsChecked)
                        {
                            if (!Glue.WeightWebChecked)
                                dtResult = MBWebCall.CheckBeforeL_H(RiReader.Barcode, Reader.Barcode, out sErrory);
                            else
                                dtResult = MBWebCall.CheckBeforeL_H_New(RiReader.Barcode, Reader.Barcode, produceName, Sys.MachineId, out sErrory);
                        }
                        else
                        {
                            if (!Glue.WeightWebChecked)
                                dtResult = MBWebCall.GetPlasmaLotionInterval(RiReader.Barcode, out sErrory);
                            else
                                dtResult = MBWebCall.GetPlasmaLotionInterval_New(RiReader.Barcode, produceName, Sys.MachineId, out sErrory);
                        }
                    }
                    if (Sys.Factory == "JM" || Sys.Factory == "TD")
                    {
                        if (Reader.IsChecked)
                        {
                            if (!Glue.WeightWebChecked)
                                dtResult = JMBWebCall.CheckBeforeL_H(RiReader.Barcode, Reader.Barcode, out sErrory);
                            else
                                dtResult = JMBWebCall.CheckBeforeL_H_New(RiReader.Barcode, Reader.Barcode, produceName, Sys.MachineId, out sErrory);
                        }
                        else
                        {
                            if (!Glue.WeightWebChecked)
                                dtResult = JMBWebCall.GetPlasmaLotionInterval(RiReader.Barcode, out sErrory);
                            else
                                dtResult = JMBWebCall.GetPlasmaLotionInterval_New(RiReader.Barcode, produceName, Sys.MachineId, out sErrory);
                        }
                    }
                }
                catch (Exception ER)
                {
                    dtResult = "";
                    sErrory = ER.Message.ToString();
                    if (sErrory == "无法连接到远程服务器")
                        sErrory = "与MES系统网络连接异常，请处理！";
                }
            }
            else
            {
                sErrory = "与MES系统网络连接异常，请处理！";
                dtResult = "";
            }
            #endregion
        }

        //判断网线是否正常连接
        private static bool PingIpOrDomainName(string strIpOrDName)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 600;
                PingReply objPinReply = objPingSender.Send(strIpOrDName, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        BPFWeb.Eqp_BPF BPFWebCall = new BPFWeb.Eqp_BPF();
        JBPFWebIn.Eqp_BPF JBPFWebInCall = new JBPFWebIn.Eqp_BPF();
        JBPFWebOut.MatchLabel JBPFWebOutCall = new JBPFWebOut.MatchLabel();
        bool bpfdtResult = false;
        void GSBPFWebIn()
        {
            #region WebService
            if (GseoConn)
            {
                try
                {
                    if (Sys.Factory == "XM")
                        bpfdtResult = BPFWebCall.CheckInputTray(RiReader.Barcode, Sys.MachineId, DateTime.Now, out sErrory);
                    if (Sys.Factory == "JM" || Sys.Factory == "TD")
                        bpfdtResult = JBPFWebInCall.CheckInputTray(RiReader.Barcode, Sys.MachineId, DateTime.Now, out sErrory);
                    if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                        weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                    if (!bpfdtResult)
                        weblog.log("Web报错:" + RiReader.Barcode + "In," + sErrory);
                    else
                        weblog.log("WebOK:" + RiReader.Barcode + "In," + sErrory);
                }
                catch (Exception ER)
                {
                    bpfdtResult = false;
                    sErrory = ER.Message.ToString();
                    if (sErrory == "无法连接到远程服务器")
                        sErrory = "无法连接到远程服务器！与MES系统网络连接异常，请检查机台与公司网络的连接状况！";
                }
            }
            else
            {
                sErrory = "与MES系统网络连接异常，请检查机台与公司网络的连接状况！";
                bpfdtResult = false;
            }
            #endregion
        }
        void GSBPFWebOut()
        {
            #region WebService
            if (GseoConn)
            {
                try
                {
                    if (Sys.Factory == "XM")
                        bpfdtResult = BPFWebCall.DoFinish(bpfWebMessage, RiReader.Barcode, Sys.MachineId, Sys.CurrentProduction, DateTime.Now, Reader.Barcode, out sErrory);
                    if (Sys.Factory == "JM" || Sys.Factory == "TD")
                        bpfdtResult = JBPFWebOutCall.AssemblyBPFOut(RiReader.Barcode, Reader.Barcode, Sys.MachineId, out sErrory);
                    if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                        weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                    if (!bpfdtResult)
                        weblog.log("Web报错:" + RiReader.Barcode + "Out," + sErrory);
                    else
                        weblog.log("WebOK:" + RiReader.Barcode + "Out," + sErrory);
                }
                catch (Exception ER)
                {
                    bpfdtResult = false;
                    sErrory = ER.Message.ToString();
                    if (sErrory == "无法连接到远程服务器")
                        sErrory = "无法连接到远程服务器！与MES系统网络连接异常，请检查机台与公司网络的连接状况！";
                }
            }
            else
            {
                sErrory = "与MES系统网络连接异常，请检查机台与公司网络的连接状况！";
                bpfdtResult = false;
            }
            #endregion
        }
        void BPFOut()
        {
            try
            {
                if (Reader.Barcode == "")
                    Reader.Barcode = "NA";
                CallWithTimeout(GSBPFWebOut, 30000);
                if (sErrory != "")
                    MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                for (int dti = bpfWebMessage.Rows.Count - 1; dti >= 0; dti--)
                {
                    bpfWebMessage.Rows.RemoveAt(dti);
                }
                bpfWebMessage = new DataTable();
                bpfWebMessage.TableName = "arrLensBarcode";
                bpfWebMessage.Columns.Add("LensBarcode");
                bpfWebMessage.Columns.Add("X");
                bpfWebMessage.Columns.Add("Y");
                for (int di = 0; di < 30; di++)
                {
                    for (int dj = 0; dj < 30; dj++)
                    {
                        string rdbar = iniFile.Read("DBarcode", di.ToString() + "_" + dj.ToString(), Sys.IniPath + "\\Data.ini");
                        if (rdbar != "")
                            iniFile.Write("DBarcode", di.ToString() + "_" + dj.ToString(), "", Sys.IniPath + "\\Data.ini");
                    }
                }
            }
            catch
            {
                sErrory = "访问系统超时！";
                if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                    weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                weblog.log("Web报错:" + RiReader.Barcode + "Out," + sErrory);
                MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnCancle_Click(object sender, EventArgs e) //BPF非满盘时过账提示
        {
            gBWarm.SendToBack();
        }
        #endregion

        #region //光源设置
        bool getStatusFlag = true, getStatusFlag3 = true;
        byte[] status_byte, status_byte3; 

        public void LIGHT1Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ////启动辅助线程， 判断是否正常接收到参数
            byte[] input = new byte[LIGHT1.Com.BytesToRead];
            LIGHT1.Com.Read(input, 0, LIGHT1.Com.BytesToRead);
            //FormLoad接收光源参数
            if (getStatusFlag)
            {
                status_byte = input;
                getStatusFlag = false;
            }
        }
        public int lit = 0, brit = 10, ch = 0;
        public void GetStatus()//获取光源参数（状态)
        {
            int timeOut = 0;
            while (keepReading)
            {
                if (!quit && LIGHT1.Com.IsOpen)
                {
                    if (status_byte == null)
                    {
                        Thread.Sleep(10);
                        timeOut += 10;
                        if (timeOut > 1000)
                        {
                            lit++;
                            if (lit > 10)
                            {
                                lit = 0;
                                LIGHT1.IsConnected = false;
                                //MessageBox.Show("获取光源参数超时,请重新开启光源和程式！");
                                byte[] cmd = VarLighter.ReadAllPara();
                                LIGHT1.Com.ReceivedBytesThreshold = 18;//当接收到18个字节才触发DataReceived事件，以防数据被分成两次接收
                                LIGHT1.Com.Write(cmd, 0, cmd.Length);
                                getStatusFlag = true;
                            }
                            //break;
                        }
                        Thread.Sleep(10);
                    }
                    else
                    {
                        lit = 0;
                        LIGHT1.IsConnected = true;
                    }
                }
                else
                {
                    break;
                }
            }
            //重新设定com1的DataReceived事件触发的条件
            LIGHT1.Com.ReceivedBytesThreshold = 1;
        }
        public void ReverseOnOff(Button sender)//打开或关闭通道
        {
            bool flag = false;
            if (sender.Text == "打开")
            {
                flag = true;
                sender.Text = "关闭";
            }
            else
            {
                flag = false;
                sender.Text = "打开";
            }
            byte[] cmd = VarLighter.SetOnOff(0, flag);
            LIGHT1.Com.Write(cmd, 0, cmd.Length);
        }
        public void LightSet()
        {
            byte[] cmd = VarLighter.SetBrit(ch, brit);
            LIGHT1.Com.Write(cmd, 0, 8);
            Thread.Sleep(20);
        }

        public void LIGHT2Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ////启动辅助线程， 判断是否正常接收到参数
            byte[] input = new byte[LIGHT2.Com.BytesToRead];
            LIGHT2.Com.Read(input, 0, LIGHT2.Com.BytesToRead);
            //FormLoad接收光源参数
            if (getStatusFlag)
            {
                status_byte = input;
                getStatusFlag = false;
            }
        }
        public int lit2 = 0, brit2 = 10, ch2 = 0;
        public void GetStatus2()//获取光源参数（状态)
        {
            int timeOut = 0;
            while (keepReading)
            {
                if (!quit && LIGHT2.Com.IsOpen)
                {
                    if (status_byte == null)
                    {
                        Thread.Sleep(10);
                        timeOut += 10;
                        if (timeOut > 1000)
                        {
                            lit2++;
                            if (lit2 > 10)
                            {
                                lit2 = 0;
                                LIGHT2.IsConnected = false;
                                //MessageBox.Show("获取光源参数超时,请重新开启光源和程式！");
                                byte[] cmd = VarLighter.ReadAllPara();
                                LIGHT2.Com.ReceivedBytesThreshold = 18;//当接收到18个字节才触发DataReceived事件，以防数据被分成两次接收
                                LIGHT2.Com.Write(cmd, 0, cmd.Length);
                                getStatusFlag = true;
                            }
                            //break;
                        }
                        Thread.Sleep(10);
                    }
                    else
                    {
                        lit2 = 0;
                        LIGHT2.IsConnected = true;
                    }
                }
                else
                {
                    break;
                }
            }
            //重新设定com1的DataReceived事件触发的条件
            LIGHT2.Com.ReceivedBytesThreshold = 1;
        }
        public void LightSet2()
        {
            byte[] cmd = VarLighter.SetBrit(ch2, brit2);
            LIGHT2.Com.Write(cmd, 0, 8);
            Thread.Sleep(20);
        }

        public void LIGHT3Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ////启动辅助线程， 判断是否正常接收到参数
            byte[] input = new byte[LIGHT3.Com.BytesToRead];
            LIGHT2.Com.Read(input, 0, LIGHT3.Com.BytesToRead);
            //FormLoad接收光源参数
            if (getStatusFlag3)
            {
                status_byte3 = input;
                getStatusFlag3 = false;
            }
        }
        public int lit3 = 0, brit3 = 10, ch3 = 0;
        public void GetStatus3()//获取光源参数（状态)
        {
            int timeOut = 0;
            while (keepReading)
            {
                if (!quit && LIGHT3.Com.IsOpen)
                {
                    if (status_byte3 == null)
                    {
                        Thread.Sleep(10);
                        timeOut += 10;
                        if (timeOut > 1000)
                        {
                            lit3++;
                            if (lit3 > 10)
                            {
                                lit3 = 0;
                                LIGHT3.IsConnected = false;
                                //MessageBox.Show("获取光源参数超时,请重新开启光源和程式！");
                                byte[] cmd = VarLighter.ReadAllPara();
                                LIGHT3.Com.ReceivedBytesThreshold = 18;//当接收到18个字节才触发DataReceived事件，以防数据被分成两次接收
                                LIGHT3.Com.Write(cmd, 0, cmd.Length);
                                getStatusFlag3 = true;
                            }
                            //break;
                        }
                        Thread.Sleep(10);
                    }
                    else
                    {
                        lit3 = 0;
                        LIGHT3.IsConnected = true;
                    }
                }
                else
                {
                    break;
                }
            }
            //重新设定com1的DataReceived事件触发的条件
            LIGHT3.Com.ReceivedBytesThreshold = 1;
        }
        public void LightSet3()
        {
            byte[] cmd = VarLighter.SetBrit(ch3, brit3);
            LIGHT3.Com.Write(cmd, 0, 8);
            Thread.Sleep(20);
        }
        #endregion 

        #region//PLC通讯
        #region 变量
        string cmdstatus = "", cmdstatusL = ""; string Curts = "";
        string PowerSupplyTime_Days;
        string PowerSupplyTime_Hours, PowerSupplyTime_Minutes, PowerSupplyTime_Seconds;
        string AutoRunningTime_Hours, AutoRunningTime_Minutes, AutoRunningTime_Seconds;
        string AlarmPauseTime_Hours, AlarmPauseTime_Minutes, AlarmPauseTime_Seconds;
        string WaitTime_Hours, WaitTime_Minutes, WaitTime_Seconds, AssemblyHeight;
        string y1, y2, x1, x2; string Tray_XY, Tray_XYLast;
        string[] pathIP1 = new string[] { };
        public static string[,] strD1 = new string[30, 30], strDmin = new string[30, 30], strRlt = new string[30, 30];
        public static string[,] strresult = new string[30, 30], strDistance = new string[30, 30], strDistance1 = new string[30, 30], strDistance2 = new string[30, 30];
        Int32 ProductCount = 0; Int32 tempVar;
        Int32 AlarmCount, CycleTime; double NumAddLog = 0.0;
        public static Int32 LensCount = 0, LensCountL = 0; // MacS = 10, MacSL = 10;
        public static int t3x = 0, t3y = 0, curt3x = 0, curt3y = 0;
        bool LogTriRead = false;
        bool readCurT = false, readtemporary = false;
        bool[] Mchstatus = new bool[16]; 
        bool Isrun = false, IsIdle = false; 
        bool Mchstatus3 = false; 
        DataTable bpfWebMessage = new DataTable();
        #endregion
        #region AlarmMessage
        int alnum = 0, alnumL = 0;
        string[] Alarray = new string[] {"A0016_Axis_AC1_Servo_Alarm","A0015_Axis_AX1_Servo_Alarm",
                                         "A0014_Axis_PX1_Servo_Alarm","A0013_Axis_AZ0_Servo_Alarm",
                                         "A0012_Axis_PZ0_Servo_Alarm","A0011_Axis_X2_Servo_Alarm", 
                                         "A0010_Axis_X1_Servo_Alarm", "A0009_Axis_GZ_Servo_Alarm",
                                         "A0008_Axis_PZ1_Servo_Alarm","A0007_Axis_AZ2_Servo_Alarm",
                                         "A0006_Axis_AZ1_Servo_Alarm","A0005_Axis_GY_Servo_Alarm",
                                         "A0004_Axis_QY_Servo_Alarm", "A0003_Axis_PY_Servo_Alarm", 
                                         "A0002_Axis_AY2_Servo_Alarm","A0001_Axis_AY1_Servo_Alarm",
                                         //D353
                                         "A0032","A0031","A0030","A0029","A0028","A0027",
                                         "A0026_Axis_AX2_Sensor_Alarm","A0025_Axis_PX2_Sensor_Alarm",
                                         "A0024_Axis_AX2_Servo_Alarm","A0023_Axis_PX2_Servo_Alarm",
                                         "A0022_Axis_C2_Servo_Alarm", "A0021_Axis_C1_Servo_Alarm",
                                         "A0020_Axis_GC_Servo_Alarm", "A0019_Axis_PC2_Servo_Alarm",
                                         "A0018_Axis_PC1_Servo_Alarm","A0017_Axis_AC2_Servo_Alarm",
                                         //D354
                                         "A0048_Axis_PC2_LocateModule_Alarm","A0047_Axis_PC1_LocateModule_Alarm",
                                         "A0046_Axis_AC2_LocateModule_Alarm","A0045_Axis_AC1_LocateModule_Alarm",
                                         "A0044_Axis_AX1_LocateModule_Alarm","A0043_Axis_AZ0_LocateModule_Alarm",
                                         "A0042_Axis_PX1_LocateModule_Alarm","A0041_Axis_PZ0_LocateModule_Alarm",
                                         "A0040_Axis_GZ_LocateModule_Alarm", "A0039_Axis_PZ1_LocateModule_Alarm",
                                         "A0038_Axis_AZ2_LocateModule_Alarm","A0037_Axis_AZ1_LocateModule_Alarm",
                                         "A0036_Axis_QY_LocateModule_Alarm", "A0035_Axis_PY_LocateModule_Alarm", 
                                         "A0034_Axis_AY2_LocateModule_Alarm","A0033_Axis_AY1_LocateModule_Alarm",
                                         //D355
                                         "A0064","A0063","A0062","A0061",
                                         "A0060_Axis_GC_LocateModule_Alarm", "A0059_Axis_X2_LocateModule_Alarm", 
                                         "A0058_Axis_X1_LocateModule_Alarm","A0057_Axis_GY_LocateModule_Alarm",
                                         "A0056","A0055","A0054","A0053",
                                         "A0052_Axis_AX2_LocateModule_Alarm", "A0051_Axis_PX2_LocateModule_Alarm", 
                                         "A0050_Axis_C2_LocateModule_Alarm","A0049_Axis_C1_LocateModule_Alarm",
                                         //D356
                                         "A0080_Axis_X1_OverPositiveLimit", "A0079_Axis_X1_OverNegativeLimit", 
                                         "A0078_Axis_GZ_OverPositiveLimit", "A0077_Axis_GZ_OverNegativeLimit",
                                         "A0076_Axis_AZ2_OverPositiveLimit","A0075_Axis_AZ2_OverNegativeLimit",
                                         "A0074_Axis_AZ1_OverPositiveLimit","A0073_Axis_AZ1_OverNegativeLimit",
                                         "A0072_Axis_QY_OverPositiveLimit", "A0071_Axis_QY_OverNegativeLimit", 
                                         "A0070_Axis_PY_OverPositiveLimit", "A0069_Axis_PY_OverNegativeLimit",
                                         "A0068_Axis_AY2_OverPositiveLimit","A0067_Axis_AY2_OverNegativeLimit",
                                         "A0066_Axis_AY1_OverPositiveLimit","A0065_Axis_AY1_OverNegativeLimit",
                                         //D357
                                         "A0096_Axis_PZ1_OverPositiveLimit","A0095_Axis_PZ1_OverNegativeLimit",
                                         "A0094_Axis_AX2_OverPositiveLimit","A0093_Axis_AX2_OverNegativeLimit",
                                         "A0092_Axis_AX1_OverPositiveLimit","A0091_Axis_AX1_OverNegativeLimit",
                                         "A0090_Axis_PX2_OverPositiveLimit","A0089_Axis_PX2_OverNegativeLimit",
                                         "A0088_Axis_PX1_OverPositiveLimit","A0087_Axis_PX1_OverNegativeLimit",
                                         "A0086_Axis_AZ0_OverPositiveLimit","A0085_Axis_AZ0_OverNegativeLimit",
                                         "A0084_Axis_PZ0_OverPositiveLimit","A0083_Axis_PZ0_OverNegativeLimit",
                                         "A0082_Axis_X2_OverPositiveLimit", "A0081_Axis_X2_OverNegativeLimit",
                                         //D358
                                         "A0112","A0111","A0110","A0109","A0108","A0107","A0106","A0105",
                                         "A0104","A0103","A0102","A0101","A0100","A0099","A0098","A0097",
                                         //D359
                                         "A0128_UVLight_2_PowerErr","A0127_UVLight_1_PowerErr",
                                         "A0126_ChangingPlate_Abnormal_Alarm(Notice)","A0125_AirPressure_Alarm",
                                         "A0124_Glue_PlatformAbnormal_Alarm","A0123_CoverNozzle_2_UpDown_Alarm",
                                         "A0122_CoverNozzle_1_UpDown_Alarm", "A0121_GT2_UpDown_Alarm",
                                         "A0120_Clamp_4_UpDown_Alarm",       "A0119_Clamp_3_UpDown_Alarm",
                                         "A0118_Clamp_2_UpDown_Alarm",       "A0117_Nozzle_2_UpDown_Alarm",
                                         "A0116_Clamp_1_UpDown_Alarm",       "A0115_Nozzle_1_UpDown_Alarm",
                                         "A0114_Clamp_PZ0_Abnormal",         "A0113_Clamp_AZ0_Abnormal",
                                         //D360
                                         "A0144_RightDoor_Alarm",                  "A0143_LeftDoor_Alarm",
                                         "A0142_BackDoor_Alarm",                   "A0141_OverDoor_Alarm",
                                         "A0140",                                  "A0180_Storage_Fully_PZ0_Alarm",
                                         "A0179_Storage_Fully_AZ0_Alarm",               "A0137_Nozzle_4_Absorb_Alarm",
                                         "A0136_Platform_2_Absorb_Alarm",          "A0135_Platform_1_Absorb_Alarm",
                                         "A0134_Nozzle_2_Residuum_Alarm",          "A0133_Nozzle_1_Residuum_Alarm",
                                         "A0132_Platform_2_ResidualMaterial_Alarm","A0131_Platform_1_ResidualMaterial_Alarm",
                                         "A0130_Nozzle_2_Absorb_Alarm",            "A0129_Nozzle_1_Absorb_Alarm",
                                         //D361
                                         "A0160","A0159_QCCDPlatform_Grabimage_NG_Alarm",
                                         "A0158_PCCD1_Grabimage_NG_Alarm", "A0157_Platform_2_Height_Alarm",
                                         "A0156_Platform_1_Height_Alarm",  "A0155_GCCD2_GlueAVI_NG_Alarm",
                                         "A0154_PCCD2Platform_Grabimage_NG_Alarm", "A0153_GCCD1_GrabNeedleimage_NG_Alarm",
                                         "A0152_PCCD2Tray_Grabimage_NG_Alarm",     "A0151_GCCD2Platform_Grabimage_NG_Alarm",
                                         "A0150_A2CCD2Platform_Grabimage_NG_Alarm","A0149_A2CCD2Tray_Grabimage_NG_Alarm",
                                         "A0148_A2CCD1_Grabimage_NG_Alarm",        "A0147_A1CCD2Platform_Grabimage_NG_Alarm",
                                         "A0146_A1CCD2Tray_Grabimage_NG_Alarm",    "A0145_A1CCD1_Grabimage_NG_Alarm",
                                         //D362
                                         "A0176","A0175","A0174_Tray_AZ0_Into_The_Station_Error_Alarm","A0173_Tray_PZ0_Into_The_Station_Error_Alarm",
                                         "A0172_Tray_AZ0_Match_Alarm",        "A0171_Tray_AZ0_Cleaning_TimeOut_Alarm",
                                         "A0170_Tray_PZ0_Match_Alarm",        "A0169_Tray_PZ0_Cleaning_TimeOut_Alarm",
                                         "A0168_ExpiredGlue",                 "A0167_Tray_AZ0_BarcodeTrigger_NG_Alarm",
                                         "A0166_Tray_PZ0_BarcodeTrigger_NG_Alarm","A0165_Product_BarcodeReader_NG_Alarm",
                                         "A0164_SuctionNozzle_A2_BarcodeReader_NG_Alarm","A0163_SuctionNozzle_A1_BarcodeReader_NG_Alarm",
                                         "A0162_Tray_AZ0_PC_BarcodeReader_NG_Alarm", "A0161_Tray_PZ0_PC_BarcodeReader_NG_Alarm",
                                         //D363
                                         "","","","","","",
                                         "","","","", "","",
                                         "",
                                         "",
                                         "A0178_Storage_Empty_PZ0_Alarm",
                                         "A0177_Storage_Empty_AZ0_Alarm",
                                         //D364
                                         "A0208_Axis_GZ_OverPositiveSoftLimit","A0207_Axis_GZ_OverNegativeSoftLimit","A0206_Axis_PZ1_OverPositiveSoftLimit","A0205_Axis_PZ1_OverNegativeSoftLimit",
                                         "A0204_Axis_AZ2_OverPositiveSoftLimit","A0203_Axis_AZ2_OverNegativeSoftLimit","A0202_Axis_AZ1_OverPositiveSoftLimit","A0201_Axis_AZ1_OverNegativeSoftLimit",
                                         "A0200_Axis_QY_OverPositiveSoftLimit","A0199_Axis_Qy_OverNegativeSoftLimit","A0198_Axis_PY_OverPositiveSoftLimit","A0197_Axis_PY_OverNegativeSoftLimit",
                                         "A0196_Axis_AY2_OverPositiveSoftLimit","A0195_Axis_AY2_OverNegativeSoftLimit","A0194_Axis_AY1_OverNegativeSoftLimit","A0193_Axis_AY1_OverNegativeSoftLimit",
                                         //D365
                                         "A0224_Axis_X1_OverPositiveSoftLimit","A0223_Axis_X1_OverNegativeSoftLimit","A0222_Axis_GY_OverPositiveSoftLimit","A0221_Axis_GY_OverNegativeSoftLimit",
                                         "A0220_Axis_AX2_OverPositiveSoftLimit","A0219_Axis_AX2_OverNegativeSoftLimit","A0218_Axis_PX2_OverPositiveSoftLimit","A0217_Axis_PX2_OverNegativeSoftLimit",
                                         "A0216_Axis_AX1_OverPositiveSoftLimit","A0215_Axis_AX1_OverNegativeSoftLimit","A0214_Axis_AZ0_OverPositiveSoftLimit","A0213_Axis_AZ0_OverNegativeSoftLimit",
                                         "A0212_Axis_PX1_OverPositiveSoftLimit","A0211_Axis_PX1_OverNegativeSoftLimit","A0210_Axis_PZ0_OverPositiveSoftLimit","A0209_Axis_PZ0_OverNegativeSoftLimit",
                                         //D366
                                         "","","","",
                                         "","","","",
                                         "","","","",
                                         "","","A0226_Axis_X2_OverPositiveSoftLimit","A0225_Axis_X2_OverNegativeSoftLimit"
                                         };
        string[] AlarrayCh = new string[] {"A0016_AC1伺服异常报警","A0015_AX1伺服异常报警","A0014_PX1伺服异常报警","A0013_AZ0伺服异常报警",
                                         "A0012_PZ0伺服异常报警","A0011_X2伺服异常报警", "A0010_X1伺服异常报警", "A0009_GZ伺服异常报警",
                                         "A0008_PZ1伺服异常报警","A0007_AZ2伺服异常报警","A0006_AZ1伺服异常报警","A0005_GY伺服异常报警",
                                         "A0004_QY伺服异常报警", "A0003_PY伺服异常报警", "A0002_AY2伺服异常报警","A0001_AY1伺服异常报警",
                                         //D353
                                         "A0032","A0031","A0030","A0029","A0028","A0027","A0026_AX2传感器异常需回原点","A0025_PX2传感器异常需回原点",
                                         "A0024_AX2伺服异常报警","A0023_PX2伺服异常报警","A0022_C2伺服异常报警", "A0021_C1伺服异常报警",
                                         "A0020_GC伺服异常报警", "A0019_PC2伺服异常报警","A0018_PC1伺服异常报警","A0017_AC2伺服异常报警",
                                         //D354
                                         "A0048_PC2定位模块报警","A0047_PC1定位模块报警","A0046_AC2定位模块报警","A0045_AC1定位模块报警",
                                         "A0044_AX1定位模块报警","A0043_AZ0定位模块报警","A0042_PX1定位模块报警","A0041_PZ0定位模块报警",
                                         "A0040_GZ定位模块报警", "A0039_PZ1定位模块报警","A0038_AZ2定位模块报警","A0037_AZ1定位模块报警",
                                         "A0036_QY定位模块报警", "A0035_PY定位模块报警", "A0034_AY2定位模块报警","A0033_AY1定位模块报警",
                                         //D355
                                         "A0064","A0063","A0062","A0061",
                                         "A0060_GC定位模块报警", "A0059_X2定位模块报警","A0058_X1定位模块报警","A0057_GY定位模块报警",
                                         "A0056","A0055","A0054","A0053",
                                         "A0052_AX2定位模块报警", "A0051_PX2定位模块报警","A0050_C2定位模块报警","A0049_C1定位模块报警",
                                         //D356
                                         "A0080_X1正极限传感器报警", "A0079_X1负极限传感器报警", "A0078_GZ正极限传感器报警", "A0077_GZ负极限传感器报警",
                                         "A0076_AZ2正极限传感器报警","A0075_AZ2负极限传感器报警","A0074_AZ1正极限传感器报警","A0073_AZ1负极限传感器报警",
                                         "A0072_QY正极限传感器报警", "A0071_QY负极限传感器报警", "A0070_PY正极限传感器报警", "A0069_PY负极限传感器报警",
                                         "A0068_AY2正极限传感器报警","A0067_AY2负极限传感器报警","A0066_AY1正极限传感器报警","A0065_AY1负极限传感器报警",
                                         //D357
                                         "A0096_PZ1正极限传感器报警","A0095_PZ1负极限传感器报警","A0094_AX2正极限传感器报警","A0093_AX2负极限传感器报警",
                                         "A0092_AX1正极限传感器报警","A0091_AX1负极限传感器报警","A0090_PX2正极限传感器报警","A0089_PX2负极限传感器报警",
                                         "A0088_PX1正极限传感器报警","A0087_PX1负极限传感器报警","A0086_AZ0正极限传感器报警","A0085_AZ0负极限传感器报警",
                                         "A0084_PZ0正极限传感器报警","A0083_PZ0负极限传感器报警","A0082_X2正极限传感器报警", "A0081_X2负极限传感器报警",
                                         //D358
                                         "A0112","A0111","A0110","A0109","A0108","A0107","A0106","A0105",
                                         "A0104","A0103","A0102","A0101","A0100","A0099","A0098","A0097",
                                         //D359
                                         "A0128_UV光源2上电异常","A0127_UV光源1上电异常",
                                         "A0126_换板异常(注意)","A0125_气源气压报警","A0124_点胶平台气缸报警","A0123_压覆2上下异常",
                                         "A0122_压覆1上下异常","A0121_测高上下异常","A0120_夹子4上下异常","A0119_夹子3上下异常",
                                         "A0118_夹子2上下异常","A0117_吸嘴2上下异常","A0116_夹子1上下异常","A0115_吸嘴1上下异常",
                                         "A0114_PZ0夹子异常","A0113_AZ0夹子异常",
                                         //D360
                                         "A0144_右安全门被开启","A0143_左安全门被开启","A0142_后安全门被开启","A0141_前安全门被开启",
                                         "A0140",
                                         "A0180_PZ0满仓报警","A0179_AZ0满仓报警",
                                         "A0137_吸嘴4吸料异常","A0136_平台2吸料异常","A0135_平台1吸料异常",
                                         "A0134_吸嘴2残料异常","A0133_吸嘴1残料异常","A0132_平台2残料异常","A0131_平台1残料异常",
                                         "A0130_吸嘴2吸料异常","A0129_吸嘴1吸料异常",
                                         //D361
                                         "A0160","A0159_QCCD平台取像NG",
                                         "A0158_PCCD1取像NG","A0157_平台2测高异常",
                                         "A0156_平台1测高异常","A0155_GCCD2胶点辨识NG",
                                         "A0154_PCCD2平台取像NG","A0153_GCCD1针尖取像NG",
                                         "A0152_PCCD2Tray盘取像NG","A0151_GCCD2平台取像NG","A0150_A2CCD2平台取像NG","A0149_A2CCD2Tray盘取像NG",
                                         "A0148_A2CCD1取像NG","A0147_A1CCD2平台取像NG","A0146_A1CCD2Tray盘取像NG","A0145_A1CCD1取像NG",
                                         //D362
                                         "A0176","A0175","A0174_AZ0-TRAY进站出错","A0173_PZ0-TRAY进站出错",
                                         "A0172_AZ0-TRAY配对错误","A0171_AZO-TRAY清洗超时报警",
                                         "A0170_PZ0-TRAY配对错误","A0169_PZO-TRAY清洗超时报警",
                                         "A0168_胶水计数总数报警","A0167_AZ0-Tray-读码器-读码NG","A0166_PZ0-Tray-读码器-读码NG","A0165_成品读码NG",
                                         "A0164_A2读码NG","A0163_A1读码NG","A0162_AZ0-Tray-PC读码NG","A0161_PZ0-Tray-PC读码NG",
                                         //D363
                                         "","","","","","",
                                         "","","", "","","",
                                         "",
                                         "",
                                         "A0178_PZ0空仓报警",
                                         "A0177_AZ0空仓报警",
                                         //D364
                                         "A0208_GZ正软极限警报","A0207_GZ负软极限警报","A0206_PZ1正软极限警报","A0205_PZ1负软极限警报",
                                         "A0204_AZ2正软极限警报","A0203_AZ2负软极限警报","A0202_AZ1正软极限警报","A0201_AZ1负软极限警报",
                                         "A0200_QY正软极限警报","A0199_QY负软极限警报","A0198_PY正软极限警报","A0197_PY负软极限警报",
                                         "A0196_AY2正软极限警报","A0195_AY2负软极限警报","A0194_AY1正软极限警报","A0193_AY1负软极限警报",
                                         //D365
                                         "A0224_X1正软极限警报","A0223_X1负软极限警报","A0222_GY正软极限警报","A0221_GY负软极限警报",
                                         "A0220_AX2正软极限警报","A0219_AX2负软极限警报","A0218_PX2正软极限警报","A0217_PX2负软极限警报",
                                         "A0216_AX1正软极限警报","A0215_AX1负软极限警报","A0214_AZ0正软极限警报","A0213_AZ0负软极限警报",
                                         "A0212_PX1正软极限警报","A0211_PX1负软极限警报","A0210_PZ0正软极限警报","A0209_PZ0负软极限警报",
                                         //D366
                                         "","","","",
                                         "","","","",
                                         "","","","",
                                         "","","A0226_X2正软极限警报","A0225_X2负软极限警报"
                                         };
        #endregion
        #region OEEtableMessage
        string[] OEEarray = new string[] { "O3-首次换料", "O4-换料", "O5-换胶",
                                           "C1-设备异常首件","C2-制程异常首件","C4-换料首件","C5-开机首件","B6-保养首件","C7-称胶重",
                                           "E2-制程异常","E3-物料异常","E4-待料","D1-切换机种","E1-设备故障",
                                           "B1-日保养","B3-月保养","B4-季保养","F1-制程DOE","F3-委托物料","E5-厂务异常"}; //20
        #endregion
        void ConnectToPlc()
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(PLC.Ip, PLC.Port);
                PLC.socket.Connect(ipe);
                PLC.IsConnected = true;
            }
            catch
            {
                PLC.IsConnected = false;
                PLC.socket.Close();
            }
        }
        private void CommucationPlc_DoWork(object sender, DoWorkEventArgs e)
        {
            string add = "000000000000000000000000";
            while (true)
            {
                if (CommucationPlc.CancellationPending)
                    return;
                if (!PLC.IsConnected)
                {
                    Thread.Sleep(10);
                    continue;
                }
                //PLC有連上,就一直給1(避免PLC不知PC有連上)
                if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD00198,01,0001\r\n")))
                    return;
                if (SocketReceive(6) == "")
                    return;
                // PLC&人機版本
                if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD00990,04\r\n")))
                    return;
                string buffer1 = SocketReceive(22);
                if (buffer1 == "")
                    return;
                ResolveData(buffer1, DataType.PLCHMI_Versions);
                #region Tray整盤點數
                if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD00214,01\r\n")))
                    return;
                buffer1 = SocketReceive(10);
                if (buffer1 == "")
                    return;
                ResolveData(buffer1, DataType.PlateCount);
                #endregion
                #region
                if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD10024,12\r\n")))
                    return;
                buffer1 = SocketReceive(54);
                if (buffer1 == "")
                    return;
                ResolveData(buffer1, DataType.TrayMaxXY);
                #endregion
                #region 點膠次數
                if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD05584,04\r\n")))//點膠最大次數
                    return;
                buffer1 = SocketReceive(22);
                if (buffer1 == "")
                    return;
                ResolveData(buffer1, DataType.GlueCount);

                #endregion
                #region VisionResult
                string cmd = "";
                #region a1ccd1
                if (WriteToPlc.CMDsend[0])
                {
                    WriteToPlc.CMDsend[0] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[0];
                    if (!Sys.NoAutoMatic)
                        cmd = A1CCD1.sendaddress + WriteToPlc.CMDOKNG[0] + WriteToPlc.CMDresult[0] + "\r\n";
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    A1CCD1.GrabSignal = true;
                }
                #endregion 
                #region a1ccd2
                if (WriteToPlc.CMDsend[1])
                {
                    WriteToPlc.CMDsend[1] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[1];
                    if (!Sys.NoAutoMatic & A1CCD2.IntSingle == 1)
                    {
                        cmd = "01WWRD00010,08,0000" + WriteToPlc.CMDOKNG[1] + WriteToPlc.CMDresult[1] + "\r\n";
                        add = WriteToPlc.CMDresult[1];
                    }
                    else if (!Sys.NoAutoMatic & A1CCD2.IntSingle == 2)
                    {
                        cmd = "01WWRD00010,14,0000" + WriteToPlc.CMDOKNG[1] + add + WriteToPlc.CMDresult[1] + "\r\n";
                        add = "000000000000000000000000";
                    }
                    else
                    {

                    }
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    A1CCD2.GrabSignal = true;
                }
                #endregion
                #region a2ccd1
                if (WriteToPlc.CMDsend[2])
                {
                    WriteToPlc.CMDsend[2] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[2];
                    if (!Sys.NoAutoMatic)
                        cmd = A2CCD1.sendaddress + WriteToPlc.CMDOKNG[2] + WriteToPlc.CMDresult[2] + "\r\n";
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    A2CCD1.GrabSignal = true;
                }
                #endregion
                #region a2ccd2
                if (WriteToPlc.CMDsend[3])
                {
                    WriteToPlc.CMDsend[3] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[3];
                    if (!Sys.NoAutoMatic & A2CCD2.IntSingle == 1)
                    {
                        cmd = "01WWRD00032,08,0000" + WriteToPlc.CMDOKNG[3] + WriteToPlc.CMDresult[3] + "\r\n";
                        add = WriteToPlc.CMDresult[3];
                    }
                    if (!Sys.NoAutoMatic & A2CCD2.IntSingle == 2)
                    {
                        cmd = "01WWRD00032,14,0000" + WriteToPlc.CMDOKNG[3] + add + WriteToPlc.CMDresult[3] + "\r\n";
                        add = "000000000000000000000000";
                    }
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    A2CCD2.GrabSignal = true;
                }
                #endregion 
                #region pccd1
                if (WriteToPlc.CMDsend[4])
                {
                    WriteToPlc.CMDsend[4] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[4];
                    if (!Sys.NoAutoMatic)
                        cmd = PCCD1.sendaddress + WriteToPlc.CMDOKNG[4] + WriteToPlc.CMDresult[4] + "\r\n";
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    PCCD1.GrabSignal = true;
                }
                #endregion
                #region pccd2
                if (WriteToPlc.CMDsend[5])
                {
                    WriteToPlc.CMDsend[5] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[5];
                    if (!Sys.NoAutoMatic & PCCD2.IntSingle == 1)
                    {
                        cmd = "01WWRD00054,08,0000" + WriteToPlc.CMDOKNG[5] + WriteToPlc.CMDresult[5] + "\r\n";
                        add = WriteToPlc.CMDresult[5];
                    }
                    if (!Sys.NoAutoMatic & (PCCD2.IntSingle == 2 || PCCD2.IntSingle == 3))
                    {
                        cmd = "01WWRD00054,14,0000" + WriteToPlc.CMDOKNG[5] + add + WriteToPlc.CMDresult[5] + "\r\n";
                        add = "000000000000000000000000";
                    }
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    PCCD2.GrabSignal = true;
                }
                #endregion
                #region gccd1
                if (WriteToPlc.CMDsend[6])
                {
                    WriteToPlc.CMDsend[6] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[6];
                    if (!Sys.NoAutoMatic)
                        cmd = GCCD1.sendaddress + WriteToPlc.CMDOKNG[6] + WriteToPlc.CMDresult[6] + "\r\n";
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    GCCD1.GrabSignal = true;
                }
                #endregion
                #region gccd2
                if (WriteToPlc.CMDsend[7])
                {
                    WriteToPlc.CMDsend[7] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[7];
                    if (!Sys.NoAutoMatic)
                        cmd = GCCD2.sendaddress + WriteToPlc.CMDOKNG[7] + WriteToPlc.CMDresult[7] + "\r\n";
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    if (GCCD2.Deg4Checked)
                    {
                        if (Sys.NoAutoMatic)
                            cmd = WriteToPlc.CMDresult4;
                        if (!Sys.NoAutoMatic)
                            cmd = "01WWRD0500,16," + WriteToPlc.CMDresult4 + "\r\n";
                        if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                            return;
                        if (SocketReceive(6) == "")
                            return;
                        //readtemporary = true;
                    }
                    GCCD2.GrabSignal = true;
                }
                #endregion
                #region qccd
                if (WriteToPlc.CMDsend[8])
                {
                    WriteToPlc.CMDsend[8] = false;
                    if (Sys.NoAutoMatic)
                        cmd = WriteToPlc.CMDresult[8];
                    if (!Sys.NoAutoMatic)
                        cmd = QCCD.sendaddress + WriteToPlc.CMDOKNG[8] + WriteToPlc.CMDresult[8] + "\r\n";
                    if (!SocketSend(Encoding.ASCII.GetBytes(cmd)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    QCCD.GrabSignal = true;
                }
                #endregion
                #endregion
                #region AlwaysRead
                if (!Sys.NoAutoMatic)
                {
                    #region a11
                    if (A1CCD1.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewA1CCD1))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input1);
                    }
                    #endregion
                    #region a12
                    if (A1CCD2.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewA1CCD2))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input2);
                    }
                    #endregion
                    #region a21
                    if (A2CCD1.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewA2CCD1))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input3);
                    }
                    #endregion
                    #region a22
                    if (A2CCD2.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewA2CCD2))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input4);
                    }
                    #endregion
                    #region p1
                    if (PCCD1.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewPCCD1))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input5);
                    }
                    #endregion
                    #region p2
                    if (PCCD2.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewPCCD2))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input6);
                    }
                    #endregion
                    #region g1
                    if (GCCD1.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewGCCD1))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input7);
                    }
                    #endregion
                    #region g2
                    if (GCCD2.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewGCCD2))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input8);
                    }
                    #endregion
                    #region q
                    if (QCCD.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewQCCD))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input9);
                    }
                    #endregion
                    #region 获取记录标记

                    //if (!SocketSend(Protocol.GetLogMesSign))
                    //    return;
                    //buffer1 = SocketReceive(38);
                    //if (buffer1 == "")
                    //    return;
                    //ResolveData(buffer1, DataType.LogMsgSign);

                     #endregion
                    #region Output
                    if (!SocketSend(Protocol.GetReader))
                        return;
                    buffer1 = SocketReceive(50);
                    if (buffer1 == "")
                        return;
                    ResolveData(buffer1, DataType.Output);
                    #endregion
                    #region LogTrigger
                    if (!SocketSend(Protocol.GetLogTri))
                        return;
                    buffer1 = SocketReceive(10);
                    if (buffer1 == "")
                        return;
                    ResolveData(buffer1, DataType.LogTrigger);
                    #endregion
                    #region GetErrTri
                    if (!SocketSend(Protocol.GetErrTri))
                        return;
                    buffer1 = SocketReceive(10);
                    if (buffer1 == "")
                        return;
                    ResolveData(buffer1, DataType.ErrTrigger);
                    #endregion
                    
                }
                else
                {
                    if (GCCD2.IsCheck)
                    {
                        if (!SocketSend(Protocol.GetViewGCCD2))
                            return;
                        string buffer = SocketReceive(10);
                        if (buffer == "")
                            return;
                        ResolveData(buffer, DataType.Input8);
                    }
                }
                if (WriteToPlc.CMDRsend)
                {
                    WriteToPlc.CMDRsend = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes(WriteToPlc.CMDRead)))
                        return;
                    string buffer11 = SocketReceive(14);
                    if (buffer11 == "")
                        return;
                    ResolveData(buffer11, DataType.readLocation);
                }
                if (Sys.bCalView)
                {
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD00160,01\r\n")))
                        return;
                    string buffer11 = SocketReceive(10);
                    if (buffer11 == "")
                        return;
                    ResolveData(buffer11, DataType.pianbai);
                }
               
                #endregion
                #region  Cancel
                if (PLC.Tray1end)
                {
                    PLC.Tray1end = false;

                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0301,01,0000\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                if (cancelTrigger)
                {
                    cancelTrigger = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0300,01,0001\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                #endregion
                #region curTsta
                if (readCurT)
                {
                    readCurT = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD00213,01\r\n")))
                        return;
                    string buffer = SocketReceive(10);
                    if (buffer == "")
                        return;
                    string statusP = buffer.Substring(4, 4);
                    double tray_xy = Convert.ToInt32(statusP, 16);  //D213
                    curt3x = (int)Math.Round(tray_xy / 100);
                    curt3y = (int)(tray_xy % 100);
                }
                #endregion
                #region PCReady
                if (Protocol.IsPCRead)
                {
                    Protocol.IsPCRead = false;
                    Protocol.strPCRead = 
                        "0"+//7位
                        (Protocol.strPCRead_GlueUpdate ? "1" : "0") + //6位
                        "0" + //5位
                        "0" +//4位
                        "0" +//3位
                        "0" + //2位
                        (Protocol.strPCRead_GlueTimeOut ? "1" : "0") + //1位
                        (Protocol.strPCRead_PCReady ? "1" : "0");//0位
                    if (Protocol.strPCRead_GlueUpdate)
                        Protocol.strPCRead_GlueUpdate = false;
                    if (Protocol.strPCRead_GlueTimeOut)
                        Protocol.strPCRead_GlueTimeOut = false;
                    if (Protocol.strPCRead_PCReady)
                        Protocol.strPCRead_PCReady = false;
                    Protocol.strPCRead = Convert.ToInt64(Protocol.strPCRead, 2).ToString("D4");//2進制轉10進制
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0001,01," + Protocol.strPCRead + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                if (Protocol.IsloopRead)
                {
                    Protocol.IsloopRead = false;
                    if (!SocketSend(Protocol.GetloopTri))
                        return;
                    string buffer = SocketReceive(10);
                    if (buffer == "")
                        return;
                    ResolveData(buffer, DataType.loop);
                }
                #endregion
                #region TrayReady & 壓覆點膠進出站
                if (Reader.IsTrRead)//左工位
                {
                    Reader.IsTrRead = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0131,01," + Reader.strTrRead + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    //Lr = false;
                }
                if (RiReader.IsTrRead)//右工位
                {
                    RiReader.IsTrRead = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0133,01," + RiReader.strTrRead + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    //Rr = false;
                }
                if (RiReader.Web_Tray_InOutStation_Complete)
                {
                    RiReader.Web_Tray_InOutStation_Complete = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0368,01," + RiReader.Web_Tray_InOutStation_PlcResult + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                #endregion
                
                #region ConStatus
                if (cmdstatus != cmdstatusL)
                {
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD00199,01," + cmdstatus + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    cmdstatusL = cmdstatus;
                }
                #endregion
                #region SendBarcodeResult
                if (sendBar)
                {
                    sendBar = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD00129,01,000" + Barcresult + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                #endregion
                #region DL-EN1 Hight
                if (WriteToPlc.bCmdGT2[0])
                {
                    WriteToPlc.bCmdGT2[0] = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0180,02," + WriteToPlc.strCmdGT2[0] + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                if (WriteToPlc.bCmdGT2[1])
                {
                    WriteToPlc.bCmdGT2[1] = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0182,02," + WriteToPlc.strCmdGT2[1] + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                if (WriteToPlc.bCmdGT2[2])
                {
                    WriteToPlc.bCmdGT2[2] = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0184,02," + WriteToPlc.strCmdGT2[2] + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                if (WriteToPlc.bCmdGT2[3])
                {
                    WriteToPlc.bCmdGT2[3] = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0186,02," + WriteToPlc.strCmdGT2[3] + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                if (WriteToPlc.bCmdGT2[4])
                {
                    WriteToPlc.bCmdGT2[4] = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WWRD0188,02," + WriteToPlc.strCmdGT2[4] + "\r\n")))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                }
                #endregion
                
                #region ReadLog
                if (PLC.LogTrigger)
                {
                    PLC.LogTrigger = false;
                    if (!SocketSend(Protocol.GetLogMes))
                        return;
                    string buffer = SocketReceive(98);
                    if (buffer == "")
                        return;
                    Logger(buffer);
                }
                #endregion
                #region ReadErr(讀錯誤代碼)
                //readcircle++;
                
                if (!SocketSend(Protocol.GetErrMes))
                    return;
                Protocol.ErrMes = SocketReceive(66);  //6+60
                if (Protocol.ErrMes == "")
                    return;
                   
               
                #endregion
                
                #region 偏摆校正
                if (WriteToPlc.CMDchecksend)
                {
                    WriteToPlc.CMDchecksend = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes(WriteToPlc.CMDcheckR)))
                        return;
                    if (SocketReceive(6) == "")
                        return;
                    Sys.CalViewPross = false;
                }
                #endregion
                #region jiancekakong
                if (Monitor.IsRead)
                {
                    Monitor.IsRead = false;
                    if (!SocketSend(Monitor.GetRead))
                        return;
                    string buffer = SocketReceive(10);
                    if (buffer == "")
                        return;
                    ResolveData(buffer, DataType.clince);
                }
                #endregion
                #region weiyong 4dian
                if (readtemporary)
                {
                    readtemporary = false;
                    if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD00500,16\r\n")))
                        return;
                    string buffer = SocketReceive(70);  //6+48
                    if (buffer == "")
                        return;
                    temLogger(buffer);
                }
                #endregion
                Thread.Sleep(1);
            }
        }
        private void OfflineReConncet_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(3000);
            while (!OfflineReConncet.CancellationPending)
            {
                if (!PLC.IsConnected)  //PLC
                {
                    Thread.Sleep(500);
                    PLC.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    PLC.socket.ReceiveTimeout = 3000;
                    PLC.socket.SendTimeout = 3000;
                    PLC.socket.LingerState = new LingerOption(false, 1);
                    ConnectToPlc();
                    if (!CommucationPlc.IsBusy)
                        CommucationPlc.RunWorkerAsync();
                }
                if (GT2.IsCheck & !GT2.IsConnected)  //测高
                {
                    Thread.Sleep(500);
                    ConnectToGT2();
                }
                if (GlueCU.IsCheck & !GlueCU.IsConnected)  //点胶控制器
                {
                    Thread.Sleep(1000);
                    try
                    {
                        GlueCU.Com.Open();
                        GlueCU.IsConnected = true;
                    }
                    catch
                    {
                        GlueCU.Com.Close();
                        GlueCU.IsConnected = false;
                    }
                }
                Thread.Sleep(10);
            }
        }
        bool SocketSend(byte[] cmd)
        {
            try
            {
                int len = PLC.socket.Send(cmd);
                if (len != cmd.Length)
                    throw new Exception();
                return true;
            }
            catch
            {
                PLC.socket.Close();
                PLC.IsConnected = false;
            }
            return false;
        }
        string SocketReceive(int LengthOfRet)
        {
            byte[] recv = new byte[1024];
            try
            {
                int len = PLC.socket.Receive(recv);
                if (len != LengthOfRet)
                    throw new Exception();
                string ret = Encoding.ASCII.GetString(recv, 0, len);
                if (!ret.StartsWith("11OK"))
                    throw new Exception();
                return ret;
            }
            catch
            {
                PLC.socket.Close();
                PLC.IsConnected = false;
            }
            return "";
        }
        void ResolveData(string buffer, DataType type)
        {
            if (!buffer.StartsWith("11OK"))
                return;
            string statusP = "", Elements = "", teml = "";
            bool flag = false; Int32 tem = 0;
            switch (type)
            {
                #region InPut1 A1
                case DataType.Input1:
                    {
                        statusP = buffer.Substring(7, 1);
                        int m = int.Parse(statusP);
                        if (0 <= m & m <= 3)
                        {
                            Sys.A11autoS = m;
                            if (m == 0)
                            {
                                A1CCD1.Grabimage = false;
                                processing[0] = false;
                            }
                            if (m == 1 && !processing[0] && !WriteToPlc.CMDsend[0])
                            {
                                processing[0] = true;
                                Sys.AssLocation = "1";
                                #region Light
                                string l1 = iniFile.Read("A1CCD1-1", "LighterValue1", propath);
                                string l2 = iniFile.Read("A1CCD1-1", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 0;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 1;
                                    LightSet2();
                                }
                                #endregion
                                OneShot1();
                            }
                            if (m == 2 && !processing[0] && !WriteToPlc.CMDsend[0])
                            {
                                processing[0] = true;
                                Sys.AssLocation = "Hold";
                                #region Light
                                string l1 = iniFile.Read("A1CCD1-Hold", "LighterValue1", propath);
                                string l2 = iniFile.Read("A1CCD1-Hold", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 0;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 1;
                                    LightSet2();
                                }
                                #endregion
                                OneShot1();
                                Thread.Sleep(50);
                                Sys.AssLocation = "Lens";
                                #region Light
                                l1 = iniFile.Read("A1CCD1-Lens", "LighterValue1", propath);
                                l2 = iniFile.Read("A1CCD1-Lens", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 0;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 1;
                                    LightSet2();
                                }
                                #endregion
                                OneShot1();
                            }
                            if (m == 3 && !processing[0] && !WriteToPlc.CMDsend[0])
                            {
                                processing[0] = true;
                                Sys.AssLocation = "2";
                                #region Light
                                string l1 = iniFile.Read("A1CCD1-2", "LighterValue1", propath);
                                string l2 = iniFile.Read("A1CCD1-2", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 0;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 1;
                                    LightSet2();
                                }
                                #endregion
                                OneShot1();
                            }
                        }
                    }
                    break;
                #endregion
                #region InPut2 A1
                case DataType.Input2:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);
                        PLC.ccdTrigger[0] = ((status.Substring(15, 1) == "1") ? true : false);
                        PLC.ccdTrigger[1] = ((status.Substring(14, 1) == "1") ? true : false);
                        PLC.ccdTrigger[2] = ((status.Substring(13, 1) == "1") ? true : false);
                        PLC.ccdTrigger[3] = ((status.Substring(12, 1) == "1") ? true : false);
                        if (!PLC.ccdTrigger[0] && !PLC.ccdTrigger[1])
                        {
                            //A1CCD2.IntSingle = 0;
                            A1CCD2.Grabimage = false;
                            processing[1] = false;
                        }
                        if ((PLC.ccdTrigger[0] || PLC.ccdTrigger[1]) && !processing[1] && !WriteToPlc.CMDsend[1])
                        {
                            string l = "";
                            if (PLC.ccdTrigger[0])
                                A1CCD2.IntSingle = 1;
                            if (PLC.ccdTrigger[1])
                                A1CCD2.IntSingle = 2;
                            if (PLC.ccdTrigger[2])
                                l = iniFile.Read("A1CCD2-PickUp", "LighterValue", propath);
                            if (PLC.ccdTrigger[3])
                                l = iniFile.Read("A1CCD2-Platform", "LighterValue", propath);
                            if (l != "")
                            {
                                brit2 = int.Parse(l); ch2 = 2;
                                LightSet2();
                            }
                            processing[1] = true;
                            if (PLC.ccdTrigger[2])
                            {
                                if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD00212,01\r\n")))
                                    return;
                                buffer = SocketReceive(10);
                                if (buffer == "")
                                    return;
                                statusP = buffer.Substring(4, 4);
                                double tray_xy = Convert.ToInt32(statusP, 16);  //D212
                                t3x = (int)Math.Round(tray_xy / 100);
                                t3y = (int)(tray_xy % 100);
                            }
                            OneShot2();
                        }
                    }
                    break;
                #endregion
                #region InPut3 A2
                case DataType.Input3:
                    {
                        statusP = buffer.Substring(7, 1);
                        int m = int.Parse(statusP);
                        if (0 <= m & m <= 3)
                        {
                            Sys.A21autoS = m;
                            if (m == 0)
                            {
                                A2CCD1.Grabimage = false;
                                processing[2] = false;
                            }
                            if (m == 1 && !processing[2] && !WriteToPlc.CMDsend[2])
                            {
                                processing[2] = true;
                                Sys.AssLocation2 = "1";
                                #region Light
                                string l1 = iniFile.Read("A2CCD1-1", "LighterValue1", propath);
                                string l2 = iniFile.Read("A2CCD1-1", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 3;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 4;
                                    LightSet2();
                                }
                                #endregion
                                OneShot3();
                            }
                            if (m == 2 && !processing[2] && !WriteToPlc.CMDsend[2])
                            {
                                processing[2] = true;
                                Sys.AssLocation2 = "Hold";
                                #region Light
                                string l1 = iniFile.Read("A2CCD1-Hold", "LighterValue1", propath);
                                string l2 = iniFile.Read("A2CCD1-Hold", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 3;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 4;
                                    LightSet2();
                                }
                                #endregion
                                OneShot3();
                                Thread.Sleep(50);
                                Sys.AssLocation2 = "Lens";
                                #region Light
                                l1 = iniFile.Read("A2CCD1-Lens", "LighterValue1", propath);
                                l2 = iniFile.Read("A2CCD1-Lens", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 3;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 4;
                                    LightSet2();
                                }
                                #endregion
                                OneShot3();
                            }
                            if (m == 3 && !processing[2] && !WriteToPlc.CMDsend[2])
                            {
                                processing[2] = true;
                                Sys.AssLocation2 = "2";
                                #region Light
                                string l1 = iniFile.Read("A2CCD1-2", "LighterValue1", propath);
                                string l2 = iniFile.Read("A2CCD1-2", "LighterValue2", propath);
                                if (l1 != "")
                                {
                                    brit2 = int.Parse(l1); ch2 = 3;
                                    LightSet2();
                                    Thread.Sleep(5);
                                    brit2 = int.Parse(l2); ch2 = 4;
                                    LightSet2();
                                }
                                #endregion
                                OneShot3();
                            }
                        }
                    }
                    break;
                #endregion
                #region InPut4 A2
                case DataType.Input4:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);
                        PLC.ccdTrigger[4] = ((status.Substring(15, 1) == "1") ? true : false);
                        PLC.ccdTrigger[5] = ((status.Substring(14, 1) == "1") ? true : false);
                        PLC.ccdTrigger[6] = ((status.Substring(13, 1) == "1") ? true : false);
                        PLC.ccdTrigger[7] = ((status.Substring(12, 1) == "1") ? true : false);
                        if (!PLC.ccdTrigger[4] && !PLC.ccdTrigger[5])
                        {
                            //A2CCD2.IntSingle = 0;
                            A2CCD2.Grabimage = false;
                            processing[3] = false;
                        }
                        if ((PLC.ccdTrigger[4] || PLC.ccdTrigger[5]) && !processing[3] && !WriteToPlc.CMDsend[3])
                        {
                            string l = "";
                            if (PLC.ccdTrigger[4])
                                A2CCD2.IntSingle = 1;
                            if (PLC.ccdTrigger[5])
                                A2CCD2.IntSingle = 2;
                            if (PLC.ccdTrigger[6])
                                l = iniFile.Read("A2CCD2-PickUp", "LighterValue", propath);
                            if (PLC.ccdTrigger[7])
                                l = iniFile.Read("A2CCD2-Platform", "LighterValue", propath);
                            if (l != "")
                            {
                                brit2 = int.Parse(l); ch2 = 5;
                                LightSet2();
                            }
                            processing[3] = true;
                            if (PLC.ccdTrigger[6])
                            {
                                if (!SocketSend(Encoding.ASCII.GetBytes("01WRDD00212,01\r\n")))
                                    return;
                                buffer = SocketReceive(10);
                                if (buffer == "")
                                    return;
                                statusP = buffer.Substring(4, 4);
                                double tray_xy = Convert.ToInt32(statusP, 16);  //D212
                                t3x = (int)Math.Round(tray_xy / 100);
                                t3y = (int)(tray_xy % 100);
                            }
                            OneShot4();
                        }
                    }
                    break;
                #endregion
                #region InPut5 P
                case DataType.Input5:
                    {
                        statusP = buffer.Substring(7, 1);
                        int m = int.Parse(statusP);
                        if (0 <= m & m < 2)
                        {
                            if (m == 0)
                            {
                                PCCD1.Grabimage = false;
                                processing[4] = false;
                            }
                            if (m == 1 && !processing[4] && !WriteToPlc.CMDsend[4])
                            {
                                Sys.P1autoS = true;
                                processing[4] = true;
                                #region Light
                                string ccdname = "";
                                if (Sys.P1DisMode2)
                                    ccdname = "PCCD1";
                                else
                                {
                                    Sys.AssLocation = "Hold";
                                    ccdname = "PCCD1-Hold";
                                }
                                string l1 = iniFile.Read(ccdname, "LighterValue1", propath);
                                if (l1 != "")
                                {
                                    brit = int.Parse(l1); ch = 0;
                                    LightSet();
                                    Thread.Sleep(5);
                                    //string l2 = iniFile.Read(ccdname, "LighterValue2", propath);
                                    //brit = int.Parse(l2); ch = 1;
                                    //LightSet();
                                }
                                #endregion
                                OneShot5();
                            }
                        }
                    }
                    break;
                #endregion
                #region InPut6 P
                case DataType.Input6:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);  //(8 9 10 11) -> (16 17 18 19 20 21)
                        PLC.ccdTrigger[16] = ((status.Substring(15, 1) == "1") ? true : false);   //0取料        
                        PLC.ccdTrigger[17] = ((status.Substring(14, 1) == "1") ? true : false);   //1平台1       
                        PLC.ccdTrigger[18] = ((status.Substring(13, 1) == "1") ? true : false);   //2平台2       
                        PLC.ccdTrigger[19] = ((status.Substring(12, 1) == "1") ? true : false);   //3取料光源    
                        PLC.ccdTrigger[20] = ((status.Substring(11, 1) == "1") ? true : false);   //4平台1光源   
                        PLC.ccdTrigger[21] = ((status.Substring(10, 1) == "1") ? true : false);   //5平台2光源   
                        if (!PLC.ccdTrigger[16] && !PLC.ccdTrigger[17] && !PLC.ccdTrigger[18])
                        {
                            //PCCD2.IntSingle = 0;
                            PCCD2.Grabimage = false;
                            processing[5] = false;
                        }
                        if ((PLC.ccdTrigger[16] || PLC.ccdTrigger[17] || PLC.ccdTrigger[18]) && !processing[5] && !WriteToPlc.CMDsend[5])
                        {
                            string l = "";
                            if (PLC.ccdTrigger[16])
                                PCCD2.IntSingle = 1;
                            if (PLC.ccdTrigger[17])
                                PCCD2.IntSingle = 2;
                            if (PLC.ccdTrigger[18])
                                PCCD2.IntSingle = 3;
                            if (PLC.ccdTrigger[19])
                                l = iniFile.Read("PCCD2-PickUp", "LighterValue", propath);
                            if (PLC.ccdTrigger[20])
                                l = iniFile.Read("PCCD2-Platform1", "LighterValue", propath);
                            if (PLC.ccdTrigger[21])
                                l = iniFile.Read("PCCD2-Platform2", "LighterValue", propath);
                            if (l != "")
                            {
                                brit = int.Parse(l); ch = 2;
                                LightSet();
                            }
                            processing[5] = true;
                            OneShot6();
                        }
                    }
                    break;
                #endregion
                #region InPut7 G
                case DataType.Input7:
                    {
                        statusP = buffer.Substring(7, 1);
                        int m = int.Parse(statusP);
                        if (0 <= m & m < 2)
                        {
                            if (m == 0)
                            {
                                GCCD1.Grabimage = false;
                                processing[6] = false;
                            }
                            if (m == 1 && !processing[6] && !WriteToPlc.CMDsend[6])
                            {
                                string l = iniFile.Read("GCCD1", "LighterValue", propath);
                                if (l != "")
                                {
                                    brit = int.Parse(l); ch = 4;
                                    LightSet();
                                }
                                processing[6] = true;
                                OneShot7();
                            }
                        }
                    }
                    break;
                #endregion
                #region InPut8 G
                case DataType.Input8:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);
                        if (!Sys.NoAutoMatic)
                        {
                            PLC.ccdTrigger[12] = ((status.Substring(15, 1) == "1") ? true : false); //0 平台1
                            PLC.ccdTrigger[13] = ((status.Substring(14, 1) == "1") ? true : false); //1 平台2
                            PLC.ccdTrigger[14] = ((status.Substring(13, 1) == "1") ? true : false); //2 胶前1
                            PLC.ccdTrigger[15] = ((status.Substring(12, 1) == "1") ? true : false); //3 胶后1
                            PLC.ccdTrigger[22] = ((status.Substring(8, 1) == "1") ? true : false); //7 胶前2
                            PLC.ccdTrigger[23] = ((status.Substring(7, 1) == "1") ? true : false); //8 胶后2

                            if (!PLC.ccdTrigger[14] && !PLC.ccdTrigger[15] && !PLC.ccdTrigger[22] && !PLC.ccdTrigger[23] & !PLC.ccdTrigger[16])
                            {
                                //GCCD2.IntSingle = 0; 
                                GCCD2.IntPosition = 0; GCCD2.Grabimage = false; processing[7] = false;
                            }
                            if ((PLC.ccdTrigger[12] || PLC.ccdTrigger[13]) & (PLC.ccdTrigger[14] || PLC.ccdTrigger[15] || PLC.ccdTrigger[22] || PLC.ccdTrigger[23]) && !processing[7] && !WriteToPlc.CMDsend[7])
                            {
                                string l = "";
                                if (PLC.ccdTrigger[12])
                                {
                                    if (PLC.ccdTrigger[14] || PLC.ccdTrigger[15])
                                    {
                                        l = iniFile.Read("GCCD2-1", "LighterValue", propath);
                                        GCCD2.IntPosition = 1;
                                    }
                                    else if (PLC.ccdTrigger[22] || PLC.ccdTrigger[23])
                                    {
                                        l = iniFile.Read("GCCD2-3", "LighterValue", propath);
                                        GCCD2.IntPosition = 3;
                                    }
                                }
                                if (PLC.ccdTrigger[13])
                                {
                                    if (PLC.ccdTrigger[14] || PLC.ccdTrigger[15])
                                    {
                                        l = iniFile.Read("GCCD2-2", "LighterValue", propath);
                                        GCCD2.IntPosition = 2;
                                    }
                                    else if (PLC.ccdTrigger[22] || PLC.ccdTrigger[23])
                                    {
                                        l = iniFile.Read("GCCD2-4", "LighterValue", propath);
                                        GCCD2.IntPosition = 4;
                                    }
                                }

                                if (l != "")
                                {
                                    brit = int.Parse(l); ch = 5;
                                    LightSet();
                                }
                                if (PLC.ccdTrigger[14] || PLC.ccdTrigger[22])
                                    GCCD2.IntSingle = 1;
                                if (PLC.ccdTrigger[15] || PLC.ccdTrigger[23])
                                    GCCD2.IntSingle = 2;
                               
                                if (GCCD2.IsConnected)
                                {
                                    processing[7] = true;
                                    OneShot8();
                                }
                            }
                        }
                        else
                        {
                            //PLC.ccdTrigger[16] = ((status.Substring(11, 1) == "1") ? true : false); //4 胶点
                            //VisionSet.ViewNum = "8";
                            //FrmVisionSet.xpm = GCCD2.xpm;
                            //FrmVisionSet.ypm = GCCD2.ypm; 
                            //string lgp = iniFile.Read("GCCD2GluePoint", "LighterValue", propath);
                            //if (lgp != "")
                            //{
                            //    brit = int.Parse(lgp); ch = 5;
                            //    LightSet();
                            //}
                            //Thread.Sleep(10);
                            //OneShot8();
                        }
                    }
                    break;
                #endregion
                #region InPut9 Q
                case DataType.Input9:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);
                        if (status.Substring(14, 1) == "1")
                            PLC.Qlocation = 1;
                        if (status.Substring(13, 1) == "1")
                            PLC.Qlocation = 2;
                        bool qtri = ((status.Substring(15, 1) == "1") ? true : false);
                        if (Barcode1.QCCDisChecked)
                            Barcode1.qBarcodeTrig = qtri;
                        if (!qtri)
                            processing[8] = false;
                        if (qtri & !processing[8])
                        {
                            processing[8] = true;
                            switch (QCCD.CCDBrand)
                            {
                                case 0: OneShot9(); break;
                                case 1: myHikvision.OneShot(); break;
                            }
                        }
                    }
                    break;
                #endregion
                #region Output
                case DataType.Output:
                    {
                        Elements = buffer.Substring(4, 44);
                        for (int i = 3; i < Elements.Length; i = i + 4)
                        {
                            flag = (Elements[i] == '1');
                            PLC.BTrigger[(i + 1) / 4 - 1] = flag;
                        }
                        Elements = buffer.Substring(23, 1);
                        if (Elements == "3")
                            PLC.Blocation = 1;
                        if (Elements == "5")
                            PLC.Blocation = 2;
                        if (!Barcode1.QCCDisChecked)
                            Pstation = PLC.Blocation.ToString();
                        if (Elements == "1" || Elements == "3" || Elements == "5")
                            PLC.BTrigger[4] = true;
                        else
                            PLC.BTrigger[4] = false;
                        Elements = buffer.Substring(44, 4);
                        string status = HexString2BinString(Elements);
                        for (int i = 0; i < status.Length; i++)
                        {
                            if (status.Substring(i, 1) == "1")
                                PLC.Barstatus[i] = true;
                            else
                                PLC.Barstatus[i] = false;
                        }
                        if (PLC.Barstatus[11] & RiReader.Barcode != RiReader.newBarcode)
                            RiReader.Barcode = RiReader.newBarcode;
                    }
                    break;
                #endregion
                #region LogTrigger
                case DataType.LogTrigger:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);
                        for (int i = 0; i < status.Length; i++)
                        {
                            if (status.Substring(i, 1) == "1")
                                PLC.allstatus[i] = true;
                            else
                                PLC.allstatus[i] = false;
                        }
                        if (!PLC.allstatus[15]) //单颗料 LogTrigger
                            LogTriRead = false;
                        if (PLC.allstatus[15] && !LogTriRead)
                        {
                            LogTriRead = true;
                            PLC.Tray1end = true;//復位滿盤信號
                            PLC.LogTrigger = true;
                        }

                        if (PLC.allstatus[14])  //满盘信号
                        {
                            #region 
                            DateTime DT = DateTime.Now;   
                            this.Invoke(new MethodInvoker(delegate
                               {
                                   if (RiReader.Web_Tray_InOutStation_sMsg == "")
                                   {
                                       Run.rtxWebMessage.SelectionColor = Color.Green;
                                       Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                       Run.rtxWebMessage.AppendText(RiReader.Barcode + "_退倉" + "\r");
                                   }
                               }));
                            weblog.log("WebOK:" + RiReader.Barcode + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_退倉" );
                      
                            if (PLC.allstatus[15])
                                Thread.Sleep(5);
                            PLC.Tray1end = true;//復位滿盤信號
                            #region  進出站功能開啟
                            if (RiReader.Web_Tray_InOutStation && Sys.Codes == "M")
                            {
                                try
                                {
                                    CallWithTimeout(m_Web_Tray_Out, 15000);
                                }
                                catch
                                {
                                    RiReader.Web_Tray_InOutStation_Result = false;
                                    RiReader.Web_Tray_InOutStation_sMsg = "訪問系統超時！";
                                }
                                
                                this.Invoke(new MethodInvoker(delegate
                                {
                                    if (RiReader.Web_Tray_InOutStation_sMsg == "")
                                    {
                                        Run.rtxWebMessage.SelectionColor = Color.Green;
                                        Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                        Run.rtxWebMessage.AppendText(RiReader.Barcode+"_出站成功!_數量:" + ProductCount + "\r");
                                    }
                                    else
                                    {
                                        Run.rtxWebMessage.SelectionColor = Color.Red;
                                        Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction  + "\t");
                                        Run.rtxWebMessage.AppendText(RiReader.Barcode + "_出站失败!_數量:" + ProductCount + "\t");
                                        Run.rtxWebMessage.AppendText(RiReader.Web_Tray_InOutStation_sMsg + "\r");
                                    }
                                    Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                                }));
                                if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                                    weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                                if (RiReader.Web_Tray_InOutStation_Result)
                                {
                                    RiReader.Web_Tray_InOutStation_PlcResult = "0001";
                                    weblog.log("WebOK:" + RiReader.Barcode + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_出站成功!_數量:" + ProductCount);
                                }
                                else
                                {
                                    if (RiReader.Web_Tray_InOutStation_ErrorIgnore)//忽略報警
                                    {
                                        RiReader.Web_Tray_InOutStation_PlcResult = "0001";
                                    }
                                    else
                                    {
                                        RiReader.Web_Tray_InOutStation_PlcResult = "0005";
                                    }

                                    weblog.log("Web报错:" + RiReader.Barcode + "Out,出站失败!" + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_" + RiReader.Web_Tray_InOutStation_sMsg + "_數量:" + ProductCount);
                                    this.Invoke(new MethodInvoker(delegate
                                   {
                                       new MessageForm(10000000, "出站失败!" + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_"  + RiReader.Barcode +"_"+ RiReader.Web_Tray_InOutStation_sMsg + "_數量:" + ProductCount).Show();
                                   }));
                                }
                                RiReader.Web_Tray_InOutStation_Complete = true;
                            }
                            else if (RiReader.Web_Tray_InOutStation && Sys.Codes != "M")
                            {
                                RiReader.Web_Tray_InOutStation_PlcResult = "0001";
                                RiReader.Web_Tray_InOutStation_Complete = true;
                                weblog.log("WebOK:" + RiReader.Barcode + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_" + RiReader.Barcode + "_" + "首件出站_數量:" + ProductCount);
                                this.Invoke(new MethodInvoker(delegate
                                {
                                    Run.rtxWebMessage.SelectionColor = Color.Green;
                                    Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                    Run.rtxWebMessage.AppendText(RiReader.Barcode + "_首件出站_數量:" + ProductCount + "\r");

                                    Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                                }));
                                weblog.log("WebOpen:" + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "首件結束_量產_點膠進出站管控自動開啟!");
                                this.Invoke(new MethodInvoker(delegate
                               {
                                   Run.cbCode.SelectedIndex = 0;
                                   
                                   new MessageForm(10000000, DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "首件結束_量產_點膠進出站管控自動開啟!").Show();
                                   Run.rtxWebMessage.SelectionColor = Color.Orange;
                                   Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "\t");
                                   Run.rtxWebMessage.AppendText("首件結束_量產_點膠進出站管控自動開啟!" + "\t");
                                   Run.rtxWebMessage.AppendText(RiReader.Web_Tray_InOutStation_sMsg + "\r");

                                   Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                               }));
                                string path = Sys.IniPath + "\\ComPortPara.ini";   //设备各参数路径
                                Sys.Codes = "M";
                                IniFile.Write("Addition", "Codes", "0", path);
                            }
                            #endregion
                            ChangeNamePicture1(RiReader.Barcode);
                            ChangeNameNewBarcode(); //changename后清空Barcode
                            //LogSerial = "";
                            Run2.ReleaseLabels();   //AVItable
                            Run2.GenerateLabels();
                            string tt = "";
                            this.Invoke(new MethodInvoker(delegate
                            {
                                tt = Run.txtGlueBarcode.Text;
                            }));
                            if (Glue.IsChecked & tt != "" & tt == Glue.Barcode)
                            {
                                FrmDisplay1.GBtimeN = DateTime.Now;
                                System.TimeSpan t = FrmDisplay1.GBtimeN - FrmDisplay1.replaceTime;
                                if (t.TotalMinutes >= Glue.glueTime)
                                {
                                    Protocol.strPCRead_GlueTimeOut = true;  //满盘信号时判断胶水时间超时
                                    Protocol.IsPCRead = true;
                                    MessageBox.Show("胶水已超过规定使用时间，请更换胶水后点击确认！", "", MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Warning);
                                    gluelog.log("Glue报错:TimeOut(HTrayFull)");
                                }
                            }
                            
                            #endregion
                        }
                        if (PLC.allstatus[13])
                            Pstation = "1";
                        if (PLC.allstatus[12])
                            Pstation = "2";
                    }
                    break;
                #endregion
                #region ErrTrigger
                case DataType.ErrTrigger:
                    {
                        PLC.PlcStatusP = buffer.Substring(4, 4);
                    }
                    break;
                #endregion
                #region  未用
                case DataType.linshi:
                    {
                        tem = Convert.ToInt32(buffer.Substring(8, 4) + buffer.Substring(4, 4), 16);     //(D322-323)
                        if (tem < 0)
                        {
                            tem = -tem;
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            y1 = "-" + (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                        else
                        {
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            y1 = (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                        tem = Convert.ToInt32(buffer.Substring(16, 4) + buffer.Substring(12, 4), 16);     //(D322-323)
                        if (tem < 0)
                        {
                            tem = -tem;
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            x1 = "-" + (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                        else
                        {
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            x1 = (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                        tem = Convert.ToInt32(buffer.Substring(24, 4) + buffer.Substring(20, 4), 16);     //(D322-323)
                        if (tem < 0)
                        {
                            tem = -tem;
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            y2 = "-" + (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                        else
                        {
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            y2 = (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                        tem = Convert.ToInt32(buffer.Substring(32, 4) + buffer.Substring(28, 4), 16);     //(D322-323)
                        if (tem < 0)
                        {
                            tem = -tem;
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            x2 = "-" + (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                        else
                        {
                            if (tem.ToString().Length == 1)
                                teml = "000";
                            if (tem.ToString().Length == 2)
                                teml = "00";
                            if (tem.ToString().Length == 3)
                                teml = "0";
                            x2 = (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        }
                    }
                    break;
                #endregion
                #region readLocation
                case DataType.readLocation:
                    {
                        tem = Convert.ToInt32(buffer.Substring(8, 4) + buffer.Substring(4, 4), 16);     //(D322-323)
                        #region 未用
                        //if (tem < 0)
                        //{
                        //    tem = -tem;
                        //    if (tem.ToString().Length == 1)
                        //        teml = "000";
                        //    if (tem.ToString().Length == 2)
                        //        teml = "00";
                        //    if (tem.ToString().Length == 3)
                        //        teml = "0";
                        //    y1 = "-" + (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        //}
                        //else
                        //{
                        //    if (tem.ToString().Length == 1)
                        //        teml = "000";
                        //    if (tem.ToString().Length == 2)
                        //        teml = "00";
                        //    if (tem.ToString().Length == 3)
                        //        teml = "0";
                        //    y1 = (tem / 1000).ToString() + "." + teml + (tem % 1000).ToString();
                        //}
                        //if (y1 != "")
                        //    WriteToPlc.DoubleRead = double.Parse(y1);
                        #endregion
                        WriteToPlc.DoubleRead = (double)tem / 1000;
                    }
                    break;
                #endregion
                #region InPut1 A1
                case DataType.pianbai:
                    {
                        statusP = buffer.Substring(7, 1);
                        int m = int.Parse(statusP);
                        if (0 <= m & m < 5)
                        {
                            if (m == 1 && !Sys.CalViewPross)
                            {
                                Sys.CalViewPross = true;
                                OneShot1(); // A11
                            }
                            if (m == 2 && !Sys.CalViewPross)
                            {
                                Sys.CalViewPross = true;
                                OneShot3(); // A21
                            }
                            if (m == 3 && !Sys.CalViewPross)
                            {
                                Sys.CalViewPross = true;
                                OneShot2(); // A12
                            }
                            if (m == 4 && !Sys.CalViewPross)
                            {
                                Sys.CalViewPross = true;
                                OneShot4(); // A22
                            }
                            
                        }
                    }
                    break;
                #endregion
                #region  (clear)点胶颗数loop
                case DataType.loop:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);
                        for (int i = 0; i < status.Length; i++)
                        {
                            if (status.Substring(i, 1) == "1")
                                PLC.loopstatus[i] = true;
                            else
                                PLC.loopstatus[i] = false;
                        }
                    }
                    break;
                #endregion
                #region  jiancekakong PLC功能开启的状况
                case DataType.clince:
                    {
                        statusP = buffer.Substring(4, 4);
                        string status = HexString2BinString(statusP);
                        for (int i = 0; i < status.Length; i++)
                        {
                            if (status.Substring(i, 1) == "1")
                                Monitor.allstatus[i] = true;
                            else
                                Monitor.allstatus[i] = false;
                        }
                    }
                    break;
                #endregion
                #region curTsta
                //case DataType.readcurLt:
                //    {
                //        statusP = buffer.Substring(4, 4);
                //        double tray_xy = Convert.ToInt32(statusP, 16);  //D324
                //        curt3x = (int)Math.Round(tray_xy / 100);
                //        curt3y = (int)(tray_xy % 100);
                //    }
                //    break;
                #endregion
                #region 部品數
                case DataType.PlateCount://一盤部品數
                    {
                        PLC.PlateCount =Convert.ToInt32( buffer.Substring(4, 4),16).ToString();
                        break;
                    }
                #endregion
                #region 膠水數量
                case DataType.GlueCount:
                    {
                        Glue.Count_Max = Convert.ToInt32(buffer.Substring(8, 4) + buffer.Substring(4, 4), 16);
                        Glue.Count_Now = Convert.ToInt32(buffer.Substring(16, 4) + buffer.Substring(12, 4), 16);
                        break;
                    }
                
                #endregion
                #region PLC&人機版本
                case DataType.PLCHMI_Versions:
                    {
                        PLC.PLCVisions = (Convert.ToInt32(buffer.Substring(8, 4) + buffer.Substring(4, 4), 16)).ToString();
                        PLC.HMIVisions = (Convert.ToInt32(buffer.Substring(16, 4) + buffer.Substring(12, 4), 16)).ToString();
                    } break;
                #endregion
                #region 盤行最大XY
                case DataType.TrayMaxXY:
                    {
                        PLC.AZ0TrayMax_X = (Convert.ToInt32(buffer.Substring(4, 4), 16));//D10024
                        PLC.AZ0TrayMax_Y = (Convert.ToInt32(buffer.Substring(12, 4), 16));//D10026
                        PLC.PZ0TrayMax_X = (Convert.ToInt32(buffer.Substring(36, 4), 16));//D10032
                        PLC.PZ0TrayMax_Y = (Convert.ToInt32(buffer.Substring(44, 4), 16));//D10034
                    } break;
                #endregion
                //case DataType.LogMsgSign:
                //   {    
                //        double rev=0.0;                     
                //        rev=Convert.ToInt32(buffer.Substring(4, 4),16);    //D200
                //        Sys.LogMsgSign[0] = rev;
                //        rev=Convert.ToInt32(buffer.Substring(8, 4),16);    //D201
                //        Sys.LogMsgSign[1] = rev;
                //        rev=Convert.ToInt32( buffer.Substring(12, 4),16);  //D202    
                //        Sys.LogMsgSign[2] = rev;
                //        rev=Convert.ToInt32( buffer.Substring(16, 4),16);  //D203
                //        Sys.LogMsgSign[3] = rev;
                //        rev=Convert.ToInt32( buffer.Substring(20, 4),16);  //D204
                //        Sys.LogMsgSign[4] = rev;
                //        rev=Convert.ToInt32( buffer.Substring(24, 4),16);  //D205
                //        Sys.LogMsgSign[5] = rev;
                //        rev=Convert.ToInt32( buffer.Substring(28, 4),16);  //D206
                //        Sys.LogMsgSign[6] = rev;
                //        rev=Convert.ToInt32( buffer.Substring(32, 4),16);  //D207 
                //        Sys.LogMsgSign[7] = rev;
                //       break;
                //   }

            }
        }
        void  Logger(string cmd)
        {
            try
            {
                #region Para
                tempVar = Convert.ToInt32(cmd.Substring(8, 4) + cmd.Substring(4, 4), 16);//(D302~D303)
                PowerSupplyTime_Days = tempVar.ToString();

                tempVar = Convert.ToInt32(cmd.Substring(12, 4), 16);//(D304~D306)
                PowerSupplyTime_Hours = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(16, 4), 16);
                PowerSupplyTime_Minutes = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(20, 4), 16);
                PowerSupplyTime_Seconds = tempVar.ToString();
                //tempVar = Convert.ToInt16(cmd.Substring(24, 4), 16); //D307  毫秒

                tempVar = Convert.ToInt32(cmd.Substring(28, 4), 16);//(D308~D310)
                AutoRunningTime_Hours = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(32, 4), 16);
                AutoRunningTime_Minutes = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(36, 4), 16);
                AutoRunningTime_Seconds = tempVar.ToString();

                tempVar = Convert.ToInt32(cmd.Substring(40, 4), 16);//(D311~D313)
                AlarmPauseTime_Hours = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(44, 4), 16);
                AlarmPauseTime_Minutes = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(48, 4), 16);
                AlarmPauseTime_Seconds = tempVar.ToString();


                tempVar = Convert.ToInt32(cmd.Substring(52, 4), 16);//(D314~D316)
                WaitTime_Hours = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(56, 4), 16);
                WaitTime_Minutes = tempVar.ToString();
                tempVar = Convert.ToInt32(cmd.Substring(60, 4), 16);
                WaitTime_Seconds = tempVar.ToString();

                AlarmCount = Convert.ToInt32(cmd.Substring(64, 4), 16);    //D317
                LensCount = Convert.ToInt32(cmd.Substring(72, 4) + cmd.Substring(68, 4), 16);//(D318~D319) 产品计数
                CycleTime = Convert.ToInt32(cmd.Substring(80, 4) + cmd.Substring(76, 4), 16);  //(D320~D321)
                Int32 tem = Convert.ToInt32(cmd.Substring(88, 4) + cmd.Substring(84, 4), 16);     //(D322-323)

                #region AssemblyHeight
                string teml = "";
                if (tem < 0)
                {
                    tem = -tem;
                    if (tem.ToString().Length == 1)
                        teml = "000";
                    if (tem.ToString().Length == 2)
                        teml = "00";
                    if (tem.ToString().Length == 3)
                        teml = "0";
                    AssemblyHeight = "-" + (tem / 10000).ToString() + "." + teml + (tem % 10000).ToString();
                }
                else
                {
                    if (tem.ToString().Length == 1)
                        teml = "000";
                    if (tem.ToString().Length == 2)
                        teml = "00";
                    if (tem.ToString().Length == 3)
                        teml = "0";
                    AssemblyHeight = (tem / 10000).ToString() + "." + teml + (tem % 10000).ToString();
                }
                #endregion
                double tray_xy = Convert.ToInt32(cmd.Substring(92, 4), 16);  //D324
                int tray_x = (int)Math.Round(tray_xy / 100);
                int tray_y = (int)tray_xy % 100;
                Tray_XY = tray_x.ToString() + "," + tray_y.ToString();
                #endregion
                #region  Write Info
                if (LensCount != LensCountL || Tray_XY != Tray_XYLast) //&& Transmission.code != ""
                {
                    if (Tray_XY != Tray_XYLast & Tray_XY != "0,0")
                    {
                        if (RiReader.IsChecked & RiReader.Barcode == "")
                        {
                            RiReader.strTrRead = "0002";
                            RiReader.IsTrRead = true;
                            MessageBox.Show("Tray1信息为空或PZ0扫码异常，请检查！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            #region WriteLine
                            string logfile = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
                            #region 捞取信息
                            string logpath = Sys.ReportLog1 + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                            if (RiReader.Barcode != "" & (DateTime.Now - DateTime.Parse(LogDate)).Days == 1 & (Tray_XY != "2,1" || Tray_XY != "1,1"))
                                logpath = Sys.ReportLog1 + "\\" + LogDate;
                            if (!Directory.Exists(logpath))
                            {
                                LogDate = DateTime.Now.ToString("yyyy-MM-dd");
                                iniFile.Write("LOGMESSAGE", "Logdate", LogDate, setpath);
                                LogTime = DateTime.Now.ToString("yyyyMMdd");
                                iniFile.Write("LOGMESSAGE", "LogdTime", LogTime, setpath);
                                Directory.CreateDirectory(logpath);
                            }
                            //建立新log標題
                            if (!File.Exists(logpath + "\\" + logfile))
                            {
                                ProductCount = 0;
                                File.WriteAllText(logpath + "\\" + logfile, FileHeader);
                            }
                            ProductCount++;
                            try
                            {
                                if (Barcode1.QCCDisChecked)
                                {
                                    switch (Pstation)
                                    {
                                        case "1": BarMessage = BarMessages1; break;
                                        case "2": BarMessage = BarMessages2; break;
                                    }
                                }
                                else
                                {
                                    if (Pstation == "")
                                        Pstation = PLC.Blocation.ToString(); //工位
                                }
                                //依照工位儲存辨識膠水結果
                                double GlueArea = 0,GlueAngle_1 = 0,GlueAngle_2 = 0;
                                if(Pstation=="1")
                                {
                                    GlueArea = Glue.ResultGlueArea_CCD1;
                                    GlueAngle_1 = Glue.ResultGlueAngle_CCD1;
                                    GlueAngle_2 = Glue.ResultGlueAngle_2_CCD1;
                                }
                                else
                                {
                                    GlueArea = Glue.ResultGlueArea_CCD2;
                                    GlueAngle_1 = Glue.ResultGlueAngle_CCD2;
                                    GlueAngle_2 = Glue.ResultGlueAngle_2_CCD2;
                                }
                                if (BarMessage == "" || BarMessage == "null")
                                    BarMessage = "NA";
                                bpfWebMessage.Rows.Add(BarMessage, tray_x.ToString(), tray_y.ToString());
                                iniFile.Write("DBarcode", tray_x.ToString() + "_" + tray_y.ToString(), BarMessage, Sys.IniPath + "\\Data.ini");
                                #region Table
                                try
                                {
                                    int m = tray_x + (tray_y - 1) * PLC.PZ0TrayMax_X - 1;
                                    if (m == 1)
                                    {
                                        for (int i = 0; i < PLC.PZ0TrayMax_Y*PLC.PZ0TrayMax_X; i++)
                                        {
                                            iniFile.Write("Table", i.ToString() + "Result", "", path);
                                        }
                                        Run2.ReleaseLabels();
                                        Run2.GenerateLabels();
                                    }
                                    if (Glue.GlueOutResult == "PASS")
                                    {
                                        Sys.LabelsTray[m].BackColor = Color.Green;
                                        iniFile.Write("Table", m.ToString() + "Result", "1", path);
                                        if (Barcode1.Result_ReadOK == "1" || Barcode1.Result_ReadOK == "2")
                                            Sys.LabelsTray[m].BackColor = ((Barcode1.Result_ReadOK == "1") ? Color.Green : Color.Red);
                                        else
                                            Sys.LabelsTray[m].BackColor = Color.Gray;
                                    }
                                    if (!Glue.Cir2AVIchecked)  //溢胶检测圆2未开启
                                    {
                                        if (Glue.GlueOutResult == "NG")
                                        {
                                            iniFile.Write("Table", m.ToString() + "Result", "2", path);
                                            Sys.LabelsTray[m].BackColor = Color.Red;
                                        }
                                    }
                                    else
                                    {
                                        switch (Glue.GlueOutResult)
                                        {
                                            case "NG1":
                                                iniFile.Write("Table", m.ToString() + "Result", "2", path);
                                                Sys.LabelsTray[m].BackColor = Color.Red; break;
                                            case "NG2":
                                                iniFile.Write("Table", m.ToString() + "Result", "3", path);
                                                Sys.LabelsTray[m].BackColor = Color.DarkRed; break;
                                        }
                                    }
                                }
                                catch
                                {
                                    //這table有問題,先不處理,遇到問題先catch調
                                }
                                #endregion
                                using (StreamWriter sw = new StreamWriter(logpath + "\\" + logfile, true))
                                {
                                    sw.WriteLine(DateTime.Now.ToString("yyyyMMdd") + "\t" + DateTime.Now.ToString("HH:mm:ss") + "\t" +
                                                 PowerSupplyTime_Days + "\t" +
                                                 string.Format("{0:d2}:{1:d2}:{2:d2}", PowerSupplyTime_Hours, PowerSupplyTime_Minutes, PowerSupplyTime_Seconds) + "\t" +
                                                 string.Format("{0:d2}:{1:d2}:{2:d2}", AutoRunningTime_Hours, AutoRunningTime_Minutes, AutoRunningTime_Seconds) + "\t" +
                                                 string.Format("{0:d2}:{1:d2}:{2:d2}", AlarmPauseTime_Hours, AlarmPauseTime_Minutes, AlarmPauseTime_Seconds) + "\t" +
                                                 string.Format("{0:d2}:{1:d2}:{2:d2}", WaitTime_Hours, WaitTime_Minutes, WaitTime_Seconds) + "\t" +
                                                 AlarmCount.ToString() + "\t" + ProductCount.ToString() + "\t" + Pstation + "\t" +
                                                 string.Format("{0:d2}:{1:d2}.{2:d2}", CycleTime / 6000, (CycleTime % 6000) / 100, (CycleTime % 6000) % 100) + "\t" +
                                                 Sys.MachineId + "\t" + Sys.CurrentProduction + "\t" + AssemblyHeight + "\t" + "1\t" +
                                                 RiReader.Barcode + "\t" + Tray_XY + "\t" + BarMessage + "\t" + Reader.Barcode + "\t" +
                                                 Glue.Barcode + "\t" + Glue.GlueOutResult + "\t" + Glue.GlueOutAreaMax + "\t" + Glue.GlueOutArea + "\t" +
                                                 Glue.ID + "\t" + Glue.OD + "\t" + Glue.GlueD + "\t" + Glue.GlueWidth + "\t" +
                                                 Glue.GlueDisR + "\t" + Glue.GlueDis1 + "\t" + Glue.GlueDis2 + "\t" +
                                                 Sys.CoatResult + "\t" + Sys.rmin.ToString() + "\t" + Sys.diam_min.ToString() + "\t" +
                                                 Sys.P1Result + "\t" + Sys.AssDisX + "\t" + Sys.AssDisY + "\t" + Sys.AssDis + "\t" +
                                                 GlueCU.arrayTime[0].ToString("0.000") + "\t" + GlueCU.arrayPressure[0].ToString("0.0") + "\t" + "-" + GlueCU.arrayPressure[10].ToString("0.0") + "\t" +
                                                 GlueCU.arrayTime[1].ToString("0.000") + "\t" + GlueCU.arrayPressure[1].ToString("0.0") + "\t" + "-" + GlueCU.arrayPressure[11].ToString("0.0") + "\t" +
                                                 GlueArea.ToString("f0") + "\t" + GlueAngle_1.ToString("f0") + "\t" + GlueAngle_2.ToString("f0")+"\t"+
                                                 Sys.Mode5result + "\t" + Sys.Mode5Distance + "\t" + Sys.Mode5Distance1 + "\t" + Sys.Mode5Distance2+"\t"+
                                                 Barcode1.LensBarcodeAngle
                                                 );
                                    sw.Flush();
                                    sw.Close();
                                    sw.Dispose();
                                }
                                Pstation = "";
                                Sys.P1Result = ""; Sys.AssDisX = ""; Sys.AssDisY = ""; Sys.AssDis = "";
                                Sys.Mode5result = ""; Sys.Mode5Distance = ""; Sys.Mode5Distance1 = ""; Sys.Mode5Distance2 = "";
                                iniFile.Write("AssShift", "Result", Sys.P1Result, Sys.IniPath + "\\Data.ini");
                                iniFile.Write("AssShift", "X", Sys.AssDisX, Sys.IniPath + "\\Data.ini");
                                iniFile.Write("AssShift", "Y", Sys.AssDisY, Sys.IniPath + "\\Data.ini");
                                iniFile.Write("AssShift", "Dis", Sys.AssDis, Sys.IniPath + "\\Data.ini");
                            }
                            catch
                            { }
                            #endregion
                            #region 临时信息(未用)
                            //string logpathTemp = Sys.ReportLog + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                            //if (!Directory.Exists(logpathTemp))
                            //    Directory.CreateDirectory(logpathTemp);
                            //if (!File.Exists(logpathTemp + "\\" + logfile))
                            //{
                            //    string Header = "Date\t" + "CreateTime\t" + "PowerSupplyTime-day\t" + "PowerSupplyTime-time\t" + "AutoOperationTime\t" +
                            //                    "AlarmPauseTime\t" + "WaitTime\t" + "AlarmCounter\t" + "ProductionCounter\t" + "Station\t" + "CycleTime\t" +
                            //                    "MachineId\t" + "ProductName\t" + "Height\t" + "Mode\t" + "R1TrayBarcode\t" + "R1TrayX-Y\t" + "LensBarcode\t" + "R3TrayBarcode\t" +
                            //                    "GlueBarcode\t" + "AVIResult\t" + "SetValue\t" + "MeasuredValue\t" + "ID\t" + "OD\t" + "GlueD\t" + "GlueWidth\t" +
                            //                    "DisResult\t" + "GlueDis1\t" + "GlueDis2\t" + "Result\t" + "DValue\t" + "DiamMin\t" +
                            //                    "AssShiftResult\t" + "AssShiftX\t" + "AssShiftY\t" + "AssShift" +
                            //                    "GlueCUTimeCH1(s)\t" + "GlueCUPressureCH1(Kpa)\t" + "GlueCUPressureCH1(Kpa)\t" +
                            //                    "GlueCUTimeCH2(s)\t" + "GlueCUPressureCH2(Kpa)\t" + "GlueCUPressureCH2(Kpa)\t" + "\r\n";
                            //    File.WriteAllText(logpathTemp + "\\" + logfile, Header);
                            //}
                            //try
                            //{
                            //    using (StreamWriter sw = new StreamWriter(logpathTemp + "\\" + logfile, true))
                            //    {
                            //        sw.WriteLine(DateTime.Now.ToString("yyyyMMdd") + "\t" + DateTime.Now.ToString("HH:mm:ss") + "\t" +
                            //                     PowerSupplyTime_Days + "\t" +
                            //                     string.Format("{0:d2}:{1:d2}:{2:d2}", PowerSupplyTime_Hours, PowerSupplyTime_Minutes, PowerSupplyTime_Seconds) + "\t" +
                            //                     string.Format("{0:d2}:{1:d2}:{2:d2}", AutoRunningTime_Hours, AutoRunningTime_Minutes, AutoRunningTime_Seconds) + "\t" +
                            //                     string.Format("{0:d2}:{1:d2}:{2:d2}", AlarmPauseTime_Hours, AlarmPauseTime_Minutes, AlarmPauseTime_Seconds) + "\t" +
                            //                     string.Format("{0:d2}:{1:d2}:{2:d2}", WaitTime_Hours, WaitTime_Minutes, WaitTime_Seconds) + "\t" +
                            //                     AlarmCount.ToString() + "\t" + ProductCount.ToString() + "\t" + PLC.Blocation.ToString() + "\t" +
                            //                     string.Format("{0:d2}:{1:d2}.{2:d2}", CycleTime / 6000, (CycleTime % 6000) / 100, (CycleTime % 6000) % 100) + "\t" +
                            //                     Sys.MachineId + "\t" + Sys.CurrentProduction + "\t" + AssemblyHeight + "\t" + "1\t" +
                            //                     RiReader.Barcode + "\t" + Tray_XY + "\t" + BarMessage + "\t" + Reader.Barcode + "\t" +
                            //                     Glue.Barcode + "\t" + Glue.GlueOutResult + "\t" + Glue.GlueOutAreaMax + "\t" + Glue.GlueOutArea + "\t" +
                            //                     Glue.ID + "\t" + Glue.OD + "\t" + Glue.GlueD + "\t" + Glue.GlueWidth + "\t" +
                            //                     Glue.GlueDisR + "\t" + Glue.GlueDis1 + "\t" + Glue.GlueDis2 + "\t" +
                            //                     Sys.CoatResult + "\t" + Sys.rmin.ToString() + "\t" + Sys.diam_min.ToString() + "\t" +
                            //                     Sys.P1Result + "\t" + Sys.AssDisX + "\t" + Sys.AssDisY + "\t" + Sys.AssDis + "\t" +
                            //                     GlueCU.arrayTime[0].ToString("0.000") + "\t" + GlueCU.arrayPressure[0].ToString("0.0") + "\t" + "-" + GlueCU.arrayPressure[10].ToString("0.0") + "\t" +
                            //                     GlueCU.arrayTime[1].ToString("0.000") + "\t" + GlueCU.arrayPressure[1].ToString("0.0") + "\t" + "-" + GlueCU.arrayPressure[11].ToString("0.0"));
                            //        sw.Flush();
                            //        sw.Close();
                            //        sw.Dispose();
                            //    }
                            //    //Pstation = "";
                            //    Sys.P1Result = ""; Sys.AssDisX = ""; Sys.AssDisY = ""; Sys.AssDis = "";
                            //    iniFile.Write("AssShift", "Result", Sys.P1Result, Sys.IniPath + "\\Data.ini");
                            //    iniFile.Write("AssShift", "X", Sys.AssDisX, Sys.IniPath + "\\Data.ini");
                            //    iniFile.Write("AssShift", "Y", Sys.AssDisY, Sys.IniPath + "\\Data.ini");
                            //    iniFile.Write("AssShift", "Dis", Sys.AssDis, Sys.IniPath + "\\Data.ini");
                            //}
                            //catch
                            //{ }
                            #endregion
                            #region 满盘捞取信息
                            if (ProductCount >= int.Parse(PLC.PlateCount))
                            {
                                string lpath = Sys.ReportLog1 + "\\" + LogDate;
                                logfile = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
                                string lFileName = lpath + "\\" + logfile;
                                string lnewFileName = lpath + "\\" + "Done_" + logfile;
                                if (File.Exists(lFileName))// && Transmission.code != ""
                                {
                                    if (!ModifyFilename(lFileName, lnewFileName))
                                    {
                                        try
                                        {
                                            if (File.Exists(lFileName))
                                            {
                                                ProductCount = 0;
                                                File.Move(lFileName, lnewFileName);
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    if (RiReader.BPFWebChecked)
                                    {
                                        BPFOut();
                                    }
                                }
                            }
                            #endregion
                            #endregion
                        }
                    }
                }
                Glue.GlueOutResult = "";
                Glue.GlueDis1 = ""; Glue.GlueDis2 = "";
                Sys.CoatResult = ""; Sys.rmin = 0.0; Sys.diam_min = 0.0;
                BarMessage = "";
                LensCountL = LensCount;
                Tray_XYLast = Tray_XY;
                #endregion
            }
            catch
            {
                //
            }
        }
        /// <summary>
        /// 報警log紀錄
        /// </summary>
        /// <param name="cmd"></param>
        void MSLogger(string cmd, out string AlarmMessageA, out string AlarmMessageACh)
        { 
            string mmm = cmd.Substring(cmd.Length - 2, 2);
            AlarmMessageA = "";
            AlarmMessageACh = "";
            if (mmm != "\r\n")
                return;
            string ala = ""; string[] alar = new string[15] { "", "", "", "", "", "", "", "", "", "", "", "","","","" };
            string AlarmMessage = "",AlarmMessageCh = "";
            for (int i = 0; i < 15; i++)
            {
                tempVar = Convert.ToInt16(cmd.Substring(i * 4 + 4, 4), 16);
                string ala0 = Convert.ToString(tempVar, 2);
                alar[i] = ala0.PadLeft(16, '0');
                ala = ala + alar[i];
            }
            for (int i = 0; i < ala.Length; i++)
            {
                AlarmMessage = ""; AlarmMessageCh = "";
                if (ala.Substring(i, 1) == "1")
                {
                    AlarmMessage = Alarray[i];
                    AlarmMessageCh = AlarrayCh[i];
                    if (AlarmMessage != "")
                    {
                        AlarmMessageA = ((AlarmMessageA == "") ? AlarmMessage : (AlarmMessageA + "," + AlarmMessage));
                        AlarmMessageACh = ((AlarmMessageACh == "") ? AlarmMessageCh : (AlarmMessageACh + "," + AlarmMessageCh));
                    }
                }
            }
            //alnum = ((AlarmMessageA != "") ? 1 : 0);
            //if (alnum == 0 & alnumL == 0 && Mchstatus[1])
            //{
            //    Process.Start(@"C:\EquipmentState\EquipmentState.exe", Sys.MachineId + ",Pause");
            //    MSChLogger("Pause");
            //}
            //if (alnum == 1 && alnumL == 0 && Mchstatus[1])
            //{
            //    alnum = 0;
            //Process.Start(@"C:\EquipmentState\EquipmentState.exe", Sys.MachineId + ",Down,Message=" + AlarmMessageA);
            //MSChLogger(AlarmMessageACh);
            //}
            //alnumL = alnum;
            iniFile.Write("PARAMETER", "ALARMLOGGER", alnumL.ToString(), setpath);
        }
        void MSChLogger(string strmessage)
        {
            string AlarmLogTime = DateTime.Now.ToString("yyyyMMdd");
            string Alogpath = Sys.AlarmPath;
            if (!Directory.Exists(Alogpath))
                Directory.CreateDirectory(Alogpath);
            string mslogfile = AlarmLogTime + "_" + Sys.MachineId + "_EquipAlarm.txt";
            if (!File.Exists(Alogpath + "\\" + mslogfile))
            {
                string Header = "Date\t" + "CreateTime\t" + "MachineID\t" + "EquipMessage\t\r\n";
                File.WriteAllText(Alogpath + "\\" + mslogfile, Header);
            }
            using (StreamWriter sw = new StreamWriter(Alogpath + "\\" + mslogfile, true))
            {
                sw.WriteLine(AlarmLogTime + "\t" + DateTime.Now.ToString("HH:mm:ss") + "\t" + Sys.MachineId + "\t" + strmessage);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            Curts = strmessage;
            if (strmessage != "Down" )
            {
                if (strmessage.Length > 4 & Mchstatus[3] & strmessage != "Pause")
                {
                    Curts = strmessage.Substring(0, 4);
                    if (Curts.Substring(0, 1) == "A")
                        Curts = "Down";
                }
                #region OEE
                try
                {
                    string Ls = iniFile.Read("OEE", "LastStatus", SysPath);
                    if (Ls != "" & Ls != Curts & Ls != "Run")
                    {
                        DateTime LastT = Convert.ToDateTime(iniFile.Read("OEE", "LastTime", SysPath));
                        DateTime CurT = DateTime.Now;
                        TimeSpan ts = CurT - LastT;
                        string onum = iniFile.Read("OEE", "ErrNum", SysPath);
                        if (onum == "" & Sys.FCTShow)
                            onum = "0";
                        if (onum != "")
                            Sys.CurErrMessage = OEEarray[int.Parse(onum)];
                        if (ts.TotalMinutes < 1)
                            Sys.CurErrMessage = "E1-设备故障";
                        if (Sys.CurErrMessage != "")
                        {
                            double releseT = ts.TotalSeconds;
                            int left1 = (int)(releseT);// % 86400
                            int Hours1 = left1 / 3600;
                            left1 = left1 % 3600;
                            int Minutes1 = left1 / 60;
                            int Seconds1 = left1 % 60;
                            string durTs = Hours1.ToString() + ":" + Minutes1.ToString() + ":" + Seconds1.ToString();
                            oeeTimeRunning = 0;
                            string path = "D:\\MB Mounter Report\\OEELog";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            //File
                            string file = Sys.MachineId + "_OEE_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                            if (!File.Exists(path + "\\" + file))
                            {
                                string Header = "MachineId\t" + "LastTime\t" + "CurrentTime\t" + "Duration\t" + "StopReason\t" + "LastMessage\t" + "CurrentMessage\t\r\n";
                                File.WriteAllText(path + "\\" + file, Header);
                            }
                            using (StreamWriter sw = new StreamWriter(path + "\\" + file, true))
                            {
                                sw.WriteLine(Sys.MachineId + "\t" + LastT.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + CurT.ToString("yyyy/MM/dd HH:mm:ss") + "\t" +
                                             durTs + "\t" + Sys.CurErrMessage + "\t" + Ls + "\t" + Curts);
                                sw.Flush();
                                sw.Close();
                                sw.Dispose();
                            }
                            Sys.CurErrMessage = "";
                            iniFile.Write("OEE", "ErrNum", "", SysPath);
                        }
                    }
                }
                catch
                {
                }
                #endregion
            }
            iniFile.Write("OEE", "LastStatus", Curts, SysPath);
            iniFile.Write("OEE", "LastTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), SysPath);
        }
        void OEELogger()
        {
            Sys.TimerOEE = false;
            int oeeMinutes = (int)(oeeTimeRunning / 60);
            if (oeeMinutes < 1)
                Sys.CurErrMessage = "E1-设备故障";
            string onum = iniFile.Read("OEE", "ErrNum", SysPath);
            if (onum != "")
                Sys.CurErrMessage = OEEarray[int.Parse(onum)];
            if (Sys.CurErrMessage != "")
            {
                oeeTimeRunning = 0;
                string Ls = iniFile.Read("OEE", "LastStatus", SysPath);
                DateTime LastT = Convert.ToDateTime(iniFile.Read("OEE", "LastTime", SysPath));
                DateTime CurT = DateTime.Now;
                TimeSpan ts = CurT - LastT;
                string path = "D:\\MB Mounter Report\\OEELog";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                //File
                string file = Sys.MachineId + "_OEE_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(path + "\\" + file))
                {
                    string Header = "MachineId\t" + "LastTime\t" + "CurrentTime\t" + "Duration\t" + "StopReason\t" + "LastMessage\t" + "CurrentMessage\t\r\n";
                    File.WriteAllText(path + "\\" + file, Header);
                }
                using (StreamWriter sw = new StreamWriter(path + "\\" + file, true))
                {
                    sw.WriteLine(Sys.MachineId + "\t" + LastT.ToString() + "\t" + CurT.ToString() + "\t" + ts.Minutes + "\t" + Sys.CurErrMessage + "\t" + Ls + "\t" + "Run");
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                Sys.CurErrMessage = "";
                iniFile.Write("OEE", "ErrNum", "", SysPath);
            }
        }
        void Mode5Logger(string CCDname)
        {

 
        }
        void temLogger(string cmd)
        {
            tempVar = Convert.ToInt32(cmd.Substring(8, 4) + cmd.Substring(4, 4), 16); //x1
            string x1 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            tempVar = Convert.ToInt32(cmd.Substring(16, 4) + cmd.Substring(12, 4), 16); //y1
            string y1 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            tempVar = Convert.ToInt32(cmd.Substring(24, 4) + cmd.Substring(20, 4), 16); //x2
            string x2 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            tempVar = Convert.ToInt32(cmd.Substring(32, 4) + cmd.Substring(28, 4), 16); //y2
            string y2 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            tempVar = Convert.ToInt32(cmd.Substring(40, 4) + cmd.Substring(36, 4), 16); //x3
            string x3 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            tempVar = Convert.ToInt32(cmd.Substring(48, 4) + cmd.Substring(44, 4), 16); //y3
            string y3 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            tempVar = Convert.ToInt32(cmd.Substring(56, 4) + cmd.Substring(52, 4), 16); //x4
            string x4 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            tempVar = Convert.ToInt32(cmd.Substring(64, 4) + cmd.Substring(60, 4), 16); //y4
            string y4 = (tempVar / 1000).ToString() + "." + (tempVar % 1000).ToString();
            string logfile = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_Pointloacation.txt";
            string logpath = Sys.ReportLog + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(logpath))
                Directory.CreateDirectory(logpath);
            if (!File.Exists(logpath + "\\" + logfile))
            {
                string Header = "Date\t" + "CreateTime\t" + "x1\t" + "y1\t" + "x2\t" +"y2\t" + "x3\t" + "y3\t" + "x4\t" + "y4\t\r\n";
                File.WriteAllText(logpath + "\\" + logfile, Header);
            }
            try
            {
                using (StreamWriter sw = new StreamWriter(logpath + "\\" + logfile, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyyMMdd") + "\t" + DateTime.Now.ToString("HH:mm:ss") + "\t" +
                                 x1 + "\t" +y1 + "\t" +x2 + "\t" +y2 + "\t" +x3 + "\t" +y3 + "\t" +x4 + "\t" +y4 );
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch
            { }

        }
        #endregion

        #region //影像
        //影像连接
        void LocalConnect()
        {
            while (true)
            {
                if (quit)
                    break;
                string strHostName = Dns.GetHostName();
                IPHostEntry localhost = Dns.GetHostEntry(strHostName);
                string strIP1 = "", strIP2 = "", strIP3 = "", strIP4 = "", strIP5 = "";
                string strIP6 = "", strIP7 = "", strIP8 = "", strIP9 = "", strIP10 = "", strIPw = "";
                for (int i = 0; i < localhost.AddressList.Count(); i++)
                {
                    #region GetIp
                    string localIP = localhost.AddressList[i].ToString();
                    if (localIP == A1CCD1.ip.ToString())
                        strIP1 = localIP.ToString();
                    if (localIP == A1CCD2.ip.ToString())
                        strIP2 = localIP.ToString();
                    if (localIP == A2CCD1.ip.ToString())
                        strIP3 = localIP.ToString();
                    if (localIP == A2CCD2.ip.ToString())
                        strIP4 = localIP.ToString();
                    if (localIP == PCCD1.ip.ToString())
                        strIP5 = localIP.ToString();
                    if (localIP == PCCD2.ip.ToString())
                        strIP6 = localIP.ToString();
                    if (localIP == GCCD1.ip.ToString())
                        strIP7 = localIP.ToString();
                    if (localIP == GCCD2.ip.ToString())
                        strIP8 = localIP.ToString();
                    if (localIP == QCCD.ip.ToString())
                        strIP9 = localIP.ToString();
                    if (localIP == Barcode1.ip.ToString())
                        strIP10 = localIP.ToString();
                    if (localIP.Length > 9)
                    {
                        if (localIP.Substring(0, 9) != "100.100.1" && localIP.Substring(0, 9) != "200.200.0"
                             && localIP.Substring(0, 10) != "192.168.1." && (localIP.Substring(3, 1) == "."||localIP.Substring(2, 1) == "."))
                            strIPw = localIP.ToString();
                    }
                    #endregion
                }
                #region A1CCD1
                if (strIP1 != "" && A1CCD1.IsCheck)
                {
                    Ping p1 = new Ping();
                    PingReply pr = p1.Send(IPAddress.Parse(strIP1), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!A1CCD1.IsConnected)
                        {
                            A1CCD1.IsConnected = true;
                            CloseTheImageProvider1();
                            Thread.Sleep(5);
                            OpenCam1();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider1.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider1.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(A1CCD1.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(A1CCD1.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        A1CCD1.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        A1CCD1.IsConnected = false;
                }
                else
                {
                    A1CCD1.IsConnected = false;
                }
                #endregion
                #region A1CCD2
                if (strIP2 != "" && A1CCD2.IsCheck)
                {
                    Ping p2 = new Ping();
                    PingReply pr = p2.Send(IPAddress.Parse(strIP2), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!A1CCD2.IsConnected)
                        {
                            A1CCD2.IsConnected = true;
                            CloseTheImageProvider2();
                            Thread.Sleep(5);
                            OpenCam2();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider2.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider2.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(A1CCD2.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(A1CCD2.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        A1CCD2.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        A1CCD2.IsConnected = false;
                }
                else
                {
                    A1CCD2.IsConnected = false;
                }
                #endregion
                #region A2CCD1
                if (strIP3 != "" && A2CCD1.IsCheck)
                {
                    Ping p3 = new Ping();
                    PingReply pr = p3.Send(IPAddress.Parse(strIP3), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!A2CCD1.IsConnected)
                        {
                            A2CCD1.IsConnected = true;
                            CloseTheImageProvider3();
                            Thread.Sleep(5);
                            OpenCam3();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider3.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider3.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(A2CCD1.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(A2CCD1.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        A2CCD1.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        A2CCD1.IsConnected = false;
                }
                else
                {
                    A2CCD1.IsConnected = false;
                }
                #endregion
                #region A2CCD2
                if (strIP4 != "" && A2CCD2.IsCheck)
                {
                    Ping p4 = new Ping();
                    PingReply pr = p4.Send(IPAddress.Parse(strIP4), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!A2CCD2.IsConnected)
                        {
                            A2CCD2.IsConnected = true;
                            CloseTheImageProvider4();
                            Thread.Sleep(5);
                            OpenCam4();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider4.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider4.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(A2CCD2.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(A2CCD2.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        A2CCD2.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        A2CCD2.IsConnected = false;
                }
                else
                {
                    A2CCD2.IsConnected = false;
                }
                #endregion
                #region PCCD1
                if (strIP5 != "" && PCCD1.IsCheck)
                {
                    Ping p5 = new Ping();
                    PingReply pr = p5.Send(IPAddress.Parse(strIP5), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!PCCD1.IsConnected)
                        {
                            PCCD1.IsConnected = true;
                            CloseTheImageProvider5();
                            Thread.Sleep(5);
                            OpenCam5();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider5.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider5.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(PCCD1.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(PCCD1.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        PCCD1.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        PCCD1.IsConnected = false;
                }
                else
                {
                    PCCD1.IsConnected = false;
                }
                #endregion
                #region PCCD2
                if (strIP6 != "" && PCCD2.IsCheck)
                {
                    Ping p6 = new Ping();
                    PingReply pr = p6.Send(IPAddress.Parse(strIP6), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!PCCD2.IsConnected)
                        {
                            PCCD2.IsConnected = true;
                            CloseTheImageProvider6();
                            Thread.Sleep(5);
                            OpenCam6();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider6.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider6.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(PCCD2.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(PCCD2.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        PCCD2.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        PCCD2.IsConnected = false;
                }
                else
                {
                    PCCD2.IsConnected = false;
                }
                #endregion
                #region GCCD1
                if (strIP7 != "" && GCCD1.IsCheck)
                {
                    Ping p7 = new Ping();
                    PingReply pr = p7.Send(IPAddress.Parse(strIP7), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!GCCD1.IsConnected)
                        {
                            GCCD1.IsConnected = true;
                            CloseTheImageProvider7();
                            Thread.Sleep(5);
                            OpenCam7();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider7.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider7.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(GCCD1.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(GCCD1.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        GCCD1.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        GCCD1.IsConnected = false;
                }
                else
                {
                    GCCD1.IsConnected = false;
                }
                #endregion
                #region GCCD2
                if (strIP8 != "" && GCCD2.IsCheck)
                {
                    Ping p8 = new Ping();
                    PingReply pr = p8.Send(IPAddress.Parse(strIP8), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!GCCD2.IsConnected)
                        {
                            GCCD2.IsConnected = true;
                            CloseTheImageProvider8();
                            Thread.Sleep(5);
                            OpenCam8();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider8.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider8.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(GCCD2.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(GCCD2.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        GCCD2.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        GCCD2.IsConnected = false;
                }
                else
                {
                    GCCD2.IsConnected = false;
                }
                #endregion
                #region QCCD
                if (strIP9 != "" && QCCD.IsCheck)
                {
                    Ping p9 = new Ping();
                    PingReply pr = p9.Send(IPAddress.Parse(strIP9), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!QCCD.IsConnected)
                        {
                            switch (QCCD.CCDBrand)
                            {
                                case 0:
                                    QCCD.IsConnected = true;
                                    CloseTheImageProvider9();
                                    Thread.Sleep(5);
                                    OpenCam9();
                                    try
                                    {
                                        Thread.Sleep(100);
                                        hnoteGain1 = m_imageProvider9.GetNodeFromDevice("GainRaw");
                                        hnoteExp1 = m_imageProvider9.GetNodeFromDevice("ExposureTimeRaw");
                                        GenApi.IntegerSetValue(hnoteGain1, int.Parse(QCCD.Gain));
                                        GenApi.IntegerSetValue(hnoteExp1, int.Parse(QCCD.ExposureTime));
                                    }
                                    catch (Exception er)
                                    {
                                        MessageBox.Show(er.ToString());
                                    }
                                    break;
                                case 1:
                                    if (!Barcode1.QCCDisChecked)
                                    {
                                        myHikvision.ip = IPAddress.Parse("100.100.19.2");
                                        myHikvision.Gain = double.Parse(QCCD.Gain);
                                        myHikvision.ExposureTime = double.Parse(QCCD.ExposureTime);
                                        myHikvision.TriggerMode = 0;
                                        myHikvision.OpenCamera();
                                        myHikvision.eventProcessImage += processHImage;
                                    }
                                    break;
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        QCCD.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        QCCD.IsConnected = false;
                }
                else
                {
                    QCCD.IsConnected = false;
                }
                #endregion
                #region Barcode1
                if (strIP10 != "" && Barcode1.IsChecked)
                {
                    Ping p10 = new Ping();
                    PingReply pr = p10.Send(IPAddress.Parse(strIP10), 1);
                    if (pr.Status == IPStatus.Success)
                    {
                        if (!Barcode1.IsConnected)
                        {
                            Barcode1.IsConnected = true;
                            CloseTheImageProvider10();
                            Thread.Sleep(5);
                            OpenCam10();
                            try
                            {
                                Thread.Sleep(100);
                                hnoteGain1 = m_imageProvider10.GetNodeFromDevice("GainRaw");
                                hnoteExp1 = m_imageProvider10.GetNodeFromDevice("ExposureTimeRaw");
                                GenApi.IntegerSetValue(hnoteGain1, int.Parse(Barcode1.Gain));
                                GenApi.IntegerSetValue(hnoteExp1, int.Parse(Barcode1.ExposureTime));
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                            }
                        }
                    }
                    if (pr.Status == IPStatus.HardwareError)
                        Barcode1.IsConnected = false;
                    if (pr.Status == IPStatus.DestinationHostUnreachable)
                        Barcode1.IsConnected = false;
                }
                else
                {
                    Barcode1.IsConnected = false;
                }
                #endregion
                #region GSEO
                if (strIPw != "")
                {
                    Ping p11 = new Ping();
                    PingReply pr = p11.Send(IPAddress.Parse(strIPw), 1);
                    if (pr.Status == IPStatus.Success)
                        GseoConn = true;
                    else
                        GseoConn = false;
                }
                #endregion
                Thread.Sleep(1000);
            }
        }
        #region Mirror变量
        HTuple hv_HomMat2DIdentity, hv_HomMat2DRotate;
        HObject ho_ImageAffinTrans = new HObject();
        HObject ho_ImageA11Mirror = new HObject(), ho_ImageA11Mirror1 = new HObject();
        HObject ho_ImageA12Mirror = new HObject(), ho_ImageA12Mirror1 = new HObject();
        HObject ho_ImageA21Mirror = new HObject(), ho_ImageA21Mirror1 = new HObject();
        HObject ho_ImageA22Mirror = new HObject(), ho_ImageA22Mirror1 = new HObject();
        HObject ho_ImageP1Mirror = new HObject(), ho_ImageP1Mirror1 = new HObject();
        HObject ho_ImageP2Mirror = new HObject(), ho_ImageP2Mirror1 = new HObject();
        HObject ho_ImageG1Mirror = new HObject(), ho_ImageG1Mirror1 = new HObject();
        HObject ho_ImageG2Mirror = new HObject(), ho_ImageG2Mirror1 = new HObject();
        HObject ho_ImageQMirror = new HObject(), ho_ImageQMirror1 = new HObject();
        public static DateTime afterDT;
        #endregion
        //HObject ho_HRectangle = null, ho_HImageReduced = null, ho_HImageResult = null;
        //HObject ho_HImagePart00 = null, ho_HImagePart20 = null, ho_HImageSub = null;
        //HTuple hv_HValue = new HTuple(), hv_HDeviation = new HTuple(); 
        bool P1shot2 = false;
        public static HTuple hv_FocusRow1 = null, hv_FocusColumn1 = null;
        public static HTuple hv_FocusRow2 = null, hv_FocusColumn2 = null;
        #region //Pylon1原碼
        private void OnGrabErrorEventCallback1(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback1), grabException, additionalErrorMessage);
                return;
            }
            ShowException1(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback1()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback1));
                return;
            }
            Stop1();
            CloseTheImageProvider1();
            UpdateDeviceList1();
        }
        private void OnDeviceOpenedEventCallback1()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback1));
                return;
            }
        }
        private void OnDeviceClosedEventCallback1()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback1));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback1()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback1));
                return;
            }
        }
        private void OnImageReadyEventCallback1()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback1));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider1.GetLatestImage();
               if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                     }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage1 = null;
                    HOperatorSet.GenEmptyObj(out theImage1);
                    theImage1.Dispose();

                    HTuple width, height;

                    BitmapToHObject1(m_bitmap, out theImage1);
                    DateTime beferDT_1 = System.DateTime.Now;
                    halcon.ImageOri[0] = theImage1;
                    HOperatorSet.CopyImage(theImage1, out halcon.ImageOri[0]);
                    halcon.Image[0] = theImage1;

                    ho_ImageA11Mirror.Dispose();
                    HOperatorSet.MirrorImage(theImage1, out ho_ImageA11Mirror, "row");
                    //ho_ImageA11Mirror.Dispose();
                    //HOperatorSet.MirrorImage(ho_ImageA11Mirror1, out ho_ImageA11Mirror, "column");
                    halcon.Image[0].Dispose();
                    HOperatorSet.CopyImage(ho_ImageA11Mirror, out halcon.Image[0]);
                    halcon.ImageOri[0].Dispose();
                    HOperatorSet.CopyImage(ho_ImageA11Mirror, out halcon.ImageOri[0]);
                    HOperatorSet.GetImageSize(halcon.ImageOri[0], out width, out height);
                    if (A1CCD1.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -A1CCD1.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);//width/2, height/2,
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[0], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant", "false");
                        halcon.Image[0].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[0]);
                    }
                    if (Sys.NoAutoMatic)
                    {
                        #region NoAuto
                        halcon.HWindowID[0] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[0], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[0], halcon.HWindowID[0]);
                        HD.ReadImage1(halcon.HWindowID[0]);
                        if (Sys.AssTest && Sys.AssLocation!="")
                            HD.ImagePro12(halcon.HWindowID[0]);
                        if (Sys.bCalView)
                        {
                            FrmVisionSet.xpm = A1CCD1.xpm;
                            FrmVisionSet.ypm = A1CCD1.ypm;
                            VisionSet.CalViewSet("A1CCD1");
                        }
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[0], width, height);
                        if (FrmVisionSet.Definition)
                        {
                            HD.ImageFocus(halcon.HWindowID[0], halcon.Image[0], width, height);
                            #region Focus
                            //HOperatorSet.GenEmptyObj(out ho_HRectangle);
                            //HOperatorSet.GenEmptyObj(out ho_HImageReduced);
                            //HOperatorSet.GenEmptyObj(out ho_HImageResult);
                            //HOperatorSet.GenEmptyObj(out ho_HImagePart00);
                            //HOperatorSet.GenEmptyObj(out ho_HImagePart20);
                            //HOperatorSet.GenEmptyObj(out ho_HImageSub);
                            ////Image Acquisition 01: Do something
                            //ho_HRectangle.Dispose();
                            //HOperatorSet.GenRectangle1(out ho_HRectangle, hv_FocusRow1, hv_FocusColumn1, hv_FocusRow2,hv_FocusColumn2);
                            //ho_HImageReduced.Dispose();
                            //HOperatorSet.ReduceDomain(halcon.Image[0], ho_HRectangle, out ho_HImageReduced);

                            //ho_HImagePart00.Dispose();
                            //HOperatorSet.CropPart(ho_HImageReduced, out ho_HImagePart00, 0, 0, width, height - 2);

                            //{
                            //    HObject ExpTmpOutVar_0;
                            //    HOperatorSet.ConvertImageType(ho_HImagePart00, out ExpTmpOutVar_0, "real");
                            //    ho_HImagePart00.Dispose();
                            //    ho_HImagePart00 = ExpTmpOutVar_0;
                            //}

                            //ho_HImagePart20.Dispose();
                            //HOperatorSet.CropPart(ho_HImageReduced, out ho_HImagePart20, 2, 0, width, height - 2);

                            //{
                            //    HObject ExpTmpOutVar_0;
                            //    HOperatorSet.ConvertImageType(ho_HImagePart20, out ExpTmpOutVar_0, "real");
                            //    ho_HImagePart20.Dispose();
                            //    ho_HImagePart20 = ExpTmpOutVar_0;
                            //}

                            //ho_HImageSub.Dispose();
                            //HOperatorSet.SubImage(ho_HImagePart20, ho_HImagePart00, out ho_HImageSub, 1,0);

                            //ho_HImageResult.Dispose();
                            //HOperatorSet.MultImage(ho_HImageSub, ho_HImageSub, out ho_HImageResult, 1, 0);

                            //HOperatorSet.Intensity(ho_HImageResult, ho_HImageResult, out hv_HValue, out hv_HDeviation);
                            //HOperatorSet.SetColor(halcon.HWindowID[0], "blue");
                            //HOperatorSet.SetDraw(halcon.HWindowID[0], "margin");
                            //HOperatorSet.DispObj(ho_HRectangle, halcon.HWindowID[0]);
                            //HOperatorSet.SetColor(halcon.HWindowID[0], "red");
                            ////HD.set_display_font(halcon.HWindowID[0], 30, "mono", "true", "false");
                            //HOperatorSet.SetTposition(halcon.HWindowID[0], 100, 0);
                            //HOperatorSet.WriteString(halcon.HWindowID[0], hv_HDeviation);
                            //HOperatorSet.WaitSeconds(0.01);
                            #endregion
                        }
                        m_imageProvider1.ReleaseImage();
                        #endregion
                    }
                    else
                    {
                        #region Auto
                        halcon.HWindowID[0] = Run.hWA1CCD1.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[0], 0, 0, height, width);
                        //HOperatorSet.DispObj(halcon.Image[0], halcon.HWindowID[0]);
                        FrmVisionSet.xpm = A1CCD1.xpm;
                        FrmVisionSet.ypm = A1CCD1.ypm;
                        if (Sys.A11autoS == 1)
                            HD.ImagePro1(halcon.HWindowID[0]);
                        if (Sys.A11autoS == 2)
                            HD.ImagePro13(halcon.HWindowID[0]);
                        if (Sys.A11autoS == 3)
                            HD.ImagePro1(halcon.HWindowID[0]);
                        m_imageProvider1.ReleaseImage();
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_1);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run.lblA11Time1.Text = time_1 + "ms";
                        Thread.Sleep(10);
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                ShowException1(e, m_imageProvider1.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback1()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback1));
                return;
            }
        }
        private void ShowException1(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList1()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();
                ListView.ListViewItemCollection items = deviceListView_1.Items;

                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }
                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_1.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];

                    if (device.Name.Substring(0, 6) == theCamIp[0])
                        theCamIndex[0] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        A1CCD1.IsConnected = false;
                        deviceListView_1.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException1(e, m_imageProvider1.GetLastErrorMessage());
            }
        }
        public void Stop1()
        {
            try
            {
                m_imageProvider1.Stop();
            }
            catch (Exception e)
            {
                ShowException1(e, m_imageProvider1.GetLastErrorMessage());
            }

        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject1(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider1()
        {
            try
            {
                m_imageProvider1.Close();
            }
            catch (Exception e)
            {
                //如果CCD斷線會報錯,不影響,先關閉此功能
                ShowException1(e, m_imageProvider1.GetLastErrorMessage());
            }
        }
        public void OneShot1()
        {
            try
            {
                m_imageProvider1.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException1(e, m_imageProvider1.GetLastErrorMessage());
            }
        }
        public void ContinuousShot1()
        {
            try
            {
                m_imageProvider1.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException1(e, m_imageProvider1.GetLastErrorMessage());
            }
        }
        #endregion
        #region//Pylon2原碼
        private void OnGrabErrorEventCallback2(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback2), grabException, additionalErrorMessage);
                return;
            }
            ShowException2(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback2()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback2));
                return;

            }
            Stop2();
            CloseTheImageProvider2();
            UpdateDeviceList2();
        }
        private void OnDeviceOpenedEventCallback2()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback2));
                return;
            }
        }
        private void OnDeviceClosedEventCallback2()
        {
            if (InvokeRequired)
            {
                 BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback2));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback2()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback2));
                return;
            }
        }
        private void OnImageReadyEventCallback2()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback2));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider2.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage2 = null;
                    HOperatorSet.GenEmptyObj(out theImage2);
                    theImage2.Dispose();

                    HTuple width, height;

                    BitmapToHObject2(m_bitmap, out theImage2);
                    DateTime beferDT_2 = System.DateTime.Now;
                    halcon.ImageOri[1] = theImage2;
                    HOperatorSet.CopyImage(theImage2, out halcon.ImageOri[1]);
                    halcon.Image[1] = theImage2;

                    #region 
                    //ho_ImageA12Mirror.Dispose();
                    //HOperatorSet.MirrorImage(theImage2, out ho_ImageA12Mirror, "row");
                    ////ho_ImageA12Mirror.Dispose();
                    ////HOperatorSet.MirrorImage(ho_ImageA12Mirror1, out ho_ImageA12Mirror, "column");
                    //halcon.Image[1].Dispose();
                    //HOperatorSet.CopyImage(ho_ImageA12Mirror, out halcon.Image[1]);
                    //halcon.ImageOri[1].Dispose();
                    //HOperatorSet.CopyImage(ho_ImageA12Mirror, out halcon.ImageOri[1]);
                    #endregion
                    HOperatorSet.GetImageSize(halcon.ImageOri[1], out width, out height);
                    if (A1CCD2.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -A1CCD2.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[1], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                             "false");
                        halcon.Image[1].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[1]);
                    }
                    if (Sys.NoAutoMatic)
                    {
                        #region NoAuto
                        halcon.HWindowID[1] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[1], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[1], halcon.HWindowID[1]);
                        HD.ReadImage2(halcon.HWindowID[1]);
                        if (Sys.AssTest && Sys.AssLocation != "")
                            HD.ImagePro22(halcon.HWindowID[1]);
                        if (Sys.bCalView)
                        {
                            FrmVisionSet.xpm = A1CCD2.xpm;
                            FrmVisionSet.ypm = A1CCD2.ypm;
                            VisionSet.CalViewSet("A1CCD2");
                        }
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[1], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[1], halcon.Image[1], width, height);
                        m_imageProvider2.ReleaseImage();
                        #endregion
                    }
                    else
                    {
                        halcon.HWindowID[1] = Run.hWA1CCD2.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[1], 0, 0, height, width);
                        //HOperatorSet.DispObj(halcon.Image[1], halcon.HWindowID[1]);
                        FrmVisionSet.xpm = A1CCD2.xpm;
                        FrmVisionSet.ypm = A1CCD2.ypm;
                        HD.ImagePro2(halcon.HWindowID[1]);
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_2);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run.lblA12Time1.Text = time_1 + "ms";
                        m_imageProvider2.ReleaseImage();
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException2(e, m_imageProvider2.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback2()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback2));
                return;
            }
        }
        private void ShowException2(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList2()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();

                ListView.ListViewItemCollection items = deviceListView_2.Items;
                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }

                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_2.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];
                    if (device.Name.Substring(0, 6) == theCamIp[1])
                        theCamIndex[1] = device.Index;
                }

                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        A1CCD2.IsConnected = false;
                        deviceListView_2.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException2(e, m_imageProvider2.GetLastErrorMessage());
            }
        }
        public void Stop2()
        {
            try
            {
                m_imageProvider2.Stop();
            }
            catch (Exception e)
            {
                ShowException2(e, m_imageProvider2.GetLastErrorMessage());
            }
        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject2(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider2()
        {
             try
            {
                m_imageProvider2.Close();
            }
            catch (Exception e)
            {
                ShowException2(e, m_imageProvider2.GetLastErrorMessage());
            }
        }
        public void OneShot2()
        {
            try
            {
                m_imageProvider2.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException2(e, m_imageProvider2.GetLastErrorMessage());
            }
        }
        public void ContinuousShot2()
        {
            try
            {
                m_imageProvider2.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException2(e, m_imageProvider2.GetLastErrorMessage());
            }
        }
        #endregion
        #region //Pylon3原碼
        private void OnGrabErrorEventCallback3(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback3), grabException, additionalErrorMessage);
                return;
            }
            ShowException3(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback3()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback3));
                return;
            }
            Stop3();
            CloseTheImageProvider3();
            UpdateDeviceList3();
        }
        private void OnDeviceOpenedEventCallback3()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback3));
                return;
            }
        }
        private void OnDeviceClosedEventCallback3()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback3));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback3()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback3));
                return;
            }
        }
        private void OnImageReadyEventCallback3()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback3));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider3.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage3 = null;
                    HOperatorSet.GenEmptyObj(out theImage3);
                    theImage3.Dispose();

                    HTuple width, height;

                    BitmapToHObject3(m_bitmap, out theImage3);
                    DateTime beferDT_1 = System.DateTime.Now;
                    halcon.ImageOri[2] = theImage3;
                    HOperatorSet.CopyImage(theImage3, out halcon.ImageOri[2]);
                    halcon.Image[2] = theImage3;

                    //ho_ImageA21Mirror.Dispose();
                    //HOperatorSet.MirrorImage(theImage3, out ho_ImageA21Mirror, "row");
                    ho_ImageA21Mirror.Dispose();
                    HOperatorSet.MirrorImage(theImage3, out ho_ImageA21Mirror, "column");
                    halcon.Image[2].Dispose();
                    HOperatorSet.CopyImage(ho_ImageA21Mirror, out halcon.Image[2]);
                    halcon.ImageOri[2].Dispose();
                    HOperatorSet.CopyImage(ho_ImageA21Mirror, out halcon.ImageOri[2]);
                    HOperatorSet.GetImageSize(halcon.ImageOri[2], out width, out height);
                    if (A2CCD1.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -A2CCD1.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[2], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                             "false");
                        halcon.Image[2].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[2]);
                    }

                    if (Sys.NoAutoMatic)
                    {
                        #region NoAuto
                        halcon.HWindowID[2] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[2], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[2], halcon.HWindowID[2]);
                        HD.ReadImage3(halcon.HWindowID[2]);
                        if (Sys.AssTest && Sys.AssLocation2 != "")
                            HD.ImagePro32(halcon.HWindowID[2]);
                        if (Sys.bCalView)
                        {
                            FrmVisionSet.xpm = A2CCD1.xpm;
                            FrmVisionSet.ypm = A2CCD1.ypm;
                            VisionSet.CalViewSet("A2CCD1");
                        }
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[2], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[2], halcon.Image[2], width, height);
                        m_imageProvider3.ReleaseImage();
                        #endregion
                    }
                    else
                    {
                        halcon.HWindowID[2] = Run.hWA2CCD1.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[2], 0, 0, height, width);
                        //HOperatorSet.DispObj(halcon.Image[2], halcon.HWindowID[2]);
                        FrmVisionSet.xpm = A2CCD1.xpm;
                        FrmVisionSet.ypm = A2CCD1.ypm;
                        if (Sys.A21autoS == 1)
                            HD.ImagePro3(halcon.HWindowID[2]);
                        if (Sys.A21autoS == 2)
                            HD.ImagePro33(halcon.HWindowID[2]);
                        if (Sys.A21autoS == 3)
                            HD.ImagePro3(halcon.HWindowID[2]);
                        m_imageProvider3.ReleaseImage();
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_1);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run.lblA21Time1.Text = time_1 + "ms";
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException3(e, m_imageProvider3.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback3()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback1));
                return;
            }
        }
        private void ShowException3(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList3()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();
                ListView.ListViewItemCollection items = deviceListView_3.Items;

                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }
                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_3.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];

                    if (device.Name.Substring(0, 6) == theCamIp[2])
                        theCamIndex[2] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        A2CCD1.IsConnected = false;
                        deviceListView_3.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException3(e, m_imageProvider3.GetLastErrorMessage());
            }
        }
        public void Stop3()
        {
            try
            {
                m_imageProvider3.Stop();
            }
            catch (Exception e)
            {
                ShowException3(e, m_imageProvider3.GetLastErrorMessage());
            }

        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject3(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider3()
        {
            try
            {
                m_imageProvider3.Close();
            }
            catch (Exception e)
            {
                ShowException3(e, m_imageProvider3.GetLastErrorMessage());
            }
        }
        public void OneShot3()
        {
            try
            {
                m_imageProvider3.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException3(e, m_imageProvider3.GetLastErrorMessage());
            }
        }
        public void ContinuousShot3()
        {
            try
            {
                m_imageProvider3.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException3(e, m_imageProvider3.GetLastErrorMessage());
            }
        }
        #endregion
        #region//Pylon4原碼
        private void OnGrabErrorEventCallback4(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback4), grabException, additionalErrorMessage);
                return;
            }
            ShowException4(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback4()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback4));
                return;
            }
            Stop4();
            CloseTheImageProvider4();
            UpdateDeviceList4();
        }
        private void OnDeviceOpenedEventCallback4()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback4));
                return;
            }
        }
        private void OnDeviceClosedEventCallback4()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback4));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback4()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback4));
                return;
            }
        }
        private void OnImageReadyEventCallback4()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback4));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider4.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage4 = null;
                    HOperatorSet.GenEmptyObj(out theImage4);
                    theImage4.Dispose();

                    HTuple width, height;

                    BitmapToHObject4(m_bitmap, out theImage4);
                    DateTime beferDT_2 = System.DateTime.Now;
                    halcon.ImageOri[3] = theImage4;
                    HOperatorSet.CopyImage(theImage4, out halcon.ImageOri[3]);
                    halcon.Image[3] = theImage4;

                    ho_ImageA22Mirror1.Dispose();
                    HOperatorSet.MirrorImage(theImage4, out ho_ImageA22Mirror1, "row");
                    ho_ImageA22Mirror.Dispose();
                    HOperatorSet.MirrorImage(ho_ImageA22Mirror1, out ho_ImageA22Mirror, "column");
                    halcon.Image[3].Dispose();
                    HOperatorSet.CopyImage(ho_ImageA22Mirror, out halcon.Image[3]);
                    halcon.ImageOri[3].Dispose();
                    HOperatorSet.CopyImage(ho_ImageA22Mirror, out halcon.ImageOri[3]);
                    HOperatorSet.GetImageSize(halcon.ImageOri[3], out width, out height);
                    if (A2CCD2.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -A2CCD2.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[3], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                             "false");
                        halcon.Image[3].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[3]);
                    }

                    if (Sys.NoAutoMatic)
                    {
                        #region 
                        halcon.HWindowID[3] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[3], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[3], halcon.HWindowID[3]);
                        HD.ReadImage4(halcon.HWindowID[3]);
                        if (Sys.AssTest && Sys.AssLocation != "")
                            HD.ImagePro42(halcon.HWindowID[3]);
                        if (Sys.bCalView)
                        {
                            FrmVisionSet.xpm = A2CCD2.xpm;
                            FrmVisionSet.ypm = A2CCD2.ypm;
                            VisionSet.CalViewSet("A2CCD2");
                        }
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[3], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[3], halcon.Image[3], width, height);
                        #endregion
                        m_imageProvider4.ReleaseImage();
                    }
                    else
                    {
                        halcon.HWindowID[3] = Run.hWA2CCD2.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[3], 0, 0, height, width);
                        //HOperatorSet.DispObj(halcon.Image[3], halcon.HWindowID[3]);
                        FrmVisionSet.xpm = A2CCD2.xpm;
                        FrmVisionSet.ypm = A2CCD2.ypm;
                        HD.ImagePro4(halcon.HWindowID[3]);
                        m_imageProvider4.ReleaseImage();
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_2);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run.lblA22Time1.Text = time_1 + "ms";
                        Thread.Sleep(20);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException4(e, m_imageProvider4.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback4()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback4));
                return;
            }
        }
        private void ShowException4(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList4()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();

                ListView.ListViewItemCollection items = deviceListView_4.Items;
                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }

                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_4.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];
                    if (device.Name.Substring(0, 6) == theCamIp[3])
                        theCamIndex[3] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        A2CCD2.IsConnected = false;
                        deviceListView_4.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException4(e, m_imageProvider4.GetLastErrorMessage());
            }
        }
        public void Stop4()
        {
            try
            {
                m_imageProvider4.Stop();
            }
            catch (Exception e)
            {
                ShowException4(e, m_imageProvider4.GetLastErrorMessage());
            }
        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject4(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider4()
        {
            try
            {
                m_imageProvider4.Close();
            }
            catch (Exception e)
            {
                ShowException4(e, m_imageProvider4.GetLastErrorMessage());
            }
        }
        public void OneShot4()
        {
            try
            {
                m_imageProvider4.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException4(e, m_imageProvider4.GetLastErrorMessage());
            }
        }
        public void ContinuousShot4()
        {
            try
            {
                m_imageProvider4.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException4(e, m_imageProvider4.GetLastErrorMessage());
            }
        }
        #endregion
        #region //Pylon5原碼
        private void OnGrabErrorEventCallback5(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback5), grabException, additionalErrorMessage);
                return;
            }
            ShowException5(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback5()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback5));
                return;
            }
            Stop5();
            CloseTheImageProvider5();
            UpdateDeviceList5();
        }
        private void OnDeviceOpenedEventCallback5()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback5));
                return;
            }
        }
        private void OnDeviceClosedEventCallback5()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback5));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback5()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback5));
                return;
            }
        }
        private void OnImageReadyEventCallback5()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback5));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider5.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage5 = null;
                    HOperatorSet.GenEmptyObj(out theImage5);
                    theImage5.Dispose();

                    HTuple width, height;

                    BitmapToHObject1(m_bitmap, out theImage5);
                    DateTime beferDT_1 = System.DateTime.Now;
                    halcon.ImageOri[4] = theImage5;
                    HOperatorSet.CopyImage(theImage5, out halcon.ImageOri[4]);
                    halcon.Image[4] = theImage5;

                    //ho_ImageP1Mirror.Dispose();
                    //HOperatorSet.MirrorImage(theImage5, out ho_ImageP1Mirror, "row");
                    ho_ImageP1Mirror.Dispose();
                    HOperatorSet.MirrorImage(theImage5, out ho_ImageP1Mirror, "column");
                    halcon.Image[4].Dispose();
                    HOperatorSet.CopyImage(ho_ImageP1Mirror, out halcon.Image[4]);
                    halcon.ImageOri[4].Dispose();
                    HOperatorSet.CopyImage(ho_ImageP1Mirror, out halcon.ImageOri[4]);
                    HOperatorSet.GetImageSize(halcon.ImageOri[4], out width, out height);
                    if (PCCD1.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -PCCD1.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[4], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                             "false");
                        halcon.Image[4].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[4]);
                    }

                    if (Sys.NoAutoMatic)
                    {
                        #region 
                        halcon.HWindowID[4] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[4], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[4], halcon.HWindowID[4]);
                        if (Sys.AssTest && Sys.AssLocation2 != "")
                            HD.ImagePro52(halcon.HWindowID[4]);
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[4], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[4], halcon.Image[4], width, height);
                        #endregion
                        m_imageProvider5.ReleaseImage();
                    }
                    else
                    {
                        halcon.HWindowID[4] = Run2.hWPCCD1.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[4], 0, 0, height, width);
                        FrmVisionSet.xpm = PCCD1.xpm;
                        FrmVisionSet.ypm = PCCD1.ypm;
                        Sys.P1DisMode2 = (iniFile.Read("PCCD1", "DisMode2Checked", propath) == "True" ? true : false);
                        if (Sys.P1autoS || P1shot2)
                        {
                            if (Sys.P1DisMode2)
                            {
                                Sys.P1autoS = false;
                                P1shot2 = false;
                                HD.ImagePro5(halcon.HWindowID[4]); //Mode2
                            }
                            else
                            {
                                Sys.P1autoS = false;
                                P1shot2 = false;
                                HD.ImagePro53(halcon.HWindowID[4]);  //Mode1
                            }
                        }
                        m_imageProvider5.ReleaseImage();
                        DateTime afterDT_1 = System.DateTime.Now;
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_1);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run2.lblP1Time.Text = time_1 + "ms";
                        Thread.Sleep(10);
                    }
                    if (Sys.AssLocation == "Hold" & !Sys.P1DisMode2)
                    {
                        P1shot2 = true;
                        Sys.AssLocation = "Lens";
                        #region Light
                        string l1 = iniFile.Read("PCCD1-Lens", "LighterValue1", propath);
                        string l2 = iniFile.Read("PCCD1-Lens", "LighterValue2", propath);
                        if (l1 != "")
                        {
                            brit = int.Parse(l1); ch = 0;
                            LightSet();
                            Thread.Sleep(5);
                            brit = int.Parse(l2); ch = 1;
                            LightSet();
                        }
                        #endregion
                        OneShot5();
                    }
                }
            }
            catch (Exception e)
            {
                ShowException5(e, m_imageProvider5.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback5()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback5));
                return;
            }
        }
        private void ShowException5(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList5()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();
                ListView.ListViewItemCollection items = deviceListView_5.Items;

                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }
                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_5.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];

                    if (device.Name.Substring(0, 5) == theCamIp[4])
                        theCamIndex[4] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        PCCD1.IsConnected = false;
                        deviceListView_5.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException5(e, m_imageProvider5.GetLastErrorMessage());
            }
        }
        public void Stop5()
        {
            try
            {
                m_imageProvider5.Stop();
            }
            catch (Exception e)
            {
                ShowException1(e, m_imageProvider5.GetLastErrorMessage());
            }

        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject5(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider5()
        {
            try
            {
                m_imageProvider5.Close();
            }
            catch (Exception e)
            {
                ShowException5(e, m_imageProvider5.GetLastErrorMessage());
            }
        }
        public void OneShot5()
        {
            try
            {
                m_imageProvider5.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException5(e, m_imageProvider5.GetLastErrorMessage());
            }
        }
        public void ContinuousShot5()
        {
            try
            {
                m_imageProvider5.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException5(e, m_imageProvider5.GetLastErrorMessage());
            }
        }
        #endregion
        #region//Pylon6原碼
        private void OnGrabErrorEventCallback6(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback6), grabException, additionalErrorMessage);
                return;
            }
            ShowException6(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback6()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback6));
                return;

            }
            Stop6();
            CloseTheImageProvider6();
            UpdateDeviceList6();
        }
        private void OnDeviceOpenedEventCallback6()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback6));
                return;
            }
        }
        private void OnDeviceClosedEventCallback6()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback6));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback6()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback6));
                return;
            }
        }
        private void OnImageReadyEventCallback6()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback6));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider6.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage6 = null;
                    HOperatorSet.GenEmptyObj(out theImage6);
                    theImage6.Dispose();

                    HTuple width, height;

                    BitmapToHObject6(m_bitmap, out theImage6);
                    DateTime beferDT_2 = System.DateTime.Now;
                    halcon.ImageOri[5] = theImage6;
                    HOperatorSet.CopyImage(theImage6, out halcon.ImageOri[5]);
                    halcon.Image[5] = theImage6;

                    #region
                    //ho_ImageP2Mirror.Dispose();
                    //HOperatorSet.MirrorImage(theImage6, out ho_ImageP2Mirror, "row");
                    ////ho_ImageP2Mirror.Dispose();
                    ////HOperatorSet.MirrorImage(ho_ImageP2Mirror1, out ho_ImageP2Mirror, "column");
                    //halcon.Image[5].Dispose();
                    //HOperatorSet.CopyImage(ho_ImageP2Mirror, out halcon.Image[5]);
                    //halcon.ImageOri[5].Dispose();
                    //HOperatorSet.CopyImage(ho_ImageP2Mirror, out halcon.ImageOri[5]);
                    #endregion
                    HOperatorSet.GetImageSize(halcon.ImageOri[5], out width, out height);
                    if (PCCD2.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -PCCD2.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[5], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                             "false");
                        halcon.Image[5].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[5]);
                    }

                    if (Sys.NoAutoMatic)
                    {
                        #region
                        halcon.HWindowID[5] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[5], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[5], halcon.HWindowID[5]);
                        HD.ReadImage6(halcon.HWindowID[5]);
                        if (PCCD2.IntSingle == 2 || PCCD2.IntSingle==3)
                        {
                            FrmVisionSet.xpm = 0.00441;
                            FrmVisionSet.ypm = PCCD2.ypm;
                            HD.ImagePro6(halcon.HWindowID[5]);
                        }
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[5], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[5], halcon.Image[5], width, height);
                        #endregion
                        m_imageProvider6.ReleaseImage();
                    }
                    else
                    {
                        halcon.HWindowID[5] = Run2.hWPCCD2.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[5], 0, 0, height, width);
                        //HOperatorSet.DispObj(halcon.Image[5], halcon.HWindowID[5]);
                        FrmVisionSet.xpm = PCCD2.xpm;
                        FrmVisionSet.ypm = PCCD2.ypm;
                        HD.ImagePro6(halcon.HWindowID[5]);
                        DateTime afterDT_2 = System.DateTime.Now;
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_2);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run2.lblP2Time.Text = time_1 + "ms";
                        m_imageProvider6.ReleaseImage();
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException6(e, m_imageProvider6.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback6()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback2));
                return;
            }
        }
        private void ShowException6(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList6()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();

                ListView.ListViewItemCollection items = deviceListView_6.Items;
                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }

                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_6.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];
                    if (device.Name.Substring(0, 5) == theCamIp[5])
                        theCamIndex[5] = device.Index;
                }

                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        PCCD2.IsConnected = false;
                        deviceListView_6.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException6(e, m_imageProvider6.GetLastErrorMessage());
            }
        }
        public void Stop6()
        {
            try
            {
                m_imageProvider6.Stop();
            }
            catch (Exception e)
            {
                ShowException6(e, m_imageProvider6.GetLastErrorMessage());
            }
        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject6(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider6()
        {
            try
            {
                m_imageProvider6.Close();
            }
            catch (Exception e)
            {
                ShowException6(e, m_imageProvider6.GetLastErrorMessage());
            }
        }
        public void OneShot6()
        {
            try
            {
                m_imageProvider6.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException6(e, m_imageProvider6.GetLastErrorMessage());
            }
        }
        public void ContinuousShot6()
        {
            try
            {
                m_imageProvider6.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException6(e, m_imageProvider6.GetLastErrorMessage());
            }
        }
        #endregion
        #region //Pylon7原碼
        private void OnGrabErrorEventCallback7(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback7), grabException, additionalErrorMessage);
                return;
            }
            ShowException7(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback7()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback7));
                return;
            }
            Stop7();
            CloseTheImageProvider7();
            UpdateDeviceList7();
        }
        private void OnDeviceOpenedEventCallback7()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback7));
                return;
            }
        }
        private void OnDeviceClosedEventCallback7()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback7));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback7()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback7));
                return;
            }
        }
        private void OnImageReadyEventCallback7()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback7));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider7.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage7 = null;
                    HOperatorSet.GenEmptyObj(out theImage7);
                    theImage7.Dispose();

                    HTuple width, height;

                    BitmapToHObject7(m_bitmap, out theImage7);
                    DateTime beferDT_1 = System.DateTime.Now;
                    halcon.ImageOri[6] = theImage7;
                    HOperatorSet.CopyImage(theImage7, out halcon.ImageOri[6]);
                    halcon.Image[6] = theImage7;

                    #region 
                    //ho_ImageG1Mirror.Dispose();
                    //HOperatorSet.MirrorImage(theImage7, out ho_ImageG1Mirror, "row");
                    ////ho_ImageG1Mirror.Dispose();
                    ////HOperatorSet.MirrorImage(ho_ImageG1Mirror1, out ho_ImageG1Mirror, "column");
                    //halcon.Image[6].Dispose();
                    //HOperatorSet.CopyImage(ho_ImageG1Mirror, out halcon.Image[6]);
                    //halcon.ImageOri[6].Dispose();
                    //HOperatorSet.CopyImage(ho_ImageG1Mirror, out halcon.ImageOri[6]);
                    #endregion
                    HOperatorSet.GetImageSize(halcon.ImageOri[6], out width, out height);
                    if (GCCD1.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -GCCD1.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[6], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                             "false");
                        halcon.Image[6].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[6]);
                    }

                    if (Sys.NoAutoMatic)
                    {
                        #region 
                        halcon.HWindowID[6] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[6], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[6], halcon.HWindowID[6]);
                        HD.ReadImage7(halcon.HWindowID[6]);
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[6], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[6], halcon.Image[6], width, height);
                        #endregion
                        m_imageProvider7.ReleaseImage();
                    }
                    else
                    {
                        halcon.HWindowID[6] = Run2.hWGCCD1.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[6], 0, 0, height, width);
                        //HOperatorSet.DispObj(halcon.Image[6], halcon.HWindowID[6]);
                        FrmVisionSet.xpm = GCCD1.xpm;
                        FrmVisionSet.ypm = GCCD1.ypm;
                        HWindow Window = halcon.HWindowID[6];
                        HObject ho_ResultImage = new HObject();
                        //針頭檢測
                        int ImageProcessResult = HD.ImageProcess_NeedleTipTest(Window, halcon.Image[6], out ho_ResultImage);
                        string TESTtime = DateTime.Now.ToString("HHmmss");
                        FrmMain.afterDT = System.DateTime.Now;
                        if (ImageProcessResult == 1)
                        {
                            WriteToPlc.CMDOKNG[6] = "0001";
                        }
                        else
                        {
                            WriteToPlc.CMDOKNG[6] = "0002";
                        }
                        WriteToPlc.CMDresult[6] = "000000000000000000000000";
                        WriteToPlc.CMDsend[6] = true;
                        //if (saveOPic)
                        //    HOperatorSet.WriteImage(halcon.Image[6], "png", -1, ImagePath + "\\NG\\OriginalImage\\" + TESTtime);
                        //if (saveRPic)
                        //    HOperatorSet.DumpWindow(Window, "png", ImagePath + "\\NG\\ResultImage\\" + TESTtime);
                        FrmMain.processing[6] = false;
                        //HD.ImagePro7(halcon.HWindowID[6]);
                        m_imageProvider7.ReleaseImage();
                        DateTime afterDT_1 = System.DateTime.Now;
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_1);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run2.lblG1Time.Text = time_1 + "ms";
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException7(e, m_imageProvider7.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback7()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback7));
                return;
            }
        }
        private void ShowException7(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList7()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();
                ListView.ListViewItemCollection items = deviceListView_7.Items;

                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }
                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_7.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];

                    if (device.Name.Substring(0, 5) == theCamIp[6])
                        theCamIndex[6] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        GCCD1.IsConnected = false;
                        deviceListView_7.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException7(e, m_imageProvider7.GetLastErrorMessage());
            }
        }
        public void Stop7()
        {
            try
            {
                m_imageProvider7.Stop();
            }
            catch (Exception e)
            {
                ShowException3(e, m_imageProvider7.GetLastErrorMessage());
            }

        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject7(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider7()
        {
            try
            {
                m_imageProvider7.Close();
            }
            catch (Exception e)
            {
                ShowException7(e, m_imageProvider7.GetLastErrorMessage());
            }
        }
        public void OneShot7()
        {
            try
            {
                m_imageProvider7.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException7(e, m_imageProvider7.GetLastErrorMessage());
            }
        }
        public void ContinuousShot7()
        {
            try
            {
                m_imageProvider7.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException7(e, m_imageProvider7.GetLastErrorMessage());
            }
        }
        #endregion
        #region//Pylon8原碼
        private void OnGrabErrorEventCallback8(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback8), grabException, additionalErrorMessage);
                return;
            }
            ShowException8(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback8()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback8));
                return;
            }
            Stop8();
            CloseTheImageProvider8();
            UpdateDeviceList8();
        }
        private void OnDeviceOpenedEventCallback8()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback8));
                return;
            }
        }
        private void OnDeviceClosedEventCallback8()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback8));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback8()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback8));
                return;
            }
        }
        private void OnImageReadyEventCallback8()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback8));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider8.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage8 = null;
                    HOperatorSet.GenEmptyObj(out theImage8);
                    theImage8.Dispose();

                    HTuple width, height;

                    BitmapToHObject8(m_bitmap, out theImage8);
                    DateTime beferDT_2 = System.DateTime.Now;
                    halcon.ImageOri[7] = theImage8;
                    HOperatorSet.CopyImage(theImage8, out halcon.ImageOri[7]);
                    halcon.Image[7] = theImage8;

                    ho_ImageG2Mirror1.Dispose();
                    HOperatorSet.MirrorImage(theImage8, out ho_ImageG2Mirror1, "row");
                    ho_ImageG2Mirror.Dispose();
                    HOperatorSet.MirrorImage(ho_ImageG2Mirror1, out ho_ImageG2Mirror, "column");
                    halcon.Image[7].Dispose();
                    HOperatorSet.CopyImage(ho_ImageG2Mirror, out halcon.Image[7]);
                    halcon.ImageOri[7].Dispose();
                    HOperatorSet.CopyImage(ho_ImageG2Mirror, out halcon.ImageOri[7]);
                    HOperatorSet.GetImageSize(halcon.ImageOri[7], out width, out height);
                    if (GCCD2.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -GCCD2.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.ImageOri[7], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant", "false");
                        halcon.Image[7].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[7]);
                    }
                    if (Sys.NoAutoMatic)
                    {
                        #region NoAuto
                        halcon.HWindowID[7] = VisionSet.hWImageSet.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[7], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[7], halcon.HWindowID[7]);
                        HD.ReadImage8(halcon.HWindowID[7]);
                        if (VisionSet.ViewNum == "8" && VisionSet.cBGlueCir.Checked)
                        {
                            VisionSet.readtemPara("GCCD2GluePoint");
                            VisionSet.CorShow();
                            VisionSet.txtGluePointX.Text = Math.Round(VisionSet.Xb, 3).ToString(); 
                            VisionSet.txtGluePointY.Text = Math.Round(VisionSet.Yb, 3).ToString();
                        }
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[7], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[7], halcon.Image[7], width, height);
                        m_imageProvider8.ReleaseImage();
                        #endregion
                    }
                    else
                    {
                        halcon.HWindowID[7] = Run2.hWGCCD2.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[7], 0, 0, height, width);
                        FrmVisionSet.xpm = GCCD2.xpm;
                        FrmVisionSet.ypm = GCCD2.ypm;
                        HD.ImagePro8(halcon.HWindowID[7]);
                        DateTime afterDT_2 = System.DateTime.Now;
                        TimeSpan ts_1 = afterDT.Subtract(beferDT_2);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        Run2.lblG2Time.Text = time_1 + "ms";
                        m_imageProvider8.ReleaseImage();
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException8(e, m_imageProvider8.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback8()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback8));
                return;
            }
        }
        private void ShowException8(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList8()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();

                ListView.ListViewItemCollection items = deviceListView_8.Items;
                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }

                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_8.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];
                    if (device.Name.Substring(0, 5) == theCamIp[7])
                        theCamIndex[7] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        GCCD2.IsConnected = false;
                        deviceListView_8.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException8(e, m_imageProvider8.GetLastErrorMessage());
            }
        }
        public void Stop8()
        {
            try
            {
                m_imageProvider8.Stop();
            }
            catch (Exception e)
            {
                ShowException8(e, m_imageProvider8.GetLastErrorMessage());
            }
        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject8(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider8()
        {
            try
            {
                m_imageProvider8.Close();
            }
            catch (Exception e)
            {
                ShowException8(e, m_imageProvider8.GetLastErrorMessage());
            }
        }
        public void OneShot8()
        {
            try
            {
                m_imageProvider8.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException8(e, m_imageProvider8.GetLastErrorMessage());
            }
        }
        public void ContinuousShot8()
        {
            try
            {
                m_imageProvider8.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException8(e, m_imageProvider8.GetLastErrorMessage());
            }
        }
        #endregion
        #region//Pylon9原碼
        private void OnGrabErrorEventCallback9(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback9), grabException, additionalErrorMessage);
                return;
            }
            ShowException9(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback9()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback9));
                return;
            }
            Stop9();
            CloseTheImageProvider9();
            UpdateDeviceList9();
        }
        private void OnDeviceOpenedEventCallback9()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback9));
                return;
            }
        }
        private void OnDeviceClosedEventCallback9()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback9));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback9()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback9));
                return;
            }
        }
        private void OnImageReadyEventCallback9()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback9));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider9.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    HObject theImage9 = null;
                    HOperatorSet.GenEmptyObj(out theImage9);
                    theImage9.Dispose();
                    HTuple width, height;
                    BitmapToHObject9(m_bitmap, out theImage9);
                    DateTime beferDT_2 = System.DateTime.Now;
                    halcon.ImageOri[8] = theImage9;
                    halcon.Image[8] = theImage9;
                    HOperatorSet.GetImageSize(halcon.ImageOri[8], out width, out height);
                    if (QCCD.angleC != 0 && !halcon.AIsChecked)
                    {
                        double ba = -QCCD.angleC * (Math.PI / 180);
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                        ho_ImageAffinTrans.Dispose();
                        HOperatorSet.AffineTransImage(halcon.Image[8], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                             "false");
                        halcon.Image[8].Dispose();
                        HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[8]);
                    }
                    if (Sys.NoAutoMatic)
                    {
                        #region
                        if (!Barcode1.QCCDisChecked)
                            halcon.HWindowID[8] = VisionSet.hWImageSet.HalconWindow;
                        else
                            halcon.HWindowID[8] = Fbarcode.hWindowControl1.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[8], 0, 0, height, width);
                        HOperatorSet.DispObj(halcon.Image[8], halcon.HWindowID[8]);

                        HD.ReadImage9(halcon.HWindowID[8]);
                        if (halcon.IsCrossDraw)
                            HD.CrossDraw(halcon.HWindowID[8], width, height);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(halcon.HWindowID[8], halcon.Image[8], width, height);
                        #endregion
                    }
                    else
                    {
                        if (!Barcode1.QCCDisChecked)
                        {
                            #region NoBarcode
                            halcon.HWindowID[8] = Run2.hWQCCD.HalconWindow;
                            HOperatorSet.SetPart(halcon.HWindowID[8], 0, 0, height, width);
                            FrmVisionSet.xpm = QCCD.xpm;
                            FrmVisionSet.ypm = QCCD.ypm;
                            HD.ImagePro9(halcon.HWindowID[8]);
                            DateTime afterDT_2 = System.DateTime.Now;
                            TimeSpan ts_1 = afterDT.Subtract(beferDT_2);
                            string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                            Run2.lblQTime.Text = time_1 + "ms";
                            m_imageProvider9.ReleaseImage();
                            Thread.Sleep(10);
                            #endregion
                        }
                    }
                    if (Barcode1.QCCDisChecked)
                    {
                        #region barcode
                        HObject BartheImage = new HObject();
                        HOperatorSet.GenEmptyObj(out BartheImage);
                        BartheImage = halcon.Image[8];
                        HTuple ImageWindow = new HTuple(); HWindow barWindow = new HWindow();
                        if (Sys.NoAutoMatic)
                        {
                            ImageWindow = Fbarcode.hWindowControl1.HalconWindow;
                            barWindow = Fbarcode.hWindowControl1.HalconWindow;
                        }
                        else
                            ImageWindow = Run2.hWQCCD.HalconWindow;
                        HOperatorSet.GetImageSize(BartheImage, out width, out height);
                        HOperatorSet.SetPart(ImageWindow, 0, 0, height, width);
                        HOperatorSet.DispObj(BartheImage, ImageWindow);
                        if (FrmVisionSet.Definition)
                            HD.ImageFocus(barWindow, BartheImage, width, height);
                        #region barcodeResult
                        /*畫掃碼範圍*/
                        if (Barcode1.bBarcodeRangeSearch)
                        {
                            Barcode1.bBarcodeRangeSearch = false;
                            HD.SearchRange(ImageWindow, BartheImage, out Barcode1.HandleRow1, out Barcode1.HandleCol1, out Barcode1.HandleRow2, out Barcode1.HandleCol2);
                            try
                            {
                                iniFile.Write("BarcodeReader", "SearchRangeRow1", Barcode1.HandleRow1.ToString(), BarPath);
                                iniFile.Write("BarcodeReader", "SearchRangeCol1", Barcode1.HandleCol1.ToString(), BarPath);
                                iniFile.Write("BarcodeReader", "SearchRangeRow2", Barcode1.HandleRow2.ToString(), BarPath);
                                iniFile.Write("BarcodeReader", "SearchRangeCol2", Barcode1.HandleCol2.ToString(), BarPath);
                                MessageBox.Show("扫码范围已存储。");
                            }
                            catch
                            {}
                        }
                        BarMessage = "";
                        Barcode1.theBarImage.Dispose();
                        HOperatorSet.CopyImage(BartheImage, out Barcode1.theBarImage);
                        HTuple dTime; bool bError; string sErrorMessage;
                        //string rmm = iniFile.Read("BarcodeReader", "SearchRangeRow1", BarPath);
                        //if (rmm != "")
                        //{
                        //    Barcode1.HandleRow1 = double.Parse(rmm);
                        //    Barcode1.HandleCol1 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeCol1", BarPath));
                        //    Barcode1.HandleRow2 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeRow2", BarPath));
                        //    Barcode1.HandleCol2 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeCol2", BarPath));
                        //}
                        //Barcode.Read(ImageWindow, BartheImage, "maximum_recognition", Barcode1.HandleRow1, Barcode1.HandleCol1, Barcode1.HandleRow2, Barcode1.HandleCol2,
                        //             out Barcode1.ResultImage, out Barcode1.sResultBarcode, out dTime, out bError, out sErrorMessage);    //enhanced_recognition
                        //掃碼
                        Barcode1.sResultBarcode = "";
                        int Result = 0;
                        if (Barcode1.ResultImage != null)
                            Barcode1.ResultImage.Dispose();
                        Barcode1.ResultImage = new HObject();
                        HObject ho_Rectangle = new HObject(), ho_ImageReduced = new HObject(), ho_ImagePart = new HObject();
                        HObject ho_SymbolXLDs = new HObject();
                        HTuple hv_DecodedMirrored = new HTuple(), hv_DecodedOrientation = new HTuple();
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle, Barcode1.HandleRow1, Barcode1.HandleCol1, Barcode1.HandleRow2, Barcode1.HandleCol2);
                        ho_ImageReduced.Dispose();
                        HOperatorSet.ReduceDomain(BartheImage, ho_Rectangle, out ho_ImageReduced);
                        ho_ImagePart.Dispose();
                        HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
                        ho_SymbolXLDs.Dispose();
                        ReadBarcode(ho_ImagePart, out ho_SymbolXLDs, out Result, out Barcode1.sResultBarcode, out hv_DecodedMirrored, out hv_DecodedOrientation);

                        //管控Barcode鏡像並且鏡像
                        double DifferenceAngle = 0;
                        if (Math.Abs((Math.Round(hv_DecodedOrientation.D, 0) - Barcode1.BarcodeAngleSet)) > 180)
                            DifferenceAngle = 360 - (Math.Abs(Math.Round(hv_DecodedOrientation.D, 0) - Barcode1.BarcodeAngleSet));
                        else
                            DifferenceAngle = Math.Abs((Math.Round(hv_DecodedOrientation.D, 0) - Barcode1.BarcodeAngleSet));
                        bool bDecodedMirroredNG = false;
                        bool bDecodeAngleNG = false;
                        if (Barcode1.sResultBarcode == "")
                        {
                            HD.disp_message(ImageWindow, "NG", "", 0, 0, "red", "true");
                        }
                        else
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.GenRegionContourXld(ho_SymbolXLDs, out ExpTmpOutVar_0, "margin");
                                ho_SymbolXLDs.Dispose();
                                HOperatorSet.MoveRegion(ExpTmpOutVar_0, out ho_SymbolXLDs, Barcode1.HandleRow1, Barcode1.HandleCol1);
                            }

                            //HD.disp_message(ImageWindow, "OK", "", 0, 0, "green", "true");
                            //HD.disp_message(ImageWindow, "LensBarcode:" + Barcode1.sResultBarcode, "", 20, 0, "black", "true");
                            //如果鏡像NG
                            if (hv_DecodedMirrored == "yes" && !Barcode1.Mirrored)
                            {
                                HD.disp_message(ImageWindow, "Mirrored:" + hv_DecodedMirrored.S, "", 40, 0, "red", "false");
                                bDecodedMirroredNG = true;
                            }
                            else
                            {
                                HD.disp_message(ImageWindow, "Mirrored:" + hv_DecodedMirrored.S, "", 40, 0, "green", "false");
                            }
                            //角度NG
                            if (DifferenceAngle > Barcode1.AllowableOffsetAngle)
                            {
                                HOperatorSet.SetColor(ImageWindow, "red");
                                HOperatorSet.DispObj(ho_SymbolXLDs, ImageWindow);
                                HD.disp_message(ImageWindow, "Angle:" + Math.Round(hv_DecodedOrientation.D, 0), "", 60, 0, "red", "false");
                                bDecodeAngleNG = true;
                            }
                            else
                            {
                                HOperatorSet.SetColor(ImageWindow, "green");
                                HOperatorSet.DispObj(ho_SymbolXLDs, ImageWindow);
                                HD.disp_message(ImageWindow, "Angle:" + Math.Round(hv_DecodedOrientation.D, 0), "", 60, 0, "green", "false");
                            }
                        }
                        Barcode1.ResultImage.Dispose();
                        HOperatorSet.DumpWindowImage(out Barcode1.ResultImage, ImageWindow);
                        HOperatorSet.GetImageSize(Barcode1.ResultImage, out width, out height);
                        HOperatorSet.SetPart(ImageWindow, 0, 0, height, width);
                        HOperatorSet.DispObj(Barcode1.ResultImage, ImageWindow);
                        Barcode1.Result_ReadOK = ((Barcode1.sResultBarcode.Length > 5) ? "1" : "2");
                        BarMessage = "";
                        BarMessage = Barcode1.sResultBarcode;
                        switch (PLC.Qlocation)
                        {
                            case 1: BarMessages1 = BarMessage; break;
                            case 2: BarMessages2 = BarMessage; break;
                        }
                        if (!Sys.NoAutoMatic)
                        {
                            WriteToPlc.CMDOKNG[8] = "000" + Barcode1.Result_ReadOK;
                            WriteToPlc.CMDresult[8] = "000000000000000000000000";
                            WriteToPlc.CMDsend[8] = true;
                            //Pstation = PLC.Blocation.ToString();//工位
                            ChangeNamePicture(PLC.Qlocation, Barcode1.Result_ReadOK);
                            WriteImage();
                            b2 = DateTime.Now;
                            System.TimeSpan t = b2 - beferDT_2;
                            string qTspan = Math.Round(t.TotalMilliseconds).ToString();
                            Run2.lblQTime.Text = qTspan;
                            Run2.lblQbar.Text = BarMessage;
                            if (Sys.CurProduceIDCheck & BarMessage.Substring(0, 2) != Sys.CurrentBarID)  //Barcode前两个字母的判断
                            {
                                Run2.lblQbarResult.Text = "NG";
                                Run2.lblQbarResult.ForeColor = Color.Red;
                                Barcode1.Result_ReadOK = "2";
                            }
                            else
                            {
                                Run2.lblQbarResult.Text = ((Barcode1.sResultBarcode.Length > 5) ? "OK" : "NG");
                                Run2.lblQbarResult.ForeColor = ((Barcode1.sResultBarcode.Length > 5) ? Color.Green : Color.Red);
                            }
                        }
                        qreadisend = false;
                        #endregion
                        #endregion
                    }
                    m_imageProvider9.ReleaseImage();
                }
            }
            catch (Exception e)
            {
                qreadisend = false;
                ShowException9(e, m_imageProvider9.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback9()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback9));
                return;
            }
        }
        private void ShowException9(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList9()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();

                ListView.ListViewItemCollection items = deviceListView_9.Items;
                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }

                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_9.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];
                    if (device.Name.Substring(0, 4) == theCamIp[8])
                        theCamIndex[8] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        QCCD.IsConnected = false;
                        deviceListView_9.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException9(e, m_imageProvider9.GetLastErrorMessage());
            }
        }
        public void Stop9()
        {
            try
            {
                m_imageProvider9.Stop();
            }
            catch (Exception e)
            {
                ShowException9(e, m_imageProvider9.GetLastErrorMessage());
            }
        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject9(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider9()
        {
            try
            {
                m_imageProvider9.Close();
            }
            catch (Exception e)
            {
                ShowException9(e, m_imageProvider9.GetLastErrorMessage());
            }
        }
        public void OneShot9()
        {
            try
            {
                m_imageProvider9.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException9(e, m_imageProvider9.GetLastErrorMessage());
            }
        }
        public void ContinuousShot9()
        {
            try
            {
                m_imageProvider9.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException9(e, m_imageProvider9.GetLastErrorMessage());
            }
        }
        #endregion
        #region QCCDHikvision
        public void processHImage(HObject image)
        {
            try
            {
                DateTime beferDT_2 = System.DateTime.Now;
                HTuple width, height;
                halcon.ImageOri[8] = image.CopyObj(1, -1);
                halcon.Image[8] = image.CopyObj(1, -1);
                HOperatorSet.GetImageSize(halcon.ImageOri[8], out width, out height);
                if (QCCD.angleC != 0 && !halcon.AIsChecked)
                {
                    double ba = -QCCD.angleC * (Math.PI / 180);
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, ba, 0, 0, out hv_HomMat2DRotate);
                    ho_ImageAffinTrans.Dispose();
                    HOperatorSet.AffineTransImage(halcon.Image[8], out ho_ImageAffinTrans, hv_HomMat2DRotate, "constant",
                         "false");
                    halcon.Image[8].Dispose();
                    HOperatorSet.CopyImage(ho_ImageAffinTrans, out halcon.Image[8]);
                }
                if (Sys.NoAutoMatic)
                {
                    #region
                    if (!Barcode1.QCCDisChecked)
                        halcon.HWindowID[8] = VisionSet.hWImageSet.HalconWindow;
                    else
                        halcon.HWindowID[8] = Fbarcode.hWindowControl1.HalconWindow;
                    HOperatorSet.SetPart(halcon.HWindowID[8], 0, 0, height, width);
                    HOperatorSet.DispObj(halcon.Image[8], halcon.HWindowID[8]);

                    HD.ReadImage9(halcon.HWindowID[8]);
                    if (halcon.IsCrossDraw)
                        HD.CrossDraw(halcon.HWindowID[8], width, height);
                    if (FrmVisionSet.Definition)
                        HD.ImageFocus(halcon.HWindowID[8], halcon.Image[8], width, height);
                    #endregion
                }
                else
                {
                    if (!Barcode1.QCCDisChecked)
                    {
                        #region NoBarcode
                        halcon.HWindowID[8] = Run2.hWQCCD.HalconWindow;
                        HOperatorSet.SetPart(halcon.HWindowID[8], 0, 0, height, width);
                        FrmVisionSet.xpm = QCCD.xpm;
                        FrmVisionSet.ypm = QCCD.ypm;
                        HD.ImagePro9(halcon.HWindowID[8]);
                        DateTime afterDT_2 = System.DateTime.Now;
                        TimeSpan ts_1 = afterDT_2.Subtract(beferDT_2);
                        string time_1 = Math.Round(ts_1.TotalMilliseconds).ToString();
                        QCCD.QTime = time_1 + "ms";
                        Thread.Sleep(10);
                        #endregion
                    }
                }
                if (Barcode1.QCCDisChecked)
                {
                    #region barcode
                    HObject BartheImage = new HObject();
                    HOperatorSet.GenEmptyObj(out BartheImage);
                    BartheImage = halcon.Image[8];
                    HTuple ImageWindow = new HTuple(); HWindow barWindow = new HWindow();
                    if (Sys.NoAutoMatic)
                    {
                        ImageWindow = Fbarcode.hWindowControl1.HalconWindow;
                        barWindow = Fbarcode.hWindowControl1.HalconWindow;
                    }
                    else
                        ImageWindow = Run2.hWQCCD.HalconWindow;
                    HOperatorSet.GetImageSize(BartheImage, out width, out height);
                    HOperatorSet.SetPart(ImageWindow, 0, 0, height, width);
                    HOperatorSet.DispObj(BartheImage, ImageWindow);
                    if (FrmVisionSet.Definition)
                        HD.ImageFocus(barWindow, BartheImage, width, height);
                    #region barcodeResult
                    /*畫掃碼範圍*/
                    if (Barcode1.bBarcodeRangeSearch)
                    {
                        Barcode1.bBarcodeRangeSearch = false;
                        HD.SearchRange(ImageWindow, BartheImage, out Barcode1.HandleRow1, out Barcode1.HandleCol1, out Barcode1.HandleRow2, out Barcode1.HandleCol2);
                        try
                        {
                            iniFile.Write("BarcodeReader", "SearchRangeRow1", Barcode1.HandleRow1.ToString(), BarPath);
                            iniFile.Write("BarcodeReader", "SearchRangeCol1", Barcode1.HandleCol1.ToString(), BarPath);
                            iniFile.Write("BarcodeReader", "SearchRangeRow2", Barcode1.HandleRow2.ToString(), BarPath);
                            iniFile.Write("BarcodeReader", "SearchRangeCol2", Barcode1.HandleCol2.ToString(), BarPath);
                            MessageBox.Show("扫码范围已存储。");
                        }
                        catch
                        { }
                    }
                    BarMessage = "";
                    Barcode1.theBarImage.Dispose();
                    HOperatorSet.CopyImage(BartheImage, out Barcode1.theBarImage);
                    HTuple dTime; bool bError; string sErrorMessage;
                    Barcode.Read(ImageWindow, BartheImage, "maximum_recognition", Barcode1.HandleRow1, Barcode1.HandleCol1, Barcode1.HandleRow2, Barcode1.HandleCol2,
                                 out Barcode1.ResultImage, out Barcode1.sResultBarcode, out dTime, out bError, out sErrorMessage);    //enhanced_recognition

                    HOperatorSet.GetImageSize(Barcode1.ResultImage, out width, out height);
                    HOperatorSet.SetPart(ImageWindow, 0, 0, height, width);
                    HOperatorSet.DispObj(Barcode1.ResultImage, ImageWindow);
                    Barcode1.Result_ReadOK = ((Barcode1.sResultBarcode.Length > 5) ? "1" : "2");
                    BarMessage = "";
                    BarMessage = Barcode1.sResultBarcode;
                    switch (PLC.Qlocation)
                    {
                        case 1: BarMessages1 = BarMessage; break;
                        case 2: BarMessages2 = BarMessage; break;
                    }
                    if (!Sys.NoAutoMatic)
                    {
                        WriteToPlc.CMDOKNG[8] = "000" + Barcode1.Result_ReadOK;
                        WriteToPlc.CMDresult[8] = "000000000000000000000000";
                        WriteToPlc.CMDsend[8] = true;
                        //Pstation = PLC.Blocation.ToString();//工位
                        ChangeNamePicture(PLC.Qlocation, Barcode1.Result_ReadOK);
                        WriteImage();
                        b2 = DateTime.Now;
                        System.TimeSpan t = b2 - beferDT_2;
                        string qTspan = Math.Round(t.TotalMilliseconds).ToString();
                        Run2.lblQTime.Text = qTspan;
                        Run2.lblQbar.Text = BarMessage;
                        //Barcode前两个字母的判断
                        if (Sys.CurProduceIDCheck & BarMessage.Substring(0, 2) != Sys.CurrentBarID)  
                        {
                            Run2.lblQbarResult.Text = "NG";
                            Run2.lblQbarResult.ForeColor = Color.Red;
                            Barcode1.Result_ReadOK = "2";
                        }
                        else
                        {
                            Run2.lblQbarResult.Text = ((Barcode1.sResultBarcode.Length > 5) ? "OK" : "NG");
                            Run2.lblQbarResult.ForeColor = ((Barcode1.sResultBarcode.Length > 5) ? Color.Green : Color.Red);
                        }
                    }
                    qreadisend = false;
                    #endregion
                    #endregion
                }
            }
            catch (Exception ER)
            {
                MessageBox.Show(ER.ToString());
            }
        }
        #endregion
        #region //Barcode原碼
        private void OnGrabErrorEventCallback10(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback10), grabException, additionalErrorMessage);
                return;
            }
            ShowException10(grabException, additionalErrorMessage);
        }
        private void OnDeviceRemovedEventCallback10()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback10));
                return;
            }
            Stop10();
            CloseTheImageProvider10();
            UpdateDeviceList10();
        }
        private void OnDeviceOpenedEventCallback10()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback10));
                return;
            }
        }
        private void OnDeviceClosedEventCallback10()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback10));
                return;
            }
        }
        private void OnGrabbingStartedEventCallback10()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback10));
                return;
            }
        }
        private void OnImageReadyEventCallback10()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback10));
                return;
            }
            try
            {
                ImageProvider.Image image = m_imageProvider10.GetLatestImage();
                if (image != null)
                {
                    if (BitmapFactory1.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory1.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory1.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    DateTime beferDT = System.DateTime.Now;
                    HObject BartheImage = new HObject(); 
                    HOperatorSet.GenEmptyObj(out BartheImage);
                    BartheImage.Dispose();
                    BitmapToHObject10(m_bitmap, out BartheImage);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.RotateImage(BartheImage, out ExpTmpOutVar_0, 270, "constant");
                        BartheImage.Dispose();
                        BartheImage = ExpTmpOutVar_0;
                    }
                    HTuple ImageWindow = new HTuple(); HWindow barWindow = new HWindow();

                    ThreadInfo threadInfo = new ThreadInfo();
                    threadInfo.image = BartheImage.CopyObj(1, -1);
                    BartheImage.Dispose();
                   if (Sys.NoAutoMatic)
                    {
                        threadInfo.Window = Fbarcode.hWindowControl1.HalconWindow;
                    }
                    else
                    {
                        threadInfo.Window = Run2.hImgResult.HalconWindow;
                    }
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ImageProcess_Barcode), threadInfo);
                   
                    
                    readisend = false;
                    #endregion
                }
            }
            catch (Exception e)
            {
                readisend = false;
                ShowException10(e, m_imageProvider10.GetLastErrorMessage());
            }
        }
        private void OnGrabbingStoppedEventCallback10()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback10));
                return;
            }
        }
        private void ShowException10(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateDeviceList10()
        {
            try
            {
                List<DeviceEnumerator1.Device> list = DeviceEnumerator1.EnumerateDevices();
                ListView.ListViewItemCollection items = deviceListView_10.Items;

                foreach (DeviceEnumerator1.Device device in list)
                {
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        DeviceEnumerator1.Device tag = item.Tag as DeviceEnumerator1.Device;
                        if (tag.FullName == device.FullName)
                        {
                            tag.Index = device.Index;
                            newitem = false;
                            break;
                        }
                    }
                    if (newitem)
                    {
                        ListViewItem item = new ListViewItem(device.Name);
                        if (device.Tooltip.Length > 0)
                            item.ToolTipText = device.Tooltip;
                        item.Tag = device;
                        deviceListView_10.Items.Add(item);
                    }
                    string[] ShowText = new string[3];
                    string strName = device.FullName;

                    string[] split1 = strName.Split('#');
                    string[] split2 = split1[2].Split(':');
                    ShowText[0] = device.Index.ToString();
                    ShowText[1] = split1[0];
                    ShowText[2] = split2[0];
                    if (device.Name.Substring(0, 13) == theCamIp[9])
                        theCamIndex[9] = device.Index;
                }
                foreach (ListViewItem item in items)
                {
                    bool exists = false;
                    foreach (DeviceEnumerator1.Device device in list)
                    {
                        if (((DeviceEnumerator1.Device)item.Tag).FullName == device.FullName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        Barcode1.IsConnected = false;
                        deviceListView_10.Items.Remove(item);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException10(e, m_imageProvider10.GetLastErrorMessage());
            }
        }
        public void Stop10()
        {
            try
            {
                m_imageProvider10.Stop();
            }
            catch (Exception e)
            {
                ShowException10(e, m_imageProvider10.GetLastErrorMessage());
            }

        }
        // 将bitmap转化为Hobject
        public void BitmapToHObject10(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                System.IntPtr srcPtr = srcBmData.Scan0;
                HOperatorSet.GenImageInterleaved(out image, srcPtr, "bgrx", bmp.Width, bmp.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmData);
            }
            catch (Exception ex)
            {
                image = null;
                MessageBox.Show(ex.Message);
            }
        }
        public void CloseTheImageProvider10()
        {
            try
            {
                m_imageProvider10.Close();
            }
            catch (Exception e)
            {
                //如果CCD斷線會報錯,不影響,先關閉此功能
                ShowException10(e, m_imageProvider10.GetLastErrorMessage());
            }
        }
        public void OneShot10()
        {
            try
            {
                m_imageProvider10.OneShot(); /* Starts the grabbing of one image. */
            }
            catch (Exception e)
            {
                ShowException10(e, m_imageProvider10.GetLastErrorMessage());
            }
        }
        public void ContinuousShot10()
        {
            try
            {
                m_imageProvider10.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }
            catch (Exception e)
            {
                ShowException10(e, m_imageProvider10.GetLastErrorMessage());
            }
        }
        #endregion
        /// <summary>
        /// Barcode相機圖像處理
        /// </summary>
        /// <param name="a"></param>
        private void ImageProcess_Barcode(Object a)
        {
            
            ThreadInfo threadInfo = new ThreadInfo();
            threadInfo = a as ThreadInfo;

            HObject BartheImage = new HObject(); HTuple width = new HTuple(), height = new HTuple();
            HWindow ImageWindow = threadInfo.Window;
            BartheImage = threadInfo.image.CopyObj(1, -1);
            
            HOperatorSet.GetImageSize(BartheImage, out width, out height);
            //if (height > width)
            //{
            //    HOperatorSet.SetPart(ImageWindow, 0, 0, height, height*4/3);
            //}
            //else
            //{
            HOperatorSet.SetPart(ImageWindow, 0, 0, height, width);
            //}
            HOperatorSet.DispObj(BartheImage, ImageWindow);
            #region Focus
            if (FrmVisionSet.Definition)
                HD.ImageFocus(ImageWindow, BartheImage, width, height);
            #endregion
            #region barcodeResult
            /*畫掃碼範圍*/
            if (Barcode1.bBarcodeRangeSearch)
            {
                Barcode1.bBarcodeRangeSearch = false;
                HD.SearchRange(ImageWindow, BartheImage, out Barcode1.HandleRow1, out Barcode1.HandleCol1, out Barcode1.HandleRow2, out Barcode1.HandleCol2);
                try
                {
                    iniFile.Write("BarcodeReader", "SearchRangeRow1", Barcode1.HandleRow1.ToString(), BarPath);
                    iniFile.Write("BarcodeReader", "SearchRangeCol1", Barcode1.HandleCol1.ToString(), BarPath);
                    iniFile.Write("BarcodeReader", "SearchRangeRow2", Barcode1.HandleRow2.ToString(), BarPath);
                    iniFile.Write("BarcodeReader", "SearchRangeCol2", Barcode1.HandleCol2.ToString(), BarPath);
                    MessageBox.Show("扫码范围已存储。");
                }
                catch
                {

                }
            }
            BarMessage = "";
            Barcode1.theBarImage.Dispose();
            HOperatorSet.CopyImage(BartheImage, out Barcode1.theBarImage);
            //HTuple dTime; bool bError; string sErrorMessage;
            string rmm = iniFile.Read("BarcodeReader", "SearchRangeRow1", BarPath);
            if (rmm != "")
            {
                Barcode1.HandleRow1 = double.Parse(rmm);
                Barcode1.HandleCol1 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeCol1", BarPath));
                Barcode1.HandleRow2 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeRow2", BarPath));
                Barcode1.HandleCol2 = double.Parse(iniFile.Read("BarcodeReader", "SearchRangeCol2", BarPath));
            }
            //掃碼
            Barcode1.sResultBarcode = "";
            int Result = 0;
            if (Barcode1.ResultImage != null)
                Barcode1.ResultImage.Dispose();
            Barcode1.ResultImage = new HObject();
            HObject ho_Rectangle = new HObject(), ho_ImageReduced = new HObject(), ho_ImagePart = new HObject();
            HObject ho_SymbolXLDs = new HObject();
            HTuple hv_DecodedMirrored = new HTuple(), hv_DecodedOrientation = new HTuple();
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle1(out ho_Rectangle, Barcode1.HandleRow1, Barcode1.HandleCol1, Barcode1.HandleRow2, Barcode1.HandleCol2);
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(BartheImage, ho_Rectangle, out ho_ImageReduced);
            ho_ImagePart.Dispose();
            HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
            ho_SymbolXLDs.Dispose();
            ReadBarcode(ho_ImagePart, out ho_SymbolXLDs, out Result, out Barcode1.sResultBarcode, out hv_DecodedMirrored, out hv_DecodedOrientation);
            //計Barcode角度Log用 
            Barcode1.LensBarcodeAngle = Math.Round(hv_DecodedOrientation.D, 1);
            //管控Barcode鏡像並且鏡像
            double DifferenceAngle = 0;
            if (Math.Abs((Math.Round(hv_DecodedOrientation.D, 0) - Barcode1.BarcodeAngleSet)) > 180)
                DifferenceAngle = 360 - (Math.Abs(Math.Round(hv_DecodedOrientation.D, 0) - Barcode1.BarcodeAngleSet));
            else
                DifferenceAngle = Math.Abs((Math.Round(hv_DecodedOrientation.D, 0) - Barcode1.BarcodeAngleSet));
            bool bDecodedMirroredNG = false;
            bool bDecodeAngleNG = false;
            if (Barcode1.sResultBarcode == "")
            {
                HD.disp_message(ImageWindow, "NG", "", 0, 0, "red", "true");
            }
            else
            {
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.GenRegionContourXld(ho_SymbolXLDs, out ExpTmpOutVar_0, "margin");
                    ho_SymbolXLDs.Dispose();
                    HOperatorSet.MoveRegion(ExpTmpOutVar_0, out ho_SymbolXLDs, Barcode1.HandleRow1, Barcode1.HandleCol1);
                }

                //HD.disp_message(ImageWindow, "OK", "", 0, 0, "green", "true");
                //HD.disp_message(ImageWindow, "LensBarcode:" + Barcode1.sResultBarcode, "", 20, 0, "black", "true");
                //如果鏡像NG
                if (hv_DecodedMirrored == "yes" && !Barcode1.Mirrored)
                {
                    HD.disp_message(ImageWindow, "Mirrored:" + hv_DecodedMirrored.S, "", 40, 0, "red", "false");
                    bDecodedMirroredNG = true;
                }
                else
                {
                    HD.disp_message(ImageWindow, "Mirrored:" + hv_DecodedMirrored.S, "", 40, 0, "green", "false");
                }
                //角度NG
                if (DifferenceAngle > Barcode1.AllowableOffsetAngle)
                {
                    HOperatorSet.SetColor(ImageWindow, "red");
                    HOperatorSet.DispObj(ho_SymbolXLDs, ImageWindow);
                    HD.disp_message(ImageWindow, "Angle:" + Math.Round(hv_DecodedOrientation.D, 0), "", 60, 0, "red", "false");
                    bDecodeAngleNG = true;
                }
                else
                {
                    HOperatorSet.SetColor(ImageWindow, "green");
                    HOperatorSet.DispObj(ho_SymbolXLDs, ImageWindow);
                    HD.disp_message(ImageWindow, "Angle:" + Math.Round(hv_DecodedOrientation.D, 0), "", 60, 0, "green", "false");
                }
            }
            Barcode1.ResultImage.Dispose();
            HOperatorSet.DumpWindowImage(out Barcode1.ResultImage, ImageWindow);
            BartheImage.Dispose();
            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_ImagePart.Dispose();
            ho_SymbolXLDs.Dispose();
            //Barcode.Read(ImageWindow, BartheImage, "maximum_recognition", Barcode1.HandleRow1, Barcode1.HandleCol1, Barcode1.HandleRow2, Barcode1.HandleCol2,
            //             out Barcode1.ResultImage, out Barcode1.sResultBarcode, out dTime, out bError, out sErrorMessage);    //enhanced_recognition

            //HOperatorSet.GetImageSize(Barcode1.ResultImage, out width, out height);
            //HOperatorSet.SetPart(ImageWindow, 0, 0, height, width);
            //HOperatorSet.DispObj(Barcode1.ResultImage, ImageWindow);
            Barcode1.Result_ReadOK = ((Barcode1.sResultBarcode.Length > 5 && Result == 1) ? "1" : "2");
            BarMessage = "";
            BarMessage = Barcode1.sResultBarcode;
            try
            {
                if (!Sys.NoAutoMatic)
                {
                    b2 = DateTime.Now;
                    System.TimeSpan t = b2 - b1;
                    bTspan = Math.Round(t.TotalMilliseconds).ToString();
                    this.Invoke(new MethodInvoker(delegate
                    {
                        Run2.labBar.Text = BarMessage;
                        if (Sys.CurProduceIDCheck & Barcode1.sResultBarcode.Substring(0, 2) != Sys.CurrentBarID || bDecodeAngleNG || bDecodedMirroredNG)  //Barcode前两个字母的判断
                        {
                            Run2.labresult.Text = "NG";
                            Run2.labresult.ForeColor = Color.Red;
                            Barcode1.Result_ReadOK = "2";
                        }
                        else
                        {
                            Run2.labresult.Text = ((Barcode1.sResultBarcode.Length > 5) ? "OK" : "NG");
                            Run2.labresult.ForeColor = ((Barcode1.sResultBarcode.Length > 5) ? Color.Green : Color.Red);
                        }
                        if (Barcode1.Result_ReadOK == "1")
                        {
                            if (bDecodedMirroredNG || bDecodeAngleNG)
                            {
                                Run2.labresult.Text = "NG";
                                Run2.labresult.ForeColor = Color.Red;
                            }
                            else
                            {
                                Run2.labresult.Text = "OK";
                                Run2.labresult.ForeColor = Color.Green;
                            }
                        }
                    }));
                    Barcresult = Barcode1.Result_ReadOK;
                    sendBar = true;
                    Pstation = PLC.Blocation.ToString();//工位



                    ChangeNamePicture(PLC.Blocation, Barcode1.Result_ReadOK);
                    WriteImage();
                }
            }
            catch
            {
            }
        }

         /// <summary>
        /// QCCD相機圖像處理
        /// </summary>
        /// <param name="a"></param>
        private void ImageProcess_QCCD(Object a)
        {

            ThreadInfo threadInfo = new ThreadInfo();
            threadInfo = a as ThreadInfo;
        }

        #region 
        
        public void ReadBarcode(HObject ho_Image, out HObject ho_SymbolXLDs, out int iResult, out string sBarcode, out HTuple hv_DecodedMirrored, out HTuple hv_DecodedOrientation)
        {
            iResult = 0;
            ho_SymbolXLDs = new HObject();
            HTuple hv_DataCodeHandle = new HTuple(), hv_ResultHandles = new HTuple(), hv_DecodedDataStrings = new HTuple();
            HTuple hv_Number = new HTuple(), hv_DecodedData = new HTuple();
            string _sBarcode = "";
            sBarcode = "";
            hv_DecodedMirrored = "no";
            hv_DecodedOrientation = 0;
            HOperatorSet.CreateDataCode2dModel("Data Matrix ECC 200", "default_parameters", "maximum_recognition", out hv_DataCodeHandle);
            HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "polarity", "any");
            HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "timeout", 2000);
            HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "contrast_tolerance", "high");
            HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "small_modules_robustness", "high");
            HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "symbol_rows_min", 14);
            HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "symbol_rows_max", 18);
            //HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "module_size_min", 2);
            //HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "module_size_max", 5);
            if (Barcode1.Production == 0)
                HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "decoding_scheme", "raw");
            HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle, "persistence", 0);
            ho_SymbolXLDs.Dispose();

            HOperatorSet.FindDataCode2d(ho_Image, out ho_SymbolXLDs, hv_DataCodeHandle, "stop_after_result_num", 1, out hv_ResultHandles, out hv_DecodedDataStrings);
            HOperatorSet.CountObj(ho_SymbolXLDs, out hv_Number);
            if (hv_Number == 0)
            {
                sBarcode = "NA";
                iResult = -2;
                HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);
                return;
            }
            else
            {
                HOperatorSet.GetDataCode2dResults(hv_DataCodeHandle, hv_ResultHandles.TupleSelect(0), "decoded_data", out hv_DecodedData);
                HOperatorSet.GetDataCode2dResults(hv_DataCodeHandle, hv_ResultHandles.TupleSelect(0), "mirrored", out hv_DecodedMirrored);
                HOperatorSet.GetDataCode2dResults(hv_DataCodeHandle, hv_ResultHandles.TupleSelect(0), "orientation", out hv_DecodedOrientation);
                if (Barcode1.Production == 0)
                {
                    int[] Barcode = new int[8];
                    for (int i = 0; i <= 7; i++)
                    {
                        Barcode[i] = (((hv_DecodedData.TupleSelect(i)) - ((149 * (i + 1)) % 255)) - 1) % 256;
                        _sBarcode = Convert.ToString(Barcode[i], 16);
                        //不足兩位補0
                        if (_sBarcode.Length < 2)
                        {
                            _sBarcode = "0" + _sBarcode;
                        }
                        _sBarcode = _sBarcode.Substring(_sBarcode.Length - 2);
                        sBarcode = sBarcode + _sBarcode;
                    }
                    if (sBarcode.Substring(2, 2) != "40")//如果解出來的碼第3.4碼不等於40,代表解出來的碼有問題,判定為解碼錯誤重拍
                    {
                        HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);
                        iResult = -1;
                        return;//跳出
                    }
                }
                else if (Barcode1.Production == 1)
                {
                    int Length = 0;
                    Length = hv_DecodedData.Length;
                    int[] Barcode = new int[Length];
                    for (int i = 0; i <= Length - 1; i++)
                    {
                        Barcode[i] = hv_DecodedData.TupleSelect(i);
                        _sBarcode = ((char)Barcode[i]).ToString();
                        sBarcode = sBarcode + _sBarcode;
                    }
                }
                else if (Barcode1.Production == 2)
                {
                    int Length = 0;
                    Length = hv_DecodedData.Length;
                    int[] Barcode = new int[Length];

                    for (int i = 0; i <= Length - 1; i++)
                    {
                        Barcode[i] = hv_DecodedData.TupleSelect(i);
                        _sBarcode = Convert.ToString(Barcode[i], 16);
                        //不足兩位補0
                        if (_sBarcode.Length < 2)
                        {
                            _sBarcode = "0" + _sBarcode;
                        }
                        _sBarcode = _sBarcode.Substring(_sBarcode.Length - 2);
                        sBarcode = sBarcode + _sBarcode;
                    }
                }
                HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);
                iResult = 1;
            }
        }

        #endregion
        #region Open
        public void OpenCam1()
        {
            try
            {
                m_imageProvider1.Open(theCamIndex[0]);
            }
            catch (Exception )
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam2()
        {
            try
            {
                m_imageProvider2.Open(theCamIndex[1]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam3()
        {
            try
            {
                m_imageProvider3.Open(theCamIndex[2]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam4()
        {
            try
            {
                m_imageProvider4.Open(theCamIndex[3]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam5()
        {
            try
            {
                m_imageProvider5.Open(theCamIndex[4]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam6()
        {
            try
            {
                m_imageProvider6.Open(theCamIndex[5]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam7()
        {
            try
            {
                m_imageProvider7.Open(theCamIndex[6]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam8()
        {
            try
            {
                m_imageProvider8.Open(theCamIndex[7]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam9()
        {
            try
            {
                m_imageProvider9.Open(theCamIndex[8]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void OpenCam10()
        {
            try
            {
                m_imageProvider10.Open(theCamIndex[9]);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #endregion
        
        #region //扫码
        int r_cont = 0;
        public DateTime b1, b2; public static  string bTspan;
        bool readisend = false, qreadisend = false;
        string[] pathIP = new string[] { };
        public string BarMessage, BarMessages1, BarMessages2; public bool Barcancel = false;
        private void LTrayData_DoWork(object sender, DoWorkEventArgs e)
        {
            bool bReading = false;
            char[] c = new char[] { 'E', 'R', 'R', 'O', 'R' };
            while (!LTrayData.CancellationPending)
            {
                if (!PLC.BTrigger[6])
                    bReading = false;
                if (PLC.BTrigger[6] && !bReading)
                {
                    bReading = true;
                    System.Threading.Thread.Sleep(200);
                    byte[] buffer = new byte[1024];
                    int len = Reader.Com.Read(buffer, 0, Reader.Com.BytesToRead);
                    if (len <= 0)
                    {
                        Reader.IsConnected = false;
                        continue;
                    }
                    string s = "";
                    s = Encoding.ASCII.GetString(buffer, 0, len).Trim(c).Trim();
                    if (s.Length < 17)
                    {
                        System.Threading.Thread.Sleep(200);
                        buffer = new byte[1024];
                        len = RiReader.Com.Read(buffer, 0, RiReader.Com.BytesToRead);
                        if (len <= 0)
                        {
                            RiReader.IsConnected = false;
                            continue;
                        }
                        s = s + Encoding.ASCII.GetString(buffer, 0, len).Trim(c).Trim();
                    }
                    s = s.Replace("ERROR", "");
                    s = s.Replace("\r", "");
                    if (s.Length >= 17)
                    {
                        if (s.Length >= 17)
                        {
                            if (s.Length % 17 == 0)
                                s = s.Substring(0, 17);
                        }
                        if (s.Length >= 20)
                        {
                            if (s.Length % 20 == 0)
                                s = s.Substring(0, 20);
                        }
                        Reader.IsConnected = true;
                        //lb_cont = 0;
                        Reader.newBarcode = s;
                        if (Reader.Barcode == "" || Reader.Barcode != Reader.newBarcode)
                            Reader.Barcode = Reader.newBarcode;
                        Reader.OpTime = DateTime.Now;
                        dtResult = "Y";
                        if (RiReader.WebChecked)
                        {
                            #region 8982配对web
                            Scaning = true;
                            Thread.Sleep(1);
                            ScanStatus = Scaning;
                            try
                            {
                                CallWithTimeout(GSWeb, 30000);
                            }
                            catch
                            {
                                dtResult = "N";
                                sErrory = "访问系统超时！";
                                MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            #endregion
                        }
                        if (dtResult == "Y")
                        {
                            Protocol.strPCRead_PCReady = true; Protocol.IsPCRead = true;  //胶水ok
                            Reader.strTrRead = "0001";
                        }
                        else
                        {
                            //Protocol.strPCRead = "0020"; Protocol.IsPCRead = true;  //匹配LensTary信息NG
                            Thread.Sleep(5);
                            Reader.strTrRead = "0004";
                            MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Reader.Barcode = "";
                        }
                        #region 管控壓覆點膠進出站
                        if (RiReader.Web_Tray_InOutStation && RiReader.Web_Tray2InTray1Out_InOutStation && Sys.Codes == "M")
                        {
                            RiReader.Barcode_In = Reader.Barcode;
                            #region
                            try
                            {
                                CallWithTimeout(m_Web_Tray_In, 15000);  //Web访问计时
                            }
                            catch
                            {
                                RiReader.Web_Tray_InOutStation_Result = false;
                                RiReader.Web_Tray_InOutStation_sMsg = "访问系统超时！";
                                //MessageBox.Show(RiReader.Web_Tray_InOutStation_sMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            DateTime DT = DateTime.Now;
                            this.Invoke(new MethodInvoker(delegate
                            {
                                if (RiReader.Web_Tray_InOutStation_sMsg == "")
                                {
                                    Run.rtxWebMessage.SelectionColor = Color.Green;
                                    Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                    Run.rtxWebMessage.AppendText(RiReader.Barcode_In + "_進站成功!" + "\r");
                                }
                                else
                                {
                                    Run.rtxWebMessage.SelectionColor = Color.Red;
                                    Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                    Run.rtxWebMessage.AppendText(RiReader.Barcode_In + "_進站失败" + "\t");
                                    Run.rtxWebMessage.AppendText(RiReader.Web_Tray_InOutStation_sMsg + "\r");
                                }
                                Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                            }));
                            if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                                weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                            if (RiReader.Web_Tray_InOutStation_Result)
                            {
                                RiReader.strTrRead = "0001";
                                weblog.log("WebOK:" + s + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_進站成功!");
                            }
                            else
                            {
                                if (RiReader.Web_Tray_InOutStation_ErrorIgnore)
                                {
                                    RiReader.strTrRead = "0001";
                                }
                                else
                                {
                                    RiReader.strTrRead = "0004";
                                }
                                weblog.log("Web报错:" + s + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_" + RiReader.Web_Tray_InOutStation_sMsg);
                                try
                                {
                                    this.Invoke(new MethodInvoker(delegate
                                    {
                                        new MessageForm(100000, DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_" + RiReader.Barcode_In + "_" + RiReader.Web_Tray_InOutStation_sMsg).Show();

                                        Run.rtxWebMessage.SelectionColor = Color.Red;
                                        Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                        Run.rtxWebMessage.AppendText(RiReader.Barcode_In + "_進站失败" + "\t");
                                        Run.rtxWebMessage.AppendText(RiReader.Web_Tray_InOutStation_sMsg + "\r");
                                        Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                                    }));
                                }
                                catch
                                {
                                }
                            }
                            RiReader.IsTrRead = true;


                            #endregion
                        }
                        else if (RiReader.Web_Tray_InOutStation && RiReader.Web_Tray2InTray1Out_InOutStation && Sys.Codes != "M")
                        {
                            DateTime DT = DateTime.Now;
                            RiReader.strTrRead = "0001";
                            RiReader.IsTrRead = true;
                            weblog.log("WebOK:" + s + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_" + "首件進站");
                            this.Invoke(new MethodInvoker(delegate
                            {
                                Run.rtxWebMessage.SelectionColor = Color.Green;
                                Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                Run.rtxWebMessage.AppendText(RiReader.Barcode_In + "_首件進站" + "\r");
                                Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                            }));
                        }
                        #endregion
                        Scaning = false;
                        Thread.Sleep(1);
                        ScanStatus = Scaning;
                        Reader.IsTrRead = true;
                    }
                    else
                        Reader.strTrRead = "0004";
                    Reader.IsTrRead = true;
                    iniFile.Write("CurrentMessage", "Tray2Barcode", Reader.Barcode, propath);
                }
                Thread.Sleep(10);
            }
        }
        private void RTrayData_DoWork(object sender, DoWorkEventArgs e)
        {
            bool bReading = false;
            char[] c = new char[] { 'E', 'R', 'R', 'O', 'R' };
            while (!RTrayData.CancellationPending)
            {
                if (!PLC.BTrigger[8])
                    bReading = false;
                if (PLC.BTrigger[8] && !bReading)
                {
                    #region
                    bReading = true;
                    System.Threading.Thread.Sleep(200);
                    byte[] buffer = new byte[1024];
                    int len = RiReader.Com.Read(buffer, 0, RiReader.Com.BytesToRead);
                    if (len <= 0)
                    {
                        RiReader.IsConnected = false;
                        continue;
                    }
                    string s ="";
                    s = Encoding.ASCII.GetString(buffer, 0, len).Trim(c).Trim();
                    if (s.Length < 17)
                    {
                        System.Threading.Thread.Sleep(200);
                        buffer = new byte[1024];
                        len = RiReader.Com.Read(buffer, 0, RiReader.Com.BytesToRead);
                        if (len <= 0)
                        {
                            RiReader.IsConnected = false;
                            continue;
                        }
                        s = s + Encoding.ASCII.GetString(buffer, 0, len).Trim(c).Trim();
                    }
                    #endregion
                    s = s.Replace("ERROR", "");
                    s = s.Replace("\r", "");
                    if (s.Length >=17)
                    {
                        s = s.Substring(0, 17);
                    
                        RiReader.IsConnected = true;
                        if (RiReader.Barcode != s)  //新码时changename
                        {
                            ChangeNameNewBarcode();
                        }
                        RiReader.newBarcode = s;
                        if (RiReader.Barcode == "" || RiReader.Barcode != RiReader.newBarcode)
                            RiReader.Barcode = RiReader.newBarcode;
                        if (RiReader.BPFWebChecked & ProductCount == 0)
                        {
                            #region BPFIn
                            Scaning = true;
                            Thread.Sleep(1);
                            ScanStatus = Scaning;
                            try
                            {
                                CallWithTimeout(GSBPFWebIn, 30000);
                            }
                            catch
                            {
                                dtResult = "N";
                                if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                                    weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                                weblog.log("Web报错:" + RiReader.Barcode + "In," + sErrory);
                                sErrory = "访问系统超时！";
                                MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            #endregion
                        }

                        RiReader.OpTime = DateTime.Now;
                        dtResult = "Y";
                        if (RiReader.BPFWebChecked)
                            dtResult = (!bpfdtResult ? "N" : "Y");
                       
                        if (dtResult == "Y")
                        {
                            #region Y(一盤開始了就不判斷膠水是否過期,等滿盤再判斷)
                            //string tt = "";
                            //this.Invoke(new MethodInvoker(delegate
                            //{
                            //    tt = Run.txtGlueBarcode.Text;
                            //}));
                            //if (Glue.IsChecked & tt != "" & tt == Glue.Barcode)
                            //{
                            //    FrmDisplay1.GBtimeN = DateTime.Now;
                            //    System.TimeSpan t = FrmDisplay1.GBtimeN - FrmDisplay1.replaceTime;
                            //    if (t.TotalMinutes >= Glue.glueTime)
                            //    {
                            //        Protocol.strPCRead = "0002";  //满盘信号时判断胶水时间超时
                            //        Protocol.IsPCRead = true;
                            //        Thread.Sleep(5);
                            //        RiReader.strTrRead = "0002";
                            //        RiReader.IsTrRead = true;
                            //        gluelog.log("Glue报错:TimeOut(HTrayBarcode)");
                            //    }
                            //    else
                            //    {
                            //        RiReader.strTrRead = "0001"; RiReader.IsTrRead = true;
                            //        Protocol.strPCRead = "0001"; Protocol.IsPCRead = true;  //胶水ok
                            //    }
                            //}
                            //else
                            //{
                            //    RiReader.strTrRead = "0001"; RiReader.IsTrRead = true;
                            //    Protocol.strPCRead = "0001"; Protocol.IsPCRead = true; //胶水ok
                            //}
                            #endregion
                        }
                        else
                        {
                            #region N
                            if (RiReader.WebChecked)
                            {
                                if (Reader.IsChecked & Reader.Barcode != "")
                                {
                                    //Protocol.strPCRead = "0010";   //匹配HolderTray信息NG
                                    
                                    RiReader.strTrRead = "0004";
                                    
                                }
                                else
                                {
                                    //Protocol.strPCRead = "0003";   //洗净时间HolderTray信息NG
                                    RiReader.strTrRead = "0003";
                                }
                                RiReader.IsTrRead = true;
                                MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                RiReader.Barcode = "";
                                //Protocol.IsPCRead = true;
                            }
                            
                            #endregion
                        }
                        if (RiReader.WebChecked)
                        {
                            #region
                            Scaning = true;
                            Thread.Sleep(1);
                            ScanStatus = Scaning;
                            try
                            {
                                CallWithTimeout(GSWeb, 30000);  //Web访问计时
                            }
                            catch
                            {
                                dtResult = "N";
                                sErrory = "访问系统超时！";
                                MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            DateTime DT = DateTime.Now;
                            if (dtResult == "Y")
                            {
                                RiReader.strTrRead = "0001";
                                RiReader.IsTrRead = true;
                                
                            }
                            else
                            {
                                RiReader.strTrRead = "0003";
                                RiReader.IsTrRead = true;
                                this.Invoke(new MethodInvoker(delegate
                                {
                                    new MessageForm(100000, DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_" + sErrory).Show();
                                    //MessageBox.Show(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_" + RiReader.Web_Tray_InOutStation_sMsg + "_GlueMax:" + Glue.Count_Max.ToString() + "_GlueNow:" + Glue.Count_Now.ToString());
                                }));
                            }
                            #endregion
                        }
                        #region 管控壓覆點膠進出站
                        if (RiReader.Web_Tray_InOutStation)
                            RiReader.Barcode_Out = RiReader.Barcode;
                        if (RiReader.Web_Tray_InOutStation && !RiReader.Web_Tray2InTray1Out_InOutStation && Sys.Codes=="M")
                        {
                            RiReader.Barcode_In = RiReader.Barcode;
                            #region
                            try
                            {
                                CallWithTimeout(m_Web_Tray_In, 15000);  //Web访问计时
                            }
                            catch
                            {
                                RiReader.Web_Tray_InOutStation_Result = false;
                                RiReader.Web_Tray_InOutStation_sMsg = "访问系统超时！";
                                //MessageBox.Show(RiReader.Web_Tray_InOutStation_sMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            DateTime DT = DateTime.Now;
                            this.Invoke(new MethodInvoker(delegate
                            {
                                if (RiReader.Web_Tray_InOutStation_sMsg == "")
                                {
                                    Run.rtxWebMessage.SelectionColor = Color.Green;
                                    Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                    Run.rtxWebMessage.AppendText(RiReader.Barcode_In+"_進站成功!" + "\r");
                                }
                                else
                                {
                                    Run.rtxWebMessage.SelectionColor = Color.Red;
                                    Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                    Run.rtxWebMessage.AppendText(RiReader.Barcode_In+"_進站失败" + "\t");
                                    Run.rtxWebMessage.AppendText(RiReader.Web_Tray_InOutStation_sMsg + "\r");
                                }
                                Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                            }));
                            if (!File.Exists(Sys.AlarmPath + "\\" + weblogfile))
                                weblog = new Log(Sys.AlarmPath + "\\" + weblogfile);
                            if (RiReader.Web_Tray_InOutStation_Result)
                            {
                                RiReader.strTrRead = "0001";
                                weblog.log("WebOK:" + s + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId +"_機種:"+Sys.CurrentProduction + "_進站成功!");
                            }
                            else
                            {
                                if (RiReader.Web_Tray_InOutStation_ErrorIgnore)
                                {
                                    RiReader.strTrRead = "0001";
                                }
                                else
                                {
                                    RiReader.strTrRead = "0005";
                                }
                                weblog.log("Web报错:" + s + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") +"_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_" +  RiReader.Web_Tray_InOutStation_sMsg);
                                try
                                {
                                    this.Invoke(new MethodInvoker(delegate
                                    {
                                        new MessageForm(100000, DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "_" + RiReader.Barcode_In+"_"  +RiReader.Web_Tray_InOutStation_sMsg).Show();
                      
                                        Run.rtxWebMessage.SelectionColor = Color.Red;
                                        Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                        Run.rtxWebMessage.AppendText(RiReader.Barcode_In+"_進站失败" + "\t");
                                        Run.rtxWebMessage.AppendText(RiReader.Web_Tray_InOutStation_sMsg + "\r");
                                        Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                                    }));
                                }
                                catch
                                {
                                }
                            }
                            RiReader.IsTrRead = true;
                            #endregion
                        }
                        else if (RiReader.Web_Tray_InOutStation && !RiReader.Web_Tray2InTray1Out_InOutStation && Sys.Codes != "M")
                        {
                            DateTime DT = DateTime.Now;
                            RiReader.strTrRead = "0001";
                            RiReader.IsTrRead = true;
                            weblog.log("WebOK:" + s + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_" + "首件進站");
                            this.Invoke(new MethodInvoker(delegate
                            {
                                Run.rtxWebMessage.SelectionColor = Color.Green;
                                Run.rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_機台號:" + Sys.MachineId + "_機種:" + Sys.CurrentProduction + "\t");
                                Run.rtxWebMessage.AppendText(RiReader.Barcode_In + "_首件進站" + "\r");
                                Run.rtxWebMessage.SelectionIndent = Run.rtxWebMessage.SelectionLength - 1;//至頂
                            }));
                        }
                        else if ((RiReader.Web_Tray_InOutStation && RiReader.Web_Tray2InTray1Out_InOutStation))
                        {
                            RiReader.strTrRead = "0001";
                            RiReader.IsTrRead = true;
                        }
                        #endregion
                        
                        Scaning = false;
                        Thread.Sleep(1);
                        ScanStatus = Scaning;
                        //string logpaths = Sys.ReportLog1 + "\\"  + LogDate;
                        //string logfiles = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
                        //if (!File.Exists(logpaths + "\\" + logfiles))
                        //    LogSerial = DateTime.Now.ToString("HHmmss");
                        //iniFile.Write("LOGMESSAGE", "LogSerial", LogSerial, setpath);
                    }
                    else
                    {
                        RiReader.strTrRead = "0002";
                        RiReader.IsTrRead = true;
                    }
                    iniFile.Write("CurrentMessage", "Tray1Barcode", RiReader.Barcode, propath);
                    ClearMemory();
                }
                Thread.Sleep(10);
            }
        }       
        private void ScanTrigger_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(500);
            bool bReading = false;
            while (true)
            {
                if (ScanTrigger.CancellationPending)
                    return;
                if (!Barcode1.QCCDisChecked)
                {
                    #region BarcodeReader
                    if (!PLC.BTrigger[4])
                        bReading = false;
                    if (Barcode1.IsConnected & PLC.BTrigger[4] && !bReading && !readisend)
                    {
                        readisend = true;
                        readCurT = true;
                        Thread.Sleep(80);
                        r_cont = 0;
                        bReading = true;
                        b1 = DateTime.Now;
                        OneShot10();
                        readisend = false;
                    }
                    else
                    {
                        if (PLC.BTrigger[4])
                        {
                            r_cont++;
                            if (r_cont >= 200)
                            {
                                bReading = false;
                                r_cont = 0;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region QCCD
                    if (!Barcode1.qBarcodeTrig)
                        bReading = false;
                    if (QCCD.IsConnected & Barcode1.qBarcodeTrig & !bReading && !qreadisend)
                    {
                        qreadisend = true;
                        readCurT = true;
                        Thread.Sleep(80);
                        r_cont = 0;
                        bReading = true;
                        b1 = DateTime.Now;
                        OneShot9();
                        qreadisend = false;
                    }
                    else
                    {
                        if (Barcode1.qBarcodeTrig)
                        {
                            r_cont++;
                            if (r_cont >= 200)
                            {
                                bReading = false;
                                r_cont = 0;
                            }
                        }
                    }
                    #endregion
                }
                Thread.Sleep(20);
            }
        }
        #endregion

        #region ChangeName
        public void WriteImage()
        {
            string Loadpatho = "", Loadpathr = "";
            if (Barcode1.Result_ReadOK == "1" & Barcode1.OkSave)
            {
                Loadpatho = Sys.ReportImage + "\\" + FrmMain.mtime + "\\" + Sys.CurrentProduction + "\\BarcodeReader\\PASS\\OriginalImage\\";
                Loadpathr = Sys.ReportImage + "\\" + FrmMain.mtime + "\\" + Sys.CurrentProduction + "\\BarcodeReader\\PASS\\ResultImage\\";
                HOperatorSet.WriteImage(Barcode1.theBarImage, "bmp", 0, Loadpatho + DateTime.Now.ToString("HHmmss"));
                HOperatorSet.WriteImage(Barcode1.ResultImage, "png", 0, Loadpathr + DateTime.Now.ToString("HHmmss") + "_" + Barcode1.sResultBarcode);
            }
            if (Barcode1.Result_ReadOK == "2" & Barcode1.NgSave)
            {
                Loadpatho = Sys.ReportImage + "\\" + FrmMain.mtime + "\\" + Sys.CurrentProduction + "\\BarcodeReader\\NG\\OriginalImage\\";
                Loadpathr = Sys.ReportImage + "\\" + FrmMain.mtime + "\\" + Sys.CurrentProduction + "\\BarcodeReader\\NG\\ResultImage\\";
                HOperatorSet.WriteImage(Barcode1.theBarImage, "bmp", 0, Loadpatho + DateTime.Now.ToString("HHmmss"));
                HOperatorSet.WriteImage(Barcode1.ResultImage, "png", 0, Loadpathr + DateTime.Now.ToString("HHmmss") + "_" + Barcode1.sResultBarcode);
            }
        }
        public void ChangeNameNewBarcode()
        {
            #region ChangeName
            string lpath = Sys.ReportLog1 + "\\" + LogDate;
            string logfile1 = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
            string lFileName = lpath + "\\" + logfile1;
            string lnewFileName = lpath + "\\" + "Done_" + logfile1;
            if ((RiReader.IsChecked && RiReader.Barcode != "") || !RiReader.IsChecked)
            {
                if (File.Exists(lFileName))// && Transmission.code != ""
                {
                    if (!ModifyFilename(lFileName, lnewFileName))
                    {
                        try
                        {
                            if (File.Exists(lFileName))
                                File.Move(lFileName, lnewFileName);
                        }
                        catch
                        {
                        }
                    }
                    if (RiReader.BPFWebChecked)
                    {
                        BPFOut();
                    }
                }
            }
            
            #endregion
            RiReader.Barcode = "";
            iniFile.Write("CurrentMessage", "Tray1Barcode", RiReader.Barcode, propath);
            #region  生成新的log文件夹及文件（防呆）
            string logfile = LogTime + "_" + Sys.MachineId + "_" + Sys.CurrentProduction + "_" + RiReader.Barcode + ".txt";//+ "_" + LogSerial
            string logpath = Sys.ReportLog1 + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(logpath))
            {
                LogDate = DateTime.Now.ToString("yyyy-MM-dd");
                LogTime = DateTime.Now.ToString("yyyyMMdd");
                iniFile.Write("LOGMESSAGE", "LogdTime", LogTime, setpath);
                iniFile.Write("LOGMESSAGE", "Logdate", LogDate, setpath);
                Directory.CreateDirectory(logpath);
            }
            if (!File.Exists(logpath + "\\" + logfile))
            {
                File.WriteAllText(logpath + "\\" + logfile, FileHeader);
            }
            #endregion
            ProductCount = 0;
        }
        public void ChangeNamePicture(int location, string strBarResult) //图片（+Barcode）重命名
        {
            try
            {
                string strResult = ""; string chPath = "";
                switch (strBarResult)
                {
                    case "1": strResult = "PASS"; break;
                    case "2": strResult = "NG"; break;
                }
                NumAddLog = Sys.NumAdd;
                switch (location)
                {
                    case 1:
                        pathIP = new string[3] { "PCCD2-Platform1", "GCCD2-1", "A1CCD2-PickUp" };
                        if (PCCD2.isPUAVI)
                            pathIP = new string[4] { "PCCD2-PickUp", "PCCD2-Platform1", "GCCD2-1", "A1CCD2-PickUp" };
                        Glue.GlueDisR = Glue.P1GlueDisR;
                        Glue.GlueDis1 = Glue.P1GlueDis1;
                        Glue.GlueDis2 = Glue.P1GlueDis2; break;
                    case 2:
                        pathIP = new string[3] { "PCCD2-Platform2", "GCCD2-2", "A2CCD2-PickUp" };
                        if (PCCD2.isPUAVI)
                            pathIP = new string[4] { "PCCD2-PickUp", "PCCD2-Platform2", "GCCD2-2", "A2CCD2-PickUp" };
                        Glue.GlueDisR = Glue.P2GlueDisR;
                        Glue.GlueDis1 = Glue.P2GlueDis1;
                        Glue.GlueDis2 = Glue.P2GlueDis2; break;
                }
                if (!Barcode1.QCCDisChecked) //QCCDAVI(胶)
                {
                    List<string> listpath = pathIP.ToList<string>();
                    listpath.Add("QCCD");
                    pathIP = listpath.ToArray();
                }
                for (int i = 0; i < pathIP.Length; i++)
                {
                    strResult = "PASS";
                    if (Glue.GlueOutResult == "PASS" & (pathIP[i] == "PCCD2-PickUp" || pathIP[i] == "PCCD2-Platform1" || pathIP[i] == "PCCD2-Platform2"))
                        strResult = "PASS";
                    if ((Glue.GlueOutResult == "NG" || Glue.GlueOutResult == "NG1" || Glue.GlueOutResult == "NG2")
                        & (pathIP[i] == "PCCD2-PickUp" || pathIP[i] == "PCCD2-Platform1" || pathIP[i] == "PCCD2-Platform2"))
                        strResult = "NG";
                    chPath = Sys.ReportImage + "\\" + FrmMain.mtime + "\\" + Sys.CurrentProduction + "\\" + pathIP[i] + "\\" + strResult + "\\ResultImage";
                    FileTimeInfo file = new FileTimeInfo();
                    if (pathIP[i] != "A1CCD2-PickUp" & pathIP[i] != "A2CCD2-PickUp")
                    {
                        file = GetLatestFileTimeInfo(chPath, ".png");
                        #region change
                        if (file != null)
                        {
                            if (!file.ChFileName.Contains("_" + strResult + "_"))
                            {
                                string lFileName = file.ChFileName;
                                string lnewFileName = file.ChFileName.Substring(0, file.ChFileName.Length - 4) + "_" +
                                                      pathIP[i] + "_" + strResult + "_" + Barcode1.sResultBarcode + ".png";
                                if (!ModifyFilename(lFileName, lnewFileName))
                                {
                                    try
                                    {
                                        if (File.Exists(lFileName))
                                            File.Move(lFileName, lnewFileName);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    if ((pathIP[i] == "A1CCD2-PickUp" & A1CCD2.HCoatCh1) || (pathIP[i] == "A2CCD2-PickUp" & A2CCD2.HCoatCh2))
                    {
                        Sys.rmin = 0.0;
                        Sys.diam_min = 0.0;
                        if (strD1[curt3x, curt3y] != "")
                            Sys.rmin = double.Parse(strD1[curt3x, curt3y]);
                        if (strDmin[curt3x, curt3y] != "")
                            Sys.diam_min = double.Parse(strDmin[curt3x, curt3y]);
                        Sys.CoatResult = strRlt[curt3x, curt3y];
                        #region change
                        string lFileName = chPath + "\\" + curt3x.ToString() + "a" + curt3y.ToString() + ".png";
                        string lnewFileName = chPath + "\\" + curt3x.ToString() + "a" + curt3y.ToString() + "_" + pathIP[i] +
                                              "_" + strResult + "_" + Barcode1.sResultBarcode + ".png";
                        if (!ModifyFilename(lFileName, lnewFileName))
                        {
                            try
                            {
                                if (File.Exists(lFileName))
                                    File.Move(lFileName, lnewFileName);
                            }
                            catch
                            { }
                        }
                        #endregion
                        curt3x = 0; curt3y = 0;
                    }
                    if ((pathIP[i] == "A1CCD2-PickUp" & Mode5A1CCD2.isMode5) || (pathIP[i] == "A2CCD2-PickUp" & Mode5A2CCD2.isMode5))
                    {
                        Sys.Mode5result = "";
                        Sys.Mode5Distance = "";
                        Sys.Mode5Distance1 = "";
                        Sys.Mode5Distance2 = "";
                        Sys.Mode5result = FrmMain.strresult[curt3x, curt3y];
                        Sys.Mode5Distance = FrmMain.strDistance[curt3x, curt3y];
                        Sys.Mode5Distance1 = FrmMain.strDistance1[curt3x, curt3y];
                        Sys.Mode5Distance2 = FrmMain.strDistance2[curt3x, curt3y];
                        #region change
                        string lFileName = chPath + "\\" + curt3x.ToString() + "a" + curt3y.ToString() + ".png";
                        string lnewFileName = chPath + "\\" + curt3x.ToString() + "a" + curt3y.ToString() + "_" + pathIP[i] +
                                              "_" + strResult + "_" + Barcode1.sResultBarcode + ".png";
                        if (!ModifyFilename(lFileName, lnewFileName))
                        {
                            try
                            {
                                if (File.Exists(lFileName))
                                    File.Move(lFileName, lnewFileName);
                            }
                            catch
                            { }
                        }
                        #endregion
                        curt3x = 0; curt3y = 0;
                    }  
                }
                #region PCCD1 Dis change图片
                chPath = Sys.ReportImage + "\\" + FrmMain.mtime + "\\" + Sys.CurrentProduction + "\\PCCD1\\" + Sys.P1Result + "\\ResultImage";
                FileTimeInfo p1file = new FileTimeInfo();
                p1file = GetLatestFileTimeInfo(chPath, ".png");
                if (p1file != null)
                {
                    if (!p1file.ChFileName.Contains("_" + Sys.P1Result + "_"))
                    {
                        string lFileName = p1file.ChFileName;
                        string lnewFileName = p1file.ChFileName.Substring(0, p1file.ChFileName.Length - 4) + "_" +
                                              "PCCD1" + "_" + Sys.P1Result + "_" + Barcode1.sResultBarcode + ".png";
                        if (!ModifyFilename(lFileName, lnewFileName))
                        {
                            try
                            {
                                if (File.Exists(lFileName))
                                    File.Move(lFileName, lnewFileName);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                #endregion
            }
            catch
            {
            }
        }
        public void ChangeNamePicture1(string TrayName)  //图片（+Barcode）重命名 ps：未组装的Lens
        {
            try
            {
                string chPath = "";
                pathIP1 = new string[4] { "A1CCD2-PickUp\\PASS\\OriginalImage", "A1CCD2-PickUp\\PASS\\ResultImage",
                                         "A2CCD2-PickUp\\PASS\\OriginalImage", "A2CCD2-PickUp\\NG\\ResultImage" };
                for (int i = 0; i < pathIP1.Length; i++)
                {
                    chPath = Sys.ReportImage + "\\" + mtime + "\\" + Sys.CurrentProduction + "\\" + pathIP1[i];
                    DirectoryInfo theFolder = new DirectoryInfo(chPath);
                    FileInfo[] TfileInfo = theFolder.GetFiles();
                    for (int j = 0; j < TfileInfo.Length; j++)
                    {
                        if (TfileInfo[j].Name.Length <= 9)
                        {
                            string lFileName = chPath + "\\" + TfileInfo[j].Name;
                            string lnewFileName = chPath + "\\" + TrayName + "_" + TfileInfo[j].Name;
                            if (!ModifyFilename(lFileName, lnewFileName))
                            {
                                try
                                {
                                    if (File.Exists(lFileName))
                                        File.Move(lFileName, lnewFileName);
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        public bool ModifyFilename(string Filename, string newFilename)
        {
            try
            {
                if (File.Exists(Filename))
                {
                    File.Move(Filename, newFilename);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        static FileTimeInfo GetLatestFileTimeInfo(string dir, string ext) //获取最新修改的文件名称及时间
        {
            try
            {
                List<FileTimeInfo> list = new List<FileTimeInfo>();
                DirectoryInfo d = new DirectoryInfo(dir);
                foreach (FileInfo file in d.GetFiles())
                {
                    if (file.Extension.ToUpper() == ext.ToUpper())
                    {
                        list.Add(new FileTimeInfo()
                        {
                            ChFileName = file.FullName,
                            ChFileCreateTime = file.CreationTime
                        });
                    }
                }
                var f = from x in list
                        orderby x.ChFileCreateTime
                        select x;
                return f.LastOrDefault();
            }
            catch
            {
                List<FileTimeInfo> list = new List<FileTimeInfo>();
                var f = from x in list
                        orderby x.ChFileCreateTime
                        select x;
                return f.LastOrDefault();
            }
        }
        #endregion

        #region //测高
      
        void ConnectToGT2()
        {
            try
            {
                //if (GT2.IsConnected)
                GT2.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                GT2.socket.ReceiveTimeout = 3000;
                GT2.socket.SendTimeout = 3000;
                GT2.socket.Close();
                GT2.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                GT2.socket.ReceiveTimeout = 3000;
                GT2.socket.SendTimeout = 3000;
                GT2.socket.LingerState = new LingerOption(false, 1);
                IPEndPoint ipe = new IPEndPoint(GT2.ip, GT2.Port);
                GT2.socket.Connect(ipe);
                GT2.IsConnected = true;
            }
            catch
            {
                GT2.IsConnected = false;
                GT2.socket.Close();
            }
        }
        void GT2Read(int n, byte[] output)
        {
            try
            {
                GT2.socket.Send(output);  //读取测高器原始值
                Thread.Sleep(50);
                byte[] recv = new byte[4096];
                int len = GT2.socket.Receive(recv);
                string height = Encoding.ASCII.GetString(recv, 0, len);
                if (len < 2)
                    throw new Exception();
                if (height.Substring(0, 2) == "SR")
                    RealHight[n] = (Convert.ToDouble(height.Substring(10)) / 1000).ToString("f3") + "mm";
                else
                    RealHight[n] = height.Substring(10);
                WriteToPlc.strCmdGT2[n] = NToHString(Convert.ToInt32(height.Substring(10)));
                WriteToPlc.bCmdGT2[n] = true;
            }
            catch // (Exception er)
            {
                GT2.IsConnected = false;
                //MessageBox.Show(er.ToString());
            }
        }
        void GT2Read2()
        {
            try
            {
                GT2.socket.Send(Encoding.ASCII.GetBytes("M0\r\n"));  //读取测高器原始值
                Thread.Sleep(50);
                byte[] recv = new byte[4096];
                int len = GT2.socket.Receive(recv);
                string height = Encoding.ASCII.GetString(recv, 0, len);
                if (len < 2)
                    throw new Exception();
                double[] dHeight = new double[5];
                if (height.Substring(0, 2) == "M0")
                {
                    string height1 = height.Substring(3);
                    string height2 = height1.Substring(0, height1.Length - 2);
                    string[] btData = height2.Split(',');
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            dHeight[i] = Convert.ToDouble(btData[i]) / 1000;
                        }
                        catch
                        {
                        }
                    }
                }
                if (GT2.CH1IsCheck && GT2.IsConnected)
                {
                    RealHight[0] = dHeight[0].ToString("f3") + "mm";
                    WriteToPlc.strCmdGT2[0] = NToHString((Int32)(dHeight[0] * 1000));
                    WriteToPlc.bCmdGT2[0] = true;
                }
                if (GT2.CH2IsCheck && GT2.IsConnected)
                {
                    RealHight[1] = dHeight[1].ToString("f3") + "mm";
                    WriteToPlc.strCmdGT2[1] = NToHString((Int32)(dHeight[1] * 1000));
                    WriteToPlc.bCmdGT2[1] = true;
                }
                if (GT2.CH1IsCheck && GT2.IsConnected)
                {
                    RealHight[2] = dHeight[2].ToString("f3") + "mm";
                    WriteToPlc.strCmdGT2[2] = NToHString((Int32)(dHeight[2] * 1000));
                    WriteToPlc.bCmdGT2[2] = true;
                }
                if (GT2.CH2IsCheck && GT2.IsConnected)
                {
                    RealHight[3] = dHeight[3].ToString("f3") + "mm";
                    WriteToPlc.strCmdGT2[3] = NToHString((Int32)(dHeight[3] * 1000));
                    WriteToPlc.bCmdGT2[3] = true;
                }
                if (GT2.CH2IsCheck && GT2.IsConnected)
                {
                    RealHight[4] = dHeight[4].ToString("f3") + "mm";
                    WriteToPlc.strCmdGT2[4] = NToHString((Int32)(dHeight[4] * 1000));
                    WriteToPlc.bCmdGT2[4] = true;
                }
            }
            catch // (Exception er)
            {
                GT2.IsConnected = false;
                //MessageBox.Show(er.ToString());
            }
        }
        private void DLEN1loop_DoWork(object sender, DoWorkEventArgs e)  //GT2
        {
            Thread.Sleep(3000);
            while (true)
            {
                if (DLEN1loop.CancellationPending)
                    return;
                if (quit && GT2.IsConnected)
                {
                    GT2.socket.Close();
                    break;
                }
                if (!GT2.IsConnected)
                {
                    Thread.Sleep(10);
                    continue;
                }
                try
                {
                    if (!GT2.clearCH)
                    {
                        GT2Read2();
                        #region 各通道
                        //if (GT2.CH1IsCheck && GT2.IsConnected)
                        //    GT2Read(0, Encoding.ASCII.GetBytes("SR,01,038\r\n"));
                        //if (GT2.CH2IsCheck && GT2.IsConnected)
                        //    GT2Read(1, Encoding.ASCII.GetBytes("SR,02,038\r\n"));
                        //if (GT2.CH3IsCheck && GT2.IsConnected)
                        //    GT2Read(2, Encoding.ASCII.GetBytes("SR,03,038\r\n"));
                        //if (GT2.CH4IsCheck && GT2.IsConnected)
                        //    GT2Read(3, Encoding.ASCII.GetBytes("SR,04,038\r\n"));
                        //if (GT2.CH5IsCheck && GT2.IsConnected)
                        //    GT2Read(4, Encoding.ASCII.GetBytes("SR,05,038\r\n"));
                        #endregion
                    }
                    else
                    {
                        if (GT2.IsConnected)
                        {
                            GT2.clearCH = false;
                            GT2.socket.Send(GT2.clearCHcmd);  //写入清零指令,读取测高器原始值
                            Thread.Sleep(50);
                            byte[] recv1 = new byte[4096];
                            int len1 = GT2.socket.Receive(recv1);
                            if (len1 == 0)
                                return;
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        #region  //进制转换
        public static string NToHString(int iNumber) 
        {
            string strResult = string.Empty;
            if (iNumber < 0)
            {
                iNumber = -iNumber;
                string strNegate = string.Empty;
                char[] binChar = Convert.ToString(iNumber, 2).PadLeft(16, '0').ToArray();
                char[] b = new char[] { '0' };
                int charlens = binChar.Length;
                if (charlens < 32)
                {
                    for (int i = 0; i < 32 - charlens; i++)
                    {
                        binChar = b.Concat(binChar).ToArray();
                    }
                }
                foreach (char ch in binChar)
                {
                    if (Convert.ToInt32(ch) == 48)
                        strNegate += "1";
                    else
                        strNegate += "0";
                }
                int iComplement = Convert.ToInt32(strNegate, 2) + 1;
                strResult = Convert.ToString(iComplement, 16).ToUpper();
                while (strResult.Length < 8)
                {
                    strResult = 'F' + strResult;
                }
            }
            else
            {
                strResult = Convert.ToString(iNumber, 16).ToUpper();
                while (strResult.Length < 8)
                {
                    strResult = '0' + strResult;
                }
            }
            strResult = strResult.Substring(4, 4) + strResult.Substring(0, 4);
            return strResult;
        }
        public static string HexString2BinString(string hexString)
        {
            string result = string.Empty;
            foreach (char c in hexString)
            {
                int v = Convert.ToInt32(c.ToString(), 16);
                int v2 = int.Parse(Convert.ToString(v, 2));
                result += string.Format("{0:d4}", v2);
            }
            return result;
        }
        public static bool IsNumber(string str)
        {
            bool blResult = true;
            if (str == "")
            {
                blResult = false;
            }
            else
            {
                foreach (char Char in str)
                {
                    if (!char.IsNumber(Char))
                    {
                        blResult = false;
                        break;
                    }
                }
            }
            return blResult;
        }
        #endregion

        void DeleFiles()
        {
            try
            {
                #region Delefiles
                string[] files = Directory.GetDirectories(Sys.ReportImage);
                foreach (string file in files)
                {
                    string s = file;
                    FileInfo f = new FileInfo(s);
                    DateTime nowtime = DateTime.Now;
                    TimeSpan t = nowtime - f.CreationTime;
                    int day = t.Days;
                    if (day > 2)
                        Directory.Delete(s, true);
                }
                files = Directory.GetDirectories(Sys.ReportLog1);
                foreach (string file in files)
                {
                    string s = file;
                    string ssub = s.Substring(s.Length - 3, 3);
                    if (ssub != "Log")
                    {
                        FileInfo f = new FileInfo(s);
                        DateTime nowtime = DateTime.Now;
                        TimeSpan t = nowtime - f.CreationTime;
                        int day = t.Days;
                        if (day > 7)
                            Directory.Delete(s, true);
                    }
                    else
                    {
                        string lipath = "";
                        if (s.Substring(s.Length - 5, 5) == "EELog")
                            lipath = Sys.ReportLog1 + "\\OEELog";
                        DirectoryInfo dyInfoli = new DirectoryInfo(lipath);
                        foreach (FileInfo fInfo in dyInfoli.GetFiles())
                        {
                            DateTime nowtime = DateTime.Now;
                            TimeSpan t = nowtime - fInfo.CreationTime;
                            int day = t.Days;
                            if (day > 30)
                                fInfo.Delete();
                        }
                    }
                }
                files = Directory.GetDirectories(Sys.ReportLog);
                foreach (string file in files)
                {
                    string s = file;
                    string ssub = s.Substring(s.Length - 3, 3);
                    if (ssub != "Log")
                    {
                        FileInfo f = new FileInfo(s);
                        DateTime nowtime = DateTime.Now;
                        TimeSpan t = nowtime - f.CreationTime;
                        int day = t.Days;
                        if (day > 7)
                            Directory.Delete(s, true);
                    }
                    else
                    {
                        string lipath = "";
                        if (s.Substring(s.Length - 5, 5) == "ueLog")
                            lipath = Sys.ReportLog + "\\GlueLog";
                        if (s.Substring(s.Length - 5, 5) == "atLog")
                            lipath = Sys.ReportLog + "\\CoatLog";
                        DirectoryInfo dyInfoli = new DirectoryInfo(lipath);
                        foreach (FileInfo fInfo in dyInfoli.GetFiles())
                        {
                            DateTime nowtime = DateTime.Now;
                            TimeSpan t = nowtime - fInfo.CreationTime;
                            int day = t.Days;
                            if (day > 30)
                                fInfo.Delete();
                        }
                    }
                }
                #endregion
                #region Delefile
                DirectoryInfo dyInfo = new DirectoryInfo(Sys.AlarmPath);
                foreach (FileInfo fInfo in dyInfo.GetFiles())
                {
                    DateTime nowtime = DateTime.Now;
                    TimeSpan t = nowtime - fInfo.CreationTime;
                    int day = t.Days;
                    if (day > 30)
                        fInfo.Delete();
                }
                #endregion
            }
            catch
            {
            }
        }

        #region Zip 暂未用 耗时3~5min
        private void btnZip_Click(object sender, EventArgs e)
        {
            string dy = DateTime.Now.Year.ToString(), dm = DateTime.Now.Month.ToString(), dd = DateTime.Now.AddDays(-1).Day.ToString();
            if (DateTime.Now.Month < 10)
                dm = "0" + dm;
            if (DateTime.Now.Day <= 10)
                dd = "0" + dd;
            string lpath = Sys.ReportImage + "\\" + dy + dm + dd;
            #region
            string[] strs1 = new string[2], strs2 = new string[2], strs3 = new string[2], strs4 = new string[2];
            string[] strs5 = new string[2], strs6 = new string[2], strs7 = new string[2], strs8 = new string[2];
            string[] strs9 = new string[2], strs10 = new string[2], strs11 = new string[2], strs12 = new string[2];
            if (!Directory.Exists(lpath + "\\8982LH\\IT_ECC"))
                Directory.CreateDirectory(lpath + "\\8982LH\\IT_ECC");
            else
                return;
            Run.lblZip.Show();
            DateTime c1s = DateTime.Now;
            #region PASS
            strs1[0] = lpath + "\\8982LH\\A1CCD2-PickUp\\PASS\\ResultImage";
            strs1[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_A1CCD2-PickUp_PASS_ResultImage.zip";
            strs2[0] = lpath + "\\8982LH\\A2CCD2-PickUp\\PASS\\ResultImage";
            strs2[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_A2CCD2-PickUp_PASS_ResultImage.zip";
            strs5[0] = lpath + "\\8982LH\\PCCD2-Platform1\\PASS\\ResultImage";
            strs5[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_PCCD2-Platform1_PASS_ResultImage.zip";
            strs6[0] = lpath + "\\8982LH\\PCCD2-Platform2\\PASS\\ResultImage";
            strs6[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_PCCD2-Platform2_PASS_ResultImage.zip";
            strs9[0] = lpath + "\\8982LH\\GCCD2-1\\PASS\\ResultImage";
            strs9[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_GCCD2-1_PASS_ResultImage.zip";
            strs10[0] = lpath + "\\8982LH\\GCCD2-2\\PASS\\ResultImage";
            strs10[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_GCCD2-2_PASS_ResultImage.zip";
            //Thread t1 = new Thread(() =>
            //{
            //    ZipFile(strs1[0], strs1[1]);
            //});
            //Thread t2 = new Thread(() =>
            //{
            //    ZipFile(strs2[0], strs2[1]);
            //});
            //Thread t5 = new Thread(() =>
            //{
            //    ZipFile(strs5[0], strs5[1]);
            //});
            //Thread t6 = new Thread(() =>
            //{
            //    ZipFile(strs6[0], strs6[1]);
            //});
            //Thread t9 = new Thread(() =>
            //{
            //    ZipFile(strs9[0], strs9[1]);
            //});
            //Thread t10 = new Thread(() =>
            //{
            //    ZipFile(strs10[0], strs10[1]);
            //});
            #endregion
            #region NG
            strs3[0] = lpath + "\\8982LH\\A1CCD2-PickUp\\NG\\ResultImage";
            strs3[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_A1CCD2-PickUp_NG_ResultImage.zip";
            strs4[0] = lpath + "\\8982LH\\A2CCD2-PickUp\\NG\\ResultImage";
            strs4[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_A2CCD2-PickUp_NG_ResultImage.zip";
            strs7[0] = lpath + "\\8982LH\\PCCD2-Platform1\\NG\\ResultImage";
            strs7[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_PCCD2-Platform1_NG_ResultImage.zip";
            strs8[0] = lpath + "\\8982LH\\PCCD2-Platform2\\NG\\ResultImage";
            strs8[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_PCCD2-Platform2_NG_ResultImage.zip";
            strs11[0] = lpath + "\\8982LH\\GCCD2-1\\NG\\ResultImage";
            strs11[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_GCCD2-1_NG_ResultImage.zip";
            strs12[0] = lpath + "\\8982LH\\GCCD2-2\\NG\\ResultImage";
            strs12[1] = lpath + "\\8982LH\\IT_ECC\\" + Sys.MachineId + "_GCCD2-2_NG_ResultImage.zip";
            //Thread t3 = new Thread(() =>
            //{
            //    ZipFile(strs3[0], strs3[1]);
            //});
            //Thread t4 = new Thread(() =>
            //{
            //    ZipFile(strs4[0], strs4[1]);
            //});
            //Thread t7 = new Thread(() =>
            //{
            //    ZipFile(strs7[0], strs7[1]);
            //});
            //Thread t8 = new Thread(() =>
            //{
            //    ZipFile(strs8[0], strs8[1]);
            //});

            //Thread t11 = new Thread(() =>
            //{
            //    ZipFile(strs11[0], strs11[1]);
            //});
            //Thread t12 = new Thread(() =>
            //{
            //    ZipFile(strs12[0], strs12[1]);
            //});
            #endregion
            #region 多线程
            //if (Directory.Exists(strs1[0]))
            //    t1.Start();
            //if (Directory.Exists(strs2[0]))
            //    t2.Start();
            //if (Directory.Exists(strs3[0]))
            //    t3.Start();
            //if (Directory.Exists(strs4[0]))
            //    t4.Start();
            //if (Directory.Exists(strs5[0]))
            //    t5.Start();
            //if (Directory.Exists(strs6[0]))
            //    t6.Start();
            //if (Directory.Exists(strs7[0]))
            //    t7.Start();
            //if (Directory.Exists(strs8[0])) 
            //    t8.Start();
            //if (Directory.Exists(strs9[0])) 
            //    t9.Start();
            //if (Directory.Exists(strs10[0]))
            //    t10.Start();
            //if (Directory.Exists(strs11[0])) 
            //    t11.Start();
            //if (Directory.Exists(strs12[0]))
            //    t12.Start();

            //if (Directory.Exists(strs1[0])) 
            //    t1.Join();
            //if (Directory.Exists(strs2[0])) 
            //    t2.Join();
            //if (Directory.Exists(strs3[0])) 
            //    t3.Join();
            //if (Directory.Exists(strs4[0])) 
            //    t4.Join();
            //if (Directory.Exists(strs5[0])) 
            //    t5.Join();
            //if (Directory.Exists(strs6[0])) 
            //    t6.Join();
            //if (Directory.Exists(strs7[0])) 
            //    t7.Join();
            //if (Directory.Exists(strs8[0]))
            //    t8.Join();
            //if (Directory.Exists(strs9[0])) 
            //    t9.Join();
            //if (Directory.Exists(strs10[0])) 
            //    t10.Join();
            //if (Directory.Exists(strs11[0])) 
            //    t11.Join();
            //if (Directory.Exists(strs12[0])) 
            //    t12.Join();
            #endregion
            Parallel.For(0, 11, i =>
            {

            });
            Run.lblZip.Hide();
            DateTime c1e = DateTime.Now;
            TimeSpan ts = c1e.Subtract(c1s).Duration();
            string tt = Math.Round(ts.TotalMilliseconds).ToString();
            #endregion
        }
        private void ZipFile(string strFile, string strZip)
        {
            var len = strFile.Length;
            var strlen = strFile[len - 1];
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
            {
                strFile += Path.DirectorySeparatorChar;
            }
            ZipOutputStream outstream = new ZipOutputStream(File.Create(strZip));
            outstream.SetLevel(6);
            zip(strFile, outstream, strFile);
            outstream.Finish();
            outstream.Close();
        }
        private void zip(string strFile, ZipOutputStream outstream, string staticFile)
        {
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
            {
                strFile += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();
            //获取指定目录下所有文件和子目录文件名称
            string[] filenames = Directory.GetFileSystemEntries(strFile);
            //遍历文件
            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    //string pPath = staticFile;
                    //pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    //pPath += "\\";
                    //zip(file, outstream, pPath);
                    zip(file, outstream, staticFile);
                }
                //直接压缩文件
                else
                {
                    //打开压缩文件
                    FileStream fs = File.OpenRead(file);
                    //定义缓存区对象
                    byte[] buffer = new byte[fs.Length];
                    //通过字符流、读取文件
                    fs.Read(buffer, 0, buffer.Length);
                    //得到目录下的文件
                    string temfile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    //string temfile = staticFile + file.Substring(file.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(temfile);

                    entry.Size = fs.Length;

                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;
                    outstream.PutNextEntry(entry);
                    //写文件
                    outstream.Write(buffer, 0, buffer.Length);
                }
            }
        }
        #endregion

        private void timerRefleshUI_Tick(object sender, EventArgs e)
        {
            if (labMachineName.Text != Sys.MachineId)
                labMachineName.Text = Sys.MachineId;
            if (labProduceName.Text != Sys.CurrentProduction)
                labProduceName.Text = Sys.CurrentProduction;
            if (lblFactory.Text != Sys.Factory)
                lblFactory.Text = Sys.Factory;

            ConStatus[15] = (A1CCD2.IsConnected ? "0" : "1"); //"1" : "0"
            ConStatus[14] = (A2CCD2.IsConnected ? "0" : "1");
            ConStatus[13] = (PCCD2.IsConnected ? "0" : "1");
            ConStatus[12] = (QCCD.IsConnected ? "0" : "1");
            ConStatus[11] = (GCCD2.IsConnected ? "0" : "1");
            ConStatus[10] = (A1CCD1.IsConnected ? "0" : "1");
            ConStatus[9] = (A2CCD1.IsConnected ? "0" : "1");
            ConStatus[8] = (PCCD1.IsConnected ? "0" : "1");
            ConStatus[7] = (GT2.IsConnected ? "0" : "1");
            ConStatus[6] = (GCCD1.IsConnected ? "0" : "1");
            ConStatus[3] = (Barcode1.IsConnected ? "0" : "1"); //p Barcode reader
            ConStatus[4] = (Barcode2.IsConnected ? "0" : "1");
            ConStatus[5] = (Barcode3.IsConnected ? "0" : "1");
            ConStatus[2] = (Reader.IsConnected ? "0" : "1");
            ConStatus[1] = (RiReader.IsConnected ? "0" : "1");
            string by = "";
            for (int i = 0; i < ConStatus.Length; i++)
            {
                by = by + ConStatus[i];
            }
            string cby = string.Format("{0:X}", Convert.ToInt32(by, 2));
            cmdstatus = cby.PadLeft(4, '0');

            #region FrmDisolay1
            if (!ScanStatus & Scaning)
                Run.lbling.Show();
            if (!Scaning & ScanStatus)
                Run.lbling.Hide();
            Run.btnConfirm.Enabled = ((PLC.MachineStatus == "2" || PLC.MachineStatus == "10") ? false : true);
            Run.btnUntie.Enabled = ((PLC.MachineStatus == "2" || PLC.MachineStatus == "10") ? false : true);
            Run.btnClearT1.Enabled = ((PLC.MachineStatus == "2" || PLC.MachineStatus == "10") ? false : true);
            Run.btnClearT2.Enabled = ((PLC.MachineStatus == "2" || PLC.MachineStatus == "10") ? false : true);
            if (Run.txtTray1Barcode.Text != RiReader.Barcode)
                Run.txtTray1Barcode.Text = RiReader.Barcode;
            if (Run.txtTray2Barcode.Text != Reader.Barcode)
                Run.txtTray2Barcode.Text = Reader.Barcode;
            Run.lblA11Num.Text = (FrmMain.processing[0] ? "1" : "0");
            Run.lblA12Num.Text = A1CCD2.IntSingle.ToString();
            Run.lblA21Num.Text = (FrmMain.processing[2] ? "1" : "0");
            Run.lblA22Num.Text = A2CCD2.IntSingle.ToString();
            Run.lblRT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", webGlueHours, webGlueMinutes, webGlueSeconds);
            Run.lblGRT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", GlueHours, GlueMinutes, GlueSeconds);
            #endregion

            #region 暂时未用
            #region FrmDisolay2  
            //Run2.lblP1Num.Text = (FrmMain.processing[4] ? "1" : "0");
            //Run2.lblP2Num.Text = PCCD2.IntSingle.ToString();
            //Run2.lblG1Num.Text = (FrmMain.processing[6] ? "1" : "0");
            //Run2.lblG2Num.Text = GCCD2.IntPosition.ToString() + "." + GCCD2.IntSingle.ToString();
            //Run2.lblQNum.Text = (FrmMain.processing[8] ? "1" : "0");
            //if (Run2.labBarTime.Text != bTspan)
            //    Run2.labBarTime.Text = bTspan;
            //if (Run2.lblBarID.Text != Sys.CurrentProID)
            //    Run2.lblBarID.Text = Sys.CurrentProID;
            //#region Barcode
            //Barcode1.bColor = (Barcode1.IsConnected ? Color.Green : Color.Red);
            //if (Barcode1.IsChecked & Run2.QRC1ConnStatus.BackColor != Barcode1.bColor)
            //    Run2.QRC1ConnStatus.BackColor = Barcode1.bColor;
            //#endregion
            //#region 测高
            //if (GT2.IsConnected)
            //{
            //    if (Run2.txtGT2RV1.Text != FrmMain.RealHight[0])
            //        Run2.txtGT2RV1.Text = FrmMain.RealHight[0];
            //    if (Run2.txtGT2RV2.Text != FrmMain.RealHight[1])
            //        Run2.txtGT2RV2.Text = FrmMain.RealHight[1];
            //    if (Run2.txtGT2RV3.Text != FrmMain.RealHight[2])
            //        Run2.txtGT2RV3.Text = FrmMain.RealHight[2];
            //    if (Run2.txtGT2RV4.Text != FrmMain.RealHight[3])
            //        Run2.txtGT2RV4.Text = FrmMain.RealHight[3];
            //}
            //if (Run2.GT2Status.BackColor != GT2.gColor)
            //    Run2.GT2Status.BackColor = GT2.gColor;
            //#endregion
            //#region 点胶控制器
            //Glue.gColor = (Glue.IsConnected ? Color.Green : Color.Red);
            //if (Run2.GlueCUConnStatus.BackColor != Glue.gColor)
            //    Run2.GlueCUConnStatus.BackColor = Glue.gColor;
            //#endregion
            //#region PLC
            //PLC.pColor = (PLC.IsConnected ? Color.Green : Color.Red);
            //if (Run2.PlcConnStatus.BackColor != PLC.pColor)
            //    Run2.PlcConnStatus.BackColor = PLC.pColor;
            //#endregion
            //#region CCD
            //A1CCD1.color = (A1CCD1.IsConnected ? Color.Green : Color.Red);
            //if (Run2.A11ConnStatus.BackColor != A1CCD1.color)
            //    Run2.A11ConnStatus.BackColor = A1CCD1.color;
            //A1CCD2.color = (A1CCD2.IsConnected ? Color.Green : Color.Red);
            //if (Run2.A12ConnStatus.BackColor != A1CCD2.color)
            //    Run2.A12ConnStatus.BackColor = A1CCD2.color;
            //A2CCD1.color = (A2CCD1.IsConnected ? Color.Green : Color.Red);
            //if (Run2.A21ConnStatus.BackColor != A2CCD1.color)
            //    Run2.A21ConnStatus.BackColor = A2CCD1.color;
            //A2CCD2.color = (A2CCD2.IsConnected ? Color.Green : Color.Red);
            //if (Run2.A22ConnStatus.BackColor != A2CCD2.color)
            //    Run2.A22ConnStatus.BackColor = A2CCD2.color;
            //PCCD1.color = (PCCD1.IsConnected ? Color.Green : Color.Red);
            //if (Run2.P1ConnStatus.BackColor != PCCD1.color)
            //    Run2.P1ConnStatus.BackColor = PCCD1.color;
            //PCCD2.color = (PCCD2.IsConnected ? Color.Green : Color.Red);
            //if (Run2.P2ConnStatus.BackColor != PCCD2.color)
            //    Run2.P2ConnStatus.BackColor = PCCD2.color;
            //GCCD1.color = (GCCD1.IsConnected ? Color.Green : Color.Red);
            //if (Run2.G1ConnStatus.BackColor != GCCD1.color)
            //    Run2.G1ConnStatus.BackColor = GCCD1.color;
            //GCCD2.color = (GCCD2.IsConnected ? Color.Green : Color.Red);
            //if (Run2.G2ConnStatus.BackColor != GCCD2.color)
            //    Run2.G2ConnStatus.BackColor = GCCD2.color;
            //QCCD.color = (QCCD.IsConnected ? Color.Green : Color.Red);
            //if (Run2.QConnStatus.BackColor != QCCD.color)
            //    Run2.QConnStatus.BackColor = QCCD.color;
            //#endregion
            //#region Light
            //LIGHT1.lColor = (LIGHT1.IsConnected ? Color.Green : Color.Red);
            //if (Run2.L1ConnStatus.BackColor != LIGHT1.lColor)
            //    Run2.L1ConnStatus.BackColor = LIGHT1.lColor;
            //LIGHT2.lColor = (LIGHT2.IsConnected ? Color.Green : Color.Red);
            //if (Run2.L2ConnStatus.BackColor != LIGHT2.lColor)
            //    Run2.L2ConnStatus.BackColor = LIGHT2.lColor;
            //#endregion
            #endregion
            {
                //if (!Barcode1.IsConnected && Run2.QRC1ConnStatus.BackColor != Color.Red)
                //    Run2.QRC1ConnStatus.BackColor = Color.Red;
                //if (Barcode1.IsConnected && Run2.QRC1ConnStatus.BackColor != Color.Green)
                //    Run2.QRC1ConnStatus.BackColor = Color.Green;
            }
            //if (PLC.IsConnected && Run2.PlcConnStatus.BackColor == Color.Red)
            //    Run2.PlcConnStatus.BackColor = Color.Green;
            //if (!PLC.IsConnected && Run2.PlcConnStatus.BackColor == Color.Green)
            //    Run2.PlcConnStatus.BackColor = Color.Red;
            //if (LIGHT1.IsConnected && Run2.L1ConnStatus.BackColor == Color.Red)
            //    Run2.L1ConnStatus.BackColor = Color.Green;
            //if (!LIGHT1.IsConnected && Run2.L1ConnStatus.BackColor == Color.Green)
            //    Run2.L1ConnStatus.BackColor = Color.Red;
            //if (LIGHT2.IsConnected && Run2.L2ConnStatus.BackColor == Color.Red)
            //    Run2.L2ConnStatus.BackColor = Color.Green;
            //if (!LIGHT2.IsConnected && Run2.L2ConnStatus.BackColor == Color.Green)
            //    Run2.L2ConnStatus.BackColor = Color.Red;
            //if (A1CCD1.IsConnected && Run2.A11ConnStatus.BackColor == Color.Red)
            //    Run2.A11ConnStatus.BackColor = Color.Green;
            //if (!A1CCD1.IsConnected && Run2.A11ConnStatus.BackColor == Color.Green)
            //    Run2.A11ConnStatus.BackColor = Color.Red;
            //if (A1CCD2.IsConnected && Run2.A12ConnStatus.BackColor == Color.Red)
            //    Run2.A12ConnStatus.BackColor = Color.Green;
            //if (!A1CCD2.IsConnected && Run2.A12ConnStatus.BackColor == Color.Green)
            //    Run2.A12ConnStatus.BackColor = Color.Red;
            //if (A2CCD1.IsConnected && Run2.A21ConnStatus.BackColor == Color.Red)
            //    Run2.A21ConnStatus.BackColor = Color.Green;
            //if (!A2CCD1.IsConnected && Run2.A21ConnStatus.BackColor == Color.Green)
            //    Run2.A21ConnStatus.BackColor = Color.Red;
            //if (A2CCD2.IsConnected && Run2.A22ConnStatus.BackColor == Color.Red)
            //    Run2.A22ConnStatus.BackColor = Color.Green;
            //if (!A2CCD2.IsConnected && Run2.A22ConnStatus.BackColor == Color.Green)
            //    Run2.A22ConnStatus.BackColor = Color.Red;
            //if (PCCD1.IsConnected && Run2.P1ConnStatus.BackColor == Color.Red)
            //    Run2.P1ConnStatus.BackColor = Color.Green;
            //if (!PCCD1.IsConnected && Run2.P1ConnStatus.BackColor == Color.Green)
            //    Run2.P1ConnStatus.BackColor = Color.Red;
            //if (PCCD2.IsConnected && Run2.P2ConnStatus.BackColor == Color.Red)
            //    Run2.P2ConnStatus.BackColor = Color.Green;
            //if (!PCCD2.IsConnected && Run2.P2ConnStatus.BackColor == Color.Green)
            //    Run2.P2ConnStatus.BackColor = Color.Red;
            //if (GCCD1.IsConnected && Run2.G1ConnStatus.BackColor == Color.Red)
            //    Run2.G1ConnStatus.BackColor = Color.Green;
            //if (!GCCD1.IsConnected && Run2.G1ConnStatus.BackColor == Color.Green)
            //    Run2.G1ConnStatus.BackColor = Color.Red;
            //if (GCCD2.IsConnected && Run2.G2ConnStatus.BackColor == Color.Red)
            //    Run2.G2ConnStatus.BackColor = Color.Green;
            //if (!GCCD2.IsConnected && Run2.G2ConnStatus.BackColor == Color.Green)
            //    Run2.G2ConnStatus.BackColor = Color.Red;
            //if (QCCD.IsConnected && Run2.QConnStatus.BackColor == Color.Red)
            //    Run2.QConnStatus.BackColor = Color.Green;
            //if (!QCCD.IsConnected && Run2.QConnStatus.BackColor == Color.Green)
            //    Run2.QConnStatus.BackColor = Color.Red;
            #endregion
        }
        int webGlueHours = 0, webGlueMinutes = 0, webGlueSeconds = 0;
        int GlueHours = 0, GlueMinutes = 0, GlueSeconds = 0;
        private void bWtimerGlue_DoWork(object sender, DoWorkEventArgs e) 
        {
            while (true)
            {
                if (Sys.TimerGlue) //过账胶水倒计时
                {
                    #region 
                    try
                    {
                        int tsecond = Glue.GlueUseMins * 60;
                        DateTime gdt2 = DateTime.Now;
                        TimeSpan gts = gdt2 - FrmDisplay1.replaceTime;
                        if (gts.TotalSeconds <= tsecond & !Glue.IsGlueUntie)
                        {
                            double releseT = (double)tsecond - gts.TotalSeconds;
                            int left1 = (int)(releseT);// % 86400
                            webGlueHours = left1 / 3600;
                            left1 = left1 % 3600;
                            webGlueMinutes = left1 / 60;
                            webGlueSeconds = left1 % 60;
                        }
                        else
                        {
                            Glue.IsGlueUntie = false;
                            string bar="";
                            this.Invoke(new MethodInvoker(delegate
                                {
                                    bar = Run.txtTray1Barcode.Text;
                                }));
                            if (bar == "")
                            {
                                Protocol.strPCRead_GlueTimeOut = true;  //满盘信号时判断胶水时间超时
                                Protocol.IsPCRead = true;
                                Sys.TimerGlue = false;
                                webGlueHours = 0; webGlueMinutes = 0; webGlueSeconds = 0;
                                this.Invoke(new MethodInvoker(delegate
                                    {
                                        Run.txtGluePiao.Enabled = true;
                                        //Run.lblRT.Text = "00:00:00";
                                    }));
                                gluelog.log("Glue数据:" + Glue.GlueUseMins.ToString() + "_" + gdt2.ToString("HH:mm:ss") + "_" + FrmDisplay1.replaceTime.ToString("HH:mm:ss")
                                                + "_IsUntie" + Glue.IsGlueUntie.ToString());
                                Glue.GlueUseMins = 0;
                                iniFile.Write("GlueBarcodePara", "GlueCanUseMins", Glue.GlueUseMins.ToString(), path);
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        if (!File.Exists(Sys.AlarmPath + "\\" + gluelogfile))
                            gluelog = new Log(Sys.AlarmPath + "\\" + gluelogfile);
                        gluelog.log("Glue报错:" + er.ToString());
                    }
                    Thread.Sleep(995);
                    #endregion
                }
                if (Sys.TimerOtherGlue) //非过账胶水倒计时
                {
                    #region
                    try
                    {
                        int tsecond = Glue.glueTime * 60;
                        DateTime gdt2 = DateTime.Now;
                        TimeSpan gts = gdt2 - FrmDisplay1.replaceTime;
                        if (gts.TotalSeconds <= tsecond)
                        {
                            double releseT = (double)tsecond - gts.TotalSeconds;
                            int left1 = (int)(releseT);// % 86400
                            GlueHours = left1 / 3600;
                            left1 = left1 % 3600;
                            GlueMinutes = left1 / 60;
                            GlueSeconds = left1 % 60;
                        }
                        else
                        {
                            Protocol.strPCRead_GlueTimeOut = true;  //满盘信号时判断胶水时间超时
                            Protocol.IsPCRead = true;
                            Sys.TimerOtherGlue = false;
                            GlueHours = 0; GlueMinutes = 0; GlueSeconds = 0;
                            this.Invoke(new MethodInvoker(delegate
                            {
                                Run.lblGOutShow.Show();
                                //Run.lblGRT.Text = "00:00:00";
                            }));
                            gluelog.log("Glue数据:" + gdt2.ToString("HH:mm:ss") + "_" + FrmDisplay1.replaceTime.ToString("HH:mm:ss"));
                        }
                    }
                    catch (Exception er)
                    {
                        if (!File.Exists(Sys.AlarmPath + "\\" + gluelogfile))
                            gluelog = new Log(Sys.AlarmPath + "\\" + gluelogfile);
                        gluelog.log("Glue报错:" + er.ToString());
                    }
                    Thread.Sleep(995);
                    #endregion
                }
                Thread.Sleep(5);
            }
        }
        private void bWOEE_DoWork(object sender, DoWorkEventArgs e)  //OEE界面倒计时
        {
            while (true)
            {
                if (Sys.TimerOEE)
                {
                    oeeTimeRunning0 += 1;
                    int oeeMinutes = (int)(oeeTimeRunning0 / 60);
                    oeeTimeRunning = oeeTimeRunning0;
                    if (oeeMinutes >= 1)
                    {
                        oeeTimeRunning0 = 0;
                        Sys.TimerOEE = false;
                        Fct = new FrmChoseTable();
                        Fct.Left = 0;// Screen.PrimaryScreen.Bounds.Width;
                        Fct.Top = 0;
                        Fct.StartPosition = FormStartPosition.Manual;
                        Fct.Size = new System.Drawing.Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
                        DialogResult result = Fct.ShowDialog();
                    }
                    Thread.Sleep(995);
                }
                Thread.Sleep(5);
            }
        }
        private void bWGlueCU_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (GlueCU.IsConnected)
                {
                    switch (GlueCU.choice)
                    {
                        case 0:
                            try
                            {
                                #region 读取
                                //读取时间参数
                                GlueCU.Com.Write(GlueCU.send_Time_buff, 0, GlueCU.send_Time_buff.Length);
                                Thread.Sleep(50);
                                string str_time = null;
                                MethodGlueCU.str_buff(10, out str_time);
                                string[] btData = str_time.Split(',');
                                for (int i = 0; i < 10; i++)
                                {
                                    GlueCU.arrayTime[i] = double.Parse((double.Parse(btData[i]) / 1000).ToString("0.000"));
                                }
                                //读取压力参数
                                GlueCU.Com.Write(GlueCU.send_Pressure_buff1, 0, GlueCU.send_Pressure_buff1.Length);
                                Thread.Sleep(100);
                                MethodGlueCU.str_buff(20, out str_time);
                                btData = str_time.Split(',');
                                for (int i = 0; i < 20; i++)
                                {
                                    GlueCU.arrayPressure[i] = double.Parse((double.Parse(btData[i]) / 10).ToString("0.0"));
                                }
                                //读取通道数
                                GlueCU.Com.Write(GlueCU.chNum, 0, GlueCU.chNum.Length);
                                Thread.Sleep(50);
                                MethodGlueCU.str_buff(1, out str_time);
                                GlueCU.ChanleNumber = int.Parse(str_time);
                                #endregion
                                #region 写入
                                if (GlueCU.IsSendPara)
                                {
                                    GlueCU.IsSendPara = false;

                                    int n = GlueCU.sendChanleNumber;
                                    byte[] send_buff1 = MethodGlueCU.Write_plc(GlueCU.TimeAddress, n, GlueCU.sendTime);
                                    GlueCU.Com.Write(send_buff1, 0, send_buff1.Length);
                                    Thread.Sleep(50);
                                    int count = GlueCU.Com.BytesToRead;//获取接收字节数
                                    byte[] rcvBuffer = new byte[1024];//设置数组长度
                                    if (count > 0)
                                    {
                                        GlueCU.Com.Read(rcvBuffer, 0, count);//读取数据到数组
                                        if (rcvBuffer[5] == 0x00)  //成功的标志
                                            GlueCU.IsOk1 = true;
                                    }

                                    byte[] send_buff2 = MethodGlueCU.Write_plc(GlueCU.PressureAddress1, n, GlueCU.sendPressure1);
                                    GlueCU.Com.Write(send_buff2, 0, send_buff2.Length);
                                    Thread.Sleep(50);
                                    count = GlueCU.Com.BytesToRead;//获取接收字节数
                                    rcvBuffer = new byte[1024];//设置数组长度
                                    if (count > 0)
                                    {
                                        GlueCU.Com.Read(rcvBuffer, 0, count);//读取数据到数组
                                        if (rcvBuffer[5] == 0x00)
                                            GlueCU.IsOk2 = true;
                                    }

                                    byte[] send_buff3 = MethodGlueCU.Write_plc(GlueCU.PressureAddress2, n, GlueCU.sendPressure2);
                                    GlueCU.Com.Write(send_buff3, 0, send_buff3.Length);
                                    Thread.Sleep(50);
                                    count = GlueCU.Com.BytesToRead;//获取接收字节数
                                    rcvBuffer = new byte[1024];//设置数组长度
                                    if (count > 0)
                                    {
                                        GlueCU.Com.Read(rcvBuffer, 0, count);//读取数据到数组
                                        if (rcvBuffer[5] == 0x00)
                                            GlueCU.IsOk3 = true;
                                    }

                                    send_buff1 = MethodGlueCU.Write_plc(GlueCU.ChNumAddress, 1, GlueCU.sendChanleNumber.ToString());
                                    GlueCU.Com.Write(send_buff1, 0, send_buff1.Length);
                                    Thread.Sleep(50);
                                    count = GlueCU.Com.BytesToRead;//获取接收字节数
                                    rcvBuffer = new byte[1024];//设置数组长度
                                    if (count > 0)
                                    {
                                        GlueCU.Com.Read(rcvBuffer, 0, count);//读取数据到数组
                                        if (rcvBuffer[5] == 0x00)  //成功的标志
                                            GlueCU.IsOk = true;
                                    }

                                    if (GlueCU.IsOk & GlueCU.IsOk1 & GlueCU.IsOk2 & GlueCU.IsOk3)
                                        MessageBox.Show("设置参数成功");
                                    else
                                        MessageBox.Show("设置参数失败");
                                    GlueCU.IsOk = false;
                                    GlueCU.IsOk1 = false;
                                    GlueCU.IsOk2 = false;
                                    GlueCU.IsOk3 = false;
                                }
                                #endregion
                            }
                            catch
                            {
                                GlueCU.IsConnected = false;
                            }
                            break;
                        case 1:
                            try
                            {
                                #region 读取
                                GlueCU.Com.DiscardInBuffer();
                                GlueCU.Com.Write(MethodGlueCU.GetAll_ch("1"));
                                Thread.Sleep(50);
                                GlueCU.Com.Write(MethodGlueCU.GetAll_ch("2"));
                                Thread.Sleep(200);
                                //str1 = "- 02A02D- 1AD0P2202T1120OD00000OF00000E4-- 02A02D- 1AD0P1159T0610OD00000OF00000D7"
                                string str1 = GlueCU.Com.ReadExisting();
                                if (str1.Length == 82)
                                {
                                    GlueCU.arrayPressure[0] = double.Parse(str1.Substring(15, 3) + "." + str1.Substring(18, 1));
                                    GlueCU.arrayTime[0] = double.Parse(str1.Substring(20, 1) + "." + str1.Substring(21, 3));
                                    GlueCU.arrayPressure[1] = double.Parse(str1.Substring(56, 3) + "." + str1.Substring(59, 1));
                                    GlueCU.arrayTime[1] = double.Parse(str1.Substring(61, 1) + "." + str1.Substring(62, 3));
                                }
                                GlueCU.Com.DiscardInBuffer();
                                #endregion
                                #region 写入
                                if (GlueCU.IsSendPara808)
                                {
                                    GlueCU.IsSendPara808 = false;
                                    bool read1 = false;
                                    bool read2 = false;
                                    GlueCU.Com.Write(MethodGlueCU.SetAll_ch("1", GlueCU.sendPressure808[0], GlueCU.sendTime808[0], "00000", "00000")); //延时全部设为0
                                    Thread.Sleep(200);
                                    str1 = GlueCU.Com.ReadExisting();
                                    Thread.Sleep(50);
                                    //str = "- 02A02D";
                                    if (str1.Substring(4, 2) == "A0")
                                        read1 = true;
                                    GlueCU.Com.Write(MethodGlueCU.SetAll_ch("2", GlueCU.sendPressure808[1], GlueCU.sendTime808[1], "00000", "00000")); //延时全部设为0
                                    Thread.Sleep(200);
                                    string str2 = GlueCU.Com.ReadExisting();
                                    Thread.Sleep(50);
                                    //str = "- 02A02D";
                                    if (str2.Substring(4, 2) == "A0")
                                        read2 = true;
                                    if (read1 && read2)
                                        MessageBox.Show("设置OK");
                                    else
                                        MessageBox.Show("设置失败");
                                    GlueCU.Com.DiscardInBuffer();
                                }
                                #endregion
                            }
                            catch
                            {
                                GlueCU.IsConnected = false;
                            }
                            break;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void timerAlatm_Tick(object sender, EventArgs e)
        {
             try
            {
                if (PLC.IsConnected)
                { 
                    PLC.PlcStatus = HexString2BinString(PLC.PlcStatusP);
                    Mchstatus[0] = ((PLC.PlcStatus.Substring(15, 1) == "1") ? true : false);  //Down   0001         1
                    Mchstatus[1] = ((PLC.PlcStatus.Substring(14, 1) == "1") ? true : false);  //Run    0010         2
                    Mchstatus[2] = ((PLC.PlcStatus.Substring(13, 1) == "1") ? true : false);  //Idel   0100         4
                    Mchstatus[3] = ((PLC.PlcStatus.Substring(12, 1) == "1") ? true : false);  //PAUSE  1000         8
                    Int32 tempVarm = Convert.ToInt32(PLC.PlcStatusP, 16);
                    PLC.MachineStatus = tempVarm.ToString();//當前結果
                    if (PLC.MachineStatus != PLC.MachineStatusL & PLC.MachineStatusL != "")
                    {
                        string Ls = iniFile.Read("OEE", "LastStatus", SysPath);
                        string AlarmTime = DateTime.Now.ToString("yyyy-MM-dd*HH:mm:ss");
                        if (Mchstatus[2])//Idel
                        {
                            Process.Start(@"C:\EquipmentState\EquipmentState.exe", Sys.MachineId + ",Idle" + "," + AlarmTime);
                            MSChLogger("Idle");
                            Sys.TimerOEE = true;
                            if (Glue.IsChecked)
                            {
                                Protocol.strPCRead_PCReady = false;  //PC NG
                                Protocol.IsPCRead = true;
                            }
                        }
                        else if (Mchstatus[0])   //DOWN
                        {
                            string AlarmMessageA = "", AlarmMessageACh = "";
                            //導出錯誤代碼
                            MSLogger(Protocol.ErrMes, out AlarmMessageA, out AlarmMessageACh);

                            if (AlarmMessageA != "")//如果報警信息為空,則不報警
                            {
                                Process.Start(@"C:\EquipmentState\EquipmentState.exe", Sys.MachineId + ",Down,Message=" + AlarmMessageA + "," + AlarmTime);
                                MSChLogger(AlarmMessageACh);
                                Sys.TimerOEE = true;
                                if (Glue.IsChecked)
                                {
                                    Protocol.strPCRead_PCReady = false;  //PC NG
                                    Protocol.IsPCRead = true;
                                }
                            }
                        }
                        else if (Mchstatus[3])   // PAUSE 人為暫停=IDle（資訊報表不認PAUSE）
                        {
                            Sys.TimerOEE = true;
                            Process.Start(@"C:\EquipmentState\EquipmentState.exe", Sys.MachineId + ",Idle" + "," + AlarmTime);
                            MSChLogger("Idle");
                        }
                        else if (Ls != "Run" & Mchstatus[1])   //RUN
                        {
                            Sys.TimerOEE = false;
                            oeeTimeRunning0 = 0;
                            Process.Start(@"C:\EquipmentState\EquipmentState.exe", Sys.MachineId + ",Run" + "," + AlarmTime);
                            MSChLogger("Run");
                        }
                        
                        else if ((PLC.MachineStatusL != "4" & PLC.MachineStatus == "4") ||
                            (PLC.MachineStatusL != "1" & PLC.MachineStatus == "1"))
                        {
                            if (RiReader.BPFWebChecked)
                            {
                                this.Invoke(new MethodInvoker(delegate
                                {
                                    gBWarm.BringToFront();
                                }));
                            }
                        }
                    }
                    Isrun = Mchstatus[1];
                    IsIdle = Mchstatus[2];
                    Mchstatus3 = Mchstatus[3];
                    PLC.MachineStatusL = PLC.MachineStatus;
                }
            }
            catch
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fop = new OpenFileDialog();

            fop.Title = "请选择文件";
            fop.Filter = "所有文件(*.*)|*.*";
            if (fop.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string names = fop.FileName;
                    HWindow Window = Run2.hWGCCD1.HalconWindow;
                    halcon.Image[6] = new HObject();
                    halcon.Image[6].Dispose();
                    HOperatorSet.ReadImage(out halcon.Image[6], names);
                    HTuple hv_Height = new HTuple(), hv_Width = new HTuple();
                    HOperatorSet.GetImageSize(halcon.Image[6], out hv_Width, out hv_Height);
                    HOperatorSet.SetPart(Window, 0, 0, hv_Height, hv_Width);
                    halcon.Image[6].DispObj(Window);
                }
                catch
                {
                    MessageBox.Show("請先選擇相機");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HWindow Window = Run2.hWGCCD1.HalconWindow;
            HObject ho_ResultImage = new HObject();
            HD.ImageProcess_NeedleTipTest(Window, halcon.Image[6], out ho_ResultImage);
        }
    }

    public partial class Log
    {
        private string logFile;
        private StreamWriter writer;
        private FileStream fileStream = null;

        public Log(string fileName)
        {
            logFile = fileName;
            CreateDirectory(logFile);
        }

        public void log(string info)
        {

            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile);
                if (!fileInfo.Exists)
                {
                    fileStream = fileInfo.Create();
                    writer = new StreamWriter(fileStream);
                }
                else
                {
                    fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write);
                    writer = new StreamWriter(fileStream);
                }
                writer.WriteLine(DateTime.Now + ": " + info);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        public void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }

}
