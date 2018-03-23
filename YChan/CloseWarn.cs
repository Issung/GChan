using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YChan
{
  public class CloseWarn : Form
  {
    public bool closeit = true;
    private IContainer components;
    private Label lblText;
    private Button btnClose;
    private Button btnNoClose;
    private CheckBox chkWarning;
    private CheckBox chkSave;

    public CloseWarn()
    {
      this.InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btnNoClose_Click(object sender, EventArgs e)
    {
      this.closeit = false;
      this.Close();
    }

    private void chkWarning_CheckedChanged(object sender, EventArgs e)
    {
      General.setSettings(General.path, General.timer, General.loadHTML, General.saveOnClose, General.minimizeToTray, !this.chkWarning.Checked);
    }

    private void chkSave_CheckedChanged(object sender, EventArgs e)
    {
      General.setSettings(General.path, General.timer, General.loadHTML, this.chkSave.Checked, General.minimizeToTray, General.warnOnClose);
    }

    private void CloseWarn_Load(object sender, EventArgs e)
    {
      this.chkSave.Checked = General.saveOnClose;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

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
            this.lblText.Location = new System.Drawing.Point(37, 20);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(327, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Are you sure you want to close YChan? There are still open threads!";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 92);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close ";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNoClose
            // 
            this.btnNoClose.Location = new System.Drawing.Point(289, 92);
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
            this.chkWarning.Location = new System.Drawing.Point(40, 55);
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
            this.chkSave.Location = new System.Drawing.Point(40, 69);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(282, 17);
            this.chkSave.TabIndex = 4;
            this.chkSave.Text = "Save threads and load them next time you start YChan";
            this.chkSave.UseVisualStyleBackColor = true;
            this.chkSave.CheckedChanged += new System.EventHandler(this.chkSave_CheckedChanged);
            // 
            // CloseWarn
            // 
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
  }
}
