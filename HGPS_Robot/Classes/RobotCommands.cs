using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Media;
using System.Threading.Tasks;
using System.Configuration;
using SpeechLibrary;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    public class RobotCommands
    {
        public bool MediaPlaying { get; set; } = false;
        public RobotCommand CurrentCommand { get; private set; } = null;

        public delegate void CommandUpdateHandler(object sender, CommandEventArgs e);
        public event CommandUpdateHandler OnCommandUpdate;
        private List<RobotCommand> _commands;
        private SoundPlayer _soundPlayer = new SoundPlayer();        
        private const int QUIZ_BUFFER_SECONDS = 2;

        private Timer quizTimer = new Timer();
        private int quizTimerTick, quizTime;

        
        public RobotCommands(List<RobotCommand> commands)
        {
            _commands = commands;
        }


        public void PauseSpeak()
        {            
            Synthesizer.Pause();
        }
        public void ResumeSpeak()
        {
            Synthesizer.Resume();
        }
        public void StopSpeak()
        {
            Synthesizer.StopSpeaking();
        }
        public void Execute()
        {
            if (_commands != null)
            {
                StopSpeak();
                CommandHandler();
            }
            else
            {
                MessageBox.Show("Error: Commands not found!");
            }
        }

        public void InsertCommand(string cmdType, string cmdValue)
        {
            var robotCommand = new RobotCommand(cmdType, cmdValue);
            //_commands.Insert(commandIteration + 1, robotCommand);
            _commands.Add(robotCommand);
        }

    

        int commandIteration = 0;
        private void CommandHandler()
        {
            var quiz = new Quiz();

            for (commandIteration = 0; commandIteration < _commands.Count; commandIteration++)
            {
                while (LessonHelper.PauseRequested)
                {
                    Thread.Sleep(500);
                    Debug.WriteLine("Busy waiting");
                    //LessonHelper.Pause();
                }

                CurrentCommand = _commands[commandIteration];
                var cmd = CurrentCommand.Type;
                var val = CurrentCommand.Value;
                
                UpdateCommand(cmd.ToLower(), val);
                
                Debug.WriteLine(cmd + "/" + val); 
                
                
                switch (cmd.ToLower())
                {
                    case "speak":
                        Speak(val);
                        break;

                    case "speakasync":
                        SpeakAsync(val);
                        break;

                    case "playaudio":
                        AudioHelper.PlayAudio(val);
                        break;
                    case "myspeech":
                        MySpeech(val);
                        break;

                    case "myspeechasync":
                        MySpeechAsync(val);
                        break;

                    case "move":
                        Move(val);
                        break;

                    case "wait":
                        Wait(Convert.ToInt32(val));
                        break;

                    case "playmedia":
                        PlayMedia(val);
                        break;

                    case "quizformat":
                        quiz.QuizFormat = val;
                        break;

                    case "answer":
                        quiz.Answer = val;
                        break;

                    case "timeout":
                        quiz.TimeOut = val;
                        break;

                    case "gesture":
                        UpperBodyHelper.DoGestures(val);
                        break;

                    case "start":
                        if (val.ToLower() == "quiz")
                        {
                            LessonHelper.QuestionNumber += 1;
                            quiz.QuestionNumber = LessonHelper.QuestionNumber;
                            StartQuiz(quiz);

                            LessonStatusHelper.LessonStatus.CurQuiz = null;
                            //Wait(Convert.ToInt32(quiz.TimeOut) * 1000 + QUIZ_BUFFER_SECONDS * 1000);                                                        

                            Wait(QUIZ_BUFFER_SECONDS * 800);
                            // This QUIZ BUFFER TO give extra time for all student submit the answer

                            Debug.WriteLine("Stopping quiz");
                            StopQuiz();
                           
                            Wait(QUIZ_BUFFER_SECONDS * 1000);

                            var now = DateTime.Now;
                            
                            Debug.WriteLine(now + " : QUIZ COMPLETED");

                            // This QUIZ BUFFER TO give extra time for teacher send student result to robot
                        }
                        else if (val.ToLower() == "emotion-survey")
                        {
                            TakeEmotionSurvey();
                        }
                        break;
                    case "gountil":
                        BaseHelper.GoUntilReachedGoalOrCanceled(val);
                        break;

                    case "asking":
                        var status = LessonStatusHelper.LessonStatus;
                        if (Convert.ToInt32(val) == 1)
                        {
                            status.LessonState = "asking";
                        }
                        else
                        {
                            status.LessonState = "notAsking";
                        }
                        WebHelper.UpdateStatus(status);
                        Wait(1500);
                        break;
                    
                    default:
                        //MessageBox.Show($"Unknown Type: {cmd}");
                        break;
                }
            }
        }
        private void Speak(string text)
        {
            Synthesizer.Speak(text);
        }
        private void SpeakAsync(string text)
        {
            Synthesizer.SpeakAsync(text);
        }
        private void MySpeech(string file)
        {
            var status = LessonStatusHelper.LessonStatus;
            var fileLocation = FileHelper.BasePath + $@"{status.LessonName}\Speech\{file}";
            _soundPlayer.SoundLocation = fileLocation;
            _soundPlayer.PlaySync();
        }

        private void MySpeechAsync(string file)
        {
            var status = LessonStatusHelper.LessonStatus;
            var fileLocation = FileHelper.BasePath + $@"{status.LessonName}\Speech\{file}";
            _soundPlayer.SoundLocation = fileLocation;
            _soundPlayer.Play();
        }

        private void Move(string action_name)
        {
            //MessageBox.Show($"Move: {action_name}");
        }
        private void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        private void TakeEmotionSurvey()
        {
            Debug.WriteLine("Request server to take student's emotion survey");

            var status = LessonStatusHelper.LessonStatus;

            var previousLessonStatus = status.LessonState;
            status.LessonState = "TakeEmotionSurvey";

            WebHelper.UpdateStatus(status);

            while (!GlobalFlowControl.Lesson.StudentFeedbackReceived) ;

            LessonStatusHelper.LessonStatus.LessonState = previousLessonStatus;
            Wait(10000); // Remove thread conflict
            
            Debug.WriteLine("Received student feedback");

            GlobalFlowControl.Lesson.StudentFeedbackReceived = false; // Reset for the next feedback
        }

        private void PlayMedia(string url)
        {
            MediaPlaying = true;
            var status = LessonStatusHelper.LessonStatus;
            status.MediaPath = url;
            WebHelper.UpdateStatus(status);

            while (MediaPlaying) ; //wait for media to finish playing
            Thread.Sleep(2000); // wait for 2 seconds before resuming
        }

        private void StartQuizTimer()
        {
            GlobalFlowControl.Lesson.StartingQuiz = true;
            quizTimer.Interval = 1000;
            quizTimer.AutoReset = true;
            quizTimer.Elapsed += Timer_Elapsed;
            quizTimerTick = 0;

            quizTimer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            quizTimerTick++;

            if (LessonHelper.PauseRequested) quizTimerTick--;
            Debug.WriteLine("Timeout: " + quizTimerTick + " < " + quizTime);
            
            if (quizTimerTick >= quizTime || GlobalFlowControl.Lesson.StartingQuiz == false)
            {
                GlobalFlowControl.Lesson.StartingQuiz = false;
                quizTimer.Stop();             
            }
        }

        private void StartQuiz(Quiz q)
        {
            // unselect student for asking after ending a quiz
            GlobalFlowControl.Lesson.ChosenStudent = null;
            
            var status = LessonStatusHelper.LessonStatus;
            status.CurQuiz = q;
            status.LessonId = LessonHelper.LessonId;
            status.AskQuestionNumber = q.QuestionNumber;

            LessonStatusHelper.LessonStatus = status;
            WebHelper.UpdateStatus(status);

            quizTime = int.Parse(q.TimeOut);
            
            StartQuizTimer();

            while (GlobalFlowControl.Lesson.StartingQuiz)
            {
                // Busy-waiting: wait for quiz timeout or all students already submitted the answers
            };

        }
        private void StopQuiz()
        {
            var status = LessonStatusHelper.LessonStatus;
            status.CurQuiz = null;
            status.LessonState = "quiz completed";
            
            WebHelper.UpdateStatus(status);
        }
        private void UpdateCommand(string type, string value)
        {
            if (OnCommandUpdate != null)
            {
                CommandEventArgs args = new CommandEventArgs(type, value);
                OnCommandUpdate(this, args);
            }
        }
    }
    public class CommandEventArgs : EventArgs
    {
        public string CommandType { get; private set; }
        public string CommandValue { get; private set; }

        public CommandEventArgs(string type, string value)
        {
            CommandType = type;
            CommandValue = value;
        }
    }
}
