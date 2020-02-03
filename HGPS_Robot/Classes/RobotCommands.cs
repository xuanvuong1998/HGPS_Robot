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
                CurrentCommand = _commands[commandIteration];
                    
                
                
                UpdateCommand(CurrentCommand.Type.ToLower(), CurrentCommand.Value);
                
                Debug.WriteLine(CurrentCommand.Type + "/" + CurrentCommand.Value);                
                switch (CurrentCommand.Type.ToLower())
                {
                    case "speak":
                        Speak(CurrentCommand.Value);
                        break;

                    case "speakasync":
                        SpeakAsync(CurrentCommand.Value);
                        break;

                    case "playaudio":
                        AudioHelper.PlayAudio(CurrentCommand.Value);
                        break;
                    case "myspeech":
                        MySpeech(CurrentCommand.Value);
                        break;

                    case "myspeechasync":
                        MySpeechAsync(CurrentCommand.Value);
                        break;

                    case "move":
                        Move(CurrentCommand.Value);
                        break;

                    case "wait":
                        Wait(Convert.ToInt32(CurrentCommand.Value));
                        break;

                    case "playmedia":
                        PlayMedia(CurrentCommand.Value);
                        break;

                    case "quizformat":
                        quiz.QuizFormat = CurrentCommand.Value;
                        break;

                    case "answer":
                        quiz.Answer = CurrentCommand.Value;
                        break;

                    case "timeout":
                        quiz.TimeOut = CurrentCommand.Value;
                        break;

                    case "gesture":
                        UpperBodyHelper.DoGestures(CurrentCommand.Value);
                        break;

                    case "start":
                        if (CurrentCommand.Value.ToLower() == "quiz")
                        {
                            LessonHelper.QuestionNumber += 1;
                            quiz.QuestionNumber = LessonHelper.QuestionNumber;
                            StartQuiz(quiz);
                            Wait(Convert.ToInt32(quiz.TimeOut) * 1000 + QUIZ_BUFFER_SECONDS * 1000);
                            // This QUIZ BUFFER TO give extra time for all student submit the answer

                            Debug.WriteLine("Stopping quiz");
                            StopQuiz();
                           
                            Wait(QUIZ_BUFFER_SECONDS * 1000);

                            Debug.WriteLine("QUIZ COMPLETED");

                            // This QUIZ BUFFER TO give extra time for teacher send student result to robot
                        }
                        break;
                    case "gountil":
                        BaseHelper.GoUntilReachedGoalOrCanceled(CurrentCommand.Value);
                        break;

                    case "asking":
                        var status = LessonStatusHelper.LessonStatus;
                        if (Convert.ToInt32(CurrentCommand.Value) == 1)
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
                        //MessageBox.Show($"Unknown Type: {CurrentCommand.Type}");
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
        private void PlayMedia(string url)
        {
            MediaPlaying = true;
            var status = LessonStatusHelper.LessonStatus;
            status.MediaPath = url;
            WebHelper.UpdateStatus(status);

            while (MediaPlaying) ; //wait for media to finish playing
            Thread.Sleep(2000); // wait for 2 seconds before resuming
        }
        private void StartQuiz(Quiz q)
        {
            var status = LessonStatusHelper.LessonStatus;
            status.CurQuiz = q;
            status.LessonId = LessonHelper.LessonId;
            WebHelper.UpdateStatus(status);
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
