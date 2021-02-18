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
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }
        FrmMain parent = new FrmMain();
        string path = Sys.IniPath + "\\Users.ini";
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            string totalUsers = iniFile.Read("Users", "Total", path);
            if (totalUsers == "")
            {
                iniFile.Write("Users", "Total", "Admin,白班,夜班", path);
                iniFile.Write("Admin", "PassWord", "667807", path);
                iniFile.Write("白班", "PassWord", "123456", path);
                iniFile.Write("夜班", "PassWord", "123456", path);
            }
            totalUsers = iniFile.Read("Users", "Total", path);
            User.Total.Clear();
            cmbUsers.Items.Clear();
            string[] Users = totalUsers.Split(',');
            if (Users.Length > 0)
            {
                string[] PassWord = new string[Users.Length];
                for (int i = 0; i < Users.Length; ++i)
                {
                    PassWord[i] = iniFile.Read(Users[i], "PassWord", path);
                    User.Total.Add(Users[i], PassWord[i]);
                }
                if (!User.Total.ContainsKey("Admin"))
                    User.Total.Add("Admin", "667807");
                cmbUsers.Items.AddRange(Users);
                cmbUsers.SelectedIndex = 0;
            }
            this.AcceptButton = btnLogin;
        }

        private void cmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPwd.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (cmbUsers.SelectedIndex < 0)
            {
                return;
            }
            if (User.Total[(string)cmbUsers.SelectedItem] == txtPwd.Text)
            {
                User.CurrentUser = (string)cmbUsers.SelectedItem;
                parent.lblUser.Text = User.CurrentUser;
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
            txtPwd.Clear();
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
    }
}
