namespace YChan
{
    partial class CloseWarn
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
            this.lblText = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNoClose = new System.Windows.Forms.Button();
            this.chkWarning = new System.Windows.Forms.CheckBox();
            this.chkSave = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 9);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(327, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Are you sure you want to close YChan? There are still open threads!";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(15, 92);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close ";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNoClose
            // 
            this.btnNoClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNoClose.Location = new System.Drawing.Point(306, 92);
            this.btnNoClose.Name = "btnNoClose";
            this.btnNoClose.Size = new System.Drawing.Size(75, 23);
            this.btnNoClose.TabIndex = 2;
            this.btnNoClose.Text = "Don\'t Close";
            this.btnNoClose.UseVisualStyleBackColor = true;
            this.btnNoClose.Click += new System.EventHandler(this.btnNoClose_Click);
            // 
            // chkWarning
            // 
            this.chkWarning.AutoSize = true;
            this.chkWarning.Location = new System.Drawing.Point(28, 37);
            this.chkWarning.Name = "chkWarning";
            this.chkWarning.Size = new System.Drawing.Size(170, 17);
            this.chkWarning.TabIndex = 3;
            this.chkWarning.Text = "Don\'t show this warning again.";
            this.chkWarning.UseVisualStyleBackColor = true;
            this.chkWarning.CheckedChanged += new System.EventHandler(this.chkWarning_CheckedChanged);
            // 
            // chkSave
            // 
            this.chkSave.AutoSize = true;
            this.chkSave.Location = new System.Drawing.Point(28, 60);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(282, 17);
            this.chkSave.TabIndex = 4;
            this.chkSave.Text = "Save threads and load them next time you start YChan";
            this.chkSave.UseVisualStyleBackColor = true;
            this.chkSave.CheckedChanged += new System.EventHandler(this.chkSave_CheckedChanged);
            // 
            // CloseWarn
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 124);
            this.ControlBox = false;
            this.Controls.Add(this.chkSave);
            this.Controls.Add(this.chkWarning);
            this.Controls.Add(this.btnNoClose);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblText);
            this.MaximumSize = new System.Drawing.Size(409, 163);
            this.Name = "CloseWarn";
            this.Text = "Closing YChan";
            this.Load += new System.EventHandler(this.CloseWarn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}