using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SpeechLibrary;

namespace HGPS_Robot
{
    public static class LessonHelper
    {
        public static event EventHandler LessonEnded;
        public static int QuestionNumber { get; set; } = 0;
        public static string LessonId { get; set; } = "";
        public static int CurrentSlideNumber { get; private set; } = 0;

        private static Thread _thread = null;
        private static RobotCommands _robotCommands = null;
        private static LessonSpeechUI form2;
        private static string _lessonName = null;
        private static List<RobotProgSlide> progData = null;
        static LessonHelper() { }

        public static void Start(string lessonName, int startSlideNum, string voiceName)
        {
            GlobalFlowControl.Lesson.Starting = true;
            //UpperBodyHelper.MoveRandomlyAllMotors();
            form2 = new LessonSpeechUI();
            Synthesizer.SetSpeed(1);

            StudentPositionHelper.LoadTablesInfo();
            form2.ShowForm();

            _lessonName = lessonName;
            QuestionNumber = 0;

            _thread = new Thread(new ThreadStart(() =>
            {
                int endSlideNum = FileHelper.GetLessonSlidesNumber(lessonName);
                string codePath = FileHelper.BasePath + @"\" + lessonName + @"\code.pptx";
                progData = PowerpointHelper.GetSlidesData(codePath);

                for (CurrentSlideNumber = startSlideNum; CurrentSlideNumber <= endSlideNum; CurrentSlideNumber++)
                {
                    Debug.WriteLine("Current Slide -----------" + CurrentSlideNumber);
                    LessonStatusHelper.Update(lessonName, CurrentSlideNumber, "started", null, null, null);
                    RobotProgSlide _currentProgSlide = progData[CurrentSlideNumber - 1];
                    _robotCommands = new RobotCommands(_currentProgSlide.Commands);
                    _robotCommands.OnCommandUpdate += _robotCommands_OnCommandUpdate;
                    _robotCommands.Execute();
                }
                OnLessonEnded();
            }));
            _thread.Start();
        }

        public static void SaveLessonHistory(string lessonName, string teacherId, string className)
        {
            var lessonHistory = new LessonHistory
            {
                Lesson_name = lessonName,
                DateTime = DateTime.Now,
                Teacher_id = teacherId,
                Class_name = className
            };

            WebHelper.AddLessonHistory(lessonHistory);

        }

        public static void InsertCommand(string cmdType, string cmdValue)
        {
            _robotCommands.InsertCommand(cmdType, cmdValue);
        }



        public static void InsertPraise(string speech)
        {
            //this method will insert a 'speak' command into
            //the end of next slide
            int slideNum = CurrentSlideNumber;
            
            var _nextProgSlide = progData[slideNum]; //next slide since slide 1 starts from index 0
            _nextProgSlide.Commands.Add(new RobotCommand("asking", "0"));
            _nextProgSlide.Commands.Add(new RobotCommand("speak", speech));
            _nextProgSlide.Commands.Add(new RobotCommand("playaudio", "applause.wav"));

            if (GlobalFlowControl.Lesson.ApproachStudent != null)
            {
                var currentLocation = GlobalFlowControl.Lesson.ApproachStudent;
                string centerLocation = "C" + ((currentLocation[1] - '0' + 1) / 2) ;
                _nextProgSlide.Commands.Add(new RobotCommand("gountil", centerLocation));
            }
        }

        [Obsolete]
        public static void Pause()
        {
            _robotCommands.PauseSpeak();
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Suspend();
            }
            
        }

        [Obsolete]
        public static void Resume()
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Resume();
            }                
            _robotCommands.ResumeSpeak();
        }
        public static void EndLesson()
        {
            BaseHelper.Go("A0");
            GlobalFlowControl.Lesson.Starting = false;
            try
            {
                Synthesizer.SetSpeed(0);
                LessonStatusHelper.Update("", null, "ended", null, null, null);
                if (_thread != null && _thread.IsAlive)
                {
                    form2.CloseForm();
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
                    form2.CloseForm();
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
                form2.ShowMessage(e.CommandValue);
            }
        }
        private static void OnLessonEnded()
        {
            EndLesson();
            LessonEnded?.Invoke(null, EventArgs.Empty);
        }


    }
}
