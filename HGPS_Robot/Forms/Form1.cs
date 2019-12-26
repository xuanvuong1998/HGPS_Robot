using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HGPS_Robot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Size = new Size(1000, 400);
            this.CenterToScreen();

            var lessonNames = FileHelper.GetLessons();
            cmbLessons.DataSource = lessonNames;
            cmbVoiceGender.SelectedIndex = 0;

            SystemUpdateHelper.Start();

            SyncHelper.StatusChanged += SyncHelper_StatusChanged;
        }

        private void SyncHelper_StatusChanged(object sender, StatusEventArgs e)
        {
            var status = e.Status;
            LessonStatusHelper.LessonStatus = status;

            if (status.LessonState.Contains("starting"))
            {
                var teacherId = status.LessonState.Split('-')[1];    
                var voiceName = status.LessonState.Split('-')[2];    
                
                LessonHelper.SaveLessonHistory(status.LessonName, teacherId);
                LessonHelper.LessonId = status.LessonId;

                //this.Invoke(new MethodInvoker(() => this.Hide()));

                if (InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    LessonHelper.Start(status.LessonName, Convert.ToInt32(status.LessonSlide),voiceName)));
                    return;
                }

            }
            else if (status.LessonState == "pause")
            {
                LessonHelper.Pause();
            }
            else if (status.LessonState == "continue")
            {
                LessonHelper.Resume();
            }
            else if (status.LessonState == "ended")
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => LessonHelper.EndLesson()));
                    return;
                }
            }

            if (status.MediaCompleted != null)
            {
                if (status.MediaCompleted == "true")
                {
                    LessonHelper.MediaEnded();
                    status.MediaCompleted = null;
                    WebHelper.UpdateStatus(status);
                }
            }
        }

        private void cmbLessons_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSlideNumber.Items.Clear();
            var numOfSlides = FileHelper.GetLessonSlidesNumber(cmbLessons.Text);
            if (numOfSlides > 0)
            {
                for (int i = 1; i <= numOfSlides; i++)
                {
                    cmbSlideNumber.Items.Add(i);
                }
                cmbSlideNumber.SelectedIndex = 0;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            LessonHelper.Start(cmbLessons.Text, Convert.ToInt32(cmbSlideNumber.Text), cmbVoiceGender.Text);
            //LessonHelper.LessonEnded += LessonHelper_LessonEnded;
        }

        private void LessonHelper_LessonEnded(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            LessonHelper.ForceStop();
        }
    }
}
