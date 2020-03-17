using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HGPS_Robot
{
    public class Quiz
    {
        // Quiz is equivalent to Question
        
        public string QuizFormat { get; set; }
        public int QuestionNumber { get; set; } 
        public string Answer { get; set; }
        public int TimeOut { get; set; }
        public int Points { get; set; }

        public string QuestionContent { get; set; }

        // Notice: there will be a redundant semicolon at the end of the string
        // Thus, if there are 4 choices, means there are 4 semicolons
        public string Choices { get; set; } = "";// ChoiceA;ChoiceB;ChoiceC;ChoiceD;

        public Quiz() { }
    }
}
