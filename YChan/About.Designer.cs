namespace YChan {
    partial class About {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.btnAClose = new System.Windows.Forms.Button();
            this.rtAbout = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnAClose
            // 
            this.btnAClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAClose.Location = new System.Drawing.Point(121, 186);
            this.btnAClose.Name = "btnAClose";
            this.btnAClose.Size = new System.Drawing.Size(127, 23);
            this.btnAClose.TabIndex = 0;
            this.btnAClose.Text = "Close";
            this.btnAClose.UseVisualStyleBackColor = true;
            this.btnAClose.Click += new System.EventHandler(this.btnAClose_Click);
            // 
            // rtAbout
            // 
            this.rtAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtAbout.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtAbout.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rtAbout.Location = new System.Drawing.Point(12, 12);
            this.rtAbout.Name = "rtAbout";
            this.rtAbout.ReadOnly = true;
            this.rtAbout.Size = new System.Drawing.Size(236, 168);
            this.rtAbout.TabIndex = 1;
            this.rtAbout.Text = resources.GetString("rtAbout.Text");
            this.rtAbout.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtAbout_LinkClicked);
            // 
            // About
            // 
            this.AcceptButton = this.btnAClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 212);
            this.ControlBox = false;
            this.Controls.Add(this.rtAbout);
            this.Controls.Add(this.btnAClose);
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAClose;
        private System.Windows.Forms.RichTextBox rtAbout;
    }
}