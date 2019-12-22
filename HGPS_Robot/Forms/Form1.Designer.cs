namespace HGPS_Robot
{
    partial class Form1
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
            this.btnStart = new System.Windows.Forms.Button();
            this.cmbLessons = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSlideNumber = new System.Windows.Forms.ComboBox();
            this.cmbVoiceGender = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(358, 228);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(254, 93);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cmbLessons
            // 
            this.cmbLessons.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.85714F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLessons.FormattingEnabled = true;
            this.cmbLessons.Location = new System.Drawing.Point(221, 21);
            this.cmbLessons.Margin = new System.Windows.Forms.Padding(2);
            this.cmbLessons.Name = "cmbLessons";
            this.cmbLessons.Size = new System.Drawing.Size(697, 50);
            this.cmbLessons.TabIndex = 7;
            this.cmbLessons.SelectedIndexChanged += new System.EventHandler(this.cmbLessons_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.85714F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 44);
            this.label1.TabIndex = 8;
            this.label1.Text = "Lessons:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.85714F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 88);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(453, 44);
            this.label2.TabIndex = 10;
            this.label2.Text = "Start From Slide Number:";
            // 
            // cmbSlideNumber
            // 
            this.cmbSlideNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.85714F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSlideNumber.FormattingEnabled = true;
            this.cmbSlideNumber.Location = new System.Drawing.Point(493, 84);
            this.cmbSlideNumber.Margin = new System.Windows.Forms.Padding(2);
            this.cmbSlideNumber.Name = "cmbSlideNumber";
            this.cmbSlideNumber.Size = new System.Drawing.Size(425, 50);
            this.cmbSlideNumber.TabIndex = 9;
            // 
            // cmbVoiceGender
            // 
            this.cmbVoiceGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbVoiceGender.FormattingEnabled = true;
            this.cmbVoiceGender.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.cmbVoiceGender.Location = new System.Drawing.Point(279, 149);
            this.cmbVoiceGender.Name = "cmbVoiceGender";
            this.cmbVoiceGender.Size = new System.Drawing.Size(639, 50);
            this.cmbVoiceGender.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.85714F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 155);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(263, 44);
            this.label3.TabIndex = 12;
            this.label3.Text = "Voice Gender:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(952, 337);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbVoiceGender);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSlideNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbLessons);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Start Lesson";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ComboBox cmbLessons;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSlideNumber;
        private System.Windows.Forms.ComboBox cmbVoiceGender;
        private System.Windows.Forms.Label label3;
    }
}

