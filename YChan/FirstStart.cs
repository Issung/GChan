using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YChan {
    public partial class FirstStart : Form {
        public FirstStart() {
            InitializeComponent();
        }

        private void btnAClose_Click(object sender, EventArgs e) {
            this.Hide();
            Settings tSettings = new Settings();
            tSettings.ShowDialog();
            this.Close();
        }

        private void rtWelcome_LinkClicked(object sender, LinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
