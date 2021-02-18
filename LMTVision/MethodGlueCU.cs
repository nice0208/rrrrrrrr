using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMTVision
{
    class MethodGlueCU
    {
        //----------------------------------------------------------//
        //GS-2.0
        //----------------------------------------------------------//
        /// <summary>
        /// 读取数据
        /// </summary>
        public static byte[] Read_plc(string addr, int number)
        {
            //（0开始 1） 2站号 （3长度 4） 5字读 6地址D （7     8   9)  (10个数 11)（12结束 13）（14校验码 15）  
            byte[] uSend = new byte[] { 0x10, 0x02, 0x00, 0x07, 0x00, 0x20, 0xA0, 0x30, 0x30, 0x30, 0x30, 0x30, 0x10, 0x03, 0x30, 0x30 };
            try
            {
                addr = "000000" + addr;
                addr = addr.Substring(addr.Length - 6);
                uSend[9] = Convert.ToByte(addr.Substring(0, 2), 16);
                uSend[8] = Convert.ToByte(addr.Substring(2, 2), 16);
                uSend[7] = Convert.ToByte(addr.Substring(4, 2), 16);
                //写入个数
                string _numberStr = number.ToString("X4");
                uSend[10] = Convert.ToByte(_numberStr.Substring(2, 2), 16);
                uSend[11] = Convert.ToByte(_numberStr.Substring(0, 2), 16);
            }
            catch { }

            UInt32 uSum = 0;
            for (int i = 2; i < uSend.Length - 4; i++)
                uSum = uSum + (byte)uSend[i];
            //求校验码
            UInt32 uTmp = uSum & 0x000f;
            uSend[uSend.Length - 1] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[uSend.Length - 2] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            return uSend;
        }

        /// <summary>
        /// 写入命令
        /// </summary>
        public static byte[] Write_plc(string addr, int number, string data)
        {
            byte[] uSend = new byte[number * 2 + 16];
            try
            {
                uSend[0] = 0x10; uSend[1] = 0x02; uSend[2] = 0x00;  //开始，站号
                uSend[5] = 0x28; uSend[6] = 0xA0; //字写功能
                uSend[number * 2 + 12] = 0x10; uSend[number * 2 + 13] = 0x03;
                //资讯个数
                int value = number * 2 + 7;
                string _valueStr = value.ToString("X4");
                uSend[3] = Convert.ToByte(_valueStr.Substring(2, 2), 16);
                uSend[4] = Convert.ToByte(_valueStr.Substring(0, 2), 16);
                //地址
                addr = "000000" + addr;
                addr = addr.Substring(addr.Length - 6);
                uSend[7] = Convert.ToByte(addr.Substring(4, 2), 16);
                uSend[8] = Convert.ToByte(addr.Substring(2, 2), 16);
                uSend[9] = Convert.ToByte(addr.Substring(0, 2), 16);
                //写入个数
                string _numberStr = number.ToString("X4");
                uSend[10] = Convert.ToByte(_numberStr.Substring(2, 2), 16);
                uSend[11] = Convert.ToByte(_numberStr.Substring(0, 2), 16);
                //数据
                string[] btData = data.Split(',');
                for (int i = 0; i < number; i++)
                {
                    int value_buff = Convert.ToInt32(btData[i]);
                    string str_buff = value_buff.ToString("X4");
                    uSend[12 + i * 2] = Convert.ToByte(str_buff.Substring(2, 2), 16);
                    uSend[13 + i * 2] = Convert.ToByte(str_buff.Substring(0, 2), 16);
                }
                //求校验码
                UInt32 uSum = 0;
                for (int i = 2; i < uSend.Length - 4; i++)
                    uSum = uSum + (byte)uSend[i];

                UInt32 uTmp = uSum & 0x000f;
                uSend[uSend.Length - 1] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
                uTmp = (uSum >> 4) & 0x000f;
                uSend[uSend.Length - 2] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            }
            catch { }

            return uSend;
        }

        /// <summary>
        /// 读取返回的数据
        /// </summary>
        public static void str_buff(int number, out String str)
        {
            str = "";
            int count = GlueCU.Com.BytesToRead;//获取接收字节数
            byte[] rcvBuffer = new byte[1024];//设置数组长度
            if (count > 0)
            {
                GlueCU.Com.Read(rcvBuffer, 0, count);//读取数据到数组
                if (rcvBuffer[5] == 0x00) //通讯成功标志
                {
                    for (int i = 0; i < number; i++)
                    {
                        int list_buff = rcvBuffer[7 + i * 2] * 0x100 + rcvBuffer[6 + i * 2];
                        str += ("," + list_buff.ToString());
                    }
                    str = str.Substring(1);
                }
                else
                    GlueCU.IsConnected = false;
            }
            else
                GlueCU.IsConnected = false;
        }

        //----------------------------------------------------------//
        //ML-808GX
        //----------------------------------------------------------//
        /// <summary>
        /// 通道所有参数的设置
        /// </summary>
        /// <param name="uAddress"></param>
        /// <returns></returns>
        public static string SetAll_ch(string set_ch, string set_pess, string set_time, string set_od, string set_of)
        {
            byte[] uSend = new byte[] {0x05,  0x02, 0x32, 0x31, 0x53, 0x43, 0x20, 0x20, 0x43, 0x48, 0x30, 0x30, 0x30,  0x50, 0x30, 0x30, 0x30, 0x30, 
                0x54, 0x30, 0x30, 0x30, 0x30,  0x4F, 0x44, 0x30, 0x30, 0x30, 0x30, 0x30,   0x4F, 0x46, 0x30, 0x30, 0x30, 0x30, 0x30,    0x30, 0x30, 0x03, 0x04 };
            try
            {
                //通道数
                set_ch.PadLeft(3, '0');
                byte[] ch_data = System.Text.Encoding.Default.GetBytes(set_ch);
                //byte[] ch_data = Encoding.ASCII.GetBytes(values.ToString("X3"));
                uSend[10] = ch_data[0];
                uSend[11] = ch_data[1];
                uSend[12] = ch_data[2];
                //输入压力值
                if (set_pess.Length == 4)
                    set_pess = "0" + set_pess.Substring(0, 2) + set_pess.Substring(3, 1);
                else
                    set_pess = set_pess.Substring(0, 3) + set_pess.Substring(4, 1);
                byte[] press_data = System.Text.Encoding.Default.GetBytes(set_pess);
                //byte[] press_data = Encoding.ASCII.GetBytes(press.ToString("X4"));
                uSend[14] = press_data[0];
                uSend[15] = press_data[1];
                uSend[16] = press_data[2];
                uSend[17] = press_data[3];
                //输入时间值
                if (set_time.Length == 3)
                    set_time = "0" + set_time;
                byte[] time_data = System.Text.Encoding.Default.GetBytes(set_time);
                //byte[] press_data = Encoding.ASCII.GetBytes(press.ToString("X4"));
                uSend[19] = time_data[0];
                uSend[20] = time_data[1];
                uSend[21] = time_data[2];
                uSend[22] = time_data[3];
                //输入接通延时
                byte[] od_data = System.Text.Encoding.Default.GetBytes(set_od);
                //byte[] press_data = Encoding.ASCII.GetBytes(press.ToString("X4"));
                uSend[25] = od_data[0];
                uSend[26] = od_data[1];
                uSend[27] = od_data[2];
                uSend[28] = od_data[3];
                uSend[29] = od_data[4];
                //输入断开延时   
                byte[] of_data = System.Text.Encoding.Default.GetBytes(set_od);
                //byte[] press_data = Encoding.ASCII.GetBytes(press.ToString("X4"));
                uSend[32] = of_data[0];
                uSend[33] = of_data[1];
                uSend[34] = of_data[2];
                uSend[35] = of_data[3];
                uSend[36] = of_data[4];
            }
            catch { }

            UInt32 uSum = 0;
            for (int i = 2; i < uSend.Length - 4; i++)
                uSum = uSum + (byte)uSend[i];
            uSum = (uSum > 0xFF) ? (~uSum + 1) : (~uSum);

            UInt32 uTmp = uSum & 0x000f;
            uSend[uSend.Length - 3] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[uSend.Length - 4] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            return Encoding.ASCII.GetString(uSend);
        }

        /// <summary>
        /// 获取指定通道的全部参数
        /// </summary>
        /// <param name="uAddress"></param>
        /// <returns></returns>
        public static string GetAll_ch(string _ch)
        {
            byte[] uSend = new byte[] { 0x05, 0x02, 0x30, 0x35, 0x47, 0x43, 0x30, 0x30, 0x30, 0x30, 0x30, 0x03, 0x06, 0x04 };
            try
            {
                //输入通道数
                _ch.PadLeft(3, '0');
                byte[] ch_data = System.Text.Encoding.Default.GetBytes(_ch);
                //byte[] ch_data = Encoding.ASCII.GetBytes(values.ToString("X3"));
                uSend[6] = ch_data[0];
                uSend[7] = ch_data[1];
                uSend[8] = ch_data[2];
            }
            catch { }

            UInt32 uSum = 0;
            for (int i = 2; i < uSend.Length - 5; i++)
                uSum = uSum + (byte)uSend[i];
            uSum = (uSum > 0xFF) ? (~uSum + 1) : (~uSum);

            UInt32 uTmp = uSum & 0x000f;
            uSend[uSend.Length - 4] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));
            uTmp = (uSum >> 4) & 0x000f;
            uSend[uSend.Length - 5] = (byte)((uTmp < 10) ? (uTmp + 0x30) : (uTmp + 0x41 - 0xa));

            return Encoding.ASCII.GetString(uSend);
        }

    }
}
