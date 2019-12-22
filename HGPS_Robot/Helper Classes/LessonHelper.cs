using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HGPS_Robot
{
    public static class LessonHelper
    {
        public static event EventHandler LessonEnded;
        public static int QuestionNumber { get; set; } = 0;
        private static Thread _thread = null;
        private static RobotCommands _robotCommands = null;
        private static Form2 form2;
        private static string _lessonName = null;
        static LessonHelper() { }
        public static void Start(string lessonName, int startSlideNum, string voiceGender)
        {
            form2 = new Form2();
            form2.ShowForm();
            _lessonName = lessonName;

            _thread = new Thread(new ThreadStart(() =>
            {
                int endSlideNum = FileHelper.GetLessonSlidesNumber(lessonName);
                string codePath = FileHelper.BasePath + @"\" + lessonName + @"\code.pptx";
                var progData = PowerpointHelper.GetSlidesData(codePath);

                for (int i = startSlideNum; i <= endSlideNum; i++)
                {
                    LessonStatusHelper.Update(lessonName, i, "started", null, null, null);
                    RobotProgSlide _currentProgSlide = progData[i-1];
                    _robotCommands = new RobotCommands(_currentProgSlide.Commands);
                    _robotCommands.SetVoiceGender(voiceGender);
                    _robotCommands.OnCommandUpdate += _robotCommands_OnCommandUpdate;
                    _robotCommands.Execute();
                }
                OnLessonEnded();
            }));
            _thread.Start();
        }

        public static void SaveLessonHistory(string lessonName, string teacherId)
        {
            var lessonHistory = new LessonHistory
            {
                LessonName = lessonName,
                DateTime = DateTime.Now,
                TeacherId = teacherId                
            };

            WebHelper.AddLessonHistory(lessonHistory);
            
        }

        [Obsolete]
        public static void Pause()
        {
            _robotCommands.PauseSpeak();
            _thread.Suspend();
        }

        [Obsolete]
        public static void Resume()
        {
            _thread.Resume();
            _robotCommands.ResumeSpeak();
        }
        public static void EndLesson()
        {
            try
            {
                //LessonStatusHelper.Update(lessonName, null, "ended", null, null, null);
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
