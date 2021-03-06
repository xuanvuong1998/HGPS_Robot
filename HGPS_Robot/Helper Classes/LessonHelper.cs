﻿using SpeechLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public static class LessonHelper
    {
        public static event EventHandler LessonEnded;
        public static int QuestionNumber { get; set; } = 0;
        public static int ChallengeNumber { get; set; } = 0;
        public static int ChallengeNumberTotal { get; set; } = 0;
        public static int CurrentQuizTimeout { get; set; }
        public static string LessonId { get; set; } = "";
        public static string LessonSubject { get; set; } = "";

        // Current slide according to srcipt flow
        public static int CurrentSlideNumber { get; private set; } = 0;

        // Current slide according to the thing display on the projector

        public static int CurrentDisplaySlideNumber { get; set; } = 0;

        public static int LastSlideNumber { get; set; } = 0;
       

        private static Thread _thread = null;
        public static bool PauseRequested { get; set; } = false;

        public static string LessonName { get; set; }

        private static RobotCommands _robotCommands = null;
        private static LessonSpeechUI frmInLesson;
        private static string _lessonName = null;
        private static List<RobotProgSlide> progData = null;

        static LessonHelper() { }

        public static void InitGroupChallenge()
        {
            ChallengeNumber++;
            frmInLesson.ShowMessage("GROUP CHALLENGE"); 
        }

        public static void OfferHint()
        {
            frmInLesson.OfferHint();
        }

        public static async void Start(string lessonName, int startSlideNum, string voiceName)
        {
            LessonName = lessonName;

            //int endSlideNum = FileHelper.GetLessonSlidesNumber(lessonName);
            //LastSlideNumber = endSlideNum;
            //string codePath = FileHelper.BasePath + @"\" + lessonName + @"\code.pptx";
            //progData = PowerpointHelper.GetSlidesData(codePath);
            progData = await WebHelper.GetLessonCommands(lessonName);
            int endSlideNum = progData.Count;

            GlobalFlowControl.Lesson.ResetAll();
            Synthesizer.SelectVoiceByName(GlobalData.GetVoiceNameFromVoiceNumber(voiceName));
            
            RobotActionHelper.MoveDuringLesson();
            frmInLesson = new LessonSpeechUI();

            frmInLesson.ShowForm();

            _lessonName = lessonName;
            QuestionNumber = 0;

            _thread = new Thread(new ThreadStart(() =>
            {                
                for (CurrentSlideNumber = 1; CurrentSlideNumber <= endSlideNum; CurrentSlideNumber++)
                {
                    if (CurrentSlideNumber < startSlideNum)
                    {
                        RobotProgSlide _currentProgSlide = progData[CurrentSlideNumber - 1];

                        var commands = _currentProgSlide.Commands;

                        foreach (var cmd in commands)
                        {
                            if (cmd.Type.ToLower() == "start")
                                if (cmd.Value.ToLower() == "quiz")
                                {
                                    QuestionNumber++; // Synchronize question number flow
                                    // in case if teacher won't start the lesson from begining 
                                }
                        }

                    }
                    else
                    {
                        Wait(1000);
                        while (PauseRequested)
                        {
                            Wait(1000); // Remove busy waiting overloading
                        }

                        CurrentDisplaySlideNumber = CurrentSlideNumber;
                        Debug.WriteLine("Current Slide -----------" + CurrentSlideNumber);

                        RobotProgSlide _currentProgSlide = progData[CurrentSlideNumber - 1];
                        _robotCommands = new RobotCommands(_currentProgSlide.Commands);
                        
                        
                        if (_robotCommands.IsGroupChallengeSlide())
                        {
                            GroupChallengeHelper.DeclareGroupMembers();
                        }else if (_robotCommands.IsGroupCompetitionStartingPoint())
                        {
                            GroupCompetitionHelper.AnnouceGroupCompetition();
                        }

                        LessonStatusHelper.Update(lessonName, CurrentSlideNumber, "started", null, null, null);
                        
                        _robotCommands.OnCommandUpdate += _robotCommands_OnCommandUpdate;
                        _robotCommands.Execute();
                    }
                }
                OnLessonEnded();
            }));
            _thread.Start();
        }

        public static void InsertCommand(string cmdType, string cmdValue)
        {
            _robotCommands.InsertCommand(cmdType, cmdValue);
        }

        public static void InsertNextSlideCommand(string cmdType, string cmdValue)
        {
            int slideNum = CurrentSlideNumber;
            var _nextProgSlide = progData[slideNum]; //next slide since slide 1 starts from index 0
            _nextProgSlide.Commands.Add(new RobotCommand(cmdType, cmdValue));

        }

        /// <summary>
        /// Insert praise after assessing student performance
        /// </summary>
        /// <param name="speech"></param>
        public static void InsertPraise(string speech)
        {
            //this method will insert a 'speak' command into
            //the end of next slide
            int slideNum = CurrentSlideNumber;

            var _nextProgSlide = progData[slideNum]; //next slide since slide 1 starts from index 0
            _nextProgSlide.Commands.Add(new RobotCommand("asking", "0")); // tell server that
            // robot not asking student anymore, please show the result

            _nextProgSlide.Commands.Add(new RobotCommand("speak", speech));
            _nextProgSlide.Commands.Add(new RobotCommand("playaudio", "applause"));

            if (GlobalFlowControl.Lesson.ApproachStudent != null)
            {
                var currentLocation = GlobalFlowControl.Lesson.ApproachStudent;
                string centerLocation = "C" + ((currentLocation[1] - '0' + 1) / 2);
                _nextProgSlide.Commands.Add(new RobotCommand("gountil", centerLocation));
            }
        }

        public static void PauseSpeak()
        {
            if (_robotCommands != null)
            _robotCommands.PauseSpeak();
        }

        public static void ResumeSpeak()
        {
            if (_robotCommands != null)
            _robotCommands.ResumeSpeak();
        }

        /// <summary>
        /// To change the label of the button to 'continue' in teacher panel UI
        /// </summary>

        public static void SendPausedStatusToServer(string stt)
        {
            LessonStatusHelper.LessonStatus.LessonState = stt;

            WebHelper.UpdateStatus(LessonStatusHelper.LessonStatus);
        }

        public static void PauseLesson()
        {
            Debug.WriteLine("Pausing Lesson");
            PauseSpeak();
            PauseRequested = true;
        }

        public static void ResumeLesson()
        {
            Debug.WriteLine("Inside resume function");
            try
            {
                PauseRequested = false;
                _robotCommands.ResumeSpeak();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
        public static void Wait(int miliSec)
        {
            Thread.Sleep(miliSec);
        }
        public static void EndLesson()
        {
            BaseHelper.Go("A0");
            GlobalFlowControl.Lesson.Starting = false;
            RobotActionHelper.StopAll();
            try
            {
                Synthesizer.SetSpeed(0);
                LessonStatusHelper.Update("", null, "ended", null, null, null);
                if (_thread != null && _thread.IsAlive)
                {
                    frmInLesson.CloseForm();
                    if (_robotCommands != null)
                        _robotCommands.StopSpeak();
                    if (_thread != null && _thread.IsAlive)
                    {
                        _thread.Abort();
                    }
                }
            }
            catch
            {
            }
        }
        public static void ForceStop()
        {
            try
            {
                LessonStatusHelper.Update(null, null, null, null, null, null);
                if (_thread != null && _thread.IsAlive)
                {
                    frmInLesson.CloseForm();
                    if (_robotCommands != null)
                        _robotCommands.StopSpeak();
                    _thread.Abort();
                }
            }
            catch
            {
            }
        }
        public static void MediaEnded()
        {
            _robotCommands.MediaPlaying = false;
        }
        private static void _robotCommands_OnCommandUpdate(object sender, CommandEventArgs e)
        {
            string type = e.CommandType;

            if (type == "speak" || type == "speakasync")
            {
                frmInLesson.ShowMessage(e.CommandValue);
            }
        }
        private static void OnLessonEnded()
        {
            EndLesson();
            LessonEnded?.Invoke(null, EventArgs.Empty);
        }

    }
}
