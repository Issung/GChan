using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YChan {
    public partial class About : Form {
        public About() {
            InitializeComponent();
        }

        private void btnAClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void rtAbout_LinkClicked(object sender, LinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
