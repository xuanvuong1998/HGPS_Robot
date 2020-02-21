namespace HGPS_Robot
{
    partial class LessonUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picClose = new System.Windows.Forms.PictureBox();
            this.picBackground = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // picClose
            // 
            this.picClose.Location = new System.Drawing.Point(271, 21);
            this.picClose.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.picClose.Name = "picClose";
            this.picClose.Size = new System.Drawing.Size(175, 170);
            this.picClose.TabIndex = 1;
            this.picClose.TabStop = false;
            this.picClose.Click += new System.EventHandler(this.picClose_Click);
            // 
            // picBackground
            // 
            this.picBackground.Image = global::HGPS_Robot.Properties.Resources.CoddieLessonUI1;
            this.picBackground.Location = new System.Drawing.Point(21, 21);
            this.picBackground.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(200, 256);
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBackground.TabIndex = 0;
            this.picBackground.TabStop = false;
            // 
            // LessonUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1351, 590);
            this.Controls.Add(this.picClose);
            this.Controls.Add(this.picBackground);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "LessonUI";
            this.Text = "LessonUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.PictureBox picClose;
    }
}

