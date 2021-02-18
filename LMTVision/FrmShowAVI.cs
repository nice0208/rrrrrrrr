using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LMTVision
{
    public partial class FrmShowAVI : Form
    {
        public FrmShowAVI()
        {
            InitializeComponent();
        }
        string path = Sys.IniPath + "\\ComPortPara.ini";   //设备各参数路径
        private void FrmAVIshow_Load(object sender, EventArgs e)
        {
            ReleaseLabelsshow();
            GenerateLabelsshow();
            for (int i = 0; i < 49; i++)
            {
                string mcolor = iniFile.Read("Table", i.ToString() + "Result", path);
                if (mcolor == "1")
                    Sys.LabelsTray[i].BackColor = Color.Green;
                if (mcolor == "2")
                    Sys.LabelsTray[i].BackColor = Color.Red;
                if (mcolor == "3")
                    Sys.LabelsTray[i].BackColor = Color.DarkRed;
            }
            timer1.Enabled = true;
        }
        private List<Label> LabelsTrayshow = new List<Label>();
        public void GenerateLabelsshow()//生成Labels（映射Tray上面的Lens）
        {
            int Trayrowsshow = 7;
            int Traycolsshow = 7;
            int Trayheightshow = (pnlTrayshow.Height) / Trayrowsshow;
            int Traywidthshow = (pnlTrayshow.Width) / Traycolsshow;
            for (int i = 0; i < Trayrowsshow; ++i)
            {
                for (int j = 0; j < Traycolsshow; ++j)
                {
                    Label lblTrayshow = new Label();
                    lblTrayshow.AutoSize = false;
                    lblTrayshow.BackColor = Color.Gray;
                    lblTrayshow.Size = new System.Drawing.Size(Traywidthshow - 1, Trayheightshow - 1);
                    lblTrayshow.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    lblTrayshow.Location = new Point(Traywidthshow * j, Trayheightshow * i);
                    lblTrayshow.ForeColor = Color.Black;
                    lblTrayshow.TextAlign = ContentAlignment.MiddleCenter;
                    //lblTray.Font = label5.Font;
                    //lblTray.Click += LabelClick;
                    pnlTrayshow.Controls.Add(lblTrayshow);
                    LabelsTrayshow.Add(lblTrayshow);
                }
            }
        }
        public void ReleaseLabelsshow()//释放Labels
        {
            for (int i = 0; i < LabelsTrayshow.Count; ++i)
            {
                LabelsTrayshow[i].Dispose();
            }
            LabelsTrayshow.Clear();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lblBarcode.Text != RiReader.Barcode)
                lblBarcode.Text = RiReader.Barcode;
            if (PLC.allstatus[14])  //满盘信号
            {
                ReleaseLabelsshow();
                GenerateLabelsshow();
            }
            for (int i = 0; i < 49; i++)
            {
                if (LabelsTrayshow[i].BackColor != Sys.LabelsTray[i].BackColor)
                    LabelsTrayshow[i].BackColor = Sys.LabelsTray[i].BackColor;
            }
        }

        private void FrmAVIshow_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
        }

    }
}
