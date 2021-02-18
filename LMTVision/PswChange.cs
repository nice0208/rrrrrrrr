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
    public partial class PswChange : Form
    {
        public PswChange()
        {
            InitializeComponent();
        }

        string path = Sys.IniPath + "\\Users.ini";

        private void PswChange_Load(object sender, EventArgs e)
        {
            string totalUsers = iniFile.Read("Users", "Total", path);
            User.Total.Clear();
            cmbUsers2.Items.Clear();
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
                cmbUsers2.Items.AddRange(Users);
                cmbUsers2.SelectedIndex = 0;
            }
        }

        private void btnModifi_Click(object sender, EventArgs e)
        {
            string newPsw = txtNewPsw.Text;
            string againPsw = txtAgainPsw.Text;
            if (cmbUsers2.SelectedIndex < 0)
            {
                return;
            }
            string user = (string)cmbUsers2.SelectedItem;
            string OldPassword = "";
            if (User.Total.ContainsKey(user))
            {
                OldPassword = User.Total[user];
            }
            if (txtOldPsw.Text != OldPassword)
            {
                MessageBox.Show("密码输入有误!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOldPsw.Clear();
                txtOldPsw.Focus();
                return;
            }
            if (txtNewPsw.Text.Trim().Length < 6)
            {
                MessageBox.Show("密码长度不能低于6位!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPsw.Clear();
                txtNewPsw.Focus();
                return;
            }
            if (againPsw != newPsw)
            {
                MessageBox.Show("确认密码出错，请重新输入！");
                return;
            }
            iniFile.Write(user, "PassWord", txtNewPsw.Text.Trim(), path);
            User.Total[user] = txtNewPsw.Text.Trim();
            txtOldPsw.Clear();
            txtNewPsw.Clear();
            MessageBox.Show("修改OK!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FrmLogin Form = new FrmLogin();
            Form.Show();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
