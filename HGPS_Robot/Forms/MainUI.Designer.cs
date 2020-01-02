namespace HGPS_Robot
{
    partial class MainUI
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
            this.picTalk = new System.Windows.Forms.PictureBox();
            this.picLesson = new System.Windows.Forms.PictureBox();
            this.picBackground = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picTalk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLesson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // picTalk
            // 
            this.picTalk.Location = new System.Drawing.Point(193, 101);
            this.picTalk.Name = "picTalk";
            this.picTalk.Size = new System.Drawing.Size(170, 50);
            this.picTalk.TabIndex = 2;
            this.picTalk.TabStop = false;
            this.picTalk.Click += new System.EventHandler(this.picTalk_Click);
            // 
            // picLesson
            // 
            this.picLesson.Location = new System.Drawing.Point(193, 36);
            this.picLesson.Name = "picLesson";
            this.picLesson.Size = new System.Drawing.Size(170, 50);
            this.picLesson.TabIndex = 1;
            this.picLesson.TabStop = false;
            this.picLesson.Click += new System.EventHandler(this.picLesson_Click);
            // 
            // picBackground
            // 
            this.picBackground.Image = global::HGPS_Robot.Properties.Resources.CoddieMainUI;
            this.picBackground.Location = new System.Drawing.Point(44, 36);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(111, 150);
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBackground.TabIndex = 0;
            this.picBackground.TabStop = false;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 450);
            this.Controls.Add(this.picTalk);
            this.Controls.Add(this.picLesson);
            this.Controls.Add(this.picBackground);
            this.Name = "MainUI";
            this.Text = "Main";
            this.Shown += new System.EventHandler(this.Form3_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainUI_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picTalk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLesson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.PictureBox picLesson;
        private System.Windows.Forms.PictureBox picTalk;
    }
}