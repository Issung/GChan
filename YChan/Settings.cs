using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YChan {
    public partial class Settings : Form {
        public Settings() {
            InitializeComponent();
        }

        private void btnSSave_Click(object sender, EventArgs e) {
            if((edtPath.Text != "") && (General.IsDigitsOnly(edtTimer.Text))) {
                if(int.Parse(edtTimer.Text) < 5){
                    MessageBox.Show("Timer has to be higher than 5 seconds");
                } else {
                    General.setSettings(edtPath.Text, int.Parse(edtTimer.Text)*1000, chkHTML.Checked, chkSave.Checked, chkTray.Checked);
                    
                    this.Close();
                }              
            } else {
                MessageBox.Show("Check value for timer and path");
            } 
        }

        private void btnSCan_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void edtPath_Click(object sender, EventArgs e) {
            FolderBrowserDialog FolD = new FolderBrowserDialog();
            FolD.Description = "Select Folder";
            FolD.SelectedPath=@"C:\";       // Vorgabe Pfad (und danach der gewählte Pfad)
            DialogResult dRes = FolD.ShowDialog(this);
            if(dRes == DialogResult.OK) {
                edtPath.Text = FolD.SelectedPath;
            } else
                MessageBox.Show("Aborted");
        }

        private void Settings_Shown(object sender, EventArgs e) {
            edtPath.Text    = General.path;
            edtTimer.Text   = (General.timer / 1000).ToString();
            chkHTML.Checked = General.loadHTML;
            chkSave.Checked = General.saveOnClose;
            chkTray.Checked = General.minimizeToTray;
        }

    }
}
