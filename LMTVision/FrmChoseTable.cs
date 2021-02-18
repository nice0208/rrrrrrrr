using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace LMTVision
{
    public partial class FrmChoseTable : Form
    {
        public FrmChoseTable()
        {
            InitializeComponent();
            //this.TopMost = true;
            this.KeyPreview = true;
        }
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private void rBtnO3_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnO3.Checked)
            {
                lblShow.Text = " O3-首次换料 ";
                Sys.CurErrNum = "0";
            }
        }
        private void rBtnO4_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnO4.Checked)
            {
                lblShow.Text = "  O4-换料  ";
                Sys.CurErrNum = "1";
            }
        }
        private void rBtnO5_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnO5.Checked)
            {
                lblShow.Text = " O5-换胶 ";
                Sys.CurErrNum = "2";
            }
        }

        private void rBtnC1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnC1.Checked)
            {
                lblShow.Text = "C1-设备异常首件";
                Sys.CurErrNum = "3";
            }
        }
        private void rBtnC2_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnC2.Checked)
            {
                lblShow.Text = "C2-制程异常首件";
                Sys.CurErrNum = "4";
            }
        }
        private void rBtnC4_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnC4.Checked)
            {
                lblShow.Text = " C4-换料首件 ";
                Sys.CurErrNum = "5";
            }
        }
        private void rBtnC5_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnC5.Checked)
            {
                lblShow.Text = " C5-开机首件 ";
                Sys.CurErrNum = "6";
            }
        }
        private void rBtnB6_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnB6.Checked)
            {
                lblShow.Text = " B6-保养首件 ";
                Sys.CurErrNum = "7";
            }
        }
        private void rBtnC7_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnC7.Checked)
            {
                lblShow.Text = " C7-称胶重 ";
                Sys.CurErrNum = "8";
            }
        }
        
        private void rBtnE2_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnE2.Checked)
            {
                lblShow.Text = " E2-制程异常 ";
                Sys.CurErrNum = "9";
            }
        }
        private void rBtnE3_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnE3.Checked)
            {
                lblShow.Text = " E3-物料异常 ";
                Sys.CurErrNum = "10";
            }
        }
        private void rBtnE4_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnE4.Checked)
            {
                lblShow.Text = "  E4-待料  ";
                Sys.CurErrNum = "11";
            }
        }

        private void rBtnD1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnD1.Checked)
            {
                lblShow.Text = " D1-切换机种 ";
                Sys.CurErrNum = "12";
            }
        }

        private void rBtnE1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnE1.Checked)
            {
                lblShow.Text = " E1-设备故障 ";
                Sys.CurErrNum = "13";
            }
        }

        private void rBtnB1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnB1.Checked)
            {
                lblShow.Text = "  B1-日保养  ";
                Sys.CurErrNum = "14";
            }
        }
        private void rBtnB3_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnB3.Checked)
            {
                lblShow.Text = "  B3-月保养  ";
                Sys.CurErrNum = "15";
            }
        }
        private void rBtnB4_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnB4.Checked)
            {
                lblShow.Text = "  B4-季保养  ";
                Sys.CurErrNum = "16";
            }
        }

        private void rBtnF1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnF1.Checked)
            {
                lblShow.Text = " F1-制程DOE ";
                Sys.CurErrNum = "17";
            }
        }
        private void rBtnF3_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnF3.Checked)
            {
                lblShow.Text = " F3-委托物料 ";
                Sys.CurErrNum = "18";
            }
        }

        private void rBtnE5_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnE5.Checked)
            {
                lblShow.Text = " E5-厂务异常 ";
                Sys.CurErrNum = "19";
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (lblShow.Text == "")
            {
                MessageBox.Show("请选择停机选项！");
                return;
            }
            Sys.CurErrMessage = lblShow.Text;
            iniFile.Write("OEE", "ErrNum", Sys.CurErrNum, Sys.IniPath + "\\System.ini");
            this.DialogResult = DialogResult.OK;
            Sys.FCTShow = false;
            this.Close();
        }

        private void FrmChoseTable_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar != 48)
            //{
            //    MessageBox.Show(e.KeyChar.ToString());
            //}
        }

        private void FrmChoseTable_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.LWin || (e.KeyCode == Keys.LWin & e.KeyCode == Keys.D))
            //{
            //    //keybd_event(Keys.LWin, 0, 0, 0);
            //    Thread.Sleep(50);
            //    SendKeys.Send("^{ESC}");
            //}
        }

        private void FrmChoseTable_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            Sys.FCTShow = true;
        }

    }
}
