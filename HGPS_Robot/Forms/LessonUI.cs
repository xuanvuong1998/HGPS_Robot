using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HGPS_Robot
{
    public partial class LessonUI : Form
    {
        public LessonUI()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            picBackground.Controls.Add(picClose);

            picClose.Location = new Point(300, 820);
            picClose.Size = new Size(170, 200);
            picClose.BackColor = Color.Transparent;

            SyncHelper.StatusChanged += SyncHelper_StatusChanged;
            SyncHelper.RobotCommandChanged += SyncHelper_RobotCommandChanged;

        }

        private void SyncHelper_RobotCommandChanged(object sender, RobotCommandEventArgs e)
        {
            e.Command.ProcessCommand();
        }

        private void SyncHelper_StatusChanged(object sender, StatusEventArgs e)
        {
            var status = e.Status;
            LessonStatusHelper.LessonStatus = status;
            
            if (status.LessonState != null)
            {                
                if (status.LessonState.Contains("starting") && 
                    GlobalFlowControl.Lesson.Starting == false)
                {
                    this.Invoke(new Action(() => { picClose.Visible = false;  }));
                    var lessonStt = status.LessonState.Split('-');
                    var teacherId = lessonStt[1];
                    string voiceName = "Voice 1";
                    if (lessonStt.Length > 2)
                    {
                        voiceName = lessonStt[2];
                    }
                    string className = null;

                    if (lessonStt.Length > 3)
                    {
                        className = lessonStt[3];
                    }

                    GlobalFlowControl.Lesson.Name = status.LessonName;

                    //LessonHelper.SaveLessonHistory(status.LessonName, teacherId, className); will save internally in server
                    LessonHelper.LessonId = status.LessonId;

                    if (InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        LessonHelper.Start(status.LessonName, Convert.ToInt32(status.LessonSlide), voiceName)));
                        return;
                    }
                    
                }
                else if (status.LessonState == "pause" || status.LessonState == "asking")
                {
                    Debug.WriteLine("PAUSING");
                    LessonHelper.PauseLesson();
                }
                else if (status.LessonState == "continue")
                {
                    Debug.WriteLine("Continue from server");
                    LessonHelper.ResumeLesson();
                }
                else if (status.LessonState == "end")
                {
                    GlobalFlowControl.Lesson.StartingQuiz = false;
                    try
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => { 
                                picClose.Visible = true;
                                LessonHelper.EndLesson();
                            }));
                            return;
                        }
                    }
                    catch { }
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
        }

        private void LessonHelper_LessonEnded(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            LessonHelper.ForceStop();
            SyncHelper.StatusChanged -= SyncHelper_StatusChanged;
            SyncHelper.RobotCommandChanged -= SyncHelper_RobotCommandChanged;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
