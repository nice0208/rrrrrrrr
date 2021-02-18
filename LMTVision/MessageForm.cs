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
    public partial class MessageForm : Form
    {
        int t;
        string txt;
        /// <summary>
        /// 自定义弹窗
        /// </summary>
        /// <param name="time">窗体消失时间</param>
        /// <param name="text">窗体提示内容</param>
        public MessageForm(int time,string text)
        {
            t = time;
            this.TopMost = true;
            InitializeComponent();
            txt = text;
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            timer1.Interval = t;
            timer1.Enabled = true;
            lblMessage.Text = txt;
            //DialogResult dr = MessageBox.Show(this, txt, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //if (dr == DialogResult.OK)
            //    this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
