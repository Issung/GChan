using GChan.Properties;
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
            Settings.Default.WarnOnClose = !chkWarning.Checked;
            Settings.Default.Save();
        }

        private void chkSave_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SaveListsOnClose = chkSave.Checked;
            Settings.Default.Save();
        }

        private void CloseWarn_Load(object sender, EventArgs e)
        {
            chkSave.Checked = Settings.Default.SaveListsOnClose;
            chkWarning.Checked = !Settings.Default.WarnOnClose;
        }
    }
}