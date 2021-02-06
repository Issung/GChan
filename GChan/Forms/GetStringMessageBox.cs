using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GChan
{
    public partial class GetStringMessageBox : Form
    {
        /// <summary>
        /// Get the user's entry into the textbox.
        /// </summary>
        public string UserEntry => entryTextBox.Text;

        /// <summary>
        /// Characters not allowed in paths or filenames, for when renaming the thread folder to the subject.
        /// </summary>
        char[] disallowedCharacters = Path.GetInvalidFileNameChars();

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

        private void entryTextBox_TextChanged(object sender, EventArgs e)
        {
            int prevSelectionStart = entryTextBox.SelectionStart;
            int prevSelectionLength = entryTextBox.SelectionLength;

            string originalText = entryTextBox.Text;
            string cleanedText = Utils.RemoveCharactersFromString(entryTextBox.Text, disallowedCharacters);

            entryTextBox.Text = cleanedText;

            if (cleanedText != originalText)
            { 
                entryTextBox.SelectionStart = Math.Max(prevSelectionStart - 1, 0);
                entryTextBox.SelectionLength = Math.Max(prevSelectionLength - 1, 0);
            }
        }
    }
}