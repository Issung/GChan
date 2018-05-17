using System;
using System.Windows.Forms;

namespace YChan
{
    public partial class CloseWarn : Form
    {
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
    }
}