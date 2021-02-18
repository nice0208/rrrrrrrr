using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using HalconDotNet;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections.Generic;

namespace LMTVision
{
    public partial class FrmDisplay2 : Form
    {
        string path = Sys.IniPath + "\\ComPortPara.ini";   //设备各参数路径

        public FrmDisplay2()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.ICO2; //在Resources中添加图标后直接调用
            //this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);//共用同一标识
        }

        private void FrmDisplay2_Load(object sender, EventArgs e)
        {
            GlueCU.ShowTime_array = new Label[10] { lbldT1, lbldT2, lbldT3, lbldT4, lbldT5, lbldT6, lbldT7, lbldT8, lbldT9, lbldT10 };
            GlueCU.ShowPressure_array1 = new Label[10] { lbldP1, lbldP2, lbldP3, lbldP4, lbldP5, lbldP6, lbldP7, lbldP8, lbldP9, lbldP10 };
            GlueCU.ShowPressure_array2 = new Label[10] { lbldP11, lbldP12, lbldP13, lbldP14, lbldP15, lbldP16, lbldP17, lbldP18, lbldP19, lbldP20 };
            lblGlueCUName.Text = (GlueCU.choice == 1 ? "ML-808GX" : "GS-2.0");
            if (GlueCU.choice == 1)
            {
                label29.Hide();
                for (int i = 0; i < 10; i++)
                {
                    GlueCU.ShowPressure_array2[i].Hide();
                }
                for (int i = 2; i < 10; i++)
                {
                    GlueCU.ShowTime_array[i].Hide();
                    GlueCU.ShowPressure_array1[i].Hide();
                }
            }
            #region CCD show
            if (A1CCD1.IsCheck)
            {
                A11ConnStatus.Show(); label55.Show();
            }
            else
            {
                A11ConnStatus.Hide(); label55.Hide();
            }
            if (A1CCD2.IsCheck)
            {
                A12ConnStatus.Show(); label14.Show();
            }
            else
            {
                A12ConnStatus.Hide(); label14.Hide();
            }
            if (A2CCD1.IsCheck)
            {
                A21ConnStatus.Show(); label17.Show();
            }
            else
            {
                A21ConnStatus.Hide(); label17.Hide();
            }
            if (A2CCD2.IsCheck)
            {
                A22ConnStatus.Show(); label6.Show();
            }
            else
            {
                A22ConnStatus.Hide(); label6.Hide();
            }
            if (PCCD1.IsCheck)
            {
                P1ConnStatus.Show(); label4.Show();
            }
            else
            {
                P1ConnStatus.Hide(); label4.Hide();
            }
            if (PCCD2.IsCheck)
            {
                P2ConnStatus.Show(); label7.Show();
            }
            else
            {
                P2ConnStatus.Hide(); label7.Hide();
            }
            if (GCCD1.IsCheck)
            {
                G1ConnStatus.Show(); label5.Show();
            }
            else
            {
                G1ConnStatus.Hide(); label5.Hide();
            }
            if (GCCD2.IsCheck)
            {
                G2ConnStatus.Show(); label8.Show();
            }
            else
            {
                G2ConnStatus.Hide(); label8.Hide();
            }
            if (QCCD.IsCheck)
            {
                QConnStatus.Show(); label9.Show();
            }
            else
            {
                QConnStatus.Hide(); label9.Hide();
            }
            if (Barcode1.IsChecked)
            {
                QRC1ConnStatus.Show(); label12.Show();
            }
            else
            {
                QRC1ConnStatus.Hide(); label12.Hide();
            }
            #endregion
            GT2CHShow();
            ReleaseLabels();
            GenerateLabels();
            for (int i = 0; i < PLC.PZ0TrayMax_Y * PLC.PZ0TrayMax_X; i++)
            {
                string mcolor = iniFile.Read("Table", i.ToString() + "Result", path);
                switch (mcolor)
                {
                    case "1": Sys.LabelsTray[i].BackColor = Color.Green; break;
                    case "2": Sys.LabelsTray[i].BackColor = Color.Red; break;
                    case "3": Sys.LabelsTray[i].BackColor = Color.DarkRed; break;
                }
            }
            timerRefleshUI.Enabled = true;
        }
        
        #region 窗体移动
        private bool isMouseDown = false;
        private Point FromLocation;
        private Point mouseOffset;
        private void FrmDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                FromLocation = this.Location;
                mouseOffset = Control.MousePosition;
            }
        }
        private void FrmDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        private void FrmDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            int _x = 0;
            int _y = 0;
            if (isMouseDown)
            {
                Point pt = Control.MousePosition;
                _x = mouseOffset.X - pt.X;
                _y = mouseOffset.Y - pt.Y;
                this.Location = new Point(FromLocation.X - _x, FromLocation.Y - _y);
            }
        }
        #endregion

        public void GT2CHShow()
        {
            #region GT2CH
            if (GT2.CH1IsCheck)
            {
                lblCH1.Show(); lblGT2RV1.Show();
            }
            else
            {
                lblCH1.Hide(); lblGT2RV1.Hide();
            }
            if (GT2.CH2IsCheck)
            {
                lblCH2.Show(); lblGT2RV2.Show();
            }
            else
            {
                lblCH2.Hide(); lblGT2RV2.Hide();
            }
            if (GT2.CH3IsCheck)
            {
                lblCH3.Show(); lblGT2RV3.Show();
            }
            else
            {
                lblCH3.Hide(); lblGT2RV3.Hide();
            }
            if (GT2.CH4IsCheck)
            {
                lblCH4.Show(); lblGT2RV4.Show();
            }
            else
            {
                lblCH4.Hide(); lblGT2RV4.Hide();
            }
            if (GT2.CH5IsCheck)
            {
                lblCH5.Show(); lblGT2RV5.Show();
            }
            else
            {
                lblCH5.Hide(); lblGT2RV5.Hide();
            }
            #endregion
        }
        public void GenerateLabels()//生成Labels（映射Tray上面的Lens）
        {
            int Trayrows = PLC.PZ0TrayMax_Y;
            int Traycols = PLC.PZ0TrayMax_X;
            int Trayheight = (pnlTray.Height) / Trayrows;
            int Traywidth = (pnlTray.Width) / Traycols;
            for (int i = 0; i < Trayrows; ++i)
            {
                for (int j = 0; j < Traycols; ++j)
                {
                    Label lblTray = new Label();
                    lblTray.AutoSize = false;
                    lblTray.BackColor = Color.Gray;
                    lblTray.Size = new System.Drawing.Size(Traywidth - 1, Trayheight - 1);
                    lblTray.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    lblTray.Location = new Point(Traywidth * j, Trayheight * i);
                    lblTray.ForeColor = Color.Black;
                    lblTray.TextAlign = ContentAlignment.MiddleCenter;
                    lblTray.Font = label5.Font;
                    this.Invoke(new MethodInvoker(delegate
                            {
                                pnlTray.Controls.Add(lblTray);
                                Sys.LabelsTray.Add(lblTray);
                            }));
                }
            }
        }
        public void ReleaseLabels()//释放Labels
        {
            if (Sys.LabelsTray.Count == 0)
                return;
            for (int i = 0; i < PLC.PZ0TrayMax_Y * PLC.PZ0TrayMax_X; ++i)
            {
                this.Invoke(new MethodInvoker(delegate
                            {
                                Sys.LabelsTray[i].Dispose();
                            }));
                iniFile.Write("Table", i.ToString() + "Result", "", path);
            }
            Sys.LabelsTray.Clear();
        }
        bool gt2ch1 = false, gt2ch2 = false, gt2ch3 = false, gt2ch4 = false, gt2ch5 = false;
        private void timerRefleshUI_Tick(object sender, EventArgs e)
        {
            lblP1Num.Text = (FrmMain.processing[4] ? "1" : "0");
            lblP2Num.Text = PCCD2.IntSingle.ToString();
            lblG1Num.Text = (FrmMain.processing[6] ? "1" : "0");
            lblG2Num.Text = GCCD2.IntPosition.ToString() + "." + GCCD2.IntSingle.ToString();
            lblQNum.Text = (FrmMain.processing[8] ? "1" : "0");
            if (lblQTime.Text != QCCD.QTime)
                lblQTime.Text = QCCD.QTime;
            if (labBarTime.Text != FrmMain.bTspan)
                labBarTime.Text = FrmMain.bTspan;
            if (lblBarID.Text != Sys.CurrentProID)
                lblBarID.Text = Sys.CurrentProID;
            #region Barcode
            Barcode1.bColor = (Barcode1.IsConnected ? Color.Green : Color.Red);
            if (Barcode1.IsChecked & QRC1ConnStatus.BackColor != Barcode1.bColor)
                QRC1ConnStatus.BackColor = Barcode1.bColor;
            #endregion
            #region 测高
            if (GT2.IsConnected)
            {
                if (lblGT2RV1.Text != FrmMain.RealHight[0])
                    lblGT2RV1.Text = FrmMain.RealHight[0];
                if (lblGT2RV2.Text != FrmMain.RealHight[1])
                    lblGT2RV2.Text = FrmMain.RealHight[1];
                if (lblGT2RV3.Text != FrmMain.RealHight[2])
                    lblGT2RV3.Text = FrmMain.RealHight[2];
                if (lblGT2RV4.Text != FrmMain.RealHight[3])
                    lblGT2RV4.Text = FrmMain.RealHight[3];
                if (lblGT2RV5.Text != FrmMain.RealHight[4])
                    lblGT2RV5.Text = FrmMain.RealHight[4];
            }
            GT2.gColor = (GT2.IsConnected ? Color.Green : Color.Red);
            if (GT2Status.BackColor != GT2.gColor)
                GT2Status.BackColor = GT2.gColor;
            if ((GT2.CH1IsCheck & !gt2ch1) || (GT2.CH2IsCheck & !gt2ch2) || (GT2.CH3IsCheck & !gt2ch3) || (GT2.CH4IsCheck & !gt2ch4) || (GT2.CH5IsCheck & !gt2ch5) ||
                (!GT2.CH1IsCheck & gt2ch1) || (!GT2.CH2IsCheck & gt2ch2) || (!GT2.CH3IsCheck & gt2ch3) || (!GT2.CH4IsCheck & gt2ch4) || (!GT2.CH5IsCheck & gt2ch5))
                GT2CHShow();
            gt2ch1 = GT2.CH1IsCheck;
            gt2ch2 = GT2.CH2IsCheck;
            gt2ch3 = GT2.CH3IsCheck;
            gt2ch4 = GT2.CH4IsCheck;
            gt2ch5 = GT2.CH5IsCheck;
            #endregion
            #region 点胶控制器
            GlueCU.gColor = (GlueCU.IsConnected ? Color.Green : Color.Red);
            if (GlueCUConnStatus.BackColor != GlueCU.gColor)
                GlueCUConnStatus.BackColor = GlueCU.gColor;
            switch (GlueCU.choice)
            {
                case 0:
                    #region GS-2.0
                    if (GlueCU.ChanleNumber != GlueCU.LChanleNumber)
                    {
                        for (int i = 0; i < GlueCU.ChanleNumber; i++)
                        {
                            GlueCU.ShowTime_array[i].Show();
                            GlueCU.ShowPressure_array1[i].Show();
                            GlueCU.ShowPressure_array2[i].Show();
                        }
                        for (int i = GlueCU.ChanleNumber; i < 10; i++)
                        {
                            GlueCU.ShowTime_array[i].Hide();
                            GlueCU.ShowPressure_array1[i].Hide();
                            GlueCU.ShowPressure_array2[i].Hide();
                        }
                    }
                    GlueCU.LChanleNumber = GlueCU.ChanleNumber;
                    for (int i = 0; i < GlueCU.ChanleNumber; i++)
                    {
                        if (GlueCU.ShowTime_array[i].Text != GlueCU.arrayTime[i].ToString("0.000"))
                            GlueCU.ShowTime_array[i].Text = GlueCU.arrayTime[i].ToString("0.000");
                        if (GlueCU.ShowPressure_array1[i].Text != GlueCU.arrayPressure[i].ToString("0.0"))
                            GlueCU.ShowPressure_array1[i].Text = GlueCU.arrayPressure[i].ToString("0.0");
                        if (GlueCU.ShowPressure_array2[i].Text != "-" + GlueCU.arrayPressure[10 + i].ToString("0.0"))
                            GlueCU.ShowPressure_array2[i].Text = "-" + GlueCU.arrayPressure[10 + i].ToString("0.0");
                    }
                    #endregion
                    break;
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        if (GlueCU.ShowTime_array[i].Text != GlueCU.arrayTime[i].ToString("0.000"))
                            GlueCU.ShowTime_array[i].Text = GlueCU.arrayTime[i].ToString("0.000");
                        if (GlueCU.ShowPressure_array1[i].Text != GlueCU.arrayPressure[i].ToString("0.0"))
                            GlueCU.ShowPressure_array1[i].Text = GlueCU.arrayPressure[i].ToString("0.0");
                    }  
                    break;
            }
            #endregion
            #region PLC
            PLC.pColor = (PLC.IsConnected ? Color.Green : Color.Red);
            if (PlcConnStatus.BackColor != PLC.pColor)
                PlcConnStatus.BackColor = PLC.pColor;
            #endregion
            #region CCD
            A1CCD1.color = (A1CCD1.IsConnected ? Color.Green : Color.Red);
            if (A11ConnStatus.BackColor != A1CCD1.color)
                A11ConnStatus.BackColor = A1CCD1.color;
            A1CCD2.color = (A1CCD2.IsConnected ? Color.Green : Color.Red);
            if (A12ConnStatus.BackColor != A1CCD2.color)
                A12ConnStatus.BackColor = A1CCD2.color;
            A2CCD1.color = (A2CCD1.IsConnected ? Color.Green : Color.Red);
            if (A21ConnStatus.BackColor != A2CCD1.color)
                A21ConnStatus.BackColor = A2CCD1.color;
            A2CCD2.color = (A2CCD2.IsConnected ? Color.Green : Color.Red);
            if (A22ConnStatus.BackColor != A2CCD2.color)
                A22ConnStatus.BackColor = A2CCD2.color;
            PCCD1.color = (PCCD1.IsConnected ? Color.Green : Color.Red);
            if (P1ConnStatus.BackColor != PCCD1.color)
                P1ConnStatus.BackColor = PCCD1.color;
            PCCD2.color = (PCCD2.IsConnected ? Color.Green : Color.Red);
            if (P2ConnStatus.BackColor != PCCD2.color)
                P2ConnStatus.BackColor = PCCD2.color;
            GCCD1.color = (GCCD1.IsConnected ? Color.Green : Color.Red);
            if (G1ConnStatus.BackColor != GCCD1.color)
                G1ConnStatus.BackColor = GCCD1.color;
            GCCD2.color = (GCCD2.IsConnected ? Color.Green : Color.Red);
            if (G2ConnStatus.BackColor != GCCD2.color)
                G2ConnStatus.BackColor = GCCD2.color;
            QCCD.color = (QCCD.IsConnected ? Color.Green : Color.Red);
            if (QConnStatus.BackColor != QCCD.color)
                QConnStatus.BackColor = QCCD.color;
            #endregion
            #region Light
            LIGHT1.lColor = (LIGHT1.IsConnected ? Color.Green : Color.Red);
            if (L1ConnStatus.BackColor != LIGHT1.lColor)
                L1ConnStatus.BackColor = LIGHT1.lColor;
            LIGHT2.lColor = (LIGHT2.IsConnected ? Color.Green : Color.Red);
            if (L2ConnStatus.BackColor != LIGHT2.lColor)
                L2ConnStatus.BackColor = LIGHT2.lColor;
            #endregion

        }

   
    }
}
