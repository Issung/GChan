using System;
using System.Windows.Forms;

namespace GChan
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
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNoClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkWarning_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.warnOnClose = !chkWarning.Checked;
            Properties.Settings.Default.Save();
        }

        private void chkSave_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.saveOnClose = chkSave.Checked;
            Properties.Settings.Default.Save();
        }

        private void CloseWarn_Load(object sender, EventArgs e)
        {
            chkSave.Checked = Properties.Settings.Default.saveOnClose;
            chkWarning.Checked = !Properties.Settings.Default.warnOnClose;
        }
    }
}