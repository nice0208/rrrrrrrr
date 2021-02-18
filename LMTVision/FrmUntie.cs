using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LMTVision
{
    public partial class FrmUntie : Form
    {
        public FrmUntie()
        {
            InitializeComponent();
        }
        private DateTime _dt;
        string path = Sys.IniPath + "\\SetParam.ini";
        string totalUsers = "";
        int k13 = 0;
        private void txtPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            DateTime tempDt = DateTime.Now;
            TimeSpan ts = tempDt.Subtract(_dt);
            if (ts.Milliseconds > 50)
            {
                txtPwd.Text = "";
                k13 = 0;
            }
            _dt = tempDt;
            if (e.KeyChar == 13)
                k13 = 1;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (k13 != 1)
            {
                MessageBox.Show("请使用扫描枪扫描录入（当使用扫描枪无法输入时，请在文本文件中扫描确认是否有回车符）！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
                k13 = 0;
            if (Sys.Factory == "XM")
            {
                totalUsers = iniFile.Read("CodeNumber", "XMUntieTotal", path);
                if (totalUsers == "")
                {
                    totalUsers = "Q-3494,Y-7998,U-4532,YA-0046,Y-3117,R-2275,Y-6824,YA-1112,W-0616,U-2650,X-1713,C-0188,YA-1284,O-3464,Y-5662,Y-8021,Y-5232,Y-3853,W-1873,U-2615,R-1157";
                    iniFile.Write("CodeNumber", "XMUntieTotal", totalUsers, "D:\\MB Mounter Report\\Coord.ini");
                }
            }
            if (Sys.Factory == "JM" || Sys.Factory == "TD")
            {
                totalUsers = iniFile.Read("CodeNumber", "JMUntieTotal", path);
                if (totalUsers == "")
                {
                    totalUsers = "Y-1619,R-1478,U-3483,YA-0711";
                    iniFile.Write("CodeNumber", "JMUntieTotal", totalUsers, "D:\\MB Mounter Report\\Coord.ini");
                }
            }
            if (totalUsers == "")
            {
                MessageBox.Show("未设置解绑可操作人员，请联系相关人员设置！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Glue.WebChecked & !FrmMain.GseoConn)
            {
                MessageBox.Show("与MES系统网络连接异常，请处理！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] Users = totalUsers.Split(',');
            int pwdnum = 0;
            for (int i = 0; i < Users.Length; ++i)
            {
                if (Users[i] == txtPwd.Text & txtPwd.Text != "")
                {
                    #region 解绑
                    lblUnitShow.BringToFront();
                    string sMessage = string.Empty;
                    if (Sys.Factory == "XM")
                    {
                        MBMatchLabel.MatchLabel MBWebCall = new MBMatchLabel.MatchLabel();
                        Glue.IsGlueCan = MBWebCall.bDownGlueEqpNo(Sys.MachineId, Glue.Barcode, out  sMessage);
                    }
                    if (Sys.Factory == "JM" || Sys.Factory == "TD")
                    {
                        JMBMatchLabel.MatchLabel MBWebCall = new JMBMatchLabel.MatchLabel();
                        Glue.IsGlueCan = MBWebCall.bDownGlueEqpNo(Sys.MachineId, Glue.Barcode, out  sMessage);
                    }
                    GlueLog(sMessage);
                    if (!Glue.IsGlueCan)
                    {
                        MessageBox.Show(sMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    #endregion
                    pwdnum++;
                    lblUnitShow.SendToBack();
                    Glue.IsGlueCan = false;
                    Glue.IsGlueUntie = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            if (pwdnum == 0)
            {
                DialogResult dr = MessageBox.Show("工号错误,请重新输入！", "",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button2);
            }
        }

        void GlueLog(string strMesssage)
        {
            string path = Sys.ReportLog + "\\GlueLog";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            //File
            string file = DateTime.Now.ToString("yyyyMMdd") + "_" + Sys.MachineId + "_GlueLog.txt";
            if (!File.Exists(path + "\\" + file))
            {
                string Header = "CreateTime\t" + "GlueBarcode\t" + "ReplacementTime\t" + "Status\t" + "IsOrNot\t" + "IsCanUseMins\t" + "ErrMessage\t" + "\r\n";
                File.WriteAllText(path + "\\" + file, Header);
            }
            using (StreamWriter sw = new StreamWriter(path + "\\" + file, true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMdd") + "\t" + Glue.Barcode + "\t" + DateTime.Now.ToString("HH:mm:ss") + "\t" +
                             "Down" + "\t" + Glue.IsGlueCan + "\t" + "" + "\t" + strMesssage);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            this.Close();
        }

        private void FrmUntie_Load(object sender, EventArgs e)
        {
            lblUnitShow.SendToBack();
        }

    }
}
