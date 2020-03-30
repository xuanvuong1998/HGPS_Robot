namespace HGPS_Robot
{
    partial class LessonSpeechUI
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.btnHideHint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial Rounded MT Bold", 52F, System.Drawing.FontStyle.Italic);
            this.lblMessage.Location = new System.Drawing.Point(391, 71);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(434, 177);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMessage.Click += new System.EventHandler(this.lblMessage_Click);
            // 
            // picBackground
            // 
            this.picBackground.Image = global::HGPS_Robot.Properties.Resources.LessonInProgress;
            this.picBackground.Location = new System.Drawing.Point(56, 71);
            this.picBackground.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(290, 196);
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBackground.TabIndex = 1;
            this.picBackground.TabStop = false;
            this.picBackground.Click += new System.EventHandler(this.picBackground_Click);
            // 
            // btnHideHint
            // 
            this.btnHideHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 17.875F);
            this.btnHideHint.Location = new System.Drawing.Point(764, 507);
            this.btnHideHint.Name = "btnHideHint";
            this.btnHideHint.Size = new System.Drawing.Size(370, 164);
            this.btnHideHint.TabIndex = 2;
            this.btnHideHint.Text = "OK! THANKS";
            this.btnHideHint.UseVisualStyleBackColor = true;
            this.btnHideHint.Visible = false;
            this.btnHideHint.Click += new System.EventHandler(this.btnHideHint_Click);
            // 
            // LessonSpeechUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1786, 1704);
            this.Controls.Add(this.btnHideHint);
            this.Controls.Add(this.picBackground);
            this.Controls.Add(this.lblMessage);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LessonSpeechUI";
            this.Text = "LessonSpeech";
            this.Shown += new System.EventHandler(this.Form2_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.Button btnHideHint;
    }
}