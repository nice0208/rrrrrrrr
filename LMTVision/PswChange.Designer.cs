namespace LMTVision
{
    partial class PswChange
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
            this.cmbUsers2 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnModifi = new System.Windows.Forms.Button();
            this.txtAgainPsw = new System.Windows.Forms.TextBox();
            this.txtNewPsw = new System.Windows.Forms.TextBox();
            this.txtOldPsw = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbUsers2
            // 
            this.cmbUsers2.Font = new System.Drawing.Font("Georgia", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUsers2.ForeColor = System.Drawing.Color.Black;
            this.cmbUsers2.FormattingEnabled = true;
            this.cmbUsers2.Location = new System.Drawing.Point(120, 31);
            this.cmbUsers2.Name = "cmbUsers2";
            this.cmbUsers2.Size = new System.Drawing.Size(100, 23);
            this.cmbUsers2.TabIndex = 43;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(42, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 42;
            this.label4.Text = "用户名：";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(147, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 34);
            this.btnCancel.TabIndex = 41;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnModifi
            // 
            this.btnModifi.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnModifi.Location = new System.Drawing.Point(39, 198);
            this.btnModifi.Name = "btnModifi";
            this.btnModifi.Size = new System.Drawing.Size(75, 34);
            this.btnModifi.TabIndex = 40;
            this.btnModifi.Text = "修改";
            this.btnModifi.UseVisualStyleBackColor = true;
            this.btnModifi.Click += new System.EventHandler(this.btnModifi_Click);
            // 
            // txtAgainPsw
            // 
            this.txtAgainPsw.Location = new System.Drawing.Point(120, 155);
            this.txtAgainPsw.Name = "txtAgainPsw";
            this.txtAgainPsw.PasswordChar = '*';
            this.txtAgainPsw.Size = new System.Drawing.Size(100, 21);
            this.txtAgainPsw.TabIndex = 39;
            // 
            // txtNewPsw
            // 
            this.txtNewPsw.Location = new System.Drawing.Point(120, 114);
            this.txtNewPsw.Name = "txtNewPsw";
            this.txtNewPsw.PasswordChar = '*';
            this.txtNewPsw.Size = new System.Drawing.Size(100, 21);
            this.txtNewPsw.TabIndex = 38;
            // 
            // txtOldPsw
            // 
            this.txtOldPsw.Location = new System.Drawing.Point(120, 73);
            this.txtOldPsw.Name = "txtOldPsw";
            this.txtOldPsw.PasswordChar = '*';
            this.txtOldPsw.Size = new System.Drawing.Size(100, 21);
            this.txtOldPsw.TabIndex = 37;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 36;
            this.label3.Text = "确认密码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 35;
            this.label2.Text = "新密码";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 34;
            this.label1.Text = "旧密码";
            // 
            // PswChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 269);
            this.Controls.Add(this.cmbUsers2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnModifi);
            this.Controls.Add(this.txtAgainPsw);
            this.Controls.Add(this.txtNewPsw);
            this.Controls.Add(this.txtOldPsw);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PswChange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PswChange";
            this.Load += new System.EventHandler(this.PswChange_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbUsers2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnModifi;
        private System.Windows.Forms.TextBox txtAgainPsw;
        private System.Windows.Forms.TextBox txtNewPsw;
        private System.Windows.Forms.TextBox txtOldPsw;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}