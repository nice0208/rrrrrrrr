using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LMTVision
{
    public static class iniFile
    {
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filepath);
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        public static string Read(string section, string key, string file)
        {
            StringBuilder sb = new StringBuilder(4096);
            GetPrivateProfileString(section, key, "", sb, 4096, file);
            return sb.ToString();
        }
        public static void Write(string section, string key, string value, string file)
        {
            WritePrivateProfileString(section, key, value, file);
        }
        public static long Delete(string section, string key, string value, string file)
        {
            return WritePrivateProfileString("", "", "", file);
        }
    }
}
