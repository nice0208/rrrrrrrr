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

namespace LMTVision
{
    public partial class FrmSetUp : Form
    {
        FrmMain parent;
        FrmSetLogin fb;
        FrmGlueUserIn fgui;
        int prodownf = 0;
        string ProduceSele = "";
        bool readpara = false;
        public string path = Sys.IniPath + "\\ComPortPara.ini";   //设备各参数路径
        string glpath = Sys.IniPath + "\\SetParam.ini"; //胶水解绑人员设定路径
        Label[] gCUlable = new Label[10];// { lblgCUch1, lblgCUch2, lblgCUch3, lblgCUch4, lblgCUch5, lblgCUch6, lblgCUch7, lblgCUch8, lblgCUch9, lblgCUch10 };
        Label[] lblMinus = new Label[10];
        public FrmSetUp(FrmMain parent)
        {
            this.parent = parent;
            this.MdiParent = parent;
            this.Dock = DockStyle.Top;
            InitializeComponent();
        }
        private void FrmSetUp_Load(object sender, EventArgs e)
        {
            cBPCD2TrayAVI.Hide();
            cBPCD2Simpling.Hide();
            ReadReportPara();
            timerRefleshUI.Enabled = true;
        }      
        void ReadReportPara()
        {
            readpara = true;
            try
            {
                #region MachineID
                txtMachineId.Text = Sys.MachineId;
                switch (iniFile.Read("System", "FactoryChose", path))
                {
                    case "0": FactoryCh.SelectedIndex = 0; break;
                    case "1": FactoryCh.SelectedIndex = 1; break;
                }
                //Productions
                string name = iniFile.Read("CURRENTPRODUCENAME", "ProductName", Sys.IniPath + "\\Products.ini");
                string pidname = iniFile.Read("CURRENTPRODUCEID", "ProductID", Sys.IniPath + "\\Products.ini");
                string[] MName = new string[10] { "", "", "", "", "", "", "", "", "", "" };
                string[] MNamebar = new string[10] { "", "", "", "", "", "", "", "", "", "" };
                int pronum = 0, pidnum = 0;
                for (int i = 0; i < 10; i++)
                {
                    MName[i] = iniFile.Read("PRODUCENAME", "List&Name" + i.ToString(), Sys.IniPath + "\\Products.ini");
                    MNamebar[i] = iniFile.Read("PRODUCEID", "List&Name" + i.ToString(), Sys.IniPath + "\\Products.ini");
                    MNamebar[i] = (MNamebar[i].Length == 9) ? MNamebar[i].Substring(0, 6) : "";
                    if (MName[i] != "")
                    {
                        pronum++;
                        ListItem listItem = new ListItem("i", MName[i]);
                        ProduceName.Items.Add(listItem);
                        ProduceNameShow.Items.Add(listItem);
                        if (MName[i] == name)
                        {
                            prodownf = 1;
                            ProduceNameShow.SelectedIndex = pronum - 1;
                            ProduceName.SelectedIndex = pronum - 1;
                            string numBar = iniFile.Read("Tray1Num", "List&Name" + (ProduceNameShow.SelectedIndex + 1).ToString(), Sys.IniPath + "\\Products.ini");
                            lblTrayNum.Text = "42";
                            switch (numBar.Length)
                            {
                                case 9: lblTrayNum.Text = numBar.Substring(numBar.Length - 2, 2); break;
                                case 10: lblTrayNum.Text = numBar.Substring(numBar.Length - 3, 3); break;
                            }
                            FrmMain.Pnumber = (FrmMain.IsNumber(lblTrayNum.Text) ? int.Parse(lblTrayNum.Text) : 42);
                        }
                    }
                    if (MNamebar[i] != "")
                    {
                        pidnum++;
                        ListItem listItem = new ListItem("i", MNamebar[i]);
                        ProduceID.Items.Add(listItem);
                        ProduceIDShow.Items.Add(listItem);
                        if (MNamebar[i] == pidname)
                        {
                            piddownf = 1;
                            ProduceIDShow.SelectedIndex = pidnum - 1;
                            ProduceID.SelectedIndex = pidnum - 1;
                            string numBar = iniFile.Read("PRODUCEID", "List&Name" + pidnum.ToString(), Sys.IniPath + "\\Products.ini");
                            Sys.CurrentBarID = (numBar.Length == 9) ? numBar.Substring(numBar.Length - 2, 2) : "";
                            lblBarProgram.Text = Sys.CurrentBarID;
                        }
                    }
                }
                switch (iniFile.Read("CURRENTPRODUCEID", "Checked", path))
                {
                    case "YES": Sys.CurProduceIDCheck = true; ProduceIDCheck.SelectedIndex = 0; break;
                    case "NO": Sys.CurProduceIDCheck = false; ProduceIDCheck.SelectedIndex = 1; break;
                }
                #endregion
                #region LIGHT1
                cmbLig1Port.Text = LIGHT1.Com.PortName.Substring(3, 1);
                cmbLig1Baudrate.Text = LIGHT1.Com.BaudRate.ToString();
                cmbLig1DataBit.Text = LIGHT1.Com.DataBits.ToString();
                cmbLig1Parity.Text = LIGHT1.Com.Parity.ToString();
                cmbLig1StopBit.Text = (LIGHT1.Com.StopBits.ToString() == "Two" ? 2 : 1).ToString();
                #endregion
                #region LIGHT2
                cmbLig2Port.Text = LIGHT2.Com.PortName.Substring(3, 1);
                cmbLig2Baudrate.Text = LIGHT2.Com.BaudRate.ToString();
                cmbLig2DataBit.Text = LIGHT2.Com.DataBits.ToString();
                cmbLig2Parity.Text = LIGHT2.Com.Parity.ToString();
                cmbLig2StopBit.Text = (LIGHT2.Com.StopBits.ToString() == "Two" ? 2 : 1).ToString();
                #endregion
                #region LIGHT3
                cmbLig3Port.Text = LIGHT3.Com.PortName.Substring(3, 1);
                cmbLig3Baudrate.Text = LIGHT3.Com.BaudRate.ToString();
                cmbLig3DataBit.Text = LIGHT3.Com.DataBits.ToString();
                cmbLig3Parity.Text = LIGHT3.Com.Parity.ToString();
                cmbLig3StopBit.Text = (LIGHT3.Com.StopBits.ToString() == "Two" ? 2 : 1).ToString();
                cmbLight3Check.SelectedIndex = (LIGHT3.IsChecked ? 0 : 1);
                #endregion
                #region Left Reader
                cmbReaderPort.Text = Reader.Com.PortName.Substring(3, 1);
                cmbReaderBaudrate.Text = Reader.Com.BaudRate.ToString();
                cmbReaderDataBit.Text = Reader.Com.DataBits.ToString();
                cmbReaderParity.Text = Reader.Com.Parity.ToString();
                cmbReaderStopBit.Text = (Reader.Com.StopBits.ToString() == "Two" ? 2 : 1).ToString();
                cBR2Tray.Checked = Reader.IsChecked;
                #endregion
                #region Right Reader
                RcmbReaderPort.Text = RiReader.Com.PortName.Substring(3, 1);
                RcmbReaderBaudrate.Text = RiReader.Com.BaudRate.ToString();
                RcmbReaderDataBit.Text = RiReader.Com.DataBits.ToString();
                RcmbReaderParity.Text = RiReader.Com.Parity.ToString();
                RcmbReaderStopBit.Text = (RiReader.Com.StopBits.ToString() == "Two" ? 2 : 1).ToString();
                cBR1Tray.Checked = RiReader.IsChecked;
                cBWeb.Checked = RiReader.WebChecked;
                cBBpfWeb.Checked = RiReader.BPFWebChecked;  //bpf进出站
                cbWeb_Tray_InOutStation.Checked = RiReader.Web_Tray_InOutStation;
                cbWeb_Tray2InTray1Out_InOutStation.Checked = RiReader.Web_Tray2InTray1Out_InOutStation;
                cbWeb_Tray_InOutStation_ErrorIgnore.Checked = RiReader.Web_Tray_InOutStation_ErrorIgnore;

                #endregion

                #region GlueCU
                cmbGlueCUChoice.SelectedIndex = GlueCU.choice;
                cmbGlueCUPort.Text = GlueCU.Com.PortName.Substring(3, 1);
                cmbGlueCUBaudrate.Text = GlueCU.Com.BaudRate.ToString();
                cmbGlueCUDataBit.Text = GlueCU.Com.DataBits.ToString();
                cmbGlueCUParity.Text = GlueCU.Com.Parity.ToString();
                cmbGlueCUStopBit.Text = (GlueCU.Com.StopBits.ToString() == "Two" ? 2 : 1).ToString();
                cmbGlueCUCheck.SelectedIndex = (GlueCU.IsCheck ? 0 : 1);
                switch (GlueCU.choice)
                {
                    case 0:
                        groupBox3.Enabled = true;
                        groupBox4.Enabled = false;
                        gCUlable = new Label[10] { lblgCUch1, lblgCUch2, lblgCUch3, lblgCUch4, lblgCUch5, lblgCUch6, lblgCUch7, lblgCUch8, lblgCUch9, lblgCUch10 };
                        lblMinus = new Label[10] { lblMinus1, lblMinus2, lblMinus3, lblMinus4, lblMinus5, lblMinus6, lblMinus7, lblMinus8, lblMinus9, lblMinus10 };
                        GlueCU.Time_array = new NumericUpDown[10] { nudT1, nudT2, nudT3, nudT4, nudT5, nudT6, nudT7, nudT8, nudT9, nudT10 };
                        GlueCU.Pressure_array1 = new NumericUpDown[10] { nudP1, nudP2, nudP3, nudP4, nudP5, nudP6, nudP7, nudP8, nudP9, nudP10 };
                        GlueCU.Pressure_array2 = new NumericUpDown[10] { nudP11, nudP12, nudP13, nudP14, nudP15, nudP16, nudP17, nudP18, nudP19, nudP20 };
                        cbGlueCUUsingCh.SelectedIndex = GlueCU.ChanleNumber - 1;
                        for (int i = 0; i < GlueCU.ChanleNumber; i++)
                        {
                            GlueCU.Time_array[i].Value = decimal.Parse(GlueCU.arrayTime[i].ToString());
                            GlueCU.Pressure_array1[i].Value = decimal.Parse(GlueCU.arrayPressure[i].ToString());
                            GlueCU.Pressure_array2[i].Value = decimal.Parse(GlueCU.arrayPressure[10 + i].ToString());
                        }
                        break;
                    case 1:
                        groupBox3.Enabled = false;
                        groupBox4.Enabled = true;
                        nu808dT1.Value = decimal.Parse(GlueCU.arrayTime[0].ToString());
                        nu808dT2.Value = decimal.Parse(GlueCU.arrayTime[1].ToString());
                        nu808dP1.Value = decimal.Parse(GlueCU.arrayPressure[0].ToString());
                        nu808dP2.Value = decimal.Parse(GlueCU.arrayPressure[1].ToString());
                        break;
                }
                #endregion
                #region Barcode
                cBRead1.Checked = Barcode1.IsChecked;
                cB1OK.Checked = Barcode1.OkSave;
                cB1NG.Checked = Barcode1.NgSave;
                #endregion
                #region Glue
                cBGlue.Checked = Glue.IsChecked;
                txtGlueH.Text = Glue.Hour;
                txtGlueM.Text = Glue.Minute;
                cBkeyEnter.Checked = Glue.IsKeyEnter;
                cBGlueWeb.Checked = Glue.WebChecked;
                cBGlueWeightWeb.Checked = Glue.WeightWebChecked;
                listProduction.Items.AddRange(Glue.AllProductPairlist.ToArray());
                #endregion
                #region Monitor
                cBUVMode.Checked = Monitor.UVModeCheck;
                cBPCD2TrayAVI.Checked = Monitor.PCCD2TrayAVI;
                cBPCD2Simpling.Checked = Monitor.PCCD2Sampling;
                #endregion

                #region A1CCD1
                txtA1CD1Ip.Text = A1CCD1.ip.ToString();
                txtA1CD1Port.Text = A1CCD1.Port.ToString();
                cmbA1CD1Check.SelectedIndex = (A1CCD1.IsCheck ? 0 : 1);
                //string check1 = iniFile.Read("A1_CCD1", "Checked", path);
                //switch (check1)
                //{
                //    case "YES": A1CCD1.IsCheck = true; cmbA1CD1Check.SelectedIndex = 0; break;
                //    case "NO": A1CCD1.IsCheck = false; cmbA1CD1Check.SelectedIndex = 1; break;
                //}
                #endregion
                #region A1CCD2
                txtA1CD2Ip.Text = A1CCD2.ip.ToString();
                txtA1CD2Port.Text = A1CCD2.Port.ToString();
                cmbA1CD2Check.SelectedIndex = (A1CCD2.IsCheck ? 0 : 1);
                #endregion
                #region A2CCD1
                txtA2CD1Ip.Text = A2CCD1.ip.ToString();
                txtA2CD1Port.Text = A2CCD1.Port.ToString();
                cmbA2CD1Check.SelectedIndex = (A2CCD1.IsCheck ? 0 : 1);
                #endregion
                #region A2CCD2
                txtA2CD2Ip.Text = A2CCD2.ip.ToString();
                txtA2CD2Port.Text = A2CCD2.Port.ToString();
                cmbA2CD2Check.SelectedIndex = (A2CCD2.IsCheck ? 0 : 1);
                #endregion
                #region PCCD1
                txtPCD1Ip.Text = PCCD1.ip.ToString();
                txtPCD1Port.Text = PCCD1.Port.ToString();
                cmbPCD1Check.SelectedIndex = (PCCD1.IsCheck ? 0 : 1);
                #endregion
                #region PCCD2
                txtPCD2Ip.Text = PCCD2.ip.ToString();
                txtPCD2Port.Text = PCCD2.Port.ToString();
                cmbPCD2Check.SelectedIndex = (PCCD2.IsCheck ? 0 : 1);
                #endregion
                #region GCCD1
                txtGCD1Ip.Text = GCCD1.ip.ToString();
                txtGCD1Port.Text = GCCD1.Port.ToString();
                cmbGCD1Check.SelectedIndex = (GCCD1.IsCheck ? 0 : 1);
                #endregion
                #region GCCD2
                txtGCD2Ip.Text = GCCD2.ip.ToString();
                txtGCD2Port.Text = GCCD2.Port.ToString();
                cmbGCD2Check.SelectedIndex = (GCCD2.IsCheck ? 0 : 1);
                #endregion
                #region QCCD
                cmbCCDChoice.SelectedIndex = QCCD.CCDBrand;
                txtQCDIp.Text = QCCD.ip.ToString();
                txtQCDPort.Text = QCCD.Port.ToString();
                cmbQCCDCheck.SelectedIndex = (QCCD.IsCheck ? 0 : 1);
                #endregion

                #region PLC
                txtPLCIp.Text = PLC.Ip.ToString();
                txtPLCPort.Text = PLC.Port.ToString();
                #endregion
                #region GT2
                txtGT2Ip.Text = GT2.ip.ToString();
                txtGT2Port.Text = GT2.Port.ToString();
                cmbGT2Check.SelectedIndex = (GT2.IsCheck ? 0 : 1);
                gBGT2Set.Enabled = GT2.IsCheck;
                GT21check.Checked = GT2.CH1IsCheck;
                GT22check.Checked = GT2.CH2IsCheck;
                GT23check.Checked = GT2.CH3IsCheck;
                GT24check.Checked = GT2.CH4IsCheck;
                GT25check.Checked = GT2.CH5IsCheck;
                #endregion
            }
            catch
            {
            }
            readpara = false;
        }

        //厂区选择
        private void FactoryCh_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (FactoryCh.SelectedIndex)
            {
                case 0: Sys.Factory = "XM"; break;
                case 1: Sys.Factory = "JM"; break;
                case 2: Sys.Factory = "TD"; break;
            }
            iniFile.Write("System", "FactoryChose", FactoryCh.SelectedIndex.ToString(), path);
        }
        private void btnMachineId_Click(object sender, EventArgs e)
        {
            if (txtMachineId.Text.Trim() == "")
            {
                MessageBox.Show("机台编号不能为空");
                return;
            }
            iniFile.Write("System", "MachineId", txtMachineId.Text.Trim(), path);
            Sys.MachineId = txtMachineId.Text.Trim();
            MessageBox.Show("保存OK");
        }

        //机种名称及部品数 默认为42
        string strProName = ""; int curProSeleIndex = -1;
        private void btnChangeP_Click(object sender, EventArgs e)
        {
            if (strProName != ProduceName.Text & curProSeleIndex != -1)
            {
                DialogResult dr = MessageBox.Show("确定要修改机种名称吗?", "",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Cancel)
                    return;
                string curProName = ProduceName.Text;
                string SIDBar = "42";
                if (txtTrayNum.Text != "")
                    SIDBar = PLC.PlateCount;
                iniFile.Write("PRODUCENAME", "List&Name" + (curProSeleIndex).ToString(), curProName, Sys.IniPath + "\\Products.ini");
                iniFile.Write("Tray1Num", "List&Name" + (curProSeleIndex).ToString(), curProName + "*" + SIDBar, Sys.IniPath + "\\Products.ini");
                string strIniFileName = Sys.IniPath + "\\" + strProName + "_SetReport.ini";
                string curIniFileName = Sys.IniPath + "\\" + curProName + "_SetReport.ini";
                if (File.Exists(strIniFileName))
                    File.Move(strIniFileName, curIniFileName);
                string strFilePath = Sys.IniPath + "\\" + strProName;
                string curFilePath = Sys.IniPath + "\\" + curProName;
                if (Directory.Exists(strFilePath))
                    Directory.Move(strFilePath, curFilePath);
                MessageBox.Show("修改成功！");
            }
            else
            {
                MessageBox.Show("机种名称并未修改！");
            }
        }
        private void AddP_Click(object sender, EventArgs e)
        {
            int index = ProduceName.Items.Count;
            if (ProduceName.Text == "")
            {
                MessageBox.Show("机种名称不能为空！");
                AddP.Focus();
                return;
            }
            if (ProduceName.Text.Length != 6)
            {
                MessageBox.Show("请确认机种名称是否正确！");
                return;
            }
            DialogResult dr = MessageBox.Show("确定要添加新机种吗?", "",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Cancel)
                return;
            string SID = (ProduceName.SelectedIndex + 1).ToString();
            string SName = ProduceName.Text;
            string SIDBar = "42";
            if (txtTrayNum.Text != "")
                SIDBar = txtTrayNum.Text;
            for (int i = 0; i < index; ++i)
            {
                string readname = iniFile.Read("PRODUCENAME", "List&Name" + (i + 1).ToString(), Sys.IniPath + "\\Products.ini");
                string numbar = iniFile.Read("Tray1Num", "List&Name" + (i + 1).ToString(), Sys.IniPath + "\\Products.ini");
                if (SName == readname)
                {
                    if (numbar == SName + "*" + SIDBar)
                        MessageBox.Show("机种名称不能重复");
                    else
                    {
                        iniFile.Write("Tray1Num", "List&Name" + (i + 1).ToString(), SName + "*" + SIDBar, Sys.IniPath + "\\Products.ini");
                        MessageBox.Show("机种所对应的Tray1部品数修改成功");
                    }
                    AddP.Focus();
                    return;
                }
            }
            ListItem item = new ListItem(SID, SName);
            ProduceName.Items.Add(item);
            ProduceNameShow.Items.Add(item);
            iniFile.Write("PRODUCENAME", "List&Name" + (index + 1).ToString(), SName, Sys.IniPath + "\\Products.ini");
            iniFile.Write("Tray1Num", "List&Name" + (index + 1).ToString(), SName + "*" + SIDBar, Sys.IniPath + "\\Products.ini");
            AddP.Focus();
        }
        private void RemoveP_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定要删除所选机种吗?", "",
                                           MessageBoxButtons.OKCancel,
                                           MessageBoxIcon.Information,
                                           MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Cancel)
                return;
            int index = ProduceName.Items.Count;
            int idir = ProduceName.SelectedIndex;
            ListItem listItem = ProduceName.SelectedItem as ListItem;
            string dir = Sys.IniPath + "\\" + listItem.ToString();
            for (int i = 0; i < index; ++i)
            {
                if (i == idir)
                {
                    for (int j = i; j < index - 1; j++)
                    {
                        string x = iniFile.Read("PRODUCENAME", "List&Name" + (j + 2).ToString(), Sys.IniPath + "\\Products.ini");
                        string y = iniFile.Read("Tray1Num", "List&Name" + (j + 2).ToString(), Sys.IniPath + "\\Products.ini");
                        iniFile.Write("PRODUCENAME", "List&Name" + (j + 1).ToString(), x, Sys.IniPath + "\\Products.ini");
                        iniFile.Write("Tray1Num", "List&Name" + (j + 1).ToString(), y, Sys.IniPath + "\\Products.ini");
                    }
                    iniFile.Write("PRODUCENAME", "List&Name" + index.ToString(), "", Sys.IniPath + "\\Products.ini");
                    iniFile.Write("Tray1Num", "List&Name" + index.ToString(), "", Sys.IniPath + "\\Products.ini");
                    break;
                }
            }
            ProduceName.Items.Remove(listItem);
            ProduceNameShow.Items.Remove(listItem);
            ProduceName.Text = "";
            RemoveP.Focus();
        }
        private void ProduceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProduceName.SelectedIndex != -1)
            {
                strProName = ProduceName.Text;
                curProSeleIndex = ProduceName.SelectedIndex + 1;
                string numBar = iniFile.Read("Tray1Num", "List&Name" + (ProduceName.SelectedIndex + 1).ToString(), Sys.IniPath + "\\Products.ini");
                txtTrayNum.Text = PLC.PlateCount;
                //switch (numBar.Length)
                //{
                //    case 9: txtTrayNum.Text = numBar.Substring(numBar.Length - 2, 2); break;
                //    case 10: txtTrayNum.Text = numBar.Substring(numBar.Length - 3, 3); break;
                //}
            }
        }
        private void ProduceNameShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (prodownf != 1)
            {
                if (ProduceNameShow.SelectedIndex != -1)
                {
                    DialogResult dr = MessageBox.Show("确定要选择“" + ProduceNameShow.Text + "”机种吗?", "",
                                                    MessageBoxButtons.OKCancel,
                                                    MessageBoxIcon.Information,
                                                    MessageBoxDefaultButton.Button2);

                    if (dr == DialogResult.OK)
                    {
                        ProduceSele = ProduceNameShow.Text;
                        string numBar = iniFile.Read("Tray1Num", "List&Name" + (ProduceNameShow.SelectedIndex + 1).ToString(), Sys.IniPath + "\\Products.ini");
                        lblTrayNum.Text = "42";
                        switch (numBar.Length)
                        {
                            case 9: lblTrayNum.Text = numBar.Substring(numBar.Length - 2, 2); break;
                            case 10: lblTrayNum.Text = numBar.Substring(numBar.Length - 3, 3); break;
                        }
                        FrmMain.Pnumber = (FrmMain.IsNumber(lblTrayNum.Text) ? int.Parse(lblTrayNum.Text) : 42);
                        if (!Directory.Exists(Sys.ReportLog + "\\" + ProduceSele))
                        {
                            DirectoryInfo di = new DirectoryInfo(Sys.ReportLog + "\\" + ProduceSele);
                            di.Create();
                        }
                        if (!File.Exists(Sys.ReportLog + "\\" + ProduceSele + "\\Products.ini"))
                            iniFile.Write("Login", "Information", "Vision", Sys.IniPath + "\\" + ProduceSele + "_SetReport.ini");
                        iniFile.Write("CURRENTPRODUCENAME", "ProductName", ProduceSele, Sys.IniPath + "\\Products.ini");
                        iniFile.Write("CURRENTTRAY1NUM", "Tray1Num", lblTrayNum.Text, Sys.IniPath + "\\Products.ini");
                    }
                    else
                    {
                        ProduceNameShow.SelectedIndex = -1;
                    }
                    Sys.CurrentProduction = ProduceNameShow.Text;
                }
            }
            else
            {
                prodownf = 0;
                ProduceSele = ProduceNameShow.Text;
                Sys.CurrentProduction = ProduceNameShow.Text;
            }
            FrmMain.propath = Sys.IniPath + "\\" + Sys.CurrentProduction + "_SetReport.ini";
        }

        //部品ID
        int piddownf = 0;
        private void AddPID_Click(object sender, EventArgs e)
        {
            int index = ProduceID.Items.Count;
            if (ProduceID.Text == "" || IDBarProgram.Text == "")
            {
                MessageBox.Show("部品ID和Barcode(Program)不能为空！");
                AddPID.Focus();
                return;
            }
            DialogResult dr = MessageBox.Show("确定要添加部品ID吗?", "",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Cancel)
                return;
            string SID = (ProduceID.SelectedIndex + 1).ToString();
            string SName = ProduceID.Text;
            string SIDBar = IDBarProgram.Text;
            for (int i = 0; i < index; ++i)
            {
                if (i == ProduceID.SelectedIndex)
                    continue;
                string numBar = iniFile.Read("PRODUCEID", "List&Name" + (index + 1).ToString(), Sys.IniPath + "\\Products.ini");
                numBar = (numBar.Length == 9) ? numBar.Substring(numBar.Length - 2, 2) : "";
                if ((ProduceID.Items[i]).ToString().Trim() == SName.Trim() && SIDBar == numBar)
                {
                    MessageBox.Show("部品ID和Barcode(Program)不能同时重复");
                    AddPID.Focus();
                    return;
                }
            }
            ListItem item = new ListItem(SID, SName);
            ProduceID.Items.Add(item);
            ProduceIDShow.Items.Add(item);
            iniFile.Write("PRODUCEID", "List&Name" + (index + 1).ToString(), SName + "*" + SIDBar, Sys.IniPath + "\\Products.ini");
            AddPID.Focus();
        }
        private void RemoveID_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定要删除所选部品ID吗?", "",
                                           MessageBoxButtons.OKCancel,
                                           MessageBoxIcon.Information,
                                           MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Cancel)
                return;
            int index = ProduceID.Items.Count;
            int idir = ProduceID.SelectedIndex;
            ListItem listItem = ProduceID.SelectedItem as ListItem;
            for (int i = 0; i < index; ++i)
            {
                if (i != index - 1)
                {
                    if (i == idir)
                    {
                        for (int j = i; j < index - 1; j++)
                        {
                            string x = iniFile.Read("PRODUCEID", "List&Name" + (j + 2).ToString(), Sys.IniPath + "\\Products.ini");
                            iniFile.Write("PRODUCEID", "List&Name" + (j + 1).ToString(), x, Sys.IniPath + "\\Products.ini");
                        }
                        iniFile.Write("PRODUCEID", "List&Name" + index.ToString(), "", Sys.IniPath + "\\Products.ini");
                        break;
                    }
                }
                else
                {
                    iniFile.Write("PRODUCEID", "List&Name" + index.ToString(), "", Sys.IniPath + "\\Products.ini");
                }
            }
            ProduceID.Items.Remove(listItem);
            ProduceIDShow.Items.Remove(listItem);
            ProduceID.Text = "";
            RemoveID.Focus();
        }
        private void ProduceIDShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (piddownf != 1)
            {
                if (ProduceIDShow.SelectedIndex != -1)
                {
                    DialogResult dr = MessageBox.Show("确定要选择“" + ProduceIDShow.Text + "”部品ID吗?", "",
                                                    MessageBoxButtons.OKCancel,
                                                    MessageBoxIcon.Information,
                                                    MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.OK)
                    {
                        string numBar = iniFile.Read("PRODUCEID", "List&Name" + (ProduceIDShow.SelectedIndex + 1).ToString(), Sys.IniPath + "\\Products.ini");
                        Sys.CurrentBarID = (numBar.Length == 9) ? numBar.Substring(numBar.Length - 2, 2) : "";
                        Sys.CurrentProID = (numBar.Length == 9) ? numBar.Substring(0, 6) : "";
                        lblBarProgram.Text = Sys.CurrentBarID;
                        iniFile.Write("CURRENTPRODUCEID", "ProductID", ProduceIDShow.Text, Sys.IniPath + "\\Products.ini");
                    }
                    else
                    {
                        ProduceIDShow.SelectedIndex = -1;
                    }
                }
            }
            else
            {
                piddownf = 0;
                Sys.CurrentBarID = lblBarProgram.Text;
            }
            ProduceID.Text = ProduceIDShow.Text;
        }
        private void btnProduceIdSave_Click(object sender, EventArgs e)
        {
            if (ProduceIDCheck.SelectedIndex >= 0)
                iniFile.Write("CURRENTPRODUCEID", "Checked", (string)ProduceIDCheck.SelectedItem, path);
            switch ((string)ProduceIDCheck.SelectedItem)
            {
                case "YES": Sys.CurProduceIDCheck = true; Sys.CurrentProID = ProduceID.Text; break;
                case "NO": Sys.CurProduceIDCheck = false; Sys.CurrentProID = ""; break;
            }
            MessageBox.Show("保存OK");
        }
        private void IDBarProgram_TextChanged(object sender, EventArgs e)
        {
            if (IDBarProgram.TextLength > 2)
            {
                MessageBox.Show("请确认Barcode(Program)输入是否有误！");
                IDBarProgram.Text = IDBarProgram.Text.Substring(0, 2);
                return;
            }
        }
        private void ProduceID_TextChanged(object sender, EventArgs e)
        {
            IDBarProgram.Text = "";
            if (ProduceID.Text.Length > 6)
            {
                MessageBox.Show("请确认部品ID输入是否有误！");
                ProduceID.Text = ProduceID.Text.Substring(0, 6);
                return;
            }
        }

        //时间校正
        private void btnTimeCelebrete_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string Year = (dt.Year % 100).ToString("X4");
            string Month = dt.Month.ToString("X4");
            string Day = dt.Day.ToString("X4");
            string Hour = dt.Hour.ToString("X4");
            string Minute = dt.Minute.ToString("X4");
            string Second = dt.Second.ToString("X4");
            WriteToPlc.sTimeCelebrate = "01WWRD00300,07,0001" +
                                                Year + Month + Day +
                                                Hour + Minute + Second + "\r\n";
            WriteToPlc.bTimeCelebrate = true;
            MessageBox.Show("校正完毕");
        }

        //通讯参数1
        private void btnLig1Port_Click(object sender, EventArgs e)
        {
            if (cmbLig1Port.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig1", "Port", (string)cmbLig1Port.SelectedItem, path);
            }
            if (cmbLig1Baudrate.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig1", "Baudrate", (string)cmbLig1Baudrate.SelectedItem, path);
            }
            if (cmbLig1DataBit.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig1", "DataBit", (string)cmbLig1DataBit.SelectedItem, path);
            }
            if (cmbLig1Parity.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig1", "Parity", (string)cmbLig1Parity.SelectedItem, path);
            }
            if (cmbLig1StopBit.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig1", "StopBit", (string)cmbLig1StopBit.SelectedItem, path);
            }
            MessageBox.Show("保存OK");
        }
        private void btnLig2Port_Click(object sender, EventArgs e)
        {
            if (cmbLig2Port.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig2", "Port", (string)cmbLig2Port.SelectedItem, path);
            }
            if (cmbLig2Baudrate.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig2", "Baudrate", (string)cmbLig2Baudrate.SelectedItem, path);
            }
            if (cmbLig2DataBit.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig2", "DataBit", (string)cmbLig2DataBit.SelectedItem, path);
            }
            if (cmbLig2Parity.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig2", "Parity", (string)cmbLig2Parity.SelectedItem, path);
            }
            if (cmbLig2StopBit.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig2", "StopBit", (string)cmbLig2StopBit.SelectedItem, path);
            }
            MessageBox.Show("保存OK");
        }
        private void btnReaderPortL_Click(object sender, EventArgs e)
        {
            if (cmbReaderPort.SelectedIndex >= 0)
            {
                iniFile.Write("Reader2", "Port", (string)cmbReaderPort.SelectedItem, path);
            }
            if (cmbReaderBaudrate.SelectedIndex >= 0)
            {
                iniFile.Write("Reader2", "Baudrate", (string)cmbReaderBaudrate.SelectedItem, path);
            }
            if (cmbReaderDataBit.SelectedIndex >= 0)
            {
                iniFile.Write("Reader2", "DataBit", (string)cmbReaderDataBit.SelectedItem, path);
            }
            if (cmbReaderParity.SelectedIndex >= 0)
            {
                iniFile.Write("Reader2", "Parity", (string)cmbReaderParity.SelectedItem, path);
            }
            if (cmbReaderStopBit.SelectedIndex >= 0)
            {
                iniFile.Write("Reader2", "StopBit", (string)cmbReaderStopBit.SelectedItem, path);
            }
            MessageBox.Show("保存OK");
        }
        private void btnReaderPortR_Click(object sender, EventArgs e)
        {
            if (RcmbReaderPort.SelectedIndex >= 0)
            {
                iniFile.Write("Reader1", "Port", (string)RcmbReaderPort.SelectedItem, path);
            }
            if (RcmbReaderBaudrate.SelectedIndex >= 0)
            {
                iniFile.Write("Reader1", "Baudrate", (string)RcmbReaderBaudrate.SelectedItem, path);
            }
            if (RcmbReaderDataBit.SelectedIndex >= 0)
            {
                iniFile.Write("Reader1", "DataBit", (string)RcmbReaderDataBit.SelectedItem, path);
            }
            if (RcmbReaderParity.SelectedIndex >= 0)
            {
                iniFile.Write("Reader1", "Parity", (string)RcmbReaderParity.SelectedItem, path);
            }
            if (RcmbReaderStopBit.SelectedIndex >= 0)
            {
                iniFile.Write("Reader1", "StopBit", (string)RcmbReaderStopBit.SelectedItem, path);
            }
            MessageBox.Show("保存OK");
        }
        private void btnGlueCUSave_Click(object sender, EventArgs e)
        {
            if (cmbGlueCUChoice.SelectedIndex >= 0)
            {
                GlueCU.choice = cmbGlueCUChoice.SelectedIndex;
                iniFile.Write("GlueCU", "Choice", GlueCU.choice.ToString(), path);
            }
            if (cmbGlueCUPort.SelectedIndex >= 0)
            {
                iniFile.Write("GlueCU", "Port", (string)cmbGlueCUPort.SelectedItem, path);
            }
            if (cmbGlueCUBaudrate.SelectedIndex >= 0)
            {
                iniFile.Write("GlueCU", "Baudrate", (string)cmbGlueCUBaudrate.SelectedItem, path);
            }
            if (cmbGlueCUDataBit.SelectedIndex >= 0)
            {
                iniFile.Write("GlueCU", "DataBit", (string)cmbGlueCUDataBit.SelectedItem, path);
            }
            if (cmbGlueCUParity.SelectedIndex >= 0)
            {
                iniFile.Write("GlueCU", "Parity", (string)cmbGlueCUParity.SelectedItem, path);
            }
            if (cmbGlueCUStopBit.SelectedIndex >= 0)
            {
                iniFile.Write("GlueCU", "StopBit", (string)cmbGlueCUStopBit.SelectedItem, path);
            }
            if (cmbGlueCUCheck.SelectedIndex >= 0)
            {
                iniFile.Write("GlueCU", "Checked", (string)cmbGlueCUCheck.SelectedItem, path);
            }
            MessageBox.Show("保存OK");
        }
        private void btnLig3Port_Click(object sender, EventArgs e)
        {
            if (cmbLig3Port.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig3", "Port", (string)cmbLig3Port.SelectedItem, path);
            }
            if (cmbLig3Baudrate.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig3", "Baudrate", (string)cmbLig3Baudrate.SelectedItem, path);
            }
            if (cmbLig3DataBit.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig3", "DataBit", (string)cmbLig3DataBit.SelectedItem, path);
            }
            if (cmbLig3Parity.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig3", "Parity", (string)cmbLig3Parity.SelectedItem, path);
            }
            if (cmbLig3StopBit.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig3", "StopBit", (string)cmbLig3StopBit.SelectedItem, path);
            }
            if (cmbLight3Check.SelectedIndex >= 0)
            {
                iniFile.Write("btnLig3", "Checked", (string)cmbLight3Check.SelectedItem, path);
            }
            MessageBox.Show("保存OK");
        }
        //通讯参数2
        private void btnA1CD1Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtA1CD1Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtA1CD1Ip.Focus();
                return;
            }
            if (!int.TryParse(txtA1CD1Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtA1CD1Port.Focus();
                return;
            }
            iniFile.Write("A1_CCD1", "Ip", txtA1CD1Ip.Text, path);
            iniFile.Write("A1_CCD1", "Port", txtA1CD1Port.Text, path);
            A1CCD1.ip = ip;
            A1CCD1.Port = port;
            if (cmbA1CD1Check.SelectedIndex >= 0)
                iniFile.Write("A1_CCD1", "Checked", (string)cmbA1CD1Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnA1CD2Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtA1CD2Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtA1CD2Ip.Focus();
                return;
            }
            if (!int.TryParse(txtA1CD2Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtA1CD2Port.Focus();
                return;
            }
            iniFile.Write("A1_CCD2", "Ip", txtA1CD2Ip.Text, path);
            iniFile.Write("A1_CCD2", "Port", txtA1CD2Port.Text, path);
            A1CCD2.ip = ip;
            A1CCD2.Port = port;
            if (cmbA1CD2Check.SelectedIndex >= 0)
                iniFile.Write("A1_CCD2", "Checked", (string)cmbA1CD2Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnA2CD1Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtA2CD1Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtA2CD1Ip.Focus();
                return;
            }
            if (!int.TryParse(txtA2CD1Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtA2CD1Port.Focus();
                return;
            }
            iniFile.Write("A2_CCD1", "Ip", txtA2CD1Ip.Text, path);
            iniFile.Write("A2_CCD1", "Port", txtA2CD1Port.Text, path);
            A2CCD1.ip = ip;
            A2CCD1.Port = port;
            if (cmbA2CD1Check.SelectedIndex >= 0)
                iniFile.Write("A2_CCD1", "Checked", (string)cmbA2CD1Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnA2CD2Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtA2CD2Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtA2CD2Ip.Focus();
                return;
            }
            if (!int.TryParse(txtA2CD2Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtA2CD2Port.Focus();
                return;
            }
            iniFile.Write("A2_CCD2", "Ip", txtA2CD2Ip.Text, path);
            iniFile.Write("A2_CCD2", "Port", txtA2CD2Port.Text, path);
            A2CCD2.ip = ip;
            A2CCD2.Port = port;
            if (cmbA2CD2Check.SelectedIndex >= 0)
                iniFile.Write("A2_CCD2", "Checked", (string)cmbA2CD2Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnPCD1Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtPCD1Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtPCD1Ip.Focus();
                return;
            }
            if (!int.TryParse(txtPCD1Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtPCD1Port.Focus();
                return;
            }
            iniFile.Write("P_CCD1", "Ip", txtPCD1Ip.Text, path);
            iniFile.Write("P_CCD1", "Port", txtPCD1Port.Text, path);
            PCCD1.ip = ip;
            PCCD1.Port = port;
            if (cmbPCD1Check.SelectedIndex >= 0)
                iniFile.Write("P_CCD1", "Checked", (string)cmbPCD1Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnPCD2Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtPCD2Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtPCD2Ip.Focus();
                return;
            }
            if (!int.TryParse(txtPCD2Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtPCD2Port.Focus();
                return;
            }
            iniFile.Write("P_CCD2", "Ip", txtPCD2Ip.Text, path);
            iniFile.Write("P_CCD2", "Port", txtPCD2Port.Text, path);
            PCCD2.ip = ip;
            PCCD2.Port = port;
            if (cmbPCD2Check.SelectedIndex >= 0)
                iniFile.Write("P_CCD2", "Checked", (string)cmbPCD2Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnGCD1Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtGCD1Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtGCD1Ip.Focus();
                return;
            }
            if (!int.TryParse(txtGCD1Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtGCD1Port.Focus();
                return;
            }
            iniFile.Write("G_CCD1", "Ip", txtGCD1Ip.Text, path);
            iniFile.Write("G_CCD1", "Port", txtGCD1Port.Text, path);
            GCCD1.ip = ip;
            GCCD1.Port = port;
            if (cmbGCD1Check.SelectedIndex >= 0)
                iniFile.Write("G_CCD1", "Checked", (string)cmbGCD1Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnGCD2Save_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtGCD2Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtGCD2Ip.Focus();
                return;
            }
            if (!int.TryParse(txtGCD2Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtGCD2Port.Focus();
                return;
            }
            iniFile.Write("G_CCD2", "Ip", txtGCD2Ip.Text, path);
            iniFile.Write("G_CCD2", "Port", txtGCD2Port.Text, path);
            GCCD2.ip = ip;
            GCCD2.Port = port;
            if (cmbGCD2Check.SelectedIndex >= 0)
                iniFile.Write("G_CCD2", "Checked", (string)cmbGCD2Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnQCDSave_Click(object sender, EventArgs e)
        {

            QCCD.CCDBrand = cmbCCDChoice.SelectedIndex;
            iniFile.Write("CCD", "CCDChoice", QCCD.CCDBrand.ToString(), path); 
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtQCDIp.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtQCDIp.Focus();
                return;
            }
            if (!int.TryParse(txtQCDPort.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtQCDPort.Focus();
                return;
            }
            iniFile.Write("Q_CCD", "Ip", txtQCDIp.Text, path);
            iniFile.Write("Q_CCD", "Port", txtQCDPort.Text, path);
            QCCD.ip = ip;
            QCCD.Port = port;
            if (cmbQCCDCheck.SelectedIndex >= 0)
                iniFile.Write("Q_CCD", "Checked", (string)cmbQCCDCheck.SelectedItem, path);
            MessageBox.Show("保存OK");
        }
        private void btnPLCSave_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtPLCIp.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtPLCIp.Focus();
                return;
            }
            if (!int.TryParse(txtPLCPort.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtPLCPort.Focus();
                return;
            }
            iniFile.Write("PLC", "Ip", txtPLCIp.Text, path);
            iniFile.Write("PLC", "Port", txtPLCPort.Text, path);
            PLC.startIp = ip.ToString();
            PLC.Port = port;
            MessageBox.Show("保存OK");
        }
        private void btnGT2sSave_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Any;
            int port = 0;
            if (!IPAddress.TryParse(txtGT2Ip.Text, out ip))
            {
                MessageBox.Show("Ip地址格式不正确!");
                txtGT2Ip.Focus();
                return;
            }
            if (!int.TryParse(txtGT2Port.Text, out port) && port > 0)
            {
                MessageBox.Show("端口设置不正确!");
                txtGT2Port.Focus();
                return;
            }
            iniFile.Write("GT2", "Ip", txtGT2Ip.Text, path);
            iniFile.Write("GT2", "Port", txtGT2Port.Text, path);
            GT2.ip = ip;
            GT2.Port = port;
            if (cmbGT2Check.SelectedIndex >= 0)
                iniFile.Write("GT2", "Checked", (string)cmbGT2Check.SelectedItem, path);
            MessageBox.Show("保存OK");
        }

        int ti = 0;  //修改设置
        private void btnChange_Click(object sender, EventArgs e)
        {
            switch (ti)
            {
                case 0:
                    fb = new FrmSetLogin();
                    DialogResult result = fb.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        btnChange.BackColor = Color.Green;
                        groupBox29.Enabled = true;
                        btnChange.Text = "修改完成";
                    }
                    break;
                case 1:
                    btnChange.BackColor = Color.WhiteSmoke;
                    groupBox29.Enabled = false;
                    btnChange.Text = "修改设置";
                    break;
            }
            ti++;
            if (ti == 2)
            {
                ti = 0;
            }
        }

        //设置参数
        private void cBWeb_CheckedChanged(object sender, EventArgs e)
        {
            if (cBWeb.Checked & cBBpfWeb.Checked)
            {
                cBWeb.Checked = false;
                MessageBox.Show("管控Tray1时间功能与Tray1进出站过账不能同时勾选！");
            }
        }
        private void cBBpfWeb_CheckedChanged(object sender, EventArgs e)
        {
            if (cBWeb.Checked & cBBpfWeb.Checked)
            {
                cBBpfWeb.Checked = false;
                MessageBox.Show("管控Tray1时间功能与Tray1进出站过账不能同时勾选！");
            }
        }
        private void cBRead1_CheckedChanged(object sender, EventArgs e)
        {
            gBPicture.Enabled = cBRead1.Checked;
        }
        private void cBGlue_CheckedChanged(object sender, EventArgs e)
        {
            txtGlueH.Enabled = cBGlue.Checked;
            txtGlueM.Enabled = cBGlue.Checked;
        }
        private void txtGlueH_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void txtGlueM_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void btnReadSave_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否确定保存设置?", "",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Information,
                                                   MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                string option1 = (cBR1Tray.Checked ? "True" : "False");
                iniFile.Write("Reader1", "Tray1Checked", option1, path);
                string option2 = (cBWeb.Checked ? "True" : "False");
                iniFile.Write("WebServer", "Checked", option2, path);
                string option = (cBBpfWeb.Checked ? "True" : "False");
                iniFile.Write("BPFWebServer", "Checked", option, path);
                //PZ0進出站(壓覆點膠用)
                option = (cbWeb_Tray_InOutStation.Checked ? "True" : "False");
                iniFile.Write("Web_PZ0_InOutStation", "Checked", option, path);
                option = (cbWeb_Tray_InOutStation_ErrorIgnore.Checked ? "True" : "False");
                iniFile.Write("Web_PZ0_InOutStation_ErrorIgnore", "Checked", option, path);
                option = (cbWeb_Tray2InTray1Out_InOutStation.Checked ? "True" : "False");
                iniFile.Write("Web_AZ0InPZ0Out_InOutStation", "Checked", option, path);
                string option3 = (cBR2Tray.Checked ? "True" : "False");
                iniFile.Write("Reader2", "Tray2Checked", option3, path);
                
                string option4 = (cBRead1.Checked ? "True" : "False");
                iniFile.Write("BarcodeReader", "IsChecked", option4, path);
                string option5 = (cB1OK.Checked ? "True" : "False");
                iniFile.Write("BarcodeReader", "OkSave", option5, path);
                string option6 = (cB1NG.Checked ? "True" : "False");
                iniFile.Write("BarcodeReader", "NgSave", option6, path);
                string option7 = (cBUVMode.Checked ? "True" : "False");
                iniFile.Write("Monitor", "UVModeChecked", option7, path);
                option7 = (cBPCD2TrayAVI.Checked ? "True" : "False");
                iniFile.Write("Monitor", "PCCD2TrayAVIChecked", option7, path);
                option7 = (cBPCD2Simpling.Checked ? "True" : "False");
                iniFile.Write("Monitor", "PCCD2Sampling", option7, path);

                string option13 = (cBGlue.Checked ? "True" : "False");
                iniFile.Write("GlueBarcodePara", "IsChecked", option13, path);
                iniFile.Write("GlueBarcodePara", "GlueTimeH", txtGlueH.Text, path);
                iniFile.Write("GlueBarcodePara", "GlueTimeM", txtGlueM.Text, path);
                option13 = (cBkeyEnter.Checked ? "True" : "False");
                iniFile.Write("GlueBarcodePara", "IsKeyEnter", option13, path);
                option2 = (cBGlueWeb.Checked ? "True" : "False");
                iniFile.Write("GlueWebServer", "Checked", option2, path);
                option2 = (cBGlueWeightWeb.Checked ? "True" : "False");
                iniFile.Write("GlueWeightWebServer", "Checked", option2, path);
                if (!cBGlue.Checked)
                {
                    Protocol.strPCRead_PCReady = true;   //胶水未选用时将信号置为ok
                    Protocol.IsPCRead = true;
                }
                MessageBox.Show("保存OK!");
            }
        }
        // Tray_BarcodeReader测试
        private void btnLBTrigger_MouseDown(object sender, MouseEventArgs e)
        {
            WriteToPlc.sBarcodeTrigger = "01WWRD00141,01,0001\r\n";
            WriteToPlc.bBarcodeTrigger = true;
        }
        private void btnLBTrigger_MouseUp(object sender, MouseEventArgs e)
        {
            WriteToPlc.sBarcodeTrigger = "01WWRD00141,01,0000\r\n";
            WriteToPlc.bBarcodeTrigger = true;
        }
        private void btnRBTrigger_MouseDown(object sender, MouseEventArgs e)
        {
            WriteToPlc.sRBarcodeTrigger = "01WWRD00142,01,0001\r\n";
            WriteToPlc.bRBarcodeTrigger = true;
        }
        private void btnRBTrigger_MouseUp(object sender, MouseEventArgs e)
        {
            WriteToPlc.sRBarcodeTrigger = "01WWRD00142,01,0001\r\n";
            WriteToPlc.bRBarcodeTrigger = true;
        }

        //胶水解绑可操作性人员参数
        private void btnNew_Click(object sender, EventArgs e)
        {
            string name = txtProduction.Text.Trim();
            if (name == "" && listProduction.Items.Count == 0)
            {
                MessageBox.Show("工号不能为空!"); txtProduction.Focus(); return;
            }
            if (name == "" && listProduction.Items.Count != 0)
                name = Glue.CurProductPair;
            if (Glue.AllProductPair.Contains(name))
            {
                MessageBox.Show("工号重复!"); txtProduction.Focus(); return;
            }
            fgui = new FrmGlueUserIn();
            DialogResult result = fgui.ShowDialog();
            if (result == DialogResult.OK)
            {
                int line = listProduction.Items.Count;
                Glue.AllProductPair.Add(name);
                if (Glue.AllProductPair.Count == 1)
                    Glue.CurProductPair = name;
                Glue.CurProductPair = name;
                string lname = (line + 1).ToString() + "." + name;
                Glue.AllProductPairlist.Add(lname);
                listProduction.Items.Add(lname);
                string curlist = ""; string AllProductPair = "";
                for (int i = 0; i < Glue.AllProductPair.Count; i++)
                {
                    if (Glue.CurProductPair == Glue.AllProductPair[i])
                        curlist = (i + 1).ToString() + "." + Glue.CurProductPair;
                    AllProductPair += Glue.AllProductPair[i];
                    if (i != Glue.AllProductPair.Count - 1)
                        AllProductPair += ',';
                }
                listProduction.SelectedItem = curlist;
                if (Sys.Factory == "XM")
                    iniFile.Write("CodeNumber", "XMUntieTotal", AllProductPair, glpath);
                if (Sys.Factory == "JM" || Sys.Factory == "TD")
                    iniFile.Write("CodeNumber", "JMUntieTotal", AllProductPair, glpath);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listProduction.SelectedIndex < 0)
            {
                MessageBox.Show("未选定任何工号!");
                return;
            }
            fgui = new FrmGlueUserIn();
            DialogResult result = fgui.ShowDialog();
            if (result == DialogResult.OK)
            {
                string cp = (string)listProduction.SelectedItem;
                string delcp = cp.Substring(cp.IndexOf(".") + 1, cp.Length - cp.IndexOf(".") - 1);
                Glue.AllProductPair.Remove(delcp);

                listProduction.Items.Clear();
                Glue.AllProductPairlist.Clear();
                for (int i = 0; i < Glue.AllProductPair.Count; ++i)
                {
                    string product = Glue.AllProductPair[i].Trim();
                    if (product != "")
                        Glue.AllProductPairlist.Add((i + 1).ToString() + "." + product);
                }
                listProduction.Items.AddRange(Glue.AllProductPairlist.ToArray());

                if (!Glue.AllProductPair.Contains(Glue.CurProductPair) && Glue.AllProductPair.Count > 0)
                    Glue.CurProductPair = Glue.AllProductPair[0];
                string curlist = ""; string AllProductPair = "";
                for (int i = 0; i < Glue.AllProductPair.Count; i++)
                {
                    if (Glue.CurProductPair == Glue.AllProductPair[i])
                        curlist = (i + 1).ToString() + "." + Glue.CurProductPair;
                    AllProductPair += Glue.AllProductPair[i];
                    if (i != Glue.AllProductPair.Count - 1)
                        AllProductPair += ',';
                }
                listProduction.SelectedItem = curlist;
                if (Sys.Factory == "XM")
                    iniFile.Write("CodeNumber", "XMUntieTotal", AllProductPair, glpath);
                if (Sys.Factory == "JM" || Sys.Factory == "TD")
                    iniFile.Write("CodeNumber", "JMUntieTotal", AllProductPair, glpath);
                MessageBox.Show("删除成功!");
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定要清空工号列表吗?", "",
                                                           MessageBoxButtons.OKCancel,
                                                           MessageBoxIcon.Information,
                                                           MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Cancel)
                return;
            if (dr == DialogResult.OK)
            {
                fgui = new FrmGlueUserIn();
                DialogResult result = fgui.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Glue.AllProductPair.Clear();
                    Glue.AllProductPairlist.Clear();
                    Glue.CurProductPair = "";
                    listProduction.Items.Clear();
                    if (Sys.Factory == "XM")
                        iniFile.Write("CodeNumber", "XMUntieTotal", "", glpath);
                    if (Sys.Factory == "JM" || Sys.Factory == "TD")
                        iniFile.Write("CodeNumber", "JMUntieTotal","", glpath);
                    MessageBox.Show("清空成功!");
                }
            }
        }

        private void tabControl12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl12.SelectedIndex == 3)
            {
                Glue.AllProductPair.Clear();
                Glue.AllProductPairlist.Clear();
                listProduction.Items.Clear();
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
                listProduction.Items.AddRange(Glue.AllProductPairlist.ToArray());
            }
        }        
        private void timerRefleshUI_Tick(object sender, EventArgs e)
        {
            if (Transmission.ChangeUI)
            {
                try
                {
                    txtLBarcode.Text = Reader.Barcode;
                    txtRBarcode.Text = RiReader.Barcode;
                    Transmission.ChangeUI = false;
                }
                catch
                {
                    //
                }
            }
        }
        void TextSPEC(object sender, KeyPressEventArgs e) //文本框格式设定
        {
            #region
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)//设置只允许输入  .  -  0~9  否则无效
                e.Handled = true;
            if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart != 0 || ((TextBox)sender).Text.IndexOf("-") >= 0))//输入为负号时，只能输入一次且只能输入一次
                e.Handled = true;
            if (e.KeyChar == 46)  //输入小数点时判断
            {
                if (((TextBox)sender).SelectionStart <= 0 || ((TextBox)sender).Text.IndexOf(".") >= 0) //第二位开始允许输入小数,且只能输入一次
                    e.Handled = true;
                if (((TextBox)sender).SelectionStart < ((TextBox)sender).Text.Length - 2) //限定小数点出现位置,防止出现三位以上小数的情况
                    e.Handled = true;
            }
            //限定只能输入两位小数 
            if (e.KeyChar != '\b' && (((TextBox)sender).SelectionStart) > (((TextBox)sender).Text.LastIndexOf('.')) + 3 && ((TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;
            //光标在小数点右侧时判断输入是否合规
            if (e.KeyChar != '\b' && ((TextBox)sender).SelectionStart >= (((TextBox)sender).Text.LastIndexOf('.')) && ((TextBox)sender).Text.IndexOf(".") >= 0)
            {
                if ((((TextBox)sender).SelectionStart) == (((TextBox)sender).Text.LastIndexOf('.')) + 1) //光标位于小数点右侧第一位时判断
                {
                    if ((((TextBox)sender).Text.Length).ToString() == (((TextBox)sender).Text.IndexOf(".") + 3).ToString())
                        e.Handled = true;
                }
                if ((((TextBox)sender).SelectionStart) == (((TextBox)sender).Text.LastIndexOf('.')) + 2) //光标位于小数点右侧第一位时判断
                {
                    if ((((TextBox)sender).Text.Length).ToString() == (((TextBox)sender).Text.IndexOf(".") + 4).ToString())
                        e.Handled = true;
                }
                if ((((TextBox)sender).SelectionStart) == (((TextBox)sender).Text.LastIndexOf('.')) + 3) //光标位于小数点右侧第一位时判断
                {
                    if ((((TextBox)sender).Text.Length - 4).ToString() == ((TextBox)sender).Text.IndexOf(".").ToString())
                        e.Handled = true;
                }
            }
            if (((TextBox)sender).TextLength > 5 && e.KeyChar != '\b')
                e.Handled = true;
            #endregion
        }
        //测高
        private void GT21check_CheckedChanged(object sender, EventArgs e)
        {
            GT2.CH1IsCheck = GT21check.Checked;
            iniFile.Write("GT2", "CH1Checked", GT21check.Checked.ToString(), path);
        }
        private void GT22check_CheckedChanged(object sender, EventArgs e)
        {
            GT2.CH2IsCheck = GT22check.Checked;
            iniFile.Write("GT2", "CH2Checked", GT22check.Checked.ToString(), path);
        }
        private void GT23check_CheckedChanged(object sender, EventArgs e)
        {
            GT2.CH3IsCheck = GT23check.Checked;
            iniFile.Write("GT2", "CH3Checked", GT23check.Checked.ToString(), path);
        }
        private void GT24check_CheckedChanged(object sender, EventArgs e)
        {
            GT2.CH4IsCheck = GT24check.Checked;
            iniFile.Write("GT2", "CH4Checked", GT24check.Checked.ToString(), path);
        }
        private void GT25check_CheckedChanged(object sender, EventArgs e)
        {
            GT2.CH5IsCheck = GT25check.Checked;
            iniFile.Write("GT2", "CH5Checked", GT25check.Checked.ToString(), path);
        }
        private void btnGT21Clear_Click(object sender, EventArgs e)
        {
            GT2.clearCH = true;
            GT2.clearCHcmd = Encoding.ASCII.GetBytes("SW,01,001,+000000001\r\n");
            //GT2Clear(GT2.clearCH1cmd);
        }
        private void btnGT22Clear_Click(object sender, EventArgs e)
        {
            GT2.clearCH = true;
            GT2.clearCHcmd = Encoding.ASCII.GetBytes("SW,02,001,+000000001\r\n");
            //GT2Clear(GT2.clearCH2cmd);
        }
        private void btnGT23Clear_Click(object sender, EventArgs e)
        {
            GT2.clearCH = true;
            GT2.clearCHcmd = Encoding.ASCII.GetBytes("SW,03,001,+000000001\r\n");
            //GT2Clear(GT2.clearCH3cmd);
        }
        private void btnGT24Clear_Click(object sender, EventArgs e)
        {
            GT2.clearCH = true;
            GT2.clearCHcmd = Encoding.ASCII.GetBytes("SW,04,001,+000000001\r\n");
            //GT2Clear(GT2.clearCH4cmd);
        }
        private void btnGT25Clear_Click(object sender, EventArgs e)
        {
            GT2.clearCH = true;
            GT2.clearCHcmd = Encoding.ASCII.GetBytes("SW,05,001,+000000001\r\n");
            //GT2Clear(GT2.clearCH5cmd);
        }
        void GT2Clear( byte[] cmd)
        {
            GT2.clearCH = false;
            if (GT2.IsConnected)
            {
                GT2.socket.Send(cmd);  //写入清零指令,读取测高器原始值
                Thread.Sleep(50);
                byte[] recv1 = new byte[4096];
                int len1 = GT2.socket.Receive(recv1);
                if (len1 == 0)
                    return;
                string height1 = Encoding.ASCII.GetString(recv1, 0, len1);
            }
        }
        //点胶控制器
        private void btnParamSet_Click(object sender, EventArgs e)
        {
            if (!GlueCU.IsConnected || !GlueCU.IsCheck)
            {
                MessageBox.Show("点胶控制器未选用或点胶控制器未连接，请确认！");
                return;
            }
            DialogResult dr = MessageBox.Show("确定要修改点胶控制器的参数吗？", "",
                                                    MessageBoxButtons.OKCancel,
                                                    MessageBoxIcon.Information,
                                                    MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.OK)
            {
                GlueCU.sendTime = null;
                GlueCU.sendPressure1 = null;
                GlueCU.sendPressure2 = null;
                GlueCU.sendChanleNumber = cbGlueCUUsingCh.SelectedIndex + 1;
                for (int i = 0; i < GlueCU.sendChanleNumber; i++)
                {
                    GlueCU.sendTime += "," + Convert.ToInt32(GlueCU.Time_array[i].Value * 1000);//获取要更改时间参数
                    GlueCU.sendPressure1 += "," + Convert.ToInt32(GlueCU.Pressure_array1[i].Value * 10); //获取要更改压力参数
                    GlueCU.sendPressure2 += "," + Convert.ToInt32(GlueCU.Pressure_array2[i].Value * 10);
                }

                GlueCU.sendTime = GlueCU.sendTime.Substring(1);  //去掉第一个，
                GlueCU.sendPressure1 = GlueCU.sendPressure1.Substring(1);
                GlueCU.sendPressure2 = GlueCU.sendPressure2.Substring(1);
                //关闭控件
                foreach (Control controlPanel in groupBox2.Controls)
                {
                    Panel panel = controlPanel as Panel;
                    if (panel == null) continue;
                    EnableClass.SetControlEnabled(panel, false);
                }
                GlueCU.IsSendPara = true;
                iniFile.Write("GlueCU", "ChanleNumber", GlueCU.sendChanleNumber.ToString(), path);
            }
        }
        private void cbGlueCUUsingCh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!readpara & (!GlueCU.IsConnected || !GlueCU.IsCheck))
            {
                MessageBox.Show("点胶控制器未选用或点胶控制器未连接，请确认！");
                return;
            }
            if (cbGlueCUUsingCh.SelectedIndex >= 0)
            {
                foreach (Control controlPanel in groupBox2.Controls)
                {
                    Panel panel = controlPanel as Panel;
                    if (panel == null) continue;
                    if (Convert.ToInt32(panel.Tag) <= cbGlueCUUsingCh.SelectedIndex)
                        panel.Visible = true;
                    else
                        panel.Visible = false;
                }
                GlueCU.sendChanleNumber = cbGlueCUUsingCh.SelectedIndex + 1;
                for (int i = 0; i < GlueCU.sendChanleNumber; i++)
                {
                    gCUlable[i].Show();
                    GlueCU.Time_array[i].Show();
                    GlueCU.Pressure_array1[i].Show();
                    GlueCU.Pressure_array2[i].Show();
                    lblMinus[i].Show();
                    GlueCU.Time_array[i].Value = decimal.Parse(GlueCU.arrayTime[i].ToString());
                    GlueCU.Pressure_array1[i].Value = decimal.Parse(GlueCU.arrayPressure[i].ToString());
                    GlueCU.Pressure_array2[i].Value = decimal.Parse(GlueCU.arrayPressure[10 + i].ToString());
                }
                for (int i = GlueCU.sendChanleNumber; i < 10; i++)
                {
                    gCUlable[i].Hide();
                    GlueCU.Time_array[i].Hide();
                    GlueCU.Pressure_array1[i].Hide();
                    GlueCU.Pressure_array2[i].Hide();
                    lblMinus[i].Hide();
                }
                btnParamSet.Focus();
            }
        }
        private void btnParamSet808_Click(object sender, EventArgs e)
        {
            if (!GlueCU.IsConnected || !GlueCU.IsCheck)
            {
                MessageBox.Show("点胶控制器未选用或点胶控制器未连接，请确认！");
                return;
            }
            DialogResult dr = MessageBox.Show("确定要修改点胶控制器的参数吗？", "",
                                                    MessageBoxButtons.OKCancel,
                                                    MessageBoxIcon.Information,
                                                    MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.OK)
            {
                GlueCU.sendTime808[0] = nu808dT1.Text;
                GlueCU.sendPressure808[0] = nu808dP1.Text;
                GlueCU.sendTime808[1] = nu808dT2.Text;
                GlueCU.sendPressure808[1] = nu808dP2.Text;
                GlueCU.IsSendPara808 = true;
            }
        }

        private void cbWeb_Tray2InTray1Out_InOutStation_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWeb_Tray2InTray1Out_InOutStation.Checked)
            {
                if (!cbWeb_Tray_InOutStation.Checked)
                {
                    MessageBox.Show("PZ0进出站过账未開啟!");
                    cbWeb_Tray2InTray1Out_InOutStation.Checked = false;
                    return;
                }
                if (!cbWeb_Tray_InOutStation.Checked)
                {
                    MessageBox.Show("Tray2选用未開啟!");
                    cbWeb_Tray2InTray1Out_InOutStation.Checked = false;
                    return;
                }
            }
        }

       
    }
    public class ListItem
    {
        private string id = string.Empty;
        private string name = string.Empty;
        public ListItem(string sid, string sname)
        {
            id = sid;
            name = sname;
        }
        public override string ToString()
        {
            return this.name;
        }
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}
