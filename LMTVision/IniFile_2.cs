using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LMTVision
{
    public static class IniFile
    {
        [DllImport("kernel32")]
        private static extern Int32 GetPrivateProfileString(string section, string key, string defval,
                           StringBuilder ret, Int32 len, string file);
        [DllImport("kernel32")]
        private static extern Int32 WritePrivateProfileString(string section, string key, string val,
                           string file);
        public static string Read(string Section, string Key, string DefVal, string File)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, DefVal, sb, 255, File);
            return sb.ToString();
        }
        public static void Write(string Section, string Key, string Val, string File)
        {
            WritePrivateProfileString(Section, Key, Val, File);
        }
    }
}
