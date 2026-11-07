using System;
using System.Windows.Forms;

namespace GChan
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            Text = $"About {Program.NAME}";
            labelProductName.Text = Program.NAME;
            labelVersion.Text = $"Version {Program.VERSION}";
        }

        private void richTextBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Utils.OpenWebpage(e.LinkText);
        }

        private void richTextBoxDescription_TextChanged(object sender, EventArgs e)
        {

        }
    }
}