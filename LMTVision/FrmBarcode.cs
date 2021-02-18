using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PylonC.NET;
using HalconDotNet;

namespace LMTVision
{
    public partial class FrmBarcode : Form
    {
        FrmMain parent;
        public FrmBarcode(FrmMain parent)
        {
            this.parent = parent;
            this.MdiParent = parent;
            this.Dock = DockStyle.Top;
            InitializeComponent();
        }
        bool readpara = false;
        public string BarPath = Sys.IniPath + "\\BarcodePara.ini"; //扫码信息
        private void FrmBarcode_Load(object sender, EventArgs e)
        {
            readpara = true;
            cBqccdBarcode.Checked = Barcode1.QCCDisChecked;
            string ccdName = (cBqccdBarcode.Checked ? "QCCD" : "BarcodeReader");
            string li = iniFile.Read(ccdName, "LighterValue", parent.BarPath);
            if (li != "")
                UDBarLight.Value = int.Parse(li);
            ReadCCDSet();
            ReadPara();
            readpara = false;
        }
        public void ReadPara()
        {
            cmbProduction.SelectedIndex = int.Parse(IniFile.Read("BarcodeReader", "Production", "0", BarPath));
            cmbMirrored.SelectedIndex = bool.Parse(IniFile.Read("BarcodeReader", "Mirrored", "false", BarPath))==true?0:1;
            nudBarcodeAngleSet.Value = int.Parse(IniFile.Read("BarcodeReader", "BarcodeAngleSet", "0", BarPath));
            nudAllowableOffsetAngle.Value = int.Parse(IniFile.Read("BarcodeReader", "AllowableOffsetAngle", "0", BarPath));
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
            string showMessage = (cBqccdBarcode.Checked ? "QCCD未连接！" : "BarcodeReaderCCD未连接！");
            if ((!Barcode1.IsConnected & !cBqccdBarcode.Checked) || (!QCCD.IsConnected & cBqccdBarcode.Checked))
            {
                MessageBox.Show(showMessage);
                return;
            }
            btnPreview.BackColor = Color.Green;
            //Barcode1.bContinuousShot = true;
            timer1.Enabled = true;
        }
        private void btnStopview_Click(object sender, EventArgs e)
        {
            btnPreview.BackColor = Color.WhiteSmoke;
            //Barcode1.bContinuousShot = false;
            timer1.Enabled = false;
            hv_DRow1 = null; hv_DColumn1 = null; hv_DRow2 = null; hv_DColumn2 = null;
            cBDefinition.Checked = false;
        }
        private void btnDrawRegion_Click(object sender, EventArgs e)
        {
            string showMessage = (cBqccdBarcode.Checked ? "QCCD未连接！" : "BarcodeReaderCCD未连接！");
            if ((!Barcode1.IsConnected & !cBqccdBarcode.Checked) || (!QCCD.IsConnected & cBqccdBarcode.Checked))
            {
                MessageBox.Show(showMessage);
                return;
            }
            Barcode1.bBarcodeRangeSearch = true;
            if (cBqccdBarcode.Checked)
                parent.OneShot9();
            else
                parent.OneShot10();
        }
        private void btnOneShot_Click(object sender, EventArgs e)
        {
            string showMessage = (cBqccdBarcode.Checked ? "QCCD未连接！" : "BarcodeReaderCCD未连接！");
            if ((!Barcode1.IsConnected & !cBqccdBarcode.Checked) || (!QCCD.IsConnected & cBqccdBarcode.Checked))
            {
                MessageBox.Show(showMessage);
                return;
            }
            if (cBqccdBarcode.Checked)
                parent.OneShot9();
            else
                parent.OneShot10();
        }

        private void btnSaveBar_Click(object sender, EventArgs e)
        {
            if (cBqccdBarcode.Checked)
            {
                QCCD.AVI1IsCheck = false;
                QCCD.AVI2IsCheck = false;
                iniFile.Write("QCCD", "AVI1ischecked", QCCD.AVI1IsCheck.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "AVI2ischecked", QCCD.AVI2IsCheck.ToString(), FrmMain.propath);
            }
            iniFile.Write("BarcodeChoose", "QCCDchecked", cBqccdBarcode.Checked.ToString(), parent.BarPath);
            string ccdName = (cBqccdBarcode.Checked ? "QCCD" : "BarcodeReader");
            iniFile.Write(ccdName, "Gain", Barcode1.Gain, parent.BarPath);
            iniFile.Write(ccdName, "ExposureTime", Barcode1.ExposureTime, parent.BarPath);
            iniFile.Write(ccdName, "LighterValue", (UDBarLight.Value).ToString(), parent.BarPath);
            MessageBox.Show("存储成功！");
        }

        private void tBBarLight_ValueChanged(object sender, EventArgs e)
        {
            int Lvalue = (int)tBBarLight.Value;
            UDBarLight.Value = Lvalue;
        }
        private void UDBarLight_ValueChanged(object sender, EventArgs e)
        {
            int Lvalue = (int)UDBarLight.Value;
            tBBarLight.Value = Lvalue;
            if (readpara)
                return;
            parent.ch = 3;
            parent.brit = Lvalue;
            parent.LightSet();
        }

        //CCD曝光和曝光補償
        public NODE_HANDLE hnoteGain;
        public NODE_HANDLE hnoteExp;
        private void tbGain_ValueChanged(object sender, EventArgs e)
        {
            string showMessage = (cBqccdBarcode.Checked ? "QCCD未连接！" : "BarcodeReaderCCD未连接！");
            if ((!Barcode1.IsConnected & !cBqccdBarcode.Checked) || (!QCCD.IsConnected & cBqccdBarcode.Checked))
            {
                MessageBox.Show(showMessage);
                return;
            }
            nudGain.Value = tbGain.Value;
        }        
        private void nudGain_ValueChanged(object sender, EventArgs e)
        {
            string showMessage = (cBqccdBarcode.Checked ? "QCCD未连接！" : "BarcodeReaderCCD未连接！");
            if ((!Barcode1.IsConnected & !cBqccdBarcode.Checked) || (!QCCD.IsConnected &cBqccdBarcode.Checked))
            {
                MessageBox.Show(showMessage);
                return;
            }
            int Gain = Convert.ToInt32(nudGain.Value);
            if (hnoteGain.IsValid)
            {
                int value = tbGain.Value - ((tbGain.Value - tbGain.Minimum) % tbGain.SmallChange);
                /* Set the value. */
                GenApi.IntegerSetValue(hnoteGain, value);
                //GenApi.IntegerSetValue(hnoteGian, trackBar1.Value);
                nudGain.Value = value;
                Barcode1.Gain = value.ToString();
            }
        }
        private void tbExposureTime_ValueChanged(object sender, EventArgs e)
        {
            string showMessage = (cBqccdBarcode.Checked ? "QCCD未连接！" : "BarcodeReaderCCD未连接！");
            if ((!Barcode1.IsConnected & !cBqccdBarcode.Checked) || (!QCCD.IsConnected & cBqccdBarcode.Checked))
            {
                MessageBox.Show(showMessage);
                return;
            }
            nudExposureTime.Value = tbExposureTime.Value;
        }
        private void nudExposureTime_ValueChanged(object sender, EventArgs e)
        {
            string showMessage = (cBqccdBarcode.Checked ? "QCCD未连接！" : "BarcodeReaderCCD未连接！");
            if ((!Barcode1.IsConnected & !cBqccdBarcode.Checked) || (!QCCD.IsConnected & cBqccdBarcode.Checked))
            {
                MessageBox.Show(showMessage);
                return;
            }
            tbExposureTime.Value = Convert.ToInt32(nudExposureTime.Value);
            if (hnoteExp.IsValid)
            {
                /* Correct the increment of the new value. */
                int value = tbExposureTime.Value - ((tbExposureTime.Value - tbExposureTime.Minimum) % tbExposureTime.SmallChange);
                /* Set the value. */
                GenApi.IntegerSetValue(hnoteExp, value);
                //GenApi.IntegerSetValue(hnoteExp, trackBar2.Value);
                nudExposureTime.Value = value;
                Barcode1.ExposureTime = value.ToString();
            }
        }
        public void ReadCCDSet()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                if (QCCD.IsConnected&(Barcode1.QCCDisChecked || cBqccdBarcode.Checked))
                {
                    hnoteGain = parent.m_imageProvider9.GetNodeFromDevice("GainRaw");
                    hnoteExp = parent.m_imageProvider9.GetNodeFromDevice("ExposureTimeRaw");
                }
                if (Barcode1.IsConnected & !(Barcode1.QCCDisChecked || cBqccdBarcode.Checked))
                {
                    hnoteGain = parent.m_imageProvider10.GetNodeFromDevice("GainRaw");
                    hnoteExp = parent.m_imageProvider10.GetNodeFromDevice("ExposureTimeRaw");
                }

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGain));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGain));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGain));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGain));

                tbGain.Minimum = mingain;
                tbGain.Maximum = maxgain;
                tbGain.Value = valgain;
                tbGain.SmallChange = incgain;
                tbGain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                lblGainMin.Text = "" + mingain;
                lblGainMax.Text = "" + maxgain;
                nudGain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExp));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExp));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExp));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExp));

                /* Update the slider. */
                tbExposureTime.Minimum = minexp;
                tbExposureTime.Maximum = maxexp;
                tbExposureTime.Value = valexp;
                tbExposureTime.SmallChange = incexp;
                tbExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                lblExposureTimeMin.Text = "" + minexp;
                lblExposureTimeMax.Text = "" + maxexp;
                nudExposureTime.Text = "" + valexp;

                /* Update accessibility. */
                //slider.Enabled = writable;
                //labelMin.Enabled = writable;
                //labelMax.Enabled = writable;
                //labelName.Enabled = writable;
                //labelCurrentValue.Enabled = writable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (cBqccdBarcode.Checked)
                parent.OneShot9();
            else
                parent.OneShot10();
        }

        HTuple hv_DRow1 = null, hv_DColumn1 = null, hv_DRow2 = null, hv_DColumn2 = null;
        HTuple hv_DValue = new HTuple(), hv_Deviation = new HTuple();
        HObject ho_DRectangle = null;
        private void cBDefinition_CheckedChanged(object sender, EventArgs e)
        {
            if (cBDefinition.Checked)
            {
                if (hv_DRow1 == null || hv_DColumn1 == null || hv_DRow2 == null || hv_DColumn2 == null)
                {
                    cBDefinition.Checked = false;
                    MessageBox.Show("请先设定对焦位置！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            FrmVisionSet.Definition = (cBDefinition.Checked ? true : false);
        }
        private void btnFocusSet_Click(object sender, EventArgs e)
        {
            btnFocusSet.BackColor = Color.Green;
            HOperatorSet.DrawRectangle1(hWindowControl1.HalconWindow, out hv_DRow1, out hv_DColumn1, out hv_DRow2, out hv_DColumn2);
            btnFocusSet.BackColor = Color.WhiteSmoke;
            HOperatorSet.GenRectangle1(out ho_DRectangle, hv_DRow1, hv_DColumn1, hv_DRow2, hv_DColumn2);
            FrmMain.hv_FocusRow1 = hv_DRow1;
            FrmMain.hv_FocusColumn1 = hv_DColumn1;
            FrmMain.hv_FocusRow2 = hv_DRow2;
            FrmMain.hv_FocusColumn2 = hv_DColumn2;
            HOperatorSet.SetColor(hWindowControl1.HalconWindow, "blue");
            HOperatorSet.SetDraw(hWindowControl1.HalconWindow, "margin");
            HOperatorSet.DispObj(ho_DRectangle, hWindowControl1.HalconWindow);
        }

        private void nudBarcodeAngleSet_ValueChanged(object sender, EventArgs e)
        {
            Barcode1.BarcodeAngleSet = (int)nudBarcodeAngleSet.Value;
            IniFile.Write("BarcodeReader", "BarcodeAngleSet", Barcode1.BarcodeAngleSet.ToString(), BarPath);
        }

        private void nudAllowableOffsetAngle_ValueChanged(object sender, EventArgs e)
        {
            Barcode1.AllowableOffsetAngle = (int)nudAllowableOffsetAngle.Value;
            IniFile.Write("BarcodeReader", "AllowableOffsetAngle", Barcode1.AllowableOffsetAngle.ToString(), BarPath);
        }

        private void cmbProduction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Barcode1.Production = cmbProduction.SelectedIndex;
            IniFile.Write("BarcodeReader", "Production", Barcode1.Production.ToString(), BarPath);
        }

        private void cmbMirrored_SelectedIndexChanged(object sender, EventArgs e)
        {
            Barcode1.Mirrored = cmbMirrored.SelectedIndex == 0 ? true : false;
            IniFile.Write("BarcodeReader", "Mirrored", Barcode1.Mirrored.ToString(), BarPath);
        }

    

    }
}
