using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace HGPS_Robot
{
    public class RobotProgSlide
    {
        public string Code {
            get { return _code; }
            set {
                if (value != null)
                    _code = value;
                    ProcessCommands();
            }
        }
        public List<RobotCommand> Commands { get; private set; } = null;
        public Question Question { get; private set; } = null;
        public string TeacherId { get; private set; } = null;
        public string Subject { get; private set; } = null;

        public Image Image { get; set; } = null;
        private string _code = null;
        private Question _question = new Question();

        private void ProcessCommands()
        {
            if (Code != null)
            {
                List<RobotCommand> _commands = new List<RobotCommand>();
                string[] CommandLines = Code.Split(';');
                foreach (var cmdLine in CommandLines)
                {                    
                    if (!String.IsNullOrEmpty(cmdLine.Trim()))
                    {
                        var cmdInfo = cmdLine.Trim().Split(new[] { '/' }, 2);
                        if (cmdInfo[0].ToLower() == "teacherid") TeacherId = cmdInfo[1];
                        if (cmdInfo[0].ToLower() == "subject") Subject = cmdInfo[1];

                        RobotCommand cmd = new RobotCommand(cmdInfo[0], cmdInfo[1]);
                        CheckForQuiz(cmd);
                        _commands.Add(cmd);
                    }
                }
                Commands = _commands;
            }
        }

        private void CheckForQuiz(RobotCommand cmd)
        {
            switch (cmd.Type.ToLower())
            {
                case "quizformat":
                    if (cmd.Value.ToLower() == "mcq")
                    {
                        _question.Type = 0;
                    }
                    else if (cmd.Value.ToLower() == "text")
                    {
                        _question.Type = 1;
                    }
                    else if (cmd.Value.ToLower() == "group-challenge")
                    {
                        _question.Type = 2;
                    }
                    break;

                case "question":
                    _question.Query = cmd.Value;
                    break;

                case "optiona":
                    _question.ChoiceA = cmd.Value;
                    break;

                case "optionb":
                    _question.ChoiceB = cmd.Value;
                    break;

                case "optionc":
                    _question.ChoiceC = cmd.Value;
                    break;

                case "optiond":
                    _question.ChoiceD = cmd.Value;
                    break;

                case "answer":
                    _question.Answer = cmd.Value;
                    break;

                case "timeout":
                    _question.TimeOut = Convert.ToInt32(cmd.Value);
                    break;

                case "points":
                    _question.Points = Convert.ToInt32(cmd.Value);
                    break;

                case "start": 
                    if (cmd.Value.ToLower() == "quiz" 
                        || cmd.Value.ToLower() == "group-challenge")
                            Question = _question;
                    break;
            }
        }
    }
}
