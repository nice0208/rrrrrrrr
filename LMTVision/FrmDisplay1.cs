using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace LMTVision
{
    public partial class FrmDisplay1 : Form
    {
        FrmMain parent;
        FrmUntie fu;
        Log gluelog;
        string path = Sys.IniPath + "\\ComPortPara.ini";   //设备各参数路径

        public FrmDisplay1(FrmMain parent)
        {
            this.parent = parent;
            this.MdiParent = parent;
            this.Dock = DockStyle.Top;
            InitializeComponent();
        }
        private void FrmDisplay1_Load(object sender, EventArgs e)
        {
            lbling.Hide();
            lblZip.Hide();
            lblUpShow.Hide();
            lblGOutShow.Hide();
            gluelog = new Log(Sys.AlarmPath + "\\" + FrmMain.gluelogfile);
            if (RiReader.IsChecked)
            {
                btnClearT1.Show();
                label2.Show(); txtTray1Barcode.Show();
                txtGlueBarcode.Text = Glue.Barcode;
            }
            else
            {
                btnClearT1.Hide();
                label2.Hide(); txtTray1Barcode.Hide();
            }
            if (Reader.IsChecked)
            {
                btnClearT2.Show();
                label3.Show(); txtTray2Barcode.Show();
            }
            else
            {
                btnClearT2.Hide();
                label3.Hide(); txtTray2Barcode.Hide();
            }
            if (Glue.IsChecked)
            {
                lblRT.Show(); btnUntie.Show();
                lblGRT.Show();
                labcode3.Show(); txtGluePiao.Show();
                labGlue.Show();  txtGlueBarcode.Show();
                btnConfirm.Show(); btnConfirm.Enabled = true;
            }
            else
            {
                lblRT.Hide(); btnUntie.Hide();
                lblGRT.Hide();
                labcode3.Hide(); txtGluePiao.Hide();
                labGlue.Hide();  txtGlueBarcode.Hide();
                btnConfirm.Hide(); btnConfirm.Enabled = false;
            }
            if (Glue.WebChecked)
            {
                btnUntie.Show();
                lblRT.Show();
                lblGRT.Hide();
                if (Glue.Barcode != "" & lblRT.Text != "00:00:00")
                    txtGluePiao.Enabled = false;
            }
            else
            {
                btnUntie.Hide();
                lblRT.Hide();
            }
            if (RiReader.Web_Tray_InOutStation)//膠水進出站顯示/不顯示
            {
                pnlWeb_Tray_InOutStation.Visible = true;
            }
            else
            {
                pnlWeb_Tray_InOutStation.Visible = false;
            }
            switch (Sys.Codes)
            {
                case "M": cbCode.SelectedIndex = 0; break;
                case "F": cbCode.SelectedIndex = 1; break;
            }
            //timerRefleshUI.Enabled = true;
        }

        private void txtGluePiao_KeyDown(object sender, KeyEventArgs e)
        {
            if (FrmMain.MacS == 1)
            {
                e.Handled = true;
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                    txtGluePiao.Focus();
            }
        }
        public static DateTime replaceTime, GBtimeN; private DateTime _dt;
        //public bool bLoadGlueEqpNo(string sEqpNo, string sGlueBarcode, out int iCanUseMins, out string sMsg);
        string StrErr = string.Empty;
        private void txtGluePiao_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FrmMain.MacS == 1)
            {
                e.Handled = true;
            }
            else
            {
                if (!Glue.IsKeyEnter)
                {
                    DateTime tempDt = DateTime.Now;
                    TimeSpan ts = tempDt.Subtract(_dt);
                    if (ts.Milliseconds > 50)
                        txtGluePiao.Text = "";
                    _dt = tempDt;
                    if (e.KeyChar == 13 & txtGluePiao.Text != "")
                    {
                        if (Glue.WebChecked & !FrmMain.GseoConn)
                        {
                            MessageBox.Show("与MES系统网络连接异常，请处理！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        #region saoma
                        if (txtGluePiao.Text.Substring(0, 1) != "A")
                        {
                            if (txtGluePiao.Text.Length >= 4)
                            {
                                if ((txtGluePiao.Text.Substring(0, 1) == "B" & txtGluePiao.Text.Substring(0, 4) == "B67-") ||
                                    (txtGluePiao.Text.Substring(0, 1) == "b" & txtGluePiao.Text.Substring(0, 4) == "b67-") ||
                                    (txtGluePiao.Text.Substring(0, 1) != "B" & txtGluePiao.Text.Substring(0, 1) != "b"))
                                {
                                    if ((Glue.WebChecked & lblRT.Text == "00:00:00") || txtGluePiao.Text != txtGlueBarcode.Text || txtGluePiao.Text != Glue.Barcode)
                                    {
                                        if (lblRT.Text == "00:00:00" & txtGluePiao.Text != txtGlueBarcode.Text & txtGlueBarcode.Text != "")
                                        {
                                            MessageBox.Show("请先将原胶水信息码解绑！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return;
                                        }
                                        txtGlueBarcode.Text = txtGluePiao.Text.ToUpper().Trim();
                                        Glue.Barcode = txtGlueBarcode.Text;
                                        if (txtGlueBarcode.Text != "")
                                        {
                                            if (Glue.WebChecked)
                                            {
                                                try
                                                {
                                                    CallWithTimeout(GSWebDisplay2, 25000);
                                                }
                                                catch
                                                {
                                                    sErrory = "胶水信息上传系统超时！";
                                                    lblUpShow.Hide();
                                                    txtGlueBarcode.Text = "";
                                                    Glue.Barcode = "";
                                                    MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                }
                                            }
                                            else
                                            {
                                                iniFile.Write("GlueBarcodePara", "GlueBarcode", txtGlueBarcode.Text, path);
                                                replaceTime = DateTime.Now;
                                                iniFile.Write("GlueBarcodePara", "GlueReplacementTime", replaceTime.ToString(), path);
                                            }
                                            GlueLog(StrErr);
                                            Glue.IsGlueCan = false;
                                            StrErr = "";
                                        }
                                        if (txtGlueBarcode.Text != "")
                                        {
                                            Protocol.strPCRead_GlueUpdate = true;   //胶水码更新ok
                                            Protocol.IsPCRead = true;
                                            Thread.Sleep(5);
                                            sendloop();
                                        }
                                    }
                                    else
                                    {
                                        GBtimeN = DateTime.Now;
                                        System.TimeSpan t = GBtimeN - replaceTime;
                                        if (t.TotalMinutes >= Glue.glueTime)
                                        {
                                            Protocol.strPCRead_GlueTimeOut = true;   //胶水码扫入时超时
                                            Protocol.IsPCRead = true;
                                            MessageBox.Show("胶水已超过规定使用时间，请更换胶水后点击确认！", "", MessageBoxButtons.OKCancel,
                                                            MessageBoxIcon.Warning);
                                        }
                                        if (t.TotalMinutes < 30)
                                        {
                                            Protocol.strPCRead_GlueUpdate = true;   //胶水码更新ok
                                            Protocol.IsPCRead = true;
                                            Thread.Sleep(5);
                                            sendloop();
                                        }
                                    }
                                    txtGluePiao.Text = "";
                                }
                                else
                                    MessageBox.Show("扫入非胶水码，请确认！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                                MessageBox.Show("胶水信息必须大于4位，请确认！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtGluePiao.Text = "";
                        }
                        else
                            MessageBox.Show("扫入非胶水码，请确认！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        #endregion
                    }
                }
                else
                {
                    if (e.KeyChar == 13)
                    {
                        if (Glue.WebChecked & !FrmMain.GseoConn)
                        {
                            MessageBox.Show("与MES系统网络连接异常，请处理！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        #region shoushu
                        if (txtGluePiao.Text != txtGlueBarcode.Text || txtGluePiao.Text != Glue.Barcode)
                        {
                            txtGlueBarcode.Text = txtGluePiao.Text.ToUpper().Trim();
                            Glue.Barcode = txtGlueBarcode.Text;
                            if (txtGlueBarcode.Text != "")
                            {
                                if (Glue.WebChecked)
                                {
                                    try
                                    {
                                        CallWithTimeout(GSWebDisplay2, 25000);
                                    }
                                    catch
                                    {
                                        sErrory = "胶水信息上传系统超时！";
                                        lblUpShow.Hide();
                                        txtGlueBarcode.Text = "";
                                        Glue.Barcode = "";
                                        MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else
                                {
                                    iniFile.Write("GlueBarcodePara", "GlueBarcode", txtGlueBarcode.Text, path);
                                    replaceTime = DateTime.Now;
                                    iniFile.Write("GlueBarcodePara", "GlueReplacementTime", replaceTime.ToString(), path);
                                }
                                GlueLog(StrErr);
                                Glue.IsGlueCan = false;
                                StrErr = "";
                            }
                            if (txtGlueBarcode.Text != "")
                            {
                                Protocol.strPCRead_GlueUpdate = true;   //胶水码更新ok
                                Protocol.IsPCRead = true;
                                Thread.Sleep(5);
                                sendloop();
                            }
                        }
                        else
                        {
                            GBtimeN = DateTime.Now;
                            System.TimeSpan t = GBtimeN - replaceTime;
                            if (t.TotalMinutes >= Glue.glueTime)
                            {
                                Protocol.strPCRead_GlueTimeOut = true;  //胶水码扫入时超时
                                Protocol.IsPCRead = true;
                                MessageBox.Show("胶水已超过规定使用时间，请更换胶水后点击确认！", "", MessageBoxButtons.OKCancel,
                                                MessageBoxIcon.Warning);
                            }
                            if (t.TotalMinutes < 30)
                            {
                                Protocol.strPCRead_GlueUpdate = true;   //胶水码更新ok
                                Protocol.IsPCRead = true;
                                Thread.Sleep(5);
                                sendloop();
                            }
                        }
                        txtGluePiao.Text = "";
                        #endregion
                    }
                }
            }
        }
        void sendloop()
        {
            int i_loop = 0;
            while (true)
            {
                i_loop++;
                Protocol.IsloopRead = true;
                Thread.Sleep(5);
                if (PLC.IsConnected & !PLC.loopstatus[10])
                {
                    if (i_loop > 3)
                    {
                        i_loop = 0;
                        MessageBox.Show("请重新扫描胶水二维码！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                    Protocol.strPCRead_GlueUpdate = true;  //胶水码更新ok
                    Protocol.IsPCRead = true;
                    Thread.Sleep(5);
                }
                else
                {
                    i_loop = 0;
                    break;
                }
            }
        }
        int glueWriteMins = 0;
        void GlueLog(string strMesssage)
        {
            try
            {
                string gpath = Sys.ReportLog + "\\GlueLog";
                if (!Directory.Exists(gpath))
                    Directory.CreateDirectory(gpath);
                //File
                string file = DateTime.Now.ToString("yyyyMMdd") + "_" + Sys.MachineId + "_GlueLog.txt";
                if (!File.Exists(gpath + "\\" + file))
                {
                    string Header = "CreateTime\t" + "GlueBarcode\t" + "ReplacementTime\t" + "Status\t" + "IsOrNot\t" + "IsCanUseMins\t" + "ErrMessage\t" + "\r\n";
                    File.WriteAllText(gpath + "\\" + file, Header);
                }
                using (StreamWriter sw = new StreamWriter(gpath + "\\" + file, true))
                {
                    if (Glue.IsGlueCan & Glue.WebChecked)
                    {
                        Sys.TimerGlue = true;
                        iniFile.Write("GlueBarcodePara", "GlueCanUseMins", Glue.GlueUseMins.ToString(), path);
                    }
                    else
                    {
                        Sys.TimerOtherGlue = true;
                    }
                    glueWriteMins = Glue.GlueUseMins;
                    if (Glue.WebChecked)
                        sw.WriteLine(DateTime.Now.ToString("yyyyMMdd") + "\t" + Glue.Barcode + "\t" + DateTime.Now.ToString("HH:mm:ss") + "\t" +
                                     "Load" + "\t" + Glue.IsGlueCan + "\t" + Glue.GlueUseMins + "\t" + strMesssage);
                    else
                        sw.WriteLine(DateTime.Now.ToString("yyyyMMdd") + "\t" + Glue.Barcode + "\t" + DateTime.Now.ToString("HH:mm:ss") + "\t" +
                                     "Load" );
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch
            {
            }
        }
        string dtResult = "Y"; string sErrory = string.Empty;
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            Monitor.IsRead = true;
            Thread.Sleep(100);
            #region Kakong
            if (PLC.IsConnected & RiReader.IsChecked & !Monitor.allstatus[14])
            {
                MessageBox.Show("PZ0盘读码未开启，请在人机界面上确认!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (PLC.IsConnected & Reader.IsChecked & !Monitor.allstatus[15])
            {
                MessageBox.Show("AZ0盘读码未开启，请在人机界面上确认!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (PLC.IsConnected & Monitor.UVModeCheck & !Monitor.allstatus[12])
            {
                MessageBox.Show("UV固化方式未选择，请在人机界面上确认!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Barcode1.IsChecked && !Barcode1.IsConnected)
            {
                MessageBox.Show("读码器1未连接，请确认后重启程式!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Glue.IsChecked && Glue.Barcode == "")
            {
                Protocol.strPCRead_GlueTimeOut = true;  //胶水信息为空
                Protocol.IsPCRead = true;
                MessageBox.Show("胶水信息为空，请确认是否需要胶水信息，如若需要请输入!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Glue.IsKeyEnter & !(Glue.Barcode.Substring(0, 4) == "B67-" & Glue.Barcode.Length == 41) || (Glue.WebChecked & lblRT.Text == "00:00:00"))
            {
                if (glueWriteMins != 0)
                {
                    iniFile.Write("GlueBarcodePara", "GlueCanUseMins", Glue.GlueUseMins.ToString(), path);
                    replaceTime = Convert.ToDateTime(iniFile.Read("GlueBarcodePara", "GlueReplacementTime", path));
                    Sys.TimerGlue = true;
                }
                Thread.Sleep(50);
                if (lblRT.Text == "00:00:00" & Glue.WebChecked)
                {
                    MessageBox.Show("胶水信息有误或已无可使用时间(00:00:00)，请重新确认!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (!File.Exists(Sys.AlarmPath + "\\" + FrmMain.gluelogfile))
                        gluelog = new Log(Sys.AlarmPath + "\\" + FrmMain.gluelogfile);
                    gluelog.log("Glue报错:TimeOut(ButtonCheck)");
                    return;
                }
            }
            if (Glue.IsChecked & !Glue.WebChecked & lblGRT.Text == "00:00:00")
            {
                MessageBox.Show("胶水已过期，请及时更新并输入信息!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            if (txtTray1Barcode.Text != "")
            {
                if (RiReader.WebChecked)
                {
                    lbling.Show();
                    try
                    {
                        CallWithTimeout(GSWebDisplay1, 25000);
                    }
                    catch
                    {
                        dtResult = "N";
                        sErrory = "访问系统超时！";
                        MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            if (dtResult == "Y")
            {
                #region Y
                lbling.Hide();
                if (Glue.IsChecked)
                {
                    GBtimeN = DateTime.Now;
                    System.TimeSpan t = GBtimeN - replaceTime;
                    if (t.TotalMinutes >= Glue.glueTime)
                    {
                        Protocol.strPCRead_GlueTimeOut = true;  //确认时胶水超时
                        Protocol.IsPCRead = true;
                        MessageBox.Show("胶水已超过规定使用时间，请更换胶水后点击确认！", "", MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning);
                        if (!File.Exists(Sys.AlarmPath + "\\" + FrmMain.gluelogfile))
                            gluelog = new Log(Sys.AlarmPath + "\\" + FrmMain.gluelogfile);
                        gluelog.log("Glue报错:TimeOut(ButtonRead)");
                    }
                    else
                    {
                        Protocol.strPCRead_PCReady = true; //胶水ok
                        Protocol.IsPCRead = true;
                        new MessageForm(5000, "准备完成，可以启动！").Show();
                    }
                }
                else
                {
                    Protocol.strPCRead_PCReady = true;  //胶水ok
                    Protocol.IsPCRead = true;
                    new MessageForm(5000, "准备完成，可以启动！").Show();
                }
                #endregion
            }
            else
            {
                lbling.Hide();
                MessageBox.Show(sErrory, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
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
        void GSWebDisplay1()
        {
            #region WebService
            if (FrmMain.GseoConn)
            {
                try
                {
                    string produceName = "";
                    if (Sys.CurrentProduction == "8982LH")
                        produceName = (Sys.CurrentProID != "" ? "8982AA" : Sys.CurrentProID);
                    else
                        produceName = Sys.CurrentProduction;
                    if (Sys.Factory == "XM" )
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
                dtResult = "";
                sErrory = "与MES系统网络连接异常，请处理！";
            }
            #endregion
        }
        void GSWebDisplay2()
        {
            #region 判断胶水信息+可用时间
            lblUpShow.Text = "胶水信息上传中，请勿关闭程式！";
            lblUpShow.Show();
            string sMessage = string.Empty;
            if (Sys.Factory == "XM")
            {
                MBMatchLabel.MatchLabel MBWebCall = new MBMatchLabel.MatchLabel();
                Glue.IsGlueCan = MBWebCall.bLoadGlueEqpNo(Sys.MachineId, Glue.Barcode, out Glue.GlueUseMins, out  sMessage);
            }
            if (Sys.Factory == "JM" || Sys.Factory == "TD")
            {
                JMBMatchLabel.MatchLabel MBWebCall = new JMBMatchLabel.MatchLabel();
                Glue.IsGlueCan = MBWebCall.bLoadGlueEqpNo(Sys.MachineId, Glue.Barcode, out Glue.GlueUseMins, out  sMessage);
            }
            if (Glue.IsGlueCan)
            {
                lblUpShow.Hide();
                iniFile.Write("GlueBarcodePara", "GlueBarcode", txtGlueBarcode.Text, path);
                replaceTime = DateTime.Now;
                iniFile.Write("GlueBarcodePara", "GlueReplacementTime", replaceTime.ToString(), path);
                iniFile.Write("GlueBarcodePara", "GlueCanUseMins", Glue.GlueUseMins.ToString(), path);
                //开始计时
                Sys.TimerGlue = true;
                txtGluePiao.Enabled = false;
            }
            else
            {
                lblUpShow.Hide();
                MessageBox.Show(sMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGlueBarcode.Text = "";
                Glue.Barcode = "";
                return;
            }
            StrErr = sMessage;
            #endregion
        }

        private void timerRefleshUI_Tick(object sender, EventArgs e)
        {
            if (txtTray1Barcode.Text != RiReader.Barcode)
                txtTray1Barcode.Text = RiReader.Barcode;
            if (txtTray2Barcode.Text != Reader.Barcode)
                txtTray2Barcode.Text = Reader.Barcode;
            lblA11Num.Text = (FrmMain.processing[0] ? "1" : "0");
            lblA12Num.Text = A1CCD2.IntSingle.ToString();
            lblA21Num.Text = (FrmMain.processing[2] ? "1" : "0");
            lblA22Num.Text = A2CCD2.IntSingle.ToString();
            
            if (Sys.NoAutoMatic)
                Thread.Sleep(5);
        }

        private void btnClearT1_Click(object sender, EventArgs e)
        {
            RiReader.Barcode = "";
            iniFile.Write("CurrentMessage", "Tray1Barcode", RiReader.Barcode, FrmMain.propath);
            for (int i = 0; i < PLC.PZ0TrayMax_Y * PLC.PZ0TrayMax_X; i++)
            {
                iniFile.Write("Table", i.ToString() + "Result", "", path);
                Sys.LabelsTray[i].BackColor = Color.Gray;
            }
        }
        private void btnClearT2_Click(object sender, EventArgs e)
        {
            Reader.Barcode = "";
            iniFile.Write("CurrentMessage", "Tray2Barcode", RiReader.Barcode, FrmMain.propath);
        }

        private void btnUntie_Click(object sender, EventArgs e)
        {
            if (!Glue.WebChecked)
            {
                MessageBox.Show("管控胶水可用时长功能未开启！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Glue.Barcode == "")
                return;
            fu = new FrmUntie();
            DialogResult result = fu.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtGlueBarcode.Text = "";
                txtGluePiao.Enabled = true;
                Glue.Barcode = txtGlueBarcode.Text;
                iniFile.Write("GlueBarcodePara", "GlueBarcode", txtGlueBarcode.Text, path);
                replaceTime = DateTime.Now;
                iniFile.Write("GlueBarcodePara", "GlueReplacementTime", replaceTime.ToString(), path);
            }
        }

        private void txtGluePiao_TextChanged(object sender, EventArgs e)
        {
            lblGOutShow.Hide();
        }

        public void cbCode_DropDownClosed(object sender, EventArgs e)
        {
            //补充Codes和产品类型
            string CodesIndex = cbCode.SelectedIndex.ToString();
            switch (cbCode.SelectedIndex)
            {
                case 0: Sys.Codes = "M"; break;
                case 1: Sys.Codes = "F"; break;
            }
            DateTime DT = DateTime.Now;
            if (RiReader.Web_Tray_InOutStation && Sys.Codes != "M")
            {
                parent.weblog.log("WebClose:" + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_首件_點膠進出站管控已關閉!");
                new MessageForm(10000000, DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_首件_點膠進出站管控已關閉").Show();
              
                rtxWebMessage.SelectionColor = Color.Orange;
                rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2")+"\t");
                rtxWebMessage.AppendText("首件_點膠進出站管控已關閉!" + "\r");
            }
            else if (RiReader.Web_Tray_InOutStation && Sys.Codes == "M")
            {
                parent.weblog.log("WebOpen:" + "Out," + DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_量產_點膠進出站管控已開啟!");
                new MessageForm(10000000, DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "_量產_點膠進出站管控已開啟!").Show();
               
                rtxWebMessage.SelectionColor = Color.Orange;
                rtxWebMessage.AppendText(DT.Hour.ToString("D2") + ":" + DT.Minute.ToString("D2") + ":" + DT.Second.ToString("D2") + "\t");
                rtxWebMessage.AppendText("量產_點膠進出站管控已開啟!"+"\r");
            }
            rtxWebMessage.SelectionIndent = rtxWebMessage.SelectionLength - 1;//至頂
            string path = Sys.IniPath + "\\ComPortPara.ini";   //设备各参数路径
            IniFile.Write("Addition", "Codes", CodesIndex, path);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            rtxWebMessage.SelectionStart = rtxWebMessage.TextLength;
            rtxWebMessage.ScrollToCaret();  
        }

     
      

       
    }
}
