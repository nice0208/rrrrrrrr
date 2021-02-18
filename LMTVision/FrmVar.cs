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
    public partial class FrmVar : Form
    {
        public FrmVar()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            iniFile.Write(Sys.setStation, "jallCoatOffset", txtOffset.Text, FrmMain.propath);
            iniFile.Write(Sys.setStation, "jBaseNum", txtSta.Text, FrmMain.propath);
            iniFile.Write(Sys.setStation, "jPlusNum", txtAdd.Text, FrmMain.propath);
            Sys.setStation = "";
            this.Close();
        }

        private void FrmVar_Load(object sender, EventArgs e)
        {
            txtOffset.Text = iniFile.Read(Sys.setStation, "jallCoatOffset", FrmMain.propath);
            txtSta.Text = iniFile.Read(Sys.setStation, "jBaseNum", FrmMain.propath);
            txtAdd.Text = iniFile.Read(Sys.setStation, "jPlusNum", FrmMain.propath);
            btnSave.Focus();
        }
    }
}
