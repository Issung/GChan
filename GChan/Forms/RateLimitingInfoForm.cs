using System;
using System.Windows.Forms;

namespace GChan.Forms
{
    public partial class RateLimitingInfoForm : Form
    {
        public RateLimitingInfoForm()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
