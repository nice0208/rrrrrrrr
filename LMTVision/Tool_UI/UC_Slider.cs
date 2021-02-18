using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace UC_Slider
{
     [DefaultEvent("ValueChanged")]
    public partial class UC_Slider : UserControl
    {
        public UC_Slider()
        {
            InitializeComponent();
            Reset();
        }

        private int value = 0;
        private int maximum = 100;
        private int minimum = 0;
        private int SmallChange = 1;
        /// <summary>
        /// 控鍵數值更改時發生
        /// </summary>
        /// <param name="CurrentValue"></param>
        public delegate void ValueChangeEventHandler(object sender, EventArgs e);
        public event ValueChangeEventHandler ValueChanged;
        public void ValueChangedEvent(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this,e);
            }
        }
        
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                //如果超過最大最小值就等於最大或最小值
                value = value > maximum ? maximum : value;
                value = value < minimum ? minimum : value;
                nudCurrentValue.Value = value;
            }
        }
        //如果Maximum沒設置默認為100
        public int Maximum
        {
            get
            {

                return maximum;
            }
            set
            {
                maximum = value < minimum ? minimum + 1 : value;
                Slider.Maximum = maximum;
                nudCurrentValue.Maximum = maximum;
            }
        }
        //如果沒設置默認為0
        public int Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                minimum = value > maximum ? maximum -1 : value;
                Slider.Minimum = minimum;
                nudCurrentValue.Minimum = minimum;
            }
        }
        public void ParaMeter()
        {

        }

        // Deactivate the control and deregister the callback.
        private void Reset()
        {
            Maximum = maximum;
            Minimum = minimum;
            Value = value;
        }
        
        private void Slider_Scroll(object sender, EventArgs e)
        {
            nudCurrentValue.Value = Slider.Value;
        }

        private void nudCurrentValue_ValueChanged(object sender, EventArgs e)
        {
            Slider.Value = (int)nudCurrentValue.Value;
            value = (int)nudCurrentValue.Value;
            ValueChangedEvent(sender,e);
        }
    }
}
