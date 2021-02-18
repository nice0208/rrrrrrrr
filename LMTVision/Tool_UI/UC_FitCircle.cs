using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UC_FirCircle
{
    public partial class UC_FitCircle : UserControl
    {
        public UC_FitCircle()
        {
            InitializeComponent();
            SetValue(radius, measure_transition, measure_select, num_measures, measure_length1, measure_length2, measure_threshold);
        }

        public delegate void SetChangeHandler();
        public event SetChangeHandler SetChangedEvent;

        public delegate void DisplayResultHandler();
        public event DisplayResultHandler DispResultEvent;
        //public void SetChangedEvent(object sender, EventArgs e)
        //{
        //    if (SetChanged != null)
        //    {
        //        SetChanged(this, e);
        //    }
        //}
  
        /// <summary>
        /// 圓半徑
        /// </summary>
        private int radius = 100;
        /// <summary>
        /// 極性設定
        /// </summary>
        private string measure_transition = "positive";
        /// <summary>
        /// 邊設定
        /// </summary>
        private string measure_select = "last";
        /// <summary>
        /// 卡尺數量設定
        /// </summary>
        private int num_measures = 10;
        /// <summary>
        /// 卡尺高設定
        /// </summary>
        private int measure_length1 = 30;
        /// <summary>
        /// 卡尺寬設定
        /// </summary>
        private int measure_length2 = 5;
        /// <summary>
        /// 卡尺閥值設定
        /// </summary>
        private int measure_threshold = 128;
       
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                ucRadius.Value = value;
            }
        }

        public string Measure_Transition
        {
            get
            {
                return measure_transition;
            }
            set
            {
                measure_transition = value;
                switch (value)
                {
                    case "positive": cmbMeasure_Transition.SelectedIndex = 0; break;
                    case "negative": cmbMeasure_Transition.SelectedIndex = 1; break;
                    case "all": cmbMeasure_Transition.SelectedIndex = 2; break;
                }
            }
        }

        public string Measure_Select
        {
            get
            {
                return measure_select;
            }
            set
            {
                measure_select = value;
                switch (value)
                {
                    case "first": cmbMeasure_Select.SelectedIndex = 0; break;
                    case "last": cmbMeasure_Select.SelectedIndex = 1; break;
                }
            }
        }

        public int Num_Measures
        {
            get
            {
                return num_measures;
            }
            set
            {
                num_measures = value;
                ucNum_Measures.Value = value;
            }
        }

        public int Measure_Length1
        {
            get
            {
                return measure_length1;
            }
            set
            {
                measure_length1 = value;
                ucMeasure_Length1.Value = value;
            }
        }

        public int Measure_Length2
        {
            get
            {
                return measure_length2;
            }
            set
            {
                measure_length2 = value;
                ucMeasure_Length2.Value = value;
            }
        }

        public int Measure_Threshold
        {
            get
            {
                return measure_threshold;
            }
            set
            {
                measure_threshold = value;
                ucMeasure_Threshold.Value = value;
            }
        }

        private void ucRadius_ValueChanged(object sender, EventArgs e)
        {
            radius = ucRadius.Value;
            SetChangedEvent();
        }

        private void cmbMeasure_Transition_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbMeasure_Transition.SelectedIndex)
            {
                case 0: measure_transition = "positive"; break;
                case 1: measure_transition = "negative"; break;
                case 2: measure_transition = "all"; break;   
            }
            SetChangedEvent();
        }

        private void cmbMeasure_Select_SelectedIndexChanged(object sender, EventArgs e)
        
        {
            switch (cmbMeasure_Select.SelectedIndex)
            {
                case 0: measure_select = "first"; break;
                case 1: measure_select = "last"; break;
            }
            SetChangedEvent();
        }

        private void ucNum_Measures_ValueChanged(object sender, EventArgs e)
        {
            num_measures = ucNum_Measures.Value;
            SetChangedEvent();
        }

        private void ucMeasure_Length1_ValueChanged(object sender, EventArgs e)
        {
            measure_length1 = ucMeasure_Length1.Value;
            SetChangedEvent();
        }

        private void ucMeasure_Length2_ValueChanged(object sender, EventArgs e)
        {
            measure_length2 = ucMeasure_Length2.Value;
            SetChangedEvent();
        }
        
        private void ucMeasure_Threshold_ValueChanged(object sender, EventArgs e)
        {
            measure_threshold = ucMeasure_Threshold.Value;
            SetChangedEvent();
        }

        private void btnFitCircle_Click(object sender, EventArgs e)
        {
            DispResultEvent();
        }
        /// <summary>
        /// 設置參數
        /// </summary>
        /// <param name="_Radius">卡尺半徑</param>
        /// <param name="_Measure_Transition">卡尺極性</param>
        /// <param name="_Measure_Select">卡尺邊</param>
        /// <param name="_Num_Measures">卡尺數量</param>
        /// <param name="_Measure_Length1">卡尺高</param>
        /// <param name="_Measure_Length2">卡尺寬</param>
        public void SetValue(int _Radius, string _Measure_Transition, string _Measure_Select, int _Num_Measures, int _Measure_Length1, int _Measure_Length2,int _Measure_Threshold)
        {
            //斷開委託(避免設置參數時觸發到圖像處理引起bug)
            ucRadius.ValueChanged -= new UC_Slider.UC_Slider.ValueChangeEventHandler(ucRadius_ValueChanged);
            cmbMeasure_Transition.SelectedIndexChanged -= new EventHandler(cmbMeasure_Transition_SelectedIndexChanged);
            cmbMeasure_Select.SelectedIndexChanged -= new EventHandler(cmbMeasure_Select_SelectedIndexChanged);
            ucNum_Measures.ValueChanged -= new UC_Slider.UC_Slider.ValueChangeEventHandler(ucNum_Measures_ValueChanged);
            ucMeasure_Length1.ValueChanged -= new UC_Slider.UC_Slider.ValueChangeEventHandler(ucMeasure_Length1_ValueChanged);
            ucMeasure_Length2.ValueChanged -= new UC_Slider.UC_Slider.ValueChangeEventHandler(ucMeasure_Length2_ValueChanged);
            ucMeasure_Threshold.ValueChanged -= new UC_Slider.UC_Slider.ValueChangeEventHandler(ucMeasure_Threshold_ValueChanged);
            //設置參數
            Radius = _Radius;
            Measure_Transition = _Measure_Transition;
            Measure_Select = _Measure_Select;
            Num_Measures = _Num_Measures;
            Measure_Length1 = _Measure_Length1;
            Measure_Length2 = _Measure_Length2;
            Measure_Threshold = _Measure_Threshold;
            //連接委託
            ucRadius.ValueChanged += new UC_Slider.UC_Slider.ValueChangeEventHandler(ucRadius_ValueChanged);
            cmbMeasure_Transition.SelectedIndexChanged += new EventHandler(cmbMeasure_Transition_SelectedIndexChanged);
            cmbMeasure_Select.SelectedIndexChanged += new EventHandler(cmbMeasure_Select_SelectedIndexChanged);
            ucNum_Measures.ValueChanged += new UC_Slider.UC_Slider.ValueChangeEventHandler(ucNum_Measures_ValueChanged);
            ucMeasure_Length1.ValueChanged += new UC_Slider.UC_Slider.ValueChangeEventHandler(ucMeasure_Length1_ValueChanged);
            ucMeasure_Length2.ValueChanged += new UC_Slider.UC_Slider.ValueChangeEventHandler(ucMeasure_Length2_ValueChanged);
            ucMeasure_Threshold.ValueChanged += new UC_Slider.UC_Slider.ValueChangeEventHandler(ucMeasure_Threshold_ValueChanged);
        }

        

        
       
     
        
    }
}
