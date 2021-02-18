namespace LMTVision
{
    partial class FrmVar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSta = new System.Windows.Forms.TextBox();
            this.label331 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtAdd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSta
            // 
            this.txtSta.Location = new System.Drawing.Point(94, 42);
            this.txtSta.Name = "txtSta";
            this.txtSta.Size = new System.Drawing.Size(46, 21);
            this.txtSta.TabIndex = 177;
            this.txtSta.Text = "7.51";
            // 
            // label331
            // 
            this.label331.AutoSize = true;
            this.label331.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label331.Location = new System.Drawing.Point(36, 46);
            this.label331.Name = "label331";
            this.label331.Size = new System.Drawing.Size(54, 12);
            this.label331.TabIndex = 176;
            this.label331.Text = "if( D<=";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(141, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 12);
            this.label1.TabIndex = 178;
            this.label1.Text = ") ";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(121, 100);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 180;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtAdd
            // 
            this.txtAdd.Location = new System.Drawing.Point(93, 67);
            this.txtAdd.Name = "txtAdd";
            this.txtAdd.Size = new System.Drawing.Size(46, 21);
            this.txtAdd.TabIndex = 182;
            this.txtAdd.Text = "0.02";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(61, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 181;
            this.label2.Text = "D=D+";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(94, 16);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(46, 21);
            this.txtOffset.TabIndex = 184;
            this.txtOffset.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(40, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 12);
            this.label3.TabIndex = 183;
            this.label3.Text = "Offset:";
            // 
            // FrmVar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 134);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAdd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSta);
            this.Controls.Add(this.label331);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmVar";
            this.Text = "FrmVar";
            this.Load += new System.EventHandler(this.FrmVar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSta;
        private System.Windows.Forms.Label label331;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtAdd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Label label3;
    }
}