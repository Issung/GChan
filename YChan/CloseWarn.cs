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
            Properties.Settings.Default.warnOnClose = !this.chkWarning.Checked;
            Properties.Settings.Default.Save();
        }

        private void chkSave_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void CloseWarn_Load(object sender, EventArgs e)
        {
            this.chkSave.Checked = Properties.Settings.Default.saveOnClose;
        }
    }
}