using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMTVision
{
    public static class VarLighter
    {
        //获取设定亮度cmd
        public static byte[] SetBrit(int ch, int brit)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0x40);//标识符
            cmd.Add(0x05);//Len
            cmd.Add(0x01);//设备型号
            cmd.Add(0x00);//设备ID
            cmd.Add(0x1A);//设定亮度命令码
            cmd.Add((byte)ch);//通道
            cmd.Add((byte)brit);//亮度
            cmd.Add(SumCheck(cmd.ToArray()));
            return cmd.ToArray();

        }
        //获取打开or关闭通道cmd
        public static byte[] SetOnOff(int ch, bool on)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0x40);//标识符
            cmd.Add(0x05);//LEN
            cmd.Add(0x01);//设备型号
            cmd.Add(0x00);//设备ID
            cmd.Add(0x2A);//命令码
            cmd.Add((byte)ch);//通道
            cmd.Add(on ? (byte)1 : (byte)0);
            cmd.Add(SumCheck(cmd.ToArray()));
            return cmd.ToArray();
        }
        //获取读所有参数cmd
        public static byte[] ReadAllPara()
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0x40);//标识符
            cmd.Add(0x04);//LEN
            cmd.Add(0x01);//设备型号
            cmd.Add(0x00);//设备ID
            cmd.Add(0x31);//命令码
            cmd.Add(0xFF);//通道
            cmd.Add(SumCheck(cmd.ToArray()));
            return cmd.ToArray();
        }
        //SumCheck
        static byte SumCheck(byte[] cmd)
        {
            int sum = 0;
            foreach (byte c in cmd)
            {
                sum += c;
            }
            string hex = sum.ToString("X");
            if (hex.Length < 2)
            {
                hex = hex.PadLeft(2, '0');
            }
            return (byte)(Convert.ToInt32(hex, 16));
        }
    }
}
