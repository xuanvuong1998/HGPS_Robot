﻿using System;
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
            //this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            SystemUpdateHelper.Start();

            SyncHelper.StatusChanged += SyncHelper_StatusChanged;
            SyncHelper.RobotCommandChanged += SyncHelper_RobotCommandChanged;
        }

        private void SyncHelper_RobotCommandChanged(object sender, RobotCommandEventArgs e)
        {
            e.Command.ProcessCommand();
            /*var list = e.Command.AssessPerformance;
            if (list != null)
            {
                Praise(list);
            }*/
        }

        private void SyncHelper_StatusChanged(object sender, StatusEventArgs e)
        {
            var status = e.Status;
            LessonStatusHelper.LessonStatus = status;

            if (status.LessonState != null)
            {
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
                        LessonHelper.Start(status.LessonName, Convert.ToInt32(status.LessonSlide), voiceName)));
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
                    try
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => LessonHelper.EndLesson()));
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
        }
    }
}
