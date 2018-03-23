namespace YChan {
    partial class FirstStart {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstStart));
            this.btnAClose = new System.Windows.Forms.Button();
            this.rtWelcome = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnAClose
            // 
            this.btnAClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAClose.Location = new System.Drawing.Point(90, 248);
            this.btnAClose.Name = "btnAClose";
            this.btnAClose.Size = new System.Drawing.Size(60, 23);
            this.btnAClose.TabIndex = 2;
            this.btnAClose.Text = "Open Settings";
            this.btnAClose.UseVisualStyleBackColor = true;
            this.btnAClose.Click += new System.EventHandler(this.btnAClose_Click);
            // 
            // rtWelcome
            // 
            this.rtWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtWelcome.BackColor = System.Drawing.SystemColors.Control;
            this.rtWelcome.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtWelcome.Cursor = System.Windows.Forms.Cursors.Default;
            this.rtWelcome.Location = new System.Drawing.Point(12, 12);
            this.rtWelcome.Name = "rtWelcome";
            this.rtWelcome.Size = new System.Drawing.Size(216, 230);
            this.rtWelcome.TabIndex = 3;
            this.rtWelcome.Text = "Welcome to YChan 2.4 (Fork)! \n\nSince the original has been marked as abandoned on" +
    " SourceForge, I decided to fork it and fix a bug that was annoying me.\n\nThat is " +
    "all, have fun.";
            this.rtWelcome.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtWelcome_LinkClicked);
            // 
            // FirstStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 277);
            this.Controls.Add(this.rtWelcome);
            this.Controls.Add(this.btnAClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FirstStart";
            this.Text = " ";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAClose;
        private System.Windows.Forms.RichTextBox rtWelcome;
    }
}