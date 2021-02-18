using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using MvCamCtrl.NET;
using System.Net;
using HalconDotNet;
using System.Windows.Forms;

namespace LMTVision
{
    struct _MV_MATCH_INFO_NET_DETECT_
    {
        public UInt64 nReviceDataSize;    // 已接收数据大小  [统计StartGrabbing和StopGrabbing之间的数据量]
        public UInt32 nLostPacketCount;   // 丢失的包数量
        public uint nLostFrameCount;    // 丢帧数量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] nReserved;          // 保留
    };

    public class MyHikvision
    {

        public MyCamera camera;
        public UInt32 m_nBufSizeForSaveImage;
        public byte[] m_pBufForSaveImage;         // 用于保存图像的缓存
        public bool IsConnected;
        /// <summary>
        /// 相機IP
        /// </summary>
        public IPAddress ip = IPAddress.Parse("192.0.0.0");
        /// <summary>
        /// 相機初始模式 0=軟體觸發 1=IO觸發
        /// </summary>
        public int TriggerMode = 0;
        //相機增益,曝光參數
        public double GainMaximum = new double();
        public double GainMinimum = new double();
        public double Gain = new double();
        public double ExposureTimeMaximum = new double();
        public double ExposureTimeMinimum = new double();
        public double ExposureTime = new double();
        public HObject image = new HObject();
        MyCamera.cbOutputdelegate cbImage;
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        private MyCamera.MV_CC_DEVICE_INFO deviceInfo;//设备对象

        bool m_bGrabbing;
        int m_nCanOpenDeviceNum;        // ch:设备使用数量 | en:Used Device Number
        int m_nDevNum;        // ch:在线设备数量 | en:Online Device Number
        int m_nFrames;      // ch:帧数 | en:Frame Number
        bool m_bTimerFlag;     // ch:定时器开始计时标志位 | en:Timer Start Timing Flag Bit
        bool m_bSaveImg;    // ch:保存图片标志位 | en:Save Image Flag Bit
        IntPtr m_hDisplayHandle;
        //自添加變數
        public int MyCameraNum = 5;
        public int NowCamera = 0;

        /// <summary>
        /// 图像处理自定义委托
        /// </summary>
        /// <param name="hImage">halcon图像变量</param>
        public delegate void delegateProcessHImage(HObject hImage);
        /// <summary>
        /// 图像处理委托事件
        /// </summary>
        public event delegateProcessHImage eventProcessImage;

        public void InitializeSetting()
        {
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_bGrabbing = false;
            m_nCanOpenDeviceNum = 0;
            m_nDevNum = 0;
            DeviceListAcq();
            m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
            m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];


            m_nFrames = new int();
            cbImage = new MyCamera.cbOutputdelegate(ImageCallBack1);
            m_bTimerFlag = false;
            m_bSaveImg = new bool();
            m_hDisplayHandle = new IntPtr();

        }

        // ch:枚举设备 | en:Create Device List
        private void DeviceListAcq()
        {
            int nRet;

            System.GC.Collect();
            nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if (0 != nRet)
            {
                ShowErrorMsg("Enumerate devices fail!", 0);
                return;
            }

            m_nDevNum = (int)m_pDeviceList.nDeviceNum;
            //tbDevNum.Text = m_nDevNum.ToString("d");
        }

        // ch:取流回调函数 | en:Aquisition Callback Function
        private void ImageCallBack1(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)
        {
            int nIndex = (int)pUser;

            // ch:抓取的帧数 | en:Aquired Frame Number
            ++m_nFrames;
            //讀取圖片

            image = ReadImage(pData, pFrameInfo, nIndex);
            eventProcessImage(image);
            image.Dispose();
        }

        // 讀取图片 | en:Readimage
        private HImage ReadImage(IntPtr pData, MyCamera.MV_FRAME_OUT_INFO stFrameInfo, int nIndex)
        {
            if ((3 * stFrameInfo.nFrameLen + 2048) > m_nBufSizeForSaveImage)
            {
                m_nBufSizeForSaveImage = 3 * stFrameInfo.nFrameLen + 2048;
                m_pBufForSaveImage = new byte[m_nBufSizeForSaveImage];
            }

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
            stSaveParam.enPixelType = stFrameInfo.enPixelType;
            stSaveParam.pData = pData;
            stSaveParam.nDataLen = stFrameInfo.nFrameLen;
            stSaveParam.nHeight = stFrameInfo.nHeight;
            stSaveParam.nWidth = stFrameInfo.nWidth;
            stSaveParam.pImageBuffer = pImage;
            stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
            stSaveParam.nJpgQuality = 80;

            HImage image = new HImage();
            if (IsMonoData(stFrameInfo.enPixelType))//判断是否为黑白图像
            {
                //如果是黑白图像，则利用Halcon图像库中的GenImage1算子来构建图像
                image.GenImage1("byte", (int)stFrameInfo.nWidth, (int)stFrameInfo.nHeight, pData);
            }
            else
            {
                if (stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed)
                {
                    //如果彩色图像是RGB8格式，则可以直接利用GenImageInterleaved算子来构建图像
                    image.GenImageInterleaved(pData, "rgb", (int)stFrameInfo.nWidth, (int)stFrameInfo.nHeight, 0, "byte", (int)stFrameInfo.nWidth, (int)stFrameInfo.nHeight, 0, 0, -1, 0);
                }
                else
                {
                    //如果彩色图像不是RGB8格式，则需要将图像格式转换为RGB8。
                    IntPtr pBufForSaveImage = IntPtr.Zero;
                    if (pBufForSaveImage == IntPtr.Zero)
                    {
                        pBufForSaveImage = Marshal.AllocHGlobal((int)(stFrameInfo.nWidth * stFrameInfo.nHeight * 3 + 2048));
                    }
                    MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
                    stConverPixelParam.nWidth = stFrameInfo.nWidth;
                    stConverPixelParam.nHeight = stFrameInfo.nHeight;
                    stConverPixelParam.pSrcData = pData;
                    stConverPixelParam.nSrcDataLen = stFrameInfo.nFrameLen;
                    stConverPixelParam.enSrcPixelType = stFrameInfo.enPixelType;
                    stConverPixelParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;//在此处选择需要转换的目标类型
                    stConverPixelParam.pDstBuffer = pBufForSaveImage;
                    stConverPixelParam.nDstBufferSize = (uint)(stFrameInfo.nWidth * stFrameInfo.nHeight * 3 + 2048);
                    camera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                    image.GenImageInterleaved(pBufForSaveImage, "rgb", (int)stFrameInfo.nWidth, (int)stFrameInfo.nHeight, 0, "byte", (int)stFrameInfo.nWidth, (int)stFrameInfo.nHeight, 0, 0, -1, 0);
                    //释放指针
                    Marshal.FreeHGlobal(pBufForSaveImage);
                }
            }
            return image;
        }

        //判断是否为黑白图像
        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;
                default:
                    return false;
            }
        }
        // ch:显示错误信息 | en:Show error message
        private void ShowErrorMsg(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " No data "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case MyCamera.MV_E_NETER: errorMsg += " Network error "; break;
            }
            MessageBox.Show(errorMsg, "PROMPT");
        }

        public void OpenCamera()
        {
            string m_SerialNumber = "";//接收设备返回的序列号
            int nRet = -1;

            for (int j = 0; j < m_nDevNum; ++j)
            {
                deviceInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[j], typeof(MyCamera.MV_CC_DEVICE_INFO));//获取设备信息
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(deviceInfo.SpecialInfo.stGigEInfo, 0);
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                /*****************************************************************/
                m_SerialNumber = IntToIp(gigeInfo.nCurrentIp, true); //与相机连接的电脑IP
                if (m_SerialNumber == ip.ToString())
                {

                    //ch:获取选择的设备信息 | en:Get Selected Device Information
                    MyCamera.MV_CC_DEVICE_INFO device =
                        (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[j],
                                                                  typeof(MyCamera.MV_CC_DEVICE_INFO));

                    //ch:打开设备 | en:Open Device
                    if (null == camera)
                    {
                        camera = new MyCamera();
                        if (null == camera)
                        {
                            IsConnected = false;
                            QCCD.IsConnected = false;
                            return;
                        }
                    }

                    nRet = camera.MV_CC_CreateDevice_NET(ref device);
                    if (MyCamera.MV_OK != nRet)
                    {
                        IsConnected = false;
                        QCCD.IsConnected = false;
                        return;
                    }

                    nRet = camera.MV_CC_OpenDevice_NET();
                    if (MyCamera.MV_OK != nRet)
                    {

                    }
                    else
                    {
                        m_nCanOpenDeviceNum++;

                        // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                        if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                        {
                            int nPacketSize = camera.MV_CC_GetOptimalPacketSize_NET();
                            if (nPacketSize > 0)
                            {
                                nRet = camera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                                if (nRet != MyCamera.MV_OK)
                                {
                                    Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                            }
                        }
                        //心跳时间最小为500ms
                        uint tempTime = 1000;
                        int temp = camera.MV_CC_SetIntValue_NET("GevHeartbeatTimeout", tempTime);

                        //設置圖片回調
                        camera.MV_CC_RegisterImageCallBack_NET(cbImage, (IntPtr)0);

                        // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                        camera.MV_CC_SetEnumValue_NET("AcquisitionMode", 2);// ch:工作在连续模式 | en:Acquisition On Continuous Mode
                        if (TriggerMode == 0)
                            SetTriggerMode_Software();   // ch:單次模式 | en:Continuous
                        else if (TriggerMode == 1)
                            SetTriggerMode_Line1();
                        //獲取增益,曝光參數
                        GetParam();
                        //bnGetParam_Click(null, null);// ch:获取参数 | en:Get parameters
                        //設置增益曝光參數
                        SetGain(Gain);
                        SetExposureTime(ExposureTime);
                        nRet = camera.MV_CC_StartGrabbing_NET();
                        if (MyCamera.MV_OK != nRet)
                        {
                            ShowErrorMsg("Trigger Fail!", nRet);
                            return;
                        }
                        IsConnected = true;
                        QCCD.IsConnected = true;
                        break;
                    }
                }
                else
                {
                    IsConnected = false;
                    QCCD.IsConnected = false;
                }
            }

        }

        private string IntToIp(long ipInt, bool host_or_camera)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipInt >> 24) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            if (host_or_camera)
                sb.Append(ipInt & 0xFF);
            else
            {
                if ((ipInt & 0xFF) < 254)
                    sb.Append((ipInt & 0xFF) + 1);
                else
                    sb.Append((ipInt & 0xFF) - 1);
            }
            return sb.ToString();
        }
        public void GetParam()
        {
            //曝光
            int nRet = GetExposureTime();

            if (nRet != 0)
            {
                MessageBox.Show("获取失败");
            }
            //增益
            nRet = GetGain();
            if (nRet != 0)
            {
                MessageBox.Show("获取失败");
            }
        }
        //获取曝光时间
        public int GetExposureTime()
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = camera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                //曝光
                ExposureTimeMaximum = (double)stParam.fMax;
                ExposureTimeMinimum = (double)stParam.fMin;
                if (ExposureTime == 0)
                    ExposureTime = (double)stParam.fCurValue;
                return 0;
            }
            ExposureTimeMaximum = 999500;
            ExposureTimeMinimum = 34;
            ExposureTime = ExposureTime == 0 ? 34 : ExposureTime;
            return -1;
        }
        //获取增益值
        public int GetGain()
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = camera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                //增益
                GainMaximum = (double)stParam.fMax;
                GainMinimum = (double)stParam.fMin;
                if (Gain == 0)
                    Gain = (double)stParam.fCurValue;
                return 0;
            }
            GainMaximum = 20;
            GainMinimum = 0;
            Gain = 0;
            return -1;
        }

        public int SetGain(double Gain)
        {
            int temp = camera.MV_CC_SetEnumValue_NET("GainAuto", 0);
            temp = camera.MV_CC_SetFloatValue_NET("Gain", (float)Gain);
            if (MyCamera.MV_OK != temp)
            {
                return -1;
            }
            return 0;
        }

        public int SetExposureTime(double ExposureTime)
        {
            int temp = camera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
            temp = camera.MV_CC_SetFloatValue_NET("ExposureTime", (float)ExposureTime);
            if (MyCamera.MV_OK != temp)
            {
                return -1;
            }
            return 0;
        }

        public void OneShot()
        {
            int nRet = 0;
            nRet = camera.MV_CC_SetEnumValue_NET("TriggerMode", 1);
            if (nRet != 0)
            {
                //IsConnected = false;
                return;
            }
            // ch:触发源选择:0 - Line0; | en:Trigger source select:0 - Line0;
            //           1 - Line1;
            //           2 - Line2;
            //           3 - Line3;
            //           4 - Counter;
            //           7 - Software;
            nRet = camera.MV_CC_SetEnumValue_NET("TriggerSource", 7);
            if (nRet != 0)
            {
                //IsConnected = false;
                return;
            }
            // ch:触发命令 | en:Trigger command

            nRet = camera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (nRet != 0)
            {
                //IsConnected = false;
                return;
            }
            //if (MyCamera.MV_OK != nRet)
            //{
            //    ShowErrorMsg("Trigger Fail!", nRet);
            //}
        }
        public void ContinuousShot()
        {
            int nRet = 0;
            nRet = camera.MV_CC_SetEnumValue_NET("TriggerMode", 0);
            if (MyCamera.MV_OK != nRet)
            {
                //IsConnected = false;
                return;
            }
            // ch:显示 | en:Display
            //camera.MV_CC_Display_NET(hWindowControl1.Handle);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    ShowErrorMsg("Trigger Fail!", nRet);
            //}
        }
        /// <summary>
        /// 停止連續採集
        /// </summary>
        public void Stop()
        {
            camera.MV_CC_SetEnumValue_NET("TriggerMode", 1);
            //int nRet;
            // ch:显示 | en:Display
            //nRet = camera.MV_CC_Display_NET(hWindowControl1.Handle);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    ShowErrorMsg("Trigger Fail!", nRet);
            //}
        }
        /// <summary>
        /// 關閉設備
        /// </summary>
        public void DestroyCamera()
        {
            //如果相機還連接,關閉前自動切換為軟體觸發模式
            if (IsConnected)
                SetTriggerMode_Software();
            // ch:关闭设备 | en:Close Device
            int nRet = -1;
            // ch:停止采集 | en:Stop Grabbing
            nRet = camera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("Stop Grabbing Fail!", nRet);
            }

            nRet = camera.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }

            nRet = camera.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }
        }
        /// <summary>
        /// 設置為IO觸發
        /// </summary>
        public void SetTriggerMode_Line1()
        {
            int nRet = 0;
            nRet = camera.MV_CC_SetEnumValue_NET("TriggerMode", 1);
            if (MyCamera.MV_OK != nRet)
            {
                //IsConnected = false;
                return;
            }
            nRet = 0;
            nRet = camera.MV_CC_SetEnumValue_NET("TriggerSource", 0);
            if (MyCamera.MV_OK != nRet)
            {
                //IsConnected = false;
                return;
            }
        }

        /// <summary>
        /// 設置為軟體觸發
        /// </summary>
        public void SetTriggerMode_Software()
        {
            int nRet = 0;
            nRet = camera.MV_CC_SetEnumValue_NET("TriggerMode", 1);
            if (MyCamera.MV_OK != nRet)
            {
                //IsConnected = false;
                return;
            }
            nRet = 0;
            nRet = camera.MV_CC_SetEnumValue_NET("TriggerSource", 7);
            if (MyCamera.MV_OK != nRet)
            {
                //IsConnected = false;
                return;
            }
        }
    }
}
