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
    public partial class FrmSetLogin : Form
    {
        public FrmSetLogin()
        {
            InitializeComponent();
        }
        FrmMain parent = new FrmMain();
        string path = Sys.IniPath + "\\SetParam.ini";
        string totalUsers = "";
        string[] Users = new string[2] { "", "" };

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            totalUsers = iniFile.Read("CodeNumber", "Total", path);
            if (totalUsers == "")
                iniFile.Write("CodeNumber", "Total", "ch667807", path);
            Users[0] = "Administrator";
            cmbUsers.Items.AddRange(Users);
            cmbUsers.SelectedIndex = 0;
        }

        private void cmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPwd.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            totalUsers = iniFile.Read("CodeNumber", "Total", path);
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

        private void btnPwdChange_Click(object sender, EventArgs e)
        {
            PswChange from = new PswChange();
            from.ShowDialog();
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            totalUsers = iniFile.Read("CodeNumber", "Total", path);
            if (txtPwd.Text == totalUsers)
                btnLogin.Focus();
        }
    }
}
