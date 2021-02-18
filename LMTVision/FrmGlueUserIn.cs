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
    public partial class FrmGlueUserIn : Form
    {
        public FrmGlueUserIn()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string totalUsers = "GSEO915824123";
            if (totalUsers == txtPwd.Text)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                DialogResult dr = MessageBox.Show("用户名或密码错误,请重新输入！", "",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button2);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            this.Close();
        }
    }
}
