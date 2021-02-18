 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.IO;
using System.Threading;
using PylonC.NET;
using UC_FirCircle;

namespace LMTVision
{
    public partial class FrmVisionSet : Form
    {
        FrmMain parent;
        FrmVar fv;
        public static FrmVisionSet frm;

        public string ViewNum = "0", SetNum = "0";
        public static double xpm = 0.00441, ypm = 0.00441;
        double Numbase = 0.0, Numadd = 0.0, coatos = 0.0, Numbase2 = 0.0, Numadd2 = 0.0, coatos2 = 0.0;
        HTuple hv_ResultRow = new HTuple(), hv_ResultColumn = new HTuple(), hv_ResultPhi = new HTuple(),hv_ResultLength1 = new HTuple(),hv_ResultLength2 = new HTuple();
        double RectangleLength1_FigureShape = 0, RectangleLength2_FigureShape = 0;
        HWindow hWVision;
        HDevelopExport HD = new HDevelopExport();
        bool bDrawing = false;
        VisionPara MyVisionPara = new VisionPara();
        UC_FitCircleTool ucFitCircleTool_NeedleTipTest = new UC_FitCircleTool();

        public FrmVisionSet(FrmMain parent)
        {
            try
            {
                this.parent = parent;
                this.MdiParent = parent;
                this.Dock = DockStyle.Top;
                InitializeComponent();
                CheckForIllegalCrossThreadCalls = false;
                frm = this;
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void FrmVisionSet_Load(object sender, EventArgs e)
        {
            readpara = true;
            ucFitCircleTool_NeedleTipTest.ucFitCircle = ucFitCircle_NeedleTipTest;
            ucFitCircleTool_NeedleTipTest.Reconnect();
            try
            {
                cmbContrastSet_NeedleTipTest.SelectedIndex = GCCD1.NeedleTipTest.ContrastSet;
                ucGray_NeedleTipTest.Value = GCCD1.NeedleTipTest.Gray;
                cmbNeedleChoice_NeedleTipTest.SelectedIndex = GCCD1.NeedleTipTest.NeedleChoice;
                ucFitCircle_NeedleTipTest.SetValue(GCCD1.NeedleTipTest.Radius,
                    GCCD1.NeedleTipTest.Measure_Transition,
                    GCCD1.NeedleTipTest.Measure_Select,
                    GCCD1.NeedleTipTest.Num_Measures,
                    GCCD1.NeedleTipTest.Measure_Length1,
                    GCCD1.NeedleTipTest.Measure_Length2,
                    GCCD1.NeedleTipTest.Measure_Threshold);

                nudX_UpperSet_NeedleTipTest.Value = GCCD1.NeedleTipTest.X_UpperSet;
                nudX_LowerSet_NeedleTipTest.Value = GCCD1.NeedleTipTest.X_LowerSet;
                nudY_UpperSet_NeedleTipTest.Value = GCCD1.NeedleTipTest.Y_UpperSet;
                nudY_LowerSet_NeedleTipTest.Value = GCCD1.NeedleTipTest.Y_LowerSet;

                lblX_UpperValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.X_UpperValue.ToString();
                lblX_LowerValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.X_LowerValue.ToString();
                lblY_UpperValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.Y_UpperValue.ToString();
                lblY_LowerValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.Y_LowerValue.ToString();

                ucGlue_Circle_OuterRadius_2.Value = Glue.Glue_Circle_OuterRadius_2;
                ucGlue_Circle_InnerRadius_2.Value = Glue.Glue_Circle_InnerRadius_2;
                ucGlue_Circle_StartAngle_2.Value = Glue.Glue_Circle_StartAngle_2;
                ucGlue_Circle_EndAngle_2.Value = Glue.Glue_Circle_EndAngle_2;
                cbGlue_Circle_2.Checked = Glue.Glue_Circle_2;
                nudGlueAngleRatio.Value = (decimal)Glue.GlueAngleRatio;
                ViewNum = (cBViewCCD.SelectedIndex + 1).ToString();
                SetNum = (cBSetCCD.SelectedIndex + 1).ToString();
                hWVision = hWImageSet.HalconWindow;
                CrosslineCkBox.Checked = halcon.IsCrossDraw;
                if (A1CCD1.IsConnected)
                    ReadA1CCD1Set();
                if (A1CCD2.IsConnected)
                    ReadA1CCD2Set();
                if (A2CCD1.IsConnected)
                    ReadA2CCD1Set();
                if (A2CCD2.IsConnected)
                    ReadA2CCD2Set();
                if (PCCD1.IsConnected)
                    ReadPCCD1Set();
                if (PCCD2.IsConnected)
                    ReadPCCD2Set();
                if (GCCD1.IsConnected)
                    ReadGCCD1Set();
                if (GCCD2.IsConnected)
                    ReadGCCD2Set();
                if (QCCD.IsConnected)
                {
                    switch (QCCD.CCDBrand)
                    {
                        case 0:
                            ReadQCCDBSet();
                            break;
                        case 1:
                            ReadQCCDHSet();
                            break;
                    }
                }
                lblShow.Hide();
                lblShowZ.Hide();
                cBGlueCir.Hide();
                cBAVI.Hide();
                cBCutGQ.Hide();
                cBCutGH.Hide();
                gBAngleShow.Hide();
                btnTestGO.Hide();
                btntest1.Hide();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
            readpara = false;
        }

        private void Preview_Click(object sender, EventArgs e)
        {
            if (halcon.IsPreview)
                return;
            #region Choice
            //if (tabProcessMode.SelectedIndex == 0)
            //{
            //    #region 视觉校正
            //    if (ViewNum == "0")
            //    {
            //        MessageBox.Show("请选择CCD!");
            //        return;
            //    }
            //    if (ViewNum == "1")
            //        parent.ContinuousShot1();
            //    if (ViewNum == "2")
            //        parent.ContinuousShot2();
            //    if (ViewNum == "3")
            //        parent.ContinuousShot3();
            //    if (ViewNum == "4")
            //        parent.ContinuousShot4();
            //    if (ViewNum == "5")
            //        parent.ContinuousShot5();
            //    if (ViewNum == "6")
            //        parent.ContinuousShot6();
            //    if (ViewNum == "7")
            //        parent.ContinuousShot7();
            //    if (ViewNum == "8")
            //        parent.ContinuousShot8();
            //    if (ViewNum == "9")
            //        parent.ContinuousShot9();
            //    #endregion
            //}
            //if (tabProcessMode.SelectedIndex == 1)
            //{
            //    #region 辨识识别
            //    if (SetNum == "0")
            //    {
            //        MessageBox.Show("请选择CCD!");
            //        return;
            //    }
            //    if (SetNum == "1")
            //        parent.ContinuousShot1();
            //    if (SetNum == "2")
            //        parent.ContinuousShot2();
            //    if (SetNum == "3")
            //        parent.ContinuousShot3();
            //    if (SetNum == "4")
            //        parent.ContinuousShot4();
            //    if (SetNum == "5")
            //        parent.ContinuousShot5();
            //    if (SetNum == "6")
            //        parent.ContinuousShot6();
            //    if (SetNum == "7")
            //        parent.ContinuousShot7();
            //    if (SetNum == "8")
            //        parent.ContinuousShot8();
            //    if (SetNum == "9")
            //        parent.ContinuousShot9();
            //    #endregion
            //}
            #endregion
            halcon.IsPreview = true;
            Preview.BackColor = Color.Green;
            halcon.AIsChecked = ((tabProcessMode.SelectedIndex == 0 && CallibratManu.SelectedIndex == 0) ? true : false);
            if (halcon.AIsChecked)
                angleC = 0;
            if (((tabProcessMode.SelectedIndex == 0 & ViewNum == "9") || (tabProcessMode.SelectedIndex == 1 & SetNum == "9")) & QCCD.CCDBrand == 1)
                parent.myHikvision.ContinuousShot();
            else
                timer1.Enabled = true;
        }
        private void StopView_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            halcon.IsPreview = false;
            Preview.BackColor = Color.WhiteSmoke;
            hv_DRow1 = null; hv_DColumn1 = null; hv_DRow2 = null; hv_DColumn2 = null;
            cBDefinition.Checked = false;
            string Num = "";
            if (tabProcessMode.SelectedIndex == 0)
                Num = ViewNum;  //视觉校正
            if (tabProcessMode.SelectedIndex == 1)
                Num = SetNum;  // 辨识检测
            switch (Num)
            {
                case "1": parent.Stop1(); break;
                case "2": parent.Stop2(); break;
                case "3": parent.Stop3(); break;
                case "4": parent.Stop4(); break;
                case "5": parent.Stop5(); break;
                case "6": parent.Stop6(); break;
                case "7": parent.Stop7(); break;
                case "8": parent.Stop8(); break;
                case "9":
                    if (QCCD.CCDBrand == 0)
                        parent.Stop9();
                    if (QCCD.CCDBrand == 1)
                        parent.myHikvision.Stop();
                    break;
            }
        }
        private void SingleGrab_Click(object sender, EventArgs e)
        {
            StopPreview();
            string Num = "";
            if (tabProcessMode.SelectedIndex == 0)
                Num = ViewNum;
            if (tabProcessMode.SelectedIndex == 1)
                Num = SetNum;
            switch (Num)
            {
                case "1": parent.OneShot1(); break;
                case "2": parent.OneShot2(); break;
                case "3": parent.OneShot3(); break;
                case "4": parent.OneShot4(); break;
                case "5": parent.OneShot5(); break;
                case "6": PCCD2.IntSingle = cBLocation3.SelectedIndex + 1;
                    parent.OneShot6(); break;
                case "7": parent.OneShot7(); break;
                case "8": parent.OneShot8(); break;
                case "9": parent.OneShot9(); break;
            }
        }
        public void StopPreview()
        {
            if (halcon.IsPreview)
            {
                try
                {
                    parent.Stop1();
                    parent.Stop2();
                    parent.Stop3();
                    parent.Stop4();
                    parent.Stop5();
                    parent.Stop6();
                    parent.Stop7();
                    parent.Stop8();
                    if (QCCD.CCDBrand == 0)
                        parent.Stop9();
                    if (QCCD.CCDBrand == 1)
                        parent.myHikvision.Stop();
                    halcon.IsPreview = false;
                }
                catch
                { }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)//预览
        {
            string Num = "";
            //依照功能選擇相機
            switch (tabProcessMode.SelectedIndex)
            {
                case 0: Num = ViewNum; break;//视觉校正
                case 1: Num = SetNum; break; // 辨识识别
                case 3: Num = "7"; break;//GCCD1針頭檢測
            }
          
            switch (Num)
            {
                case "1": parent.OneShot1(); break;
                case "2": parent.OneShot2(); break;
                case "3": parent.OneShot3(); break;
                case "4": parent.OneShot4(); break;
                case "5": parent.OneShot5(); break;
                case "6": parent.OneShot6(); break;
                case "7": parent.OneShot7(); break;
                case "8": parent.OneShot8(); break;
                case "9": parent.OneShot9(); break;
            }
            Thread.Sleep(1);
        }

        OpenFileDialog fop = new OpenFileDialog();
        string names = "";
        HTuple LoadWidth = new HTuple(), LoadHeight = new HTuple();
        private void LoadImg_Click(object sender, EventArgs e)
        {
            fop.Title = "请选择文件";
            fop.Filter = "所有文件(*.*)|*.*";
            if (fop.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadImg.BackColor = Color.GreenYellow;
                    names = fop.FileName;
                    ho_ImageSet.Dispose();
                    HOperatorSet.ReadImage(out ho_ImageSet, names);
                    HOperatorSet.GenEmptyObj(out halcon.Image[int.Parse(SetNum) - 1]);
                    HOperatorSet.CopyImage(ho_ImageSet, out halcon.Image[int.Parse(SetNum) - 1]);
                    HOperatorSet.GenEmptyObj(out halcon.ImageOri[int.Parse(SetNum) - 1]);
                    HOperatorSet.CopyImage(ho_ImageSet, out halcon.ImageOri[int.Parse(SetNum) - 1]);
                    HOperatorSet.GetImageSize(ho_ImageSet, out LoadWidth, out LoadHeight);
                    HOperatorSet.SetPart(hWVision, 0, 0, LoadHeight, LoadWidth);
                    hWVision.DispObj(ho_ImageSet);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                    LoadImg.BackColor = Color.WhiteSmoke;
                }
                catch
                {
                    MessageBox.Show("請先選擇相機");
                }
            }
        }
        private void btntest1_Click(object sender, EventArgs e)
        {
            halcon.HWindowID[3] = hWVision;
            FrmVisionSet.xpm = A2CCD2.xpm;
            FrmVisionSet.ypm = A2CCD2.ypm;
            //GCCD2.IntSingle = 1;
            //PLC.ccdTrigger[12] = true;
            A2CCD2.IntSingle = 2;
            HD.ImagePro4(halcon.HWindowID[3]);
        }
        private void btnTestGO_Click(object sender, EventArgs e)
        {
            //halcon.HWindowID[5] = hWVision;
            //FrmVisionSet.xpm = PCCD2.xpm;
            //FrmVisionSet.ypm = PCCD2.ypm;
            //PCCD2.IntSingle = cBLocation3.SelectedIndex + 1;
            //HD.ImagePro6(halcon.HWindowID[5]);
            //halcon.HWindowID[1] = hWVision;
            //FrmVisionSet.xpm = A1CCD2.xpm;
            //FrmVisionSet.ypm = A1CCD2.ypm;
            //A1CCD2.IntSingle = cBLocation.SelectedIndex + 1;
            //HD.ImagePro2(halcon.HWindowID[1]);
            halcon.HWindowID[7] = hWVision;
            FrmVisionSet.xpm = GCCD2.xpm;
            FrmVisionSet.ypm = GCCD2.ypm;
            GCCD2.IntSingle = 2;
            PLC.ccdTrigger[12] = true;
            HD.ImagePro8(halcon.HWindowID[7]);
        }

        //校正&辨识
        private void tabProcessMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (halcon.IsPreview)
            {
                MessageBox.Show("请先停止预览！");
                return;
            }
            if (tabProcessMode.SelectedIndex != 2)
            {
                Sys.AssLocation = "";
                Sys.AssLocation2 = "";
            }
            if (tabProcessMode.SelectedIndex == 3)
                SetNum = "7";
        }
        //校正
        bool readpara = false;
        private void cBViewCCD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (halcon.IsPreview)
            {
                MessageBox.Show("请先停止预览！");
                return;
            }
            label1.Focus();
            SetNum = ViewNum = (cBViewCCD.SelectedIndex + 1).ToString();
            tabLight.SelectedIndex = cBViewCCD.SelectedIndex;
            tabCCDParam.SelectedIndex = cBViewCCD.SelectedIndex;
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: CCDNAME = "A1CCD2"; xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: CCDNAME = "A2CCD1"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: CCDNAME = "A2CCD2"; xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: CCDNAME = "PCCD1"; xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: CCDNAME = "PCCD2"; xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: CCDNAME = "GCCD1"; xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: CCDNAME = "GCCD2"; xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: CCDNAME = "QCCD"; xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            ReadParaCor(CCDNAME);
            if (ViewNum == "1" || ViewNum == "3")
                if (CallibratManu.SelectedIndex == 5 || CallibratManu.SelectedIndex == 6)
                    CallibratManu.SelectedIndex = 4;
            if (ViewNum == "2" || ViewNum == "4")
                if (CallibratManu.SelectedIndex == 4 || CallibratManu.SelectedIndex == 5)
                    CallibratManu.SelectedIndex = 6;
            if (ViewNum == "7")
                if (CallibratManu.SelectedIndex == 4 || CallibratManu.SelectedIndex == 6)
                    CallibratManu.SelectedIndex = 5;
            cBGlueCir.Hide();
            if (ViewNum == "8")
            {
                cBGlueCir.Show();
                if (CallibratManu.SelectedIndex == 4 || CallibratManu.SelectedIndex == 5 || CallibratManu.SelectedIndex == 6)
                    CallibratManu.SelectedIndex = 7;
            }
        }
        void ReadParaCor(string CCDName)//校正单纯读取
        {
            readpara = true;
            #region
            #region 基础
            string pr = iniFile.Read(CCDName, "PixRegionR", parent.setpath);
            if (pr != "")
                tBPRegion.Value = int.Parse(pr);
            else
                RegionRadius = 0;
            string tt = iniFile.Read(CCDName, "BinBviThreshold", parent.setpath);
            if (tt != "")
            {
                tBthreshold.Value = int.Parse(tt);
                tBViewRa.Value = int.Parse(iniFile.Read(CCDName, "PixRadius", parent.setpath));
                tBViewWidth.Value = int.Parse(iniFile.Read(CCDName, "PixWidth", parent.setpath));
                tBViewW2B.Value = int.Parse(iniFile.Read(CCDName, "PixW2BThreshold", parent.setpath));
                tBViewB2W.Value = int.Parse(iniFile.Read(CCDName, "PixB2WThreshold", parent.setpath));
                hv_transition = iniFile.Read(CCDName, "Transition", parent.setpath);
            }
            #endregion
            //角度
            txtAdeviation.Text = iniFile.Read(CCDName, "defiexuion_angle", parent.setpath);
            if (txtAdeviation.Text != "")
                angleC = double.Parse(txtAdeviation.Text);
            switch (int.Parse(ViewNum))
            {
                case 1: A1CCD1.angleC = angleC; break;
                case 2: A1CCD2.angleC = angleC; break;
                case 3: A2CCD1.angleC = angleC; break;
                case 4: A2CCD2.angleC = angleC; break;
                case 5: PCCD1.angleC = angleC; break;
                case 6: PCCD2.angleC = angleC; break;
                case 7: GCCD1.angleC = angleC; break;
                case 8: GCCD2.angleC = angleC; break;
                case 9: QCCD.angleC = angleC; break;
            }
            //像素
            //txtPixrange.Text = iniFile.Read(CCDName, "MoveRange", parent.setpath);
            txtXpix.Text = iniFile.Read(CCDName, "Pixel_X", parent.setpath);
            txtYpix.Text = iniFile.Read(CCDName, "Pixel_Y", parent.setpath);
            txtAbrange.Text = iniFile.Read(CCDName, "AssemblyMoveRange", parent.setpath);
            txtAbX.Text = iniFile.Read(CCDName, "AssemblyPixel_X", parent.setpath);
            txtAbY.Text = iniFile.Read(CCDName, "AssemblyPixel_Y", parent.setpath);
            //三心
            txtBlockX.Text = iniFile.Read(CCDName, "BlockCenX", parent.setpath);
            txtBlockY.Text = iniFile.Read(CCDName, "BlockCenY", parent.setpath);
            //吸嘴
            txtNozzleX.Text = iniFile.Read(CCDName, "NozzleX", parent.setpath);
            txtNozzleY.Text = iniFile.Read(CCDName, "NozzleY", parent.setpath);
            string Fchecked = iniFile.Read(CCDName, "FChecked", parent.setpath);
            SqcheckBox.Checked = (Fchecked == "true" ? true : false);
            string fw = iniFile.Read(CCDName, "FWidth", parent.setpath);
            if (fw != "")
                UDBlockWidth.Value = int.Parse(fw);
            string fd = iniFile.Read(CCDName, "FDis", parent.setpath);
            if (fd != "")
                UDBlockdist.Value = int.Parse(fd);
            string ft = iniFile.Read(CCDName, "Ftransition", parent.setpath);
            if (ft == "positive")
                UDBlockB2W.Value = int.Parse(iniFile.Read(CCDName, "Fthreshold", parent.setpath));
            if (ft == "negative")
                UDBlockW2B.Value = int.Parse(iniFile.Read(CCDName, "Fthreshold", parent.setpath));
            iniFile.Write(CCDName, "FNozzleX", txtFNozzleX.Text, parent.setpath);
            iniFile.Write(CCDName, "FNozzleY", txtFNozzleY.Text, parent.setpath);
            iniFile.Write(CCDName, "FNozzleAngle", txtFAngle.Text, parent.setpath);

            //针头
            txtNeedleX.Text = iniFile.Read("GCCD1", "NeedleX", parent.setpath);
            txtNeedleY.Text = iniFile.Read("GCCD1", "NeedleY", parent.setpath);
            #endregion
            readpara = false;
        }
        //辨识square&circle
        private void cBSetCCD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (halcon.IsPreview)
            {
                MessageBox.Show("请先停止预览！");
                return;
            }

            cBfigure.SelectedIndex = cBSetCCD.SelectedIndex;
            tabLight.SelectedIndex = cBSetCCD.SelectedIndex;
            tabCCDParam.SelectedIndex = cBSetCCD.SelectedIndex;
            string CCDNAME = ""; string area = "", area1 = "", area2 = "", area4 = "";
            SetNum = (cBSetCCD.SelectedIndex + 1).ToString();
            if ((SetNum == "2" || SetNum == "4" || SetNum == "5" || SetNum == "6") & tabVisionSet.SelectedIndex == 3)
                tabVisionSet.SelectedIndex = 3;
            else
                if (tabVisionSet.SelectedIndex == 2)
                    tabVisionSet.SelectedIndex = 2;
            #region  下拉框选择
            cBtest.Hide(); cBtest.Enabled = false;
            cBLocation.Hide(); cBLocation.Enabled = false;
            cBLocation2.Hide(); cBLocation2.Enabled = false;
            cBLocation3.Hide(); cBLocation3.Enabled = false;
            cBLocation4.Hide(); cBLocation4.Enabled = false;
            if ((SetNum == "1" || SetNum == "3"))
            {
                #region A1CCD1&A2CCD1
                if (cBoxTest.Checked)
                {
                    cBtest.Show(); cBtest.Enabled = true;
                    cBtest.SelectedIndex = 0;
                }
                else
                {
                    cBLocation4.Show(); cBLocation4.Enabled = true;
                    if (cBLocation4.SelectedIndex == 0)
                        area4 = "1";
                    if (cBLocation4.SelectedIndex == 1)
                        area4 = "2";
                }
                #endregion
            }
            if ((SetNum == "2" || SetNum == "4"))
            {
                #region A1CCD2&A2CCD2 Pic&Pla
                if (cBoxTest.Checked)
                {
                    cBtest.Show(); cBtest.Enabled = true;
                    cBtest.SelectedIndex = 0;
                }
                else
                {
                    cBLocation.Show(); cBLocation.Enabled = true;
                    if (cBLocation.SelectedIndex == 0)
                        area = "PickUp";
                    if (cBLocation.SelectedIndex == 1)
                        area = "Platform";
                }
                #endregion
            }
            //gBP1AVI.SendToBack();
            //gBQAVI.SendToBack(); 
            cBCutGQ.Hide();
            cBCutGH.Hide();
            cbGlueFollow.Hide();
            #endregion
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: CCDNAME = "A1CCD2"; xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: CCDNAME = "A2CCD1"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: CCDNAME = "A2CCD2"; xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: CCDNAME = "PCCD1"; xpm = PCCD1.xpm; ypm = PCCD1.ypm;
                    #region PCCD1
                    //gBP1AVI.BringToFront();
                    tabGlueOut.SelectedIndex = 2;
                    cBGOutMode.Value = 3;
                    cBtest.Hide();
                    if (cBoxTest.Checked)
                    {
                        cBtest.Show();
                        cBtest.Enabled = true;
                        cBtest.SelectedIndex = 0;
                    }
                    #endregion
                    break;
                case 6: CCDNAME = "PCCD2"; xpm = PCCD2.xpm; ypm = PCCD2.ypm;
                    #region PCCD2 Pic&Pla1&Pla2
                    cBLocation3.Show();
                    cBLocation3.Enabled = true;
                    switch (cBLocation3.SelectedIndex)
                    {
                        case 0: area = "PickUp"; break;
                        case 1: area = "Platform1"; break;
                        case 2: area = "Platform2"; break;
                    }
                    #endregion
                    break;
                case 7: CCDNAME = "GCCD1"; xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: CCDNAME = "GCCD2"; xpm = GCCD2.xpm; ypm = GCCD2.ypm;
                    cBLocation2.Show(); cBLocation2.Enabled = true;
                    cBCutGQ.Show(); cBCutGH.Show();
                    cbGlueFollow.Show();
                    break;
                case 9: CCDNAME = "QCCD"; xpm = QCCD.xpm; ypm = QCCD.ypm;
                    //gBQAVI.BringToFront(); 
                    tabGlueOut.SelectedIndex = 3;
                    cBGOutMode.Value = 4;
                    break;
            }
            #region CCDNAME
            if ((SetNum == "1" || SetNum == "3"))
            {
                #region A1CCD1&A2CCD1  H&L
                area4 = iniFile.Read(CCDNAME, "Location", FrmMain.propath);
                if (cBoxTest.Checked)
                {
                    if (area4 == "")
                    {
                        cBtest.SelectedIndex = -1;
                        return;
                    }
                    if (area4 == "Hold")
                        cBtest.SelectedIndex = 0;
                    if (area4 == "Lens")
                        cBtest.SelectedIndex = 1;
                }
                else
                {
                    if (area4 == "")
                    {
                        cBLocation4.SelectedIndex = -1;
                        return;
                    }
                    else
                    {
                        cBLocation4.SelectedIndex = int.Parse(area4) - 1;
                    }
                }
                CCDNAME = CCDNAME + "-" + area4;
                #endregion
            }
            if (SetNum == "2" || SetNum == "4")
            {
                #region A1CCD2&A2CCD2  H&L Pic&Pla
                area2 = iniFile.Read(CCDNAME, "Location", FrmMain.propath);
                if (cBoxTest.Checked)
                {
                    if (area2 == "")
                    {
                        cBtest.SelectedIndex = -1;
                        return;
                    }
                    if (area2 == "Hold")
                        cBtest.SelectedIndex = 0;
                    if (area2 == "Lens")
                        cBtest.SelectedIndex = 1;
                }
                else
                {
                    if (area2 == "")
                    {
                        cBLocation.SelectedIndex = -1;
                        return;
                    }
                    if (area2 == "PickUp")
                        cBLocation.SelectedIndex = 0;
                    if (area2 == "Platform")
                        cBLocation.SelectedIndex = 1;
                }
                CCDNAME = CCDNAME + "-" + area2;
                #endregion
                Coatcheck2.Enabled = true;
                cBGOutMode.Value = 2;
            }
            else
                Coatcheck2.Enabled = false;
            if (SetNum == "5")
            {
                area2 = iniFile.Read(CCDNAME, "Location", FrmMain.propath);
                if (cBoxTest.Checked)
                {
                    if (area2 == "")
                    {
                        cBtest.SelectedIndex = -1;
                        return;
                    }
                    if (area2 == "Hold")
                        cBtest.SelectedIndex = 0;
                    if (area2 == "Lens")
                        cBtest.SelectedIndex = 1;
                }
            }
            if (SetNum == "6")
            {
                #region PCCD2 Pic&Pla1&Pla2
                area = iniFile.Read(CCDNAME, "Location", FrmMain.propath);
                if (area == "")
                {
                    cBLocation3.SelectedIndex = -1;
                    return;
                }
                if (area == "PickUp")
                {
                    cBLocation3.SelectedIndex = 0;
                    cBAVI.Show();
                }
                else
                    cBAVI.Hide();
                if (area == "Platform1")
                    cBLocation3.SelectedIndex = 1;
                if (area == "Platform2")
                    cBLocation3.SelectedIndex = 1;
                CCDNAME = CCDNAME + "-" + area;
                #endregion
                GlueOutcheck.Enabled = true;
                Coatcheck.Enabled = true;
                cBGOutMode.Value = 1;
            }
            else
            {
                GlueOutcheck.Enabled = false;
                Coatcheck.Enabled = false;
            }
            if (SetNum == "8")
            {
                #region GCCD2 1&2
                area1 = iniFile.Read(CCDNAME, "Location", FrmMain.propath);
                if (area1 == "")
                {
                    cBLocation2.SelectedIndex = -1;
                    return;
                }
                if (area1 == "1")
                    cBLocation2.SelectedIndex = 0;
                if (area1 == "2")
                    cBLocation2.SelectedIndex = 1;
                if (area1 == "3")
                    cBLocation2.SelectedIndex = 2;
                if (area1 == "4")
                    cBLocation2.SelectedIndex = 3;
                CCDNAME = CCDNAME + "-" + area1;
                #endregion
            }
            #endregion
            ReadParaIde(CCDNAME);
        }
        private void cBLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetNum = (cBSetCCD.SelectedIndex + 1).ToString();
            cBfigure.SelectedIndex = cBSetCCD.SelectedIndex;
            tabLight.SelectedIndex = cBSetCCD.SelectedIndex;
            tabCCDParam.SelectedIndex = cBSetCCD.SelectedIndex;
            string CCDNAME = ""; string area = "";
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            switch (int.Parse(SetNum))
            {
                case 2: CCDNAME = "A1CCD2"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 6: CCDNAME = "PCCD2"; break;
            }
            CCDNAME = CCDNAME + "-" + area;
            ReadParaIde(CCDNAME);
        }
        private void cBLocation2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetNum = (cBSetCCD.SelectedIndex + 1).ToString();
            cBfigure.SelectedIndex = cBSetCCD.SelectedIndex;
            tabLight.SelectedIndex = cBSetCCD.SelectedIndex;
            tabCCDParam.SelectedIndex = cBSetCCD.SelectedIndex;
            string CCDNAME = "";
            string area1 = (cBLocation2.SelectedIndex + 1).ToString();
            switch (int.Parse(SetNum))
            {
                case 8: CCDNAME = "GCCD2"; break;
            }
            CCDNAME = CCDNAME + "-" + area1;
            ReadParaIde(CCDNAME);
        }
        private void cBLocation3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetNum = (cBSetCCD.SelectedIndex + 1).ToString();
            cBfigure.SelectedIndex = cBSetCCD.SelectedIndex;
            tabLight.SelectedIndex = cBSetCCD.SelectedIndex;
            tabCCDParam.SelectedIndex = cBSetCCD.SelectedIndex;
            string CCDNAME = ""; string area = "";
            if (cBLocation3.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation3.SelectedIndex == 1)
                area = "Platform1";
            if (cBLocation3.SelectedIndex == 2)
                area = "Platform2";
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            CCDNAME = CCDNAME + "-" + area;
            if (CCDNAME == "PCCD2-PickUp")
                cBAVI.Show();
            else
                cBAVI.Hide();
            ReadParaIde(CCDNAME);
        }
        private void cBLocation4_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetNum = (cBSetCCD.SelectedIndex + 1).ToString();
            cBfigure.SelectedIndex = cBSetCCD.SelectedIndex;
            tabLight.SelectedIndex = cBSetCCD.SelectedIndex;
            tabCCDParam.SelectedIndex = cBSetCCD.SelectedIndex;
            string CCDNAME = "";
            string area1 = (cBLocation4.SelectedIndex + 1).ToString();
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 5: CCDNAME = "PCCD1"; break;
            }
            CCDNAME = CCDNAME + "-" + area1;
            ReadParaIde(CCDNAME);
        }
        void ReadParaIde(string CCDName)//辨识单纯读取
        {
            readpara = true;
            try
            {
                #region
                #region Light
                try
                {
                    string l = iniFile.Read(CCDName, "LighterValue", FrmMain.propath);
                    string l1 = iniFile.Read(CCDName, "LighterValue1", FrmMain.propath);
                    string l2 = iniFile.Read(CCDName, "LighterValue2", FrmMain.propath);
                    string l3 = iniFile.Read(CCDName, "LighterValue3", FrmMain.propath);
                    string l4 = iniFile.Read(CCDName, "LighterValue4", FrmMain.propath);
                    if (SetNum == "1" && l1 != "")
                    {
                        UD_LED1Lig.Value = int.Parse(l1);
                        UD_LED2Lig.Value = int.Parse(l2);
                    }
                    if (SetNum == "2" && l != "")
                        UD_LED3Lig.Value = int.Parse(l);
                    if (SetNum == "3" && l1 != "")
                    {
                        UD_LED4Lig.Value = int.Parse(l1);
                        UD_LED5Lig.Value = int.Parse(l2);
                    }
                    if (SetNum == "4" && l != "")
                        UD_LED6Lig.Value = int.Parse(l);
                    if (SetNum == "5" && l1 != "")
                    {
                        UD_LED7Lig.Value = int.Parse(l1);
                        //UD_LED8Lig.Value = int.Parse(l2);
                    }
                    if (SetNum == "6" && l != "")
                        UD_LED9Lig.Value = int.Parse(l);
                    if (SetNum == "7" && l != "")
                        UD_LED10Lig.Value = int.Parse(l);
                    if (SetNum == "8" && l != "")
                        UD_LED11Lig.Value = int.Parse(l);
                    if (SetNum == "9" && l1 != "")
                    {
                        UD_LED8Lig.Value = int.Parse(l1);
                        //UD_LED12Lig.Value = int.Parse(l1);
                        //UD_LED13Lig.Value = int.Parse(l2);
                        //UD_LED14Lig.Value = int.Parse(l3);
                        //UD_LED15Lig.Value = int.Parse(l4);
                    }
                }
                catch
                {
                    MessageBox.Show("光源切換NG!");
                }
                #endregion
                #region  找圆心Mode2
                if (CCDName == "GCCD2-1" || CCDName == "GCCD2-2" || CCDName == "GCCD2-3" || CCDName == "GCCD2-4")
                {
                    string astatus = iniFile.Read(CCDName, "GQAngleStatus", FrmMain.propath);
                    cBCutGQ.Checked = ((astatus == "true") ? true : false);
                    astatus = iniFile.Read(CCDName, "GHAngleStatus", FrmMain.propath);
                    cBCutGH.Checked = ((astatus == "true") ? true : false);
                    Glue.Glue_Follow = cbGlueFollow.Checked = bool.Parse(IniFile.Read(CCDName, "Glue_Follow", "false", FrmMain.propath));
                }
                else
                {
                    string astatus = iniFile.Read(CCDName, "AngleStatus", FrmMain.propath);
                    cBCut.Checked = ((astatus == "true") ? true : false);
                }
                string fs = iniFile.Read(CCDName, "FigureShape", FrmMain.propath);
                if (fs == "Circle")
                    cbFigureShape.SelectedIndex = 0;
                if (fs == "Square")
                {
                    cbFigureShape.SelectedIndex = 1;
                }
                string m2 = iniFile.Read(CCDName, "Mode2RegionRadius", FrmMain.propath);
                if (m2 != "")
                {
                    tBRegion2.Value = int.Parse(m2);
                    tBgray2.Value = int.Parse(iniFile.Read(CCDName, "Mode2gray", FrmMain.propath));
                    tBRRadius.Value = int.Parse(iniFile.Read(CCDName, "Mode2RingRadius", FrmMain.propath));
                    tBRWidth.Value = int.Parse(iniFile.Read(CCDName, "Mode2UDRingWidth", FrmMain.propath));
                    hv_transition = iniFile.Read(CCDName, "Transition", FrmMain.propath);
                    tBPRmin2.Value = int.Parse(iniFile.Read(CCDName, "ZoneRmin", FrmMain.propath));
                    tBPRmax2.Value = int.Parse(iniFile.Read(CCDName, "ZoneRmax", FrmMain.propath));
                    txtXmax2.Text = iniFile.Read(CCDName, "Mode2BaCenX+", FrmMain.propath);
                    txtXmin2.Text = iniFile.Read(CCDName, "Mode2BaCenX-", FrmMain.propath);
                    txtYmax2.Text = iniFile.Read(CCDName, "Mode2BaCenY+", FrmMain.propath);
                    txtYmin2.Text = iniFile.Read(CCDName, "Mode2BaCenY-", FrmMain.propath);
                    if (hv_transition == "negative")
                        tBCirB2W2.Value = int.Parse(iniFile.Read(CCDName, "Mode2UDRingThreshold", FrmMain.propath));
                    if (hv_transition == "positive")
                        tBCirW2B2.Value = int.Parse(iniFile.Read(CCDName, "Mode2UDRingThreshold", FrmMain.propath));
                    string grayc = iniFile.Read(CCDName, "GrayChecked", FrmMain.propath);
                    cBGrayChecked.Checked = (grayc == "False" ? false : true);
                }
                #endregion
                string cbam = iniFile.Read(CCDName, "AngleMode", FrmMain.propath);
                if (cbam != "")
                    cBAMode.Value = int.Parse(cbam);
                #region 找角度Mode1
                string udt = iniFile.Read(CCDName, "Mode1DegLineGray", FrmMain.propath);
                if (udt != "")
                {
                    UDlineThreshold.Value = int.Parse(udt);
                    UDlineWidth.Value = int.Parse(iniFile.Read(CCDName, "Mode1DegLineWidth", FrmMain.propath));
                    txtDegmax.Text = iniFile.Read(CCDName, "Mode1Deg+", FrmMain.propath);
                    txtDegmin.Text = iniFile.Read(CCDName, "Mode1Deg-", FrmMain.propath);
                }
                #endregion
                #region 找角度Mode2
                string ar = iniFile.Read(CCDName, "AngleRmin", FrmMain.propath);
                if (ar != "")
                {
                    tBAMinRa.Value = int.Parse(ar);
                    tBAMaxRa.Value = int.Parse(iniFile.Read(CCDName, "AngleRmax", FrmMain.propath));
                    tBAB2W.Value = int.Parse(iniFile.Read(CCDName, "AnglebinB2W", FrmMain.propath));
                    tBAW2B.Value = int.Parse(iniFile.Read(CCDName, "AnglebinW2B", FrmMain.propath));
                    tBAAreaMin.Value = int.Parse(iniFile.Read(CCDName, "AngleAreamin", FrmMain.propath));
                    tBAAreaMax.Value = int.Parse(iniFile.Read(CCDName, "AngleAreamax", FrmMain.propath));
                    string AddStatus = iniFile.Read(CCDName, "Mode2Add", FrmMain.propath);
                    cBAddFCT.Checked = ((AddStatus == "true") ? true : false);
                    gBCutKuan.Enabled = cBAddFCT.Checked;
                    string lmin = iniFile.Read(CCDName, "Mode2Widthmin", FrmMain.propath);
                    tBAWMin.Value = int.Parse((lmin != "") ? lmin : "0");
                    string lmax = iniFile.Read(CCDName, "Mode2Widthmax", FrmMain.propath);
                    tBAWMax.Value = int.Parse((lmax != "") ? lmax : "30");
                    string LLmin = iniFile.Read(CCDName, "Mode2Lengthmin", FrmMain.propath);
                    tBALMin.Value = int.Parse((LLmin != "") ? LLmin : "60");
                    string LLmax = iniFile.Read(CCDName, "Mode2Lengthmax", FrmMain.propath);
                    tBALMax.Value = int.Parse((LLmax != "") ? LLmax : "200");
                    txtDegmax.Text = iniFile.Read(CCDName, "BaDeg+", FrmMain.propath);
                    txtDegmin.Text = iniFile.Read(CCDName, "BaDeg-", FrmMain.propath);
                }
                #endregion
                #region 找角度Mode3
                string fat = iniFile.Read(CCDName, "TemplateShape", FrmMain.propath);
                if (fat != "")
                {
                    ar = iniFile.Read(CCDName, "AngleModeRmin", FrmMain.propath);
                    tBAMinCut.Value = int.Parse((ar != "") ? ar : "1");
                    ar = iniFile.Read(CCDName, "AngleModeRmax", FrmMain.propath);
                    tBAMaxCut.Value = int.Parse((ar != "") ? ar : "200");
                    FeatureChose.SelectedIndex = int.Parse(fat);
                    UDModeTh.Value = int.Parse(iniFile.Read(CCDName, "AngleThreshold3", FrmMain.propath));
                    //iniFile.Write(CCDNAME, "LighterValue", UDLight2.Value.ToString(), FrmMain.propath);
                    AngleMScore.Value = int.Parse(iniFile.Read(CCDName, "SetScore", FrmMain.propath));
                }
                #endregion
                #region 找角度Mode4
                ar = iniFile.Read(CCDName, "Mode4AngleRmin", FrmMain.propath);
                if (ar != "")
                {
                    tBAMinRa4.Value = int.Parse(ar);
                    tBAMaxRa4.Value = int.Parse(iniFile.Read(CCDName, "Mode4AngleRmax", FrmMain.propath));
                    tBAB2W4.Value = int.Parse(iniFile.Read(CCDName, "Mode4AnglebinB2W", FrmMain.propath));
                    tBAW2B4.Value = int.Parse(iniFile.Read(CCDName, "Mode4AnglebinW2B", FrmMain.propath));
                    tBAAreaMin4.Value = int.Parse(iniFile.Read(CCDName, "Mode4AngleAreamin", FrmMain.propath));
                    tBAAreaMax4.Value = int.Parse(iniFile.Read(CCDName, "Mode4AngleAreamax", FrmMain.propath));
                    txtDegmax.Text = iniFile.Read(CCDName, "Mode4BaDeg+", FrmMain.propath);
                    txtDegmin.Text = iniFile.Read(CCDName, "Mode4BaDeg-", FrmMain.propath);
                }
                #endregion
                #region 找角度Mode5
                if (cBAMode.Value == 5)
                {
                    string adis = iniFile.Read(CCDName, "ARegionCenDistance", FrmMain.propath);
                    if (adis != "")
                    {
                        tBARegionDis.Value = int.Parse(adis);
                        tBARegionlen1.Value = int.Parse(iniFile.Read(CCDName, "ARegionLength", FrmMain.propath));
                        tBARegionlen2.Value = int.Parse(iniFile.Read(CCDName, "ARegionWidth", FrmMain.propath));
                        tBARegionGray.Value = int.Parse(iniFile.Read(CCDName, "ARegionDryThreshold", FrmMain.propath));
                    }
                    txtDegmax.Text = iniFile.Read(CCDName, "ARegionDeg+", FrmMain.propath);
                    txtDegmin.Text = iniFile.Read(CCDName, "ARegionDeg-", FrmMain.propath);
                }
                #endregion
                #region 找角度Mode6
                if (cBAMode.Value == 6)
                {
                    cmbCircleMeasureSelect.SelectedIndex = IniFile.Read(CCDName, "CircleMeasureSelect", "last", FrmMain.propath) == "last" ? 0 : 1;
                    ucCircleRadius.Value = int.Parse(IniFile.Read(CCDName, "CircleRadius", "1", FrmMain.propath));
                    ucCircleLength.Value = int.Parse(IniFile.Read(CCDName, "CircleLength", "1", FrmMain.propath));
                    halcon.CircleMeasureTransition = IniFile.Read(CCDName, "Transition", "negative", FrmMain.propath);
                    if (halcon.CircleMeasureTransition == "negative")
                    {
                        ucCircleWhite2Black.Value = halcon.CircleMeasureThreshold;
                    }
                    else
                    {
                        ucCircleBlack2White.Value = halcon.CircleMeasureThreshold;
                    }
                }
                #endregion
                #region 辅助mode3
                string cbd3 = iniFile.Read(CCDName, "Deg3RChecked", FrmMain.propath);
                cbDeg3.Checked = (cbd3 == "True" ? true : false);
                if (cbDeg3.Checked)
                    tabControl1.SelectedIndex = 2;
                string deg3d = iniFile.Read(CCDName, "Deg3RDegree", FrmMain.propath);
                if (deg3d != "")
                {
                    tBDeg3RShift.Value = int.Parse(deg3d);
                    tBDeg3RDis.Value = int.Parse(iniFile.Read(CCDName, "Deg3RSetDis", FrmMain.propath));
                    tBDeg3RLen1.Value = int.Parse(iniFile.Read(CCDName, "Deg3RLength", FrmMain.propath));
                    tBDeg3RLen2.Value = int.Parse(iniFile.Read(CCDName, "Deg3RWidth", FrmMain.propath));
                    tBDeg3RB2W.Value = int.Parse(iniFile.Read(CCDName, "Deg3RgrayB2W", FrmMain.propath));
                    tBDeg3RW2B.Value = int.Parse(iniFile.Read(CCDName, "Deg3RgrayW2B", FrmMain.propath));
                }
                #endregion
                #region 辅助mode4
                string cbmark = iniFile.Read(CCDName, "MarkRChecked", FrmMain.propath);
                cBMark4.Checked = (cbmark == "True" ? true : false);
                if (cBMark4.Checked)
                    tabControl1.SelectedIndex = 3;
                string marksh = iniFile.Read(CCDName, "MarkRDegree", FrmMain.propath);
                if (marksh != "")
                {
                    UDMarkRShift.Value = int.Parse(marksh);
                    UDMarkRDis.Value = int.Parse(iniFile.Read(CCDName, "MarkRSetDis", FrmMain.propath));
                    UDMarkRLen1.Value = int.Parse(iniFile.Read(CCDName, "MarkRLength", FrmMain.propath));
                    UDMarkRLen2.Value = int.Parse(iniFile.Read(CCDName, "MarkRWidth", FrmMain.propath));
                    UDMarkNum.Value = int.Parse(iniFile.Read(CCDName, "MarkRCount", FrmMain.propath));
                    UDmarkRMin.Value = int.Parse(iniFile.Read(CCDName, "MarkRrMin", FrmMain.propath));
                    UDmarkB2W.Value = int.Parse(iniFile.Read(CCDName, "MarkRgray1", FrmMain.propath));
                    UDmarkW2B.Value = int.Parse(iniFile.Read(CCDName, "MarkRgray2", FrmMain.propath));
                }
                #endregion
                #region 找角度DegLine
                string dlc = iniFile.Read(CCDName, "DegLineChecked", FrmMain.propath);
                cBDegLine.Checked = (dlc == "True" ? true : false);
                if (cBDegLine.Checked)
                {
                    tabControl1.SelectedIndex = 0;
                    UDDegLth.Value = int.Parse(iniFile.Read(CCDName, "DegLineGray", FrmMain.propath));
                    UDDegLWidth.Value = int.Parse(iniFile.Read(CCDName, "DegLineWidth", FrmMain.propath));
                }
                #endregion
                #region 四点
                string d4 = iniFile.Read(CCDName, "Deg4Checked", FrmMain.propath);
                cBDeg4.Checked = (d4 == "True" ? true : false);
                if (cBDeg4.Checked)
                {
                    tabControl1.SelectedIndex = 1;
                    ucDeg4Shift.Value = int.Parse(iniFile.Read(CCDName, "Deg4AnglePlus", FrmMain.propath));
                    ucDeg4Dis.Value = int.Parse(iniFile.Read(CCDName, "Deg4AngleDis", FrmMain.propath));
                    ucDeg4AngleIntersection.Value = int.Parse(IniFile.Read(CCDName, "Deg4AngleIntersection","0", FrmMain.propath));
                }
                #endregion
                #region 胶点辨识
                string gcd = iniFile.Read(CCDName, "GlueChecked", FrmMain.propath);
                if (gcd != "")
                {
                    //cbGlue_Circle_2.Checked = bool.Parse(IniFile.Read(CCDName, "Glue_Circle_2", "false", FrmMain.propath));
                    cbGlue_Circle_2.Checked = bool.Parse("true");
                    ucGlue_Circle_OuterRadius_2.Value = int.Parse(IniFile.Read(CCDName, "Glue_Circle_OuterRadius_2", "1", FrmMain.propath));
                    ucGlue_Circle_InnerRadius_2.Value = int.Parse(IniFile.Read(CCDName, "Glue_Circle_InnerRadius_2", "1", FrmMain.propath));
                    ucGlue_Circle_StartAngle_2.Value = int.Parse(IniFile.Read(CCDName, "Glue_Circle_StartAngle_2", "0", FrmMain.propath));
                    ucGlue_Circle_EndAngle_2.Value = int.Parse(IniFile.Read(CCDName, "Glue_Circle_EndAngle_2", "180", FrmMain.propath));
                    ucGlueGray_2.Value = int.Parse(IniFile.Read(CCDName, "Glue_Circle_Gray_2", "1", FrmMain.propath));
                    nudGlueAngleRatio.Value = int.Parse(IniFile.Read(CCDName, "GlueAngleRatio", "0", FrmMain.propath));
                    nudGlueAngleRatio_2.Value = int.Parse(IniFile.Read(CCDName, "GlueAngleRatio_2", "0", FrmMain.propath));



                    Gluecheck.Checked = ((gcd == "True") ? true : false);
                    cBglueChose.SelectedIndex = int.Parse(iniFile.Read(CCDName, "GlueMode", FrmMain.propath));
                    string gcdis = iniFile.Read(CCDName, "GlueCenDistance", FrmMain.propath);
                    if (gcdis != "")
                    {
                        tBglueDis.Value = int.Parse(gcdis);
                        string feg = iniFile.Read(CCDName, "GlueFDegPlue", FrmMain.propath);
                        if (feg != "")
                            tBFDegPlus.Value = int.Parse(feg);
                        tBgluelen1.Value = int.Parse(iniFile.Read(CCDName, "GlueLength", FrmMain.propath));
                        tBgluelen2.Value = int.Parse(iniFile.Read(CCDName, "GlueWidth", FrmMain.propath));
                    }
                    string grr = iniFile.Read(CCDName, "GlueRingRadius", FrmMain.propath);
                    if (grr != "")
                    {
                        UDglueR.Value = int.Parse(grr);
                        UDglueW.Value = int.Parse(iniFile.Read(CCDName, "GlueRingWidth", FrmMain.propath));
                        UDglueStartA.Value = int.Parse(iniFile.Read(CCDName, "GlueStartA", FrmMain.propath));
                        UDglueEndA.Value = int.Parse(iniFile.Read(CCDName, "GlueEndA", FrmMain.propath));
                    }
                    string gblc = iniFile.Read(CCDName, "GlueEdgeChecked", FrmMain.propath);
                    cBGlueEdge.Checked = ((gblc == "True") ? true : false);
                    if (!cBGlueEdge.Checked)
                        gBErrorShow.Hide();
                    string gblin = iniFile.Read(CCDName, "GlueEdgeMin", FrmMain.propath);
                    if (gblin != "")
                    {
                        tBLimitOut.Value = int.Parse(iniFile.Read(CCDName, "GlueEdgeMax", FrmMain.propath));
                        tBLimitIn.Value = int.Parse(gblin);
                    }
                    string gbcolor = iniFile.Read(CCDName, "GlueColorChecked", FrmMain.propath);
                    cBGlueRegionColor.Checked = ((gbcolor == "True") ? true : false);
                    string tbg = iniFile.Read(CCDName, "GlueGray", FrmMain.propath);
                    if (tbg != "")
                        tBGlueGray.Value = int.Parse(tbg);
                    txtGlueMin.Text = iniFile.Read(CCDName, "GlueAreaMin", FrmMain.propath);
                    string gmax = iniFile.Read(CCDName, "GlueAreaMax", FrmMain.propath);
                    if (gmax != "")
                        txtGlueMax.Text = gmax;
                    else
                    {
                        if (txtGlueMin.Text != "0")
                            txtGlueMax.Text = (int.Parse(txtGlueMin.Text) + 100000).ToString();
                        else
                            txtGlueMax.Text = "100000";
                        iniFile.Write(CCDName, "GlueAreaMax", txtGlueMax.Text, FrmMain.propath);
                    }
                    txtInLimitMin.Text = iniFile.Read(CCDName, "GlueEdgeInAreaMax", FrmMain.propath);
                    txtOutLimitMin.Text = iniFile.Read(CCDName, "GlueEdgeOutAreaMax", FrmMain.propath);
                    cBGlueWidth.Checked = ((iniFile.Read(CCDName, "GlueWidthIschecked", FrmMain.propath) == "True") ? true : false);
                    txtCirRadius.Text = iniFile.Read(CCDName, "GlueWidthRradius", FrmMain.propath);
                    txtCirPixel.Text = iniFile.Read(CCDName, "GlueWidthPixel", FrmMain.propath);
                    txtWidthMax.Text = iniFile.Read(CCDName, "GlueWidthMax", FrmMain.propath);
                    cBIandORadius.Checked = ((iniFile.Read(CCDName, "GlueInOutRadius", FrmMain.propath) == "True") ? true : false);
                    txtIRmin.Text = iniFile.Read(CCDName, "GlueInRadiusMin", FrmMain.propath);
                    txtORMax.Text = iniFile.Read(CCDName, "GlueOutRadiusMax", FrmMain.propath);
                    string DisAddChecked = iniFile.Read(CCDName, "DisAddChecked", FrmMain.propath);
                    cBDisAdd.Checked = ((DisAddChecked == "True") ? true : false);
                    //if (cBDisAdd.Checked)
                    //{
                    string ta = iniFile.Read(CCDName, "DisAddDegree", FrmMain.propath);
                    tBAddAngle.Value = int.Parse((ta == "") ? "45" : ta);
                    string tap = iniFile.Read(CCDName, "DisAddDegreePlus", FrmMain.propath);
                    tBAddAnglePlus.Value = int.Parse((tap == "") ? "45" : tap);
                    string td = iniFile.Read(CCDName, "DisAddSetDis", FrmMain.propath);
                    tBAddDis.Value = int.Parse((td == "") ? "300" : td);
                    string tl1 = iniFile.Read(CCDName, "DisAddLength", FrmMain.propath);
                    tBAddlen1.Value = int.Parse((tl1 == "") ? "80" : tl1);
                    string tl2 = iniFile.Read(CCDName, "DisAddWidth", FrmMain.propath);
                    tBAddlen2.Value = int.Parse((tl2 == "") ? "80" : tl2);
                    txtDisMin.Text = iniFile.Read(CCDName, "DisAddDisMin", FrmMain.propath);
                    txtDisMax.Text = iniFile.Read(CCDName, "DisAddDisMax", FrmMain.propath);
                    //}
                    string gbch = iniFile.Read(CCDName, "GlueBreakChecked", FrmMain.propath);
                    cBGlueBreaked.Checked = (gbch == "False" ? false : true);
                    //膠水內外徑補償
                    Glue.Offset_InnerRadius = double.Parse(IniFile.Read(CCDName, "Offset_InnerRadius", "0", FrmMain.propath));
                    Glue.Offset_OuterRadius = double.Parse(IniFile.Read(CCDName, "Offset_OuterRadius", "0", FrmMain.propath));
                }
                #endregion
                #region  外观检测
                string gcic = iniFile.Read(CCDName, "GlueCIChecked", FrmMain.propath);
                GlueOutcheck.Checked = ((gcic == "True") ? true : false);
                string gcim = iniFile.Read(CCDName, "GlueCIMode", FrmMain.propath);
                string bn = iniFile.Read(CCDName, "jBaseNum", FrmMain.propath);
                Numbase = double.Parse((bn == "") ? "7.51" : bn);
                if (bn == "")
                    iniFile.Write(CCDName, "jBaseNum", "7.51", FrmMain.propath);
                string pn = iniFile.Read(CCDName, "jPlusNum", FrmMain.propath);
                Numadd = double.Parse((pn == "") ? "0.07" : pn);
                if (pn == "")
                    iniFile.Write(CCDName, "jPlusNum", "0.07", FrmMain.propath);
                string tofs = iniFile.Read(CCDName, "jallCoatOffset", FrmMain.propath);
                if (tofs == "")
                    iniFile.Write(CCDName, "jallCoatOffset", "0.04", FrmMain.propath);
                coatos = double.Parse((tofs == "") ? "0.04" : tofs);
                if (gcim != "" & gcim == "1")
                {
                    cBGOutMode.Value = int.Parse(gcim);
                    txtAimCirR.Text = iniFile.Read(CCDName, "GlueCIAimCirRadius", FrmMain.propath);
                    txtOutCirR.Text = iniFile.Read(CCDName, "GlueCIOutCirRadius", FrmMain.propath);
                    string r2c = iniFile.Read(CCDName, "GlueCIOutCir2RChecked", FrmMain.propath);
                    cBAVIR2.Checked = (r2c == "True" ? true : false);
                    txtOutCirR2.Text = iniFile.Read(CCDName, "GlueCIOutCir2Radius", FrmMain.propath);
                    FrmVisionSet.txtOutP = double.Parse(iniFile.Read(CCDName, "GlueCIOutCirRadiusPlus", FrmMain.propath));
                    UDGlueOutWidth.Value = int.Parse(iniFile.Read(CCDName, "GlueCIOutWidth", FrmMain.propath));
                    UDGlueOutGray.Value = int.Parse(iniFile.Read(CCDName, "GlueCIOutGray", FrmMain.propath));
                    txtGlueOutAreaMax.Text = iniFile.Read(CCDName, "GlueCIOutAreamax", FrmMain.propath);
                    UD_LED9Lig.Value = int.Parse(iniFile.Read(CCDName, "LighterValue", FrmMain.propath));
                    txtPpix.Text = iniFile.Read(CCDName, "GlueCIPixel", FrmMain.propath);

                    string cc = iniFile.Read(CCDName, "CoatCIChecked", FrmMain.propath);
                    Coatcheck.Checked = (cc == "True" ? true : false);
                    string tr = iniFile.Read(CCDName, "CoatRingRadius", FrmMain.propath);
                    tBCoatRadius.Value = int.Parse((tr == "") ? "100" : tr);
                    string tw = iniFile.Read(CCDName, "CoatRingWidth", FrmMain.propath);
                    tBCoatWidth.Value = int.Parse((tw == "") ? "100" : tw);
                    txtCoatRMin.Text = iniFile.Read(CCDName, "CoatRmin", FrmMain.propath);
                    string tcrm = iniFile.Read(CCDName, "CoatRmax", FrmMain.propath);
                    if (tcrm == "")
                        iniFile.Write(CCDName, "CoatRmax", "7.75", FrmMain.propath);
                    txtCoatRMax.Text = ((tcrm == "") ? "7.75" : tcrm);
                    hv_transition = (HTuple)iniFile.Read(CCDName, "CoatTransition", FrmMain.propath);
                    string tgray = iniFile.Read(CCDName, "CoatRingThreshold", FrmMain.propath);
                    if (hv_transition == "negative")
                        tBCoatB2W.Value = int.Parse((tgray == "") ? "1" : tgray);
                    if (hv_transition == "positive")
                        tBCoatW2B.Value = int.Parse((tgray == "") ? "255" : tgray);

                    txtDMinmax.Text = iniFile.Read(CCDName, "DiamMinMax", FrmMain.propath);
                    txtDMinmin.Text = iniFile.Read(CCDName, "DiamMinMin", FrmMain.propath);
                }
                if (CCDName == "PCCD2-PickUp")
                    cBAVI.Checked = ((iniFile.Read(CCDName, "PickUpAVI", FrmMain.propath) == "True") ? true : false);
                string bn2 = iniFile.Read(CCDName, "jBaseNum", FrmMain.propath);
                Numbase2 = double.Parse((bn2 == "") ? "7.51" : bn2);
                if (bn2 == "")
                    iniFile.Write(CCDName, "jBaseNum", "7.51", FrmMain.propath);
                string pn2 = iniFile.Read(CCDName, "jPlusNum", FrmMain.propath);
                Numadd2 = double.Parse((pn2 == "") ? "0.07" : pn2);
                if (pn2 == "")
                    iniFile.Write(CCDName, "jPlusNum", "0.07", FrmMain.propath);
                string tofs2 = iniFile.Read(CCDName, "jallCoatOffset", FrmMain.propath);
                if (tofs2 == "")
                    iniFile.Write(CCDName, "jallCoatOffset", "0.04", FrmMain.propath);
                coatos2 = double.Parse((tofs2 == "") ? "0.04" : tofs2);
                if (gcim != "" & gcim == "2")
                {
                    string cc = iniFile.Read(CCDName, "HCoatCIChecked", FrmMain.propath);
                    Coatcheck2.Checked = (cc == "True" ? true : false);
                    string tr2 = iniFile.Read(CCDName, "HCoatRingRadius", FrmMain.propath);
                    tBCoatRadius2.Value = int.Parse((tr2 == "") ? "100" : tr2);
                    string tw2 = iniFile.Read(CCDName, "HCoatRingWidth", FrmMain.propath);
                    tBCoatWidth2.Value = int.Parse((tw2 == "") ? "100" : tw2);
                    txtCoatRMin2.Text = iniFile.Read(CCDName, "HCoatRmin", FrmMain.propath);
                    string tcrm = iniFile.Read(CCDName, "HCoatRmax", FrmMain.propath);
                    txtCoatRMax2.Text = ((tcrm == "") ? "7.75" : tcrm);
                    if (tcrm == "")
                        iniFile.Write(CCDName, "HCoatRmax", "7.75", FrmMain.propath);
                    hv_transition = (HTuple)iniFile.Read(CCDName, "HCoatTransition", FrmMain.propath);
                    string tgray2 = iniFile.Read(CCDName, "HCoatRingThreshold", FrmMain.propath);
                    if (hv_transition == "negative")
                        tBCoatB2W2.Value = int.Parse((tgray2 == "") ? "1" : tgray2);
                    if (hv_transition == "positive")
                        tBCoatW2B2.Value = int.Parse((tgray2 == "") ? "255" : tgray2);

                    txtDMinmax2.Text = iniFile.Read(CCDName, "HCDiamMinMax", FrmMain.propath);
                    txtDMinmin2.Text = iniFile.Read(CCDName, "HCDiamMinMin", FrmMain.propath);

                    txtLensAimR.Text = iniFile.Read(CCDName, "LensAimR", FrmMain.propath);
                    txtLenspix.Text = iniFile.Read(CCDName, "LensPix", FrmMain.propath);
                    string thr = iniFile.Read(CCDName, "HoleRingRadius", FrmMain.propath);
                    tBHoleRadius.Value = int.Parse((thr == "") ? "100" : thr);
                    string thw = iniFile.Read(CCDName, "HoleRingWidth", FrmMain.propath);
                    tBHoleWidth.Value = int.Parse((thw == "") ? "100" : thw);
                    hv_Holetransition = (HTuple)iniFile.Read(CCDName, "HoleTransition", FrmMain.propath);
                    string thgray = iniFile.Read(CCDName, "HoleRingThreshold", FrmMain.propath);
                    if (hv_Holetransition == "negative")
                        tBHoleB2W.Value = int.Parse((thgray == "") ? "1" : thgray);
                    if (hv_Holetransition == "positive")
                        tBHoleW2B.Value = int.Parse((thgray == "") ? "255" : thgray);
                }
                #endregion
                #region 外观检测pccd1
                string strp1disMode2 = iniFile.Read(CCDName, "DisMode2Checked", FrmMain.propath);
                if (CCDName == "PCCD1" & strp1disMode2 != "")
                {
                    cBP1DisMode2.Checked = (strp1disMode2 == "True" ? true : false);
                    tBP1lensRRadius.Value = int.Parse(iniFile.Read(CCDName, "LensRingRadius", FrmMain.propath));
                    tBP1LensWidth.Value = int.Parse(iniFile.Read(CCDName, "LensRingWidth", FrmMain.propath));
                    hv_transition = iniFile.Read(CCDName, "LensTransition", FrmMain.propath);
                    tBP1LensRmin.Value = int.Parse(iniFile.Read(CCDName, "LensZoneRmin", FrmMain.propath));
                    tBP1LensRmax.Value = int.Parse(iniFile.Read(CCDName, "LensZoneRmax", FrmMain.propath));
                    if (hv_transition == "negative")
                        tBP1LensB2W.Value = int.Parse(iniFile.Read(CCDName, "LensRingThreshold", FrmMain.propath));
                    if (hv_transition == "positive")
                        tBP1LensW2B.Value = int.Parse(iniFile.Read(CCDName, "LensRingThreshold", FrmMain.propath));
                }
                #endregion
                #region 外观检测QCCD
                //固定环
                cBQAVI1.Checked = QCCD.AVI1IsCheck;
                tbPF1InRange.Value = QCCD.dInRange;
                tbPF1OutRange.Value = QCCD.dOutRange;
                cbDetection_Black.Checked = QCCD.Detection_Black;
                cbDetection_White.Checked = QCCD.Detection_White;
                tbGraythresholdBlack.Value = QCCD.dGraythresholdBlack;
                tbGraythresholdWhite.Value = QCCD.dGraythresholdWhite;
                tbUnderSizeArea.Value = QCCD.dUnderSizeArea;
                nudGlueAngleSet.Value = QCCD.iGlueAngleSet;
                nudGlueRatioSet.Value = QCCD.iGlueRatioSet;
                nudAngleSet.Value = QCCD.dAngleSet;
                //小台阶
                cBQAVI2.Checked = QCCD.AVI2IsCheck;
                tbPFInRange.Value = QCCD.dInRangePF;
                tbPFOutRange.Value = QCCD.dOutRangePF;
                cbDetectionPF_Dark2.Checked = QCCD.DetectionPF_Dark2;
                cbDetectionPF_Light2.Checked = QCCD.DetectionPF_Light2;
                cbDetectionPF_Black2.Checked = QCCD.DetectionPF_Black;
                cbDetectionPF_White2.Checked = QCCD.DetectionPF_White;
                cbClosingPF2.Checked = QCCD.ClosingPF2;
                cbOpeningPF2.Checked = QCCD.OpeningPF2;

                nudDynthresholdDarkPF2.Value = QCCD.iDynthresholdDarkPF2;
                nudDynthresholdLightPF2.Value = QCCD.iDynthresholdLightPF2;
                tbGraythresholdBlackPF2.Value = QCCD.iGraythresholdBlackPF2;
                tbGraythresholdWhitePF2.Value = QCCD.iGraythresholdWhitePF2;
                nudCloseWidthPF2.Value = QCCD.iCloseWidthPF2;
                nudCloseHeightPF2.Value = QCCD.iCloseHeightPF2;
                nudOpenWidthPF2.Value = QCCD.iOpenWidthPF2;
                nudOpenHeightPF2.Value = QCCD.iOpenHeightPF2;
                tbUnderSizeAreaPF2.Value = QCCD.iUnderSizeAreaPF2;
                nudGlueAngleSetPF.Value = QCCD.iGlueAngleSetPF;
                nudGlueRatioSetPF.Value = QCCD.iGlueRatioSetPF;
                nudAngleSetPF.Value = QCCD.dAngleSetPF;
                #endregion
                #region Square
                if (fs == "Square")
                {
                    //udFMode.Value = int.Parse(smode);
                    UDSWidth.Value = int.Parse(iniFile.Read(CCDName, "FWidth", FrmMain.propath));
                    ucRectangleLength1_FigureShape.Value = int.Parse(IniFile.Read(CCDName, "RectangleLength1_FigureShape","100", FrmMain.propath));
                    ucRectangleLength2_FigureShape.Value = int.Parse(IniFile.Read(CCDName, "RectangleLength2_FigureShape","100", FrmMain.propath));
                    string ftr = iniFile.Read(CCDName, "Ftransition", FrmMain.propath);
                    if (ftr == "negative")
                        UDSB2W.Value = int.Parse(iniFile.Read(CCDName, "Fthreshold", FrmMain.propath));
                    if (ftr == "positive")
                        UDSW2B.Value = int.Parse(iniFile.Read(CCDName, "Fthreshold", FrmMain.propath));
                     string fselect = IniFile.Read(CCDName, "Fselect", "last", FrmMain.propath);
                     if (fselect == "first")
                     {
                         cb.SelectedIndex = 0;
                     }
                     else
                     {
                         cb.SelectedIndex = 1;
                     }
                     
                }
                #endregion
                #endregion
            }
            catch
            {
                MessageBox.Show("参数读取异常，请检查参数或重新设置参数！");
            }
            readpara = false;
        }
        private void cBSetName_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabFigureShape.SelectedIndex = cbFigureShape.SelectedIndex;
        }
        private void tabVisionSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((SetNum == "2" || SetNum == "4" || SetNum == "5" || SetNum == "6" || SetNum == "9") & tabVisionSet.SelectedIndex == 3)
                tabVisionSet.SelectedIndex = 3;
            else
                if (tabVisionSet.SelectedIndex == 3)
                    tabVisionSet.SelectedIndex = 2;
            if (cbFigureShape.SelectedIndex == 3  & (SetNum == "2" || SetNum == "4"))
                cBLocation.SelectedIndex = 0;
        }

        #region 图像保存
        private void SaveOfigure_CheckedChanged(object sender, EventArgs e)
        {
            if (cBfigure.SelectedIndex < 0)
                return;
            if (Sys.CurrentProduction == "")
                return;
            string CCDName = ""; bool Sof = false;
            Sof = ((SaveOfigure.Checked) ? true : false);
            #region CCDchoose
            if (cBfigure.SelectedIndex == 0)
            {
                CCDName = "A1CCD1";
                A1CCD1.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 1)
            {
                CCDName = "A1CCD2";
                A1CCD2.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 2)
            {
                CCDName = "A2CCD1";
                A2CCD1.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 3)
            {
                CCDName = "A2CCD2";
                A2CCD2.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 4)
            {
                CCDName = "PCCD1";
                PCCD1.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 5)
            {
                CCDName = "PCCD2";
                PCCD2.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 6)
            {
                CCDName = "GCCD1";
                GCCD1.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 7)
            {
                CCDName = "GCCD2";
                GCCD2.SaveOf = Sof;
            }
            if (cBfigure.SelectedIndex == 8)
            {
                CCDName = "QCCD";
                QCCD.SaveOf = Sof;
            }
            #endregion
            iniFile.Write(CCDName, "SaveOfigure", Sof.ToString(), FrmMain.propath);
        }
        private void SaveRfigure_CheckedChanged(object sender, EventArgs e)
        {
            if (cBfigure.SelectedIndex < 0)
                return;
            if (Sys.CurrentProduction == "")
                return;
            string CCDName = ""; bool Srf = false;
            Srf = ((SaveRfigure.Checked) ? true : false);
            #region CCDchoose
            if (cBfigure.SelectedIndex == 0)
            {
                CCDName = "A1CCD1";
                A1CCD1.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 1)
            {
                CCDName = "A1CCD2";
                A1CCD2.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 2)
            {
                CCDName = "A2CCD1";
                A2CCD1.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 3)
            {
                CCDName = "A2CCD2";
                A2CCD2.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 4)
            {
                CCDName = "PCCD1";
                PCCD1.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 5)
            {
                CCDName = "PCCD2";
                PCCD2.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 6)
            {
                CCDName = "GCCD1";
                GCCD1.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 7)
            {
                CCDName = "GCCD2";
                GCCD2.SaveRf = Srf;
            }
            if (cBfigure.SelectedIndex == 8)
            {
                CCDName = "QCCD";
                QCCD.SaveRf = Srf;
            }
            #endregion
            iniFile.Write(CCDName, "SaveRfigure", Srf.ToString(), FrmMain.propath);
        }
        private void cBfigure_SelectedIndexChanged(object sender, EventArgs e)
        {
            string CCDName = "";
            if (cBfigure.SelectedIndex == 0)
                CCDName = "A1CCD1";
            if (cBfigure.SelectedIndex == 1)
                CCDName = "A1CCD2";
            if (cBfigure.SelectedIndex == 2)
                CCDName = "A2CCD1";
            if (cBfigure.SelectedIndex == 3)
                CCDName = "A2CCD2";
            if (cBfigure.SelectedIndex == 4)
                CCDName = "PCCD1";
            if (cBfigure.SelectedIndex == 5)
                CCDName = "PCCD2";
            if (cBfigure.SelectedIndex == 6)
                CCDName = "GCCD1";
            if (cBfigure.SelectedIndex == 7)
                CCDName = "GCCD2";
            if (cBfigure.SelectedIndex == 8)
                CCDName = "QCCD";
            string Sof = iniFile.Read(CCDName, "SaveOfigure", FrmMain.propath);
            SaveOfigure.Checked = ((Sof == "True") ? true : false);
            string Srf = iniFile.Read(CCDName, "SaveRfigure", FrmMain.propath);
            SaveRfigure.Checked = ((Srf == "True") ? true : false);
        }
        #endregion

        #region 图像处理/校正
        int binBvi = 128;
        HObject ho_ImageSet = new HObject(), ho_ImageTest = new HObject(), ho_Rcircle = new HObject();
        HObject ho_RegionReduced = new HObject(), ho_Border = new HObject();
        HTuple RegionRadius = new HTuple();
        HTuple width = new HTuple(), height = new HTuple(), area = new HTuple(), row = new HTuple(), col = new HTuple();
        #region 变量
        HTuple hv_Number1 = new HTuple(), hv_LengthMax1 = new HTuple(), hv_Length1 = new HTuple(), hv_fitRow = new HTuple(), hv_fitCol = new HTuple(), hv_fitRadius = new HTuple();
        HTuple hv_StartPhi = new HTuple(), hv_EndPhi = new HTuple(), hv_PointOrder = new HTuple();
        HTuple hv_RingRow = new HTuple(), hv_RingCol = new HTuple(), hv_RingRadius = new HTuple(), hv_RDetectHeight = new HTuple(), hv_transition = "all";
        //HTuple hv_DistanceStart, hv_DistanceEnd, hv_RLength, hv_RLength2, hv_RResultRow, hv_RResultColumn, hv_Elements;
        //HTuple hv_i, hv_j, hv_k, hv_RowXLD, hv_ColXLD;
        //HTuple hv_RowE, hv_ColE, hv_RATan, hv_Direct, hv_RradiusIn, hv_RradiusOut;
        //HTuple hv_RowL2, hv_RowL1, hv_ColL2, hv_ColL1;//计算的线的坐标;
        //HTuple hv_ROIWidth, hv_TmpCtrl_Row, hv_TmpCtrl_Column, hv_TmpCtrl_Dr, hv_TmpCtrl_Dc, hv_TmpCtrl_Phi;
        //HTuple hv_TmpCtrl_Len1, hv_TmpCtrl_Len2, hv_MsrHandle_Measure_02_0; 
        //HTuple hv_RRowEdge, hv_RColEdge, hv_Amplitude, hv_Distance, hv_tRow, hv_tCol,  hv_Number;
        //HTuple hv_Length, hv_Attrib;
        HTuple hv_t = new HTuple(), hv_RowCenter = 0, hv_ColCenter = 0, hv_CenterRadius = 0, hv_ActiveNum = 15, hv_ArcType = "circle", hv_RAmplitudeThreshold = 68;


        HObject ho_ContoursSplit = new HObject(), ho_SelectedXLD = new HObject(), ho_ObjectSelectedM = new HObject(), ho_ObjectSelectedN = new HObject();
        HObject ho_RContCircle = new HObject(), ho_RRegions = new HObject(), ho_RCircleIn = new HObject(), ho_RCircleOut = new HObject();
        HObject ho_RCircle = new HObject(), ho_RCircle0 = new HObject(), ho_Contour = new HObject(), ho_SortedContours = new HObject();
        HObject ho_XLDTrans = new HObject(), ho_Arrow1 = new HObject();
        HObject[] OTemp = new HObject[20]; long SP_O = 0;
        HObject ho_Circle = new HObject(), ho_ObjectSelected2 = new HObject();
        #endregion
        #region 基础设置
        private void NbtnDrawRe_Click(object sender, EventArgs e)
        {
            if (tBPRegion.Value == 0)
            {

            }
            else
            {
                HD.disp_message(hWVision, "请调节检测区域半径。", "", 100, 100, "green", "false");
                DrawRegion();
            }
        }
        private void tBPRegion_ValueChanged(object sender, EventArgs e)
        {
            RegionRadius = tBPRegion.Value;
            UDPRegion.Value = RegionRadius;
        }
        private void UDPRegion_ValueChanged(object sender, EventArgs e)
        {
            RegionRadius = (HTuple)UDPRegion.Value;
            tBPRegion.Value = RegionRadius;
            DrawRegion();
        }
        private void tBthreshold_ValueChanged(object sender, EventArgs e)
        {
            binBvi = (int)tBthreshold.Value;
            UDthreshold.Value = binBvi;
        }
        private void UDthreshold_ValueChanged(object sender, EventArgs e)
        {
            binBvi = (int)UDthreshold.Value;
            tBthreshold.Value = binBvi;
            binBviShow();
        }
        public void DrawRegion()
        {
            if (readpara)
                return;
            if (ho_ImageSet == null)
                return;
            int i = int.Parse(ViewNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);

            hWVision.ClearWindow();
            hWVision.SetColor("red");
            HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
            ho_RegionReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
            hWVision.DispObj(ho_RegionReduced);
            if (halcon.IsCrossDraw)
            {
                HOperatorSet.SetColor(hWVision, "red");
                HD.CrossDraw(hWVision, width, height);
            }
        }
        public void binBviShow()
        {
            if (readpara)
                return;
            int i = int.Parse(ViewNum) - 1;
            HOperatorSet.GenEmptyObj(out ho_ImageSet);
            HOperatorSet.CopyImage(halcon.Image[i], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            try
            {
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (RegionRadius != 0)
                {
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                ho_Border.Dispose();
                HOperatorSet.ThresholdSubPix(ho_ImageTest, out ho_Border, binBvi);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageTest);
                hWVision.SetColor("red");
                hWVision.DispObj(ho_Border);
                if (halcon.IsCrossDraw)
                {
                    HOperatorSet.SetColor(hWVision, "red");
                    HD.CrossDraw(hWVision, width, height);
                }
            }
            catch
            {
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
            }
        }
        private void btnDrawViewCircle_Click(object sender, EventArgs e)
        {
            #region
            int i_image = int.Parse(ViewNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            ho_ImageTest.Dispose();
            HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
            if (RegionRadius != 0)
            {
                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
            }
            try
            {
                ho_Border.Dispose();
                HOperatorSet.ThresholdSubPix(ho_ImageTest, out ho_Border, binBvi);
                ho_ContoursSplit.Dispose();
                HOperatorSet.SegmentContoursXld(ho_Border, out ho_ContoursSplit, "lines_circles", 5, 4, 3);
                ho_SelectedXLD.Dispose();
                HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_SelectedXLD, "contlength", "and", 50, 99999);
                HOperatorSet.CountObj(ho_SelectedXLD, out hv_Number1);
                HOperatorSet.GenEmptyObj(out ho_selectCir);
                for (int i = 1; i <= (int)hv_Number1; i++)
                {
                    ho_selectContour.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedXLD, out ho_selectContour, i);
                    HTuple hv_attrib;
                    HOperatorSet.GetContourGlobalAttribXld(ho_selectContour, "cont_approx", out hv_attrib);
                    if (hv_attrib.D == 1)
                        HOperatorSet.ConcatObj(ho_selectCir, ho_selectContour, out ho_selectCir);
                }
                HOperatorSet.CountObj(ho_selectCir, out hv_Number1);
                ho_ObjectSelectedM.Dispose();
                HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedM, 1);
                HOperatorSet.LengthXld(ho_ObjectSelectedM, out hv_LengthMax1);
                HTuple hv_max_L = 0.0;
                hv_max_L = hv_LengthMax1.Clone();
                for (int i = 2; i < (int)hv_Number1; i++)
                {
                    ho_ObjectSelectedN.Dispose();
                    HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedN, i);
                    HOperatorSet.LengthXld(ho_ObjectSelectedN, out hv_Length1);
                    if ((int)(new HTuple(hv_Length1.TupleGreater(hv_max_L))) != 0)
                    {
                        hv_max_L = hv_Length1.Clone();
                        ho_ObjectSelectedM.Dispose();
                        HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedM, i);
                    }
                }
                try
                {
                    HOperatorSet.FitCircleContourXld(ho_ObjectSelectedM, "algebraic", -1, 0, 0,
                        3, 2, out hv_fitRow, out hv_fitCol, out hv_fitRadius, out hv_StartPhi, out hv_EndPhi,
                        out hv_PointOrder);
                    hv_RingRow = hv_fitRow; hv_RingCol = hv_fitCol;
                    hv_RingRadius = hv_fitRadius;
                    UDViewRa.Value = (int)Math.Round((double)hv_fitRadius);
                    hv_RDetectHeight = (int)UDViewWidth.Value;
                    hv_transition = "all";
                    if (tBViewB2W.Value != 1)
                        hv_transition = "negative";
                    if (tBViewW2B.Value != 255)
                        hv_transition = "positive";
                    ShowRing();
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.ToString());
                    //
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("请重新调节初始灰度阀值！" + ex.ToString());
            }
            #endregion
        }
        #region old
        //public void ShowRing()
        //{
        //    if (readpara)
        //        return;
        //    int i_image = int.Parse(ViewNum) - 1;
        //    ho_ImageSet.Dispose();
        //    HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
        //    HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
        //    HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
        //    ho_ImageTest.Dispose();
        //    HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
        //    if (RegionRadius != 0)
        //    {
        //        HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
        //        ho_RegionReduced.Dispose();
        //        HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
        //        ho_ImageTest.Dispose();
        //        HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
        //    }
        //    hWVision.ClearWindow();
        //    hWVision.DispObj(ho_ImageTest);
        //    ho_RRegions.Dispose();
        //    HOperatorSet.GenEmptyObj(out ho_RRegions);
        //    ho_RContCircle.Dispose();
        //    HOperatorSet.GenCircleContourXld(out ho_RContCircle, hv_RingRow, hv_RingCol, hv_RingRadius, 0, 6.28318, "positive", 1);
        //    OTemp[SP_O] = ho_RRegions.CopyObj(1, -1);
        //    SP_O++;
        //    ho_RRegions.Dispose();
        //    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_RContCircle, out ho_RRegions);
        //    OTemp[SP_O - 1].Dispose();
        //    SP_O = 0;
        //    HOperatorSet.GetContourXld(ho_RContCircle, out hv_RowXLD, out hv_ColXLD);
        //    //假设一条平行于x的直线，起点和终点分别是圆最右侧点的两侧  275
        //    HOperatorSet.DistancePp(hv_RingRow, hv_RingRow, (hv_RingRow + hv_RingRadius) + 10, hv_RingCol, out hv_DistanceStart);
        //    HOperatorSet.DistancePp(hv_RingRow, hv_RingRow, (hv_RingRow + hv_RingRadius) - 10, hv_RingCol, out hv_DistanceEnd);
        //    //计算圆形的轮廓长度
        //    HOperatorSet.LengthXld(ho_RContCircle, out hv_RLength);
        //    HOperatorSet.TupleLength(hv_ColXLD, out hv_RLength2);
        //    hv_RResultRow = new HTuple();
        //    hv_RResultColumn = new HTuple();
        //    hv_Elements = 180;
        //    for (hv_i = 0; hv_i.Continue(hv_Elements - 1, hv_Elements / 180); hv_i = hv_i.TupleAdd(hv_Elements / 180))
        //    {
        //        if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(hv_RLength2 - 1)))) != 0)
        //            HOperatorSet.TupleInt(((1.0 * hv_RLength2) / 179) * hv_i, out hv_j);
        //        else
        //            HOperatorSet.TupleInt(((1.0 * hv_RLength2) / 179) * hv_i, out hv_j);
        //        if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_RLength2))) != 0)
        //            hv_j = hv_RLength2 - 1;
        //        hv_RowE = hv_RowXLD.TupleSelect(hv_j);
        //        hv_ColE = hv_ColXLD.TupleSelect(hv_j);
        //        //超出图像区域，不检测，否则容易报异常
        //        if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(height - 1))).TupleOr(
        //            new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
        //            width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
        //        {
        //            continue;
        //        }
        //        //比较距离来计算角度
        //        if ((int)(new HTuple(hv_DistanceStart.TupleGreater(hv_DistanceEnd))) != 0)
        //        {
        //            //以开始点Row1为原点，来计算线的角度
        //            HOperatorSet.TupleAtan2((-hv_RowE) + hv_RingRow, hv_ColE - hv_RingCol, out hv_RATan);
        //            hv_RATan = ((new HTuple(180)).TupleRad()) + hv_RATan;
        //            hv_Direct = "inner";
        //        }
        //        else
        //        {
        //            //以结束点Row2为原点，来计算线的角度
        //            HOperatorSet.TupleAtan2((-hv_RowE) + hv_RingRow, hv_ColE - hv_RingCol, out hv_RATan);
        //            hv_Direct = "outer";
        //        }
        //        if ((int)(new HTuple(hv_i.TupleGreaterEqual(0))) != 0)
        //        {
        //            hv_RowL1 = hv_RowE + (hv_RDetectHeight * (((-hv_RATan)).TupleSin()));
        //            hv_RowL2 = hv_RowE - (hv_RDetectHeight * (((-hv_RATan)).TupleSin()));
        //            hv_ColL1 = hv_ColE + (hv_RDetectHeight * (((-hv_RATan)).TupleCos()));
        //            hv_ColL2 = hv_ColE - (hv_RDetectHeight * (((-hv_RATan)).TupleCos()));
        //            hv_RradiusIn = Math.Sqrt(Math.Abs((double)hv_RowL1 - (double)hv_RingRow) * Math.Abs((double)hv_RowL1 - (double)hv_RingRow) +
        //        Math.Abs((double)hv_ColL1 - (double)hv_RingCol) * Math.Abs((double)hv_ColL1 - (double)hv_RingCol));
        //            HOperatorSet.GenCircleContourXld(out ho_RCircleIn, hv_RingRow, hv_RingCol, hv_RradiusIn, 0, 6.28318, "positive", 1);
        //            hv_RradiusOut = Math.Sqrt(Math.Abs((double)hv_RowL2 - (double)hv_RingRow) * Math.Abs((double)hv_RowL2 - (double)hv_RingRow) +
        //        Math.Abs((double)hv_ColL2 - (double)hv_RingCol) * Math.Abs((double)hv_ColL2 - (double)hv_RingCol));
        //            HOperatorSet.GenCircleContourXld(out ho_RCircleOut, hv_RingRow, hv_RingCol, hv_RradiusOut, 0, 6.28318, "positive", 1);
        //            if (HDevWindowStack.IsOpen())
        //                HOperatorSet.SetColor(HDevWindowStack.GetActive(), "green");
        //            ho_Arrow1.Dispose();
        //            HD.gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2, 25, 25);
        //            OTemp[SP_O] = ho_RRegions.CopyObj(1, -1);
        //            SP_O++;
        //            ho_RRegions.Dispose();
        //            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow1, out ho_RRegions);
        //            OTemp[SP_O - 1].Dispose();
        //            SP_O = 0;
        //            hv_ROIWidth = 14;
        //            HOperatorSet.SetSystem("int_zooming", "true");
        //            hv_TmpCtrl_Row = 0.5 * (hv_RowL2 + hv_RowL1);
        //            hv_TmpCtrl_Column = 0.5 * (hv_ColL2 + hv_ColL1);
        //            hv_TmpCtrl_Dr = hv_RowL1 - hv_RowL2;
        //            hv_TmpCtrl_Dc = hv_ColL2 - hv_ColL1;
        //            hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(hv_TmpCtrl_Dc);
        //            hv_TmpCtrl_Len1 = 0.5 * ((((hv_TmpCtrl_Dr * hv_TmpCtrl_Dr) + (hv_TmpCtrl_Dc * hv_TmpCtrl_Dc))).TupleSqrt()
        //                );
        //            hv_TmpCtrl_Len2 = hv_ROIWidth.Clone();
        //            HOperatorSet.GenMeasureRectangle2(hv_TmpCtrl_Row, hv_TmpCtrl_Column, hv_TmpCtrl_Phi,
        //                hv_TmpCtrl_Len1, hv_TmpCtrl_Len2, 2592, 1944, "nearest_neighbor", out hv_MsrHandle_Measure_02_0);
        //            OTemp[SP_O] = ho_ImageTest.CopyObj(1, -1);
        //            SP_O++;
        //            ho_ImageTest.Dispose();
        //            HOperatorSet.CopyObj(OTemp[SP_O - 1], out ho_ImageTest, 1, 1);
        //            OTemp[SP_O - 1].Dispose();
        //            SP_O = 0;
        //            if (hv_transition == "")
        //                hv_transition = "all";
        //            HOperatorSet.MeasurePos(ho_ImageTest, hv_MsrHandle_Measure_02_0, 1, hv_RAmplitudeThreshold, hv_transition,
        //                "all", out hv_RRowEdge, out hv_RColEdge, out hv_Amplitude, out hv_Distance);
        //            HOperatorSet.CloseMeasure(hv_MsrHandle_Measure_02_0);
        //            hv_tRow = 0;
        //            hv_tCol = 0;
        //            hv_t = 0;
        //            HOperatorSet.TupleLength(hv_RRowEdge, out hv_Number);
        //            if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
        //            {
        //                continue;
        //            }
        //            //循环求
        //            for (hv_k = 0; hv_k.Continue(hv_Number - 1, 1); hv_k = hv_k.TupleAdd(1))
        //            {
        //                if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
        //                    hv_t))) != 0)
        //                {
        //                    hv_tRow = hv_RRowEdge.TupleSelect(hv_k);
        //                    hv_tCol = hv_RColEdge.TupleSelect(hv_k);
        //                    hv_t = ((hv_Amplitude.TupleSelect(hv_k))).TupleAbs();
        //                }
        //            }
        //            hWVision.SetColor("blue");
        //            hWVision.DispCircle(hv_tRow, hv_tCol, 8);
        //            if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
        //            {
        //                hv_RResultRow = hv_RResultRow.TupleConcat(hv_tRow);
        //                hv_RResultColumn = hv_RResultColumn.TupleConcat(hv_tCol);
        //            }
        //        }
        //    }
        //    hWVision.SetColor("green");
        //    hWVision.DispObj(ho_RRegions);
        //    hWVision.DispObj(ho_RCircleIn);
        //    hWVision.DispObj(ho_RCircleOut);
        //}
        #endregion
        #region new
        HObject ho_ModelContour = new HObject(), ho_MeasureContour = new HObject(), ho_Cross = new HObject();
        HObject ho_UsedEdges = new HObject(), ho_ResultContours = new HObject(), ho_CrossCenter = new HObject();
        HTuple hv_MetrologyHandle = null, hv_circleIndices = null;
        HTuple hv_circleParameter = null, hv_Row1 = null, hv_Column1 = null, hv_UsedRow = null, hv_UsedColumn = null;
        public void ShowRing()
        {
            if (readpara)
                return;
            try
            {
                int i_image = int.Parse(ViewNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBPRegion.Value;
                    ho_Rcircle.Dispose();
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                //HOperatorSet.GenCircleContourXld(out ho_RContCircle, hv_RingRow, hv_RingCol, hv_RingRadius, 0, 6.28318, "positive", 1);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.GenCircle(out ho_Circle, hv_RingRow, hv_RingCol, hv_RingRadius);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour, hv_MetrologyHandle, "all", 1.5);
                ho_MeasureContour.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_MeasureContour, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 10);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.5);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_Contour.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour, hv_MetrologyHandle, "all", hv_transition, out hv_Row1, out hv_Column1);
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row1, hv_Column1, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "used_edges", "row", out hv_UsedRow);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "used_edges", "column", out hv_UsedColumn);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                ho_UsedEdges.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_UsedEdges, hv_UsedRow, hv_UsedColumn, 10, (new HTuple(45)).TupleRad());
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageTest);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_Contour);
                hWVision.SetColor("blue");
                hWVision.DispObj(ho_UsedEdges);

                ho_ImageSet.Dispose();
                ho_ImageTest.Dispose();
                ho_Rcircle.Dispose();
                ho_RegionReduced.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                ho_Contour.Dispose();
                ho_Cross.Dispose();
                ho_UsedEdges.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        #endregion
        private void tBViewRa_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = tBViewRa.Value;
            UDViewRa.Value = hv_RingRadius;
        }
        private void UDViewRa_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = (HTuple)UDViewRa.Value;
            tBViewRa.Value = hv_RingRadius;
            ShowRing();
        }
        private void tBViewWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = tBViewWidth.Value;
            UDViewWidth.Value = hv_RDetectHeight;
        }
        private void UDViewWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = (HTuple)UDViewWidth.Value;
            tBViewWidth.Value = hv_RDetectHeight;
            ShowRing();
        }
        private void tBViewW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBViewW2B.Value;
            UDViewW2B.Value = hv_RAmplitudeThreshold;
        }
        private void UDViewW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDViewW2B.Value;
            tBViewW2B.Value = hv_RAmplitudeThreshold;
            tBViewB2W.Value = 1;
            hv_transition = "positive";
            ShowRing();
        }
        private void tBViewB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBViewB2W.Value;
            UDViewB2W.Value = hv_RAmplitudeThreshold;
        }
        private void UDViewB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDViewB2W.Value;
            tBViewB2W.Value = hv_RAmplitudeThreshold;
            tBViewW2B.Value = 255;
            hv_transition = "negative";
            ShowRing();
        }
        private void btnViewCir_Click(object sender, EventArgs e)
        {
            int i_image = int.Parse(ViewNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            #region
            //try
            //{
            //    hv_RowCenter = 0; hv_ColCenter = 0; hv_CenterRadius = 0; hv_ActiveNum = 15; hv_ArcType = "circle";

            //    //ho_Circle.Dispose();
            //    HOperatorSet.GenEmptyObj(out ho_Circle);

            //    HOperatorSet.TupleLength(hv_RResultColumn, out hv_Length);
            //    if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(new HTuple(hv_ActiveNum.TupleGreater(
            //        2)))) != 0)
            //    {
            //        ho_Contour.Dispose();
            //        HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_RResultRow, hv_RResultColumn);
            //        HOperatorSet.FitCircleContourXld(ho_Contour, "geotukey", -1, 0, 0, 3, 2, out hv_RowCenter,
            //            out hv_ColCenter, out hv_CenterRadius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);

            //        HOperatorSet.TupleLength(hv_StartPhi, out hv_Length1);
            //        if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
            //        {
            //            ho_Arrow1.Dispose();
            //            ho_Circle.Dispose();
            //            ho_Contour.Dispose();
            //            ho_ContoursSplit.Dispose();
            //            ho_SortedContours.Dispose();
            //            ho_ObjectSelected2.Dispose();
            //            ho_RCircle0.Dispose();

            //            return;
            //        }
            //        if ((int)(new HTuple(hv_ArcType.TupleEqual("arc"))) != 0)
            //        {
            //            ho_RCircle.Dispose();
            //            HOperatorSet.GenCircleContourXld(out ho_RCircle, hv_RowCenter, hv_ColCenter,
            //                hv_CenterRadius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
            //        }
            //        else
            //        {
            //            ho_RCircle.Dispose();
            //            HOperatorSet.GenCircleContourXld(out ho_RCircle, hv_RowCenter, hv_ColCenter,
            //                hv_CenterRadius, 0, (new HTuple(360)).TupleRad(), hv_PointOrder, 1);
            //        }
            //        HOperatorSet.TupleLength(hv_RowCenter, out hv_Length);
            //        if (hv_Length.Length != 0)
            //        {
            //            ho_ContoursSplit.Dispose();
            //            HOperatorSet.SegmentContoursXld(ho_Contour, out ho_ContoursSplit, "lines_circles",
            //                5, 4, 2);
            //            ho_SortedContours.Dispose();
            //            HOperatorSet.SortContoursXld(ho_ContoursSplit, out ho_SortedContours, "upper_left",
            //                "true", "row");
            //            HOperatorSet.CountObj(ho_SortedContours, out hv_Number1);
            //            ho_RCircle.Dispose();
            //            HOperatorSet.GenEmptyObj(out ho_RCircle);
            //            for (hv_i = 1; hv_i.Continue(hv_Number1, 1); hv_i = hv_i.TupleAdd(1))
            //            {
            //                //ho_ObjectSelected2.Dispose();
            //                HOperatorSet.GenEmptyObj(out ho_ObjectSelected2);
            //                HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected2, hv_i);
            //                HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected2, "cont_approx",
            //                    out hv_Attrib);
            //                if ((int)(new HTuple(hv_Attrib.TupleEqual(1))) != 0)
            //                {
            //                    OTemp[SP_O] = ho_RCircle.CopyObj(1, -1);
            //                    SP_O++;
            //                    ho_RCircle.Dispose();
            //                    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_ObjectSelected2, out ho_RCircle
            //                        );
            //                    OTemp[SP_O - 1].Dispose();
            //                    SP_O = 0;
            //                }
            //            }
            //        }
            //    }

            //    hWVision.DispObj(ho_ImageSet);
            //    hWVision.SetColor("green");
            //    hWVision.DispObj(ho_RCircle);
            //    hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
            //    hWVision.SetColor("red");
            //    hWVision.SetLineWidth(1);
            //    hWVision.DispCross(row, col, width, 0);
            //    HD.set_display_font(hWVision, 18, "sans", "true", "false");
            //    HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col), "", 150, 150, "green", "false");
            //    HD.disp_message(hWVision, "Y_bias:" + -(hv_RowCenter - row), "", 300, 150, "green", "false");

            //    ho_RRegions.Dispose();
            //    ho_RContCircle.Dispose();
            //    ho_Arrow1.Dispose();
            //    ho_Contour.Dispose();
            //    ho_ContoursSplit.Dispose();
            //    ho_SortedContours.Dispose();
            //    ho_ObjectSelected2.Dispose();
            //}
            //catch
            //{
            //    MessageBox.Show("ERROR!");
            //    //
            //}
            #endregion
            try
            {
                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_CrossCenter);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);
                HOperatorSet.GenEmptyObj(out ho_ResultContours);

                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBRegion2.Value;
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.5);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_ResultContours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours, hv_MetrologyHandle, "all", "all", 1.5);
                HOperatorSet.FitCircleContourXld(ho_ResultContours, "algebraic", -1, 0, 0,
                        3, 2, out hv_fitRow, out hv_fitCol, out hv_fitRadius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                //hv_ColCenter = hv_circleParameter.TupleSelect(1);
                //hv_RowCenter = hv_circleParameter.TupleSelect(0);
                //hv_CenterRadius = hv_circleParameter.TupleSelect(2);
                hv_ColCenter = hv_fitCol;
                hv_RowCenter = hv_fitRow;
                hv_CenterRadius = hv_fitRadius;

                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_ResultContours);
                hWVision.SetColor("red");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                if (halcon.IsCrossDraw)
                    hWVision.DispCross(row, col, width, 0);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col), "", 150, 150, "green", "false");
                HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row)), "", 300, 150, "green", "false");
                HD.disp_message(hWVision, "R:" + hv_CenterRadius, "", 450, 150, "green", "false");

                ho_ImageTest.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                ho_ResultContours.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void btnSaveView_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (ViewNum == "8" && cBGlueCir.Checked)
            {
                CCDNAME = "GCCD2GluePoint";
                iniFile.Write(CCDNAME, "LighterValue", (Bar_LED11Lig.Value).ToString(), parent.setpath);
            }
            iniFile.Write(CCDNAME, "PixRegionR", (tBPRegion.Value).ToString(), parent.setpath);
            iniFile.Write(CCDNAME, "BinBviThreshold", (tBthreshold.Value).ToString(), parent.setpath);
            iniFile.Write(CCDNAME, "PixRadius", (tBViewRa.Value).ToString(), parent.setpath);
            iniFile.Write(CCDNAME, "PixWidth", (tBViewWidth.Value).ToString(), parent.setpath);
            iniFile.Write(CCDNAME, "PixW2BThreshold", (tBViewW2B.Value).ToString(), parent.setpath);
            iniFile.Write(CCDNAME, "PixB2WThreshold", (tBViewB2W.Value).ToString(), parent.setpath);
            iniFile.Write(CCDNAME, "Transition", (string)hv_transition, parent.setpath);
            //iniFile.Write(CCDNAME, "LighterValue", (Bar_LED11Lig.Value).ToString(), parent.setpath);
        }
        private void CallibratManu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewNum == "1" || ViewNum == "3")
                if (CallibratManu.SelectedIndex == 5 || CallibratManu.SelectedIndex == 6)
                    CallibratManu.SelectedIndex = 4;
            if (ViewNum == "2" || ViewNum == "4")
                if (CallibratManu.SelectedIndex == 4 || CallibratManu.SelectedIndex == 5)
                    CallibratManu.SelectedIndex = 6;
            if (ViewNum == "7")
                if (CallibratManu.SelectedIndex == 4 || CallibratManu.SelectedIndex == 6)
                    CallibratManu.SelectedIndex = 5;
            if (ViewNum == "8")
                if (CallibratManu.SelectedIndex == 4 || CallibratManu.SelectedIndex == 5 || CallibratManu.SelectedIndex == 6)
                    CallibratManu.SelectedIndex = 7;
            if (CallibratManu.SelectedIndex == 7)
            {
                cBViewCCD.SelectedIndex = 7;
                //cBGlueCir.Checked = true;
            }
        }
        #endregion
        #region 角度校正
        public Double X1, Y1, X2, Y2, Xnum, Ynum, Xb, Yb, angleC = 0;
        private void btnAngleY1_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            readtemPara(CCDNAME);
            CorShow();
            X1 = Xnum;
            Y1 = Ynum;
            txtA_X1.Text = X1.ToString();
            txtA_Y1.Text = Y1.ToString();
            if (X1 != 0)
            {
                btnAngleY1.BackColor = Color.YellowGreen;
                if (!Directory.Exists(Sys.IniPath + "\\" + "Image"))
                {
                    DirectoryInfo di = new DirectoryInfo(Sys.IniPath + "\\" + "Image");
                    di.Create();
                }
                HOperatorSet.WriteImage(ho_ImageSet, "png", -1, Sys.IniPath + "\\" + "Image" + "\\" + CCDNAME + "_AngleY1");
            }
        }
        private void btnAngleY2_Click(object sender, EventArgs e)
        {
            if (X1 != 0)
            {
                string CCDNAME = "";
                if (ViewNum == "0")
                    return;
                switch (int.Parse(ViewNum))
                {
                    case 1: CCDNAME = "A1CCD1"; break;
                    case 2: CCDNAME = "A1CCD2"; break;
                    case 3: CCDNAME = "A2CCD1"; break;
                    case 4: CCDNAME = "A2CCD2"; break;
                    case 5: CCDNAME = "PCCD1"; break;
                    case 6: CCDNAME = "PCCD2"; break;
                    case 7: CCDNAME = "GCCD1"; break;
                    case 8: CCDNAME = "GCCD2"; break;
                    case 9: CCDNAME = "QCCD"; break;
                }
                readtemPara(CCDNAME);
                CorShow();
                X2 = Xnum;
                Y2 = Ynum;
                txtA_X2.Text = X2.ToString();
                txtA_Y2.Text = Y2.ToString();
                btnAngleY2.BackColor = Color.YellowGreen;
                if (!Directory.Exists(Sys.IniPath + "\\" + "Image"))
                {
                    DirectoryInfo di = new DirectoryInfo(Sys.IniPath + "\\" + "Image");
                    di.Create();
                }
                HOperatorSet.WriteImage(ho_ImageSet, "png", -1, Sys.IniPath + "\\" + "Image" + "\\" + CCDNAME + "_AngleY2");
            }
        }
        private void btnAngleCel_Click(object sender, EventArgs e)
        {
            if (X1 != 0 & X2 != 0)
            {
                btnAngleY1.BackColor = Color.WhiteSmoke;
                btnAngleY1.Enabled = true;
                btnAngleY2.BackColor = Color.WhiteSmoke;
                btnAngleY2.Enabled = true;
                angleC = Math.Round(Math.Atan((X2 - X1) / (Y2 - Y1)), 3) * 180 / Math.PI;
                txtAdeviation.Text = angleC.ToString("f3");
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "defiexuion_angle:" + string.Format("{0:f3}°", angleC), "", 150, 150, "green", "false");
                X1 = 0; X2 = 0;
            }
        }
        private void btnSaveJD_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; A1CCD1.angleC = angleC; break;
                case 2: CCDNAME = "A1CCD2"; A1CCD2.angleC = angleC; break;
                case 3: CCDNAME = "A2CCD1"; A2CCD1.angleC = angleC; break;
                case 4: CCDNAME = "A2CCD2"; A2CCD2.angleC = angleC; break;
                case 5: CCDNAME = "PCCD1"; PCCD1.angleC = angleC; break;
                case 6: CCDNAME = "PCCD2"; PCCD2.angleC = angleC; break;
                case 7: CCDNAME = "GCCD1"; GCCD1.angleC = angleC; break;
                case 8: CCDNAME = "GCCD2"; GCCD2.angleC = angleC; break;
                case 9: CCDNAME = "QCCD"; QCCD.angleC = angleC; break;
            }
            iniFile.Write(CCDNAME, "defiexuion_angle", txtAdeviation.Text, parent.setpath);
        }
        public void readtemPara(string ccdOrd)//校正读取处理
        {
            string pr = iniFile.Read(ccdOrd, "PixRegionR", parent.setpath);
            if (pr != "")
                RegionRadius = int.Parse(pr);
            string i = iniFile.Read(ccdOrd, "BinBviThreshold", parent.setpath);
            if (i == "")
                return;
            binBvi = int.Parse(i);
            hv_RingRadius = (HTuple)int.Parse(iniFile.Read(ccdOrd, "PixRadius", parent.setpath));
            hv_RDetectHeight = (HTuple)int.Parse(iniFile.Read(ccdOrd, "PixWidth", parent.setpath));
            hv_transition = iniFile.Read(ccdOrd, "Transition", parent.setpath);
            if (hv_transition == "negative")
                hv_RAmplitudeThreshold = int.Parse(iniFile.Read(ccdOrd, "PixB2WThreshold", parent.setpath));
            if (hv_transition == "positive")
                hv_RAmplitudeThreshold = int.Parse(iniFile.Read(ccdOrd, "PixW2BThreshold", parent.setpath));
        }
        public void CorShow()
        {
            if (readpara)
                return;
            int i_image = int.Parse(ViewNum) - 1;
            if (halcon.Image[i_image] == null)
                return;
            try
            {
                switch(tabCorrectionMode.SelectedIndex)
                {
                    case 0:
                        {
                            HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                            HOperatorSet.GenEmptyObj(out ho_CrossCenter);
                            HOperatorSet.GenEmptyObj(out ho_Contour);
                            HOperatorSet.GenEmptyObj(out ho_Cross);
                            HOperatorSet.GenEmptyObj(out ho_UsedEdges);
                            HOperatorSet.GenEmptyObj(out ho_ResultContours);

                            ho_ImageSet.Dispose();
                            HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
                            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                            ho_ImageTest.Dispose();
                            HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                            if (RegionRadius != 0)
                            {
                                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                                ho_RegionReduced.Dispose();
                                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                                ho_ImageTest.Dispose();
                                HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                            }
                            #region 粗找圆心
                            ho_Border.Dispose();
                            HOperatorSet.ThresholdSubPix(ho_ImageTest, out ho_Border, binBvi);
                            ho_ContoursSplit.Dispose();
                            HOperatorSet.SegmentContoursXld(ho_Border, out ho_ContoursSplit, "lines_circles", 5, 4, 3);
                            ho_SelectedXLD.Dispose();
                            HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_SelectedXLD, "contlength", "and", 50, 99999);
                            HOperatorSet.CountObj(ho_SelectedXLD, out hv_Number1);
                            HOperatorSet.GenEmptyObj(out ho_selectCir);
                            for (int i = 1; i <= (int)hv_Number1; i++)
                            {
                                ho_selectContour.Dispose();
                                HOperatorSet.SelectObj(ho_SelectedXLD, out ho_selectContour, i);
                                HTuple hv_attrib;
                                HOperatorSet.GetContourGlobalAttribXld(ho_selectContour, "cont_approx", out hv_attrib);
                                if (hv_attrib.D == 1)
                                    HOperatorSet.ConcatObj(ho_selectCir, ho_selectContour, out ho_selectCir);
                            }
                            HOperatorSet.CountObj(ho_selectCir, out hv_Number1);
                            ho_ObjectSelectedM.Dispose();
                            HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedM, 1);
                            HOperatorSet.LengthXld(ho_ObjectSelectedM, out hv_LengthMax1);
                            HTuple hv_max_L = 0.0;
                            hv_max_L = hv_LengthMax1.Clone();
                            for (int i = 2; i < (int)hv_Number1; i++)
                            {
                                ho_ObjectSelectedN.Dispose();
                                HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedN, i);
                                HOperatorSet.LengthXld(ho_ObjectSelectedN, out hv_Length1);
                                if ((int)(new HTuple(hv_Length1.TupleGreater(hv_max_L))) != 0)
                                {
                                    hv_max_L = hv_Length1.Clone();
                                    ho_ObjectSelectedM.Dispose();
                                    HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedM, i);
                                }
                            }
                            #endregion
                            try
                            {
                                HOperatorSet.FitCircleContourXld(ho_ObjectSelectedM, "algebraic", -1, 0, 0,
                                    3, 2, out hv_fitRow, out hv_fitCol, out hv_fitRadius, out hv_StartPhi, out hv_EndPhi,
                                    out hv_PointOrder);
                                hv_RingRow = hv_fitRow; hv_RingCol = hv_fitCol;
                                #region 找圆心 new
                                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.5);
                                //应用测量
                                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                                //获取结果
                                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                                ho_RCircle.Dispose();
                                HOperatorSet.GetMetrologyObjectResultContour(out ho_RCircle, hv_MetrologyHandle, "all", "all", 1.5);
                                HOperatorSet.FitCircleContourXld(ho_RCircle, "algebraic", -1, 0, 0,
                                    3, 2, out hv_fitRow, out hv_fitCol, out hv_fitRadius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                                //hv_ColCenter = hv_circleParameter.TupleSelect(1);
                                //hv_RowCenter = hv_circleParameter.TupleSelect(0);
                                //hv_CenterRadius = hv_circleParameter.TupleSelect(2);
                                hv_ColCenter = hv_fitCol;
                                hv_RowCenter = hv_fitRow;
                                hv_CenterRadius = hv_fitRadius;
                                ho_RCircle0.Dispose();
                                HOperatorSet.GenCircle(out ho_RCircle0, hv_RowCenter, hv_ColCenter, 8);
                                #endregion
                                hWVision.DispObj(ho_ImageSet);
                                hWVision.SetColor("green");
                                hWVision.SetLineWidth(1);
                                hWVision.DispObj(ho_RCircle);
                                hWVision.SetColor("red");
                                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                                hWVision.DispCross(row, col, width, 0);
                                Xnum = hv_ColCenter;
                                Ynum = hv_RowCenter;
                                Xb = (hv_ColCenter - col) * xpm;
                                Yb = -(hv_RowCenter - row) * ypm;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("s");
                                //
                            }
                        }break;
                    case 1:
                            {
                                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                                Xnum = hv_ColCenter;
                                Ynum = hv_RowCenter;
                                Xb = (hv_ColCenter - col) * xpm;
                                Yb = -(hv_RowCenter - row) * ypm;
                            }break;
                        
                }
                ho_ImageTest.Dispose();
                ho_Rcircle.Dispose();
                ho_RegionReduced.Dispose();
                ho_Region.Dispose();
                ho_Border.Dispose();
                ho_Circle.Dispose();
                ho_SelectedXLD.Dispose();
                ho_ObjectSelectedN.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                ho_ResultContours.Dispose();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("请重新调节初始灰度阀值！" + ex.ToString());
            }
        }
        #endregion
        #region 像素校正
        Double Xx1, Xx2, Yx1, Yx2, rl; double R1, R2;
        private void txtbtnY1_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string straddress = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; straddress = "01WRDD00250,02\r\n"; break;
                case 2: CCDNAME = "A1CCD2"; straddress = "01WRDD00250,02\r\n"; break;
                case 3: CCDNAME = "A2CCD1"; straddress = "01WRDD00252,02\r\n"; break;
                case 4: CCDNAME = "A2CCD2"; straddress = "01WRDD00252,02\r\n"; break;
                case 5: CCDNAME = "PCCD1"; straddress = "01WRDD00254,02\r\n"; break;
                case 6: CCDNAME = "PCCD2"; straddress = "01WRDD00254,02\r\n"; break;
                case 7: CCDNAME = "GCCD1"; straddress = "01WRDD00290,02\r\n"; break;
                case 8: CCDNAME = "GCCD2"; straddress = "01WRDD00290,02\r\n"; break;
                case 9: CCDNAME = "QCCD"; straddress = "01WRDD00256,02\r\n"; break;
            }
            WriteToPlc.CMDRead = straddress;
            WriteToPlc.CMDRsend = true;
            Thread.Sleep(10);
            R1 = WriteToPlc.DoubleRead;
            lblY1.Text = R1.ToString();
            if (R2 != 0)
                txtPixrange.Text = (Math.Round((Math.Abs(R2 - R1)), 3)).ToString();
            readtemPara(CCDNAME);
            CorShow();
            Xx1 = Xnum;
            Yx1 = Ynum;
            txtP_X1.Text = Xx1.ToString();
            txtP_Y1.Text = Yx1.ToString();
            txtbtnY1.BackColor = Color.YellowGreen;
            if (!Directory.Exists(Sys.IniPath + "\\" + "Image"))
            {
                DirectoryInfo di = new DirectoryInfo(Sys.IniPath + "\\" + "Image");
                di.Create();
            }
            if (Xx1 != 0)
                HOperatorSet.WriteImage(ho_ImageSet, "png", -1, Sys.IniPath + "\\" + "Image" + "\\" + CCDNAME + "_PixY1Figure");
        }
        private void txtbtnY2_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string straddress = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; straddress = "01WRDD00250,02\r\n"; break;
                case 2: CCDNAME = "A1CCD2"; straddress = "01WRDD00250,02\r\n"; break;
                case 3: CCDNAME = "A2CCD1"; straddress = "01WRDD00252,02\r\n"; break;
                case 4: CCDNAME = "A2CCD2"; straddress = "01WRDD00252,02\r\n"; break;
                case 5: CCDNAME = "PCCD1"; straddress = "01WRDD00254,02\r\n"; break;
                case 6: CCDNAME = "PCCD2"; straddress = "01WRDD00254,02\r\n"; break;
                case 7: CCDNAME = "GCCD1"; straddress = "01WRDD00290,02\r\n"; break;
                case 8: CCDNAME = "GCCD2"; straddress = "01WRDD00290,02\r\n"; break;
                case 9: CCDNAME = "QCCD"; straddress = "01WRDD00256,02\r\n"; break;
            }
            WriteToPlc.CMDRead = straddress;
            WriteToPlc.CMDRsend = true;
            Thread.Sleep(10);
            R2 = WriteToPlc.DoubleRead;
            lblY2.Text = R2.ToString();
            if (R2 != 0)
                txtPixrange.Text = (Math.Round((Math.Abs(R2 - R1)), 3)).ToString();
            readtemPara(CCDNAME);
            CorShow();
            Xx2 = Xnum;
            Yx2 = Ynum;
            txtP_X2.Text = Xx2.ToString();
            txtP_Y2.Text = Yx2.ToString();
            txtbtnY2.BackColor = Color.YellowGreen;
            if (!Directory.Exists(Sys.IniPath + "\\" + "Image"))
            {
                DirectoryInfo di = new DirectoryInfo(Sys.IniPath + "\\" + "Image");
                di.Create();
            }
            if (Xx2 != 0)
                HOperatorSet.WriteImage(ho_ImageSet, "png", -1, Sys.IniPath + "\\" + "Image" + "\\" + CCDNAME + "_PixY2Figure");
        }
        private void btnPix_Click(object sender, EventArgs e)
        {
            string range = txtPixrange.Text;
            txtAbrange.Text = txtPixrange.Text;
            if (range != "")
                rl = Convert.ToDouble(range);
            if (Xx1 != 0 && Xx2 != 0)
            {
                Double Yp = rl / (Math.Abs(Yx2 - Yx1));
                txtXpix.Text = Math.Round(Yp, 12).ToString();
                txtYpix.Text = Math.Round(Yp, 12).ToString();
                txtAbX.Text = Math.Round(Yp, 12).ToString();
                txtAbY.Text = Math.Round(Yp, 12).ToString();
                Xx1 = 0; Xx2 = 0; Yx1 = 0; Yx2 = 0; rl = 0;
                R1 = 0; R2 = 0;
                txtbtnY1.BackColor = Color.WhiteSmoke;
                txtbtnY2.BackColor = Color.WhiteSmoke;
            }
        }
        private void btnSavePix_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            //iniFile.Write(CCDNAME, "MoveRange", txtPixrange.Text, parent.setpath);
            iniFile.Write(CCDNAME, "Pixel_X", txtXpix.Text, parent.setpath);
            iniFile.Write(CCDNAME, "Pixel_Y", txtYpix.Text, parent.setpath);
            iniFile.Write(CCDNAME, "AssemblyMoveRange", txtAbrange.Text, parent.setpath);
            iniFile.Write(CCDNAME, "AssemblyPixel_X", txtAbX.Text, parent.setpath);
            iniFile.Write(CCDNAME, "AssemblyPixel_Y", txtAbY.Text, parent.setpath);
            switch (int.Parse(ViewNum))
            {
                case 1: A1CCD1.xpm = double.Parse(txtXpix.Text); A1CCD1.ypm = double.Parse(txtYpix.Text); break;   //A1CCD1.Pxpm
                case 2: A1CCD2.xpm = double.Parse(txtXpix.Text); A1CCD2.ypm = double.Parse(txtYpix.Text); break;
                case 3: A2CCD1.xpm = double.Parse(txtXpix.Text); A2CCD1.ypm = double.Parse(txtYpix.Text); break;
                case 4: A2CCD2.xpm = double.Parse(txtXpix.Text); A2CCD2.ypm = double.Parse(txtYpix.Text); break;
                case 5: PCCD1.xpm = double.Parse(txtXpix.Text); PCCD1.ypm = double.Parse(txtYpix.Text); break;
                case 6: PCCD2.xpm = double.Parse(txtXpix.Text); PCCD2.ypm = double.Parse(txtYpix.Text); break;
                case 7: GCCD1.xpm = double.Parse(txtXpix.Text); GCCD1.ypm = double.Parse(txtYpix.Text); break;
                case 8: GCCD2.xpm = double.Parse(txtXpix.Text); GCCD2.ypm = double.Parse(txtYpix.Text); break;
                case 9: QCCD.xpm = double.Parse(txtXpix.Text); QCCD.ypm = double.Parse(txtYpix.Text); break;
            }
            //btnSaveAb.PerformClick();
        }
        #endregion
        #region 三心校正
        string Xorigin, Yorigin;
        private void btnBlockCenter_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string straddress = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; straddress = "01WWRD00138,04,"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: CCDNAME = "A1CCD2"; straddress = "01WWRD00092,04,"; xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: CCDNAME = "A2CCD1"; straddress = "01WWRD00142,04,"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: CCDNAME = "A2CCD2"; straddress = "01WWRD00100,04,"; xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: CCDNAME = "PCCD1"; straddress = "01WWRD00146,04,"; xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: CCDNAME = "PCCD2"; straddress = "01WWRD00108,04,"; xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: CCDNAME = "GCCD1"; straddress = "01WWRD00150,04,"; xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: CCDNAME = "GCCD2"; straddress = "01WWRD00116,04,"; xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: CCDNAME = "QCCD"; xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            readtemPara(CCDNAME);
            CorShow();
            Xorigin = Xnum.ToString(); Yorigin = Ynum.ToString();
            txtBlockX.Text = Math.Round(Xb, 5).ToString(); txtBlockY.Text = Math.Round(Yb, 5).ToString();
            string cmdBX = NToHString((int)(Math.Round(Xb, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Yb, 3) * 1000));
            WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        private void btnWToplc_Click(object sender, EventArgs e)
        {
            string straddress = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: straddress = "01WWRD00138,04,"; break;
                case 2: straddress = "01WWRD00092,04,"; break;
                case 3: straddress = "01WWRD00142,04,"; break;
                case 4: straddress = "01WWRD00100,04,"; break;
                case 5: straddress = "01WWRD00146,04,"; break;
                case 6: straddress = "01WWRD00108,04,"; break;
                case 7: straddress = "01WWRD00150,04,"; break;
                case 8: straddress = "01WWRD00116,04,"; break;
            }
            double Xbs = double.Parse(txtBlockX.Text); double Ybs = double.Parse(txtBlockY.Text);
            string cmdBX = NToHString((int)(Math.Round(Xbs, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Ybs, 3) * 1000));
            WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        private void btnSaveQX_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            iniFile.Write(CCDNAME, "BlockCenX", txtBlockX.Text, parent.setpath);
            iniFile.Write(CCDNAME, "BlockCenY", txtBlockY.Text, parent.setpath);
        }
        #endregion
        #region 组装像素
        private void txtbtnAbY1_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            readtemPara(CCDNAME);
            CorShow();
            Xx1 = Xnum;
            Yx1 = Ynum;
            txtAb_X1.Text = Xx1.ToString();
            txtAb_Y1.Text = Yx1.ToString();
            txtbtnAbY1.BackColor = Color.YellowGreen;
            if (!Directory.Exists(Sys.IniPath + "\\" + "AssemblyImage"))
            {
                DirectoryInfo di = new DirectoryInfo(Sys.IniPath + "\\" + "AssemblyImage");
                di.Create();
            }
            HOperatorSet.WriteImage(ho_ImageSet, "png", -1, Sys.IniPath + "\\" + "AssemblyImage" + "\\" + CCDNAME + "_AbY1Figure");
        }
        private void txtbtnAbY2_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            readtemPara(CCDNAME);
            CorShow();
            Xx2 = Xnum;
            Yx2 = Ynum;
            txtAb_X2.Text = Xx2.ToString();
            txtAb_Y2.Text = Yx2.ToString();
            txtbtnAbY2.BackColor = Color.YellowGreen;
            if (!Directory.Exists(Sys.IniPath + "\\" + "AssemblyImage"))
            {
                DirectoryInfo di = new DirectoryInfo(Sys.IniPath + "\\" + "AssemblyImage");
                di.Create();
            }
            HOperatorSet.WriteImage(ho_ImageSet, "png", -1, Sys.IniPath + "\\" + "AssemblyImage" + "\\" + CCDNAME + "_AbY2Figure");
        }
        private void btnAb_Click(object sender, EventArgs e)
        {
            string range = txtAbrange.Text;
            if (range != "")
            {
                rl = Convert.ToDouble(range);
            }
            if (Xx1 != 0 && Xx2 != 0)
            {
                Double Yp = rl / (Math.Abs(Yx2 - Yx1));
                txtAbX.Text = Yp.ToString();
                txtAbY.Text = Yp.ToString();
                Xx1 = 0; Xx2 = 0; Yx1 = 0; Yx2 = 0; rl = 0;
                txtbtnAbY1.BackColor = Color.WhiteSmoke;
                txtbtnAbY2.BackColor = Color.WhiteSmoke;
            }
        }
        private void btnSaveAb_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0")
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            iniFile.Write(CCDNAME, "AssemblyMoveRange", txtAbrange.Text, parent.setpath);
            iniFile.Write(CCDNAME, "AssemblyPixel_X", txtAbX.Text, parent.setpath);
            iniFile.Write(CCDNAME, "AssemblyPixel_Y", txtAbY.Text, parent.setpath);
            switch (int.Parse(ViewNum))
            {
                case 1: A1CCD1.xpm = double.Parse(txtAbX.Text); A1CCD1.ypm = double.Parse(txtAbY.Text); break;
                case 2: A1CCD2.xpm = double.Parse(txtAbX.Text); A1CCD2.ypm = double.Parse(txtAbY.Text); break;
                case 3: A2CCD1.xpm = double.Parse(txtAbX.Text); A2CCD1.ypm = double.Parse(txtAbY.Text); break;
                case 4: A2CCD2.xpm = double.Parse(txtAbX.Text); A2CCD2.ypm = double.Parse(txtAbY.Text); break;
                case 5: PCCD1.xpm = double.Parse(txtAbX.Text); PCCD1.ypm = double.Parse(txtAbY.Text); break;
                case 6: PCCD2.xpm = double.Parse(txtAbX.Text); PCCD2.ypm = double.Parse(txtAbY.Text); break;
                case 7: GCCD1.xpm = double.Parse(txtAbX.Text); GCCD1.ypm = double.Parse(txtAbY.Text); break;
                case 8: GCCD2.xpm = double.Parse(txtAbX.Text); GCCD2.ypm = double.Parse(txtAbY.Text); break;
                case 9: QCCD.xpm = double.Parse(txtAbX.Text); QCCD.ypm = double.Parse(txtAbY.Text); break;
            }
        }
        #endregion
        #region 吸嘴求心
        private void btnNozzleCen_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string straddress = "";
            if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3" || ViewNum == "5"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; straddress = "01WWRD00096,04,"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 3: CCDNAME = "A2CCD1"; straddress = "01WWRD00104,04,"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 5: CCDNAME = "PCCD1"; straddress = "01WWRD00112,04,"; xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
            }
            readtemPara(CCDNAME);
            CorShow();
            Xorigin = Xnum.ToString(); Yorigin = Ynum.ToString();
            txtNozzleX.Text = Math.Round(Xb, 5).ToString(); txtNozzleY.Text = Math.Round(Yb, 5).ToString();
            string cmdBX = NToHString((int)(Math.Round(Xb, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Yb, 3) * 1000));
            WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        private void btnN2Plc_Click(object sender, EventArgs e)
        {
            string straddress = "";
            if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3" || ViewNum == "5"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: straddress = "01WWRD00096,04,"; break;
                case 3: straddress = "01WWRD00104,04,"; break;
                case 5: straddress = "01WWRD00112,04,"; break;
            }
            double Xbs = double.Parse(txtNozzleX.Text); double Ybs = double.Parse(txtNozzleY.Text);
            string cmdBX = NToHString((int)(Math.Round(Xbs, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Ybs, 3) * 1000));
            WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        private void btnSaveNo_Click(object sender, EventArgs e)
        {
            try
            {
                string CCDNAME = "";
                if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3" || ViewNum == "5"))
                    return;
                switch (int.Parse(ViewNum))
                {
                    case 1: CCDNAME = "A1CCD1"; break;
                    case 3: CCDNAME = "A2CCD1"; break;
                    case 5: CCDNAME = "PCCD1"; break;
                }
                iniFile.Write(CCDNAME, "NozzleX", txtNozzleX.Text, parent.setpath);
                iniFile.Write(CCDNAME, "NozzleY", txtNozzleY.Text, parent.setpath);
                iniFile.Write(CCDNAME, "NozzleCheckX", txtMatchX.Text, parent.setpath);
                iniFile.Write(CCDNAME, "NozzleCheckY", txtMatchY.Text, parent.setpath);
                string Fchecked = (SqcheckBox.Checked ? "true" : "false");
                iniFile.Write(CCDNAME, "FChecked", Fchecked, parent.setpath);
                if (SqcheckBox.Checked)
                {
                    if (hv_FModelID != null)
                    {
                        HOperatorSet.WriteShapeModel(hv_FModelID, Sys.IniPath + "\\" + "Image" + "\\" + CCDNAME + "_RegionModel.shm");
                        iniFile.Write(CCDNAME, "Frow", Math.Round((double)hv_RowCh, 4).ToString(), parent.setpath);
                        iniFile.Write(CCDNAME, "Fcol", Math.Round((double)hv_ColumnCh, 4).ToString(), parent.setpath);
                        iniFile.Write(CCDNAME, "Fangle", Math.Round((double)hv_angle, 4).ToString(), parent.setpath);
                        iniFile.Write(CCDNAME, "Flength1", Math.Round((double)hv_length1, 4).ToString(), parent.setpath);
                        iniFile.Write(CCDNAME, "Flength2", Math.Round((double)hv_length2, 4).ToString(), parent.setpath);
                        iniFile.Write(CCDNAME, "FWidth", UDBlockWidth.Value.ToString(), parent.setpath);
                        iniFile.Write(CCDNAME, "FDis", UDBlockdist.Value.ToString(), parent.setpath);
                        if (UDBlockB2W.Value != 1)
                        {
                            iniFile.Write(CCDNAME, "Ftransition", "positive", parent.setpath);
                            iniFile.Write(CCDNAME, "Fthreshold", UDBlockB2W.Value.ToString(), parent.setpath);
                        }
                        if (UDBlockW2B.Value != 255)
                        {
                            iniFile.Write(CCDNAME, "Ftransition", "negative", parent.setpath);
                            iniFile.Write(CCDNAME, "Fthreshold", UDBlockW2B.Value.ToString(), parent.setpath);
                        }
                        iniFile.Write(CCDNAME, "FNozzleX", txtFNozzleX.Text, parent.setpath);
                        iniFile.Write(CCDNAME, "FNozzleY", txtFNozzleY.Text, parent.setpath);
                        iniFile.Write(CCDNAME, "FNozzleAngle", txtFAngle.Text, parent.setpath);
                    }
                    else
                    {
                        MessageBox.Show("请调整参数后再保存！");
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        PointF matchp;
        int ti = 0;
        private void btnCalView_Click(object sender, EventArgs e)
        {
            switch (ti)
            {
                case 0:
                    Sys.bCalView = true;
                    btnCalView.BackColor = Color.Green;
                    btnCalView.Text = "1、结束拍摄并求心";
                    string CCDNAME = "";
                    if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3"))
                        return;
                    switch (int.Parse(ViewNum))
                    {
                        case 1: CCDNAME = "A1CCD1"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                        case 3: CCDNAME = "A2CCD1"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    }
                    lblShow.Show();
                    //CalViewSet(CCDNAME);
                    break;
                case 1:
                    Sys.calviewNum = 0;
                    Sys.calviewNumL = 0;
                    Sys.bCalView = false;
                    btnCalView.BackColor = Color.WhiteSmoke;
                    btnCalView.Text = "1、开始拍摄并求心";
                    lblShow.Hide();
                    break;
            }
            ti++;
            if (ti == 2)
            {
                ti = 0;
            }
        }
        public void CalViewSet(string cname)
        {
            try
            {
                Sys.calviewNum++;
                readtemPara(cname);
                CorShow();
                Sys.pXY[Sys.calviewNum - 1, 0] = Xb;
                Sys.pXY[Sys.calviewNum - 1, 1] = Yb;
                if (Xb != 0)
                    WriteToPlc.CMDcheckR = "01WWRD00161,01,0001\r\n";
                else
                    WriteToPlc.CMDcheckR = "01WWRD00161,01,0002\r\n";
                WriteToPlc.CMDchecksend = true;
                if (Sys.calviewNum == 8)
                {
                    lblShow.Hide();
                    txtMatchX.Text = "";
                    txtMatchY.Text = "";
                    lblShowZ.Hide();
                    txtMatchZX.Text = "";
                    txtMatchZY.Text = "";
                }
            }
            catch (Exception ER)
            {
                MessageBox.Show(ER.ToString());
            }
        }
        private void btnCalMatch_Click(object sender, EventArgs e)
        {
            if (btnCalView.Text == "1、结束拍摄并求心")
                btnCalView.PerformClick();
            btnCalMatch.BackColor = Color.Green;
            string straddress = "";
            if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: straddress = "01WWRD00096,04,"; break;
                case 3: straddress = "01WWRD00104,04,"; break;
            }
            if (Sys.pXY.Length == 0)
                return;
            if (Sys.pXY[0, 1] == 0 & Sys.pXY[7, 1] == 0)
            {
                btnCalMatch.BackColor = Color.WhiteSmoke;
                MessageBox.Show("未取得足够多的角度中心，请重新操作！");
                return;
            }
            double[] result = qiuyuan(Sys.pXY);
            matchp.X = (float)result[0];
            matchp.Y = (float)result[1];
            txtMatchX.Text = (Math.Round(matchp.X, 3)).ToString();
            txtMatchY.Text = (Math.Round(matchp.Y, 3)).ToString();
            string Xnum = NToHString((int)(Math.Round(matchp.X, 3) * 1000));
            string Ynum = NToHString((int)(Math.Round(matchp.Y, 3) * 1000));
            string cmdL = Xnum + Ynum;
            WriteToPlc.CMDcheckR = straddress + cmdL + "\r\n";
            WriteToPlc.CMDchecksend = true;
            Sys.pXY = new double[,] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };//      
            btnCalMatch.BackColor = Color.WhiteSmoke;
        }
        private void btnMWToplc_Click(object sender, EventArgs e)
        {
            string straddress = "";
            if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: straddress = "01WWRD00096,04,"; break;
                case 3: straddress = "01WWRD00104,04,"; break;
            }
            double Xbs = double.Parse(txtMatchX.Text); double Ybs = double.Parse(txtMatchY.Text);
            string cmdBX = NToHString((int)(Math.Round(Xbs, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Ybs, 3) * 1000));
            WriteToPlc.CMDcheckR = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDchecksend = true;
        }
        public static double[] qiuyuan(double[,] XY)  //最小二乘法拟合圆
        {
            double[] resultPiont = new double[3];
            double X1 = 0;
            double Y1 = 0;
            double X2 = 0;
            double Y2 = 0;
            double X3 = 0;
            double Y3 = 0;
            double X1Y1 = 0;
            double X1Y2 = 0;
            double X2Y1 = 0;
            for (int i = 0; i < XY.GetLength(0); i++)
            {
                X1 += XY[i, 0];
                Y1 += XY[i, 1];
                X2 += XY[i, 0] * XY[i, 0];
                Y2 += XY[i, 1] * XY[i, 1];
                X3 += XY[i, 0] * XY[i, 0] * XY[i, 0];
                Y3 += XY[i, 1] * XY[i, 1] * XY[i, 1];
                X1Y1 += XY[i, 0] * XY[i, 1];
                X1Y2 += XY[i, 0] * XY[i, 1] * XY[i, 1];
                X2Y1 += XY[i, 0] * XY[i, 0] * XY[i, 1];
            }
            double C, D, E, G, H, N = XY.GetLength(0);
            double a, b, c;
            //N = XY.Length;
            C = N * X2 - X1 * X1;
            D = N * X1Y1 - X1 * Y1;
            E = N * X3 + N * X1Y2 - (X2 + Y2) * X1;
            G = N * Y2 - Y1 * Y1;
            H = N * X2Y1 + N * Y3 - (X2 + Y2) * Y1;
            a = (H * D - E * G) / (C * G - D * D);
            b = (H * C - E * D) / (D * D - G * C);
            c = -(a * X1 + b * Y1 + X2 + Y2) / N;
            double A, B, R;
            A = a / (-2);
            B = b / (-2);
            R = Math.Sqrt(a * a + b * b - 4 * c) / 2;

            resultPiont[0] = A;
            resultPiont[1] = B;
            resultPiont[2] = R;
            return resultPiont;
        }
        #region

        #region
        HTuple hv_RowCh = new HTuple(), hv_ColumnCh = new HTuple(), hv_angle = new HTuple(), hv_length1 = new HTuple(), hv_length2 = new HTuple();
        HObject ho_FModelRegion = new HObject(), ho_FModelImage = new HObject(), ho_FModelContours = new HObject();
        HObject ho_ShowContours = new HObject(), ho_ShowContoursRegion = new HObject();
        HTuple hv_FModelID = new HTuple(), Fhv_Area = new HTuple(), hv_FRow = new HTuple(), hv_FCol = new HTuple();
        HTuple hv_HomMat2D = new HTuple();
        HTuple hv_IRtransition = "positive", hv_IRTH = 10, hv_NumberTh = new HTuple(), hv_AreaMax = new HTuple();
        HTuple hv_Select = "first";
        HTuple hv_AreaIR = new HTuple(), hv_RowIR = new HTuple(), hv_ColumnIR = new HTuple();
        HTuple hv_RectangleIndices = new HTuple(), hv_F2DRow = new HTuple(), hv_F2DColumn = new HTuple();
        HTuple hv_IRWidth = 55, hv_IRdis = 10, hv_FRectangleParameter = new HTuple(), hv_FDeg = new HTuple();
        HTuple hv_RowFound = new HTuple(), hv_ColFound = new HTuple(), hv_AngleFound = new HTuple(), hv_ScaleFound = new HTuple(), hv_ScoreFound = new HTuple();
        HObject ho_RectangleIR = new HObject(), ho_ImageReducedIR = new HObject(), ho_SqRegion = new HObject();
        HObject ho_RegionFill = new HObject(), ho_SqContours = new HObject(), ho_Rectangle1 = new HObject();
        #endregion

        private void SqcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SqcheckBox.Checked)
                gBFNo.BringToFront();
            else
                gBFNo.SendToBack();
        }
        private void btnBlockModel_Click(object sender, EventArgs e)
        {
            if (ho_ImageSet == null)
                return;
            int i = int.Parse(ViewNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            HD.disp_message(hWVision, "点击鼠标左键画检测区域,点击右键确认", "", 100, 100, "green", "false");
            btnBlockModel.BackColor = Color.GreenYellow;
            hWVision.SetColor("red");
            HOperatorSet.DrawRectangle2(hWVision, out hv_RowCh, out hv_ColumnCh, out hv_angle, out hv_length1, out hv_length2);
            btnBlockModel.BackColor = Color.WhiteSmoke;
            ShowFRegion();
        }
        void ShowFRegion()
        {
            if (readpara)
                return;
            if (ho_ImageSet == null)
                return;
            int i = 0;
            if (tabProcessMode.SelectedIndex == 0)
                i = int.Parse(ViewNum) - 1;
            if (tabProcessMode.SelectedIndex == 1)
                i = int.Parse(SetNum) - 1;
            string CCDNAME = "";
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: CCDNAME = "A1CCD2"; xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: CCDNAME = "A2CCD1"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: CCDNAME = "A2CCD2"; xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: CCDNAME = "PCCD1"; xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: CCDNAME = "PCCD2"; xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: CCDNAME = "GCCD1"; xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: CCDNAME = "GCCD2"; xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: CCDNAME = "QCCD"; xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            ho_FModelRegion.Dispose();
            HOperatorSet.GenRectangle2(out ho_FModelRegion, hv_RowCh, hv_ColumnCh, hv_angle, hv_length1, hv_length2);
            ho_FModelImage.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_FModelRegion, out ho_FModelImage);
            HOperatorSet.CreateShapeModel(ho_FModelImage, "auto", (new HTuple(-90)).TupleRad()
                , (new HTuple(180)).TupleRad(), "auto", "auto", "use_polarity", "auto", "auto", out hv_FModelID);
            ho_FModelContours.Dispose();
            HOperatorSet.GetShapeModelContours(out ho_FModelContours, hv_FModelID, 1);
            HOperatorSet.AreaCenter(ho_FModelRegion, out Fhv_Area, out hv_FRow, out hv_FCol);
            HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_FRow, hv_FCol, 0, out hv_HomMat2D);
            ho_ShowContours.Dispose();
            HOperatorSet.AffineTransContourXld(ho_FModelContours, out ho_ShowContours, hv_HomMat2D);
            HOperatorSet.GenRectangle2ContourXld(out ho_ShowContoursRegion, hv_RowCh, hv_ColumnCh, hv_angle, hv_length1, hv_length2);
            
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            hWVision.DispObj(ho_ShowContours);
            hWVision.DispObj(ho_ShowContoursRegion);
            string Path = Sys.IniPath + "\\" + "Model";
            //不存在文件夹，创建先
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            HOperatorSet.WriteShapeModel(hv_FModelID, Sys.IniPath + "\\" + "Model" + "\\" + CCDNAME + "_FigureShape_Square_Model");
        }
        private void btnBlockLine_Click(object sender, EventArgs e)
        {
            if (ho_ImageSet == null)
                return;
            if (ViewNum == "0")
                return;
            int i = int.Parse(ViewNum) - 1;
            string CCDNAME = "";
            switch (int.Parse(ViewNum))
            {
                case 1: CCDNAME = "A1CCD1"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: CCDNAME = "A1CCD2"; xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: CCDNAME = "A2CCD1"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: CCDNAME = "A2CCD2"; xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: CCDNAME = "PCCD1"; xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: CCDNAME = "PCCD2"; xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: CCDNAME = "GCCD1"; xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: CCDNAME = "GCCD2"; xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: CCDNAME = "QCCD"; xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            if (hv_FModelID.Length == 0 || hv_FModelID == null)
                HOperatorSet.ReadShapeModel(Sys.IniPath + "\\" + "Image" + "\\" + CCDNAME + "_RegionModel.shm", out hv_FModelID);
            HD.disp_message(hWVision, "点击鼠标左键画检测区域,点击右键确认", "", 100, 100, "green", "false");
            btnBlockLine.BackColor = Color.GreenYellow;
            hWVision.SetColor("red");
            HOperatorSet.DrawRectangle2(hWVision, out hv_RowCh, out hv_ColumnCh, out hv_angle, out hv_length1, out hv_length2);
            btnBlockLine.BackColor = Color.WhiteSmoke;
            hv_angle = 0.0;
            try
            {
                ShowFang();
            }
            catch (Exception ER)
            {
                MessageBox.Show(ER.ToString());
            }
        }
        void ShowFang()
        {
            if (readpara)
                return;
            int i = 0;
            if (tabProcessMode.SelectedIndex == 0)
                i = int.Parse(ViewNum) - 1;
            if (tabProcessMode.SelectedIndex == 1)
                i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            #region CCD
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1-" + area4; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1-" + area4; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            #endregion
            try
            {
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);

                //** 创建2D测量矩形模板 ***
                //HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //讀取模組
                try
                {

                    RegionRadius = double.Parse(IniFile.Read(CCDNAME, "Mode2RegionRadius","1000", FrmMain.propath));
                    string Path = Sys.IniPath + "\\" + "Model";
                    HOperatorSet.ReadShapeModel(Path + "\\" + CCDNAME + "_FigureShape_Square_Model", out hv_FModelID);
                }
                catch
                {
                    MessageBox.Show("請先創建模組!");
                    return;
                }
                HObject ho_ResultContours2 = new HObject(), ho_CrossCenter2 = new HObject(), ho_ImageMedian = new HObject(), ho_ImageEmphasize = new HObject();
                HTuple hv_RectangleRow = new HTuple(), hv_RectangleColumn = new HTuple(), hv_RectanglePhi = new HTuple(), hv_RectangleLength1 = new HTuple(), hv_RectangleLength2 = new HTuple();
        
                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);

                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_RegionReduced, out ho_ImageMedian, 10, 10);
                ho_ImageEmphasize.Dispose();
                HOperatorSet.Emphasize(ho_ImageMedian, out ho_ImageEmphasize, 50, 50, 1);
                HOperatorSet.GetShapeModelContours(out ho_FModelContours, hv_FModelID, 1);
                HOperatorSet.FindScaledShapeModel(ho_ImageEmphasize, hv_FModelID, (new HTuple(-180)).TupleRad(), (new HTuple(180)).TupleRad(), 0.9, 1.1, 0.3, 1, 0.5,
                    "least_squares", 0, 0.9, out hv_RowFound, out hv_ColFound, out hv_AngleFound, out hv_ScaleFound, out hv_ScoreFound);
                if ((int)(new HTuple((new HTuple(1)).TupleEqual(new HTuple(hv_RowFound.TupleLength())))) != 0)
                {
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    //HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RowFound, hv_ColFound, hv_AngleFound, out hv_HomMat2D);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_AngleFound, 0, 0, out hv_HomMat2D);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_RowFound, hv_ColFound, out hv_HomMat2D);
                    ho_ShowContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_FModelContours, out ho_ShowContours, hv_HomMat2D);
                    ho_Region.Dispose();
                    HOperatorSet.GenRegionContourXld(ho_ShowContours, out ho_Region, "filled");
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_Region, out ho_RegionUnion);
                    HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_RectangleRow, out hv_RectangleColumn, out hv_RectanglePhi, out hv_RectangleLength1, out hv_RectangleLength2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_RectangleRow, hv_RectangleColumn, hv_RectanglePhi, hv_RectangleLength1, hv_RectangleLength2);

                    try
                    {
                        HD.gen_rectangle2_center(ho_ImageSet, out ho_Contour, out ho_Cross,
                        out ho_ResultContours, out ho_CrossCenter2, hv_RectangleRow, hv_RectangleColumn, hv_RectanglePhi, RectangleLength1_FigureShape, RectangleLength2_FigureShape,
                       hv_IRWidth, hv_IRTH, hv_IRtransition, hv_Select, out hv_ResultRow, out hv_ResultColumn, out hv_ResultPhi,
                       out hv_ResultLength1, out hv_ResultLength2);
                    }
                    catch
                    {
                    }



                    //HOperatorSet.GenRectangle2ContourXld(out ho_ShowContoursRegion, hv_RowCh, hv_ColumnCh, hv_angle, hv_length1, hv_length2);
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_RegionReduced);
                    hWVision.SetColor("green");
                    hWVision.DispObj(ho_Contour);
                    hWVision.SetColor("blue");
                    hWVision.DispObj(ho_Cross);

                }
                else
                {
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_RegionReduced);
                    HD.disp_message(hWVision, "没有找到模板", "window", 24, 24, "black", "true");
                }




                /*


                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                ho_RectangleIR.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectangleIR, hv_RowCh, hv_ColumnCh, hv_angle, hv_length1, hv_length2);

                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "rectangle2", ((((((hv_RowCh.TupleConcat(
                    hv_ColumnCh))).TupleConcat(hv_angle))).TupleConcat(hv_length1))).TupleConcat(hv_length2), 25, 5, 1, 30,
                    new HTuple(), new HTuple(), out hv_RectangleIndices);
                ho_ModelContour.Dispose();
                HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour, hv_MetrologyHandle, "all", 1.5);
                ho_MeasureContour.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_MeasureContour, hv_MetrologyHandle, "all", "all", out hv_F2DRow, out hv_F2DColumn);

                HOperatorSet.SetMetrologyModelParam(hv_MetrologyHandle, "reference_system", ((hv_FRow.TupleConcat(hv_FCol))).TupleConcat(0));
                //设置二维测量参数
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_RectangleIndices, "measure_transition", hv_IRtransition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_RectangleIndices, "measure_select",  hv_Select);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_RectangleIndices, "measure_length1", hv_IRWidth);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_RectangleIndices, "measure_length2", hv_IRdis);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_RectangleIndices, "measure_threshold", hv_IRTH);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_RectangleIndices, "min_score", 0.2);

                HOperatorSet.GetShapeModelContours(out ho_FModelContours, hv_FModelID, 1);
                HOperatorSet.FindScaledShapeModel(ho_ImageSet, hv_FModelID, (new HTuple(-90)).TupleRad(), (new HTuple(180)).TupleRad(), 0.9, 1.1, 0.3, 1, 0.5,
                    "least_squares", 0, 0.9, out hv_RowFound, out hv_ColFound, out hv_AngleFound, out hv_ScaleFound, out hv_ScoreFound);
                //如果找到模板
                if ((int)(new HTuple((new HTuple(1)).TupleEqual(new HTuple(hv_RowFound.TupleLength())))) != 0)
                {
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    HOperatorSet.HomMat2dScale(hv_HomMat2D, hv_ScaleFound, hv_ScaleFound, 0, 0, out hv_HomMat2D);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_AngleFound, 0, 0, out hv_HomMat2D);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_RowFound - 0, hv_ColFound - 0, out hv_HomMat2D);
                    ho_ResultContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_FModelContours, out ho_ResultContours, hv_HomMat2D);

                    //按照找到的模板位置，移动测量位置
                    HOperatorSet.AlignMetrologyModel(hv_MetrologyHandle, hv_RowFound, hv_ColFound, hv_AngleFound);
                    //应用测量
                    HOperatorSet.ApplyMetrologyModel(ho_ImageSet, hv_MetrologyHandle);
                    //获取结果
                    ho_Contour.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                    ho_Cross.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, hv_AngleFound);
                    if (hv_Row.Length != 0)
                    {
                        HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_RectangleIndices, "all", "result_type", "all_param", out hv_FRectangleParameter);
                        ho_CrossCenter.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_CrossCenter, hv_FRectangleParameter.TupleSelect(0), hv_FRectangleParameter.TupleSelect(1), 20, 0.785398);
                        ho_Contours.Dispose();
                        HOperatorSet.GetMetrologyObjectResultContour(out ho_Contours, hv_MetrologyHandle, "all", "all", 1.5);
                        ho_Contour.Dispose();
                        HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour, hv_MetrologyHandle, "all", "all", out hv_Row1, out hv_Column1);
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row1, hv_Column1, 6, 0.785398);
                        HOperatorSet.TupleDeg(hv_FRectangleParameter.TupleSelect(2), out hv_FDeg);
                        HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "used_edges", "row", out hv_UsedRow);
                        HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "used_edges", "column", out hv_UsedColumn);
                        ho_UsedEdges.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_UsedEdges, hv_UsedRow, hv_UsedColumn, 10, (new HTuple(45)).TupleRad());
                        ho_ResultContours.Dispose();
                        HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours, hv_MetrologyHandle, "all", "all", 1.5);
                        
                        hWVision.ClearWindow();
                        hWVision.DispObj(ho_ImageSet);
                        hWVision.SetColor("green");
                        hWVision.DispObj(ho_Contour);
                        hWVision.SetColor("blue");
                        hWVision.DispObj(ho_Cross);
                       
                        
   
                    }
                    else
                    {
                        hWVision.ClearWindow();
                        hWVision.DispObj(ho_ImageSet);
                        hWVision.SetColor("green");
                        hWVision.DispObj(ho_Contour);
                    }
                    
                }
                else
                {
                    HD.disp_message(hWVision, "没有找到模板", "window", 24, 24, "black", "true");
                }* */
            }
            catch (Exception er)
            {
                //MessageBox.Show(er.ToString());
            }
            //HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                 
        }

        void FindModel_FigureShape_Square()//搜尋矩形模組
        {
            if (readpara)
                return;
            int i = 0;
            if (tabProcessMode.SelectedIndex == 0)
                i = int.Parse(ViewNum) - 1;
            if (tabProcessMode.SelectedIndex == 1)
                i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            #region CCD
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1-"+area4; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1-" + area4; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            #endregion
            try
            {
                RegionRadius = double.Parse(IniFile.Read(CCDNAME, "Mode2RegionRadius","1000", FrmMain.propath));
                string Path = Sys.IniPath + "\\" + "Model";
                HOperatorSet.ReadShapeModel(Path + "\\" + CCDNAME + "_FigureShape_Square_Model", out hv_FModelID);
            }
            catch
            {
                MessageBox.Show("請先創建模組!");
                return;
            }
            try
            {
                HObject ho_ImageMedian = new HObject(), ho_ImageEmphasize = new HObject();
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);

                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);

                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                HOperatorSet.GetShapeModelContours(out ho_FModelContours, hv_FModelID, 1);
                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_RegionReduced, out ho_ImageMedian, 10, 10);
                ho_ImageEmphasize.Dispose();
                HOperatorSet.Emphasize(ho_ImageMedian, out ho_ImageEmphasize, 50, 50, 1);
                HOperatorSet.FindScaledShapeModel(ho_ImageEmphasize, hv_FModelID, (new HTuple(-180)).TupleRad(), (new HTuple(180)).TupleRad(), 0.9, 1.1, 0.3, 1, 0.5,
                    "least_squares", 0, 0.9, out hv_RowFound, out hv_ColFound, out hv_AngleFound, out hv_ScaleFound, out hv_ScoreFound);
                //如果找到模板
                if ((int)(new HTuple((new HTuple(1)).TupleEqual(new HTuple(hv_RowFound.TupleLength())))) != 0)
                {
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    //HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RowFound, hv_ColFound, hv_AngleFound, out hv_HomMat2D);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_AngleFound, 0, 0, out hv_HomMat2D);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_RowFound, hv_ColFound, out hv_HomMat2D);
                    ho_ShowContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_FModelContours, out ho_ShowContours, hv_HomMat2D);
                    //HOperatorSet.GenRectangle2ContourXld(out ho_ShowContoursRegion, hv_RowCh, hv_ColumnCh, hv_angle, hv_length1, hv_length2);
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_RegionReduced);
                    hWVision.SetColor("green");
                    hWVision.DispObj(ho_ShowContours);
                }
                else
                {
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_RegionReduced);
                    HD.disp_message(hWVision, "没有找到模板", "window", 24, 24, "black", "true");
                }
            }
            catch (Exception er)
            {
                //MessageBox.Show(er.ToString());
            }
            HOperatorSet.ClearShapeModel(hv_FModelID);
        }

        void CreateModel_FigureShape_Square()//創建矩形模組
        {
            if (readpara)
                return;
            int i = 0;
            if (tabProcessMode.SelectedIndex == 0)
                i = int.Parse(ViewNum) - 1;
            if (tabProcessMode.SelectedIndex == 1)
                i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            #region CCD
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1-" + area4; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1-" + area4; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            #endregion
            try
            {
                HObject ho_ImageMedian = new HObject(), ho_ImageEmphasize = new HObject();
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_ImageSet, out ho_ImageMedian, 10, 10);
                ho_ImageEmphasize.Dispose();
                HOperatorSet.Emphasize(ho_ImageMedian, out ho_ImageEmphasize, 50, 50, 1);
                ho_FModelRegion.Dispose();
                HOperatorSet.GenRectangle2(out ho_FModelRegion, hv_RowCh, hv_ColumnCh, hv_angle, hv_length1, hv_length2);
                ho_FModelImage.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageEmphasize, ho_FModelRegion, out ho_FModelImage);
                HOperatorSet.CreateShapeModel(ho_FModelImage, "auto", (new HTuple(-180)).TupleRad()
                    , (new HTuple(180)).TupleRad(), "auto", "auto", "use_polarity", "auto", "auto", out hv_FModelID);
                ho_FModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_FModelContours, hv_FModelID, 1);
               
                HOperatorSet.AreaCenter(ho_FModelRegion, out Fhv_Area, out hv_FRow, out hv_FCol);
                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_FRow, hv_FCol, 0, out hv_HomMat2D);
                ho_ShowContours.Dispose();
                HOperatorSet.AffineTransContourXld(ho_FModelContours, out ho_ShowContours, hv_HomMat2D);
                HOperatorSet.GenRectangle2ContourXld(out ho_ShowContoursRegion, hv_RowCh, hv_ColumnCh, hv_angle, hv_length1, hv_length2);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.DispObj(ho_ShowContours);
                hWVision.DispObj(ho_ShowContoursRegion);


                if (MessageBox.Show("是否儲存模組?", "模組設定", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    string Path = Sys.IniPath + "\\" + "Model";
                    //不存在文件夹，创建先
                    if (!Directory.Exists(Path))
                    {
                        Directory.CreateDirectory(Path);
                    }
                    HOperatorSet.WriteShapeModel(hv_FModelID, Path + "\\" + CCDNAME + "_FigureShape_Square_Model");
                }
            }
            catch (Exception er)
            {
                //MessageBox.Show(er.ToString());
            }
            HOperatorSet.ClearShapeModel(hv_FModelID);
        }
        private void tBBlockWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_IRWidth = tBBlockWidth.Value;
            UDBlockWidth.Value = hv_IRWidth;
        }
        private void UDBlockWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_IRWidth = (int)UDBlockWidth.Value;
            tBBlockWidth.Value = hv_IRWidth;
            ShowFang();
        }
        private void tBBlockdist_ValueChanged(object sender, EventArgs e)
        {
            hv_IRdis = tBBlockdist.Value;
            UDBlockdist.Value = hv_IRdis;
        }
        private void UDBlockdist_ValueChanged(object sender, EventArgs e)
        {
            hv_IRdis = (int)UDBlockdist.Value;
            tBBlockdist.Value = hv_IRdis;
            ShowFang();
        }
        private void tBBlockB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_IRTH = tBBlockB2W.Value;
            UDBlockB2W.Value = hv_IRTH;
        }
        private void UDBlockB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_IRtransition = "positive";
            hv_IRTH = (int)UDBlockB2W.Value;
            tBBlockB2W.Value = hv_IRTH;
            ShowFang();
            if (UDBlockB2W.Value != 1)
                UDBlockW2B.Value = 255;
        }
        private void tBBlockW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_IRTH = tBBlockW2B.Value;
            UDBlockW2B.Value = hv_IRTH;
        }
        private void UDBlockW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_IRtransition = "negative";
            hv_IRTH = (int)UDBlockW2B.Value;
            tBBlockW2B.Value = hv_IRTH;
            ShowFang();
            if (UDBlockW2B.Value != 255)
                UDBlockB2W.Value = 1;
        }
        private void FNozzleCen_Click(object sender, EventArgs e)
        {
            try
            {
                string CCDNAME = ""; string straddress = "";
                if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3" || ViewNum == "5"))
                    return;
                switch (int.Parse(ViewNum))
                {
                    case 1: CCDNAME = "A1CCD1"; straddress = "01WWRD00096,04,"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                    case 3: CCDNAME = "A2CCD1"; straddress = "01WWRD00104,04,"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    case 5: CCDNAME = "PCCD1"; straddress = "01WWRD00112,04,"; xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                }
                if (hv_FModelID.Length == 0 || hv_FModelID == null)
                {
                    HOperatorSet.ReadShapeModel(Sys.IniPath + "\\" + "Image" + "\\" + CCDNAME + "_RegionModel.shm", out hv_FModelID);
                    hv_RowCh = double.Parse(iniFile.Read(CCDNAME, "Frow", parent.setpath));
                    hv_ColumnCh = double.Parse(iniFile.Read(CCDNAME, "Fcol", parent.setpath));
                    hv_angle = double.Parse(iniFile.Read(CCDNAME, "Fangle", parent.setpath));
                    hv_length1 = double.Parse(iniFile.Read(CCDNAME, "Flength1", parent.setpath));
                    hv_length2 = double.Parse(iniFile.Read(CCDNAME, "Flength2", parent.setpath));
                    hv_IRWidth = int.Parse(iniFile.Read(CCDNAME, "FWidth", parent.setpath));
                    hv_IRdis = int.Parse(iniFile.Read(CCDNAME, "FDis", parent.setpath));
                    hv_IRtransition = iniFile.Read(CCDNAME, "Ftransition", parent.setpath);
                    hv_IRTH = int.Parse(iniFile.Read(CCDNAME, "Fthreshold", parent.setpath));
                }
                ShowFang();
                if (hv_FRectangleParameter.Length != 0)
                {
                    HTuple hv_r = hv_FRectangleParameter.TupleSelect(0);
                    HTuple hv_c = hv_FRectangleParameter.TupleSelect(1);
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_ImageSet);
                    hWVision.SetColor("red");
                    hWVision.DispCircle(hv_r, hv_c, 8);
                    hWVision.DispCross(row, col, width, 0);
                    hWVision.SetColor("green");
                    hWVision.DispObj(ho_ResultContours);
                    HD.set_display_font(hWVision, 14, "sans", "true", "false");
                    HOperatorSet.SetTposition(hWVision, 150, 24);
                    HOperatorSet.WriteString(hWVision, ("矩形中心点坐标X= " + (hv_c - col) * xpm));
                    txtFNozzleX.Text = Math.Round((double)(hv_c - col) * xpm, 4).ToString();
                    HOperatorSet.SetTposition(hWVision, 250, 24);
                    HOperatorSet.WriteString(hWVision, ("矩形中心点坐标Y= " + (-(hv_r - row)) * ypm));
                    txtFNozzleY.Text = Math.Round((double)(-(hv_r - row)) * ypm, 4).ToString();
                    HOperatorSet.SetTposition(hWVision, 350, 24);
                    HOperatorSet.WriteString(hWVision, (("矩形角度值Degree = ") + hv_FDeg) + "°");
                    txtFAngle.Text = Math.Round((double)hv_FDeg, 4).ToString();
                    string cmdBX = NToHString((int)(Math.Round((double)(hv_c - col) * xpm, 3) * 1000));
                    string cmdBY = NToHString((int)(Math.Round((double)(-(hv_r - row)) * ypm, 3) * 1000));
                    WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
                    WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
                }
                else
                {
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_ImageSet);
                    HD.set_display_font(hWVision, 14, "sans", "true", "false");
                    HOperatorSet.SetTposition(hWVision, 150, 24);
                    HOperatorSet.WriteString(hWVision, "未找到中心，请重新调节参数！");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void btnFN2Plc_Click(object sender, EventArgs e)
        {
            string straddress = "";
            if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "3" || ViewNum == "5"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 1: straddress = "01WWRD00096,04,"; break;
                case 3: straddress = "01WWRD00104,04,"; break;
                case 5: straddress = "01WWRD00112,04,"; break;
            }
            double Xbs = double.Parse(txtFNozzleX.Text); double Ybs = double.Parse(txtFNozzleY.Text);
            string cmdBX = NToHString((int)(Math.Round(Xbs, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Ybs, 3) * 1000));
            WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        #endregion

        #endregion
        #region 针头求心
        private void btnNeedle_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string straddress = "";
            if (ViewNum != "7")
                return;
            switch (int.Parse(ViewNum))
            {
                case 7: CCDNAME = "GCCD1"; straddress = "01WWRD00120,04,"; xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
            }
            readtemPara(CCDNAME);
            CorShow();
            Xorigin = Xnum.ToString(); Yorigin = Ynum.ToString();
            txtNeedleX.Text = Math.Round(Xb, 5).ToString(); txtNeedleY.Text = Math.Round(Yb, 5).ToString();
            string cmdBX = NToHString((int)(Math.Round(Xb, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Yb, 3) * 1000));
            WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        private void NW2plc_Click(object sender, EventArgs e)
        {
            string straddress = "";
            if (ViewNum != "7")
                return;
            switch (int.Parse(ViewNum))
            {
                case 7: straddress = "01WWRD00120,04,"; break;
            }
            double Xbs = double.Parse(txtNeedleX.Text); double Ybs = double.Parse(txtNeedleY.Text);
            string cmdBX = NToHString((int)(Math.Round(Xbs, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Ybs, 3) * 1000));
            WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        private void btnSaveN_Click(object sender, EventArgs e)
        {
            iniFile.Write("GCCD1", "NeedleX", txtNeedleX.Text, parent.setpath);
            iniFile.Write("GCCD1", "NeedleY", txtNeedleY.Text, parent.setpath);
        }
        #endregion
        #region 支撑座求心
        private void btnCalViewZ_Click(object sender, EventArgs e)
        {
            switch (ti)
            {
                case 0:
                    Sys.bCalView = true;
                    btnCalViewZ.BackColor = Color.Green;
                    btnCalViewZ.Text = "1、结束拍摄并求心";
                    string CCDNAME = "";
                    if (ViewNum == "0" && !(ViewNum == "2" || ViewNum == "4"))
                        return;
                    switch (int.Parse(ViewNum))
                    {
                        case 2: CCDNAME = "A1CCD2"; xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                        case 4: CCDNAME = "A2CCD2"; xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                    }
                    lblShowZ.Show();
                    //CalViewSet(CCDNAME);
                    break;
                case 1:
                    Sys.calviewNum = 0;
                    Sys.calviewNumL = 0;
                    Sys.bCalView = false;
                    btnCalViewZ.BackColor = Color.WhiteSmoke;
                    btnCalViewZ.Text = "1、开始拍摄并求心";
                    lblShowZ.Hide();
                    break;
            }
            ti++;
            if (ti == 2)
            {
                ti = 0;
            }
        }
        private void btnCalMatchZ_Click(object sender, EventArgs e)
        {
            if (btnCalViewZ.Text == "1、结束拍摄并求心")
                btnCalViewZ.PerformClick();
            btnCalMatchZ.BackColor = Color.Green;
            string straddress = "";
            if (ViewNum == "0" && !(ViewNum == "2" || ViewNum == "4"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 2: straddress = "01WWRD00162,04,"; break;
                case 4: straddress = "01WWRD00166,04,"; break;
            }
            if (Sys.pXY[0, 1] == 0 & Sys.pXY[7, 1] == 0)
            {
                btnCalMatchZ.BackColor = Color.WhiteSmoke;
                MessageBox.Show("未取得足够多的角度中心，请重新操作！");
                return;
            }
            double[] result = qiuyuan(Sys.pXY);
            matchp.X = (float)result[0];
            matchp.Y = (float)result[1];
            txtMatchZX.Text = (Math.Round(matchp.X, 3)).ToString();
            txtMatchZY.Text = (Math.Round(matchp.Y, 3)).ToString();
            string Xnum = NToHString((int)(Math.Round(matchp.X, 3) * 1000));
            string Ynum = NToHString((int)(Math.Round(matchp.Y, 3) * 1000));
            string cmdL = Xnum + Ynum;
            WriteToPlc.CMDcheckR = straddress + cmdL + "\r\n";
            WriteToPlc.CMDchecksend = true;
            Sys.pXY = new double[,] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };//      
            btnCalMatchZ.BackColor = Color.WhiteSmoke;
        }
        private void btnMWToplcZ_Click(object sender, EventArgs e)
        {
            string straddress = "";
            if (ViewNum == "0" && !(ViewNum == "1" || ViewNum == "2" || ViewNum == "4"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 2: straddress = "01WWRD00162,04,"; break;
                case 4: straddress = "01WWRD00166,04,"; break;
            }
            double Xbs = double.Parse(txtMatchZX.Text); double Ybs = double.Parse(txtMatchZY.Text);
            string cmdBX = NToHString((int)(Math.Round(Xbs, 3) * 1000));
            string cmdBY = NToHString((int)(Math.Round(Ybs, 3) * 1000));
            WriteToPlc.CMDcheckR = straddress + cmdBX + cmdBY + "\r\n";
            WriteToPlc.CMDchecksend = true;
        }
        private void btnSaveZ_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            if (ViewNum == "0" && !(ViewNum == "2" || ViewNum == "4"))
                return;
            switch (int.Parse(ViewNum))
            {
                case 2: CCDNAME = "A1CCD2"; break;
                case 4: CCDNAME = "A2CCD2"; break;
            }
            iniFile.Write(CCDNAME, "SseatCheckX", txtMatchX.Text, parent.setpath);
            iniFile.Write(CCDNAME, "SseatCheckY", txtMatchY.Text, parent.setpath);
        }
        #endregion
        #region 胶点求心
        private void btnPoint_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; //string straddress = "";
            if (ViewNum != "8")
                return;
            switch (int.Parse(ViewNum))
            {
                case 8: CCDNAME = "GCCD2GluePoint"; xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;//straddress = "01WWRD00154,04,";
            }
            string lgp = iniFile.Read(CCDNAME, "LighterValue", FrmMain.propath);
            if (lgp != "")
            {
                parent.brit = int.Parse(lgp); parent.ch = 4;
                parent.LightSet();
            }
            Thread.Sleep(5);
            parent.OneShot8();
            //readtemPara(CCDNAME);
            //CorShow();
            //Xorigin = Xnum.ToString(); Yorigin = Ynum.ToString();
            //txtGluePointX.Text = Math.Round(Xb, 5).ToString(); txtGluePointY.Text = Math.Round(Yb, 5).ToString();
            //string cmdBX = NToHString((int)(Math.Round(Xb, 3) * 1000));
            //string cmdBY = NToHString((int)(Math.Round(Yb, 3) * 1000));
            //WriteToPlc.CMDresult[int.Parse(ViewNum) - 1] = "01WWRD00154,04," + cmdBX + cmdBY + "\r\n";
            //WriteToPlc.CMDsend[int.Parse(ViewNum) - 1] = true;
        }
        private void btnSavePoint_Click(object sender, EventArgs e)
        {
            iniFile.Write("GCCD2", "GluePointX", txtGluePointX.Text, parent.setpath);
            iniFile.Write("GCCD2", "GluePointY", txtGluePointY.Text, parent.setpath);
        }
        #endregion
        #region 膠前膠後參數
        HObject ho_GlueImage_Befort = new HObject(), ho_GlueImage_After = new HObject(), ho_GlueImage_Befort_2 = new HObject();
        HObject ho_GlueRegion = new HObject(), ho_GlueRegion_2 = new HObject();
        HTuple hv_RowCut_Befort = new HTuple(), hv_ColumnCut_Befort = new HTuple(), hv_RowCut_After = new HTuple(), hv_ColumnCut_After = new HTuple();
        HTuple hv_RowCenter_Befort = new HTuple(), hv_ColumnCenter_Befort = new HTuple(), hv_RowCenter_After = new HTuple(), hv_ColumnCenter_After = new HTuple();
        HTuple hv_RowCenter_Befort_2 = new HTuple(), hv_ColumnCenter_Befort_2 = new HTuple();
        #endregion
        #endregion

        #region 图像处理/辨识
        #region Circle/找圆心
        HTuple hv_regionr = new HTuple(), hv_regionc = new HTuple(), hv_radiusR = new HTuple();
        HObject ho_circleR, ho_selectCir, ho_selectContour = new HObject();
        HObject ho_TmpRegion = new HObject(), ho_ModelRegion = new HObject(), ho_ModelRegionDiff = new HObject(), ho_ImageReduced = new HObject();
        HObject ho_SelectedRegions = new HObject(), RegionMax = new HObject(), ho_RegionFillUp = new HObject();
        HObject ho_Contours = new HObject(), ho_ContCircle = new HObject();
        HTuple AreaS = new HTuple(), RowS = new HTuple(), ColumnS = new HTuple(), AreaMax = new HTuple();
        HTuple RowCen = new HTuple(), ColumnCen = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
        HTuple hv_RowCen = new HTuple(), hv_ColumnCen = new HTuple(), hv_RadiusMax = new HTuple();
        HTuple hv_StartPhi1 = new HTuple(), hv_EndPhi1 = new HTuple(), hv_PointOrder1 = new HTuple();
       
        
        #region Mode2
        private void btnDrawRe2_Click(object sender, EventArgs e)
        {
            int i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            if (tBRegion2.Value == 0)
            {
                btnDrawRe2.BackColor = Color.GreenYellow;
                HD.disp_message(hWVision, "请单击左键，并拉取圆形区域。", "", 100, 100, "green", "false");
                hWVision.SetColor("red");
                HOperatorSet.DrawCircle(hWVision, out hv_regionr, out hv_regionc, out hv_radiusR);
                btnDrawRe2.BackColor = Color.WhiteSmoke;
                HOperatorSet.GenCircle(out ho_circleR, row, col, hv_radiusR);
                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_circleR, out ho_RegionReduced);
                tBRegion2.Value = (int)Math.Round((double)hv_radiusR);
            }
            else
            {
                HD.disp_message(hWVision, "请直接调节检测区域半径。", "", 100, 100, "green", "false");
            }
        }
        public void DrawRegion1()
        {
            if (readpara)
                return;
            if (ho_ImageSet == null)
                return;
            int i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);

            hWVision.SetColor("red");
            HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
            ho_RegionReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_RegionReduced);
            if (halcon.IsCrossDraw)
            {
                HOperatorSet.SetColor(hWVision, "red");
                HD.CrossDraw(hWVision, width, height);
            }
        }
        private void tBRegion2_ValueChanged(object sender, EventArgs e)
        {
            RegionRadius = tBRegion2.Value;
            UDRegion2.Value = RegionRadius;
        }
        private void UDRegion2_ValueChanged(object sender, EventArgs e)
        {
            RegionRadius = (HTuple)UDRegion2.Value;
            tBRegion2.Value = RegionRadius;
            DrawRegion1();
        }
        private void tBgray2_ValueChanged(object sender, EventArgs e)
        {
            binBvi = tBgray2.Value;
            UDgray2.Value = binBvi;
        }
        private void UDgray2_ValueChanged(object sender, EventArgs e)
        {
            binBvi = (int)UDgray2.Value;
            tBgray2.Value = binBvi;
            binBviShow1();
        }
        public void binBviShow1()
        {
            if (readpara)
                return;
            try
            {
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBRegion2.Value;
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                ho_Border.Dispose();
                HOperatorSet.ThresholdSubPix(ho_ImageTest, out ho_Border, binBvi);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageTest);
                hWVision.SetColor("red");
                hWVision.DispObj(ho_Border);
                if (halcon.IsCrossDraw)
                {
                    HOperatorSet.SetColor(hWVision, "red");
                    HD.CrossDraw(hWVision, width, height);
                }
            }
            catch
            {
                hWVision.ClearWindow();
                //hWVision.DispObj(ho_ImageSet);
            }
        }
        HTuple hv_grayRow = new HTuple(), hv_grayColumn = new HTuple(), hv_grayRadius = new HTuple();
        private void cBGrayChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGrayChecked.Checked)
            {
                tBgray2.Enabled = true;
                UDgray2.Enabled = true;
            }
            else
            {
                tBgray2.Enabled = false;
                UDgray2.Enabled = false;
            }
        }
        private void btnDrawCirR2_Click(object sender, EventArgs e)
        {
            #region
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            ho_ImageTest.Dispose();
            HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
            if (tBRegion2.Value != 0)
            {
                RegionRadius = (HTuple)tBRegion2.Value;
                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
            }
            if (cBGrayChecked.Checked)
            {
                #region graycheck
                try
                {
                    ho_Border.Dispose();
                    HOperatorSet.ThresholdSubPix(ho_ImageTest, out ho_Border, binBvi);
                    ho_ContoursSplit.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Border, out ho_ContoursSplit, "lines_circles", 5, 4, 3);
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_SelectedXLD, "contlength", "and", 50, 99999);
                    HOperatorSet.CountObj(ho_SelectedXLD, out hv_Number1);
                    HOperatorSet.GenEmptyObj(out ho_selectCir);
                    for (int i = 1; i <= (int)hv_Number1; i++)
                    {
                        ho_selectContour.Dispose();
                        HOperatorSet.SelectObj(ho_SelectedXLD, out ho_selectContour, i);
                        HTuple hv_attrib;
                        HOperatorSet.GetContourGlobalAttribXld(ho_selectContour, "cont_approx", out hv_attrib);
                        if (hv_attrib.D == 1)
                            HOperatorSet.ConcatObj(ho_selectCir, ho_selectContour, out ho_selectCir);
                    }
                    HOperatorSet.CountObj(ho_selectCir, out hv_Number1);
                    ho_ObjectSelectedM.Dispose();
                    HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedM, 1);
                    HOperatorSet.LengthXld(ho_ObjectSelectedM, out hv_LengthMax1);
                    HTuple hv_max_L = 0.0;
                    hv_max_L = hv_LengthMax1.Clone();
                    for (int i = 2; i < (int)hv_Number1; i++)
                    {
                        ho_ObjectSelectedN.Dispose();
                        HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedN, i);
                        HOperatorSet.LengthXld(ho_ObjectSelectedN, out hv_Length1);
                        if ((int)(new HTuple(hv_Length1.TupleGreater(hv_max_L))) != 0)
                        {
                            hv_max_L = hv_Length1.Clone();
                            ho_ObjectSelectedM.Dispose();
                            HOperatorSet.SelectObj(ho_selectCir, out ho_ObjectSelectedM, i);
                        }
                    }
                    try
                    {
                        HOperatorSet.FitCircleContourXld(ho_ObjectSelectedM, "algebraic", -1, 0, 0,
                            3, 2, out hv_fitRow, out hv_fitCol, out hv_fitRadius, out hv_StartPhi, out hv_EndPhi,
                            out hv_PointOrder);
                        hv_RingRow = hv_fitRow; hv_RingCol = hv_fitCol;
                        hv_RingRadius = hv_fitRadius;
                        tBRRadius.Value = (int)Math.Round((double)hv_fitRadius);
                        hv_RDetectHeight = (int)UDRWidth.Value;
                        hv_transition = "all";
                        if (tBCirB2W2.Value != 1)
                            hv_transition = "negative";
                        if (tBCirW2B2.Value != 255)
                            hv_transition = "positive";

                        HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                        HOperatorSet.GenEmptyObj(out ho_Contour);
                        HOperatorSet.GenEmptyObj(out ho_Cross);
                        HOperatorSet.GenEmptyObj(out ho_UsedEdges);

                        ShowRing1();
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("请重新调节初始灰度阀值！" + ex.ToString());
                }
                #endregion
            }
            else
            {
                hv_grayRow = new HTuple(); hv_grayColumn = new HTuple(); hv_grayRadius = new HTuple();
                HD.disp_message(hWVision, "请单击左键，并拉取圆形区域。", "", 100, 100, "green", "false");
                btnDrawCirR2.BackColor = Color.GreenYellow;
                hWVision.SetColor("red");
                HOperatorSet.DrawCircle(hWVision, out hv_grayRow, out hv_grayColumn, out hv_grayRadius);
                btnDrawCirR2.BackColor = Color.WhiteSmoke;
                hv_RingRow = hv_grayRow; hv_RingCol = hv_grayColumn; hv_RingRadius = hv_grayRadius;
                //RingCrow = hv_RowRC; RingCcolumn = hv_ColumnRC;
                hv_transition = "all";
                if (tBCirB2W2.Value != 1)
                    hv_transition = "negative";
                if (tBCirW2B2.Value != 255)
                    hv_transition = "positive";
                tBRRadius.Value = (int)Math.Round((double)hv_grayRow);
                hv_RDetectHeight = (int)UDRWidth.Value;
                ShowRing1();
            }
            #endregion
        }
        private void tBRRadius_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = tBRRadius.Value;
            UDRRadius.Value = hv_RingRadius;
        }
        private void UDRRadius_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = (HTuple)UDRRadius.Value;
            tBRRadius.Value = hv_RingRadius;
            ShowRing1();
        }
        #region old
        //public void ShowRing1()
        //{
        //    if (readpara)
        //        return;
        //    int i_image = int.Parse(SetNum) - 1;
        //    ho_ImageSet.Dispose();
        //    HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
        //    HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
        //    HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
        //    ho_ImageTest.Dispose();
        //    HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
        //    if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
        //    {
        //        RegionRadius = (HTuple)tBRegion2.Value;
        //        HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
        //        ho_RegionReduced.Dispose();
        //        HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
        //        ho_ImageTest.Dispose();
        //        HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
        //    }
        //    hWVision.ClearWindow();
        //    hWVision.DispObj(ho_ImageTest);
        //    HOperatorSet.GenEmptyObj(out ho_RRegions);
        //    HOperatorSet.GenEmptyObj(out ho_RContCircle);
        //    HOperatorSet.GenCircleContourXld(out ho_RContCircle, hv_RingRow, hv_RingCol, hv_RingRadius, 0, 6.28318, "positive", 1);
        //    OTemp[SP_O] = ho_RRegions.CopyObj(1, -1);
        //    SP_O++;
        //    ho_RRegions.Dispose();
        //    HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_RContCircle, out ho_RRegions);
        //    OTemp[SP_O - 1].Dispose();
        //    SP_O = 0;
        //    HOperatorSet.GetContourXld(ho_RContCircle, out hv_RowXLD, out hv_ColXLD);
        //    //假设一条平行于x的直线，起点和终点分别是圆最右侧点的两侧  275
        //    HOperatorSet.DistancePp(hv_RingRow, hv_RingRow, (hv_RingRow + hv_RingRadius) + 10, hv_RingCol, out hv_DistanceStart);
        //    HOperatorSet.DistancePp(hv_RingRow, hv_RingRow, (hv_RingRow + hv_RingRadius) - 10, hv_RingCol, out hv_DistanceEnd);
        //    //计算圆形的轮廓长度
        //    HOperatorSet.LengthXld(ho_RContCircle, out hv_RLength);
        //    HOperatorSet.TupleLength(hv_ColXLD, out hv_RLength2);
        //    hv_RResultRow = new HTuple();
        //    hv_RResultColumn = new HTuple();
        //    hv_Elements = 180;
        //    for (hv_i = 0; hv_i.Continue(hv_Elements - 1, hv_Elements / 180); hv_i = hv_i.TupleAdd(hv_Elements / 180))
        //    {
        //        if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(hv_RLength2 - 1)))) != 0)
        //            HOperatorSet.TupleInt(((1.0 * hv_RLength2) / 179) * hv_i, out hv_j);
        //        else
        //            HOperatorSet.TupleInt(((1.0 * hv_RLength2) / 179) * hv_i, out hv_j);
        //        if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_RLength2))) != 0)
        //            hv_j = hv_RLength2 - 1;
        //        hv_RowE = hv_RowXLD.TupleSelect(hv_j);
        //        hv_ColE = hv_ColXLD.TupleSelect(hv_j);
        //        //超出图像区域，不检测，否则容易报异常
        //        if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(height - 1))).TupleOr(
        //            new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
        //            width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
        //        {
        //            continue;
        //        }
        //        //比较距离来计算角度
        //        if ((int)(new HTuple(hv_DistanceStart.TupleGreater(hv_DistanceEnd))) != 0)
        //        {
        //            //以开始点Row1为原点，来计算线的角度
        //            HOperatorSet.TupleAtan2((-hv_RowE) + hv_RingRow, hv_ColE - hv_RingCol, out hv_RATan);
        //            hv_RATan = ((new HTuple(180)).TupleRad()) + hv_RATan;
        //            hv_Direct = "inner";
        //        }
        //        else
        //        {
        //            //以结束点Row2为原点，来计算线的角度
        //            HOperatorSet.TupleAtan2((-hv_RowE) + hv_RingRow, hv_ColE - hv_RingCol, out hv_RATan);
        //            hv_Direct = "outer";
        //        }
        //        if ((int)(new HTuple(hv_i.TupleGreaterEqual(0))) != 0)
        //        {
        //            hv_RowL1 = hv_RowE + (hv_RDetectHeight * (((-hv_RATan)).TupleSin()));
        //            hv_RowL2 = hv_RowE - (hv_RDetectHeight * (((-hv_RATan)).TupleSin()));
        //            hv_ColL1 = hv_ColE + (hv_RDetectHeight * (((-hv_RATan)).TupleCos()));
        //            hv_ColL2 = hv_ColE - (hv_RDetectHeight * (((-hv_RATan)).TupleCos()));
        //            hv_RradiusIn = Math.Sqrt(Math.Abs((double)hv_RowL1 - (double)hv_RingRow) * Math.Abs((double)hv_RowL1 - (double)hv_RingRow) +
        //        Math.Abs((double)hv_ColL1 - (double)hv_RingCol) * Math.Abs((double)hv_ColL1 - (double)hv_RingCol));
        //            HOperatorSet.GenCircleContourXld(out ho_RCircleIn, hv_RingRow, hv_RingCol, hv_RradiusIn, 0, 6.28318, "positive", 1);
        //            hv_RradiusOut = Math.Sqrt(Math.Abs((double)hv_RowL2 - (double)hv_RingRow) * Math.Abs((double)hv_RowL2 - (double)hv_RingRow) +
        //        Math.Abs((double)hv_ColL2 - (double)hv_RingCol) * Math.Abs((double)hv_ColL2 - (double)hv_RingCol));
        //            HOperatorSet.GenCircleContourXld(out ho_RCircleOut, hv_RingRow, hv_RingCol, hv_RradiusOut, 0, 6.28318, "positive", 1);
        //            if (HDevWindowStack.IsOpen())
        //                HOperatorSet.SetColor(HDevWindowStack.GetActive(), "green");
        //            ho_Arrow1.Dispose();
        //            HD.gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2, 25, 25);
        //            OTemp[SP_O] = ho_RRegions.CopyObj(1, -1);
        //            SP_O++;
        //            ho_RRegions.Dispose();
        //            HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_Arrow1, out ho_RRegions);
        //            OTemp[SP_O - 1].Dispose();
        //            SP_O = 0;
        //            hv_ROIWidth = 14;
        //            HOperatorSet.SetSystem("int_zooming", "true");
        //            hv_TmpCtrl_Row = 0.5 * (hv_RowL2 + hv_RowL1);
        //            hv_TmpCtrl_Column = 0.5 * (hv_ColL2 + hv_ColL1);
        //            hv_TmpCtrl_Dr = hv_RowL1 - hv_RowL2;
        //            hv_TmpCtrl_Dc = hv_ColL2 - hv_ColL1;
        //            hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(hv_TmpCtrl_Dc);
        //            hv_TmpCtrl_Len1 = 0.5 * ((((hv_TmpCtrl_Dr * hv_TmpCtrl_Dr) + (hv_TmpCtrl_Dc * hv_TmpCtrl_Dc))).TupleSqrt()
        //                );
        //            hv_TmpCtrl_Len2 = hv_ROIWidth.Clone();
        //            HOperatorSet.GenMeasureRectangle2(hv_TmpCtrl_Row, hv_TmpCtrl_Column, hv_TmpCtrl_Phi,
        //                hv_TmpCtrl_Len1, hv_TmpCtrl_Len2, 2592, 1944, "nearest_neighbor", out hv_MsrHandle_Measure_02_0);
        //            OTemp[SP_O] = ho_ImageTest.CopyObj(1, -1);
        //            SP_O++;
        //            ho_ImageTest.Dispose();
        //            HOperatorSet.CopyObj(OTemp[SP_O - 1], out ho_ImageTest, 1, 1);
        //            OTemp[SP_O - 1].Dispose();
        //            SP_O = 0;
        //            if (hv_transition == "")
        //                hv_transition = "all";
        //            HOperatorSet.MeasurePos(ho_ImageTest, hv_MsrHandle_Measure_02_0, 1, hv_RAmplitudeThreshold, hv_transition,
        //                "all", out hv_RRowEdge, out hv_RColEdge, out hv_Amplitude, out hv_Distance);
        //            HOperatorSet.CloseMeasure(hv_MsrHandle_Measure_02_0);
        //            hv_tRow = 0;
        //            hv_tCol = 0;
        //            hv_t = 0;
        //            HOperatorSet.TupleLength(hv_RRowEdge, out hv_Number);
        //            if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
        //            {
        //                continue;
        //            }
        //            //循环求
        //            for (hv_k = 0; hv_k.Continue(hv_Number - 1, 1); hv_k = hv_k.TupleAdd(1))
        //            {
        //                if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
        //                    hv_t))) != 0)
        //                {
        //                    hv_tRow = hv_RRowEdge.TupleSelect(hv_k);
        //                    hv_tCol = hv_RColEdge.TupleSelect(hv_k);
        //                    hv_t = ((hv_Amplitude.TupleSelect(hv_k))).TupleAbs();
        //                }
        //            }
        //            hWVision.SetColor("blue");
        //            hWVision.DispCircle(hv_tRow, hv_tCol, 8);
        //            if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
        //            {
        //                hv_RResultRow = hv_RResultRow.TupleConcat(hv_tRow);
        //                hv_RResultColumn = hv_RResultColumn.TupleConcat(hv_tCol);
        //            }
        //        }
        //    }
        //    hWVision.SetColor("green");
        //    hWVision.DispObj(ho_RRegions);
        //    hWVision.DispObj(ho_RCircleIn);
        //    hWVision.DispObj(ho_RCircleOut);
        //}
        #endregion
        #region new
        public void ShowRing1()
        {
            if (readpara)
                return;
            try
            {
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBRegion2.Value;
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                //HOperatorSet.GenCircleContourXld(out ho_RContCircle, hv_RingRow, hv_RingCol, hv_RingRadius, 0, 6.28318, "positive", 1);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.GenCircle(out ho_Circle, hv_RingRow, hv_RingCol, hv_RingRadius);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour, hv_MetrologyHandle, "all", 1.5);
                ho_MeasureContour.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_MeasureContour, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_Contour.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour, hv_MetrologyHandle, "all", hv_transition, out hv_Row1, out hv_Column1);
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row1, hv_Column1, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "used_edges", "row", out hv_UsedRow);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "used_edges", "column", out hv_UsedColumn);
                ho_UsedEdges.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_UsedEdges, hv_UsedRow, hv_UsedColumn, 10, (new HTuple(45)).TupleRad());
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                hWVision.DispObj(ho_ImageTest);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_Contour);
                hWVision.SetColor("blue");
                hWVision.DispObj(ho_UsedEdges);

                ho_ImageTest.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                ho_Contour.Dispose();
                ho_Cross.Dispose();
                ho_UsedEdges.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        #endregion
        private void tBRWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = tBRWidth.Value;
            UDRWidth.Value = hv_RDetectHeight;
        }
        private void UDRWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = (HTuple)UDRWidth.Value;
            tBRWidth.Value = hv_RDetectHeight;
            ShowRing1();
        }
        private void tBCirW2B2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBCirW2B2.Value;
            UDCirW2B2.Value = hv_RAmplitudeThreshold;
        }
        private void UDCirW2B2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDCirW2B2.Value;
            tBCirW2B2.Value = hv_RAmplitudeThreshold;
            tBCirB2W2.Value = 1;
            hv_transition = "positive";
            ShowRing1();
        }
        private void tBCirB2W2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBCirB2W2.Value;
            UDCirB2W2.Value = hv_RAmplitudeThreshold;
        }
        private void UDCirB2W2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDCirB2W2.Value;
            tBCirB2W2.Value = hv_RAmplitudeThreshold;
            tBCirW2B2.Value = 255;
            hv_transition = "negative";
            ShowRing1();
        }
        HTuple hv_HoldRow = new HTuple(), hv_HoldCol = new HTuple(), hv_HoldRadius = new HTuple();
        private void btnCir2_Click(object sender, EventArgs e)
        {
            switch (int.Parse(SetNum))
            {
                case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm;
                    hv_HoldRow = null; hv_HoldCol = null; hv_HoldRadius = null; break;
                case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            try
            {
                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_CrossCenter);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);
                HOperatorSet.GenEmptyObj(out ho_ResultContours);

                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBRegion2.Value;
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_ResultContours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours, hv_MetrologyHandle, "all", "all", 1.5);
                hv_ColCenter = hv_circleParameter.TupleSelect(1);
                hv_RowCenter = hv_circleParameter.TupleSelect(0);
                hv_CenterRadius = hv_circleParameter.TupleSelect(2);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                if (SetNum == "5")
                {
                    hv_HoldRow = hv_RowCenter;
                    hv_HoldCol = hv_ColCenter;
                    hv_HoldRadius = hv_CenterRadius;
                }
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_ResultContours);
                hWVision.SetColor("red");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                if (halcon.IsCrossDraw)
                    hWVision.DispCross(row, col, width, 0);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm, "", 150, 150, "green", "false");
                HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row) * ypm), "", 300, 150, "green", "false");
                HD.disp_message(hWVision, "R:" + hv_CenterRadius, "", 450, 150, "green", "false");
                //HD.disp_message(hWVision, "D:" + Math.Round((double)hv_CenterRadius * 0.004477*2, 4), "", 450, 150, "green", "false");
                ho_ImageTest.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                ho_ResultContours.Dispose();
                if (cBGlueQ.Checked)
                {
                    hv_RowCenter_Befort = hv_RowCenter;
                    hv_ColumnCenter_Befort = hv_ColCenter;
                }
                if (cBGlueH.AutoCheck)
                {
                    hv_RowCenter_After = hv_RowCenter;
                    hv_ColumnCenter_After = hv_ColCenter;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tBPRmin2_ValueChanged(object sender, EventArgs e)
        {
            int pr = tBPRmin2.Value;
            UDPRmin2.Value = pr;
        }
        private void UDPRmin2_ValueChanged(object sender, EventArgs e)
        {
            int pr = (int)UDPRmin2.Value;
            tBPRmin2.Value = pr;
        }
        private void tBPRmax2_ValueChanged(object sender, EventArgs e)
        {
            int pr = tBPRmax2.Value;
            UDPRmax2.Value = pr;
        }
        private void UDPRmax2_ValueChanged(object sender, EventArgs e)
        {
            int pr = (int)UDPRmax2.Value;
            tBPRmax2.Value = pr;
        }
        private void txtXmax2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void txtXmin2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void txtYmax2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void txtYmin2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void btnSaveCir2_Click(object sender, EventArgs e)
        {
            #region showchoose
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area3 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == "") ||
                                 (cBtest.Enabled && cBtest.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (cBtest.SelectedIndex == 0)
                area3 = "Hold";
            if (cBtest.SelectedIndex == 1)
                area3 = "Lens";
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if ((SetNum == "1" || SetNum == "3"))
            {
                if (cBoxTest.Checked)
                {
                    CCDNAME = CCDNAME + "-" + area3;
                    iniFile.Write(CCDNAME, "Location", area3, FrmMain.propath);
                }
                else
                {
                    CCDNAME = CCDNAME + "-" + area4;
                    iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
                }
            }
            if (SetNum == "2" || SetNum == "4")
            {
                if (cBoxTest.Checked)
                {
                    CCDNAME = cn + "-" + area3;
                    iniFile.Write(CCDNAME, "Location", area3, FrmMain.propath);
                }
                else
                    iniFile.Write(cn, "Location", area1, FrmMain.propath);
            }
            if (SetNum == "5")
            {
                if (cBoxTest.Checked)
                {
                    CCDNAME = CCDNAME + "-" + area3;
                    iniFile.Write("PCCD1", "Location", area3, FrmMain.propath);
                }
            }
            if (SetNum == "6")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            if (SetNum == "8")
            {
                iniFile.Write(cn, "Location", area2, FrmMain.propath);
                string astatus = ((cBCutGQ.Checked) ? "true" : "false");
                iniFile.Write(CCDNAME, "GQAngleStatus", astatus, FrmMain.propath);
                astatus = ((cBCutGH.Checked) ? "true" : "false");
                iniFile.Write(CCDNAME, "GHAngleStatus", astatus, FrmMain.propath);
                iniFile.Write(CCDNAME, "Glue_Follow", Glue.Glue_Follow.ToString(), FrmMain.propath);

            }
            else
            {
                string astatus = ((cBCut.Checked) ? "true" : "false");
                iniFile.Write(CCDNAME, "AngleStatus", astatus, FrmMain.propath);
            }
            string sn = "";
            if (cbFigureShape.SelectedIndex == 1)
                sn = "Square";
            if (cbFigureShape.SelectedIndex == 0)
                sn = "Circle";
            iniFile.Write(CCDNAME, "FigureShape", sn, FrmMain.propath);
            #endregion
            #region 光源亮度
            if (SetNum == "1")
            {
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED1Lig.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LighterValue2", (UD_LED2Lig.Value).ToString(), FrmMain.propath);
            }
            if (SetNum == "2")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED3Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "3")
            {
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED4Lig.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LighterValue2", (UD_LED5Lig.Value).ToString(), FrmMain.propath);
            }
            if (SetNum == "4")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED6Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "5")
            {
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED7Lig.Value).ToString(), FrmMain.propath);
                //iniFile.Write(CCDNAME, "LighterValue2", (UD_LED8Lig.Value).ToString(), FrmMain.propath); //调整给QCCD
            }
            if (SetNum == "6")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED9Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "7")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED10Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED11Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "9")
            {
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED8Lig.Value).ToString(), FrmMain.propath);
                //iniFile.Write(CCDNAME, "LighterValue1", (UD_LED12Lig.Value).ToString(), FrmMain.propath);
                //iniFile.Write(CCDNAME, "LighterValue2", (UD_LED13Lig.Value).ToString(), FrmMain.propath);
                //iniFile.Write(CCDNAME, "LighterValue3", (UD_LED14Lig.Value).ToString(), FrmMain.propath);
                //iniFile.Write(CCDNAME, "LighterValue4", (UD_LED15Lig.Value).ToString(), FrmMain.propath);
            }
            #endregion
            iniFile.Write(CCDNAME, "Mode2RegionRadius", (tBRegion2.Value).ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "Mode2gray", tBgray2.Value.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "GrayChecked", cBGrayChecked.Checked.ToString(), FrmMain.propath);
            if (!cBGrayChecked.Checked && hv_grayRow.ToString().Length >= 3)
            {
                iniFile.Write(CCDNAME, "Mode2RingCRow", (Math.Round((double)hv_grayRow, 3)).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "Mode2RingCColumn", (Math.Round((double)hv_grayColumn, 3)).ToString(), FrmMain.propath);
            }
            iniFile.Write(CCDNAME, "Mode2RingRadius", (tBRRadius.Value).ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "Mode2UDRingWidth", (tBRWidth.Value).ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "Transition", (string)hv_transition, FrmMain.propath);
            iniFile.Write(CCDNAME, "ZoneRmin", tBPRmin2.Value.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "ZoneRmax", tBPRmax2.Value.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "Mode2BaCenX+", txtXmax2.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "Mode2BaCenX-", txtXmin2.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "Mode2BaCenY+", txtYmax2.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "Mode2BaCenY-", txtYmin2.Text, FrmMain.propath);
            if (hv_transition == "negative")
                iniFile.Write(CCDNAME, "Mode2UDRingThreshold", (tBCirB2W2.Value).ToString(), FrmMain.propath);
            if (hv_transition == "positive")
                iniFile.Write(CCDNAME, "Mode2UDRingThreshold", (tBCirW2B2.Value).ToString(), FrmMain.propath);
            if (CCDNAME == "PCCD2-PickUp")
                iniFile.Write(CCDNAME, "PickUpAVI", cBAVI.Checked.ToString(), FrmMain.propath);

            //Square
            iniFile.Write(CCDNAME, "FWidth", UDSWidth.Value.ToString(), FrmMain.propath);
            IniFile.Write(CCDNAME, "RectangleLength1_FigureShape", ucRectangleLength1_FigureShape.Value.ToString(), FrmMain.propath);
            IniFile.Write(CCDNAME, "RectangleLength2_FigureShape", ucRectangleLength2_FigureShape.Value.ToString(), FrmMain.propath);
            if (UDSB2W.Value != 1)
            {
                iniFile.Write(CCDNAME, "Ftransition", "negative", FrmMain.propath);
                iniFile.Write(CCDNAME, "Fthreshold", UDSB2W.Value.ToString(), FrmMain.propath);
            }
            if (UDSW2B.Value != 255)
            {
                iniFile.Write(CCDNAME, "Ftransition", "positive", FrmMain.propath);
                iniFile.Write(CCDNAME, "Fthreshold", UDSW2B.Value.ToString(), FrmMain.propath);
            }
            iniFile.Write(CCDNAME, "Fselect", hv_Select.ToString(), FrmMain.propath);
        }
        #endregion
        #endregion

        #region 找角度
        private void cBAMode_ValueChanged(object sender, EventArgs e)
        {
            tabAngle.SelectedIndex = (int)cBAMode.Value - 1;
        }
        private void tabAngle_SelectedIndexChanged(object sender, EventArgs e)
        {
            cBAMode.Value = tabAngle.SelectedIndex + 1;
            if (cBAMode.Value == 3)
            {
                btnCutCheck.Enabled = false;
                gBXYrange.Enabled = false;
                btnSaveCut.Enabled = false;
            }
            else
            {
                btnCutCheck.Enabled = true;
                gBXYrange.Enabled = true;
                btnSaveCut.Enabled = true;
            }
        }

        #region Mode1
        private void btnLineLocate_Click(object sender, EventArgs e)
        {
            HD.disp_message(hWVision, "点击鼠标左键画检测区域,点击右键确认", "", 100, 100, "green", "false");
            btnLineLocate.BackColor = Color.GreenYellow;
            hWVision.SetColor("red");
            HOperatorSet.DrawRectangle2(hWVision, out hv_DLRowCh, out hv_DLColumnCh, out hv_DLangle, out hv_DLlength1, out hv_DLlength2);
            btnLineLocate.BackColor = Color.WhiteSmoke;
            ShowDLRegion();
        }
        private void btnDrawline_Click(object sender, EventArgs e)
        {
            #region CCD
            string CCDNAME = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
                CCDNAME = CCDNAME + "-" + area4;
            #endregion
            HD.set_display_font(hWVision, 18, "sans", "true", "false");
            string Tfilepath = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME + "\\DegTemplate";
            if (hv_DLModelID.Length == 0 || hv_DLModelID == null)
                HOperatorSet.ReadShapeModel(Tfilepath + "\\DegLocation.shm", out hv_DLModelID);

            HD.disp_message(hWVision, "点击鼠标左键在对应边缘“以逆时针方向”画一条直线,点击右键确认", "", 100, 100, "red", "true");
            hWVision.SetColor("red");
            btnDrawline.BackColor = Color.GreenYellow;
            HOperatorSet.DrawLine(hWVision, out hv_RowIRLs, out hv_ColIRLs, out hv_RowIRLe, out hv_ColIRLe);
            btnDrawline.BackColor = Color.WhiteSmoke;
            DegLineShow();
        }
        private void tBlineThreshold_ValueChanged(object sender, EventArgs e)
        {
            int dlth = tBlineThreshold.Value;
            UDlineThreshold.Value = dlth;
            hv_DegLth = dlth;
            DegLineShow();
        }
        private void UDlineThreshold_ValueChanged(object sender, EventArgs e)
        {
            int dlth = (int)UDlineThreshold.Value;
            tBlineThreshold.Value = dlth;
        }
        private void tBlineWidth_ValueChanged(object sender, EventArgs e)
        {
            int dlw = tBlineWidth.Value;
            UDlineWidth.Value = dlw;
            hv_DegLWidth = dlw;
            DegLineShow();
        }
        private void UDlineWidth_ValueChanged(object sender, EventArgs e)
        {
            int dlw = (int)UDlineWidth.Value;
            tBlineWidth.Value = dlw;
        }
        #endregion
        #region Mode2
        int RadiusMin = 244; int RadiusMax = 280;
        HObject ho_CutRegion1 = new HObject(), ho_CutRegion2 = new HObject(), ho_ImageReduced1 = new HObject(), ho_Region = new HObject();
        HObject ho_SelectedRegions1 = new HObject(), ho_SelectedRegionsMax = new HObject(), ho_ConnectedRegions = new HObject();
        HTuple hv_AreaCut = new HTuple(), hv_RowCut = new HTuple(), hv_ColumnCut = new HTuple(), hv_Deg2 = new HTuple();
        private void btnDrawARing_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0)
                return;
            if (hv_RowCenter == 0.0)
                return;
            RadiusMax = (int)Math.Round((double)hv_CenterRadius) - 2;
            RadiusMin = RadiusMax - 20;
            ShowDonut();
        }
        private void ShowDonut()
        {
            if (readpara)
                return;
            try
            {
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetDraw("margin");
                hWVision.SetLineWidth(1);
                hWVision.SetColor("red");
                hWVision.DispCross(row, col, width, 0);
                hWVision.SetColor("green");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, RadiusMin);
                hWVision.SetLineWidth(2);
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, RadiusMax);
            }
            catch
            {
            }
        }
        private void tBAMinRa_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = tBAMinRa.Value;
            UDAMinRa.Value = RadiusMin;
        }
        private void UDAMinRa_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = (HTuple)UDAMinRa.Value;
            tBAMinRa.Value = RadiusMin;
            ShowDonut();
        }
        private void tBAMaxRa_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = tBAMaxRa.Value;
            UDAMaxRa.Value = RadiusMax;
        }
        private void UDAMaxRa_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = (HTuple)UDAMaxRa.Value;
            tBAMaxRa.Value = RadiusMax;
            ShowDonut();
        }
        int binBW = 1, binWB = 255;
        private void tBAB2W_ValueChanged(object sender, EventArgs e)
        {
            binBW = tBAB2W.Value;
            UDAB2W.Value = binBW;
        }
        private void UDAB2W_ValueChanged(object sender, EventArgs e)
        {
            binBW = (int)UDAB2W.Value;
            tBAB2W.Value = binBW;
            tBAW2B.Value = 255;
            BinShow();
        }
        private void BinShow()
        {
            if (readpara)
                return;
            //找剪口
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            ho_CutRegion1.Dispose();
            HOperatorSet.GenCircle(out ho_CutRegion1, hv_RowCenter, hv_ColCenter, RadiusMin);
            ho_CutRegion2.Dispose();
            HOperatorSet.GenCircle(out ho_CutRegion2, hv_RowCenter, hv_ColCenter, RadiusMax);
            OTemp[SP_O] = ho_CutRegion2.CopyObj(1, -1);
            SP_O++;
            ho_CutRegion2.Dispose();
            HOperatorSet.Difference(OTemp[SP_O - 1], ho_CutRegion1, out ho_CutRegion2);
            OTemp[SP_O - 1].Dispose();
            SP_O = 0;
            ho_ImageReduced1.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_CutRegion2, out ho_ImageReduced1);
            ho_Region.Dispose();
            HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region, binBW, binWB);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
            ho_SelectedRegions1.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, "area", "and", 500, 999999);
            HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_AreaCut, out hv_RowCut, out hv_ColumnCut);
            hWVision.ClearWindow();
            hWVision.SetDraw("fill");
            hWVision.DispObj(ho_ImageSet);
            hWVision.DispObj(ho_Region);
            for (int i = 0; i < hv_AreaCut.Length; i++)
            {
                HD.disp_message(hWVision, Math.Round((double)hv_AreaCut[i]).ToString(), "", hv_RowCut[i], hv_ColumnCut[i], "blue", "false");
            }
        }
        private void tBAW2B_ValueChanged(object sender, EventArgs e)
        {
            binWB = tBAW2B.Value;
            UDAW2B.Value = binWB;
        }
        private void UDAW2B_ValueChanged(object sender, EventArgs e)
        {
            binWB = (int)UDAW2B.Value;
            tBAW2B.Value = binWB;
            tBAB2W.Value = 1;
            BinShow();
        }
        int cutamin = 1000, cutamax = 30000;
        private void tBAAreaMin_ValueChanged(object sender, EventArgs e)
        {
            cutamin = tBAAreaMin.Value;
            UDAAreaMin.Value = cutamin;
        }
        private void UDAAreaMin_ValueChanged(object sender, EventArgs e)
        {
            cutamin = (int)UDAAreaMin.Value;
            tBAAreaMin.Value = cutamin;
            CutAreaShow();
        }
        HObject ho_Rectangle = new HObject(), ho_EmptyRectangle = new HObject(), ho_emSelectedRegion = new HObject(), ho_SelectedRegion = new HObject();
        HTuple hv_emRow = new HTuple(), hv_emColumn = new HTuple(), hv_emPhi = new HTuple(), hv_emLength1 = new HTuple(), hv_emLength2 = new HTuple();
        HTuple hv_i = new HTuple(), hv_j = new HTuple(), hv_val1 = new HTuple(), hv_val2 = new HTuple(), hv_Number2 = new HTuple(), hv_valueM = new HTuple(), hv_va = new HTuple();
        HTuple hv_CRow2 = new HTuple(), hv_CColumn2 = new HTuple(), hv_CPhi = new HTuple(), hv_CLength1 = new HTuple(), hv_CLength2 = new HTuple();
        HTuple hv_Areare = new HTuple(), hv_Rowre = new HTuple(), hv_Columnre = new HTuple(), hv_valueLM = new HTuple();
        HTuple hv_Areareall = new HTuple(), hv_Rowreall = new HTuple(), hv_Columnreall = new HTuple();
        int Widmax = 30, Widmin = 0, ALmax = 200, ALmin = 60;
        private void CutAreaShow()
        {
            if (readpara)
                return;
            try
            {
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                if (cutamin < cutamax)
                {
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, "area", "and", cutamin, cutamax);
                    HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_AreaCut, out hv_RowCut, out hv_ColumnCut);
                    HOperatorSet.SelectObj(ho_SelectedRegions1, out ho_SelectedRegionsMax, 1);
                    hWVision.ClearWindow();
                    hWVision.SetDraw("fill");
                    hWVision.DispObj(ho_ImageSet);
                    if (!cBAddFCT.Checked)
                    {
                        #region
                        if (cutamin < hv_AreaCut && cutamax > hv_AreaCut)
                        {
                            if (hv_AreaCut.Length > 1)
                            {
                                double cutarea = hv_AreaCut[0], cutrow = hv_RowCut[0], cutcol = hv_ColumnCut[0];
                                for (int i = 1; i < hv_AreaCut.Length; i++)
                                {
                                    if (cutarea < hv_AreaCut[i])
                                    {
                                        cutarea = hv_AreaCut[i]; cutrow = hv_RowCut[i]; cutcol = hv_ColumnCut[i];
                                        ho_SelectedRegionsMax.Dispose();
                                        HOperatorSet.SelectObj(ho_SelectedRegions1, out ho_SelectedRegionsMax, i + 1);
                                    }
                                }
                                hv_AreaCut = cutarea;
                                hv_RowCut = cutrow;
                                hv_ColumnCut = cutcol;
                                //ho_SelectedRegions1.Dispose();
                                //HOperatorSet.CopyImage(ho_SelectedRegionsMax, out ho_SelectedRegions1);
                            }
                            hWVision.DispObj(ho_SelectedRegionsMax);
                            HD.set_display_font(hWVision, 16, "mono", "true", "false");
                            HD.disp_message(hWVision, hv_AreaCut.ToString(), "", hv_RowCut, hv_ColumnCut, "blue", "false");
                        }
                        #endregion
                    }
                    else
                    {
                        hWVision.DispObj(ho_SelectedRegions1);
                        HD.set_display_font(hWVision, 16, "sans", "true", "false");
                        for (int i = 0; i < hv_AreaCut.Length; i++)
                        {
                            HD.disp_message(hWVision, Math.Round((double)hv_AreaCut[i]).ToString(), "", hv_RowCut[i], hv_ColumnCut[i], "blue", "false");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请重新调节剪口面积最小值/最大值！");
                }
            }
            catch
            {
                MessageBox.Show("请重新调节灰度阀值或剪口面积最小值/最大值！");
            }
        }
        private void tBAAreaMax_ValueChanged(object sender, EventArgs e)
        {
            cutamax = tBAAreaMax.Value;
            UDAAreaMax.Value = cutamax;
        }
        private void UDAAreaMax_ValueChanged(object sender, EventArgs e)
        {
            cutamax = (int)UDAAreaMax.Value;
            tBAAreaMax.Value = cutamax;
            CutAreaShow();
        }
        private void cBAddFCT_CheckedChanged(object sender, EventArgs e)
        {
            gBCutKuan.Enabled = ((cBAddFCT.Checked) ? true : false);
        }
        private void btnDrawAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AddRShow();
            }
            catch
            {
                MessageBox.Show("请重新调节参数！");
            }
        }
        void AddRShow()
        {
            if (readpara)
                return;
            if (hv_AreaCut.Length != 0)
            {
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions1, out hv_CRow2, out hv_CColumn2, out hv_CPhi, out hv_CLength1, out hv_CLength2);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_CRow2, hv_CColumn2, hv_CPhi, hv_CLength1, hv_CLength2);
                HOperatorSet.CountObj(ho_Rectangle, out hv_Number1);
                ho_EmptyRectangle.Dispose();
                HOperatorSet.SelectObj(ho_Rectangle, out ho_EmptyRectangle, 1);
                hv_emRow[0] = hv_CRow2.TupleSelect(0);
                hv_emColumn[0] = hv_CColumn2.TupleSelect(0);
                hv_emPhi[0] = hv_CPhi.TupleSelect(0);
                hv_emLength1[0] = hv_CLength1.TupleSelect(0);
                hv_emLength2[0] = hv_CLength2.TupleSelect(0);
                if (hv_Number1 > 1)
                {
                    HOperatorSet.GenEmptyObj(out ho_EmptyRectangle);
                    hv_i = 0;
                    HTuple end_val84 = hv_Number1 - 1, step_val84 = 1;
                    for (hv_j = 0; hv_j.Continue(end_val84, step_val84); hv_j = hv_j.TupleAdd(step_val84))
                    {
                        hv_val1 = hv_CLength2.TupleSelect(hv_j);
                        hv_val2 = hv_CLength1.TupleSelect(hv_j);
                        if ((int)(new HTuple(hv_val1.TupleLess(Widmax))) != 0 & (int)(new HTuple(hv_val1.TupleGreater(Widmin))) != 0 &
                            (int)(new HTuple(hv_val2.TupleLess(ALmax))) != 0 & (int)(new HTuple(hv_val2.TupleGreater(ALmin))) != 0)
                        {
                            hv_i = hv_i + 1;
                            ho_emSelectedRegion.Dispose();
                            HOperatorSet.SelectObj(ho_Rectangle, out ho_emSelectedRegion, hv_j + 1);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_EmptyRectangle, ho_emSelectedRegion, out ExpTmpOutVar_0);
                                ho_EmptyRectangle.Dispose();
                                ho_EmptyRectangle = ExpTmpOutVar_0;
                            }
                            hv_emRow[hv_i - 1] = hv_CRow2.TupleSelect(hv_j);
                            hv_emColumn[hv_i - 1] = hv_CColumn2.TupleSelect(hv_j);
                            hv_emPhi[hv_i - 1] = hv_CPhi.TupleSelect(hv_j);
                            hv_emLength1[hv_i - 1] = hv_CLength1.TupleSelect(hv_j);
                            hv_emLength2[hv_i - 1] = hv_CLength2.TupleSelect(hv_j);
                        }
                    }
                }
                HOperatorSet.AreaCenter(ho_EmptyRectangle, out hv_Areareall, out hv_Rowreall, out hv_Columnreall);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetDraw("margin");
                hWVision.DispObj(ho_EmptyRectangle);
                HD.set_display_font(hWVision, 16, "sans", "true", "false");
                for (int i = 0; i < hv_Areareall.Length; i++)
                {
                    HD.disp_message(hWVision, Math.Round((double)hv_emLength2[i], 3).ToString(), "", hv_Rowreall[i], hv_Columnreall[i], "blue", "false");
                    HD.disp_message(hWVision, Math.Round((double)hv_emLength1[i], 3).ToString(), "", hv_Rowreall[i] + 60, hv_Columnreall[i], "green", "false");
                }
            }
        }
        void AddRCal()
        {
            if (readpara)
                return;
            if (hv_AreaCut.Length != 0)
            {
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions1, out hv_CRow2, out hv_CColumn2, out hv_CPhi, out hv_CLength1, out hv_CLength2);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_CRow2, hv_CColumn2, hv_CPhi, hv_CLength1, hv_CLength2);
                HOperatorSet.CountObj(ho_Rectangle, out hv_Number1);
                ho_EmptyRectangle.Dispose();
                HOperatorSet.SelectObj(ho_Rectangle, out ho_EmptyRectangle, 1);
                hv_emRow[0] = hv_CRow2.TupleSelect(0);
                hv_emColumn[0] = hv_CColumn2.TupleSelect(0);
                hv_emPhi[0] = hv_CPhi.TupleSelect(0);
                hv_emLength1[0] = hv_CLength1.TupleSelect(0);
                hv_emLength2[0] = hv_CLength2.TupleSelect(0);
                if (hv_Number1 > 1)
                {
                    HOperatorSet.GenEmptyObj(out ho_EmptyRectangle);
                    hv_i = 0;
                    HTuple end_val84 = hv_Number1 - 1, step_val84 = 1;
                    for (hv_j = 0; hv_j.Continue(end_val84, step_val84); hv_j = hv_j.TupleAdd(step_val84))
                    {
                        hv_val1 = hv_CLength2.TupleSelect(hv_j);
                        hv_val2 = hv_CLength1.TupleSelect(hv_j);
                        if ((int)(new HTuple(hv_val1.TupleLess(Widmax))) != 0 & (int)(new HTuple(hv_val1.TupleGreater(Widmin))) != 0 &
                            (int)(new HTuple(hv_val2.TupleLess(ALmax))) != 0 & (int)(new HTuple(hv_val2.TupleGreater(ALmin))) != 0)
                        {
                            hv_i = hv_i + 1;
                            ho_emSelectedRegion.Dispose();
                            HOperatorSet.SelectObj(ho_Rectangle, out ho_emSelectedRegion, hv_j + 1);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_EmptyRectangle, ho_emSelectedRegion, out ExpTmpOutVar_0);
                                ho_EmptyRectangle.Dispose();
                                ho_EmptyRectangle = ExpTmpOutVar_0;
                            }
                            hv_emRow[hv_i - 1] = hv_CRow2.TupleSelect(hv_j);
                            hv_emColumn[hv_i - 1] = hv_CColumn2.TupleSelect(hv_j);
                            hv_emPhi[hv_i - 1] = hv_CPhi.TupleSelect(hv_j);
                            hv_emLength1[hv_i - 1] = hv_CLength1.TupleSelect(hv_j);
                            hv_emLength2[hv_i - 1] = hv_CLength2.TupleSelect(hv_j);
                        }
                    }
                }
                HOperatorSet.CountObj(ho_EmptyRectangle, out hv_Number2);
                hv_valueM = hv_emLength2.TupleSelect(0);
                hv_valueLM = hv_emLength1.TupleSelect(0);
                ho_SelectedRegion.Dispose();
                HOperatorSet.SelectObj(ho_EmptyRectangle, out ho_SelectedRegion, 1);
                if (hv_Number2 > 1)
                {
                    HTuple end_val101 = hv_Number2 - 1;
                    HTuple step_val101 = 1;
                    for (hv_j = 1; hv_j.Continue(end_val101, step_val101); hv_j = hv_j.TupleAdd(step_val101))
                    {
                        hv_va = hv_emLength2.TupleSelect(hv_j);
                        if ((int)(new HTuple(hv_va.TupleGreaterEqual(hv_valueM))) != 0)
                        {
                            hv_valueM = hv_va.Clone();
                            hv_valueLM = hv_emLength1.TupleSelect(hv_j);
                            ho_SelectedRegion.Dispose();
                            HOperatorSet.SelectObj(ho_EmptyRectangle, out ho_SelectedRegion, hv_j + 1);
                        }
                    }
                }
                HOperatorSet.AreaCenter(ho_SelectedRegion, out hv_Areare, out hv_Rowre, out hv_Columnre);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetDraw("margin");
                hWVision.DispObj(ho_SelectedRegion);
                if (hv_Rowre.Length == 0)
                    return;
                HD.disp_message(hWVision, Math.Round((double)hv_valueM, 3).ToString(), "", hv_Rowre, hv_Columnre, "blue", "false");
                HD.disp_message(hWVision, Math.Round((double)hv_valueLM, 3).ToString(), "", hv_Rowre + 60, hv_Columnre, "green", "false");
            }
        }
        private void tBAWMin_ValueChanged(object sender, EventArgs e)
        {
            Widmin = tBAWMin.Value;
            UDAWMin.Value = Widmin;
        }
        private void UDAWMin_ValueChanged(object sender, EventArgs e)
        {
            Widmin = (int)UDAWMin.Value;
            tBAWMin.Value = Widmin;
            AddRCal();
        }
        private void tBAWMax_ValueChanged(object sender, EventArgs e)
        {
            Widmax = tBAWMax.Value;
            UDAWMax.Value = Widmax;
        }
        private void UDAWMax_ValueChanged(object sender, EventArgs e)
        {
            Widmax = (int)UDAWMax.Value;
            tBAWMax.Value = Widmax;
            AddRCal();
        }
        private void tBALMin_ValueChanged(object sender, EventArgs e)
        {
            ALmin = tBALMin.Value;
            UDALMin.Value = ALmin;
        }
        private void UDALMin_ValueChanged(object sender, EventArgs e)
        {
            ALmin = (int)UDALMin.Value;
            tBALMin.Value = ALmin;
            AddRCal();
        }
        private void tBALMax_ValueChanged(object sender, EventArgs e)
        {
            ALmax = tBALMax.Value;
            UDALMax.Value = ALmax;
        }
        private void UDALMax_ValueChanged(object sender, EventArgs e)
        {
            ALmax = (int)UDALMax.Value;
            tBALMax.Value = ALmax;
            AddRCal();
        }
        #endregion
        #region Mode3
        HTuple hv_rowFCenter = new HTuple(), hv_colFCenter = new HTuple(), hv_MidCirRadius = new HTuple(), MidCirRadius = new HTuple(), hv_startPhi = new HTuple(), hv_endPhi = new HTuple();
        HTuple RegionWidth = new HTuple(), hv_rowF = new HTuple(), hv_colF = new HTuple(), hv_rowF1 = new HTuple(), hv_colF1 = new HTuple();
        HTuple hv_RowL = new HTuple(), hv_ColumnL = new HTuple(), hv_PhiL = new HTuple(), hv_LengthS1 = new HTuple(), hv_LengthS2 = new HTuple();
        HTuple hv_AreaMode = new HTuple(), hv_RowMode = new HTuple(), hv_ColMode = new HTuple(), hv_ModelID = new HTuple();
        HTuple hv_RowM = new HTuple(), hv_ColumnM = new HTuple(), hv_AngleM = new HTuple(), hv_Score = new HTuple();

        HObject ho_ContCircleOut = new HObject(), ho_ContCircleIn = new HObject(), ho_Contour1 = new HObject();
        HObject ho_ContoursUnion = new HObject(), ho_ContoursUnion1 = new HObject(), ho_RegionS = new HObject(), ho_RegionS1 = new HObject();
        HObject ho_RegionDifference = new HObject(), ho_RegionDifference_2 = new HObject(), ho_R = new HObject(), ho_RegionD = new HObject(), ho_ROI_0 = new HObject(), ho_Mode = new HObject();
        HObject ho_Template = new HObject();

        int StartAngle = 0, EndAngle = 180; int bin_Border = 128;
        private void btnDrawModeRing_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0)
                return;
            if (hv_RowCenter == 0.0)
                return;
            RadiusMax = (int)Math.Round((double)hv_CenterRadius);
            RadiusMin = RadiusMax - 20;
            ShowDonut();
        }
        private void tBAMinCut_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = tBAMinCut.Value;
            UDAMinCut.Value = RadiusMin;
        }
        private void UDAMinCut_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = (HTuple)UDAMinCut.Value;
            tBAMinCut.Value = RadiusMin;
            ShowDonut();
        }
        private void tBAMaxCut_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = tBAMaxCut.Value;
            UDAMaxCut.Value = RadiusMax;
        }
        private void UDAMaxCut_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = (HTuple)UDAMaxCut.Value;
            tBAMaxCut.Value = RadiusMax;
            ShowDonut();
        }
        private void FeatureChose_SelectedIndexChanged(object sender, EventArgs e)
        {
            gBtemplate.Enabled = ((FeatureChose.SelectedIndex == 0) ? true : false);
        }
        private void DrawFF_Click(object sender, EventArgs e)
        {
            btnCutCheck.Enabled = false;
            gBXYrange.Enabled = false;
            btnSaveCut.Enabled = false;
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            if (!(UDAMinCut.Value == 1 & UDAMaxCut.Value == 200))
            {
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                ho_CutRegion1.Dispose();
                HOperatorSet.GenCircle(out ho_CutRegion1, hv_RowCenter, hv_ColCenter, RadiusMin);
                ho_CutRegion2.Dispose();
                HOperatorSet.GenCircle(out ho_CutRegion2, hv_RowCenter, hv_ColCenter, RadiusMax);
                OTemp[SP_O] = ho_CutRegion2.CopyObj(1, -1);
                SP_O++;
                ho_CutRegion2.Dispose();
                HOperatorSet.Difference(OTemp[SP_O - 1], ho_CutRegion1, out ho_CutRegion2);
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                ho_ImageReduced1.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_CutRegion2, out ho_ImageReduced1);
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(ho_ImageReduced1, out ho_ImageSet);
            }
            if (FeatureChose.SelectedIndex == 0)//圆形
            {
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("red");
                HOperatorSet.DrawCircle(hWVision, out hv_rowFCenter, out hv_colFCenter, out hv_MidCirRadius);
                double mid = hv_MidCirRadius;
                MidCirRadius = (int)mid;
                RegionWidth = tBTRwidth.Value;
                ShowSector();
            }
            if (FeatureChose.SelectedIndex == 1)//方形
            {
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("red");
                HOperatorSet.DrawRectangle2(hWVision, out hv_RowL, out hv_ColumnL, out hv_PhiL, out hv_LengthS1, out hv_LengthS2);
                ho_ROI_0.Dispose();
                HOperatorSet.GenRectangle2(out ho_ROI_0, hv_RowL, hv_ColumnL, hv_PhiL, hv_LengthS1, hv_LengthS2);
                ho_RegionDifference.Dispose();
                HOperatorSet.CopyObj(ho_ROI_0, out ho_RegionDifference, 1, 1);
                HOperatorSet.GenContourRegionXld(ho_RegionDifference, out ho_RegionD, "border");
                hWVision.DispObj(ho_RegionD);
            }
        }
        void ShowSector()
        {
            if (readpara)
                return;
            hv_startPhi = (StartAngle * 3.14159) / 180;
            hv_endPhi = (EndAngle * 3.14159) / 180;
            //创建外扇形
            ho_ContCircleOut.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircleOut, hv_rowFCenter, hv_colFCenter,
                 MidCirRadius + RegionWidth / 2, hv_startPhi, hv_endPhi, "positive", 1);
            hv_rowF = new HTuple();
            hv_rowF[0] = hv_rowFCenter - ((MidCirRadius + RegionWidth / 2) * (hv_startPhi.TupleSin()));
            hv_rowF[1] = hv_rowFCenter;
            hv_rowF[2] = hv_rowFCenter - ((MidCirRadius + RegionWidth / 2) * (hv_endPhi.TupleSin()));
            hv_colF = new HTuple();
            hv_colF[0] = hv_colFCenter + ((MidCirRadius + RegionWidth / 2) * (hv_startPhi.TupleCos()));
            hv_colF[1] = hv_colFCenter;
            hv_colF[2] = hv_colFCenter + ((MidCirRadius + RegionWidth / 2) * (hv_endPhi.TupleCos()));
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_rowF, hv_colF);
            if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
            {
                ho_ContoursUnion.Dispose();
                HOperatorSet.Union2ClosedContoursXld(ho_ContCircleOut, ho_Contour, out ho_ContoursUnion);
            }
            else if ((EndAngle - StartAngle) < 360)
            {
                ho_ContoursUnion.Dispose();
                HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleOut, ho_Contour, out ho_ContoursUnion);
            }
            ho_RegionS.Dispose();
            HOperatorSet.GenRegionContourXld(ho_ContoursUnion, out ho_RegionS, "filled");

            //创建内扇形
            ho_ContCircleIn.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircleIn, hv_rowFCenter, hv_colFCenter,
                (MidCirRadius - RegionWidth / 2), hv_startPhi, hv_endPhi, "positive", 1);
            hv_rowF1 = new HTuple();
            hv_rowF1[0] = hv_rowFCenter - ((MidCirRadius - RegionWidth / 2) * (hv_startPhi.TupleSin()));
            hv_rowF1[1] = hv_rowFCenter;
            hv_rowF1[2] = hv_rowFCenter - ((MidCirRadius - RegionWidth / 2) * (hv_endPhi.TupleSin()));
            hv_colF1 = new HTuple();
            hv_colF1[0] = hv_colFCenter + ((MidCirRadius - RegionWidth / 2) * (hv_startPhi.TupleCos()));
            hv_colF1[1] = hv_colFCenter;
            hv_colF1[2] = hv_colFCenter + ((MidCirRadius - RegionWidth / 2) * (hv_endPhi.TupleCos()));
            ho_Contour1.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour1, hv_rowF1, hv_colF1);
            if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
            {
                ho_ContoursUnion1.Dispose();
                HOperatorSet.Union2ClosedContoursXld(ho_ContCircleIn, ho_Contour1, out ho_ContoursUnion1);
            }
            else if ((EndAngle - StartAngle) < 360)
            {
                ho_ContoursUnion1.Dispose();
                HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleIn, ho_Contour1, out ho_ContoursUnion1);
            }
            ho_RegionS1.Dispose();
            HOperatorSet.GenRegionContourXld(ho_ContoursUnion1, out ho_RegionS1, "filled");
            //生产扇形环
            if ((EndAngle - StartAngle) < 360)
            {
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_RegionS, ho_RegionS1, out ho_RegionDifference);
            }
            else
            {
                ho_RegionS.Dispose();
                HOperatorSet.GenCircle(out ho_RegionS, hv_rowFCenter, hv_colFCenter, MidCirRadius + RegionWidth / 2);
                ho_RegionS1.Dispose();
                HOperatorSet.GenCircle(out ho_RegionS1, hv_rowFCenter, hv_colFCenter, MidCirRadius - RegionWidth / 2);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_RegionS, ho_RegionS1, out ho_RegionDifference);
                ho_R.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionS1, out ho_R, "border");
            }
            HOperatorSet.GenContourRegionXld(ho_RegionDifference, out ho_RegionD, "border");
            //显示最终结果
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageTest);
            if ((EndAngle - StartAngle) >= 360)
                hWVision.DispObj(ho_R);
            hWVision.DispObj(ho_RegionD);
        }
        private void tBMidCirR_ValueChanged(object sender, EventArgs e)
        {
            MidCirRadius = tBMidCirR.Value;
            UDMidCirR.Value = MidCirRadius;
        }
        private void UDMidCirR_ValueChanged(object sender, EventArgs e)
        {
            if (RegionWidth / 2 > MidCirRadius)
            {
                MessageBox.Show("区域宽度不能大于区域直径！");
                return;
            }
            MidCirRadius = (int)UDMidCirR.Value;
            tBMidCirR.Value = MidCirRadius;
            ShowSector();
        }
        private void tBTRwidth_ValueChanged(object sender, EventArgs e)
        {
            RegionWidth = tBTRwidth.Value;
            UDTRwidth.Value = RegionWidth;
        }
        private void UDTRwidth_ValueChanged(object sender, EventArgs e)
        {
            if (RegionWidth / 2 > MidCirRadius)
            {
                MessageBox.Show("区域宽度不能大于区域直径！");
                return;
            }
            RegionWidth = (int)UDTRwidth.Value;
            tBTRwidth.Value = RegionWidth;
            ShowSector();
        }
        private void tBStartA_ValueChanged(object sender, EventArgs e)
        {
            StartAngle = tBStartA.Value;
            UDStartA.Value = StartAngle;
        }
        private void UDStartA_ValueChanged(object sender, EventArgs e)
        {
            StartAngle = (int)UDStartA.Value;
            tBStartA.Value = StartAngle;
            ShowSector();
        }
        private void tBEndA_ValueChanged(object sender, EventArgs e)
        {
            EndAngle = (int)tBEndA.Value;
            UDEndA.Value = EndAngle;
        }
        private void UDEndA_ValueChanged(object sender, EventArgs e)
        {
            EndAngle = (int)UDEndA.Value;
            tBEndA.Value = EndAngle;
            ShowSector();
        }
        private void tBModeTh_ValueChanged(object sender, EventArgs e)
        {
            bin_Border = tBModeTh.Value;
            UDModeTh.Value = bin_Border;
            binModeShow();
        }
        private void UDModeTh_ValueChanged(object sender, EventArgs e)
        {
            bin_Border = (int)UDModeTh.Value;
            tBModeTh.Value = bin_Border;
        }
        void binModeShow()
        {
            if (readpara)
                return;
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            ho_Mode.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_Mode);
            try
            {
                ho_Border.Dispose();
                HOperatorSet.ThresholdSubPix(ho_Mode, out ho_Border, bin_Border);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetColor("red");
                hWVision.DispObj(ho_Border);
            }
            catch (Exception)
            {
                //
            }
        }
        private void btnModeA_Click(object sender, EventArgs e)
        {
            MCircleShow();
        }
        void MCircleShow()
        {
            if (readpara)
                return;
            try
            {
                if (hv_RowCenter.D != 0.0)
                {
                    double SubRow, SubCol;
                    int i_image = int.Parse(SetNum) - 1;
                    ho_ImageSet.Dispose();
                    HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                    HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                    HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                    //HOperatorSet.SetPart(HWindowID, 0, 0, m_ImageHeight, m_ImageWidth);
                    if (!(UDAMinCut.Value == 1 & UDAMaxCut.Value == 200))
                    {
                        HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                        HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                        ho_CutRegion1.Dispose();
                        HOperatorSet.GenCircle(out ho_CutRegion1, hv_RowCenter, hv_ColCenter, RadiusMin);
                        ho_CutRegion2.Dispose();
                        HOperatorSet.GenCircle(out ho_CutRegion2, hv_RowCenter, hv_ColCenter, RadiusMax);
                        OTemp[SP_O] = ho_CutRegion2.CopyObj(1, -1);
                        SP_O++;
                        ho_CutRegion2.Dispose();
                        HOperatorSet.Difference(OTemp[SP_O - 1], ho_CutRegion1, out ho_CutRegion2);
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                        ho_ImageReduced1.Dispose();
                        HOperatorSet.ReduceDomain(ho_ImageSet, ho_CutRegion2, out ho_ImageReduced1);
                        ho_ImageSet.Dispose();
                        HOperatorSet.CopyImage(ho_ImageReduced1, out ho_ImageSet);
                    }
                    ho_Mode.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_Mode);
                    HOperatorSet.AreaCenter(ho_Mode, out hv_AreaMode, out hv_RowMode, out hv_ColMode);
                    #region //ROI区域
                    SubRow = hv_RowMode - hv_RowCenter;
                    SubCol = hv_ColMode - hv_ColCenter;
                    hv_Deg2 = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                    if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                        hv_Deg2 = ((SubCol > 0) ? 360 - hv_Deg2 : 180 + hv_Deg2);
                    else
                        hv_Deg2 = ((SubCol > 0) ? hv_Deg2 : 180 - hv_Deg2);
                    hWVision.DispObj(halcon.Image[i_image]);
                    hWVision.SetColor("green");
                    //HWindowID.DispObj(ho_MCircle);
                    hWVision.SetColor("blue");
                    hWVision.DispLine(hv_RowMode, hv_ColMode, hv_RowCenter, hv_ColCenter);
                    hWVision.SetColor("red");
                    //HWindowID.DispObj(ho_MCircle0);
                    hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 5);
                    if (halcon.IsCrossDraw)
                    {
                        hWVision.SetLineWidth(1);
                        hWVision.DispCross(row, col, width, 0);
                    }
                    HD.disp_message(hWVision, "T:" + string.Format("{0:f3}°", (double)hv_Deg2), "", 450, 150, "green", "false");
                    #endregion
                    ho_ImageSet.Dispose();
                    HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                    if (SetNum == "8")
                        hv_AddDeg = hv_Deg2;
                }
                else
                {
                    MessageBox.Show("请先找到圆心！");
                }
            }
            catch
            { }
        }
        private void SetTemplate_Click(object sender, EventArgs e)
        {
            int i_image = int.Parse(SetNum) - 1;
            HOperatorSet.GenEmptyObj(out ho_Template);
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_Template);
            HOperatorSet.CreateShapeModel(ho_Template, "auto", 0, (new HTuple(360)).TupleRad()
                , "auto", "auto", "use_polarity", "auto", "auto", out hv_ModelID);
            if (SetNum == "1" || SetNum == "3")
            {
                HOperatorSet.FindShapeModel(ho_ImageSet, hv_ModelID, 0, (new HTuple(360)).TupleRad(), 0.3, 1, 0.5,
                      "least_squares", 0, 0.5, out hv_RowM, out hv_ColumnM, out hv_AngleM, out hv_Score);
            }
            else
            {
                HOperatorSet.FindShapeModel(ho_ImageSet, hv_ModelID, 0, (new HTuple(360)).TupleRad(), 0.3, 1, 0.5,
                      "least_squares", 0, 0.9, out hv_RowM, out hv_ColumnM, out hv_AngleM, out hv_Score);
            }
            if (cBGlueQ.Checked)
            {
                hv_RowCut_Befort = hv_RowM;
                hv_ColumnCut_Befort = hv_ColumnM;
            }
            if (cBGlueH.Checked)
            {
                hv_RowCut_After = hv_RowM;
                hv_ColumnCut_After = hv_ColumnM;
            }
            hWVision.ClearWindow();
            hWVision.DispObj(halcon.Image[i_image]);
            HD.dev_display_shape_matching_results(hv_ModelID, "red", hv_RowM, hv_ColumnM, hv_AngleM, 1, 1, 0);
            HD.set_display_font(hWVision, 18, "sans", "true", "false");
            HD.disp_message(hWVision, "Score:" + hv_Score * 100, "", 250, 250, "green", "false");
        }
        int testnum = 0; HTuple[] Scores = new HTuple[10];
        private void btnTest_Click(object sender, EventArgs e)
        {
            testnum++;
            if (testnum <= 10 & hv_ModelID != null)
            {
                if (SetNum == "1" || SetNum == "3")
                {
                    HOperatorSet.FindShapeModel(ho_ImageSet, hv_ModelID, 0, (new HTuple(360)).TupleRad(), 0.3, 1, 0.5,
                          "least_squares", 0, 0.5, out hv_RowM, out hv_ColumnM, out hv_AngleM, out hv_Score);
                }
                else
                {
                    HOperatorSet.FindShapeModel(ho_ImageSet, hv_ModelID, 0, (new HTuple(360)).TupleRad(), 0.3, 1, 0.5,
                          "least_squares", 0, 0.9, out hv_RowM, out hv_ColumnM, out hv_AngleM, out hv_Score);
                }
                HD.dev_display_shape_matching_results(hv_ModelID, "red", hv_RowM, hv_ColumnM, hv_AngleM, 1, 1, 0);
                if (hv_Score.Length == 0)
                    hv_Score = 0.0;
                Scores[testnum - 1] = hv_Score;
                if (testnum > 1)
                {
                    if (hv_Score != 0.0)
                    {
                        if (Scores[testnum - 1] > Scores[testnum - 2])
                            Scores[testnum - 1] = Scores[testnum - 2];
                    }
                    else
                        Scores[testnum - 1] = Scores[testnum - 2];
                }
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                try
                {
                    ho_ROI_0.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_ROI_0, hv_RowM, hv_ColumnM, hv_AngleM, 60, 60);
                    hWVision.DispObj(ho_ROI_0);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.GenRectangle2(out ho_RegionDifference, hv_RowM, hv_ColumnM, hv_AngleM, 60, 60);
                    MCircleShow();
                }
                catch
                {
                    //
                }
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "Score:" + hv_Score * 100, "", 150, 250, "green", "false");
                HD.disp_message(hWVision, "Number:" + testnum, "", 250, 250, "blue", "false");
                HD.disp_message(hWVision, "MinScore:" + Scores[testnum - 1] * 100, "", 350, 250, "blue", "false");
            }
            else
            {
                DialogResult dr = MessageBox.Show("是否重新测试?", "", MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Cancel)
                {
                    HD.disp_message(hWVision, "MinScore:" + Scores[testnum - 1] * 10, "", 450, 250, "blue", "false");
                    return;
                }
                testnum = 0;
            }
        }
        private void SaveTemplate_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            string astatus = ((cBCut.Checked) ? "true" : "false");
            iniFile.Write(CCDNAME, "AngleStatus", astatus, FrmMain.propath);
            iniFile.Write(CCDNAME, "AngleMode", cBAMode.Value.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "AngleThreshold3", UDModeTh.Value.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "SetScore", AngleMScore.Value.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "TemplateShape", FeatureChose.SelectedIndex.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "AngleModeRmin", (tBAMinCut.Value).ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "AngleModeRmax", (tBAMaxCut.Value).ToString(), FrmMain.propath);
            string Tfilepath = "";
            #region Circle
            if (FeatureChose.SelectedIndex == 0)
                Tfilepath = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME + "\\CircleTemplate";
            #endregion
            #region Square
            if (FeatureChose.SelectedIndex == 1)
                Tfilepath = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME + "\\SquareTemplate";
            #endregion
            if (hv_ModelID.Length == 0)
            {
                MessageBox.Show("1,模板已保存，请勿重复保存；\r\n或\r\n2,未创建模板，请创建后再做保存！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Directory.Exists(Tfilepath)) //创建ini文档H
                Directory.CreateDirectory(Tfilepath);
            DirectoryInfo theFolder = new DirectoryInfo(Tfilepath);
            FileInfo[] TfileInfo = theFolder.GetFiles();
            string sDirectoryName = "", sNewDirectoryName = "";
            if (TfileInfo.Length != 0)
            {
                for (int i = TfileInfo.Length; i > 0; i--)
                {
                    sDirectoryName = Tfilepath + "\\" + i.ToString() + ".shm";
                    sNewDirectoryName = Tfilepath + "\\" + (1 + i).ToString() + ".shm";
                    if (!parent.ModifyFilename(sDirectoryName, sNewDirectoryName))
                    {
                        try
                        {
                            if (File.Exists(sDirectoryName))
                            {
                                FileInfo fi = new FileInfo(sDirectoryName);
                                fi.MoveTo(sNewDirectoryName);
                            }
                        }
                        catch //(Exception er)
                        {
                            //MessageBox.Show(er.ToString());
                        }
                    }
                }
                if (TfileInfo.Length > 10)
                {
                    for (int i = TfileInfo.Length + 1; i > 10; i--)
                    {
                        File.Delete(Tfilepath + "\\" + i.ToString() + ".shm");
                    }
                }
            }
            HOperatorSet.WriteShapeModel(hv_ModelID, Tfilepath + "\\1.shm");

            MessageBox.Show("模板保存成功！");
            hv_ModelID = new HTuple();
        }
        #endregion
        #region Mode4
        HTuple hv_AreaCut4 = new HTuple(), hv_RowCut4 = new HTuple(), hv_ColumnCut4 = new HTuple(), hv_DistanceCut4 = null;
        HTuple hv_NumberCut4 = null, hv_iCut4 = null, hv_dismaxCut4 = null, hv_kCut4 = null;
        private void btnDrawARing4_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0)
                return;
            if (hv_RowCenter == 0.0)
                return;
            RadiusMax = (int)Math.Round((double)hv_CenterRadius) - 5;
            tBAMaxRa4.Value = RadiusMax;
            RadiusMin = RadiusMax - 20;
            ShowDonut();
        }
        private void tBAMinRa4_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = tBAMinRa4.Value;
            UDAMinRa4.Value = RadiusMin;
        }
        private void UDAMinRa4_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = (HTuple)UDAMinRa4.Value;
            tBAMinRa4.Value = RadiusMin;
            ShowDonut();
        }
        private void tBAMaxRa4_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = tBAMaxRa4.Value;
            UDAMaxRa4.Value = RadiusMax;
        }
        private void UDAMaxRa4_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = (HTuple)UDAMaxRa4.Value;
            tBAMaxRa4.Value = RadiusMax;
            ShowDonut();
        }
        private void tBAB2W4_ValueChanged(object sender, EventArgs e)
        {
            binBW = tBAB2W4.Value;
            UDAB2W4.Value = binBW;
        }
        private void UDAB2W4_ValueChanged(object sender, EventArgs e)
        {
            binBW = (int)UDAB2W4.Value;
            tBAB2W4.Value = binBW;
            BinShow();
        }
        private void tBAW2B4_ValueChanged(object sender, EventArgs e)
        {
            binWB = tBAW2B4.Value;
            UDAW2B4.Value = binWB;
        }
        private void UDAW2B4_ValueChanged(object sender, EventArgs e)
        {
            binWB = (int)UDAW2B4.Value;
            tBAW2B4.Value = binWB;
            BinShow();
        }
        private void tBAAreaMin4_ValueChanged(object sender, EventArgs e)
        {
            cutamin = tBAAreaMin4.Value;
            UDAAreaMin4.Value = cutamin;
        }
        private void UDAAreaMin4_ValueChanged(object sender, EventArgs e)
        {
            cutamin = (int)UDAAreaMin4.Value;
            tBAAreaMin4.Value = cutamin;
            CutAreaShow4();
        }
        private void tBAAreaMax4_ValueChanged(object sender, EventArgs e)
        {
            cutamax = tBAAreaMax4.Value;
            UDAAreaMax4.Value = cutamax;
        }
        private void UDAAreaMax4_ValueChanged(object sender, EventArgs e)
        {
            cutamax = (int)UDAAreaMax4.Value;
            tBAAreaMax4.Value = cutamax;
            CutAreaShow4();
        }
        private void CutAreaShow4()
        {
            try
            {
                if (readpara)
                    return;
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                if (cutamin < cutamax)
                {
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, "area", "and", cutamin, cutamax);
                    HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_AreaCut4, out hv_RowCut4, out hv_ColumnCut4);
                    HOperatorSet.CountObj(ho_SelectedRegions1, out hv_NumberCut4);
                    hWVision.ClearWindow();
                    hWVision.SetDraw("fill");
                    hWVision.DispObj(ho_ImageSet);
                    hWVision.DispObj(ho_SelectedRegions1);
                    HD.set_display_font(hWVision, 16, "sans", "true", "false");
                    for (int i_c = 0; i_c < (int)hv_NumberCut4; i_c++)
                    {
                        HD.disp_message(hWVision, hv_AreaCut4[i_c].D.ToString(), "", hv_RowCut4[i_c], hv_ColumnCut4[i_c], "green", "false");
                    }
                }
                else
                {
                    MessageBox.Show("请重新调节剪口面积最小值/最大值！");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        #endregion
        #region Mode5
        private void btnARegion_Click(object sender, EventArgs e)
        {
            LoadImg.BackColor = Color.WhiteSmoke;
            if (SetNum == "8")
            {
                if (hv_ColCenter.Length == 0)
                {
                    MessageBox.Show("请先找到圆心！");
                }
            }
            else
            {
                if (hv_ColCenter.Length == 0)
                {
                    MessageBox.Show("请先找到圆心！");
                }
            }
            LoadImg.BackColor = Color.WhiteSmoke;
            hv_Deg2 = 90;
            hv_FdegPlus = 45;
            ShowGlueFang();
        }
        private void tBARegionDis_ValueChanged(object sender, EventArgs e)
        {
            double gluedistance = tBARegionDis.Value;
            UDARegionDis.Value = (int)gluedistance;
            hv_Deg2 = 90;
            ShowGlueFang();
        }
        private void UDARegionDis_ValueChanged(object sender, EventArgs e)
        {
            double gluedistance = (HTuple)UDARegionDis.Value;
            tBARegionDis.Value = (int)gluedistance;
        }
        private void tBARegionlen1_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = tBARegionlen1.Value;
            UDARegionlen1.Value = (int)gluelen;
            hv_Deg2 = 90;
            ShowGlueFang();
        }
        private void UDARegionlen1_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = (HTuple)UDARegionlen1.Value;
            tBARegionlen1.Value = (int)gluelen;
        }
        private void tBARegionlen2_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = tBARegionlen2.Value;
            UDARegionlen2.Value = (int)gluelen;
            hv_Deg2 = 90;
            ShowGlueFang();
        }
        private void UDARegionlen2_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = (HTuple)UDARegionlen2.Value;
            tBARegionlen2.Value = (int)gluelen;
        }
        private void tBARegionGray_ValueChanged(object sender, EventArgs e)
        {
            double gray = tBARegionGray.Value;
            UDARegionGray.Value = (int)gray;
            hv_Deg2 = 90;
            ShowGlueFang();
        }
        private void UDARegionGray_ValueChanged(object sender, EventArgs e)
        {
            double gray = (HTuple)UDARegionGray.Value;
            tBARegionGray.Value = (int)gray;
        }
        //像素最小面积值（隐藏）
        private void lblAngelShow_DoubleClick(object sender, EventArgs e)
        {
            gBAngleShow.Show();
            string CCDNAME = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            string pam = iniFile.Read(CCDNAME, "PAreaMin", FrmMain.propath);
            if (pam == "")
                pam = "200";
            txtPAreaMin.Text = pam;
        }
        private void lblAngelHide_DoubleClick(object sender, EventArgs e)
        {
            gBAngleShow.Hide();
        }
        private void btnAngleShow_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            iniFile.Write(CCDNAME, "PAreaMin", txtPAreaMin.Text, FrmMain.propath);

        }
        #endregion
        private void btnCutCheck_Click(object sender, EventArgs e)
        {
            try
            {
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
                hv_RowCut = new HTuple();
                hv_ColumnCut = new HTuple();
                if (cBAMode.Value == 1)
                {
                    #region  Mode1
                    try
                    {
                        if (hv_AngleDeg < 0)
                            hv_AngleDeg = hv_AngleDeg + 360;
                        hv_Deg2 = hv_AngleDeg;
                        hWVision.ClearWindow();
                        hWVision.DispObj(ho_ImageSet);
                        hWVision.SetColor("red");
                        hWVision.DispCross(row, col, width, 0);
                        hWVision.SetColor("green");
                        hWVision.DispObj(ho_DLRLines);
                        HOperatorSet.SetTposition(hWVision, 350, 24);
                        HOperatorSet.WriteString(hWVision, (("Degree = ") + hv_AngleDeg) + "°");
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.ToString());
                    }
                    #endregion
                }
                if (cBAMode.Value == 2)
                {
                    #region  Mode2
                    if (cBAddFCT.Checked)
                    {
                        if (hv_Areare.Length == 0)
                            AddRCal();
                        hv_AreaCut = hv_Areare;
                        hv_RowCut = hv_Rowre;
                        hv_ColumnCut = hv_Columnre;

                    }
                    #region
                    double SubRow, SubCol, angle;
                    if (hv_AreaCut.Length != 0)
                    {
                        hWVision.ClearWindow();
                        hWVision.SetDraw("fill");
                        hWVision.DispObj(ho_ImageSet);
                        hWVision.SetColor("blue");
                        if (!cBAddFCT.Checked)
                            hWVision.DispObj(ho_SelectedRegions1);
                        else
                            hWVision.DispObj(ho_SelectedRegion);
                        hWVision.DispLine(hv_RowCut, hv_ColumnCut, hv_RowCenter, hv_ColCenter);
                        hWVision.SetColor("green");
                        hWVision.SetLineWidth(1);
                        hWVision.DispCross(row, col, width, 0);
                        SubRow = hv_RowCut - hv_RowCenter;
                        SubCol = hv_ColumnCut - hv_ColCenter;
                        angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                        if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                            angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                        else
                            angle = ((SubCol > 0) ? angle : 180 - angle);
                        hv_Deg2 = angle;
                        //hv_Deg2 = ((hv_Deg2 >= 180) ? hv_Deg2 - 180 : hv_Deg2 + 180);
                        HD.set_display_font(hWVision, 18, "sans", "true", "false");
                        HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm + "mm", "", 150, 150, "green", "false");
                        HD.disp_message(hWVision, "Y_bias:" + (hv_RowCenter - row) * ypm + "mm", "", 300, 150, "green", "false");
                        HD.disp_message(hWVision, "T:" + string.Format("{0:f3}°", (double)hv_Deg2), "", 450, 150, "green", "false");

                    }
                    else
                    {
                        if (!cBAddFCT.Checked)
                        {
                            BinShow();
                            CutAreaShow();
                            if (hv_RowCut != "")
                            {
                                if (hv_RowCut.Length > 1)
                                {
                                    hv_RowCut = hv_RowCut[1];
                                    hv_ColumnCut = hv_ColumnCut[1];
                                }
                                hWVision.ClearWindow();
                                hWVision.SetDraw("fill");
                                hWVision.DispObj(ho_ImageSet);
                                hWVision.SetColor("blue");
                                hWVision.DispObj(ho_SelectedRegions1);
                                hWVision.DispLine(hv_RowCut, hv_ColumnCut, hv_RowCenter, hv_ColCenter);
                                hWVision.SetColor("green");
                                hWVision.SetLineWidth(1);
                                hWVision.DispCross(row, col, width, 0);
                                SubRow = hv_RowCut - hv_RowCenter;
                                SubCol = hv_ColumnCut - hv_ColCenter;
                                angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                                if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                                    angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                                else
                                    angle = ((SubCol > 0) ? angle : 180 - angle);
                                hv_Deg2 = angle;
                                hv_Deg2 = ((hv_Deg2 >= 180) ? hv_Deg2 - 180 : hv_Deg2 + 180);
                                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm, "", 150, 150, "green", "false");
                                HD.disp_message(hWVision, "Y_bias:" + (hv_RowCenter - row) * ypm, "", 300, 150, "green", "false");
                                HD.disp_message(hWVision, "T:" + string.Format("{0:f3}°", angle), "", 450, 150, "green", "false");

                            }
                        }
                    }
                    #endregion
                    #endregion
                }
                if (cBAMode.Value == 4)
                {
                    #region Mode4
                    double SubRow, SubCol, angle;
                    HOperatorSet.CountObj(ho_SelectedRegions1, out hv_NumberCut4);
                    if (hv_NumberCut4.D == 5)
                    {
                        HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_AreaCut4, out hv_RowCut4, out hv_ColumnCut4);
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_RowCut4, hv_ColumnCut4, 50, 0.785398);
                        HTuple hv_dis = new HTuple();
                        hv_dis[0] = 0;
                        for (hv_iCut4 = 1; (int)hv_iCut4 <= 4; hv_iCut4 = (int)hv_iCut4 + 1)
                        {
                            HOperatorSet.DistancePp(hv_RowCut4[0], hv_ColumnCut4[0], hv_RowCut4[hv_iCut4], hv_ColumnCut4[hv_iCut4], out hv_DistanceCut4);
                            if (hv_dis == null)
                                hv_dis = new HTuple();
                            hv_dis[0] = hv_dis[0] + hv_DistanceCut4;
                        }

                        hv_dis[1] = 0;
                        HOperatorSet.DistancePp(hv_RowCut4[0], hv_ColumnCut4[0], hv_RowCut4[1], hv_ColumnCut4[1], out hv_DistanceCut4);
                        hv_dis[1] = hv_dis[1] + hv_DistanceCut4;
                        for (hv_iCut4 = 2; (int)hv_iCut4 <= 4; hv_iCut4 = (int)hv_iCut4 + 1)
                        {
                            HOperatorSet.DistancePp(hv_RowCut4[1], hv_ColumnCut4[1], hv_RowCut4[hv_iCut4], hv_ColumnCut4[hv_iCut4], out hv_DistanceCut4);
                            hv_dis[1] = hv_dis[1] + hv_DistanceCut4;
                        }

                        hv_dis[2] = 0;
                        for (hv_iCut4 = 0; (int)hv_iCut4 <= 1; hv_iCut4 = (int)hv_iCut4 + 1)
                        {
                            HOperatorSet.DistancePp(hv_RowCut4[2], hv_ColumnCut4[2], hv_RowCut4[hv_iCut4], hv_ColumnCut4[hv_iCut4], out hv_DistanceCut4);
                            hv_dis[2] = hv_dis[2] + hv_DistanceCut4;
                        }
                        for (hv_iCut4 = 3; (int)hv_iCut4 <= 4; hv_iCut4 = (int)hv_iCut4 + 1)
                        {
                            HOperatorSet.DistancePp(hv_RowCut4[2], hv_ColumnCut4[2], hv_RowCut4[hv_iCut4], hv_ColumnCut4[hv_iCut4], out hv_DistanceCut4);
                            hv_dis[2] = hv_dis[2] + hv_DistanceCut4;
                        }

                        hv_dis[3] = 0;
                        for (hv_iCut4 = 0; (int)hv_iCut4 <= 2; hv_iCut4 = (int)hv_iCut4 + 1)
                        {
                            HOperatorSet.DistancePp(hv_RowCut4[3], hv_ColumnCut4[3], hv_RowCut4[hv_iCut4], hv_ColumnCut4[hv_iCut4], out hv_DistanceCut4);
                            hv_dis[3] = hv_dis[3] + hv_DistanceCut4;
                        }
                        HOperatorSet.DistancePp(hv_RowCut4[3], hv_ColumnCut4[3], hv_RowCut4[4], hv_ColumnCut4[4], out hv_DistanceCut4);
                        hv_dis[3] = hv_dis[3] + hv_DistanceCut4;

                        hv_dis[4] = 0;
                        for (hv_iCut4 = 0; (int)hv_iCut4 <= 3; hv_iCut4 = (int)hv_iCut4 + 1)
                        {
                            HOperatorSet.DistancePp(hv_RowCut4[4], hv_ColumnCut4[4], hv_RowCut4[hv_iCut4], hv_ColumnCut4[hv_iCut4], out hv_DistanceCut4);
                            hv_dis[4] = hv_dis[4] + hv_DistanceCut4;
                        }

                        hv_dismaxCut4 = hv_dis[0];
                        hv_kCut4 = 0;
                        for (hv_iCut4 = 1; (int)hv_iCut4 <= 4; hv_iCut4 = (int)hv_iCut4 + 1)
                        {
                            if ((int)(new HTuple(((hv_dis.TupleSelect(hv_iCut4))).TupleLess(hv_dismaxCut4))) != 0)
                            {
                                hv_dismaxCut4 = hv_dis.TupleSelect(hv_iCut4);
                                hv_kCut4 = hv_iCut4.Clone();
                            }
                        }
                        hWVision.ClearWindow();
                        hWVision.SetDraw("fill");
                        hWVision.DispObj(ho_ImageSet);
                        SubRow = hv_RowCut4[hv_kCut4] - hv_RowCenter;
                        SubCol = hv_ColumnCut4[hv_kCut4] - hv_ColCenter;
                        angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                        if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                            angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                        else
                            angle = ((SubCol > 0) ? angle : 180 - angle);
                        hv_Deg2 = angle;
                        hWVision.SetColor("blue");
                        hWVision.SetLineWidth(4);
                        hWVision.DispObj(ho_Cross);
                        hWVision.SetLineWidth(1);
                        hWVision.DispLine(hv_RowCut4[hv_kCut4], hv_ColumnCut4[hv_kCut4], hv_RowCenter, hv_ColCenter);
                        HD.set_display_font(hWVision, 18, "sans", "true", "false");
                        HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm + "mm", "", 150, 150, "green", "false");
                        HD.disp_message(hWVision, "Y_bias:" + (hv_RowCenter - row) * ypm + "mm", "", 300, 150, "green", "false");
                        HD.disp_message(hWVision, "T:" + string.Format("{0:f3}°", (double)hv_Deg2), "", 450, 150, "green", "false");
                    }
                    #endregion
                }
                if (cBAMode.Value == 5)
                {
                    #region Mode5
                    HObject ho_ARegion = new HObject();
                    HTuple[] hv_Aarea = new HTuple[4] { 0, 0, 0, 0 }, hv_Arow = new HTuple[4], hv_Acol = new HTuple[4];
                    hv_Deg2 = 90;
                    ShowGlueFang();
                    HOperatorSet.AreaCenter(ho_g1Reduced, out hv_Aarea[0], out hv_Arow[0], out hv_Acol[0]);
                    HOperatorSet.AreaCenter(ho_g2Reduced, out hv_Aarea[1], out hv_Arow[1], out hv_Acol[1]);
                    HOperatorSet.AreaCenter(ho_g3Reduced, out hv_Aarea[2], out hv_Arow[2], out hv_Acol[2]);
                    HOperatorSet.AreaCenter(ho_g4Reduced, out hv_Aarea[3], out hv_Arow[3], out hv_Acol[3]);
                    if (hv_Arow[0].Length == 0)
                    {
                        hv_Aarea[0] = 0;
                        hv_Arow[0] = 0;
                        hv_Acol[0] = 0;
                    }
                    if (hv_Arow[1].Length == 0)
                    {
                        hv_Aarea[1] = 0;
                        hv_Arow[1] = 0;
                        hv_Acol[1] = 0;
                    }
                    if (hv_Arow[2].Length == 0)
                    {
                        hv_Aarea[2] = 0;
                        hv_Arow[2] = 0;
                        hv_Acol[2] = 0;
                    }
                    if (hv_Arow[3].Length == 0)
                    {
                        hv_Aarea[3] = 0;
                        hv_Arow[3] = 0;
                        hv_Acol[3] = 0;
                    }
                    double Amax = hv_Aarea[0], Arow = hv_Arow[0], Acol = hv_Acol[0];
                    for (int I = 1; I < 4; I++)
                    {
                        if (hv_Aarea[I] > Amax)
                        {
                            Amax = hv_Aarea[I];
                            Arow = hv_Arow[I];
                            Acol = hv_Acol[I];
                        }
                    }
                    double SubRow, SubCol, angle;
                    hWVision.ClearWindow();
                    hWVision.SetDraw("fill");
                    hWVision.DispObj(ho_ImageSet);
                    hWVision.SetColor("blue");
                    hWVision.DispLine((HTuple)Arow, (HTuple)Acol, hv_RowCenter, hv_ColCenter);
                    hWVision.SetColor("green");
                    hWVision.SetLineWidth(1);
                    hWVision.DispCross(row, col, width, 0);
                    SubRow = (HTuple)Arow - hv_RowCenter;
                    SubCol = (HTuple)Acol - hv_ColCenter;
                    angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                    if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                        angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                    else
                        angle = ((SubCol > 0) ? angle : 180 - angle);
                    hv_Deg2 = angle;
                    //hv_Deg2 = ((hv_Deg2 >= 180) ? hv_Deg2 - 180 : hv_Deg2 + 180);
                    HD.set_display_font(hWVision, 18, "sans", "true", "false");
                    HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm + "mm", "", 150, 150, "green", "false");
                    HD.disp_message(hWVision, "Y_bias:" + (hv_RowCenter - row) * ypm + "mm", "", 300, 150, "green", "false");
                    HD.disp_message(hWVision, "T:" + string.Format("{0:f3}°", (double)hv_Deg2), "", 450, 150, "green", "false");
                    hv_RowCut = SubRow;
                    hv_ColumnCut = SubCol;
                    #endregion
                }
                if (cBAMode.Value == 6)
                {
                    #region Mode6
                    HTuple hv_Angle = new HTuple();
                    HOperatorSet.GenCircle(out ho_Circle, halcon.hv_ResultRow, halcon.hv_ResultColumn, halcon.hv_ResultRadius);
                    hWVision.ClearWindow();
                    hWVision.SetDraw("margin");
                    hWVision.DispObj(ho_ImageSet);
                    hWVision.SetColor("green");
                    hWVision.DispObj(ho_Circle);
                    hWVision.SetColor("blue");
                    hWVision.DispLine(halcon.hv_ResultRow, halcon.hv_ResultColumn, hv_RowCenter, hv_ColCenter);
                    hWVision.SetColor("green");
                    hWVision.SetLineWidth(2);
                    hWVision.DispCross(halcon.hv_ResultRow, halcon.hv_ResultColumn, 50, 0);
                    HOperatorSet.AngleLx(halcon.hv_ResultRow, halcon.hv_ResultColumn, hv_RowCenter, hv_ColCenter, out hv_Angle);
                    //SubRow = hv_RowCut - hv_RowCenter;
                    //SubCol = hv_ColumnCut - hv_ColCenter;
                    //angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                    //if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                    //    angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                    //else
                    //    angle = ((SubCol > 0) ? angle : 180 - angle);
                    hv_Deg2 = hv_Angle.TupleDeg();
                    //hv_Deg2 = ((hv_Deg2 >= 180) ? hv_Deg2 - 180 : hv_Deg2 + 180);
                    HD.set_display_font(hWVision, 18, "sans", "true", "false");
                    HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - halcon.hv_ResultColumn.D) * xpm + "mm", "", 150, 150, "green", "false");
                    HD.disp_message(hWVision, "Y_bias:" + (hv_RowCenter - halcon.hv_ResultRow.D) * ypm + "mm", "", 300, 150, "green", "false");
                    HD.disp_message(hWVision, "T:" + string.Format("{0:f3}°", (double)hv_Deg2), "", 450, 150, "green", "false");

                    #endregion
                }
                if (SetNum == "8")
                    hv_AddDeg = hv_Deg2;
                if (cBGlueQ.Checked)
                {
                    hv_RowCut_Befort = hv_RowCut;
                    hv_ColumnCut_Befort = hv_ColumnCut;
                }
                if (cBGlueH.Checked)
                {
                    hv_RowCut_After = hv_RowCut;
                    hv_ColumnCut_After = hv_ColumnCut;
                }

            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void txtDegmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void txtDegmax_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void btnSaveCut_Click(object sender, EventArgs e)
        {
            try
            {
                #region CCD
                string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area4 = "";
                if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                     (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                     (cBLocation4.Enabled && cBLocation4.Text == ""))
                    return;
                if (cBLocation.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation.SelectedIndex == 1)
                    area1 = "Platform";
                area2 = (cBLocation2.SelectedIndex + 1).ToString();
                area4 = (cBLocation4.SelectedIndex + 1).ToString();
                if (SetNum == "6")
                {
                    if (cBLocation3.SelectedIndex == 0)
                        area1 = "PickUp";
                    if (cBLocation3.SelectedIndex == 1)
                        area1 = "Platform1";
                    if (cBLocation3.SelectedIndex == 2)
                        area1 = "Platform2";
                }
                switch (int.Parse(SetNum))
                {
                    case 1: CCDNAME = "A1CCD1-" + area4; break;
                    case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                    case 3: CCDNAME = "A2CCD1-" + area4; break;
                    case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                    case 5: CCDNAME = "PCCD1"; break;
                    case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                    case 7: CCDNAME = "GCCD1"; break;
                    case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                    case 9: CCDNAME = "QCCD"; break;
                }
                #endregion
                if (txtDegmin.Text != "")
                {
                    #region CCD区分
                    if (SetNum == "1" || SetNum == "3")
                    {
                        //CCDNAME = CCDNAME + "-" + area4;
                        iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
                    }
                    if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                        iniFile.Write(cn, "Location", area1, FrmMain.propath);
                    if (SetNum == "8")
                    {
                        iniFile.Write(cn, "Location", area2, FrmMain.propath);
                        string astatus = ((cBCutGQ.Checked) ? "true" : "false");
                        iniFile.Write(CCDNAME, "GQAngleStatus", astatus, FrmMain.propath);
                        astatus = ((cBCutGH.Checked) ? "true" : "false");
                        iniFile.Write(CCDNAME, "GHAngleStatus", astatus, FrmMain.propath);
                        iniFile.Write(CCDNAME, "Glue_Follow", Glue.Glue_Follow.ToString(), FrmMain.propath);
                    }
                    else
                    {
                        string astatus = ((cBCut.Checked) ? "true" : "false");
                        iniFile.Write(CCDNAME, "AngleStatus", astatus, FrmMain.propath);
                    }
                    iniFile.Write(CCDNAME, "AngleMode", cBAMode.Value.ToString(), FrmMain.propath);
                    #endregion
                    if (cBAMode.Value == 2)
                    {
                        #region  Mode2
                        iniFile.Write(CCDNAME, "AngleRmin", (tBAMinRa.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "AngleRmax", (tBAMaxRa.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "AnglebinB2W", (tBAB2W.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "AnglebinW2B", (tBAW2B.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "AngleAreamin", (tBAAreaMin.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "AngleAreamax", (tBAAreaMax.Value).ToString(), FrmMain.propath);
                        string AddStatus = ((cBAddFCT.Checked) ? "true" : "false");
                        iniFile.Write(CCDNAME, "Mode2Add", AddStatus, FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode2Widthmin", (tBAWMin.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode2Widthmax", (tBAWMax.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode2Lengthmin", (tBALMin.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode2Lengthmax", (tBALMax.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "BaDeg+", txtDegmax.Text, FrmMain.propath);
                        iniFile.Write(CCDNAME, "BaDeg-", txtDegmin.Text, FrmMain.propath);
                        #endregion
                    }
                    if (cBAMode.Value == 4)
                    {
                        #region  Mode4
                        iniFile.Write(CCDNAME, "Mode4AngleRmin", (tBAMinRa4.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode4AngleRmax", (tBAMaxRa4.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode4AnglebinB2W", (tBAB2W4.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode4AnglebinW2B", (tBAW2B4.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode4AngleAreamin", (tBAAreaMin4.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode4AngleAreamax", (tBAAreaMax4.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode4BaDeg+", txtDegmax.Text, FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode4BaDeg-", txtDegmin.Text, FrmMain.propath);
                        #endregion
                    }
                    if (cBAMode.Value == 5)
                    {
                        #region Mode5
                        iniFile.Write(CCDNAME, "ARegionCenDistance", tBARegionDis.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "ARegionLength", tBARegionlen1.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "ARegionWidth", tBARegionlen2.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "ARegionDryThreshold", tBARegionGray.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "ARegionDeg+", txtDegmax.Text, FrmMain.propath);
                        iniFile.Write(CCDNAME, "ARegionDeg-", txtDegmin.Text, FrmMain.propath);
                        #endregion
                    }
                    if (cBAMode.Value == 6)
                    {
                        #region Mode6
                        iniFile.Write(CCDNAME, "CircleMeasureSelect", halcon.CircleMeasureSelect, FrmMain.propath);
                        iniFile.Write(CCDNAME, "CircleRadius", halcon.CircleRadius.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "CircleLength", halcon.CircleLength.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "CircleMeasureTransition", halcon.CircleMeasureTransition.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "CircleMeasureThreshold", halcon.CircleMeasureThreshold.ToString(), FrmMain.propath);
                        #endregion
                    }
                    #region 四点
                    iniFile.Write(CCDNAME, "Deg4Checked", cBDeg4.Checked.ToString(), FrmMain.propath);
                    if (cBDeg4.Checked)
                    {
                        iniFile.Write(CCDNAME, "Deg4AnglePlus", ucDeg4Shift.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Deg4AngleDis", ucDeg4Dis.Value.ToString(), FrmMain.propath);
                        IniFile.Write(CCDNAME, "Deg4AngleIntersection", ucDeg4AngleIntersection.Value.ToString(), FrmMain.propath);
                    }
                    #endregion
                    #region Light
                    if (SetNum == "1")
                    {
                        iniFile.Write(CCDNAME, "LighterValue1", (UD_LED1Lig.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "LighterValue2", (UD_LED2Lig.Value).ToString(), FrmMain.propath);
                    }
                    if (SetNum == "2")
                        iniFile.Write(CCDNAME, "LighterValue", (UD_LED3Lig.Value).ToString(), FrmMain.propath);
                    if (SetNum == "3")
                    {
                        iniFile.Write(CCDNAME, "LighterValue1", (UD_LED4Lig.Value).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "LighterValue2", (UD_LED5Lig.Value).ToString(), FrmMain.propath);
                    }
                    if (SetNum == "4")
                        iniFile.Write(CCDNAME, "LighterValue", (UD_LED6Lig.Value).ToString(), FrmMain.propath);
                    if (SetNum == "5")
                    {
                        iniFile.Write(CCDNAME, "LighterValue1", (UD_LED7Lig.Value).ToString(), FrmMain.propath);
                        //iniFile.Write(CCDNAME, "LighterValue2", (UD_LED8Lig.Value).ToString(), FrmMain.propath); //调整至QCCD
                    }
                    if (SetNum == "6")
                        iniFile.Write(CCDNAME, "LighterValue", (UD_LED9Lig.Value).ToString(), FrmMain.propath);
                    if (SetNum == "7")
                        iniFile.Write(CCDNAME, "LighterValue", (UD_LED10Lig.Value).ToString(), FrmMain.propath);
                    if (SetNum == "8")
                        iniFile.Write(CCDNAME, "LighterValue", (UD_LED11Lig.Value).ToString(), FrmMain.propath);
                    if (SetNum == "9")
                    {
                        iniFile.Write(CCDNAME, "LighterValue1", (UD_LED8Lig.Value).ToString(), FrmMain.propath);
                        //iniFile.Write(CCDNAME, "LighterValue1", (UD_LED12Lig.Value).ToString(), FrmMain.propath);
                        //iniFile.Write(CCDNAME, "LighterValue2", (UD_LED13Lig.Value).ToString(), FrmMain.propath);
                        //iniFile.Write(CCDNAME, "LighterValue3", (UD_LED14Lig.Value).ToString(), FrmMain.propath);
                        //iniFile.Write(CCDNAME, "LighterValue4", (UD_LED15Lig.Value).ToString(), FrmMain.propath);
                    }
                    #endregion
                    iniFile.Write(CCDNAME, "DegLineChecked", cBDegLine.Checked.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "Deg3RChecked", cbDeg3.Checked.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "MarkRChecked", cBMark4.Checked.ToString(), FrmMain.propath);
                    #region fuzhu3
                    if (cbDeg3.Checked)
                    {
                        iniFile.Write(CCDNAME, "Deg3RDegree", tBDeg3RShift.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Deg3RSetDis", tBDeg3RDis.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Deg3RLength", tBDeg3RLen1.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Deg3RWidth", tBDeg3RLen2.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Deg3RgrayB2W", tBDeg3RB2W.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Deg3RgrayW2B", tBDeg3RW2B.Value.ToString(), FrmMain.propath);
                    }
                    #endregion
                    #region fuzhu4
                    if (cBMark4.Checked)
                    {
                        iniFile.Write(CCDNAME, "MarkRDegree", UDMarkRShift.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "MarkRSetDis", UDMarkRDis.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "MarkRLength", UDMarkRLen1.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "MarkRWidth", UDMarkRLen2.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "MarkRCount", UDMarkNum.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "MarkRrMin", UDmarkRMin.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "MarkRgray1", UDmarkB2W.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "MarkRgray2", UDmarkW2B.Value.ToString(), FrmMain.propath);
                    }
                    #endregion
                    if (cBAMode.Value == 1)
                    {
                        #region Mode1
                        iniFile.Write(CCDNAME, "Mode1DegLineRow1", Math.Round((double)hv_RowIRLs, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode1DegLineCol1", Math.Round((double)hv_ColIRLs, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode1DegLineRow2", Math.Round((double)hv_RowIRLe, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode1DegLineCol2", Math.Round((double)hv_ColIRLe, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode1DegLineGray", UDlineThreshold.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode1DegLineWidth", UDlineWidth.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode1Deg+", txtDegmax.Text, FrmMain.propath);
                        iniFile.Write(CCDNAME, "Mode1Deg-", txtDegmin.Text, FrmMain.propath);
                        string Tfilepath = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME + "\\DegTemplate";
                        if (!Directory.Exists(Tfilepath)) //创建ini文档H
                            Directory.CreateDirectory(Tfilepath);
                        HOperatorSet.WriteShapeModel(hv_DLModelID, Tfilepath + "\\DegLocation.shm");
                        HOperatorSet.ClearShapeModel(hv_DLModelID);
                        HOperatorSet.WriteMetrologyModel(hv_MetrologyHandle, Tfilepath + "\\DegTemplate.shm");
                        HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                        #endregion
                    }
                    #region DegLine
                    if (cBDegLine.Checked)
                    {
                        iniFile.Write(CCDNAME, "DegLineRow1", Math.Round((double)hv_RowIRLs, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "DegLineCol1", Math.Round((double)hv_ColIRLs, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "DegLineRow2", Math.Round((double)hv_RowIRLe, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "DegLineCol2", Math.Round((double)hv_ColIRLe, 4).ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "DegLineGray", UDDegLth.Value.ToString(), FrmMain.propath);
                        iniFile.Write(CCDNAME, "DegLineWidth", UDDegLWidth.Value.ToString(), FrmMain.propath);
                        string Tfilepath = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME;
                        if (!Directory.Exists(Tfilepath)) //创建ini文档H
                            Directory.CreateDirectory(Tfilepath);
                        HOperatorSet.WriteShapeModel(hv_DLModelID, Tfilepath + "\\DegLineLocation.shm");
                        HOperatorSet.ClearShapeModel(hv_DLModelID);
                        HOperatorSet.WriteMetrologyModel(hv_MetrologyHandle, Tfilepath + "\\DegLineTemplate.shm");
                        HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                    }
                    #endregion
                }
                else
                {
                    MessageBox.Show("角度偏差范围未设置！");
                }
            }
            catch
            { }
        }

        private void cBDegLine_CheckedChanged(object sender, EventArgs e)
        {
            if (cBDegLine.Checked & cbDeg3.Checked)
            {
                cBDegLine.Checked = false;
                MessageBox.Show("角度辅助1和角度辅助3不能同时开启，请重新确认选择！");
            }
        }
        #region 角度辅助1
        HTuple hv_RowIRLs, hv_ColIRLs, hv_RowIRLe, hv_ColIRLe;
        HTuple hv_Index1 = null;
        HTuple hv_RowBegin = null, hv_ColBegin = null, hv_RowEnd = null;
        HTuple hv_ColEnd = null, hv_Angle = null, hv_AngleDeg = null;
        HTuple hv_DegLth = 20, hv_DegLWidth = 80;
        HObject ho_MeasureContours = new HObject(), ho_MeasuredLines = new HObject();
        HTuple hv_DLRowCh, hv_DLColumnCh, hv_DLangle = new HTuple(), hv_DLlength1, hv_DLlength2;
        HTuple hv_DLModelID = new HTuple();
        private void btnDegLLocate_Click(object sender, EventArgs e)
        {
            HD.disp_message(hWVision, "点击鼠标左键画检测区域,点击右键确认", "", 100, 100, "green", "false");
            btnDegLLocate.BackColor = Color.GreenYellow;
            hWVision.SetColor("red");
            HOperatorSet.DrawRectangle2(hWVision, out hv_DLRowCh, out hv_DLColumnCh, out hv_DLangle, out hv_DLlength1, out hv_DLlength2);
            btnDegLLocate.BackColor = Color.WhiteSmoke;
            ShowDLRegion();
        }
        void ShowDLRegion()
        {
            if (readpara)
                return;
            try
            {
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);

                ho_FModelRegion.Dispose();
                HOperatorSet.GenRectangle2(out ho_FModelRegion, hv_DLRowCh, hv_DLColumnCh, hv_DLangle, hv_DLlength1, hv_DLlength2);
                ho_FModelImage.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_FModelRegion, out ho_FModelImage);
                HOperatorSet.CreateShapeModel(ho_FModelImage, "auto", (new HTuple(0)).TupleRad()
                    , (new HTuple(360)).TupleRad(), "auto", "auto", "use_polarity", "auto", "auto", out hv_DLModelID);
                ho_FModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_FModelContours, hv_DLModelID, 1);
                HOperatorSet.AreaCenter(ho_FModelRegion, out Fhv_Area, out hv_FRow, out hv_FCol);
                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_FRow, hv_FCol, 0, out hv_DLHomMat2D);
                ho_ShowContours.Dispose();
                HOperatorSet.AffineTransContourXld(ho_FModelContours, out ho_ShowContours, hv_DLHomMat2D);
                ho_ShowContoursRegion.Dispose();
                HOperatorSet.GenRectangle2ContourXld(out ho_ShowContoursRegion, hv_DLRowCh, hv_DLColumnCh, hv_DLangle, hv_DLlength1, hv_DLlength2);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.DispObj(ho_ShowContours);
                hWVision.DispObj(ho_ShowContoursRegion);

            }
            catch (Exception ER)
            {
                MessageBox.Show(ER.ToString());
            }
        }
        private void btnDegLine_Click(object sender, EventArgs e)
        {
            #region CCD
            string CCDNAME = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
                CCDNAME = CCDNAME + "-" + area4;
            #endregion
            HD.set_display_font(hWVision, 18, "sans", "true", "false");
            string Tfilepath = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME;
            if (hv_DLModelID.Length == 0 || hv_DLModelID == null)
                HOperatorSet.ReadShapeModel(Tfilepath + "\\DegLineLocation.shm", out hv_DLModelID);

            HD.disp_message(hWVision, "点击鼠标左键在对应边缘“以逆时针方向”画一条直线,点击右键确认", "", 100, 100, "red", "true");
            hWVision.SetColor("red");
            btnDegLine.BackColor = Color.GreenYellow;
            HOperatorSet.DrawLine(hWVision, out hv_RowIRLs, out hv_ColIRLs, out hv_RowIRLe, out hv_ColIRLe);
            btnDegLine.BackColor = Color.WhiteSmoke;
            DegLineShow();
        }
        HTuple hv_DLRowFound = new HTuple(), hv_DLColFound = new HTuple(), hv_DLAngleFound = new HTuple();
        HTuple hv_DLScaleFound = new HTuple(), hv_DLScoreFound = new HTuple(), hv_DLHomMat2D = new HTuple();
        HObject ho_DLRContours = new HObject(), ho_DLRLines = new HObject();
        void DegLineShow()
        {
            try
            {
                if (readpara)
                    return;
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);

                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_RowIRLs, hv_ColIRLs, hv_RowIRLe, hv_ColIRLe,
                     hv_DegLWidth, 10, 1, hv_DegLth, new HTuple(), "positive", out hv_Index1);
                ho_MeasureContours.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_MeasureContours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                ho_MeasuredLines.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_MeasuredLines, hv_MetrologyHandle, "all", "all", 1.5);

                HOperatorSet.SetMetrologyModelParam(hv_MetrologyHandle, "reference_system", ((hv_FRow.TupleConcat(hv_FCol))).TupleConcat(0));

                HOperatorSet.GetShapeModelContours(out ho_FModelContours, hv_DLModelID, 1);
                HOperatorSet.FindScaledShapeModel(ho_ImageSet, hv_DLModelID, (new HTuple(-90)).TupleRad(), (new HTuple(180)).TupleRad(), 0.9, 1.1, 0.3, 1, 0.5,
                    "least_squares", 0, 0.9, out hv_DLRowFound, out hv_DLColFound, out hv_DLAngleFound, out hv_DLScaleFound, out hv_DLScoreFound);
                //如果找到模板
                if ((int)(new HTuple((new HTuple(1)).TupleEqual(new HTuple(hv_DLRowFound.TupleLength())))) != 0)
                {
                    HOperatorSet.HomMat2dIdentity(out hv_DLHomMat2D);
                    HOperatorSet.HomMat2dScale(hv_DLHomMat2D, hv_DLScaleFound, hv_DLScaleFound, 0, 0, out hv_DLHomMat2D);
                    HOperatorSet.HomMat2dRotate(hv_DLHomMat2D, hv_DLAngleFound, 0, 0, out hv_DLHomMat2D);
                    HOperatorSet.HomMat2dTranslate(hv_DLHomMat2D, hv_DLRowFound - 0, hv_DLColFound - 0, out hv_DLHomMat2D);
                    ho_ResultContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_FModelContours, out ho_ResultContours, hv_DLHomMat2D);

                    //按照找到的模板位置，移动测量位置
                    HOperatorSet.AlignMetrologyModel(hv_MetrologyHandle, hv_DLRowFound, hv_DLColFound, hv_DLAngleFound);
                    //应用测量
                    HOperatorSet.ApplyMetrologyModel(ho_ImageSet, hv_MetrologyHandle);
                    //获取结果
                    ho_DLRContours.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_DLRContours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_Index1, "all", "result_type", "all_param", out hv_FRectangleParameter);

                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "row_begin", out hv_RowBegin);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "column_begin", out hv_ColBegin);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "row_end", out hv_RowEnd);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "column_end", out hv_ColEnd);
                    HOperatorSet.AngleLx(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, out hv_Angle);
                    hv_AngleDeg = hv_Angle.TupleDeg();
                    ho_DLRLines.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_DLRLines, hv_MetrologyHandle, "all", "all", 1.5);

                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_ImageSet);
                    hWVision.SetColor("red");
                    hWVision.DispCross(row, col, width, 0);
                    hWVision.SetColor("green");
                    hWVision.DispObj(ho_DLRContours);
                    hWVision.SetColor("yellow");
                    hWVision.DispObj(ho_DLRLines);
                }
                else
                {
                    HD.disp_message(hWVision, "没有找到模板", "window", 24, 24, "black", "true");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tBDegLth_ValueChanged(object sender, EventArgs e)
        {
            int dlth = tBDegLth.Value;
            UDDegLth.Value = dlth;
            hv_DegLth = dlth;
            DegLineShow();
        }
        private void UDDegLth_ValueChanged(object sender, EventArgs e)
        {
            int dlth = (int)UDDegLth.Value;
            tBDegLth.Value = dlth;
        }
        private void tBDegLWidth_ValueChanged(object sender, EventArgs e)
        {
            int dlw = tBDegLWidth.Value;
            UDDegLWidth.Value = dlw;
            hv_DegLWidth = dlw;
            DegLineShow();
        }
        private void UDDegLWidth_ValueChanged(object sender, EventArgs e)
        {
            int dlw = (int)UDDegLWidth.Value;
            tBDegLWidth.Value = dlw;
        }
        private void btnDegLCal_Click(object sender, EventArgs e)
        {
            try
            {
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetColor("red");
                hWVision.DispCross(row, col, width, 0);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_DLRLines);
                HOperatorSet.SetTposition(hWVision, 350, 24);
                hv_Deg2 = hv_AngleDeg;
                HOperatorSet.WriteString(hWVision, (("Degree = ") + hv_AngleDeg) + "°");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        #endregion
        #region  角度辅助2 四点坐标
        private void ucDeg4Shift_ValueChanged(int CurrentValue)
        {
            ShowDeg4();
        }

        private void ucDeg4Dis_ValueChanged(int CurrentValue)
        {
            ShowDeg4();
        }

        private void ucDeg4AngleIntersection_ValueChanged(int CurrentValue)
        {
            ShowDeg4();
        }
        HObject ho_CrossP1 = new HObject(), ho_CrossP2 = new HObject();
        HObject ho_CrossP3 = new HObject(), ho_CrossP4 = new HObject();
        void ShowDeg4()
        {
            if (readpara)
                return;
            try
            {
            
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                if (!(hv_Deg2 == null || hv_Deg2 == 720.0 || hv_Deg2.Length == 0))
                {
                    hv_AddDegPlus = (HTuple)ucDeg4Shift.Value;
                    hv_grayDistance = (HTuple)ucDeg4Dis.Value;

                    ho_ImageTest.Dispose();
                    ho_ImageTest = ho_ImageSet;
                    double distance = hv_grayDistance;
                    double AngleIntersection = ucDeg4AngleIntersection.Value;
                    //double k1 = (hv_ColCenter - hv_g1ColumnCh) * 1.0 / (hv_RowCenter - hv_g1RowCh);// 坐标直线斜率k
                    double k1 = Math.Tan(Math.PI * (hv_Deg2 + hv_AddDegPlus - AngleIntersection) / 180);
                    double k2 = -1.0 / k1;
                    cenPoint.X = hv_RowCenter[0].F; cenPoint.Y = hv_ColCenter[0].F;
                    GetPointXY(cenPoint, distance, k1, ref g2Point, ref g4Point);
                    hv_g1RowCh = g2Point.X;
                    hv_g1ColumnCh = g2Point.Y;
                    hv_g3RowCh = g4Point.X;
                    hv_g3ColumnCh = g4Point.Y;
                    k1 = Math.Tan(Math.PI * (hv_Deg2 + hv_AddDegPlus + AngleIntersection) / 180);
                    k2 = -1.0 / k1;
                    GetPointXY(cenPoint, distance, k2, ref g2Point, ref g4Point);
                    hv_g2RowCh = g2Point.X;
                    hv_g2ColumnCh = g2Point.Y;
                    hv_g4RowCh = g4Point.X;
                    hv_g4ColumnCh = g4Point.Y;

                    HOperatorSet.GenCrossContourXld(out ho_CrossP1, hv_g1RowCh, hv_g1ColumnCh, 30, 0.785398);
                    HOperatorSet.GenCrossContourXld(out ho_CrossP2, hv_g2RowCh, hv_g2ColumnCh, 30, 0.785398);
                    HOperatorSet.GenCrossContourXld(out ho_CrossP3, hv_g3RowCh, hv_g3ColumnCh, 30, 0.785398);
                    HOperatorSet.GenCrossContourXld(out ho_CrossP4, hv_g4RowCh, hv_g4ColumnCh, 30, 0.785398);
                    hWVision.DispObj(ho_ImageSet);
                    hWVision.SetColor("red");
                    hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                    hWVision.SetColor("green");
                    hWVision.SetLineWidth(2);
                    hWVision.DispObj(ho_CrossP1);
                    hWVision.DispObj(ho_CrossP2);
                    hWVision.DispObj(ho_CrossP3);
                    hWVision.DispObj(ho_CrossP4);
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        #endregion
        #region 角度辅助3
        HTuple hv_deg3b2w = new HTuple(), hv_deg3w2b = new HTuple();
        HTuple hv_deg31area = new HTuple(), hv_deg32area = new HTuple();
        HTuple hv_deg31row = new HTuple(), hv_deg32row = new HTuple();
        HTuple hv_deg31col = new HTuple(), hv_deg32col = new HTuple();
        HTuple hv_deg3row = new HTuple(), hv_deg3col = new HTuple();
        HObject ho_deg3Region1 = new HObject(), ho_deg3Region2 = new HObject();
        private void btnDegDraw3_Click(object sender, EventArgs e)
        {
            try
            {
                if (SetNum == "8")
                {
                    if (hv_ColCenter.Length == 0)
                    {
                        if ((cBCutGQ.Checked || cBCutGH.Checked) & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                            MessageBox.Show("请先找到圆心和角度！");
                        else
                            MessageBox.Show("请先找到圆心！");
                        return;
                    }
                }
                else
                {
                    if (hv_ColCenter.Length == 0)
                    {
                        if (cBCut.Checked & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                            MessageBox.Show("请先找到圆心和角度！");
                        else
                            MessageBox.Show("请先找到圆心！");
                        return;
                    }
                }
                hv_AddDeg = hv_Deg2;
                ShowDeg3Fang();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        void ShowDeg3Fang()
        {
            if (readpara)
                return;
            try
            {
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                if (!(hv_AddDeg == null || hv_AddDeg == 720.0 || hv_AddDeg.Length == 0))
                {
                    hv_AddDegPlus = (HTuple)UDDeg3RShift.Value;
                    hv_g1angle = (Math.PI / 180) * hv_AddDeg;
                    hv_g1length1 = (HTuple)UDDeg3RLen1.Value;
                    hv_g1length2 = (HTuple)UDDeg3RLen2.Value;
                    hv_grayDistance = (HTuple)UDDeg3RDis.Value;
                    hv_deg3b2w = (HTuple)UDDeg3RB2W.Value;
                    hv_deg3w2b = (HTuple)UDDeg3RW2B.Value;
                    double distance = hv_grayDistance;
                    //double k1 = (hv_ColCenter - hv_g1ColumnCh) * 1.0 / (hv_RowCenter - hv_g1RowCh);// 坐标直线斜率k
                    double k1 = Math.Tan(Math.PI * (hv_AddDeg + hv_AddDegPlus) / 180);
                    cenPoint.X = hv_RowCenter[0].F; cenPoint.Y = hv_ColCenter[0].F;
                    GetPointXY(cenPoint, distance, k1, ref g2Point, ref g4Point);
                    hv_g1RowCh = g2Point.X;
                    hv_g1ColumnCh = g2Point.Y;
                    hv_g2RowCh = g4Point.X;
                    hv_g2ColumnCh = g4Point.Y;

                    #region 区域1
                    ho_g1Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g1Rectangle, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g1Region, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Reduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_g1Region, out ho_g1Reduced);
                    ho_deg3Region1.Dispose();
                    HOperatorSet.Threshold(ho_g1Reduced, out ho_deg3Region1, hv_deg3b2w, hv_deg3w2b);
                    HOperatorSet.AreaCenter(ho_deg3Region1, out hv_deg31area, out hv_deg31row, out hv_deg31col);
                    #endregion
                    #region 区域2
                    ho_g2Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g2Rectangle, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g2Region, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Reduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_g2Region, out ho_g2Reduced);
                    ho_deg3Region2.Dispose();
                    HOperatorSet.Threshold(ho_g2Reduced, out ho_deg3Region2, hv_deg3b2w, hv_deg3w2b);
                    HOperatorSet.AreaCenter(ho_deg3Region2, out hv_deg32area, out hv_deg32row, out hv_deg32col);
                    #endregion

                    hWVision.DispObj(ho_ImageSet);
                    hWVision.SetColor("red");
                    hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                    hWVision.DispObj(ho_g1Rectangle);  //检测区域
                    hWVision.DispObj(ho_g2Rectangle);
                    hWVision.DispObj(ho_deg3Region1);  //检测区域
                    hWVision.DispObj(ho_deg3Region2);
                    HD.set_display_font(hWVision, 18, "sans", "true", "false");
                    HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                    HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tBDeg3RDis_ValueChanged(object sender, EventArgs e)
        {
            UDDeg3RDis.Value = tBDeg3RDis.Value;
        }
        private void UDDeg3RDis_ValueChanged(object sender, EventArgs e)
        {
            tBDeg3RDis.Value = (int)UDDeg3RDis.Value;
            ShowDeg3Fang();
        }
        private void tBDeg3RShift_ValueChanged(object sender, EventArgs e)
        {
            UDDeg3RShift.Value = tBDeg3RShift.Value;
        }
        private void UDDeg3RShift_ValueChanged(object sender, EventArgs e)
        {
            tBDeg3RShift.Value = (int)UDDeg3RShift.Value;
            ShowDeg3Fang();
        }
        private void tBDeg3RLen1_ValueChanged(object sender, EventArgs e)
        {
            UDDeg3RLen1.Value = tBDeg3RLen1.Value;
        }
        private void UDDeg3RLen1_ValueChanged(object sender, EventArgs e)
        {
            tBDeg3RLen1.Value = (int)UDDeg3RLen1.Value;
            ShowDeg3Fang();
        }
        private void tBDeg3RLen2_ValueChanged(object sender, EventArgs e)
        {
            UDDeg3RLen2.Value = tBDeg3RLen2.Value;
        }
        private void UDDeg3RLen2_ValueChanged(object sender, EventArgs e)
        {
            tBDeg3RLen2.Value = (int)UDDeg3RLen2.Value;
            ShowDeg3Fang();
        }
        private void tBDeg3RB2W_ValueChanged(object sender, EventArgs e)
        {
            UDDeg3RB2W.Value = tBDeg3RB2W.Value;
        }
        private void UDDeg3RB2W_ValueChanged(object sender, EventArgs e)
        {
            tBDeg3RB2W.Value = (int)UDDeg3RB2W.Value;
            tBDeg3RW2B.Value = 255;
            ShowDeg3Fang();
        }
        private void tBDeg3RW2B_ValueChanged(object sender, EventArgs e)
        {
            UDDeg3RW2B.Value = tBDeg3RW2B.Value;
        }
        private void UDDeg3RW2B_ValueChanged(object sender, EventArgs e)
        {
            tBDeg3RW2B.Value = (int)UDDeg3RW2B.Value;
            tBDeg3RB2W.Value = 1;
            ShowDeg3Fang();
        }
        private void btnDegCal_Click(object sender, EventArgs e)
        {
            if (hv_deg31area.Length != 0)
            {
                hv_deg3row = hv_deg31row;
                hv_deg3col = hv_deg31col;
                if (hv_deg31area < hv_deg32area)
                {
                    hv_deg3row = hv_deg32row;
                    hv_deg3col = hv_deg32col;
                }
                double SubRow, SubCol, angle;
                SubRow = hv_deg3row - hv_RowCenter;
                SubCol = hv_deg3col - hv_ColCenter;
                angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                    angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                else
                    angle = ((SubCol > 0) ? angle : 180 - angle);
                hv_Deg2 = angle;
                hWVision.ClearWindow();
                hWVision.SetDraw("fill");
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetColor("blue");
                hWVision.DispLine(hv_deg3row, hv_deg3col, hv_RowCenter, hv_ColCenter);
                hWVision.SetColor("green");
                hWVision.SetLineWidth(1);
                hWVision.DispCross(row, col, width, 0);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm + "mm", "", 150, 150, "green", "false");
                HD.disp_message(hWVision, "Y_bias:" + (hv_RowCenter - row) * ypm + "mm", "", 300, 150, "green", "false");
                HD.disp_message(hWVision, "T:" + string.Format("{0:f3}°", (double)hv_Deg2), "", 450, 150, "green", "false");
                hv_RowCut = new HTuple();
            }
            else
            {
                MessageBox.Show("请重新调整参数！");
            }
        }
        private void cbDeg3_CheckedChanged(object sender, EventArgs e)
        {
            if (cBDegLine.Checked & cbDeg3.Checked)
            {
                cbDeg3.Checked = false;
                MessageBox.Show("角度辅助1和角度辅助3不能同时开启，请重新确认选择！");
            }
        }
        #endregion
        #region var
        HObject ho_Wires = new HObject(), ho_WiresFilled = new HObject();
        HObject ho_Balls = new HObject(), ho_SingleBalls = new HObject();
        HObject ho_IntermediateBalls = new HObject(), ho_FinalBalls = new HObject();
        HObject ho_markg1 = new HObject(), ho_markg2 = new HObject(), ho_markg3 = new HObject(), ho_markg4 = new HObject();
        HTuple hv_MarkRow = new HTuple(), hv_MarkColumn = new HTuple(), hv_MarkRadius = new HTuple();
        HTuple hv_MarkRow1 = new HTuple(), hv_MarkColumn1 = new HTuple(), hv_MarkRadius1 = new HTuple();
        HTuple hv_MarkRow2 = new HTuple(), hv_MarkColumn2 = new HTuple(), hv_MarkRadius2 = new HTuple();
        HTuple hv_MarkRow3 = new HTuple(), hv_MarkColumn3 = new HTuple(), hv_MarkRadius3 = new HTuple();
        HTuple hv_MarkRow4 = new HTuple(), hv_MarkColumn4 = new HTuple(), hv_MarkRadius4 = new HTuple();
        HTuple hv_NumBalls1 = new HTuple(), hv_Diameter = new HTuple(), hv_meanDiameter = new HTuple();
        HTuple hv_NumBalls2 = new HTuple(), hv_NumBalls3 = new HTuple(), hv_NumBalls4 = new HTuple();
        HTuple hv_minDiameter = new HTuple();
        HTuple hv_markgray1 = 1, hv_markgray2 = 128;
        #endregion
        #region 角度辅助4
        private void btnMarkDraw_Click(object sender, EventArgs e)
        {
            try
            {
                if (SetNum == "8")
                {
                    if (hv_ColCenter.Length == 0)
                    {
                        if ((cBCutGQ.Checked || cBCutGH.Checked) & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                            MessageBox.Show("请先找到圆心和角度！");
                        else
                            MessageBox.Show("请先找到圆心！");
                        return;
                    }
                }
                else
                {
                    if (hv_ColCenter.Length == 0)
                    {
                        if (cBCut.Checked & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                            MessageBox.Show("请先找到圆心和角度！");
                        else
                            MessageBox.Show("请先找到圆心！");
                        return;
                    }
                }
                hv_AddDeg = hv_Deg2;
                ShowMarkFang();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        void ShowMarkFang()
        {
            try
            {
                if (readpara)
                    return;
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                if (!(hv_AddDeg.Length == 0 || hv_AddDeg.D == 720.0))
                {
                    #region 基础参数

                    hv_AddDegPlus = (HTuple)UDMarkRShift.Value;
                    hv_g1angle = (Math.PI / 180) * hv_AddDeg;
                    hv_g1length1 = (HTuple)UDMarkRLen1.Value;
                    hv_g1length2 = (HTuple)UDMarkRLen2.Value;
                    hv_grayDistance = (HTuple)UDMarkRDis.Value;
                    double distance = hv_grayDistance;
                    //double k1 = (hv_ColCenter - hv_g1ColumnCh) * 1.0 / (hv_RowCenter - hv_g1RowCh);// 坐标直线斜率k
                    double k1 = Math.Tan(Math.PI * (hv_AddDeg + hv_AddDegPlus) / 180);
                    double k2 = -1.0 / k1;
                    cenPoint.X = hv_RowCenter[0].F; cenPoint.Y = hv_ColCenter[0].F;
                    GetPointXY(cenPoint, distance, k1, ref g2Point, ref g4Point);
                    hv_g1RowCh = g2Point.X;
                    hv_g1ColumnCh = g2Point.Y;
                    hv_g3RowCh = g4Point.X;
                    hv_g3ColumnCh = g4Point.Y;
                    GetPointXY(cenPoint, distance, k2, ref g2Point, ref g4Point);
                    hv_g2RowCh = g2Point.X;
                    hv_g2ColumnCh = g2Point.Y;
                    hv_g4RowCh = g4Point.X;
                    hv_g4ColumnCh = g4Point.Y;
                    #endregion

                    HObject ho_g1conn = new HObject(), ho_g1sele = new HObject();
                    HObject ho_g2conn = new HObject(), ho_g2sele = new HObject();
                    HObject ho_g3conn = new HObject(), ho_g3sele = new HObject();
                    HObject ho_g4conn = new HObject(), ho_g4sele = new HObject();

                    #region 区域1
                    ho_g1Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g1Rectangle, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g1Region, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g1Region, out ho_g1Reduced);
                    #endregion
                    #region 区域3
                    ho_g3Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g3Rectangle, hv_g3RowCh, hv_g3ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g3Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g3Region, hv_g3RowCh, hv_g3ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g3Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g3Region, out ho_g3Reduced);
                    #endregion
                    #region 区域2
                    ho_g2Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g2Rectangle, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g2Region, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g2Region, out ho_g2Reduced);
                    #endregion
                    #region 区域4
                    ho_g4Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g4Rectangle, hv_g4RowCh, hv_g4ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g4Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g4Region, hv_g4RowCh, hv_g4ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g4Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g4Region, out ho_g4Reduced);
                    #endregion

                    if (!caling)
                    {
                        hWVision.DispObj(ho_ImageSet);
                        hWVision.SetColor("red");
                        hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                        hWVision.DispObj(ho_g1Rectangle);  //检测区域
                        hWVision.DispObj(ho_g3Rectangle);
                        hWVision.DispObj(ho_g2Rectangle);  //检测区域
                        hWVision.DispObj(ho_g4Rectangle);
                        HD.set_display_font(hWVision, 18, "sans", "true", "false");
                    }
                    #region quyudianshu
                    ShowMark(ho_g1Reduced);
                    hv_MarkRow1 = hv_MarkRow; hv_MarkColumn1 = hv_MarkColumn; hv_MarkRadius1 = hv_MarkRadius;
                    hv_NumBalls1 = new HTuple(hv_MarkRadius1.TupleLength());
                    if (hv_NumBalls1 >= 1 & !caling)
                    {
                        hv_Diameter = hv_MarkRadius1[0];
                        //hv_Diameter = 2 * hv_MarkRadius1;
                        //hv_meanDiameter = hv_Diameter.TupleMean();
                        //hv_minDiameter = hv_Diameter.TupleMin();
                        hWVision.SetColor("red");
                        HOperatorSet.DispCircle(hWVision, hv_MarkRow1, hv_MarkColumn1, hv_MarkRadius1);
                        //disp_message1(hv_WindowID, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow1 - (2 * hv_MarkRadius1),  
                        //    hv_MarkColumn1, "blue", "false");  //按数组格式显示
                        HD.disp_message(hWVision, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow1[0] - (2 * hv_MarkRadius1[0]),
                            hv_MarkColumn1[0], "blue", "false");
                    }
                    ShowMark(ho_g2Reduced);
                    hv_MarkRow2 = hv_MarkRow; hv_MarkColumn2 = hv_MarkColumn; hv_MarkRadius2 = hv_MarkRadius;
                    hv_NumBalls2 = new HTuple(hv_MarkRadius2.TupleLength());
                    if (hv_NumBalls2 >= 1 & !caling)
                    {
                        hv_Diameter = hv_MarkRadius2[0];
                        //hv_Diameter = 2 * hv_MarkRadius2;
                        //hv_meanDiameter = hv_Diameter.TupleMean();
                        //hv_minDiameter = hv_Diameter.TupleMin();
                        hWVision.SetColor("red");
                        HOperatorSet.DispCircle(hWVision, hv_MarkRow2, hv_MarkColumn2, hv_MarkRadius2);
                        //disp_message1(hv_WindowID, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow2 - (2 * hv_MarkRadius2),
                        //    hv_MarkColumn2, "blue", "false");
                        HD.disp_message(hWVision, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow2[0] - (2 * hv_MarkRadius2[0]),
                            hv_MarkColumn2[0], "blue", "false");
                    }
                    ShowMark(ho_g3Reduced);
                    hv_MarkRow3 = hv_MarkRow; hv_MarkColumn3 = hv_MarkColumn; hv_MarkRadius3 = hv_MarkRadius;
                    hv_NumBalls3 = new HTuple(hv_MarkRadius3.TupleLength());
                    if (hv_NumBalls3 >= 1 & !caling)
                    {
                        hv_Diameter = hv_MarkRadius3[0];
                        //hv_Diameter = 2 * hv_MarkRadius3;
                        //hv_meanDiameter = hv_Diameter.TupleMean();
                        //hv_minDiameter = hv_Diameter.TupleMin();
                        hWVision.SetColor("red");
                        HOperatorSet.DispCircle(hWVision, hv_MarkRow3, hv_MarkColumn3, hv_MarkRadius3);
                        //disp_message1(hv_WindowID, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow3 - (2 * hv_MarkRadius3),
                        //    hv_MarkColumn3, "blue", "false");
                        HD.disp_message(hWVision, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow3[0] - (2 * hv_MarkRadius3[0]),
                            hv_MarkColumn3[0], "blue", "false");
                    }
                    ShowMark(ho_g4Reduced);
                    hv_MarkRow4 = hv_MarkRow; hv_MarkColumn4 = hv_MarkColumn; hv_MarkRadius4 = hv_MarkRadius;
                    hv_NumBalls4 = new HTuple(hv_MarkRadius4.TupleLength());
                    if (hv_NumBalls4 >= 1 & !caling)
                    {
                        hv_Diameter = hv_MarkRadius4[0];
                        //hv_Diameter = 2 * hv_MarkRadius4;
                        //hv_meanDiameter = hv_Diameter.TupleMean();
                        //hv_minDiameter = hv_Diameter.TupleMin();
                        hWVision.SetColor("red");
                        HOperatorSet.DispCircle(hWVision, hv_MarkRow4, hv_MarkColumn4, hv_MarkRadius4);
                        //disp_message1(hv_WindowID, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow4 - (2 * hv_MarkRadius4),
                        //    hv_MarkColumn4, "blue", "false");
                        HD.disp_message(hWVision, "R: " + (hv_Diameter.TupleString(".4")), "image", hv_MarkRow4[0] - (2 * hv_MarkRadius4[0]),
                            hv_MarkColumn4[0], "blue", "false");
                    }
                    caling = false;
                    #endregion
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void UDMarkRShift_ValueChanged(object sender, EventArgs e)
        {
            ShowMarkFang();
        }
        private void UDMarkRLen1_ValueChanged(object sender, EventArgs e)
        {
            ShowMarkFang();
        }
        private void UDMarkRLen2_ValueChanged(object sender, EventArgs e)
        {
            ShowMarkFang();
        }
        private void UDMarkRDis_ValueChanged(object sender, EventArgs e)
        {
            ShowMarkFang();
        }
        int MarkRmin = 10;
        void ShowMark(HObject ho_DieGrey)
        {
            try
            {
                MarkRmin = (int)UDmarkRMin.Value;
                ho_Wires.Dispose();
                HOperatorSet.Threshold(ho_DieGrey, out ho_Wires, hv_markgray1, hv_markgray2);
                ho_WiresFilled.Dispose();
                HOperatorSet.FillUpShape(ho_Wires, out ho_WiresFilled, "area", 1, 100);
                ho_Balls.Dispose();
                HOperatorSet.OpeningCircle(ho_WiresFilled, out ho_Balls, MarkRmin);
                ho_SingleBalls.Dispose();
                HOperatorSet.Connection(ho_Balls, out ho_SingleBalls);
                ho_IntermediateBalls.Dispose();
                HOperatorSet.SelectShape(ho_SingleBalls, out ho_IntermediateBalls, "circularity", "and", 0.8, 1.0);
                ho_FinalBalls.Dispose();
                HOperatorSet.SortRegion(ho_IntermediateBalls, out ho_FinalBalls, "first_point", "true", "column");
                HOperatorSet.SmallestCircle(ho_FinalBalls, out hv_MarkRow, out hv_MarkColumn, out hv_MarkRadius);
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_DieGrey.Dispose();
                ho_Wires.Dispose();
                ho_WiresFilled.Dispose();
                ho_Balls.Dispose();
                ho_SingleBalls.Dispose();
                ho_IntermediateBalls.Dispose();
                ho_FinalBalls.Dispose();

                throw HDevExpDefaultException;
            }
            ho_DieGrey.Dispose();
            ho_Wires.Dispose();
            ho_WiresFilled.Dispose();
            ho_Balls.Dispose();
            ho_SingleBalls.Dispose();
            ho_IntermediateBalls.Dispose();
            ho_FinalBalls.Dispose();
        }
        private void UDmarkRMin_ValueChanged(object sender, EventArgs e)
        {
            ShowMarkFang();
        }
        private void UDmarkB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_markgray1 = (int)UDmarkB2W.Value;
            hv_markgray2 = (int)UDmarkW2B.Value;
            UDmarkW2B.Value = 255;
            ShowMarkgray();
        }
        private void UDmarkW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_markgray1 = (int)UDmarkB2W.Value;
            hv_markgray2 = (int)UDmarkW2B.Value;
            UDmarkB2W.Value = 1;
            ShowMarkgray();
        }
        void ShowMarkgray()
        {
            try
            {
                if (readpara)
                    return;
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                if (!(hv_AddDeg.Length == 0 || hv_AddDeg.D == 720.0))
                {
                    #region 基础参数

                    hv_AddDegPlus = (HTuple)UDMarkRShift.Value;
                    hv_g1angle = (Math.PI / 180) * hv_AddDeg;
                    hv_g1length1 = (HTuple)UDMarkRLen1.Value;
                    hv_g1length2 = (HTuple)UDMarkRLen2.Value;
                    hv_grayDistance = (HTuple)UDMarkRDis.Value;
                    double distance = hv_grayDistance;
                    //double k1 = (hv_ColCenter - hv_g1ColumnCh) * 1.0 / (hv_RowCenter - hv_g1RowCh);// 坐标直线斜率k
                    double k1 = Math.Tan(Math.PI * (hv_AddDeg + hv_AddDegPlus) / 180);
                    double k2 = -1.0 / k1;
                    cenPoint.X = hv_RowCenter[0].F; cenPoint.Y = hv_ColCenter[0].F;
                    GetPointXY(cenPoint, distance, k1, ref g2Point, ref g4Point);
                    hv_g1RowCh = g2Point.X;
                    hv_g1ColumnCh = g2Point.Y;
                    hv_g3RowCh = g4Point.X;
                    hv_g3ColumnCh = g4Point.Y;
                    GetPointXY(cenPoint, distance, k2, ref g2Point, ref g4Point);
                    hv_g2RowCh = g2Point.X;
                    hv_g2ColumnCh = g2Point.Y;
                    hv_g4RowCh = g4Point.X;
                    hv_g4ColumnCh = g4Point.Y;
                    #endregion

                    HObject ho_g1conn = new HObject(), ho_g1sele = new HObject();
                    HObject ho_g2conn = new HObject(), ho_g2sele = new HObject();
                    HObject ho_g3conn = new HObject(), ho_g3sele = new HObject();
                    HObject ho_g4conn = new HObject(), ho_g4sele = new HObject();

                    #region 区域1
                    ho_g1Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g1Rectangle, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g1Region, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g1Region, out ho_g1Reduced);
                    ho_markg1.Dispose();
                    HOperatorSet.Threshold(ho_g1Reduced, out ho_markg1, hv_markgray1, hv_markgray2);
                    #endregion
                    #region 区域3
                    ho_g3Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g3Rectangle, hv_g3RowCh, hv_g3ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g3Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g3Region, hv_g3RowCh, hv_g3ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g3Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g3Region, out ho_g3Reduced);
                    ho_markg3.Dispose();
                    HOperatorSet.Threshold(ho_g3Reduced, out ho_markg3, hv_markgray1, hv_markgray2);
                    #endregion
                    #region 区域2
                    ho_g2Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g2Rectangle, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g2Region, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g2Region, out ho_g2Reduced);
                    ho_markg2.Dispose();
                    HOperatorSet.Threshold(ho_g2Reduced, out ho_markg2, hv_markgray1, hv_markgray2);
                    #endregion
                    #region 区域4
                    ho_g4Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g4Rectangle, hv_g4RowCh, hv_g4ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g4Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g4Region, hv_g4RowCh, hv_g4ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g4Reduced.Dispose();
                    HOperatorSet.ReduceDomain(halcon.Image[i], ho_g4Region, out ho_g4Reduced);
                    ho_markg4.Dispose();
                    HOperatorSet.Threshold(ho_g4Reduced, out ho_markg4, hv_markgray1, hv_markgray2);
                    #endregion

                    if (!caling) //非计算状态显示
                    {
                        hWVision.DispObj(ho_ImageSet);
                        hWVision.SetColor("red");
                        hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                        hWVision.DispObj(ho_g1Rectangle);  //检测区域
                        hWVision.DispObj(ho_g3Rectangle);
                        hWVision.DispObj(ho_g2Rectangle);  //检测区域
                        hWVision.DispObj(ho_g4Rectangle);
                        hWVision.DispObj(ho_markg1);
                        hWVision.DispObj(ho_markg2);
                        hWVision.DispObj(ho_markg3);
                        hWVision.DispObj(ho_markg4);
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        HTuple hv_MarkRowmax = new HTuple(), hv_MarkColumnmax = new HTuple(), hv_MarkDistancemax = new HTuple();
        HTuple hv_MarkDistance1 = new HTuple(), hv_MarkDistance2 = new HTuple(), hv_MarkDistance3 = new HTuple();
        HTuple hv_MarkDistance4 = new HTuple(); bool caling = false;
        private void btnMark_Click(object sender, EventArgs e)
        {
            try
            {
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                caling = true;
                ShowMarkFang();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetColor("red");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                #region cal
                hv_NumBalls1 = new HTuple(hv_MarkRadius1.TupleLength());
                double SubRow, SubCol, angle;
                if (hv_NumBalls1 >= 1 & hv_NumBalls1 == (int)UDMarkNum.Value)
                {
                    HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow1[0], hv_MarkColumn1[0], out hv_MarkDistancemax);
                    hv_MarkRowmax = hv_MarkRow1[0]; hv_MarkColumnmax = hv_MarkColumn1[0];
                    for (int i1 = 1; i1 < hv_NumBalls1; i1++)
                    {
                        HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow1[i1], hv_MarkColumn1[i1], out hv_MarkDistance1);
                        if (hv_MarkDistance1 > hv_MarkDistancemax)
                        {
                            hv_MarkDistancemax = hv_MarkDistance1;
                            hv_MarkRowmax = hv_MarkRow1[i1]; hv_MarkColumnmax = hv_MarkColumn1[i1];
                        }
                    }
                    hWVision.SetColor("red");
                    HOperatorSet.DispCircle(hWVision, hv_MarkRow1, hv_MarkColumn1, hv_MarkRadius1);
                    hWVision.SetColor("blue");
                    HOperatorSet.DispLine(hWVision, hv_RowCenter, hv_ColCenter, hv_MarkRowmax, hv_MarkColumnmax);
                    SubRow = hv_MarkRowmax - hv_RowCenter;
                    SubCol = hv_MarkColumnmax - hv_ColCenter;
                    angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                    if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                        angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                    else
                        angle = ((SubCol > 0) ? angle : 180 - angle);
                    hv_Deg2 = angle;
                    if (SetNum == "8")
                        hv_AddDeg = hv_Deg2;
                }
                hv_NumBalls2 = new HTuple(hv_MarkRadius2.TupleLength());
                if (hv_NumBalls2 >= 1 & hv_NumBalls2 == (int)UDMarkNum.Value)
                {
                    HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow2[0], hv_MarkColumn2[0], out hv_MarkDistancemax);
                    hv_MarkRowmax = hv_MarkRow2[0]; hv_MarkColumnmax = hv_MarkColumn2[0];
                    for (int i1 = 1; i1 < hv_NumBalls2; i1++)
                    {
                        HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow2[i1], hv_MarkColumn2[i1], out hv_MarkDistance2);
                        if (hv_MarkDistance2 > hv_MarkDistancemax)
                        {
                            hv_MarkDistancemax = hv_MarkDistance2;
                            hv_MarkRowmax = hv_MarkRow2[i1]; hv_MarkColumnmax = hv_MarkColumn2[i1];
                        }
                    }
                    hWVision.SetColor("red");
                    HOperatorSet.DispCircle(hWVision, hv_MarkRow2, hv_MarkColumn2, hv_MarkRadius2);
                    hWVision.SetColor("blue");
                    HOperatorSet.DispLine(hWVision, hv_RowCenter, hv_ColCenter, hv_MarkRowmax, hv_MarkColumnmax);
                    SubRow = hv_MarkRowmax - hv_RowCenter;
                    SubCol = hv_MarkColumnmax - hv_ColCenter;
                    angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                    if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                        angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                    else
                        angle = ((SubCol > 0) ? angle : 180 - angle);
                    hv_Deg2 = angle;
                    if (SetNum == "8")
                        hv_AddDeg = hv_Deg2;
                }
                hv_NumBalls3 = new HTuple(hv_MarkRadius3.TupleLength());
                if (hv_NumBalls3 >= 1 & hv_NumBalls3 == (int)UDMarkNum.Value)
                {
                    HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow3[0], hv_MarkColumn3[0], out hv_MarkDistancemax);
                    hv_MarkRowmax = hv_MarkRow3[0]; hv_MarkColumnmax = hv_MarkColumn3[0];
                    for (int i1 = 1; i1 < hv_NumBalls3; i1++)
                    {
                        HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow3[i1], hv_MarkColumn3[i1], out hv_MarkDistance3);
                        if (hv_MarkDistance3 > hv_MarkDistancemax)
                        {
                            hv_MarkDistancemax = hv_MarkDistance3;
                            hv_MarkRowmax = hv_MarkRow3[i1]; hv_MarkColumnmax = hv_MarkColumn3[i1];
                        }
                    }
                    hWVision.SetColor("red");
                    HOperatorSet.DispCircle(hWVision, hv_MarkRow3, hv_MarkColumn3, hv_MarkRadius3);
                    hWVision.SetColor("blue");
                    HOperatorSet.DispLine(hWVision, hv_RowCenter, hv_ColCenter, hv_MarkRowmax, hv_MarkColumnmax);
                    SubRow = hv_MarkRowmax - hv_RowCenter;
                    SubCol = hv_MarkColumnmax - hv_ColCenter;
                    angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                    if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                        angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                    else
                        angle = ((SubCol > 0) ? angle : 180 - angle);
                    hv_Deg2 = angle;
                    if (SetNum == "8")
                        hv_AddDeg = hv_Deg2;
                }
                hv_NumBalls4 = new HTuple(hv_MarkRadius4.TupleLength());
                if (hv_NumBalls4 >= 1 & hv_NumBalls4 == (int)UDMarkNum.Value)
                {
                    HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow4[0], hv_MarkColumn4[0], out hv_MarkDistancemax);
                    hv_MarkRowmax = hv_MarkRow4[0]; hv_MarkColumnmax = hv_MarkColumn4[0];
                    for (int i1 = 1; i1 < hv_NumBalls4; i1++)
                    {
                        HOperatorSet.DistancePp(hv_RowCenter, hv_ColCenter, hv_MarkRow4[i1], hv_MarkColumn4[i1], out hv_MarkDistance4);
                        if (hv_MarkDistance4 > hv_MarkDistancemax)
                        {
                            hv_MarkDistancemax = hv_MarkDistance4;
                            hv_MarkRowmax = hv_MarkRow4[i1]; hv_MarkColumnmax = hv_MarkColumn4[i1];
                        }
                    }
                    hWVision.SetColor("red");
                    HOperatorSet.DispCircle(hWVision, hv_MarkRow4, hv_MarkColumn4, hv_MarkRadius4);
                    hWVision.SetColor("blue");
                    HOperatorSet.DispLine(hWVision, hv_RowCenter, hv_ColCenter, hv_MarkRowmax, hv_MarkColumnmax);
                    SubRow = hv_MarkRowmax - hv_RowCenter;
                    SubCol = hv_MarkColumnmax - hv_ColCenter;
                    angle = Math.Atan(Math.Abs(SubRow / SubCol)) * 180 / Math.PI;
                    if (SubRow > 0)   //以右侧为基准点，逆时针0至360度
                        angle = ((SubCol > 0) ? 360 - angle : 180 + angle);
                    else
                        angle = ((SubCol > 0) ? angle : 180 - angle);
                    hv_Deg2 = angle;
                    if (SetNum == "8")
                        hv_AddDeg = hv_Deg2;
                }
                #endregion
            }
            catch (Exception er)
            {
                caling = false;
                MessageBox.Show(er.ToString());
            }
        }
        #endregion
        #endregion

        #region 胶点辨识
        private void Gluecheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Gluecheck.Checked)
            {
                gBglue.Enabled = true;
                gBglue1.Enabled = true;
            }
            else
            {
                gBglue.Enabled = false;
                gBglue1.Enabled = false;
            }
        }
        private void cBGlueQ_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGlueH.Checked)
                cBGlueH.Checked = false;
            if (cBGlueQ.Checked)
            {
                hv_Deg2 = 720.0;
                btnDrawGLUE.Enabled = true;
                tBglueDis.Enabled = true;
                UDglueDis.Enabled = true;
                tBgluelen1.Enabled = true;
                UDgluelen1.Enabled = true;
                tBgluelen2.Enabled = true;
                UDgluelen2.Enabled = true;
                tBGlueGray.Enabled = false;
                UDGlueGray.Enabled = false;
                ucGlueGray_2.Enabled = false;
            }
        }
        private void cBGlueH_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGlueQ.Checked)
                cBGlueQ.Checked = false;
            if (cBGlueH.Checked)
            {
                btnDrawGLUE.Enabled = false;
                tBglueDis.Enabled = false;
                UDglueDis.Enabled = false;
                tBgluelen1.Enabled = false;
                UDgluelen1.Enabled = false;
                tBgluelen2.Enabled = false;
                UDgluelen2.Enabled = false;
                tBGlueGray.Enabled = true;
                UDGlueGray.Enabled = true;
                ucGlueGray_2.Enabled = true;
            }
        }
        private void cBglueChose_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabGlueMode.SelectedIndex = cBglueChose.SelectedIndex;
        }
        private void btnDrawGLUE_Click(object sender, EventArgs e)
        {
            try
            {
                if (!cBGlueQ.Checked && !cBGlueH.Checked)
                {
                    MessageBox.Show("请勾选1、点胶前或2、点胶后区域标示！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                LoadImg.BackColor = Color.WhiteSmoke;
                #region 找圆找角度提示
                //if (SetNum == "8")
                //{
                //    if (hv_ColCenter.Length == 0)
                //    {
                //        tabCircle.SelectedIndex = ((RMode2Check.Checked) ? 1 : 0);
                //        if ((cBCutGQ.Checked || cBCutGH.Checked) & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                //            MessageBox.Show("请先找到圆心和角度！");
                //        else
                //            MessageBox.Show("请先找到圆心！");
                //        return;
                //    }
                //}
                //else
                //{
                //    if (hv_ColCenter.Length == 0)
                //    {
                //        tabCircle.SelectedIndex = ((RMode2Check.Checked) ? 1 : 0);
                //        if (cBCut.Checked & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                //            MessageBox.Show("请先找到圆心和角度！");
                //        else
                //            MessageBox.Show("请先找到圆心！");
                //        return;
                //    }
                //}
                #endregion
                halcon.centerDeg = 720.0;
                HD.GlueOrder = 1;
                string area1 = (cBLocation2.SelectedIndex + 1).ToString();

                HD.BarrelViewMode2("GCCD2-" + area1, hWVision, 7);

                if ((cBGlueQ.Checked & cBCutGQ.Checked & halcon.centerDeg != 720.0) ||
                    (cBGlueH.Checked & cBCutGH.Checked & halcon.centerDeg != 720.0) ||
                    (cBGlueQ.Checked & !cBCutGQ.Checked) || (cBGlueH.Checked & !cBCutGH.Checked))
                {
                    hv_RowCenter = halcon.centerRow;
                    hv_ColCenter = halcon.centerCol;
                    hv_Deg2 = halcon.centerDeg;
                    LoadImg.BackColor = Color.WhiteSmoke;
                    if (cBglueChose.SelectedIndex == 0)
                    {
                        hv_FdegPlus = tBFDegPlus.Value;
                        ShowGlueFang();
                    }
                    if (cBglueChose.SelectedIndex == 1)
                    {
                        hWVision.ClearWindow();
                        hWVision.DispObj(ho_ImageSet);
                        hv_rowFCenter = hv_RowCenter;
                        hv_colFCenter = hv_ColCenter;
                        hv_MidCirRadius = tBglueR.Value;
                        double mid = hv_MidCirRadius;
                        MidCirRadius = (int)mid;
                        RegionWidth = (int)tBglueW.Value;
                        int i = int.Parse(SetNum) - 1;
                        ho_ImageSet.Dispose();
                        HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                        hWVision.ClearWindow();
                        hWVision.DispObj(ho_ImageSet);
                        ShowGlueSector();
                        ShowGlueSectorGray();
                        if (Glue.Glue_Circle_2)
                        {
                            ShowGlueSector_2(Glue.Glue_Circle_OuterRadius_2, Glue.Glue_Circle_InnerRadius_2, Glue.Glue_Circle_StartAngle_2, Glue.Glue_Circle_EndAngle_2);
                            ShowGlueSectorGray_2(Glue.Glue_Circle_Gray_2);
                        }
                        if (cBGlueEdge.Checked)
                        {
                            GERadiusMin = MidCirRadius - RegionWidth / 2 + 2;
                            GERadiusMax = MidCirRadius + RegionWidth / 2 - 2;
                            tBLimitIn.Value = MidCirRadius - RegionWidth / 2 + 2;
                            tBLimitOut.Value = MidCirRadius + RegionWidth / 2 - 2;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("已设置的参数未能得到圆心和角度，请先调整参数找到圆心和角度！");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tabGlueMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBGlueQ.Checked & tabGlueMode.SelectedIndex == 3)
                tabGlueMode.SelectedIndex = 1;
            if (tabGlueMode.SelectedIndex == 3)
                gBtxtValue.Enabled = false;
            else
                gBtxtValue.Enabled = true;
            if (tabGlueMode.SelectedIndex > -1 & tabGlueMode.SelectedIndex < 2)
                cBglueChose.SelectedIndex = tabGlueMode.SelectedIndex;
            cBglueChose.SelectedIndex = tabGlueMode.SelectedIndex == 2 ? 1 : cBglueChose.SelectedIndex;
        }
        #region 方形
        #region Var
        HTuple hv_g1angle = new HTuple(), hv_g1length1 = new HTuple(), hv_g1length2 = new HTuple(), hv_grayDistance = new HTuple();
        HTuple hv_g1RowCh = new HTuple(), hv_g1ColumnCh = new HTuple(), hv_g2RowCh = new HTuple(), hv_g2ColumnCh = new HTuple();
        HTuple hv_g3RowCh = new HTuple(), hv_g3ColumnCh = new HTuple(), hv_g4RowCh = new HTuple(), hv_g4ColumnCh = new HTuple();
        HTuple glueGray = 128;
        HTuple hv_g1Regionarea = new HTuple(), hv_g2Regionarea = new HTuple(), hv_g3Regionarea = new HTuple(), hv_g4Regionarea = new HTuple();
        HTuple hv_g1row = new HTuple(), hv_g2row = new HTuple(), hv_g3row = new HTuple(), hv_g4row = new HTuple();
        HTuple hv_g1col = new HTuple(), hv_g2col = new HTuple(), hv_g3col = new HTuple(), hv_g4col = new HTuple();
        HTuple hv_g1area = 0, hv_g2area = 0, hv_g3area = 0, hv_g4area = 0;
        HTuple hv_GlueRegionArea = 0, hv_GlueArea = 0;

        HObject ho_g1Rectangle = new HObject(), ho_g3Rectangle = new HObject();
        HObject ho_g2Rectangle = new HObject(), ho_g4Rectangle = new HObject();
        HObject ho_g1Region = new HObject(), ho_g3Region = new HObject();
        HObject ho_g2Region = new HObject(), ho_g4Region = new HObject();
        HObject ho_g1Reduced = new HObject(), ho_g3Reduced = new HObject();
        HObject ho_g2Reduced = new HObject(), ho_g4Reduced = new HObject();
        HObject ho_g1Reduced_2 = new HObject(), ho_g3Reduced_2 = new HObject();
        HObject ho_g2Reduced_2 = new HObject(), ho_g4Reduced_2 = new HObject();
        HObject ho_g1Grey = new HObject(), ho_g3Grey = new HObject();
        HObject ho_g2Grey = new HObject(), ho_g4Grey = new HObject();
        HObject ho_g1Grey_2 = new HObject(), ho_g3Grey_2 = new HObject();
        HObject ho_g2Grey_2 = new HObject(), ho_g4Grey_2 = new HObject();
        HObject ho_GlueQ1 = new HObject(), ho_GlueQ2 = new HObject(), ho_GlueQ3 = new HObject(), ho_GlueQ4 = new HObject();
        HObject ho_GlueH1 = new HObject(), ho_GlueH2 = new HObject(), ho_GlueH3 = new HObject(), ho_GlueH4 = new HObject();
        HObject ho_GlueQ1_2 = new HObject(), ho_GlueQ2_2 = new HObject(), ho_GlueQ3_2 = new HObject(), ho_GlueQ4_2 = new HObject();
        HObject ho_GlueH1_2 = new HObject(), ho_GlueH2_2 = new HObject(), ho_GlueH3_2 = new HObject(), ho_GlueH4_2 = new HObject();
        HObject ho_GlueDiff1 = new HObject(), ho_GlueDiff2 = new HObject(), ho_GlueDiff3 = new HObject(), ho_GlueDiff4 = new HObject();

        PointF g2Point, g4Point, cenPoint;
        #endregion
        HObject ho_ImageMean1 = new HObject(), ho_ImageMean = new HObject(), ho_odd_region = new HObject();
        HTuple hv_GrayoffSet = new HTuple(), hv_FdegPlus = new HTuple();
        void ShowGlueFang()
        {
            try
            {
                if (readpara)
                    return;
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                if (!(hv_Deg2 == null || hv_Deg2 == 720.0))
                {
                    #region 基础参数
                    hv_g1angle = (Math.PI / 180) * (hv_Deg2 + hv_FdegPlus - 45);
                    if (tabVisionSet.SelectedIndex == 1)
                    {
                        hv_g1length1 = (HTuple)UDARegionlen1.Value;
                        hv_g1length2 = (HTuple)UDARegionlen2.Value;
                        hv_grayDistance = (HTuple)UDARegionDis.Value;
                        hv_GrayoffSet = (HTuple)UDARegionGray.Value;
                    }
                    if (tabVisionSet.SelectedIndex == 2)
                    {
                        hv_g1length1 = (HTuple)UDgluelen1.Value;
                        hv_g1length2 = (HTuple)UDgluelen2.Value;
                        hv_grayDistance = (HTuple)UDglueDis.Value;
                        glueGray = (HTuple)UDGlueGray.Value;
                    }
                    ho_ImageTest.Dispose();
                    ho_ImageTest = ho_ImageSet;
                    double distance = hv_grayDistance;
                    //double k1 = (hv_ColCenter - hv_g1ColumnCh) * 1.0 / (hv_RowCenter - hv_g1RowCh);// 坐标直线斜率k
                    double k1 = Math.Tan(Math.PI * (hv_Deg2 + hv_FdegPlus) / 180);
                    double k2 = -1.0 / k1;
                    cenPoint.X = hv_RowCenter[0].F; cenPoint.Y = hv_ColCenter[0].F;
                    GetPointXY(cenPoint, distance, k1, ref g2Point, ref g4Point);
                    hv_g1RowCh = g2Point.X;
                    hv_g1ColumnCh = g2Point.Y;
                    hv_g3RowCh = g4Point.X;
                    hv_g3ColumnCh = g4Point.Y;
                    GetPointXY(cenPoint, distance, k2, ref g2Point, ref g4Point);
                    hv_g2RowCh = g2Point.X;
                    hv_g2ColumnCh = g2Point.Y;
                    hv_g4RowCh = g4Point.X;
                    hv_g4ColumnCh = g4Point.Y;
                    #endregion

                    if (tabVisionSet.SelectedIndex == 1)
                    {
                        ho_ImageMean1.Dispose();
                        HOperatorSet.MeanImage(ho_ImageSet, out  ho_ImageMean1, 9, 9);
                        ho_ImageMean.Dispose();
                        HOperatorSet.MeanImage(ho_ImageSet, out  ho_ImageMean, 90, 90);
                        ho_odd_region.Dispose();
                        HOperatorSet.DynThreshold(ho_ImageMean1, ho_ImageMean, out ho_odd_region, hv_GrayoffSet, "not_equal");
                    }

                    HObject ho_g1conn = new HObject(), ho_g1sele = new HObject();
                    HObject ho_g2conn = new HObject(), ho_g2sele = new HObject();
                    HObject ho_g3conn = new HObject(), ho_g3sele = new HObject();
                    HObject ho_g4conn = new HObject(), ho_g4sele = new HObject();

                    #region 区域1
                    ho_g1Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g1Rectangle, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g1Region, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Reduced.Dispose();
                    if (tabVisionSet.SelectedIndex == 1)
                    {
                        HOperatorSet.ReduceDomain(ho_odd_region, ho_g1Region, out ho_g1Reduced);
                        ho_g1conn.Dispose();
                        HOperatorSet.Connection(ho_g1Reduced, out ho_g1conn);
                        ho_g1sele.Dispose();
                        HOperatorSet.SelectShape(ho_g1conn, out ho_g1sele, "area", "and", 50, 999999);
                        ho_g1Reduced.Dispose();
                        HOperatorSet.Union1(ho_g1sele, out ho_g1Reduced);
                    }
                    if (tabVisionSet.SelectedIndex == 2)
                        HOperatorSet.ReduceDomain(ho_ImageTest, ho_g1Region, out ho_g1Reduced);
                    #endregion
                    #region 区域3
                    ho_g3Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g3Rectangle, hv_g3RowCh, hv_g3ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g3Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g3Region, hv_g3RowCh, hv_g3ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g3Reduced.Dispose();
                    if (tabVisionSet.SelectedIndex == 1)
                    {
                        HOperatorSet.ReduceDomain(ho_odd_region, ho_g3Region, out ho_g3Reduced);
                        ho_g3conn.Dispose();
                        HOperatorSet.Connection(ho_g3Reduced, out ho_g3conn);
                        ho_g3sele.Dispose();
                        HOperatorSet.SelectShape(ho_g3conn, out ho_g3sele, "area", "and", 50, 999999);
                        ho_g3Reduced.Dispose();
                        HOperatorSet.Union1(ho_g3sele, out ho_g3Reduced);
                    }
                    if (tabVisionSet.SelectedIndex == 2)
                        HOperatorSet.ReduceDomain(ho_ImageTest, ho_g3Region, out ho_g3Reduced);
                    #endregion
                    #region 区域2
                    ho_g2Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g2Rectangle, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g2Region, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Reduced.Dispose();
                    if (tabVisionSet.SelectedIndex == 1)
                    {
                        HOperatorSet.ReduceDomain(ho_odd_region, ho_g2Region, out ho_g2Reduced);
                        ho_g2conn.Dispose();
                        HOperatorSet.Connection(ho_g2Reduced, out ho_g2conn);
                        ho_g2sele.Dispose();
                        HOperatorSet.SelectShape(ho_g2conn, out ho_g2sele, "area", "and", 50, 999999);
                        ho_g2Reduced.Dispose();
                        HOperatorSet.Union1(ho_g2sele, out ho_g2Reduced);
                    }
                    if (tabVisionSet.SelectedIndex == 2)
                        HOperatorSet.ReduceDomain(ho_ImageTest, ho_g2Region, out ho_g2Reduced);
                    #endregion
                    #region 区域4
                    ho_g4Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g4Rectangle, hv_g4RowCh, hv_g4ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g4Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g4Region, hv_g4RowCh, hv_g4ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g4Reduced.Dispose();
                    if (tabVisionSet.SelectedIndex == 1)
                    {
                        HOperatorSet.ReduceDomain(ho_odd_region, ho_g4Region, out ho_g4Reduced);
                        ho_g4conn.Dispose();
                        HOperatorSet.Connection(ho_g4Reduced, out ho_g4conn);
                        ho_g4sele.Dispose();
                        HOperatorSet.SelectShape(ho_g4conn, out ho_g4sele, "area", "and", 50, 999999);
                        ho_g4Reduced.Dispose();
                        HOperatorSet.Union1(ho_g4sele, out ho_g4Reduced);
                    }
                    if (tabVisionSet.SelectedIndex == 2)
                        HOperatorSet.ReduceDomain(ho_ImageTest, ho_g4Region, out ho_g4Reduced);
                    #endregion

                    if (tabVisionSet.SelectedIndex == 2 & cBGlueQ.Checked)
                    {
                        #region Q
                        ho_GlueQ1.Dispose();
                        HOperatorSet.CopyImage(ho_g1Reduced, out ho_GlueQ1);
                        ho_GlueQ2.Dispose();
                        HOperatorSet.CopyImage(ho_g2Reduced, out ho_GlueQ2);
                        ho_GlueQ3.Dispose();
                        HOperatorSet.CopyImage(ho_g3Reduced, out ho_GlueQ3);
                        ho_GlueQ4.Dispose();
                        HOperatorSet.CopyImage(ho_g4Reduced, out ho_GlueQ4);
                        #endregion
                    }
                    if (tabVisionSet.SelectedIndex == 2 & cBGlueH.Checked)
                    {
                        #region H
                        ho_GlueH1.Dispose();
                        HOperatorSet.CopyImage(ho_g1Reduced, out ho_GlueH1);
                        HOperatorSet.AbsDiffImage(ho_GlueH1, ho_GlueQ1, out ho_GlueDiff1, 4);
                        ho_g1Grey.Dispose();
                        HOperatorSet.Threshold(ho_GlueDiff1, out ho_g1Grey, glueGray, 255);
                        HOperatorSet.AreaCenter(ho_g1Region, out hv_g1Regionarea, out hv_g1row, out hv_g1col);
                        HOperatorSet.AreaCenter(ho_g1Grey, out hv_g1area, out hv_g1row, out hv_g1col);
                        ho_GlueH2.Dispose();
                        HOperatorSet.CopyImage(ho_g2Reduced, out ho_GlueH2);
                        HOperatorSet.AbsDiffImage(ho_GlueH2, ho_GlueQ2, out ho_GlueDiff2, 4);
                        ho_g2Grey.Dispose();
                        HOperatorSet.Threshold(ho_GlueDiff2, out ho_g2Grey, glueGray, 255);
                        HOperatorSet.AreaCenter(ho_g2Region, out hv_g2Regionarea, out hv_g2row, out hv_g2col);
                        HOperatorSet.AreaCenter(ho_g2Grey, out hv_g2area, out hv_g2row, out hv_g2col);
                        ho_GlueH3.Dispose();
                        HOperatorSet.CopyImage(ho_g3Reduced, out ho_GlueH3);
                        HOperatorSet.AbsDiffImage(ho_GlueH3, ho_GlueQ3, out ho_GlueDiff3, 4);
                        ho_g3Grey.Dispose();
                        HOperatorSet.Threshold(ho_GlueDiff3, out ho_g3Grey, glueGray, 255);
                        HOperatorSet.AreaCenter(ho_g3Region, out hv_g3Regionarea, out hv_g3row, out hv_g3col);
                        HOperatorSet.AreaCenter(ho_g3Grey, out hv_g3area, out hv_g3row, out hv_g3col);
                        ho_GlueH4.Dispose();
                        HOperatorSet.CopyImage(ho_g4Reduced, out ho_GlueH4);
                        HOperatorSet.AbsDiffImage(ho_GlueH4, ho_GlueQ4, out ho_GlueDiff4, 4);
                        ho_g4Grey.Dispose();
                        HOperatorSet.Threshold(ho_GlueDiff4, out ho_g4Grey, glueGray, 255);
                        HOperatorSet.AreaCenter(ho_g4Region, out hv_g4Regionarea, out hv_g4row, out hv_g4col);
                        HOperatorSet.AreaCenter(ho_g4Grey, out hv_g4area, out hv_g4row, out hv_g4col);
                        #endregion
                    }

                    hWVision.DispObj(ho_ImageTest);
                    hWVision.SetColor("red");
                    hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                    hWVision.DispObj(ho_g1Rectangle);  //检测区域
                    hWVision.DispObj(ho_g3Rectangle);
                    hWVision.DispObj(ho_g2Rectangle);  //检测区域
                    hWVision.DispObj(ho_g4Rectangle);
                    if (tabVisionSet.SelectedIndex == 1)
                    {
                        HTuple[] hv_Aarea = new HTuple[4] { 0, 0, 0, 0 }, hv_Arow = new HTuple[4], hv_Acol = new HTuple[4];
                        HOperatorSet.AreaCenter(ho_g1Reduced, out hv_Aarea[0], out hv_Arow[0], out hv_Acol[0]);
                        HOperatorSet.AreaCenter(ho_g2Reduced, out hv_Aarea[1], out hv_Arow[1], out hv_Acol[1]);
                        HOperatorSet.AreaCenter(ho_g3Reduced, out hv_Aarea[2], out hv_Arow[2], out hv_Acol[2]);
                        HOperatorSet.AreaCenter(ho_g4Reduced, out hv_Aarea[3], out hv_Arow[3], out hv_Acol[3]);
                        hWVision.DispObj(ho_g1Reduced);
                        hWVision.DispObj(ho_g2Reduced);
                        hWVision.DispObj(ho_g3Reduced);
                        hWVision.DispObj(ho_g4Reduced);
                        HD.set_display_font(hWVision, 18, "sans", "false", "false");
                        HD.disp_message(hWVision, hv_Aarea[0], "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                        HD.disp_message(hWVision, hv_Aarea[1], "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
                        HD.disp_message(hWVision, hv_Aarea[2], "", hv_g3RowCh, hv_g3ColumnCh, "green", "false");
                        HD.disp_message(hWVision, hv_Aarea[3], "", hv_g4RowCh, hv_g4ColumnCh, "green", "false");
                    }
                    else
                    {
                        if (cBGlueH.Checked)
                        {
                            hWVision.DispObj(ho_g1Grey);
                            hWVision.DispObj(ho_g2Grey);
                            hWVision.DispObj(ho_g3Grey);
                            hWVision.DispObj(ho_g4Grey);
                            HD.set_display_font(hWVision, 16, "sans", "false", "false");
                            HD.disp_message(hWVision, "1中标示面积" + Math.Round((double)hv_g1area).ToString() + "(" + Math.Round((double)hv_g1Regionarea).ToString() + ")", "", 100, 150, "green", "false");
                            HD.disp_message(hWVision, "2中标示面积" + Math.Round((double)hv_g2area).ToString() + "(" + Math.Round((double)hv_g2Regionarea).ToString() + ")", "", 200, 150, "green", "false");
                            HD.disp_message(hWVision, "3中标示面积" + Math.Round((double)hv_g3area).ToString() + "(" + Math.Round((double)hv_g3Regionarea).ToString() + ")", "", 300, 150, "green", "false");
                            HD.disp_message(hWVision, "4中标示面积" + Math.Round((double)hv_g4area).ToString() + "(" + Math.Round((double)hv_g4Regionarea).ToString() + ")", "", 400, 150, "green", "false");
                        }
                        HD.set_display_font(hWVision, 18, "sans", "true", "false");
                        HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                        HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
                        HD.disp_message(hWVision, "3", "", hv_g3RowCh, hv_g3ColumnCh, "green", "false");
                        HD.disp_message(hWVision, "4", "", hv_g4RowCh, hv_g4ColumnCh, "green", "false");
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tBFDegPlus_ValueChanged(object sender, EventArgs e)
        {
            UDFDegPlus.Value = tBFDegPlus.Value;
            hv_FdegPlus = tBFDegPlus.Value;
            ShowGlueFang();
        }
        private void UDFDegPlus_ValueChanged(object sender, EventArgs e)
        {
            tBFDegPlus.Value = (int)UDFDegPlus.Value;
        }
        private void tBglueDis_ValueChanged(object sender, EventArgs e)
        {
            double gluedistance = tBglueDis.Value;
            UDglueDis.Value = (int)gluedistance;
            ShowGlueFang();
        }
        private void UDglueDis_ValueChanged(object sender, EventArgs e)
        {
            double gluedistance = (HTuple)UDglueDis.Value;
            tBglueDis.Value = (int)gluedistance;
        }
        private void tBgluelen1_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = tBgluelen1.Value;
            UDgluelen1.Value = (int)gluelen;
            ShowGlueFang();
        }
        private void UDgluelen1_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = (HTuple)UDgluelen1.Value;
            tBgluelen1.Value = (int)gluelen;
        }
        private void tBgluelen2_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = tBgluelen2.Value;
            UDgluelen2.Value = (int)gluelen;
            ShowGlueFang();
        }
        private void UDgluelen2_ValueChanged(object sender, EventArgs e)
        {
            double gluelen = (HTuple)UDgluelen2.Value;
            tBgluelen2.Value = (int)gluelen;
        }
        #endregion
        #region 环形
        #region 变量
        HTuple hv_startPhi1 = new HTuple(), hv_endPhi1 = new HTuple();
        HTuple hv_rowF0 = new HTuple(), hv_colF0 = new HTuple(), hv_rowF01 = new HTuple(), hv_colF01 = new HTuple(), hv_g1Area = new HTuple(), hv_g2Area = new HTuple();
        HObject ho_ContCircleOut1 = new HObject(), ho_Contour0 = new HObject();
        HObject ho_ContoursUnion0 = new HObject(), ho_RegionS0 = new HObject();
        HObject ho_ContCircleIn1 = new HObject(), ho_Contour01 = new HObject();
        HObject ho_ContoursUnion01 = new HObject(), ho_RegionS01 = new HObject();
        HObject ho_RegionDifference0 = new HObject(), ho_RegionDifference0_2 = new HObject(), ho_R0 = new HObject();
        HObject ho_RegionD0 = new HObject();
        HObject ho_GrayDiff1 = new HObject(), ho_GrayDiff2 = new HObject(), ho_DiffCirIn = new HObject(), ho_DiffCirOut = new HObject();
        HObject ho_XYTransImage = new HObject(), ho_Glue_Test_Region = new HObject();
        HTuple GERadiusMin = new HTuple(), GERadiusMax = new HTuple();
        HTuple hv_diffI1Area = new HTuple(), hv_diffI2Area = new HTuple(), hv_diffRow = new HTuple(), hv_diffCol = new HTuple();
        HTuple hv_diffO1Area = new HTuple(), hv_diffO2Area = new HTuple(), hv_diffIArea = new HTuple(), hv_diffOArea = new HTuple();

        #endregion
        ////旋轉圖片
        //void RotateImage(HObject ho_Image_Befort, out HObject ho_Image_Befort_2, HTuple hv_Row_Befort, HTuple hv_Column_Befort, HTuple hv_ResultRow_Befort, HTuple hv_ResultColumn_Befort,
        //HTuple hv_Row_After, HTuple hv_Column_After, HTuple hv_ResultRow_After, HTuple hv_ResultColumn_After, out HTuple hv_ResultRow_Befort2, out HTuple hv_ResultColumn_Befort2)
        //{
        //    HObject ho_UsedEdges_Befort = new HObject(), ho_Contour_Befort = new HObject(), ho_ResultContours_Befort = new HObject(), ho_CrossCenter_Befort = new HObject();
        //    ho_Image_Befort_2 = null;
        //    HTuple hv_ResultRadius_Befort = 0;
        //    hv_ResultRow_Befort2 = hv_ResultColumn_Befort2 = 0;
        //    try
        //    {
        //        //旋轉膠前圖
        //        HOperatorSet.AngleLl(hv_Row_Befort, hv_Column_Befort, hv_ResultRow_Befort, hv_ResultColumn_Befort,
        //            hv_Row_After, hv_Column_After, hv_ResultRow_After, hv_ResultColumn_After,
        //            out hv_Angle);
        //        HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
        //        //hom_mat2d_translate (HomMat2D, ResultColumn_After-ResultColumn_Befort, ResultRow_After-ResultRow_Befort, HomMat2D)
        //        HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_Angle, hv_ResultRow_Befort, hv_ResultColumn_Befort,
        //            out hv_HomMat2D);
        //        HOperatorSet.AffineTransImage(ho_Image_Befort, out ho_Image_Befort_2, hv_HomMat2D,
        //            "constant", "false");

        //        //旋轉完要重新抓圓否則圓心會偏移
        //        ho_UsedEdges_Befort.Dispose(); ho_Contour_Befort.Dispose(); ho_ResultContours_Befort.Dispose(); ho_CrossCenter_Befort.Dispose();
        //        HD.gen_circle_center(ho_Image_Befort_2, out ho_UsedEdges_Befort, out ho_Contour_Befort,
        //            out ho_ResultContours_Befort, out ho_CrossCenter_Befort, 974, 1289, 677,
        //            30, 60, "positive", "last", out hv_ResultRow_Befort2, out hv_ResultColumn_Befort2,
        //            out hv_ResultRadius_Befort);

        //    }
        //    catch
        //    {
        //    }
        //}
        //檢測膠水
        //void GlueTest(HObject ho_Image_Befort, HObject ho_Image_After, out HObject ho_Region, HTuple hv_ResultRow_Befort,
        //    HTuple hv_ResultColumn_Befort, HTuple hv_ResultRow_After, HTuple hv_ResultColumn_After,
        //    HTuple hv_OuterRadius, HTuple hv_InnerRadius,HTuple hv_Gray)
        //{
        //    ho_Region = null;
        //    try
        //    {
        //        ho_XYTransImage.Dispose();
        //        HD.polar_trans_and_mean_and_abs_diff(ho_Image_Befort, ho_Image_After, out ho_XYTransImage,
        //            hv_ResultRow_Befort, hv_ResultColumn_Befort, hv_ResultRow_After, hv_ResultColumn_After,
        //            hv_OuterRadius, hv_InnerRadius, 3, 5, 1);
        //        HOperatorSet.Threshold(ho_XYTransImage, out ho_Region, hv_Gray, 255);
        //    }
        //    catch
        //    {
        //    }
        //}
        void ShowGlueSector()
        {
            if (readpara)
                return;
            int i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            if (cBGlueQ.Checked)
            {
                HOperatorSet.CopyImage(halcon.Image[i], out ho_GlueImage_Befort);
            }
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            hv_startPhi = (StartAngle * 3.14159) / 180;
            hv_endPhi = (EndAngle * 3.14159) / 180;
            hv_startPhi1 = ((StartAngle + 180) * 3.14159) / 180;
            hv_endPhi1 = ((EndAngle + 180) * 3.14159) / 180;
            #region //创建外扇形
            HOperatorSet.GenEmptyObj(out ho_ContCircleOut);
            HOperatorSet.GenCircleContourXld(out ho_ContCircleOut, hv_RowCenter, hv_ColCenter,
                 MidCirRadius + RegionWidth / 2, hv_startPhi, hv_endPhi, "positive", 1);  //hv_ColCenter  hv_RowCenter
            hv_rowF = new HTuple();
            hv_rowF[0] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_startPhi.TupleSin()));
            hv_rowF[1] = hv_RowCenter;
            hv_rowF[2] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_endPhi.TupleSin()));
            hv_colF = new HTuple();
            hv_colF[0] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_startPhi.TupleCos()));
            hv_colF[1] = hv_ColCenter;
            hv_colF[2] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_endPhi.TupleCos()));
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_rowF, hv_colF);
            if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
            {
                ho_ContoursUnion.Dispose();
                HOperatorSet.Union2ClosedContoursXld(ho_ContCircleOut, ho_Contour, out ho_ContoursUnion);
            }
            else if ((EndAngle - StartAngle) < 360)
            {
                ho_ContoursUnion.Dispose();
                HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleOut, ho_Contour, out ho_ContoursUnion);
            }
            ho_RegionS.Dispose();
            HOperatorSet.GenRegionContourXld(ho_ContoursUnion, out ho_RegionS, "filled");
            #endregion
            #region //创建内扇形
            ho_ContCircleIn.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircleIn, hv_RowCenter, hv_ColCenter,
                (MidCirRadius - RegionWidth / 2), hv_startPhi, hv_endPhi, "positive", 1);
            hv_rowF1 = new HTuple();
            hv_rowF1[0] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_startPhi.TupleSin()));
            hv_rowF1[1] = hv_RowCenter;
            hv_rowF1[2] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_endPhi.TupleSin()));
            hv_colF1 = new HTuple();
            hv_colF1[0] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_startPhi.TupleCos()));
            hv_colF1[1] = hv_ColCenter;
            hv_colF1[2] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_endPhi.TupleCos()));
            ho_Contour1.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour1, hv_rowF1, hv_colF1);
            if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
            {
                ho_ContoursUnion1.Dispose();
                HOperatorSet.Union2ClosedContoursXld(ho_ContCircleIn, ho_Contour1, out ho_ContoursUnion1);
            }
            else if ((EndAngle - StartAngle) < 360)
            {
                ho_ContoursUnion1.Dispose();
                HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleIn, ho_Contour1, out ho_ContoursUnion1);
            }
            ho_RegionS1.Dispose();
            HOperatorSet.GenRegionContourXld(ho_ContoursUnion1, out ho_RegionS1, "filled");
            #endregion
            #region //生产扇形环
            if ((EndAngle - StartAngle) < 360)
            {
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_RegionS, ho_RegionS1, out ho_RegionDifference);
            }
            else
            {
                ho_RegionS.Dispose();
                HOperatorSet.GenCircle(out ho_RegionS, hv_RowCenter, hv_ColCenter, MidCirRadius + RegionWidth / 2);
                ho_RegionS1.Dispose();
                HOperatorSet.GenCircle(out ho_RegionS1, hv_RowCenter, hv_ColCenter, MidCirRadius - RegionWidth / 2);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_RegionS, ho_RegionS1, out ho_RegionDifference);
                ho_R.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionS1, out ho_R, "border");
            }
            HOperatorSet.GenContourRegionXld(ho_RegionDifference, out ho_RegionD, "border");
            #endregion

            #region //创建外扇形
            ho_ContCircleOut1.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircleOut1, hv_RowCenter, hv_ColCenter,
                 MidCirRadius + RegionWidth / 2, hv_startPhi1, hv_endPhi1, "positive", 1);
            hv_rowF0 = new HTuple();
            hv_rowF0[0] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_startPhi1.TupleSin()));
            hv_rowF0[1] = hv_RowCenter;
            hv_rowF0[2] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_endPhi1.TupleSin()));
            hv_colF0 = new HTuple();
            hv_colF0[0] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_startPhi1.TupleCos()));
            hv_colF0[1] = hv_ColCenter;
            hv_colF0[2] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_endPhi1.TupleCos()));
            ho_Contour0.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour0, hv_rowF0, hv_colF0);
            if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
            {
                ho_ContoursUnion0.Dispose();
                HOperatorSet.Union2ClosedContoursXld(ho_ContCircleOut1, ho_Contour0, out ho_ContoursUnion0);
            }
            else if ((EndAngle - StartAngle) < 360)
            {
                ho_ContoursUnion0.Dispose();
                HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleOut1, ho_Contour0, out ho_ContoursUnion0);
            }
            ho_RegionS0.Dispose();
            HOperatorSet.GenRegionContourXld(ho_ContoursUnion0, out ho_RegionS0, "filled");
            #endregion
            #region //创建内扇形
            ho_ContCircleIn1.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircleIn1, hv_RowCenter, hv_ColCenter,
                (MidCirRadius - RegionWidth / 2), hv_startPhi1, hv_endPhi1, "positive", 1);
            hv_rowF01 = new HTuple();
            hv_rowF01[0] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_startPhi1.TupleSin()));
            hv_rowF01[1] = hv_RowCenter;
            hv_rowF01[2] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_endPhi1.TupleSin()));
            hv_colF01 = new HTuple();
            hv_colF01[0] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_startPhi1.TupleCos()));
            hv_colF01[1] = hv_ColCenter;
            hv_colF01[2] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_endPhi1.TupleCos()));
            ho_Contour01.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour01, hv_rowF01, hv_colF01);
            if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
            {
                ho_ContoursUnion01.Dispose();
                HOperatorSet.Union2ClosedContoursXld(ho_ContCircleIn1, ho_Contour01, out ho_ContoursUnion01);
            }
            else if ((EndAngle - StartAngle) < 360)
            {
                ho_ContoursUnion01.Dispose();
                HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleIn1, ho_Contour01, out ho_ContoursUnion01);
            }
            ho_RegionS01.Dispose();
            HOperatorSet.GenRegionContourXld(ho_ContoursUnion01, out ho_RegionS01, "filled");
            #endregion
            #region //生产扇形环
            if ((EndAngle - StartAngle) < 360)
            {
                ho_RegionDifference0.Dispose();
                HOperatorSet.Difference(ho_RegionS0, ho_RegionS01, out ho_RegionDifference0);
            }
            else
            {
                ho_RegionS0.Dispose();
                HOperatorSet.GenCircle(out ho_RegionS0, hv_RowCenter, hv_ColCenter, MidCirRadius + RegionWidth / 2);
                ho_RegionS01.Dispose();
                HOperatorSet.GenCircle(out ho_RegionS01, hv_RowCenter, hv_ColCenter, MidCirRadius - RegionWidth / 2);
                ho_RegionDifference0.Dispose();
                HOperatorSet.Difference(ho_RegionS0, ho_RegionS01, out ho_RegionDifference0);
                ho_R0.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionS01, out ho_R0, "border");
            }
            HOperatorSet.GenContourRegionXld(ho_RegionDifference0, out ho_RegionD0, "border");
            #endregion

            #region 区域1
            ho_g1Rectangle.Dispose();
            HOperatorSet.GenContourRegionXld(ho_RegionDifference, out ho_g1Rectangle, "border");
            ho_g1Reduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_g1Reduced);
            HOperatorSet.AreaCenter(ho_g1Reduced, out hv_g1Area, out hv_g1RowCh, out hv_g1ColumnCh);
            #endregion
            #region 区域2
            ho_g2Rectangle.Dispose();
            HOperatorSet.GenContourRegionXld(ho_RegionDifference0, out ho_g2Rectangle, "border");
            ho_g2Reduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference0, out ho_g2Reduced);
            HOperatorSet.AreaCenter(ho_g2Reduced, out hv_g2Area, out hv_g2RowCh, out hv_g2ColumnCh);
            #endregion

            //显示最终结果
            //hWVision.ClearWindow();
            //hWVision.DispObj(ho_ImageSet);
            if ((EndAngle - StartAngle) >= 360)
            {
                hWVision.DispObj(ho_R);
                hWVision.DispObj(ho_R0);
            }
            hWVision.DispObj(ho_RegionD);
            hWVision.DispObj(ho_RegionD0);
        }

        void ShowGlueSector_2(double OuterRadius, double InnerRadius, double StartAngle, double EndAngle)
        {
            try
            {
                if (hv_RowCenter.D == 0 || hv_colFCenter.D == 0)
                {
                    MessageBox.Show("請先抓圓心!");
                    return;
                }

                HTuple MidCirRadius = (OuterRadius + InnerRadius) / 2;
                HTuple RegionWidth = OuterRadius - InnerRadius;
                if (readpara)
                    return;
                int i = int.Parse(SetNum) - 1;
                if (halcon.Image[i] == null)
                    return;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                hv_startPhi = (StartAngle * Math.PI) / 180;
                hv_endPhi = (EndAngle * Math.PI) / 180;
                hv_startPhi1 = ((StartAngle + 180) * Math.PI) / 180;
                hv_endPhi1 = ((EndAngle + 180) * Math.PI) / 180;
                #region //创建外扇形
                HOperatorSet.GenEmptyObj(out ho_ContCircleOut);
                HOperatorSet.GenCircleContourXld(out ho_ContCircleOut, hv_RowCenter, hv_colFCenter,
                     MidCirRadius + RegionWidth / 2, hv_startPhi, hv_endPhi, "positive", 1);  //hv_ColCenter  hv_RowCenter
                hv_rowF = new HTuple();
                hv_rowF[0] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_startPhi.TupleSin()));
                hv_rowF[1] = hv_RowCenter;
                hv_rowF[2] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_endPhi.TupleSin()));
                hv_colF = new HTuple();
                hv_colF[0] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_startPhi.TupleCos()));
                hv_colF[1] = hv_ColCenter;
                hv_colF[2] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_endPhi.TupleCos()));
                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_rowF, hv_colF);
                if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
                {
                    ho_ContoursUnion.Dispose();
                    HOperatorSet.Union2ClosedContoursXld(ho_ContCircleOut, ho_Contour, out ho_ContoursUnion);
                }
                else if ((EndAngle - StartAngle) < 360)
                {
                    ho_ContoursUnion.Dispose();
                    HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleOut, ho_Contour, out ho_ContoursUnion);
                }
                ho_RegionS.Dispose();
                HOperatorSet.GenRegionContourXld(ho_ContoursUnion, out ho_RegionS, "filled");
                #endregion
                #region //创建内扇形
                ho_ContCircleIn.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircleIn, hv_RowCenter, hv_colFCenter,
                    (MidCirRadius - RegionWidth / 2), hv_startPhi, hv_endPhi, "positive", 1);
                hv_rowF1 = new HTuple();
                hv_rowF1[0] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_startPhi.TupleSin()));
                hv_rowF1[1] = hv_RowCenter;
                hv_rowF1[2] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_endPhi.TupleSin()));
                hv_colF1 = new HTuple();
                hv_colF1[0] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_startPhi.TupleCos()));
                hv_colF1[1] = hv_ColCenter;
                hv_colF1[2] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_endPhi.TupleCos()));
                ho_Contour1.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour1, hv_rowF1, hv_colF1);
                if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
                {
                    ho_ContoursUnion1.Dispose();
                    HOperatorSet.Union2ClosedContoursXld(ho_ContCircleIn, ho_Contour1, out ho_ContoursUnion1);
                }
                else if ((EndAngle - StartAngle) < 360)
                {
                    ho_ContoursUnion1.Dispose();
                    HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleIn, ho_Contour1, out ho_ContoursUnion1);
                }
                ho_RegionS1.Dispose();
                HOperatorSet.GenRegionContourXld(ho_ContoursUnion1, out ho_RegionS1, "filled");
                #endregion
                #region //生产扇形环
                if ((EndAngle - StartAngle) < 360)
                {
                    ho_RegionDifference_2.Dispose();
                    HOperatorSet.Difference(ho_RegionS, ho_RegionS1, out ho_RegionDifference_2);
                }
                else
                {
                    ho_RegionS.Dispose();
                    HOperatorSet.GenCircle(out ho_RegionS, hv_RowCenter, hv_colFCenter, MidCirRadius + RegionWidth / 2);
                    ho_RegionS1.Dispose();
                    HOperatorSet.GenCircle(out ho_RegionS1, hv_RowCenter, hv_colFCenter, MidCirRadius - RegionWidth / 2);
                    ho_RegionDifference_2.Dispose();
                    HOperatorSet.Difference(ho_RegionS, ho_RegionS1, out ho_RegionDifference_2);
                    ho_R.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionS1, out ho_R, "border");
                }
                HOperatorSet.GenContourRegionXld(ho_RegionDifference_2, out ho_RegionD, "border");
                #endregion

                #region //创建外扇形
                ho_ContCircleOut1.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircleOut1, hv_RowCenter, hv_colFCenter,
                     MidCirRadius + RegionWidth / 2, hv_startPhi1, hv_endPhi1, "positive", 1);
                hv_rowF0 = new HTuple();
                hv_rowF0[0] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_startPhi1.TupleSin()));
                hv_rowF0[1] = hv_RowCenter;
                hv_rowF0[2] = hv_RowCenter - ((MidCirRadius + RegionWidth / 2) * (hv_endPhi1.TupleSin()));
                hv_colF0 = new HTuple();
                hv_colF0[0] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_startPhi1.TupleCos()));
                hv_colF0[1] = hv_ColCenter;
                hv_colF0[2] = hv_ColCenter + ((MidCirRadius + RegionWidth / 2) * (hv_endPhi1.TupleCos()));
                ho_Contour0.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour0, hv_rowF0, hv_colF0);
                if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
                {
                    ho_ContoursUnion0.Dispose();
                    HOperatorSet.Union2ClosedContoursXld(ho_ContCircleOut1, ho_Contour0, out ho_ContoursUnion0);
                }
                else if ((EndAngle - StartAngle) < 360)
                {
                    ho_ContoursUnion0.Dispose();
                    HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleOut1, ho_Contour0, out ho_ContoursUnion0);
                }
                ho_RegionS0.Dispose();
                HOperatorSet.GenRegionContourXld(ho_ContoursUnion0, out ho_RegionS0, "filled");
                #endregion
                #region //创建内扇形
                ho_ContCircleIn1.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircleIn1, hv_RowCenter, hv_colFCenter,
                    (MidCirRadius - RegionWidth / 2), hv_startPhi1, hv_endPhi1, "positive", 1);
                hv_rowF01 = new HTuple();
                hv_rowF01[0] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_startPhi1.TupleSin()));
                hv_rowF01[1] = hv_RowCenter;
                hv_rowF01[2] = hv_RowCenter - ((MidCirRadius - RegionWidth / 2) * (hv_endPhi1.TupleSin()));
                hv_colF01 = new HTuple();
                hv_colF01[0] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_startPhi1.TupleCos()));
                hv_colF01[1] = hv_ColCenter;
                hv_colF01[2] = hv_ColCenter + ((MidCirRadius - RegionWidth / 2) * (hv_endPhi1.TupleCos()));
                ho_Contour01.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour01, hv_rowF01, hv_colF01);
                if ((EndAngle - StartAngle) < 180 && (EndAngle - StartAngle) > 0)
                {
                    ho_ContoursUnion01.Dispose();
                    HOperatorSet.Union2ClosedContoursXld(ho_ContCircleIn1, ho_Contour01, out ho_ContoursUnion01);
                }
                else if ((EndAngle - StartAngle) < 360)
                {
                    ho_ContoursUnion01.Dispose();
                    HOperatorSet.DifferenceClosedContoursXld(ho_ContCircleIn1, ho_Contour01, out ho_ContoursUnion01);
                }
                ho_RegionS01.Dispose();
                HOperatorSet.GenRegionContourXld(ho_ContoursUnion01, out ho_RegionS01, "filled");
                #endregion
                #region //生产扇形环
                if ((EndAngle - StartAngle) < 360)
                {
                    ho_RegionDifference0_2.Dispose();
                    HOperatorSet.Difference(ho_RegionS0, ho_RegionS01, out ho_RegionDifference0_2);
                }
                else
                {
                    ho_RegionS0.Dispose();
                    HOperatorSet.GenCircle(out ho_RegionS0, hv_RowCenter, hv_ColCenter, MidCirRadius + RegionWidth / 2);
                    ho_RegionS01.Dispose();
                    HOperatorSet.GenCircle(out ho_RegionS01, hv_RowCenter, hv_ColCenter, MidCirRadius - RegionWidth / 2);
                    ho_RegionDifference0_2.Dispose();
                    HOperatorSet.Difference(ho_RegionS0, ho_RegionS01, out ho_RegionDifference0_2);
                    ho_R0.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionS01, out ho_R0, "border");
                }
                HOperatorSet.GenContourRegionXld(ho_RegionDifference0_2, out ho_RegionD0, "border");
                #endregion

                #region 区域1
                ho_g1Rectangle.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionDifference_2, out ho_g1Rectangle, "border");
                ho_g1Reduced_2.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference_2, out ho_g1Reduced_2);
                HOperatorSet.AreaCenter(ho_g1Reduced_2, out hv_g1Area, out hv_g1RowCh, out hv_g1ColumnCh);
                #endregion
                #region 区域2
                ho_g2Rectangle.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionDifference0_2, out ho_g2Rectangle, "border");
                ho_g2Reduced_2.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference0_2, out ho_g2Reduced_2);
                HOperatorSet.AreaCenter(ho_g2Reduced_2, out hv_g2Area, out hv_g2RowCh, out hv_g2ColumnCh);
                #endregion

                //显示最终结果
                //hWVision.ClearWindow();
                //hWVision.DispObj(ho_ImageSet);
                if ((EndAngle - StartAngle) >= 360)
                {
                    hWVision.DispObj(ho_R);
                    hWVision.DispObj(ho_R0);
                }
                hWVision.SetColor("orange");
                hWVision.DispObj(ho_RegionD);
                hWVision.DispObj(ho_RegionD0);
            }
            catch
            {
                return;
            }
        }
        void ShowGlueSectorGray()
        {
            if (readpara)
                return;
            int i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            hWVision.SetColor("red");
            if (!(hv_Deg2 == null || hv_Deg2 == 720.0) || !cBCutGH.Checked)
            {
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                #region 区域1
                ho_g1Rectangle.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionDifference, out ho_g1Rectangle, "border");
                ho_g1Reduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageTest, ho_RegionDifference, out ho_g1Reduced);
                HOperatorSet.AreaCenter(ho_g1Reduced, out hv_g1Area, out hv_g1RowCh, out hv_g1ColumnCh);
                #endregion
                #region 区域2
                ho_g2Rectangle.Dispose();
                HOperatorSet.GenContourRegionXld(ho_RegionDifference0, out ho_g2Rectangle, "border");
                ho_g2Reduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageTest, ho_RegionDifference0, out ho_g2Reduced);
                HOperatorSet.AreaCenter(ho_g2Reduced, out hv_g2Area, out hv_g2RowCh, out hv_g2ColumnCh);
                #endregion

                if (cBGlueQ.Checked)
                {
                    ho_GlueQ1.Dispose();
                    HOperatorSet.CopyImage(ho_g1Reduced, out ho_GlueQ1);
                    ho_GlueQ2.Dispose();
                    HOperatorSet.CopyImage(ho_g2Reduced, out ho_GlueQ2);
                }
                if (cBGlueH.Checked)
                {
                    //RotateImage()
                    if (Glue.Glue_Follow)
                    {
                        try
                        {
                            HTuple RowCut_Befort = new HTuple(), RowCut_After = new HTuple();
                            
                            if (!cBCutGQ.Checked && !cBCutGH.Checked)
                            {
                                hv_RowCut_Befort = hv_RowCenter_Befort;
                                hv_ColumnCut_Befort = hv_ColumnCenter_Befort;
                                hv_RowCut_After = hv_RowCenter_After;
                                hv_ColumnCut_After = hv_ColumnCenter_After;
                            }
                            else if (cBCutGQ.Checked && !cBCutGH.Checked)
                            {
                                hv_RowCut_After = hv_RowCut_Befort;
                                hv_ColumnCut_After = hv_ColumnCut_Befort;
                            }
                            else if (!cBCutGQ.Checked && cBCutGH.Checked)
                            {
                                hv_RowCut_Befort = hv_RowCut_After;
                                hv_ColumnCut_Befort = hv_ColumnCut_After;
                            }
                            HOperatorSet.TupleLength(hv_RowCut_Befort, out RowCut_Befort);
                            HOperatorSet.TupleLength(hv_RowCut_After, out RowCut_After);
                            if (RowCut_Befort.I == 0)
                            {
                                MessageBox.Show("未取得膠前角度,需先求膠前角度!");
                                return;
                            }
                            if (RowCut_After.I == 0)
                            {
                                MessageBox.Show("未取得膠後角度,需先求膠後角度!");
                                return;
                            }
                            HOperatorSet.GenEmptyObj(out ho_GlueImage_Befort_2);
                            HOperatorSet.GenEmptyObj(out ho_GlueRegion);
                            //旋轉膠前圖
                            ho_GlueImage_Befort_2.Dispose();
                            HD.RotateImage(ho_GlueImage_Befort, out ho_GlueImage_Befort_2, hv_RowCut_Befort, hv_ColumnCut_Befort, hv_RowCenter_Befort, hv_ColumnCenter_Befort,
                                hv_RowCut_After, hv_ColumnCut_After, hv_RowCenter_After, hv_ColumnCenter_After, hv_RingRadius, hv_RDetectHeight, hv_RAmplitudeThreshold, hv_transition, out hv_RowCenter_Befort_2, out hv_ColumnCenter_Befort_2);
                            //檢測膠水
                            ho_GlueRegion.Dispose();
                            HD.GlueTest(ho_GlueImage_Befort_2, ho_ImageTest, out ho_GlueRegion,
                                hv_RowCenter_Befort_2, hv_ColumnCenter_Befort_2, hv_RowCenter_After, hv_ColumnCenter_After, MidCirRadius + RegionWidth / 2, MidCirRadius - RegionWidth / 2, glueGray);
                            HOperatorSet.AreaCenter(ho_GlueRegion, out hv_GlueArea, out hv_g2row, out hv_g2col);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {

                        ho_GlueH1.Dispose();
                        HOperatorSet.CopyImage(ho_g1Reduced, out ho_GlueH1);
                        HOperatorSet.AbsDiffImage(ho_GlueH1, ho_GlueQ1, out ho_GlueDiff1, 4);
                        ho_g1Grey.Dispose();
                        HOperatorSet.Threshold(ho_GlueDiff1, out ho_g1Grey, glueGray, 255);
                        HOperatorSet.AreaCenter(ho_RegionDifference, out hv_g1Regionarea, out hv_g1row, out hv_g1col);
                        HOperatorSet.AreaCenter(ho_g1Grey, out hv_g1area, out hv_g1row, out hv_g1col);

                        ho_GlueH2.Dispose();
                        HOperatorSet.CopyImage(ho_g2Reduced, out ho_GlueH2);
                        HOperatorSet.AbsDiffImage(ho_GlueH2, ho_GlueQ2, out ho_GlueDiff2, 4);
                        ho_g2Grey.Dispose();
                        HOperatorSet.Threshold(ho_GlueDiff2, out ho_g2Grey, glueGray, 255);
                        HOperatorSet.AreaCenter(ho_RegionDifference0, out hv_g2Regionarea, out hv_g2row, out hv_g2col);
                        HOperatorSet.AreaCenter(ho_g2Grey, out hv_g2area, out hv_g2row, out hv_g2col);
                        ho_GlueRegion.Dispose();
                        HOperatorSet.Union2(ho_g1Grey, ho_g2Grey, out ho_GlueRegion);
                        
                        hv_GlueRegionArea = hv_g1Regionarea + hv_g2Regionarea;
                        hv_GlueArea = hv_g1area + hv_g2area;
                    }
                    ho_gGreyUnion.Dispose();
                    ho_gGreyUnion = ho_GlueRegion.CopyObj(1, -1);
                }
                //hWVision.DispObj(ho_ImageTest);
                if (cBGlueQ.Checked)
                {
                    hWVision.SetColor("red");
                    hWVision.SetLineWidth(1);
                    hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                    hWVision.SetDraw("margin");

                    hWVision.DispObj(ho_g1Rectangle);  //检测区域
                    hWVision.DispObj(ho_g2Rectangle);
                }
                if (cBGlueH.Checked)
                {
                    //hWVision.DispObj(ho_ImageTest);
                    hWVision.SetDraw("fill");
                    hWVision.DispObj(ho_GlueRegion);
                    HD.set_display_font(hWVision, 16, "sans", "false", "false");
                    HD.disp_message(hWVision, "總面积" + Math.Round(hv_GlueArea.D).ToString()/*+ "(" + Math.Round(hv_GlueRegionArea.D).ToString() + ")"*/, "", 100, 150, "green", "false");
                    //HD.disp_message(hWVision, "1中标示面积" + Math.Round((double)hv_g1area).ToString() + "(" + Math.Round((double)hv_g1Regionarea).ToString() + ")", "", 100, 150, "green", "false");
                    //HD.disp_message(hWVision, "2中标示面积" + Math.Round((double)hv_g2area).ToString() + "(" + Math.Round((double)hv_g2Regionarea).ToString() + ")", "", 200, 150, "green", "false");
                }
                //HD.set_display_font(hWVision, 18, "sans", "true", "false");
                //HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                //HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
            }
        }
        //環形區域二
        void ShowGlueSectorGray_2(HTuple glueGray_2)
        {
            try
            {
                if (readpara)
                    return;
                int i = int.Parse(SetNum) - 1;
                if (halcon.Image[i] == null)
                    return;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                hWVision.SetColor("orange");
                if (!(hv_Deg2 == null || hv_Deg2 == 720.0) || !cBCutGH.Checked)
                {
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                    #region 区域1
                    ho_g1Rectangle.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionDifference_2, out ho_g1Rectangle, "border");
                    ho_g1Reduced_2.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageTest, ho_RegionDifference_2, out ho_g1Reduced_2);
                    HOperatorSet.AreaCenter(ho_g1Reduced_2, out hv_g1Area, out hv_g1RowCh, out hv_g1ColumnCh);
                    #endregion
                    #region 区域2
                    ho_g2Rectangle.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionDifference0_2, out ho_g2Rectangle, "border");
                    ho_g2Reduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageTest, ho_RegionDifference0_2, out ho_g2Reduced_2);
                    HOperatorSet.AreaCenter(ho_g2Reduced_2, out hv_g2Area, out hv_g2RowCh, out hv_g2ColumnCh);
                    #endregion

                    if (cBGlueQ.Checked)
                    {
                        ho_GlueQ1_2.Dispose();
                        HOperatorSet.CopyImage(ho_g1Reduced_2, out ho_GlueQ1_2);
                        ho_GlueQ2_2.Dispose();
                        HOperatorSet.CopyImage(ho_g2Reduced_2, out ho_GlueQ2_2);
                    }
                    if (cBGlueH.Checked)
                    {
                        //RotateImage()
                        if (Glue.Glue_Follow)
                        {
                            try
                            {
                                HTuple RowCut_Befort = new HTuple(), RowCut_After = new HTuple();
                                HOperatorSet.TupleLength(hv_RowCut_Befort, out RowCut_Befort);
                                HOperatorSet.TupleLength(hv_RowCut_After, out RowCut_After);
                                if (RowCut_Befort.I == 0)
                                {
                                    MessageBox.Show("未取得膠前角度,需先求膠前角度!");
                                    return;
                                }
                                if (RowCut_After.I == 0)
                                {
                                    MessageBox.Show("未取得膠後角度,需先求膠後角度!");
                                    return;
                                }
                                //旋轉膠前圖
                                ho_GlueImage_Befort_2.Dispose();
                                HD.RotateImage(ho_GlueImage_Befort, out ho_GlueImage_Befort_2, hv_RowCut_Befort, hv_ColumnCut_Befort, hv_RowCenter_Befort, hv_ColumnCenter_Befort,
                                    hv_RowCut_After, hv_ColumnCut_After, hv_RowCenter_After, hv_ColumnCenter_After, hv_RingRadius, hv_RDetectHeight, hv_RAmplitudeThreshold, hv_transition, out hv_RowCenter_Befort_2, out hv_ColumnCenter_Befort_2);
                                //檢測膠水
                                ho_GlueRegion_2.Dispose();
                                HD.GlueTest(ho_GlueImage_Befort_2, ho_ImageTest, out ho_GlueRegion_2,
                                    hv_RowCenter_Befort_2, hv_ColumnCenter_Befort_2, hv_RowCenter_After, hv_ColumnCenter_After, Glue.Glue_Circle_OuterRadius_2, Glue.Glue_Circle_InnerRadius_2, Glue.Glue_Circle_Gray_2);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            ho_GlueH1_2.Dispose();
                            HOperatorSet.CopyImage(ho_g1Reduced_2, out ho_GlueH1_2);
                            HOperatorSet.AbsDiffImage(ho_GlueH1_2, ho_GlueQ1_2, out ho_GlueDiff1, 4);
                            ho_g1Grey_2.Dispose();
                            HOperatorSet.Threshold(ho_GlueDiff1, out ho_g1Grey_2, glueGray_2, 255);
                            //HOperatorSet.AreaCenter(ho_RegionDifference_2, out hv_g1Regionarea, out hv_g1row, out hv_g1col);
                            HOperatorSet.AreaCenter(ho_g1Grey_2, out hv_g1area, out hv_g1row, out hv_g1col);

                            ho_GlueH2_2.Dispose();
                            HOperatorSet.CopyImage(ho_g2Reduced_2, out ho_GlueH2_2);
                            HOperatorSet.AbsDiffImage(ho_GlueH2_2, ho_GlueQ2_2, out ho_GlueDiff2, 4);
                            ho_g2Grey_2.Dispose();
                            HOperatorSet.Threshold(ho_GlueDiff2, out ho_g2Grey_2, glueGray_2, 255);
                            //HOperatorSet.AreaCenter(ho_RegionDifference0_2, out hv_g2Regionarea, out hv_g2row, out hv_g2col);
                            HOperatorSet.AreaCenter(ho_g2Grey_2, out hv_g2area, out hv_g2row, out hv_g2col);
                            ho_GlueRegion_2.Dispose();
                            HOperatorSet.Union2(ho_g1Grey_2, ho_g2Grey_2, out ho_GlueRegion_2);
                            hv_GlueArea = hv_g1area + hv_g2area;
                        }
                    }
                    //hWVision.DispObj(ho_ImageTest);
                    if (cBGlueQ.Checked)
                    {
                        hWVision.SetColor("orange");
                        hWVision.SetLineWidth(1);
                        hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                        hWVision.SetDraw("margin");
                        hWVision.DispObj(ho_g1Rectangle);  //检测区域
                        hWVision.DispObj(ho_g2Rectangle);
                    }
                    if (cBGlueH.Checked)
                    {
                        //hWVision.DispObj(ho_ImageTest);
                        hWVision.SetDraw("fill");
                        hWVision.DispObj(ho_GlueRegion_2);
                        HD.set_display_font(hWVision, 16, "sans", "false", "false");
                        //HD.disp_message(hWVision, "1中标示面积" + Math.Round((double)hv_g1area).ToString() + "(" + Math.Round((double)hv_g1Regionarea).ToString() + ")", "", 100, 150, "green", "false");
                        //HD.disp_message(hWVision, "2中标示面积" + Math.Round((double)hv_g2area).ToString() + "(" + Math.Round((double)hv_g2Regionarea).ToString() + ")", "", 200, 150, "green", "false");
                    }
                    HD.set_display_font(hWVision, 18, "sans", "true", "false");
                    HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                    HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
                }
            }
            catch
            {
                return;
            }
        }




        HObject ho_GrayDiff11 = new HObject(), ho_GrayDiff21 = new HObject();
        HObject ho_CutRegion10 = new HObject(), ho_CutRegion20 = new HObject(), ho_ImageReducedI = new HObject(), ho_ImageReducedO = new HObject();
        HObject ho_GrayDiffIn11 = new HObject(), ho_GrayDiffIn12 = new HObject(), ho_GrayDiffOut11 = new HObject(), ho_GrayDiffOut12 = new HObject();
        HObject ho_GrayDiff = new HObject(), ho_GrayDiffSelect = new HObject(), ho_GrayDiffSelectIn = new HObject(), ho_GrayDiffSelectOut = new HObject();
        HObject ho_GlueUnion = new HObject(), ho_GlueBinImage = new HObject(), ho_ContourIn = new HObject(), ho_ContourOut = new HObject();
        void GlueEdge()
        {
            try
            {
                if (readpara)
                    return;
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                if (cBGlueH.Checked)
                {
                    #region  圆环
                    ho_CutRegion1.Dispose();
                    HOperatorSet.GenCircle(out ho_CutRegion1, hv_RowCenter, hv_ColCenter, GERadiusMin);
                    ho_CutRegion2.Dispose();
                    HOperatorSet.GenCircle(out ho_CutRegion2, hv_RowCenter, hv_ColCenter, GERadiusMax);
                    ho_ImageReduced1.Dispose();
                    HOperatorSet.Difference(ho_CutRegion2, ho_CutRegion1, out ho_ImageReduced1);
                    #endregion
                    #region  内圆环
                    ho_CutRegion10.Dispose();
                    HOperatorSet.GenCircle(out ho_CutRegion10, hv_RowCenter, hv_ColCenter, GERadiusMin - 50);
                    ho_ImageReducedI.Dispose();
                    HOperatorSet.Difference(ho_CutRegion1, ho_CutRegion10, out ho_ImageReducedI);
                    #endregion
                    #region  外圆环
                    ho_CutRegion20.Dispose();
                    HOperatorSet.GenCircle(out ho_CutRegion20, hv_RowCenter, hv_ColCenter, GERadiusMax + 50);
                    ho_ImageReducedO.Dispose();
                    HOperatorSet.Difference(ho_CutRegion20, ho_CutRegion2, out ho_ImageReducedO);
                    #endregion
                    #region 溢胶
                    ho_GlueUnion.Dispose();
                    HOperatorSet.Union2(ho_g1Grey, ho_g2Grey, out ho_GlueUnion);
                    ho_GrayDiff.Dispose();
                    HOperatorSet.Difference(ho_GlueUnion, ho_ImageReduced1, out ho_GrayDiff);
                    ho_GrayDiffSelect.Dispose();
                    HOperatorSet.SelectShape(ho_GrayDiff, out ho_GrayDiffSelect, "area", "and", 20, 99999);
                    ho_GrayDiffSelectIn.Dispose(); //内溢
                    HOperatorSet.Difference(ho_GrayDiffSelect, ho_ImageReducedO, out ho_GrayDiffSelectIn);
                    HOperatorSet.AreaCenter(ho_GrayDiffSelectIn, out hv_diffIArea, out hv_diffRow, out hv_diffCol);
                    ho_GrayDiffSelectOut.Dispose(); //外溢
                    HOperatorSet.Difference(ho_GrayDiffSelect, ho_ImageReducedI, out ho_GrayDiffSelectOut);
                    HOperatorSet.AreaCenter(ho_GrayDiffSelectOut, out hv_diffOArea, out hv_diffRow, out hv_diffCol);
                    if (hv_diffIArea.Length == 0)
                        hv_diffIArea = 0;
                    if (hv_diffOArea.Length == 0)
                        hv_diffOArea = 0;
                    #endregion
                }

                hWVision.DispObj(ho_ImageSet);
                hWVision.SetColor("red");
                hWVision.SetLineWidth(1);
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                hWVision.DispObj(ho_g1Rectangle);  //检测区域
                hWVision.DispObj(ho_g2Rectangle);
                if (cBGlueH.Checked)
                {
                    hWVision.DispObj(ho_g1Grey);
                    hWVision.DispObj(ho_g2Grey);
                    hWVision.SetColor("cyan");
                    hWVision.DispObj(ho_GrayDiffSelectIn);
                    hWVision.DispObj(ho_GrayDiffSelectOut);
                    HD.set_display_font(hWVision, 16, "sans", "false", "false");
                    HD.disp_message(hWVision, "1中标示面积" + Math.Round((double)hv_g1area).ToString() + "(" + Math.Round((double)hv_g1Regionarea).ToString() + ")", "", 100, 150, "green", "false");
                    HD.disp_message(hWVision, "2中标示面积" + Math.Round((double)hv_g2area).ToString() + "(" + Math.Round((double)hv_g2Regionarea).ToString() + ")", "", 200, 150, "green", "false");
                    HD.disp_message(hWVision, "点胶内溢面积" + Math.Round((double)hv_diffIArea).ToString(), "", 300, 150, "blue", "false");
                    HD.disp_message(hWVision, "点胶外溢面积" + Math.Round((double)hv_diffOArea).ToString(), "", 400, 150, "blue", "false");
                }
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
                hWVision.SetColor("yellow");
                ho_DiffCirIn.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_DiffCirIn, hv_RowCenter, hv_ColCenter, GERadiusMin, 0, 6.28318, "positive", 1);
                hWVision.DispObj(ho_DiffCirIn);
                hWVision.SetLineWidth(2);
                ho_DiffCirOut.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_DiffCirOut, hv_RowCenter, hv_ColCenter, GERadiusMax, 0, 6.28318, "positive", 1);
                hWVision.DispObj(ho_DiffCirOut);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tBglueR_ValueChanged(object sender, EventArgs e)
        {
            MidCirRadius = (int)tBglueR.Value;
            UDglueR.Value = MidCirRadius;
        }
        private void UDglueR_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                MidCirRadius = (int)UDglueR.Value;
                tBglueR.Value = MidCirRadius;
                switch (int.Parse(SetNum))
                {
                    case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                    case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                    case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                    case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                    case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                    case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                    case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                    case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
                }
                //顯示實際內外徑
                lblOuterRadius.Text = Math.Round((double)(UDglueR.Value + UDglueW.Value) * xpm, 3).ToString();
                lblInnerRadius.Text = Math.Round((double)(UDglueR.Value - UDglueW.Value) * xpm, 3).ToString();
                if (readpara)
                    return;
                if (RegionWidth / 2 > MidCirRadius & !readpara)
                {
                    MessageBox.Show("区域宽度不能大于区域直径！");
                    return;
                }
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                ShowGlueSector();
                ShowGlueSectorGray();
                if (!cBGlueEdge.Checked)
                    return;
                if ((MidCirRadius - RegionWidth / 2) < (int)tBLimitIn.Value)
                {
                    UDglueR.BackColor = Color.WhiteSmoke;
                    UDglueW.BackColor = Color.WhiteSmoke;
                    UDLimitIn.BackColor = Color.White;
                    gBErrorShow.Hide();
                }
                else
                {
                    UDglueR.BackColor = Color.Red;
                    UDglueW.BackColor = Color.Red;
                    UDLimitIn.BackColor = Color.Red;
                    if (readpara)
                    {
                        gBErrorShow.Hide();
                        return;
                    }
                    gBErrorShow.Show();
                    lblErrorShow.Text = "请按提示重新设置相关参数！\r\n\r\n" + "点胶范围内径必须大于（区域中心半径-区域宽度/2）\r\n" + "即必须>" + (MidCirRadius - RegionWidth / 2).ToString()
                        + "(=" + MidCirRadius.ToString() + "-" + RegionWidth.ToString() + "/2)\r\n";
                    return;
                }
                if ((MidCirRadius + RegionWidth / 2) > (int)tBLimitOut.Value)
                {
                    UDglueR.BackColor = Color.WhiteSmoke;
                    UDglueW.BackColor = Color.WhiteSmoke;
                    UDLimitOut.BackColor = Color.White;
                    gBErrorShow.Hide();
                }
                else
                {
                    UDglueR.BackColor = Color.Red;
                    UDglueW.BackColor = Color.Red;
                    UDLimitOut.BackColor = Color.Red;
                    if (readpara)
                    {
                        gBErrorShow.Hide();
                        return;
                    }
                    gBErrorShow.Show();
                    lblErrorShow.Text = "请按提示重新设置相关参数！\r\n\r\n" + "点胶范围内径必须小于（区域中心半径+区域宽度/2）\r\n" + "即必须<" + (MidCirRadius + RegionWidth / 2).ToString()
                        + "(=" + MidCirRadius.ToString() + "+" + RegionWidth.ToString() + "/2)\r\n";
                }
            }
            catch
            {
            }
        }
        private void tBglueW_ValueChanged(object sender, EventArgs e)
        {
            RegionWidth = (int)tBglueW.Value;
            UDglueW.Value = RegionWidth;
        }
        private void UDglueW_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                RegionWidth = (int)UDglueW.Value;
                tBglueW.Value = RegionWidth;
                switch (int.Parse(SetNum))
                {
                    case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                    case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                    case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                    case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                    case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                    case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                    case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                    case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
                }
                //顯示實際內外徑
                lblOuterRadius.Text = Math.Round((double)(UDglueR.Value + UDglueW.Value) * xpm, 3).ToString();
                lblInnerRadius.Text = Math.Round((double)(UDglueR.Value - UDglueW.Value) * xpm, 3).ToString();
                if (readpara)
                    return;
                if (RegionWidth / 2 > MidCirRadius & !readpara)
                {
                    MessageBox.Show("区域宽度不能大于区域直径！");
                    return;
                }
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                ShowGlueSector();
                ShowGlueSectorGray();
                if (!cBGlueEdge.Checked)
                    return;
                if ((MidCirRadius - RegionWidth / 2) < (int)tBLimitIn.Value)
                {
                    UDglueR.BackColor = Color.WhiteSmoke;
                    UDglueW.BackColor = Color.WhiteSmoke;
                    UDLimitIn.BackColor = Color.White;
                    gBErrorShow.Hide();
                }
                else
                {
                    UDglueR.BackColor = Color.Red;
                    UDglueW.BackColor = Color.Red;
                    UDLimitIn.BackColor = Color.Red;
                    if (readpara)
                    {
                        gBErrorShow.Hide();
                        return;
                    }
                    gBErrorShow.Show();
                    lblErrorShow.Text = "请按提示重新设置相关参数！\r\n\r\n" + "点胶范围内径必须大于（区域中心半径-区域宽度/2）\r\n" + "即必须>" + (MidCirRadius - RegionWidth / 2).ToString()
                        + "(=" + MidCirRadius.ToString() + "-" + RegionWidth.ToString() + "/2)\r\n";
                    return;
                }
                if ((MidCirRadius + RegionWidth / 2) > (int)tBLimitOut.Value)
                {
                    UDglueR.BackColor = Color.WhiteSmoke;
                    UDglueW.BackColor = Color.WhiteSmoke;
                    UDLimitOut.BackColor = Color.White;
                    gBErrorShow.Hide();
                }
                else
                {
                    UDglueR.BackColor = Color.Red;
                    UDglueW.BackColor = Color.Red;
                    UDLimitOut.BackColor = Color.Red;
                    if (readpara)
                    {
                        gBErrorShow.Hide();
                        return;
                    }
                    gBErrorShow.Show();
                    lblErrorShow.Text = "请按提示重新设置相关参数！\r\n\r\n" + "点胶范围内径必须小于（区域中心半径+区域宽度/2）\r\n" + "即必须<" + (MidCirRadius + RegionWidth / 2).ToString()
                        + "(=" + MidCirRadius.ToString() + "+" + RegionWidth.ToString() + "/2)\r\n";
                }
            }
            catch
            {
            }
        }
        private void tBglueStartA_ValueChanged(object sender, EventArgs e)
        {
            StartAngle = (int)tBglueStartA.Value;
            UDglueStartA.Value = StartAngle;
        }
        private void UDglueStartA_ValueChanged(object sender, EventArgs e)
        {
            StartAngle = (int)UDglueStartA.Value;
            tBglueStartA.Value = StartAngle;
            int i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            ShowGlueSector();
            ShowGlueSectorGray();
        }
        private void tBglueEndA_ValueChanged(object sender, EventArgs e)
        {
            EndAngle = (int)tBglueEndA.Value;
            UDglueEndA.Value = EndAngle;
        }
        private void UDglueEndA_ValueChanged(object sender, EventArgs e)
        {
            EndAngle = (int)UDglueEndA.Value;
            tBglueEndA.Value = EndAngle;
            int i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            ShowGlueSector();
            ShowGlueSectorGray();
        }
        private void tBLimitIn_ValueChanged(object sender, EventArgs e)
        {
            GERadiusMin = tBLimitIn.Value;
            UDLimitIn.Value = GERadiusMin;
        }
        private void UDLimitIn_ValueChanged(object sender, EventArgs e)
        {
            switch (int.Parse(SetNum))
            {
                case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            GERadiusMin = (int)UDLimitIn.Value;
            tBLimitIn.Value = GERadiusMin;
            lblIn.Text = Math.Round((double)GERadiusMin * xpm, 3).ToString();
            if (GERadiusMin >= (int)UDLimitOut.Value)
            {
                UDLimitIn.Value = (int)UDLimitIn.Value - 1;
                return;
            }
            GlueEdge();
            if (!cBGlueEdge.Checked)
                return;
            if ((MidCirRadius - RegionWidth / 2) < (int)tBLimitIn.Value)
            {
                UDglueR.BackColor = Color.WhiteSmoke;
                UDglueW.BackColor = Color.WhiteSmoke;
                UDLimitIn.BackColor = Color.WhiteSmoke;
                gBErrorShow.Hide();
            }
            else
            {
                UDglueR.BackColor = Color.Red;
                UDglueW.BackColor = Color.Red;
                UDLimitIn.BackColor = Color.Red;
                if (readpara)
                {
                    gBErrorShow.Hide();
                    return;
                }
                gBErrorShow.Show();
                lblErrorShow.Text = "请按提示重新设置相关参数！\r\n\r\n" + "点胶范围内径必须大于（区域中心半径-区域宽度/2）\r\n" + "即必须>" + (MidCirRadius - RegionWidth / 2).ToString()
                    + "(=" + MidCirRadius.ToString() + "-" + RegionWidth.ToString() + "/2)\r\n";
            }
        }
        private void tBLimitOut_ValueChanged(object sender, EventArgs e)
        {
            GERadiusMax = tBLimitOut.Value;
            UDLimitOut.Value = GERadiusMax;
        }
        private void UDLimitOut_ValueChanged(object sender, EventArgs e)
        {
            switch (int.Parse(SetNum))
            {
                case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            GERadiusMax = (int)UDLimitOut.Value;
            tBLimitOut.Value = GERadiusMax;
            lblOut.Text = Math.Round((double)GERadiusMax * xpm, 3).ToString();
            if (GERadiusMax <= (int)UDLimitIn.Value)
            {
                UDLimitOut.Value = (int)UDLimitOut.Value + 1;
                return;
            }
            GlueEdge();
            if (!cBGlueEdge.Checked)
                return;
            if ((MidCirRadius + RegionWidth / 2) > (int)tBLimitOut.Value)
            {
                gBErrorShow.Hide();
                UDglueR.BackColor = Color.WhiteSmoke;
                UDglueW.BackColor = Color.WhiteSmoke;
                UDLimitOut.BackColor = Color.White;
            }
            else
            {
                UDglueR.BackColor = Color.Red;
                UDglueW.BackColor = Color.Red;
                UDLimitOut.BackColor = Color.Red;
                if (readpara)
                {
                    gBErrorShow.Hide();
                    return;
                }
                gBErrorShow.Show();
                lblErrorShow.Text = "请按提示重新设置相关参数！\r\n\r\n" + "点胶范围内径必须小于（区域中心半径+区域宽度/2）\r\n" + "即必须<" + (MidCirRadius + RegionWidth / 2).ToString()
                    + "(=" + MidCirRadius.ToString() + "+" + RegionWidth.ToString() + "/2)\r\n";
            }
        }
        private void cBGlueEdge_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGlueEdge.Checked)
            {
                gBLimit.Enabled = true;
                gBLimitArea.Enabled = true;
            }
            else
            {
                gBLimit.Enabled = false;
                gBLimitArea.Enabled = false;
            }
        }
        #region 外观检测（半成品胶点Add）
        //溢胶
        private void cBGlueCir_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGlueCir.Checked)
                CallibratManu.SelectedIndex = 7;
        }
        HTuple hv_GlueOutAimR = new HTuple(), hv_GlueOutCheckR = new HTuple(), hv_GlueOutCheckR2 = new HTuple(), hv_GlueOutgray = new HTuple();
        HTuple hv_GlueOutArea = new HTuple(), hv_GlueOutRow = new HTuple(), hv_GlueOutCol = new HTuple();
        HObject ho_GlueOutCir1 = new HObject(), ho_GlueOutCir2 = new HObject(), ho_GlueOutDiff = new HObject();
        HObject ho_GlueOutReduced = new HObject(), ho_GlueOutRegion = new HObject(), ho_GlueOutFillup = new HObject();
        private void tabGlueOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((SetNum == "2" || SetNum == "4") & tabGlueOut.SelectedIndex == 0)
                tabGlueOut.SelectedIndex = 1;
            if (SetNum == "6" & tabGlueOut.SelectedIndex == 1)
                tabGlueOut.SelectedIndex = 0;
            if (SetNum == "5")
                tabGlueOut.SelectedIndex = 2;
            if (SetNum == "9")
                tabGlueOut.SelectedIndex = 3;
            cBGOutMode.Value = tabGlueOut.SelectedIndex + 1;
            if (tabGlueOut.SelectedIndex == 4)
            {
                try
                {
                    Sign.isMode5ParaChange = true;
                    string CCDname = GetCCDName();
                    Mode5Parm(CCDname);
                    Sign.isMode5ParaChange = false;
                }
                catch
                {
                    Sign.isMode5ParaChange = false;

                }

            }
        }
        private void cBGOutMode_ValueChanged(object sender, EventArgs e)
        {
            if ((SetNum == "2" || SetNum == "4") & cBGOutMode.Value == 1)
                tabGlueOut.SelectedIndex = 2;
            if (SetNum == "6" & cBGOutMode.Value == 2)
                tabGlueOut.SelectedIndex = 1;
            if (SetNum == "5")
                tabGlueOut.SelectedIndex = 2;
            if (SetNum == "9")
                tabGlueOut.SelectedIndex = 3;
            tabGlueOut.SelectedIndex = (int)cBGOutMode.Value - 1;
        }
        #region AVIMode1
        private void lblVarShow_DoubleClick(object sender, EventArgs e)
        {
            string CCDNAME = ""; string area1 = "", area2 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            Sys.setStation = CCDNAME;
            fv = new FrmVar();
            fv.ShowDialog();
        }
        private void GlueOutcheck_CheckedChanged(object sender, EventArgs e)
        {
            if (GlueOutcheck.Checked)
                tabGlueOut.SelectedIndex = 0;
        }
        private void cBAVIR2_CheckedChanged(object sender, EventArgs e)
        {
            txtOutCirR2.Enabled = (cBAVIR2.Checked ? true : false);
        }
        public static double txtOutP = 0.0;
        private void btnQECir_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0 || hv_RowCenter == 0.0)
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            if (txtAimCirR.Text == "0" || txtOutCirR.Text == "0")
            {
                MessageBox.Show("目标检测圆直径或溢胶检测圆直径未设置！");
                return;
            }
            if (cBAVIR2.Checked)
            {
                if (double.Parse(txtOutCirR.Text) > double.Parse(txtOutCirR2.Text))
                {
                    MessageBox.Show("检测圆2的直径必须大于或等于检测圆1的直径！");
                    return;
                }
            }
            try
            {
                hv_GlueOutAimR = hv_CenterRadius;
                hv_GlueOutCheckR = (double.Parse(txtOutCirR.Text) - txtOutP) * hv_GlueOutAimR / double.Parse(txtAimCirR.Text);
                if (cBAVIR2.Checked)
                    hv_GlueOutCheckR2 = (double.Parse(txtOutCirR2.Text) - txtOutP) * hv_GlueOutAimR / double.Parse(txtAimCirR.Text);
                PCCD2.ppxm = Math.Round(double.Parse(txtAimCirR.Text) / (double)hv_GlueOutAimR / 2, 10);
                cpxm = PCCD2.ppxm;
                txtPpix.Text = PCCD2.ppxm.ToString();
                PCCD2.ppxm = 1;
                ShowGlueOutCir();
                hx = (hv_ColCenter - col) * cpxm;
                hy = -(hv_RowCenter - row) * cpxm;
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void ShowGlueOutCir()
        {
            if (readpara)
                return;
            int i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            hWVision.SetDraw("margin");
            hWVision.SetLineWidth(1);
            hWVision.SetColor("red");
            hWVision.DispCross(row, col, width, 0);
            hWVision.SetColor("green");
            HTuple goWidth = tBGlueOutWidth.Value;
            hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR);
            if (!cBAVIR2.Checked)
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR + goWidth);
            else
            {
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR2 + goWidth);
                hWVision.SetColor("cyan");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR2);
            }
            hWVision.SetColor("blue");
            hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutAimR);
        }
        private void tBGlueOutWidth_ValueChanged(object sender, EventArgs e)
        {
            int gbW = tBGlueOutWidth.Value;
            UDGlueOutWidth.Value = gbW;
        }
        private void UDGlueOutWidth_ValueChanged(object sender, EventArgs e)
        {
            int gbW = (int)UDGlueOutWidth.Value;
            tBGlueOutWidth.Value = gbW;
            if (readpara)
                return;
            if (hv_RowCenter.Length == 0.0 || hv_RowCenter == 0.0)
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            if (txtAimCirR.Text == "0" || txtOutCirR.Text == "0")
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            ShowGlueOutCir();
        }
        private void tBGlueOutGray_ValueChanged(object sender, EventArgs e)
        {
            hv_GlueOutgray = tBGlueOutGray.Value;
            UDGlueOutGray.Value = hv_GlueOutgray;
        }
        private void UDGlueOutGray_ValueChanged(object sender, EventArgs e)
        {
            hv_GlueOutgray = (int)UDGlueOutGray.Value;
            tBGlueOutGray.Value = hv_GlueOutgray;
            if (readpara)
                return;
            if (hv_RowCenter.Length == 0.0 || hv_RowCenter == 0.0)
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            if (txtAimCirR.Text == "0" || txtOutCirR.Text == "0")
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            ShowGlueOutResult();
        }
        private void ShowGlueOutResult()
        {
            if (readpara)
                return;
            int i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            HTuple goWidth = tBGlueOutWidth.Value;
            ho_GlueOutCir1.Dispose();
            HOperatorSet.GenCircle(out ho_GlueOutCir1, hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR);
            ho_GlueOutCir2.Dispose();
            if (!cBAVIR2.Checked)
                HOperatorSet.GenCircle(out ho_GlueOutCir2, hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR + goWidth);
            else
                HOperatorSet.GenCircle(out ho_GlueOutCir2, hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR2 + goWidth);
            ho_GlueOutDiff.Dispose();
            HOperatorSet.Difference(ho_GlueOutCir2, ho_GlueOutCir1, out ho_GlueOutDiff);
            ho_GlueOutReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageSet, ho_GlueOutDiff, out ho_GlueOutReduced);
            ho_GlueOutRegion.Dispose();
            HOperatorSet.Threshold(ho_GlueOutReduced, out ho_GlueOutRegion, hv_GlueOutgray, 255);
            ho_GlueOutFillup.Dispose();
            HOperatorSet.FillUp(ho_GlueOutRegion, out ho_GlueOutFillup);
            HOperatorSet.AreaCenter(ho_GlueOutFillup, out hv_GlueOutArea, out hv_GlueOutRow, out hv_GlueOutCol);


            hWVision.ClearWindow();
            hWVision.SetDraw("fill");
            hWVision.DispObj(ho_ImageSet);
            hWVision.SetLineWidth(1);
            hWVision.SetColor("red");
            hWVision.DispCross(row, col, width, 0);
            hWVision.DispObj(ho_GlueOutRegion);
            hWVision.SetColor("green");
            hWVision.SetDraw("margin");
            hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR);
            hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutCheckR + goWidth);
            hWVision.SetColor("blue");
            hWVision.DispCircle(hv_RowCenter, hv_ColCenter, hv_GlueOutAimR);
            HD.disp_message(hWVision, "Area:" + (hv_GlueOutArea * PCCD2.ppxm * PCCD2.ppxm).ToString(), "", 150, 150, "red", "false");

        }
        private void btnDrawCoatCir_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0 || hv_RowCenter == 0.0)
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            #region
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            ho_ImageTest.Dispose();
            HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
            if (tBRegion2.Value != 0)
            {
                RegionRadius = (HTuple)tBRegion2.Value;
                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
            }
            try
            {
                hv_RingRow = hv_RowCenter; hv_RingCol = hv_ColCenter;
                hv_RingRadius = hv_CenterRadius;
                tBCoatRadius.Value = (int)Math.Round((double)hv_CenterRadius);
                hv_RDetectHeight = (int)UDCoatWidth.Value;
                hv_transition = "all";
                if (tBCoatB2W.Value != 1)
                    hv_transition = "negative";
                if (tBCoatW2B.Value != 255)
                    hv_transition = "positive";

                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);

                ShowRing1();
            }
            catch (Exception)
            {
                //
            }
            #endregion
        }
        private void tBCoatRadius_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = tBCoatRadius.Value;
            UDCoatRadius.Value = hv_RingRadius;
        }
        private void UDCoatRadius_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = (HTuple)UDCoatRadius.Value;
            tBCoatRadius.Value = hv_RingRadius;
            ShowRing1();
        }
        private void tBCoatWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = tBCoatWidth.Value;
            UDCoatWidth.Value = hv_RDetectHeight;
        }
        private void UDCoatWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = (int)UDCoatWidth.Value;
            tBCoatWidth.Value = hv_RDetectHeight;
            ShowRing1();
        }
        private void tBCoatW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBCoatW2B.Value;
            UDCoatW2B.Value = hv_RAmplitudeThreshold;
        }
        private void UDCoatW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDCoatW2B.Value;
            tBCoatW2B.Value = hv_RAmplitudeThreshold;
            tBCoatB2W.Value = 1;
            hv_transition = "positive";
            ShowRing1();
        }
        private void tBCoatB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBCoatB2W.Value;
            UDCoatB2W.Value = hv_RAmplitudeThreshold;
        }
        private void UDCoatB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDCoatB2W.Value;
            tBCoatB2W.Value = hv_RAmplitudeThreshold;
            tBCoatW2B.Value = 1;
            hv_transition = "negative";
            ShowRing1();
        }
        private void btnCoatCirR_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string area1 = "", area2 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; CCDNAME = "A1CCD1"; break;
                case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; CCDNAME = "A1CCD2-" + area1; break;
                case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; CCDNAME = "A2CCD1"; break;
                case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; CCDNAME = "A2CCD2-" + area1; break;
                case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; CCDNAME = "PCCD1"; break;
                case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; CCDNAME = "PCCD2-" + area1; break;
                case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; CCDNAME = "GCCD1"; break;
                case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; CCDNAME = "GCCD2-" + area2; break;
                case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; CCDNAME = "QCCD"; break;
            }
            Numbase = double.Parse(iniFile.Read(CCDNAME, "jBaseNum", FrmMain.propath));
            Numadd = double.Parse(iniFile.Read(CCDNAME, "jPlusNum", FrmMain.propath));
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            try
            {
                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_CrossCenter);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);
                HOperatorSet.GenEmptyObj(out ho_ResultContours);

                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBRegion2.Value;
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_ResultContours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours, hv_MetrologyHandle, "all", "all", 1.5);
                hv_ColCenter = hv_circleParameter.TupleSelect(1);
                hv_RowCenter = hv_circleParameter.TupleSelect(0);
                hv_CenterRadius = hv_circleParameter.TupleSelect(2);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                cx = (hv_ColCenter - col) * cpxm;
                cy = -(hv_RowCenter - row) * cpxm;
                //double coatos = double.Parse(txtOffset.Text);
                double rvalue = Math.Round((double)hv_CenterRadius * cpxm * 2, 3) - coatos;

                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_ResultContours);
                hWVision.SetColor("red");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                if (halcon.IsCrossDraw)
                    hWVision.DispCross(row, col, width, 0);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm, "", 150, 150, "green", "false");
                HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row) * ypm), "", 300, 150, "green", "false");
                //HD.disp_message(hWVision, "R:" + hv_CenterRadius, "", 450, 150, "green", "false");
                //double cmin = 7.5 - coatos;
                //if (cmin < 7.48)
                //    cmin = 7.48;
                HD.disp_message(hWVision, "D:" + rvalue + "mm", "", 450, 150, "green", "false");
                ho_ImageTest.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                //ho_ResultContours.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void btnCalO1O2_Click(object sender, EventArgs e)
        {
            double coatos = 0;
            double rvalue = Math.Round((double)hv_CenterRadius * cpxm * 2, 3);
            double cmin = 7.6 - coatos;
            if (cmin < 7.48)
                cmin = 7.48;
            if (hx == 0 || cx == 0)
            {
                MessageBox.Show("请先找到O1、O2的圆心！");
                return;
            }
            HOperatorSet.DistancePp((HTuple)hx, (HTuple)hy, (HTuple)cx, (HTuple)cy, out dishc);
            double diam_min = Math.Round(Math.Round((double)hv_CenterRadius * cpxm * 2, 3) - 2 * (double)dishc, 3);
            txtDiamMin.Text = diam_min.ToString();
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            hWVision.SetLineWidth(1);
            hWVision.SetColor("green");
            hWVision.DispObj(ho_ResultContours);
            hWVision.SetColor("red");
            hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
            if (halcon.IsCrossDraw)
                hWVision.DispCross(row, col, width, 0);
            HD.set_display_font(hWVision, 18, "sans", "true", "false");
            HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm, "", 150, 150, "green", "false");
            HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row) * ypm), "", 300, 150, "green", "false");
            //HD.disp_message(hWVision, "R:" + hv_CenterRadius, "", 450, 150, "green", "false");
            HD.disp_message(hWVision, "D:" + Math.Round((double)hv_CenterRadius * cpxm * 2, 3) + "mm", "", 1650, 150, "green", "false");
            HD.disp_message(hWVision, "Diam_Min:" + Math.Round(diam_min, 3) + "mm", "", 1800, 150, "green", "false");
        }
        #endregion
        #region AVIMode2
        private void lblVarShow2_DoubleClick(object sender, EventArgs e)
        {
            string CCDNAME = ""; string area1 = "", area2 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            Sys.setStation = CCDNAME;
            fv = new FrmVar();
            fv.ShowDialog();
        }
        private void Coatcheck_CheckedChanged(object sender, EventArgs e)
        {
            //tabGlueOut.SelectedIndex = 1;
            string cc1 = iniFile.Read("A1CCD2-PickUp", "HCoatCIChecked", FrmMain.propath);
            string cc2 = iniFile.Read("A2CCD2-PickUp", "HCoatCIChecked", FrmMain.propath);
            if (Coatcheck.Checked & (cc1 == "True" || cc2 == "True"))
            {
                MessageBox.Show("此功能不可与A1CCD2&A2CCD2中启动镀膜检测同时开启，请重新确认！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Coatcheck.Checked = false;
            }
            gBCoat.Enabled = Coatcheck.Checked;
        }
        double cpxm = 1.0, hx = 0, hy = 0, cx = 0, cy = 0;
        HTuple dishc = new HTuple(), hv_HoleThreshold = new HTuple(), hv_Holetransition = new HTuple();
        private void btnDrawHoleCir_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0 || hv_RowCenter == 0.0)
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            cpxm = Math.Round(double.Parse(txtLensAimR.Text) / (double)hv_CenterRadius / 2, 10);
            txtLenspix.Text = cpxm.ToString();
            #region
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            ho_ImageTest.Dispose();
            HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
            if (tBRegion2.Value != 0)
            {
                RegionRadius = (HTuple)tBRegion2.Value;
                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
            }
            try
            {
                hv_RingRow = hv_RowCenter; hv_RingCol = hv_ColCenter;
                hv_RingRadius = hv_CenterRadius;
                tBHoleRadius.Value = (int)Math.Round((double)hv_CenterRadius);
                hv_RDetectHeight = (int)UDHoleWidth.Value;
                hv_transition = "all";
                if (tBHoleB2W.Value != 1)
                    hv_transition = "negative";
                if (tBHoleW2B.Value != 255)
                    hv_transition = "positive";

                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);

                ShowRing1();
            }
            catch (Exception)
            {
                //
            }
            #endregion
        }
        private void tBHoleRadius_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = tBHoleRadius.Value;
            UDHoleRadius.Value = hv_RingRadius;
        }
        private void UDHoleRadius_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = (HTuple)UDHoleRadius.Value;
            tBHoleRadius.Value = hv_RingRadius;
            ShowRing1();
        }
        private void tBHoleWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = tBHoleWidth.Value;
            UDHoleWidth.Value = hv_RDetectHeight;
        }
        private void UDHoleWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = (int)UDHoleWidth.Value;
            tBHoleWidth.Value = hv_RDetectHeight;
            ShowRing1();
        }
        private void tBHoleW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_HoleThreshold = tBHoleW2B.Value;
            UDHoleW2B.Value = hv_HoleThreshold;
        }
        private void UDHoleW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_HoleThreshold = (HTuple)UDHoleW2B.Value;
            tBHoleW2B.Value = hv_HoleThreshold;
            hv_RAmplitudeThreshold = hv_HoleThreshold;
            tBHoleB2W.Value = 1;
            hv_Holetransition = "positive";
            hv_transition = hv_Holetransition;
            ShowRing1();
        }
        private void tBHoleB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_HoleThreshold = tBHoleB2W.Value;
            UDHoleB2W.Value = hv_HoleThreshold;
        }
        private void UDHoleB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_HoleThreshold = (HTuple)UDHoleB2W.Value;
            tBHoleB2W.Value = hv_HoleThreshold;
            hv_RAmplitudeThreshold = hv_HoleThreshold;
            tBHoleW2B.Value = 1;
            hv_Holetransition = "negative";
            hv_transition = hv_Holetransition;
            ShowRing1();
        }
        private void btnHoleCirR_Click(object sender, EventArgs e)
        {
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            try
            {
                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_CrossCenter);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);
                HOperatorSet.GenEmptyObj(out ho_ResultContours);

                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBRegion2.Value;
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_ResultContours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours, hv_MetrologyHandle, "all", "all", 1.5);
                hv_ColCenter = hv_circleParameter.TupleSelect(1);
                hv_RowCenter = hv_circleParameter.TupleSelect(0);
                hv_CenterRadius = hv_circleParameter.TupleSelect(2);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                hx = (hv_ColCenter - col) * cpxm;
                hy = -(hv_RowCenter - row) * cpxm;

                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_ResultContours);
                hWVision.SetColor("red");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                if (halcon.IsCrossDraw)
                    hWVision.DispCross(row, col, width, 0);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * cpxm, "", 150, 150, "green", "false");
                HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row) * cpxm), "", 300, 150, "green", "false");
                //HD.disp_message(hWVision, "D:" + Math.Round((double)hv_CenterRadius * xpm * 2, 4) + "mm", "", 450, 150, "green", "false");
                ho_ImageTest.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                ho_ResultContours.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }

        private void Coatcheck2_CheckedChanged(object sender, EventArgs e)
        {
            string cc1 = iniFile.Read("PCCD2-Platform1", "CoatCIChecked", FrmMain.propath);
            string cc2 = iniFile.Read("PCCD2-Platform2", "CoatCIChecked", FrmMain.propath);
            if (Coatcheck2.Checked & (cc1 == "True" || cc2 == "True"))
            {
                MessageBox.Show("此功能不可与PCCD2中启动镀膜检测同时开启，请重新确认！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Coatcheck2.Checked = false;
            }
            gBCoatMode21.Enabled = Coatcheck2.Checked;
            gBCoatMode22.Enabled = Coatcheck2.Checked;
            gBCoatMode23.Enabled = Coatcheck2.Checked;
        }
        private void btnDrawCoatCir2_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0 || hv_RowCenter == 0.0)
            {
                MessageBox.Show("请通过*找圆心1或找圆心2*找到圆心及目标检测圆！");
                return;
            }
            #region
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            ho_ImageTest.Dispose();
            HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
            if (tBRegion2.Value != 0)
            {
                RegionRadius = (HTuple)tBRegion2.Value;
                HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                ho_RegionReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
            }
            try
            {
                hv_RingRow = hv_RowCenter; hv_RingCol = hv_ColCenter;
                hv_RingRadius = hv_CenterRadius;
                tBCoatRadius.Value = (int)Math.Round((double)hv_CenterRadius);
                hv_RDetectHeight = (int)UDCoatWidth2.Value;
                hv_transition = "all";
                if (tBCoatB2W2.Value != 1)
                    hv_transition = "negative";
                if (tBCoatW2B2.Value != 255)
                    hv_transition = "positive";

                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);

                ShowRing1();
            }
            catch (Exception)
            {
                //
            }
            #endregion
        }
        private void tBCoatRadius2_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = tBCoatRadius2.Value;
            UDCoatRadius2.Value = hv_RingRadius;
        }
        private void UDCoatRadius2_ValueChanged(object sender, EventArgs e)
        {
            hv_RingRadius = (HTuple)UDCoatRadius2.Value;
            tBCoatRadius2.Value = hv_RingRadius;
            ShowRing1();
        }
        private void tBCoatWidth2_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = tBCoatWidth2.Value;
            UDCoatWidth2.Value = hv_RDetectHeight;
        }
        private void UDCoatWidth2_ValueChanged(object sender, EventArgs e)
        {
            hv_RDetectHeight = (int)UDCoatWidth2.Value;
            tBCoatWidth2.Value = hv_RDetectHeight;
            ShowRing1();
        }
        private void tBCoatW2B2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBCoatW2B2.Value;
            UDCoatW2B2.Value = hv_RAmplitudeThreshold;
        }
        private void UDCoatW2B2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDCoatW2B2.Value;
            tBCoatW2B2.Value = hv_RAmplitudeThreshold;
            tBCoatB2W2.Value = 1;
            hv_transition = "positive";
            ShowRing1();
        }
        private void tBCoatB2W2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = tBCoatB2W2.Value;
            UDCoatB2W2.Value = hv_RAmplitudeThreshold;
        }
        private void UDCoatB2W2_ValueChanged(object sender, EventArgs e)
        {
            hv_RAmplitudeThreshold = (HTuple)UDCoatB2W2.Value;
            tBCoatB2W2.Value = hv_RAmplitudeThreshold;
            tBCoatW2B2.Value = 1;
            hv_transition = "negative";
            ShowRing1();
        }
        private void btnCoatCirR2_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string area1 = "", area2 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; CCDNAME = "A1CCD1"; break;
                case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; CCDNAME = "A1CCD2-" + area1; break;
                case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; CCDNAME = "A2CCD1"; break;
                case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; CCDNAME = "A2CCD2-" + area1; break;
                case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; CCDNAME = "PCCD1"; break;
                case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; CCDNAME = "PCCD2-" + area1; break;
                case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; CCDNAME = "GCCD1"; break;
                case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; CCDNAME = "GCCD2-" + area2; break;
                case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; CCDNAME = "QCCD"; break;
            }
            Numbase2 = double.Parse(iniFile.Read(CCDNAME, "jBaseNum", FrmMain.propath));
            Numadd2 = double.Parse(iniFile.Read(CCDNAME, "jPlusNum", FrmMain.propath));
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            try
            {
                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_CrossCenter);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);
                HOperatorSet.GenEmptyObj(out ho_ResultContours);

                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                if (tabVisionSet.SelectedIndex == 1 && tBRRadius.Value != 0)
                {
                    RegionRadius = (HTuple)tBRegion2.Value;
                    HOperatorSet.GenCircle(out ho_Rcircle, row, col, RegionRadius);
                    ho_RegionReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageSet, ho_Rcircle, out ho_RegionReduced);
                    ho_ImageTest.Dispose();
                    HOperatorSet.CopyImage(ho_RegionReduced, out ho_ImageTest);
                }
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_ResultContours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours, hv_MetrologyHandle, "all", "all", 1.5);
                hv_ColCenter = hv_circleParameter.TupleSelect(1);
                hv_RowCenter = hv_circleParameter.TupleSelect(0);
                hv_CenterRadius = hv_circleParameter.TupleSelect(2);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                cx = (hv_ColCenter - col) * cpxm;
                cy = -(hv_RowCenter - row) * cpxm;
                //double coatos = double.Parse(txtOffset2.Text);
                double rvalue = Math.Round((double)hv_CenterRadius * cpxm * 2, 3) - coatos2;

                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_ResultContours);
                hWVision.SetColor("red");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                if (halcon.IsCrossDraw)
                    hWVision.DispCross(row, col, width, 0);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm, "", 150, 150, "green", "false");
                HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row) * ypm), "", 300, 150, "green", "false");
                //HD.disp_message(hWVision, "R:" + hv_CenterRadius, "", 450, 150, "green", "false");
                //if (rvalue < 7.7)
                HD.disp_message(hWVision, "D:" + rvalue + "mm", "", 450, 150, "green", "false");
                //else
                //    HD.disp_message(hWVision, "D:" + rvalue + "mm(此值范围7.6~7.7)", "", 450, 150, "red", "false");
                ho_ImageTest.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                //ho_ResultContours.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void btn2CalO1O2_Click(object sender, EventArgs e)
        {
            double rvalue = Math.Round((double)hv_CenterRadius * cpxm * 2, 3) - coatos2;
            if (hx == 0 || cx == 0)
            {
                MessageBox.Show("请先找到O1、O2的圆心！");
                return;
            }
            HOperatorSet.DistancePp((HTuple)hx, (HTuple)hy, (HTuple)cx, (HTuple)cy, out dishc);
            double diam_min = Math.Round(Math.Round((double)hv_CenterRadius * cpxm * 2, 3) - 2 * (double)dishc, 3);
            txtDiamMin2.Text = diam_min.ToString();
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            hWVision.SetLineWidth(1);
            hWVision.SetColor("green");
            hWVision.DispObj(ho_ResultContours);
            hWVision.SetColor("red");
            hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
            if (halcon.IsCrossDraw)
                hWVision.DispCross(row, col, width, 0);
            HD.set_display_font(hWVision, 18, "sans", "true", "false");
            HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm, "", 150, 150, "green", "false");
            HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row) * ypm), "", 300, 150, "green", "false");
            //HD.disp_message(hWVision, "R:" + hv_CenterRadius, "", 450, 150, "green", "false");
            HD.disp_message(hWVision, "D:" + Math.Round((double)hv_CenterRadius * cpxm * 2, 5) + "mm", "", 1650, 150, "green", "false");
            HD.disp_message(hWVision, "Diam_Min:" + Math.Round(diam_min, 5) + "mm", "", 1800, 150, "green", "false");
        }
        #endregion
        private void btnQESave_Click(object sender, EventArgs e)
        {
            if ((Coatcheck.Checked & SetNum == "6" & txtDiamMin.Text == "") ||
                (Coatcheck2.Checked & (SetNum == "2" || SetNum == "4") & txtDiamMin2.Text == ""))
            {
                MessageBox.Show("还未获得Diam_Min值，是否继续保存？", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //return;
            }
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1-" + area4; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1-" + area4; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
               
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area2, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueCIMode", cBGOutMode.Value.ToString(), FrmMain.propath);
            if (cBGOutMode.Value == 1)
            {
                iniFile.Write(CCDNAME, "GlueCIChecked", GlueOutcheck.Checked.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIAimCirRadius", txtAimCirR.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIOutCirRadius", txtOutCirR.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIOutCir2RChecked", cBAVIR2.Checked.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIOutCir2Radius", txtOutCirR2.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIOutCirRadiusPlus", txtOutP.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIOutWidth", UDGlueOutWidth.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIOutGray", UDGlueOutGray.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIOutAreamax", txtGlueOutAreaMax.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueCIPixel", (txtPpix.Text).ToString(), FrmMain.propath);
                if (CCDNAME == "PCCD2-PickUp")
                    iniFile.Write(CCDNAME, "PickUpAVI", cBAVI.Checked.ToString(), FrmMain.propath);

                iniFile.Write(CCDNAME, "CoatCIChecked", Coatcheck.Checked.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "CoatRingRadius", (tBCoatRadius.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "CoatRingWidth", (tBCoatWidth.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "CoatTransition", (string)hv_transition, FrmMain.propath);
                iniFile.Write(CCDNAME, "CoatRmin", txtCoatRMin.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "CoatRmax", txtCoatRMax.Text, FrmMain.propath);
                //iniFile.Write(CCDNAME, "CoatOffset", txtOffset.Text, FrmMain.propath);
                if (hv_transition == "negative")
                    iniFile.Write(CCDNAME, "CoatRingThreshold", (tBCoatB2W.Value).ToString(), FrmMain.propath);
                if (hv_transition == "positive")
                    iniFile.Write(CCDNAME, "CoatRingThreshold", (tBCoatW2B.Value).ToString(), FrmMain.propath);

                iniFile.Write(CCDNAME, "DiamMinMax", txtDMinmax.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "DiamMinMin", txtDMinmin.Text, FrmMain.propath);
            }
            if (cBGOutMode.Value == 2)
            {
                iniFile.Write(CCDNAME, "HCoatCIChecked", Coatcheck2.Checked.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LensAimR", txtLensAimR.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "LensPix", txtLenspix.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "HoleRingRadius", (tBHoleRadius.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "HoleRingWidth", (tBHoleWidth.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "HoleTransition", (string)hv_Holetransition, FrmMain.propath);
                if (hv_Holetransition == "negative")
                    iniFile.Write(CCDNAME, "HoleRingThreshold", (tBHoleB2W.Value).ToString(), FrmMain.propath);
                if (hv_Holetransition == "positive")
                    iniFile.Write(CCDNAME, "HoleRingThreshold", (tBHoleW2B.Value).ToString(), FrmMain.propath);

                iniFile.Write(CCDNAME, "HCoatRingRadius", (tBCoatRadius2.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "HCoatRingWidth", (tBCoatWidth2.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "HCoatTransition", (string)hv_transition, FrmMain.propath);
                iniFile.Write(CCDNAME, "HCoatRmin", txtCoatRMin2.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "HCoatRmax", txtCoatRMax2.Text, FrmMain.propath);
                //iniFile.Write(CCDNAME, "HCoatOffset", txtOffset2.Text, FrmMain.propath);
                if (hv_transition == "negative")
                    iniFile.Write(CCDNAME, "HCoatRingThreshold", (tBCoatB2W2.Value).ToString(), FrmMain.propath);
                if (hv_transition == "positive")
                    iniFile.Write(CCDNAME, "HCoatRingThreshold", (tBCoatW2B2.Value).ToString(), FrmMain.propath);

                iniFile.Write(CCDNAME, "HCDiamMinMax", txtDMinmax2.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "HCDiamMinMin", txtDMinmin2.Text, FrmMain.propath);
            }
        }
        //AVIshow
        private void cBAVI_CheckedChanged(object sender, EventArgs e)
        {
            PCCD2.isPUAVI = cBAVI.Checked;
        }
       
        //胶宽
        HObject ho_gGreyUnion = new HObject(), ho_BinImage = new HObject();
        HTuple hv_RowIn = new HTuple(), hv_ColumnIn = new HTuple(), hv_RowOut = new HTuple(), hv_ColumnOut = new HTuple();
        HTuple hv_Distance = new HTuple(), hv_Max = new HTuple(), hv_Length = new HTuple(), hv_k_m = new HTuple(), hv_m = new HTuple();
        HTuple hv_SpaceDis = new HTuple();
        private void btngWidth_Click(object sender, EventArgs e)
        {
            if (txtCirRadius.Text != "")
            {
                Glue.Gxpm = Math.Round(double.Parse(txtCirRadius.Text) / (double)hv_CenterRadius / 2, 10);
                txtCirPixel.Text = Glue.Gxpm.ToString();
            }
            if (ho_g1Grey == null)
                return;
            try
            {
                //ho_gGreyUnion.Dispose();
                //HOperatorSet.Union2(ho_g1Grey, ho_g2Grey, out ho_gGreyUnion);
                ho_BinImage.Dispose();
                HOperatorSet.RegionToBin(ho_gGreyUnion, out ho_BinImage, 255, 0, width, height);

                hv_SpaceDis = 2 * Math.PI * hv_MidCirRadius / 30;
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.GenCircle(out ho_Circle, hv_rowFCenter, hv_colFCenter, hv_MidCirRadius);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour, hv_MetrologyHandle, "all", 1.5);

                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", RegionWidth + 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_distance", hv_SpaceDis);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", 100);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_BinImage, hv_MetrologyHandle);
                ho_ContourIn.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_ContourIn, hv_MetrologyHandle, "all", "all", out hv_RowIn, out hv_ColumnIn);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);

                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.GenCircle(out ho_Circle, hv_rowFCenter, hv_colFCenter, hv_MidCirRadius);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour, hv_MetrologyHandle, "all", 1.5);

                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", RegionWidth + 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_distance", hv_SpaceDis);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", 100);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_BinImage, hv_MetrologyHandle);
                ho_ContourOut.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_ContourOut, hv_MetrologyHandle, "all", "all", out hv_RowOut, out hv_ColumnOut);

                HOperatorSet.DistancePp(hv_RowIn, hv_ColumnIn, hv_RowOut, hv_ColumnOut, out hv_Distance);
                HOperatorSet.TupleMax(hv_Distance, out hv_Max);
                HOperatorSet.TupleLength(hv_Distance, out hv_Length);
                hv_k_m = 0;
                HTuple end_val187 = hv_Length - 1;
                HTuple step_val187 = 1;
                for (hv_m = 0; hv_m.Continue(end_val187, step_val187); hv_m = hv_m.TupleAdd(step_val187))
                {
                    if ((int)(new HTuple(((hv_Distance.TupleSelect(hv_m))).TupleEqual(hv_Max))) != 0)
                    {
                        hv_k_m = hv_m.Clone();
                    }
                }
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                hWVision.DispObj(ho_ImageTest);
                hWVision.SetLineWidth(1);
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                hWVision.DispObj(ho_g1Rectangle);  //检测区域
                hWVision.DispObj(ho_g2Rectangle);
                hWVision.SetColor("blue");
                hWVision.DispObj(ho_gGreyUnion);
                //hWVision.DispObj(ho_g1Grey);
                //hWVision.DispObj(ho_g2Grey);
                HD.set_display_font(hWVision, 16, "sans", "false", "false");
                //HD.disp_message(hWVision, "1中标示面积" + Math.Round((double)hv_g1area).ToString() + "(" + Math.Round((double)hv_g1Regionarea).ToString() + ")", "", 100, 150, "green", "false");
                //HD.disp_message(hWVision, "2中标示面积" + Math.Round((double)hv_g2area).ToString() + "(" + Math.Round((double)hv_g2Regionarea).ToString() + ")", "", 200, 150, "green", "false");
                HD.disp_message(hWVision, "總面积" + Math.Round(hv_GlueArea.D).ToString(), "", 100, 150, "green", "false");
                HD.disp_message(hWVision, "GlueWidth_Max:" + hv_Max + "(" + Math.Round((double)hv_Max * Glue.Gxpm, 3).ToString() + ")", "", 300, 150, "green", "false");
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
                hWVision.SetColor("green");
                hWVision.DispLine(hv_RowIn, hv_ColumnIn, hv_RowOut, hv_ColumnOut);
                hWVision.DispCross(hv_RowIn, hv_ColumnIn, 10, 0);
                hWVision.DispCross(hv_RowOut, hv_ColumnOut, 10, 0);
                hWVision.SetColor("red");
                hWVision.DispLine(hv_RowIn.TupleSelect(hv_k_m), hv_ColumnIn.TupleSelect(hv_k_m), hv_RowOut.TupleSelect(hv_k_m), hv_ColumnOut.TupleSelect(hv_k_m));
            }
            catch
            {
            }
        }
        private void cBGlueWidth_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGlueWidth.Checked)
                groupBox9.Enabled = true;
            if (!cBGlueWidth.Checked)
                groupBox9.Enabled = false;
        }
        //内外径
        HObject ho_RInCir = new HObject(), ho_ROutCir = new HObject();
        HTuple hv_RowInCenter = new HTuple(), hv_ColInCenter = new HTuple(), hv_RadiusInCenter = new HTuple();
        HTuple hv_RowOutCenter = new HTuple(), hv_ColOutCenter = new HTuple(), hv_RadiusOutCenter = new HTuple();
        private void cBIandORadius_CheckedChanged(object sender, EventArgs e)
        {
            if (cBIandORadius.Checked)
                gBIOradius.Enabled = true;
            if (!cBIandORadius.Enabled)
                gBIOradius.Enabled = false;
        }
        private void btnIORadius_Click(object sender, EventArgs e)
        {
            if (txtCirRadius.Text != "")
            {
                Glue.Gxpm = Math.Round(double.Parse(txtCirRadius.Text) / (double)hv_CenterRadius / 2, 10);
                txtCirPixel.Text = Glue.Gxpm.ToString();
            }
            if (ho_g1Grey == null)
                return;
            try
            {
                //ho_gGreyUnion.Dispose();
                //HOperatorSet.Union2(ho_g1Grey, ho_g2Grey, out ho_gGreyUnion);
                ho_BinImage.Dispose();
                HOperatorSet.RegionToBin(ho_gGreyUnion, out ho_BinImage, 255, 0, width, height);

                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.GenCircle(out ho_Circle, hv_rowFCenter, hv_colFCenter, hv_MidCirRadius);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour, hv_MetrologyHandle, "all", 1.5);

                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", RegionWidth + 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", 100);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_BinImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_ROutCir.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_ROutCir, hv_MetrologyHandle, "all", "all", 1.5);
                Thread.Sleep(5);
                hv_RowOutCenter = hv_circleParameter.TupleSelect(0);
                hv_ColOutCenter = hv_circleParameter.TupleSelect(1);
                hv_RadiusOutCenter = hv_circleParameter.TupleSelect(2);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);

                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.GenCircle(out ho_Circle, hv_rowFCenter, hv_colFCenter, hv_MidCirRadius);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour, hv_MetrologyHandle, "all", 1.5);

                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", RegionWidth + 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", 100);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_BinImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_RInCir.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_RInCir, hv_MetrologyHandle, "all", "all", 1.5);
                Thread.Sleep(5);
                hv_RowInCenter = hv_circleParameter.TupleSelect(0);
                hv_ColInCenter = hv_circleParameter.TupleSelect(1);
                hv_RadiusInCenter = hv_circleParameter.TupleSelect(2);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                double DIO = Math.Round((double)hv_RadiusInCenter * Glue.Gxpm * 2 + Glue.Offset_InnerRadius, 3) + (Math.Round((double)hv_RadiusOutCenter * Glue.Gxpm * 2 + Glue.Offset_OuterRadius, 3) - Math.Round((double)hv_RadiusInCenter * Glue.Gxpm * 2 + Glue.Offset_InnerRadius, 3)) / 2;
                hWVision.DispObj(ho_ImageTest);
                hWVision.SetLineWidth(1);
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                hWVision.DispObj(ho_g1Rectangle);  //检测区域
                hWVision.DispObj(ho_g2Rectangle);
                hWVision.SetColor("blue");
                hWVision.DispObj(ho_gGreyUnion);
                //hWVision.DispObj(ho_g1Grey);
                //hWVision.DispObj(ho_g2Grey);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_RInCir);
                hWVision.DispObj(ho_ROutCir);
                HD.set_display_font(hWVision, 16, "sans", "false", "false");
                //HD.disp_message(hWVision, "1中标示面积" + Math.Round((double)hv_g1area).ToString() + "(" + Math.Round((double)hv_g1Regionarea).ToString() + ")", "", 100, 150, "green", "false");
                //HD.disp_message(hWVision, "2中标示面积" + Math.Round((double)hv_g2area).ToString() + "(" + Math.Round((double)hv_g2Regionarea).ToString() + ")", "", 200, 150, "green", "false");
                HD.disp_message(hWVision, "總面积" + Math.Round(hv_GlueArea.D).ToString(), "", 100, 150, "green", "false");
                HD.disp_message(hWVision, "DI:" + Math.Round((double)hv_RadiusInCenter * 2).ToString() + "(" + Math.Round((double)hv_RadiusInCenter * Glue.Gxpm * 2+Glue.Offset_InnerRadius, 3).ToString() + ")", "", 300, 150, "green", "false");
                HD.disp_message(hWVision, "DO:" + Math.Round((double)hv_RadiusOutCenter * 2).ToString() + "(" + Math.Round((double)hv_RadiusOutCenter * Glue.Gxpm * 2 + Glue.Offset_OuterRadius, 3).ToString() + ")", "", 400, 150, "green", "false");
                HD.disp_message(hWVision, "DI+(DO-DI)/2:" + DIO.ToString(), "", 500, 150, "green", "false");
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
            }
            catch
            {
            }
        }
        #endregion
        #endregion
        #region fuzhu
        HTuple hv_AddDeg = new HTuple(), hv_AddRegionDeg = new HTuple(), hv_AddDegPlus = new HTuple();
        HObject ho_AddRegion1 = new HObject(), ho_AddRegion2 = new HObject();
        HObject ho_AddConRegion1 = new HObject(), ho_AddConRegion2 = new HObject();
        HObject ho_AR11 = new HObject(), ho_AR12 = new HObject(), ho_AR21 = new HObject(), ho_AR22 = new HObject();
        HObject ho_AddContour11 = new HObject(), ho_AddContour12 = new HObject();
        HObject ho_AddContour21 = new HObject(), ho_AddContour22 = new HObject();
        HObject ho_Line1 = new HObject(), ho_Line2 = new HObject();
        HObject ho_AddShowRegion1 = new HObject(), ho_AddShowRegion2 = new HObject();
        HTuple hv_AddArea1 = new HTuple(), hv_AddRow1 = new HTuple(), hv_AddColumn1 = new HTuple();
        HTuple hv_AddArea2 = new HTuple(), hv_AddRow2 = new HTuple(), hv_AddColumn2 = new HTuple();
        HTuple hv_DisMin1 = new HTuple(), hv_DisMin11Min = new HTuple(), hv_DisMin11Max = new HTuple(), hv_DisMin12Min = new HTuple(), hv_DisMin12Max = new HTuple();
        HTuple hv_DisMin2 = new HTuple(), hv_DisMin21Min = new HTuple(), hv_DisMin21Max = new HTuple(), hv_DisMin22Min = new HTuple(), hv_DisMin22Max = new HTuple();
        HTuple hv_Add1Row1 = new HTuple(), hv_Add1Col1 = new HTuple(), hv_Add1Row2 = new HTuple(), hv_Add1Col2 = new HTuple();
        HTuple hv_Add2Row1 = new HTuple(), hv_Add2Col1 = new HTuple(), hv_Add2Row2 = new HTuple(), hv_Add2Col2 = new HTuple();
        PointF a11, a12, b11, b12, a21, a22, b21, b22;
        private void cBDisAdd_CheckedChanged(object sender, EventArgs e)
        {
            gBDisAdd.Enabled = cBDisAdd.Checked;
        }
        private void btnDisAdd_Click(object sender, EventArgs e)
        {
            if (SetNum == "8")
            {
                if (hv_ColCenter.Length == 0)
                {
                    if ((cBCutGQ.Checked || cBCutGH.Checked) & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                        MessageBox.Show("请先找到圆心和角度！");
                    else
                        MessageBox.Show("请先找到圆心！");
                    return;
                }
            }
            else
            {
                if (hv_ColCenter.Length == 0)
                {
                    if (cBCut.Checked & (hv_Deg2.Length == 0 || hv_Deg2.D == 720.0))
                        MessageBox.Show("请先找到圆心和角度！");
                    else
                        MessageBox.Show("请先找到圆心！");
                    return;
                }
            }
            if (hv_g2row == null || hv_g2row.Length == 0)
            {
                MessageBox.Show("请先找到点胶区域，即完成1,2步骤！");
            }
            ShowAddFang();
        }
        void ShowAddFang()
        {
            if (readpara)
                return;
            try
            {
                int i = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
                if (!(hv_AddDeg == null || hv_AddDeg == 720.0 || hv_AddDeg.Length == 0))
                {
                    hv_AddRegionDeg = (HTuple)UDAddAngle.Value;
                    hv_AddDegPlus = (HTuple)UDAddAnglePlus.Value;
                    hv_g1angle = (Math.PI / 180) * (hv_AddDeg + hv_AddRegionDeg);
                    hv_g1length1 = (HTuple)UDAddlen1.Value;
                    hv_g1length2 = (HTuple)UDAddlen2.Value;
                    hv_grayDistance = (HTuple)UDAddDis.Value;
                    ho_ImageTest.Dispose();
                    ho_ImageTest = ho_ImageSet;
                    double distance = hv_grayDistance;
                    //double k1 = (hv_ColCenter - hv_g1ColumnCh) * 1.0 / (hv_RowCenter - hv_g1RowCh);// 坐标直线斜率k
                    double k1 = Math.Tan(Math.PI * (hv_AddDeg + hv_AddDegPlus) / 180);
                    cenPoint.X = hv_RowCenter[0].F; cenPoint.Y = hv_ColCenter[0].F;
                    GetPointXY(cenPoint, distance, k1, ref g2Point, ref g4Point);
                    hv_g1RowCh = g2Point.X;
                    hv_g1ColumnCh = g2Point.Y;
                    hv_g2RowCh = g4Point.X;
                    hv_g2ColumnCh = g4Point.Y;

                    //ho_gGreyUnion.Dispose();
                    //HOperatorSet.Union2(ho_g1Grey, ho_g2Grey, out ho_gGreyUnion);
                    ho_BinImage.Dispose();
                    HOperatorSet.RegionToBin(ho_gGreyUnion, out ho_BinImage, 255, 0, width, height);

                    #region 区域1
                    ho_g1Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g1Rectangle, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g1Region, hv_g1RowCh, hv_g1ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g1Reduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_BinImage, ho_g1Region, out ho_g1Reduced);
                    #endregion
                    #region 区域2
                    ho_g2Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_g2Rectangle, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Region.Dispose();
                    HOperatorSet.GenRectangle2(out ho_g2Region, hv_g2RowCh, hv_g2ColumnCh, hv_g1angle, hv_g1length1, hv_g1length2);
                    ho_g2Reduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_BinImage, ho_g2Region, out ho_g2Reduced);
                    #endregion

                    hWVision.DispObj(ho_ImageTest);
                    hWVision.SetColor("red");
                    hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                    hWVision.DispObj(ho_g1Rectangle);  //检测区域
                    hWVision.DispObj(ho_g2Rectangle);
                    HD.set_display_font(hWVision, 18, "sans", "true", "false");
                    HD.disp_message(hWVision, "1", "", hv_g1RowCh, hv_g1ColumnCh, "green", "false");
                    HD.disp_message(hWVision, "2", "", hv_g2RowCh, hv_g2ColumnCh, "green", "false");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tBAddAngle_ValueChanged(object sender, EventArgs e)
        {
            UDAddAngle.Value = tBAddAngle.Value;
            ShowAddFang();
        }
        private void UDAddAngle_ValueChanged(object sender, EventArgs e)
        {
            tBAddAngle.Value = (int)UDAddAngle.Value;
        }
        private void tBAddAnglePlus_ValueChanged(object sender, EventArgs e)
        {
            UDAddAnglePlus.Value = tBAddAnglePlus.Value;
            ShowAddFang();
        }
        private void UDAddAnglePlus_ValueChanged(object sender, EventArgs e)
        {
            tBAddAnglePlus.Value = (int)UDAddAnglePlus.Value;
        }
        private void tBAddDis_ValueChanged(object sender, EventArgs e)
        {
            UDAddDis.Value = (HTuple)tBAddDis.Value;
            ShowAddFang();
        }
        private void UDAddDis_ValueChanged(object sender, EventArgs e)
        {
            tBAddDis.Value = (int)UDAddDis.Value;
        }
        private void tBAddlen1_ValueChanged(object sender, EventArgs e)
        {
            UDAddlen1.Value = tBAddlen1.Value;
            ShowAddFang();
        }
        private void UDAddlen1_ValueChanged(object sender, EventArgs e)
        {
            tBAddlen1.Value = (int)UDAddlen1.Value;
        }
        private void tBAddlen2_ValueChanged(object sender, EventArgs e)
        {
            UDAddlen2.Value = tBAddlen2.Value;
            ShowAddFang();
        }
        private void UDAddlen2_ValueChanged(object sender, EventArgs e)
        {
            tBAddlen2.Value = (int)UDAddlen2.Value;
        }
        private void btnCalAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region 区域1
                HOperatorSet.Threshold(ho_g1Reduced, out ho_AddRegion1, 128, 255);
                HOperatorSet.Connection(ho_AddRegion1, out  ho_AddConRegion1);
                HOperatorSet.AreaCenter(ho_AddConRegion1, out hv_AddArea1, out hv_AddRow1, out hv_AddColumn1);
                if (hv_AddArea1.Length <= 1)
                {
                    MessageBox.Show("所找端点个数不足，请重新设置区域参数！");
                    return;
                }
                #region
                int max1, max2;
                if (hv_AddArea1[0] > hv_AddArea1[1])
                {
                    max1 = hv_AddArea1[0];
                    HOperatorSet.SelectObj(ho_AddConRegion1, out ho_AR11, 1);
                    max2 = hv_AddArea1[1];
                    HOperatorSet.SelectObj(ho_AddConRegion1, out ho_AR12, 2);
                }
                else
                {
                    max1 = hv_AddArea1[1];
                    HOperatorSet.SelectObj(ho_AddConRegion1, out ho_AR11, 2);
                    max2 = hv_AddArea1[0];
                    HOperatorSet.SelectObj(ho_AddConRegion1, out ho_AR12, 1);
                }
                for (int i = 2; i < hv_AddArea1.Length; i++)
                {
                    if (hv_AddArea1[i] > max1)
                    {
                        max2 = max1;
                        ho_AR12 = ho_AR11;
                        max1 = hv_AddArea1[i];
                        HOperatorSet.SelectObj(ho_AddConRegion1, out ho_AR11, i + 1);
                    }
                    else if (hv_AddArea1[i] > max2)
                    {
                        max2 = hv_AddArea1[i];
                        HOperatorSet.SelectObj(ho_AddConRegion1, out ho_AR12, i + 1);
                    }
                }
                #endregion
                HOperatorSet.GenContourRegionXld(ho_AR11, out ho_AddContour11, "border");
                HOperatorSet.GenContourRegionXld(ho_AR12, out ho_AddContour12, "border");
                HOperatorSet.DistanceCcMinPoints(ho_AddContour11, ho_AddContour12, "fast_point_to_segment", out hv_DisMin1,
                                                 out hv_Add1Row1, out hv_Add1Col1, out hv_Add1Row2, out hv_Add1Col2);
                a11.X = (float)((double)hv_Add1Col1);
                a11.Y = (float)((double)hv_Add1Row1);
                a12.X = (float)((double)hv_Add1Col2);
                a12.Y = (float)((double)hv_Add1Row2);

                b11.X = (float)((double)hv_ColCenter);
                b11.Y = (float)((double)hv_RowCenter);
                b12.X = ((float)((double)hv_Add1Col1) + (float)((double)hv_Add1Col2)) / 2;
                b12.Y = ((float)((double)hv_Add1Row1) + (float)((double)hv_Add1Row2)) / 2;

                var A = Math.Atan2(a12.Y - a11.Y, a12.X - a11.X);
                var B = Math.Atan2(b12.Y - b11.Y, b12.X - b11.X);
                var C = (180 * (float)(B - A)) / Math.PI;

                HOperatorSet.GenRegionLine(out ho_Line1, b11.Y, b11.X, b12.Y, b12.X);
                HOperatorSet.DistancePr(ho_Line1, hv_Add1Row1, hv_Add1Col1, out hv_DisMin11Min, out hv_DisMin11Max);
                HOperatorSet.DistancePr(ho_Line1, hv_Add1Row2, hv_Add1Col2, out hv_DisMin12Min, out hv_DisMin12Max);
                double am1 = (double)(hv_DisMin11Min + hv_DisMin12Min);
                HOperatorSet.GenRectangle2ContourXld(out ho_AddShowRegion1, b12.Y, b12.X, C, am1, 30);
                hv_DisMin1 = am1 * GCCD2.xpm;
                #endregion
                #region 区域2
                HOperatorSet.Threshold(ho_g2Reduced, out ho_AddRegion2, 128, 255);
                HOperatorSet.Connection(ho_AddRegion2, out  ho_AddConRegion2);
                HOperatorSet.AreaCenter(ho_AddConRegion2, out hv_AddArea2, out hv_AddRow2, out hv_AddColumn2);
                if (hv_AddArea2.Length <= 1)
                {
                    MessageBox.Show("所找端点个数不足，请重新设置区域参数！");
                    return;
                }
                #region
                if (hv_AddArea2[0] > hv_AddArea2[1])
                {
                    max1 = hv_AddArea2[0];
                    HOperatorSet.SelectObj(ho_AddConRegion2, out ho_AR21, 1);
                    max2 = hv_AddArea2[1];
                    HOperatorSet.SelectObj(ho_AddConRegion2, out ho_AR22, 2);
                }
                else
                {
                    max1 = hv_AddArea2[1];
                    HOperatorSet.SelectObj(ho_AddConRegion2, out ho_AR21, 2);
                    max2 = hv_AddArea2[0];
                    HOperatorSet.SelectObj(ho_AddConRegion2, out ho_AR22, 1);
                }
                for (int i = 2; i < hv_AddArea2.Length; i++)
                {
                    if (hv_AddArea2[i] > max1)
                    {
                        max2 = max1;
                        ho_AR22 = ho_AR21;
                        max1 = hv_AddArea2[i];
                        HOperatorSet.SelectObj(ho_AddConRegion2, out ho_AR21, i + 1);
                    }
                    else if (hv_AddArea2[i] > max2)
                    {
                        max2 = hv_AddArea2[i];
                        HOperatorSet.SelectObj(ho_AddConRegion2, out ho_AR22, i + 1);
                    }
                }
                #endregion
                HOperatorSet.GenContourRegionXld(ho_AR21, out ho_AddContour21, "border");
                HOperatorSet.GenContourRegionXld(ho_AR22, out ho_AddContour22, "border");
                HOperatorSet.DistanceCcMinPoints(ho_AddContour21, ho_AddContour22, "fast_point_to_segment", out hv_DisMin2,
                                                 out hv_Add2Row1, out hv_Add2Col1, out hv_Add2Row2, out hv_Add2Col2);
                a21.X = (float)((double)hv_Add2Col1);
                a21.Y = (float)((double)hv_Add2Row1);
                a22.X = (float)((double)hv_Add2Col2);
                a22.Y = (float)((double)hv_Add2Row2);

                b21.X = (float)((double)hv_ColCenter);
                b21.Y = (float)((double)hv_RowCenter);
                b22.X = ((float)((double)hv_Add2Col1) + (float)((double)hv_Add2Col2)) / 2;
                b22.Y = ((float)((double)hv_Add2Row1) + (float)((double)hv_Add2Row2)) / 2;

                A = Math.Atan2(a22.Y - a21.Y, a22.X - a21.X);
                B = Math.Atan2(b22.Y - b21.Y, b22.X - b21.X);
                C = (180 * (float)(B - A)) / Math.PI;

                HOperatorSet.GenRegionLine(out ho_Line2, b21.Y, b21.X, b22.Y, b22.X);
                HOperatorSet.DistancePr(ho_Line2, hv_Add2Row1, hv_Add2Col1, out hv_DisMin21Min, out hv_DisMin21Max);
                HOperatorSet.DistancePr(ho_Line2, hv_Add2Row2, hv_Add2Col2, out hv_DisMin22Min, out hv_DisMin22Max);
                double am2 = (double)(hv_DisMin21Min + hv_DisMin22Min);
                HOperatorSet.GenRectangle2ContourXld(out ho_AddShowRegion2, b22.Y, b22.X, C, am2, 30);
                hv_DisMin2 = am2 * GCCD2.xpm;
                #endregion

                hWVision.DispObj(ho_ImageTest);
                hWVision.SetColor("red");
                hWVision.SetLineWidth(1);
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                hWVision.DispObj(ho_g1Rectangle);  //检测区域
                hWVision.DispObj(ho_g2Rectangle);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_AddContour11);
                hWVision.DispObj(ho_AddContour12);
                hWVision.DispObj(ho_AddContour21);
                hWVision.DispObj(ho_AddContour22);
                HOperatorSet.DispLine(hWVision, hv_Add1Row1, hv_Add1Col1, hv_Add1Row2, hv_Add1Col2);
                HOperatorSet.DispLine(hWVision, hv_Add2Row1, hv_Add2Col1, hv_Add2Row2, hv_Add2Col2);
                //HOperatorSet.DispLine(hWVision, b11.Y, b11.X, b12.Y, b12.X);
                //HOperatorSet.DispLine(hWVision, b21.Y, b21.X, b22.Y, b22.X);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, Math.Round((double)hv_DisMin1, 3), "", hv_g1RowCh, hv_g1ColumnCh, "blue", "false");
                HD.disp_message(hWVision, Math.Round((double)hv_DisMin2, 3), "", hv_g2RowCh, hv_g2ColumnCh, "blue", "false");

            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        #endregion
        private void tBGlueGray_ValueChanged(object sender, EventArgs e)
        {
            glueGray = tBGlueGray.Value;
            UDGlueGray.Value = glueGray;
            if (cBglueChose.SelectedIndex == 0)
                ShowGlueFang();
            if (cBglueChose.SelectedIndex == 1)
            {
                try
                {
                    int i = int.Parse(SetNum) - 1;
                    ho_ImageSet.Dispose();
                    HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_ImageSet);
                    ShowGlueSectorGray();
                }
                catch
                {
                }
            }
        }
        private void UDGlueGray_ValueChanged(object sender, EventArgs e)
        {
            glueGray = (HTuple)UDGlueGray.Value;
            tBGlueGray.Value = glueGray;
        }
        private void txtGlueMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void btnGlueSave_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1-" + area4; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1-" + area4; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
               
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area2, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueChecked", Gluecheck.Checked.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueMode", cBglueChose.SelectedIndex.ToString(), FrmMain.propath);
            if (cBglueChose.SelectedIndex == 0)
            {
                if (hv_g1length1 != 0)
                {
                    iniFile.Write(CCDNAME, "GlueFDegPlue", tBFDegPlus.Value.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "GlueCenDistance", tBglueDis.Value.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "GlueLength", tBgluelen1.Value.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "GlueWidth", tBgluelen2.Value.ToString(), FrmMain.propath);
                }
                else
                    MessageBox.Show("还未设置参数！");
            }
            if (cBglueChose.SelectedIndex == 1)
            {
                iniFile.Write(CCDNAME, "GlueEdgeChecked", cBGlueEdge.Checked.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueColorChecked", cBGlueRegionColor.Checked.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueRingRadius", UDglueR.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueRingWidth", UDglueW.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueStartA", UDglueStartA.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueEndA", UDglueEndA.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueEdgeMin", tBLimitIn.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueEdgeMax", tBLimitOut.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "GlueAngleRatio", Glue.GlueAngleRatio.ToString(), FrmMain.propath);
            }
            if (cBglueChose.SelectedIndex == 1)
            {
                if (Glue.Glue_Circle_2)
                {
                    iniFile.Write(CCDNAME, "Glue_Circle_2", Glue.Glue_Circle_2.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "Glue_Circle_OuterRadius_2", Glue.Glue_Circle_OuterRadius_2.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "Glue_Circle_InnerRadius_2", Glue.Glue_Circle_InnerRadius_2.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "Glue_Circle_StartAngle_2", Glue.Glue_Circle_StartAngle_2.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "Glue_Circle_EndAngle_2", Glue.Glue_Circle_EndAngle_2.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "Glue_Circle_Gray_2", Glue.Glue_Circle_Gray_2.ToString(), FrmMain.propath);
                    iniFile.Write(CCDNAME, "GlueAngleRatio_2", Glue.GlueAngleRatio_2.ToString(), FrmMain.propath);
                }
            }
            iniFile.Write(CCDNAME, "GlueGray", tBGlueGray.Value.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueAreaMin", txtGlueMin.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueAreaMax", txtGlueMax.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueEdgeInAreaMax", txtInLimitMin.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueEdgeOutAreaMax", txtOutLimitMin.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueWidthIschecked", cBGlueWidth.Checked.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueWidthRradius", txtCirRadius.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueWidthPixel", txtCirPixel.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueWidthMax", txtWidthMax.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueInOutRadius", cBIandORadius.Checked.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueInRadiusMin", txtIRmin.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "GlueOutRadiusMax", txtORMax.Text, FrmMain.propath);
            string DisAddChecked = (cBDisAdd.Checked ? "True" : "False");
            iniFile.Write(CCDNAME, "DisAddChecked", DisAddChecked, FrmMain.propath);
            if (DisAddChecked == "True")
            {
                iniFile.Write(CCDNAME, "DisAddDegree", tBAddAngle.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "DisAddDegreePlus", tBAddAnglePlus.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "DisAddSetDis", tBAddDis.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "DisAddLength", tBAddlen1.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "DisAddWidth", tBAddlen2.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "DisAddDisMin", txtDisMin.Text, FrmMain.propath);
                iniFile.Write(CCDNAME, "DisAddDisMax", txtDisMax.Text, FrmMain.propath);
            }
            iniFile.Write(CCDNAME, "GlueBreakChecked", cBGlueBreaked.Checked.ToString(), FrmMain.propath);

            //膠水內外徑補償
            iniFile.Write(CCDNAME, "Offset_InnerRadius", Glue.Offset_InnerRadius.ToString(), FrmMain.propath);
            iniFile.Write(CCDNAME, "Offset_OuterRadius", Glue.Offset_OuterRadius.ToString(), FrmMain.propath);
        }
        #endregion

        #region Fang
        //private void btnLocate_Click(object sender, EventArgs e)
        //{
        //    int i = int.Parse(SetNum) - 1;
        //    ho_ImageSet.Dispose();
        //    HOperatorSet.CopyImage(halcon.Image[i], out ho_ImageSet);
        //    HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
        //    HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
        //    HD.disp_message(hWVision, "点击鼠标左键画检测区域,点击右键确认", "", 100, 100, "green", "false");
        //    btnLocate.BackColor = Color.GreenYellow;
        //    hWVision.SetColor("red");
        //    HOperatorSet.DrawRectangle2(hWVision, out hv_RowCh, out hv_ColumnCh, out hv_angle, out hv_length1, out hv_length2);
        //    btnLocate.BackColor = Color.WhiteSmoke;
        //    ShowFRegion();
        //}
        private void btnSLine_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (tabProcessMode.SelectedIndex == 0)
                i = int.Parse(ViewNum) - 1;
            if (tabProcessMode.SelectedIndex == 1)
                i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            //if (hv_FModelID.Length == 0 || hv_FModelID == null)
            //     HOperatorSet.ReadShapeModel(Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME + "_RegionModel.shm", out hv_FModelID);
            HD.disp_message(hWVision, "点击鼠标左键画检测区域,点击右键确认", "", 100, 100, "green", "false");
            btnSLine.BackColor = Color.GreenYellow;
            hWVision.SetColor("red");
            HOperatorSet.DrawRectangle2(hWVision, out hv_RowCh, out hv_ColumnCh, out hv_angle, out hv_length1, out hv_length2);
            btnSLine.BackColor = Color.WhiteSmoke;
            //hv_angle = 0.0;
            try
            {
                CreateModel_FigureShape_Square();
            }
            catch (Exception ER)
            {
                MessageBox.Show(ER.ToString());
            }
        }
        private void tBSWidth_ValueChanged(object sender, EventArgs e)
        {
            hv_IRWidth = tBSWidth.Value;
            UDSWidth.Value = hv_IRWidth;
        }
        private void UDSWidth_ValueChanged(object sender, EventArgs e)
        {
             hv_IRWidth = (int)UDSWidth.Value;
            tBSWidth.Value = hv_IRWidth;
            try
            {
                ShowFang();
            }
            catch
            { 
            }
        }
       
        private void tBSB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_IRTH = tBSB2W.Value;
            UDSB2W.Value = hv_IRTH;
        }
        private void UDSB2W_ValueChanged(object sender, EventArgs e)
        {
            hv_IRtransition = "negative";
            hv_IRTH = (int)UDSB2W.Value;
            tBSB2W.Value = hv_IRTH;
            try
            {
                ShowFang();
            }
            catch
            {
            }
            if ((int)UDSB2W.Value != 1)
                UDSW2B.Value = 255;
        }
        private void tBSW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_IRTH = tBSW2B.Value;
            UDSW2B.Value = hv_IRTH;
        }
        private void UDSW2B_ValueChanged(object sender, EventArgs e)
        {
            hv_IRtransition = "positive";
            hv_IRTH = (int)UDSW2B.Value;
            tBSW2B.Value = hv_IRTH;
            try
            {
                ShowFang();
            }
            catch
            {
            }
            if ((int)UDSW2B.Value != 255)
                UDSB2W.Value = 1;
        }
        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cb.SelectedIndex)
            {
                case 0: hv_Select = "first"; break;
                case 1: hv_Select = "last";  break;
            }
            try
            {
                ShowFang();
            }
            catch
            { }
        }

        private void btnCirAng_Click(object sender, EventArgs e)
        {
            try
            {
                string CCDNAME = ""; string area1 = "", area2 = "", area4 = "";
                if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                     (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                     (cBLocation4.Enabled && cBLocation4.Text == "") ||
                                     (cBtest.Enabled && cBtest.Text == ""))
                    return;
                if (cBLocation.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation.SelectedIndex == 1)
                    area1 = "Platform";
                area2 = (cBLocation2.SelectedIndex + 1).ToString();
                area4 = (cBLocation4.SelectedIndex + 1).ToString();
                if (SetNum == "6")
                {
                    if (cBLocation3.SelectedIndex == 0)
                        area1 = "PickUp";
                    if (cBLocation3.SelectedIndex == 1)
                        area1 = "Platform1";
                    if (cBLocation3.SelectedIndex == 2)
                        area1 = "Platform2";
                }
                switch (int.Parse(SetNum))
                {
                    case 1: CCDNAME = "A1CCD1-" + area4; break;
                    case 2: CCDNAME = "A1CCD2-" + area1; break;
                    case 3: CCDNAME = "A2CCD1_" + area4; break;
                    case 4: CCDNAME = "A2CCD2-" + area1; break;
                    case 5: CCDNAME = "PCCD1"; break;
                    case 6: CCDNAME = "PCCD2-" + area1; break;
                    case 7: CCDNAME = "GCCD1"; break;
                    case 8: CCDNAME = "GCCD2-" + area2; break;
                    case 9: CCDNAME = "QCCD"; break;
                }
                if (SetNum == "1" || SetNum == "3")
                    CCDNAME = CCDNAME + "-" + area4;
                if (hv_FModelID.Length == 0 || hv_FModelID == null)
                {
                    HOperatorSet.ReadShapeModel(Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME + "_RegionModel.shm", out hv_FModelID);
                    hv_RowCh = double.Parse(iniFile.Read(CCDNAME, "Frow", FrmMain.propath));
                    hv_ColumnCh = double.Parse(iniFile.Read(CCDNAME, "Fcol", FrmMain.propath));
                    hv_angle = double.Parse(iniFile.Read(CCDNAME, "Fangle", FrmMain.propath));
                    hv_length1 = double.Parse(iniFile.Read(CCDNAME, "Flength1", FrmMain.propath));
                    hv_length2 = double.Parse(iniFile.Read(CCDNAME, "Flength2", FrmMain.propath));
                    hv_IRWidth = int.Parse(iniFile.Read(CCDNAME, "FWidth", FrmMain.propath));
                    hv_IRdis = int.Parse(iniFile.Read(CCDNAME, "FDis", FrmMain.propath));
                    hv_IRtransition = iniFile.Read(CCDNAME, "Ftransition", FrmMain.propath);
                    hv_IRTH = int.Parse(iniFile.Read(CCDNAME, "Fthreshold", FrmMain.propath));
                    hv_Select =IniFile.Read(CCDNAME, "Fselect","last" , FrmMain.propath);
                }
                ShowFang();
                hv_Deg2 = hv_FDeg = hv_ResultPhi.TupleDeg();
                if (hv_ResultRow.Length != 0)
                {
                    hv_RowCenter =hv_ResultRow;
                    hv_ColCenter = hv_ResultColumn;
                    HTuple hv_r = hv_ResultRow;
                    HTuple hv_c = hv_ResultColumn;
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_ImageSet);
                    hWVision.SetColor("red");
                    hWVision.DispCircle(hv_r, hv_c, 8);
                    hWVision.DispCross(row, col, width, 0);
                    hWVision.SetColor("green");
                    hWVision.DispObj(ho_ResultContours);
                    HD.set_display_font(hWVision, 14, "sans", "true", "false");
                    HOperatorSet.SetTposition(hWVision, 150, 24);
                    HOperatorSet.WriteString(hWVision, ("矩形中心点坐标X= " + (hv_c - col) * xpm));
                    HOperatorSet.SetTposition(hWVision, 250, 24);
                    HOperatorSet.WriteString(hWVision, ("矩形中心点坐标Y= " + (-(hv_r - row)) * ypm));
                    HOperatorSet.SetTposition(hWVision, 350, 24);
                    HOperatorSet.WriteString(hWVision, (("矩形角度值Degree = ") + hv_FDeg) + "°");
                }
                else
                {
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_ImageSet);
                    HD.set_display_font(hWVision, 14, "sans", "true", "false");
                    HOperatorSet.SetTposition(hWVision, 150, 24);
                    HOperatorSet.WriteString(hWVision, "未找到中心，请重新调节参数！");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void btnSaveIR_Click(object sender, EventArgs e)
        {
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == "") ||
                                 (cBtest.Enabled && cBtest.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            if (SetNum == "6")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area2, FrmMain.propath);
            string sn = "";
            if (cbFigureShape.SelectedIndex == 0)
                sn = "Circle";
            if (cbFigureShape.SelectedIndex == 1)
                sn = "Square";
            iniFile.Write(CCDNAME, "FigureShape", sn, FrmMain.propath);
            //if (hv_FModelID != null & hv_FModelID.Length != 0)
            //{
            //    HOperatorSet.WriteShapeModel(hv_FModelID, Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME + "_RegionModel.shm");
            //    iniFile.Write(CCDNAME, "Frow", Math.Round((double)hv_RowCh, 4).ToString(), FrmMain.propath);
            //    iniFile.Write(CCDNAME, "Fcol", Math.Round((double)hv_ColumnCh, 4).ToString(), FrmMain.propath);
            //    iniFile.Write(CCDNAME, "Fangle", Math.Round((double)hv_angle, 4).ToString(), FrmMain.propath);
            //    iniFile.Write(CCDNAME, "Flength1", Math.Round((double)hv_length1, 4).ToString(), FrmMain.propath);
            //    iniFile.Write(CCDNAME, "Flength2", Math.Round((double)hv_length2, 4).ToString(), FrmMain.propath);
            //}
            iniFile.Write(CCDNAME, "FWidth", UDSWidth.Value.ToString(), FrmMain.propath);
            IniFile.Write(CCDNAME, "RectangleLength1_FigureShape", ucRectangleLength1_FigureShape.Value.ToString(), FrmMain.propath);
            IniFile.Write(CCDNAME, "RectangleLength2_FigureShape", ucRectangleLength2_FigureShape.Value.ToString(), FrmMain.propath);
            if (UDSB2W.Value != 1)
            {
                iniFile.Write(CCDNAME, "Ftransition", "negative", FrmMain.propath);
                iniFile.Write(CCDNAME, "Fthreshold", UDSB2W.Value.ToString(), FrmMain.propath);
            }
            if (UDSW2B.Value != 255)
            {
                iniFile.Write(CCDNAME, "Ftransition", "positive", FrmMain.propath);
                iniFile.Write(CCDNAME, "Fthreshold", UDSW2B.Value.ToString(), FrmMain.propath);
            }
            iniFile.Write(CCDNAME, "Fselect", hv_Select.ToString(), FrmMain.propath);
            #region 光源亮度
            if (SetNum == "1")
            {
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED1Lig.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LighterValue2", (UD_LED2Lig.Value).ToString(), FrmMain.propath);
            }
            if (SetNum == "2")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED3Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "3")
            {
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED4Lig.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LighterValue2", (UD_LED5Lig.Value).ToString(), FrmMain.propath);
            }
            if (SetNum == "4")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED6Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "5")
            {
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED7Lig.Value).ToString(), FrmMain.propath);
                //iniFile.Write(CCDNAME, "LighterValue2", (UD_LED8Lig.Value).ToString(), FrmMain.propath);  //调整至QCCD
            }
            if (SetNum == "6")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED9Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "7")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED10Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(CCDNAME, "LighterValue", (UD_LED11Lig.Value).ToString(), FrmMain.propath);
            if (SetNum == "9")
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED8Lig.Value).ToString(), FrmMain.propath);
            //iniFile.Write(CCDNAME, "LighterValue", (UD_LED12Lig.Value).ToString(), FrmMain.propath);
            #endregion
        }
        #endregion
        #endregion

        #region 进制转换及点计算
        public static string NToHString(int iNumber)//进制转换(有符号十进制转十六进制、高低位置换)
        {
            string strResult = string.Empty;
            if (iNumber < 0)
            {
                iNumber = -iNumber;
                string strNegate = string.Empty;
                char[] binChar = Convert.ToString(iNumber, 2).PadLeft(16, '0').ToArray();
                char[] b = new char[] { '0' };
                int charlens = binChar.Length;
                if (charlens < 32)
                {
                    for (int i = 0; i < charlens; i++)
                    {
                        binChar = b.Concat(binChar).ToArray();
                    }
                }
                foreach (char ch in binChar)
                {
                    if (Convert.ToInt32(ch) == 48)
                    {
                        strNegate += "1";
                    }
                    else
                    {
                        strNegate += "0";
                    }
                }
                int iComplement = Convert.ToInt32(strNegate, 2) + 1;
                strResult = Convert.ToString(iComplement, 16).ToUpper();
                while (strResult.Length < 8)
                {
                    strResult = 'F' + strResult;
                }
            }
            else
            {
                strResult = Convert.ToString(iNumber, 16).ToUpper();
                while (strResult.Length < 8)
                {
                    strResult = '0' + strResult;
                }
            }
            strResult = strResult.Substring(4, 4) + strResult.Substring(0, 4);
            return strResult;
        }
        public static int HToNInt(string str)//进制转换(十六进制转换十进制)
        {
            Int32[] Data = new Int32[] { };
            string S = "";
            int intResult = 0;
            Int32 Xnum = Convert.ToInt32(str, 16);
            string Xnum2 = Convert.ToString(Xnum, 2);
            for (int i = 0; i < Xnum2.Length; i++)
            {
                Data[i] = Convert.ToInt32(Xnum2.Substring(i, 1));
                if (Data[i] == 0)
                {
                    Data[i] = 1;
                }
                else
                {
                    Data[i] = 0;
                }
                S = S + Convert.ToString(Data[i]);
            }
            intResult = Convert.ToInt32(S, 2);
            return intResult;
        }
        public static bool IsNumber(string str)
        {
            bool blResult = true;
            if (str == "")
            {
                blResult = false;
            }
            else
            {
                foreach (char Char in str)
                {
                    if (!char.IsNumber(Char))
                    {
                        blResult = false;
                        break;
                    }
                }
            }
            return blResult;
        }
        public static void GetPointXY(PointF curPoint, double distance, double angle,
                               ref PointF nextPoint1, ref PointF nextPoint2)
        {
            double lenthUnit = distance / 5;// 单位长度
            // 第一步：求得直线方程相关参数y=kx+b
            double k = angle;// 坐标直线斜率k
            double b = curPoint.Y - k * curPoint.X;// 坐标直线b
            // 第二步：求得在直线y=kx+b上，距离当前坐标距离为L的某点
            // 一元二次方程Ax^2+Bx+C=0中,
            // 一元二次方程求根公式：
            // 两根x1,x2= [-B±√(B^2-4AC)]/2A
            // ①(y-y0)^2+(x-x0)^2=L^2;
            // ②y=kx+b;
            // 式中x,y即为根据以上lenthUnit单位长度(这里就是距离L)对应点的坐标
            // 由①②表达式得到:(k^2+1)x^2+2[(b-y0)k-x0]x+[(b-y0)^2+x0^2-L^2]=0
            double A = Math.Pow(k, 2) + 1;// A=k^2+1;
            double B = 2 * ((b - curPoint.Y) * k - curPoint.X);// B=2[(b-y0)k-x0];
            int m = 5;
            double L = m * lenthUnit;
            // C=(b-y0)^2+x0^2-L^2
            double C = Math.Pow(b - curPoint.Y, 2) + Math.Pow(curPoint.X, 2)
                    - Math.Pow(L, 2);
            // 两根x1,x2= [-B±√(B^2-4AC)]/2A
            double x1 = (-B + Math.Sqrt(Math.Pow(B, 2) - 4 * A * C)) / (2 * A);
            double x2 = (-B - Math.Sqrt(Math.Pow(B, 2) - 4 * A * C)) / (2 * A);
            if (x1 > x2)
            {
                double iv = x1;
                x1 = x2;
                x2 = iv;
            }
            double y1 = k * x1 + b;
            double y2 = k * x2 + b;
            nextPoint1 = new Point((int)x1, (int)y1);
            nextPoint2 = new Point((int)x2, (int)y2);
        }
        public void GetPointXY_2(PointF curPoint, double distance, double angle, double AngleIntersection,
                               ref PointF nextPoint1, ref PointF nextPoint2)
        {
            double lenthUnit = distance / 5;// 单位长度
            // 第一步：求得直线方程相关参数y=kx+b
            double k_1 = angle + AngleIntersection;// 坐标直线斜率k
            double b_1 = curPoint.Y - k_1 * curPoint.X;// 坐标直线b
            double k_2 = angle - AngleIntersection;// 坐标直线斜率k
            double b_2 = curPoint.Y - k_1 * curPoint.X;// 坐标直线b
            // 第二步：求得在直线y=kx+b上，距离当前坐标距离为L的某点
            // 一元二次方程Ax^2+Bx+C=0中,
            // 一元二次方程求根公式：
            // 两根x1,x2= [-B±√(B^2-4AC)]/2A
            // ①(y-y0)^2+(x-x0)^2=L^2;
            // ②y=kx+b;
            // 式中x,y即为根据以上lenthUnit单位长度(这里就是距离L)对应点的坐标
            // 由①②表达式得到:(k^2+1)x^2+2[(b-y0)k-x0]x+[(b-y0)^2+x0^2-L^2]=0
            double A_1 = Math.Pow(k_1, 2) + 1;// A=k^2+1;
            double B_1 = 2 * ((b_1 - curPoint.Y) * k_2 - curPoint.X);// B=2[(b-y0)k-x0];
            double A_2 = Math.Pow(k_2, 2) + 1;// A=k^2+1;
            double B_2 = 2 * ((b_2 - curPoint.Y) * k_2 - curPoint.X);// B=2[(b-y0)k-x0];
            int m = 5;
            double L = m * lenthUnit;
            // C=(b-y0)^2+x0^2-L^2
            double C_1 = Math.Pow(b_1 - curPoint.Y, 2) + Math.Pow(curPoint.X, 2)
                    - Math.Pow(L, 2);
            double C_2 = Math.Pow(b_2 - curPoint.Y, 2) + Math.Pow(curPoint.X, 2)
                    - Math.Pow(L, 2);
            // 两根x1,x2= [-B±√(B^2-4AC)]/2A
            double x1 = (-B_1 + Math.Sqrt(Math.Pow(B_1, 2) - 4 * A_1 * C_1)) / (2 * A_1);
            double x2 = (-B_2 - Math.Sqrt(Math.Pow(B_2, 2) - 4 * A_2 * C_2)) / (2 * A_2);
            if (x1 > x2)
            {
                double iv = x1;
                x1 = x2;
                x2 = iv;
            }
            double y1 = k_1 * x1 + b_1;
            double y2 = k_2 * x2 + b_2;
            nextPoint1 = new Point((int)x1, (int)y1);
            nextPoint2 = new Point((int)x2, (int)y2);
        }
        #endregion

        #region 光源调节
        private void Bar_LED1Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED1Lig.Value = Bar_LED1Lig.Value;
        }
        private void UD_LED1Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit2 = Bar_LED1Lig.Value = (int)UD_LED1Lig.Value;
            parent.ch2 = 0;
            parent.LightSet2();
        }
        private void Bar_LED2Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED2Lig.Value = Bar_LED2Lig.Value;
        }
        private void UD_LED2Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit2 = Bar_LED2Lig.Value = (int)UD_LED2Lig.Value;
            parent.ch2 = 1;
            parent.LightSet2();
        }

        private void Bar_LED3Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED3Lig.Value = Bar_LED3Lig.Value;
        }
        private void UD_LED3Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit2 = Bar_LED3Lig.Value = (int)UD_LED3Lig.Value;
            parent.ch2 = 2;
            parent.LightSet2();
        }

        private void Bar_LED4Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED4Lig.Value = Bar_LED4Lig.Value;
        }
        private void UD_LED4Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit2 = Bar_LED4Lig.Value = (int)UD_LED4Lig.Value;
            parent.ch2 = 3;
            parent.LightSet2();
        }
        private void Bar_LED5Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED5Lig.Value = Bar_LED5Lig.Value;
        }
        private void UD_LED5Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit2 = Bar_LED5Lig.Value = (int)UD_LED5Lig.Value;
            parent.ch2 = 4;
            parent.LightSet2();
        }

        private void Bar_LED6Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED6Lig.Value = Bar_LED6Lig.Value;
        }
        private void UD_LED6Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit2 = Bar_LED6Lig.Value = (int)UD_LED6Lig.Value;
            parent.ch2 = 5;
            parent.LightSet2();
        }

        private void Bar_LED7Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED7Lig.Value = Bar_LED7Lig.Value;
        }
        private void UD_LED7Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit = Bar_LED7Lig.Value = (int)UD_LED7Lig.Value;
            parent.ch = 0;
            parent.LightSet();
        }

        private void Bar_LED9Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED9Lig.Value = Bar_LED9Lig.Value;
        }
        private void UD_LED9Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit = Bar_LED9Lig.Value = (int)UD_LED9Lig.Value;
            parent.ch = 2;
            parent.LightSet();
        }

        //QCCD原光源LightSet通道ch = 3调整给BarcodeReader且QCCD调整为扫码时使用此通道

        private void Bar_LED10Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED10Lig.Value = Bar_LED10Lig.Value;
        }
        private void UD_LED10Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit = Bar_LED10Lig.Value = (int)UD_LED10Lig.Value;
            parent.ch = 4;
            parent.LightSet();
        }

        private void Bar_LED11Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED11Lig.Value = Bar_LED11Lig.Value;
        }
        private void UD_LED11Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit = Bar_LED11Lig.Value = (int)UD_LED11Lig.Value;
            parent.ch = 5;
            parent.LightSet();
        }

        //原PCCD1蓝光通道调整给QCCD做外观检测
        private void Bar_LED8Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED8Lig.Value = Bar_LED8Lig.Value;
        }
        private void UD_LED8Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit = Bar_LED8Lig.Value = (int)UD_LED8Lig.Value;
            parent.ch = 1;
            parent.LightSet();
        }

        #region 4通道控制器暂未用
        private void Bar_LED12Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED12Lig.Value = Bar_LED12Lig.Value;
        }
        private void UD_LED12Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit3 = Bar_LED12Lig.Value = (int)UD_LED12Lig.Value;
            parent.ch3 = 0;
            parent.LightSet3();
        }
        private void Bar_LED13Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED13Lig.Value = Bar_LED13Lig.Value;
        }
        private void UD_LED13Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit3 = Bar_LED13Lig.Value = (int)UD_LED13Lig.Value;
            parent.ch3 = 1;
            parent.LightSet3();
        }
        private void Bar_LED14Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED14Lig.Value = Bar_LED14Lig.Value;
        }
        private void UD_LED14Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit3 = Bar_LED14Lig.Value = (int)UD_LED14Lig.Value;
            parent.ch3 = 2;
            parent.LightSet3();
        }
        private void Bar_LED15Lig_ValueChanged(object sender, EventArgs e)
        {
            UD_LED15Lig.Value = Bar_LED15Lig.Value;
        }
        private void UD_LED15Lig_ValueChanged(object sender, EventArgs e)
        {
            parent.brit3 = Bar_LED15Lig.Value = (int)UD_LED15Lig.Value;
            parent.ch3 = 3;
            parent.LightSet3();
        }
        #endregion
        #endregion

        #region Button
        private void btnSet1Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet2Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet3Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet4Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet5Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet6Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet7Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet8Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet9Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet10Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet11Brit_Click(object sender, EventArgs e)
        {

        }
        private void btnSet12Brit_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 结果检测
        public double[] dx = new double[5]; public double[] dy = new double[5]; public double[] dis = new double[5];
        private void cBtest_SelectedIndexChanged(object sender, EventArgs e)
        {
            string CCDNAME = "";
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: CCDNAME = "A1CCD2"; xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: CCDNAME = "A2CCD1"; xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: CCDNAME = "A2CCD2"; xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: CCDNAME = "PCCD1"; xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: CCDNAME = "PCCD2"; xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: CCDNAME = "GCCD1"; xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: CCDNAME = "GCCD2"; xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: CCDNAME = "QCCD"; xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            if (cBtest.SelectedIndex == 0)
                CCDNAME = CCDNAME + "-" + "Hold";
            if (cBtest.SelectedIndex == 1)
                CCDNAME = CCDNAME + "-" + "Lens";
            ReadParaIde(CCDNAME);
        }
        private void cBoxTest_CheckedChanged(object sender, EventArgs e)
        {
            if (cBoxTest.Checked)
            {
                Sys.AssTest = true;
                cBoxCCD.Enabled = true;
                cBoxNum.Enabled = true;
                btnHold.Enabled = true;
                btnLens.Enabled = true;
                gBAssDis.Enabled = true;
                if (SetNum == "1" || SetNum == "3" || SetNum == "2" || SetNum == "4")
                {
                    cBtest.Show();
                    cBLocation2.Hide();
                    cBLocation.Hide();
                }
            }
            if (!cBoxTest.Checked)
            {
                Sys.AssTest = false;
                cBoxCCD.Enabled = false;
                cBoxNum.Enabled = false;
                btnHold.Enabled = false;
                btnLens.Enabled = false;
                gBAssDis.Enabled = false;
                cBtest.Hide();
                cBLocation2.Show();
                cBLocation.Show();
            }
        }
        private void btnHold_Click(object sender, EventArgs e)
        {
            if (cBoxCCD.SelectedIndex == -1)
            {
                MessageBox.Show("请选择相机!");
                return;
            }
            Sys.AssLocation = "Hold";
            Sys.AssLocation2 = "Hold";
            if (cBoxCCD.SelectedIndex == 0)
            {
                #region Light
                string l1 = iniFile.Read("A1CCD1-Hold", "LighterValue1", FrmMain.propath);
                string l2 = iniFile.Read("A1CCD1-Hold", "LighterValue2", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 0;
                    parent.LightSet2();
                    Thread.Sleep(5);
                    parent.brit2 = int.Parse(l2); parent.ch2 = 1;
                    parent.LightSet2();
                }
                #endregion
                parent.OneShot1();
            }
            if (cBoxCCD.SelectedIndex == 1)
            {
                #region Light
                string l1 = iniFile.Read("A2CCD1-Hold", "LighterValue1", FrmMain.propath);
                string l2 = iniFile.Read("A2CCD1-Hold", "LighterValue2", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 3;
                    parent.LightSet2();
                    Thread.Sleep(5);
                    parent.brit2 = int.Parse(l2); parent.ch2 = 4;
                    parent.LightSet2();
                }
                #endregion
                parent.OneShot3();
            }
            if (cBoxCCD.SelectedIndex == 2)
            {
                #region Light
                string l1 = iniFile.Read("A1CCD2-Hold", "LighterValue", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 2;
                    parent.LightSet2();
                    Thread.Sleep(5);
                }
                #endregion
                parent.OneShot2();
            }
            if (cBoxCCD.SelectedIndex == 3)
            {
                #region Light
                string l1 = iniFile.Read("A2CCD2-Hold", "LighterValue", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 5;
                    parent.LightSet2();
                    Thread.Sleep(5);
                }
                #endregion
                parent.OneShot4();
            }
            if (cBoxCCD.SelectedIndex == 4)
            {
                #region Light
                string l1 = iniFile.Read("PCCD1-Hold", "LighterValue1", FrmMain.propath);
                string l2 = iniFile.Read("PCCD1-Hold", "LighterValue2", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit = int.Parse(l1); parent.ch = 0;
                    parent.LightSet();
                    Thread.Sleep(5);
                    parent.brit = int.Parse(l2); parent.ch = 1;
                    parent.LightSet();
                }
                #endregion
                parent.OneShot5();
            }
            #region  Holder值
            //switch (Sys.AssTestNum)
            //{
            //    case 1: labHx1.Text = Sys.AssHX[0]; labHy1.Text = Sys.AssHY[0]; break;
            //    case 2: labHx2.Text = Sys.AssHX[1]; labHy2.Text = Sys.AssHY[1]; break;
            //    case 3: labHx3.Text = Sys.AssHX[2]; labHy3.Text = Sys.AssHY[2]; break;
            //    case 4: labHx4.Text = Sys.AssHX[3]; labHy4.Text = Sys.AssHY[3]; break;
            //    case 5: labHx5.Text = Sys.AssHX[4]; labHy5.Text = Sys.AssHY[4]; break;
            //}
            #endregion
        }
        private void btnLens_Click(object sender, EventArgs e)
        {
            if (cBoxCCD.SelectedIndex == -1)
            {
                MessageBox.Show("请选择相机!");
                return;
            }
            Sys.AssLocation = "Lens";
            Sys.AssLocation2 = "Lens";
            if (cBoxCCD.SelectedIndex == 0)
            {
                #region Light
                string l1 = iniFile.Read("A1CCD1-Lens", "LighterValue1", FrmMain.propath);
                string l2 = iniFile.Read("A1CCD1-Lens", "LighterValue2", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 0;
                    parent.LightSet2();
                    Thread.Sleep(5);
                    parent.brit2 = int.Parse(l2); parent.ch2 = 1;
                    parent.LightSet2();
                }
                #endregion
                parent.OneShot1();
            }
            if (cBoxCCD.SelectedIndex == 1)
            {
                #region Light
                string l1 = iniFile.Read("A2CCD1-Lens", "LighterValue1", FrmMain.propath);
                string l2 = iniFile.Read("A2CCD1-Lens", "LighterValue2", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 3;
                    parent.LightSet2();
                    Thread.Sleep(5);
                    parent.brit2 = int.Parse(l2); parent.ch2 = 4;
                    parent.LightSet2();
                }
                #endregion
                parent.OneShot3();
            }
            if (cBoxCCD.SelectedIndex == 2)
            {
                #region Light
                string l1 = iniFile.Read("A1CCD2-Lens", "LighterValue", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 2;
                    parent.LightSet2();
                    Thread.Sleep(5);
                }
                #endregion
                parent.OneShot2();
            }
            if (cBoxCCD.SelectedIndex == 3)
            {
                #region Light
                string l1 = iniFile.Read("A2CCD2-Lens", "LighterValue", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit2 = int.Parse(l1); parent.ch2 = 5;
                    parent.LightSet2();
                    Thread.Sleep(5);
                }
                #endregion
                parent.OneShot4();
            }
            if (cBoxCCD.SelectedIndex == 4)
            {
                #region Light
                string l1 = iniFile.Read("PCCD1-Lens", "LighterValue1", FrmMain.propath);
                string l2 = iniFile.Read("PCCD1-Lens", "LighterValue2", FrmMain.propath);
                if (l1 != "")
                {
                    parent.brit = int.Parse(l1); parent.ch = 0;
                    parent.LightSet();
                    Thread.Sleep(5);
                    parent.brit = int.Parse(l2); parent.ch = 1;
                    parent.LightSet();
                }
                #endregion
                parent.OneShot5();
            }
            #region Lens及差值
            //switch (Sys.AssTestNum)
            //{
            //    case 1: labLx1.Text = Sys.AssLX[0]; labLy1.Text = Sys.AssLY[0];
            //        if (Sys.AssLX[0] != "" && labHx1.Text != "-" & labHx1.Text != "") 
            //        {
            //            dx[0] = double.Parse(Sys.AssLX[0]) - double.Parse(Sys.AssHX[0]);
            //            dy[0] = double.Parse(Sys.AssLY[0]) - double.Parse(Sys.AssHY[0]);
            //            labDx1.Text = dx[0].ToString();
            //            labDy1.Text = dy[0].ToString();
            //        } break;
            //    case 2: labLx2.Text = Sys.AssLX[1]; labLy2.Text = Sys.AssLY[1];
            //        if (Sys.AssLX[1] != "" && labHx2.Text != "-" & labHx2.Text != "") 
            //        {
            //            dx[1] = double.Parse(Sys.AssLX[1]) - double.Parse(Sys.AssHX[1]);
            //            dy[1] = double.Parse(Sys.AssLY[1]) - double.Parse(Sys.AssHY[1]);
            //            labDx2.Text = dx[1].ToString();
            //            labDy2.Text = dy[1].ToString();
            //        }break;
            //    case 3: labLx3.Text = Sys.AssLX[2]; labLy3.Text = Sys.AssLY[2];
            //        if (Sys.AssLX[2] != "" && labHx3.Text != "-" & labHx3.Text != "") 
            //        {
            //            dx[2] = double.Parse(Sys.AssLX[2]) - double.Parse(Sys.AssHX[2]);
            //            dy[2] = double.Parse(Sys.AssLY[2]) - double.Parse(Sys.AssHY[2]);
            //            labDx3.Text = dx[2].ToString();
            //            labDy3.Text = dy[2].ToString();
            //        }break;
            //    case 4: labLx4.Text = Sys.AssLX[3]; labLy4.Text = Sys.AssLY[3];
            //        if (Sys.AssLX[3] != "" && labHx4.Text != "-" & labHx4.Text != "") 
            //        {
            //            dx[3] = double.Parse(Sys.AssLX[3]) - double.Parse(Sys.AssHX[3]);
            //            dy[3] = double.Parse(Sys.AssLY[3]) - double.Parse(Sys.AssHY[3]);
            //            labDx4.Text = dx[3].ToString();
            //            labDy4.Text = dy[3].ToString();
            //        }break;
            //    case 5: labLx5.Text = Sys.AssLX[4]; labLy5.Text = Sys.AssLY[4];
            //        if (Sys.AssLX[4] != "" && labHx5.Text != "-" & labHx5.Text != "") 
            //        {
            //            dx[4] = double.Parse(Sys.AssLX[4]) - double.Parse(Sys.AssHX[4]);
            //            dy[4] = double.Parse(Sys.AssLY[4]) - double.Parse(Sys.AssHY[4]);
            //            labDx5.Text = dx[4].ToString();
            //            labDy5.Text = dy[4].ToString();
            //        }break;
            //}
            #endregion
            #region 平均值
            //if (dx.Where(x => x != 0).Count() != 0)
            //{
            //    labDx.Text = dx.Where(x => x != 0).Average().ToString();
            //    labDy.Text = dy.Where(x => x != 0).Average().ToString();
            //}
            //else
            //{
            //    labDx.Text = "0";
            //    labDy.Text = "0";
            //}
            #endregion
        }
        private void cBoxNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sys.AssTestNum = cBoxNum.SelectedIndex + 1;
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            Sys.AssHX = new string[5]; Sys.AssHY = new string[5];
            Sys.AssLX = new string[5]; Sys.AssLY = new string[5];
            Array.Clear(dx, 0, dx.Length);
            Array.Clear(dy, 0, dy.Length);
            Array.Clear(dis, 0, dis.Length);
            labHx1.Text = "-"; labLx1.Text = "-"; labDx1.Text = "-";
            labHx2.Text = "-"; labLx2.Text = "-"; labDx2.Text = "-";
            labHx3.Text = "-"; labLx3.Text = "-"; labDx3.Text = "-";
            labHx4.Text = "-"; labLx4.Text = "-"; labDx4.Text = "-";
            labHx5.Text = "-"; labLx5.Text = "-"; labDx5.Text = "-";
            labDx.Text = "-";
            labHy1.Text = "-"; labLy1.Text = "-"; labDy1.Text = "-";
            labHy2.Text = "-"; labLy2.Text = "-"; labDy2.Text = "-";
            labHy3.Text = "-"; labLy3.Text = "-"; labDy3.Text = "-";
            labHy4.Text = "-"; labLy4.Text = "-"; labDy4.Text = "-";
            labHy5.Text = "-"; labLy5.Text = "-"; labDy5.Text = "-";
            labDy.Text = "-";
            labDis1.Text = "-"; labDis2.Text = "-"; labDis3.Text = "-"; labDis4.Text = "-"; labDis5.Text = "-";
        }

        private void btnSaveDisMax_Click(object sender, EventArgs e)
        {
            string CCDNAME = "";
            switch (cBoxCCD.SelectedIndex)
            {
                case 0: CCDNAME = "A1CCD1"; break;
                case 1: CCDNAME = "A1CCD2"; break;
                case 2: CCDNAME = "A2CCD1"; break;
                case 3: CCDNAME = "A2CCD2"; break;
                case 4: CCDNAME = "PCCD1"; break;
            }
            iniFile.Write(CCDNAME, "AssXDismax", txtXdisMax.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "AssYDismax", txtYdisMax.Text, FrmMain.propath);
            iniFile.Write(CCDNAME, "AssDismax", txtAssDisMax.Text, FrmMain.propath);
        }
        private void txtXdisMax_TextChanged(object sender, EventArgs e)
        {
            //if (!IsNumber(txtXdisMax.Text))
            //{
            //    txtXdisMax.Text = "";
            //    MessageBox.Show("请输入数字！");
            //}
        }
        private void txtYdisMax_TextChanged(object sender, EventArgs e)
        {
            //if (!IsNumber(txtYdisMax.Text))
            //{
            //    txtYdisMax.Text = "";
            //    MessageBox.Show("请输入数字！");
            //}
        }
        private void txtAssDisMax_TextChanged(object sender, EventArgs e)
        {
            //if (!IsNumber(txtAssDisMax.Text))
            //{
            //    txtAssDisMax.Text = "";
            //    MessageBox.Show("请输入数字！");
            //}
        }
        private void cBoxCCD_SelectedIndexChanged(object sender, EventArgs e)
        {
            string CCDNAME = "";
            switch (cBoxCCD.SelectedIndex)
            {
                case 0: CCDNAME = "A1CCD1"; break;
                case 1: CCDNAME = "A1CCD2"; break;
                case 2: CCDNAME = "A2CCD1"; break;
                case 3: CCDNAME = "A2CCD2"; break;
                case 4: CCDNAME = "PCCD1"; break;
            }
            txtXdisMax.Text = iniFile.Read(CCDNAME, "AssXDismax", FrmMain.propath);
            txtYdisMax.Text = iniFile.Read(CCDNAME, "AssYDismax", FrmMain.propath);
            txtAssDisMax.Text = iniFile.Read(CCDNAME, "AssDismax", FrmMain.propath);
        }
        #endregion
        #region 结果检测Mode2（外观检测PCCD1Lens）
        HTuple hv_LensRow = new HTuple(), hv_LensCol = new HTuple(), hv_LensRadius = new HTuple();
        private void cBP1DisMode2_CheckedChanged(object sender, EventArgs e)
        {
            if (cBP1DisMode2.Checked)
            {
                cBtest.Hide();
                gBP1lens2step.Enabled = true;
                //btnSaveP1DisMode2.Enabled = true;
                gBP1lens3step.Enabled = true;
            }
            else
            {
                gBP1lens2step.Enabled = false;
                //btnSaveP1DisMode2.Enabled = false;
                gBP1lens3step.Enabled = false;
            }
        }
        private void btnDrawP1LensCir_Click(object sender, EventArgs e)
        {
            if (hv_HoldCol.Length == 0)
            {
                MessageBox.Show("请执行第一步！");
                return;
            }
            hv_RingRow = hv_HoldRow; hv_RingCol = hv_HoldCol;
            hv_RingRadius = hv_HoldRadius;
            tBP1lensRRadius.Value = (int)Math.Round((double)hv_RingRadius);
            hv_RDetectHeight = (int)UDP1LensWidth.Value;
            hv_transition = "all";
            if (tBP1LensB2W.Value != 1)
                hv_transition = "negative";
            if (tBP1LensW2B.Value != 255)
                hv_transition = "positive";

            HOperatorSet.GenEmptyObj(out ho_MeasureContour);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_UsedEdges);

            ShowRing1();
        }
        private void tBP1lensRRadius_ValueChanged(object sender, EventArgs e)
        {
            UDP1lensRRadius.Value = tBP1lensRRadius.Value;
            hv_RingRadius = tBP1lensRRadius.Value;
            ShowRing1();
        }
        private void UDP1lensRRadius_ValueChanged(object sender, EventArgs e)
        {
            tBP1lensRRadius.Value = (int)UDP1lensRRadius.Value;
        }
        private void tBP1LensWidth_ValueChanged(object sender, EventArgs e)
        {
            UDP1LensWidth.Value = tBP1LensWidth.Value;
            hv_RDetectHeight = tBP1LensWidth.Value;
            ShowRing1();
        }
        private void UDP1LensWidth_ValueChanged(object sender, EventArgs e)
        {
            tBP1LensWidth.Value = (int)UDP1LensWidth.Value;
        }
        private void tBP1LensW2B_ValueChanged(object sender, EventArgs e)
        {
            UDP1LensW2B.Value = tBP1LensW2B.Value;
            hv_RAmplitudeThreshold = tBP1LensW2B.Value;
        }
        private void UDP1LensW2B_ValueChanged(object sender, EventArgs e)
        {
            tBP1LensW2B.Value = (int)UDP1LensW2B.Value;
            tBP1LensB2W.Value = 1;
            hv_transition = "positive";
            ShowRing1();
        }
        private void tBP1LensB2W_ValueChanged(object sender, EventArgs e)
        {
            UDP1LensB2W.Value = tBP1LensB2W.Value;
            hv_RAmplitudeThreshold = tBP1LensB2W.Value;
        }
        private void UDP1LensB2W_ValueChanged(object sender, EventArgs e)
        {
            tBP1LensB2W.Value = (int)UDP1LensB2W.Value;
            tBP1LensW2B.Value = 255;
            hv_transition = "negative";
            ShowRing1();
        }
        private void btnP1LensCir_Click(object sender, EventArgs e)
        {
            switch (int.Parse(SetNum))
            {
                case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm;
                    hv_LensRow = null; hv_LensCol = null; hv_LensRadius = null; break;
                case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);
            try
            {
                HOperatorSet.GenEmptyObj(out ho_MeasureContour);
                HOperatorSet.GenEmptyObj(out ho_CrossCenter);
                HOperatorSet.GenEmptyObj(out ho_Contour);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_UsedEdges);
                HOperatorSet.GenEmptyObj(out ho_ResultContours);

                ho_ImageTest.Dispose();
                HOperatorSet.CopyImage(ho_ImageSet, out ho_ImageTest);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, width, height);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_RingRow.TupleConcat(
                    hv_RingCol))).TupleConcat(hv_RingRadius), 25, 5, 1, 30, new HTuple(), new HTuple(), out hv_circleIndices);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_transition", hv_transition);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length1", hv_RDetectHeight);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_length2", 5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "measure_threshold", hv_RAmplitudeThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_circleIndices, "min_score", 0.2);
                //应用测量
                HOperatorSet.ApplyMetrologyModel(ho_ImageTest, hv_MetrologyHandle);
                //获取结果
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, hv_circleIndices, "all", "result_type", "all_param", out hv_circleParameter);
                ho_ResultContours.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours, hv_MetrologyHandle, "all", "all", 1.5);
                hv_ColCenter = hv_circleParameter.TupleSelect(1);
                hv_RowCenter = hv_circleParameter.TupleSelect(0);
                hv_CenterRadius = hv_circleParameter.TupleSelect(2);
                hv_LensRow = hv_RowCenter;
                hv_LensCol = hv_ColCenter;
                hv_LensRadius = hv_CenterRadius;
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);

                hWVision.DispObj(ho_ImageSet);
                hWVision.SetLineWidth(1);
                hWVision.SetColor("green");
                hWVision.DispObj(ho_ResultContours);
                hWVision.SetColor("red");
                hWVision.DispCircle(hv_RowCenter, hv_ColCenter, 8);
                if (halcon.IsCrossDraw)
                    hWVision.DispCross(row, col, width, 0);
                HD.set_display_font(hWVision, 18, "sans", "true", "false");
                HD.disp_message(hWVision, "X_bias:" + (hv_ColCenter - col) * xpm, "", 150, 150, "green", "false");
                HD.disp_message(hWVision, "Y_bias:" + (-(hv_RowCenter - row) * ypm), "", 300, 150, "green", "false");
                HD.disp_message(hWVision, "R:" + hv_CenterRadius, "", 450, 150, "green", "false");
                ho_ImageTest.Dispose();
                ho_Circle.Dispose();
                ho_ModelContour.Dispose();
                ho_MeasureContour.Dispose();
                ho_ResultContours.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void tBP1LensRmin_ValueChanged(object sender, EventArgs e)
        {
            UDP1LensRmin.Value = tBP1LensRmin.Value;
        }
        private void UDP1LensRmin_ValueChanged(object sender, EventArgs e)
        {
            tBP1LensRmin.Value = (int)UDP1LensRmin.Value;
        }
        private void tBP1LensRmax_ValueChanged(object sender, EventArgs e)
        {
            UDP1LensRmax.Value = tBP1LensRmax.Value;
        }
        private void UDP1LensRmax_ValueChanged(object sender, EventArgs e)
        {
            tBP1LensRmax.Value = (int)UDP1LensRmax.Value;
        }
        HObject ho_P1HoldC = new HObject(), ho_P1HoldC0 = new HObject();
        HObject ho_P1LensC = new HObject(), ho_P1LensC0 = new HObject();
        private void btnCalHLDis_Click(object sender, EventArgs e)
        {
            if (hv_HoldRadius.Length == 0 || hv_LensRadius.Length == 0)
            {
                MessageBox.Show("请先找到Holder和Lens的中心！");
                return;
            }
            double dx = ((double)hv_LensCol - (double)hv_HoldCol) * xpm;
            double dy = ((double)hv_LensRow - (double)hv_HoldRow) * ypm;
            double dis = Math.Sqrt(dx * dx + dy * dy);
            hWVision.DispObj(ho_ImageSet);
            hWVision.SetColor("green");
            HOperatorSet.GenCircleContourXld(out ho_P1LensC, hv_LensRow, hv_LensCol, hv_LensRadius, 0, 6.28318, "positive", 1);
            hWVision.DispObj(ho_P1LensC);
            HOperatorSet.GenCircle(out ho_P1LensC0, hv_LensRow, hv_LensCol, 8);
            hWVision.DispObj(ho_P1LensC0);
            hWVision.SetColor("red");
            HOperatorSet.GenCircleContourXld(out ho_P1HoldC, hv_HoldRow, hv_HoldCol, hv_HoldRadius, 0, 6.28318, "positive", 1);
            hWVision.DispObj(ho_P1HoldC);
            HOperatorSet.GenCircle(out ho_P1HoldC0, hv_HoldRow, hv_HoldCol, 8);
            hWVision.DispObj(ho_P1HoldC0);
            HD.disp_message(hWVision, "DisX:" + Math.Round(dx, 4) + "mm", "", 150, 150, "green", "false");
            HD.disp_message(hWVision, "DisY:" + Math.Round(dy, 4) + "mm", "", 300, 150, "green", "false");
            HD.disp_message(hWVision, "Dis:" + Math.Round(dis, 4) + "mm", "", 450, 150, "green", "false");
        }
        private void btnSaveP1DisMode2_Click(object sender, EventArgs e)
        {
            string CCDNAME = "PCCD1";
            iniFile.Write(CCDNAME, "DisMode2Checked", cBP1DisMode2.Checked.ToString(), FrmMain.propath);
            if (cBP1DisMode2.Checked)
            {
                iniFile.Write(CCDNAME, "FigureShape", "Circle", FrmMain.propath);
                iniFile.Write(CCDNAME, "LighterValue1", (UD_LED7Lig.Value).ToString(), FrmMain.propath);
                //iniFile.Write(CCDNAME, "LighterValue2", (UD_LED8Lig.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LensRingRadius", (tBP1lensRRadius.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LensRingWidth", (tBP1LensWidth.Value).ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LensTransition", (string)hv_transition, FrmMain.propath);
                iniFile.Write(CCDNAME, "LensZoneRmin", tBP1LensRmin.Value.ToString(), FrmMain.propath);
                iniFile.Write(CCDNAME, "LensZoneRmax", tBP1LensRmax.Value.ToString(), FrmMain.propath);
                if (hv_transition == "negative")
                    iniFile.Write(CCDNAME, "LensRingThreshold", (tBP1LensB2W.Value).ToString(), FrmMain.propath);
                if (hv_transition == "positive")
                    iniFile.Write(CCDNAME, "LensRingThreshold", (tBP1LensW2B.Value).ToString(), FrmMain.propath);
            }
        }
        #endregion

        #region 对焦
        public static bool Definition = false;
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
            Definition = (cBDefinition.Checked ? true : false);
        }
        private void btnFocusSet_Click(object sender, EventArgs e)
        {
            btnFocusSet.BackColor = Color.Green;
            HOperatorSet.DrawRectangle1(hWVision, out hv_DRow1, out hv_DColumn1, out hv_DRow2, out hv_DColumn2);
            btnFocusSet.BackColor = Color.WhiteSmoke;
            HOperatorSet.GenRectangle1(out ho_DRectangle, hv_DRow1, hv_DColumn1, hv_DRow2, hv_DColumn2);
            FrmMain.hv_FocusRow1 = hv_DRow1;
            FrmMain.hv_FocusColumn1 = hv_DColumn1;
            FrmMain.hv_FocusRow2 = hv_DRow2;
            FrmMain.hv_FocusColumn2 = hv_DColumn2;
            HOperatorSet.SetColor(hWVision, "blue");
            HOperatorSet.SetDraw(hWVision, "margin");
            HOperatorSet.DispObj(ho_DRectangle, hWVision);
        }
        #endregion

        #region  相机参数调整
        public NODE_HANDLE hnoteGainA11;
        public NODE_HANDLE hnoteExpA11;
        private void tbA11Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD1.IsConnected)
            {
                MessageBox.Show("A1CCD1未连线！");
                return;
            }
            UDA11Gain.Value = tbA11Gain.Value;
        }
        private void UDA11Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD1.IsConnected)
            {
                MessageBox.Show("A1CCD1未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDA11Gain.Value);
            if (hnoteGainA11.IsValid)
            {
                int value = tbA11Gain.Value - ((tbA11Gain.Value - tbA11Gain.Minimum) % tbA11Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainA11, value);
                UDA11Gain.Value = value;
                A1CCD1.Gain = value.ToString();
                iniFile.Write("A1CCD1", "Gain", A1CCD1.Gain, parent.setpath);
            }
        }
        private void tbA11ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD1.IsConnected)
            {
                MessageBox.Show("A1CCD1未连线！");
                return;
            }
            UDA11ExposureTime.Value = tbA11ExposureTime.Value;
        }
        private void UDA11ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD1.IsConnected)
            {
                MessageBox.Show("A1CCD1未连线！");
                return;
            }
            tbA11ExposureTime.Value = Convert.ToInt32(UDA11ExposureTime.Value);
            if (hnoteExpA11.IsValid)
            {
                int value = tbA11ExposureTime.Value - ((tbA11ExposureTime.Value - tbA11ExposureTime.Minimum) % tbA11ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpA11, value);
                UDA11ExposureTime.Value = value;
                A1CCD1.ExposureTime = value.ToString();
                iniFile.Write("A1CCD1", "ExposureTime", A1CCD1.ExposureTime, parent.setpath);
            }
        }
        public void ReadA1CCD1Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainA11 = parent.m_imageProvider1.GetNodeFromDevice("GainRaw");
                hnoteExpA11 = parent.m_imageProvider1.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainA11));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainA11));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainA11));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainA11));

                tbA11Gain.Minimum = mingain;
                tbA11Gain.Maximum = maxgain;
                tbA11Gain.Value = valgain;
                tbA11Gain.SmallChange = incgain;
                tbA11Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDA11Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpA11));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpA11));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpA11));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpA11));

                /* Update the slider. */
                tbA11ExposureTime.Minimum = minexp;
                tbA11ExposureTime.Maximum = maxexp;
                tbA11ExposureTime.Value = valexp;
                tbA11ExposureTime.SmallChange = incexp;
                tbA11ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDA11ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainA12;
        public NODE_HANDLE hnoteExpA12;
        private void tbA12Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD2.IsConnected)
            {
                MessageBox.Show("A1CCD2未连线！");
                return;
            }
            UDA12Gain.Value = tbA12Gain.Value;
        }
        private void UDA12Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD2.IsConnected)
            {
                MessageBox.Show("A1CCD2未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDA12Gain.Value);
            if (hnoteGainA12.IsValid)
            {
                int value = tbA12Gain.Value - ((tbA12Gain.Value - tbA12Gain.Minimum) % tbA12Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainA12, value);
                UDA12Gain.Value = value;
                A1CCD2.Gain = value.ToString();
                iniFile.Write("A1CCD2", "Gain", A1CCD2.Gain, parent.setpath);
            }
        }
        private void tbA12ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD2.IsConnected)
            {
                MessageBox.Show("A1CCD2未连线！");
                return;
            }
            UDA12ExposureTime.Value = tbA12ExposureTime.Value;
        }
        private void UDA12ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A1CCD2.IsConnected)
            {
                MessageBox.Show("A1CCD2未连线！");
                return;
            }
            tbA12ExposureTime.Value = Convert.ToInt32(UDA12ExposureTime.Value);
            if (hnoteExpA12.IsValid)
            {
                int value = tbA12ExposureTime.Value - ((tbA12ExposureTime.Value - tbA12ExposureTime.Minimum) % tbA12ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpA12, value);
                UDA12ExposureTime.Value = value;
                A1CCD2.ExposureTime = value.ToString();
                iniFile.Write("A1CCD2", "ExposureTime", A1CCD2.ExposureTime, parent.setpath);
            }
        }
        public void ReadA1CCD2Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainA12 = parent.m_imageProvider2.GetNodeFromDevice("GainRaw");
                hnoteExpA12 = parent.m_imageProvider2.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainA12));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainA12));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainA12));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainA12));

                tbA12Gain.Minimum = mingain;
                tbA12Gain.Maximum = maxgain;
                tbA12Gain.Value = valgain;
                tbA12Gain.SmallChange = incgain;
                tbA12Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDA12Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpA12));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpA12));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpA12));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpA12));

                /* Update the slider. */
                tbA12ExposureTime.Minimum = minexp;
                tbA12ExposureTime.Maximum = maxexp;
                tbA12ExposureTime.Value = valexp;
                tbA12ExposureTime.SmallChange = incexp;
                tbA12ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDA12ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainA21;
        public NODE_HANDLE hnoteExpA21;
        private void tbA21Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD1.IsConnected)
            {
                MessageBox.Show("A2CCD1未连线！");
                return;
            }
            UDA21Gain.Value = tbA21Gain.Value;
        }
        private void UDA21Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD1.IsConnected)
            {
                MessageBox.Show("A2CCD1未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDA21Gain.Value);
            if (hnoteGainA21.IsValid)
            {
                int value = tbA21Gain.Value - ((tbA21Gain.Value - tbA21Gain.Minimum) % tbA21Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainA21, value);
                UDA21Gain.Value = value;
                A2CCD1.Gain = value.ToString();
                iniFile.Write("A2CCD1", "Gain", A2CCD1.Gain, parent.setpath);
            }
        }
        private void tbA21ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD1.IsConnected)
            {
                MessageBox.Show("A2CCD1未连线！");
                return;
            }
            UDA21ExposureTime.Value = tbA21ExposureTime.Value;
        }
        private void UDA21ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD1.IsConnected)
            {
                MessageBox.Show("A2CCD1未连线！");
                return;
            }
            tbA21ExposureTime.Value = Convert.ToInt32(UDA21ExposureTime.Value);
            if (hnoteExpA21.IsValid)
            {
                int value = tbA21ExposureTime.Value - ((tbA21ExposureTime.Value - tbA21ExposureTime.Minimum) % tbA21ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpA21, value);
                UDA21ExposureTime.Value = value;
                A2CCD1.ExposureTime = value.ToString();
                iniFile.Write("A2CCD1", "ExposureTime", A2CCD1.ExposureTime, parent.setpath);
            }
        }
        public void ReadA2CCD1Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainA21 = parent.m_imageProvider3.GetNodeFromDevice("GainRaw");
                hnoteExpA21 = parent.m_imageProvider3.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainA21));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainA21));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainA21));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainA21));

                tbA21Gain.Minimum = mingain;
                tbA21Gain.Maximum = maxgain;
                tbA21Gain.Value = valgain;
                tbA21Gain.SmallChange = incgain;
                tbA21Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDA21Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpA21));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpA21));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpA21));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpA21));

                /* Update the slider. */
                tbA21ExposureTime.Minimum = minexp;
                tbA21ExposureTime.Maximum = maxexp;
                tbA21ExposureTime.Value = valexp;
                tbA21ExposureTime.SmallChange = incexp;
                tbA21ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDA21ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainA22;
        public NODE_HANDLE hnoteExpA22;
        private void tbA22Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD2.IsConnected)
            {
                MessageBox.Show("A2CCD2未连线！");
                return;
            }
            UDA22Gain.Value = tbA22Gain.Value;
        }
        private void UDA22Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD2.IsConnected)
            {
                MessageBox.Show("A2CCD2未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDA22Gain.Value);
            if (hnoteGainA22.IsValid)
            {
                int value = tbA22Gain.Value - ((tbA22Gain.Value - tbA22Gain.Minimum) % tbA22Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainA22, value);
                UDA22Gain.Value = value;
                A2CCD2.Gain = value.ToString();
                iniFile.Write("A2CCD2", "Gain", A2CCD2.Gain, parent.setpath);
            }
        }
        private void tbA22ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD2.IsConnected)
            {
                MessageBox.Show("A2CCD2未连线！");
                return;
            }
            UDA22ExposureTime.Value = tbA22ExposureTime.Value;
        }
        private void UDA22ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!A2CCD2.IsConnected)
            {
                MessageBox.Show("A2CCD2未连线！");
                return;
            }
            tbA22ExposureTime.Value = Convert.ToInt32(UDA22ExposureTime.Value);
            if (hnoteExpA22.IsValid)
            {
                int value = tbA22ExposureTime.Value - ((tbA22ExposureTime.Value - tbA22ExposureTime.Minimum) % tbA22ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpA22, value);
                UDA22ExposureTime.Value = value;
                A2CCD2.ExposureTime = value.ToString();
                iniFile.Write("A2CCD2", "ExposureTime", A2CCD2.ExposureTime, parent.setpath);
            }
        }
        public void ReadA2CCD2Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainA22 = parent.m_imageProvider4.GetNodeFromDevice("GainRaw");
                hnoteExpA22 = parent.m_imageProvider4.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainA22));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainA22));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainA22));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainA22));

                tbA22Gain.Minimum = mingain;
                tbA22Gain.Maximum = maxgain;
                tbA22Gain.Value = valgain;
                tbA22Gain.SmallChange = incgain;
                tbA22Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDA22Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpA22));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpA22));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpA22));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpA22));

                /* Update the slider. */
                tbA22ExposureTime.Minimum = minexp;
                tbA22ExposureTime.Maximum = maxexp;
                tbA22ExposureTime.Value = valexp;
                tbA22ExposureTime.SmallChange = incexp;
                tbA22ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDA22ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainP1;
        public NODE_HANDLE hnoteExpP1;
        private void tbP1Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD1.IsConnected)
            {
                MessageBox.Show("PCCD1未连线！");
                return;
            }
            UDP1Gain.Value = tbP1Gain.Value;
        }
        private void UDP1Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD1.IsConnected)
            {
                MessageBox.Show("PCCD1未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDP1Gain.Value);
            if (hnoteGainP1.IsValid)
            {
                int value = tbP1Gain.Value - ((tbP1Gain.Value - tbP1Gain.Minimum) % tbP1Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainP1, value);
                UDP1Gain.Value = value;
                PCCD1.Gain = value.ToString();
                iniFile.Write("PCCD1", "Gain", PCCD1.Gain, parent.setpath);
            }
        }
        private void tbP1ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD1.IsConnected)
            {
                MessageBox.Show("PCCD1未连线！");
                return;
            }
            UDP1ExposureTime.Value = tbP1ExposureTime.Value;
        }
        private void UDP1ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD1.IsConnected)
            {
                MessageBox.Show("PCCD1未连线！");
                return;
            }
            tbP1ExposureTime.Value = Convert.ToInt32(UDP1ExposureTime.Value);
            if (hnoteExpP1.IsValid)
            {
                int value = tbP1ExposureTime.Value - ((tbP1ExposureTime.Value - tbP1ExposureTime.Minimum) % tbP1ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpP1, value);
                UDP1ExposureTime.Value = value;
                PCCD1.ExposureTime = value.ToString();
                iniFile.Write("PCCD1", "ExposureTime", PCCD1.ExposureTime, parent.setpath);
            }
        }
        public void ReadPCCD1Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainP1 = parent.m_imageProvider5.GetNodeFromDevice("GainRaw");
                hnoteExpP1 = parent.m_imageProvider5.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainP1));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainP1));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainP1));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainP1));

                tbP1Gain.Minimum = mingain;
                tbP1Gain.Maximum = maxgain;
                tbP1Gain.Value = valgain;
                tbP1Gain.SmallChange = incgain;
                tbP1Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDP1Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpP1));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpP1));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpP1));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpP1));

                /* Update the slider. */
                tbP1ExposureTime.Minimum = minexp;
                tbP1ExposureTime.Maximum = maxexp;
                tbP1ExposureTime.Value = valexp;
                tbP1ExposureTime.SmallChange = incexp;
                tbP1ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDP1ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainP2;
        public NODE_HANDLE hnoteExpP2;
        private void tbP2Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD2.IsConnected)
            {
                MessageBox.Show("PCCD2未连线！");
                return;
            }
            UDP2Gain.Value = tbP2Gain.Value;
        }
        private void UDP2Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD2.IsConnected)
            {
                MessageBox.Show("PCCD2未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDP2Gain.Value);
            if (hnoteGainP2.IsValid)
            {
                int value = tbP2Gain.Value - ((tbP2Gain.Value - tbP2Gain.Minimum) % tbP2Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainP2, value);
                UDP2Gain.Value = value;
                PCCD2.Gain = value.ToString();
                iniFile.Write("PCCD2", "Gain", PCCD2.Gain, parent.setpath);
            }
        }
        private void tbP2ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD2.IsConnected)
            {
                MessageBox.Show("PCCD2未连线！");
                return;
            }
            UDP2ExposureTime.Value = tbP2ExposureTime.Value;
        }
        private void UDP2ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!PCCD2.IsConnected)
            {
                MessageBox.Show("PCCD2未连线！");
                return;
            }
            tbP2ExposureTime.Value = Convert.ToInt32(UDP2ExposureTime.Value);
            if (hnoteExpP2.IsValid)
            {
                int value = tbP2ExposureTime.Value - ((tbP2ExposureTime.Value - tbP2ExposureTime.Minimum) % tbP2ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpP2, value);
                UDP2ExposureTime.Value = value;
                PCCD2.ExposureTime = value.ToString();
                iniFile.Write("PCCD2", "ExposureTime", PCCD2.ExposureTime, parent.setpath);
            }
        }
        public void ReadPCCD2Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainP2 = parent.m_imageProvider6.GetNodeFromDevice("GainRaw");
                hnoteExpP2 = parent.m_imageProvider6.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainP2));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainP2));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainP2));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainP2));

                tbP2Gain.Minimum = mingain;
                tbP2Gain.Maximum = maxgain;
                tbP2Gain.Value = valgain;
                tbP2Gain.SmallChange = incgain;
                tbP2Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDP2Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpP2));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpP2));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpP2));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpP2));

                /* Update the slider. */
                tbP2ExposureTime.Minimum = minexp;
                tbP2ExposureTime.Maximum = maxexp;
                tbP2ExposureTime.Value = valexp;
                tbP2ExposureTime.SmallChange = incexp;
                tbP2ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDP2ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainG1;
        public NODE_HANDLE hnoteExpG1;
        private void tbG1Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD1.IsConnected)
            {
                MessageBox.Show("GCCD1未连线！");
                return;
            }
            UDG1Gain.Value = tbG1Gain.Value;
        }
        private void UDG1Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD1.IsConnected)
            {
                MessageBox.Show("GCCD1未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDG1Gain.Value);
            if (hnoteGainG1.IsValid)
            {
                int value = tbG1Gain.Value - ((tbG1Gain.Value - tbG1Gain.Minimum) % tbG1Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainG1, value);
                UDG1Gain.Value = value;
                GCCD1.Gain = value.ToString();
                iniFile.Write("GCCD1", "Gain", GCCD1.Gain, parent.setpath);
            }
        }
        private void tbG1ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD1.IsConnected)
            {
                MessageBox.Show("GCCD1未连线！");
                return;
            }
            UDG1ExposureTime.Value = tbG1ExposureTime.Value;
        }
        private void UDG1ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD1.IsConnected)
            {
                MessageBox.Show("GCCD1未连线！");
                return;
            }
            tbG1ExposureTime.Value = Convert.ToInt32(UDG1ExposureTime.Value);
            if (hnoteExpG1.IsValid)
            {
                int value = tbG1ExposureTime.Value - ((tbG1ExposureTime.Value - tbG1ExposureTime.Minimum) % tbG1ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpG1, value);
                UDG1ExposureTime.Value = value;
                GCCD1.ExposureTime = value.ToString();
                iniFile.Write("GCCD1", "ExposureTime", GCCD1.ExposureTime, parent.setpath);
            }
        }
        public void ReadGCCD1Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainG1 = parent.m_imageProvider7.GetNodeFromDevice("GainRaw");
                hnoteExpG1 = parent.m_imageProvider7.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainG1));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainG1));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainG1));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainG1));

                tbG1Gain.Minimum = mingain;
                tbG1Gain.Maximum = maxgain;
                tbG1Gain.Value = valgain;
                tbG1Gain.SmallChange = incgain;
                tbG1Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDG1Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpG1));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpG1));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpG1));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpG1));

                /* Update the slider. */
                tbG1ExposureTime.Minimum = minexp;
                tbG1ExposureTime.Maximum = maxexp;
                tbG1ExposureTime.Value = valexp;
                tbG1ExposureTime.SmallChange = incexp;
                tbG1ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDG1ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainG2;
        public NODE_HANDLE hnoteExpG2;
        private void tbG2Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD2.IsConnected)
            {
                MessageBox.Show("GCCD2未连线！");
                return;
            }
            UDG2Gain.Value = tbG2Gain.Value;
        }
        private void UDG2Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD2.IsConnected)
            {
                MessageBox.Show("GCCD2未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDG2Gain.Value);
            if (hnoteGainG2.IsValid)
            {
                int value = tbG2Gain.Value - ((tbG2Gain.Value - tbG2Gain.Minimum) % tbG2Gain.SmallChange);
                GenApi.IntegerSetValue(hnoteGainG2, value);
                UDG2Gain.Value = value;
                GCCD2.Gain = value.ToString();
                iniFile.Write("GCCD2", "Gain", GCCD2.Gain, parent.setpath);
            }
        }
        private void tbG2ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD2.IsConnected)
            {
                MessageBox.Show("GCCD2未连线！");
                return;
            }
            UDG2ExposureTime.Value = tbG2ExposureTime.Value;
        }
        private void UDG2ExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!GCCD2.IsConnected)
            {
                MessageBox.Show("GCCD2未连线！");
                return;
            }
            tbG2ExposureTime.Value = Convert.ToInt32(UDG2ExposureTime.Value);
            if (hnoteExpG2.IsValid)
            {
                int value = tbG2ExposureTime.Value - ((tbG2ExposureTime.Value - tbG2ExposureTime.Minimum) % tbG2ExposureTime.SmallChange);
                GenApi.IntegerSetValue(hnoteExpG2, value);
                UDG2ExposureTime.Value = value;
                GCCD2.ExposureTime = value.ToString();
                iniFile.Write("GCCD2", "ExposureTime", GCCD2.ExposureTime, parent.setpath);
            }
        }
        public void ReadGCCD2Set()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainG2 = parent.m_imageProvider8.GetNodeFromDevice("GainRaw");
                hnoteExpG2 = parent.m_imageProvider8.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainG2));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainG2));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainG2));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainG2));

                tbG2Gain.Minimum = mingain;
                tbG2Gain.Maximum = maxgain;
                tbG2Gain.Value = valgain;
                tbG2Gain.SmallChange = incgain;
                tbG2Gain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDG2Gain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpG2));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpG2));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpG2));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpG2));

                /* Update the slider. */
                tbG2ExposureTime.Minimum = minexp;
                tbG2ExposureTime.Maximum = maxexp;
                tbG2ExposureTime.Value = valexp;
                tbG2ExposureTime.SmallChange = incexp;
                tbG2ExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDG2ExposureTime.Text = "" + valexp;

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
        public NODE_HANDLE hnoteGainQ;
        public NODE_HANDLE hnoteExpQ;
        private void tbQGain_ValueChanged(object sender, EventArgs e)
        {
            if (!QCCD.IsConnected)
            {
                MessageBox.Show("QCCD未连线！");
                return;
            }
            UDQGain.Value = tbQGain.Value;
        }
        private void UDQGain_ValueChanged(object sender, EventArgs e)
        {
            if (!QCCD.IsConnected)
            {
                MessageBox.Show("QCCD未连线！");
                return;
            }
            int Gain = Convert.ToInt32(UDQGain.Value);
            int value = 0;
            switch (QCCD.CCDBrand)
            {
                case 0:
                    if (hnoteGainQ.IsValid)
                    {
                        value = tbQGain.Value - ((tbQGain.Value - tbQGain.Minimum) % tbQGain.SmallChange);
                        GenApi.IntegerSetValue(hnoteGainQ, value);
                        UDQGain.Value = value;
                    }
                    break;
                case 1:
                    value = Gain;
                    parent.myHikvision.SetGain(value);
                    break;
            }
            QCCD.Gain = value.ToString();
            iniFile.Write("QCCD", "Gain", QCCD.Gain, parent.setpath);
        }
        private void tbQExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!QCCD.IsConnected)
            {
                MessageBox.Show("QCCD未连线！");
                return;
            }
            UDQExposureTime.Value = tbQExposureTime.Value;
        }
        private void UDQExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!QCCD.IsConnected)
            {
                MessageBox.Show("QCCD未连线！");
                return;
            }
            tbQExposureTime.Value = Convert.ToInt32(UDQExposureTime.Value);
            int value = 0;
            switch (QCCD.CCDBrand)
            {
                case 0: //Basler
                    if (hnoteExpQ.IsValid)
                    {
                        value = tbQExposureTime.Value - ((tbQExposureTime.Value - tbQExposureTime.Minimum) % tbQExposureTime.SmallChange);
                        GenApi.IntegerSetValue(hnoteExpQ, value);
                        UDQExposureTime.Value = value;
                    }
                    break;
                case 1:  //H
                    value = tbQExposureTime.Value;
                    parent.myHikvision.SetExposureTime(value);
                    break;
            }
            QCCD.ExposureTime = value.ToString();
            iniFile.Write("QCCD", "ExposureTime", QCCD.ExposureTime, parent.setpath);
        }
        public void ReadQCCDBSet()
        {
            try
            {
                /* Open the image provider using the index from the device data. */
                hnoteGainQ = parent.m_imageProvider9.GetNodeFromDevice("GainRaw");
                hnoteExpQ = parent.m_imageProvider9.GetNodeFromDevice("ExposureTimeRaw");

                //long maxgain = GenApi.IntegerGetMax(hnoteGian);
                //long mingain = GenApi.IntegerGetMin(hnoteGian);

                int mingain = checked((int)GenApi.IntegerGetMin(hnoteGainQ));
                int maxgain = checked((int)GenApi.IntegerGetMax(hnoteGainQ));
                int valgain = checked((int)GenApi.IntegerGetValue(hnoteGainQ));
                int incgain = checked((int)GenApi.IntegerGetInc(hnoteGainQ));

                tbQGain.Minimum = mingain;
                tbQGain.Maximum = maxgain;
                tbQGain.Value = valgain;
                tbQGain.SmallChange = incgain;
                tbQGain.TickFrequency = (maxgain - mingain + 5) / 10;

                /* Update the values. */
                //lblGainMin.Text = "" + mingain;
                //lblGainMax.Text = "" + maxgain;
                UDQGain.Text = "" + valgain;
                //long Maxexp = GenApi.IntegerGetMax(hnoteExp);
                //long Minexp = GenApi.IntegerGetMin(hnoteExp);

                int minexp = checked((int)GenApi.IntegerGetMin(hnoteExpQ));
                int maxexp = checked((int)GenApi.IntegerGetMax(hnoteExpQ));
                int valexp = checked((int)GenApi.IntegerGetValue(hnoteExpQ));
                int incexp = checked((int)GenApi.IntegerGetInc(hnoteExpQ));

                /* Update the slider. */
                tbQExposureTime.Minimum = minexp;
                tbQExposureTime.Maximum = maxexp;
                tbQExposureTime.Value = valexp;
                tbQExposureTime.SmallChange = incexp;
                tbQExposureTime.TickFrequency = (maxexp - minexp + 5) / 10;

                /* Update the values. */
                //lblExposureTimeMin.Text = "" + minexp;
                //lblExposureTimeMax.Text = "" + maxexp;
                UDQExposureTime.Text = "" + valexp;

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
        public void ReadQCCDHSet()
        {
            try
            {
                int mingain = (int)parent.myHikvision.GainMinimum;
                int maxgain = (int)parent.myHikvision.GainMaximum;
                int valgain = (int)parent.myHikvision.Gain;

                tbQGain.Minimum = mingain;
                tbQGain.Maximum = maxgain;
                tbQGain.Value = valgain;

                int minexp = (int)parent.myHikvision.ExposureTimeMinimum;
                int maxexp = (int)parent.myHikvision.ExposureTimeMaximum;
                int valexp = (int)parent.myHikvision.ExposureTime;

                /* Update the slider. */
                tbQExposureTime.Minimum = minexp;
                tbQExposureTime.Maximum = maxexp;
                tbQExposureTime.Value = valexp;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        public void TextSPEC(object sender, KeyPressEventArgs e) //文本框格式设定
        {
            #region
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)//设置只允许输入  .  -  0~9  否则无效
                e.Handled = true;
            if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart != 0 || ((TextBox)sender).Text.IndexOf("-") >= 0))//输入为负号时，只能输入一次且只能输入一次
                e.Handled = true;
            if (e.KeyChar == 46)  //输入小数点时判断
            {
                if (((TextBox)sender).SelectionStart <= 0 || ((TextBox)sender).Text.IndexOf(".") >= 0) //第二位开始允许输入小数,且只能输入一次
                    e.Handled = true;
                if (((TextBox)sender).SelectionStart < ((TextBox)sender).Text.Length - 2) //限定小数点出现位置,防止出现三位以上小数的情况
                    e.Handled = true;
            }
            //限定只能输入两位小数 
            if (e.KeyChar != '\b' && (((TextBox)sender).SelectionStart) > (((TextBox)sender).Text.LastIndexOf('.')) + 3 && ((TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;
            //光标在小数点右侧时判断输入是否合规
            if (e.KeyChar != '\b' && ((TextBox)sender).SelectionStart >= (((TextBox)sender).Text.LastIndexOf('.')) && ((TextBox)sender).Text.IndexOf(".") >= 0)
            {
                if ((((TextBox)sender).SelectionStart) == (((TextBox)sender).Text.LastIndexOf('.')) + 1) //光标位于小数点右侧第一位时判断
                {
                    if ((((TextBox)sender).Text.Length).ToString() == (((TextBox)sender).Text.IndexOf(".") + 3).ToString())
                        e.Handled = true;
                }
                if ((((TextBox)sender).SelectionStart) == (((TextBox)sender).Text.LastIndexOf('.')) + 2) //光标位于小数点右侧第一位时判断
                {
                    if ((((TextBox)sender).Text.Length).ToString() == (((TextBox)sender).Text.IndexOf(".") + 4).ToString())
                        e.Handled = true;
                }
                if ((((TextBox)sender).SelectionStart) == (((TextBox)sender).Text.LastIndexOf('.')) + 3) //光标位于小数点右侧第一位时判断
                {
                    if ((((TextBox)sender).Text.Length - 4).ToString() == ((TextBox)sender).Text.IndexOf(".").ToString())
                        e.Handled = true;
                }
            }
            if (((TextBox)sender).TextLength > 5 && e.KeyChar != '\b')
                e.Handled = true;
            #endregion
        }
        private void CrosslineCkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CrosslineCkBox.Checked)
                halcon.IsCrossDraw = true;
            else
                halcon.IsCrossDraw = false;
            iniFile.Write("CROSSSHOWSET", "IsChecked", halcon.IsCrossDraw.ToString(), parent.setpath);
        }

        #region  固定环胶水
        //固定環灰度值(黑、白)
        int dGraythresholdBlack = 1, dGraythresholdWhite = 1;
        //過濾過小面積
        int dUnderSizeArea = 1;
        private void cBQAVI1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void btnDrawARingPF1_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0)
                return;
            if (hv_RowCenter == 0.0)
                return;
            RadiusMax = tbPF1OutRange.Value;
            RadiusMin = tbPF1InRange.Value;
            ShowDonut();
        }
        private void tbPF1InRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = tbPF1InRange.Value;
            UDPF1InRange.Value = RadiusMin;
        }
        private void UDPF1InRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = (HTuple)UDPF1InRange.Value;
            tbPF1InRange.Value = RadiusMin;
            ShowDonut();
        }
        private void tbPF1OutRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = tbPF1OutRange.Value;
            UDPF1OutRange.Value = RadiusMax;
        }
        private void UDPF1OutRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = (HTuple)UDPF1OutRange.Value;
            tbPF1OutRange.Value = RadiusMax;
            ShowDonut();
        }
        private void tbGraythresholdBlack_ValueChanged(object sender, EventArgs e)
        {
            nudGraythresholdBlack.Value = tbGraythresholdBlack.Value;
        }
        private void nudGraythresholdBlack_ValueChanged(object sender, EventArgs e)
        {
            dGraythresholdBlack = tbGraythresholdBlack.Value = Convert.ToInt32(nudGraythresholdBlack.Value);
            dGraythresholdWhite = tbGraythresholdWhite.Value = Convert.ToInt32(nudGraythresholdWhite.Value);
            dUnderSizeArea = tbUnderSizeArea.Value = Convert.ToInt32(nudUnderSizeArea.Value);
            Graythreshold1();
        }
        private void tbGraythresholdWhite_ValueChanged(object sender, EventArgs e)
        {
            nudGraythresholdWhite.Value = tbGraythresholdWhite.Value;
        }
        private void nudGraythresholdWhite_ValueChanged(object sender, EventArgs e)
        {
            dGraythresholdBlack = tbGraythresholdBlack.Value = Convert.ToInt32(nudGraythresholdBlack.Value);
            dGraythresholdWhite = tbGraythresholdWhite.Value = Convert.ToInt32(nudGraythresholdWhite.Value);
            dUnderSizeArea = tbUnderSizeArea.Value = Convert.ToInt32(nudUnderSizeArea.Value);
            Graythreshold1();
        }
        HTuple hv_CircleRadius1 = new HTuple(), hv_CircleRadius2 = new HTuple();
        HTuple hv_grayBlack = new HTuple(), hv_grayWhite = new HTuple();
        HTuple hv_UnderSizeArea = new HTuple();
        HObject ho_Edges = new HObject(), ho_RegionClosing = new HObject();
        HObject ho_RegionUnion = new HObject(), ho_RegionIntersection = new HObject();
        void Graythreshold1()
        {
            if (readpara)
                return;
            switch (int.Parse(SetNum))
            {
                case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
            }
            int i_image = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);

            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_Circle2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference_2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Edges);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            try
            {
                //畫出固定環區域
                hv_CircleRadius1 = RadiusMin;
                hv_CircleRadius2 = RadiusMax;
                //平滑
                //ho_ImageSmooth.Dispose();
                //HOperatorSet.SmoothImage(ho_Image, out ho_ImageSmooth, "deriche2", 0.3);
                ho_Circle1.Dispose();
                HOperatorSet.GenCircle(out ho_Circle1, hv_RowCenter, hv_ColCenter, hv_CircleRadius1);
                ho_Circle2.Dispose();
                HOperatorSet.GenCircle(out ho_Circle2, hv_RowCenter, hv_ColCenter, hv_CircleRadius2);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Circle2, ho_Circle1, out ho_RegionDifference);
                HOperatorSet.SetDraw(hWVision, "margin");
                //分割出固定環
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_ImageReduced);
                HOperatorSet.SetDraw(hWVision, "fill");
                //灰度設定
                hv_grayBlack = dGraythresholdBlack;
                hv_grayWhite = dGraythresholdWhite;

                //膠水為黑色白色設定
                ho_Edges.Dispose();
                if (QCCD.Detection_Black && QCCD.Detection_White)
                {
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Edges, (new HTuple(0)).TupleConcat(hv_grayWhite), hv_grayBlack.TupleConcat(255));
                }
                else if (QCCD.Detection_Black && !QCCD.Detection_White)
                {
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Edges, 0, hv_grayBlack);
                }
                else if (!QCCD.Detection_Black && QCCD.Detection_White)
                {
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Edges, hv_grayWhite, 255);
                }
                else
                {
                    MessageBox.Show("黑白至少要檢測一項");
                    return;
                }
                //填滿縫隙
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_Edges, out ho_RegionFillUp);
                //將相鄰的面積相連
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, 3.5);
                //分割
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                //開放設置去掉過小面積
                hv_UnderSizeArea = dUnderSizeArea;
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", hv_UnderSizeArea, 99999999);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                //找出與固定環區域的交集,避免360度整個填滿的情況
                ho_RegionIntersection.Dispose();
                HOperatorSet.Intersection(ho_RegionUnion, ho_RegionDifference, out ho_RegionIntersection);
                HOperatorSet.DispObj(ho_ImageSet, hWVision);
                HOperatorSet.SetColor(hWVision, "yellow");
                HOperatorSet.DispObj(ho_RegionIntersection, hWVision);
            }
            catch
            {
            }
        }
        private void tbUnderSizeArea_ValueChanged(object sender, EventArgs e)
        {
            nudUnderSizeArea.Value = tbUnderSizeArea.Value;
        }
        private void nudUnderSizeArea_ValueChanged(object sender, EventArgs e)
        {
            dGraythresholdWhite = tbGraythresholdWhite.Value = Convert.ToInt32(nudGraythresholdWhite.Value);
            dGraythresholdBlack = tbGraythresholdBlack.Value = Convert.ToInt32(nudGraythresholdBlack.Value);
            dUnderSizeArea = tbUnderSizeArea.Value = Convert.ToInt32(nudUnderSizeArea.Value);
            Graythreshold1();
        }
        #endregion
        #region 小台阶胶水
        HTuple hv_CircleRadiusPF1 = new HTuple();
        HTuple hv_CircleRadiusPF2 = new HTuple();
        HTuple hv_MeanWidth_1 = new HTuple(), hv_MeanHeight_1 = new HTuple();
        HTuple hv_MeanWidth_2 = new HTuple(), hv_MeanHeight_2 = new HTuple();
        HTuple hv_grayBlackPF = new HTuple(), hv_grayWhitePF = new HTuple();
        HTuple hv_closeWidthValue = new HTuple(), hv_closeHeightValue = new HTuple();
        HTuple hv_OpenHeightValue = new HTuple(), hv_OpenWidthValue = new HTuple();
        HTuple hv_UnderSizeArea2 = new HTuple();

        HObject ho_Circle1 = new HObject(), ho_Circle2 = new HObject();
        HObject ho_PolarTransImagePF = new HObject(), ho_ImageMean2 = new HObject();
        HObject ho_ImageMeanPF1 = new HObject(), ho_ImageMeanPF2 = new HObject();
        HObject ho_RegionDynThreshPF1 = new HObject(), ho_RegionDynThreshPF2 = new HObject();
        HObject ho_RegionUnionPF1 = new HObject(), ho_RegionUnionPF2 = new HObject();
        HObject ho_XYTransRegionPF2 = new HObject();

        int dInRangePF = 1, dOutRangePF = 1; //小台階内、外圍
        int iGlueAngleSetPF = 1; // 膠水角度閥值
        int iGlueRatioSetPF = 1; // 膠水面積閥值
        int dAngleSetPF = 1; // 檢測角度設定
        int iDynthresholdDarkPF2 = 1, iDynthresholdLightPF2 = 1; // 方法二差異閥值
        int iGraythresholdBlackPF2 = 1, iGraythresholdWhitePF2 = 1; // 方法二灰度閥值
        int iUnderSizeAreaPF2 = 1; // 方法二過濾小面積
        int iCloseWidthPF2 = 1, iCloseHeightPF2 = 1;
        int iOpenWidthPF2 = 1, iOpenHeightPF2 = 1;

        private void cBQAVI2_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void btnDrawARingPF2_Click(object sender, EventArgs e)
        {
            if (hv_RowCenter.Length == 0.0)
                return;
            if (hv_RowCenter == 0.0)
                return;
            RadiusMax = tbPFOutRange.Value;
            RadiusMin = tbPFInRange.Value;
            ShowDonut();
        }
        private void tbPFInRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = tbPFInRange.Value;
            UDPFInRange.Value = RadiusMin;
        }
        private void UDPFInRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMin = (HTuple)UDPFInRange.Value;
            tbPFInRange.Value = RadiusMin;
            ShowDonut();
        }
        private void tbPFOutRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = tbPFOutRange.Value;
            UDPFOutRange.Value = RadiusMax;
        }
        private void UDPFOutRange_ValueChanged(object sender, EventArgs e)
        {
            RadiusMax = (HTuple)UDPFOutRange.Value;
            tbPFOutRange.Value = RadiusMax;
            ShowDonut();
        }
        //差异阀值设定
        private void cbDetectionPF_Dark2_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.DetectionPF_Dark2 = (cbDetectionPF_Dark2.Checked ? true : false);
        }
        private void nudDynthresholdDarkPF2_ValueChanged(object sender, EventArgs e)
        {
            iDynthresholdDarkPF2 = (int)nudDynthresholdDarkPF2.Value;
            Dynthreshold();
        }
        private void cbDetectionPF_Light2_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.DetectionPF_Dark2 = (cbDetectionPF_Dark2.Checked ? true : false);
        }
        private void nudDynthresholdLightPF2_ValueChanged(object sender, EventArgs e)
        {
            iDynthresholdLightPF2 = (int)nudDynthresholdLightPF2.Value;
            Dynthreshold();
        }
        void Dynthreshold()
        {
            if (readpara)
                return;
            try
            {
                switch (int.Parse(SetNum))
                {
                    case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                    case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                    case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                    case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                    case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                    case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                    case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                    case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
                }
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);

                hv_CircleRadiusPF1 = RadiusMin;
                hv_CircleRadiusPF2 = RadiusMax;
                ho_Circle1.Dispose();
                HOperatorSet.GenCircle(out ho_Circle1, hv_RowCenter, hv_ColCenter, hv_CircleRadiusPF1);
                ho_Circle2.Dispose();
                HOperatorSet.GenCircle(out ho_Circle2, hv_RowCenter, hv_ColCenter, hv_CircleRadiusPF2);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Circle2, ho_Circle1, out ho_RegionDifference);
                HOperatorSet.SetDraw(hWVision, "margin");
                //分割出固定環
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_ImageReduced);
                //dev_set_draw ('fill')
                //方法二
                ho_PolarTransImagePF.Dispose();
                HOperatorSet.PolarTransImageExt(ho_ImageReduced, out ho_PolarTransImagePF, hv_RowCenter, hv_ColCenter, 0, 6.28319, hv_CircleRadiusPF1, hv_CircleRadiusPF2,
                    (hv_CircleRadiusPF2 + hv_CircleRadiusPF1) * Math.PI, hv_CircleRadiusPF2 - hv_CircleRadiusPF1, "nearest_neighbor");
                hv_MeanWidth_1 = 1;
                hv_MeanHeight_1 = 1;
                hv_MeanWidth_2 = 1;
                hv_MeanHeight_2 = hv_CircleRadiusPF2 - hv_CircleRadiusPF1;

                ho_ImageMean1.Dispose();
                HOperatorSet.MeanImage(ho_PolarTransImagePF, out ho_ImageMeanPF1, hv_MeanWidth_1, hv_MeanHeight_1);
                ho_ImageMean2.Dispose();
                HOperatorSet.MeanImage(ho_PolarTransImagePF, out ho_ImageMeanPF2, hv_MeanWidth_2, hv_MeanHeight_2);
                if (QCCD.DetectionPF_Light2 && QCCD.DetectionPF_Dark2)
                {
                    ho_RegionDynThreshPF2.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionDynThreshPF2, iDynthresholdLightPF2, "light");
                    ho_RegionDynThreshPF1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionDynThreshPF1, iDynthresholdDarkPF2, "dark");
                    ho_RegionUnionPF1.Dispose();
                    HOperatorSet.Union2(ho_RegionDynThreshPF1, ho_RegionDynThreshPF2, out ho_RegionUnionPF1);
                }
                else if (QCCD.DetectionPF_Dark2)
                {
                    ho_RegionUnionPF1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionUnionPF1, iDynthresholdDarkPF2, "dark");
                }
                else if (QCCD.DetectionPF_Light2)
                {
                    ho_RegionUnionPF1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionUnionPF1, iDynthresholdLightPF2, "light");
                }
                ho_XYTransRegionPF2.Dispose();
                HOperatorSet.PolarTransRegionInv(ho_RegionUnionPF1, out ho_XYTransRegionPF2, hv_RowCenter, hv_ColCenter, 0, 6.28319, hv_CircleRadiusPF1, hv_CircleRadiusPF2,
                    (hv_CircleRadiusPF2 + hv_CircleRadiusPF1) * Math.PI, hv_CircleRadiusPF2 - hv_CircleRadiusPF1, width, height, "nearest_neighbor");
                HOperatorSet.DispObj(ho_ImageSet, hWVision);
                HOperatorSet.SetColor(hWVision, "yellow");
                HOperatorSet.DispObj(ho_XYTransRegionPF2, hWVision);
            }
            catch
            { }
            finally
            {
                ho_Circle1.Dispose();
                ho_Circle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageReduced.Dispose();
                ho_PolarTransImagePF.Dispose();
                ho_ImageMean1.Dispose();
                ho_ImageMean2.Dispose();
                ho_RegionDynThreshPF2.Dispose();
                ho_RegionDynThreshPF1.Dispose();
                ho_RegionUnionPF1.Dispose();
                ho_XYTransRegionPF2.Dispose();
            }
        }
        //灰度阀值设定
        private void cbDetectionPF_Black2_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.DetectionPF_Black = (cbDetectionPF_Black2.Checked ? true : false);
        }
        private void tbGraythresholdBlackPF2_ValueChanged(object sender, EventArgs e)
        {
            nudGraythresholdBlackPF2.Value = tbGraythresholdBlackPF2.Value;
        }
        private void nudGraythresholdBlackPF2_ValueChanged(object sender, EventArgs e)
        {
            iGraythresholdBlackPF2 = tbGraythresholdBlackPF2.Value = (int)nudGraythresholdBlackPF2.Value;
            Graythreshold();
        }
        private void cbDetectionPF_White2_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.DetectionPF_White = (cbDetectionPF_White2.Checked ? true : false);
        }
        private void tbGraythresholdWhitePF2_ValueChanged(object sender, EventArgs e)
        {
            nudGraythresholdWhitePF2.Value = tbGraythresholdWhitePF2.Value;
        }
        private void nudGraythresholdWhitePF2_ValueChanged(object sender, EventArgs e)
        {
            iGraythresholdWhitePF2 = tbGraythresholdWhitePF2.Value = (int)nudGraythresholdWhitePF2.Value;
            Graythreshold();
        }
        void Graythreshold()
        {
            if (readpara)
                return;
            try
            {
                switch (int.Parse(SetNum))
                {
                    case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                    case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                    case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                    case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                    case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                    case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                    case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                    case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
                }
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);

                hv_CircleRadiusPF1 = RadiusMin;
                hv_CircleRadiusPF2 = RadiusMax;
                ho_Circle1.Dispose();
                HOperatorSet.GenCircle(out ho_Circle1, hv_RowCenter, hv_ColCenter, hv_CircleRadiusPF1);
                ho_Circle2.Dispose();
                HOperatorSet.GenCircle(out ho_Circle2, hv_RowCenter, hv_ColCenter, hv_CircleRadiusPF2);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Circle2, ho_Circle1, out ho_RegionDifference);
                HOperatorSet.SetDraw(hWVision, "margin");
                //分割出固定環
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_ImageReduced);
                //dev_set_draw ('fill')
                //方法二
                ho_PolarTransImagePF.Dispose();
                HOperatorSet.PolarTransImageExt(ho_ImageReduced, out ho_PolarTransImagePF, hv_RowCenter, hv_ColCenter, 0, 6.28319, hv_CircleRadiusPF1, hv_CircleRadiusPF2,
                    (hv_CircleRadiusPF2 + hv_CircleRadiusPF1) * Math.PI, hv_CircleRadiusPF2 - hv_CircleRadiusPF1, "nearest_neighbor");
                hv_grayBlackPF = iGraythresholdBlackPF2;
                hv_grayWhitePF = iGraythresholdWhitePF2;
                if (QCCD.DetectionPF_Black && QCCD.DetectionPF_White)
                {
                    ho_RegionUnionPF2.Dispose();
                    HOperatorSet.Threshold(ho_PolarTransImagePF, out ho_RegionUnionPF2, (new HTuple(0)).TupleConcat(hv_grayWhitePF), hv_grayBlackPF.TupleConcat(255));
                }
                else if (QCCD.DetectionPF_Black)
                {
                    ho_RegionUnionPF2.Dispose();
                    HOperatorSet.Threshold(ho_PolarTransImagePF, out ho_RegionUnionPF2, 0, hv_grayBlackPF);
                }
                else if (QCCD.DetectionPF_White)
                {
                    ho_RegionUnionPF2.Dispose();
                    HOperatorSet.Threshold(ho_PolarTransImagePF, out ho_RegionUnionPF2, hv_grayWhitePF, 255);
                }
                ho_XYTransRegionPF2.Dispose();
                HOperatorSet.PolarTransRegionInv(ho_RegionUnionPF2, out ho_XYTransRegionPF2, hv_RowCenter, hv_ColCenter, 0, 6.28319, hv_CircleRadiusPF1, hv_CircleRadiusPF2,
                    (hv_CircleRadiusPF2 + hv_CircleRadiusPF1) * Math.PI, hv_CircleRadiusPF2 - hv_CircleRadiusPF1, width, height, "nearest_neighbor");
                HOperatorSet.DispObj(ho_ImageSet, hWVision);
                HOperatorSet.SetColor(hWVision, "yellow");
                HOperatorSet.DispObj(ho_XYTransRegionPF2, hWVision);
            }
            catch
            {
            }
            finally
            {

            }
        }
        //区域连接
        private void cbClosingPF2_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.ClosingPF2 = cbClosingPF2.Checked ? true : false;
        }
        private void nudCloseWidthPF2_ValueChanged(object sender, EventArgs e)
        {
            iCloseWidthPF2 = (int)nudCloseWidthPF2.Value;
            MethodPF2();
        }
        private void nudCloseHeightPF2_ValueChanged(object sender, EventArgs e)
        {
            iCloseHeightPF2 = (int)nudCloseHeightPF2.Value;
        }
        public void MethodPF2()
        {
            if (readpara)
                return;
            try
            {
                switch (int.Parse(SetNum))
                {
                    case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                    case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                    case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                    case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                    case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                    case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                    case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                    case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
                }
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.AreaCenter(halcon.ImageOri[i_image], out area, out row, out col);

                hv_CircleRadiusPF1 = dInRangePF;
                hv_CircleRadiusPF2 = dOutRangePF;
                ho_Circle1.Dispose();
                HOperatorSet.GenCircle(out ho_Circle1, hv_RowCenter, hv_ColCenter, hv_CircleRadiusPF1);
                ho_Circle2.Dispose();
                HOperatorSet.GenCircle(out ho_Circle2, hv_RowCenter, hv_ColCenter, hv_CircleRadiusPF2);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Circle2, ho_Circle1, out ho_RegionDifference);
                HOperatorSet.SetDraw(hWVision, "margin");
                //分割出固定環
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSet, ho_RegionDifference, out ho_ImageReduced);
                //dev_set_draw ('fill')
                //方法二
                ho_PolarTransImagePF.Dispose();
                HOperatorSet.PolarTransImageExt(ho_ImageReduced, out ho_PolarTransImagePF, hv_RowCenter, hv_ColCenter, 0, 6.28319, hv_CircleRadiusPF1, hv_CircleRadiusPF2,
                    (hv_CircleRadiusPF2 + hv_CircleRadiusPF1) * Math.PI, hv_CircleRadiusPF2 - hv_CircleRadiusPF1, "nearest_neighbor");

                hv_MeanWidth_1 = 1;
                hv_MeanHeight_1 = 1;
                hv_MeanWidth_2 = 1;
                hv_MeanHeight_2 = hv_CircleRadiusPF2 - hv_CircleRadiusPF1;

                ho_ImageMean1.Dispose();
                HOperatorSet.MeanImage(ho_PolarTransImagePF, out ho_ImageMeanPF1, hv_MeanWidth_1, hv_MeanHeight_1);
                ho_ImageMean2.Dispose();
                HOperatorSet.MeanImage(ho_PolarTransImagePF, out ho_ImageMeanPF2, hv_MeanWidth_2, hv_MeanHeight_2);
                if (QCCD.DetectionPF_Light2 && QCCD.DetectionPF_Dark2)
                {
                    ho_RegionDynThreshPF2.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionDynThreshPF2, iDynthresholdLightPF2, "light");
                    ho_RegionDynThreshPF1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionDynThreshPF1, iDynthresholdDarkPF2, "dark");
                    ho_RegionUnionPF1.Dispose();
                    HOperatorSet.Union2(ho_RegionDynThreshPF1, ho_RegionDynThreshPF2, out ho_RegionUnionPF1);
                }
                else if (QCCD.DetectionPF_Dark2)
                {
                    ho_RegionUnionPF1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionUnionPF1, iDynthresholdDarkPF2, "dark");
                }
                else if (QCCD.DetectionPF_Light2)
                {
                    ho_RegionUnionPF1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMeanPF1, ho_ImageMeanPF2, out ho_RegionUnionPF1, iDynthresholdLightPF2, "light");
                }

                hv_grayBlackPF = iGraythresholdBlackPF2;
                hv_grayWhitePF = iGraythresholdWhitePF2;
                if (QCCD.DetectionPF_Black && QCCD.DetectionPF_White)
                {
                    ho_RegionUnionPF2.Dispose();
                    HOperatorSet.Threshold(ho_PolarTransImagePF, out ho_RegionUnionPF2, (new HTuple(0)).TupleConcat(hv_grayWhitePF), hv_grayBlackPF.TupleConcat(255));
                }
                else if (QCCD.DetectionPF_Black)
                {
                    ho_RegionUnionPF2.Dispose();
                    HOperatorSet.Threshold(ho_PolarTransImagePF, out ho_RegionUnionPF2, 0, hv_grayBlackPF);
                }
                else if (QCCD.DetectionPF_White)
                {
                    ho_RegionUnionPF2.Dispose();
                    HOperatorSet.Threshold(ho_PolarTransImagePF, out ho_RegionUnionPF2, hv_grayWhitePF, 255);
                }
                ho_RegionDynThreshPF2.Dispose();
                if ((QCCD.DetectionPF_Light2 || QCCD.DetectionPF_Dark2) && (QCCD.DetectionPF_White || QCCD.DetectionPF_Black))
                    HOperatorSet.Intersection(ho_RegionUnionPF1, ho_RegionUnionPF2, out ho_RegionDynThreshPF2);
                else if (!QCCD.DetectionPF_White && !QCCD.DetectionPF_Black)
                    HOperatorSet.Union1(ho_RegionUnionPF1, out ho_RegionDynThreshPF2);
                else if (!QCCD.DetectionPF_Dark2 && !QCCD.DetectionPF_Light2)
                    HOperatorSet.Union1(ho_RegionUnionPF2, out ho_RegionDynThreshPF2);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FillUp(ho_RegionDynThreshPF2, out ExpTmpOutVar_0);
                    ho_RegionDynThreshPF2.Dispose();
                    ho_RegionDynThreshPF2 = ExpTmpOutVar_0;
                }
                if (QCCD.ClosingPF2)
                {
                    hv_closeWidthValue = iCloseWidthPF2;
                    hv_closeHeightValue = iCloseHeightPF2;
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ClosingRectangle1(ho_RegionDynThreshPF2, out ExpTmpOutVar_0, hv_closeWidthValue, hv_closeHeightValue);
                        ho_RegionDynThreshPF2.Dispose();
                        ho_RegionDynThreshPF2 = ExpTmpOutVar_0;
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FillUp(ho_RegionDynThreshPF2, out ExpTmpOutVar_0);
                    ho_RegionDynThreshPF2.Dispose();
                    ho_RegionDynThreshPF2 = ExpTmpOutVar_0;
                }
                if (QCCD.OpeningPF2)
                {
                    hv_OpenHeightValue = iOpenHeightPF2;
                    hv_OpenWidthValue = iOpenWidthPF2;
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.OpeningRectangle1(ho_RegionDynThreshPF2, out ExpTmpOutVar_0, hv_OpenWidthValue, hv_OpenHeightValue);
                        ho_RegionDynThreshPF2.Dispose();
                        ho_RegionDynThreshPF2 = ExpTmpOutVar_0;
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Connection(ho_RegionDynThreshPF2, out ExpTmpOutVar_0);
                    ho_RegionDynThreshPF2.Dispose();
                    ho_RegionDynThreshPF2 = ExpTmpOutVar_0;
                }
                hv_UnderSizeArea2 = iUnderSizeAreaPF2;
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.SelectShape(ho_RegionDynThreshPF2, out ExpTmpOutVar_0, "area", "and", hv_UnderSizeArea2, 99999999);
                    ho_RegionDynThreshPF2.Dispose();
                    ho_RegionDynThreshPF2 = ExpTmpOutVar_0;
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Union1(ho_RegionDynThreshPF2, out ExpTmpOutVar_0);
                    ho_RegionDynThreshPF2.Dispose();
                    ho_RegionDynThreshPF2 = ExpTmpOutVar_0;
                }
                ho_XYTransRegionPF2.Dispose();
                HOperatorSet.PolarTransRegionInv(ho_RegionDynThreshPF2, out ho_XYTransRegionPF2, hv_RowCenter, hv_ColCenter, 0, 6.28319, hv_CircleRadiusPF1, hv_CircleRadiusPF2,
                    (hv_CircleRadiusPF2 + hv_CircleRadiusPF1) * Math.PI, hv_CircleRadiusPF2 - hv_CircleRadiusPF1, width, height, "nearest_neighbor");
                HOperatorSet.DispObj(ho_ImageSet, hWVision);
                HOperatorSet.DispObj(ho_XYTransRegionPF2, hWVision);
            }
            catch
            {
            }
            finally
            {
                ho_Circle1.Dispose();
                ho_Circle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageReduced.Dispose();
                //ho_PolarTransImage.Dispose();
                ho_ImageMean1.Dispose();
                ho_ImageMean2.Dispose();
                ho_RegionDynThreshPF2.Dispose();
                ho_RegionDynThreshPF1.Dispose();
                ho_RegionUnionPF1.Dispose();
                ho_RegionUnionPF2.Dispose();
                ho_XYTransRegionPF2.Dispose();
            }

        }
        //区域断开
        private void cbOpeningPF2_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.OpeningPF2 = cbOpeningPF2.Checked ? true : false;
        }
        private void nudOpenWidthPF2_ValueChanged(object sender, EventArgs e)
        {
            iOpenWidthPF2 = (int)nudOpenWidthPF2.Value;
            MethodPF2();
        }
        private void nudOpenHeightPF2_ValueChanged(object sender, EventArgs e)
        {
            iOpenHeightPF2 = (int)nudOpenHeightPF2.Value;
        }
        //过滤
        private void tbUnderSizeAreaPF2_ValueChanged(object sender, EventArgs e)
        {
            nudUnderSizeAreaPF2.Value = tbUnderSizeAreaPF2.Value;
        }
        private void nudUnderSizeAreaPF2_ValueChanged(object sender, EventArgs e)
        {
            iUnderSizeAreaPF2 = tbUnderSizeAreaPF2.Value = (int)nudUnderSizeAreaPF2.Value;
            MethodPF2();
        }
        #endregion
        private void btnPFSave_Click(object sender, EventArgs e)
        {
            //固定环
            QCCD.dInRange = tbPF1InRange.Value;
            QCCD.dOutRange = tbPF1OutRange.Value;
            QCCD.Detection_Black = cbDetection_Black.Checked ? true : false;
            QCCD.Detection_White = cbDetection_White.Checked ? true : false;
            QCCD.dGraythresholdBlack = dGraythresholdBlack;
            QCCD.dGraythresholdWhite = dGraythresholdWhite;
            QCCD.dUnderSizeArea = dUnderSizeArea;
            QCCD.iGlueAngleSet = iGlueAngleSetPF = Convert.ToInt32(nudGlueAngleSet.Value);
            QCCD.iGlueRatioSet = iGlueRatioSetPF = Convert.ToInt32(nudGlueRatioSet.Value);
            QCCD.dAngleSet = dAngleSetPF = Convert.ToInt32(nudAngleSet.Value);
            QCCD.AVI1IsCheck = cBQAVI1.Checked;
            iniFile.Write("QCCD", "AVI1ischecked", QCCD.AVI1IsCheck.ToString(), FrmMain.propath);
            if (QCCD.AVI1IsCheck)
            {
                #region 固定环
                iniFile.Write("QCCD", "InRange", QCCD.dInRange.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "OutRange", QCCD.dOutRange.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "Detection_Black", QCCD.Detection_Black.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "Detection_White", QCCD.Detection_White.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "GraythresholdBlack", QCCD.dGraythresholdBlack.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "GraythresholdWhite", QCCD.dGraythresholdWhite.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "UnderSizeArea", QCCD.dUnderSizeArea.ToString(), FrmMain.propath);

                iniFile.Write("QCCD", "GlueAngleSet", QCCD.iGlueAngleSet.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "GlueRatioSet", QCCD.iGlueRatioSet.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "AngleSet", QCCD.dAngleSet.ToString(), FrmMain.propath);
                #endregion
            }
            //小台阶
            QCCD.dInRangePF = tbPFInRange.Value;
            QCCD.dOutRangePF = tbPFOutRange.Value;
            QCCD.AVI2IsCheck = cBQAVI2.Checked ? true : false;
            QCCD.DetectionPF_Dark2 = cbDetectionPF_Dark2.Checked ? true : false;
            QCCD.DetectionPF_Light2 = cbDetectionPF_Light2.Checked ? true : false;
            QCCD.DetectionPF_Black = cbDetectionPF_Black2.Checked ? true : false;
            QCCD.DetectionPF_White = cbDetectionPF_White2.Checked ? true : false;
            QCCD.ClosingPF2 = cbClosingPF2.Checked ? true : false;
            QCCD.OpeningPF2 = cbOpeningPF2.Checked ? true : false;

            QCCD.iDynthresholdDarkPF2 = iDynthresholdDarkPF2;
            QCCD.iDynthresholdLightPF2 = iDynthresholdLightPF2;
            QCCD.iGraythresholdBlackPF2 = iGraythresholdBlackPF2;
            QCCD.iGraythresholdWhitePF2 = iGraythresholdWhitePF2;
            QCCD.iCloseWidthPF2 = iCloseWidthPF2;
            QCCD.iCloseHeightPF2 = iCloseHeightPF2;
            QCCD.iOpenWidthPF2 = iOpenWidthPF2;
            QCCD.iOpenHeightPF2 = iOpenHeightPF2;
            QCCD.iUnderSizeAreaPF2 = iUnderSizeAreaPF2;
            QCCD.iGlueAngleSetPF = iGlueAngleSetPF = Convert.ToInt32(nudGlueAngleSetPF.Value);
            QCCD.iGlueRatioSetPF = iGlueRatioSetPF = Convert.ToInt32(nudGlueRatioSetPF.Value);
            QCCD.dAngleSetPF = dAngleSetPF = Convert.ToInt32(nudAngleSetPF.Value);
            QCCD.AVI2IsCheck = cBQAVI2.Checked;
            QCCD.CompareSetPF = cmbCompareSetPF.SelectedIndex;

            if (QCCD.AVI2IsCheck)
            {
                #region  小台阶
                iniFile.Write("QCCD", "InRangePF", QCCD.dInRangePF.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "OutRangePF", QCCD.dOutRangePF.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "DetectionPF_Dark2", QCCD.DetectionPF_Dark2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "DetectionPF_Light2", QCCD.DetectionPF_Light2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "DetectionPF_Black", QCCD.DetectionPF_Black.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "DetectionPF_White", QCCD.DetectionPF_White.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "ClosingPF2", QCCD.ClosingPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "OpeningPF2", QCCD.OpeningPF2.ToString(), FrmMain.propath);

                iniFile.Write("QCCD", "DynthresholdDarkPF2", QCCD.iDynthresholdDarkPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "DynthresholdLightPF2", QCCD.iDynthresholdLightPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "GraythresholdBlackPF2", QCCD.iGraythresholdBlackPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "GraythresholdWhitePF2", QCCD.iGraythresholdWhitePF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "CloseWidthPF2", QCCD.iCloseWidthPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "CloseHeightPF2", QCCD.iCloseHeightPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "OpenWidthPF2", QCCD.iOpenWidthPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "OpenHeightPF2", QCCD.iOpenHeightPF2.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "UnderSizeAreaPF2", QCCD.iUnderSizeAreaPF2.ToString(), FrmMain.propath);

                iniFile.Write("QCCD", "GlueAngleSetPF", QCCD.iGlueAngleSetPF.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "GlueRatioSetPF", QCCD.iGlueRatioSetPF.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "AngleSetPF", QCCD.dAngleSetPF.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "AVI2ischecked", QCCD.AVI2IsCheck.ToString(), FrmMain.propath);
                iniFile.Write("QCCD", "CompareSetPF", QCCD.CompareSetPF.ToString(), FrmMain.propath);
                #endregion
            }
            if (QCCD.AVI1IsCheck || QCCD.AVI2IsCheck)
            {
                Barcode1.QCCDisChecked = false;
                iniFile.Write("BarcodeChoose", "QCCDchecked", Barcode1.QCCDisChecked.ToString(), parent.BarPath);
            }
        }

        private void hWImageSet_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (cbTransformOpen.Checked)
                hWindowControl1_HMouseDown(hWVision);
        }
        private void hWImageSet_HMouseUp(object sender, HMouseEventArgs e)
        {
            if (cbTransformOpen.Checked)
                hWindowControl1_HMouseUp(hWVision);
        }
        private void hWImageSet_HMouseWheel(object sender, HMouseEventArgs e)
        {
            if (cbTransformOpen.Checked)
                hWindowControl1_HMouseWheel(hWVision, e);
        }
        private void cbTransformOpen_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void btnShowOriginalImage_Click(object sender, EventArgs e)
        {
            try
            {
                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.GetImageSize(halcon.ImageOri[i_image], out width, out height);
                HOperatorSet.SetPart(hWVision, 0, 0, height, width);
                HOperatorSet.DispObj(ho_ImageSet, hWVision);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        HTuple m_hRowB, m_hColB, m_hButton, m_hRowE, m_hColE;
        public void hWindowControl1_HMouseDown(HTuple hWindow)
        {
            try
            {
                HOperatorSet.SetCheck("~give_error");
                HOperatorSet.GetMposition(hWindow, out m_hRowB, out m_hColB, out m_hButton);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        public void hWindowControl1_HMouseUp(HTuple hWindow)
        {
            try
            {
                HOperatorSet.SetCheck("~give_error");
                HOperatorSet.GetMposition(hWindow, out m_hRowE, out m_hColE, out m_hButton);

                HTuple row1, col1, row2, col2;
                double dbRowMove, dbColMove;
                double dbRowB, dbColB, dbRowE, dbColE;
                dbRowB = m_hRowB;
                dbRowE = m_hRowE;
                dbColB = m_hColB;
                dbColE = m_hColE;
                dbRowMove = -dbRowE + dbRowB;
                dbColMove = -dbColE + dbColB;

                HOperatorSet.GetPart(hWindow, out row1, out col1, out row2, out col2);
                HOperatorSet.SetPart(hWindow, row1 + dbRowMove, col1 + dbColMove, row2 + dbRowMove, col2 + dbColMove);

                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.ClearWindow(hWindow);
                HOperatorSet.DispObj(ho_ImageSet, hWVision);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        public void hWindowControl1_HMouseWheel(HTuple hWindow, HMouseEventArgs e)
        {
            try
            {
                double x = e.X;
                double y = e.Y;
                double Zoom = 1.0;
                double ZoomTrans = 1.0, ZoomOrg = 1.0, RowShif, ColShif;
                HTuple Row0, Column0, Row00, Column00, Ht, Wt, r1, c1, r2, c2;
                HTuple Row, Col, Button;

                HOperatorSet.SetCheck("~give_error");
                HOperatorSet.GetMposition(hWindow, out Row, out Col, out Button);
                HOperatorSet.GetPart(hWindow, out Row0, out Column0, out Row00, out Column00);

                if (e.Delta >= 0)
                {
                    if (0 != Zoom && Zoom >= 32.0)
                    {
                        Zoom = 32.0;
                    }
                    else
                    {
                        Zoom = 1.0 + Zoom;
                    }
                }
                else
                {
                    if (0 != Zoom && Zoom <= (1.0 / 16))
                    {
                        Zoom = 1.0 / 16;
                    }
                    else
                    {
                        Zoom = Zoom / 2;
                    }
                }
                ZoomTrans = Zoom / ZoomOrg;
                ZoomOrg = Zoom;
                RowShif = 0;
                ColShif = 0;
                Ht = Row00 - Row0;
                Wt = Column00 - Column0;
                r1 = (Row0 + ((1 - (1.0 / ZoomTrans)) * (Row - Row0))) - (RowShif / ZoomTrans);
                c1 = (Column0 + ((1 - (1.0 / ZoomTrans)) * (Col - Column0))) - (ColShif / ZoomTrans);
                r2 = r1 + (Ht / ZoomTrans);
                c2 = c1 + (Wt / ZoomTrans);

                HOperatorSet.SetPart(hWindow, r1, c1, r2, c2);

                int i_image = int.Parse(SetNum) - 1;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i_image], out ho_ImageSet);
                HOperatorSet.ClearWindow(hWindow);
                HOperatorSet.DispObj(ho_ImageSet, hWVision);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }

        private void btnImageProPlus_Click(object sender, EventArgs e)
        {
            halcon.HWindowID[8] = hWVision;
            FrmVisionSet.xpm = QCCD.xpm;
            FrmVisionSet.ypm = QCCD.ypm;
            HD.ImagePro9(halcon.HWindowID[8]);
        }

        private void cbDetection_Black_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.Detection_Black = cbDetection_Black.Checked ? true : false;
        }

        private void cbDetection_White_CheckedChanged(object sender, EventArgs e)
        {
            QCCD.Detection_White = cbDetection_White.Checked ? true : false;
        }

        private void nudGlueAngleRatio_ValueChanged(object sender, EventArgs e)
        {
            Glue.GlueAngleRatio = (double)nudGlueAngleRatio.Value;
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "";

            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            iniFile.Write(CCDNAME, "GlueAngleRatio", Glue.GlueAngleRatio.ToString(), FrmMain.propath);

        }

        private void cbGlue_Circle_2_CheckedChanged(object sender, EventArgs e)
        {
            Glue.Glue_Circle_2 = cbGlue_Circle_2.Checked;
            if (Glue.Glue_Circle_2)
            {
                ucGlue_Circle_OuterRadius_2.Enabled = true;
                ucGlue_Circle_InnerRadius_2.Enabled = true;
                ucGlue_Circle_StartAngle_2.Enabled = true;
                ucGlue_Circle_EndAngle_2.Enabled = true;
            }
            else
            {
                ucGlue_Circle_OuterRadius_2.Enabled = false;
                ucGlue_Circle_InnerRadius_2.Enabled = false;
                ucGlue_Circle_StartAngle_2.Enabled = false;
                ucGlue_Circle_EndAngle_2.Enabled = false;
            }

            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "";
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            iniFile.Write(CCDNAME, "Glue_Circle_2", Glue.Glue_Circle_2.ToString(), FrmMain.propath);
        }

        private void ucGlueGray_2_ValueChanged(int CurrentValue)
        {
            if (readpara)
                return;
            Glue.Glue_Circle_Gray_2 = ucGlueGray_2.Value;
            //if (cBglueChose.SelectedIndex == 0)
            //    ShowGlueFang();
            if (cBglueChose.SelectedIndex == 1)
            {
                int i = int.Parse(SetNum) - 1;
                if (halcon.Image[i] == null)
                    return;
                ho_ImageSet.Dispose();
                HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                hWVision.ClearWindow();
                hWVision.DispObj(ho_ImageSet);
                ShowGlueSectorGray_2(Glue.Glue_Circle_Gray_2);
            }
        }

        private void ucGlue_Circle_OuterRadius_2_ValueChanged(int CurrentValue)
        {
            if (readpara)
                return;
            Glue.Glue_Circle_OuterRadius_2 = ucGlue_Circle_OuterRadius_2.Value;
            int i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            ShowGlueSector_2(Glue.Glue_Circle_OuterRadius_2, Glue.Glue_Circle_InnerRadius_2, Glue.Glue_Circle_StartAngle_2, Glue.Glue_Circle_EndAngle_2);
            ShowGlueSectorGray_2(Glue.Glue_Circle_Gray_2);
        }

        private void ucGlue_Circle_InnerRadius_2_ValueChanged(int CurrentValue)
        {
            if (readpara)
                return;
            Glue.Glue_Circle_InnerRadius_2 = ucGlue_Circle_InnerRadius_2.Value;
            int i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            ShowGlueSector_2(Glue.Glue_Circle_OuterRadius_2, Glue.Glue_Circle_InnerRadius_2, Glue.Glue_Circle_StartAngle_2, Glue.Glue_Circle_EndAngle_2);
            ShowGlueSectorGray_2(Glue.Glue_Circle_Gray_2);
        }

        private void ucGlue_Circle_StartAngle_2_ValueChanged(int CurrentValue)
        {
            if (readpara)
                return;
            Glue.Glue_Circle_StartAngle_2 = ucGlue_Circle_StartAngle_2.Value;
            int i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            ShowGlueSector_2(Glue.Glue_Circle_OuterRadius_2, Glue.Glue_Circle_InnerRadius_2, Glue.Glue_Circle_StartAngle_2, Glue.Glue_Circle_EndAngle_2);
            ShowGlueSectorGray_2(Glue.Glue_Circle_Gray_2);
        }

        private void ucGlue_Circle_EndAngle_2_ValueChanged(int CurrentValue)
        {
            if (readpara)
                return;
            Glue.Glue_Circle_EndAngle_2 = ucGlue_Circle_EndAngle_2.Value;
            int i = int.Parse(SetNum) - 1;
            if (halcon.Image[i] == null)
                return;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            hWVision.ClearWindow();
            hWVision.DispObj(ho_ImageSet);
            ShowGlueSector_2(Glue.Glue_Circle_OuterRadius_2, Glue.Glue_Circle_InnerRadius_2, Glue.Glue_Circle_StartAngle_2, Glue.Glue_Circle_EndAngle_2);
            ShowGlueSectorGray_2(Glue.Glue_Circle_Gray_2);
        }

        private void nudGlueAngleRatio_2_ValueChanged(object sender, EventArgs e)
        {
            Glue.GlueAngleRatio_2 = (double)nudGlueAngleRatio_2.Value;
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            iniFile.Write(CCDNAME, "GlueAngleRatio_2", Glue.GlueAngleRatio_2.ToString(), FrmMain.propath);
        }

        private void btnGlueTest_Click(object sender, EventArgs e)
        {
            HObject ho_gGreyUnion = null, ho_AllRegionXLDPF = null, ho_SelectRegionPF = null, ho_IgnoreRegionPF = null;
            HObject ho_gGreyUnion_2 = null, ho_AllRegionXLDPF_2 = null, ho_SelectRegionPF_2 = null, ho_IgnoreRegionPF_2 = null;
            HOperatorSet.GenEmptyObj(out ho_gGreyUnion);
            HOperatorSet.GenEmptyObj(out ho_AllRegionXLDPF);
            HOperatorSet.GenEmptyObj(out ho_SelectRegionPF);
            HOperatorSet.GenEmptyObj(out ho_IgnoreRegionPF);
            HOperatorSet.GenEmptyObj(out ho_gGreyUnion_2);
            HOperatorSet.GenEmptyObj(out ho_AllRegionXLDPF_2);
            HOperatorSet.GenEmptyObj(out ho_SelectRegionPF_2);
            HOperatorSet.GenEmptyObj(out ho_IgnoreRegionPF_2);
            try
            {
                if (cBglueChose.SelectedIndex == 1)
                {
                    int i = int.Parse(SetNum) - 1;
                    ho_ImageSet.Dispose();
                    HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
                    hWVision.ClearWindow();
                    hWVision.DispObj(ho_ImageSet);
                    ShowGlueSectorGray();

                    //ho_gGreyUnion.Dispose();
                    //HOperatorSet.Union2(ho_g2Grey, ho_g1Grey, out ho_gGreyUnion);
                    ho_AllRegionXLDPF.Dispose(); ho_SelectRegionPF.Dispose(); ho_IgnoreRegionPF.Dispose();
                    find_ring_angle(ho_GlueRegion, out ho_AllRegionXLDPF, out ho_SelectRegionPF, out ho_IgnoreRegionPF, hv_RowCenter, hv_ColCenter, MidCirRadius + RegionWidth / 2,
                MidCirRadius - RegionWidth / 2, 2, 0, 360, 1, out hv_TotalAnglePF);

                    //區域二 灰度值檢測
                    if (Glue.Glue_Circle_2)
                    {
                        ShowGlueSectorGray_2(Glue.Glue_Circle_Gray_2);
                        //ho_gGreyUnion.Dispose();
                        //HOperatorSet.Union2(ho_g2Grey_2, ho_g1Grey_2, out ho_gGreyUnion_2);
                        ho_AllRegionXLDPF_2.Dispose(); ho_SelectRegionPF_2.Dispose(); ho_IgnoreRegionPF_2.Dispose();
                        find_ring_angle(ho_GlueRegion_2, out ho_AllRegionXLDPF_2, out ho_SelectRegionPF_2, out ho_IgnoreRegionPF_2, hv_RowCenter, hv_ColCenter, Glue.Glue_Circle_OuterRadius_2,
                    Glue.Glue_Circle_InnerRadius_2, 2, 0, 360, 1, out hv_TotalAnglePF_2);
                        disp_message(hWVision, "GlueAngle2:" + Math.Round(hv_TotalAnglePF_2.D).ToString(), "", 1780, 150, "green", "false");

                    }
                    disp_message(hWVision, "GlueAngle:  " + Math.Round(hv_TotalAnglePF.D).ToString(), "", 1720, 150, "green", "false");

                }
            }
            catch
            {
            }

        }
        HTuple hv_TotalAngle = new HTuple(), hv_TotalAnglePF = new HTuple();
        HTuple hv_TotalAngle_2 = new HTuple(), hv_TotalAnglePF_2 = new HTuple();
        // Short Description: 計算圓環區域總角度 
        public void find_ring_angle(HObject ho_Region, out HObject ho_AllRingXLD, out HObject ho_SelectRegion,
            out HObject ho_IgnoreRegion, HTuple hv_RingRow, HTuple hv_RingColumn, HTuple hv_RingOuterDiameter,
            HTuple hv_RingInnerDiameter, HTuple hv_AngleSet, HTuple hv_StartAngle, HTuple hv_EndAngle,
            HTuple hv_RatioSet, out HTuple hv_TotalAngle)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_CircleSectorOut = null, ho_CircleSectorIn = null;
            HObject ho_CircleSector = null, ho_SectorXLD = null, ho_RegionIntersection = null;

            // Local control variables 

            HTuple hv_i = null, hv_AreaCircleSector = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_AreaSectorIntersection = new HTuple(), hv_Ratio = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AllRingXLD);
            HOperatorSet.GenEmptyObj(out ho_SelectRegion);
            HOperatorSet.GenEmptyObj(out ho_IgnoreRegion);
            HOperatorSet.GenEmptyObj(out ho_CircleSectorOut);
            HOperatorSet.GenEmptyObj(out ho_CircleSectorIn);
            HOperatorSet.GenEmptyObj(out ho_CircleSector);
            HOperatorSet.GenEmptyObj(out ho_SectorXLD);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            try
            {
                ho_AllRingXLD.Dispose();
                HOperatorSet.GenEmptyObj(out ho_AllRingXLD);
                ho_SelectRegion.Dispose();
                HOperatorSet.GenEmptyObj(out ho_SelectRegion);
                ho_IgnoreRegion.Dispose();
                HOperatorSet.GenEmptyObj(out ho_IgnoreRegion);
                hv_TotalAngle = 0;
                HTuple end_val4 = hv_EndAngle - hv_AngleSet;
                HTuple step_val4 = hv_AngleSet;
                for (hv_i = hv_StartAngle; hv_i.Continue(end_val4, step_val4); hv_i = hv_i.TupleAdd(step_val4))
                {
                    ho_CircleSectorOut.Dispose();
                    HOperatorSet.GenCircleSector(out ho_CircleSectorOut, hv_RingRow, hv_RingColumn,
                        hv_RingOuterDiameter, hv_i.TupleRad(), ((hv_i + hv_AngleSet)).TupleRad()
                        );
                    ho_CircleSectorIn.Dispose();
                    HOperatorSet.GenCircleSector(out ho_CircleSectorIn, hv_RingRow, hv_RingColumn,
                        hv_RingInnerDiameter, hv_i.TupleRad(), ((hv_i + hv_AngleSet)).TupleRad()
                        );
                    ho_CircleSector.Dispose();
                    HOperatorSet.Difference(ho_CircleSectorOut, ho_CircleSectorIn, out ho_CircleSector
                        );
                    //把每個區域填到數組中
                    ho_SectorXLD.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_CircleSector, out ho_SectorXLD, "border");
                    HOperatorSet.AreaCenter(ho_CircleSector, out hv_AreaCircleSector, out hv_Row,
                        out hv_Column);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_SectorXLD, ho_AllRingXLD, out ExpTmpOutVar_0);
                        ho_AllRingXLD.Dispose();
                        ho_AllRingXLD = ExpTmpOutVar_0;
                    }
                    //求出膠和圓弧的交集
                    ho_RegionIntersection.Dispose();
                    HOperatorSet.Intersection(ho_CircleSector, ho_Region, out ho_RegionIntersection
                        );
                    HOperatorSet.AreaCenter(ho_RegionIntersection, out hv_AreaSectorIntersection,
                        out hv_Row, out hv_Column);
                    //算出交集比率>多少以上就算有膠
                    hv_Ratio = (hv_AreaSectorIntersection * 100) / hv_AreaCircleSector;
                    if ((int)(new HTuple(hv_Ratio.TupleGreater(hv_RatioSet))) != 0)
                    {
                        hv_TotalAngle = hv_TotalAngle + hv_AngleSet;
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Union2(ho_SelectRegion, ho_RegionIntersection, out ExpTmpOutVar_0
                                );
                            ho_SelectRegion.Dispose();
                            ho_SelectRegion = ExpTmpOutVar_0;
                        }
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Union2(ho_IgnoreRegion, ho_RegionIntersection, out ExpTmpOutVar_0
                                );
                            ho_IgnoreRegion.Dispose();
                            ho_IgnoreRegion = ExpTmpOutVar_0;
                        }
                    }

                }
                ho_CircleSectorOut.Dispose();
                ho_CircleSectorIn.Dispose();
                ho_CircleSector.Dispose();
                ho_SectorXLD.Dispose();
                ho_RegionIntersection.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_CircleSectorOut.Dispose();
                ho_CircleSectorIn.Dispose();
                ho_CircleSector.Dispose();
                ho_SectorXLD.Dispose();
                ho_RegionIntersection.Dispose();

                throw HDevExpDefaultException;
            }
        }
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
           HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            // Local control variables 
            HTuple hv_Red, hv_Green, hv_Blue, hv_Row1Part;
            HTuple hv_Column1Part, hv_Row2Part, hv_Column2Part, hv_RowWin;
            HTuple hv_ColumnWin, hv_WidthWin, hv_HeightWin, hv_MaxAscent;
            HTuple hv_MaxDescent, hv_MaxWidth, hv_MaxHeight, hv_R1 = new HTuple();
            HTuple hv_C1 = new HTuple(), hv_FactorRow = new HTuple(), hv_FactorColumn = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Index = new HTuple(), hv_Ascent = new HTuple();
            HTuple hv_Descent = new HTuple(), hv_W = new HTuple(), hv_H = new HTuple();
            HTuple hv_FrameHeight = new HTuple(), hv_FrameWidth = new HTuple();
            HTuple hv_R2 = new HTuple(), hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_CurrentColor = new HTuple();

            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            // Initialize local and output iconic variables 

            //This procedure displays text in a graphics window.        
            //prepare window
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
                out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            //
            //default settings
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            //
            hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
            //
            //Estimate extentions of text depending on font size.
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //display text box depending on text size
            if ((int)(new HTuple(hv_Box.TupleEqual("true"))) != 0)
            {
                //calculate box extents
                hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                HOperatorSet.SetColor(hv_WindowHandle, "light gray");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 3, hv_C1 + 3, hv_R2 + 3, hv_C2 + 3);
                HOperatorSet.SetColor(hv_WindowHandle, "white");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            else if ((int)(new HTuple(hv_Box.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Box";
                throw new HalconException(hv_Exception);
            }
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                    )));
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                    hv_Index));
            }
            //reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }
        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
           HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = null, hv_Fonts = new HTuple();
            HTuple hv_Style = null, hv_Exception = new HTuple(), hv_AvailableFonts = null;
            HTuple hv_Fdx = null, hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
            HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();

            // Initialize local and output iconic variables 
            //This procedure sets the text font of the current window with
            //the specified attributes.
            //
            //Input parameters:
            //WindowHandle: The graphics window for which the font will be set
            //Size: The font size. If Size=-1, the default of 16 is used.
            //Bold: If set to 'true', a bold font is used
            //Slant: If set to 'true', a slanted font is used
            //
            HOperatorSet.GetSystem("operating_system", out hv_OS);
            // dev_get_preferences(...); only in hdevelop
            // dev_set_preferences(...); only in hdevelop
            if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
            {
                hv_Size_COPY_INP_TMP = 16;
            }
            if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
            {
                //Restore previous behaviour
                hv_Size_COPY_INP_TMP = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt();
            }
            else
            {
                hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP.TupleInt();
            }
            if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Courier";
                hv_Fonts[1] = "Courier 10 Pitch";
                hv_Fonts[2] = "Courier New";
                hv_Fonts[3] = "CourierNew";
                hv_Fonts[4] = "Liberation Mono";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Consolas";
                hv_Fonts[1] = "Menlo";
                hv_Fonts[2] = "Courier";
                hv_Fonts[3] = "Courier 10 Pitch";
                hv_Fonts[4] = "FreeMono";
                hv_Fonts[5] = "Liberation Mono";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Luxi Sans";
                hv_Fonts[1] = "DejaVu Sans";
                hv_Fonts[2] = "FreeSans";
                hv_Fonts[3] = "Arial";
                hv_Fonts[4] = "Liberation Sans";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Times New Roman";
                hv_Fonts[1] = "Luxi Serif";
                hv_Fonts[2] = "DejaVu Serif";
                hv_Fonts[3] = "FreeSerif";
                hv_Fonts[4] = "Utopia";
                hv_Fonts[5] = "Liberation Serif";
            }
            else
            {
                hv_Fonts = hv_Font_COPY_INP_TMP.Clone();
            }
            hv_Style = "";
            if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
            {
                hv_Style = hv_Style + "Bold";
            }
            else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Bold";
                throw new HalconException(hv_Exception);
            }
            if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
            {
                hv_Style = hv_Style + "Italic";
            }
            else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Slant";
                throw new HalconException(hv_Exception);
            }
            if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
            {
                hv_Style = "Normal";
            }
            HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
            hv_Font_COPY_INP_TMP = "";
            for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
            {
                hv_Indices = hv_AvailableFonts.TupleFind(hv_Fonts.TupleSelect(hv_Fdx));
                if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_Fdx);
                        break;
                    }
                }
            }
            if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                throw new HalconException("Wrong value of control parameter Font");
            }
            hv_Font_COPY_INP_TMP = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
            HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);
            // dev_set_preferences(...); only in hdevelop

            return;
        }

        private void cbGlueFollow_CheckedChanged(object sender, EventArgs e)
        {
            Glue.Glue_Follow = cbGlueFollow.Checked;
           
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area3 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == "") ||
                                 (cBtest.Enabled && cBtest.Text == ""))
                return;
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
          
            if (SetNum == "8")
            {
               
                iniFile.Write(CCDNAME, "Glue_Follow", Glue.Glue_Follow.ToString(), FrmMain.propath);

            }
        }

        private void cBCutGQ_CheckedChanged(object sender, EventArgs e)
        {
            //if (cBCutGQ.Checked == false)
            //{
            //    if (Glue.Glue_Follow)
            //    {
            //        cBCutGQ.Checked = true;
            //        MessageBox.Show("膠後跟隨開啟中,無法關閉");
            //    }
            //}
        }

        private void cBCutGH_CheckedChanged(object sender, EventArgs e)
        {
            //if (cBCutGH.Checked == false)
            //{
            //    if (Glue.Glue_Follow)
            //    {
            //        cBCutGH.Checked = true;
            //        MessageBox.Show("膠後跟隨開啟中,無法關閉");
            //    }
            //}
        }

        private void btnDrawModel_Click(object sender, EventArgs e)
        {
            if (halcon.IsPreview)
            {
                MessageBox.Show("請先停止預覽");
                return;
            }
            if (bDrawing)
                return;
            bDrawing = true;
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject();
            HObject ho_Circle = new HObject(), ho_ReducedImage = new HObject(), ho_ImageMedian = new HObject(), ho_ImageEmphasize = new HObject();
            HObject ho_Circle_Outer = new HObject(), ho_Circle_Inner = new HObject(), ho_Rectangle = new HObject(), ho_RegionIntersection = new HObject();
            HObject ho_RegionDifference = new HObject(), ho_Region = new HObject(), ho_ModelContours = new HObject(), ho_TransContours = new HObject();
            HObject ho_RegionUnion = new HObject();
            HTuple hv_Height = new HTuple(), hv_Width = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Radius = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple(), hv_FirstRadius = new HTuple();
            HTuple hv_ModelID = new HTuple(), hv_NumLevels = new HTuple(), hv_AngleStart = new HTuple(), hv_AngleExtent = new HTuple(), hv_AngleStep = new HTuple();
            HTuple hv_ScaleMin = new HTuple(), hv_ScaleMax = new HTuple(), hv_ScaleStep = new HTuple(), hv_Metric = new HTuple(), hv_MinContrast = new HTuple();
            HTuple hv_Angle = new HTuple(), hv_Score = new HTuple();
            HTuple hv_HomMat2D = new HTuple(), hv_RectangleRow = new HTuple(), hv_RectangleColumn = new HTuple(), hv_RectanglePhi = new HTuple(), hv_RectangleLength1 = new HTuple(), hv_RectangleLength2 = new HTuple();
            HOperatorSet.SetDraw(Window, "margin");
            HOperatorSet.SetColor(Window, "green");

            try
            {
                int i_image = int.Parse(SetNum) - 1;
                ho_Image.Dispose();
                ho_Image = halcon.Image[i_image].CopyObj(1, -1);
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetPart(Window, 0, 0, hv_Height - 1, hv_Width - 1);
                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_Image, out ho_ImageMedian, 50, 50);
                ho_ImageEmphasize.Dispose();
                HOperatorSet.Emphasize(ho_ImageMedian, out ho_ImageEmphasize, 100, 100, 1);
                Window.ClearWindow();
                ho_ImageEmphasize.DispObj(Window);
                disp_message(Window, "1.畫產品外圍", "", 0, 0, "green", "false");
                HOperatorSet.DrawCircle(Window, out hv_Row, out hv_Column, out hv_Radius);
                ho_Circle_Outer.Dispose();
                HOperatorSet.GenCircle(out ho_Circle_Outer, hv_Row, hv_Column, hv_Radius);
                ho_Circle_Outer.DispObj(Window);
                disp_message(Window, "2.畫產品內圍", "", 40, 0, "green", "false");
                HOperatorSet.DrawCircle(Window, out hv_Row, out hv_Column, out hv_Radius);
                ho_Circle_Inner.Dispose();
                HOperatorSet.GenCircle(out ho_Circle_Inner, hv_Row, hv_Column, hv_Radius);
                ho_Circle_Inner.DispObj(Window);
                //內外圓相減
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Circle_Outer, ho_Circle_Inner, out ho_RegionDifference);
                disp_message(Window, "3.畫去除區域方形1", "", 80, 0, "green", "false");
                HOperatorSet.DrawRectangle2(Window, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Difference(ho_RegionDifference, ho_Rectangle, out ExpTmpOutVar_0);
                    ho_RegionDifference.Dispose();
                    ho_RegionDifference = ExpTmpOutVar_0;
                }
                Window.ClearWindow();
                ho_ImageEmphasize.DispObj(Window);
                ho_RegionDifference.DispObj(Window);
                disp_message(Window, "4.畫去除區域方形2", "", 100, 0, "green", "false");
                HOperatorSet.DrawRectangle2(Window, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Difference(ho_RegionDifference, ho_Rectangle, out ExpTmpOutVar_0);
                    ho_RegionDifference.Dispose();
                    ho_RegionDifference = ExpTmpOutVar_0;
                }
                ho_ReducedImage.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageEmphasize, ho_RegionDifference, out ho_ReducedImage);
                //HOperatorSet.CreateShapeModel(ho_ReducedImage, "auto", new HTuple(0).TupleRad(), new HTuple(360).TupleRad(), "auto", "auto", "use_polarity", "auto", "auto", out hv_ModelID);
            }
            catch
            {
            }
            try
            {
                HOperatorSet.CreateShapeModel(ho_ReducedImage, "auto", new HTuple(0).TupleRad(), new HTuple(360).TupleRad(), "auto", "auto", "use_polarity", "auto", "auto", out hv_ModelID); HOperatorSet.GetShapeModelParams(hv_ModelID, out hv_NumLevels, out hv_AngleStart, out hv_AngleExtent, out hv_AngleStep, out hv_ScaleMin, out hv_ScaleMax, out hv_ScaleStep, out hv_Metric, out hv_MinContrast);
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
                //HOperatorSet.FindShapeModel(ho_ImageEmphasize, hv_ModelID, (new HTuple(0)).TupleRad(), (new HTuple(360)).TupleRad(), 0.5, 1, 0.5, "least_squares", (new HTuple(7)).TupleConcat(1), 0.75, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);
                HOperatorSet.FindShapeModel(ho_ImageEmphasize, hv_ModelID, (new HTuple(0)).TupleRad(), (new HTuple(360)).TupleRad(), 0.5, 1, 0.5, "least_squares", (new HTuple(6)).TupleConcat(1), 0.75, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);
                if (hv_Score.TupleGreater(0) != 0)
                {
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_Angle, 0, 0, out hv_HomMat2D);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_Row, hv_Column, out hv_HomMat2D);
                    ho_TransContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours, hv_HomMat2D);

                    Window.ClearWindow();
                    ho_Image.DispObj(Window);
                    Window.SetColor("yellow");
                    ho_RegionDifference.DispObj(Window);
                    disp_message(Window, "模組分數:" + Math.Round(hv_Score.D * 100, 0), "", 0, 0, "green", "false");
                    if (MessageBox.Show("是否儲存模組?", "模組設定", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        string Path = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME;
                        if (!Directory.Exists(Path))
                        {
                            Directory.CreateDirectory(Path);
                        }
                        HOperatorSet.WriteShapeModel(hv_ModelID, Path + "\\Arc_Model");
                    }
                }
            }
            catch
            {
            }
            HOperatorSet.ClearShapeModel(hv_ModelID);
            bDrawing = false;
            ho_Image.Dispose();
            ho_Circle.Dispose();
            ho_ReducedImage.Dispose();
            ho_ImageMedian.Dispose();
            ho_ImageEmphasize.Dispose();
            ho_ModelContours.Dispose();
            ho_Region.Dispose();
            ho_RegionUnion.Dispose();
            ho_Rectangle.Dispose();
            ho_Circle_Outer.Dispose();
            ho_Circle_Inner.Dispose();
            ho_RegionDifference.Dispose();
            ho_RegionIntersection.Dispose();
        }

        private void btnFindModel_Click(object sender, EventArgs e)
        {
            RegionRadius = (HTuple)UDRegion2.Value;
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject(), ho_Circle = new HObject(), ho_ReducedImage = new HObject(), ho_ImageMedian = new HObject();
            HObject ho_ImageEmphasize = new HObject(), ho_ModelContours = new HObject(), ho_TransContours = new HObject();
            HObject ho_RegionUnion = new HObject(), ho_SelectedContours = new HObject();
            HTuple hv_ModelID = new HTuple(), hv_HomMat2D = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Width = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Radius = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Angle = new HTuple(), hv_Score = new HTuple();
            HTuple hv_StartPhi = new HTuple(), hv_EndPhi = new HTuple(), hv_PointOrder = new HTuple();
            HTuple hv_RectangleRow = new HTuple(), hv_RectangleColumn = new HTuple(), hv_RectanglePhi = new HTuple(), hv_RectangleLength1 = new HTuple(), hv_RectangleLength2 = new HTuple();
            try
            {
                string Path = Sys.IniPath + "\\" + Sys.CurrentProduction + "\\" + CCDNAME;
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }
                HOperatorSet.ReadShapeModel(Path + "\\Arc_Model", out hv_ModelID);
            }
            catch
            {
                MessageBox.Show("請建立初始模組");
                return;
            }
            int i_image = int.Parse(SetNum) - 1;
            ho_Image.Dispose();
            ho_Image = halcon.Image[i_image].CopyObj(1, -1);
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            HOperatorSet.SetPart(Window, 0, 0, hv_Height - 1, hv_Width - 1);
            ho_Circle.Dispose();
            HOperatorSet.GenCircle(out ho_Circle, hv_Height / 2, hv_Width / 2, RegionRadius);
            ho_ReducedImage.Dispose();
            HOperatorSet.ReduceDomain(ho_Image, ho_Circle, out ho_ReducedImage);
            ho_ImageMedian.Dispose();
            HOperatorSet.MedianRect(ho_ReducedImage, out ho_ImageMedian, 50, 50);
            ho_ImageEmphasize.Dispose();
            HOperatorSet.Emphasize(ho_ImageMedian, out ho_ImageEmphasize, 100, 100, 1);
            try
            {
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
                HOperatorSet.FindShapeModel(ho_ImageEmphasize, hv_ModelID, (new HTuple(0)).TupleRad(), (new HTuple(360)).TupleRad(), 0.5, 0, 0.5, "least_squares", (new HTuple(6)).TupleConcat(1), 0.75, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);
                if (hv_Score.TupleGreater(0) != 0)
                {
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_Angle[0], 0, 0, out hv_HomMat2D);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_Row[0], hv_Column[0], out hv_HomMat2D);
                    ho_TransContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours, hv_HomMat2D);
                    ho_SelectedContours.Dispose();
                    HOperatorSet.SelectContoursXld(ho_TransContours, out ho_SelectedContours, "contour_length", 100, 99999, -0.5, 0.5);
                    //初步利用圓弧擬合圓
                    HOperatorSet.FitCircleContourXld(ho_SelectedContours, "ahuber", -1, 2, 0, 3, 2, out hv_Row, out hv_Column, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                    HOperatorSet.GenCircle(out ho_Circle, hv_Row[0], hv_Column[0], hv_Radius[0]);

                    HOperatorSet.SetDraw(Window, "margin");
                    Window.ClearWindow();
                    ho_Image.DispObj(Window);
                    Window.SetColor("green");
                    ho_Circle.DispObj(Window);
                    disp_message(Window, "模組分數:" + Math.Round(hv_Score[0].D * 100, 0), "", 0, 0, "green", "false");
                    halcon.hv_FirstRow = hv_Row[0];
                    halcon.hv_FirstColumn = hv_Column[0];
                }
                HOperatorSet.ClearShapeModel(hv_ModelID);
            }
            catch
            {
            }
        }

        private void cmbCircleMeasureSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            halcon.CircleMeasureSelect = cmbCircleMeasureSelect.SelectedIndex == 0 ? "first" : "last";
        }

        private void ucCircleRadius_ValueChanged(int CurrentValue)
        {
            halcon.CircleRadius = ucCircleRadius.Value;
            if (readpara)
                return;
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject(), ho_UsedEdges = new HObject(), ho_Contour = new HObject(), ho_ResultContours = new HObject(), ho_CrossCenter = new HObject();
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            int i_image = int.Parse(SetNum) - 1;
            ho_Image.Dispose();
            ho_Image = halcon.Image[i_image].CopyObj(1, -1);
            HD.GenAngleMode6(ho_Image, out ho_UsedEdges, out ho_Contour, out ho_ResultContours, out ho_CrossCenter, out halcon.hv_ResultRow, out halcon.hv_ResultColumn, out halcon.hv_ResultRadius);
            ho_Image.DispObj(Window);
            Window.SetColor("green");
            ho_Contour.DispObj(Window);
            Window.SetColor("blue");
            ho_ResultContours.DispObj(Window);
            Window.SetColor("red");
            ho_UsedEdges.DispObj(Window);
        }

        private void ucCircleLength_ValueChanged(int CurrentValue)
        {
            halcon.CircleLength = ucCircleLength.Value;
            if (readpara)
                return;
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject(), ho_UsedEdges = new HObject(), ho_Contour = new HObject(), ho_ResultContours = new HObject(), ho_CrossCenter = new HObject();
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            int i_image = int.Parse(SetNum) - 1;
            ho_Image.Dispose();
            ho_Image = halcon.Image[i_image].CopyObj(1, -1);
            HD.GenAngleMode6(ho_Image, out ho_UsedEdges, out ho_Contour, out ho_ResultContours, out ho_CrossCenter, out halcon.hv_ResultRow, out halcon.hv_ResultColumn, out halcon.hv_ResultRadius);
            ho_Image.DispObj(Window);
            Window.SetColor("green");
            ho_Contour.DispObj(Window);
            Window.SetColor("blue");
            ho_ResultContours.DispObj(Window);
            Window.SetColor("red");
            ho_UsedEdges.DispObj(Window);
        }

        private void ucCircleBlack2White_ValueChanged(int CurrentValue)
        {
            halcon.CircleMeasureTransition = "positive";
            halcon.CircleMeasureThreshold = ucCircleBlack2White.Value;
            if (readpara)
                return;
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject(), ho_UsedEdges = new HObject(), ho_Contour = new HObject(), ho_ResultContours = new HObject(), ho_CrossCenter = new HObject();
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            int i_image = int.Parse(SetNum) - 1;
            ho_Image.Dispose();
            ho_Image = halcon.Image[i_image].CopyObj(1, -1);
            HD.GenAngleMode6(ho_Image, out ho_UsedEdges, out ho_Contour, out ho_ResultContours, out ho_CrossCenter, out halcon.hv_ResultRow, out halcon.hv_ResultColumn, out halcon.hv_ResultRadius);
            ho_Image.DispObj(Window);
            Window.SetColor("green");
            ho_Contour.DispObj(Window);
            Window.SetColor("blue");
            ho_ResultContours.DispObj(Window);
            Window.SetColor("red");
            ho_UsedEdges.DispObj(Window);
        }

        private void ucCircleWhite2Black_ValueChanged(int CurrentValue)
        {
            halcon.CircleMeasureTransition = "negative";
            halcon.CircleMeasureThreshold = ucCircleWhite2Black.Value;
            if (readpara)
                return;
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject(), ho_UsedEdges = new HObject(), ho_Contour = new HObject(), ho_ResultContours = new HObject(), ho_CrossCenter = new HObject();
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            int i_image = int.Parse(SetNum) - 1;
            ho_Image.Dispose();
            ho_Image = halcon.Image[i_image].CopyObj(1, -1);
            HD.GenAngleMode6(ho_Image, out ho_UsedEdges, out ho_Contour, out ho_ResultContours, out ho_CrossCenter, out halcon.hv_ResultRow, out halcon.hv_ResultColumn, out halcon.hv_ResultRadius);
            ho_Image.DispObj(Window);
            Window.SetColor("green");
            ho_Contour.DispObj(Window);
            Window.SetColor("blue");
            ho_ResultContours.DispObj(Window);
            Window.SetColor("red");
            ho_UsedEdges.DispObj(Window);
        }


        private void FindCircleCenter_Click(object sender, EventArgs e)
        {
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject(), ho_UsedEdges = new HObject(), ho_Contour = new HObject(), ho_ResultContours = new HObject(), ho_CrossCenter = new HObject();
            string CCDNAME = ""; string cn = ""; string area = "", area1 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == "") ||
                                 (cBLocation4.Enabled && cBLocation4.Text == ""))
                return;
            if (cBLocation.SelectedIndex == 0)
                area = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area = "Platform";
            area1 = (cBLocation2.SelectedIndex + 1).ToString();
            area4 = (cBLocation4.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area1; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if (SetNum == "1" || SetNum == "3")
            {
                CCDNAME = CCDNAME + "-" + area4;
                iniFile.Write(CCDNAME, "Location", area4, FrmMain.propath);
            }
            if (SetNum == "2" || SetNum == "4" || SetNum == "6")
                iniFile.Write(cn, "Location", area, FrmMain.propath);
            if (SetNum == "8")
                iniFile.Write(cn, "Location", area1, FrmMain.propath);
            int i_image = int.Parse(SetNum) - 1;
            ho_Image.Dispose();
            ho_Image = halcon.Image[i_image].CopyObj(1, -1);
            HD.GenAngleMode6(ho_Image, out ho_UsedEdges, out ho_Contour, out ho_ResultContours, out ho_CrossCenter, out halcon.hv_ResultRow, out halcon.hv_ResultColumn, out halcon.hv_ResultRadius);
            ho_Image.DispObj(Window);
            Window.SetColor("green");
            ho_ResultContours.DispObj(Window);
            Window.SetColor("blue");
            ho_CrossCenter.DispObj(Window);
        }

        #region WGB增加
        HTuple dOutRangeRadius = 1;
        HTuple dInRangeRadius = 1;
        HTuple dGraythreshold = 1;
        HTuple dMeasurethreshold = 1;
        HTuple dMeasureLength1 = 1;
        HTuple dMeasureLength2 = 1;
        HTuple dAreaLimitUp = 10000;
        HTuple dAreaLimitDown = 1;    
        HTuple dWidthLimitUp = 1000;
        HTuple dWidthLimitDown = 1;
        HTuple dHeightLimitUp = 1000;
        HTuple dHeightLimitDown = 1;
        int DarkLightChoice1 = 0;  //灰度值   0：黑找白，1：白找黑
        int DarkLightChoice2 = 0;    //边缘阈值 0: 黑找白，1：白找黑
        int DarklightChoce3 = 0;   //拟合位置 0：First,1:Last        
        double dLimitUp = 3.82;
        double dLimitDown = 3.79;
        double doffset = 0.000;      //缺口补偿
        double dlimitUpGap = 7.66;   //对称缺口上限
        double dlimitDownGap = 7.55; //对称缺口下限
        double dGapoffset = 0.000;      //对称缺口补偿

        #region 事件
        private void SUCRingOutRange_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            int i_image = int.Parse(SetNum) - 1;
            dOutRangeRadius = SUCRingOutRange.Value;
            FixationRing(halcon.Image[i_image], hWImageSet.HalconWindow, dInRangeRadius, dOutRangeRadius, hv_RowCenter, hv_ColCenter);
            Thread.Sleep(1);
        }

        private void SUCRingInRange_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dInRangeRadius = SUCRingInRange.Value;
            int i_image = int.Parse(SetNum) - 1;

            FixationRing(halcon.Image[i_image], hWImageSet.HalconWindow, dInRangeRadius, dOutRangeRadius, hv_RowCenter, hv_ColCenter);
            Thread.Sleep(1);
        }
        private void SUCGraythreshold_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dGraythreshold = SUCGraythreshold.Value;
            int i_image = int.Parse(SetNum) - 1;
            try
            {

                Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold,dAreaLimitUp,dAreaLimitDown,dWidthLimitUp,dWidthLimitDown,dHeightLimitUp,dHeightLimitDown);
                Thread.Sleep(1);
            }
            catch
            {
                
            }

        }
        private void SUCMeasurethreshold_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dMeasurethreshold = SUCMeasurethreshold.Value;
            HObject ho_region = null;
            int i_image = int.Parse(SetNum) - 1;
            ho_region = Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold, dAreaLimitUp, dAreaLimitDown, dWidthLimitUp, dWidthLimitDown, dHeightLimitUp, dHeightLimitDown);
            HTuple ReasultBeginRows = null;
            HTuple ReasultBeginCols = null;
            HTuple ReasultEndRows = null;
            HTuple ReasultEndCols = null;
            GetLine(ho_region, hv_RowCenter, hv_ColCenter, DarkLightChoice2, dMeasurethreshold, dMeasureLength1, dMeasureLength2, DarklightChoce3, halcon.Image[i_image], hWImageSet.HalconWindow, out  ReasultBeginRows, out  ReasultBeginCols, out  ReasultEndRows, out  ReasultEndCols);
            Thread.Sleep(1);
        }
        private void SUCMeasureLength1_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dMeasureLength1 = SUCMeasureLength1.Value;
            HObject ho_region = null;
            int i_image = int.Parse(SetNum) - 1;
            ho_region = Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold, dAreaLimitUp, dAreaLimitDown, dWidthLimitUp, dWidthLimitDown, dHeightLimitUp, dHeightLimitDown);
            HTuple ReasultBeginRows = null;
            HTuple ReasultBeginCols = null;
            HTuple ReasultEndRows = null;
            HTuple ReasultEndCols = null;
            GetLine(ho_region, hv_RowCenter, hv_ColCenter, DarkLightChoice2, dMeasurethreshold, dMeasureLength1, dMeasureLength2, DarklightChoce3, halcon.Image[i_image], hWImageSet.HalconWindow, out  ReasultBeginRows, out  ReasultBeginCols, out  ReasultEndRows, out  ReasultEndCols);
            Thread.Sleep(1);
        }
        private void SUCMeasureLength2_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dMeasureLength2 = SUCMeasureLength1.Value;
            HObject ho_region = null;
            int i_image = int.Parse(SetNum) - 1;
            ho_region = Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold, dAreaLimitUp, dAreaLimitDown, dWidthLimitUp, dWidthLimitDown, dHeightLimitUp, dHeightLimitDown);
            HTuple ReasultBeginRows = null;
            HTuple ReasultBeginCols = null;
            HTuple ReasultEndRows = null;
            HTuple ReasultEndCols = null;
            GetLine(ho_region, hv_RowCenter, hv_ColCenter, DarkLightChoice2, dMeasurethreshold, dMeasureLength1, dMeasureLength2, DarklightChoce3, halcon.Image[i_image], hWImageSet.HalconWindow, out  ReasultBeginRows, out  ReasultBeginCols, out  ReasultEndRows, out  ReasultEndCols);
            Thread.Sleep(1);
        }
        private void SUCAreaLimitUp_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dAreaLimitUp = SUCAreaLimitUp.Value;
            if (dAreaLimitUp <= dAreaLimitDown)
            {
                dAreaLimitDown = dAreaLimitUp;
                SUCAreaLimitDown.Value = dAreaLimitDown;
            }
            int i_image = int.Parse(SetNum) - 1;
            try
            {

                Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold,dAreaLimitUp,dAreaLimitDown,dWidthLimitUp,dWidthLimitDown,dHeightLimitUp,dHeightLimitDown);
                Thread.Sleep(1);
            }
            catch
            {

            }


        }


        private void SUCAreaLimitDown_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dAreaLimitDown = SUCAreaLimitDown.Value;
            if (dAreaLimitUp <= dAreaLimitDown)
            {
                dAreaLimitUp = dAreaLimitDown;
                SUCAreaLimitUp.Value = dAreaLimitUp;
            }
            int i_image = int.Parse(SetNum) - 1;
            try
            {

                Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold,dAreaLimitUp,dAreaLimitDown,dWidthLimitUp,dWidthLimitDown,dHeightLimitUp,dHeightLimitDown);
                Thread.Sleep(1);
            }
            catch
            {

            }

        }

        private void SUCWidthLimitUp_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
     
            dWidthLimitUp = SUCWidthLimitUp.Value;
            if (dWidthLimitUp <= dWidthLimitDown)
            {
                dWidthLimitDown = dWidthLimitUp;
                SUCWidthLimitDown.Value = dWidthLimitDown;
            }
            int i_image = int.Parse(SetNum) - 1;
            try
            {

                Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold,dAreaLimitUp,dAreaLimitDown,dWidthLimitUp,dWidthLimitDown,dHeightLimitUp,dHeightLimitDown);
                Thread.Sleep(1);
            }
            catch
            {

            }

        }

        private void SUCWidthLimitDown_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dWidthLimitDown = SUCWidthLimitDown.Value;
            if (dWidthLimitUp <= dWidthLimitDown)
            {
                dWidthLimitUp = dWidthLimitDown;
                SUCWidthLimitUp.Value = dWidthLimitUp;
            }
               int i_image = int.Parse(SetNum) - 1;
            try
            {

                Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold,dAreaLimitUp,dAreaLimitDown,dWidthLimitUp,dWidthLimitDown,dHeightLimitUp,dHeightLimitDown);
                Thread.Sleep(1);
            }
            catch
            {

            }
        }

        private void mySliderUC5_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dHeightLimitUp = SUCHeightLimitUp.Value;

            if (dHeightLimitUp <= dHeightLimitDown)
            {
                dHeightLimitDown = dHeightLimitUp;
                SUCHeightLimitDown.Value = dHeightLimitDown;
            }
               int i_image = int.Parse(SetNum) - 1;
            try
            {

                Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold,dAreaLimitUp,dAreaLimitDown,dWidthLimitUp,dWidthLimitDown,dHeightLimitUp,dHeightLimitDown);
                Thread.Sleep(1);
            }
            catch
            {

            }


        }

        private void mySliderUC6_ValueChanged(int CurrentValue)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            dHeightLimitDown = SUCHeightLimitDown.Value;

            if (dHeightLimitUp <= dHeightLimitDown)
            {
                dHeightLimitUp = dHeightLimitDown;
                SUCHeightLimitUp.Value = dHeightLimitUp;
            }
               int i_image = int.Parse(SetNum) - 1;
               try
               {

                   Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold, dAreaLimitUp, dAreaLimitDown, dWidthLimitUp, dWidthLimitDown, dHeightLimitUp, dHeightLimitDown);
                   Thread.Sleep(1);
               }
               catch
               {
 
               }
        }
        private void rbtnFirst_CheckedChanged(object sender, EventArgs e)
        {
            if (Sign.isMode5ParaChange)
            {
                return;
            }
            DarklightChoce3 = 0;
            HObject ho_region = null;
            int i_image = int.Parse(SetNum) - 1;
            ho_region = Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold, dAreaLimitUp, dAreaLimitDown, dWidthLimitUp, dWidthLimitDown, dHeightLimitUp, dHeightLimitDown);
            HTuple ReasultBeginRows = null;
            HTuple ReasultBeginCols = null;
            HTuple ReasultEndRows = null;
            HTuple ReasultEndCols = null;
            GetLine(ho_region, hv_RowCenter, hv_ColCenter, DarkLightChoice2, dMeasurethreshold, dMeasureLength1, dMeasureLength2, DarklightChoce3, halcon.Image[i_image], hWImageSet.HalconWindow, out  ReasultBeginRows, out  ReasultBeginCols, out  ReasultEndRows, out  ReasultEndCols);

        }
        private void rbtnLast_CheckedChanged(object sender, EventArgs e)
        {

            if (Sign.isMode5ParaChange)
            {
                return;
            }
            DarklightChoce3 = 1;
            HObject ho_region = null;
            int i_image = int.Parse(SetNum) - 1;
            ho_region = Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold, dAreaLimitUp, dAreaLimitDown, dWidthLimitUp, dWidthLimitDown, dHeightLimitUp, dHeightLimitDown);
            HTuple ReasultBeginRows = null;
            HTuple ReasultBeginCols = null;
            HTuple ReasultEndRows = null;
            HTuple ReasultEndCols = null;
            GetLine(ho_region, hv_RowCenter, hv_ColCenter, DarkLightChoice2, dMeasurethreshold, dMeasureLength1, dMeasureLength2, DarklightChoce3, halcon.Image[i_image], hWImageSet.HalconWindow, out  ReasultBeginRows, out  ReasultBeginCols, out  ReasultEndRows, out  ReasultEndCols);

        }
        private void cbDarkLightChoice_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbDarkLightChoice.SelectedIndex == 0)
                DarkLightChoice1 = 0;
            else

                DarkLightChoice1 = 1;

        }

        private void cbDarkLightChoice2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDarkLightChoice2.SelectedIndex == 0)
                DarkLightChoice2 = 0;
            else

                DarkLightChoice2 = 1;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string CCDname = "";
                CCDname = GetCCDName();
                if (CCDname == null)
                {
                    return;
                }
                #region   参数记录在INI文件上
                string SysPath = Sys.IniPath + "\\Mode5.ini";
                IniFile.Write(CCDname, "OutRangeRadius", dOutRangeRadius.ToString(), SysPath);
                IniFile.Write(CCDname, "InRangeRadius", dInRangeRadius.ToString(), SysPath);
                IniFile.Write(CCDname, "Graythreshold", dGraythreshold.ToString(), SysPath);
                IniFile.Write(CCDname, "MeasureThreshold", dMeasurethreshold.ToString(), SysPath);
                IniFile.Write(CCDname, "DarkLightGray", DarkLightChoice1.ToString(), SysPath);
                IniFile.Write(CCDname, "DarkLightMeasure", DarkLightChoice2.ToString(), SysPath);
                IniFile.Write(CCDname, "Length1Measure", dMeasureLength1.ToString(), SysPath);
                IniFile.Write(CCDname, "Length2Measure", dMeasureLength2.ToString(), SysPath);
                IniFile.Write(CCDname, "FirstorLast", DarklightChoce3.ToString(), SysPath);
                IniFile.Write(CCDname, "LimitUp", txtLimitUp.Text.Trim(), SysPath);
                IniFile.Write(CCDname, "LimitDown", txtLimitDown.Text.Trim(), SysPath);
                iniFile.Write(CCDname, "GlueCIMode", cBGOutMode.Value.ToString(), FrmMain.propath);
                IniFile.Write(CCDname, "Offset", txtoffset.Text.Trim(), SysPath);
                IniFile.Write(CCDname, "LimitUpGap",txtlimitUpGap.Text.Trim(), SysPath);
                IniFile.Write(CCDname, "LimitDownGap", txtlimitDownGap.Text.Trim(), SysPath);
                IniFile.Write(CCDname, "Gapoffset", txtGapOffSet.Text.Trim(), SysPath);
                IniFile.Write(CCDname, "AreaLimitUP", dAreaLimitUp.ToString(), SysPath);
                IniFile.Write(CCDname, "AreaLimitDown", dAreaLimitDown.ToString(), SysPath);
                IniFile.Write(CCDname, "WidthLimitUp", dWidthLimitUp.ToString(), SysPath);
                IniFile.Write(CCDname, "WidthLimitDown", dWidthLimitDown.ToString(), SysPath);
                IniFile.Write(CCDname, "HeightLimitUp", dHeightLimitUp.ToString(), SysPath);
                IniFile.Write(CCDname, "HeightLimitDown", dHeightLimitDown.ToString(), SysPath);
                #endregion
                MessageBox.Show("保存成功");

            }
            catch
            {


            }
        }
        private void txtLimitUp_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dLimitUp = Convert.ToDouble(txtLimitUp.Text.Trim());
            }
            catch
            {
 
            }
        }

        private void txtLimitDown_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dLimitDown = Convert.ToDouble(txtLimitDown.Text.Trim());
            }
            catch
            {

            }

        }

        private void txtlimitUpGap_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dlimitUpGap = Convert.ToDouble(txtlimitUpGap.Text.Trim());
            }
            catch
            {

            }
        }

        private void txtlimitDownGap_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dlimitDownGap = Convert.ToDouble(txtlimitDownGap.Text.Trim());
            }
            catch
            {

            }

        }

        private void txtoffset_TextChanged(object sender, EventArgs e)
        {
            try
            {
                doffset = Convert.ToDouble(txtoffset.Text.Trim());

            }
            catch
            {

            }

        }

        private void txtGapOffSet_TextChanged(object sender, EventArgs e)
        {

            try
            {
                dGapoffset = Convert.ToDouble(txtGapOffSet.Text.Trim());

            }
            catch
            {

            }
        }

        private void txtGapOffSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }  
        private void txtoffset_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        private void txtLimitUp_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }

        private void txtLimitDown_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
     
        private void txtlimitUpGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }

        private void txtlimitDownGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextSPEC(sender, e);
        }
        #endregion
        private void btnMode5Test_Click(object sender, EventArgs e)
        {
            try
            {
                HObject ho_region = null;
                int i_image = int.Parse(SetNum) - 1;
                switch (int.Parse(SetNum))
                {
                    case 1: xpm = A1CCD1.xpm; ypm = A1CCD1.ypm; break;
                    case 2: xpm = A1CCD2.xpm; ypm = A1CCD2.ypm; break;
                    case 3: xpm = A2CCD1.xpm; ypm = A2CCD1.ypm; break;
                    case 4: xpm = A2CCD2.xpm; ypm = A2CCD2.ypm; break;
                    case 5: xpm = PCCD1.xpm; ypm = PCCD1.ypm; break;
                    case 6: xpm = PCCD2.xpm; ypm = PCCD2.ypm; break;
                    case 7: xpm = GCCD1.xpm; ypm = GCCD1.ypm; break;
                    case 8: xpm = GCCD2.xpm; ypm = GCCD2.ypm; break;
                    case 9: xpm = QCCD.xpm; ypm = QCCD.ypm; break;
                };
                HTuple Width = null;
                HTuple Height = null;

                HOperatorSet.GetImageSize(halcon.Image[i_image], out Width, out  Height);
                HOperatorSet.SetPart(hWImageSet.HalconWindow, 0, 0, Height, Width);
                ho_region = Difference(halcon.Image[i_image], hWImageSet.HalconWindow, dOutRangeRadius, dInRangeRadius, hv_RowCenter, hv_ColCenter, DarkLightChoice1, dGraythreshold, dAreaLimitUp, dAreaLimitDown, dWidthLimitUp, dWidthLimitDown, dHeightLimitUp, dHeightLimitDown);
                HTuple ReasultBeginRows = null;
                HTuple ReasultBeginCols = null;
                HTuple ReasultEndRows = null;
                HTuple ReasultEndCols = null;
                GetLine(ho_region, hv_RowCenter, hv_ColCenter, DarkLightChoice2, dMeasurethreshold, dMeasureLength1, dMeasureLength2, DarklightChoce3, halcon.Image[i_image], hWImageSet.HalconWindow, out  ReasultBeginRows, out  ReasultBeginCols, out  ReasultEndRows, out  ReasultEndCols);
                bool isImageProcess=false;
                HTuple distance=null;
                HTuple distance1= null;
                HTuple distance2 = null;
                isImageProcess = ImageProcess(ypm, doffset, dGapoffset,halcon.Image[i_image], hWImageSet.HalconWindow, hv_RowCenter, hv_ColCenter, ReasultBeginRows, ReasultBeginCols, ReasultEndRows, ReasultEndCols, out distance, out distance1, out distance2);
                if (isImageProcess)
                {
                    ResultImProve(dLimitUp, dLimitDown,dlimitUpGap,dlimitDownGap, distance, distance1, distance2, hWImageSet.HalconWindow);
                }
            }
            catch
            {

            }
        }

        public string GetCCDName()
        {
            string CCDNAME = ""; string cn = ""; string area1 = "", area2 = "", area3 = "", area4 = "";
            if (SetNum == "0" || (cBLocation.Enabled && cBLocation.Text == "") ||
                                 (cBLocation2.Enabled && cBLocation2.Text == ""))
                return null;
            if (cBLocation.SelectedIndex == 0)
                area1 = "PickUp";
            if (cBLocation.SelectedIndex == 1)
                area1 = "Platform";
            area2 = (cBLocation2.SelectedIndex + 1).ToString();
            if (SetNum == "6")
            {
                if (cBLocation3.SelectedIndex == 0)
                    area1 = "PickUp";
                if (cBLocation3.SelectedIndex == 1)
                    area1 = "Platform1";
                if (cBLocation3.SelectedIndex == 2)
                    area1 = "Platform2";
            }
            switch (int.Parse(SetNum))
            {
                case 1: CCDNAME = "A1CCD1"; break;
                case 2: CCDNAME = "A1CCD2-" + area1; cn = "A1CCD2"; break;
                case 3: CCDNAME = "A2CCD1"; break;
                case 4: CCDNAME = "A2CCD2-" + area1; cn = "A2CCD2"; break;
                case 5: CCDNAME = "PCCD1"; break;
                case 6: CCDNAME = "PCCD2-" + area1; cn = "PCCD2"; break;
                case 7: CCDNAME = "GCCD1"; break;
                case 8: CCDNAME = "GCCD2-" + area2; cn = "GCCD2"; break;
                case 9: CCDNAME = "QCCD"; break;
            }
            if ((SetNum == "1" || SetNum == "3"))
            {
                if (cBoxTest.Checked)
                {
                    CCDNAME = CCDNAME + "-" + area3;

                }
                else
                {
                    CCDNAME = CCDNAME + "-" + area4;

                }
            }
            if (SetNum == "2" || SetNum == "4")
            {
                if (cBoxTest.Checked)
                {
                    CCDNAME = cn + "-" + area3;

                }
            }
            if (SetNum == "5")
            {
                if (cBoxTest.Checked)
                {
                    CCDNAME = CCDNAME + "-" + area3;
                }
            }
            return CCDNAME;
        }
        public void Mode5Parm(string CCDName)
        {
            try
            {
                string SysPath = Sys.IniPath + "\\Mode5.ini";
                SUCRingOutRange.Value = Convert.ToInt32(IniFile.Read(CCDName, "OutRangeRadius", "500", SysPath));
                SUCRingInRange.Value = Convert.ToInt32(IniFile.Read(CCDName, "InRangeRadius", "100", SysPath));
                SUCGraythreshold.Value = Convert.ToInt32(IniFile.Read(CCDName, "Graythreshold", "5", SysPath));
                SUCMeasurethreshold.Value = Convert.ToInt32(IniFile.Read(CCDName, "MeasureThreshold", "5", SysPath));
                cbDarkLightChoice.SelectedIndex = Convert.ToInt32(IniFile.Read(CCDName, "DarkLightGray", "0", SysPath));
                cbDarkLightChoice2.SelectedIndex = Convert.ToInt32(IniFile.Read(CCDName, "DarkLightMeasure", "0", SysPath));
                SUCMeasureLength1.Value = Convert.ToInt32(IniFile.Read(CCDName, "Length1Measure", "5", SysPath));
                SUCMeasureLength2.Value = Convert.ToInt32(IniFile.Read(CCDName, "Length2Measure", "5", SysPath));
                DarklightChoce3 = Convert.ToInt32(IniFile.Read(CCDName, "FirstorLast", "0", SysPath));
                dLimitUp = Convert.ToDouble(IniFile.Read(CCDName, "LimitUp", "3.82", SysPath));
                dLimitDown = Convert.ToDouble(IniFile.Read(CCDName, "LimitDown", "3.79", SysPath));
                doffset =  Convert.ToDouble(IniFile.Read(CCDName, "Offset", "0.000", SysPath));
                dlimitUpGap=  Convert.ToDouble(IniFile.Read(CCDName, "LimitUpGap", "7.66", SysPath));
                dlimitDownGap =Convert.ToDouble(IniFile.Read(CCDName, "LimitDownGap","7.55", SysPath));
                dGapoffset = Convert.ToDouble(IniFile.Read(CCDName, "Gapoffset", "0.000", SysPath));
                SUCAreaLimitUp.Value = Convert.ToInt32(IniFile.Read(CCDName, "AreaLimitUP", "10000", SysPath));
                SUCAreaLimitDown.Value = Convert.ToInt32(IniFile.Read(CCDName, "AreaLimitDown", "1", SysPath));
                SUCWidthLimitUp.Value = Convert.ToInt32(IniFile.Read(CCDName, "WidthLimitUp", "10000", SysPath));
                SUCWidthLimitDown.Value = Convert.ToInt32(IniFile.Read(CCDName, "WidthLimitDown", "1", SysPath));
                SUCHeightLimitUp.Value = Convert.ToInt32(IniFile.Read(CCDName, "HeightLimitUp", "10000", SysPath));
                SUCHeightLimitDown.Value = Convert.ToInt32(IniFile.Read(CCDName, "HeightLimitDown", "1", SysPath));
                dOutRangeRadius = SUCRingOutRange.Value;
                dInRangeRadius = SUCRingInRange.Value;
                dGraythreshold = SUCGraythreshold.Value;
                dMeasurethreshold = SUCMeasurethreshold.Value;
                dMeasureLength1 = SUCMeasureLength1.Value;
                dMeasureLength2 = SUCMeasureLength2.Value;
                DarkLightChoice1 = cbDarkLightChoice.SelectedIndex;
                DarkLightChoice2 = cbDarkLightChoice2.SelectedIndex;
                txtLimitUp.Text = dLimitUp.ToString();
                txtLimitDown.Text = dLimitDown.ToString();
                txtoffset.Text = doffset.ToString();
                txtlimitUpGap.Text = dlimitUpGap.ToString();
                txtlimitDownGap.Text = dlimitDownGap.ToString();
                txtGapOffSet.Text = dGapoffset.ToString();
                dAreaLimitUp=SUCAreaLimitUp.Value;
                dAreaLimitDown=SUCAreaLimitDown.Value;
                dWidthLimitUp=SUCWidthLimitUp.Value;
                dWidthLimitDown=SUCWidthLimitDown.Value;
                dHeightLimitUp=SUCHeightLimitUp.Value;
                dHeightLimitDown=SUCHeightLimitDown.Value;
                if (DarklightChoce3 == 0)
                {
                    rbtnFirst.Checked = true;
                }
                else
                {
                    rbtnLast.Checked = true;

                }

            }
            catch
            {


            }

        }
        #region
        public HObject Difference(HObject ho_Image, HWindow window, HTuple OutRangeRadius, HTuple InRangeRadius, HTuple Row, HTuple Col, int DarkLightChoice, HTuple GrayThreshold, HTuple AreaLimitUp, HTuple AreaLimitDown, HTuple WidthLimitUp, HTuple WidthLimitDown, HTuple HeightLimitUp, HTuple HeightLimitDown)
        {

            try
            {
                HObject ho_Circle1 = null;
                HObject ho_Circle2 = null;
                HObject ho_Image1 = null;
                HObject ho_ImageReduced2 = null;
                HObject ho_ReducedImage = null;
                HObject ho_ImageMedian = null;
                HObject ho_Region = null;
                HObject ho_Regions1 = null;
                HObject ho_RegionOpening = null;
                HObject ho_ConnectedRegions1 = null;
                HObject ho_SelectedRegions2 = null;
                HWindow hv_ExpDefaultWinHandle = window;
                HOperatorSet.GenEmptyObj(out ho_Circle1);
                HOperatorSet.GenEmptyObj(out ho_Circle2);
                HOperatorSet.GenEmptyObj(out ho_Image1);
                HOperatorSet.GenEmptyObj(out ho_ImageMedian);
                HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
                HOperatorSet.GenEmptyObj(out ho_ReducedImage);
                HOperatorSet.GenEmptyObj(out ho_Region);
                HOperatorSet.GenEmptyObj(out ho_Regions1);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);

                hv_ExpDefaultWinHandle.ClearWindow();
                //畫檢測區域
                if (ho_Image == null)
                    return null;
                ho_Image1.Dispose();
                HOperatorSet.CopyImage(ho_Image, out ho_Image1);

                ho_ImageMedian.Dispose();
                HOperatorSet.MedianImage(ho_Image1, out ho_ImageMedian, "circle",
          20, "mirrored");
                ho_Circle1.Dispose();
                HOperatorSet.GenCircle(out ho_Circle1, Row, Col, OutRangeRadius);
                ho_Circle2.Dispose();
                HOperatorSet.GenCircle(out ho_Circle2, Row, Col, InRangeRadius);
                HOperatorSet.Difference(ho_Circle1, ho_Circle2, out ho_RegionDifference);
                ho_ImageReduced2.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageMedian, ho_RegionDifference, out ho_ImageReduced2);
                ho_Region.Dispose();
                if (DarkLightChoice == 0)
                    HOperatorSet.Threshold(ho_ImageReduced2, out ho_Region, GrayThreshold, 255);
                else
                    HOperatorSet.Threshold(ho_ImageReduced2, out ho_Region, 0, GrayThreshold);
                HOperatorSet.DispObj(ho_Region, hv_ExpDefaultWinHandle);
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 3.5);
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningRectangle1(ho_RegionClosing, out ho_RegionOpening, 10,
          10);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions1);
                ho_SelectedRegions2.Dispose();
                
                HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions2, ((new HTuple("area")).TupleConcat(
       "rect2_len1")).TupleConcat("rect2_len2"), "and", ((new HTuple(AreaLimitDown)).TupleConcat(
       WidthLimitDown)).TupleConcat(HeightLimitDown), ((new HTuple(AreaLimitUp)).TupleConcat(WidthLimitUp)).TupleConcat(
       HeightLimitUp));
                HOperatorSet.DispObj(ho_Image1, hv_ExpDefaultWinHandle);
                HOperatorSet.SetDraw(hv_ExpDefaultWinHandle, "margin");
                HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "yellow");
                HOperatorSet.DispObj(ho_Circle1, hv_ExpDefaultWinHandle);
                HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "orange");
                HOperatorSet.DispObj(ho_Circle2, hv_ExpDefaultWinHandle);
                HOperatorSet.SetDraw(hv_ExpDefaultWinHandle, "fill");
                HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "red");
                HOperatorSet.DispObj(ho_SelectedRegions2, hv_ExpDefaultWinHandle);
                HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions2, out hv_Row1, out hv_Column1,
                 out hv_Phi, out hv_Length1, out hv_Length2);
                return ho_SelectedRegions2;
            }
            catch
            {
                return null;
            }

        }

        public void GetLine(HObject region, HTuple hv_ResultRow, HTuple hv_ResultColumn, int MeasureTransition, HTuple MeasureThreshold, HTuple MeasureLength1, HTuple MeasureLength2, int MeasureSelect, HObject ho_Image, HWindow window, out HTuple ReasultBeginRows, out HTuple ReasultBeginCols, out HTuple ReasultEndRows, out HTuple ReasultEndCols)
        {
            HTuple hv_Phi = null;
            HTuple hv_Length2 = null;
            HTuple hv_Cos = null;
            HTuple hv_Sin = null;
            HTuple hv_ColLU = null;
            HTuple hv_RowLU = null;
            HTuple hv_ColRU = null;
            HTuple hv_RowRU = null;
            HTuple hv_ColRD = null;
            HTuple hv_ColLD = null;
            HTuple hv_RowLD = null;
            HTuple hv_RowRD = null;
            HTuple hv_InitialRows = null;
            HTuple hv_InitialColumns = null;
            HTuple hv_EndRows = null;
            HTuple hv_EndColumns = null;
            HTuple hv_Number = null;
            HTuple hv_Index = null;
            HTuple hv_InitialRow = null;
            HTuple hv_InitialColumn = null;
            HTuple hv_EndRow = null;
            HTuple hv_EndColumn = null;
            HTuple Transition = null;
            HTuple hv_RowBegin1 = null;
            HTuple hv_ColBegin1 = null;
            HTuple hv_RowEnd1 = null;
            HTuple hv_ColEnd1 = null;
            HTuple hv_Nr1 = null;
            HTuple hv_Nc1 = null;
            HTuple hv_Dist = null;
            HTuple hv_Phi3 = null;
            HTuple ho_Select = null;
            HTuple hv_Row2 = null;
            HTuple hv_Column2 = null;
            HTuple hv_MetrologyHandle1 = null;
            HTuple hv_Row1 = null;
            HObject ho_Rectangle3 = null;
            HObject ho_Rectangle = null;
            HObject ho_ResultContours1 = null;
            HObject ho_RegionLines = null;
            HObject ho_Contour3 = null;
            HObject ho_UsedEdges1 = null;
            HObject ho_ModelContour1 = null;

            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ResultContours1);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Contour3);
            HOperatorSet.GenEmptyObj(out ho_UsedEdges1);
            HWindow hv_ExpDefaultWinHandle = window;
            try
            {
                hv_ExpDefaultWinHandle.ClearWindow();
                HOperatorSet.DispObj(ho_Image, hv_ExpDefaultWinHandle);
                HOperatorSet.SmallestRectangle2(region, out hv_Row1, out hv_Column1,
                    out hv_Phi, out hv_Length1, out hv_Length2);
                ho_Rectangle3.Dispose();
                HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle3, hv_Row1, hv_Column1,
                    hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.TupleCos(hv_Phi, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi, out hv_Sin);
                //矩形第一个端点计算（左上）
                hv_ColLU = (hv_Column1 - (hv_Length1 * hv_Cos)) - (hv_Length2 * hv_Sin);
                hv_RowLU = hv_Row1 - (((-hv_Length1) * hv_Sin) + (hv_Length2 * hv_Cos));
                //矩形第二个端点计算（右上）
                hv_ColRU = (hv_Column1 + (hv_Length1 * hv_Cos)) - (hv_Length2 * hv_Sin);
                hv_RowRU = hv_Row1 - ((hv_Length1 * hv_Sin) + (hv_Length2 * hv_Cos));
                //矩形第三个端点计算（右下）
                hv_ColRD = (hv_Column1 + (hv_Length1 * hv_Cos)) + (hv_Length2 * hv_Sin);
                hv_RowRD = hv_Row1 - ((hv_Length1 * hv_Sin) - (hv_Length2 * hv_Cos));
                //矩形的第四个端点计算（左下）
                hv_ColLD = (hv_Column1 - (hv_Length1 * hv_Cos)) + (hv_Length2 * hv_Sin);
                hv_RowLD = hv_Row1 - (((-hv_Length1) * hv_Sin) - (hv_Length2 * hv_Cos));
                //存储拟合起点终点
                hv_InitialRows = new HTuple();
                hv_InitialColumns = new HTuple();
                hv_EndRows = new HTuple();
                hv_EndColumns = new HTuple();
                HOperatorSet.CountObj(ho_Rectangle3, out hv_Number);
                HTuple end_val50 = hv_Number - 1;
                HTuple step_val50 = 1;
                if (MeasureSelect == 0)
                {
                    ho_Select = "first";

                }
                else
                {
                    ho_Select = "last";

                }
                if (MeasureTransition == 0)
                    Transition = "positive";
                else
                    Transition = "negative";
                for (hv_Index = 0; hv_Index.Continue(end_val50, step_val50); hv_Index = hv_Index.TupleAdd(step_val50))
                {
                    if ((int)(new HTuple(((hv_Row1.TupleSelect(hv_Index))).TupleLess(hv_ResultRow))) != 0)
                    {
                        hv_InitialRow = hv_RowLD.TupleSelect(hv_Index);
                        hv_InitialColumn = hv_ColLD.TupleSelect(hv_Index);
                        hv_EndRow = hv_RowRD.TupleSelect(hv_Index);
                        hv_EndColumn = hv_ColRD.TupleSelect(hv_Index);

                    }
                    else if ((int)(new HTuple(((hv_Row1.TupleSelect(hv_Index))).TupleGreater(
                        hv_ResultRow))) != 0)
                    {
                        hv_InitialRow = hv_RowRU.TupleSelect(hv_Index);
                        hv_InitialColumn = hv_ColRU.TupleSelect(hv_Index);
                        hv_EndRow = hv_RowLU.TupleSelect(hv_Index);
                        hv_EndColumn = hv_ColLU.TupleSelect(hv_Index);
                    }
                    else if ((int)((new HTuple(((hv_Row1.TupleSelect(hv_Index))).TupleEqual(
                        hv_ResultRow))).TupleAnd(new HTuple(((hv_Column1.TupleSelect(hv_Index))).TupleLess(
                        hv_ResultColumn)))) != 0)
                    {
                        hv_InitialRow = hv_RowRD.TupleSelect(hv_Index);

                        hv_InitialColumn = hv_ColRD.TupleSelect(hv_Index);
                        hv_EndRow = hv_RowRU.TupleSelect(hv_Index);
                        hv_EndColumn = hv_ColRU.TupleSelect(hv_Index);
                    }
                    else
                    {

                        hv_InitialRow = hv_RowLU.TupleSelect(hv_Index);
                        hv_InitialColumn = hv_ColLU.TupleSelect(hv_Index);
                        hv_EndRow = hv_RowLD.TupleSelect(hv_Index);
                        hv_EndColumn = hv_ColLD.TupleSelect(hv_Index);
                    }


                    HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle1);
                    HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle1, "line", ((((hv_InitialRow.TupleConcat(
                        hv_InitialColumn))).TupleConcat(hv_EndRow))).TupleConcat(hv_EndColumn),
                        MeasureLength1, MeasureLength2, 1, MeasureThreshold, new HTuple(), new HTuple(), out hv_circleIndices);
                    ho_ModelContour.Dispose();
                    HOperatorSet.GetMetrologyObjectModelContour(out ho_ModelContour1, hv_MetrologyHandle1,
                        "all", 1.5);
                    //第一個點或最後一個點
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle1, hv_circleIndices,
                        "measure_select", ho_Select);
                    HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle1, hv_circleIndices,
                        "min_score", 0.2);
                    HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle1);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle1, hv_circleIndices,
                        "all", "result_type", "all_param", out hv_circleParameter);
                    //白找黑('negative')或黑找白('positive')
                    ho_Contour3.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour3, hv_MetrologyHandle1,
                        "all", Transition, out hv_Row2, out hv_Column2);
                    ho_Contours.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_Contours, hv_MetrologyHandle1,
                        "all", "all", 1.5);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle1, "all", "all", "used_edges",
                        "row", out hv_UsedRow);
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle1, "all", "all", "used_edges",
                        "column", out hv_UsedColumn);
                    ho_UsedEdges1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_UsedEdges1, hv_UsedRow, hv_UsedColumn,
                        10, (new HTuple(45)).TupleRad());
                    ho_ResultContours1.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_ResultContours1, hv_MetrologyHandle1,
                        "all", "all", 1.5);

                    HOperatorSet.FitLineContourXld(ho_ResultContours1, "tukey", -1, 0, 5, 2, out hv_RowBegin1,
                        out hv_ColBegin1, out hv_RowEnd1, out hv_ColEnd1, out hv_Nr1, out hv_Nc1,
                        out hv_Dist);
                    if (hv_InitialRows == null)
                        hv_InitialRows = new HTuple();
                    hv_InitialRows[hv_Index] = hv_RowBegin1;
                    if (hv_InitialColumns == null)
                        hv_InitialColumns = new HTuple();
                    hv_InitialColumns[hv_Index] = hv_ColBegin1;
                    if (hv_EndRows == null)
                        hv_EndRows = new HTuple();
                    hv_EndRows[hv_Index] = hv_RowEnd1;
                    if (hv_EndColumns == null)
                        hv_EndColumns = new HTuple();
                    hv_EndColumns[hv_Index] = hv_ColEnd1;
                    HOperatorSet.SetDraw(window, "margin");
                    HOperatorSet.SetColor(window, "red");
                    HOperatorSet.DispObj(ho_UsedEdges1, hv_ExpDefaultWinHandle);
                    HOperatorSet.SetDraw(window, "margin");
                    HOperatorSet.SetColor(window, "green");
                    HOperatorSet.DispObj(ho_Contour3, hv_ExpDefaultWinHandle);
                    HOperatorSet.SetDraw(window, "margin");
                    HOperatorSet.SetColor(window, "yellow");
                    HOperatorSet.DispObj(ho_ResultContours1, hv_ExpDefaultWinHandle);
                    HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle1);
                }
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_InitialRows, hv_InitialColumns,
                    hv_EndRows, hv_EndColumns);
                ReasultBeginRows = hv_InitialRows;
                ReasultBeginCols = hv_InitialColumns;
                ReasultEndRows = hv_EndRows;
                ReasultEndCols = hv_EndColumns;


            }
            catch
            {
                ReasultBeginRows = null;
                ReasultBeginCols = null;
                ReasultEndRows = null;
                ReasultEndCols = null;
            }
        }
        public void FixationRing(HObject ho_Image, HWindow Window, HTuple CircleRadius1, HTuple CircleRadius2, HTuple RowCenter, HTuple ColCenter)
        {


            HObject ho_Circle1 = null;
            HObject ho_Circle2 = null;
            HObject ho_Image1 = null;
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_Circle2);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HTuple hv_CircleRadius1 = null;
            HTuple hv_CircleRadius2 = null;
            HTuple hv_ResultRow = null;
            HTuple hv_ResultColumn = null;

            if (ho_Image == null)
                return;
            ho_Image1.Dispose();
            HOperatorSet.CopyImage(ho_Image, out ho_Image1);
            HWindow hv_ExpDefaultWinHandle;
            hv_ExpDefaultWinHandle = Window;
            //畫檢視範圍
            hv_ExpDefaultWinHandle.ClearWindow();
            HOperatorSet.DispObj(ho_Image1, hv_ExpDefaultWinHandle);
            //畫出固定環區域
            hv_CircleRadius1 = CircleRadius1;
            hv_CircleRadius2 = CircleRadius2;
            hv_ResultRow = RowCenter;
            hv_ResultColumn = ColCenter;
            ho_Circle1.Dispose();
            HOperatorSet.GenCircle(out ho_Circle1, hv_ResultRow, hv_ResultColumn, hv_CircleRadius1);
            ho_Circle2.Dispose();
            HOperatorSet.GenCircle(out ho_Circle2, hv_ResultRow, hv_ResultColumn, hv_CircleRadius2);
            HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "yellow");
            HOperatorSet.SetDraw(hv_ExpDefaultWinHandle, "margin");
            HOperatorSet.DispObj(ho_Circle1, hv_ExpDefaultWinHandle);
            HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "orange");
            HOperatorSet.DispObj(ho_Circle2, hv_ExpDefaultWinHandle);
        }
        public bool ImageProcess(double ypm,double offset,double dGapoffset,HObject ho_Image, HWindow window, HTuple RowCenter, HTuple ColCenter, HTuple ReasultBeginRows, HTuple ReasultBeginCols, HTuple ReasultEndRows, HTuple ReasultEndCols, out HTuple Distance, out HTuple Distance1, out HTuple Distance2)
        {
            HObject ho_ImageMedian;
            HObject ho_Circle, ho_ImageReduced, ho_Regions, ho_RegionClosing1;
            HObject ho_RegionFillUp, ho_ConnectedRegions, ho_SelectedRegions;
            HObject ho_UsedEdges, ho_Contour, ho_ResultContours, ho_CrossCenter;
            HObject ho_Cross, ho_Circle1, ho_Circle2, ho_RegionDifference;
            HObject ho_ImageReduced2, ho_Regions1, ho_RegionClosing;
            HObject ho_RegionOpening, ho_ConnectedRegions1, ho_SelectedRegions2;
            HObject ho_Rectangle, ho_ModelContour = null, ho_Contours = null;
            HObject ho_RegionLines, ho_Cross1 = null, ho_RegionLines1 = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null;
            HTuple hv_Area1 = null, hv_Row2 = null, hv_Column2 = null;
            HTuple hv_Value = null, hv_ResultRow = null, hv_ResultColumn = null;
            HTuple hv_ResultRadius = null, hv_Row1 = null, hv_Column1 = null;
            HTuple hv_Phi = null, hv_Length1 = null, hv_Length2 = null;
            HTuple hv_Cos = null, hv_Sin = null, hv_ColLU = null, hv_RowLU = null;
            HTuple hv_ColRU = null, hv_RowRU = null, hv_ColRD = null;
            HTuple hv_RowRD = null, hv_ColLD = null, hv_RowLD = null;
            HTuple hv_InitialRows = null, hv_InitialColumns = null;
            HTuple hv_EndRows = null, hv_EndColumns = null, hv_Number = null;
            HTuple hv_Index = null, hv_InitialRow = new HTuple(), hv_InitialColumn = new HTuple();
            HTuple hv_EndRow = new HTuple(), hv_EndColumn = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_circleIndices = new HTuple();
            HTuple hv_circleParameter = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_UsedRow = new HTuple();
            HTuple hv_UsedColumn = new HTuple(), hv_RowBegin = new HTuple();
            HTuple hv_ColBegin = new HTuple(), hv_RowEnd = new HTuple();
            HTuple hv_ColEnd = new HTuple(), hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Phi3 = null;
            HTuple hv_RowProjs = null, hv_ColProj = null, hv_Phis = null;
            HTuple hv_Index1 = null, hv_RowProj = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_realDistance = new HTuple();
            HTuple hv_ColProjs = new HTuple(), hv_realRowProj = null;
            HTuple hv_realColProj = null, hv_A1 = null, hv_Index3 = new HTuple();
            HTuple hv_Phi2 = new HTuple(), hv_Distances1 = null, hv_Distances2 = null;
            HTuple hv_Distance1 = null, hv_A2 = null, hv_Index2 = null;
            HTuple hv_Distance2 = null, hv_A3 = null, hv_Index4 = null;
            HTuple hv_Index5 = new HTuple(), hv_Distance3 = null, hv_resultDistance = null;
            HTuple hv_Angle2 = null;
            HTuple hv_Angles = null;
            HTuple hv_Angle = null;
            HTuple hv_Deg = null;
            HTuple width = null;
            HTuple height = null;
            HTuple hv_Angle1 = null;
            HTuple hv_Deg1 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageMedian);
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_UsedEdges);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ResultContours);
            HOperatorSet.GenEmptyObj(out ho_CrossCenter);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_Circle2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_Regions1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ModelContour);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_RegionLines1);
            HWindow hv_WindowHandle;
            try
            {
                hv_WindowHandle = window;
                hv_InitialRows = ReasultBeginRows;
                hv_InitialColumns = ReasultBeginCols;
                hv_EndRows = ReasultEndRows;
                hv_EndColumns = ReasultEndCols;
                hv_Number = hv_InitialRows.TupleLength();
                if (hv_Number != 5)
                {
                    set_display_font(hv_WindowHandle, 10, "mono", "true", "false");
                    disp_message(hv_WindowHandle, "Miss", "image", 10,
                        10, "red", "false");
                    Distance = 0;
                    Distance1 = 0;
                    Distance2 = 0;
                    return false;

                }
                hv_ResultRow = RowCenter;
                hv_ResultColumn = ColCenter;
                hv_WindowHandle.ClearWindow();
                HOperatorSet.GetImageSize(ho_Image, out width, out height);
                HOperatorSet.SetPart(hv_WindowHandle, 0, 0, height - 1, width - 1);
                HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_InitialRows, hv_InitialColumns,
                    hv_EndRows, hv_EndColumns);
                HOperatorSet.SetColor(hv_WindowHandle, "green");
                HOperatorSet.DispObj(ho_RegionLines, hv_WindowHandle);
                hv_RowProjs = new HTuple();
                hv_ColProj = new HTuple();
                hv_Phis = new HTuple();

                HTuple end_val107 = hv_Number - 1;
                HTuple step_val107 = 1;
                for (hv_Index1 = 0; hv_Index1.Continue(end_val107, step_val107); hv_Index1 = hv_Index1.TupleAdd(step_val107))
                {
                    //计算出圆心与线段的垂足
                    HOperatorSet.ProjectionPl(hv_ResultRow, hv_ResultColumn, hv_InitialRows.TupleSelect(
                        hv_Index1), hv_InitialColumns.TupleSelect(hv_Index1), hv_EndRows.TupleSelect(
                        hv_Index1), hv_EndColumns.TupleSelect(hv_Index1), out hv_RowProj, out hv_ColProj);
                    //显示垂足点
                    ho_Cross1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowProj, hv_ColProj, 20,
                        0);
                    HOperatorSet.SetColor(hv_WindowHandle, "red");
                    HOperatorSet.DispObj(ho_Cross1, hv_WindowHandle);

                    //显示垂线
                    ho_RegionLines1.Dispose();
                    HOperatorSet.GenRegionLine(out ho_RegionLines1, hv_ResultRow, hv_ResultColumn,
                        hv_RowProj, hv_ColProj);
                    HOperatorSet.AngleLx(hv_ResultRow, hv_ResultColumn, hv_RowProj, hv_ColProj,
                        out hv_Angle);
                    HOperatorSet.SetColor(hv_WindowHandle, "green");
                    HOperatorSet.DispObj(ho_RegionLines1, hv_WindowHandle);
                    HOperatorSet.OrientationRegion(ho_RegionLines1, out hv_Phi1);
                    //圆心到线段的距离显示
                    HOperatorSet.DistancePl(hv_ResultRow, hv_ResultColumn, hv_InitialRows.TupleSelect(
                        hv_Index1), hv_InitialColumns.TupleSelect(hv_Index1), hv_EndRows.TupleSelect(
                        hv_Index1), hv_EndColumns.TupleSelect(hv_Index1), out hv_Distance);           
                    if (hv_RowProjs == null)
                        hv_RowProjs = new HTuple();
                    hv_RowProjs[hv_Index1] = hv_RowProj;
                    if (hv_ColProjs == null)
                        hv_ColProjs = new HTuple();
                    hv_ColProjs[hv_Index1] = hv_ColProj;
                    if (hv_Phis == null)
                        hv_Phis = new HTuple();
                    hv_Phis[hv_Index1] = hv_Phi1;
                    if (hv_Angles == null)
                        hv_Angles = new HTuple();
                    hv_Angles[hv_Index1] = hv_Angle;
                }
                hv_realRowProj = 0;
                hv_realColProj = 0;
                hv_A1 = new HTuple();
                hv_A2 = new HTuple();
                for (hv_Index2 = 0; (int)hv_Index2 <= 4; hv_Index2 = (int)hv_Index2 + 1)
                {
                    HTuple end_val136 = 4;
                    HTuple step_val136 = 1;
                    for (hv_Index3 = hv_Index2 + 1; hv_Index3.Continue(end_val136, step_val136); hv_Index3 = hv_Index3.TupleAdd(step_val136))
                    {
                        hv_Angle2 = (((hv_Angles.TupleSelect(hv_Index2)) - (hv_Angles.TupleSelect(
                            hv_Index3)))).TupleAbs();
                        HOperatorSet.TupleDeg(hv_Angle2, out hv_Deg);
                        if ((int)((new HTuple((new HTuple(170)).TupleLess(hv_Deg))).TupleAnd(new HTuple(hv_Deg.TupleLess(
                            190)))) != 0)
                        {
                            if ((int)(new HTuple((new HTuple(hv_A1.TupleLength())).TupleEqual(0))) != 0)
                            {
                                hv_A1 = new HTuple();
                                hv_A1 = hv_A1.TupleConcat(hv_Index2);
                                hv_A1 = hv_A1.TupleConcat(hv_Index3);
                            }
                            else
                            {
                                hv_A2 = new HTuple();
                                hv_A2 = hv_A2.TupleConcat(hv_Index2);
                                hv_A2 = hv_A2.TupleConcat(hv_Index3);
                            }
                            break;
                        }

                    }
                }

                //HOperatorSet.DistancePp(hv_ResultRow, hv_ResultColumn, hv_RowProjs.TupleSelect(
                //    hv_A1.TupleSelect(0)), hv_ColProjs.TupleSelect(hv_A1.TupleSelect(0)), out hv_Distances1);
                //HOperatorSet.DistancePp(hv_ResultRow, hv_ResultColumn, hv_RowProjs.TupleSelect(
                //    hv_A1.TupleSelect(1)), hv_ColProjs.TupleSelect(hv_A1.TupleSelect(1)), out hv_Distances2);

                HOperatorSet.DistancePp(hv_RowProjs.TupleSelect(hv_A1.TupleSelect(0)), hv_ColProjs.TupleSelect(hv_A1.TupleSelect(0)), hv_RowProjs.TupleSelect(
                   hv_A1.TupleSelect(1)), hv_ColProjs.TupleSelect(hv_A1.TupleSelect(1)), out hv_Distance1);
                //hv_Distance1 = hv_Distances1 + hv_Distances2;
                set_display_font(hv_WindowHandle, 10, "mono", "true", "false");
                double distan1 = hv_Distance1 * ypm + dGapoffset;
                disp_message(hv_WindowHandle, Math.Round(distan1,4) + "mm", "image", hv_RowProjs.TupleSelect(
                    hv_A1.TupleSelect(0)), hv_ColProjs.TupleSelect(hv_A1.TupleSelect(0)), "red",
                    "false");
                //HOperatorSet.DistancePp(hv_ResultRow, hv_ResultColumn, hv_RowProjs.TupleSelect(
                //    hv_A2.TupleSelect(0)), hv_ColProjs.TupleSelect(hv_A2.TupleSelect(0)), out hv_Distances1);
                //HOperatorSet.DistancePp(hv_ResultRow, hv_ResultColumn, hv_RowProjs.TupleSelect(
                //    hv_A2.TupleSelect(1)), hv_ColProjs.TupleSelect(hv_A2.TupleSelect(1)), out hv_Distances2);
                //hv_Distance2 = hv_Distances1 + hv_Distances2;
                HOperatorSet.DistancePp(hv_RowProjs.TupleSelect(hv_A2.TupleSelect(0)), hv_ColProjs.TupleSelect(hv_A2.TupleSelect(0)), hv_RowProjs.TupleSelect(
                   hv_A2.TupleSelect(1)), hv_ColProjs.TupleSelect(hv_A2.TupleSelect(1)), out hv_Distance2);
                set_display_font(hv_WindowHandle, 10, "mono", "true", "false");
                double distan2 = hv_Distance2 * ypm + dGapoffset;
                disp_message(hv_WindowHandle, Math.Round(distan2, 4) + "mm", "image", hv_RowProjs.TupleSelect(
                    hv_A2.TupleSelect(0)), hv_ColProjs.TupleSelect(hv_A2.TupleSelect(0)), "red",
                    "false");
                hv_A3 = new HTuple();
                hv_A3 = hv_A3.TupleConcat(hv_A1.TupleSelect(0));
                hv_A3 = hv_A3.TupleConcat(hv_A1.TupleSelect(
                    1));
                hv_A3 = hv_A3.TupleConcat(hv_A2.TupleSelect(0));
                hv_A3 = hv_A3.TupleConcat(hv_A2.TupleSelect(
                    1));
                for (hv_Index4 = 0; (int)hv_Index4 <= 4; hv_Index4 = (int)hv_Index4 + 1)
                {
                    for (hv_Index5 = 0; (int)hv_Index5 <= 3; hv_Index5 = (int)hv_Index5 + 1)
                    {
                        if ((int)(new HTuple(((hv_A3.TupleSelect(hv_Index5))).TupleEqual(hv_Index4))) != 0)
                        {
                            break;
                        }
                        if ((int)(new HTuple(hv_Index5.TupleEqual(3))) != 0)
                        {
                            hv_realRowProj = hv_RowProjs.TupleSelect(hv_Index4);
                            hv_realColProj = hv_ColProjs.TupleSelect(hv_Index4);
                        }
                    }
                }
                HOperatorSet.DistancePp(hv_ResultRow, hv_ResultColumn, hv_realRowProj, hv_realColProj,
                    out hv_Distance3);
                set_display_font(hv_WindowHandle, 10, "mono", "true", "false");
                double distance3 = hv_Distance3 * ypm + offset;
                disp_message(hv_WindowHandle, Math.Round(distance3,4) + "mm", "image", hv_realRowProj,
                    hv_realColProj, "red", "false");
                HOperatorSet.DistancePp(hv_ResultRow, hv_ResultColumn, hv_realRowProj, hv_realColProj,
                    out hv_resultDistance);
                HOperatorSet.AngleLl(hv_ResultRow, hv_ResultColumn, hv_realRowProj, hv_realColProj,
        hv_RowProjs.TupleSelect(hv_A1.TupleSelect(0)), hv_ColProjs.TupleSelect(
        hv_A1.TupleSelect(0)), hv_RowProjs.TupleSelect(hv_A1.TupleSelect(1)), hv_ColProjs.TupleSelect(
        hv_A1.TupleSelect(1)), out hv_Angle1);
                HOperatorSet.TupleDeg(hv_Angle1, out hv_Deg1);
                if ((int)((new HTuple((new HTuple((new HTuple(40)).TupleLess(hv_Deg1))).TupleAnd(
                    new HTuple(hv_Deg1.TupleLess(50))))).TupleOr((new HTuple((new HTuple(-140)).TupleLess(
                    hv_Deg1))).TupleAnd(new HTuple(hv_Deg1.TupleLess(-130)))))!= 0)
                
                {
                    Distance = hv_Distance3 * ypm + offset;
                    Distance1 = hv_Distance2 * ypm + dGapoffset;
                    Distance2 = hv_Distance1 * ypm + dGapoffset;


                }
                else
                {
                 
                    Distance = hv_Distance3 * ypm + offset;
                    Distance1 = hv_Distance1 * ypm + dGapoffset;
                    Distance2 = hv_Distance2 * ypm + dGapoffset;
 
                }

                ho_ImageMedian.Dispose();
                ho_Circle.Dispose();
                ho_ImageReduced.Dispose();
                ho_Regions.Dispose();
                ho_RegionClosing1.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_UsedEdges.Dispose();
                ho_Contour.Dispose();
                ho_ResultContours.Dispose();
                ho_CrossCenter.Dispose();
                ho_Cross.Dispose();
                ho_Circle1.Dispose();
                ho_Circle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Regions1.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_Rectangle.Dispose();
                ho_ModelContour.Dispose();
                ho_Contours.Dispose();
                ho_RegionLines.Dispose();
                ho_Cross1.Dispose();
                ho_RegionLines1.Dispose();
                return true;

            }
            catch
            {

                ho_ImageMedian.Dispose();
                ho_Circle.Dispose();
                ho_ImageReduced.Dispose();
                ho_Regions.Dispose();
                ho_RegionClosing1.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_UsedEdges.Dispose();
                ho_Contour.Dispose();
                ho_ResultContours.Dispose();
                ho_CrossCenter.Dispose();
                ho_Cross.Dispose();
                ho_Circle1.Dispose();
                ho_Circle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Regions1.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_Rectangle.Dispose();
                ho_ModelContour.Dispose();
                ho_Contours.Dispose();
                ho_RegionLines.Dispose();
                ho_Cross1.Dispose();
                ho_RegionLines1.Dispose();
                Distance = 0;
                Distance1 = 0;
                Distance2 = 0;
                return false;


            }

        }
        public bool ResultImProve(double dlimitUp, double dlimitDown, double dlimitUpGap, double dlimitDownGap, HTuple distance, HTuple distance1, HTuple distance2, HWindow window)
        {
            bool result = false;
            double dDistance = distance;
            double dDistance1 = distance1;
            double dDistance2 = distance2;
            if ((dDistance < dlimitUp && dDistance > dlimitDown) && (dDistance1 < dlimitUpGap && dDistance1 > dlimitDownGap) && (dDistance2 < dlimitUpGap && dDistance2 > dlimitDownGap))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            if (result)
            {
                set_display_font(window, 20, "mono", "true", "false");
                disp_message(window, "Pass", "image", 20, 50, "green", "false");
                disp_message(window, "BR:" + Math.Round(dDistance, 4) + "mm", "image", 140, 50, "green", "false");
                disp_message(window, "D1:" + Math.Round(dDistance1, 4) + "mm", "image", 260, 50, "green", "false");
                disp_message(window, "D2:" + Math.Round(dDistance2, 4) + "mm", "image", 380, 50, "green", "false");

            }
            else
            {
                set_display_font(window, 20, "mono", "true", "false");
                disp_message(window, "NG", "image", 20, 50, "red", "false");
                disp_message(window, "BR:" + Math.Round(dDistance, 4) + "mm", "image", 140, 50, "red", "false");
                disp_message(window, "D1:" + Math.Round(dDistance1, 4) + "mm", "image", 260, 50, "red", "false");
                disp_message(window, "D2:" + Math.Round(dDistance2, 4) + "mm", "image", 380, 50, "red", "false");

            }

            return result;
        }
        #endregion

        private void tabCenterMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbFigureShape.SelectedIndex = tabFigureShape.SelectedIndex;
        }

      
    

        #endregion

        private void btnFigureShape_FindModel_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (tabProcessMode.SelectedIndex == 0)
                i = int.Parse(ViewNum) - 1;
            if (tabProcessMode.SelectedIndex == 1)
                i = int.Parse(SetNum) - 1;
            ho_ImageSet.Dispose();
            HOperatorSet.CopyImage(halcon.Image[i], out  ho_ImageSet);
            HOperatorSet.GetImageSize(halcon.ImageOri[i], out width, out height);
            HOperatorSet.AreaCenter(halcon.ImageOri[i], out area, out row, out col);
            try
            {
                FindModel_FigureShape_Square();
            }
            catch (Exception ER)
            {
                MessageBox.Show(ER.ToString());
            }
        }

        private void ucRectangleLength1_FigureShape_ValueChanged(int CurrentValue)
        {
            RectangleLength1_FigureShape = ucRectangleLength1_FigureShape.Value;
            try
            {
                ShowFang();
            }
            catch
            {
            }

        }

        private void ucRectangleLength2_FigureShape_ValueChanged(int CurrentValue)
        {
            RectangleLength2_FigureShape = ucRectangleLength2_FigureShape.Value;
            try
            {
                ShowFang();
            }
            catch
            {
            }
        }

        private void nudOffset_InnerRadius_ValueChanged(object sender, EventArgs e)
        {
            Glue.Offset_InnerRadius = (double)nudOffset_InnerRadius.Value;
        }

        private void nudOffset_OuterRadius_ValueChanged(object sender, EventArgs e)
        {
            Glue.Offset_OuterRadius = (double)nudOffset_OuterRadius.Value;
        }

        private void btnDrawRegion_NeedleTip_Click(object sender, EventArgs e)
        {
            HWindow Window = hWImageSet.HalconWindow;
            HOperatorSet.SetColor(Window, "red");
            HOperatorSet.SetDraw(Window, "margin");
            HObject ho_Rectangle = new HObject();
            HTuple hv_RegionRow1 = new HTuple(), hv_RegionColumn1 = new HTuple(), hv_RegionRow2 = new HTuple(), hv_RegionColumn2 = new HTuple();
            //找出初始半徑
            HOperatorSet.DrawRectangle1(Window, out  hv_RegionRow1, out hv_RegionColumn1, out hv_RegionRow2, out hv_RegionColumn2);
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_RegionRow1, hv_RegionColumn1, hv_RegionRow2, hv_RegionColumn2);
            HOperatorSet.DispObj(ho_Rectangle, Window);
            ho_Rectangle.Dispose();
            MyVisionPara.m_NeedleTip.RegionRow1 = hv_RegionRow1;
            MyVisionPara.m_NeedleTip.RegionColumn1 = hv_RegionColumn1;
            MyVisionPara.m_NeedleTip.RegionRow2 = hv_RegionRow2;
            MyVisionPara.m_NeedleTip.RegionColumn2 = hv_RegionColumn2;
        }

        private void cmbContrastSet_NeedleTip_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ContrastSet = MyVisionPara.m_NeedleTip.ContrastSet = cmbContrastSet_NeedleTip.SelectedIndex;

        }

        private void ucGray_NeedleTip_ValueChanged(int CurrentValue)
        {
            if (readpara)
                return;
          
            MyVisionPara.m_NeedleTip.Gray = ucGray_NeedleTip.Value;
            int i = int.Parse(ViewNum) - 1;
            HWindow Window = hWImageSet.HalconWindow;

            HObject ho_Image = new HObject(), ho_Rectangle = new HObject(), ho_ImageReduced = new HObject(), ho_Region = new HObject();
            HObject ho_ConnectedRegions = new HObject(), ho_SelectedRegions = new HObject(), ho_ImageMedian = new HObject(), ho_Cross = new HObject();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_RegionRow1 = MyVisionPara.m_NeedleTip.RegionRow1;
            HTuple hv_RegionColumn1 = MyVisionPara.m_NeedleTip.RegionColumn1;
            HTuple hv_RegionRow2 = MyVisionPara.m_NeedleTip.RegionRow2;
            HTuple hv_RegionColumn2 = MyVisionPara.m_NeedleTip.RegionColumn2;

            try
            {
                ho_Image = halcon.Image[i].CopyObj(1, -1);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_RegionRow1, hv_RegionColumn1, hv_RegionRow2, hv_RegionColumn2);

                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);

                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_ImageReduced, out ho_ImageMedian, 15, 15);
                ho_Region.Dispose();
                if (MyVisionPara.m_NeedleTip.ContrastSet == 0)
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, MyVisionPara.m_NeedleTip.Gray, 255);
                else
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 0, MyVisionPara.m_NeedleTip.Gray);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", hv_Area.TupleMax(), hv_Area.TupleMax() + 1);

                ho_Image.DispObj(Window);
                HOperatorSet.SetDraw(Window, "fill");
                HOperatorSet.SetColor(Window, "green");
                ho_Region.DispObj(Window);
                HOperatorSet.SetDraw(Window, "margin");
                HOperatorSet.SetColor(Window, "blue");
                ho_SelectedRegions.DispObj(Window);
            }
            catch
            {
            }
            ho_Image.Dispose();
            ho_Rectangle.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_SelectedRegions.Dispose();
            ho_Cross.Dispose();
        }

        private void btnFindNeedleTip_NeedleTip_Click(object sender, EventArgs e)
        {
            if (readpara)
                return;

            HWindow Window = hWImageSet.HalconWindow;
            int i = int.Parse(ViewNum) - 1;
            HObject ho_ResultImage = new HObject();
            HTuple hv_ResultRow = new HTuple(), hv_ResultColumn = new HTuple();
            ImageProcess_FindNeedleTip(Window, halcon.Image[i],out ho_ResultImage, out hv_ResultRow, out hv_ResultColumn);

            hv_RowCenter = hv_ResultRow;
            hv_ColCenter = hv_ResultColumn;
        }
        //針頭校正用
        public int ImageProcess_FindNeedleTip(HWindow Window, HObject ho_Image, out HObject ho_ResultImage, out HTuple ResultRow_NeedleTip, out HTuple ResultColumn_NeedleTip)
        {
            int iImageProcessResult = 0;
            ResultRow_NeedleTip = 0;
            ResultColumn_NeedleTip = 0;
            HOperatorSet.GenEmptyObj(out ho_ResultImage);

            HObject ho_Rectangle = new HObject(), ho_ImageReduced = new HObject(), ho_Region = new HObject();
            HObject ho_ConnectedRegions = new HObject(), ho_SelectedRegions = new HObject(), ho_ImageMedian = new HObject(), ho_Cross = new HObject();
            HObject ho_RectangleSet = new HObject();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
           
            try
            {

                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, MyVisionPara.m_NeedleTip.RegionRow1, MyVisionPara.m_NeedleTip.RegionColumn1, MyVisionPara.m_NeedleTip.RegionRow2, MyVisionPara.m_NeedleTip.RegionColumn2);

                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);

                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_ImageReduced, out ho_ImageMedian, 15, 15);
                ho_Region.Dispose();
                if (MyVisionPara.m_NeedleTip.ContrastSet == 0)
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, MyVisionPara.m_NeedleTip.Gray, 255);
                else
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 0, MyVisionPara.m_NeedleTip.Gray);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", hv_Area.TupleMax(), hv_Area.TupleMax() + 1);
                HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row, out hv_Column);

                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
     
                HTuple hv_ResultRow_1 = new HTuple(), hv_ResultColumn_1 = new HTuple(), hv_ResultRow_2 = new HTuple(), hv_ResultColumn_2 = new HTuple();
                HTuple hv_ResultRow = new HTuple(), hv_ResultColumn = new HTuple();
                hv_ResultRow_1 = hv_Row - hv_Length1 * hv_Phi.TupleSin();
                hv_ResultColumn_1 = hv_Column - hv_Length1 * hv_Phi.TupleCos();
                hv_ResultRow_2 = hv_Row - hv_Length1 * (hv_Phi + ((HTuple)180).TupleRad()).TupleSin();
                hv_ResultColumn_2 = hv_Column - hv_Length1 * (hv_Phi + ((HTuple)180).TupleRad()).TupleCos();
                if (hv_ResultRow_1.D > hv_ResultRow_2.D)
                {
                    hv_ResultRow = hv_ResultRow_1.TupleRound();
                    hv_ResultColumn = hv_ResultColumn_1.TupleRound();
                }
                else
                {
                    hv_ResultRow = hv_ResultRow_2.TupleRound();
                    hv_ResultColumn = hv_ResultColumn_2.TupleRound();
                }

                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_ResultRow, hv_ResultColumn, 50, 0);

                HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetPart(Window, 0, 0, hv_Height - 1, hv_Width - 1);

                Window.ClearWindow();
                ho_Image.DispObj(Window);
                HOperatorSet.SetDraw(Window, "margin");
                HOperatorSet.SetColor(Window, "yellow");
                ho_SelectedRegions.DispObj(Window);
                HOperatorSet.SetColor(Window, "blue");
                ho_Rectangle.DispObj(Window);
 
                HOperatorSet.SetColor(Window, "red");
                ho_Cross.DispObj(Window);
                set_display_font(Window, 20, "mono", "true", "false");
               ResultRow_NeedleTip = hv_ResultRow;
                ResultColumn_NeedleTip = hv_ResultColumn;
            }
            catch
            {
                Window.ClearWindow();
                ho_Image.DispObj(Window);
                set_display_font(Window, 20, "mono", "true", "false");
                disp_message(Window, "針尖搜尋NG", "", 0, 0, "red", "false");
                disp_message(Window, "NG", "", 1000, 0, "red", "false");
            }
            ho_Rectangle.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_SelectedRegions.Dispose();
            ho_Cross.Dispose();
            ho_RectangleSet.Dispose();

            HOperatorSet.DumpWindowImage(out ho_ResultImage, Window);
            return iImageProcessResult;
        }
        //針頭檢測用
        public int ImageProcess_FindNeedleTip_Test(HWindow Window, HObject ho_Image, out HObject ho_ResultImage, out HTuple ResultRow_NeedleTip, out HTuple ResultColumn_NeedleTip)
        {
            int iImageProcessResult = 0;
            ResultRow_NeedleTip = 0;
            ResultColumn_NeedleTip = 0;
            HOperatorSet.GenEmptyObj(out ho_ResultImage);

            HObject ho_Rectangle = new HObject(), ho_ImageReduced = new HObject(), ho_Region = new HObject();
            HObject ho_ConnectedRegions = new HObject(), ho_SelectedRegions = new HObject(), ho_ImageMedian = new HObject(), ho_Cross = new HObject();
            HObject ho_RectangleSet = new HObject();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();

            try
            {
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, GCCD1.NeedleTipTest.RegionRow, GCCD1.NeedleTipTest.RegionColumn, GCCD1.NeedleTipTest.RegionPhi, GCCD1.NeedleTipTest.RegionLength1,GCCD1.NeedleTipTest.RegionLength2);

                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);

                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_ImageReduced, out ho_ImageMedian, 15, 15);
                ho_Region.Dispose();
                if (MyVisionPara.m_NeedleTip.ContrastSet == 0)
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, GCCD1.NeedleTipTest.Gray, 255);
                else
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 0, GCCD1.NeedleTipTest.Gray);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", hv_Area.TupleMax(), hv_Area.TupleMax() + 1);
                HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row, out hv_Column);

                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);

                HTuple hv_ResultRow_1 = new HTuple(), hv_ResultColumn_1 = new HTuple(), hv_ResultRow_2 = new HTuple(), hv_ResultColumn_2 = new HTuple();
                HTuple hv_ResultRow = new HTuple(), hv_ResultColumn = new HTuple();
                hv_ResultRow_1 = hv_Row - hv_Length1 * hv_Phi.TupleSin();
                hv_ResultColumn_1 = hv_Column - hv_Length1 * hv_Phi.TupleCos();
                hv_ResultRow_2 = hv_Row - hv_Length1 * (hv_Phi + ((HTuple)180).TupleRad()).TupleSin();
                hv_ResultColumn_2 = hv_Column - hv_Length1 * (hv_Phi + ((HTuple)180).TupleRad()).TupleCos();
                if (hv_ResultRow_1.D > hv_ResultRow_2.D)
                {
                    hv_ResultRow = hv_ResultRow_1.TupleRound();
                    hv_ResultColumn = hv_ResultColumn_1.TupleRound();
                }
                else
                {
                    hv_ResultRow = hv_ResultRow_2.TupleRound();
                    hv_ResultColumn = hv_ResultColumn_2.TupleRound();
                }

                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_ResultRow, hv_ResultColumn, 50, 0);

                HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetPart(Window, 0, 0, hv_Height - 1, hv_Width - 1);

                Window.ClearWindow();
                ho_Image.DispObj(Window);
                HOperatorSet.SetDraw(Window, "margin");
                HOperatorSet.SetColor(Window, "yellow");
                ho_SelectedRegions.DispObj(Window);
                HOperatorSet.SetColor(Window, "blue");
                ho_Rectangle.DispObj(Window);

                HOperatorSet.SetColor(Window, "red");
                ho_Cross.DispObj(Window);
                set_display_font(Window, 20, "mono", "true", "false");
                ResultRow_NeedleTip = hv_ResultRow;
                ResultColumn_NeedleTip = hv_ResultColumn;
            }
            catch
            {
                Window.ClearWindow();
                ho_Image.DispObj(Window);
                set_display_font(Window, 20, "mono", "true", "false");
                disp_message(Window, "針尖搜尋NG", "", 0, 0, "red", "false");
                disp_message(Window, "NG", "", 1000, 0, "red", "false");
            }
            ho_Rectangle.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_SelectedRegions.Dispose();
            ho_Cross.Dispose();
            ho_RectangleSet.Dispose();

            HOperatorSet.DumpWindowImage(out ho_ResultImage, Window);
            return iImageProcessResult;
        }


        private void cmbNeedleChoice_NeedleTipTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            
           GCCD1.NeedleTipTest.NeedleChoice =  tabNeedleTipChoice.SelectedIndex = cmbNeedleChoice_NeedleTipTest.SelectedIndex;
            if (readpara)
                return;
            iniFile.Write("GCCD1", "NeedleTipTest.NeedleChoice", GCCD1.NeedleTipTest.NeedleChoice.ToString(), FrmMain.propath);
        }

        private void tabNeedleTipChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbNeedleChoice_NeedleTipTest.SelectedIndex = tabNeedleTipChoice.SelectedIndex;
        }

        private void btnDrawRange_NeedleTipTest_Click(object sender, EventArgs e)
        {
            if (ho_ImageTest == null)
                return;
            if (bDrawing)
                return;
            bDrawing = true;
            try
            {
                HWindow Window = hWImageSet.HalconWindow;
                Window.ClearWindow();
                halcon.Image[6].DispObj(Window);
                set_display_font(Window, 30, "mono", "true", "false");
                disp_message(Window, "畫方形檢測區域", "", 0, 0, "green", "false");

                HObject ho_Rectangle = new HObject();
                HTuple hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
                HOperatorSet.DrawRectangle2(Window, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);

                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                Window.SetDraw("margin");
                Window.SetColor("blue");
                ho_Rectangle.DispObj(Window);

                GCCD1.NeedleTipTest.RegionRow = hv_Row;
                GCCD1.NeedleTipTest.RegionColumn = hv_Column;
                GCCD1.NeedleTipTest.RegionPhi = hv_Phi;
                GCCD1.NeedleTipTest.RegionLength1 = hv_Length1;
                GCCD1.NeedleTipTest.RegionLength2 = hv_Length2;
                
            }
            catch
            {
            }
           bDrawing = false;
        }

        private void cmbContrastSet_NeedleTipTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (readpara)
                return;
            GCCD1.NeedleTipTest.ContrastSet = cmbContrastSet_NeedleTipTest.SelectedIndex;
        }

        private void ucGray_NeedleTipTest_ValueChanged(object sender, EventArgs e)
        {
            if (readpara)
                return;
            GCCD1.NeedleTipTest.Gray = ucGray_NeedleTipTest.Value;
            HWindow Window = hWImageSet.HalconWindow;
            HObject ho_Image = new HObject(), ho_Rectangle = new HObject(), ho_ImageReduced = new HObject(), ho_Region = new HObject();
            HObject ho_ConnectedRegions = new HObject(), ho_SelectedRegions = new HObject(), ho_Cross = new HObject(), ho_ImageMedian = new HObject();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_RegionRow = GCCD1.NeedleTipTest.RegionRow;
            HTuple hv_RegionColumn = GCCD1.NeedleTipTest.RegionColumn;
            HTuple hv_RegionPhi = GCCD1.NeedleTipTest.RegionPhi;
            HTuple hv_RegionLength1 = GCCD1.NeedleTipTest.RegionLength1;
            HTuple hv_RegionLength2 = GCCD1.NeedleTipTest.RegionLength2;

            try
            {
                ho_Image = halcon.Image[6].CopyObj(1, -1);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_RegionRow, hv_RegionColumn, hv_RegionPhi, hv_RegionLength1, hv_RegionLength2);

                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_ImageReduced, out ho_ImageMedian, 15, 15);
                ho_Region.Dispose();
                if (GCCD1.NeedleTipTest.ContrastSet == 0)
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, GCCD1.NeedleTipTest.Gray, 255);
                else
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 0, GCCD1.NeedleTipTest.Gray);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", hv_Area.TupleMax(), hv_Area.TupleMax() + 1);
                HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row, out hv_Column);
                ho_Image.DispObj(Window);
                HOperatorSet.SetDraw(Window, "fill");
                HOperatorSet.SetColor(Window, "green");
                ho_Region.DispObj(Window);
                HOperatorSet.SetDraw(Window, "margin");
                HOperatorSet.SetColor(Window, "blue");
                ho_SelectedRegions.DispObj(Window);
                //參數匯入tool
                ucFitCircleTool_NeedleTipTest.Window = Window;
                ucFitCircleTool_NeedleTipTest.ho_Image = ho_Image.CopyObj(1,-1);
                ucFitCircleTool_NeedleTipTest.hv_InitialRow = hv_Row;
                ucFitCircleTool_NeedleTipTest.hv_InitialColumn = hv_Column;
            }
            catch
            {
            }
            ho_Image.Dispose();
            ho_Rectangle.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_SelectedRegions.Dispose();
            ho_Cross.Dispose();
        }

        private void btnSave_NeedleTipTest_Click(object sender, EventArgs e)
        {
            IniFile.Write("GCCD1", "NeedleTipTest.RegionRow", GCCD1.NeedleTipTest.RegionRow.D.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.RegionColumn", GCCD1.NeedleTipTest.RegionColumn.D.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.RegionPhi", GCCD1.NeedleTipTest.RegionPhi.D.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.RegionLength1", GCCD1.NeedleTipTest.RegionLength1.D.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.RegionLength2", GCCD1.NeedleTipTest.RegionLength2.D.ToString(), FrmMain.propath);

            IniFile.Write("GCCD1", "NeedleTipTest.ContrastSet", GCCD1.NeedleTipTest.ContrastSet.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.Gray", GCCD1.NeedleTipTest.Gray.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.NeedleChoice", GCCD1.NeedleTipTest.NeedleChoice.ToString(), FrmMain.propath);

            GCCD1.NeedleTipTest.Radius = ucFitCircleTool_NeedleTipTest.radius;
            GCCD1.NeedleTipTest.Measure_Transition = ucFitCircleTool_NeedleTipTest.measure_transition;
            GCCD1.NeedleTipTest.Measure_Select = ucFitCircleTool_NeedleTipTest.measure_select;
            GCCD1.NeedleTipTest.Num_Measures = ucFitCircleTool_NeedleTipTest.num_measures;
            GCCD1.NeedleTipTest.Measure_Length1 = ucFitCircleTool_NeedleTipTest.measure_length1;
            GCCD1.NeedleTipTest.Measure_Length2 = ucFitCircleTool_NeedleTipTest.measure_length2;
            GCCD1.NeedleTipTest.Measure_Threshold = ucFitCircleTool_NeedleTipTest.measure_threshold;

            IniFile.Write("GCCD1", "NeedleTipTest.Radius", GCCD1.NeedleTipTest.Radius.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.Measure_Transition", GCCD1.NeedleTipTest.Measure_Transition.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.Measure_Select", GCCD1.NeedleTipTest.Measure_Select.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.Num_Measures", GCCD1.NeedleTipTest.Num_Measures.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.Measure_Length1", GCCD1.NeedleTipTest.Measure_Length1.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.Measure_Length2", GCCD1.NeedleTipTest.Measure_Length2.ToString(), FrmMain.propath);
            IniFile.Write("GCCD1", "NeedleTipTest.Measure_Threshold", GCCD1.NeedleTipTest.Measure_Threshold.ToString(), FrmMain.propath);

        }

        private void btnFindNeedleTip_NeedleTipTest_Click(object sender, EventArgs e)
        {
            if (readpara)
                return;

            HWindow Window = hWImageSet.HalconWindow;
            //int i = int.Parse(ViewNum) - 1;
            int i = 6;//默認GCCD1
            HObject ho_ResultImage = new HObject();
            HTuple hv_ResultRow = new HTuple(), hv_ResultColumn = new HTuple();
            ImageProcess_FindNeedleTip_Test(Window, halcon.Image[i], out ho_ResultImage, out hv_ResultRow, out hv_ResultColumn);

            GCCD1.NeedleTipTest.ResultRow = hv_ResultRow;
            GCCD1.NeedleTipTest.ResultColumn = hv_ResultColumn;
        }

        private void nudX_UpperSet_NeedleTipTest_ValueChanged(object sender, EventArgs e)
        {
            GCCD1.NeedleTipTest.X_UpperSet = (int)nudX_UpperSet_NeedleTipTest.Value;
            IniFile.Write("GCCD1", "NeedleTipTest.X_UpperSet", GCCD1.NeedleTipTest.X_UpperSet.ToString(), FrmMain.propath);
        }

        private void nudX_LowerSet_NeedleTipTest_ValueChanged(object sender, EventArgs e)
        {
            GCCD1.NeedleTipTest.X_LowerSet = (int)nudX_LowerSet_NeedleTipTest.Value;
            IniFile.Write("GCCD1", "NeedleTipTest.X_LowerSet", GCCD1.NeedleTipTest.X_LowerSet.ToString(), FrmMain.propath);
        }

        private void nudY_UpperSet_NeedleTipTest_ValueChanged(object sender, EventArgs e)
        {
            GCCD1.NeedleTipTest.Y_UpperSet = (int)nudY_UpperSet_NeedleTipTest.Value;
            IniFile.Write("GCCD1", "NeedleTipTest.Y_UpperSet", GCCD1.NeedleTipTest.Y_UpperSet.ToString(), FrmMain.propath);
        }

        private void nudY_LowerSet_NeedleTipTest_ValueChanged(object sender, EventArgs e)
        {
            GCCD1.NeedleTipTest.Y_LowerSet = (int)nudY_LowerSet_NeedleTipTest.Value;
            IniFile.Write("GCCD1", "NeedleTipTest.Y_LowerSet", GCCD1.NeedleTipTest.Y_LowerSet.ToString(), FrmMain.propath);
        }

        private void btnChange_NeedleTipTest_Click(object sender, EventArgs e)
        {

            try
            {
                switch (GCCD1.NeedleTipTest.NeedleChoice)
                {
                    case 0:
                        {
                            if (ucFitCircleTool_NeedleTipTest.hv_CenterRow.Length == 0)
                            {
                                MessageBox.Show("圓心為空");
                                return;
                            }
                            GCCD1.NeedleTipTest.ResultRow = ucFitCircleTool_NeedleTipTest.hv_CenterRow;
                            GCCD1.NeedleTipTest.ResultColumn = ucFitCircleTool_NeedleTipTest.hv_CenterColumn;
                        } break;
                    case 1:
                        {
                            if (GCCD1.NeedleTipTest.ResultRow == null)
                            {
                                MessageBox.Show("圓心為空");
                                return;
                            }
                        } break;
                }

                GCCD1.NeedleTipTest.X_UpperValue = Math.Round(GCCD1.NeedleTipTest.ResultColumn.D,0) + GCCD1.NeedleTipTest.X_UpperSet;
                GCCD1.NeedleTipTest.X_LowerValue = Math.Round(GCCD1.NeedleTipTest.ResultColumn.D, 0) + GCCD1.NeedleTipTest.X_LowerSet;
                GCCD1.NeedleTipTest.Y_UpperValue = Math.Round(GCCD1.NeedleTipTest.ResultRow.D, 0) + GCCD1.NeedleTipTest.Y_UpperSet;
                GCCD1.NeedleTipTest.Y_LowerValue = Math.Round(GCCD1.NeedleTipTest.ResultRow.D, 0) + GCCD1.NeedleTipTest.Y_LowerSet;

                lblX_UpperValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.X_UpperValue.ToString();
                lblX_LowerValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.X_LowerValue.ToString();
                lblY_UpperValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.Y_UpperValue.ToString();
                lblY_LowerValue_NeedleTipTest.Text = GCCD1.NeedleTipTest.Y_LowerValue.ToString();

                IniFile.Write("GCCD1", "NeedleTipTest.X_UpperValue", GCCD1.NeedleTipTest.X_UpperValue.ToString(), FrmMain.propath);
                IniFile.Write("GCCD1", "NeedleTipTest.X_LowerValue", GCCD1.NeedleTipTest.X_LowerValue.ToString(), FrmMain.propath);
                IniFile.Write("GCCD1", "NeedleTipTest.Y_UpperValue", GCCD1.NeedleTipTest.Y_UpperValue.ToString(), FrmMain.propath);
                IniFile.Write("GCCD1", "NeedleTipTest.Y_LowerValue", GCCD1.NeedleTipTest.Y_LowerValue.ToString(), FrmMain.propath);
            }
            catch
            {
                MessageBox.Show("圓心為空");
                return;
            }
        }

        private void btnTest_NeedleTipTest_Click(object sender, EventArgs e)
        {
            HObject ho_ResultImage = new HObject();
            int i=6;
            ImageProcess_NeedleTipTest(hWImageSet.HalconWindow, halcon.Image[i], out ho_ResultImage);


        }
        /// <summary>
        /// 針頭檢測
        /// </summary>
        /// <param name="Window"></param>
        /// <param name="ho_Image"></param>
        /// <param name="ho_ResultImage"></param>
        public int ImageProcess_NeedleTipTest(HWindow Window, HObject ho_Image, out HObject ho_ResultImage)
        {
            int iImageProcessResult = 0;
            HOperatorSet.GenEmptyObj(out ho_ResultImage);
            set_display_font(Window, 30, "mono", "true", "false");
            HObject ho_TestRegion = new HObject();
            HObject ho_Rectangle = new HObject(), ho_ImageReduced = new HObject(), ho_Region = new HObject();
            HObject ho_ConnectedRegions = new HObject(), ho_SelectedRegions = new HObject(), ho_Cross = new HObject(), ho_ImageMedian = new HObject();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_RegionRow = GCCD1.NeedleTipTest.RegionRow;
            HTuple hv_RegionColumn = GCCD1.NeedleTipTest.RegionColumn;
            HTuple hv_RegionPhi = GCCD1.NeedleTipTest.RegionPhi;
            HTuple hv_RegionLength1 = GCCD1.NeedleTipTest.RegionLength1;
            HTuple hv_RegionLength2 = GCCD1.NeedleTipTest.RegionLength2;
            
            HTuple hv_Gray = GCCD1.NeedleTipTest.Gray;
            try
            {
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_RegionRow, hv_RegionColumn, hv_RegionPhi, hv_RegionLength1, hv_RegionLength2);

                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
                ho_ImageMedian.Dispose();
                HOperatorSet.MedianRect(ho_ImageReduced, out ho_ImageMedian, 15, 15);
                ho_Region.Dispose();
                if (GCCD1.NeedleTipTest.ContrastSet == 0)
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, hv_Gray, 255);
                else
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 0, hv_Gray);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", hv_Area.TupleMax(), hv_Area.TupleMax() + 1);
                HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row, out hv_Column);
                //沒找到區域
                if (hv_Row.Length == 0)
                {
                    ho_Image.DispObj(Window);
                    HOperatorSet.SetDraw(Window, "margin");
                    HOperatorSet.SetColor(Window, "blue");
                    ho_Rectangle.DispObj(Window);
                    disp_message(Window, "針尖搜尋NG", "", 0, 0, "red", "false");
                    HOperatorSet.DumpWindowImage(out ho_ResultImage, Window);
                    iImageProcessResult = -1;
                    return iImageProcessResult;
                }
                
                HTuple hv_ResultRow = new HTuple(), hv_ResultColumn = new HTuple();
                switch (GCCD1.NeedleTipTest.NeedleChoice)
                {
                    // 直針
                    case 0:
                        {
                            ucFitCircleTool_NeedleTipTest.hv_InitialRow = hv_Row;
                            ucFitCircleTool_NeedleTipTest.hv_InitialColumn = hv_Column;
                            iImageProcessResult = ucFitCircleTool_NeedleTipTest.ImageProcess_FitCircle(ho_Image, out ho_ResultContours, out ho_CrossCenter, out hv_ResultRow, out hv_ResultColumn);
                        } break;
                    // 彎針
                    case 1:
                        {
                            HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);

                            HTuple hv_ResultRow_1 = new HTuple(), hv_ResultColumn_1 = new HTuple(), hv_ResultRow_2 = new HTuple(), hv_ResultColumn_2 = new HTuple();
                            hv_ResultRow_1 = hv_Row - hv_Length1 * hv_Phi.TupleSin();
                            hv_ResultColumn_1 = hv_Column - hv_Length1 * hv_Phi.TupleCos();
                            hv_ResultRow_2 = hv_Row - hv_Length1 * (hv_Phi + ((HTuple)180).TupleRad()).TupleSin();
                            hv_ResultColumn_2 = hv_Column - hv_Length1 * (hv_Phi + ((HTuple)180).TupleRad()).TupleCos();
                            if (hv_ResultRow_1.D > hv_ResultRow_2.D)
                            {
                                hv_ResultRow = hv_ResultRow_1.TupleRound();
                                hv_ResultColumn = hv_ResultColumn_1.TupleRound();
                            }
                            else
                            {
                                hv_ResultRow = hv_ResultRow_2.TupleRound();
                                hv_ResultColumn = hv_ResultColumn_2.TupleRound();
                            }
                            iImageProcessResult = 1;
                        } break;
                }
                //找針尖NG
                if (iImageProcessResult != 1)
                {
                    ho_Image.DispObj(Window);
                    HOperatorSet.SetDraw(Window, "fill");
                    HOperatorSet.SetColor(Window, "green");
                    ho_Region.DispObj(Window);
                    HOperatorSet.SetDraw(Window, "margin");
                    HOperatorSet.SetColor(Window, "blue");
                    ho_SelectedRegions.DispObj(Window);
                    ho_Rectangle.DispObj(Window);
                    disp_message(Window, "針尖精準圓心搜尋NG", "", 0, 0, "red", "false");
                    HOperatorSet.DumpWindowImage(out ho_ResultImage, Window);
                    iImageProcessResult = -1;
                    return iImageProcessResult;
                }
                
                HOperatorSet.GenRectangle1(out ho_TestRegion, GCCD1.NeedleTipTest.Y_LowerValue, GCCD1.NeedleTipTest.X_LowerValue, GCCD1.NeedleTipTest.Y_UpperValue, GCCD1.NeedleTipTest.X_UpperValue);
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_ResultRow, hv_ResultColumn, 50, 0);
                //顯示
                ho_Image.DispObj(Window);
                    HOperatorSet.SetDraw(Window, "margin");
                    HOperatorSet.SetColor(Window, "blue");
                    ho_Rectangle.DispObj(Window);
                    HOperatorSet.SetColor(Window, "green");
                    ho_TestRegion.DispObj(Window);
                //判斷針尖是否在標準內
                //OK
                if (hv_ResultRow.D <= GCCD1.NeedleTipTest.Y_UpperValue &&
                    hv_ResultRow.D >= GCCD1.NeedleTipTest.Y_LowerValue &&
                    hv_ResultColumn.D <= GCCD1.NeedleTipTest.X_UpperValue &&
                    hv_ResultColumn.D >= GCCD1.NeedleTipTest.X_LowerValue)
                {
                    HOperatorSet.SetColor(Window, "green");
                    ho_Cross.DispObj(Window);
                    disp_message(Window, "針尖檢測OK", "", 0, 0, "green", "false");
                    HOperatorSet.DumpWindowImage(out ho_ResultImage, Window);
                    iImageProcessResult = 1;
                    return iImageProcessResult;
                }
                //NG
                else
                {
                    HOperatorSet.SetColor(Window, "red");
                    ho_Cross.DispObj(Window);
                    disp_message(Window, "針尖檢測NG", "", 0, 0, "red", "false");
                    HOperatorSet.DumpWindowImage(out ho_ResultImage, Window);
                    iImageProcessResult = -1;
                    return iImageProcessResult;
                }
            }
            catch
            {
                ho_Image.DispObj(Window);
                HOperatorSet.SetDraw(Window, "margin");
                HOperatorSet.SetColor(Window, "blue");
                ho_Rectangle.DispObj(Window);
                disp_message(Window, "未知原因NG", "", 0, 0, "red", "false");
                HOperatorSet.DumpWindowImage(out ho_ResultImage, Window);
                iImageProcessResult = -1;
                return iImageProcessResult;
            }
            ho_Image.Dispose();
            ho_Rectangle.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_SelectedRegions.Dispose();
            ho_Cross.Dispose();
            ho_TestRegion.Dispose();
            ho_ImageMedian.Dispose();
        }
        
    }
}

    
      

    