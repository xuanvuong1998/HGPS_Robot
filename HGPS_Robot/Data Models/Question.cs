using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public string ChoiceA { get; set; }
        public string ChoiceB { get; set; }
        public string ChoiceC { get; set; }
        public string ChoiceD { get; set; }
        public string Answer { get; set; }
        public int Type { get; set; }
        public string LessonId { get; set; }
        public int TimeOut { get; set; }
        public int Points { get; set; }
        public int QuestionNumber { get; set; }
    }
}
