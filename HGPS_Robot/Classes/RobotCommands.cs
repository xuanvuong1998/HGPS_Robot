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
using HGPS_Robot.Utils;

namespace HGPS_Robot
{
    public class RobotCommands
    {
        public bool MediaPlaying { get; set; } = false;
        public RobotCommand CurrentCommand { get; private set; } = null;

        private Random rand = new Random();

        public delegate void CommandUpdateHandler(object sender, CommandEventArgs e);
        public event CommandUpdateHandler OnCommandUpdate;
        private List<RobotCommand> _commands;
        private SoundPlayer _soundPlayer = new SoundPlayer();        
        private const int QUIZ_BUFFER_SECONDS = 2;

        private Timer quizTimer = new Timer();
        private int quizTime;

        private int quizElapsed;

        public int QuizElapsedTime
        {
            get { return quizElapsed; }
            set {
                GlobalFlowControl.Lesson.QuizElapsedTime = value;
                quizElapsed = value;
            }
        }


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
            if (LessonHelper.CurrentSlideNumber == LessonHelper.LastSlideNumber)
            {
                Debug.WriteLine("LAST SLIDE: annoucing top students");
                
                StudentsPerformanceHelper.PraisingBestStudents = true;
                
                SyncHelper.InvokeRankingsResult("individual");

                while(StudentsPerformanceHelper.PraisingBestStudents == true)
                {
                    Wait(1500);
                }
                
            }

            var quiz = new Quiz();

            for (commandIteration = 0; commandIteration < _commands.Count; commandIteration++)
            {
                while (LessonHelper.PauseRequested)
                {
                    Thread.Sleep(500);
                }

                CurrentCommand = _commands[commandIteration];
                var cmd = CurrentCommand.Type;
                var val = CurrentCommand.Value;
                
                UpdateCommand(cmd.ToLower(), val);
                
                Debug.WriteLine(cmd + "/" + val); 
                
                switch (cmd.ToLower())
                {
                    case "subject":
                        LessonHelper.LessonSubject = val;
                        break;
                    case "speak":
                        Speak(val);
                        break;

                    case "speakasync":
                        SpeakAsync(val);
                        break;

                    case "playaudio":
                        
                        if (val.Contains("applause"))
                        {
                            AudioHelper.PlayApplauseSound();
                        }else if (val.Contains("sad"))
                        {
                            AudioHelper.PlaySadSound();
                        }else if (val.Contains("champion"))
                        {
                            AudioHelper.PlayChampionSound();
                        }
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
                        quiz.TimeOut = int.Parse(val);
                        break;

                    case "optiona":
                    case "optionb":
                    case "optionc":
                    case "optiond":
                        quiz.Choices += val + ";";
                        break;

                    case "points":
                        quiz.Points = int.Parse(val);
                        break;

                    case "gesture":
                        UpperBodyHelper.DoGestures(val);
                        break;

                    case "start":
                        if (val.ToLower() == "quiz" || val.ToLower() == "group-challenge")
                        {
                            LessonHelper.QuestionNumber += 1;
                            if (val.ToLower() == "group-challenge")
                            {
                                GroupChallengeHelper.InitNewChallenge();
                            }
                            quiz.QuestionNumber = LessonHelper.QuestionNumber;
                            StartQuiz(quiz); 
                            
                            // This QUIZ BUFFER TO give extra time for all student submit the answer
                            // due to delay when student received the quiz signal
                            Wait(QUIZ_BUFFER_SECONDS * 1000);

                            if (val.ToLower() == "group-challenge")
                            {
                                GroupChallengeHelper.EndChallenge();
                                GroupChallengeHelper.AssessGroupChallenge();
                            }
                                                       
                            // Prevent receiving quiz again in server received side
                            LessonStatusHelper.LessonStatus.CurQuiz = null;
                                                        
                            // Haven't received 
                            StudentsPerformanceHelper.ResultReceived = false;

                            SendStopQuizSignal(); // 

                            // Waiting for student performance results from server
                            while (StudentsPerformanceHelper.ResultReceived == false)
                            {
                                Wait(500); //Prevent looping too much
                            }

                            Debug.WriteLine("Received student perfo");

                            if (GlobalFlowControl.Lesson.GroupCompetitionIsHappening)
                            {
                                // Waiting for group (left and right) results from server
                                while (GroupCompetitionHelper.ResultReceived == false)
                                {
                                    Wait(500);
                                }
                            }
                            Debug.WriteLine("Received group perfo");
                        }
                        else if (val.ToLower() == "emotion-survey")
                        {
                            TakeEmotionSurvey();
                        }else if (val.ToLower() == "group-competition")
                        {
                            GroupCompetitionHelper.DeclareGroupMembers();

                            Debug.WriteLine("Group competition is happening now");

                            LessonStatusHelper.LessonStatus.LessonState
                                = "start-group-competition";

                            WebHelper.UpdateStatus(LessonStatusHelper.LessonStatus);

                            GlobalFlowControl.Lesson.GroupCompetitionIsHappening = true;
                        }
                        break;
                    case "stop":
                        if (val.ToLower() == "group-competition")
                        {
                            LessonStatusHelper.LessonStatus.LessonState
                                = "stop-group-competition";

                            WebHelper.UpdateStatus(LessonStatusHelper.LessonStatus);
                            GlobalFlowControl.Lesson.GroupCompetitionIsHappening = false;

                            GroupCompetitionHelper.AnnouceOverallResult();
                            
                        }
                        break;
                    case "gountil":
                        BaseHelper.GoUntilReachedGoalOrCanceled(val);
                        break;
                    case "go":
                        BaseHelper.Go(val);
                        break;

                    case "asking": // Asking student answer after the quiz ended
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
                    case "lesson":
                        if (val.ToLower() == "pause")
                        {
                            LessonHelper.SendPausedStatusToServer("paused");
                            LessonHelper.PauseLesson();
                        }else if (val.ToLower() == "continue")
                        {
                            LessonHelper.SendPausedStatusToServer("resumed");
                            LessonHelper.ResumeLesson();
                        }
                        break;
                    case "robot":
                        if (val == "pickup-std")
                        {
                            RandomAskStudentQuestion();
                            LessonHelper.SendPausedStatusToServer("asking-std");
                        }
                        break;

                    case "myhub":
                        InvokeHubMethod(val);
                        break;
                    default:
                        //MessageBox.Show($"Unknown Type: {cmd}");
                        break;
                }
            }
        }

        private void InvokeHubMethod(string val)
        {
            string method = val.Split(',')[0];

            string param = val.Split(',')[1];

            SyncHelper.RequestOpeningURL(param);
        }

        private void RandomAskStudentQuestion()
        {
            //Synthesizer.Speak("")

            TablePositionHelper.RandomStudentForAsking();

            string rdmStd = TablePositionHelper.LatestChosenStudent;

            BaseHelper.GoUntilReachedGoalOrCanceled
                (TablePositionHelper.FindTablePosByStdId(rdmStd));

            int rdmIndex = rand.Next(5);

            switch (rdmIndex)
            {
                case 0: case 3: case 4:
                    Synthesizer.Speak(rdmStd + ". " + "Can you stand up?"); break;
                case 1: case 2: Synthesizer.Speak(rdmStd + ". " + "Please stand up"); break;
            }

            Wait(2500);

            string speech = "";

            switch (rdmIndex)
            {
                case 0:
                    speech = "I have a question for you. Are you ready to answer?";
                    break;

                case 1:
                    speech = "I am going to ask you a question. Please think carefully " +
                        "and give me the answer. ";
                    break;

                case 2:
                    speech = "Ok. I want to challenge you by asking a question. " +
                        "Please listen carefully and answer. ";
                    break;
                case 3:
                    speech = $"{rdmStd}. Are you willing to answer a question from me.";
                    break;
                    
                case 4:
                    speech = $"{rdmStd}. Here is the question for you. Please try your " +
                        $"best to give me the correct answer";
                    break;
            }
            
            Synthesizer.Speak(speech);

            
        }

        private void Speak(string text)
        {
            text = SynthesizerFilter.ChangeWords(text);
            Synthesizer.Speak(text);
        }
        private void SpeakAsync(string text)
        {
            text = SynthesizerFilter.ChangeWords(text);
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
            Wait(7000); // Remove thread conflict
            
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
            QuizElapsedTime = 1;
            quizTimer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (LessonHelper.PauseRequested == false)
            {
                QuizElapsedTime++;
            }
            else
            {
                Debug.WriteLine("Lesson is pausing");
            }

            Debug.WriteLine("Time Left: " + (quizTime - QuizElapsedTime));
            
            if (QuizElapsedTime >= quizTime || GlobalFlowControl.Lesson.StartingQuiz == false)
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

            quizTime = q.TimeOut;
            LessonHelper.CurrentQuizTimeout = q.TimeOut;
            
            StartQuizTimer();

            while (GlobalFlowControl.Lesson.StartingQuiz)
            {
                Wait(500); // Prevent looping too much
                // Busy-waiting: wait for quiz timeout or all students already submitted the answers
            };

            Debug.WriteLine("Busy waiting for the quiz finished");

        }
        /// <summary>
        /// Send stop signal to server to process student results
        /// </summary>
        private void SendStopQuizSignal()
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
