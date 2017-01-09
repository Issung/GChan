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
      this.lblText = new Label();
      this.btnClose = new Button();
      this.btnNoClose = new Button();
      this.chkWarning = new CheckBox();
      this.chkSave = new CheckBox();
      this.SuspendLayout();
      this.lblText.AutoSize = true;
      this.lblText.Location = new Point(37, 20);
      this.lblText.Name = "lblText";
      this.lblText.Size = new Size(327, 13);
      this.lblText.TabIndex = 0;
      this.lblText.Text = "Are you sure you want to close YChan? There are still open threads!";
      this.btnClose.Location = new Point(12, 92);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close ";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new EventHandler(this.btnClose_Click);
      this.btnNoClose.Location = new Point(289, 92);
      this.btnNoClose.Name = "btnNoClose";
      this.btnNoClose.Size = new Size(75, 23);
      this.btnNoClose.TabIndex = 2;
      this.btnNoClose.Text = "Don't Close";
      this.btnNoClose.UseVisualStyleBackColor = true;
      this.btnNoClose.Click += new EventHandler(this.btnNoClose_Click);
      this.chkWarning.AutoSize = true;
      this.chkWarning.Location = new Point(40, 55);
      this.chkWarning.Name = "chkWarning";
      this.chkWarning.Size = new Size(170, 17);
      this.chkWarning.TabIndex = 3;
      this.chkWarning.Text = "Don't show this warning again.";
      this.chkWarning.UseVisualStyleBackColor = true;
      this.chkWarning.CheckedChanged += new EventHandler(this.chkWarning_CheckedChanged);
      this.chkSave.AutoSize = true;
      this.chkSave.Location = new Point(40, 69);
      this.chkSave.Name = "chkSave";
      this.chkSave.Size = new Size(282, 17);
      this.chkSave.TabIndex = 4;
      this.chkSave.Text = "Save threads and load them next time you start YChan";
      this.chkSave.UseVisualStyleBackColor = true;
      this.chkSave.CheckedChanged += new EventHandler(this.chkSave_CheckedChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(393, 124);
      this.ControlBox = false;
      this.Controls.Add((Control) this.chkSave);
      this.Controls.Add((Control) this.chkWarning);
      this.Controls.Add((Control) this.btnNoClose);
      this.Controls.Add((Control) this.btnClose);
      this.Controls.Add((Control) this.lblText);
      this.Name = "CloseWarn";
      this.Text = "Closing YChan";
      this.Load += new EventHandler(this.CloseWarn_Load);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
