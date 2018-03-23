namespace YChan {
    partial class VInf {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VInf));
            this.rtbChange = new System.Windows.Forms.RichTextBox();
            this.btnVIClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbChange
            // 
            this.rtbChange.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbChange.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbChange.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtbChange.Location = new System.Drawing.Point(12, 12);
            this.rtbChange.Name = "rtbChange";
            this.rtbChange.ReadOnly = true;
            this.rtbChange.Size = new System.Drawing.Size(260, 197);
            this.rtbChange.TabIndex = 0;
            this.rtbChange.Text = resources.GetString("rtbChange.Text");
            // 
            // btnVIClose
            // 
            this.btnVIClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVIClose.Location = new System.Drawing.Point(106, 215);
            this.btnVIClose.Name = "btnVIClose";
            this.btnVIClose.Size = new System.Drawing.Size(75, 23);
            this.btnVIClose.TabIndex = 1;
            this.btnVIClose.Text = "Close";
            this.btnVIClose.UseVisualStyleBackColor = true;
            this.btnVIClose.Click += new System.EventHandler(this.btnVIClose_Click);
            // 
            // VInf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 245);
            this.ControlBox = false;
            this.Controls.Add(this.btnVIClose);
            this.Controls.Add(this.rtbChange);
            this.Name = "VInf";
            this.Text = "Changelog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbChange;
        private System.Windows.Forms.Button btnVIClose;
    }
}