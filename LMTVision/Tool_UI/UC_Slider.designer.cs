namespace UC_Slider
{
    partial class UC_Slider
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Slider = new System.Windows.Forms.TrackBar();
            this.nudCurrentValue = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentValue)).BeginInit();
            this.SuspendLayout();
            // 
            // Slider
            // 
            this.Slider.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Slider.AutoSize = false;
            this.Slider.BackColor = System.Drawing.Color.White;
            this.Slider.Location = new System.Drawing.Point(-1, 0);
            this.Slider.Name = "Slider";
            this.Slider.Size = new System.Drawing.Size(168, 27);
            this.Slider.TabIndex = 0;
            this.Slider.Scroll += new System.EventHandler(this.Slider_Scroll);
            // 
            // nudCurrentValue
            // 
            this.nudCurrentValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nudCurrentValue.Location = new System.Drawing.Point(168, 3);
            this.nudCurrentValue.Name = "nudCurrentValue";
            this.nudCurrentValue.Size = new System.Drawing.Size(67, 21);
            this.nudCurrentValue.TabIndex = 2;
            this.nudCurrentValue.ValueChanged += new System.EventHandler(this.nudCurrentValue_ValueChanged);
            // 
            // UC_Slider
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.Slider);
            this.Controls.Add(this.nudCurrentValue);
            this.Name = "UC_Slider";
            this.Size = new System.Drawing.Size(238, 27);
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar Slider;
        private System.Windows.Forms.NumericUpDown nudCurrentValue;
    }
}
