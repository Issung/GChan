using System;
using System.Windows.Forms;

namespace GChan
{
    public partial class Changelog : Form
    {
        public Changelog()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}