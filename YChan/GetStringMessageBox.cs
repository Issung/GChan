using System;
using System.Windows.Forms;

namespace GChan
{
    public partial class GetStringMessageBox : Form
    {
        /// <summary>
        /// Get the user's entry into the textbox.
        /// </summary>
        public string UserEntry => entryTextBox.Text;

        public GetStringMessageBox(string defaultText = "")
        {
            InitializeComponent();

            entryTextBox.Text = defaultText;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void entryTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                okButton_Click(null, null);
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                cancelButton_Click(null, null);
            }
        }
    }
}
