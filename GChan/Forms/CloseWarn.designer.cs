namespace GChan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloseWarn));
            this.lblText = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNoClose = new System.Windows.Forms.Button();
            this.chkWarning = new System.Windows.Forms.CheckBox();
            this.chkSave = new System.Windows.Forms.CheckBox();
            this.buttonTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 11);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(406, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Are you sure you want to close GChan? There are still threads/boards being tracke" +
    "d.";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.Location = new System.Drawing.Point(0, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(199, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close Now";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNoClose
            // 
            this.btnNoClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNoClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNoClose.Location = new System.Drawing.Point(203, 0);
            this.btnNoClose.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.btnNoClose.Name = "btnNoClose";
            this.btnNoClose.Size = new System.Drawing.Size(200, 23);
            this.btnNoClose.TabIndex = 2;
            this.btnNoClose.Text = "Don\'t Close";
            this.btnNoClose.UseVisualStyleBackColor = true;
            this.btnNoClose.Click += new System.EventHandler(this.btnNoClose_Click);
            // 
            // chkWarning
            // 
            this.chkWarning.AutoSize = true;
            this.chkWarning.Location = new System.Drawing.Point(20, 62);
            this.chkWarning.Name = "chkWarning";
            this.chkWarning.Size = new System.Drawing.Size(167, 17);
            this.chkWarning.TabIndex = 3;
            this.chkWarning.Text = "Don\'t show this warning again";
            this.chkWarning.UseVisualStyleBackColor = true;
            this.chkWarning.CheckedChanged += new System.EventHandler(this.chkWarning_CheckedChanged);
            // 
            // chkSave
            // 
            this.chkSave.AutoSize = true;
            this.chkSave.Location = new System.Drawing.Point(20, 39);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(283, 17);
            this.chkSave.TabIndex = 4;
            this.chkSave.Text = "Save threads and load them next time you start GChan";
            this.chkSave.UseVisualStyleBackColor = true;
            this.chkSave.CheckedChanged += new System.EventHandler(this.chkSave_CheckedChanged);
            // 
            // buttonTableLayoutPanel
            // 
            this.buttonTableLayoutPanel.ColumnCount = 2;
            this.buttonTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonTableLayoutPanel.Controls.Add(this.btnClose, 0, 0);
            this.buttonTableLayoutPanel.Controls.Add(this.btnNoClose, 1, 0);
            this.buttonTableLayoutPanel.Location = new System.Drawing.Point(15, 96);
            this.buttonTableLayoutPanel.Name = "buttonTableLayoutPanel";
            this.buttonTableLayoutPanel.RowCount = 1;
            this.buttonTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonTableLayoutPanel.Size = new System.Drawing.Size(403, 23);
            this.buttonTableLayoutPanel.TabIndex = 5;
            // 
            // CloseWarn
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 131);
            this.ControlBox = false;
            this.Controls.Add(this.buttonTableLayoutPanel);
            this.Controls.Add(this.chkSave);
            this.Controls.Add(this.chkWarning);
            this.Controls.Add(this.lblText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(451, 170);
            this.MinimumSize = new System.Drawing.Size(451, 170);
            this.Name = "CloseWarn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Closing GChan";
            this.Load += new System.EventHandler(this.CloseWarn_Load);
            this.buttonTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel buttonTableLayoutPanel;
    }
}