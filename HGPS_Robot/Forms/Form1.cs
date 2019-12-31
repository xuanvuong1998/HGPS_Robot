using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            //this.Size = new Size(1000, 400);
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            SystemUpdateHelper.Start();

            //SyncHelper.StatusChanged += SyncHelper_StatusChanged;
            //SyncHelper.RobotCommandChanged += SyncHelper_RobotCommandChanged;
        }

        private void SyncHelper_RobotCommandChanged(object sender, RobotCommandEventArgs e)
        {
            var list = e.Command.AssessPerformance;
            if (list != null)
            {
                Praise(list);
            }
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
            else if (status.LessonState == "end")
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

        private void Praise(List<StudentHistoryDTO> list)
        {
            var rdm = new Random();
            var rdmNum = rdm.Next(1, 4); // generate random number 1-3

            var speech = "";
            var topStudents = new Dictionary<string, int>();

            switch (rdmNum)
            {
                case 1:
                    var performance = StudentsPerformanceHelper.GetSummary(list);
                    speech = "The average score for this class currently at " + performance.AverageScore.ToString();
                    speech += " Well done!";
                    break;

                case 2:
                    var numOfFullScore = StudentsPerformanceHelper.GetNumOfFullScore(list);
                    if (numOfFullScore != 0)
                    {
                        speech = $"We have {numOfFullScore.ToString()} students with full score!";
                            speech += " Great job!";
                    }
                    else
                    {
                        topStudents = StudentsPerformanceHelper.GetTopStudents(list);
                        if (topStudents.Count == 1)
                        {
                            speech = $"Currently the top students for this class is {topStudents.FirstOrDefault().Key} with" +
                                     $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                            speech += " The rest of you please try your best!"; ;
                        }
                        else if (topStudents.Count <= 5)
                        {
                            foreach (var stud in topStudents)
                            {
                                speech += stud.Key;
                                speech += ", ";
                            }
                            speech += " are currently the top students in this class";
                            speech += $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                            speech += " Great job everyone!";
                        }
                    }
                    break;

                case 3:
                    topStudents = StudentsPerformanceHelper.GetTopStudents(list);
                    if (topStudents.Count == 1)
                    {
                        speech = $"Currently the top students for this class is {topStudents.FirstOrDefault().Key} with" +
                                 $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                        speech += " The rest of you please try your best!"; ;
                    }
                    else if (topStudents.Count <= 5)
                    {
                        foreach (var stud in topStudents)
                        {
                            speech += stud.Key;
                            speech += ", ";
                        }
                        speech += " are currently the top students in this class";
                        speech += $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                        speech += " Great job everyone!";
                    }
                    break;
            }

            //append a speak command using speech
            LessonHelper.InsertCommand("speak", speech);
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
